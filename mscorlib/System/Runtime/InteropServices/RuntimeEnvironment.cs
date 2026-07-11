using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public class RuntimeEnvironment
	{
		[Obsolete("Do not create instances of the RuntimeEnvironment class.  Call the static methods directly on this type instead", true)]
		public RuntimeEnvironment()
		{
		}

		public static bool FromGlobalAccessCache(Assembly a)
		{
			return a.GlobalAssemblyCache;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetSystemVersion()
		{
			return Assembly.GetExecutingAssembly().ImageRuntimeVersion;
		}

		[SecuritySafeCritical]
		public static string GetRuntimeDirectory()
		{
			if (Environment.GetEnvironmentVariable("CSC_SDK_PATH_DISABLED") != null)
			{
				return null;
			}
			return RuntimeEnvironment.GetRuntimeDirectoryImpl();
		}

		private static string GetRuntimeDirectoryImpl()
		{
			return Path.GetDirectoryName(typeof(object).Assembly.Location);
		}

		public static string SystemConfigurationFile
		{
			[SecuritySafeCritical]
			get
			{
				return Environment.GetMachineConfigPath();
			}
		}

		private static IntPtr GetRuntimeInterfaceImpl(Guid clsid, Guid riid)
		{
			throw new NotSupportedException();
		}

		[SecurityCritical]
		[ComVisible(false)]
		public static IntPtr GetRuntimeInterfaceAsIntPtr(Guid clsid, Guid riid)
		{
			return RuntimeEnvironment.GetRuntimeInterfaceImpl(clsid, riid);
		}

		[ComVisible(false)]
		[SecurityCritical]
		public static object GetRuntimeInterfaceAsObject(Guid clsid, Guid riid)
		{
			IntPtr intPtr = IntPtr.Zero;
			object objectForIUnknown;
			try
			{
				intPtr = RuntimeEnvironment.GetRuntimeInterfaceImpl(clsid, riid);
				objectForIUnknown = Marshal.GetObjectForIUnknown(intPtr);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.Release(intPtr);
				}
			}
			return objectForIUnknown;
		}
	}
}
