using System;

namespace System.Configuration
{
	[ConfigurationCollection(typeof(ProviderSettings), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class ProviderSettingsCollection : ConfigurationElementCollection
	{
		public void Add(ProviderSettings provider)
		{
			this.BaseAdd(provider);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ProviderSettings();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ProviderSettings)element).Name;
		}

		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		public ProviderSettings this[int index]
		{
			get
			{
				return (ProviderSettings)base.BaseGet(index);
			}
			set
			{
				this.BaseAdd(index, value);
			}
		}

		public ProviderSettings this[string key]
		{
			get
			{
				return (ProviderSettings)base.BaseGet(key);
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ProviderSettingsCollection.props;
			}
		}

		private static ConfigurationPropertyCollection props = new ConfigurationPropertyCollection();
	}
}
