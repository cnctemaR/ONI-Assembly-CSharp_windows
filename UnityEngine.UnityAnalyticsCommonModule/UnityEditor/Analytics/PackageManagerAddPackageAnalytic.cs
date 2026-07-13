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
	public class PackageManagerAddPackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerAddPackageAnalytic()
			: base("addPackage")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerAddPackageAnalytic CreatePackageManagerAddPackageAnalytic()
		{
			return new PackageManagerAddPackageAnalytic();
		}
	}
}
