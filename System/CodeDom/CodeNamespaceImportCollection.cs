using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeNamespaceImportCollection : IList, ICollection, IEnumerable
	{
		public CodeNamespaceImportCollection()
		{
			this.data = new ArrayList();
			this.keys = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		}

		int ICollection.Count
		{
			get
			{
				return this.data.Count;
			}
		}

		void IList.Clear()
		{
			this.Clear();
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.data[index];
			}
			set
			{
				this[index] = (CodeNamespaceImport)value;
			}
		}

		int IList.Add(object value)
		{
			this.Add((CodeNamespaceImport)value);
			return this.data.Count - 1;
		}

		bool IList.Contains(object value)
		{
			return this.data.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.data.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			this.data.Insert(index, value);
			CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)value;
			this.keys[codeNamespaceImport.Namespace] = codeNamespaceImport;
		}

		void IList.Remove(object value)
		{
			string @namespace = ((CodeNamespaceImport)value).Namespace;
			this.data.Remove(value);
			foreach (object obj in this.data)
			{
				CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)obj;
				if (codeNamespaceImport.Namespace == @namespace)
				{
					this.keys[@namespace] = codeNamespaceImport;
					return;
				}
			}
			this.keys.Remove(@namespace);
		}

		void IList.RemoveAt(int index)
		{
			string @namespace = this[index].Namespace;
			this.data.RemoveAt(index);
			foreach (object obj in this.data)
			{
				CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)obj;
				if (codeNamespaceImport.Namespace == @namespace)
				{
					this.keys[@namespace] = codeNamespaceImport;
					return;
				}
			}
			this.keys.Remove(@namespace);
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.data.IsSynchronized;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.data.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.data.Count;
			}
		}

		public CodeNamespaceImport this[int index]
		{
			get
			{
				return (CodeNamespaceImport)this.data[index];
			}
			set
			{
				CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)this.data[index];
				this.keys.Remove(codeNamespaceImport.Namespace);
				this.data[index] = value;
				this.keys[value.Namespace] = value;
			}
		}

		public void Add(CodeNamespaceImport value)
		{
			if (value == null)
			{
				throw new NullReferenceException();
			}
			if (!this.keys.ContainsKey(value.Namespace))
			{
				this.keys[value.Namespace] = value;
				this.data.Add(value);
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
			this.data.Clear();
			this.keys.Clear();
		}

		public IEnumerator GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		private Hashtable keys;

		private ArrayList data;
	}
}
