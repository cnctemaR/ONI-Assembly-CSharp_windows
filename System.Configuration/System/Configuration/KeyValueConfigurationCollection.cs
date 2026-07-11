using System;

namespace System.Configuration
{
	[ConfigurationCollection(typeof(KeyValueConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class KeyValueConfigurationCollection : ConfigurationElementCollection
	{
		public void Add(KeyValueConfigurationElement keyValue)
		{
			keyValue.Init();
			this.BaseAdd(keyValue);
		}

		public void Add(string key, string value)
		{
			this.Add(new KeyValueConfigurationElement(key, value));
		}

		public void Clear()
		{
			base.BaseClear();
		}

		public void Remove(string key)
		{
			base.BaseRemove(key);
		}

		public string[] AllKeys
		{
			get
			{
				string[] array = new string[base.Count];
				int num = 0;
				foreach (object obj in this)
				{
					KeyValueConfigurationElement keyValueConfigurationElement = (KeyValueConfigurationElement)obj;
					array[num++] = keyValueConfigurationElement.Key;
				}
				return array;
			}
		}

		public KeyValueConfigurationElement this[string key]
		{
			get
			{
				return (KeyValueConfigurationElement)base.BaseGet(key);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new KeyValueConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((KeyValueConfigurationElement)element).Key;
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection();
				}
				return this.properties;
			}
		}

		protected override bool ThrowOnDuplicate
		{
			get
			{
				return false;
			}
		}

		private ConfigurationPropertyCollection properties;
	}
}
