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
	public class CollabOperationAnalytic : AnalyticsEventBase
	{
		public CollabOperationAnalytic()
			: base("collabOperation", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static CollabOperationAnalytic CreateCollabOperationAnalytic()
		{
			return new CollabOperationAnalytic();
		}

		public string category;

		public string operation;

		public string result;

		public long start_ts;

		public long duration;
	}
}
