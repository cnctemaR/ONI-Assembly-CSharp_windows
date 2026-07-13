using System;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[ExcludeFromDocs]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class AssetImportAnalytic : AnalyticsEventBase
	{
		public AssetImportAnalytic()
			: base("assetImport", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		public static AssetImportAnalytic CreateAssetImportAnalytic()
		{
			return new AssetImportAnalytic();
		}

		public string package_name;

		public int package_import_choice;
	}
}
