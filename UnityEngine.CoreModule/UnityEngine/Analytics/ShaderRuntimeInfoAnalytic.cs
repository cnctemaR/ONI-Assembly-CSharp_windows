using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class ShaderRuntimeInfoAnalytic : AnalyticsEventBase
	{
		private ShaderRuntimeInfoAnalytic()
			: base("shaderRuntimeInfo", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		public static ShaderRuntimeInfoAnalytic CreateShaderRuntimeInfoAnalytic()
		{
			return new ShaderRuntimeInfoAnalytic();
		}

		public long VariantsRequested = 0L;

		public long VariantsRequestedMissing = 0L;

		public long VariantsRequestedUnsupported = 0L;

		public long VariantsRequestedCompiled = 0L;

		public long VariantsRequestedViaWarmup = 0L;

		public long VariantsUnused = 0L;

		public int VariantsCompilationTimeTotal = 0;

		public int VariantsCompilationTimeMax = 0;

		public int VariantsCompilationTimeMedian = 0;

		public int VariantsWarmupTimeTotal = 0;

		public int VariantsWarmupTimeMax = 0;

		public int VariantsWarmupTimeMedian = 0;

		public bool UseProgressiveWarmup = false;

		public int ShaderChunkSizeMin = 0;

		public int ShaderChunkSizeMax = 0;

		public int ShaderChunkSizeAvg = 0;

		public int ShaderChunkCountMin = 0;

		public int ShaderChunkCountMax = 0;

		public int ShaderChunkCountAvg = 0;
	}
}
