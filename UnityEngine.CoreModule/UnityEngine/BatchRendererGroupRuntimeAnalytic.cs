using System;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class BatchRendererGroupRuntimeAnalytic : AnalyticsEventBase
	{
		private BatchRendererGroupRuntimeAnalytic()
			: base("brgPlayerUsage", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		public static BatchRendererGroupRuntimeAnalytic CreateBatchRendererGroupRuntimeAnalytic()
		{
			return new BatchRendererGroupRuntimeAnalytic();
		}

		private int brgRuntimeStatus;
	}
}
