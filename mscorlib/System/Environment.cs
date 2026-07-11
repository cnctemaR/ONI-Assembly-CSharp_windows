using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using Mono;

namespace System
{
	[ComVisible(true)]
	public static class Environment
	{
		internal static string GetResourceString(string key)
		{
			return key;
		}

		internal static string GetResourceString(string key, CultureInfo culture)
		{
			return key;
		}

		internal static string GetResourceString(string key, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, key, values);
		}

		internal static string GetRuntimeResourceString(string key)
		{
			return key;
		}

		internal static string GetRuntimeResourceString(string key, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, key, values);
		}

		internal static string GetResourceStringEncodingName(int codePage)
		{
			if (codePage <= 12000)
			{
				if (codePage == 1200)
				{
					return Environment.GetResourceString("Unicode");
				}
				if (codePage == 1201)
				{
					return Environment.GetResourceString("Unicode (Big-Endian)");
				}
				if (codePage == 12000)
				{
					return Environment.GetResourceString("Unicode (UTF-32)");
				}
			}
			else if (codePage <= 20127)
			{
				if (codePage == 12001)
				{
					return Environment.GetResourceString("Unicode (UTF-32 Big-Endian)");
				}
				if (codePage == 20127)
				{
					return Environment.GetResourceString("US-ASCII");
				}
			}
			else
			{
				if (codePage == 65000)
				{
					return Environment.GetResourceString("Unicode (UTF-7)");
				}
				if (codePage == 65001)
				{
					return Environment.GetResourceString("Unicode (UTF-8)");
				}
			}
			return codePage.ToString(CultureInfo.InvariantCulture);
		}

		internal static bool IsWindows8OrAbove
		{
			get
			{
				return false;
			}
		}

		public static string CommandLine
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in Environment.GetCommandLineArgs())
				{
					bool flag = false;
					string text2 = "";
					string text3 = text;
					for (int j = 0; j < text3.Length; j++)
					{
						if (text2.Length == 0 && char.IsWhiteSpace(text3[j]))
						{
							text2 = "\"";
						}
						else if (text3[j] == '"')
						{
							flag = true;
						}
					}
					if (flag && text2.Length != 0)
					{
						text3 = text3.Replace("\"", "\\\"");
					}
					stringBuilder.AppendFormat("{0}{1}{0} ", text2, text3);
				}
				if (stringBuilder.Length > 0)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					int i = stringBuilder2.Length;
					stringBuilder2.Length = i - 1;
				}
				return stringBuilder.ToString();
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

		public static int CurrentManagedThreadId
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId;
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

		public static extern string MachineName
		{
			[EnvironmentPermission(SecurityAction.Demand, Read = "COMPUTERNAME")]
			[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetNewLine();

		public static string NewLine
		{
			get
			{
				if (Environment.nl != null)
				{
					return Environment.nl;
				}
				Environment.nl = Environment.GetNewLine();
				return Environment.nl;
			}
		}

		internal static extern PlatformID Platform
		{
			[CompilerGenerated]
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
					Version version = Environment.CreateVersionFromString(Environment.GetOSVersionString());
					PlatformID platformID = Environment.Platform;
					if (platformID == PlatformID.MacOSX)
					{
						platformID = PlatformID.Unix;
					}
					Environment.os = new OperatingSystem(platformID, version);
				}
				return Environment.os;
			}
		}

		internal static Version CreateVersionFromString(string info)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 1;
			int num6 = -1;
			if (info == null)
			{
				return new Version(0, 0, 0, 0);
			}
			foreach (char c in info)
			{
				if (char.IsDigit(c))
				{
					if (num6 < 0)
					{
						num6 = (int)(c - '0');
					}
					else
					{
						num6 = num6 * 10 + (int)(c - '0');
					}
				}
				else if (num6 >= 0)
				{
					switch (num5)
					{
					case 1:
						num = num6;
						break;
					case 2:
						num2 = num6;
						break;
					case 3:
						num3 = num6;
						break;
					case 4:
						num4 = num6;
						break;
					}
					num6 = -1;
					num5++;
				}
				if (num5 == 5)
				{
					break;
				}
			}
			if (num6 >= 0)
			{
				switch (num5)
				{
				case 1:
					num = num6;
					break;
				case 2:
					num2 = num6;
					break;
				case 3:
					num3 = num6;
					break;
				case 4:
					num4 = num6;
					break;
				}
			}
			return new Version(num, num2, num3, num4);
		}

		public static string StackTrace
		{
			[EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
			get
			{
				return new StackTrace(0, true).ToString();
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
			[EnvironmentPermission(SecurityAction.Demand, Read = "USERDOMAINNAME")]
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
			[EnvironmentPermission(SecurityAction.Demand, Read = "USERNAME;USER")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Version Version
		{
			get
			{
				return new Version("4.0.30319.42000");
			}
		}

		[MonoTODO("Currently always returns zero")]
		public static long WorkingSet
		{
			[EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
			get
			{
				return 0L;
			}
		}

		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Exit(int exitCode);

		internal static void _Exit(int exitCode)
		{
			Environment.Exit(exitCode);
		}

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
				int num3 = num2;
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
				int num4 = num2;
				num = name.IndexOf('%', num2 + 1);
				num2 = ((num == -1 || num2 > length - 1) ? (-1) : name.IndexOf('%', num + 1));
				int num5;
				if (num == -1 || num2 == -1)
				{
					num5 = length - num4 - 1;
				}
				else if (text2 != null)
				{
					num5 = num - num4 - 1;
				}
				else
				{
					num5 = num - num3;
				}
				if (num >= num4 || num == -1)
				{
					stringBuilder.Append(name, num4 + 1, num5);
				}
			}
			while (num2 > -1 && num2 < length);
			return stringBuilder.ToString();
		}

		[EnvironmentPermission(SecurityAction.Demand, Read = "PATH")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetCommandLineArgs();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string internalGetEnvironmentVariable_native(IntPtr variable);

		internal static string internalGetEnvironmentVariable(string variable)
		{
			if (variable == null)
			{
				return null;
			}
			string text;
			using (SafeStringMarshal safeStringMarshal = RuntimeMarshal.MarshalString(variable))
			{
				text = Environment.internalGetEnvironmentVariable_native(safeStringMarshal.Value);
			}
			return text;
		}

		public static string GetEnvironmentVariable(string variable)
		{
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

		public static string GetFolderPath(Environment.SpecialFolder folder)
		{
			return Environment.GetFolderPath(folder, Environment.SpecialFolderOption.None);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetWindowsFolderPath(int folder);

		public static string GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
		{
			string text;
			if (Environment.IsRunningOnWindows)
			{
				text = Environment.GetWindowsFolderPath((int)folder);
			}
			else
			{
				text = Environment.UnixGetFolderPath(folder, option);
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
							if (text4.StartsWithOrdinalUnchecked("$HOME/"))
							{
								flag = true;
								text4 = text4.Substring(6);
							}
							else if (!text4.StartsWithOrdinalUnchecked("/"))
							{
								flag = true;
							}
							return flag ? Path.Combine(home_dir, text4) : text4;
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
			}
			return Path.Combine(home_dir, fallback);
		}

		internal static string UnixGetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
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
			case Environment.SpecialFolder.Startup:
			case Environment.SpecialFolder.Recent:
			case Environment.SpecialFolder.SendTo:
			case Environment.SpecialFolder.StartMenu:
			case Environment.SpecialFolder.NetworkShortcuts:
			case Environment.SpecialFolder.CommonStartMenu:
			case Environment.SpecialFolder.CommonPrograms:
			case Environment.SpecialFolder.CommonStartup:
			case Environment.SpecialFolder.CommonDesktopDirectory:
			case Environment.SpecialFolder.PrinterShortcuts:
			case Environment.SpecialFolder.Cookies:
			case Environment.SpecialFolder.History:
			case Environment.SpecialFolder.Windows:
			case Environment.SpecialFolder.System:
			case Environment.SpecialFolder.SystemX86:
			case Environment.SpecialFolder.ProgramFilesX86:
			case Environment.SpecialFolder.CommonProgramFiles:
			case Environment.SpecialFolder.CommonProgramFilesX86:
			case Environment.SpecialFolder.CommonDocuments:
			case Environment.SpecialFolder.CommonAdminTools:
			case Environment.SpecialFolder.AdminTools:
			case Environment.SpecialFolder.CommonMusic:
			case Environment.SpecialFolder.CommonPictures:
			case Environment.SpecialFolder.CommonVideos:
			case Environment.SpecialFolder.Resources:
			case Environment.SpecialFolder.LocalizedResources:
			case Environment.SpecialFolder.CommonOemLinks:
			case Environment.SpecialFolder.CDBurning:
				return string.Empty;
			case Environment.SpecialFolder.MyDocuments:
				return text;
			case Environment.SpecialFolder.Favorites:
				if (Environment.Platform == PlatformID.MacOSX)
				{
					return Path.Combine(text, "Library", "Favorites");
				}
				return string.Empty;
			case Environment.SpecialFolder.MyMusic:
				if (Environment.Platform == PlatformID.MacOSX)
				{
					return Path.Combine(text, "Music");
				}
				return Environment.ReadXdgUserDir(text3, text, "XDG_MUSIC_DIR", "Music");
			case Environment.SpecialFolder.MyVideos:
				return Environment.ReadXdgUserDir(text3, text, "XDG_VIDEOS_DIR", "Videos");
			case Environment.SpecialFolder.MyComputer:
				return string.Empty;
			case Environment.SpecialFolder.Fonts:
				if (Environment.Platform == PlatformID.MacOSX)
				{
					return Path.Combine(text, "Library", "Fonts");
				}
				return Path.Combine(text, ".fonts");
			case Environment.SpecialFolder.Templates:
				return Environment.ReadXdgUserDir(text3, text, "XDG_TEMPLATES_DIR", "Templates");
			case Environment.SpecialFolder.ApplicationData:
				return text3;
			case Environment.SpecialFolder.LocalApplicationData:
				return text2;
			case Environment.SpecialFolder.InternetCache:
				if (Environment.Platform == PlatformID.MacOSX)
				{
					return Path.Combine(text, "Library", "Caches");
				}
				return string.Empty;
			case Environment.SpecialFolder.CommonApplicationData:
				return "/usr/share";
			case Environment.SpecialFolder.ProgramFiles:
				if (Environment.Platform == PlatformID.MacOSX)
				{
					return "/Applications";
				}
				return string.Empty;
			case Environment.SpecialFolder.MyPictures:
				if (Environment.Platform == PlatformID.MacOSX)
				{
					return Path.Combine(text, "Pictures");
				}
				return Environment.ReadXdgUserDir(text3, text, "XDG_PICTURES_DIR", "Pictures");
			case Environment.SpecialFolder.UserProfile:
				return text;
			case Environment.SpecialFolder.CommonTemplates:
				return "/usr/share/templates";
			}
			throw new ArgumentException("Invalid SpecialFolder");
		}

		[EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
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
					return (value == null) ? null : value.ToString();
				}
				break;
			}
			default:
				goto IL_00AC;
			}
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			if (!Environment.IsRunningOnWindows)
			{
				return null;
			}
			using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Environment", false))
			{
				object value2 = registryKey2.GetValue(variable);
				return (value2 == null) ? null : value2.ToString();
			}
			IL_00AC:
			throw new ArgumentException("target");
		}

		public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)
		{
			IDictionary dictionary = new Hashtable();
			switch (target)
			{
			case EnvironmentVariableTarget.Process:
				return Environment.GetEnvironmentVariables();
			case EnvironmentVariableTarget.User:
				break;
			case EnvironmentVariableTarget.Machine:
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (!Environment.IsRunningOnWindows)
				{
					return dictionary;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment"))
				{
					foreach (string text in registryKey.GetValueNames())
					{
						dictionary.Add(text, registryKey.GetValue(text));
					}
					return dictionary;
				}
				break;
			}
			default:
				goto IL_00E0;
			}
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			if (!Environment.IsRunningOnWindows)
			{
				return dictionary;
			}
			using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Environment"))
			{
				foreach (string text2 in registryKey2.GetValueNames())
				{
					dictionary.Add(text2, registryKey2.GetValue(text2));
				}
				return dictionary;
			}
			IL_00E0:
			throw new ArgumentException("target");
		}

		[EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
		public static void SetEnvironmentVariable(string variable, string value)
		{
			Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);
		}

		[EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
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
				return;
			case EnvironmentVariableTarget.User:
				break;
			case EnvironmentVariableTarget.Machine:
			{
				if (!Environment.IsRunningOnWindows)
				{
					return;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment", true))
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
					return;
				}
				break;
			}
			default:
				goto IL_0106;
			}
			if (!Environment.IsRunningOnWindows)
			{
				return;
			}
			using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Environment", true))
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
				return;
			}
			IL_0106:
			throw new ArgumentException("target");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalSetEnvironmentVariable(string variable, string value);

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public static void FailFast(string message)
		{
			throw new NotImplementedException();
		}

		internal static void FailFast(string message, uint exitCode)
		{
			throw new NotImplementedException();
		}

		[SecurityCritical]
		public static void FailFast(string message, Exception exception)
		{
			throw new ExecutionEngineException(message, exception);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIs64BitOperatingSystem();

		public static bool Is64BitOperatingSystem
		{
			get
			{
				return Environment.GetIs64BitOperatingSystem();
			}
		}

		public static int SystemPageSize
		{
			get
			{
				return Environment.GetPageSize();
			}
		}

		public static bool Is64BitProcess
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}

		public static extern int ProcessorCount
		{
			[EnvironmentPermission(SecurityAction.Demand, Read = "NUMBER_OF_PROCESSORS")]
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
					return Path.Combine(Path.Combine(new DirectoryInfo(Path.GetDirectoryName(typeof(int).Assembly.Location)).Parent.Parent.FullName, "mono"), "gac");
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPageSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_bundled_machine_config();

		internal static string GetBundledMachineConfig()
		{
			return Environment.get_bundled_machine_config();
		}

		internal static bool IsUnix
		{
			get
			{
				int platform = (int)Environment.Platform;
				return platform == 4 || platform == 128 || platform == 6;
			}
		}

		internal static bool IsMacOS
		{
			get
			{
				return Environment.Platform == PlatformID.MacOSX;
			}
		}

		internal static bool IsCLRHosted
		{
			get
			{
				return false;
			}
		}

		internal static void TriggerCodeContractFailure(ContractFailureKind failureKind, string message, string condition, string exceptionAsString)
		{
		}

		internal static string GetStackTrace(Exception e, bool needFileInfo)
		{
			StackTrace stackTrace;
			if (e == null)
			{
				stackTrace = new StackTrace(needFileInfo);
			}
			else
			{
				stackTrace = new StackTrace(e, needFileInfo);
			}
			return stackTrace.ToString(global::System.Diagnostics.StackTrace.TraceFormat.Normal);
		}

		internal static bool IsWinRTSupported
		{
			get
			{
				return true;
			}
		}

		private const int mono_corlib_version = 1051100001;

		private static string nl;

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
			CommonProgramFiles = 43,
			MyVideos = 14,
			NetworkShortcuts = 19,
			Fonts,
			CommonStartMenu = 22,
			CommonPrograms,
			CommonStartup,
			CommonDesktopDirectory,
			PrinterShortcuts = 27,
			Windows = 36,
			UserProfile = 40,
			SystemX86,
			ProgramFilesX86,
			CommonProgramFilesX86 = 44,
			CommonTemplates,
			CommonDocuments,
			CommonAdminTools,
			AdminTools,
			CommonMusic = 53,
			CommonPictures,
			CommonVideos,
			Resources,
			LocalizedResources,
			CommonOemLinks,
			CDBurning
		}

		public enum SpecialFolderOption
		{
			None,
			DoNotVerify = 16384,
			Create = 32768
		}
	}
}
