using System;

namespace System.Configuration
{
	public sealed class NameValueConfigurationElement : ConfigurationElement
	{
		public NameValueConfigurationElement(string name, string value)
		{
			base[NameValueConfigurationElement._propName] = name;
			base[NameValueConfigurationElement._propValue] = value;
		}

		static NameValueConfigurationElement()
		{
			NameValueConfigurationElement._properties.Add(NameValueConfigurationElement._propName);
			NameValueConfigurationElement._properties.Add(NameValueConfigurationElement._propValue);
		}

		[ConfigurationProperty("name", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
		public string Name
		{
			get
			{
				return (string)base[NameValueConfigurationElement._propName];
			}
		}

		[ConfigurationProperty("value", DefaultValue = "", Options = ConfigurationPropertyOptions.None)]
		public string Value
		{
			get
			{
				return (string)base[NameValueConfigurationElement._propValue];
			}
			set
			{
				base[NameValueConfigurationElement._propValue] = value;
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return NameValueConfigurationElement._properties;
			}
		}

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), string.Empty, ConfigurationPropertyOptions.IsKey);

		private static readonly ConfigurationProperty _propValue = new ConfigurationProperty("value", typeof(string), string.Empty);
	}
}
