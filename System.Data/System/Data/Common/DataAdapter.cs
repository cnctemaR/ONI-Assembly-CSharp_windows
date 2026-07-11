using System;
using System.ComponentModel;
using System.Data.ProviderBase;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace System.Data.Common
{
	public class DataAdapter : Component, IDataAdapter
	{
		[Conditional("DEBUG")]
		private void AssertReaderHandleFieldCount(DataReaderContainer readerHandler)
		{
		}

		[Conditional("DEBUG")]
		private void AssertSchemaMapping(SchemaMapping mapping)
		{
		}

		protected DataAdapter()
		{
			GC.SuppressFinalize(this);
		}

		protected DataAdapter(DataAdapter from)
		{
			this.CloneFrom(from);
		}

		[DefaultValue(true)]
		public bool AcceptChangesDuringFill
		{
			get
			{
				return this._acceptChangesDuringFill;
			}
			set
			{
				this._acceptChangesDuringFill = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializeAcceptChangesDuringFill()
		{
			return this._fillLoadOption == (LoadOption)0;
		}

		[DefaultValue(true)]
		public bool AcceptChangesDuringUpdate
		{
			get
			{
				return this._acceptChangesDuringUpdate;
			}
			set
			{
				this._acceptChangesDuringUpdate = value;
			}
		}

		[DefaultValue(false)]
		public bool ContinueUpdateOnError
		{
			get
			{
				return this._continueUpdateOnError;
			}
			set
			{
				this._continueUpdateOnError = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public LoadOption FillLoadOption
		{
			get
			{
				if (this._fillLoadOption == (LoadOption)0)
				{
					return LoadOption.OverwriteChanges;
				}
				return this._fillLoadOption;
			}
			set
			{
				if (value <= LoadOption.Upsert)
				{
					this._fillLoadOption = value;
					return;
				}
				throw ADP.InvalidLoadOption(value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetFillLoadOption()
		{
			this._fillLoadOption = (LoadOption)0;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializeFillLoadOption()
		{
			return this._fillLoadOption > (LoadOption)0;
		}

		[DefaultValue(MissingMappingAction.Passthrough)]
		public MissingMappingAction MissingMappingAction
		{
			get
			{
				return this._missingMappingAction;
			}
			set
			{
				if (value - MissingMappingAction.Passthrough <= 2)
				{
					this._missingMappingAction = value;
					return;
				}
				throw ADP.InvalidMissingMappingAction(value);
			}
		}

		[DefaultValue(MissingSchemaAction.Add)]
		public MissingSchemaAction MissingSchemaAction
		{
			get
			{
				return this._missingSchemaAction;
			}
			set
			{
				if (value - MissingSchemaAction.Add <= 3)
				{
					this._missingSchemaAction = value;
					return;
				}
				throw ADP.InvalidMissingSchemaAction(value);
			}
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		[DefaultValue(false)]
		public virtual bool ReturnProviderSpecificTypes
		{
			get
			{
				return this._returnProviderSpecificTypes;
			}
			set
			{
				this._returnProviderSpecificTypes = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataTableMappingCollection TableMappings
		{
			get
			{
				DataTableMappingCollection dataTableMappingCollection = this._tableMappings;
				if (dataTableMappingCollection == null)
				{
					dataTableMappingCollection = this.CreateTableMappings();
					if (dataTableMappingCollection == null)
					{
						dataTableMappingCollection = new DataTableMappingCollection();
					}
					this._tableMappings = dataTableMappingCollection;
				}
				return dataTableMappingCollection;
			}
		}

		ITableMappingCollection IDataAdapter.TableMappings
		{
			get
			{
				return this.TableMappings;
			}
		}

		protected virtual bool ShouldSerializeTableMappings()
		{
			return true;
		}

		protected bool HasTableMappings()
		{
			return this._tableMappings != null && 0 < this.TableMappings.Count;
		}

		public event FillErrorEventHandler FillError
		{
			add
			{
				this._hasFillErrorHandler = true;
				base.Events.AddHandler(DataAdapter.s_eventFillError, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataAdapter.s_eventFillError, value);
			}
		}

		[Obsolete("CloneInternals() has been deprecated.  Use the DataAdapter(DataAdapter from) constructor.  http://go.microsoft.com/fwlink/?linkid=14202")]
		protected virtual DataAdapter CloneInternals()
		{
			DataAdapter dataAdapter = (DataAdapter)Activator.CreateInstance(base.GetType(), BindingFlags.Instance | BindingFlags.Public, null, null, CultureInfo.InvariantCulture, null);
			dataAdapter.CloneFrom(this);
			return dataAdapter;
		}

		private void CloneFrom(DataAdapter from)
		{
			this._acceptChangesDuringUpdate = from._acceptChangesDuringUpdate;
			this._acceptChangesDuringUpdateAfterInsert = from._acceptChangesDuringUpdateAfterInsert;
			this._continueUpdateOnError = from._continueUpdateOnError;
			this._returnProviderSpecificTypes = from._returnProviderSpecificTypes;
			this._acceptChangesDuringFill = from._acceptChangesDuringFill;
			this._fillLoadOption = from._fillLoadOption;
			this._missingMappingAction = from._missingMappingAction;
			this._missingSchemaAction = from._missingSchemaAction;
			if (from._tableMappings != null && 0 < from.TableMappings.Count)
			{
				DataTableMappingCollection tableMappings = this.TableMappings;
				foreach (object obj in from.TableMappings)
				{
					tableMappings.Add((obj is ICloneable) ? ((ICloneable)obj).Clone() : obj);
				}
			}
		}

		protected virtual DataTableMappingCollection CreateTableMappings()
		{
			DataCommonEventSource.Log.Trace<int>("<comm.DataAdapter.CreateTableMappings|API> {0}", this.ObjectID);
			return new DataTableMappingCollection();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._tableMappings = null;
			}
			base.Dispose(disposing);
		}

		public virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			throw ADP.NotSupported();
		}

		protected virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, string srcTable, IDataReader dataReader)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, SchemaType>("<comm.DataAdapter.FillSchema|API> {0}, dataSet, schemaType={1}, srcTable, dataReader", this.ObjectID, schemaType);
			DataTable[] array;
			try
			{
				if (dataSet == null)
				{
					throw ADP.ArgumentNull("dataSet");
				}
				if (SchemaType.Source != schemaType && SchemaType.Mapped != schemaType)
				{
					throw ADP.InvalidSchemaType(schemaType);
				}
				if (string.IsNullOrEmpty(srcTable))
				{
					throw ADP.FillSchemaRequiresSourceTableName("srcTable");
				}
				if (dataReader == null || dataReader.IsClosed)
				{
					throw ADP.FillRequires("dataReader");
				}
				array = (DataTable[])this.FillSchemaFromReader(dataSet, null, schemaType, srcTable, dataReader);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return array;
		}

		protected virtual DataTable FillSchema(DataTable dataTable, SchemaType schemaType, IDataReader dataReader)
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<comm.DataAdapter.FillSchema|API> {0}, dataTable, schemaType, dataReader", this.ObjectID);
			DataTable dataTable2;
			try
			{
				if (dataTable == null)
				{
					throw ADP.ArgumentNull("dataTable");
				}
				if (SchemaType.Source != schemaType && SchemaType.Mapped != schemaType)
				{
					throw ADP.InvalidSchemaType(schemaType);
				}
				if (dataReader == null || dataReader.IsClosed)
				{
					throw ADP.FillRequires("dataReader");
				}
				dataTable2 = (DataTable)this.FillSchemaFromReader(null, dataTable, schemaType, null, dataReader);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataTable2;
		}

		internal object FillSchemaFromReader(DataSet dataset, DataTable datatable, SchemaType schemaType, string srcTable, IDataReader dataReader)
		{
			DataTable[] array = null;
			int num = 0;
			SchemaMapping schemaMapping;
			for (;;)
			{
				DataReaderContainer dataReaderContainer = DataReaderContainer.Create(dataReader, this.ReturnProviderSpecificTypes);
				if (0 < dataReaderContainer.FieldCount)
				{
					string text = null;
					if (dataset != null)
					{
						text = DataAdapter.GetSourceTableName(srcTable, num);
						num++;
					}
					schemaMapping = new SchemaMapping(this, dataset, datatable, dataReaderContainer, true, schemaType, text, false, null, null);
					if (datatable != null)
					{
						break;
					}
					if (schemaMapping.DataTable != null)
					{
						if (array == null)
						{
							array = new DataTable[] { schemaMapping.DataTable };
						}
						else
						{
							array = DataAdapter.AddDataTableToArray(array, schemaMapping.DataTable);
						}
					}
				}
				if (!dataReader.NextResult())
				{
					goto Block_6;
				}
			}
			return schemaMapping.DataTable;
			Block_6:
			object obj = array;
			if (obj == null && datatable == null)
			{
				obj = Array.Empty<DataTable>();
			}
			return obj;
		}

		public virtual int Fill(DataSet dataSet)
		{
			throw ADP.NotSupported();
		}

		protected virtual int Fill(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<comm.DataAdapter.Fill|API> {0}, dataSet, srcTable, dataReader, startRecord, maxRecords", this.ObjectID);
			int num2;
			try
			{
				if (dataSet == null)
				{
					throw ADP.FillRequires("dataSet");
				}
				if (string.IsNullOrEmpty(srcTable))
				{
					throw ADP.FillRequiresSourceTableName("srcTable");
				}
				if (dataReader == null)
				{
					throw ADP.FillRequires("dataReader");
				}
				if (startRecord < 0)
				{
					throw ADP.InvalidStartRecord("startRecord", startRecord);
				}
				if (maxRecords < 0)
				{
					throw ADP.InvalidMaxRecords("maxRecords", maxRecords);
				}
				if (dataReader.IsClosed)
				{
					num2 = 0;
				}
				else
				{
					DataReaderContainer dataReaderContainer = DataReaderContainer.Create(dataReader, this.ReturnProviderSpecificTypes);
					num2 = this.FillFromReader(dataSet, null, srcTable, dataReaderContainer, startRecord, maxRecords, null, null);
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return num2;
		}

		protected virtual int Fill(DataTable dataTable, IDataReader dataReader)
		{
			DataTable[] array = new DataTable[] { dataTable };
			return this.Fill(array, dataReader, 0, 0);
		}

		protected virtual int Fill(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords)
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<comm.DataAdapter.Fill|API> {0}, dataTables[], dataReader, startRecord, maxRecords", this.ObjectID);
			int num5;
			try
			{
				ADP.CheckArgumentLength(dataTables, "dataTables");
				if (dataTables == null || dataTables.Length == 0 || dataTables[0] == null)
				{
					throw ADP.FillRequires("dataTable");
				}
				if (dataReader == null)
				{
					throw ADP.FillRequires("dataReader");
				}
				if (1 < dataTables.Length && (startRecord != 0 || maxRecords != 0))
				{
					throw ADP.NotSupported();
				}
				int num2 = 0;
				bool flag = false;
				DataSet dataSet = dataTables[0].DataSet;
				try
				{
					if (dataSet != null)
					{
						flag = dataSet.EnforceConstraints;
						dataSet.EnforceConstraints = false;
					}
					int num3 = 0;
					while (num3 < dataTables.Length && !dataReader.IsClosed)
					{
						DataReaderContainer dataReaderContainer = DataReaderContainer.Create(dataReader, this.ReturnProviderSpecificTypes);
						if (dataReaderContainer.FieldCount > 0)
						{
							goto IL_00BC;
						}
						if (num3 == 0)
						{
							bool flag2;
							do
							{
								flag2 = this.FillNextResult(dataReaderContainer);
							}
							while (flag2 && dataReaderContainer.FieldCount <= 0);
							if (flag2)
							{
								goto IL_00BC;
							}
							break;
						}
						IL_00E7:
						num3++;
						continue;
						IL_00BC:
						if (0 < num3 && !this.FillNextResult(dataReaderContainer))
						{
							break;
						}
						int num4 = this.FillFromReader(null, dataTables[num3], null, dataReaderContainer, startRecord, maxRecords, null, null);
						if (num3 == 0)
						{
							num2 = num4;
							goto IL_00E7;
						}
						goto IL_00E7;
					}
				}
				catch (ConstraintException)
				{
					flag = false;
					throw;
				}
				finally
				{
					if (flag)
					{
						dataSet.EnforceConstraints = true;
					}
				}
				num5 = num2;
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return num5;
		}

		internal int FillFromReader(DataSet dataset, DataTable datatable, string srcTable, DataReaderContainer dataReader, int startRecord, int maxRecords, DataColumn parentChapterColumn, object parentChapterValue)
		{
			int num = 0;
			int num2 = 0;
			do
			{
				if (0 < dataReader.FieldCount)
				{
					SchemaMapping schemaMapping = this.FillMapping(dataset, datatable, srcTable, dataReader, num2, parentChapterColumn, parentChapterValue);
					num2++;
					if (schemaMapping != null && schemaMapping.DataValues != null && schemaMapping.DataTable != null)
					{
						schemaMapping.DataTable.BeginLoadData();
						try
						{
							if (1 == num2 && (0 < startRecord || 0 < maxRecords))
							{
								num = this.FillLoadDataRowChunk(schemaMapping, startRecord, maxRecords);
							}
							else
							{
								int num3 = this.FillLoadDataRow(schemaMapping);
								if (1 == num2)
								{
									num = num3;
								}
							}
						}
						finally
						{
							schemaMapping.DataTable.EndLoadData();
						}
						if (datatable != null)
						{
							break;
						}
					}
				}
			}
			while (this.FillNextResult(dataReader));
			return num;
		}

		private int FillLoadDataRowChunk(SchemaMapping mapping, int startRecord, int maxRecords)
		{
			DataReaderContainer dataReader = mapping.DataReader;
			while (0 < startRecord)
			{
				if (!dataReader.Read())
				{
					return 0;
				}
				startRecord--;
			}
			int i = 0;
			if (0 < maxRecords)
			{
				while (i < maxRecords)
				{
					if (!dataReader.Read())
					{
						break;
					}
					if (this._hasFillErrorHandler)
					{
						try
						{
							mapping.LoadDataRowWithClear();
							i++;
							continue;
						}
						catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
						{
							ADP.TraceExceptionForCapture(ex);
							this.OnFillErrorHandler(ex, mapping.DataTable, mapping.DataValues);
							continue;
						}
					}
					mapping.LoadDataRow();
					i++;
				}
			}
			else
			{
				i = this.FillLoadDataRow(mapping);
			}
			return i;
		}

		private int FillLoadDataRow(SchemaMapping mapping)
		{
			int num = 0;
			DataReaderContainer dataReader = mapping.DataReader;
			if (this._hasFillErrorHandler)
			{
				while (dataReader.Read())
				{
					try
					{
						mapping.LoadDataRowWithClear();
						num++;
					}
					catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
					{
						ADP.TraceExceptionForCapture(ex);
						this.OnFillErrorHandler(ex, mapping.DataTable, mapping.DataValues);
					}
				}
			}
			else
			{
				while (dataReader.Read())
				{
					mapping.LoadDataRow();
					num++;
				}
			}
			return num;
		}

		private SchemaMapping FillMappingInternal(DataSet dataset, DataTable datatable, string srcTable, DataReaderContainer dataReader, int schemaCount, DataColumn parentChapterColumn, object parentChapterValue)
		{
			bool flag = MissingSchemaAction.AddWithKey == this.MissingSchemaAction;
			string text = null;
			if (dataset != null)
			{
				text = DataAdapter.GetSourceTableName(srcTable, schemaCount);
			}
			return new SchemaMapping(this, dataset, datatable, dataReader, flag, SchemaType.Mapped, text, true, parentChapterColumn, parentChapterValue);
		}

		private SchemaMapping FillMapping(DataSet dataset, DataTable datatable, string srcTable, DataReaderContainer dataReader, int schemaCount, DataColumn parentChapterColumn, object parentChapterValue)
		{
			SchemaMapping schemaMapping = null;
			if (this._hasFillErrorHandler)
			{
				try
				{
					return this.FillMappingInternal(dataset, datatable, srcTable, dataReader, schemaCount, parentChapterColumn, parentChapterValue);
				}
				catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
				{
					ADP.TraceExceptionForCapture(ex);
					this.OnFillErrorHandler(ex, null, null);
					return schemaMapping;
				}
			}
			schemaMapping = this.FillMappingInternal(dataset, datatable, srcTable, dataReader, schemaCount, parentChapterColumn, parentChapterValue);
			return schemaMapping;
		}

		private bool FillNextResult(DataReaderContainer dataReader)
		{
			bool flag = true;
			if (this._hasFillErrorHandler)
			{
				try
				{
					return dataReader.NextResult();
				}
				catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
				{
					ADP.TraceExceptionForCapture(ex);
					this.OnFillErrorHandler(ex, null, null);
					return flag;
				}
			}
			flag = dataReader.NextResult();
			return flag;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual IDataParameter[] GetFillParameters()
		{
			return Array.Empty<IDataParameter>();
		}

		internal DataTableMapping GetTableMappingBySchemaAction(string sourceTableName, string dataSetTableName, MissingMappingAction mappingAction)
		{
			return DataTableMappingCollection.GetTableMappingBySchemaAction(this._tableMappings, sourceTableName, dataSetTableName, mappingAction);
		}

		internal int IndexOfDataSetTable(string dataSetTable)
		{
			if (this._tableMappings != null)
			{
				return this.TableMappings.IndexOfDataSetTable(dataSetTable);
			}
			return -1;
		}

		protected virtual void OnFillError(FillErrorEventArgs value)
		{
			FillErrorEventHandler fillErrorEventHandler = (FillErrorEventHandler)base.Events[DataAdapter.s_eventFillError];
			if (fillErrorEventHandler == null)
			{
				return;
			}
			fillErrorEventHandler(this, value);
		}

		private void OnFillErrorHandler(Exception e, DataTable dataTable, object[] dataValues)
		{
			FillErrorEventArgs e2 = new FillErrorEventArgs(dataTable, dataValues);
			e2.Errors = e;
			this.OnFillError(e2);
			if (e2.Continue)
			{
				return;
			}
			if (e2.Errors != null)
			{
				throw e2.Errors;
			}
			throw e;
		}

		public virtual int Update(DataSet dataSet)
		{
			throw ADP.NotSupported();
		}

		private static DataTable[] AddDataTableToArray(DataTable[] tables, DataTable newTable)
		{
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables[i] == newTable)
				{
					return tables;
				}
			}
			DataTable[] array = new DataTable[tables.Length + 1];
			for (int j = 0; j < tables.Length; j++)
			{
				array[j] = tables[j];
			}
			array[tables.Length] = newTable;
			return array;
		}

		private static string GetSourceTableName(string srcTable, int index)
		{
			if (index == 0)
			{
				return srcTable;
			}
			return srcTable + index.ToString(CultureInfo.InvariantCulture);
		}

		private static readonly object s_eventFillError = new object();

		private bool _acceptChangesDuringUpdate = true;

		private bool _acceptChangesDuringUpdateAfterInsert = true;

		private bool _continueUpdateOnError;

		private bool _hasFillErrorHandler;

		private bool _returnProviderSpecificTypes;

		private bool _acceptChangesDuringFill = true;

		private LoadOption _fillLoadOption;

		private MissingMappingAction _missingMappingAction = MissingMappingAction.Passthrough;

		private MissingSchemaAction _missingSchemaAction = MissingSchemaAction.Add;

		private DataTableMappingCollection _tableMappings;

		private static int s_objectTypeCount;

		internal readonly int _objectID = Interlocked.Increment(ref DataAdapter.s_objectTypeCount);
	}
}
