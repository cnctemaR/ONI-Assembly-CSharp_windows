using System;
using System.ComponentModel;

namespace System.Configuration
{
	public sealed class ConnectionStringSettings : ConfigurationElement
	{
		public ConnectionStringSettings()
		{
		}

		public ConnectionStringSettings(string name, string connectionString)
			: this(name, connectionString, string.Empty)
		{
		}

		public ConnectionStringSettings(string name, string connectionString, string providerName)
		{
			this.Name = name;
			this.ConnectionString = connectionString;
			this.ProviderName = providerName;
		}

		static ConnectionStringSettings()
		{
			ConnectionStringSettings._properties.Add(ConnectionStringSettings._propName);
			ConnectionStringSettings._properties.Add(ConnectionStringSettings._propProviderName);
			ConnectionStringSettings._properties.Add(ConnectionStringSettings._propConnectionString);
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ConnectionStringSettings._properties;
			}
		}

		[ConfigurationProperty("name", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey)]
		public string Name
		{
			get
			{
				return (string)base[ConnectionStringSettings._propName];
			}
			set
			{
				base[ConnectionStringSettings._propName] = value;
			}
		}

		[ConfigurationProperty("providerName", DefaultValue = "System.Data.SqlClient")]
		public string ProviderName
		{
			get
			{
				return (string)base[ConnectionStringSettings._propProviderName];
			}
			set
			{
				base[ConnectionStringSettings._propProviderName] = value;
			}
		}

		[ConfigurationProperty("connectionString", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired)]
		public string ConnectionString
		{
			get
			{
				return (string)base[ConnectionStringSettings._propConnectionString];
			}
			set
			{
				base[ConnectionStringSettings._propConnectionString] = value;
			}
		}

		public override string ToString()
		{
			return this.ConnectionString;
		}

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propConnectionString = new ConfigurationProperty("connectionString", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, TypeDescriptor.GetConverter(typeof(string)), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static readonly ConfigurationProperty _propProviderName = new ConfigurationProperty("providerName", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
	}
}
