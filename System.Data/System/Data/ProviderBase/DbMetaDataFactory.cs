using System;
using System.Data.Common;
using System.Globalization;
using System.IO;

namespace System.Data.ProviderBase
{
	internal class DbMetaDataFactory
	{
		public DbMetaDataFactory(Stream xmlStream, string serverVersion, string normalizedServerVersion)
		{
			ADP.CheckArgumentNull(xmlStream, "xmlStream");
			ADP.CheckArgumentNull(serverVersion, "serverVersion");
			ADP.CheckArgumentNull(normalizedServerVersion, "normalizedServerVersion");
			this.LoadDataSetFromXml(xmlStream);
			this._serverVersionString = serverVersion;
			this._normalizedServerVersion = normalizedServerVersion;
		}

		protected DataSet CollectionDataSet
		{
			get
			{
				return this._metaDataCollectionsDataSet;
			}
		}

		protected string ServerVersion
		{
			get
			{
				return this._serverVersionString;
			}
		}

		protected string ServerVersionNormalized
		{
			get
			{
				return this._normalizedServerVersion;
			}
		}

		protected DataTable CloneAndFilterCollection(string collectionName, string[] hiddenColumnNames)
		{
			DataTable dataTable = this._metaDataCollectionsDataSet.Tables[collectionName];
			if (dataTable == null || collectionName != dataTable.TableName)
			{
				throw ADP.DataTableDoesNotExist(collectionName);
			}
			DataTable dataTable2 = new DataTable(collectionName)
			{
				Locale = CultureInfo.InvariantCulture
			};
			DataColumnCollection columns = dataTable2.Columns;
			DataColumn[] array = this.FilterColumns(dataTable, hiddenColumnNames, columns);
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (this.SupportedByCurrentVersion(dataRow))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					for (int i = 0; i < columns.Count; i++)
					{
						dataRow2[columns[i]] = dataRow[array[i], DataRowVersion.Current];
					}
					dataTable2.Rows.Add(dataRow2);
					dataRow2.AcceptChanges();
				}
			}
			return dataTable2;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._normalizedServerVersion = null;
				this._serverVersionString = null;
				this._metaDataCollectionsDataSet.Dispose();
			}
		}

		private DataTable ExecuteCommand(DataRow requestedCollectionRow, string[] restrictions, DbConnection connection)
		{
			DataTable dataTable = this._metaDataCollectionsDataSet.Tables[DbMetaDataCollectionNames.MetaDataCollections];
			DataColumn dataColumn = dataTable.Columns["PopulationString"];
			DataColumn dataColumn2 = dataTable.Columns["NumberOfRestrictions"];
			DataColumn dataColumn3 = dataTable.Columns["CollectionName"];
			DataTable dataTable2 = null;
			string text = requestedCollectionRow[dataColumn, DataRowVersion.Current] as string;
			int num = (int)requestedCollectionRow[dataColumn2, DataRowVersion.Current];
			string text2 = requestedCollectionRow[dataColumn3, DataRowVersion.Current] as string;
			if (restrictions != null && restrictions.Length > num)
			{
				throw ADP.TooManyRestrictions(text2);
			}
			DbCommand dbCommand = connection.CreateCommand();
			dbCommand.CommandText = text;
			dbCommand.CommandTimeout = Math.Max(dbCommand.CommandTimeout, 180);
			for (int i = 0; i < num; i++)
			{
				DbParameter dbParameter = dbCommand.CreateParameter();
				if (restrictions != null && restrictions.Length > i && restrictions[i] != null)
				{
					dbParameter.Value = restrictions[i];
				}
				else
				{
					dbParameter.Value = DBNull.Value;
				}
				dbParameter.ParameterName = this.GetParameterName(text2, i + 1);
				dbParameter.Direction = ParameterDirection.Input;
				dbCommand.Parameters.Add(dbParameter);
			}
			DbDataReader dbDataReader = null;
			try
			{
				try
				{
					dbDataReader = dbCommand.ExecuteReader();
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableExceptionType(ex))
					{
						throw;
					}
					throw ADP.QueryFailed(text2, ex);
				}
				dataTable2 = new DataTable(text2)
				{
					Locale = CultureInfo.InvariantCulture
				};
				foreach (object obj in dbDataReader.GetSchemaTable().Rows)
				{
					DataRow dataRow = (DataRow)obj;
					dataTable2.Columns.Add(dataRow["ColumnName"] as string, (Type)dataRow["DataType"]);
				}
				object[] array = new object[dataTable2.Columns.Count];
				while (dbDataReader.Read())
				{
					dbDataReader.GetValues(array);
					dataTable2.Rows.Add(array);
				}
			}
			finally
			{
				if (dbDataReader != null)
				{
					dbDataReader.Dispose();
					dbDataReader = null;
				}
			}
			return dataTable2;
		}

		private DataColumn[] FilterColumns(DataTable sourceTable, string[] hiddenColumnNames, DataColumnCollection destinationColumns)
		{
			DataColumn[] array = null;
			int num = 0;
			foreach (object obj in sourceTable.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (this.IncludeThisColumn(dataColumn, hiddenColumnNames))
				{
					num++;
				}
			}
			if (num == 0)
			{
				throw ADP.NoColumns();
			}
			int num2 = 0;
			array = new DataColumn[num];
			foreach (object obj2 in sourceTable.Columns)
			{
				DataColumn dataColumn2 = (DataColumn)obj2;
				if (this.IncludeThisColumn(dataColumn2, hiddenColumnNames))
				{
					DataColumn dataColumn3 = new DataColumn(dataColumn2.ColumnName, dataColumn2.DataType);
					destinationColumns.Add(dataColumn3);
					array[num2] = dataColumn2;
					num2++;
				}
			}
			return array;
		}

		internal DataRow FindMetaDataCollectionRow(string collectionName)
		{
			DataTable dataTable = this._metaDataCollectionsDataSet.Tables[DbMetaDataCollectionNames.MetaDataCollections];
			if (dataTable == null)
			{
				throw ADP.InvalidXml();
			}
			DataColumn dataColumn = dataTable.Columns[DbMetaDataColumnNames.CollectionName];
			if (dataColumn == null || typeof(string) != dataColumn.DataType)
			{
				throw ADP.InvalidXmlMissingColumn(DbMetaDataCollectionNames.MetaDataCollections, DbMetaDataColumnNames.CollectionName);
			}
			DataRow dataRow = null;
			string text = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow2 = (DataRow)obj;
				string text2 = dataRow2[dataColumn, DataRowVersion.Current] as string;
				if (string.IsNullOrEmpty(text2))
				{
					throw ADP.InvalidXmlInvalidValue(DbMetaDataCollectionNames.MetaDataCollections, DbMetaDataColumnNames.CollectionName);
				}
				if (ADP.CompareInsensitiveInvariant(text2, collectionName))
				{
					if (!this.SupportedByCurrentVersion(dataRow2))
					{
						flag = true;
					}
					else if (collectionName == text2)
					{
						if (flag2)
						{
							throw ADP.CollectionNameIsNotUnique(collectionName);
						}
						dataRow = dataRow2;
						text = text2;
						flag2 = true;
					}
					else
					{
						if (text != null)
						{
							flag3 = true;
						}
						dataRow = dataRow2;
						text = text2;
					}
				}
			}
			if (dataRow == null)
			{
				if (!flag)
				{
					throw ADP.UndefinedCollection(collectionName);
				}
				throw ADP.UnsupportedVersion(collectionName);
			}
			else
			{
				if (!flag2 && flag3)
				{
					throw ADP.AmbigousCollectionName(collectionName);
				}
				return dataRow;
			}
		}

		private void FixUpVersion(DataTable dataSourceInfoTable)
		{
			DataColumn dataColumn = dataSourceInfoTable.Columns["DataSourceProductVersion"];
			DataColumn dataColumn2 = dataSourceInfoTable.Columns["DataSourceProductVersionNormalized"];
			if (dataColumn == null || dataColumn2 == null)
			{
				throw ADP.MissingDataSourceInformationColumn();
			}
			if (dataSourceInfoTable.Rows.Count != 1)
			{
				throw ADP.IncorrectNumberOfDataSourceInformationRows();
			}
			DataRow dataRow = dataSourceInfoTable.Rows[0];
			dataRow[dataColumn] = this._serverVersionString;
			dataRow[dataColumn2] = this._normalizedServerVersion;
			dataRow.AcceptChanges();
		}

		private string GetParameterName(string neededCollectionName, int neededRestrictionNumber)
		{
			DataColumn dataColumn = null;
			DataColumn dataColumn2 = null;
			DataColumn dataColumn3 = null;
			DataColumn dataColumn4 = null;
			string text = null;
			DataTable dataTable = this._metaDataCollectionsDataSet.Tables[DbMetaDataCollectionNames.Restrictions];
			if (dataTable != null)
			{
				DataColumnCollection columns = dataTable.Columns;
				if (columns != null)
				{
					dataColumn = columns["CollectionName"];
					dataColumn2 = columns["ParameterName"];
					dataColumn3 = columns["RestrictionName"];
					dataColumn4 = columns["RestrictionNumber"];
				}
			}
			if (dataColumn2 == null || dataColumn == null || dataColumn3 == null || dataColumn4 == null)
			{
				throw ADP.MissingRestrictionColumn();
			}
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if ((string)dataRow[dataColumn] == neededCollectionName && (int)dataRow[dataColumn4] == neededRestrictionNumber && this.SupportedByCurrentVersion(dataRow))
				{
					text = (string)dataRow[dataColumn2];
					break;
				}
			}
			if (text == null)
			{
				throw ADP.MissingRestrictionRow();
			}
			return text;
		}

		public virtual DataTable GetSchema(DbConnection connection, string collectionName, string[] restrictions)
		{
			DataTable dataTable = this._metaDataCollectionsDataSet.Tables[DbMetaDataCollectionNames.MetaDataCollections];
			DataColumn dataColumn = dataTable.Columns["PopulationMechanism"];
			DataColumn dataColumn2 = dataTable.Columns[DbMetaDataColumnNames.CollectionName];
			DataRow dataRow = this.FindMetaDataCollectionRow(collectionName);
			string text = dataRow[dataColumn2, DataRowVersion.Current] as string;
			if (!ADP.IsEmptyArray(restrictions))
			{
				for (int i = 0; i < restrictions.Length; i++)
				{
					if (restrictions[i] != null && restrictions[i].Length > 4096)
					{
						throw ADP.NotSupported();
					}
				}
			}
			string text2 = dataRow[dataColumn, DataRowVersion.Current] as string;
			DataTable dataTable2;
			if (!(text2 == "DataTable"))
			{
				if (!(text2 == "SQLCommand"))
				{
					if (!(text2 == "PrepareCollection"))
					{
						throw ADP.UndefinedPopulationMechanism(text2);
					}
					dataTable2 = this.PrepareCollection(text, restrictions, connection);
				}
				else
				{
					dataTable2 = this.ExecuteCommand(dataRow, restrictions, connection);
				}
			}
			else
			{
				string[] array;
				if (text == DbMetaDataCollectionNames.MetaDataCollections)
				{
					array = new string[] { "PopulationMechanism", "PopulationString" };
				}
				else
				{
					array = null;
				}
				if (!ADP.IsEmptyArray(restrictions))
				{
					throw ADP.TooManyRestrictions(text);
				}
				dataTable2 = this.CloneAndFilterCollection(text, array);
				if (text == DbMetaDataCollectionNames.DataSourceInformation)
				{
					this.FixUpVersion(dataTable2);
				}
			}
			return dataTable2;
		}

		private bool IncludeThisColumn(DataColumn sourceColumn, string[] hiddenColumnNames)
		{
			bool flag = true;
			string columnName = sourceColumn.ColumnName;
			if (columnName == "MinimumVersion" || columnName == "MaximumVersion")
			{
				flag = false;
			}
			else if (hiddenColumnNames != null)
			{
				for (int i = 0; i < hiddenColumnNames.Length; i++)
				{
					if (hiddenColumnNames[i] == columnName)
					{
						flag = false;
						break;
					}
				}
			}
			return flag;
		}

		private void LoadDataSetFromXml(Stream XmlStream)
		{
			this._metaDataCollectionsDataSet = new DataSet();
			this._metaDataCollectionsDataSet.Locale = CultureInfo.InvariantCulture;
			this._metaDataCollectionsDataSet.ReadXml(XmlStream);
		}

		protected virtual DataTable PrepareCollection(string collectionName, string[] restrictions, DbConnection connection)
		{
			throw ADP.NotSupported();
		}

		private bool SupportedByCurrentVersion(DataRow requestedCollectionRow)
		{
			bool flag = true;
			DataColumnCollection columns = requestedCollectionRow.Table.Columns;
			DataColumn dataColumn = columns["MinimumVersion"];
			if (dataColumn != null)
			{
				object obj = requestedCollectionRow[dataColumn];
				if (obj != null && obj != DBNull.Value && 0 > string.Compare(this._normalizedServerVersion, (string)obj, StringComparison.OrdinalIgnoreCase))
				{
					flag = false;
				}
			}
			if (flag)
			{
				dataColumn = columns["MaximumVersion"];
				if (dataColumn != null)
				{
					object obj = requestedCollectionRow[dataColumn];
					if (obj != null && obj != DBNull.Value && 0 < string.Compare(this._normalizedServerVersion, (string)obj, StringComparison.OrdinalIgnoreCase))
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		private DataSet _metaDataCollectionsDataSet;

		private string _normalizedServerVersion;

		private string _serverVersionString;

		private const string _collectionName = "CollectionName";

		private const string _populationMechanism = "PopulationMechanism";

		private const string _populationString = "PopulationString";

		private const string _maximumVersion = "MaximumVersion";

		private const string _minimumVersion = "MinimumVersion";

		private const string _dataSourceProductVersionNormalized = "DataSourceProductVersionNormalized";

		private const string _dataSourceProductVersion = "DataSourceProductVersion";

		private const string _restrictionDefault = "RestrictionDefault";

		private const string _restrictionNumber = "RestrictionNumber";

		private const string _numberOfRestrictions = "NumberOfRestrictions";

		private const string _restrictionName = "RestrictionName";

		private const string _parameterName = "ParameterName";

		private const string _dataTable = "DataTable";

		private const string _sqlCommand = "SQLCommand";

		private const string _prepareCollection = "PrepareCollection";
	}
}
