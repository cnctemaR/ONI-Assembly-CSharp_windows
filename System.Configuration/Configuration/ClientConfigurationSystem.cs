using System;
using System.Configuration.Internal;
using System.Reflection;

namespace System.Configuration
{
	internal class ClientConfigurationSystem : IInternalConfigSystem
	{
		private Configuration Configuration
		{
			get
			{
				if (this.cfg == null)
				{
					Assembly entryAssembly = Assembly.GetEntryAssembly();
					try
					{
						this.cfg = ConfigurationManager.OpenExeConfigurationInternal(ConfigurationUserLevel.None, entryAssembly, null);
					}
					catch (Exception ex)
					{
						throw new ConfigurationErrorsException("Error Initializing the configuration system.", ex);
					}
				}
				return this.cfg;
			}
		}

		object IInternalConfigSystem.GetSection(string configKey)
		{
			ConfigurationSection section = this.Configuration.GetSection(configKey);
			if (section == null)
			{
				return null;
			}
			return section.GetRuntimeObject();
		}

		void IInternalConfigSystem.RefreshConfig(string sectionName)
		{
		}

		bool IInternalConfigSystem.SupportsUserConfig
		{
			get
			{
				return false;
			}
		}

		private Configuration cfg;
	}
}
