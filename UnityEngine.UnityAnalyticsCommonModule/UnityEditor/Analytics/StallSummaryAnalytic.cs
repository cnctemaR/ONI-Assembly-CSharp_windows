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
	public class StallSummaryAnalytic : AnalyticsEventBase
	{
		public StallSummaryAnalytic()
			: base("editorStallSummary", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static StallSummaryAnalytic CreateStallSummaryAnalytic()
		{
			return new StallSummaryAnalytic();
		}

		public double Duration;
	}
}
