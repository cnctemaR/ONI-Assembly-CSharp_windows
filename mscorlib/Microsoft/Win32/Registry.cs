using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public static class Registry
	{
		private static RegistryKey ToKey(string keyName, bool setting)
		{
			if (keyName == null)
			{
				throw new ArgumentException("Not a valid registry key name", "keyName");
			}
			string[] array = keyName.Split(new char[] { '\\' });
			string text = array[0];
			if (text != null)
			{
				if (Registry.<>f__switch$map0 == null)
				{
					Registry.<>f__switch$map0 = new Dictionary<string, int>(7)
					{
						{ "HKEY_CLASSES_ROOT", 0 },
						{ "HKEY_CURRENT_CONFIG", 1 },
						{ "HKEY_CURRENT_USER", 2 },
						{ "HKEY_DYN_DATA", 3 },
						{ "HKEY_LOCAL_MACHINE", 4 },
						{ "HKEY_PERFORMANCE_DATA", 5 },
						{ "HKEY_USERS", 6 }
					};
				}
				int num;
				if (Registry.<>f__switch$map0.TryGetValue(text, out num))
				{
					RegistryKey registryKey;
					switch (num)
					{
					case 0:
						registryKey = Registry.ClassesRoot;
						break;
					case 1:
						registryKey = Registry.CurrentConfig;
						break;
					case 2:
						registryKey = Registry.CurrentUser;
						break;
					case 3:
						registryKey = Registry.DynData;
						break;
					case 4:
						registryKey = Registry.LocalMachine;
						break;
					case 5:
						registryKey = Registry.PerformanceData;
						break;
					case 6:
						registryKey = Registry.Users;
						break;
					default:
						goto IL_0132;
					}
					for (int i = 1; i < array.Length; i++)
					{
						RegistryKey registryKey2 = registryKey.OpenSubKey(array[i], setting);
						if (registryKey2 == null)
						{
							if (!setting)
							{
								return null;
							}
							registryKey2 = registryKey.CreateSubKey(array[i]);
						}
						registryKey = registryKey2;
					}
					return registryKey;
				}
			}
			IL_0132:
			throw new ArgumentException("Keyname does not start with a valid registry root", "keyName");
		}

		public static void SetValue(string keyName, string valueName, object value)
		{
			RegistryKey registryKey = Registry.ToKey(keyName, true);
			if (valueName.Length > 255)
			{
				throw new ArgumentException("valueName is larger than 255 characters", "valueName");
			}
			if (registryKey == null)
			{
				throw new ArgumentException("cant locate that keyName", "keyName");
			}
			registryKey.SetValue(valueName, value);
		}

		public static void SetValue(string keyName, string valueName, object value, RegistryValueKind valueKind)
		{
			RegistryKey registryKey = Registry.ToKey(keyName, true);
			if (valueName.Length > 255)
			{
				throw new ArgumentException("valueName is larger than 255 characters", "valueName");
			}
			if (registryKey == null)
			{
				throw new ArgumentException("cant locate that keyName", "keyName");
			}
			registryKey.SetValue(valueName, value, valueKind);
		}

		public static object GetValue(string keyName, string valueName, object defaultValue)
		{
			RegistryKey registryKey = Registry.ToKey(keyName, false);
			if (registryKey == null)
			{
				return defaultValue;
			}
			return registryKey.GetValue(valueName, defaultValue);
		}

		public static readonly RegistryKey ClassesRoot = new RegistryKey(RegistryHive.ClassesRoot);

		public static readonly RegistryKey CurrentConfig = new RegistryKey(RegistryHive.CurrentConfig);

		public static readonly RegistryKey CurrentUser = new RegistryKey(RegistryHive.CurrentUser);

		public static readonly RegistryKey DynData = new RegistryKey(RegistryHive.DynData);

		public static readonly RegistryKey LocalMachine = new RegistryKey(RegistryHive.LocalMachine);

		public static readonly RegistryKey PerformanceData = new RegistryKey(RegistryHive.PerformanceData);

		public static readonly RegistryKey Users = new RegistryKey(RegistryHive.Users);
	}
}
