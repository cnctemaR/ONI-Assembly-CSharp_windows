using System;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeX86Platform : IDetourNativePlatform
	{
		private static bool Is32Bit(long to)
		{
			return (to & 2147483647L) == to;
		}

		private unsafe static DetourNativeX86Platform.DetourType GetDetourType(IntPtr from, IntPtr to)
		{
			long num = (long)to - ((long)from + 5L);
			if ((DetourNativeX86Platform.Is32Bit(num) || DetourNativeX86Platform.Is32Bit(-num)) && ((byte*)(void*)from)[5] != 95)
			{
				return DetourNativeX86Platform.DetourType.Rel32;
			}
			if (DetourNativeX86Platform.Is32Bit((long)to))
			{
				return DetourNativeX86Platform.DetourType.Abs32;
			}
			return DetourNativeX86Platform.DetourType.Abs64;
		}

		public NativeDetourData Create(IntPtr from, IntPtr to, byte? type)
		{
			NativeDetourData nativeDetourData = new NativeDetourData
			{
				Method = from,
				Target = to
			};
			nativeDetourData.Size = DetourNativeX86Platform.DetourSizes[(int)(nativeDetourData.Type = type ?? ((byte)DetourNativeX86Platform.GetDetourType(from, to)))];
			return nativeDetourData;
		}

		public void Free(NativeDetourData detour)
		{
		}

		public void Apply(NativeDetourData detour)
		{
			int num = 0;
			switch (detour.Type)
			{
			case 0:
				detour.Method.Write(ref num, 233);
				detour.Method.Write(ref num, (uint)((int)((long)detour.Target - ((long)detour.Method + (long)num + 4L))));
				return;
			case 1:
				detour.Method.Write(ref num, 104);
				detour.Method.Write(ref num, (uint)(int)detour.Target);
				detour.Method.Write(ref num, 195);
				return;
			case 2:
				detour.Method.Write(ref num, byte.MaxValue);
				detour.Method.Write(ref num, 37);
				detour.Method.Write(ref num, 0U);
				detour.Method.Write(ref num, (ulong)(long)detour.Target);
				return;
			default:
				throw new NotSupportedException(string.Format("Unknown detour type {0}", detour.Type));
			}
		}

		public unsafe void Copy(IntPtr src, IntPtr dst, byte type)
		{
			switch (type)
			{
			case 0:
				*(UIntPtr)(long)dst = (int)(*(UIntPtr)(long)src);
				*(UIntPtr)((long)dst + 4L) = *(UIntPtr)((long)src + 4L);
				return;
			case 1:
				*(UIntPtr)(long)dst = (int)(*(UIntPtr)(long)src);
				*(UIntPtr)((long)dst + 4L) = (short)(*(UIntPtr)((long)src + 4L));
				return;
			case 2:
				*(UIntPtr)(long)dst = *(UIntPtr)(long)src;
				*(UIntPtr)((long)dst + 8L) = (int)(*(UIntPtr)((long)src + 8L));
				*(UIntPtr)((long)dst + 12L) = (short)(*(UIntPtr)((long)src + 12L));
				return;
			default:
				throw new NotSupportedException(string.Format("Unknown detour type {0}", type));
			}
		}

		public void MakeWritable(IntPtr src, uint size)
		{
		}

		public void MakeExecutable(IntPtr src, uint size)
		{
		}

		public void FlushICache(IntPtr src, uint size)
		{
		}

		public IntPtr MemAlloc(uint size)
		{
			return Marshal.AllocHGlobal((int)size);
		}

		public void MemFree(IntPtr ptr)
		{
			Marshal.FreeHGlobal(ptr);
		}

		private static readonly uint[] DetourSizes = new uint[] { 5U, 6U, 14U };

		public enum DetourType : byte
		{
			Rel32,
			Abs32,
			Abs64
		}
	}
}
