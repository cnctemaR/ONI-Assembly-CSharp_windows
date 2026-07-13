using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Runtime/GI/SceneData.h")]
	internal struct LightProbeOcclusion
	{
		[FixedBuffer(typeof(int), 4)]
		internal LightProbeOcclusion.<m_ProbeOcclusionLightIndex>e__FixedBuffer m_ProbeOcclusionLightIndex;

		[FixedBuffer(typeof(float), 4)]
		internal LightProbeOcclusion.<m_Occlusion>e__FixedBuffer m_Occlusion;

		[FixedBuffer(typeof(sbyte), 4)]
		internal LightProbeOcclusion.<m_OcclusionMaskChannel>e__FixedBuffer m_OcclusionMaskChannel;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <m_Occlusion>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 4)]
		public struct <m_OcclusionMaskChannel>e__FixedBuffer
		{
			public sbyte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <m_ProbeOcclusionLightIndex>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
