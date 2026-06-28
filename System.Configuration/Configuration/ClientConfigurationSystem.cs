using System;
using System.Configuration.Internal;
using System.Reflection;

namespace System.Configuration
{
	internal class ClientConfigurationSystem : IInternalConfigSystem
	{
		object IInternalConfigSystem.GetSection(string configKey)
		{
			ConfigurationSection section = this.Configuration.GetSection(configKey);
			return (section == null) ? null : section.GetRuntimeObject();
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

		private Configuration cfg;
	}
}
