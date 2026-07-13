using System;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class AssetImportStatusAnalytic : AnalyticsEventBase
	{
		public AssetImportStatusAnalytic()
			: base("assetImportStatus", 1, SendEventOptions.kAppendBuildTarget, "")
		{
		}

		[RequiredByNativeCode]
		public static AssetImportStatusAnalytic CreateAssetImportStatusAnalytic()
		{
			return new AssetImportStatusAnalytic();
		}

		public string package_name;

		public int package_items_count;

		public int package_import_status;

		public string error_message;

		public int project_assets_count;

		public int unselected_assets_count;

		public int selected_new_assets_count;

		public int selected_changed_assets_count;

		public int unchanged_assets_count;

		public string[] selected_asset_extensions;
	}
}
