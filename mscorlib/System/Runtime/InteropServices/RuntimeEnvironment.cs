using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public class RuntimeEnvironment
	{
		public static string SystemConfigurationFile
		{
			get
			{
				string machineConfigPath = Environment.GetMachineConfigPath();
				if (SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, machineConfigPath).Demand();
				}
				return machineConfigPath;
			}
		}

		public static bool FromGlobalAccessCache(Assembly a)
		{
			return a.GlobalAssemblyCache;
		}

		public static string GetRuntimeDirectory()
		{
			return Path.GetDirectoryName(typeof(int).Assembly.Location);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static string GetSystemVersion()
		{
			return string.Concat(new object[]
			{
				"v",
				Environment.Version.Major,
				".",
				Environment.Version.Minor,
				".",
				Environment.Version.Build
			});
		}
	}
}
