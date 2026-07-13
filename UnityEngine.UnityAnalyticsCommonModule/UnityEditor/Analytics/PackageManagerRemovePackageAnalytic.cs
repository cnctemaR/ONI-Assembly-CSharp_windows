using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PackageManagerRemovePackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerRemovePackageAnalytic()
			: base("removePackage")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerRemovePackageAnalytic CreatePackageManagerRemovePackageAnalytic()
		{
			return new PackageManagerRemovePackageAnalytic();
		}
	}
}
