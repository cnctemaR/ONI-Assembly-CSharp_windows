using System;
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
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			RegistryKey registryKey;
			if (num <= 1097425318U)
			{
				if (num != 126972219U)
				{
					if (num != 457190004U)
					{
						if (num == 1097425318U)
						{
							if (text == "HKEY_CLASSES_ROOT")
							{
								registryKey = Registry.ClassesRoot;
								goto IL_0146;
							}
						}
					}
					else if (text == "HKEY_LOCAL_MACHINE")
					{
						registryKey = Registry.LocalMachine;
						goto IL_0146;
					}
				}
				else if (text == "HKEY_CURRENT_CONFIG")
				{
					registryKey = Registry.CurrentConfig;
					goto IL_0146;
				}
			}
			else if (num <= 1568329430U)
			{
				if (num != 1198714601U)
				{
					if (num == 1568329430U)
					{
						if (text == "HKEY_CURRENT_USER")
						{
							registryKey = Registry.CurrentUser;
							goto IL_0146;
						}
					}
				}
				else if (text == "HKEY_USERS")
				{
					registryKey = Registry.Users;
					goto IL_0146;
				}
			}
			else if (num != 2823865611U)
			{
				if (num == 3554990456U)
				{
					if (text == "HKEY_PERFORMANCE_DATA")
					{
						registryKey = Registry.PerformanceData;
						goto IL_0146;
					}
				}
			}
			else if (text == "HKEY_DYN_DATA")
			{
				registryKey = Registry.DynData;
				goto IL_0146;
			}
			throw new ArgumentException("Keyname does not start with a valid registry root", "keyName");
			IL_0146:
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

		[Obsolete("Use PerformanceData instead")]
		public static readonly RegistryKey DynData = new RegistryKey(RegistryHive.DynData);

		public static readonly RegistryKey LocalMachine = new RegistryKey(RegistryHive.LocalMachine);

		public static readonly RegistryKey PerformanceData = new RegistryKey(RegistryHive.PerformanceData);

		public static readonly RegistryKey Users = new RegistryKey(RegistryHive.Users);
	}
}
