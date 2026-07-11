using System;
using System.Collections;

namespace System.Configuration
{
	public class SettingsPropertyValueCollection : ICollection, IEnumerable, ICloneable
	{
		public SettingsPropertyValueCollection()
		{
			this.items = new Hashtable();
		}

		public void Add(SettingsPropertyValue property)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			this.items.Add(property.Name, property);
		}

		internal void Add(SettingsPropertyValueCollection vals)
		{
			foreach (object obj in vals)
			{
				SettingsPropertyValue settingsPropertyValue = (SettingsPropertyValue)obj;
				this.Add(settingsPropertyValue);
			}
		}

		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			this.items.Clear();
		}

		public object Clone()
		{
			return new SettingsPropertyValueCollection
			{
				items = (Hashtable)this.items.Clone()
			};
		}

		public void CopyTo(Array array, int index)
		{
			this.items.Values.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.items.Values.GetEnumerator();
		}

		public void Remove(string name)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			this.items.Remove(name);
		}

		public void SetReadOnly()
		{
			this.isReadOnly = true;
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public SettingsPropertyValue this[string name]
		{
			get
			{
				return (SettingsPropertyValue)this.items[name];
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private Hashtable items;

		private bool isReadOnly;
	}
}
