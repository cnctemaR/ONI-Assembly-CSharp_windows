using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class WebRequestModulesSection : ConfigurationSection
	{
		static WebRequestModulesSection()
		{
			WebRequestModulesSection.properties.Add(WebRequestModulesSection.webRequestModulesProp);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return WebRequestModulesSection.properties;
			}
		}

		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public WebRequestModuleElementCollection WebRequestModules
		{
			get
			{
				return (WebRequestModuleElementCollection)base[WebRequestModulesSection.webRequestModulesProp];
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

		private static ConfigurationProperty webRequestModulesProp = new ConfigurationProperty(string.Empty, typeof(WebRequestModuleElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
