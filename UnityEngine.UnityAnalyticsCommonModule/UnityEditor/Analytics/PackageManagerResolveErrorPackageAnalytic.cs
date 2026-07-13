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
	public class PackageManagerResolveErrorPackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerResolveErrorPackageAnalytic()
			: base("resolveErrorUserAction")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerResolveErrorPackageAnalytic CreatePackageManagerResolveErrorPackageAnalytic()
		{
			return new PackageManagerResolveErrorPackageAnalytic();
		}

		public string reason;

		public string action;
	}
}
