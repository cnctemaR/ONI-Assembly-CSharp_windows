using System;
using System.Collections.Specialized;

namespace System.Configuration
{
	public sealed class ConfigurationSettings
	{
		private ConfigurationSettings()
		{
		}

		[Obsolete("This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.GetSection")]
		public static object GetConfig(string sectionName)
		{
			return ConfigurationManager.GetSection(sectionName);
		}

		[Obsolete("This property is obsolete.  Please use System.Configuration.ConfigurationManager.AppSettings")]
		public static global::System.Collections.Specialized.NameValueCollection AppSettings
		{
			get
			{
				object obj = ConfigurationManager.GetSection("appSettings");
				if (obj == null)
				{
					obj = new global::System.Collections.Specialized.NameValueCollection();
				}
				return (global::System.Collections.Specialized.NameValueCollection)obj;
			}
		}

		internal static IConfigurationSystem ChangeConfigurationSystem(IConfigurationSystem newSystem)
		{
			if (newSystem == null)
			{
				throw new ArgumentNullException("newSystem");
			}
			object obj = ConfigurationSettings.lockobj;
			IConfigurationSystem configurationSystem2;
			lock (obj)
			{
				IConfigurationSystem configurationSystem = ConfigurationSettings.config;
				ConfigurationSettings.config = newSystem;
				configurationSystem2 = configurationSystem;
			}
			return configurationSystem2;
		}

		private static IConfigurationSystem config = DefaultConfig.GetInstance();

		private static object lockobj = new object();
	}
}
