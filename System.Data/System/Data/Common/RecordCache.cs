using System;
using System.Collections;

namespace System.Data.Common
{
	internal class RecordCache
	{
		internal RecordCache(DataTable table)
		{
			this._table = table;
			this._rowsToRecords = table.NewRowArray(16);
		}

		internal int CurrentCapacity
		{
			get
			{
				return this._currentCapacity;
			}
		}

		internal DataRow this[int index]
		{
			get
			{
				return this._rowsToRecords[index];
			}
			set
			{
				if (index >= 0)
				{
					this._rowsToRecords[index] = value;
				}
			}
		}

		internal int NewRecord()
		{
			if (this._records.Count > 0)
			{
				return (int)this._records.Pop();
			}
			DataColumnCollection columns = this._table.Columns;
			if (this._nextFreeIndex >= this._currentCapacity)
			{
				this._currentCapacity *= 2;
				if (this._currentCapacity < 128)
				{
					this._currentCapacity = 128;
				}
				for (int i = 0; i < columns.Count; i++)
				{
					columns[i].DataContainer.Capacity = this._currentCapacity;
				}
				DataRow[] rowsToRecords = this._rowsToRecords;
				this._rowsToRecords = this._table.NewRowArray(this._currentCapacity);
				Array.Copy(rowsToRecords, 0, this._rowsToRecords, 0, rowsToRecords.Length);
			}
			return this._nextFreeIndex++;
		}

		internal void DisposeRecord(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException();
			}
			if (!this._records.Contains(index))
			{
				this._records.Push(index);
			}
			this[index] = null;
		}

		internal int CopyRecord(DataTable fromTable, int fromRecordIndex, int toRecordIndex)
		{
			int num = toRecordIndex;
			if (toRecordIndex == -1)
			{
				num = this.NewRecord();
			}
			int num2;
			try
			{
				foreach (object obj in fromTable.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					DataColumn dataColumn2 = this._table.Columns[dataColumn.ColumnName];
					if (dataColumn2 != null)
					{
						dataColumn2.DataContainer.CopyValue(dataColumn.DataContainer, fromRecordIndex, num);
					}
				}
				num2 = num;
			}
			catch
			{
				if (toRecordIndex == -1)
				{
					this.DisposeRecord(num);
				}
				throw;
			}
			return num2;
		}

		internal void ReadIDataRecord(int recordIndex, IDataRecord record, int[] mapping, int length)
		{
			if (mapping.Length > this._table.Columns.Count)
			{
				throw new ArgumentException();
			}
			int i;
			for (i = 0; i < length; i++)
			{
				DataColumn dataColumn = this._table.Columns[mapping[i]];
				dataColumn.DataContainer.SetItemFromDataRecord(recordIndex, record, i);
			}
			while (i < mapping.Length)
			{
				DataColumn dataColumn2 = this._table.Columns[mapping[i]];
				if (dataColumn2.AutoIncrement)
				{
					dataColumn2.DataContainer[recordIndex] = dataColumn2.AutoIncrementValue();
				}
				else
				{
					dataColumn2.DataContainer[recordIndex] = dataColumn2.DefaultValue;
				}
				i++;
			}
		}

		private const int MIN_CACHE_SIZE = 128;

		private Stack _records = new Stack(16);

		private int _nextFreeIndex;

		private int _currentCapacity;

		private DataTable _table;

		private DataRow[] _rowsToRecords;
	}
}
