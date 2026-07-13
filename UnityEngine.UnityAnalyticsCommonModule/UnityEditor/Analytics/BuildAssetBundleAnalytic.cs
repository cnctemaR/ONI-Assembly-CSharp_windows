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
	public class BuildAssetBundleAnalytic : AnalyticsEventBase
	{
		public BuildAssetBundleAnalytic()
			: base("unity5BuildAssetBundles", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static BuildAssetBundleAnalytic CreateBuildAssetBundleAnalytic()
		{
			return new BuildAssetBundleAnalytic();
		}

		public bool success;

		public string error;
	}
}
