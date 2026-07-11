using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class DefaultProxySection : ConfigurationSection
	{
		static DefaultProxySection()
		{
			DefaultProxySection.properties.Add(DefaultProxySection.bypassListProp);
			DefaultProxySection.properties.Add(DefaultProxySection.enabledProp);
			DefaultProxySection.properties.Add(DefaultProxySection.moduleProp);
			DefaultProxySection.properties.Add(DefaultProxySection.proxyProp);
			DefaultProxySection.properties.Add(DefaultProxySection.useDefaultCredentialsProp);
		}

		[ConfigurationProperty("bypasslist")]
		public BypassElementCollection BypassList
		{
			get
			{
				return (BypassElementCollection)base[DefaultProxySection.bypassListProp];
			}
		}

		[ConfigurationProperty("enabled", DefaultValue = "True")]
		public bool Enabled
		{
			get
			{
				return (bool)base[DefaultProxySection.enabledProp];
			}
			set
			{
				base[DefaultProxySection.enabledProp] = value;
			}
		}

		[ConfigurationProperty("module")]
		public ModuleElement Module
		{
			get
			{
				return (ModuleElement)base[DefaultProxySection.moduleProp];
			}
		}

		[ConfigurationProperty("proxy")]
		public ProxyElement Proxy
		{
			get
			{
				return (ProxyElement)base[DefaultProxySection.proxyProp];
			}
		}

		[ConfigurationProperty("useDefaultCredentials", DefaultValue = "False")]
		public bool UseDefaultCredentials
		{
			get
			{
				return (bool)base[DefaultProxySection.useDefaultCredentialsProp];
			}
			set
			{
				base[DefaultProxySection.useDefaultCredentialsProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return DefaultProxySection.properties;
			}
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
		}

		[MonoTODO]
		protected override void Reset(ConfigurationElement parentElement)
		{
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty bypassListProp = new ConfigurationProperty("bypasslist", typeof(BypassElementCollection), null);

		private static ConfigurationProperty enabledProp = new ConfigurationProperty("enabled", typeof(bool), true);

		private static ConfigurationProperty moduleProp = new ConfigurationProperty("module", typeof(ModuleElement), null);

		private static ConfigurationProperty proxyProp = new ConfigurationProperty("proxy", typeof(ProxyElement), null);

		private static ConfigurationProperty useDefaultCredentialsProp = new ConfigurationProperty("useDefaultCredentials", typeof(bool), false);
	}
}
