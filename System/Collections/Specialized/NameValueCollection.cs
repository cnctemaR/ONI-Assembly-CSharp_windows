using System;
using System.Runtime.Serialization;
using System.Text;

namespace System.Collections.Specialized
{
	[Serializable]
	public class NameValueCollection : NameObjectCollectionBase
	{
		public NameValueCollection()
		{
		}

		public NameValueCollection(int capacity)
			: base(capacity)
		{
		}

		public NameValueCollection(NameValueCollection col)
		{
			IEqualityComparer equalityComparer2;
			if (col == null)
			{
				IEqualityComparer equalityComparer = null;
				equalityComparer2 = equalityComparer;
			}
			else
			{
				equalityComparer2 = col.EqualityComparer;
			}
			IComparer comparer2;
			if (col == null)
			{
				IComparer comparer = null;
				comparer2 = comparer;
			}
			else
			{
				comparer2 = col.Comparer;
			}
			IHashCodeProvider hashCodeProvider2;
			if (col == null)
			{
				IHashCodeProvider hashCodeProvider = null;
				hashCodeProvider2 = hashCodeProvider;
			}
			else
			{
				hashCodeProvider2 = col.HashCodeProvider;
			}
			base..ctor(equalityComparer2, comparer2, hashCodeProvider2);
			if (col == null)
			{
				throw new ArgumentNullException("col");
			}
			this.Add(col);
		}

		[Obsolete("Use NameValueCollection (IEqualityComparer)")]
		public NameValueCollection(IHashCodeProvider hashProvider, IComparer comparer)
			: base(hashProvider, comparer)
		{
		}

		public NameValueCollection(int capacity, NameValueCollection col)
		{
			IHashCodeProvider hashCodeProvider2;
			if (col == null)
			{
				IHashCodeProvider hashCodeProvider = null;
				hashCodeProvider2 = hashCodeProvider;
			}
			else
			{
				hashCodeProvider2 = col.HashCodeProvider;
			}
			IComparer comparer2;
			if (col == null)
			{
				IComparer comparer = null;
				comparer2 = comparer;
			}
			else
			{
				comparer2 = col.Comparer;
			}
			base..ctor(capacity, hashCodeProvider2, comparer2);
			this.Add(col);
		}

		protected NameValueCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[Obsolete("Use NameValueCollection (IEqualityComparer)")]
		public NameValueCollection(int capacity, IHashCodeProvider hashProvider, IComparer comparer)
			: base(capacity, hashProvider, comparer)
		{
		}

		public NameValueCollection(IEqualityComparer equalityComparer)
			: base(equalityComparer)
		{
		}

		public NameValueCollection(int capacity, IEqualityComparer equalityComparer)
			: base(capacity, equalityComparer)
		{
		}

		public virtual string[] AllKeys
		{
			get
			{
				if (this.cachedAllKeys == null)
				{
					this.cachedAllKeys = base.BaseGetAllKeys();
				}
				return this.cachedAllKeys;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.Get(index);
			}
		}

		public string this[string name]
		{
			get
			{
				return this.Get(name);
			}
			set
			{
				this.Set(name, value);
			}
		}

		public void Add(NameValueCollection c)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			this.InvalidateCachedArrays();
			int count = c.Count;
			for (int i = 0; i < count; i++)
			{
				string key = c.GetKey(i);
				ArrayList arrayList = (ArrayList)c.BaseGet(i);
				ArrayList arrayList2 = (ArrayList)base.BaseGet(key);
				if (arrayList2 != null && arrayList != null)
				{
					arrayList2.AddRange(arrayList);
				}
				else if (arrayList != null)
				{
					arrayList2 = new ArrayList(arrayList);
				}
				base.BaseSet(key, arrayList2);
			}
		}

		public virtual void Add(string name, string val)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			this.InvalidateCachedArrays();
			ArrayList arrayList = (ArrayList)base.BaseGet(name);
			if (arrayList == null)
			{
				arrayList = new ArrayList();
				if (val != null)
				{
					arrayList.Add(val);
				}
				base.BaseAdd(name, arrayList);
			}
			else if (val != null)
			{
				arrayList.Add(val);
			}
		}

		public virtual void Clear()
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			this.InvalidateCachedArrays();
			base.BaseClear();
		}

		public void CopyTo(Array dest, int index)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest", "Null argument - dest");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "index is less than 0");
			}
			if (dest.Rank > 1)
			{
				throw new ArgumentException("dest", "multidim");
			}
			if (this.cachedAll == null)
			{
				this.RefreshCachedAll();
			}
			try
			{
				this.cachedAll.CopyTo(dest, index);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new InvalidCastException();
			}
		}

		private void RefreshCachedAll()
		{
			this.cachedAll = null;
			int count = this.Count;
			this.cachedAll = new string[count];
			for (int i = 0; i < count; i++)
			{
				this.cachedAll[i] = this.Get(i);
			}
		}

		public virtual string Get(int index)
		{
			ArrayList arrayList = (ArrayList)base.BaseGet(index);
			return NameValueCollection.AsSingleString(arrayList);
		}

		public virtual string Get(string name)
		{
			ArrayList arrayList = (ArrayList)base.BaseGet(name);
			return NameValueCollection.AsSingleString(arrayList);
		}

		private static string AsSingleString(ArrayList values)
		{
			if (values == null)
			{
				return null;
			}
			int count = values.Count;
			switch (count)
			{
			case 0:
				return null;
			case 1:
				return (string)values[0];
			case 2:
				return (string)values[0] + ',' + (string)values[1];
			default:
			{
				int num = count;
				for (int i = 0; i < count; i++)
				{
					num += ((string)values[i]).Length;
				}
				StringBuilder stringBuilder = new StringBuilder((string)values[0], num);
				for (int j = 1; j < count; j++)
				{
					stringBuilder.Append(',');
					stringBuilder.Append(values[j]);
				}
				return stringBuilder.ToString();
			}
			}
		}

		public virtual string GetKey(int index)
		{
			return base.BaseGetKey(index);
		}

		public virtual string[] GetValues(int index)
		{
			ArrayList arrayList = (ArrayList)base.BaseGet(index);
			return NameValueCollection.AsStringArray(arrayList);
		}

		public virtual string[] GetValues(string name)
		{
			ArrayList arrayList = (ArrayList)base.BaseGet(name);
			return NameValueCollection.AsStringArray(arrayList);
		}

		private static string[] AsStringArray(ArrayList values)
		{
			if (values == null)
			{
				return null;
			}
			int count = values.Count;
			if (count == 0)
			{
				return null;
			}
			string[] array = new string[count];
			values.CopyTo(array);
			return array;
		}

		public bool HasKeys()
		{
			return base.BaseHasKeys();
		}

		public virtual void Remove(string name)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			this.InvalidateCachedArrays();
			base.BaseRemove(name);
		}

		public virtual void Set(string name, string value)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			this.InvalidateCachedArrays();
			ArrayList arrayList = new ArrayList();
			if (value != null)
			{
				arrayList.Add(value);
				base.BaseSet(name, arrayList);
			}
			else
			{
				base.BaseSet(name, null);
			}
		}

		protected void InvalidateCachedArrays()
		{
			this.cachedAllKeys = null;
			this.cachedAll = null;
		}

		private string[] cachedAllKeys;

		private string[] cachedAll;
	}
}
