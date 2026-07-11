using System;
using System.Runtime.CompilerServices;

namespace System.Configuration
{
	internal class DefaultConfig : IConfigurationSystem
	{
		private DefaultConfig()
		{
		}

		public static DefaultConfig GetInstance()
		{
			return DefaultConfig.instance;
		}

		[Obsolete("This method is obsolete.  Please use System.Configuration.ConfigurationManager.GetConfig")]
		public object GetConfig(string sectionName)
		{
			this.Init();
			return this.config.GetConfig(sectionName);
		}

		public void Init()
		{
			lock (this)
			{
				if (this.config == null)
				{
					ConfigurationData configurationData = new ConfigurationData();
					if (!configurationData.LoadString(DefaultConfig.GetBundledMachineConfig()) && !configurationData.Load(DefaultConfig.GetMachineConfigPath()))
					{
						throw new ConfigurationException("Cannot find " + DefaultConfig.GetMachineConfigPath());
					}
					string appConfigPath = DefaultConfig.GetAppConfigPath();
					if (appConfigPath == null)
					{
						this.config = configurationData;
					}
					else
					{
						ConfigurationData configurationData2 = new ConfigurationData(configurationData);
						if (configurationData2.Load(appConfigPath))
						{
							this.config = configurationData2;
						}
						else
						{
							this.config = configurationData;
						}
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_bundled_machine_config();

		internal static string GetBundledMachineConfig()
		{
			return DefaultConfig.get_bundled_machine_config();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_machine_config_path();

		internal static string GetMachineConfigPath()
		{
			return DefaultConfig.get_machine_config_path();
		}

		private static string GetAppConfigPath()
		{
			string configurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			if (configurationFile == null || configurationFile.Length == 0)
			{
				return null;
			}
			return configurationFile;
		}

		private static readonly DefaultConfig instance = new DefaultConfig();

		private ConfigurationData config;
	}
}
