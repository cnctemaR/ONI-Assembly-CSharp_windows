using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public class DataAdapter : Component, IDataAdapter
	{
		protected DataAdapter()
		{
			this.acceptChangesDuringFill = true;
			this.continueUpdateOnError = false;
			this.missingMappingAction = MissingMappingAction.Passthrough;
			this.missingSchemaAction = MissingSchemaAction.Add;
			this.tableMappings = new DataTableMappingCollection();
			this.acceptChangesDuringUpdate = true;
			this.fillLoadOption = LoadOption.OverwriteChanges;
			this.returnProviderSpecificTypes = false;
		}

		protected DataAdapter(DataAdapter adapter)
		{
			this.AcceptChangesDuringFill = adapter.AcceptChangesDuringFill;
			this.ContinueUpdateOnError = adapter.ContinueUpdateOnError;
			this.MissingMappingAction = adapter.MissingMappingAction;
			this.MissingSchemaAction = adapter.MissingSchemaAction;
			if (adapter.tableMappings != null)
			{
				foreach (object obj in adapter.TableMappings)
				{
					ICloneable cloneable = (ICloneable)obj;
					this.TableMappings.Add(cloneable.Clone());
				}
			}
			this.acceptChangesDuringUpdate = adapter.AcceptChangesDuringUpdate;
			this.fillLoadOption = adapter.FillLoadOption;
			this.returnProviderSpecificTypes = adapter.ReturnProviderSpecificTypes;
		}

		public event FillErrorEventHandler FillError;

		ITableMappingCollection IDataAdapter.TableMappings
		{
			get
			{
				return this.TableMappings;
			}
		}

		[DataCategory("Fill")]
		[DefaultValue(true)]
		public bool AcceptChangesDuringFill
		{
			get
			{
				return this.acceptChangesDuringFill;
			}
			set
			{
				this.acceptChangesDuringFill = value;
			}
		}

		[DefaultValue(true)]
		public bool AcceptChangesDuringUpdate
		{
			get
			{
				return this.acceptChangesDuringUpdate;
			}
			set
			{
				this.acceptChangesDuringUpdate = value;
			}
		}

		[DataCategory("Update")]
		[DefaultValue(false)]
		public bool ContinueUpdateOnError
		{
			get
			{
				return this.continueUpdateOnError;
			}
			set
			{
				this.continueUpdateOnError = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public LoadOption FillLoadOption
		{
			get
			{
				return this.fillLoadOption;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(LoadOption), value);
				this.fillLoadOption = value;
			}
		}

		[DefaultValue(MissingMappingAction.Passthrough)]
		[DataCategory("Mapping")]
		public MissingMappingAction MissingMappingAction
		{
			get
			{
				return this.missingMappingAction;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(MissingMappingAction), value);
				this.missingMappingAction = value;
			}
		}

		[DefaultValue(MissingSchemaAction.Add)]
		[DataCategory("Mapping")]
		public MissingSchemaAction MissingSchemaAction
		{
			get
			{
				return this.missingSchemaAction;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(MissingSchemaAction), value);
				this.missingSchemaAction = value;
			}
		}

		[DefaultValue(false)]
		public virtual bool ReturnProviderSpecificTypes
		{
			get
			{
				return this.returnProviderSpecificTypes;
			}
			set
			{
				this.returnProviderSpecificTypes = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DataCategory("Mapping")]
		public DataTableMappingCollection TableMappings
		{
			get
			{
				return this.tableMappings;
			}
		}

		[MonoTODO]
		[Obsolete("Use the protected constructor instead", false)]
		protected virtual DataAdapter CloneInternals()
		{
			throw new NotImplementedException();
		}

		protected virtual DataTableMappingCollection CreateTableMappings()
		{
			return new DataTableMappingCollection();
		}

		[MonoTODO]
		protected override void Dispose(bool disposing)
		{
			throw new NotImplementedException();
		}

		protected virtual bool ShouldSerializeTableMappings()
		{
			return true;
		}

		internal int FillInternal(DataTable dataTable, IDataReader dataReader)
		{
			if (dataReader.FieldCount == 0)
			{
				dataReader.Close();
				return 0;
			}
			int num = 0;
			try
			{
				string text = this.SetupSchema(SchemaType.Mapped, dataTable.TableName);
				if (text != null)
				{
					dataTable.TableName = text;
					this.FillTable(dataTable, dataReader, 0, 0, ref num);
				}
			}
			finally
			{
				dataReader.Close();
			}
			return num;
		}

		internal int[] BuildSchema(IDataReader reader, DataTable table, SchemaType schemaType)
		{
			return DataAdapter.BuildSchema(reader, table, schemaType, this.MissingSchemaAction, this.MissingMappingAction, this.TableMappings);
		}

		internal static int[] BuildSchema(IDataReader reader, DataTable table, SchemaType schemaType, MissingSchemaAction missingSchAction, MissingMappingAction missingMapAction, DataTableMappingCollection dtMapping)
		{
			int num = 0;
			int[] array = new int[table.Columns.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = -1;
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			bool flag = true;
			DataTable schemaTable = reader.GetSchemaTable();
			DataColumn dataColumn = schemaTable.Columns["ColumnName"];
			DataColumn dataColumn2 = schemaTable.Columns["DataType"];
			DataColumn dataColumn3 = schemaTable.Columns["IsAutoIncrement"];
			DataColumn dataColumn4 = schemaTable.Columns["AllowDBNull"];
			DataColumn dataColumn5 = schemaTable.Columns["IsReadOnly"];
			DataColumn dataColumn6 = schemaTable.Columns["IsKey"];
			DataColumn dataColumn7 = schemaTable.Columns["IsUnique"];
			DataColumn dataColumn8 = schemaTable.Columns["ColumnSize"];
			foreach (object obj in schemaTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				string text;
				string text2;
				if (dataColumn == null || dataRow.IsNull(dataColumn) || (string)dataRow[dataColumn] == string.Empty)
				{
					text = "Column";
					text2 = "Column1";
				}
				else
				{
					text = (string)dataRow[dataColumn];
					text2 = text;
				}
				int num2 = 1;
				while (arrayList2.Contains(text2))
				{
					text2 = string.Format("{0}{1}", text, num2);
					num2++;
				}
				arrayList2.Add(text2);
				int num3 = dtMapping.IndexOfDataSetTable(table.TableName);
				string text3 = ((num3 == -1) ? table.TableName : dtMapping[num3].SourceTable);
				DataTableMapping tableMappingBySchemaAction = DataTableMappingCollection.GetTableMappingBySchemaAction(dtMapping, text3, table.TableName, missingMapAction);
				if (tableMappingBySchemaAction != null)
				{
					table.TableName = tableMappingBySchemaAction.DataSetTable;
					DataColumnMapping columnMappingBySchemaAction = DataColumnMappingCollection.GetColumnMappingBySchemaAction(tableMappingBySchemaAction.ColumnMappings, text2, missingMapAction);
					if (columnMappingBySchemaAction != null)
					{
						Type type = dataRow[dataColumn2] as Type;
						DataColumn dataColumn9 = ((type == null) ? null : columnMappingBySchemaAction.GetDataColumnBySchemaAction(table, type, missingSchAction));
						if (dataColumn9 != null)
						{
							if (table.Columns.IndexOf(dataColumn9) == -1)
							{
								if (missingSchAction == MissingSchemaAction.Add || missingSchAction == MissingSchemaAction.AddWithKey)
								{
									table.Columns.Add(dataColumn9);
								}
								int[] array2 = new int[array.Length + 1];
								Array.Copy(array, 0, array2, 0, dataColumn9.Ordinal);
								Array.Copy(array, dataColumn9.Ordinal, array2, dataColumn9.Ordinal + 1, array.Length - dataColumn9.Ordinal);
								array = array2;
							}
							if (missingSchAction == MissingSchemaAction.AddWithKey)
							{
								object obj2 = ((dataColumn4 == null) ? null : dataRow[dataColumn4]);
								bool flag2 = !(obj2 is bool) || (bool)obj2;
								obj2 = ((dataColumn6 == null) ? null : dataRow[dataColumn6]);
								bool flag3 = obj2 is bool && (bool)obj2;
								obj2 = ((dataColumn3 == null) ? null : dataRow[dataColumn3]);
								bool flag4 = obj2 is bool && (bool)obj2;
								obj2 = ((dataColumn5 == null) ? null : dataRow[dataColumn5]);
								bool flag5 = obj2 is bool && (bool)obj2;
								obj2 = ((dataColumn7 == null) ? null : dataRow[dataColumn7]);
								bool flag6 = obj2 is bool && (bool)obj2;
								dataColumn9.AllowDBNull = flag2;
								if (flag4 && DataColumn.CanAutoIncrement(type))
								{
									dataColumn9.AutoIncrement = true;
									if (!flag2)
									{
										dataColumn9.AllowDBNull = false;
									}
								}
								if (type == DbTypes.TypeOfString)
								{
									dataColumn9.MaxLength = ((dataColumn8 == null) ? 0 : ((int)dataRow[dataColumn8]));
								}
								if (flag5)
								{
									dataColumn9.ReadOnly = true;
								}
								if (!flag2 && (!flag5 || flag3))
								{
									dataColumn9.AllowDBNull = false;
								}
								if (flag6 && !flag3 && !type.IsArray)
								{
									dataColumn9.Unique = true;
									if (!flag2)
									{
										dataColumn9.AllowDBNull = false;
									}
								}
								bool flag7 = false;
								if (schemaTable.Columns.Contains("IsHidden"))
								{
									obj2 = dataRow["IsHidden"];
									flag7 = obj2 is bool && (bool)obj2;
								}
								if (flag3 && !flag7)
								{
									arrayList.Add(dataColumn9);
									if (flag2)
									{
										flag = false;
									}
								}
							}
							array[dataColumn9.Ordinal] = num++;
						}
					}
				}
			}
			if (arrayList.Count > 0)
			{
				DataColumn[] array3 = (DataColumn[])arrayList.ToArray(typeof(DataColumn));
				if (flag)
				{
					table.PrimaryKey = array3;
				}
				else
				{
					UniqueConstraint uniqueConstraint = new UniqueConstraint(array3);
					for (int j = 0; j < table.Constraints.Count; j++)
					{
						if (table.Constraints[j].Equals(uniqueConstraint))
						{
							uniqueConstraint = null;
							break;
						}
					}
					if (uniqueConstraint != null)
					{
						table.Constraints.Add(uniqueConstraint);
					}
				}
			}
			return array;
		}

		internal bool FillTable(DataTable dataTable, IDataReader dataReader, int startRecord, int maxRecords, ref int counter)
		{
			if (dataReader.FieldCount == 0)
			{
				return false;
			}
			int num = counter;
			int[] array = this.BuildSchema(dataReader, dataTable, SchemaType.Mapped);
			int[] array2 = new int[array.Length];
			int num2 = array2.Length;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array[i] >= 0)
				{
					array2[array[i]] = i;
				}
				else
				{
					array2[--num2] = i;
				}
			}
			for (int j = 0; j < startRecord; j++)
			{
				dataReader.Read();
			}
			dataTable.BeginLoadData();
			while (dataReader.Read() && (maxRecords == 0 || counter - num < maxRecords))
			{
				try
				{
					dataTable.LoadDataRow(dataReader, array2, num2, this.AcceptChangesDuringFill);
					counter++;
				}
				catch (Exception ex)
				{
					object[] array3 = new object[dataReader.FieldCount];
					object[] array4 = new object[array.Length];
					dataReader.GetValues(array3);
					for (int k = 0; k < array.Length; k++)
					{
						if (array[k] >= 0)
						{
							array4[k] = array3[array[k]];
						}
					}
					FillErrorEventArgs e = this.CreateFillErrorEvent(dataTable, array4, ex);
					this.OnFillErrorInternal(e);
					if (!e.Continue)
					{
						throw ex;
					}
				}
			}
			dataTable.EndLoadData();
			return true;
		}

		internal virtual void OnFillErrorInternal(FillErrorEventArgs value)
		{
			this.OnFillError(value);
		}

		internal FillErrorEventArgs CreateFillErrorEvent(DataTable dataTable, object[] values, Exception e)
		{
			return new FillErrorEventArgs(dataTable, values)
			{
				Errors = e,
				Continue = false
			};
		}

		internal string SetupSchema(SchemaType schemaType, string sourceTableName)
		{
			if (schemaType != SchemaType.Mapped)
			{
				return sourceTableName;
			}
			DataTableMapping tableMappingBySchemaAction = DataTableMappingCollection.GetTableMappingBySchemaAction(this.TableMappings, sourceTableName, sourceTableName, this.MissingMappingAction);
			if (tableMappingBySchemaAction != null)
			{
				return tableMappingBySchemaAction.DataSetTable;
			}
			return null;
		}

		internal int FillInternal(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("DataSet");
			}
			if (startRecord < 0)
			{
				throw new ArgumentException("The startRecord parameter was less than 0.");
			}
			if (maxRecords < 0)
			{
				throw new ArgumentException("The maxRecords parameter was less than 0.");
			}
			int num = 0;
			int num2 = 0;
			try
			{
				string text = srcTable;
				do
				{
					if (dataReader.FieldCount != -1)
					{
						text = this.SetupSchema(SchemaType.Mapped, text);
						if (text != null)
						{
							DataTable dataTable;
							if (dataSet.Tables.Contains(text))
							{
								dataTable = dataSet.Tables[text];
							}
							else
							{
								if (this.MissingSchemaAction == MissingSchemaAction.Ignore)
								{
									goto IL_00CF;
								}
								dataTable = dataSet.Tables.Add(text);
							}
							if (this.FillTable(dataTable, dataReader, startRecord, maxRecords, ref num2))
							{
								text = string.Format("{0}{1}", srcTable, ++num);
								startRecord = 0;
								maxRecords = 0;
							}
						}
					}
					IL_00CF:;
				}
				while (dataReader.NextResult());
			}
			finally
			{
				dataReader.Close();
			}
			return num2;
		}

		public virtual int Fill(DataSet dataSet)
		{
			throw new NotSupportedException();
		}

		protected virtual int Fill(DataTable dataTable, IDataReader dataReader)
		{
			return this.FillInternal(dataTable, dataReader);
		}

		protected virtual int Fill(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords)
		{
			int num = 0;
			if (dataReader.IsClosed)
			{
				return 0;
			}
			if (startRecord < 0)
			{
				throw new ArgumentException("The startRecord parameter was less than 0.");
			}
			if (maxRecords < 0)
			{
				throw new ArgumentException("The maxRecords parameter was less than 0.");
			}
			try
			{
				foreach (DataTable dataTable in dataTables)
				{
					string text = this.SetupSchema(SchemaType.Mapped, dataTable.TableName);
					if (text != null)
					{
						dataTable.TableName = text;
						this.FillTable(dataTable, dataReader, 0, 0, ref num);
					}
				}
			}
			finally
			{
				dataReader.Close();
			}
			return num;
		}

		protected virtual int Fill(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			return this.FillInternal(dataSet, srcTable, dataReader, startRecord, maxRecords);
		}

		[MonoTODO]
		protected virtual DataTable FillSchema(DataTable dataTable, SchemaType schemaType, IDataReader dataReader)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, string srcTable, IDataReader dataReader)
		{
			throw new NotImplementedException();
		}

		public virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			throw new NotSupportedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[MonoTODO]
		public virtual IDataParameter[] GetFillParameters()
		{
			throw new NotImplementedException();
		}

		protected bool HasTableMappings()
		{
			return this.TableMappings.Count != 0;
		}

		protected virtual void OnFillError(FillErrorEventArgs value)
		{
			if (this.FillError != null)
			{
				this.FillError(this, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetFillLoadOption()
		{
			this.FillLoadOption = LoadOption.OverwriteChanges;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializeAcceptChangesDuringFill()
		{
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializeFillLoadOption()
		{
			return false;
		}

		[MonoTODO]
		public virtual int Update(DataSet dataSet)
		{
			throw new NotImplementedException();
		}

		private const string DefaultSourceTableName = "Table";

		private const string DefaultSourceColumnName = "Column";

		private bool acceptChangesDuringFill;

		private bool continueUpdateOnError;

		private MissingMappingAction missingMappingAction;

		private MissingSchemaAction missingSchemaAction;

		private DataTableMappingCollection tableMappings;

		private bool acceptChangesDuringUpdate;

		private LoadOption fillLoadOption;

		private bool returnProviderSpecificTypes;
	}
}
