using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal struct StructFixedBuffer
	{
		[FixedBuffer(typeof(int), 4)]
		public StructFixedBuffer.<SomeInts>e__FixedBuffer SomeInts;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <SomeInts>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
