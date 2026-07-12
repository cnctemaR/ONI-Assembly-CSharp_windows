using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeLibcPlatform : IDetourNativePlatform
	{
		public DetourNativeLibcPlatform(IDetourNativePlatform inner)
		{
			this.Inner = inner;
			PropertyInfo property = typeof(Environment).GetProperty("SystemPageSize");
			if (property == null)
			{
				throw new NotSupportedException("Unsupported runtime");
			}
			this._Pagesize = (long)((int)property.GetValue(null, new object[0]));
		}

		private void SetMemPerms(IntPtr start, ulong len, DetourNativeLibcPlatform.MmapProts prot)
		{
			long pagesize = this._Pagesize;
			long num = (long)start & ~(pagesize - 1L);
			long num2 = ((long)start + (long)len + pagesize - 1L) & ~(pagesize - 1L);
			if (DetourNativeLibcPlatform.mprotect((IntPtr)num, (IntPtr)(num2 - num), prot) != 0)
			{
				throw new Win32Exception();
			}
		}

		public void MakeWritable(IntPtr src, uint size)
		{
			this.SetMemPerms(src, (ulong)size, DetourNativeLibcPlatform.MmapProts.PROT_READ | DetourNativeLibcPlatform.MmapProts.PROT_WRITE | DetourNativeLibcPlatform.MmapProts.PROT_EXEC);
		}

		public void MakeExecutable(IntPtr src, uint size)
		{
			this.SetMemPerms(src, (ulong)size, DetourNativeLibcPlatform.MmapProts.PROT_READ | DetourNativeLibcPlatform.MmapProts.PROT_WRITE | DetourNativeLibcPlatform.MmapProts.PROT_EXEC);
		}

		public void FlushICache(IntPtr src, uint size)
		{
			this.Inner.FlushICache(src, size);
		}

		public NativeDetourData Create(IntPtr from, IntPtr to, byte? type)
		{
			return this.Inner.Create(from, to, type);
		}

		public void Free(NativeDetourData detour)
		{
			this.Inner.Free(detour);
		}

		public void Apply(NativeDetourData detour)
		{
			this.Inner.Apply(detour);
		}

		public void Copy(IntPtr src, IntPtr dst, byte type)
		{
			this.Inner.Copy(src, dst, type);
		}

		public IntPtr MemAlloc(uint size)
		{
			return this.Inner.MemAlloc(size);
		}

		public void MemFree(IntPtr ptr)
		{
			this.Inner.MemFree(ptr);
		}

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		private static extern int mprotect(IntPtr start, IntPtr len, DetourNativeLibcPlatform.MmapProts prot);

		private readonly IDetourNativePlatform Inner;

		private readonly long _Pagesize;

		[Flags]
		private enum MmapProts
		{
			PROT_READ = 1,
			PROT_WRITE = 2,
			PROT_EXEC = 4,
			PROT_NONE = 0,
			PROT_GROWSDOWN = 16777216,
			PROT_GROWSUP = 33554432
		}
	}
}
