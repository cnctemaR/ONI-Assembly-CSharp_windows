using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class BatchRenderGroupUsageAnalytic : AnalyticsEventBase
	{
		public BatchRenderGroupUsageAnalytic()
			: base("brgUsageEvent", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		public static BatchRenderGroupUsageAnalytic CreateBatchRenderGroupUsageAnalytic()
		{
			return new BatchRenderGroupUsageAnalytic();
		}

		public int maxBRGInstance;

		public int maxMeshCount;

		public int maxMaterialCount;

		public int maxDrawCommandBatch;
	}
}
