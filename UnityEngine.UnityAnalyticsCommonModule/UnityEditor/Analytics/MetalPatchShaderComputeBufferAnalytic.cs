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
	public class MetalPatchShaderComputeBufferAnalytic : AnalyticsEventBase
	{
		public MetalPatchShaderComputeBufferAnalytic()
			: base("MetalPatchShaderComputeBuffersUsage", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static MetalPatchShaderComputeBufferAnalytic CreateMetalPatchShaderComputeBufferAnalytic()
		{
			return new MetalPatchShaderComputeBufferAnalytic();
		}
	}
}
