using System;

namespace System.Configuration
{
	public sealed class ConnectionStringsSection : ConfigurationSection
	{
		static ConnectionStringsSection()
		{
			ConnectionStringsSection._properties.Add(ConnectionStringsSection._propConnectionStrings);
		}

		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ConnectionStringSettingsCollection ConnectionStrings
		{
			get
			{
				return (ConnectionStringSettingsCollection)base[ConnectionStringsSection._propConnectionStrings];
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ConnectionStringsSection._properties;
			}
		}

		protected internal override object GetRuntimeObject()
		{
			return base.GetRuntimeObject();
		}

		private static readonly ConfigurationProperty _propConnectionStrings = new ConfigurationProperty("", typeof(ConnectionStringSettingsCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
	}
}
