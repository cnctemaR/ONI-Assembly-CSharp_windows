using System;

namespace MonoMod.RuntimeDetour
{
	internal struct NativeDetourData
	{
		public IntPtr Method;

		public IntPtr Target;

		public byte Type;

		public uint Size;

		public IntPtr Extra;
	}
}
