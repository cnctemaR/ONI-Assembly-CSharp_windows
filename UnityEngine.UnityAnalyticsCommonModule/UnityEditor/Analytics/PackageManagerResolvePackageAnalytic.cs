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
	public class PackageManagerResolvePackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerResolvePackageAnalytic()
			: base("resolvePackages")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerResolvePackageAnalytic CreatePackageManagerResolvePackageAnalytic()
		{
			return new PackageManagerResolvePackageAnalytic();
		}

		public string[] packages;

		public string[] package_registries;

		public string[] package_signatures;

		public string[] package_sources;

		public string[] package_types;

		public string[] package_compliance_statuses;

		public string[] package_signature_errorCodes;

		public string[] package_publishing_channels;
	}
}
