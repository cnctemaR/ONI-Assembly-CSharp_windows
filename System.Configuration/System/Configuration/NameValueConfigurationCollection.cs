using System;

namespace System.Configuration
{
	[ConfigurationCollection(typeof(NameValueConfigurationElement), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class NameValueConfigurationCollection : ConfigurationElementCollection
	{
		public string[] AllKeys
		{
			get
			{
				return (string[])base.BaseGetAllKeys();
			}
		}

		public NameValueConfigurationElement this[string name]
		{
			get
			{
				return (NameValueConfigurationElement)base.BaseGet(name);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return NameValueConfigurationCollection.properties;
			}
		}

		public void Add(NameValueConfigurationElement nameValue)
		{
			base.BaseAdd(nameValue, false);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new NameValueConfigurationElement("", "");
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((NameValueConfigurationElement)element).Name;
		}

		public void Remove(NameValueConfigurationElement nameValue)
		{
			throw new NotImplementedException();
		}

		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
