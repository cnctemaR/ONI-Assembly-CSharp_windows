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
	public class NavmeshBakingAnalytic : AnalyticsEventBase
	{
		public NavmeshBakingAnalytic()
			: base("navigation_navmesh_baking", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static NavmeshBakingAnalytic CreateNavmeshBakingAnalytic()
		{
			return new NavmeshBakingAnalytic();
		}

		private bool new_nav_api;

		private bool bake_at_runtime;

		private int height_meshes_count;

		private int offmesh_links_count;
	}
}
