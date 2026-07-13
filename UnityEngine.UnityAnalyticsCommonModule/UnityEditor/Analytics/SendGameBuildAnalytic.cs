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
	public class SendGameBuildAnalytic : AnalyticsEventBase
	{
		public SendGameBuildAnalytic()
			: base("navigation_gamebuild_info", 1, SendEventOptions.kAppendBuildGuid, "")
		{
		}

		[RequiredByNativeCode]
		internal static SendGameBuildAnalytic CreateSendGameBuildAnalytic()
		{
			return new SendGameBuildAnalytic();
		}

		private int navmesh_count;
	}
}
