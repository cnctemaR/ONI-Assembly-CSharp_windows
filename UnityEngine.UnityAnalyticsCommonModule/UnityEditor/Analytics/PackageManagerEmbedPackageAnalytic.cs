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
	public class PackageManagerEmbedPackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerEmbedPackageAnalytic()
			: base("embedPackage")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerEmbedPackageAnalytic CreatePackageManagerEmbedPackageAnalytic()
		{
			return new PackageManagerEmbedPackageAnalytic();
		}
	}
}
