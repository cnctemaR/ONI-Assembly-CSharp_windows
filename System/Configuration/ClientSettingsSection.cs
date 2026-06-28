using System;

namespace System.Configuration
{
	public sealed class ClientSettingsSection : ConfigurationSection
	{
		static ClientSettingsSection()
		{
			ClientSettingsSection.properties.Add(ClientSettingsSection.settings_prop);
		}

		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public SettingElementCollection Settings
		{
			get
			{
				return (SettingElementCollection)base[ClientSettingsSection.settings_prop];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ClientSettingsSection.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty settings_prop = new ConfigurationProperty(string.Empty, typeof(SettingElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
