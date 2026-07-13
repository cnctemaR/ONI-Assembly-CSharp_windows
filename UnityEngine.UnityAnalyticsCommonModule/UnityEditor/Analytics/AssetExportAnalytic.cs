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
	internal class AssetExportAnalytic : AnalyticsEventBase
	{
		public AssetExportAnalytic()
			: base("assetExport", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		public static AssetExportAnalytic CreateAssetExportAnalytic()
		{
			return new AssetExportAnalytic();
		}

		public string package_name;

		public string error_message;

		public int items_count;

		public string[] asset_extensions;

		public bool include_upm_dependencies;
	}
}
