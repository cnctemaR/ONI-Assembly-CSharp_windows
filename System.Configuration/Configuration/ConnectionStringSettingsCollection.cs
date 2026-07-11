using System;
using System.Globalization;

namespace System.Configuration
{
	[ConfigurationCollection(typeof(ConnectionStringSettings), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class ConnectionStringSettingsCollection : ConfigurationElementCollection
	{
		public ConnectionStringSettings this[string name]
		{
			get
			{
				foreach (object obj in this)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)obj;
					if (configurationElement is ConnectionStringSettings && string.Compare(((ConnectionStringSettings)configurationElement).Name, name, true, CultureInfo.InvariantCulture) == 0)
					{
						return configurationElement as ConnectionStringSettings;
					}
				}
				return null;
			}
		}

		public ConnectionStringSettings this[int index]
		{
			get
			{
				return (ConnectionStringSettings)base.BaseGet(index);
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}

		[MonoTODO]
		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return base.Properties;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ConnectionStringSettings();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ConnectionStringSettings)element).Name;
		}

		public void Add(ConnectionStringSettings settings)
		{
			this.BaseAdd(settings);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		public int IndexOf(ConnectionStringSettings settings)
		{
			return base.BaseIndexOf(settings);
		}

		public void Remove(ConnectionStringSettings settings)
		{
			base.BaseRemove(settings.Name);
		}

		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}

		protected override void BaseAdd(int index, ConfigurationElement element)
		{
			if (!(element is ConnectionStringSettings))
			{
				base.BaseAdd(element);
			}
			if (this.IndexOf((ConnectionStringSettings)element) >= 0)
			{
				throw new ConfigurationErrorsException(string.Format("The element {0} already exist!", ((ConnectionStringSettings)element).Name));
			}
			this[index] = (ConnectionStringSettings)element;
		}
	}
}
