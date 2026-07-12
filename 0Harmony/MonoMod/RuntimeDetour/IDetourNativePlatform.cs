using System;

namespace MonoMod.RuntimeDetour
{
	internal interface IDetourNativePlatform
	{
		NativeDetourData Create(IntPtr from, IntPtr to, byte? type = null);

		void Free(NativeDetourData detour);

		void Apply(NativeDetourData detour);

		void Copy(IntPtr src, IntPtr dst, byte type);

		void MakeWritable(IntPtr src, uint size);

		void MakeExecutable(IntPtr src, uint size);

		void FlushICache(IntPtr src, uint size);

		IntPtr MemAlloc(uint size);

		void MemFree(IntPtr ptr);
	}
}
