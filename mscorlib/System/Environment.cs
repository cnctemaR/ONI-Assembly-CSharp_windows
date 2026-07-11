using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System
{
	[ComVisible(true)]
	public static class Environment
	{
		public static string CommandLine
		{
			get
			{
				return string.Join(" ", Environment.GetCommandLineArgs());
			}
		}

		public static string CurrentDirectory
		{
			get
			{
				return Directory.GetCurrentDirectory();
			}
			set
			{
				Directory.SetCurrentDirectory(value);
			}
		}

		public static extern int ExitCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool HasShutdownStarted
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string EmbeddingHostName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool SocketSecurityEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool UnityWebSecurityEnabled
		{
			get
			{
				return Environment.SocketSecurityEnabled;
			}
		}

		public static extern string MachineName
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Read=\"COMPUTERNAME\"/>\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string NewLine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern PlatformID Platform
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetOSVersionString();

		public static OperatingSystem OSVersion
		{
			get
			{
				if (Environment.os == null)
				{
					Version version = Version.CreateFromString(Environment.GetOSVersionString());
					PlatformID platform = Environment.Platform;
					Environment.os = new OperatingSystem(platform, version);
				}
				return Environment.os;
			}
		}

		public static string StackTrace
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
			get
			{
				StackTrace stackTrace = new StackTrace(0, true);
				return stackTrace.ToString();
			}
		}

		public static string SystemDirectory
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.System);
			}
		}

		public static extern int TickCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static string UserDomainName
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Read=\"USERDOMAINNAME\"/>\n</PermissionSet>\n")]
			get
			{
				return Environment.MachineName;
			}
		}

		[MonoTODO("Currently always returns false, regardless of interactive state")]
		public static bool UserInteractive
		{
			get
			{
				return false;
			}
		}

		public static extern string UserName
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Read=\"USERNAME;USER\"/>\n</PermissionSet>\n")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Version Version
		{
			get
			{
				return new Version("2.0.50727.1433");
			}
		}

		[MonoTODO("Currently always returns zero")]
		public static long WorkingSet
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
			get
			{
				return 0L;
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Exit(int exitCode);

		public static string ExpandEnvironmentVariables(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			int num = name.IndexOf('%');
			if (num == -1)
			{
				return name;
			}
			int length = name.Length;
			int num2;
			if (num == length - 1 || (num2 = name.IndexOf('%', num + 1)) == -1)
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(name, 0, num);
			Hashtable hashtable = null;
			do
			{
				string text = name.Substring(num + 1, num2 - num - 1);
				string text2 = Environment.GetEnvironmentVariable(text);
				if (text2 == null && Environment.IsRunningOnWindows)
				{
					if (hashtable == null)
					{
						hashtable = Environment.GetEnvironmentVariablesNoCase();
					}
					text2 = hashtable[text] as string;
				}
				if (text2 == null)
				{
					stringBuilder.Append('%');
					stringBuilder.Append(text);
					num2--;
				}
				else
				{
					stringBuilder.Append(text2);
				}
				int num3 = num2;
				num = name.IndexOf('%', num2 + 1);
				num2 = ((num != -1 && num2 <= length - 1) ? name.IndexOf('%', num + 1) : (-1));
				int num4;
				if (num == -1 || num2 == -1)
				{
					num4 = length - num3 - 1;
				}
				else if (text2 != null)
				{
					num4 = num - num3 - 1;
				}
				else
				{
					num4 = num - num3;
				}
				if (num >= num3 || num == -1)
				{
					stringBuilder.Append(name, num3 + 1, num4);
				}
			}
			while (num2 > -1 && num2 < length);
			return stringBuilder.ToString();
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Read=\"PATH\"/>\n</PermissionSet>\n")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetCommandLineArgs();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string internalGetEnvironmentVariable(string variable);

		public static string GetEnvironmentVariable(string variable)
		{
			if (SecurityManager.SecurityEnabled)
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, variable).Demand();
			}
			return Environment.internalGetEnvironmentVariable(variable);
		}

		private static Hashtable GetEnvironmentVariablesNoCase()
		{
			Hashtable hashtable = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			foreach (string text in Environment.GetEnvironmentVariableNames())
			{
				hashtable[text] = Environment.internalGetEnvironmentVariable(text);
			}
			return hashtable;
		}

		public static IDictionary GetEnvironmentVariables()
		{
			StringBuilder stringBuilder = null;
			if (SecurityManager.SecurityEnabled)
			{
				stringBuilder = new StringBuilder();
			}
			Hashtable hashtable = new Hashtable();
			foreach (string text in Environment.GetEnvironmentVariableNames())
			{
				hashtable[text] = Environment.internalGetEnvironmentVariable(text);
				if (stringBuilder != null)
				{
					stringBuilder.Append(text);
					stringBuilder.Append(";");
				}
			}
			if (stringBuilder != null)
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, stringBuilder.ToString()).Demand();
			}
			return hashtable;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetWindowsFolderPath(int folder);

		public static string GetFolderPath(Environment.SpecialFolder folder)
		{
			string text;
			if (Environment.IsRunningOnWindows)
			{
				text = Environment.GetWindowsFolderPath((int)folder);
			}
			else
			{
				text = Environment.InternalGetFolderPath(folder);
			}
			if (text != null && text.Length > 0 && SecurityManager.SecurityEnabled)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text).Demand();
			}
			return text;
		}

		private static string ReadXdgUserDir(string config_dir, string home_dir, string key, string fallback)
		{
			string text = Environment.internalGetEnvironmentVariable(key);
			if (text != null && text != string.Empty)
			{
				return text;
			}
			string text2 = Path.Combine(config_dir, "user-dirs.dirs");
			if (!File.Exists(text2))
			{
				return Path.Combine(home_dir, fallback);
			}
			try
			{
				using (StreamReader streamReader = new StreamReader(text2))
				{
					string text3;
					while ((text3 = streamReader.ReadLine()) != null)
					{
						text3 = text3.Trim();
						int num = text3.IndexOf('=');
						if (num > 8 && text3.Substring(0, num) == key)
						{
							string text4 = text3.Substring(num + 1).Trim(new char[] { '"' });
							bool flag = false;
							if (text4.StartsWith("$HOME/"))
							{
								flag = true;
								text4 = text4.Substring(6);
							}
							else if (!text4.StartsWith("/"))
							{
								flag = true;
							}
							return (!flag) ? text4 : Path.Combine(home_dir, text4);
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
			}
			return Path.Combine(home_dir, fallback);
		}

		internal static string InternalGetFolderPath(Environment.SpecialFolder folder)
		{
			string text = Environment.internalGetHome();
			string text2 = Environment.internalGetEnvironmentVariable("XDG_DATA_HOME");
			if (text2 == null || text2 == string.Empty)
			{
				text2 = Path.Combine(text, ".local");
				text2 = Path.Combine(text2, "share");
			}
			string text3 = Environment.internalGetEnvironmentVariable("XDG_CONFIG_HOME");
			if (text3 == null || text3 == string.Empty)
			{
				text3 = Path.Combine(text, ".config");
			}
			switch (folder)
			{
			case Environment.SpecialFolder.Desktop:
			case Environment.SpecialFolder.DesktopDirectory:
				return Environment.ReadXdgUserDir(text3, text, "XDG_DESKTOP_DIR", "Desktop");
			case Environment.SpecialFolder.Programs:
			case Environment.SpecialFolder.Favorites:
			case Environment.SpecialFolder.Startup:
			case Environment.SpecialFolder.Recent:
			case Environment.SpecialFolder.SendTo:
			case Environment.SpecialFolder.StartMenu:
			case Environment.SpecialFolder.Templates:
			case Environment.SpecialFolder.InternetCache:
			case Environment.SpecialFolder.Cookies:
			case Environment.SpecialFolder.History:
			case Environment.SpecialFolder.System:
			case Environment.SpecialFolder.ProgramFiles:
			case Environment.SpecialFolder.CommonProgramFiles:
				return string.Empty;
			case Environment.SpecialFolder.MyDocuments:
				return text;
			case Environment.SpecialFolder.MyMusic:
				return Environment.ReadXdgUserDir(text3, text, "XDG_MUSIC_DIR", "Music");
			case Environment.SpecialFolder.MyComputer:
				return string.Empty;
			case Environment.SpecialFolder.ApplicationData:
				return text3;
			case Environment.SpecialFolder.LocalApplicationData:
				return text2;
			case Environment.SpecialFolder.CommonApplicationData:
				return "/usr/share";
			case Environment.SpecialFolder.MyPictures:
				return Environment.ReadXdgUserDir(text3, text, "XDG_PICTURES_DIR", "Pictures");
			}
			throw new ArgumentException("Invalid SpecialFolder");
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
		public static string[] GetLogicalDrives()
		{
			return Environment.GetLogicalDrivesInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void internalBroadcastSettingChange();

		public static string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
		{
			switch (target)
			{
			case EnvironmentVariableTarget.Process:
				return Environment.GetEnvironmentVariable(variable);
			case EnvironmentVariableTarget.User:
				break;
			case EnvironmentVariableTarget.Machine:
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (!Environment.IsRunningOnWindows)
				{
					return null;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment"))
				{
					object value = registryKey.GetValue(variable);
					return (value != null) ? value.ToString() : null;
				}
				break;
			}
			default:
				goto IL_00D7;
			}
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			if (!Environment.IsRunningOnWindows)
			{
				return null;
			}
			using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Environment", false))
			{
				object value2 = registryKey2.GetValue(variable);
				return (value2 != null) ? value2.ToString() : null;
			}
			IL_00D7:
			throw new ArgumentException("target");
		}

		public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)
		{
			IDictionary dictionary = new Hashtable();
			switch (target)
			{
			case EnvironmentVariableTarget.Process:
				dictionary = Environment.GetEnvironmentVariables();
				break;
			case EnvironmentVariableTarget.User:
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (Environment.IsRunningOnWindows)
				{
					using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment"))
					{
						string[] valueNames = registryKey.GetValueNames();
						foreach (string text in valueNames)
						{
							dictionary.Add(text, registryKey.GetValue(text));
						}
					}
				}
				break;
			case EnvironmentVariableTarget.Machine:
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (Environment.IsRunningOnWindows)
				{
					using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment"))
					{
						string[] valueNames2 = registryKey2.GetValueNames();
						foreach (string text2 in valueNames2)
						{
							dictionary.Add(text2, registryKey2.GetValue(text2));
						}
					}
				}
				break;
			default:
				throw new ArgumentException("target");
			}
			return dictionary;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
		public static void SetEnvironmentVariable(string variable, string value)
		{
			Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
		public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			if (variable == string.Empty)
			{
				throw new ArgumentException("String cannot be of zero length.", "variable");
			}
			if (variable.IndexOf('=') != -1)
			{
				throw new ArgumentException("Environment variable name cannot contain an equal character.", "variable");
			}
			if (variable[0] == '\0')
			{
				throw new ArgumentException("The first char in the string is the null character.", "variable");
			}
			switch (target)
			{
			case EnvironmentVariableTarget.Process:
				Environment.InternalSetEnvironmentVariable(variable, value);
				break;
			case EnvironmentVariableTarget.User:
			{
				if (!Environment.IsRunningOnWindows)
				{
					return;
				}
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment", true))
				{
					if (string.IsNullOrEmpty(value))
					{
						registryKey.DeleteValue(variable, false);
					}
					else
					{
						registryKey.SetValue(variable, value);
					}
					Environment.internalBroadcastSettingChange();
				}
				break;
			}
			case EnvironmentVariableTarget.Machine:
			{
				if (!Environment.IsRunningOnWindows)
				{
					return;
				}
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment", true))
				{
					if (string.IsNullOrEmpty(value))
					{
						registryKey2.DeleteValue(variable, false);
					}
					else
					{
						registryKey2.SetValue(variable, value);
					}
					Environment.internalBroadcastSettingChange();
				}
				break;
			}
			default:
				throw new ArgumentException("target");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalSetEnvironmentVariable(string variable, string value);

		[MonoTODO("Not implemented")]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static void FailFast(string message)
		{
			throw new NotImplementedException();
		}

		public static extern int ProcessorCount
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Read=\"NUMBER_OF_PROCESSORS\"/>\n</PermissionSet>\n")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static bool IsRunningOnWindows
		{
			get
			{
				return Environment.Platform < PlatformID.Unix;
			}
		}

		private static string GacPath
		{
			get
			{
				if (Environment.IsRunningOnWindows)
				{
					string fullName = new DirectoryInfo(Path.GetDirectoryName(typeof(int).Assembly.Location)).Parent.Parent.FullName;
					return Path.Combine(Path.Combine(fullName, "mono"), "gac");
				}
				return Path.Combine(Path.Combine(Environment.internalGetGacPath(), "mono"), "gac");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string internalGetGacPath();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetLogicalDrivesInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetEnvironmentVariableNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetMachineConfigPath();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string internalGetHome();

		private const int mono_corlib_version = 82;

		private static OperatingSystem os;

		[ComVisible(true)]
		public enum SpecialFolder
		{
			MyDocuments = 5,
			Desktop = 0,
			MyComputer = 17,
			Programs = 2,
			Personal = 5,
			Favorites,
			Startup,
			Recent,
			SendTo,
			StartMenu = 11,
			MyMusic = 13,
			DesktopDirectory = 16,
			Templates = 21,
			ApplicationData = 26,
			LocalApplicationData = 28,
			InternetCache = 32,
			Cookies,
			History,
			CommonApplicationData,
			System = 37,
			ProgramFiles,
			MyPictures,
			CommonProgramFiles = 43
		}
	}
}
