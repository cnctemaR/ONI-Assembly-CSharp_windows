using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Configuration
{
	public class ConfigurationPropertyCollection : ICollection, IEnumerable
	{
		public ConfigurationPropertyCollection()
		{
			this.collection = new List<ConfigurationProperty>();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.collection).CopyTo(array, index);
		}

		public int Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		public ConfigurationProperty this[string name]
		{
			get
			{
				foreach (ConfigurationProperty configurationProperty in this.collection)
				{
					if (configurationProperty.Name == name)
					{
						return configurationProperty;
					}
				}
				return null;
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
				return this.collection;
			}
		}

		public void Add(ConfigurationProperty property)
		{
			this.collection.Add(property);
		}

		public bool Contains(string name)
		{
			ConfigurationProperty configurationProperty = this[name];
			return configurationProperty != null && this.collection.Contains(configurationProperty);
		}

		public void CopyTo(ConfigurationProperty[] array, int index)
		{
			this.collection.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.collection.GetEnumerator();
		}

		public bool Remove(string name)
		{
			return this.collection.Remove(this[name]);
		}

		public void Clear()
		{
			this.collection.Clear();
		}

		private List<ConfigurationProperty> collection;
	}
}
