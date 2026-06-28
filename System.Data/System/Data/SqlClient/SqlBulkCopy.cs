using System;
using System.Collections;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopy : IDisposable
	{
		public SqlBulkCopy(SqlConnection connection)
		{
			this.connection = connection;
		}

		public SqlBulkCopy(string connectionString)
		{
			this.connection = new SqlConnection(connectionString);
			this.isLocalConnection = true;
		}

		[MonoTODO]
		public SqlBulkCopy(string connectionString, SqlBulkCopyOptions copyOptions)
		{
			this.connection = new SqlConnection(connectionString);
			this.copyOptions = copyOptions;
			this.isLocalConnection = true;
			throw new NotImplementedException();
		}

		[MonoTODO]
		public SqlBulkCopy(SqlConnection connection, SqlBulkCopyOptions copyOptions, SqlTransaction externalTransaction)
		{
			this.connection = connection;
			this.copyOptions = copyOptions;
			throw new NotImplementedException();
		}

		public event SqlRowsCopiedEventHandler SqlRowsCopied;

		void IDisposable.Dispose()
		{
			if (this.isLocalConnection)
			{
				this.Close();
				this.connection = null;
			}
		}

		public int BatchSize
		{
			get
			{
				return this._batchSize;
			}
			set
			{
				this._batchSize = value;
			}
		}

		public int BulkCopyTimeout
		{
			get
			{
				return this._bulkCopyTimeout;
			}
			set
			{
				this._bulkCopyTimeout = value;
			}
		}

		public SqlBulkCopyColumnMappingCollection ColumnMappings
		{
			get
			{
				return this._columnMappingCollection;
			}
		}

		public string DestinationTableName
		{
			get
			{
				return this._destinationTableName;
			}
			set
			{
				this._destinationTableName = value;
			}
		}

		public int NotifyAfter
		{
			get
			{
				return this._notifyAfter;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("NotifyAfter should be greater than or equal to 0");
				}
				this._notifyAfter = value;
			}
		}

		public void Close()
		{
			if (this.sqlRowsCopied)
			{
				throw new InvalidOperationException("Close should not be called from SqlRowsCopied event");
			}
			if (this.connection == null || this.connection.State == ConnectionState.Closed)
			{
				return;
			}
			this.connection.Close();
		}

		private DataTable[] GetColumnMetaData()
		{
			DataTable[] array = new DataTable[2];
			SqlCommand sqlCommand = new SqlCommand(string.Concat(new string[] { "select @@trancount; set fmtonly on select * from ", this.DestinationTableName, " set fmtonly off;exec sp_tablecollations_90 '", this.DestinationTableName, "'" }), this.connection);
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			int num = 0;
			do
			{
				if (num == 1)
				{
					array[num - 1] = sqlDataReader.GetSchemaTable();
				}
				else if (num == 2)
				{
					SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
					sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
					array[num - 1] = new DataTable();
					sqlDataAdapter.FillInternal(array[num - 1], sqlDataReader);
				}
				num++;
			}
			while (!sqlDataReader.IsClosed && sqlDataReader.NextResult());
			sqlDataReader.Close();
			return array;
		}

		private string GenerateColumnMetaData(SqlCommand tmpCmd, DataTable colMetaData, DataTable tableCollations)
		{
			bool flag = false;
			string text = string.Empty;
			int num = 0;
			foreach (object obj in colMetaData.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				flag = false;
				using (IEnumerator enumerator2 = colMetaData.Columns.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						DataColumn dataColumn = (DataColumn)enumerator2.Current;
						object obj2 = null;
						if (this._columnMappingCollection.Count > 0)
						{
							if (this.ordinalMapping)
							{
								foreach (object obj3 in this._columnMappingCollection)
								{
									SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = (SqlBulkCopyColumnMapping)obj3;
									if (sqlBulkCopyColumnMapping.DestinationOrdinal == num)
									{
										flag = true;
										break;
									}
								}
							}
							else
							{
								foreach (object obj4 in this._columnMappingCollection)
								{
									SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping2 = (SqlBulkCopyColumnMapping)obj4;
									if (sqlBulkCopyColumnMapping2.DestinationColumn == (string)dataRow["ColumnName"])
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								goto IL_01F2;
							}
						}
						if ((bool)dataRow["IsReadOnly"])
						{
							if (!this.ordinalMapping)
							{
								goto IL_01F2;
							}
							obj2 = false;
						}
						SqlParameter sqlParameter = new SqlParameter((string)dataRow["ColumnName"], (SqlDbType)((int)dataRow["ProviderType"]));
						sqlParameter.Value = obj2;
						if ((int)dataRow["ColumnSize"] != -1)
						{
							sqlParameter.Size = (int)dataRow["ColumnSize"];
						}
						tmpCmd.Parameters.Add(sqlParameter);
					}
					IL_01F2:;
				}
				num++;
			}
			flag = false;
			bool flag2 = false;
			foreach (object obj5 in colMetaData.Rows)
			{
				DataRow dataRow2 = (DataRow)obj5;
				if (this._columnMappingCollection.Count > 0)
				{
					num = 0;
					flag2 = false;
					foreach (object obj6 in tmpCmd.Parameters)
					{
						SqlParameter sqlParameter2 = (SqlParameter)obj6;
						if (this.ordinalMapping)
						{
							foreach (object obj7 in this._columnMappingCollection)
							{
								SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping3 = (SqlBulkCopyColumnMapping)obj7;
								if (sqlBulkCopyColumnMapping3.DestinationOrdinal == num && sqlParameter2.Value == null)
								{
									flag2 = true;
								}
							}
						}
						else
						{
							foreach (object obj8 in this._columnMappingCollection)
							{
								SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping4 = (SqlBulkCopyColumnMapping)obj8;
								if (sqlBulkCopyColumnMapping4.DestinationColumn == sqlParameter2.ParameterName && (string)dataRow2["ColumnName"] == sqlParameter2.ParameterName)
								{
									flag2 = true;
									sqlParameter2.Value = null;
								}
							}
						}
						num++;
						if (flag2)
						{
							break;
						}
					}
					if (!flag2)
					{
						continue;
					}
				}
				if (!(bool)dataRow2["IsReadOnly"])
				{
					string text2 = string.Empty;
					if ((int)dataRow2["ColumnSize"] != -1)
					{
						text2 = string.Format("{0}({1})", (SqlDbType)((int)dataRow2["ProviderType"]), dataRow2["ColumnSize"]);
					}
					else
					{
						text2 = string.Format("{0}", (SqlDbType)((int)dataRow2["ProviderType"]));
					}
					if (flag)
					{
						text += ", ";
					}
					string text3 = (string)dataRow2["ColumnName"];
					text += string.Format("[{0}] {1}", text3, text2);
					if (!flag)
					{
						flag = true;
					}
					if (tableCollations != null)
					{
						foreach (object obj9 in tableCollations.Rows)
						{
							DataRow dataRow3 = (DataRow)obj9;
							if ((string)dataRow3["name"] == text3)
							{
								text += string.Format(" COLLATE {0}", dataRow3["collation"]);
								break;
							}
						}
					}
				}
			}
			return text;
		}

		private void ValidateColumnMapping(DataTable table, DataTable tableCollations)
		{
			foreach (object obj in this._columnMappingCollection)
			{
				SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = (SqlBulkCopyColumnMapping)obj;
				if (!this.ordinalMapping && (sqlBulkCopyColumnMapping.DestinationColumn == string.Empty || sqlBulkCopyColumnMapping.SourceColumn == string.Empty))
				{
					throw new InvalidOperationException("Mappings must be either all null or ordinal");
				}
				if (this.ordinalMapping && (sqlBulkCopyColumnMapping.DestinationOrdinal == -1 || sqlBulkCopyColumnMapping.SourceOrdinal == -1))
				{
					throw new InvalidOperationException("Mappings must be either all null or ordinal");
				}
				bool flag = false;
				if (!this.ordinalMapping)
				{
					foreach (object obj2 in tableCollations.Rows)
					{
						DataRow dataRow = (DataRow)obj2;
						if ((string)dataRow["name"] == sqlBulkCopyColumnMapping.DestinationColumn)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new InvalidOperationException("ColumnMapping does not match");
					}
					flag = false;
					foreach (object obj3 in table.Columns)
					{
						DataColumn dataColumn = (DataColumn)obj3;
						if (dataColumn.ColumnName == sqlBulkCopyColumnMapping.SourceColumn)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new InvalidOperationException("ColumnName " + sqlBulkCopyColumnMapping.SourceColumn + " does not match");
					}
				}
				else if (sqlBulkCopyColumnMapping.DestinationOrdinal >= tableCollations.Rows.Count)
				{
					throw new InvalidOperationException("ColumnMapping does not match");
				}
			}
		}

		private void BulkCopyToServer(DataTable table, DataRowState state)
		{
			if (this.connection == null || this.connection.State == ConnectionState.Closed)
			{
				throw new InvalidOperationException("This method should not be called on a closed connection");
			}
			if (this._destinationTableName == null)
			{
				throw new ArgumentNullException("DestinationTableName");
			}
			if (this.identityInsert)
			{
				SqlCommand sqlCommand = new SqlCommand("set identity_insert " + table.TableName + " on", this.connection);
				sqlCommand.ExecuteScalar();
			}
			DataTable[] columnMetaData = this.GetColumnMetaData();
			DataTable dataTable = columnMetaData[0];
			DataTable dataTable2 = columnMetaData[1];
			if (this._columnMappingCollection.Count > 0)
			{
				if (this._columnMappingCollection[0].SourceOrdinal != -1)
				{
					this.ordinalMapping = true;
				}
				this.ValidateColumnMapping(table, dataTable2);
			}
			SqlCommand sqlCommand2 = new SqlCommand();
			TdsBulkCopy tdsBulkCopy = new TdsBulkCopy(this.connection.Tds);
			if (this.connection.Tds.TdsVersion >= TdsVersion.tds70)
			{
				string text = "insert bulk " + this.DestinationTableName + " (";
				text += this.GenerateColumnMetaData(sqlCommand2, dataTable, dataTable2);
				text += ")";
				tdsBulkCopy.SendColumnMetaData(text);
			}
			tdsBulkCopy.BulkCopyStart(sqlCommand2.Parameters.MetaParameters);
			long num = 0L;
			foreach (object obj in table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow.RowState != DataRowState.Deleted)
				{
					if (state == (DataRowState)0 || dataRow.RowState == state)
					{
						bool flag = true;
						int num2 = 0;
						foreach (object obj2 in sqlCommand2.Parameters)
						{
							SqlParameter sqlParameter = (SqlParameter)obj2;
							int num3 = 0;
							object obj3 = null;
							if (this._columnMappingCollection.Count > 0)
							{
								if (this.ordinalMapping)
								{
									foreach (object obj4 in this._columnMappingCollection)
									{
										SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = (SqlBulkCopyColumnMapping)obj4;
										if (sqlBulkCopyColumnMapping.DestinationOrdinal == num2 && sqlParameter.Value == null)
										{
											obj3 = dataRow[sqlBulkCopyColumnMapping.SourceOrdinal];
											SqlParameter sqlParameter2 = new SqlParameter(sqlBulkCopyColumnMapping.SourceOrdinal.ToString(), obj3);
											if (sqlParameter.MetaParameter.TypeName != sqlParameter2.MetaParameter.TypeName)
											{
												sqlParameter2.SqlDbType = sqlParameter.SqlDbType;
												object obj5 = sqlParameter2.ConvertToFrameworkType(obj3);
												sqlParameter2.Value = obj5;
												obj3 = obj5;
											}
											string text2 = string.Format("{0}", sqlParameter2.MetaParameter.TypeName);
											if (text2 == "nvarchar")
											{
												if (dataRow[num2] != null)
												{
													num3 = ((string)sqlParameter2.Value).Length;
													num3 <<= 1;
												}
											}
											else
											{
												num3 = sqlParameter2.Size;
											}
											break;
										}
									}
								}
								else
								{
									foreach (object obj6 in this._columnMappingCollection)
									{
										SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping2 = (SqlBulkCopyColumnMapping)obj6;
										if (sqlBulkCopyColumnMapping2.DestinationColumn == sqlParameter.ParameterName)
										{
											obj3 = dataRow[sqlBulkCopyColumnMapping2.SourceColumn];
											SqlParameter sqlParameter3 = new SqlParameter(sqlBulkCopyColumnMapping2.SourceColumn, obj3);
											if (sqlParameter.MetaParameter.TypeName != sqlParameter3.MetaParameter.TypeName)
											{
												sqlParameter3.SqlDbType = sqlParameter.SqlDbType;
												object obj5 = sqlParameter3.ConvertToFrameworkType(obj3);
												sqlParameter3.Value = obj5;
												obj3 = obj5;
											}
											string text3 = string.Format("{0}", sqlParameter3.MetaParameter.TypeName);
											if (text3 == "nvarchar")
											{
												if (dataRow[sqlBulkCopyColumnMapping2.SourceColumn] != null)
												{
													num3 = ((string)obj3).Length;
													num3 <<= 1;
												}
											}
											else
											{
												num3 = sqlParameter3.Size;
											}
											break;
										}
									}
								}
								num2++;
							}
							else
							{
								obj3 = dataRow[sqlParameter.ParameterName];
								string typeName = sqlParameter.MetaParameter.TypeName;
								if (typeName == "nvarchar")
								{
									num3 = ((string)dataRow[sqlParameter.ParameterName]).Length;
									num3 <<= 1;
								}
								else
								{
									num3 = sqlParameter.Size;
								}
							}
							if (obj3 != null)
							{
								tdsBulkCopy.BulkCopyData(obj3, num3, flag);
								if (flag)
								{
									flag = false;
								}
							}
						}
						if (this._notifyAfter > 0)
						{
							num += 1L;
							if (num >= (long)this._notifyAfter)
							{
								this.RowsCopied(num);
								num = 0L;
							}
						}
					}
				}
			}
			tdsBulkCopy.BulkCopyEnd();
		}

		public void WriteToServer(DataRow[] rows)
		{
			if (rows == null)
			{
				throw new ArgumentNullException("rows");
			}
			DataTable dataTable = new DataTable(rows[0].Table.TableName);
			foreach (object obj in rows[0].Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				DataColumn dataColumn2 = new DataColumn(dataColumn.ColumnName, dataColumn.DataType);
				dataTable.Columns.Add(dataColumn2);
			}
			foreach (DataRow dataRow in rows)
			{
				DataRow dataRow2 = dataTable.NewRow();
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					dataRow2[j] = dataRow[j];
				}
				dataTable.Rows.Add(dataRow2);
			}
			this.BulkCopyToServer(dataTable, (DataRowState)0);
		}

		public void WriteToServer(DataTable table)
		{
			this.BulkCopyToServer(table, (DataRowState)0);
		}

		public void WriteToServer(IDataReader reader)
		{
			DataTable dataTable = new DataTable();
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
			sqlDataAdapter.FillInternal(dataTable, reader);
			this.BulkCopyToServer(dataTable, (DataRowState)0);
		}

		public void WriteToServer(DataTable table, DataRowState rowState)
		{
			this.BulkCopyToServer(table, rowState);
		}

		private void RowsCopied(long rowsCopied)
		{
			SqlRowsCopiedEventArgs e = new SqlRowsCopiedEventArgs(rowsCopied);
			if (this.SqlRowsCopied != null)
			{
				this.SqlRowsCopied(this, e);
			}
		}

		private int _batchSize;

		private int _notifyAfter;

		private int _bulkCopyTimeout;

		private SqlBulkCopyColumnMappingCollection _columnMappingCollection = new SqlBulkCopyColumnMappingCollection();

		private string _destinationTableName;

		private bool ordinalMapping;

		private bool sqlRowsCopied;

		private bool identityInsert;

		private bool isLocalConnection;

		private SqlConnection connection;

		private SqlBulkCopyOptions copyOptions;
	}
}
