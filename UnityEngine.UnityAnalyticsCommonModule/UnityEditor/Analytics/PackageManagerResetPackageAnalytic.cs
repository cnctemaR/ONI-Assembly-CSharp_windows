using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[ExcludeFromDocs]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PackageManagerResetPackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerResetPackageAnalytic()
			: base("resetToDefaultDependencies")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerResetPackageAnalytic CreatePackageManagerResetPackageAnalytic()
		{
			return new PackageManagerResetPackageAnalytic();
		}
	}
}
