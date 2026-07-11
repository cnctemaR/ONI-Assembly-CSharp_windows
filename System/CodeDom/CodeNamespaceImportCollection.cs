using System;
using System.Collections;
using System.Collections.Generic;

namespace System.CodeDom
{
	[Serializable]
	public class CodeNamespaceImportCollection : IList, ICollection, IEnumerable
	{
		public CodeNamespaceImport this[int index]
		{
			get
			{
				return (CodeNamespaceImport)this._data[index];
			}
			set
			{
				this._data[index] = value;
				this.SyncKeys();
			}
		}

		public int Count
		{
			get
			{
				return this._data.Count;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public void Add(CodeNamespaceImport value)
		{
			if (!this._keys.ContainsKey(value.Namespace))
			{
				this._keys[value.Namespace] = value;
				this._data.Add(value);
			}
		}

		public void AddRange(CodeNamespaceImport[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			foreach (CodeNamespaceImport codeNamespaceImport in value)
			{
				this.Add(codeNamespaceImport);
			}
		}

		public void Clear()
		{
			this._data.Clear();
			this._keys.Clear();
		}

		private void SyncKeys()
		{
			this._keys.Clear();
			foreach (object obj in this._data)
			{
				CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)obj;
				this._keys[codeNamespaceImport.Namespace] = codeNamespaceImport;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this._data.GetEnumerator();
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (CodeNamespaceImport)value;
				this.SyncKeys();
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._data.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		int IList.Add(object value)
		{
			return this._data.Add((CodeNamespaceImport)value);
		}

		void IList.Clear()
		{
			this.Clear();
		}

		bool IList.Contains(object value)
		{
			return this._data.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this._data.IndexOf((CodeNamespaceImport)value);
		}

		void IList.Insert(int index, object value)
		{
			this._data.Insert(index, (CodeNamespaceImport)value);
			this.SyncKeys();
		}

		void IList.Remove(object value)
		{
			this._data.Remove((CodeNamespaceImport)value);
			this.SyncKeys();
		}

		void IList.RemoveAt(int index)
		{
			this._data.RemoveAt(index);
			this.SyncKeys();
		}

		private readonly ArrayList _data = new ArrayList();

		private readonly Dictionary<string, CodeNamespaceImport> _keys = new Dictionary<string, CodeNamespaceImport>(StringComparer.OrdinalIgnoreCase);
	}
}
