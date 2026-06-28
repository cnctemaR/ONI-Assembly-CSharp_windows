using System;

namespace System.Configuration
{
	public class KeyValueConfigurationElement : ConfigurationElement
	{
		internal KeyValueConfigurationElement()
		{
		}

		public KeyValueConfigurationElement(string key, string value)
		{
			base[KeyValueConfigurationElement.keyProp] = key;
			base[KeyValueConfigurationElement.valueProp] = value;
		}

		static KeyValueConfigurationElement()
		{
			KeyValueConfigurationElement.properties.Add(KeyValueConfigurationElement.keyProp);
			KeyValueConfigurationElement.properties.Add(KeyValueConfigurationElement.valueProp);
		}

		[ConfigurationProperty("key", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
		public string Key
		{
			get
			{
				return (string)base[KeyValueConfigurationElement.keyProp];
			}
		}

		[ConfigurationProperty("value", DefaultValue = "")]
		public string Value
		{
			get
			{
				return (string)base[KeyValueConfigurationElement.valueProp];
			}
			set
			{
				base[KeyValueConfigurationElement.valueProp] = value;
			}
		}

		[MonoTODO]
		protected internal override void Init()
		{
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return KeyValueConfigurationElement.properties;
			}
		}

		private static ConfigurationProperty keyProp = new ConfigurationProperty("key", typeof(string), string.Empty, ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty valueProp = new ConfigurationProperty("value", typeof(string), string.Empty);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
