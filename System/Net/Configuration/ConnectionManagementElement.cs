using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class ConnectionManagementElement : ConfigurationElement
	{
		public ConnectionManagementElement()
		{
		}

		public ConnectionManagementElement(string address, int maxConnection)
		{
			this.Address = address;
			this.MaxConnection = maxConnection;
		}

		static ConnectionManagementElement()
		{
			ConnectionManagementElement.properties.Add(ConnectionManagementElement.addressProp);
			ConnectionManagementElement.properties.Add(ConnectionManagementElement.maxConnectionProp);
		}

		[ConfigurationProperty("address", Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey)]
		public string Address
		{
			get
			{
				return (string)base[ConnectionManagementElement.addressProp];
			}
			set
			{
				base[ConnectionManagementElement.addressProp] = value;
			}
		}

		[ConfigurationProperty("maxconnection", DefaultValue = "1", Options = ConfigurationPropertyOptions.IsRequired)]
		public int MaxConnection
		{
			get
			{
				return (int)base[ConnectionManagementElement.maxConnectionProp];
			}
			set
			{
				base[ConnectionManagementElement.maxConnectionProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ConnectionManagementElement.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty addressProp = new ConfigurationProperty("address", typeof(string), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty maxConnectionProp = new ConfigurationProperty("maxconnection", typeof(int), 1, ConfigurationPropertyOptions.IsRequired);
	}
}
