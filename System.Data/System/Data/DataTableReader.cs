using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace System.Data
{
	public sealed class DataTableReader : DbDataReader
	{
		public DataTableReader(DataTable dt)
			: this(new DataTable[] { dt })
		{
		}

		public DataTableReader(DataTable[] dataTables)
		{
			if (dataTables == null || dataTables.Length <= 0)
			{
				throw new ArgumentException("Cannot Create DataTable. Argument Empty!");
			}
			this._tables = new DataTable[dataTables.Length];
			for (int i = 0; i < dataTables.Length; i++)
			{
				this._tables[i] = dataTables[i];
			}
			this._closed = false;
			this._index = 0;
			this._current = -1;
			this._rowRef = null;
			this._tableCleared = false;
			this.SubscribeEvents();
		}

		public override int Depth
		{
			get
			{
				return 0;
			}
		}

		public override int FieldCount
		{
			get
			{
				return this.CurrentTable.Columns.Count;
			}
		}

		public override bool HasRows
		{
			get
			{
				return this.CurrentTable.Rows.Count > 0;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return this._closed;
			}
		}

		public override object this[int index]
		{
			get
			{
				this.Validate();
				if (index < 0 || index >= this.FieldCount)
				{
					throw new ArgumentOutOfRangeException("index " + index + " is not in the range");
				}
				DataRow currentRow = this.CurrentRow;
				if (currentRow.RowState == DataRowState.Deleted)
				{
					throw new InvalidOperationException("Deleted Row's information cannot be accessed!");
				}
				return currentRow[index];
			}
		}

		private DataTable CurrentTable
		{
			get
			{
				return this._tables[this._index];
			}
		}

		private DataRow CurrentRow
		{
			get
			{
				return this.CurrentTable.Rows[this._current];
			}
		}

		public override object this[string name]
		{
			get
			{
				this.Validate();
				DataRow currentRow = this.CurrentRow;
				if (currentRow.RowState == DataRowState.Deleted)
				{
					throw new InvalidOperationException("Deleted Row's information cannot be accessed!");
				}
				return currentRow[name];
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return 0;
			}
		}

		private void SubscribeEvents()
		{
			if (this._subscribed)
			{
				return;
			}
			this.CurrentTable.TableCleared += this.OnTableCleared;
			this.CurrentTable.RowChanged += this.OnRowChanged;
			this.CurrentTable.Columns.CollectionChanged += this.OnColumnCollectionChanged;
			for (int i = 0; i < this.CurrentTable.Columns.Count; i++)
			{
				this.CurrentTable.Columns[i].PropertyChanged += this.OnColumnChanged;
			}
			this._subscribed = true;
			this._schemaChanged = false;
		}

		private void UnsubscribeEvents()
		{
			if (!this._subscribed)
			{
				return;
			}
			this.CurrentTable.TableCleared -= this.OnTableCleared;
			this.CurrentTable.RowChanged -= this.OnRowChanged;
			this.CurrentTable.Columns.CollectionChanged -= this.OnColumnCollectionChanged;
			for (int i = 0; i < this.CurrentTable.Columns.Count; i++)
			{
				this.CurrentTable.Columns[i].PropertyChanged -= this.OnColumnChanged;
			}
			this._subscribed = false;
			this._schemaChanged = false;
		}

		public override void Close()
		{
			if (this.IsClosed)
			{
				return;
			}
			this.UnsubscribeEvents();
			this._closed = true;
		}

		public override bool GetBoolean(int i)
		{
			return (bool)this.GetValue(i);
		}

		public override byte GetByte(int i)
		{
			return (byte)this.GetValue(i);
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			byte[] array = this[i] as byte[];
			if (array == null)
			{
				this.ThrowInvalidCastException(this[i].GetType(), typeof(byte[]));
			}
			if (buffer == null)
			{
				return (long)array.Length;
			}
			int num = ((length <= array.Length) ? length : array.Length);
			Array.Copy(array, dataIndex, buffer, (long)bufferIndex, (long)num);
			return (long)num;
		}

		public override char GetChar(int i)
		{
			return (char)this.GetValue(i);
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			char[] array = this[i] as char[];
			if (array == null)
			{
				this.ThrowInvalidCastException(this[i].GetType(), typeof(char[]));
			}
			if (buffer == null)
			{
				return (long)array.Length;
			}
			int num = ((length <= array.Length) ? length : array.Length);
			Array.Copy(array, dataIndex, buffer, (long)bufferIndex, (long)num);
			return (long)num;
		}

		public override string GetDataTypeName(int i)
		{
			return this.GetFieldType(i).ToString();
		}

		public override DateTime GetDateTime(int i)
		{
			return (DateTime)this.GetValue(i);
		}

		public override decimal GetDecimal(int i)
		{
			return (decimal)this.GetValue(i);
		}

		public override double GetDouble(int i)
		{
			return (double)this.GetValue(i);
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		public override Type GetProviderSpecificFieldType(int i)
		{
			return this.GetFieldType(i);
		}

		public override Type GetFieldType(int i)
		{
			this.ValidateClosed();
			return this.CurrentTable.Columns[i].DataType;
		}

		public override float GetFloat(int i)
		{
			return (float)this.GetValue(i);
		}

		public override Guid GetGuid(int i)
		{
			return (Guid)this.GetValue(i);
		}

		public override short GetInt16(int i)
		{
			return (short)this.GetValue(i);
		}

		public override int GetInt32(int i)
		{
			return (int)this.GetValue(i);
		}

		public override long GetInt64(int i)
		{
			return (long)this.GetValue(i);
		}

		public override string GetName(int i)
		{
			this.ValidateClosed();
			return this.CurrentTable.Columns[i].ColumnName;
		}

		public override int GetOrdinal(string name)
		{
			this.ValidateClosed();
			int num = this.CurrentTable.Columns.IndexOf(name);
			if (num == -1)
			{
				throw new ArgumentException(string.Format("Column {0} is not found in the schema", name));
			}
			return num;
		}

		public override object GetProviderSpecificValue(int i)
		{
			return this.GetValue(i);
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			return this.GetValues(values);
		}

		public override string GetString(int i)
		{
			return (string)this.GetValue(i);
		}

		public override object GetValue(int i)
		{
			return this[i];
		}

		public override int GetValues(object[] values)
		{
			this.Validate();
			if (this.CurrentRow.RowState == DataRowState.Deleted)
			{
				throw new DeletedRowInaccessibleException(string.Empty);
			}
			int num = ((this.FieldCount >= values.Length) ? values.Length : this.FieldCount);
			for (int i = 0; i < num; i++)
			{
				values[i] = this.CurrentRow[i];
			}
			return num;
		}

		public override bool IsDBNull(int i)
		{
			return this.GetValue(i) is DBNull;
		}

		public override DataTable GetSchemaTable()
		{
			this.ValidateClosed();
			this.ValidateSchemaIntact();
			if (this._schemaTable != null)
			{
				return this._schemaTable;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("ColumnOrdinal", typeof(int));
			dataTable.Columns.Add("ColumnSize", typeof(int));
			dataTable.Columns.Add("NumericPrecision", typeof(short));
			dataTable.Columns.Add("NumericScale", typeof(short));
			dataTable.Columns.Add("DataType", typeof(Type));
			dataTable.Columns.Add("ProviderType", typeof(int));
			dataTable.Columns.Add("IsLong", typeof(bool));
			dataTable.Columns.Add("AllowDBNull", typeof(bool));
			dataTable.Columns.Add("IsReadOnly", typeof(bool));
			dataTable.Columns.Add("IsRowVersion", typeof(bool));
			dataTable.Columns.Add("IsUnique", typeof(bool));
			dataTable.Columns.Add("IsKey", typeof(bool));
			dataTable.Columns.Add("IsAutoIncrement", typeof(bool));
			dataTable.Columns.Add("BaseCatalogName", typeof(string));
			dataTable.Columns.Add("BaseSchemaName", typeof(string));
			dataTable.Columns.Add("BaseTableName", typeof(string));
			dataTable.Columns.Add("BaseColumnName", typeof(string));
			dataTable.Columns.Add("AutoIncrementSeed", typeof(long));
			dataTable.Columns.Add("AutoIncrementStep", typeof(long));
			dataTable.Columns.Add("DefaultValue", typeof(object));
			dataTable.Columns.Add("Expression", typeof(string));
			dataTable.Columns.Add("ColumnMapping", typeof(MappingType));
			dataTable.Columns.Add("BaseTableNamespace", typeof(string));
			dataTable.Columns.Add("BaseColumnNamespace", typeof(string));
			for (int i = 0; i < this.CurrentTable.Columns.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				DataColumn dataColumn = this.CurrentTable.Columns[i];
				dataRow["ColumnName"] = dataColumn.ColumnName;
				dataRow["BaseColumnName"] = dataColumn.ColumnName;
				dataRow["ColumnOrdinal"] = dataColumn.Ordinal;
				dataRow["ColumnSize"] = dataColumn.MaxLength;
				dataRow["NumericPrecision"] = DBNull.Value;
				dataRow["NumericScale"] = DBNull.Value;
				dataRow["DataType"] = dataColumn.DataType;
				dataRow["ProviderType"] = DBNull.Value;
				dataRow["IsLong"] = false;
				dataRow["AllowDBNull"] = dataColumn.AllowDBNull;
				dataRow["IsReadOnly"] = dataColumn.ReadOnly;
				dataRow["IsRowVersion"] = false;
				dataRow["IsUnique"] = dataColumn.Unique;
				dataRow["IsKey"] = Array.IndexOf<DataColumn>(this.CurrentTable.PrimaryKey, dataColumn) != -1;
				dataRow["IsAutoIncrement"] = dataColumn.AutoIncrement;
				dataRow["AutoIncrementSeed"] = dataColumn.AutoIncrementSeed;
				dataRow["AutoIncrementStep"] = dataColumn.AutoIncrementStep;
				dataRow["BaseCatalogName"] = ((this.CurrentTable.DataSet == null) ? null : this.CurrentTable.DataSet.DataSetName);
				dataRow["BaseSchemaName"] = DBNull.Value;
				dataRow["BaseTableName"] = this.CurrentTable.TableName;
				dataRow["DefaultValue"] = dataColumn.DefaultValue;
				if (dataColumn.Expression == string.Empty)
				{
					dataRow["Expression"] = dataColumn.Expression;
				}
				else
				{
					Regex regex = new Regex("((Parent|Child)( )*[.(])", RegexOptions.IgnoreCase);
					if (regex.IsMatch(dataColumn.Expression, 0))
					{
						dataRow["Expression"] = DBNull.Value;
					}
					else
					{
						dataRow["Expression"] = dataColumn.Expression;
					}
				}
				dataRow["ColumnMapping"] = dataColumn.ColumnMapping;
				dataRow["BaseTableNamespace"] = this.CurrentTable.Namespace;
				dataRow["BaseColumnNamespace"] = dataColumn.Namespace;
				dataTable.Rows.Add(dataRow);
			}
			return this._schemaTable = dataTable;
		}

		private void Validate()
		{
			this.ValidateClosed();
			if (this._index >= this._tables.Length)
			{
				throw new InvalidOperationException("Invalid attempt to read when no data is present");
			}
			if (this._tableCleared)
			{
				throw new RowNotInTableException("The table is cleared, no rows are accessible");
			}
			if (this._current == -1)
			{
				throw new InvalidOperationException("DataReader is invalid for the DataTable");
			}
			this.ValidateSchemaIntact();
		}

		private void ValidateClosed()
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("Invalid attempt to read when the reader is closed");
			}
		}

		private void ValidateSchemaIntact()
		{
			if (this._schemaChanged)
			{
				throw new InvalidOperationException("Schema of current DataTable '" + this.CurrentTable.TableName + "' in DataTableReader has changed, DataTableReader is invalid.");
			}
		}

		private void ThrowInvalidCastException(Type sourceType, Type destType)
		{
			throw new InvalidCastException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", sourceType, destType));
		}

		private bool MoveNext()
		{
			if (this._index >= this._tables.Length || this._tableCleared)
			{
				return false;
			}
			do
			{
				this._current++;
			}
			while (this._current < this.CurrentTable.Rows.Count && this.CurrentRow.RowState == DataRowState.Deleted);
			this._rowRef = ((this._current >= this.CurrentTable.Rows.Count) ? null : this.CurrentRow);
			return this._current < this.CurrentTable.Rows.Count;
		}

		public override bool NextResult()
		{
			if (this._index + 1 >= this._tables.Length)
			{
				this.UnsubscribeEvents();
				this._index = this._tables.Length;
				return false;
			}
			this.UnsubscribeEvents();
			this._index++;
			this._current = -1;
			this._rowRef = null;
			this._schemaTable = null;
			this._tableCleared = false;
			this.SubscribeEvents();
			return true;
		}

		public override bool Read()
		{
			this.ValidateClosed();
			return this.MoveNext();
		}

		private void OnColumnChanged(object sender, PropertyChangedEventArgs args)
		{
			this._schemaChanged = true;
		}

		private void OnColumnCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			this._schemaChanged = true;
		}

		private void OnRowChanged(object src, DataRowChangeEventArgs args)
		{
			DataRowAction action = args.Action;
			DataRow row = args.Row;
			if (action == DataRowAction.Add)
			{
				if (this._tableCleared && this._current != -1)
				{
					return;
				}
				if (this._current == -1 || (this._current >= 0 && row.RowID > this.CurrentRow.RowID))
				{
					this._tableCleared = false;
					return;
				}
				this._current++;
				this._rowRef = this.CurrentRow;
			}
			if (action == DataRowAction.Commit && row.RowState == DataRowState.Detached)
			{
				if (this._rowRef == row)
				{
					this._current--;
					this._rowRef = ((this._current < 0) ? null : this.CurrentRow);
				}
				if (this._current >= this.CurrentTable.Rows.Count)
				{
					this._current--;
					this._rowRef = ((this._current < 0) ? null : this.CurrentRow);
					return;
				}
				if (this._current > 0 && this._rowRef == this.CurrentTable.Rows[this._current - 1])
				{
					this._current--;
					this._rowRef = this.CurrentRow;
					return;
				}
			}
		}

		private void OnTableCleared(object src, DataTableClearEventArgs args)
		{
			this._tableCleared = true;
		}

		private bool _closed;

		private DataTable[] _tables;

		private int _current = -1;

		private int _index;

		private DataTable _schemaTable;

		private bool _tableCleared;

		private bool _subscribed;

		private DataRow _rowRef;

		private bool _schemaChanged;
	}
}
