using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Runtime/Camera/LightProbeStructs.h")]
	internal struct Matrix3x4f
	{
		[FixedBuffer(typeof(float), 12)]
		internal Matrix3x4f.<m_Data>e__FixedBuffer m_Data;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 48)]
		public struct <m_Data>e__FixedBuffer
		{
			public float FixedElementField;
		}
	}
}
