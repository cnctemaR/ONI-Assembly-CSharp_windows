using System;
using System.Collections;

namespace System.Configuration
{
	public class SettingsPropertyCollection : ICloneable, ICollection, IEnumerable
	{
		public SettingsPropertyCollection()
		{
			this.items = new Hashtable();
		}

		public void Add(SettingsProperty property)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			this.OnAdd(property);
			this.items.Add(property.Name, property);
			this.OnAddComplete(property);
		}

		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			this.OnClear();
			this.items.Clear();
			this.OnClearComplete();
		}

		public object Clone()
		{
			return new SettingsPropertyCollection
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
			SettingsProperty settingsProperty = (SettingsProperty)this.items[name];
			this.OnRemove(settingsProperty);
			this.items.Remove(name);
			this.OnRemoveComplete(settingsProperty);
		}

		public void SetReadOnly()
		{
			this.isReadOnly = true;
		}

		protected virtual void OnAdd(SettingsProperty property)
		{
		}

		protected virtual void OnAddComplete(SettingsProperty property)
		{
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnRemove(SettingsProperty property)
		{
		}

		protected virtual void OnRemoveComplete(SettingsProperty property)
		{
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
				return false;
			}
		}

		public SettingsProperty this[string name]
		{
			get
			{
				return (SettingsProperty)this.items[name];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		private Hashtable items;

		private bool isReadOnly;
	}
}
