using System;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Collections.Specialized
{
	[global::System.ComponentModel.Design.Serialization.DesignerSerializer("System.Diagnostics.Design.StringDictionaryCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Serializable]
	public class StringDictionary : IEnumerable
	{
		public StringDictionary()
		{
			this.contents = new Hashtable();
		}

		public virtual int Count
		{
			get
			{
				return this.contents.Count;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual string this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				return (string)this.contents[key.ToLower(CultureInfo.InvariantCulture)];
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this.contents[key.ToLower(CultureInfo.InvariantCulture)] = value;
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				return this.contents.Keys;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				return this.contents.Values;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this.contents.SyncRoot;
			}
		}

		public virtual void Add(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.contents.Add(key.ToLower(CultureInfo.InvariantCulture), value);
		}

		public virtual void Clear()
		{
			this.contents.Clear();
		}

		public virtual bool ContainsKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this.contents.ContainsKey(key.ToLower(CultureInfo.InvariantCulture));
		}

		public virtual bool ContainsValue(string value)
		{
			return this.contents.ContainsValue(value);
		}

		public virtual void CopyTo(Array array, int index)
		{
			this.contents.CopyTo(array, index);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.contents.GetEnumerator();
		}

		public virtual void Remove(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.contents.Remove(key.ToLower(CultureInfo.InvariantCulture));
		}

		private Hashtable contents;
	}
}
