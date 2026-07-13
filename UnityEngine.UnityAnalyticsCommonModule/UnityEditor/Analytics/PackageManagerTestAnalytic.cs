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
	public class PackageManagerTestAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerTestAnalytic()
			: base("PackageManager")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerTestAnalytic CreatePackageManagerTestAnalytic()
		{
			return new PackageManagerTestAnalytic();
		}
	}
}
