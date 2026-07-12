using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MonoMod.Utils
{
	internal static class PlatformHelper
	{
		private static void DeterminePlatform()
		{
			PlatformHelper._current = Platform.Unknown;
			PropertyInfo property = typeof(Environment).GetProperty("Platform", BindingFlags.Static | BindingFlags.NonPublic);
			string text;
			if (property != null)
			{
				text = property.GetValue(null, new object[0]).ToString();
			}
			else
			{
				text = Environment.OSVersion.Platform.ToString();
			}
			text = text.ToLowerInvariant();
			if (text.Contains("win"))
			{
				PlatformHelper._current = Platform.Windows;
			}
			else if (text.Contains("mac") || text.Contains("osx"))
			{
				PlatformHelper._current = Platform.MacOS;
			}
			else if (text.Contains("lin") || text.Contains("unix"))
			{
				PlatformHelper._current = Platform.Linux;
			}
			if (PlatformHelper.Is(Platform.Linux) && Directory.Exists("/data") && File.Exists("/system/build.prop"))
			{
				PlatformHelper._current = Platform.Android;
			}
			else if (PlatformHelper.Is(Platform.Unix) && Directory.Exists("/Applications") && Directory.Exists("/System"))
			{
				PlatformHelper._current = Platform.iOS;
			}
			PropertyInfo property2 = typeof(Environment).GetProperty("Is64BitOperatingSystem");
			MethodInfo methodInfo = ((property2 != null) ? property2.GetGetMethod() : null);
			if (methodInfo != null)
			{
				PlatformHelper._current |= (((bool)methodInfo.Invoke(null, new object[0])) ? Platform.Bits64 : ((Platform)0));
			}
			else
			{
				PlatformHelper._current |= ((IntPtr.Size >= 8) ? Platform.Bits64 : ((Platform)0));
			}
			if ((PlatformHelper.Is(Platform.Unix) || PlatformHelper.Is(Platform.Unknown)) && Type.GetType("Mono.Runtime") != null)
			{
				try
				{
					string text2;
					using (Process process = Process.Start(new ProcessStartInfo("uname", "-m")
					{
						UseShellExecute = false,
						RedirectStandardOutput = true
					}))
					{
						text2 = process.StandardOutput.ReadLine().Trim();
					}
					if (text2.StartsWith("aarch") || text2.StartsWith("arm"))
					{
						PlatformHelper._current |= Platform.ARM;
					}
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			PortableExecutableKinds portableExecutableKinds;
			ImageFileMachine imageFileMachine;
			typeof(object).Module.GetPEKind(out portableExecutableKinds, out imageFileMachine);
			if (imageFileMachine == ImageFileMachine.ARM)
			{
				PlatformHelper._current |= Platform.ARM;
			}
		}

		public static Platform Current
		{
			get
			{
				if (!PlatformHelper._currentLocked)
				{
					if (PlatformHelper._current == Platform.Unknown)
					{
						PlatformHelper.DeterminePlatform();
					}
					PlatformHelper._currentLocked = true;
				}
				return PlatformHelper._current;
			}
			set
			{
				if (PlatformHelper._currentLocked)
				{
					throw new InvalidOperationException("Cannot set the value of PlatformHelper.Current once it has been accessed.");
				}
				PlatformHelper._current = value;
			}
		}

		public static string LibrarySuffix
		{
			get
			{
				if (PlatformHelper._librarySuffix == null)
				{
					PlatformHelper._librarySuffix = (PlatformHelper.Is(Platform.MacOS) ? "dylib" : (PlatformHelper.Is(Platform.Unix) ? "so" : "dll"));
				}
				return PlatformHelper._librarySuffix;
			}
		}

		public static bool Is(Platform platform)
		{
			return (PlatformHelper.Current & platform) == platform;
		}

		private static Platform _current = Platform.Unknown;

		private static bool _currentLocked = false;

		private static string _librarySuffix;
	}
}
