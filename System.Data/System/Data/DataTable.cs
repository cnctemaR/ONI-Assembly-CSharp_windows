using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data
{
	[XmlSchemaProvider("GetDataTableSchema")]
	[DefaultEvent("RowChanging")]
	[DefaultProperty("TableName")]
	[ToolboxItem(false)]
	[DesignTimeVisible(false)]
	[Serializable]
	public class DataTable : MarshalByValueComponent, IListSource, ISupportInitializeNotification, ISupportInitialize, ISerializable, IXmlSerializable
	{
		public DataTable()
		{
			GC.SuppressFinalize(this);
			DataCommonEventSource.Log.Trace<int>("<ds.DataTable.DataTable|API> {0}", this.ObjectID);
			this._nextRowID = 1L;
			this._recordManager = new RecordManager(this);
			this._culture = CultureInfo.CurrentCulture;
			this._columnCollection = new DataColumnCollection(this);
			this._constraintCollection = new ConstraintCollection(this);
			this._rowCollection = new DataRowCollection(this);
			this._indexes = new List<Index>();
			this._rowBuilder = new DataRowBuilder(this, -1);
		}

		public DataTable(string tableName)
			: this()
		{
			this._tableName = ((tableName == null) ? "" : tableName);
		}

		public DataTable(string tableName, string tableNamespace)
			: this(tableName)
		{
			this.Namespace = tableNamespace;
		}

		protected DataTable(SerializationInfo info, StreamingContext context)
			: this()
		{
			bool flag = context.Context == null || Convert.ToBoolean(context.Context, CultureInfo.InvariantCulture);
			SerializationFormat serializationFormat = SerializationFormat.Xml;
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Name == "DataTable.RemotingFormat")
				{
					serializationFormat = (SerializationFormat)enumerator.Value;
				}
			}
			this.DeserializeDataTable(info, context, flag, serializationFormat);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			SerializationFormat remotingFormat = this.RemotingFormat;
			bool flag = context.Context == null || Convert.ToBoolean(context.Context, CultureInfo.InvariantCulture);
			this.SerializeDataTable(info, context, flag, remotingFormat);
		}

		private void SerializeDataTable(SerializationInfo info, StreamingContext context, bool isSingleTable, SerializationFormat remotingFormat)
		{
			info.AddValue("DataTable.RemotingVersion", new Version(2, 0));
			if (remotingFormat != SerializationFormat.Xml)
			{
				info.AddValue("DataTable.RemotingFormat", remotingFormat);
			}
			if (remotingFormat != SerializationFormat.Xml)
			{
				this.SerializeTableSchema(info, context, isSingleTable);
				if (isSingleTable)
				{
					this.SerializeTableData(info, context, 0);
					return;
				}
			}
			else
			{
				string text = string.Empty;
				bool flag = false;
				if (this._dataSet == null)
				{
					DataSet dataSet = new DataSet("tmpDataSet");
					dataSet.SetLocaleValue(this._culture, this._cultureUserSet);
					dataSet.CaseSensitive = this.CaseSensitive;
					dataSet._namespaceURI = this.Namespace;
					dataSet.Tables.Add(this);
					flag = true;
				}
				else
				{
					text = this.DataSet.Namespace;
					this.DataSet._namespaceURI = this.Namespace;
				}
				info.AddValue("XmlSchema", this._dataSet.GetXmlSchemaForRemoting(this));
				info.AddValue("XmlDiffGram", this._dataSet.GetRemotingDiffGram(this));
				if (flag)
				{
					this._dataSet.Tables.Remove(this);
					return;
				}
				this._dataSet._namespaceURI = text;
			}
		}

		internal void DeserializeDataTable(SerializationInfo info, StreamingContext context, bool isSingleTable, SerializationFormat remotingFormat)
		{
			if (remotingFormat != SerializationFormat.Xml)
			{
				this.DeserializeTableSchema(info, context, isSingleTable);
				if (isSingleTable)
				{
					this.DeserializeTableData(info, context, 0);
					this.ResetIndexes();
					return;
				}
			}
			else
			{
				string text = (string)info.GetValue("XmlSchema", typeof(string));
				string text2 = (string)info.GetValue("XmlDiffGram", typeof(string));
				if (text != null)
				{
					DataSet dataSet = new DataSet();
					dataSet.ReadXmlSchema(new XmlTextReader(new StringReader(text)));
					DataTable dataTable = dataSet.Tables[0];
					dataTable.CloneTo(this, null, false);
					this.Namespace = dataTable.Namespace;
					if (text2 != null)
					{
						dataSet.Tables.Remove(dataSet.Tables[0]);
						dataSet.Tables.Add(this);
						dataSet.ReadXml(new XmlTextReader(new StringReader(text2)), XmlReadMode.DiffGram);
						dataSet.Tables.Remove(this);
					}
				}
			}
		}

		internal void SerializeTableSchema(SerializationInfo info, StreamingContext context, bool isSingleTable)
		{
			info.AddValue("DataTable.TableName", this.TableName);
			info.AddValue("DataTable.Namespace", this.Namespace);
			info.AddValue("DataTable.Prefix", this.Prefix);
			info.AddValue("DataTable.CaseSensitive", this._caseSensitive);
			info.AddValue("DataTable.caseSensitiveAmbient", !this._caseSensitiveUserSet);
			info.AddValue("DataTable.LocaleLCID", this.Locale.LCID);
			info.AddValue("DataTable.MinimumCapacity", this._recordManager.MinimumCapacity);
			info.AddValue("DataTable.NestedInDataSet", this._fNestedInDataset);
			info.AddValue("DataTable.TypeName", this.TypeName.ToString());
			info.AddValue("DataTable.RepeatableElement", this._repeatableElement);
			info.AddValue("DataTable.ExtendedProperties", this.ExtendedProperties);
			info.AddValue("DataTable.Columns.Count", this.Columns.Count);
			if (isSingleTable && !this.CheckForClosureOnExpressionTables(new List<DataTable> { this }))
			{
				throw ExceptionBuilder.CanNotRemoteDataTable();
			}
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			for (int i = 0; i < this.Columns.Count; i++)
			{
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ColumnName", i), this.Columns[i].ColumnName);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Namespace", i), this.Columns[i]._columnUri);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Prefix", i), this.Columns[i].Prefix);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ColumnMapping", i), this.Columns[i].ColumnMapping);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AllowDBNull", i), this.Columns[i].AllowDBNull);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrement", i), this.Columns[i].AutoIncrement);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrementStep", i), this.Columns[i].AutoIncrementStep);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrementSeed", i), this.Columns[i].AutoIncrementSeed);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Caption", i), this.Columns[i].Caption);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.DefaultValue", i), this.Columns[i].DefaultValue);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ReadOnly", i), this.Columns[i].ReadOnly);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.MaxLength", i), this.Columns[i].MaxLength);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.DataType_AssemblyQualifiedName", i), this.Columns[i].DataType.AssemblyQualifiedName);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.XmlDataType", i), this.Columns[i].XmlDataType);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.SimpleType", i), this.Columns[i].SimpleType);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.DateTimeMode", i), this.Columns[i].DateTimeMode);
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrementCurrent", i), this.Columns[i].AutoIncrementCurrent);
				if (isSingleTable)
				{
					info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Expression", i), this.Columns[i].Expression);
				}
				info.AddValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ExtendedProperties", i), this.Columns[i]._extendedProperties);
			}
			if (isSingleTable)
			{
				this.SerializeConstraints(info, context, 0, false);
			}
		}

		internal void DeserializeTableSchema(SerializationInfo info, StreamingContext context, bool isSingleTable)
		{
			this._tableName = info.GetString("DataTable.TableName");
			this._tableNamespace = info.GetString("DataTable.Namespace");
			this._tablePrefix = info.GetString("DataTable.Prefix");
			bool boolean = info.GetBoolean("DataTable.CaseSensitive");
			this.SetCaseSensitiveValue(boolean, true, false);
			this._caseSensitiveUserSet = !info.GetBoolean("DataTable.caseSensitiveAmbient");
			CultureInfo cultureInfo = new CultureInfo((int)info.GetValue("DataTable.LocaleLCID", typeof(int)));
			this.SetLocaleValue(cultureInfo, true, false);
			this._cultureUserSet = true;
			this.MinimumCapacity = info.GetInt32("DataTable.MinimumCapacity");
			this._fNestedInDataset = info.GetBoolean("DataTable.NestedInDataSet");
			string @string = info.GetString("DataTable.TypeName");
			this._typeName = new XmlQualifiedName(@string);
			this._repeatableElement = info.GetBoolean("DataTable.RepeatableElement");
			this._extendedProperties = (PropertyCollection)info.GetValue("DataTable.ExtendedProperties", typeof(PropertyCollection));
			int @int = info.GetInt32("DataTable.Columns.Count");
			string[] array = new string[@int];
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			for (int i = 0; i < @int; i++)
			{
				DataColumn dataColumn = new DataColumn();
				dataColumn.ColumnName = info.GetString(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ColumnName", i));
				dataColumn._columnUri = info.GetString(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Namespace", i));
				dataColumn.Prefix = info.GetString(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Prefix", i));
				string text = (string)info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.DataType_AssemblyQualifiedName", i), typeof(string));
				dataColumn.DataType = Type.GetType(text, true);
				dataColumn.XmlDataType = (string)info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.XmlDataType", i), typeof(string));
				dataColumn.SimpleType = (SimpleType)info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.SimpleType", i), typeof(SimpleType));
				dataColumn.ColumnMapping = (MappingType)info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ColumnMapping", i), typeof(MappingType));
				dataColumn.DateTimeMode = (DataSetDateTime)info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.DateTimeMode", i), typeof(DataSetDateTime));
				dataColumn.AllowDBNull = info.GetBoolean(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AllowDBNull", i));
				dataColumn.AutoIncrement = info.GetBoolean(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrement", i));
				dataColumn.AutoIncrementStep = info.GetInt64(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrementStep", i));
				dataColumn.AutoIncrementSeed = info.GetInt64(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrementSeed", i));
				dataColumn.Caption = info.GetString(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Caption", i));
				dataColumn.DefaultValue = info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.DefaultValue", i), typeof(object));
				dataColumn.ReadOnly = info.GetBoolean(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ReadOnly", i));
				dataColumn.MaxLength = info.GetInt32(string.Format(invariantCulture, "DataTable.DataColumn_{0}.MaxLength", i));
				dataColumn.AutoIncrementCurrent = info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.AutoIncrementCurrent", i), typeof(object));
				if (isSingleTable)
				{
					array[i] = info.GetString(string.Format(invariantCulture, "DataTable.DataColumn_{0}.Expression", i));
				}
				dataColumn._extendedProperties = (PropertyCollection)info.GetValue(string.Format(invariantCulture, "DataTable.DataColumn_{0}.ExtendedProperties", i), typeof(PropertyCollection));
				this.Columns.Add(dataColumn);
			}
			if (isSingleTable)
			{
				for (int j = 0; j < @int; j++)
				{
					if (array[j] != null)
					{
						this.Columns[j].Expression = array[j];
					}
				}
			}
			if (isSingleTable)
			{
				this.DeserializeConstraints(info, context, 0, false);
			}
		}

		internal void SerializeConstraints(SerializationInfo info, StreamingContext context, int serIndex, bool allConstraints)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this.Constraints.Count; i++)
			{
				Constraint constraint = this.Constraints[i];
				UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
				if (uniqueConstraint != null)
				{
					int[] array = new int[uniqueConstraint.Columns.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = uniqueConstraint.Columns[j].Ordinal;
					}
					arrayList.Add(new ArrayList { "U", uniqueConstraint.ConstraintName, array, uniqueConstraint.IsPrimaryKey, uniqueConstraint.ExtendedProperties });
				}
				else
				{
					ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
					if (allConstraints || (foreignKeyConstraint.Table == this && foreignKeyConstraint.RelatedTable == this))
					{
						int[] array2 = new int[foreignKeyConstraint.RelatedColumns.Length + 1];
						array2[0] = (allConstraints ? this.DataSet.Tables.IndexOf(foreignKeyConstraint.RelatedTable) : 0);
						for (int k = 1; k < array2.Length; k++)
						{
							array2[k] = foreignKeyConstraint.RelatedColumns[k - 1].Ordinal;
						}
						int[] array3 = new int[foreignKeyConstraint.Columns.Length + 1];
						array3[0] = (allConstraints ? this.DataSet.Tables.IndexOf(foreignKeyConstraint.Table) : 0);
						for (int l = 1; l < array3.Length; l++)
						{
							array3[l] = foreignKeyConstraint.Columns[l - 1].Ordinal;
						}
						arrayList.Add(new ArrayList
						{
							"F",
							foreignKeyConstraint.ConstraintName,
							array2,
							array3,
							new int[]
							{
								(int)foreignKeyConstraint.AcceptRejectRule,
								(int)foreignKeyConstraint.UpdateRule,
								(int)foreignKeyConstraint.DeleteRule
							},
							foreignKeyConstraint.ExtendedProperties
						});
					}
				}
			}
			info.AddValue(string.Format(CultureInfo.InvariantCulture, "DataTable_{0}.Constraints", serIndex), arrayList);
		}

		internal void DeserializeConstraints(SerializationInfo info, StreamingContext context, int serIndex, bool allConstraints)
		{
			foreach (object obj in ((ArrayList)info.GetValue(string.Format(CultureInfo.InvariantCulture, "DataTable_{0}.Constraints", serIndex), typeof(ArrayList))))
			{
				ArrayList arrayList = (ArrayList)obj;
				if (((string)arrayList[0]).Equals("U"))
				{
					string text = (string)arrayList[1];
					int[] array = (int[])arrayList[2];
					bool flag = (bool)arrayList[3];
					PropertyCollection propertyCollection = (PropertyCollection)arrayList[4];
					DataColumn[] array2 = new DataColumn[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = this.Columns[array[i]];
					}
					UniqueConstraint uniqueConstraint = new UniqueConstraint(text, array2, flag);
					uniqueConstraint._extendedProperties = propertyCollection;
					this.Constraints.Add(uniqueConstraint);
				}
				else
				{
					string text2 = (string)arrayList[1];
					int[] array3 = (int[])arrayList[2];
					int[] array4 = (int[])arrayList[3];
					int[] array5 = (int[])arrayList[4];
					PropertyCollection propertyCollection2 = (PropertyCollection)arrayList[5];
					DataTable dataTable = ((!allConstraints) ? this : this.DataSet.Tables[array3[0]]);
					DataColumn[] array6 = new DataColumn[array3.Length - 1];
					for (int j = 0; j < array6.Length; j++)
					{
						array6[j] = dataTable.Columns[array3[j + 1]];
					}
					DataTable dataTable2 = ((!allConstraints) ? this : this.DataSet.Tables[array4[0]]);
					DataColumn[] array7 = new DataColumn[array4.Length - 1];
					for (int k = 0; k < array7.Length; k++)
					{
						array7[k] = dataTable2.Columns[array4[k + 1]];
					}
					ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(text2, array6, array7);
					foreignKeyConstraint.AcceptRejectRule = (AcceptRejectRule)array5[0];
					foreignKeyConstraint.UpdateRule = (Rule)array5[1];
					foreignKeyConstraint.DeleteRule = (Rule)array5[2];
					foreignKeyConstraint._extendedProperties = propertyCollection2;
					this.Constraints.Add(foreignKeyConstraint, false);
				}
			}
		}

		internal void SerializeExpressionColumns(SerializationInfo info, StreamingContext context, int serIndex)
		{
			int count = this.Columns.Count;
			for (int i = 0; i < count; i++)
			{
				info.AddValue(string.Format(CultureInfo.InvariantCulture, "DataTable_{0}.DataColumn_{1}.Expression", serIndex, i), this.Columns[i].Expression);
			}
		}

		internal void DeserializeExpressionColumns(SerializationInfo info, StreamingContext context, int serIndex)
		{
			int count = this.Columns.Count;
			for (int i = 0; i < count; i++)
			{
				string @string = info.GetString(string.Format(CultureInfo.InvariantCulture, "DataTable_{0}.DataColumn_{1}.Expression", serIndex, i));
				if (@string.Length != 0)
				{
					this.Columns[i].Expression = @string;
				}
			}
		}

		internal void SerializeTableData(SerializationInfo info, StreamingContext context, int serIndex)
		{
			int count = this.Columns.Count;
			int count2 = this.Rows.Count;
			int num = 0;
			int num2 = 0;
			BitArray bitArray = new BitArray(count2 * 3, false);
			int i = 0;
			while (i < count2)
			{
				int num3 = i * 3;
				DataRow dataRow = this.Rows[i];
				DataRowState rowState = dataRow.RowState;
				if (rowState <= DataRowState.Added)
				{
					if (rowState != DataRowState.Unchanged)
					{
						if (rowState != DataRowState.Added)
						{
							goto IL_00A1;
						}
						bitArray[num3 + 1] = true;
					}
				}
				else if (rowState != DataRowState.Deleted)
				{
					if (rowState != DataRowState.Modified)
					{
						goto IL_00A1;
					}
					bitArray[num3] = true;
					num++;
				}
				else
				{
					bitArray[num3] = true;
					bitArray[num3 + 1] = true;
				}
				if (-1 != dataRow._tempRecord)
				{
					bitArray[num3 + 2] = true;
					num2++;
				}
				i++;
				continue;
				IL_00A1:
				throw ExceptionBuilder.InvalidRowState(rowState);
			}
			int num4 = count2 + num + num2;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			if (num4 > 0)
			{
				for (int j = 0; j < count; j++)
				{
					object emptyColumnStore = this.Columns[j].GetEmptyColumnStore(num4);
					arrayList.Add(emptyColumnStore);
					BitArray bitArray2 = new BitArray(num4);
					arrayList2.Add(bitArray2);
				}
			}
			int num5 = 0;
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			for (int k = 0; k < count2; k++)
			{
				int num6 = this.Rows[k].CopyValuesIntoStore(arrayList, arrayList2, num5);
				this.GetRowAndColumnErrors(k, hashtable, hashtable2);
				num5 += num6;
			}
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			info.AddValue(string.Format(invariantCulture, "DataTable_{0}.Rows.Count", serIndex), count2);
			info.AddValue(string.Format(invariantCulture, "DataTable_{0}.Records.Count", serIndex), num4);
			info.AddValue(string.Format(invariantCulture, "DataTable_{0}.RowStates", serIndex), bitArray);
			info.AddValue(string.Format(invariantCulture, "DataTable_{0}.Records", serIndex), arrayList);
			info.AddValue(string.Format(invariantCulture, "DataTable_{0}.NullBits", serIndex), arrayList2);
			info.AddValue(string.Format(invariantCulture, "DataTable_{0}.RowErrors", serIndex), hashtable);
			info.AddValue(string.Format(invariantCulture, "DataTable_{0}.ColumnErrors", serIndex), hashtable2);
		}

		internal void DeserializeTableData(SerializationInfo info, StreamingContext context, int serIndex)
		{
			bool enforceConstraints = this._enforceConstraints;
			bool inDataLoad = this._inDataLoad;
			try
			{
				this._enforceConstraints = false;
				this._inDataLoad = true;
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				int @int = info.GetInt32(string.Format(invariantCulture, "DataTable_{0}.Rows.Count", serIndex));
				int int2 = info.GetInt32(string.Format(invariantCulture, "DataTable_{0}.Records.Count", serIndex));
				BitArray bitArray = (BitArray)info.GetValue(string.Format(invariantCulture, "DataTable_{0}.RowStates", serIndex), typeof(BitArray));
				ArrayList arrayList = (ArrayList)info.GetValue(string.Format(invariantCulture, "DataTable_{0}.Records", serIndex), typeof(ArrayList));
				ArrayList arrayList2 = (ArrayList)info.GetValue(string.Format(invariantCulture, "DataTable_{0}.NullBits", serIndex), typeof(ArrayList));
				Hashtable hashtable = (Hashtable)info.GetValue(string.Format(invariantCulture, "DataTable_{0}.RowErrors", serIndex), typeof(Hashtable));
				hashtable.OnDeserialization(this);
				Hashtable hashtable2 = (Hashtable)info.GetValue(string.Format(invariantCulture, "DataTable_{0}.ColumnErrors", serIndex), typeof(Hashtable));
				hashtable2.OnDeserialization(this);
				if (int2 > 0)
				{
					for (int i = 0; i < this.Columns.Count; i++)
					{
						this.Columns[i].SetStorage(arrayList[i], (BitArray)arrayList2[i]);
					}
					int num = 0;
					DataRow[] array = new DataRow[int2];
					for (int j = 0; j < @int; j++)
					{
						DataRow dataRow = this.NewEmptyRow();
						array[num] = dataRow;
						int num2 = j * 3;
						DataRowState dataRowState = this.ConvertToRowState(bitArray, num2);
						if (dataRowState <= DataRowState.Added)
						{
							if (dataRowState != DataRowState.Unchanged)
							{
								if (dataRowState == DataRowState.Added)
								{
									dataRow._oldRecord = -1;
									dataRow._newRecord = num;
									num++;
								}
							}
							else
							{
								dataRow._oldRecord = num;
								dataRow._newRecord = num;
								num++;
							}
						}
						else if (dataRowState != DataRowState.Deleted)
						{
							if (dataRowState == DataRowState.Modified)
							{
								dataRow._oldRecord = num;
								dataRow._newRecord = num + 1;
								array[num + 1] = dataRow;
								num += 2;
							}
						}
						else
						{
							dataRow._oldRecord = num;
							dataRow._newRecord = -1;
							num++;
						}
						if (bitArray[num2 + 2])
						{
							dataRow._tempRecord = num;
							array[num] = dataRow;
							num++;
						}
						else
						{
							dataRow._tempRecord = -1;
						}
						this.Rows.ArrayAdd(dataRow);
						dataRow.rowID = this._nextRowID;
						this._nextRowID += 1L;
						this.ConvertToRowError(j, hashtable, hashtable2);
					}
					this._recordManager.SetRowCache(array);
					this.ResetIndexes();
				}
			}
			finally
			{
				this._enforceConstraints = enforceConstraints;
				this._inDataLoad = inDataLoad;
			}
		}

		private DataRowState ConvertToRowState(BitArray bitStates, int bitIndex)
		{
			bool flag = bitStates[bitIndex];
			bool flag2 = bitStates[bitIndex + 1];
			if (!flag && !flag2)
			{
				return DataRowState.Unchanged;
			}
			if (!flag && flag2)
			{
				return DataRowState.Added;
			}
			if (flag && !flag2)
			{
				return DataRowState.Modified;
			}
			if (flag && flag2)
			{
				return DataRowState.Deleted;
			}
			throw ExceptionBuilder.InvalidRowBitPattern();
		}

		internal void GetRowAndColumnErrors(int rowIndex, Hashtable rowErrors, Hashtable colErrors)
		{
			DataRow dataRow = this.Rows[rowIndex];
			if (dataRow.HasErrors)
			{
				rowErrors.Add(rowIndex, dataRow.RowError);
				DataColumn[] columnsInError = dataRow.GetColumnsInError();
				if (columnsInError.Length != 0)
				{
					int[] array = new int[columnsInError.Length];
					string[] array2 = new string[columnsInError.Length];
					for (int i = 0; i < columnsInError.Length; i++)
					{
						array[i] = columnsInError[i].Ordinal;
						array2[i] = dataRow.GetColumnError(columnsInError[i]);
					}
					ArrayList arrayList = new ArrayList();
					arrayList.Add(array);
					arrayList.Add(array2);
					colErrors.Add(rowIndex, arrayList);
				}
			}
		}

		private void ConvertToRowError(int rowIndex, Hashtable rowErrors, Hashtable colErrors)
		{
			DataRow dataRow = this.Rows[rowIndex];
			if (rowErrors.ContainsKey(rowIndex))
			{
				dataRow.RowError = (string)rowErrors[rowIndex];
			}
			if (colErrors.ContainsKey(rowIndex))
			{
				ArrayList arrayList = (ArrayList)colErrors[rowIndex];
				int[] array = (int[])arrayList[0];
				string[] array2 = (string[])arrayList[1];
				for (int i = 0; i < array.Length; i++)
				{
					dataRow.SetColumnError(array[i], array2[i]);
				}
			}
		}

		public bool CaseSensitive
		{
			get
			{
				return this._caseSensitive;
			}
			set
			{
				if (this._caseSensitive != value)
				{
					bool caseSensitive = this._caseSensitive;
					bool caseSensitiveUserSet = this._caseSensitiveUserSet;
					this._caseSensitive = value;
					this._caseSensitiveUserSet = true;
					if (this.DataSet != null && !this.DataSet.ValidateCaseConstraint())
					{
						this._caseSensitive = caseSensitive;
						this._caseSensitiveUserSet = caseSensitiveUserSet;
						throw ExceptionBuilder.CannotChangeCaseLocale();
					}
					this.SetCaseSensitiveValue(value, true, true);
				}
				this._caseSensitiveUserSet = true;
			}
		}

		internal bool AreIndexEventsSuspended
		{
			get
			{
				return 0 < this._suspendIndexEvents;
			}
		}

		internal void RestoreIndexEvents(bool forceReset)
		{
			DataCommonEventSource.Log.Trace<int, int>("<ds.DataTable.RestoreIndexEvents|Info> {0}, {1}", this.ObjectID, this._suspendIndexEvents);
			if (0 < this._suspendIndexEvents)
			{
				this._suspendIndexEvents--;
				if (this._suspendIndexEvents == 0)
				{
					Exception ex = null;
					this.SetShadowIndexes();
					try
					{
						int count = this._shadowIndexes.Count;
						for (int i = 0; i < count; i++)
						{
							Index index = this._shadowIndexes[i];
							try
							{
								if (forceReset || index.HasRemoteAggregate)
								{
									index.Reset();
								}
								else
								{
									index.FireResetEvent();
								}
							}
							catch (Exception ex2) when (ADP.IsCatchableExceptionType(ex2))
							{
								ExceptionBuilder.TraceExceptionWithoutRethrow(ex2);
								if (ex == null)
								{
									ex = ex2;
								}
							}
						}
						if (ex != null)
						{
							throw ex;
						}
					}
					finally
					{
						this.RestoreShadowIndexes();
					}
				}
			}
		}

		internal void SuspendIndexEvents()
		{
			DataCommonEventSource.Log.Trace<int, int>("<ds.DataTable.SuspendIndexEvents|Info> {0}, {1}", this.ObjectID, this._suspendIndexEvents);
			this._suspendIndexEvents++;
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return !this.fInitInProgress;
			}
		}

		private bool IsTypedDataTable
		{
			get
			{
				byte isTypedDataTable = this._isTypedDataTable;
				if (isTypedDataTable != 0)
				{
					return isTypedDataTable == 1;
				}
				this._isTypedDataTable = ((base.GetType() != typeof(DataTable)) ? 1 : 2);
				return 1 == this._isTypedDataTable;
			}
		}

		internal bool SetCaseSensitiveValue(bool isCaseSensitive, bool userSet, bool resetIndexes)
		{
			if (userSet || (!this._caseSensitiveUserSet && this._caseSensitive != isCaseSensitive))
			{
				this._caseSensitive = isCaseSensitive;
				if (isCaseSensitive)
				{
					this._compareFlags = CompareOptions.None;
				}
				else
				{
					this._compareFlags = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
				}
				if (resetIndexes)
				{
					this.ResetIndexes();
					foreach (object obj in this.Constraints)
					{
						((Constraint)obj).CheckConstraint();
					}
				}
				return true;
			}
			return false;
		}

		private void ResetCaseSensitive()
		{
			this.SetCaseSensitiveValue(this._dataSet != null && this._dataSet.CaseSensitive, true, true);
			this._caseSensitiveUserSet = false;
		}

		internal bool ShouldSerializeCaseSensitive()
		{
			return this._caseSensitiveUserSet;
		}

		internal bool SelfNested
		{
			get
			{
				foreach (object obj in this.ParentRelations)
				{
					DataRelation dataRelation = (DataRelation)obj;
					if (dataRelation.Nested && dataRelation.ParentTable == this)
					{
						return true;
					}
				}
				return false;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal List<Index> LiveIndexes
		{
			get
			{
				if (!this.AreIndexEventsSuspended)
				{
					int num = this._indexes.Count - 1;
					while (0 <= num)
					{
						Index index = this._indexes[num];
						if (index.RefCount <= 1)
						{
							index.RemoveRef();
						}
						num--;
					}
				}
				return this._indexes;
			}
		}

		[DefaultValue(SerializationFormat.Xml)]
		public SerializationFormat RemotingFormat
		{
			get
			{
				return this._remotingFormat;
			}
			set
			{
				if (value != SerializationFormat.Binary && value != SerializationFormat.Xml)
				{
					throw ExceptionBuilder.InvalidRemotingFormat(value);
				}
				if (this.DataSet != null && value != this.DataSet.RemotingFormat)
				{
					throw ExceptionBuilder.CanNotSetRemotingFormat();
				}
				this._remotingFormat = value;
			}
		}

		internal int UKColumnPositionForInference
		{
			get
			{
				return this._ukColumnPositionForInference;
			}
			set
			{
				this._ukColumnPositionForInference = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRelationCollection ChildRelations
		{
			get
			{
				DataRelationCollection dataRelationCollection;
				if ((dataRelationCollection = this._childRelationsCollection) == null)
				{
					dataRelationCollection = (this._childRelationsCollection = new DataRelationCollection.DataTableRelationCollection(this, false));
				}
				return dataRelationCollection;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataColumnCollection Columns
		{
			get
			{
				return this._columnCollection;
			}
		}

		private void ResetColumns()
		{
			this.Columns.Clear();
		}

		private CompareInfo CompareInfo
		{
			get
			{
				CompareInfo compareInfo;
				if ((compareInfo = this._compareInfo) == null)
				{
					compareInfo = (this._compareInfo = this.Locale.CompareInfo);
				}
				return compareInfo;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ConstraintCollection Constraints
		{
			get
			{
				return this._constraintCollection;
			}
		}

		private void ResetConstraints()
		{
			this.Constraints.Clear();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataSet DataSet
		{
			get
			{
				return this._dataSet;
			}
		}

		internal void SetDataSet(DataSet dataSet)
		{
			if (this._dataSet != dataSet)
			{
				this._dataSet = dataSet;
				DataColumnCollection columns = this.Columns;
				for (int i = 0; i < columns.Count; i++)
				{
					columns[i].OnSetDataSet();
				}
				if (this.DataSet != null)
				{
					this._defaultView = null;
				}
				if (dataSet != null)
				{
					this._remotingFormat = dataSet.RemotingFormat;
				}
			}
		}

		[Browsable(false)]
		public DataView DefaultView
		{
			get
			{
				DataView dataView = this._defaultView;
				if (dataView == null)
				{
					if (this._dataSet != null)
					{
						dataView = this._dataSet.DefaultViewManager.CreateDataView(this);
					}
					else
					{
						dataView = new DataView(this, true);
						dataView.SetIndex2("", DataViewRowState.CurrentRows, null, true);
					}
					dataView = Interlocked.CompareExchange<DataView>(ref this._defaultView, dataView, null);
					if (dataView == null)
					{
						dataView = this._defaultView;
					}
				}
				return dataView;
			}
		}

		[DefaultValue("")]
		public string DisplayExpression
		{
			get
			{
				return this.DisplayExpressionInternal;
			}
			set
			{
				this._displayExpression = ((!string.IsNullOrEmpty(value)) ? new DataExpression(this, value) : null);
			}
		}

		internal string DisplayExpressionInternal
		{
			get
			{
				if (this._displayExpression == null)
				{
					return string.Empty;
				}
				return this._displayExpression.Expression;
			}
		}

		internal bool EnforceConstraints
		{
			get
			{
				if (this.SuspendEnforceConstraints)
				{
					return false;
				}
				if (this._dataSet != null)
				{
					return this._dataSet.EnforceConstraints;
				}
				return this._enforceConstraints;
			}
			set
			{
				if (this._dataSet == null && this._enforceConstraints != value)
				{
					if (value)
					{
						this.EnableConstraints();
					}
					this._enforceConstraints = value;
				}
			}
		}

		internal bool SuspendEnforceConstraints
		{
			get
			{
				return this._suspendEnforceConstraints;
			}
			set
			{
				this._suspendEnforceConstraints = value;
			}
		}

		internal void EnableConstraints()
		{
			bool flag = false;
			foreach (object obj in this.Constraints)
			{
				Constraint constraint = (Constraint)obj;
				if (constraint is UniqueConstraint)
				{
					flag |= constraint.IsConstraintViolated();
				}
			}
			foreach (object obj2 in this.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj2;
				if (!dataColumn.AllowDBNull)
				{
					flag |= dataColumn.IsNotAllowDBNullViolated();
				}
				if (dataColumn.MaxLength >= 0)
				{
					flag |= dataColumn.IsMaxLengthViolated();
				}
			}
			if (flag)
			{
				this.EnforceConstraints = false;
				throw ExceptionBuilder.EnforceConstraint();
			}
		}

		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				PropertyCollection propertyCollection;
				if ((propertyCollection = this._extendedProperties) == null)
				{
					propertyCollection = (this._extendedProperties = new PropertyCollection());
				}
				return propertyCollection;
			}
		}

		internal IFormatProvider FormatProvider
		{
			get
			{
				if (this._formatProvider == null)
				{
					CultureInfo cultureInfo = this.Locale;
					if (cultureInfo.IsNeutralCulture)
					{
						cultureInfo = CultureInfo.InvariantCulture;
					}
					this._formatProvider = cultureInfo;
				}
				return this._formatProvider;
			}
		}

		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				for (int i = 0; i < this.Rows.Count; i++)
				{
					if (this.Rows[i].HasErrors)
					{
						return true;
					}
				}
				return false;
			}
		}

		public CultureInfo Locale
		{
			get
			{
				return this._culture;
			}
			set
			{
				long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.set_Locale|API> {0}", this.ObjectID);
				try
				{
					bool flag = true;
					if (value == null)
					{
						flag = false;
						value = ((this._dataSet != null) ? this._dataSet.Locale : this._culture);
					}
					if (this._culture != value && !this._culture.Equals(value))
					{
						bool flag2 = false;
						bool flag3 = false;
						CultureInfo culture = this._culture;
						bool cultureUserSet = this._cultureUserSet;
						try
						{
							this._cultureUserSet = true;
							this.SetLocaleValue(value, true, false);
							if (this.DataSet == null || this.DataSet.ValidateLocaleConstraint())
							{
								flag2 = false;
								this.SetLocaleValue(value, true, true);
								flag2 = true;
							}
						}
						catch
						{
							flag3 = true;
							throw;
						}
						finally
						{
							if (!flag2)
							{
								try
								{
									this.SetLocaleValue(culture, true, true);
								}
								catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
								{
									ADP.TraceExceptionWithoutRethrow(ex);
								}
								this._cultureUserSet = cultureUserSet;
								if (!flag3)
								{
									throw ExceptionBuilder.CannotChangeCaseLocale(null);
								}
							}
						}
						this.SetLocaleValue(value, true, true);
					}
					this._cultureUserSet = flag;
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(num);
				}
			}
		}

		internal bool SetLocaleValue(CultureInfo culture, bool userSet, bool resetIndexes)
		{
			if (userSet || resetIndexes || (!this._cultureUserSet && !this._culture.Equals(culture)))
			{
				this._culture = culture;
				this._compareInfo = null;
				this._formatProvider = null;
				this._hashCodeProvider = null;
				foreach (object obj in this.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					dataColumn._hashCode = this.GetSpecialHashCode(dataColumn.ColumnName);
				}
				if (resetIndexes)
				{
					this.ResetIndexes();
					foreach (object obj2 in this.Constraints)
					{
						((Constraint)obj2).CheckConstraint();
					}
				}
				return true;
			}
			return false;
		}

		internal bool ShouldSerializeLocale()
		{
			return this._cultureUserSet;
		}

		[DefaultValue(50)]
		public int MinimumCapacity
		{
			get
			{
				return this._recordManager.MinimumCapacity;
			}
			set
			{
				if (value != this._recordManager.MinimumCapacity)
				{
					this._recordManager.MinimumCapacity = value;
				}
			}
		}

		internal int RecordCapacity
		{
			get
			{
				return this._recordManager.RecordCapacity;
			}
		}

		internal int ElementColumnCount
		{
			get
			{
				return this._elementColumnCount;
			}
			set
			{
				if (value > 0 && this._xmlText != null)
				{
					throw ExceptionBuilder.TableCannotAddToSimpleContent();
				}
				this._elementColumnCount = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataRelationCollection ParentRelations
		{
			get
			{
				DataRelationCollection dataRelationCollection;
				if ((dataRelationCollection = this._parentRelationsCollection) == null)
				{
					dataRelationCollection = (this._parentRelationsCollection = new DataRelationCollection.DataTableRelationCollection(this, true));
				}
				return dataRelationCollection;
			}
		}

		internal bool MergingData
		{
			get
			{
				return this._mergingData;
			}
			set
			{
				this._mergingData = value;
			}
		}

		internal DataRelation[] NestedParentRelations
		{
			get
			{
				return this._nestedParentRelations;
			}
		}

		internal bool SchemaLoading
		{
			get
			{
				return this._schemaLoading;
			}
		}

		internal void CacheNestedParent()
		{
			this._nestedParentRelations = this.FindNestedParentRelations();
		}

		private DataRelation[] FindNestedParentRelations()
		{
			List<DataRelation> list = null;
			foreach (object obj in this.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Nested)
				{
					if (list == null)
					{
						list = new List<DataRelation>();
					}
					list.Add(dataRelation);
				}
			}
			if (list != null && list.Count != 0)
			{
				return list.ToArray();
			}
			return Array.Empty<DataRelation>();
		}

		internal int NestedParentsCount
		{
			get
			{
				int num = 0;
				using (IEnumerator enumerator = this.ParentRelations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((DataRelation)enumerator.Current).Nested)
						{
							num++;
						}
					}
				}
				return num;
			}
		}

		[TypeConverter(typeof(PrimaryKeyTypeConverter))]
		public DataColumn[] PrimaryKey
		{
			get
			{
				UniqueConstraint primaryKey = this._primaryKey;
				if (primaryKey != null)
				{
					return primaryKey.Key.ToArray();
				}
				return Array.Empty<DataColumn>();
			}
			set
			{
				UniqueConstraint uniqueConstraint = null;
				if (this.fInitInProgress && value != null)
				{
					this._delayedSetPrimaryKey = value;
					return;
				}
				if (value != null && value.Length != 0)
				{
					int num = 0;
					int num2 = 0;
					while (num2 < value.Length && value[num2] != null)
					{
						num++;
						num2++;
					}
					if (num != 0)
					{
						DataColumn[] array = value;
						if (num != value.Length)
						{
							array = new DataColumn[num];
							for (int i = 0; i < num; i++)
							{
								array[i] = value[i];
							}
						}
						uniqueConstraint = new UniqueConstraint(array);
						if (uniqueConstraint.Table != this)
						{
							throw ExceptionBuilder.TableForeignPrimaryKey();
						}
					}
				}
				if (uniqueConstraint == this._primaryKey || (uniqueConstraint != null && uniqueConstraint.Equals(this._primaryKey)))
				{
					return;
				}
				UniqueConstraint uniqueConstraint2;
				if ((uniqueConstraint2 = (UniqueConstraint)this.Constraints.FindConstraint(uniqueConstraint)) != null)
				{
					uniqueConstraint.ColumnsReference.CopyTo(uniqueConstraint2.Key.ColumnsReference, 0);
					uniqueConstraint = uniqueConstraint2;
				}
				UniqueConstraint primaryKey = this._primaryKey;
				this._primaryKey = null;
				if (primaryKey != null)
				{
					primaryKey.ConstraintIndex.RemoveRef();
					if (this._loadIndex != null)
					{
						this._loadIndex.RemoveRef();
						this._loadIndex = null;
					}
					if (this._loadIndexwithOriginalAdded != null)
					{
						this._loadIndexwithOriginalAdded.RemoveRef();
						this._loadIndexwithOriginalAdded = null;
					}
					if (this._loadIndexwithCurrentDeleted != null)
					{
						this._loadIndexwithCurrentDeleted.RemoveRef();
						this._loadIndexwithCurrentDeleted = null;
					}
					this.Constraints.Remove(primaryKey);
				}
				if (uniqueConstraint != null && uniqueConstraint2 == null)
				{
					this.Constraints.Add(uniqueConstraint);
				}
				this._primaryKey = uniqueConstraint;
				this._primaryIndex = ((uniqueConstraint != null) ? uniqueConstraint.Key.GetIndexDesc() : Array.Empty<IndexField>());
				if (this._primaryKey != null)
				{
					uniqueConstraint.ConstraintIndex.AddRef();
					for (int j = 0; j < uniqueConstraint.ColumnsReference.Length; j++)
					{
						uniqueConstraint.ColumnsReference[j].AllowDBNull = false;
					}
				}
			}
		}

		private bool ShouldSerializePrimaryKey()
		{
			return this._primaryKey != null;
		}

		private void ResetPrimaryKey()
		{
			this.PrimaryKey = null;
		}

		[Browsable(false)]
		public DataRowCollection Rows
		{
			get
			{
				return this._rowCollection;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		public string TableName
		{
			get
			{
				return this._tableName;
			}
			set
			{
				long num = DataCommonEventSource.Log.EnterScope<int, string>("<ds.DataTable.set_TableName|API> {0}, value='{1}'", this.ObjectID, value);
				try
				{
					if (value == null)
					{
						value = string.Empty;
					}
					CultureInfo locale = this.Locale;
					if (string.Compare(this._tableName, value, true, locale) != 0)
					{
						if (this._dataSet != null)
						{
							if (value.Length == 0)
							{
								throw ExceptionBuilder.NoTableName();
							}
							if (string.Compare(value, this._dataSet.DataSetName, true, this._dataSet.Locale) == 0 && !this._fNestedInDataset)
							{
								throw ExceptionBuilder.DatasetConflictingName(this._dataSet.DataSetName);
							}
							DataRelation[] nestedParentRelations = this.NestedParentRelations;
							if (nestedParentRelations.Length == 0)
							{
								this._dataSet.Tables.RegisterName(value, this.Namespace);
							}
							else
							{
								DataRelation[] array = nestedParentRelations;
								for (int i = 0; i < array.Length; i++)
								{
									if (!array[i].ParentTable.Columns.CanRegisterName(value))
									{
										throw ExceptionBuilder.CannotAddDuplicate2(value);
									}
								}
								this._dataSet.Tables.RegisterName(value, this.Namespace);
								foreach (DataRelation dataRelation in nestedParentRelations)
								{
									dataRelation.ParentTable.Columns.RegisterColumnName(value, null);
									dataRelation.ParentTable.Columns.UnregisterName(this.TableName);
								}
							}
							if (this._tableName.Length != 0)
							{
								this._dataSet.Tables.UnregisterName(this._tableName);
							}
						}
						this.RaisePropertyChanging("TableName");
						this._tableName = value;
						this._encodedTableName = null;
					}
					else if (string.Compare(this._tableName, value, false, locale) != 0)
					{
						this.RaisePropertyChanging("TableName");
						this._tableName = value;
						this._encodedTableName = null;
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(num);
				}
			}
		}

		internal string EncodedTableName
		{
			get
			{
				string text = this._encodedTableName;
				if (text == null)
				{
					text = XmlConvert.EncodeLocalName(this.TableName);
					this._encodedTableName = text;
				}
				return text;
			}
		}

		private string GetInheritedNamespace(List<DataTable> visitedTables)
		{
			DataRelation[] nestedParentRelations = this.NestedParentRelations;
			if (nestedParentRelations.Length != 0)
			{
				foreach (DataRelation dataRelation in nestedParentRelations)
				{
					if (dataRelation.ParentTable._tableNamespace != null)
					{
						return dataRelation.ParentTable._tableNamespace;
					}
				}
				int num = 0;
				while (num < nestedParentRelations.Length && (nestedParentRelations[num].ParentTable == this || visitedTables.Contains(nestedParentRelations[num].ParentTable)))
				{
					num++;
				}
				if (num < nestedParentRelations.Length)
				{
					DataTable parentTable = nestedParentRelations[num].ParentTable;
					if (!visitedTables.Contains(parentTable))
					{
						visitedTables.Add(parentTable);
					}
					return parentTable.GetInheritedNamespace(visitedTables);
				}
			}
			if (this.DataSet != null)
			{
				return this.DataSet.Namespace;
			}
			return string.Empty;
		}

		public string Namespace
		{
			get
			{
				return this._tableNamespace ?? this.GetInheritedNamespace(new List<DataTable>());
			}
			set
			{
				long num = DataCommonEventSource.Log.EnterScope<int, string>("<ds.DataTable.set_Namespace|API> {0}, value='{1}'", this.ObjectID, value);
				try
				{
					if (value != this._tableNamespace)
					{
						if (this._dataSet != null)
						{
							string text = ((value == null) ? this.GetInheritedNamespace(new List<DataTable>()) : value);
							if (text != this.Namespace)
							{
								if (this._dataSet.Tables.Contains(this.TableName, text, true, true))
								{
									throw ExceptionBuilder.DuplicateTableName2(this.TableName, text);
								}
								this.CheckCascadingNamespaceConflict(text);
							}
						}
						this.CheckNamespaceValidityForNestedRelations(value);
						this.DoRaiseNamespaceChange();
					}
					this._tableNamespace = value;
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(num);
				}
			}
		}

		internal bool IsNamespaceInherited()
		{
			return this._tableNamespace == null;
		}

		internal void CheckCascadingNamespaceConflict(string realNamespace)
		{
			foreach (object obj in this.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Nested && dataRelation.ChildTable != this && dataRelation.ChildTable._tableNamespace == null)
				{
					DataTable childTable = dataRelation.ChildTable;
					if (this._dataSet.Tables.Contains(childTable.TableName, realNamespace, false, true))
					{
						throw ExceptionBuilder.DuplicateTableName2(this.TableName, realNamespace);
					}
					childTable.CheckCascadingNamespaceConflict(realNamespace);
				}
			}
		}

		internal void CheckNamespaceValidityForNestedRelations(string realNamespace)
		{
			foreach (object obj in this.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Nested)
				{
					if (realNamespace != null)
					{
						dataRelation.ChildTable.CheckNamespaceValidityForNestedParentRelations(realNamespace, this);
					}
					else
					{
						dataRelation.ChildTable.CheckNamespaceValidityForNestedParentRelations(this.GetInheritedNamespace(new List<DataTable>()), this);
					}
				}
			}
			if (realNamespace == null)
			{
				this.CheckNamespaceValidityForNestedParentRelations(this.GetInheritedNamespace(new List<DataTable>()), this);
			}
		}

		internal void CheckNamespaceValidityForNestedParentRelations(string ns, DataTable parentTable)
		{
			foreach (object obj in this.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Nested && dataRelation.ParentTable != parentTable && dataRelation.ParentTable.Namespace != ns)
				{
					throw ExceptionBuilder.InValidNestedRelation(this.TableName);
				}
			}
		}

		internal void DoRaiseNamespaceChange()
		{
			this.RaisePropertyChanging("Namespace");
			foreach (object obj in this.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn._columnUri == null)
				{
					dataColumn.RaisePropertyChanging("Namespace");
				}
			}
			foreach (object obj2 in this.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj2;
				if (dataRelation.Nested && dataRelation.ChildTable != this)
				{
					DataTable childTable = dataRelation.ChildTable;
					dataRelation.ChildTable.DoRaiseNamespaceChange();
				}
			}
		}

		private bool ShouldSerializeNamespace()
		{
			return this._tableNamespace != null;
		}

		private void ResetNamespace()
		{
			this.Namespace = null;
		}

		public virtual void BeginInit()
		{
			this.fInitInProgress = true;
		}

		public virtual void EndInit()
		{
			if (this._dataSet == null || !this._dataSet._fInitInProgress)
			{
				this.Columns.FinishInitCollection();
				this.Constraints.FinishInitConstraints();
				foreach (object obj in this.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					if (dataColumn.Computed)
					{
						dataColumn.Expression = dataColumn.Expression;
					}
				}
			}
			this.fInitInProgress = false;
			if (this._delayedSetPrimaryKey != null)
			{
				this.PrimaryKey = this._delayedSetPrimaryKey;
				this._delayedSetPrimaryKey = null;
			}
			if (this._delayedViews.Count > 0)
			{
				foreach (DataView dataView in this._delayedViews)
				{
					dataView.EndInit();
				}
				this._delayedViews.Clear();
			}
			this.OnInitialized();
		}

		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				return this._tablePrefix;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				DataCommonEventSource.Log.Trace<int, string>("<ds.DataTable.set_Prefix|API> {0}, value='{1}'", this.ObjectID, value);
				if (XmlConvert.DecodeName(value) == value && XmlConvert.EncodeName(value) != value)
				{
					throw ExceptionBuilder.InvalidPrefix(value);
				}
				this._tablePrefix = value;
			}
		}

		internal DataColumn XmlText
		{
			get
			{
				return this._xmlText;
			}
			set
			{
				if (this._xmlText != value)
				{
					if (this._xmlText != null)
					{
						if (value != null)
						{
							throw ExceptionBuilder.MultipleTextOnlyColumns();
						}
						this.Columns.Remove(this._xmlText);
					}
					else if (value != this.Columns[value.ColumnName])
					{
						this.Columns.Add(value);
					}
					this._xmlText = value;
				}
			}
		}

		internal decimal MaxOccurs
		{
			get
			{
				return this._maxOccurs;
			}
			set
			{
				this._maxOccurs = value;
			}
		}

		internal decimal MinOccurs
		{
			get
			{
				return this._minOccurs;
			}
			set
			{
				this._minOccurs = value;
			}
		}

		internal void SetKeyValues(DataKey key, object[] keyValues, int record)
		{
			for (int i = 0; i < keyValues.Length; i++)
			{
				key.ColumnsReference[i][record] = keyValues[i];
			}
		}

		internal DataRow FindByIndex(Index ndx, object[] key)
		{
			Range range = ndx.FindRecords(key);
			if (!range.IsNull)
			{
				return this._recordManager[ndx.GetRecord(range.Min)];
			}
			return null;
		}

		internal DataRow FindMergeTarget(DataRow row, DataKey key, Index ndx)
		{
			DataRow dataRow = null;
			if (key.HasValue)
			{
				int num = ((row._oldRecord == -1) ? row._newRecord : row._oldRecord);
				object[] keyValues = key.GetKeyValues(num);
				dataRow = this.FindByIndex(ndx, keyValues);
			}
			return dataRow;
		}

		private void SetMergeRecords(DataRow row, int newRecord, int oldRecord, DataRowAction action)
		{
			if (newRecord != -1)
			{
				this.SetNewRecord(row, newRecord, action, true, true, false);
				this.SetOldRecord(row, oldRecord);
				return;
			}
			this.SetOldRecord(row, oldRecord);
			if (row._newRecord != -1)
			{
				this.SetNewRecord(row, newRecord, action, true, true, false);
			}
		}

		internal DataRow MergeRow(DataRow row, DataRow targetRow, bool preserveChanges, Index idxSearch)
		{
			if (targetRow == null)
			{
				targetRow = this.NewEmptyRow();
				targetRow._oldRecord = this._recordManager.ImportRecord(row.Table, row._oldRecord);
				targetRow._newRecord = targetRow._oldRecord;
				if (row._oldRecord != row._newRecord)
				{
					targetRow._newRecord = this._recordManager.ImportRecord(row.Table, row._newRecord);
				}
				this.InsertRow(targetRow, -1L);
			}
			else
			{
				int tempRecord = targetRow._tempRecord;
				targetRow._tempRecord = -1;
				try
				{
					DataRowState rowState = targetRow.RowState;
					int num = ((rowState == DataRowState.Added) ? targetRow._newRecord : targetRow._oldRecord);
					if (targetRow.RowState == DataRowState.Unchanged && row.RowState == DataRowState.Unchanged)
					{
						int num2 = targetRow._oldRecord;
						int num3 = (preserveChanges ? this._recordManager.CopyRecord(this, num2, -1) : targetRow._newRecord);
						num2 = this._recordManager.CopyRecord(row.Table, row._oldRecord, targetRow._oldRecord);
						this.SetMergeRecords(targetRow, num3, num2, DataRowAction.Change);
					}
					else if (row._newRecord == -1)
					{
						int num2 = targetRow._oldRecord;
						int num3;
						if (preserveChanges)
						{
							num3 = ((targetRow.RowState == DataRowState.Unchanged) ? this._recordManager.CopyRecord(this, num2, -1) : targetRow._newRecord);
						}
						else
						{
							num3 = -1;
						}
						num2 = this._recordManager.CopyRecord(row.Table, row._oldRecord, num2);
						if (num != ((rowState == DataRowState.Added) ? num3 : num2))
						{
							this.SetMergeRecords(targetRow, num3, num2, (num3 == -1) ? DataRowAction.Delete : DataRowAction.Change);
							idxSearch.Reset();
							int num4 = ((rowState == DataRowState.Added) ? num3 : num2);
						}
						else
						{
							this.SetMergeRecords(targetRow, num3, num2, (num3 == -1) ? DataRowAction.Delete : DataRowAction.Change);
						}
					}
					else
					{
						int num2 = targetRow._oldRecord;
						int num3 = targetRow._newRecord;
						if (targetRow.RowState == DataRowState.Unchanged)
						{
							num3 = this._recordManager.CopyRecord(this, num2, -1);
						}
						num2 = this._recordManager.CopyRecord(row.Table, row._oldRecord, num2);
						if (!preserveChanges)
						{
							num3 = this._recordManager.CopyRecord(row.Table, row._newRecord, num3);
						}
						this.SetMergeRecords(targetRow, num3, num2, DataRowAction.Change);
					}
					if (rowState == DataRowState.Added && targetRow._oldRecord != -1)
					{
						idxSearch.Reset();
					}
				}
				finally
				{
					targetRow._tempRecord = tempRecord;
				}
			}
			if (row.HasErrors)
			{
				if (targetRow.RowError.Length == 0)
				{
					targetRow.RowError = row.RowError;
				}
				else
				{
					DataRow dataRow = targetRow;
					dataRow.RowError = dataRow.RowError + " ]:[ " + row.RowError;
				}
				DataColumn[] columnsInError = row.GetColumnsInError();
				for (int i = 0; i < columnsInError.Length; i++)
				{
					DataColumn dataColumn = targetRow.Table.Columns[columnsInError[i].ColumnName];
					targetRow.SetColumnError(dataColumn, row.GetColumnError(columnsInError[i]));
				}
			}
			else if (!preserveChanges)
			{
				targetRow.ClearErrors();
			}
			return targetRow;
		}

		public void AcceptChanges()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.AcceptChanges|API> {0}", this.ObjectID);
			try
			{
				DataRow[] array = new DataRow[this.Rows.Count];
				this.Rows.CopyTo(array, 0);
				this.SuspendIndexEvents();
				try
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].rowID != -1L)
						{
							array[i].AcceptChanges();
						}
					}
				}
				finally
				{
					this.RestoreIndexEvents(false);
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected virtual DataTable CreateInstance()
		{
			return (DataTable)Activator.CreateInstance(base.GetType(), true);
		}

		public virtual DataTable Clone()
		{
			return this.Clone(null);
		}

		internal DataTable Clone(DataSet cloneDS)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataTable.Clone|INFO> {0}, cloneDS={1}", this.ObjectID, (cloneDS != null) ? cloneDS.ObjectID : 0);
			DataTable dataTable2;
			try
			{
				DataTable dataTable = this.CreateInstance();
				if (dataTable.Columns.Count > 0)
				{
					dataTable.Reset();
				}
				dataTable2 = this.CloneTo(dataTable, cloneDS, false);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataTable2;
		}

		private DataTable IncrementalCloneTo(DataTable sourceTable, DataTable targetTable)
		{
			foreach (object obj in sourceTable.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (targetTable.Columns[dataColumn.ColumnName] == null)
				{
					targetTable.Columns.Add(dataColumn.Clone());
				}
			}
			return targetTable;
		}

		private DataTable CloneHierarchy(DataTable sourceTable, DataSet ds, Hashtable visitedMap)
		{
			if (visitedMap == null)
			{
				visitedMap = new Hashtable();
			}
			if (visitedMap.Contains(sourceTable))
			{
				return (DataTable)visitedMap[sourceTable];
			}
			DataTable dataTable = ds.Tables[sourceTable.TableName, sourceTable.Namespace];
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				dataTable = this.IncrementalCloneTo(sourceTable, dataTable);
			}
			else
			{
				if (dataTable == null)
				{
					dataTable = new DataTable();
					ds.Tables.Add(dataTable);
				}
				dataTable = sourceTable.CloneTo(dataTable, ds, true);
			}
			visitedMap[sourceTable] = dataTable;
			foreach (object obj in sourceTable.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				this.CloneHierarchy(dataRelation.ChildTable, ds, visitedMap);
			}
			return dataTable;
		}

		private DataTable CloneTo(DataTable clone, DataSet cloneDS, bool skipExpressionColumns)
		{
			clone._tableName = this._tableName;
			clone._tableNamespace = this._tableNamespace;
			clone._tablePrefix = this._tablePrefix;
			clone._fNestedInDataset = this._fNestedInDataset;
			clone._culture = this._culture;
			clone._cultureUserSet = this._cultureUserSet;
			clone._compareInfo = this._compareInfo;
			clone._compareFlags = this._compareFlags;
			clone._formatProvider = this._formatProvider;
			clone._hashCodeProvider = this._hashCodeProvider;
			clone._caseSensitive = this._caseSensitive;
			clone._caseSensitiveUserSet = this._caseSensitiveUserSet;
			clone._displayExpression = this._displayExpression;
			clone._typeName = this._typeName;
			clone._repeatableElement = this._repeatableElement;
			clone.MinimumCapacity = this.MinimumCapacity;
			clone.RemotingFormat = this.RemotingFormat;
			DataColumnCollection columns = this.Columns;
			for (int i = 0; i < columns.Count; i++)
			{
				clone.Columns.Add(columns[i].Clone());
			}
			if (!skipExpressionColumns && cloneDS == null)
			{
				for (int j = 0; j < columns.Count; j++)
				{
					clone.Columns[columns[j].ColumnName].Expression = columns[j].Expression;
				}
			}
			DataColumn[] primaryKey = this.PrimaryKey;
			if (primaryKey.Length != 0)
			{
				DataColumn[] array = new DataColumn[primaryKey.Length];
				for (int k = 0; k < primaryKey.Length; k++)
				{
					array[k] = clone.Columns[primaryKey[k].Ordinal];
				}
				clone.PrimaryKey = array;
			}
			for (int l = 0; l < this.Constraints.Count; l++)
			{
				ForeignKeyConstraint foreignKeyConstraint = this.Constraints[l] as ForeignKeyConstraint;
				UniqueConstraint uniqueConstraint = this.Constraints[l] as UniqueConstraint;
				if (foreignKeyConstraint != null)
				{
					if (foreignKeyConstraint.Table == foreignKeyConstraint.RelatedTable)
					{
						ForeignKeyConstraint foreignKeyConstraint2 = foreignKeyConstraint.Clone(clone);
						Constraint constraint = clone.Constraints.FindConstraint(foreignKeyConstraint2);
						if (constraint != null)
						{
							constraint.ConstraintName = this.Constraints[l].ConstraintName;
						}
					}
				}
				else if (uniqueConstraint != null)
				{
					UniqueConstraint uniqueConstraint2 = uniqueConstraint.Clone(clone);
					Constraint constraint2 = clone.Constraints.FindConstraint(uniqueConstraint2);
					if (constraint2 != null)
					{
						constraint2.ConstraintName = this.Constraints[l].ConstraintName;
						foreach (object obj in uniqueConstraint2.ExtendedProperties.Keys)
						{
							constraint2.ExtendedProperties[obj] = uniqueConstraint2.ExtendedProperties[obj];
						}
					}
				}
			}
			for (int m = 0; m < this.Constraints.Count; m++)
			{
				if (!clone.Constraints.Contains(this.Constraints[m].ConstraintName, true))
				{
					ForeignKeyConstraint foreignKeyConstraint3 = this.Constraints[m] as ForeignKeyConstraint;
					UniqueConstraint uniqueConstraint3 = this.Constraints[m] as UniqueConstraint;
					if (foreignKeyConstraint3 != null)
					{
						if (foreignKeyConstraint3.Table == foreignKeyConstraint3.RelatedTable)
						{
							ForeignKeyConstraint foreignKeyConstraint4 = foreignKeyConstraint3.Clone(clone);
							if (foreignKeyConstraint4 != null)
							{
								clone.Constraints.Add(foreignKeyConstraint4);
							}
						}
					}
					else if (uniqueConstraint3 != null)
					{
						clone.Constraints.Add(uniqueConstraint3.Clone(clone));
					}
				}
			}
			if (this._extendedProperties != null)
			{
				foreach (object obj2 in this._extendedProperties.Keys)
				{
					clone.ExtendedProperties[obj2] = this._extendedProperties[obj2];
				}
			}
			return clone;
		}

		public DataTable Copy()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.Copy|API> {0}", this.ObjectID);
			DataTable dataTable2;
			try
			{
				DataTable dataTable = this.Clone();
				foreach (object obj in this.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					this.CopyRow(dataTable, dataRow);
				}
				dataTable2 = dataTable;
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataTable2;
		}

		public event DataColumnChangeEventHandler ColumnChanging
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_ColumnChanging|API> {0}", this.ObjectID);
				this._onColumnChangingDelegate = (DataColumnChangeEventHandler)Delegate.Combine(this._onColumnChangingDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_ColumnChanging|API> {0}", this.ObjectID);
				this._onColumnChangingDelegate = (DataColumnChangeEventHandler)Delegate.Remove(this._onColumnChangingDelegate, value);
			}
		}

		public event DataColumnChangeEventHandler ColumnChanged
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_ColumnChanged|API> {0}", this.ObjectID);
				this._onColumnChangedDelegate = (DataColumnChangeEventHandler)Delegate.Combine(this._onColumnChangedDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_ColumnChanged|API> {0}", this.ObjectID);
				this._onColumnChangedDelegate = (DataColumnChangeEventHandler)Delegate.Remove(this._onColumnChangedDelegate, value);
			}
		}

		public event EventHandler Initialized
		{
			add
			{
				this._onInitialized = (EventHandler)Delegate.Combine(this._onInitialized, value);
			}
			remove
			{
				this._onInitialized = (EventHandler)Delegate.Remove(this._onInitialized, value);
			}
		}

		internal event PropertyChangedEventHandler PropertyChanging
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_PropertyChanging|INFO> {0}", this.ObjectID);
				this._onPropertyChangingDelegate = (PropertyChangedEventHandler)Delegate.Combine(this._onPropertyChangingDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_PropertyChanging|INFO> {0}", this.ObjectID);
				this._onPropertyChangingDelegate = (PropertyChangedEventHandler)Delegate.Remove(this._onPropertyChangingDelegate, value);
			}
		}

		public event DataRowChangeEventHandler RowChanged
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_RowChanged|API> {0}", this.ObjectID);
				this._onRowChangedDelegate = (DataRowChangeEventHandler)Delegate.Combine(this._onRowChangedDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_RowChanged|API> {0}", this.ObjectID);
				this._onRowChangedDelegate = (DataRowChangeEventHandler)Delegate.Remove(this._onRowChangedDelegate, value);
			}
		}

		public event DataRowChangeEventHandler RowChanging
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_RowChanging|API> {0}", this.ObjectID);
				this._onRowChangingDelegate = (DataRowChangeEventHandler)Delegate.Combine(this._onRowChangingDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_RowChanging|API> {0}", this.ObjectID);
				this._onRowChangingDelegate = (DataRowChangeEventHandler)Delegate.Remove(this._onRowChangingDelegate, value);
			}
		}

		public event DataRowChangeEventHandler RowDeleting
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_RowDeleting|API> {0}", this.ObjectID);
				this._onRowDeletingDelegate = (DataRowChangeEventHandler)Delegate.Combine(this._onRowDeletingDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_RowDeleting|API> {0}", this.ObjectID);
				this._onRowDeletingDelegate = (DataRowChangeEventHandler)Delegate.Remove(this._onRowDeletingDelegate, value);
			}
		}

		public event DataRowChangeEventHandler RowDeleted
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_RowDeleted|API> {0}", this.ObjectID);
				this._onRowDeletedDelegate = (DataRowChangeEventHandler)Delegate.Combine(this._onRowDeletedDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_RowDeleted|API> {0}", this.ObjectID);
				this._onRowDeletedDelegate = (DataRowChangeEventHandler)Delegate.Remove(this._onRowDeletedDelegate, value);
			}
		}

		public event DataTableClearEventHandler TableClearing
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_TableClearing|API> {0}", this.ObjectID);
				this._onTableClearingDelegate = (DataTableClearEventHandler)Delegate.Combine(this._onTableClearingDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_TableClearing|API> {0}", this.ObjectID);
				this._onTableClearingDelegate = (DataTableClearEventHandler)Delegate.Remove(this._onTableClearingDelegate, value);
			}
		}

		public event DataTableClearEventHandler TableCleared
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.add_TableCleared|API> {0}", this.ObjectID);
				this._onTableClearedDelegate = (DataTableClearEventHandler)Delegate.Combine(this._onTableClearedDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.remove_TableCleared|API> {0}", this.ObjectID);
				this._onTableClearedDelegate = (DataTableClearEventHandler)Delegate.Remove(this._onTableClearedDelegate, value);
			}
		}

		public event DataTableNewRowEventHandler TableNewRow
		{
			add
			{
				this._onTableNewRowDelegate = (DataTableNewRowEventHandler)Delegate.Combine(this._onTableNewRowDelegate, value);
			}
			remove
			{
				this._onTableNewRowDelegate = (DataTableNewRowEventHandler)Delegate.Remove(this._onTableNewRowDelegate, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				ISite site = this.Site;
				if (value == null && site != null)
				{
					IContainer container = site.Container;
					if (container != null)
					{
						for (int i = 0; i < this.Columns.Count; i++)
						{
							if (this.Columns[i].Site != null)
							{
								container.Remove(this.Columns[i]);
							}
						}
					}
				}
				base.Site = value;
			}
		}

		internal DataRow AddRecords(int oldRecord, int newRecord)
		{
			DataRow dataRow;
			if (oldRecord == -1 && newRecord == -1)
			{
				dataRow = this.NewRow(-1);
				this.AddRow(dataRow);
			}
			else
			{
				dataRow = this.NewEmptyRow();
				dataRow._oldRecord = oldRecord;
				dataRow._newRecord = newRecord;
				this.InsertRow(dataRow, -1L);
			}
			return dataRow;
		}

		internal void AddRow(DataRow row)
		{
			this.AddRow(row, -1);
		}

		internal void AddRow(DataRow row, int proposedID)
		{
			this.InsertRow(row, proposedID, -1);
		}

		internal void InsertRow(DataRow row, int proposedID, int pos)
		{
			this.InsertRow(row, (long)proposedID, pos, true);
		}

		internal void InsertRow(DataRow row, long proposedID, int pos, bool fireEvent)
		{
			Exception ex = null;
			if (row == null)
			{
				throw ExceptionBuilder.ArgumentNull("row");
			}
			if (row.Table != this)
			{
				throw ExceptionBuilder.RowAlreadyInOtherCollection();
			}
			if (row.rowID != -1L)
			{
				throw ExceptionBuilder.RowAlreadyInTheCollection();
			}
			row.BeginEdit();
			int tempRecord = row._tempRecord;
			row._tempRecord = -1;
			if (proposedID == -1L)
			{
				proposedID = this._nextRowID;
			}
			bool flag;
			if (flag = this._nextRowID <= proposedID)
			{
				this._nextRowID = checked(proposedID + 1L);
			}
			try
			{
				try
				{
					row.rowID = proposedID;
					this.SetNewRecordWorker(row, tempRecord, DataRowAction.Add, false, false, pos, fireEvent, out ex);
				}
				catch
				{
					if (flag && this._nextRowID == proposedID + 1L)
					{
						this._nextRowID = proposedID;
					}
					row.rowID = -1L;
					row._tempRecord = tempRecord;
					throw;
				}
				if (ex != null)
				{
					throw ex;
				}
				if (this.EnforceConstraints && !this._inLoad)
				{
					int count = this._columnCollection.Count;
					for (int i = 0; i < count; i++)
					{
						DataColumn dataColumn = this._columnCollection[i];
						if (dataColumn.Computed)
						{
							dataColumn.CheckColumnConstraint(row, DataRowAction.Add);
						}
					}
				}
			}
			finally
			{
				row.ResetLastChangedColumn();
			}
		}

		internal void CheckNotModifying(DataRow row)
		{
			if (row._tempRecord != -1)
			{
				row.EndEdit();
			}
		}

		public void Clear()
		{
			this.Clear(true);
		}

		internal void Clear(bool clearAll)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataTable.Clear|INFO> {0}, clearAll={1}", this.ObjectID, clearAll);
			try
			{
				this._rowDiffId = null;
				if (this._dataSet != null)
				{
					this._dataSet.OnClearFunctionCalled(this);
				}
				bool flag = this.Rows.Count != 0;
				DataTableClearEventArgs e = null;
				if (flag)
				{
					e = new DataTableClearEventArgs(this);
					this.OnTableClearing(e);
				}
				if (this._dataSet != null && this._dataSet.EnforceConstraints)
				{
					ParentForeignKeyConstraintEnumerator parentForeignKeyConstraintEnumerator = new ParentForeignKeyConstraintEnumerator(this._dataSet, this);
					while (parentForeignKeyConstraintEnumerator.GetNext())
					{
						parentForeignKeyConstraintEnumerator.GetForeignKeyConstraint().CheckCanClearParentTable(this);
					}
				}
				this._recordManager.Clear(clearAll);
				foreach (object obj in this.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					dataRow._oldRecord = -1;
					dataRow._newRecord = -1;
					dataRow._tempRecord = -1;
					dataRow.rowID = -1L;
					dataRow.RBTreeNodeId = 0;
				}
				this.Rows.ArrayClear();
				this.ResetIndexes();
				if (flag)
				{
					this.OnTableCleared(e);
				}
				foreach (object obj2 in this.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj2;
					this.EvaluateDependentExpressions(dataColumn);
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		internal void CascadeAll(DataRow row, DataRowAction action)
		{
			if (this.DataSet != null && this.DataSet._fEnableCascading)
			{
				ParentForeignKeyConstraintEnumerator parentForeignKeyConstraintEnumerator = new ParentForeignKeyConstraintEnumerator(this._dataSet, this);
				while (parentForeignKeyConstraintEnumerator.GetNext())
				{
					parentForeignKeyConstraintEnumerator.GetForeignKeyConstraint().CheckCascade(row, action);
				}
			}
		}

		internal void CommitRow(DataRow row)
		{
			DataRowChangeEventArgs e = this.OnRowChanging(null, row, DataRowAction.Commit);
			if (!this._inDataLoad)
			{
				this.CascadeAll(row, DataRowAction.Commit);
			}
			this.SetOldRecord(row, row._newRecord);
			this.OnRowChanged(e, row, DataRowAction.Commit);
		}

		internal int Compare(string s1, string s2)
		{
			return this.Compare(s1, s2, null);
		}

		internal int Compare(string s1, string s2, CompareInfo comparer)
		{
			if (s1 == s2)
			{
				return 0;
			}
			if (s1 == null)
			{
				return -1;
			}
			if (s2 == null)
			{
				return 1;
			}
			int i = s1.Length;
			int num = s2.Length;
			while (i > 0)
			{
				if (s1[i - 1] != ' ' && s1[i - 1] != '\u3000')
				{
					IL_006C:
					while (num > 0 && (s2[num - 1] == ' ' || s2[num - 1] == '\u3000'))
					{
						num--;
					}
					return (comparer ?? this.CompareInfo).Compare(s1, 0, i, s2, 0, num, this._compareFlags);
				}
				i--;
			}
			goto IL_006C;
		}

		internal int IndexOf(string s1, string s2)
		{
			return this.CompareInfo.IndexOf(s1, s2, this._compareFlags);
		}

		internal bool IsSuffix(string s1, string s2)
		{
			return this.CompareInfo.IsSuffix(s1, s2, this._compareFlags);
		}

		public object Compute(string expression, string filter)
		{
			DataRow[] array = this.Select(filter, "", DataViewRowState.CurrentRows);
			return new DataExpression(this, expression).Evaluate(array);
		}

		bool IListSource.ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		internal void CopyRow(DataTable table, DataRow row)
		{
			int num = -1;
			int num2 = -1;
			if (row == null)
			{
				return;
			}
			if (row._oldRecord != -1)
			{
				num = table._recordManager.ImportRecord(row.Table, row._oldRecord);
			}
			if (row._newRecord != -1)
			{
				if (row._newRecord != row._oldRecord)
				{
					num2 = table._recordManager.ImportRecord(row.Table, row._newRecord);
				}
				else
				{
					num2 = num;
				}
			}
			DataRow dataRow = table.AddRecords(num, num2);
			if (row.HasErrors)
			{
				dataRow.RowError = row.RowError;
				DataColumn[] columnsInError = row.GetColumnsInError();
				for (int i = 0; i < columnsInError.Length; i++)
				{
					DataColumn dataColumn = dataRow.Table.Columns[columnsInError[i].ColumnName];
					dataRow.SetColumnError(dataColumn, row.GetColumnError(columnsInError[i]));
				}
			}
		}

		internal void DeleteRow(DataRow row)
		{
			if (row._newRecord == -1)
			{
				throw ExceptionBuilder.RowAlreadyDeleted();
			}
			this.SetNewRecord(row, -1, DataRowAction.Delete, false, true, false);
		}

		private void CheckPrimaryKey()
		{
			if (this._primaryKey == null)
			{
				throw ExceptionBuilder.TableMissingPrimaryKey();
			}
		}

		internal DataRow FindByPrimaryKey(object[] values)
		{
			this.CheckPrimaryKey();
			return this.FindRow(this._primaryKey.Key, values);
		}

		internal DataRow FindByPrimaryKey(object value)
		{
			this.CheckPrimaryKey();
			return this.FindRow(this._primaryKey.Key, value);
		}

		private DataRow FindRow(DataKey key, object[] values)
		{
			Index index = this.GetIndex(this.NewIndexDesc(key));
			Range range = index.FindRecords(values);
			if (range.IsNull)
			{
				return null;
			}
			return this._recordManager[index.GetRecord(range.Min)];
		}

		private DataRow FindRow(DataKey key, object value)
		{
			Index index = this.GetIndex(this.NewIndexDesc(key));
			Range range = index.FindRecords(value);
			if (range.IsNull)
			{
				return null;
			}
			return this._recordManager[index.GetRecord(range.Min)];
		}

		internal string FormatSortString(IndexField[] indexDesc)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IndexField indexField in indexDesc)
			{
				if (0 < stringBuilder.Length)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(indexField.Column.ColumnName);
				if (indexField.IsDescending)
				{
					stringBuilder.Append(" DESC");
				}
			}
			return stringBuilder.ToString();
		}

		internal void FreeRecord(ref int record)
		{
			this._recordManager.FreeRecord(ref record);
		}

		public DataTable GetChanges()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.GetChanges|API> {0}", this.ObjectID);
			DataTable dataTable2;
			try
			{
				DataTable dataTable = this.Clone();
				for (int i = 0; i < this.Rows.Count; i++)
				{
					DataRow dataRow = this.Rows[i];
					if (dataRow._oldRecord != dataRow._newRecord)
					{
						dataTable.ImportRow(dataRow);
					}
				}
				if (dataTable.Rows.Count == 0)
				{
					dataTable2 = null;
				}
				else
				{
					dataTable2 = dataTable;
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataTable2;
		}

		public DataTable GetChanges(DataRowState rowStates)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, DataRowState>("<ds.DataTable.GetChanges|API> {0}, rowStates={1}", this.ObjectID, rowStates);
			DataTable dataTable2;
			try
			{
				DataTable dataTable = this.Clone();
				for (int i = 0; i < this.Rows.Count; i++)
				{
					DataRow dataRow = this.Rows[i];
					if ((dataRow.RowState & rowStates) != (DataRowState)0)
					{
						dataTable.ImportRow(dataRow);
					}
				}
				if (dataTable.Rows.Count == 0)
				{
					dataTable2 = null;
				}
				else
				{
					dataTable2 = dataTable;
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataTable2;
		}

		public DataRow[] GetErrors()
		{
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < this.Rows.Count; i++)
			{
				DataRow dataRow = this.Rows[i];
				if (dataRow.HasErrors)
				{
					list.Add(dataRow);
				}
			}
			DataRow[] array = this.NewRowArray(list.Count);
			list.CopyTo(array);
			return array;
		}

		internal Index GetIndex(IndexField[] indexDesc)
		{
			return this.GetIndex(indexDesc, DataViewRowState.CurrentRows, null);
		}

		internal Index GetIndex(string sort, DataViewRowState recordStates, IFilter rowFilter)
		{
			return this.GetIndex(this.ParseSortString(sort), recordStates, rowFilter);
		}

		internal Index GetIndex(IndexField[] indexDesc, DataViewRowState recordStates, IFilter rowFilter)
		{
			this._indexesLock.EnterUpgradeableReadLock();
			try
			{
				for (int i = 0; i < this._indexes.Count; i++)
				{
					Index index = this._indexes[i];
					if (index != null && index.Equal(indexDesc, recordStates, rowFilter))
					{
						return index;
					}
				}
			}
			finally
			{
				this._indexesLock.ExitUpgradeableReadLock();
			}
			Index index2 = new Index(this, indexDesc, recordStates, rowFilter);
			index2.AddRef();
			return index2;
		}

		IList IListSource.GetList()
		{
			return this.DefaultView;
		}

		internal List<DataViewListener> GetListeners()
		{
			return this._dataViewListeners;
		}

		internal int GetSpecialHashCode(string name)
		{
			int num = 0;
			while (num < name.Length && '\u3000' > name[num])
			{
				num++;
			}
			if (name.Length == num)
			{
				if (this._hashCodeProvider == null)
				{
					this._hashCodeProvider = StringComparer.Create(this.Locale, true);
				}
				return this._hashCodeProvider.GetHashCode(name);
			}
			return 0;
		}

		public void ImportRow(DataRow row)
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.ImportRow|API> {0}", this.ObjectID);
			try
			{
				int num2 = -1;
				int num3 = -1;
				if (row != null)
				{
					if (row._oldRecord != -1)
					{
						num2 = this._recordManager.ImportRecord(row.Table, row._oldRecord);
					}
					if (row._newRecord != -1)
					{
						if (row.RowState != DataRowState.Unchanged)
						{
							num3 = this._recordManager.ImportRecord(row.Table, row._newRecord);
						}
						else
						{
							num3 = num2;
						}
					}
					if (num2 != -1 || num3 != -1)
					{
						DataRow dataRow = this.AddRecords(num2, num3);
						if (row.HasErrors)
						{
							dataRow.RowError = row.RowError;
							DataColumn[] columnsInError = row.GetColumnsInError();
							for (int i = 0; i < columnsInError.Length; i++)
							{
								DataColumn dataColumn = dataRow.Table.Columns[columnsInError[i].ColumnName];
								dataRow.SetColumnError(dataColumn, row.GetColumnError(columnsInError[i]));
							}
						}
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		internal void InsertRow(DataRow row, long proposedID)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataTable.InsertRow|INFO> {0}, row={1}", this.ObjectID, row._objectID);
			try
			{
				if (row.Table != this)
				{
					throw ExceptionBuilder.RowAlreadyInOtherCollection();
				}
				if (row.rowID != -1L)
				{
					throw ExceptionBuilder.RowAlreadyInTheCollection();
				}
				if (row._oldRecord == -1 && row._newRecord == -1)
				{
					throw ExceptionBuilder.RowEmpty();
				}
				if (proposedID == -1L)
				{
					proposedID = this._nextRowID;
				}
				row.rowID = proposedID;
				if (this._nextRowID <= proposedID)
				{
					this._nextRowID = checked(proposedID + 1L);
				}
				DataRowChangeEventArgs e = null;
				if (row._newRecord != -1)
				{
					row._tempRecord = row._newRecord;
					row._newRecord = -1;
					try
					{
						e = this.RaiseRowChanging(null, row, DataRowAction.Add, true);
					}
					catch
					{
						row._tempRecord = -1;
						throw;
					}
					row._newRecord = row._tempRecord;
					row._tempRecord = -1;
				}
				if (row._oldRecord != -1)
				{
					this._recordManager[row._oldRecord] = row;
				}
				if (row._newRecord != -1)
				{
					this._recordManager[row._newRecord] = row;
				}
				this.Rows.ArrayAdd(row);
				if (row.RowState == DataRowState.Unchanged)
				{
					this.RecordStateChanged(row._oldRecord, DataViewRowState.None, DataViewRowState.Unchanged);
				}
				else
				{
					this.RecordStateChanged(row._oldRecord, DataViewRowState.None, row.GetRecordState(row._oldRecord), row._newRecord, DataViewRowState.None, row.GetRecordState(row._newRecord));
				}
				if (this._dependentColumns != null && this._dependentColumns.Count > 0)
				{
					this.EvaluateExpressions(row, DataRowAction.Add, null);
				}
				this.RaiseRowChanged(e, row, DataRowAction.Add);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		private IndexField[] NewIndexDesc(DataKey key)
		{
			IndexField[] indexDesc = key.GetIndexDesc();
			IndexField[] array = new IndexField[indexDesc.Length];
			Array.Copy(indexDesc, 0, array, 0, indexDesc.Length);
			return array;
		}

		internal int NewRecord()
		{
			return this.NewRecord(-1);
		}

		internal int NewUninitializedRecord()
		{
			return this._recordManager.NewRecordBase();
		}

		internal int NewRecordFromArray(object[] value)
		{
			int count = this._columnCollection.Count;
			if (count < value.Length)
			{
				throw ExceptionBuilder.ValueArrayLength();
			}
			int num = this._recordManager.NewRecordBase();
			int num2;
			try
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] != null)
					{
						this._columnCollection[i][num] = value[i];
					}
					else
					{
						this._columnCollection[i].Init(num);
					}
				}
				for (int j = value.Length; j < count; j++)
				{
					this._columnCollection[j].Init(num);
				}
				num2 = num;
			}
			catch (Exception ex) when (ADP.IsCatchableOrSecurityExceptionType(ex))
			{
				this.FreeRecord(ref num);
				throw;
			}
			return num2;
		}

		internal int NewRecord(int sourceRecord)
		{
			int num = this._recordManager.NewRecordBase();
			int count = this._columnCollection.Count;
			if (-1 == sourceRecord)
			{
				for (int i = 0; i < count; i++)
				{
					this._columnCollection[i].Init(num);
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					this._columnCollection[j].Copy(sourceRecord, num);
				}
			}
			return num;
		}

		internal DataRow NewEmptyRow()
		{
			this._rowBuilder._record = -1;
			DataRow dataRow = this.NewRowFromBuilder(this._rowBuilder);
			if (this._dataSet != null)
			{
				this.DataSet.OnDataRowCreated(dataRow);
			}
			return dataRow;
		}

		private DataRow NewUninitializedRow()
		{
			return this.NewRow(this.NewUninitializedRecord());
		}

		public DataRow NewRow()
		{
			DataRow dataRow = this.NewRow(-1);
			this.NewRowCreated(dataRow);
			return dataRow;
		}

		internal DataRow CreateEmptyRow()
		{
			DataRow dataRow = this.NewUninitializedRow();
			foreach (object obj in this.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (!XmlToDatasetMap.IsMappedColumn(dataColumn))
				{
					if (!dataColumn.AutoIncrement)
					{
						if (dataColumn.AllowDBNull)
						{
							dataRow[dataColumn] = DBNull.Value;
						}
						else if (dataColumn.DefaultValue != null)
						{
							dataRow[dataColumn] = dataColumn.DefaultValue;
						}
					}
					else
					{
						dataColumn.Init(dataRow._tempRecord);
					}
				}
			}
			return dataRow;
		}

		private void NewRowCreated(DataRow row)
		{
			if (this._onTableNewRowDelegate != null)
			{
				DataTableNewRowEventArgs e = new DataTableNewRowEventArgs(row);
				this.OnTableNewRow(e);
			}
		}

		internal DataRow NewRow(int record)
		{
			if (-1 == record)
			{
				record = this.NewRecord(-1);
			}
			this._rowBuilder._record = record;
			DataRow dataRow = this.NewRowFromBuilder(this._rowBuilder);
			this._recordManager[record] = dataRow;
			if (this._dataSet != null)
			{
				this.DataSet.OnDataRowCreated(dataRow);
			}
			return dataRow;
		}

		protected virtual DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new DataRow(builder);
		}

		protected virtual Type GetRowType()
		{
			return typeof(DataRow);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected internal DataRow[] NewRowArray(int size)
		{
			if (this.IsTypedDataTable)
			{
				if (size == 0)
				{
					if (this._emptyDataRowArray == null)
					{
						this._emptyDataRowArray = (DataRow[])Array.CreateInstance(this.GetRowType(), 0);
					}
					return this._emptyDataRowArray;
				}
				return (DataRow[])Array.CreateInstance(this.GetRowType(), size);
			}
			else
			{
				if (size != 0)
				{
					return new DataRow[size];
				}
				return Array.Empty<DataRow>();
			}
		}

		internal bool NeedColumnChangeEvents
		{
			get
			{
				return this.IsTypedDataTable || this._onColumnChangingDelegate != null || this._onColumnChangedDelegate != null;
			}
		}

		protected internal virtual void OnColumnChanging(DataColumnChangeEventArgs e)
		{
			if (this._onColumnChangingDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnColumnChanging|INFO> {0}", this.ObjectID);
				this._onColumnChangingDelegate(this, e);
			}
		}

		protected internal virtual void OnColumnChanged(DataColumnChangeEventArgs e)
		{
			if (this._onColumnChangedDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnColumnChanged|INFO> {0}", this.ObjectID);
				this._onColumnChangedDelegate(this, e);
			}
		}

		protected virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			if (this._onPropertyChangingDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnPropertyChanging|INFO> {0}", this.ObjectID);
				this._onPropertyChangingDelegate(this, pcevent);
			}
		}

		internal void OnRemoveColumnInternal(DataColumn column)
		{
			this.OnRemoveColumn(column);
		}

		protected virtual void OnRemoveColumn(DataColumn column)
		{
		}

		private DataRowChangeEventArgs OnRowChanged(DataRowChangeEventArgs args, DataRow eRow, DataRowAction eAction)
		{
			if (this._onRowChangedDelegate != null || this.IsTypedDataTable)
			{
				if (args == null)
				{
					args = new DataRowChangeEventArgs(eRow, eAction);
				}
				this.OnRowChanged(args);
			}
			return args;
		}

		private DataRowChangeEventArgs OnRowChanging(DataRowChangeEventArgs args, DataRow eRow, DataRowAction eAction)
		{
			if (this._onRowChangingDelegate != null || this.IsTypedDataTable)
			{
				if (args == null)
				{
					args = new DataRowChangeEventArgs(eRow, eAction);
				}
				this.OnRowChanging(args);
			}
			return args;
		}

		protected virtual void OnRowChanged(DataRowChangeEventArgs e)
		{
			if (this._onRowChangedDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnRowChanged|INFO> {0}", this.ObjectID);
				this._onRowChangedDelegate(this, e);
			}
		}

		protected virtual void OnRowChanging(DataRowChangeEventArgs e)
		{
			if (this._onRowChangingDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnRowChanging|INFO> {0}", this.ObjectID);
				this._onRowChangingDelegate(this, e);
			}
		}

		protected virtual void OnRowDeleting(DataRowChangeEventArgs e)
		{
			if (this._onRowDeletingDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnRowDeleting|INFO> {0}", this.ObjectID);
				this._onRowDeletingDelegate(this, e);
			}
		}

		protected virtual void OnRowDeleted(DataRowChangeEventArgs e)
		{
			if (this._onRowDeletedDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnRowDeleted|INFO> {0}", this.ObjectID);
				this._onRowDeletedDelegate(this, e);
			}
		}

		protected virtual void OnTableCleared(DataTableClearEventArgs e)
		{
			if (this._onTableClearedDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnTableCleared|INFO> {0}", this.ObjectID);
				this._onTableClearedDelegate(this, e);
			}
		}

		protected virtual void OnTableClearing(DataTableClearEventArgs e)
		{
			if (this._onTableClearingDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnTableClearing|INFO> {0}", this.ObjectID);
				this._onTableClearingDelegate(this, e);
			}
		}

		protected virtual void OnTableNewRow(DataTableNewRowEventArgs e)
		{
			if (this._onTableNewRowDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnTableNewRow|INFO> {0}", this.ObjectID);
				this._onTableNewRowDelegate(this, e);
			}
		}

		private void OnInitialized()
		{
			if (this._onInitialized != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTable.OnInitialized|INFO> {0}", this.ObjectID);
				this._onInitialized(this, EventArgs.Empty);
			}
		}

		internal IndexField[] ParseSortString(string sortString)
		{
			IndexField[] array = Array.Empty<IndexField>();
			if (sortString != null && 0 < sortString.Length)
			{
				string[] array2 = sortString.Split(new char[] { ',' });
				array = new IndexField[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i].Trim();
					int length = text.Length;
					bool flag = false;
					if (length >= 5 && string.Compare(text, length - 4, " ASC", 0, 4, StringComparison.OrdinalIgnoreCase) == 0)
					{
						text = text.Substring(0, length - 4).Trim();
					}
					else if (length >= 6 && string.Compare(text, length - 5, " DESC", 0, 5, StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						text = text.Substring(0, length - 5).Trim();
					}
					if (text.StartsWith("[", StringComparison.Ordinal))
					{
						if (!text.EndsWith("]", StringComparison.Ordinal))
						{
							throw ExceptionBuilder.InvalidSortString(array2[i]);
						}
						text = text.Substring(1, text.Length - 2);
					}
					DataColumn dataColumn = this.Columns[text];
					if (dataColumn == null)
					{
						throw ExceptionBuilder.ColumnOutOfRange(text);
					}
					array[i] = new IndexField(dataColumn, flag);
				}
			}
			return array;
		}

		internal void RaisePropertyChanging(string name)
		{
			this.OnPropertyChanging(new PropertyChangedEventArgs(name));
		}

		internal void RecordChanged(int record)
		{
			this.SetShadowIndexes();
			try
			{
				int count = this._shadowIndexes.Count;
				for (int i = 0; i < count; i++)
				{
					Index index = this._shadowIndexes[i];
					if (0 < index.RefCount)
					{
						index.RecordChanged(record);
					}
				}
			}
			finally
			{
				this.RestoreShadowIndexes();
			}
		}

		internal void RecordChanged(int[] oldIndex, int[] newIndex)
		{
			this.SetShadowIndexes();
			try
			{
				int count = this._shadowIndexes.Count;
				for (int i = 0; i < count; i++)
				{
					Index index = this._shadowIndexes[i];
					if (0 < index.RefCount)
					{
						index.RecordChanged(oldIndex[i], newIndex[i]);
					}
				}
			}
			finally
			{
				this.RestoreShadowIndexes();
			}
		}

		internal void RecordStateChanged(int record, DataViewRowState oldState, DataViewRowState newState)
		{
			this.SetShadowIndexes();
			try
			{
				int count = this._shadowIndexes.Count;
				for (int i = 0; i < count; i++)
				{
					Index index = this._shadowIndexes[i];
					if (0 < index.RefCount)
					{
						index.RecordStateChanged(record, oldState, newState);
					}
				}
			}
			finally
			{
				this.RestoreShadowIndexes();
			}
		}

		internal void RecordStateChanged(int record1, DataViewRowState oldState1, DataViewRowState newState1, int record2, DataViewRowState oldState2, DataViewRowState newState2)
		{
			this.SetShadowIndexes();
			try
			{
				int count = this._shadowIndexes.Count;
				for (int i = 0; i < count; i++)
				{
					Index index = this._shadowIndexes[i];
					if (0 < index.RefCount)
					{
						if (record1 != -1 && record2 != -1)
						{
							index.RecordStateChanged(record1, oldState1, newState1, record2, oldState2, newState2);
						}
						else if (record1 != -1)
						{
							index.RecordStateChanged(record1, oldState1, newState1);
						}
						else if (record2 != -1)
						{
							index.RecordStateChanged(record2, oldState2, newState2);
						}
					}
				}
			}
			finally
			{
				this.RestoreShadowIndexes();
			}
		}

		internal int[] RemoveRecordFromIndexes(DataRow row, DataRowVersion version)
		{
			int num = this.LiveIndexes.Count;
			int[] array = new int[num];
			int recordFromVersion = row.GetRecordFromVersion(version);
			DataViewRowState recordState = row.GetRecordState(recordFromVersion);
			while (--num >= 0)
			{
				if (row.HasVersion(version) && (recordState & this._indexes[num].RecordStates) != DataViewRowState.None)
				{
					int index = this._indexes[num].GetIndex(recordFromVersion);
					if (index > -1)
					{
						array[num] = index;
						this._indexes[num].DeleteRecordFromIndex(index);
					}
					else
					{
						array[num] = -1;
					}
				}
				else
				{
					array[num] = -1;
				}
			}
			return array;
		}

		internal int[] InsertRecordToIndexes(DataRow row, DataRowVersion version)
		{
			int num = this.LiveIndexes.Count;
			int[] array = new int[num];
			int recordFromVersion = row.GetRecordFromVersion(version);
			DataViewRowState recordState = row.GetRecordState(recordFromVersion);
			while (--num >= 0)
			{
				if (row.HasVersion(version))
				{
					if ((recordState & this._indexes[num].RecordStates) != DataViewRowState.None)
					{
						array[num] = this._indexes[num].InsertRecordToIndex(recordFromVersion);
					}
					else
					{
						array[num] = -1;
					}
				}
			}
			return array;
		}

		internal void SilentlySetValue(DataRow dr, DataColumn dc, DataRowVersion version, object newValue)
		{
			int recordFromVersion = dr.GetRecordFromVersion(version);
			if ((DataStorage.IsTypeCustomType(dc.DataType) && newValue != dc[recordFromVersion]) || !dc.CompareValueTo(recordFromVersion, newValue, true))
			{
				int[] array = dr.Table.RemoveRecordFromIndexes(dr, version);
				dc.SetValue(recordFromVersion, newValue);
				int[] array2 = dr.Table.InsertRecordToIndexes(dr, version);
				if (dr.HasVersion(version))
				{
					if (version != DataRowVersion.Original)
					{
						dr.Table.RecordChanged(array, array2);
					}
					if (dc._dependentColumns != null)
					{
						dc.Table.EvaluateDependentExpressions(dc._dependentColumns, dr, version, null);
					}
				}
			}
			dr.ResetLastChangedColumn();
		}

		public void RejectChanges()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.RejectChanges|API> {0}", this.ObjectID);
			try
			{
				DataRow[] array = new DataRow[this.Rows.Count];
				this.Rows.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					this.RollbackRow(array[i]);
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		internal void RemoveRow(DataRow row, bool check)
		{
			if (row.rowID == -1L)
			{
				throw ExceptionBuilder.RowAlreadyRemoved();
			}
			if (check && this._dataSet != null)
			{
				ParentForeignKeyConstraintEnumerator parentForeignKeyConstraintEnumerator = new ParentForeignKeyConstraintEnumerator(this._dataSet, this);
				while (parentForeignKeyConstraintEnumerator.GetNext())
				{
					parentForeignKeyConstraintEnumerator.GetForeignKeyConstraint().CheckCanRemoveParentRow(row);
				}
			}
			int num = row._oldRecord;
			int newRecord = row._newRecord;
			DataViewRowState recordState = row.GetRecordState(num);
			DataViewRowState recordState2 = row.GetRecordState(newRecord);
			row._oldRecord = -1;
			row._newRecord = -1;
			if (num == newRecord)
			{
				num = -1;
			}
			this.RecordStateChanged(num, recordState, DataViewRowState.None, newRecord, recordState2, DataViewRowState.None);
			this.FreeRecord(ref num);
			this.FreeRecord(ref newRecord);
			row.rowID = -1L;
			this.Rows.ArrayRemove(row);
		}

		public virtual void Reset()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.Reset|API> {0}", this.ObjectID);
			try
			{
				this.Clear();
				this.ResetConstraints();
				DataRelationCollection dataRelationCollection = this.ParentRelations;
				int i = dataRelationCollection.Count;
				while (i > 0)
				{
					i--;
					dataRelationCollection.RemoveAt(i);
				}
				dataRelationCollection = this.ChildRelations;
				i = dataRelationCollection.Count;
				while (i > 0)
				{
					i--;
					dataRelationCollection.RemoveAt(i);
				}
				this.Columns.Clear();
				this._indexes.Clear();
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		internal void ResetIndexes()
		{
			this.ResetInternalIndexes(null);
		}

		internal void ResetInternalIndexes(DataColumn column)
		{
			this.SetShadowIndexes();
			try
			{
				int count = this._shadowIndexes.Count;
				for (int i = 0; i < count; i++)
				{
					Index index = this._shadowIndexes[i];
					if (0 < index.RefCount)
					{
						if (column == null)
						{
							index.Reset();
						}
						else
						{
							bool flag = false;
							foreach (IndexField indexField in index._indexFields)
							{
								if (column == indexField.Column)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								index.Reset();
							}
						}
					}
				}
			}
			finally
			{
				this.RestoreShadowIndexes();
			}
		}

		internal void RollbackRow(DataRow row)
		{
			row.CancelEdit();
			this.SetNewRecord(row, row._oldRecord, DataRowAction.Rollback, false, true, false);
		}

		private DataRowChangeEventArgs RaiseRowChanged(DataRowChangeEventArgs args, DataRow eRow, DataRowAction eAction)
		{
			try
			{
				if (this.UpdatingCurrent(eRow, eAction) && (this.IsTypedDataTable || this._onRowChangedDelegate != null))
				{
					args = this.OnRowChanged(args, eRow, eAction);
				}
				else if (DataRowAction.Delete == eAction && eRow._newRecord == -1 && (this.IsTypedDataTable || this._onRowDeletedDelegate != null))
				{
					if (args == null)
					{
						args = new DataRowChangeEventArgs(eRow, eAction);
					}
					this.OnRowDeleted(args);
				}
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
			}
			return args;
		}

		private DataRowChangeEventArgs RaiseRowChanging(DataRowChangeEventArgs args, DataRow eRow, DataRowAction eAction)
		{
			if (this.UpdatingCurrent(eRow, eAction) && (this.IsTypedDataTable || this._onRowChangingDelegate != null))
			{
				eRow._inChangingEvent = true;
				try
				{
					return this.OnRowChanging(args, eRow, eAction);
				}
				finally
				{
					eRow._inChangingEvent = false;
				}
			}
			if (DataRowAction.Delete == eAction && eRow._newRecord != -1 && (this.IsTypedDataTable || this._onRowDeletingDelegate != null))
			{
				eRow._inDeletingEvent = true;
				try
				{
					if (args == null)
					{
						args = new DataRowChangeEventArgs(eRow, eAction);
					}
					this.OnRowDeleting(args);
				}
				finally
				{
					eRow._inDeletingEvent = false;
				}
			}
			return args;
		}

		private DataRowChangeEventArgs RaiseRowChanging(DataRowChangeEventArgs args, DataRow eRow, DataRowAction eAction, bool fireEvent)
		{
			if (this.EnforceConstraints && !this._inLoad)
			{
				int count = this._columnCollection.Count;
				for (int i = 0; i < count; i++)
				{
					DataColumn dataColumn = this._columnCollection[i];
					if (!dataColumn.Computed || eAction != DataRowAction.Add)
					{
						dataColumn.CheckColumnConstraint(eRow, eAction);
					}
				}
				int count2 = this._constraintCollection.Count;
				for (int j = 0; j < count2; j++)
				{
					this._constraintCollection[j].CheckConstraint(eRow, eAction);
				}
			}
			if (fireEvent)
			{
				args = this.RaiseRowChanging(args, eRow, eAction);
			}
			if (!this._inDataLoad && !this.MergingData && eAction != DataRowAction.Nothing && eAction != DataRowAction.ChangeOriginal)
			{
				this.CascadeAll(eRow, eAction);
			}
			return args;
		}

		public DataRow[] Select()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.DataTable.Select|API> {0}", this.ObjectID);
			return new Select(this, "", "", DataViewRowState.CurrentRows).SelectRows();
		}

		public DataRow[] Select(string filterExpression)
		{
			DataCommonEventSource.Log.Trace<int, string>("<ds.DataTable.Select|API> {0}, filterExpression='{1}'", this.ObjectID, filterExpression);
			return new Select(this, filterExpression, "", DataViewRowState.CurrentRows).SelectRows();
		}

		public DataRow[] Select(string filterExpression, string sort)
		{
			DataCommonEventSource.Log.Trace<int, string, string>("<ds.DataTable.Select|API> {0}, filterExpression='{1}', sort='{2}'", this.ObjectID, filterExpression, sort);
			return new Select(this, filterExpression, sort, DataViewRowState.CurrentRows).SelectRows();
		}

		public DataRow[] Select(string filterExpression, string sort, DataViewRowState recordStates)
		{
			DataCommonEventSource.Log.Trace<int, string, string, DataViewRowState>("<ds.DataTable.Select|API> {0}, filterExpression='{1}', sort='{2}', recordStates={3}", this.ObjectID, filterExpression, sort, recordStates);
			return new Select(this, filterExpression, sort, recordStates).SelectRows();
		}

		internal void SetNewRecord(DataRow row, int proposedRecord, DataRowAction action = DataRowAction.Change, bool isInMerge = false, bool fireEvent = true, bool suppressEnsurePropertyChanged = false)
		{
			Exception ex = null;
			this.SetNewRecordWorker(row, proposedRecord, action, isInMerge, suppressEnsurePropertyChanged, -1, fireEvent, out ex);
			if (ex != null)
			{
				throw ex;
			}
		}

		private void SetNewRecordWorker(DataRow row, int proposedRecord, DataRowAction action, bool isInMerge, bool suppressEnsurePropertyChanged, int position, bool fireEvent, out Exception deferredException)
		{
			deferredException = null;
			if (row._tempRecord != proposedRecord)
			{
				if (!this._inDataLoad)
				{
					row.CheckInTable();
					this.CheckNotModifying(row);
				}
				if (proposedRecord == row._newRecord)
				{
					if (isInMerge)
					{
						this.RaiseRowChanged(null, row, action);
					}
					return;
				}
				row._tempRecord = proposedRecord;
			}
			DataRowChangeEventArgs e = null;
			try
			{
				row._action = action;
				e = this.RaiseRowChanging(null, row, action, fireEvent);
			}
			catch
			{
				row._tempRecord = -1;
				throw;
			}
			finally
			{
				row._action = DataRowAction.Nothing;
			}
			row._tempRecord = -1;
			int num = row._newRecord;
			int num2 = ((proposedRecord != -1) ? proposedRecord : ((row.RowState != DataRowState.Unchanged) ? row._oldRecord : (-1)));
			if (action == DataRowAction.Add)
			{
				if (position == -1)
				{
					this.Rows.ArrayAdd(row);
				}
				else
				{
					this.Rows.ArrayInsert(row, position);
				}
			}
			List<DataRow> list = null;
			if ((action == DataRowAction.Delete || action == DataRowAction.Change) && this._dependentColumns != null && this._dependentColumns.Count > 0)
			{
				list = new List<DataRow>();
				for (int i = 0; i < this.ParentRelations.Count; i++)
				{
					DataRelation dataRelation = this.ParentRelations[i];
					if (dataRelation.ChildTable == row.Table)
					{
						list.InsertRange(list.Count, row.GetParentRows(dataRelation));
					}
				}
				for (int j = 0; j < this.ChildRelations.Count; j++)
				{
					DataRelation dataRelation2 = this.ChildRelations[j];
					if (dataRelation2.ParentTable == row.Table)
					{
						list.InsertRange(list.Count, row.GetChildRows(dataRelation2));
					}
				}
			}
			if (!suppressEnsurePropertyChanged && !row.HasPropertyChanged && row._newRecord != proposedRecord && -1 != proposedRecord && -1 != row._newRecord)
			{
				row.LastChangedColumn = null;
				row.LastChangedColumn = null;
			}
			if (this.LiveIndexes.Count != 0)
			{
				if (-1 == num && -1 != proposedRecord && -1 != row._oldRecord && proposedRecord != row._oldRecord)
				{
					num = row._oldRecord;
				}
				DataViewRowState recordState = row.GetRecordState(num);
				DataViewRowState recordState2 = row.GetRecordState(num2);
				row._newRecord = proposedRecord;
				if (proposedRecord != -1)
				{
					this._recordManager[proposedRecord] = row;
				}
				DataViewRowState recordState3 = row.GetRecordState(num);
				DataViewRowState recordState4 = row.GetRecordState(num2);
				this.RecordStateChanged(num, recordState, recordState3, num2, recordState2, recordState4);
			}
			else
			{
				row._newRecord = proposedRecord;
				if (proposedRecord != -1)
				{
					this._recordManager[proposedRecord] = row;
				}
			}
			row.ResetLastChangedColumn();
			if (-1 != num && num != row._oldRecord && num != row._tempRecord && num != row._newRecord && row == this._recordManager[num])
			{
				this.FreeRecord(ref num);
			}
			if (row.RowState == DataRowState.Detached && row.rowID != -1L)
			{
				this.RemoveRow(row, false);
			}
			if (this._dependentColumns != null && this._dependentColumns.Count > 0)
			{
				try
				{
					this.EvaluateExpressions(row, action, list);
				}
				catch (Exception ex)
				{
					if (action != DataRowAction.Add)
					{
						throw ex;
					}
					deferredException = ex;
				}
			}
			try
			{
				if (fireEvent)
				{
					this.RaiseRowChanged(e, row, action);
				}
			}
			catch (Exception ex2) when (ADP.IsCatchableExceptionType(ex2))
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex2);
			}
		}

		internal void SetOldRecord(DataRow row, int proposedRecord)
		{
			if (!this._inDataLoad)
			{
				row.CheckInTable();
				this.CheckNotModifying(row);
			}
			if (proposedRecord == row._oldRecord)
			{
				return;
			}
			int num = row._oldRecord;
			try
			{
				if (this.LiveIndexes.Count != 0)
				{
					if (-1 == num && -1 != proposedRecord && -1 != row._newRecord && proposedRecord != row._newRecord)
					{
						num = row._newRecord;
					}
					DataViewRowState recordState = row.GetRecordState(num);
					DataViewRowState recordState2 = row.GetRecordState(proposedRecord);
					row._oldRecord = proposedRecord;
					if (proposedRecord != -1)
					{
						this._recordManager[proposedRecord] = row;
					}
					DataViewRowState recordState3 = row.GetRecordState(num);
					DataViewRowState recordState4 = row.GetRecordState(proposedRecord);
					this.RecordStateChanged(num, recordState, recordState3, proposedRecord, recordState2, recordState4);
				}
				else
				{
					row._oldRecord = proposedRecord;
					if (proposedRecord != -1)
					{
						this._recordManager[proposedRecord] = row;
					}
				}
			}
			finally
			{
				if (num != -1 && num != row._tempRecord && num != row._oldRecord && num != row._newRecord)
				{
					this.FreeRecord(ref num);
				}
				if (row.RowState == DataRowState.Detached && row.rowID != -1L)
				{
					this.RemoveRow(row, false);
				}
			}
		}

		private void RestoreShadowIndexes()
		{
			this._shadowCount--;
			if (this._shadowCount == 0)
			{
				this._shadowIndexes = null;
			}
		}

		private void SetShadowIndexes()
		{
			if (this._shadowIndexes == null)
			{
				this._shadowIndexes = this.LiveIndexes;
				this._shadowCount = 1;
				return;
			}
			this._shadowCount++;
		}

		internal void ShadowIndexCopy()
		{
			if (this._shadowIndexes == this._indexes)
			{
				this._shadowIndexes = new List<Index>(this._indexes);
			}
		}

		public override string ToString()
		{
			if (this._displayExpression != null)
			{
				return this.TableName + " + " + this.DisplayExpressionInternal;
			}
			return this.TableName;
		}

		public void BeginLoadData()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.BeginLoadData|API> {0}", this.ObjectID);
			try
			{
				if (!this._inDataLoad)
				{
					this._inDataLoad = true;
					this._loadIndex = null;
					this._initialLoad = this.Rows.Count == 0;
					if (this._initialLoad)
					{
						this.SuspendIndexEvents();
					}
					else
					{
						if (this._primaryKey != null)
						{
							this._loadIndex = this._primaryKey.Key.GetSortIndex(DataViewRowState.OriginalRows);
						}
						if (this._loadIndex != null)
						{
							this._loadIndex.AddRef();
						}
					}
					if (this.DataSet != null)
					{
						this._savedEnforceConstraints = this.DataSet.EnforceConstraints;
						this.DataSet.EnforceConstraints = false;
					}
					else
					{
						this.EnforceConstraints = false;
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void EndLoadData()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.EndLoadData|API> {0}", this.ObjectID);
			try
			{
				if (this._inDataLoad)
				{
					if (this._loadIndex != null)
					{
						this._loadIndex.RemoveRef();
					}
					if (this._loadIndexwithOriginalAdded != null)
					{
						this._loadIndexwithOriginalAdded.RemoveRef();
					}
					if (this._loadIndexwithCurrentDeleted != null)
					{
						this._loadIndexwithCurrentDeleted.RemoveRef();
					}
					this._loadIndex = null;
					this._loadIndexwithOriginalAdded = null;
					this._loadIndexwithCurrentDeleted = null;
					this._inDataLoad = false;
					this.RestoreIndexEvents(false);
					if (this.DataSet != null)
					{
						this.DataSet.EnforceConstraints = this._savedEnforceConstraints;
					}
					else
					{
						this.EnforceConstraints = true;
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public DataRow LoadDataRow(object[] values, bool fAcceptChanges)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataTable.LoadDataRow|API> {0}, fAcceptChanges={1}", this.ObjectID, fAcceptChanges);
			DataRow dataRow2;
			try
			{
				if (this._inDataLoad)
				{
					int num2 = this.NewRecordFromArray(values);
					DataRow dataRow;
					if (this._loadIndex != null)
					{
						int num3 = this._loadIndex.FindRecord(num2);
						if (num3 != -1)
						{
							int record = this._loadIndex.GetRecord(num3);
							dataRow = this._recordManager[record];
							dataRow.CancelEdit();
							if (dataRow.RowState == DataRowState.Deleted)
							{
								this.SetNewRecord(dataRow, dataRow._oldRecord, DataRowAction.Rollback, false, true, false);
							}
							this.SetNewRecord(dataRow, num2, DataRowAction.Change, false, true, false);
							if (fAcceptChanges)
							{
								dataRow.AcceptChanges();
							}
							return dataRow;
						}
					}
					dataRow = this.NewRow(num2);
					this.AddRow(dataRow);
					if (fAcceptChanges)
					{
						dataRow.AcceptChanges();
					}
					dataRow2 = dataRow;
				}
				else
				{
					DataRow dataRow = this.UpdatingAdd(values);
					if (fAcceptChanges)
					{
						dataRow.AcceptChanges();
					}
					dataRow2 = dataRow;
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataRow2;
		}

		public DataRow LoadDataRow(object[] values, LoadOption loadOption)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, LoadOption>("<ds.DataTable.LoadDataRow|API> {0}, loadOption={1}", this.ObjectID, loadOption);
			DataRow dataRow;
			try
			{
				Index index = null;
				if (this._primaryKey != null)
				{
					if (loadOption == LoadOption.Upsert)
					{
						if (this._loadIndexwithCurrentDeleted == null)
						{
							this._loadIndexwithCurrentDeleted = this._primaryKey.Key.GetSortIndex(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent);
							if (this._loadIndexwithCurrentDeleted != null)
							{
								this._loadIndexwithCurrentDeleted.AddRef();
							}
						}
						index = this._loadIndexwithCurrentDeleted;
					}
					else
					{
						if (this._loadIndexwithOriginalAdded == null)
						{
							this._loadIndexwithOriginalAdded = this._primaryKey.Key.GetSortIndex(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedOriginal);
							if (this._loadIndexwithOriginalAdded != null)
							{
								this._loadIndexwithOriginalAdded.AddRef();
							}
						}
						index = this._loadIndexwithOriginalAdded;
					}
				}
				if (this._inDataLoad && !this.AreIndexEventsSuspended)
				{
					this.SuspendIndexEvents();
				}
				dataRow = this.LoadRow(values, loadOption, index);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataRow;
		}

		internal DataRow UpdatingAdd(object[] values)
		{
			Index index = null;
			if (this._primaryKey != null)
			{
				index = this._primaryKey.Key.GetSortIndex(DataViewRowState.OriginalRows);
			}
			if (index == null)
			{
				return this.Rows.Add(values);
			}
			int num = this.NewRecordFromArray(values);
			int num2 = index.FindRecord(num);
			if (num2 != -1)
			{
				int record = index.GetRecord(num2);
				DataRow dataRow = this._recordManager[record];
				dataRow.RejectChanges();
				this.SetNewRecord(dataRow, num, DataRowAction.Change, false, true, false);
				return dataRow;
			}
			DataRow dataRow2 = this.NewRow(num);
			this.Rows.Add(dataRow2);
			return dataRow2;
		}

		internal bool UpdatingCurrent(DataRow row, DataRowAction action)
		{
			return action == DataRowAction.Add || action == DataRowAction.Change || action == DataRowAction.Rollback || action == DataRowAction.ChangeOriginal || action == DataRowAction.ChangeCurrentAndOriginal;
		}

		internal DataColumn AddUniqueKey(int position)
		{
			if (this._colUnique != null)
			{
				return this._colUnique;
			}
			DataColumn[] primaryKey = this.PrimaryKey;
			if (primaryKey.Length == 1)
			{
				return primaryKey[0];
			}
			DataColumn dataColumn = new DataColumn(XMLSchema.GenUniqueColumnName(this.TableName + "_Id", this), typeof(int), null, MappingType.Hidden);
			dataColumn.Prefix = this._tablePrefix;
			dataColumn.AutoIncrement = true;
			dataColumn.AllowDBNull = false;
			dataColumn.Unique = true;
			if (position == -1)
			{
				this.Columns.Add(dataColumn);
			}
			else
			{
				for (int i = this.Columns.Count - 1; i >= position; i--)
				{
					this.Columns[i].SetOrdinalInternal(i + 1);
				}
				this.Columns.AddAt(position, dataColumn);
				dataColumn.SetOrdinalInternal(position);
			}
			if (primaryKey.Length == 0)
			{
				this.PrimaryKey = new DataColumn[] { dataColumn };
			}
			this._colUnique = dataColumn;
			return this._colUnique;
		}

		internal DataColumn AddUniqueKey()
		{
			return this.AddUniqueKey(-1);
		}

		internal DataColumn AddForeignKey(DataColumn parentKey)
		{
			DataColumn dataColumn = new DataColumn(XMLSchema.GenUniqueColumnName(parentKey.ColumnName, this), parentKey.DataType, null, MappingType.Hidden);
			this.Columns.Add(dataColumn);
			return dataColumn;
		}

		internal void UpdatePropertyDescriptorCollectionCache()
		{
			this._propertyDescriptorCollectionCache = null;
		}

		internal PropertyDescriptorCollection GetPropertyDescriptorCollection(Attribute[] attributes)
		{
			if (this._propertyDescriptorCollectionCache == null)
			{
				int count = this.Columns.Count;
				int count2 = this.ChildRelations.Count;
				PropertyDescriptor[] array = new PropertyDescriptor[count + count2];
				for (int i = 0; i < count; i++)
				{
					array[i] = new DataColumnPropertyDescriptor(this.Columns[i]);
				}
				for (int j = 0; j < count2; j++)
				{
					array[count + j] = new DataRelationPropertyDescriptor(this.ChildRelations[j]);
				}
				this._propertyDescriptorCollectionCache = new PropertyDescriptorCollection(array);
			}
			return this._propertyDescriptorCollectionCache;
		}

		internal XmlQualifiedName TypeName
		{
			get
			{
				if (this._typeName != null)
				{
					return (XmlQualifiedName)this._typeName;
				}
				return XmlQualifiedName.Empty;
			}
			set
			{
				this._typeName = value;
			}
		}

		public void Merge(DataTable table)
		{
			this.Merge(table, false, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table, bool preserveChanges)
		{
			this.Merge(table, preserveChanges, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, int, bool, MissingSchemaAction>("<ds.DataTable.Merge|API> {0}, table={1}, preserveChanges={2}, missingSchemaAction={3}", this.ObjectID, (table != null) ? table.ObjectID : 0, preserveChanges, missingSchemaAction);
			try
			{
				if (table == null)
				{
					throw ExceptionBuilder.ArgumentNull("table");
				}
				if (missingSchemaAction - MissingSchemaAction.Add > 3)
				{
					throw ADP.InvalidMissingSchemaAction(missingSchemaAction);
				}
				new Merger(this, preserveChanges, missingSchemaAction).MergeTable(table);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void Load(IDataReader reader)
		{
			this.Load(reader, LoadOption.PreserveChanges, null);
		}

		public void Load(IDataReader reader, LoadOption loadOption)
		{
			this.Load(reader, loadOption, null);
		}

		public virtual void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, LoadOption>("<ds.DataTable.Load|API> {0}, loadOption={1}", this.ObjectID, loadOption);
			try
			{
				if (this.PrimaryKey.Length == 0)
				{
					DataTableReader dataTableReader = reader as DataTableReader;
					if (dataTableReader != null && dataTableReader.CurrentDataTable == this)
					{
						return;
					}
				}
				LoadAdapter loadAdapter = new LoadAdapter();
				loadAdapter.FillLoadOption = loadOption;
				loadAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
				if (errorHandler != null)
				{
					loadAdapter.FillError += errorHandler;
				}
				loadAdapter.FillFromReader(new DataTable[] { this }, reader, 0, 0);
				if (!reader.IsClosed && !reader.NextResult())
				{
					reader.Close();
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		private DataRow LoadRow(object[] values, LoadOption loadOption, Index searchIndex)
		{
			DataRow dataRow = null;
			int num2;
			if (searchIndex != null)
			{
				int[] array = Array.Empty<int>();
				if (this._primaryKey != null)
				{
					array = new int[this._primaryKey.ColumnsReference.Length];
					for (int i = 0; i < this._primaryKey.ColumnsReference.Length; i++)
					{
						array[i] = this._primaryKey.ColumnsReference[i].Ordinal;
					}
				}
				object[] array2 = new object[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = values[array[j]];
				}
				Range range = searchIndex.FindRecords(array2);
				if (!range.IsNull)
				{
					int num = 0;
					for (int k = range.Min; k <= range.Max; k++)
					{
						int record = searchIndex.GetRecord(k);
						dataRow = this._recordManager[record];
						num2 = this.NewRecordFromArray(values);
						for (int l = 0; l < values.Length; l++)
						{
							if (values[l] == null)
							{
								this._columnCollection[l].Copy(record, num2);
							}
						}
						for (int m = values.Length; m < this._columnCollection.Count; m++)
						{
							this._columnCollection[m].Copy(record, num2);
						}
						if (loadOption != LoadOption.Upsert || dataRow.RowState != DataRowState.Deleted)
						{
							this.SetDataRowWithLoadOption(dataRow, num2, loadOption, true);
						}
						else
						{
							num++;
						}
					}
					if (num == 0)
					{
						return dataRow;
					}
				}
			}
			num2 = this.NewRecordFromArray(values);
			dataRow = this.NewRow(num2);
			DataRowAction dataRowAction;
			if (loadOption - LoadOption.OverwriteChanges > 1)
			{
				if (loadOption != LoadOption.Upsert)
				{
					throw ExceptionBuilder.ArgumentOutOfRange("LoadOption");
				}
				dataRowAction = DataRowAction.Add;
			}
			else
			{
				dataRowAction = DataRowAction.ChangeCurrentAndOriginal;
			}
			DataRowChangeEventArgs e = this.RaiseRowChanging(null, dataRow, dataRowAction);
			this.InsertRow(dataRow, -1L, -1, false);
			if (loadOption - LoadOption.OverwriteChanges > 1)
			{
				if (loadOption != LoadOption.Upsert)
				{
					throw ExceptionBuilder.ArgumentOutOfRange("LoadOption");
				}
			}
			else
			{
				this.SetOldRecord(dataRow, num2);
			}
			this.RaiseRowChanged(e, dataRow, dataRowAction);
			return dataRow;
		}

		private void SetDataRowWithLoadOption(DataRow dataRow, int recordNo, LoadOption loadOption, bool checkReadOnly)
		{
			bool flag = false;
			if (checkReadOnly)
			{
				foreach (object obj in this.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					if (dataColumn.ReadOnly && !dataColumn.Computed)
					{
						switch (loadOption)
						{
						case LoadOption.OverwriteChanges:
							if (dataRow[dataColumn, DataRowVersion.Current] != dataColumn[recordNo] || dataRow[dataColumn, DataRowVersion.Original] != dataColumn[recordNo])
							{
								flag = true;
							}
							break;
						case LoadOption.PreserveChanges:
							if (dataRow[dataColumn, DataRowVersion.Original] != dataColumn[recordNo])
							{
								flag = true;
							}
							break;
						case LoadOption.Upsert:
							if (dataRow[dataColumn, DataRowVersion.Current] != dataColumn[recordNo])
							{
								flag = true;
							}
							break;
						}
					}
				}
			}
			DataRowChangeEventArgs e = null;
			DataRowAction dataRowAction = DataRowAction.Nothing;
			int tempRecord = dataRow._tempRecord;
			dataRow._tempRecord = recordNo;
			switch (loadOption)
			{
			case LoadOption.OverwriteChanges:
				dataRowAction = DataRowAction.ChangeCurrentAndOriginal;
				break;
			case LoadOption.PreserveChanges:
				if (dataRow.RowState == DataRowState.Unchanged)
				{
					dataRowAction = DataRowAction.ChangeCurrentAndOriginal;
				}
				else
				{
					dataRowAction = DataRowAction.ChangeOriginal;
				}
				break;
			case LoadOption.Upsert:
			{
				DataRowState rowState = dataRow.RowState;
				if (rowState != DataRowState.Unchanged)
				{
					if (rowState == DataRowState.Deleted)
					{
						break;
					}
				}
				else
				{
					using (IEnumerator enumerator = dataRow.Table.Columns.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj2 = enumerator.Current;
							DataColumn dataColumn2 = (DataColumn)obj2;
							if (dataColumn2.Compare(dataRow._newRecord, recordNo) != 0)
							{
								dataRowAction = DataRowAction.Change;
								break;
							}
						}
						break;
					}
				}
				dataRowAction = DataRowAction.Change;
				break;
			}
			default:
				throw ExceptionBuilder.ArgumentOutOfRange("LoadOption");
			}
			try
			{
				e = this.RaiseRowChanging(null, dataRow, dataRowAction);
				if (dataRowAction == DataRowAction.Nothing)
				{
					dataRow._inChangingEvent = true;
					try
					{
						e = this.OnRowChanging(e, dataRow, dataRowAction);
					}
					finally
					{
						dataRow._inChangingEvent = false;
					}
				}
			}
			finally
			{
				if (DataRowState.Detached == dataRow.RowState)
				{
					if (-1 != tempRecord)
					{
						this.FreeRecord(ref tempRecord);
					}
				}
				else if (dataRow._tempRecord != recordNo)
				{
					if (-1 != tempRecord)
					{
						this.FreeRecord(ref tempRecord);
					}
					if (-1 != recordNo)
					{
						this.FreeRecord(ref recordNo);
					}
					recordNo = dataRow._tempRecord;
				}
				else
				{
					dataRow._tempRecord = tempRecord;
				}
			}
			if (dataRow._tempRecord != -1)
			{
				dataRow.CancelEdit();
			}
			switch (loadOption)
			{
			case LoadOption.OverwriteChanges:
				this.SetNewRecord(dataRow, recordNo, DataRowAction.Change, false, false, false);
				this.SetOldRecord(dataRow, recordNo);
				break;
			case LoadOption.PreserveChanges:
				if (dataRow.RowState == DataRowState.Unchanged)
				{
					this.SetOldRecord(dataRow, recordNo);
					this.SetNewRecord(dataRow, recordNo, DataRowAction.Change, false, false, false);
				}
				else
				{
					this.SetOldRecord(dataRow, recordNo);
				}
				break;
			case LoadOption.Upsert:
				if (dataRow.RowState == DataRowState.Unchanged)
				{
					this.SetNewRecord(dataRow, recordNo, DataRowAction.Change, false, false, false);
					if (!dataRow.HasChanges())
					{
						this.SetOldRecord(dataRow, recordNo);
					}
				}
				else
				{
					if (dataRow.RowState == DataRowState.Deleted)
					{
						dataRow.RejectChanges();
					}
					this.SetNewRecord(dataRow, recordNo, DataRowAction.Change, false, false, false);
				}
				break;
			default:
				throw ExceptionBuilder.ArgumentOutOfRange("LoadOption");
			}
			if (flag)
			{
				string text = "ReadOnly Data is Modified.";
				if (dataRow.RowError.Length == 0)
				{
					dataRow.RowError = text;
				}
				else
				{
					dataRow.RowError = dataRow.RowError + " ]:[ " + text;
				}
				foreach (object obj3 in this.Columns)
				{
					DataColumn dataColumn3 = (DataColumn)obj3;
					if (dataColumn3.ReadOnly && !dataColumn3.Computed)
					{
						dataRow.SetColumnError(dataColumn3, text);
					}
				}
			}
			e = this.RaiseRowChanged(e, dataRow, dataRowAction);
			if (dataRowAction == DataRowAction.Nothing)
			{
				dataRow._inChangingEvent = true;
				try
				{
					this.OnRowChanged(e, dataRow, dataRowAction);
				}
				finally
				{
					dataRow._inChangingEvent = false;
				}
			}
		}

		public DataTableReader CreateDataReader()
		{
			return new DataTableReader(this);
		}

		public void WriteXml(Stream stream)
		{
			this.WriteXml(stream, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(Stream stream, bool writeHierarchy)
		{
			this.WriteXml(stream, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(TextWriter writer)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(TextWriter writer, bool writeHierarchy)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(XmlWriter writer)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(XmlWriter writer, bool writeHierarchy)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(string fileName)
		{
			this.WriteXml(fileName, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(string fileName, bool writeHierarchy)
		{
			this.WriteXml(fileName, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(Stream stream, XmlWriteMode mode)
		{
			this.WriteXml(stream, mode, false);
		}

		public void WriteXml(Stream stream, XmlWriteMode mode, bool writeHierarchy)
		{
			if (stream != null)
			{
				this.WriteXml(new XmlTextWriter(stream, null)
				{
					Formatting = Formatting.Indented
				}, mode, writeHierarchy);
			}
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode)
		{
			this.WriteXml(writer, mode, false);
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
			if (writer != null)
			{
				this.WriteXml(new XmlTextWriter(writer)
				{
					Formatting = Formatting.Indented
				}, mode, writeHierarchy);
			}
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode)
		{
			this.WriteXml(writer, mode, false);
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, XmlWriteMode>("<ds.DataTable.WriteXml|API> {0}, mode={1}", this.ObjectID, mode);
			try
			{
				if (this._tableName.Length == 0)
				{
					throw ExceptionBuilder.CanNotSerializeDataTableWithEmptyName();
				}
				if (writer != null)
				{
					if (mode == XmlWriteMode.DiffGram)
					{
						new NewDiffgramGen(this, writeHierarchy).Save(writer, this);
					}
					else if (mode == XmlWriteMode.WriteSchema)
					{
						DataSet dataSet = null;
						string tableNamespace = this._tableNamespace;
						if (this.DataSet == null)
						{
							dataSet = new DataSet();
							dataSet.SetLocaleValue(this._culture, this._cultureUserSet);
							dataSet.CaseSensitive = this.CaseSensitive;
							dataSet.Namespace = this.Namespace;
							dataSet.RemotingFormat = this.RemotingFormat;
							dataSet.Tables.Add(this);
						}
						if (writer != null)
						{
							new XmlDataTreeWriter(this, writeHierarchy).Save(writer, true);
						}
						if (dataSet != null)
						{
							dataSet.Tables.Remove(this);
							this._tableNamespace = tableNamespace;
						}
					}
					else
					{
						new XmlDataTreeWriter(this, writeHierarchy).Save(writer, false);
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void WriteXml(string fileName, XmlWriteMode mode)
		{
			this.WriteXml(fileName, mode, false);
		}

		public void WriteXml(string fileName, XmlWriteMode mode, bool writeHierarchy)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, string, XmlWriteMode>("<ds.DataTable.WriteXml|API> {0}, fileName='{1}', mode={2}", this.ObjectID, fileName, mode);
			try
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.WriteStartDocument(true);
					this.WriteXml(xmlTextWriter, mode, writeHierarchy);
					xmlTextWriter.WriteEndDocument();
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void WriteXmlSchema(Stream stream)
		{
			this.WriteXmlSchema(stream, false);
		}

		public void WriteXmlSchema(Stream stream, bool writeHierarchy)
		{
			if (stream == null)
			{
				return;
			}
			this.WriteXmlSchema(new XmlTextWriter(stream, null)
			{
				Formatting = Formatting.Indented
			}, writeHierarchy);
		}

		public void WriteXmlSchema(TextWriter writer)
		{
			this.WriteXmlSchema(writer, false);
		}

		public void WriteXmlSchema(TextWriter writer, bool writeHierarchy)
		{
			if (writer == null)
			{
				return;
			}
			this.WriteXmlSchema(new XmlTextWriter(writer)
			{
				Formatting = Formatting.Indented
			}, writeHierarchy);
		}

		private bool CheckForClosureOnExpressions(DataTable dt, bool writeHierarchy)
		{
			List<DataTable> list = new List<DataTable>();
			list.Add(dt);
			if (writeHierarchy)
			{
				this.CreateTableList(dt, list);
			}
			return this.CheckForClosureOnExpressionTables(list);
		}

		private bool CheckForClosureOnExpressionTables(List<DataTable> tableList)
		{
			foreach (DataTable dataTable in tableList)
			{
				foreach (object obj in dataTable.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					if (dataColumn.Expression.Length != 0)
					{
						DataColumn[] dependency = dataColumn.DataExpression.GetDependency();
						for (int i = 0; i < dependency.Length; i++)
						{
							if (!tableList.Contains(dependency[i].Table))
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		public void WriteXmlSchema(XmlWriter writer)
		{
			this.WriteXmlSchema(writer, false);
		}

		public void WriteXmlSchema(XmlWriter writer, bool writeHierarchy)
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTable.WriteXmlSchema|API> {0}", this.ObjectID);
			try
			{
				if (this._tableName.Length == 0)
				{
					throw ExceptionBuilder.CanNotSerializeDataTableWithEmptyName();
				}
				if (!this.CheckForClosureOnExpressions(this, writeHierarchy))
				{
					throw ExceptionBuilder.CanNotSerializeDataTableHierarchy();
				}
				DataSet dataSet = null;
				string tableNamespace = this._tableNamespace;
				if (this.DataSet == null)
				{
					dataSet = new DataSet();
					dataSet.SetLocaleValue(this._culture, this._cultureUserSet);
					dataSet.CaseSensitive = this.CaseSensitive;
					dataSet.Namespace = this.Namespace;
					dataSet.RemotingFormat = this.RemotingFormat;
					dataSet.Tables.Add(this);
				}
				if (writer != null)
				{
					new XmlTreeGen(SchemaFormat.Public).Save(null, this, writer, writeHierarchy);
				}
				if (dataSet != null)
				{
					dataSet.Tables.Remove(this);
					this._tableNamespace = tableNamespace;
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void WriteXmlSchema(string fileName)
		{
			this.WriteXmlSchema(fileName, false);
		}

		public void WriteXmlSchema(string fileName, bool writeHierarchy)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null);
			try
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartDocument(true);
				this.WriteXmlSchema(xmlTextWriter, writeHierarchy);
				xmlTextWriter.WriteEndDocument();
			}
			finally
			{
				xmlTextWriter.Close();
			}
		}

		public XmlReadMode ReadXml(Stream stream)
		{
			if (stream == null)
			{
				return XmlReadMode.Auto;
			}
			return this.ReadXml(new XmlTextReader(stream)
			{
				XmlResolver = null
			}, false);
		}

		public XmlReadMode ReadXml(TextReader reader)
		{
			if (reader == null)
			{
				return XmlReadMode.Auto;
			}
			return this.ReadXml(new XmlTextReader(reader)
			{
				XmlResolver = null
			}, false);
		}

		public XmlReadMode ReadXml(string fileName)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			xmlTextReader.XmlResolver = null;
			XmlReadMode xmlReadMode;
			try
			{
				xmlReadMode = this.ReadXml(xmlTextReader, false);
			}
			finally
			{
				xmlTextReader.Close();
			}
			return xmlReadMode;
		}

		public XmlReadMode ReadXml(XmlReader reader)
		{
			return this.ReadXml(reader, false);
		}

		private void RestoreConstraint(bool originalEnforceConstraint)
		{
			if (this.DataSet != null)
			{
				this.DataSet.EnforceConstraints = originalEnforceConstraint;
				return;
			}
			this.EnforceConstraints = originalEnforceConstraint;
		}

		private bool IsEmptyXml(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				if (reader.AttributeCount == 0 || (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1"))
				{
					return true;
				}
				if (reader.AttributeCount == 1)
				{
					reader.MoveToAttribute(0);
					if (this.Namespace == reader.Value && this.Prefix == reader.LocalName && reader.Prefix == "xmlns" && reader.NamespaceURI == "http://www.w3.org/2000/xmlns/")
					{
						return true;
					}
				}
			}
			return false;
		}

		internal XmlReadMode ReadXml(XmlReader reader, bool denyResolving)
		{
			IDisposable disposable = null;
			long num = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataTable.ReadXml|INFO> {0}, denyResolving={1}", this.ObjectID, denyResolving);
			XmlReadMode xmlReadMode2;
			try
			{
				disposable = TypeLimiter.EnterRestrictedScope(this);
				DataTable.RowDiffIdUsageSection rowDiffIdUsageSection = default(DataTable.RowDiffIdUsageSection);
				try
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					XmlReadMode xmlReadMode = XmlReadMode.Auto;
					rowDiffIdUsageSection.Prepare(this);
					if (reader == null)
					{
						xmlReadMode2 = xmlReadMode;
					}
					else
					{
						bool flag4;
						if (this.DataSet != null)
						{
							flag4 = this.DataSet.EnforceConstraints;
							this.DataSet.EnforceConstraints = false;
						}
						else
						{
							flag4 = this.EnforceConstraints;
							this.EnforceConstraints = false;
						}
						if (reader is XmlTextReader)
						{
							((XmlTextReader)reader).WhitespaceHandling = WhitespaceHandling.Significant;
						}
						XmlDocument xmlDocument = new XmlDocument();
						XmlDataLoader xmlDataLoader = null;
						reader.MoveToContent();
						if (this.Columns.Count == 0 && this.IsEmptyXml(reader))
						{
							reader.Read();
							xmlReadMode2 = xmlReadMode;
						}
						else
						{
							if (reader.NodeType == XmlNodeType.Element)
							{
								int depth = reader.Depth;
								if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
								{
									if (this.Columns.Count != 0)
									{
										this.ReadXmlDiffgram(reader);
										this.ReadEndElement(reader);
										this.RestoreConstraint(flag4);
										return XmlReadMode.DiffGram;
									}
									if (reader.IsEmptyElement)
									{
										reader.Read();
										return XmlReadMode.DiffGram;
									}
									throw ExceptionBuilder.DataTableInferenceNotSupported();
								}
								else
								{
									if (reader.LocalName == "Schema" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-data")
									{
										this.ReadXDRSchema(reader);
										this.RestoreConstraint(flag4);
										return XmlReadMode.ReadSchema;
									}
									if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
									{
										this.ReadXmlSchema(reader, denyResolving);
										this.RestoreConstraint(flag4);
										return XmlReadMode.ReadSchema;
									}
									if (reader.LocalName == "schema" && reader.NamespaceURI.StartsWith("http://www.w3.org/", StringComparison.Ordinal))
									{
										if (this.DataSet != null)
										{
											this.DataSet.RestoreEnforceConstraints(flag4);
										}
										else
										{
											this._enforceConstraints = flag4;
										}
										throw ExceptionBuilder.DataSetUnsupportedSchema("http://www.w3.org/2001/XMLSchema");
									}
									XmlElement xmlElement = xmlDocument.CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
									if (reader.HasAttributes)
									{
										int attributeCount = reader.AttributeCount;
										for (int i = 0; i < attributeCount; i++)
										{
											reader.MoveToAttribute(i);
											if (reader.NamespaceURI.Equals("http://www.w3.org/2000/xmlns/"))
											{
												xmlElement.SetAttribute(reader.Name, reader.GetAttribute(i));
											}
											else
											{
												XmlAttribute xmlAttribute = xmlElement.SetAttributeNode(reader.LocalName, reader.NamespaceURI);
												xmlAttribute.Prefix = reader.Prefix;
												xmlAttribute.Value = reader.GetAttribute(i);
											}
										}
									}
									reader.Read();
									while (this.MoveToElement(reader, depth))
									{
										if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
										{
											this.ReadXmlDiffgram(reader);
											this.ReadEndElement(reader);
											this.RestoreConstraint(flag4);
											return XmlReadMode.DiffGram;
										}
										if (!flag2 && !flag && reader.LocalName == "Schema" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-data")
										{
											this.ReadXDRSchema(reader);
											flag2 = true;
											flag3 = true;
										}
										else if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
										{
											this.ReadXmlSchema(reader, denyResolving);
											flag2 = true;
										}
										else
										{
											if (reader.LocalName == "schema" && reader.NamespaceURI.StartsWith("http://www.w3.org/", StringComparison.Ordinal))
											{
												if (this.DataSet != null)
												{
													this.DataSet.RestoreEnforceConstraints(flag4);
												}
												else
												{
													this._enforceConstraints = flag4;
												}
												throw ExceptionBuilder.DataSetUnsupportedSchema("http://www.w3.org/2001/XMLSchema");
											}
											if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
											{
												this.ReadXmlDiffgram(reader);
												xmlReadMode = XmlReadMode.DiffGram;
											}
											else
											{
												flag = true;
												if (!flag2 && this.Columns.Count == 0)
												{
													XmlNode xmlNode = xmlDocument.ReadNode(reader);
													xmlElement.AppendChild(xmlNode);
												}
												else
												{
													if (xmlDataLoader == null)
													{
														xmlDataLoader = new XmlDataLoader(this, flag3, xmlElement, false);
													}
													xmlDataLoader.LoadData(reader);
													xmlReadMode = (flag2 ? XmlReadMode.ReadSchema : XmlReadMode.IgnoreSchema);
												}
											}
										}
									}
									this.ReadEndElement(reader);
									xmlDocument.AppendChild(xmlElement);
									if (!flag2 && this.Columns.Count == 0)
									{
										if (this.IsEmptyXml(reader))
										{
											reader.Read();
											return xmlReadMode;
										}
										throw ExceptionBuilder.DataTableInferenceNotSupported();
									}
									else if (xmlDataLoader == null)
									{
										xmlDataLoader = new XmlDataLoader(this, flag3, false);
									}
								}
							}
							this.RestoreConstraint(flag4);
							xmlReadMode2 = xmlReadMode;
						}
					}
				}
				finally
				{
				}
			}
			finally
			{
				if (disposable != null)
				{
					disposable.Dispose();
				}
				DataCommonEventSource.Log.ExitScope(num);
			}
			return xmlReadMode2;
		}

		internal XmlReadMode ReadXml(XmlReader reader, XmlReadMode mode, bool denyResolving)
		{
			IDisposable disposable = null;
			DataTable.RowDiffIdUsageSection rowDiffIdUsageSection = default(DataTable.RowDiffIdUsageSection);
			XmlReadMode xmlReadMode2;
			try
			{
				disposable = TypeLimiter.EnterRestrictedScope(this);
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				int num = -1;
				XmlReadMode xmlReadMode = mode;
				rowDiffIdUsageSection.Prepare(this);
				if (reader == null)
				{
					xmlReadMode2 = xmlReadMode;
				}
				else
				{
					bool flag4;
					if (this.DataSet != null)
					{
						flag4 = this.DataSet.EnforceConstraints;
						this.DataSet.EnforceConstraints = false;
					}
					else
					{
						flag4 = this.EnforceConstraints;
						this.EnforceConstraints = false;
					}
					if (reader is XmlTextReader)
					{
						((XmlTextReader)reader).WhitespaceHandling = WhitespaceHandling.Significant;
					}
					XmlDocument xmlDocument = new XmlDocument();
					if (mode != XmlReadMode.Fragment && reader.NodeType == XmlNodeType.Element)
					{
						num = reader.Depth;
					}
					reader.MoveToContent();
					if (this.Columns.Count == 0 && this.IsEmptyXml(reader))
					{
						reader.Read();
						xmlReadMode2 = xmlReadMode;
					}
					else
					{
						XmlDataLoader xmlDataLoader = null;
						if (reader.NodeType == XmlNodeType.Element)
						{
							XmlElement xmlElement;
							if (mode == XmlReadMode.Fragment)
							{
								xmlDocument.AppendChild(xmlDocument.CreateElement("ds_sqlXmlWraPPeR"));
								xmlElement = xmlDocument.DocumentElement;
							}
							else
							{
								if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
								{
									if (mode == XmlReadMode.DiffGram || mode == XmlReadMode.IgnoreSchema)
									{
										if (this.Columns.Count == 0)
										{
											if (reader.IsEmptyElement)
											{
												reader.Read();
												return XmlReadMode.DiffGram;
											}
											throw ExceptionBuilder.DataTableInferenceNotSupported();
										}
										else
										{
											this.ReadXmlDiffgram(reader);
											this.ReadEndElement(reader);
										}
									}
									else
									{
										reader.Skip();
									}
									this.RestoreConstraint(flag4);
									return xmlReadMode;
								}
								if (reader.LocalName == "Schema" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-data")
								{
									if (mode != XmlReadMode.IgnoreSchema && mode != XmlReadMode.InferSchema)
									{
										this.ReadXDRSchema(reader);
									}
									else
									{
										reader.Skip();
									}
									this.RestoreConstraint(flag4);
									return xmlReadMode;
								}
								if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
								{
									if (mode != XmlReadMode.IgnoreSchema && mode != XmlReadMode.InferSchema)
									{
										this.ReadXmlSchema(reader, denyResolving);
									}
									else
									{
										reader.Skip();
									}
									this.RestoreConstraint(flag4);
									return xmlReadMode;
								}
								if (reader.LocalName == "schema" && reader.NamespaceURI.StartsWith("http://www.w3.org/", StringComparison.Ordinal))
								{
									if (this.DataSet != null)
									{
										this.DataSet.RestoreEnforceConstraints(flag4);
									}
									else
									{
										this._enforceConstraints = flag4;
									}
									throw ExceptionBuilder.DataSetUnsupportedSchema("http://www.w3.org/2001/XMLSchema");
								}
								xmlElement = xmlDocument.CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
								if (reader.HasAttributes)
								{
									int attributeCount = reader.AttributeCount;
									for (int i = 0; i < attributeCount; i++)
									{
										reader.MoveToAttribute(i);
										if (reader.NamespaceURI.Equals("http://www.w3.org/2000/xmlns/"))
										{
											xmlElement.SetAttribute(reader.Name, reader.GetAttribute(i));
										}
										else
										{
											XmlAttribute xmlAttribute = xmlElement.SetAttributeNode(reader.LocalName, reader.NamespaceURI);
											xmlAttribute.Prefix = reader.Prefix;
											xmlAttribute.Value = reader.GetAttribute(i);
										}
									}
								}
								reader.Read();
							}
							while (this.MoveToElement(reader, num))
							{
								if (reader.LocalName == "Schema" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-data")
								{
									if (!flag && !flag2 && mode != XmlReadMode.IgnoreSchema && mode != XmlReadMode.InferSchema)
									{
										this.ReadXDRSchema(reader);
										flag = true;
										flag3 = true;
									}
									else
									{
										reader.Skip();
									}
								}
								else if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
								{
									if (mode != XmlReadMode.IgnoreSchema && mode != XmlReadMode.InferSchema)
									{
										this.ReadXmlSchema(reader, denyResolving);
										flag = true;
									}
									else
									{
										reader.Skip();
									}
								}
								else if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
								{
									if (mode == XmlReadMode.DiffGram || mode == XmlReadMode.IgnoreSchema)
									{
										if (this.Columns.Count == 0)
										{
											if (reader.IsEmptyElement)
											{
												reader.Read();
												return XmlReadMode.DiffGram;
											}
											throw ExceptionBuilder.DataTableInferenceNotSupported();
										}
										else
										{
											this.ReadXmlDiffgram(reader);
											xmlReadMode = XmlReadMode.DiffGram;
										}
									}
									else
									{
										reader.Skip();
									}
								}
								else
								{
									if (reader.LocalName == "schema" && reader.NamespaceURI.StartsWith("http://www.w3.org/", StringComparison.Ordinal))
									{
										if (this.DataSet != null)
										{
											this.DataSet.RestoreEnforceConstraints(flag4);
										}
										else
										{
											this._enforceConstraints = flag4;
										}
										throw ExceptionBuilder.DataSetUnsupportedSchema("http://www.w3.org/2001/XMLSchema");
									}
									if (mode == XmlReadMode.DiffGram)
									{
										reader.Skip();
									}
									else
									{
										flag2 = true;
										if (mode == XmlReadMode.InferSchema)
										{
											XmlNode xmlNode = xmlDocument.ReadNode(reader);
											xmlElement.AppendChild(xmlNode);
										}
										else
										{
											if (this.Columns.Count == 0)
											{
												throw ExceptionBuilder.DataTableInferenceNotSupported();
											}
											if (xmlDataLoader == null)
											{
												xmlDataLoader = new XmlDataLoader(this, flag3, xmlElement, mode == XmlReadMode.IgnoreSchema);
											}
											xmlDataLoader.LoadData(reader);
										}
									}
								}
							}
							this.ReadEndElement(reader);
							xmlDocument.AppendChild(xmlElement);
							if (xmlDataLoader == null)
							{
								xmlDataLoader = new XmlDataLoader(this, flag3, mode == XmlReadMode.IgnoreSchema);
							}
							if (mode == XmlReadMode.DiffGram)
							{
								this.RestoreConstraint(flag4);
								return xmlReadMode;
							}
							if (mode == XmlReadMode.InferSchema && this.Columns.Count == 0)
							{
								throw ExceptionBuilder.DataTableInferenceNotSupported();
							}
						}
						this.RestoreConstraint(flag4);
						xmlReadMode2 = xmlReadMode;
					}
				}
			}
			finally
			{
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return xmlReadMode2;
		}

		internal void ReadEndElement(XmlReader reader)
		{
			while (reader.NodeType == XmlNodeType.Whitespace)
			{
				reader.Skip();
			}
			if (reader.NodeType == XmlNodeType.None)
			{
				reader.Skip();
				return;
			}
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				reader.ReadEndElement();
			}
		}

		internal void ReadXDRSchema(XmlReader reader)
		{
			new XmlDocument().ReadNode(reader);
		}

		internal bool MoveToElement(XmlReader reader, int depth)
		{
			while (!reader.EOF && reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.Element && reader.Depth > depth)
			{
				reader.Read();
			}
			return reader.NodeType == XmlNodeType.Element;
		}

		private void ReadXmlDiffgram(XmlReader reader)
		{
			int depth = reader.Depth;
			bool enforceConstraints = this.EnforceConstraints;
			this.EnforceConstraints = false;
			bool flag;
			DataTable dataTable;
			if (this.Rows.Count == 0)
			{
				flag = true;
				dataTable = this;
			}
			else
			{
				flag = false;
				dataTable = this.Clone();
				dataTable.EnforceConstraints = false;
			}
			dataTable.Rows._nullInList = 0;
			reader.MoveToContent();
			if (reader.LocalName != "diffgram" && reader.NamespaceURI != "urn:schemas-microsoft-com:xml-diffgram-v1")
			{
				return;
			}
			reader.Read();
			if (reader.NodeType == XmlNodeType.Whitespace)
			{
				this.MoveToElement(reader, reader.Depth - 1);
			}
			dataTable._fInLoadDiffgram = true;
			if (reader.Depth > depth)
			{
				if (reader.NamespaceURI != "urn:schemas-microsoft-com:xml-diffgram-v1" && reader.NamespaceURI != "urn:schemas-microsoft-com:xml-msdata")
				{
					XmlElement xmlElement = new XmlDocument().CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
					reader.Read();
					if (reader.Depth - 1 > depth)
					{
						new XmlDataLoader(dataTable, false, xmlElement, false)
						{
							_isDiffgram = true
						}.LoadData(reader);
					}
					this.ReadEndElement(reader);
				}
				if ((reader.LocalName == "before" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1") || (reader.LocalName == "errors" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1"))
				{
					new XMLDiffLoader().LoadDiffGram(dataTable, reader);
				}
				while (reader.Depth > depth)
				{
					reader.Read();
				}
				this.ReadEndElement(reader);
			}
			if (dataTable.Rows._nullInList > 0)
			{
				throw ExceptionBuilder.RowInsertMissing(dataTable.TableName);
			}
			dataTable._fInLoadDiffgram = false;
			List<DataTable> list = new List<DataTable>();
			list.Add(this);
			this.CreateTableList(this, list);
			for (int i = 0; i < list.Count; i++)
			{
				DataRelation[] nestedParentRelations = list[i].NestedParentRelations;
				foreach (DataRelation dataRelation in nestedParentRelations)
				{
					if (dataRelation != null && dataRelation.ParentTable == list[i])
					{
						foreach (object obj in list[i].Rows)
						{
							DataRow dataRow = (DataRow)obj;
							foreach (DataRelation dataRelation2 in nestedParentRelations)
							{
								dataRow.CheckForLoops(dataRelation2);
							}
						}
					}
				}
			}
			if (!flag)
			{
				this.Merge(dataTable);
			}
			this.EnforceConstraints = enforceConstraints;
		}

		internal void ReadXSDSchema(XmlReader reader, bool denyResolving)
		{
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			while (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				XmlSchema xmlSchema = XmlSchema.Read(reader, null);
				xmlSchemaSet.Add(xmlSchema);
				this.ReadEndElement(reader);
			}
			xmlSchemaSet.Compile();
			new XSDSchema().LoadSchema(xmlSchemaSet, this);
		}

		public void ReadXmlSchema(Stream stream)
		{
			if (stream == null)
			{
				return;
			}
			this.ReadXmlSchema(new XmlTextReader(stream), false);
		}

		public void ReadXmlSchema(TextReader reader)
		{
			if (reader == null)
			{
				return;
			}
			this.ReadXmlSchema(new XmlTextReader(reader), false);
		}

		public void ReadXmlSchema(string fileName)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			try
			{
				this.ReadXmlSchema(xmlTextReader, false);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		public void ReadXmlSchema(XmlReader reader)
		{
			this.ReadXmlSchema(reader, false);
		}

		internal void ReadXmlSchema(XmlReader reader, bool denyResolving)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataTable.ReadXmlSchema|INFO> {0}, denyResolving={1}", this.ObjectID, denyResolving);
			try
			{
				DataSet dataSet = new DataSet();
				SerializationFormat remotingFormat = this.RemotingFormat;
				dataSet.ReadXmlSchema(reader, denyResolving);
				string mainTableName = dataSet.MainTableName;
				if (!string.IsNullOrEmpty(this._tableName) || !string.IsNullOrEmpty(mainTableName))
				{
					DataTable dataTable = null;
					if (!string.IsNullOrEmpty(this._tableName))
					{
						if (!string.IsNullOrEmpty(this.Namespace))
						{
							dataTable = dataSet.Tables[this._tableName, this.Namespace];
						}
						else
						{
							int num2 = dataSet.Tables.InternalIndexOf(this._tableName);
							if (num2 > -1)
							{
								dataTable = dataSet.Tables[num2];
							}
						}
					}
					else
					{
						string text = string.Empty;
						int num3 = mainTableName.IndexOf(':');
						if (num3 > -1)
						{
							text = mainTableName.Substring(0, num3);
						}
						string text2 = mainTableName.Substring(num3 + 1, mainTableName.Length - num3 - 1);
						dataTable = dataSet.Tables[text2, text];
					}
					if (dataTable == null)
					{
						string text3 = string.Empty;
						if (!string.IsNullOrEmpty(this._tableName))
						{
							text3 = ((this.Namespace.Length > 0) ? (this.Namespace + ":" + this._tableName) : this._tableName);
						}
						else
						{
							text3 = mainTableName;
						}
						throw ExceptionBuilder.TableNotFound(text3);
					}
					dataTable._remotingFormat = remotingFormat;
					List<DataTable> list = new List<DataTable>();
					list.Add(dataTable);
					this.CreateTableList(dataTable, list);
					List<DataRelation> list2 = new List<DataRelation>();
					this.CreateRelationList(list, list2);
					if (list2.Count == 0)
					{
						if (this.Columns.Count == 0)
						{
							DataTable dataTable2 = dataTable;
							if (dataTable2 != null)
							{
								dataTable2.CloneTo(this, null, false);
							}
							if (this.DataSet == null && this._tableNamespace == null)
							{
								this._tableNamespace = dataTable2.Namespace;
							}
						}
					}
					else
					{
						if (string.IsNullOrEmpty(this.TableName))
						{
							this.TableName = dataTable.TableName;
							if (!string.IsNullOrEmpty(dataTable.Namespace))
							{
								this.Namespace = dataTable.Namespace;
							}
						}
						if (this.DataSet == null)
						{
							DataSet dataSet2 = new DataSet(dataSet.DataSetName);
							dataSet2.SetLocaleValue(dataSet.Locale, dataSet.ShouldSerializeLocale());
							dataSet2.CaseSensitive = dataSet.CaseSensitive;
							dataSet2.Namespace = dataSet.Namespace;
							dataSet2._mainTableName = dataSet._mainTableName;
							dataSet2.RemotingFormat = dataSet.RemotingFormat;
							dataSet2.Tables.Add(this);
						}
						this.CloneHierarchy(dataTable, this.DataSet, null);
						foreach (DataTable dataTable3 in list)
						{
							DataTable dataTable4 = this.DataSet.Tables[dataTable3._tableName, dataTable3.Namespace];
							foreach (object obj in dataSet.Tables[dataTable3._tableName, dataTable3.Namespace].Constraints)
							{
								ForeignKeyConstraint foreignKeyConstraint = ((Constraint)obj) as ForeignKeyConstraint;
								if (foreignKeyConstraint != null && foreignKeyConstraint.Table != foreignKeyConstraint.RelatedTable && list.Contains(foreignKeyConstraint.Table) && list.Contains(foreignKeyConstraint.RelatedTable))
								{
									ForeignKeyConstraint foreignKeyConstraint2 = (ForeignKeyConstraint)foreignKeyConstraint.Clone(dataTable4.DataSet);
									if (!dataTable4.Constraints.Contains(foreignKeyConstraint2.ConstraintName))
									{
										dataTable4.Constraints.Add(foreignKeyConstraint2);
									}
								}
							}
						}
						foreach (DataRelation dataRelation in list2)
						{
							if (!this.DataSet.Relations.Contains(dataRelation.RelationName))
							{
								this.DataSet.Relations.Add(dataRelation.Clone(this.DataSet));
							}
						}
						foreach (DataTable dataTable5 in list)
						{
							foreach (object obj2 in dataTable5.Columns)
							{
								DataColumn dataColumn = (DataColumn)obj2;
								bool flag = false;
								if (dataColumn.Expression.Length != 0)
								{
									DataColumn[] dependency = dataColumn.DataExpression.GetDependency();
									for (int i = 0; i < dependency.Length; i++)
									{
										if (!list.Contains(dependency[i].Table))
										{
											flag = true;
											break;
										}
									}
								}
								if (!flag)
								{
									this.DataSet.Tables[dataTable5.TableName, dataTable5.Namespace].Columns[dataColumn.ColumnName].Expression = dataColumn.Expression;
								}
							}
						}
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		private void CreateTableList(DataTable currentTable, List<DataTable> tableList)
		{
			foreach (object obj in currentTable.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (!tableList.Contains(dataRelation.ChildTable))
				{
					tableList.Add(dataRelation.ChildTable);
					this.CreateTableList(dataRelation.ChildTable, tableList);
				}
			}
		}

		private void CreateRelationList(List<DataTable> tableList, List<DataRelation> relationList)
		{
			foreach (DataTable dataTable in tableList)
			{
				foreach (object obj in dataTable.ChildRelations)
				{
					DataRelation dataRelation = (DataRelation)obj;
					if (tableList.Contains(dataRelation.ChildTable) && tableList.Contains(dataRelation.ParentTable))
					{
						relationList.Add(dataRelation);
					}
				}
			}
		}

		public static XmlSchemaComplexType GetDataTableSchema(XmlSchemaSet schemaSet)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
			xmlSchemaAny.MinOccurs = 1m;
			xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			return xmlSchemaComplexType;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return this.GetSchema();
		}

		protected virtual XmlSchema GetSchema()
		{
			if (base.GetType() == typeof(DataTable))
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			XmlWriter xmlWriter = new XmlTextWriter(memoryStream, null);
			if (xmlWriter != null)
			{
				new XmlTreeGen(SchemaFormat.WebService).Save(this, xmlWriter);
			}
			memoryStream.Position = 0L;
			return XmlSchema.Read(new XmlTextReader(memoryStream), null);
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			IXmlTextParser xmlTextParser = reader as IXmlTextParser;
			bool flag = true;
			if (xmlTextParser != null)
			{
				flag = xmlTextParser.Normalized;
				xmlTextParser.Normalized = false;
			}
			this.ReadXmlSerializable(reader);
			if (xmlTextParser != null)
			{
				xmlTextParser.Normalized = flag;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			this.WriteXmlSchema(writer, false);
			this.WriteXml(writer, XmlWriteMode.DiffGram, false);
		}

		protected virtual void ReadXmlSerializable(XmlReader reader)
		{
			this.ReadXml(reader, XmlReadMode.DiffGram, true);
		}

		internal Hashtable RowDiffId
		{
			get
			{
				if (this._rowDiffId == null)
				{
					this._rowDiffId = new Hashtable();
				}
				return this._rowDiffId;
			}
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		internal void AddDependentColumn(DataColumn expressionColumn)
		{
			if (this._dependentColumns == null)
			{
				this._dependentColumns = new List<DataColumn>();
			}
			if (!this._dependentColumns.Contains(expressionColumn))
			{
				this._dependentColumns.Add(expressionColumn);
			}
		}

		internal void RemoveDependentColumn(DataColumn expressionColumn)
		{
			if (this._dependentColumns != null && this._dependentColumns.Contains(expressionColumn))
			{
				this._dependentColumns.Remove(expressionColumn);
			}
		}

		internal void EvaluateExpressions()
		{
			if (this._dependentColumns != null && 0 < this._dependentColumns.Count)
			{
				foreach (object obj in this.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					if (dataRow._oldRecord != -1 && dataRow._oldRecord != dataRow._newRecord)
					{
						this.EvaluateDependentExpressions(this._dependentColumns, dataRow, DataRowVersion.Original, null);
					}
					if (dataRow._newRecord != -1)
					{
						this.EvaluateDependentExpressions(this._dependentColumns, dataRow, DataRowVersion.Current, null);
					}
					if (dataRow._tempRecord != -1)
					{
						this.EvaluateDependentExpressions(this._dependentColumns, dataRow, DataRowVersion.Proposed, null);
					}
				}
			}
		}

		internal void EvaluateExpressions(DataRow row, DataRowAction action, List<DataRow> cachedRows)
		{
			if (action == DataRowAction.Add || action == DataRowAction.Change || (action == DataRowAction.Rollback && (row._oldRecord != -1 || row._newRecord != -1)))
			{
				if (row._oldRecord != -1 && row._oldRecord != row._newRecord)
				{
					this.EvaluateDependentExpressions(this._dependentColumns, row, DataRowVersion.Original, cachedRows);
				}
				if (row._newRecord != -1)
				{
					this.EvaluateDependentExpressions(this._dependentColumns, row, DataRowVersion.Current, cachedRows);
				}
				if (row._tempRecord != -1)
				{
					this.EvaluateDependentExpressions(this._dependentColumns, row, DataRowVersion.Proposed, cachedRows);
				}
				return;
			}
			if ((action == DataRowAction.Delete || (action == DataRowAction.Rollback && row._oldRecord == -1 && row._newRecord == -1)) && this._dependentColumns != null)
			{
				foreach (DataColumn dataColumn in this._dependentColumns)
				{
					if (dataColumn.DataExpression != null && dataColumn.DataExpression.HasLocalAggregate() && dataColumn.Table == this)
					{
						for (int i = 0; i < this.Rows.Count; i++)
						{
							DataRow dataRow = this.Rows[i];
							if (dataRow._oldRecord != -1 && dataRow._oldRecord != dataRow._newRecord)
							{
								this.EvaluateDependentExpressions(this._dependentColumns, dataRow, DataRowVersion.Original, null);
							}
						}
						for (int j = 0; j < this.Rows.Count; j++)
						{
							DataRow dataRow2 = this.Rows[j];
							if (dataRow2._tempRecord != -1)
							{
								this.EvaluateDependentExpressions(this._dependentColumns, dataRow2, DataRowVersion.Proposed, null);
							}
						}
						for (int k = 0; k < this.Rows.Count; k++)
						{
							DataRow dataRow3 = this.Rows[k];
							if (dataRow3._newRecord != -1)
							{
								this.EvaluateDependentExpressions(this._dependentColumns, dataRow3, DataRowVersion.Current, null);
							}
						}
						break;
					}
				}
				if (cachedRows != null)
				{
					foreach (DataRow dataRow4 in cachedRows)
					{
						if (dataRow4._oldRecord != -1 && dataRow4._oldRecord != dataRow4._newRecord)
						{
							dataRow4.Table.EvaluateDependentExpressions(dataRow4.Table._dependentColumns, dataRow4, DataRowVersion.Original, null);
						}
						if (dataRow4._newRecord != -1)
						{
							dataRow4.Table.EvaluateDependentExpressions(dataRow4.Table._dependentColumns, dataRow4, DataRowVersion.Current, null);
						}
						if (dataRow4._tempRecord != -1)
						{
							dataRow4.Table.EvaluateDependentExpressions(dataRow4.Table._dependentColumns, dataRow4, DataRowVersion.Proposed, null);
						}
					}
				}
			}
		}

		internal void EvaluateExpressions(DataColumn column)
		{
			int count = column._table.Rows.Count;
			if (column.DataExpression.IsTableAggregate() && count > 0)
			{
				object obj = column.DataExpression.Evaluate();
				for (int i = 0; i < count; i++)
				{
					DataRow dataRow = column._table.Rows[i];
					if (dataRow._oldRecord != -1 && dataRow._oldRecord != dataRow._newRecord)
					{
						column[dataRow._oldRecord] = obj;
					}
					if (dataRow._newRecord != -1)
					{
						column[dataRow._newRecord] = obj;
					}
					if (dataRow._tempRecord != -1)
					{
						column[dataRow._tempRecord] = obj;
					}
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					DataRow dataRow2 = column._table.Rows[j];
					if (dataRow2._oldRecord != -1 && dataRow2._oldRecord != dataRow2._newRecord)
					{
						column[dataRow2._oldRecord] = column.DataExpression.Evaluate(dataRow2, DataRowVersion.Original);
					}
					if (dataRow2._newRecord != -1)
					{
						column[dataRow2._newRecord] = column.DataExpression.Evaluate(dataRow2, DataRowVersion.Current);
					}
					if (dataRow2._tempRecord != -1)
					{
						column[dataRow2._tempRecord] = column.DataExpression.Evaluate(dataRow2, DataRowVersion.Proposed);
					}
				}
			}
			column.Table.ResetInternalIndexes(column);
			this.EvaluateDependentExpressions(column);
		}

		internal void EvaluateDependentExpressions(DataColumn column)
		{
			if (column._dependentColumns != null)
			{
				foreach (DataColumn dataColumn in column._dependentColumns)
				{
					if (dataColumn._table != null && column != dataColumn)
					{
						this.EvaluateExpressions(dataColumn);
					}
				}
			}
		}

		internal void EvaluateDependentExpressions(List<DataColumn> columns, DataRow row, DataRowVersion version, List<DataRow> cachedRows)
		{
			if (columns == null)
			{
				return;
			}
			int num = columns.Count;
			for (int i = 0; i < num; i++)
			{
				if (columns[i].Table == this)
				{
					DataColumn dataColumn = columns[i];
					if (dataColumn.DataExpression != null && dataColumn.DataExpression.HasLocalAggregate())
					{
						DataRowVersion dataRowVersion = ((version == DataRowVersion.Proposed) ? DataRowVersion.Default : version);
						bool flag = dataColumn.DataExpression.IsTableAggregate();
						object obj = null;
						if (flag)
						{
							obj = dataColumn.DataExpression.Evaluate(row, dataRowVersion);
						}
						for (int j = 0; j < this.Rows.Count; j++)
						{
							DataRow dataRow = this.Rows[j];
							if (dataRow.RowState != DataRowState.Deleted && (dataRowVersion != DataRowVersion.Original || (dataRow._oldRecord != -1 && dataRow._oldRecord != dataRow._newRecord)))
							{
								if (!flag)
								{
									obj = dataColumn.DataExpression.Evaluate(dataRow, dataRowVersion);
								}
								this.SilentlySetValue(dataRow, dataColumn, dataRowVersion, obj);
							}
						}
					}
					else if (row.RowState != DataRowState.Deleted && (version != DataRowVersion.Original || (row._oldRecord != -1 && row._oldRecord != row._newRecord)))
					{
						this.SilentlySetValue(row, dataColumn, version, (dataColumn.DataExpression == null) ? dataColumn.DefaultValue : dataColumn.DataExpression.Evaluate(row, version));
					}
				}
			}
			num = columns.Count;
			for (int k = 0; k < num; k++)
			{
				DataColumn dataColumn2 = columns[k];
				if (dataColumn2.Table != this || (dataColumn2.DataExpression != null && !dataColumn2.DataExpression.HasLocalAggregate()))
				{
					DataRowVersion dataRowVersion2 = ((version == DataRowVersion.Proposed) ? DataRowVersion.Default : version);
					if (cachedRows != null)
					{
						foreach (DataRow dataRow2 in cachedRows)
						{
							if (dataRow2.Table == dataColumn2.Table && (dataRowVersion2 != DataRowVersion.Original || dataRow2._newRecord != dataRow2._oldRecord) && dataRow2 != null && dataRow2.RowState != DataRowState.Deleted && (version != DataRowVersion.Original || dataRow2._oldRecord != -1))
							{
								object obj2 = dataColumn2.DataExpression.Evaluate(dataRow2, dataRowVersion2);
								this.SilentlySetValue(dataRow2, dataColumn2, dataRowVersion2, obj2);
							}
						}
					}
					for (int l = 0; l < this.ParentRelations.Count; l++)
					{
						DataRelation dataRelation = this.ParentRelations[l];
						if (dataRelation.ParentTable == dataColumn2.Table)
						{
							foreach (DataRow dataRow3 in row.GetParentRows(dataRelation, version))
							{
								if ((cachedRows == null || !cachedRows.Contains(dataRow3)) && (dataRowVersion2 != DataRowVersion.Original || dataRow3._newRecord != dataRow3._oldRecord) && dataRow3 != null && dataRow3.RowState != DataRowState.Deleted && (version != DataRowVersion.Original || dataRow3._oldRecord != -1))
								{
									object obj3 = dataColumn2.DataExpression.Evaluate(dataRow3, dataRowVersion2);
									this.SilentlySetValue(dataRow3, dataColumn2, dataRowVersion2, obj3);
								}
							}
						}
					}
					for (int n = 0; n < this.ChildRelations.Count; n++)
					{
						DataRelation dataRelation2 = this.ChildRelations[n];
						if (dataRelation2.ChildTable == dataColumn2.Table)
						{
							foreach (DataRow dataRow4 in row.GetChildRows(dataRelation2, version))
							{
								if ((cachedRows == null || !cachedRows.Contains(dataRow4)) && (dataRowVersion2 != DataRowVersion.Original || dataRow4._newRecord != dataRow4._oldRecord) && dataRow4 != null && dataRow4.RowState != DataRowState.Deleted && (version != DataRowVersion.Original || dataRow4._oldRecord != -1))
								{
									object obj4 = dataColumn2.DataExpression.Evaluate(dataRow4, dataRowVersion2);
									this.SilentlySetValue(dataRow4, dataColumn2, dataRowVersion2, obj4);
								}
							}
						}
					}
				}
			}
		}

		private DataSet _dataSet;

		private DataView _defaultView;

		internal long _nextRowID;

		internal readonly DataRowCollection _rowCollection;

		internal readonly DataColumnCollection _columnCollection;

		private readonly ConstraintCollection _constraintCollection;

		private int _elementColumnCount;

		internal DataRelationCollection _parentRelationsCollection;

		internal DataRelationCollection _childRelationsCollection;

		internal readonly RecordManager _recordManager;

		internal readonly List<Index> _indexes;

		private List<Index> _shadowIndexes;

		private int _shadowCount;

		internal PropertyCollection _extendedProperties;

		private string _tableName = string.Empty;

		internal string _tableNamespace;

		private string _tablePrefix = string.Empty;

		internal DataExpression _displayExpression;

		internal bool _fNestedInDataset = true;

		private CultureInfo _culture;

		private bool _cultureUserSet;

		private CompareInfo _compareInfo;

		private CompareOptions _compareFlags = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

		private IFormatProvider _formatProvider;

		private StringComparer _hashCodeProvider;

		private bool _caseSensitive;

		private bool _caseSensitiveUserSet;

		internal string _encodedTableName;

		internal DataColumn _xmlText;

		internal DataColumn _colUnique;

		internal bool _textOnly;

		internal decimal _minOccurs = 1m;

		internal decimal _maxOccurs = 1m;

		internal bool _repeatableElement;

		private object _typeName;

		internal UniqueConstraint _primaryKey;

		internal IndexField[] _primaryIndex = Array.Empty<IndexField>();

		private DataColumn[] _delayedSetPrimaryKey;

		private Index _loadIndex;

		private Index _loadIndexwithOriginalAdded;

		private Index _loadIndexwithCurrentDeleted;

		private int _suspendIndexEvents;

		private bool _savedEnforceConstraints;

		private bool _inDataLoad;

		private bool _initialLoad;

		private bool _schemaLoading;

		private bool _enforceConstraints = true;

		internal bool _suspendEnforceConstraints;

		protected internal bool fInitInProgress;

		private bool _inLoad;

		internal bool _fInLoadDiffgram;

		private byte _isTypedDataTable;

		private DataRow[] _emptyDataRowArray;

		private PropertyDescriptorCollection _propertyDescriptorCollectionCache;

		private DataRelation[] _nestedParentRelations = Array.Empty<DataRelation>();

		internal List<DataColumn> _dependentColumns;

		private bool _mergingData;

		private DataRowChangeEventHandler _onRowChangedDelegate;

		private DataRowChangeEventHandler _onRowChangingDelegate;

		private DataRowChangeEventHandler _onRowDeletingDelegate;

		private DataRowChangeEventHandler _onRowDeletedDelegate;

		private DataColumnChangeEventHandler _onColumnChangedDelegate;

		private DataColumnChangeEventHandler _onColumnChangingDelegate;

		private DataTableClearEventHandler _onTableClearingDelegate;

		private DataTableClearEventHandler _onTableClearedDelegate;

		private DataTableNewRowEventHandler _onTableNewRowDelegate;

		private PropertyChangedEventHandler _onPropertyChangingDelegate;

		private EventHandler _onInitialized;

		private readonly DataRowBuilder _rowBuilder;

		private const string KEY_XMLSCHEMA = "XmlSchema";

		private const string KEY_XMLDIFFGRAM = "XmlDiffGram";

		private const string KEY_NAME = "TableName";

		internal readonly List<DataView> _delayedViews = new List<DataView>();

		private readonly List<DataViewListener> _dataViewListeners = new List<DataViewListener>();

		internal Hashtable _rowDiffId;

		internal readonly ReaderWriterLockSlim _indexesLock = new ReaderWriterLockSlim();

		internal int _ukColumnPositionForInference = -1;

		private SerializationFormat _remotingFormat;

		private static int s_objectTypeCount;

		private readonly int _objectID = Interlocked.Increment(ref DataTable.s_objectTypeCount);

		internal struct RowDiffIdUsageSection
		{
			internal void Prepare(DataTable table)
			{
				this._targetTable = table;
				table._rowDiffId = null;
			}

			[Conditional("DEBUG")]
			internal void Cleanup()
			{
				if (this._targetTable != null)
				{
					this._targetTable._rowDiffId = null;
				}
			}

			[Conditional("DEBUG")]
			internal static void Assert(string message)
			{
			}

			private DataTable _targetTable;
		}

		internal struct DSRowDiffIdUsageSection
		{
			internal void Prepare(DataSet ds)
			{
				this._targetDS = ds;
				for (int i = 0; i < ds.Tables.Count; i++)
				{
					ds.Tables[i]._rowDiffId = null;
				}
			}

			[Conditional("DEBUG")]
			internal void Cleanup()
			{
				if (this._targetDS != null)
				{
					for (int i = 0; i < this._targetDS.Tables.Count; i++)
					{
						this._targetDS.Tables[i]._rowDiffId = null;
					}
				}
			}

			private DataSet _targetDS;
		}
	}
}
