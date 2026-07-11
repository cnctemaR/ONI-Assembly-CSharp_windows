using System;

namespace System.Configuration
{
	public class ProtectedProviderSettings : ConfigurationElement
	{
		static ProtectedProviderSettings()
		{
			ProtectedProviderSettings.properties.Add(ProtectedProviderSettings.providersProp);
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ProtectedProviderSettings.properties;
			}
		}

		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ProviderSettingsCollection Providers
		{
			get
			{
				return (ProviderSettingsCollection)base[ProtectedProviderSettings.providersProp];
			}
		}

		private static ConfigurationProperty providersProp = new ConfigurationProperty("", typeof(ProviderSettingsCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
