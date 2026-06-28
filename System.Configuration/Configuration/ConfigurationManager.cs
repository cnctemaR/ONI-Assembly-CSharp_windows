using System;
using System.Collections.Specialized;
using System.Configuration.Internal;
using System.IO;
using System.Reflection;
using System.Text;

namespace System.Configuration
{
	public static class ConfigurationManager
	{
		[MonoTODO("Evidence and version still needs work")]
		private static string GetAssemblyInfo(Assembly a)
		{
			object[] array = a.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
			string text;
			if (array != null && array.Length > 0)
			{
				text = ((AssemblyProductAttribute)array[0]).Product;
			}
			else
			{
				text = AppDomain.CurrentDomain.FriendlyName;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("evidencehere");
			string text2 = stringBuilder.ToString();
			array = a.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
			string text3;
			if (array != null && array.Length > 0)
			{
				text3 = ((AssemblyVersionAttribute)array[0]).Version;
			}
			else
			{
				text3 = "1.0.0.0";
			}
			return Path.Combine(string.Format("{0}_{1}", text, text2), text3);
		}

		internal static Configuration OpenExeConfigurationInternal(ConfigurationUserLevel userLevel, Assembly calling_assembly, string exePath)
		{
			ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
			if (userLevel != ConfigurationUserLevel.None)
			{
				if (userLevel != ConfigurationUserLevel.PerUserRoaming)
				{
					if (userLevel != ConfigurationUserLevel.PerUserRoamingAndLocal)
					{
						goto IL_0104;
					}
				}
				else
				{
					exeConfigurationFileMap.RoamingUserConfigFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ConfigurationManager.GetAssemblyInfo(calling_assembly));
					exeConfigurationFileMap.RoamingUserConfigFilename = Path.Combine(exeConfigurationFileMap.RoamingUserConfigFilename, "user.config");
				}
				exeConfigurationFileMap.LocalUserConfigFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigurationManager.GetAssemblyInfo(calling_assembly));
				exeConfigurationFileMap.LocalUserConfigFilename = Path.Combine(exeConfigurationFileMap.LocalUserConfigFilename, "user.config");
			}
			else if (exePath == null || exePath.Length == 0)
			{
				exeConfigurationFileMap.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			}
			else
			{
				if (!Path.IsPathRooted(exePath))
				{
					exePath = Path.GetFullPath(exePath);
				}
				if (!File.Exists(exePath))
				{
					Exception ex = new ArgumentException("The specified path does not exist.", "exePath");
					throw new ConfigurationErrorsException("Error Initializing the configuration system:", ex);
				}
				exeConfigurationFileMap.ExeConfigFilename = exePath + ".config";
			}
			IL_0104:
			return ConfigurationManager.ConfigurationFactory.Create(typeof(ExeConfigurationHost), new object[] { exeConfigurationFileMap, userLevel });
		}

		public static Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
		{
			return ConfigurationManager.OpenExeConfigurationInternal(userLevel, Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly(), null);
		}

		public static Configuration OpenExeConfiguration(string exePath)
		{
			return ConfigurationManager.OpenExeConfigurationInternal(ConfigurationUserLevel.None, Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly(), exePath);
		}

		[MonoLimitation("ConfigurationUserLevel parameter is not supported.")]
		public static Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel)
		{
			return ConfigurationManager.ConfigurationFactory.Create(typeof(ExeConfigurationHost), new object[] { fileMap, userLevel });
		}

		public static Configuration OpenMachineConfiguration()
		{
			ConfigurationFileMap configurationFileMap = new ConfigurationFileMap();
			return ConfigurationManager.ConfigurationFactory.Create(typeof(MachineConfigurationHost), new object[] { configurationFileMap });
		}

		public static Configuration OpenMappedMachineConfiguration(ConfigurationFileMap fileMap)
		{
			return ConfigurationManager.ConfigurationFactory.Create(typeof(MachineConfigurationHost), new object[] { fileMap });
		}

		internal static IInternalConfigConfigurationFactory ConfigurationFactory
		{
			get
			{
				return ConfigurationManager.configFactory;
			}
		}

		internal static IInternalConfigSystem ConfigurationSystem
		{
			get
			{
				return ConfigurationManager.configSystem;
			}
		}

		public static object GetSection(string sectionName)
		{
			object section = ConfigurationManager.ConfigurationSystem.GetSection(sectionName);
			if (section is ConfigurationSection)
			{
				return ((ConfigurationSection)section).GetRuntimeObject();
			}
			return section;
		}

		public static void RefreshSection(string sectionName)
		{
			ConfigurationManager.ConfigurationSystem.RefreshConfig(sectionName);
		}

		public static NameValueCollection AppSettings
		{
			get
			{
				return (NameValueCollection)ConfigurationManager.GetSection("appSettings");
			}
		}

		[MonoTODO]
		public static ConnectionStringSettingsCollection ConnectionStrings
		{
			get
			{
				ConnectionStringsSection connectionStringsSection = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");
				return connectionStringsSection.ConnectionStrings;
			}
		}

		internal static IInternalConfigSystem ChangeConfigurationSystem(IInternalConfigSystem newSystem)
		{
			if (newSystem == null)
			{
				throw new ArgumentNullException("newSystem");
			}
			object obj = ConfigurationManager.lockobj;
			IInternalConfigSystem internalConfigSystem2;
			lock (obj)
			{
				string text = newSystem.GetType().ToString();
				if (string.Compare(text, "System.Web.Configuration.HttpConfigurationSystem", StringComparison.OrdinalIgnoreCase) == 0)
				{
					ConfigurationManager.systemWebInUse = true;
				}
				else
				{
					ConfigurationManager.systemWebInUse = false;
				}
				IInternalConfigSystem internalConfigSystem = ConfigurationManager.configSystem;
				ConfigurationManager.configSystem = newSystem;
				internalConfigSystem2 = internalConfigSystem;
			}
			return internalConfigSystem2;
		}

		private static bool systemWebInUse;

		private static InternalConfigurationFactory configFactory = new InternalConfigurationFactory();

		private static IInternalConfigSystem configSystem = new ClientConfigurationSystem();

		private static object lockobj = new object();
	}
}
