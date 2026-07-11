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

		[MonoTODO]
		protected override void PostDeserialize()
		{
		}

		[MonoTODO]
		protected override void InitializeDefault()
		{
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty webRequestModulesProp = new ConfigurationProperty("", typeof(WebRequestModuleElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
