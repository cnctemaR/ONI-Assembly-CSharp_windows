using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Runtime/Camera/LightProbeStructs.h")]
	internal struct Tetrahedron
	{
		[FixedBuffer(typeof(int), 4)]
		internal Tetrahedron.<indices>e__FixedBuffer indices;

		[FixedBuffer(typeof(int), 4)]
		internal Tetrahedron.<neighbors>e__FixedBuffer neighbors;

		internal Matrix3x4f matrix;

		internal bool isValid;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <indices>e__FixedBuffer
		{
			public int FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <neighbors>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
