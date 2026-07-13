using System;

namespace UnityEngine
{
	internal struct StructWithSelfPointer
	{
		public int value;

		public unsafe StructWithSelfPointer* other;
	}
}
