using System;
using System.Collections;

namespace System.Configuration
{
	public sealed class ConfigurationLockCollection : ICollection, IEnumerable
	{
		internal ConfigurationLockCollection(ConfigurationElement element, ConfigurationLockType lockType)
		{
			this.names = new ArrayList();
			this.element = element;
			this.lockType = lockType;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.names.CopyTo(array, index);
		}

		private void CheckName(string name)
		{
			bool flag = (this.lockType & ConfigurationLockType.Attribute) == ConfigurationLockType.Attribute;
			if (this.valid_name_hash == null)
			{
				this.valid_name_hash = new Hashtable();
				foreach (object obj in this.element.Properties)
				{
					ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
					if (flag != configurationProperty.IsElement)
					{
						this.valid_name_hash.Add(configurationProperty.Name, true);
					}
				}
				if (!flag)
				{
					ConfigurationElementCollection defaultCollection = this.element.GetDefaultCollection();
					this.valid_name_hash.Add(defaultCollection.AddElementName, true);
					this.valid_name_hash.Add(defaultCollection.ClearElementName, true);
					this.valid_name_hash.Add(defaultCollection.RemoveElementName, true);
				}
				string[] array = new string[this.valid_name_hash.Keys.Count];
				this.valid_name_hash.Keys.CopyTo(array, 0);
				this.valid_names = string.Join(",", array);
			}
			if (this.valid_name_hash[name] == null)
			{
				throw new ConfigurationErrorsException(string.Format("The {2} '{0}' is not valid in the locked list for this section.  The following {3} can be locked: '{1}'", new object[]
				{
					name,
					this.valid_names,
					(!flag) ? "element" : "attribute",
					(!flag) ? "elements" : "attributes"
				}));
			}
		}

		public void Add(string name)
		{
			this.CheckName(name);
			if (!this.names.Contains(name))
			{
				this.names.Add(name);
				this.is_modified = true;
			}
		}

		public void Clear()
		{
			this.names.Clear();
			this.is_modified = true;
		}

		public bool Contains(string name)
		{
			return this.names.Contains(name);
		}

		public void CopyTo(string[] array, int index)
		{
			this.names.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.names.GetEnumerator();
		}

		[MonoInternalNote("we can't possibly *always* return false here...")]
		public bool IsReadOnly(string name)
		{
			for (int i = 0; i < this.names.Count; i++)
			{
				if ((string)this.names[i] == name)
				{
					return false;
				}
			}
			throw new ConfigurationErrorsException(string.Format("The entry '{0}' is not in the collection.", name));
		}

		public void Remove(string name)
		{
			this.names.Remove(name);
			this.is_modified = true;
		}

		public void SetFromList(string attributeList)
		{
			this.Clear();
			char[] array = new char[] { ',' };
			string[] array2 = attributeList.Split(array);
			foreach (string text in array2)
			{
				this.Add(text.Trim());
			}
		}

		public string AttributeList
		{
			get
			{
				string[] array = new string[this.names.Count];
				this.names.CopyTo(array, 0);
				return string.Join(",", array);
			}
		}

		public int Count
		{
			get
			{
				return this.names.Count;
			}
		}

		[MonoTODO]
		public bool HasParentElements
		{
			get
			{
				return false;
			}
		}

		[MonoTODO]
		public bool IsModified
		{
			get
			{
				return this.is_modified;
			}
			internal set
			{
				this.is_modified = value;
			}
		}

		[MonoTODO]
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		[MonoTODO]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		private ArrayList names;

		private ConfigurationElement element;

		private ConfigurationLockType lockType;

		private bool is_modified;

		private Hashtable valid_name_hash;

		private string valid_names;
	}
}
