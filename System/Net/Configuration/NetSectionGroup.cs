using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class NetSectionGroup : ConfigurationSectionGroup
	{
		[MonoTODO]
		public NetSectionGroup()
		{
		}

		[ConfigurationProperty("authenticationModules")]
		public AuthenticationModulesSection AuthenticationModules
		{
			get
			{
				return (AuthenticationModulesSection)base.Sections["authenticationModules"];
			}
		}

		[ConfigurationProperty("connectionManagement")]
		public ConnectionManagementSection ConnectionManagement
		{
			get
			{
				return (ConnectionManagementSection)base.Sections["connectionManagement"];
			}
		}

		[ConfigurationProperty("defaultProxy")]
		public DefaultProxySection DefaultProxy
		{
			get
			{
				return (DefaultProxySection)base.Sections["defaultProxy"];
			}
		}

		public MailSettingsSectionGroup MailSettings
		{
			get
			{
				return (MailSettingsSectionGroup)base.SectionGroups["mailSettings"];
			}
		}

		[ConfigurationProperty("requestCaching")]
		public RequestCachingSection RequestCaching
		{
			get
			{
				return (RequestCachingSection)base.Sections["requestCaching"];
			}
		}

		[ConfigurationProperty("settings")]
		public SettingsSection Settings
		{
			get
			{
				return (SettingsSection)base.Sections["settings"];
			}
		}

		[ConfigurationProperty("webRequestModules")]
		public WebRequestModulesSection WebRequestModules
		{
			get
			{
				return (WebRequestModulesSection)base.Sections["webRequestModules"];
			}
		}

		[MonoTODO]
		public static NetSectionGroup GetSectionGroup(Configuration config)
		{
			throw new NotImplementedException();
		}
	}
}
