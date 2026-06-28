using System;
using System.Collections;

namespace System.Configuration.Provider
{
	public class ProviderCollection : ICollection, IEnumerable
	{
		public ProviderCollection()
		{
			this.lookup = new Hashtable(10, StringComparer.InvariantCultureIgnoreCase);
			this.values = new ArrayList();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.values.CopyTo(array, index);
		}

		public virtual void Add(ProviderBase provider)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			if (provider == null || provider.Name == null)
			{
				throw new ArgumentNullException();
			}
			int num = this.values.Add(provider);
			try
			{
				this.lookup.Add(provider.Name, num);
			}
			catch
			{
				this.values.RemoveAt(num);
				throw;
			}
		}

		public void Clear()
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.values.Clear();
			this.lookup.Clear();
		}

		public void CopyTo(ProviderBase[] array, int index)
		{
			this.values.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.values.GetEnumerator();
		}

		public void Remove(string name)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			object obj = this.lookup[name];
			if (obj == null || !(obj is int))
			{
				throw new ArgumentException();
			}
			int num = (int)obj;
			if (num >= this.values.Count)
			{
				throw new ArgumentException();
			}
			this.values.RemoveAt(num);
			this.lookup.Remove(name);
			ArrayList arrayList = new ArrayList();
			foreach (object obj2 in this.lookup)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
				if ((int)dictionaryEntry.Value > num)
				{
					arrayList.Add(dictionaryEntry.Key);
				}
			}
			foreach (object obj3 in arrayList)
			{
				string text = (string)obj3;
				this.lookup[text] = (int)this.lookup[text] - 1;
			}
		}

		public void SetReadOnly()
		{
			this.readOnly = true;
		}

		public int Count
		{
			get
			{
				return this.values.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public ProviderBase this[string name]
		{
			get
			{
				object obj = this.lookup[name];
				if (obj == null)
				{
					return null;
				}
				return this.values[(int)obj] as ProviderBase;
			}
		}

		private Hashtable lookup;

		private bool readOnly;

		private ArrayList values;
	}
}
