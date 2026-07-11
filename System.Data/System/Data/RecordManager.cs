using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

namespace System.Data
{
	internal sealed class RecordManager
	{
		internal RecordManager(DataTable table)
		{
			if (table == null)
			{
				throw ExceptionBuilder.ArgumentNull("table");
			}
			this._table = table;
		}

		private void GrowRecordCapacity()
		{
			this.RecordCapacity = ((RecordManager.NewCapacity(this._recordCapacity) < this.NormalizedMinimumCapacity(this._minimumCapacity)) ? this.NormalizedMinimumCapacity(this._minimumCapacity) : RecordManager.NewCapacity(this._recordCapacity));
			DataRow[] array = this._table.NewRowArray(this._recordCapacity);
			if (this._rows != null)
			{
				Array.Copy(this._rows, 0, array, 0, Math.Min(this._lastFreeRecord, this._rows.Length));
			}
			this._rows = array;
		}

		internal int LastFreeRecord
		{
			get
			{
				return this._lastFreeRecord;
			}
		}

		internal int MinimumCapacity
		{
			get
			{
				return this._minimumCapacity;
			}
			set
			{
				if (this._minimumCapacity != value)
				{
					if (value < 0)
					{
						throw ExceptionBuilder.NegativeMinimumCapacity();
					}
					this._minimumCapacity = value;
				}
			}
		}

		internal int RecordCapacity
		{
			get
			{
				return this._recordCapacity;
			}
			set
			{
				if (this._recordCapacity != value)
				{
					for (int i = 0; i < this._table.Columns.Count; i++)
					{
						this._table.Columns[i].SetCapacity(value);
					}
					this._recordCapacity = value;
				}
			}
		}

		internal static int NewCapacity(int capacity)
		{
			if (capacity >= 128)
			{
				return capacity + capacity;
			}
			return 128;
		}

		private int NormalizedMinimumCapacity(int capacity)
		{
			if (capacity >= 1014)
			{
				return (capacity + 10 >> 10) + 1 << 10;
			}
			if (capacity >= 246)
			{
				return 1024;
			}
			if (capacity < 54)
			{
				return 64;
			}
			return 256;
		}

		internal int NewRecordBase()
		{
			int num;
			if (this._freeRecordList.Count != 0)
			{
				num = this._freeRecordList[this._freeRecordList.Count - 1];
				this._freeRecordList.RemoveAt(this._freeRecordList.Count - 1);
			}
			else
			{
				if (this._lastFreeRecord >= this._recordCapacity)
				{
					this.GrowRecordCapacity();
				}
				num = this._lastFreeRecord;
				this._lastFreeRecord++;
			}
			return num;
		}

		internal void FreeRecord(ref int record)
		{
			if (-1 != record)
			{
				this[record] = null;
				int count = this._table._columnCollection.Count;
				for (int i = 0; i < count; i++)
				{
					this._table._columnCollection[i].FreeRecord(record);
				}
				if (this._lastFreeRecord == record + 1)
				{
					this._lastFreeRecord--;
				}
				else if (record < this._lastFreeRecord)
				{
					this._freeRecordList.Add(record);
				}
				record = -1;
			}
		}

		internal void Clear(bool clearAll)
		{
			if (clearAll)
			{
				for (int i = 0; i < this._recordCapacity; i++)
				{
					this._rows[i] = null;
				}
				int count = this._table._columnCollection.Count;
				for (int j = 0; j < count; j++)
				{
					DataColumn dataColumn = this._table._columnCollection[j];
					for (int k = 0; k < this._recordCapacity; k++)
					{
						dataColumn.FreeRecord(k);
					}
				}
				this._lastFreeRecord = 0;
				this._freeRecordList.Clear();
				return;
			}
			this._freeRecordList.Capacity = this._freeRecordList.Count + this._table.Rows.Count;
			for (int l = 0; l < this._recordCapacity; l++)
			{
				if (this._rows[l] != null && this._rows[l].rowID != -1L)
				{
					int num = l;
					this.FreeRecord(ref num);
				}
			}
		}

		internal DataRow this[int record]
		{
			get
			{
				return this._rows[record];
			}
			set
			{
				this._rows[record] = value;
			}
		}

		internal void SetKeyValues(int record, DataKey key, object[] keyValues)
		{
			for (int i = 0; i < keyValues.Length; i++)
			{
				key.ColumnsReference[i][record] = keyValues[i];
			}
		}

		internal int ImportRecord(DataTable src, int record)
		{
			return this.CopyRecord(src, record, -1);
		}

		internal int CopyRecord(DataTable src, int record, int copy)
		{
			if (record == -1)
			{
				return copy;
			}
			int num = -1;
			try
			{
				num = ((copy == -1) ? this._table.NewUninitializedRecord() : copy);
				int count = this._table.Columns.Count;
				for (int i = 0; i < count; i++)
				{
					DataColumn dataColumn = this._table.Columns[i];
					DataColumn dataColumn2 = src.Columns[dataColumn.ColumnName];
					if (dataColumn2 != null)
					{
						object obj = dataColumn2[record];
						ICloneable cloneable = obj as ICloneable;
						if (cloneable != null)
						{
							dataColumn[num] = cloneable.Clone();
						}
						else
						{
							dataColumn[num] = obj;
						}
					}
					else if (-1 == copy)
					{
						dataColumn.Init(num);
					}
				}
			}
			catch (Exception ex) when (ADP.IsCatchableOrSecurityExceptionType(ex))
			{
				if (-1 == copy)
				{
					this.FreeRecord(ref num);
				}
				throw;
			}
			return num;
		}

		internal void SetRowCache(DataRow[] newRows)
		{
			this._rows = newRows;
			this._lastFreeRecord = this._rows.Length;
			this._recordCapacity = this._lastFreeRecord;
		}

		[Conditional("DEBUG")]
		internal void VerifyRecord(int record)
		{
		}

		[Conditional("DEBUG")]
		internal void VerifyRecord(int record, DataRow row)
		{
		}

		private readonly DataTable _table;

		private int _lastFreeRecord;

		private int _minimumCapacity = 50;

		private int _recordCapacity;

		private readonly List<int> _freeRecordList = new List<int>();

		private DataRow[] _rows;
	}
}
