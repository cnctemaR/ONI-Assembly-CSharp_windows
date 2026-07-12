using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
	internal struct Hashes
	{
		public const int kSize = 4;

		[FixedBuffer(typeof(int), 4)]
		public Hashes.<hashes>e__FixedBuffer hashes;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <hashes>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
