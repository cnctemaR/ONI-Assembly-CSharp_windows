using System;
using System.Collections;
using System.Data.Common;
using System.Globalization;

namespace System.Data
{
	public sealed class DataTableReader : DbDataReader
	{
		public DataTableReader(DataTable dataTable)
		{
			if (dataTable == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTable");
			}
			this._tables = new DataTable[] { dataTable };
			this.Init();
		}

		public DataTableReader(DataTable[] dataTables)
		{
			if (dataTables == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTable");
			}
			if (dataTables.Length == 0)
			{
				throw ExceptionBuilder.DataTableReaderArgumentIsEmpty();
			}
			this._tables = new DataTable[dataTables.Length];
			for (int i = 0; i < dataTables.Length; i++)
			{
				if (dataTables[i] == null)
				{
					throw ExceptionBuilder.ArgumentNull("DataTable");
				}
				this._tables[i] = dataTables[i];
			}
			this.Init();
		}

		private bool ReaderIsInvalid
		{
			get
			{
				return this._readerIsInvalid;
			}
			set
			{
				if (this._readerIsInvalid == value)
				{
					return;
				}
				this._readerIsInvalid = value;
				if (this._readerIsInvalid && this._listener != null)
				{
					this._listener.CleanUp();
				}
			}
		}

		private bool IsSchemaChanged
		{
			get
			{
				return this._schemaIsChanged;
			}
			set
			{
				if (!value || this._schemaIsChanged == value)
				{
					return;
				}
				this._schemaIsChanged = value;
				if (this._listener != null)
				{
					this._listener.CleanUp();
				}
			}
		}

		internal DataTable CurrentDataTable
		{
			get
			{
				return this._currentDataTable;
			}
		}

		private void Init()
		{
			this._tableCounter = 0;
			this._reachEORows = false;
			this._schemaIsChanged = false;
			this._currentDataTable = this._tables[this._tableCounter];
			this._hasRows = this._currentDataTable.Rows.Count > 0;
			this.ReaderIsInvalid = false;
			this._listener = new DataTableReaderListener(this);
		}

		public override void Close()
		{
			if (!this._isOpen)
			{
				return;
			}
			if (this._listener != null)
			{
				this._listener.CleanUp();
			}
			this._listener = null;
			this._schemaTable = null;
			this._isOpen = false;
		}

		public override DataTable GetSchemaTable()
		{
			this.ValidateOpen("GetSchemaTable");
			this.ValidateReader();
			if (this._schemaTable == null)
			{
				this._schemaTable = DataTableReader.GetSchemaTableFromDataTable(this._currentDataTable);
			}
			return this._schemaTable;
		}

		public override bool NextResult()
		{
			this.ValidateOpen("NextResult");
			if (this._tableCounter == this._tables.Length - 1)
			{
				return false;
			}
			DataTable[] tables = this._tables;
			int num = this._tableCounter + 1;
			this._tableCounter = num;
			this._currentDataTable = tables[num];
			if (this._listener != null)
			{
				this._listener.UpdataTable(this._currentDataTable);
			}
			this._schemaTable = null;
			this._rowCounter = -1;
			this._currentRowRemoved = false;
			this._reachEORows = false;
			this._schemaIsChanged = false;
			this._started = false;
			this.ReaderIsInvalid = false;
			this._tableCleared = false;
			this._hasRows = this._currentDataTable.Rows.Count > 0;
			return true;
		}

		public override bool Read()
		{
			if (!this._started)
			{
				this._started = true;
			}
			this.ValidateOpen("Read");
			this.ValidateReader();
			if (this._reachEORows)
			{
				return false;
			}
			if (this._rowCounter >= this._currentDataTable.Rows.Count - 1)
			{
				this._reachEORows = true;
				if (this._listener != null)
				{
					this._listener.CleanUp();
				}
				return false;
			}
			this._rowCounter++;
			this.ValidateRow(this._rowCounter);
			this._currentDataRow = this._currentDataTable.Rows[this._rowCounter];
			while (this._currentDataRow.RowState == DataRowState.Deleted)
			{
				this._rowCounter++;
				if (this._rowCounter == this._currentDataTable.Rows.Count)
				{
					this._reachEORows = true;
					if (this._listener != null)
					{
						this._listener.CleanUp();
					}
					return false;
				}
				this.ValidateRow(this._rowCounter);
				this._currentDataRow = this._currentDataTable.Rows[this._rowCounter];
			}
			if (this._currentRowRemoved)
			{
				this._currentRowRemoved = false;
			}
			return true;
		}

		public override int Depth
		{
			get
			{
				this.ValidateOpen("Depth");
				this.ValidateReader();
				return 0;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return !this._isOpen;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				this.ValidateReader();
				return 0;
			}
		}

		public override bool HasRows
		{
			get
			{
				this.ValidateOpen("HasRows");
				this.ValidateReader();
				return this._hasRows;
			}
		}

		public override object this[int ordinal]
		{
			get
			{
				this.ValidateOpen("Item");
				this.ValidateReader();
				if (this._currentDataRow == null || this._currentDataRow.RowState == DataRowState.Deleted)
				{
					this.ReaderIsInvalid = true;
					throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
				}
				object obj;
				try
				{
					obj = this._currentDataRow[ordinal];
				}
				catch (IndexOutOfRangeException ex)
				{
					ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
					throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
				}
				return obj;
			}
		}

		public override object this[string name]
		{
			get
			{
				this.ValidateOpen("Item");
				this.ValidateReader();
				if (this._currentDataRow == null || this._currentDataRow.RowState == DataRowState.Deleted)
				{
					this.ReaderIsInvalid = true;
					throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
				}
				return this._currentDataRow[name];
			}
		}

		public override int FieldCount
		{
			get
			{
				this.ValidateOpen("FieldCount");
				this.ValidateReader();
				return this._currentDataTable.Columns.Count;
			}
		}

		public override Type GetProviderSpecificFieldType(int ordinal)
		{
			this.ValidateOpen("GetProviderSpecificFieldType");
			this.ValidateReader();
			return this.GetFieldType(ordinal);
		}

		public override object GetProviderSpecificValue(int ordinal)
		{
			this.ValidateOpen("GetProviderSpecificValue");
			this.ValidateReader();
			return this.GetValue(ordinal);
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			this.ValidateOpen("GetProviderSpecificValues");
			this.ValidateReader();
			return this.GetValues(values);
		}

		public override bool GetBoolean(int ordinal)
		{
			this.ValidateState("GetBoolean");
			this.ValidateReader();
			bool flag;
			try
			{
				flag = (bool)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return flag;
		}

		public override byte GetByte(int ordinal)
		{
			this.ValidateState("GetByte");
			this.ValidateReader();
			byte b;
			try
			{
				b = (byte)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return b;
		}

		public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			this.ValidateState("GetBytes");
			this.ValidateReader();
			byte[] array;
			try
			{
				array = (byte[])this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			if (buffer == null)
			{
				return (long)array.Length;
			}
			int num = (int)dataIndex;
			int num2 = Math.Min(array.Length - num, length);
			if (num < 0)
			{
				throw ADP.InvalidSourceBufferIndex(array.Length, (long)num, "dataIndex");
			}
			if (bufferIndex < 0 || (bufferIndex > 0 && bufferIndex >= buffer.Length))
			{
				throw ADP.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
			}
			if (0 < num2)
			{
				Array.Copy(array, dataIndex, buffer, (long)bufferIndex, (long)num2);
			}
			else
			{
				if (length < 0)
				{
					throw ADP.InvalidDataLength((long)length);
				}
				num2 = 0;
			}
			return (long)num2;
		}

		public override char GetChar(int ordinal)
		{
			this.ValidateState("GetChar");
			this.ValidateReader();
			char c;
			try
			{
				c = (char)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return c;
		}

		public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			this.ValidateState("GetChars");
			this.ValidateReader();
			char[] array;
			try
			{
				array = (char[])this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			if (buffer == null)
			{
				return (long)array.Length;
			}
			int num = (int)dataIndex;
			int num2 = Math.Min(array.Length - num, length);
			if (num < 0)
			{
				throw ADP.InvalidSourceBufferIndex(array.Length, (long)num, "dataIndex");
			}
			if (bufferIndex < 0 || (bufferIndex > 0 && bufferIndex >= buffer.Length))
			{
				throw ADP.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
			}
			if (0 < num2)
			{
				Array.Copy(array, dataIndex, buffer, (long)bufferIndex, (long)num2);
			}
			else
			{
				if (length < 0)
				{
					throw ADP.InvalidDataLength((long)length);
				}
				num2 = 0;
			}
			return (long)num2;
		}

		public override string GetDataTypeName(int ordinal)
		{
			this.ValidateOpen("GetDataTypeName");
			this.ValidateReader();
			return this.GetFieldType(ordinal).Name;
		}

		public override DateTime GetDateTime(int ordinal)
		{
			this.ValidateState("GetDateTime");
			this.ValidateReader();
			DateTime dateTime;
			try
			{
				dateTime = (DateTime)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return dateTime;
		}

		public override decimal GetDecimal(int ordinal)
		{
			this.ValidateState("GetDecimal");
			this.ValidateReader();
			decimal num;
			try
			{
				num = (decimal)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return num;
		}

		public override double GetDouble(int ordinal)
		{
			this.ValidateState("GetDouble");
			this.ValidateReader();
			double num;
			try
			{
				num = (double)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return num;
		}

		public override Type GetFieldType(int ordinal)
		{
			this.ValidateOpen("GetFieldType");
			this.ValidateReader();
			Type dataType;
			try
			{
				dataType = this._currentDataTable.Columns[ordinal].DataType;
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return dataType;
		}

		public override float GetFloat(int ordinal)
		{
			this.ValidateState("GetFloat");
			this.ValidateReader();
			float num;
			try
			{
				num = (float)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return num;
		}

		public override Guid GetGuid(int ordinal)
		{
			this.ValidateState("GetGuid");
			this.ValidateReader();
			Guid guid;
			try
			{
				guid = (Guid)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return guid;
		}

		public override short GetInt16(int ordinal)
		{
			this.ValidateState("GetInt16");
			this.ValidateReader();
			short num;
			try
			{
				num = (short)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return num;
		}

		public override int GetInt32(int ordinal)
		{
			this.ValidateState("GetInt32");
			this.ValidateReader();
			int num;
			try
			{
				num = (int)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return num;
		}

		public override long GetInt64(int ordinal)
		{
			this.ValidateState("GetInt64");
			this.ValidateReader();
			long num;
			try
			{
				num = (long)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return num;
		}

		public override string GetName(int ordinal)
		{
			this.ValidateOpen("GetName");
			this.ValidateReader();
			string columnName;
			try
			{
				columnName = this._currentDataTable.Columns[ordinal].ColumnName;
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return columnName;
		}

		public override int GetOrdinal(string name)
		{
			this.ValidateOpen("GetOrdinal");
			this.ValidateReader();
			DataColumn dataColumn = this._currentDataTable.Columns[name];
			if (dataColumn != null)
			{
				return dataColumn.Ordinal;
			}
			throw ExceptionBuilder.ColumnNotInTheTable(name, this._currentDataTable.TableName);
		}

		public override string GetString(int ordinal)
		{
			this.ValidateState("GetString");
			this.ValidateReader();
			string text;
			try
			{
				text = (string)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return text;
		}

		public override object GetValue(int ordinal)
		{
			this.ValidateState("GetValue");
			this.ValidateReader();
			object obj;
			try
			{
				obj = this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return obj;
		}

		public override int GetValues(object[] values)
		{
			this.ValidateState("GetValues");
			this.ValidateReader();
			if (values == null)
			{
				throw ExceptionBuilder.ArgumentNull("values");
			}
			Array.Copy(this._currentDataRow.ItemArray, values, (this._currentDataRow.ItemArray.Length > values.Length) ? values.Length : this._currentDataRow.ItemArray.Length);
			if (this._currentDataRow.ItemArray.Length <= values.Length)
			{
				return this._currentDataRow.ItemArray.Length;
			}
			return values.Length;
		}

		public override bool IsDBNull(int ordinal)
		{
			this.ValidateState("IsDBNull");
			this.ValidateReader();
			bool flag;
			try
			{
				flag = this._currentDataRow.IsNull(ordinal);
			}
			catch (IndexOutOfRangeException ex)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return flag;
		}

		public override IEnumerator GetEnumerator()
		{
			this.ValidateOpen("GetEnumerator");
			return new DbEnumerator(this);
		}

		internal static DataTable GetSchemaTableFromDataTable(DataTable table)
		{
			if (table == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTable");
			}
			DataTable dataTable = new DataTable("SchemaTable");
			dataTable.Locale = CultureInfo.InvariantCulture;
			DataColumn dataColumn = new DataColumn(SchemaTableColumn.ColumnName, typeof(string));
			DataColumn dataColumn2 = new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int));
			DataColumn dataColumn3 = new DataColumn(SchemaTableColumn.ColumnSize, typeof(int));
			DataColumn dataColumn4 = new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short));
			DataColumn dataColumn5 = new DataColumn(SchemaTableColumn.NumericScale, typeof(short));
			DataColumn dataColumn6 = new DataColumn(SchemaTableColumn.DataType, typeof(Type));
			DataColumn dataColumn7 = new DataColumn(SchemaTableColumn.ProviderType, typeof(int));
			DataColumn dataColumn8 = new DataColumn(SchemaTableColumn.IsLong, typeof(bool));
			DataColumn dataColumn9 = new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool));
			DataColumn dataColumn10 = new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
			DataColumn dataColumn11 = new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
			DataColumn dataColumn12 = new DataColumn(SchemaTableColumn.IsUnique, typeof(bool));
			DataColumn dataColumn13 = new DataColumn(SchemaTableColumn.IsKey, typeof(bool));
			DataColumn dataColumn14 = new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
			DataColumn dataColumn15 = new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string));
			DataColumn dataColumn16 = new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
			DataColumn dataColumn17 = new DataColumn(SchemaTableColumn.BaseTableName, typeof(string));
			DataColumn dataColumn18 = new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string));
			DataColumn dataColumn19 = new DataColumn(SchemaTableOptionalColumn.AutoIncrementSeed, typeof(long));
			DataColumn dataColumn20 = new DataColumn(SchemaTableOptionalColumn.AutoIncrementStep, typeof(long));
			DataColumn dataColumn21 = new DataColumn(SchemaTableOptionalColumn.DefaultValue, typeof(object));
			DataColumn dataColumn22 = new DataColumn(SchemaTableOptionalColumn.Expression, typeof(string));
			DataColumn dataColumn23 = new DataColumn(SchemaTableOptionalColumn.ColumnMapping, typeof(MappingType));
			DataColumn dataColumn24 = new DataColumn(SchemaTableOptionalColumn.BaseTableNamespace, typeof(string));
			DataColumn dataColumn25 = new DataColumn(SchemaTableOptionalColumn.BaseColumnNamespace, typeof(string));
			dataColumn3.DefaultValue = -1;
			if (table.DataSet != null)
			{
				dataColumn16.DefaultValue = table.DataSet.DataSetName;
			}
			dataColumn17.DefaultValue = table.TableName;
			dataColumn24.DefaultValue = table.Namespace;
			dataColumn11.DefaultValue = false;
			dataColumn8.DefaultValue = false;
			dataColumn10.DefaultValue = false;
			dataColumn13.DefaultValue = false;
			dataColumn14.DefaultValue = false;
			dataColumn19.DefaultValue = 0;
			dataColumn20.DefaultValue = 1;
			dataTable.Columns.Add(dataColumn);
			dataTable.Columns.Add(dataColumn2);
			dataTable.Columns.Add(dataColumn3);
			dataTable.Columns.Add(dataColumn4);
			dataTable.Columns.Add(dataColumn5);
			dataTable.Columns.Add(dataColumn6);
			dataTable.Columns.Add(dataColumn7);
			dataTable.Columns.Add(dataColumn8);
			dataTable.Columns.Add(dataColumn9);
			dataTable.Columns.Add(dataColumn10);
			dataTable.Columns.Add(dataColumn11);
			dataTable.Columns.Add(dataColumn12);
			dataTable.Columns.Add(dataColumn13);
			dataTable.Columns.Add(dataColumn14);
			dataTable.Columns.Add(dataColumn16);
			dataTable.Columns.Add(dataColumn15);
			dataTable.Columns.Add(dataColumn17);
			dataTable.Columns.Add(dataColumn18);
			dataTable.Columns.Add(dataColumn19);
			dataTable.Columns.Add(dataColumn20);
			dataTable.Columns.Add(dataColumn21);
			dataTable.Columns.Add(dataColumn22);
			dataTable.Columns.Add(dataColumn23);
			dataTable.Columns.Add(dataColumn24);
			dataTable.Columns.Add(dataColumn25);
			foreach (object obj in table.Columns)
			{
				DataColumn dataColumn26 = (DataColumn)obj;
				DataRow dataRow = dataTable.NewRow();
				dataRow[dataColumn] = dataColumn26.ColumnName;
				dataRow[dataColumn2] = dataColumn26.Ordinal;
				dataRow[dataColumn6] = dataColumn26.DataType;
				if (dataColumn26.DataType == typeof(string))
				{
					dataRow[dataColumn3] = dataColumn26.MaxLength;
				}
				dataRow[dataColumn9] = dataColumn26.AllowDBNull;
				dataRow[dataColumn10] = dataColumn26.ReadOnly;
				dataRow[dataColumn12] = dataColumn26.Unique;
				if (dataColumn26.AutoIncrement)
				{
					dataRow[dataColumn14] = true;
					dataRow[dataColumn19] = dataColumn26.AutoIncrementSeed;
					dataRow[dataColumn20] = dataColumn26.AutoIncrementStep;
				}
				if (dataColumn26.DefaultValue != DBNull.Value)
				{
					dataRow[dataColumn21] = dataColumn26.DefaultValue;
				}
				if (dataColumn26.Expression.Length != 0)
				{
					bool flag = false;
					DataColumn[] dependency = dataColumn26.DataExpression.GetDependency();
					for (int i = 0; i < dependency.Length; i++)
					{
						if (dependency[i].Table != table)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						dataRow[dataColumn22] = dataColumn26.Expression;
					}
				}
				dataRow[dataColumn23] = dataColumn26.ColumnMapping;
				dataRow[dataColumn18] = dataColumn26.ColumnName;
				dataRow[dataColumn25] = dataColumn26.Namespace;
				dataTable.Rows.Add(dataRow);
			}
			foreach (DataColumn dataColumn27 in table.PrimaryKey)
			{
				dataTable.Rows[dataColumn27.Ordinal][dataColumn13] = true;
			}
			dataTable.AcceptChanges();
			return dataTable;
		}

		private void ValidateOpen(string caller)
		{
			if (!this._isOpen)
			{
				throw ADP.DataReaderClosed(caller);
			}
		}

		private void ValidateReader()
		{
			if (this.ReaderIsInvalid)
			{
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
			if (this.IsSchemaChanged)
			{
				throw ExceptionBuilder.DataTableReaderSchemaIsInvalid(this._currentDataTable.TableName);
			}
		}

		private void ValidateState(string caller)
		{
			this.ValidateOpen(caller);
			if (this._tableCleared)
			{
				throw ExceptionBuilder.EmptyDataTableReader(this._currentDataTable.TableName);
			}
			if (this._currentDataRow == null || this._currentDataTable == null)
			{
				this.ReaderIsInvalid = true;
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
			if (this._currentDataRow.RowState == DataRowState.Deleted || this._currentDataRow.RowState == DataRowState.Detached || this._currentRowRemoved)
			{
				throw ExceptionBuilder.InvalidCurrentRowInDataTableReader();
			}
			if (0 > this._rowCounter || this._currentDataTable.Rows.Count <= this._rowCounter)
			{
				this.ReaderIsInvalid = true;
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
		}

		private void ValidateRow(int rowPosition)
		{
			if (this.ReaderIsInvalid)
			{
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
			if (0 > rowPosition || this._currentDataTable.Rows.Count <= rowPosition)
			{
				this.ReaderIsInvalid = true;
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
		}

		internal void SchemaChanged()
		{
			this.IsSchemaChanged = true;
		}

		internal void DataTableCleared()
		{
			if (!this._started)
			{
				return;
			}
			this._rowCounter = -1;
			if (!this._reachEORows)
			{
				this._currentRowRemoved = true;
			}
		}

		internal void DataChanged(DataRowChangeEventArgs args)
		{
			if (!this._started || (this._rowCounter == -1 && !this._tableCleared))
			{
				return;
			}
			DataRowAction action = args.Action;
			if (action <= DataRowAction.Rollback)
			{
				if (action != DataRowAction.Delete && action != DataRowAction.Rollback)
				{
					return;
				}
			}
			else if (action != DataRowAction.Commit)
			{
				if (action != DataRowAction.Add)
				{
					return;
				}
				this.ValidateRow(this._rowCounter + 1);
				if (this._currentDataRow == this._currentDataTable.Rows[this._rowCounter + 1])
				{
					this._rowCounter++;
					return;
				}
				return;
			}
			if (args.Row.RowState == DataRowState.Detached)
			{
				if (args.Row != this._currentDataRow)
				{
					if (this._rowCounter != 0)
					{
						this.ValidateRow(this._rowCounter - 1);
						if (this._currentDataRow == this._currentDataTable.Rows[this._rowCounter - 1])
						{
							this._rowCounter--;
							return;
						}
					}
				}
				else
				{
					this._currentRowRemoved = true;
					if (this._rowCounter > 0)
					{
						this._rowCounter--;
						this._currentDataRow = this._currentDataTable.Rows[this._rowCounter];
						return;
					}
					this._rowCounter = -1;
					this._currentDataRow = null;
				}
			}
		}

		private readonly DataTable[] _tables;

		private bool _isOpen = true;

		private DataTable _schemaTable;

		private int _tableCounter = -1;

		private int _rowCounter = -1;

		private DataTable _currentDataTable;

		private DataRow _currentDataRow;

		private bool _hasRows = true;

		private bool _reachEORows;

		private bool _currentRowRemoved;

		private bool _schemaIsChanged;

		private bool _started;

		private bool _readerIsInvalid;

		private DataTableReaderListener _listener;

		private bool _tableCleared;
	}
}
