using System;
using System.Collections;

namespace System.Data.Common
{
	internal class Index
	{
		internal Index(Key key)
		{
			this._key = key;
			this.Reset();
		}

		internal Key Key
		{
			get
			{
				return this._key;
			}
		}

		internal int Size
		{
			get
			{
				return this._size;
			}
		}

		internal int RefCount
		{
			get
			{
				return this._refCount;
			}
		}

		internal int IndexToRecord(int index)
		{
			return (index >= 0) ? this._array[index] : index;
		}

		internal bool HasDuplicates
		{
			get
			{
				if (!this.know_have_duplicates && !this.know_no_duplicates)
				{
					for (int i = 0; i < this._size - 1; i++)
					{
						if (this.Key.CompareRecords(this._array[i], this._array[i + 1]) == 0)
						{
							this.know_have_duplicates = true;
							break;
						}
					}
					this.know_no_duplicates = !this.know_have_duplicates;
				}
				return this.know_have_duplicates;
			}
		}

		internal int[] Duplicates
		{
			get
			{
				if (!this.HasDuplicates)
				{
					return null;
				}
				ArrayList arrayList = new ArrayList();
				bool flag = false;
				for (int i = 0; i < this._size - 1; i++)
				{
					if (this.Key.CompareRecords(this._array[i], this._array[i + 1]) == 0)
					{
						if (!flag)
						{
							arrayList.Add(this._array[i]);
							flag = true;
						}
						arrayList.Add(this._array[i + 1]);
					}
					else
					{
						flag = false;
					}
				}
				return (int[])arrayList.ToArray(typeof(int));
			}
		}

		internal int[] GetAll()
		{
			return this._array;
		}

		internal DataRow[] GetAllRows()
		{
			DataRow[] array = new DataRow[this._size];
			for (int i = 0; i < this._size; i++)
			{
				array[i] = this.Key.Table.RecordCache[this._array[i]];
			}
			return array;
		}

		internal DataRow[] GetDistinctRows()
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(this.Key.Table.RecordCache[this._array[0]]);
			int num = this._array[0];
			for (int i = 1; i < this._size; i++)
			{
				if (this.Key.CompareRecords(num, this._array[i]) != 0)
				{
					arrayList.Add(this.Key.Table.RecordCache[this._array[i]]);
					num = this._array[i];
				}
			}
			return (DataRow[])arrayList.ToArray(typeof(DataRow));
		}

		internal void Reset()
		{
			this._array = Index.empty;
			this._size = 0;
			this.RebuildIndex();
		}

		private void RebuildIndex()
		{
			int currentCapacity = this.Key.Table.RecordCache.CurrentCapacity;
			if (currentCapacity == 0)
			{
				return;
			}
			this._array = new int[currentCapacity];
			this._size = 0;
			foreach (object obj in this.Key.Table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				int record = this.Key.GetRecord(dataRow);
				if (record != -1)
				{
					this._array[this._size++] = record;
				}
			}
			this.know_have_duplicates = (this.know_no_duplicates = false);
			this.Sort();
			this.know_no_duplicates = !this.know_have_duplicates;
		}

		private void Sort()
		{
			this.MergeSort(this._array, this._size);
		}

		internal int Find(object[] keys)
		{
			int num = this.FindIndex(keys);
			return this.IndexToRecord(num);
		}

		internal int FindIndex(object[] keys)
		{
			if (keys == null || keys.Length != this.Key.Columns.Length)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Expecting ",
					this.Key.Columns.Length,
					" value(s) for the key being indexed, but received ",
					(keys != null) ? keys.Length : 0,
					" value(s)."
				}));
			}
			int num = this.Key.Table.RecordCache.NewRecord();
			int num2;
			try
			{
				for (int i = 0; i < this.Key.Columns.Length; i++)
				{
					this.Key.Columns[i].DataContainer[num] = keys[i];
				}
				num2 = this.FindIndex(num);
			}
			finally
			{
				this.Key.Table.RecordCache.DisposeRecord(num);
			}
			return num2;
		}

		internal int Find(int record)
		{
			int num = this.FindIndex(record);
			return this.IndexToRecord(num);
		}

		internal int[] FindAll(object[] keys)
		{
			int[] array = this.FindAllIndexes(keys);
			this.IndexesToRecords(array);
			return array;
		}

		internal int[] FindAllIndexes(object[] keys)
		{
			if (keys == null || keys.Length != this.Key.Columns.Length)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Expecting ",
					this.Key.Columns.Length,
					" value(s) for the key being indexed,but received ",
					(keys != null) ? keys.Length : 0,
					" value(s)."
				}));
			}
			int num = this.Key.Table.RecordCache.NewRecord();
			int[] array;
			try
			{
				for (int i = 0; i < this.Key.Columns.Length; i++)
				{
					this.Key.Columns[i].DataContainer[num] = keys[i];
				}
				array = this.FindAllIndexes(num);
			}
			catch (FormatException)
			{
				array = Index.empty;
			}
			catch (InvalidCastException)
			{
				array = Index.empty;
			}
			finally
			{
				this.Key.Table.RecordCache.DisposeRecord(num);
			}
			return array;
		}

		internal int[] FindAll(int record)
		{
			int[] array = this.FindAllIndexes(record);
			this.IndexesToRecords(array);
			return array;
		}

		internal int[] FindAllIndexes(int record)
		{
			int num = this.FindIndex(record);
			if (num == -1)
			{
				return Index.empty;
			}
			int num2 = num++;
			int num3 = num;
			while (num2 >= 0 && this.Key.CompareRecords(this._array[num2], record) == 0)
			{
				num2--;
			}
			while (num3 < this._size && this.Key.CompareRecords(this._array[num3], record) == 0)
			{
				num3++;
			}
			int num4 = num3 - num2 - 1;
			int[] array = new int[num4];
			for (int i = 0; i < num4; i++)
			{
				num2 = (array[i] = num2 + 1);
			}
			return array;
		}

		private int FindIndex(int record)
		{
			if (this._size == 0)
			{
				return -1;
			}
			return this.BinarySearch(this._array, 0, this._size - 1, record);
		}

		private int FindIndexExact(int record)
		{
			int i = 0;
			int size = this._size;
			while (i < size)
			{
				if (this._array[i] == record)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		private void IndexesToRecords(int[] indexes)
		{
			for (int i = 0; i < indexes.Length; i++)
			{
				indexes[i] = this._array[indexes[i]];
			}
		}

		internal void Delete(DataRow row)
		{
			int record = this.Key.GetRecord(row);
			this.Delete(record);
		}

		internal void Delete(int oldRecord)
		{
			if (oldRecord == -1)
			{
				return;
			}
			int num = this.FindIndexExact(oldRecord);
			if (num != -1)
			{
				if (this.know_have_duplicates)
				{
					int num2 = 1;
					int num3 = 1;
					if (num > 0)
					{
						num2 = this.Key.CompareRecords(this._array[num - 1], oldRecord);
					}
					if (num < this._size - 1)
					{
						num3 = this.Key.CompareRecords(this._array[num + 1], oldRecord);
					}
					if ((num2 == 0) ^ (num3 == 0))
					{
						this.know_have_duplicates = (this.know_no_duplicates = false);
					}
				}
				this.Remove(num);
			}
		}

		private void Remove(int index)
		{
			if (this._size > 1)
			{
				Array.Copy(this._array, index + 1, this._array, index, this._size - index - 1);
			}
			this._size--;
		}

		internal void Update(DataRow row, int oldRecord, DataRowVersion oldVersion, DataRowState oldState)
		{
			bool flag = this.Key.ContainsVersion(oldState, oldVersion);
			int record = this.Key.GetRecord(row);
			if (oldRecord == -1 || this._size == 0 || !flag)
			{
				if (record >= 0 && this.FindIndexExact(record) < 0)
				{
					this.Add(row, record);
				}
				return;
			}
			if (record < 0 || !this.Key.CanContain(record))
			{
				this.Delete(oldRecord);
				return;
			}
			int num = this.FindIndexExact(oldRecord);
			if (num == -1)
			{
				this.Add(row, record);
				return;
			}
			int num2 = this.Key.CompareRecords(this._array[num], record);
			int num3 = 1;
			int num4 = 1;
			if (num2 == 0)
			{
				if (this._array[num] == record)
				{
					return;
				}
			}
			else if (this.know_have_duplicates)
			{
				if (num > 0)
				{
					num3 = this.Key.CompareRecords(this._array[num - 1], record);
				}
				if (num < this._size - 1)
				{
					num4 = this.Key.CompareRecords(this._array[num + 1], record);
				}
				if (((num3 == 0) ^ (num4 == 0)) && num2 != 0)
				{
					this.know_have_duplicates = (this.know_no_duplicates = false);
				}
			}
			int num5;
			if ((num == 0 && num2 > 0) || (num == this._size - 1 && num2 < 0) || num2 == 0)
			{
				num5 = num;
			}
			else
			{
				int num6;
				int num7;
				if (num2 < 0)
				{
					num6 = num + 1;
					num7 = this._size - 1;
				}
				else
				{
					num6 = 0;
					num7 = num - 1;
				}
				num5 = this.LazyBinarySearch(this._array, num6, num7, record);
				if (num < num5)
				{
					Array.Copy(this._array, num + 1, this._array, num, num5 - num);
					if (this.Key.CompareRecords(this._array[num5], record) > 0)
					{
						num5--;
					}
				}
				else if (num > num5)
				{
					Array.Copy(this._array, num5, this._array, num5 + 1, num - num5);
					if (this.Key.CompareRecords(this._array[num5], record) < 0)
					{
						num5++;
					}
				}
			}
			this._array[num5] = record;
			if (num2 != 0 && !this.know_have_duplicates)
			{
				if (num5 > 0)
				{
					num3 = this.Key.CompareRecords(this._array[num5 - 1], record);
				}
				if (num5 < this._size - 1)
				{
					num4 = this.Key.CompareRecords(this._array[num5 + 1], record);
				}
				if (num3 == 0 || num4 == 0)
				{
					this.know_have_duplicates = true;
				}
			}
		}

		internal void Add(DataRow row)
		{
			this.Add(row, this.Key.GetRecord(row));
		}

		private void Add(DataRow row, int newRecord)
		{
			if (newRecord < 0 || !this.Key.CanContain(newRecord))
			{
				return;
			}
			int num;
			if (this._size == 0)
			{
				num = 0;
			}
			else
			{
				num = this.LazyBinarySearch(this._array, 0, this._size - 1, newRecord);
				if (this.Key.CompareRecords(this._array[num], newRecord) < 0)
				{
					num++;
				}
			}
			this.Insert(num, newRecord);
			int num2 = 1;
			int num3 = 1;
			if (!this.know_have_duplicates)
			{
				if (num > 0)
				{
					num2 = this.Key.CompareRecords(this._array[num - 1], newRecord);
				}
				if (num < this._size - 1)
				{
					num3 = this.Key.CompareRecords(this._array[num + 1], newRecord);
				}
				if (num2 == 0 || num3 == 0)
				{
					this.know_have_duplicates = true;
				}
			}
		}

		private void Insert(int index, int r)
		{
			if (this._array.Length == this._size)
			{
				int[] array = ((this._size != 0) ? new int[this._size << 1] : new int[16]);
				Array.Copy(this._array, 0, array, 0, index);
				array[index] = r;
				Array.Copy(this._array, index, array, index + 1, this._size - index);
				this._array = array;
			}
			else
			{
				Array.Copy(this._array, index, this._array, index + 1, this._size - index);
				this._array[index] = r;
			}
			this._size++;
		}

		private void MergeSort(int[] to, int length)
		{
			int[] array = new int[length];
			Array.Copy(to, 0, array, 0, array.Length);
			this.MergeSort(array, to, 0, array.Length);
		}

		private void MergeSort(int[] from, int[] to, int p, int r)
		{
			int i = p + r >> 1;
			if (i == p)
			{
				return;
			}
			this.MergeSort(to, from, p, i);
			this.MergeSort(to, from, i, r);
			int num = i;
			int num2 = p;
			for (;;)
			{
				int num3 = this.Key.CompareRecords(from[p], from[i]);
				if (num3 > 0)
				{
					to[num2++] = from[i++];
					if (i == r)
					{
						break;
					}
				}
				else
				{
					if (num3 == 0)
					{
						this.know_have_duplicates = true;
					}
					to[num2++] = from[p++];
					if (p == num)
					{
						goto Block_6;
					}
				}
			}
			while (p < num)
			{
				to[num2++] = from[p++];
			}
			return;
			Block_6:
			while (i < r)
			{
				to[num2++] = from[i++];
			}
		}

		private void QuickSort(int[] a, int p, int r)
		{
			if (p < r)
			{
				int num = this.Partition(a, p, r);
				this.QuickSort(a, p, num);
				this.QuickSort(a, num + 1, r);
			}
		}

		private int Partition(int[] a, int p, int r)
		{
			int num = a[p];
			int num2 = p - 1;
			int num3 = r + 1;
			for (;;)
			{
				num3--;
				if (this.Key.CompareRecords(a[num3], num) <= 0)
				{
					do
					{
						num2++;
					}
					while (this.Key.CompareRecords(a[num2], num) < 0);
					if (num2 >= num3)
					{
						break;
					}
					int num4 = a[num3];
					a[num3] = a[num2];
					a[num2] = num4;
				}
			}
			return num3;
		}

		private int BinarySearch(int[] a, int p, int r, int b)
		{
			int num = this.LazyBinarySearch(a, p, r, b);
			return (this.Key.CompareRecords(a[num], b) != 0) ? (-1) : num;
		}

		private int LazyBinarySearch(int[] a, int p, int r, int b)
		{
			if (p == r)
			{
				return p;
			}
			int num = p + r >> 1;
			int num2 = this.Key.CompareRecords(a[num], b);
			if (num2 < 0)
			{
				return this.LazyBinarySearch(a, num + 1, r, b);
			}
			if (num2 > 0)
			{
				return this.LazyBinarySearch(a, p, num, b);
			}
			return num;
		}

		internal void AddRef()
		{
			this._refCount++;
		}

		internal void RemoveRef()
		{
			this._refCount--;
		}

		private static readonly int[] empty = new int[0];

		private int[] _array;

		private int _size;

		private Key _key;

		private int _refCount;

		private bool know_have_duplicates;

		private bool know_no_duplicates;
	}
}
