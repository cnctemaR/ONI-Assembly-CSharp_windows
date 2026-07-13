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
	public class PackageManagerStartServerPackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerStartServerPackageAnalytic()
			: base("startPackageManagerServer")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerStartServerPackageAnalytic CreatePackageManagerStartServerPackageAnalytic()
		{
			return new PackageManagerStartServerPackageAnalytic();
		}
	}
}
