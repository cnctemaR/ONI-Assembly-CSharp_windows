using System;
using System.Collections.Specialized;
using System.Configuration;

namespace System.Runtime.Serialization
{
	internal static class AppSettings
	{
		internal static int MaxMimeParts
		{
			get
			{
				AppSettings.EnsureSettingsLoaded();
				return AppSettings.maxMimeParts;
			}
		}

		private static void EnsureSettingsLoaded()
		{
			if (!AppSettings.settingsInitalized)
			{
				object obj = AppSettings.appSettingsLock;
				lock (obj)
				{
					if (!AppSettings.settingsInitalized)
					{
						NameValueCollection nameValueCollection = null;
						try
						{
							nameValueCollection = ConfigurationManager.AppSettings;
						}
						catch (ConfigurationErrorsException)
						{
						}
						finally
						{
							if (nameValueCollection == null || !int.TryParse(nameValueCollection["microsoft:xmldictionaryreader:maxmimeparts"], out AppSettings.maxMimeParts))
							{
								AppSettings.maxMimeParts = 1000;
							}
							AppSettings.settingsInitalized = true;
						}
					}
				}
			}
		}

		internal const string MaxMimePartsAppSettingsString = "microsoft:xmldictionaryreader:maxmimeparts";

		private const int DefaultMaxMimeParts = 1000;

		private static int maxMimeParts;

		private static volatile bool settingsInitalized = false;

		private static object appSettingsLock = new object();
	}
}
