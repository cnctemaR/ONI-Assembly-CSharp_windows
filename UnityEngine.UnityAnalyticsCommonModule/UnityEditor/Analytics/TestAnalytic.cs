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
	internal class TestAnalytic : AnalyticsEventBase
	{
		public TestAnalytic()
			: base("TestAnalytic", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		public static TestAnalytic CreateTestAnalytic()
		{
			return new TestAnalytic();
		}

		public int param;
	}
}
