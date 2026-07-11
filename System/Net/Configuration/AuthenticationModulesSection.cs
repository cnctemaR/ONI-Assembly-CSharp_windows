using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class AuthenticationModulesSection : ConfigurationSection
	{
		static AuthenticationModulesSection()
		{
			AuthenticationModulesSection.properties.Add(AuthenticationModulesSection.authenticationModulesProp);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return AuthenticationModulesSection.properties;
			}
		}

		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public AuthenticationModuleElementCollection AuthenticationModules
		{
			get
			{
				return (AuthenticationModuleElementCollection)base[AuthenticationModulesSection.authenticationModulesProp];
			}
		}

		[global::System.MonoTODO]
		protected override void PostDeserialize()
		{
		}

		[global::System.MonoTODO]
		protected override void InitializeDefault()
		{
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty authenticationModulesProp = new ConfigurationProperty(string.Empty, typeof(AuthenticationModuleElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
