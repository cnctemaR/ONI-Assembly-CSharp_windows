using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeMonoPlatform : IDetourNativePlatform
	{
		public DetourNativeMonoPlatform(IDetourNativePlatform inner, string libmono)
		{
			this.Inner = inner;
			Dictionary<string, List<DynDllMapping>> dictionary = new Dictionary<string, List<DynDllMapping>>();
			if (!string.IsNullOrEmpty(libmono))
			{
				dictionary.Add("mono", new List<DynDllMapping> { libmono });
			}
			DynDll.ResolveDynDllImports(this, dictionary);
			this._Pagesize = (long)this.mono_pagesize();
		}

		private void SetMemPerms(IntPtr start, ulong len, DetourNativeMonoPlatform.MmapProts prot)
		{
			long pagesize = this._Pagesize;
			long num = (long)start & ~(pagesize - 1L);
			long num2 = ((long)start + (long)len + pagesize - 1L) & ~(pagesize - 1L);
			if (this.mono_mprotect((IntPtr)num, (IntPtr)(num2 - num), (int)prot) != 0 && Marshal.GetLastWin32Error() != 0)
			{
				throw new Win32Exception();
			}
		}

		public void MakeWritable(IntPtr src, uint size)
		{
			this.SetMemPerms(src, (ulong)size, DetourNativeMonoPlatform.MmapProts.PROT_READ | DetourNativeMonoPlatform.MmapProts.PROT_WRITE | DetourNativeMonoPlatform.MmapProts.PROT_EXEC);
		}

		public void MakeExecutable(IntPtr src, uint size)
		{
			this.SetMemPerms(src, (ulong)size, DetourNativeMonoPlatform.MmapProts.PROT_READ | DetourNativeMonoPlatform.MmapProts.PROT_WRITE | DetourNativeMonoPlatform.MmapProts.PROT_EXEC);
		}

		public void MakeReadWriteExecutable(IntPtr src, uint size)
		{
			this.SetMemPerms(src, (ulong)size, DetourNativeMonoPlatform.MmapProts.PROT_READ | DetourNativeMonoPlatform.MmapProts.PROT_WRITE | DetourNativeMonoPlatform.MmapProts.PROT_EXEC);
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

		private readonly IDetourNativePlatform Inner;

		private readonly long _Pagesize;

		[DynDllImport("mono", new string[] { })]
		private DetourNativeMonoPlatform.d_mono_pagesize mono_pagesize;

		[DynDllImport("mono", new string[] { })]
		private DetourNativeMonoPlatform.d_mono_mprotect mono_mprotect;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int d_mono_pagesize();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
		private delegate int d_mono_mprotect(IntPtr addr, IntPtr length, int flags);

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
