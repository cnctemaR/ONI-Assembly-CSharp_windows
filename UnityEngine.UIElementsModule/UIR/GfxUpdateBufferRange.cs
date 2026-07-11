using System;

namespace UnityEngine.UIR
{
	internal struct GfxUpdateBufferRange
	{
		public uint offsetFromWriteStart;

		public uint size;

		public UIntPtr source;
	}
}
