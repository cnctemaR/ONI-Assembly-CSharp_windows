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
	internal class StallMarkerAnalytic : AnalyticsEventBase
	{
		public StallMarkerAnalytic()
			: base("editorStallMarker", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static StallMarkerAnalytic CreateStallMarkerAnalytic()
		{
			return new StallMarkerAnalytic();
		}

		public string Name;

		public bool HasProgressMarkup;

		public double Duration;
	}
}
