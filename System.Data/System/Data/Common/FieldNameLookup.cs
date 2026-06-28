using System;
using System.Collections;

namespace System.Data.Common
{
	internal sealed class FieldNameLookup : IEnumerable, ICollection
	{
		public FieldNameLookup()
		{
			this.list = new ArrayList();
		}

		public FieldNameLookup(DataTable schemaTable)
			: this()
		{
			foreach (object obj in schemaTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				this.list.Add((string)dataRow["ColumnName"]);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public string this[int index]
		{
			get
			{
				return (string)this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public int Add(object value)
		{
			return this.list.Add(value);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(object value)
		{
			return this.list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			this.list.Insert(index, value);
		}

		public void Remove(object value)
		{
			this.list.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		private ArrayList list;
	}
}
