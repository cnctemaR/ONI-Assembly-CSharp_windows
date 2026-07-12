using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeWindowsPlatform : IDetourNativePlatform
	{
		public DetourNativeWindowsPlatform(IDetourNativePlatform inner)
		{
			this.Inner = inner;
		}

		public void MakeWritable(IntPtr src, uint size)
		{
			DetourNativeWindowsPlatform.PAGE page;
			if (!DetourNativeWindowsPlatform.VirtualProtect(src, (IntPtr)((long)((ulong)size)), DetourNativeWindowsPlatform.PAGE.EXECUTE_READWRITE, out page))
			{
				throw this.LogAllSections(Marshal.GetLastWin32Error(), "MakeWriteable", src, size);
			}
		}

		public void MakeExecutable(IntPtr src, uint size)
		{
			DetourNativeWindowsPlatform.PAGE page;
			if (!DetourNativeWindowsPlatform.VirtualProtect(src, (IntPtr)((long)((ulong)size)), DetourNativeWindowsPlatform.PAGE.EXECUTE_READWRITE, out page))
			{
				throw this.LogAllSections(Marshal.GetLastWin32Error(), "MakeExecutable", src, size);
			}
		}

		public void MakeReadWriteExecutable(IntPtr src, uint size)
		{
			DetourNativeWindowsPlatform.PAGE page;
			if (!DetourNativeWindowsPlatform.VirtualProtect(src, (IntPtr)((long)((ulong)size)), DetourNativeWindowsPlatform.PAGE.EXECUTE_READWRITE, out page))
			{
				throw this.LogAllSections(Marshal.GetLastWin32Error(), "MakeExecutable", src, size);
			}
		}

		public void FlushICache(IntPtr src, uint size)
		{
			if (!DetourNativeWindowsPlatform.FlushInstructionCache(DetourNativeWindowsPlatform.GetCurrentProcess(), src, (UIntPtr)size))
			{
				throw this.LogAllSections(Marshal.GetLastWin32Error(), "FlushICache", src, size);
			}
		}

		private unsafe Exception LogAllSections(int error, string from, IntPtr src, uint size)
		{
			Exception ex = new Win32Exception(error);
			if (MMDbgLog.Writer == null)
			{
				return ex;
			}
			MMDbgLog.Log(string.Format("{0} failed for 0x{1:X16} + {2} - logging all memory sections", from, (long)src, size));
			MMDbgLog.Log("reason: " + ex.Message);
			try
			{
				IntPtr currentProcess = DetourNativeWindowsPlatform.GetCurrentProcess();
				IntPtr intPtr = (IntPtr)65536;
				int num = 0;
				DetourNativeWindowsPlatform.MEMORY_BASIC_INFORMATION memory_BASIC_INFORMATION;
				while (DetourNativeWindowsPlatform.VirtualQueryEx(currentProcess, intPtr, out memory_BASIC_INFORMATION, sizeof(DetourNativeWindowsPlatform.MEMORY_BASIC_INFORMATION)) != 0)
				{
					ulong num2 = (ulong)(long)src;
					ulong num3 = num2 + (ulong)size;
					long num4 = (long)memory_BASIC_INFORMATION.BaseAddress;
					ulong num5 = (ulong)(num4 + (long)memory_BASIC_INFORMATION.RegionSize);
					bool flag = num4 <= (long)num3 && num2 <= num5;
					MMDbgLog.Log(string.Format("{0} #{1}", flag ? "*" : "-", num++));
					MMDbgLog.Log(string.Format("addr: 0x{0:X16}", (long)memory_BASIC_INFORMATION.BaseAddress));
					MMDbgLog.Log(string.Format("size: 0x{0:X16}", (long)memory_BASIC_INFORMATION.RegionSize));
					MMDbgLog.Log(string.Format("aaddr: 0x{0:X16}", (long)memory_BASIC_INFORMATION.AllocationBase));
					MMDbgLog.Log(string.Format("state: {0}", memory_BASIC_INFORMATION.State));
					MMDbgLog.Log(string.Format("type: {0}", memory_BASIC_INFORMATION.Type));
					MMDbgLog.Log(string.Format("protect: {0}", memory_BASIC_INFORMATION.Protect));
					MMDbgLog.Log(string.Format("aprotect: {0}", memory_BASIC_INFORMATION.AllocationProtect));
					try
					{
						IntPtr intPtr2 = intPtr;
						intPtr = (IntPtr)((long)memory_BASIC_INFORMATION.BaseAddress + (long)memory_BASIC_INFORMATION.RegionSize);
						if ((long)intPtr > (long)intPtr2)
						{
							continue;
						}
					}
					catch (OverflowException)
					{
						MMDbgLog.Log("overflow");
					}
					break;
				}
			}
			catch
			{
				throw ex;
			}
			return ex;
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

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, DetourNativeWindowsPlatform.PAGE flNewProtect, out DetourNativeWindowsPlatform.PAGE lpflOldProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, UIntPtr dwSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out DetourNativeWindowsPlatform.MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

		private readonly IDetourNativePlatform Inner;

		[Flags]
		private enum PAGE : uint
		{
			UNSET = 0U,
			NOACCESS = 1U,
			READONLY = 2U,
			READWRITE = 4U,
			WRITECOPY = 8U,
			EXECUTE = 16U,
			EXECUTE_READ = 32U,
			EXECUTE_READWRITE = 64U,
			EXECUTE_WRITECOPY = 128U,
			GUARD = 256U,
			NOCACHE = 512U,
			WRITECOMBINE = 1024U
		}

		private enum MEM : uint
		{
			UNSET,
			MEM_COMMIT = 4096U,
			MEM_RESERVE = 8192U,
			MEM_FREE = 65536U,
			MEM_PRIVATE = 131072U,
			MEM_MAPPED = 262144U,
			MEM_IMAGE = 16777216U
		}

		private struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;

			public IntPtr AllocationBase;

			public DetourNativeWindowsPlatform.PAGE AllocationProtect;

			public IntPtr RegionSize;

			public DetourNativeWindowsPlatform.MEM State;

			public DetourNativeWindowsPlatform.PAGE Protect;

			public DetourNativeWindowsPlatform.MEM Type;
		}
	}
}
