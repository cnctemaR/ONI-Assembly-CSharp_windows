using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoMod.Core.Interop
{
	internal static class Unix
	{
		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "read")]
		[return: NativeInteger]
		public static extern IntPtr Read(int fd, IntPtr buf, [NativeInteger] IntPtr count);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "write")]
		[return: NativeInteger]
		public static extern IntPtr Write(int fd, IntPtr buf, [NativeInteger] IntPtr count);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pipe2")]
		public unsafe static extern int Pipe2(int* pipefd, Unix.PipeFlags flags);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "close")]
		public static extern int Close(int fd);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mmap")]
		[return: NativeInteger]
		public static extern IntPtr Mmap(IntPtr addr, [NativeInteger] UIntPtr length, Unix.Protection prot, Unix.MmapFlags flags, int fd, int offset);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "munmap")]
		public static extern int Munmap(IntPtr addr, [NativeInteger] UIntPtr length);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mprotect")]
		public static extern int Mprotect(IntPtr addr, [NativeInteger] UIntPtr len, Unix.Protection prot);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sysconf")]
		public static extern long Sysconf(Unix.SysconfName name);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mincore")]
		public unsafe static extern int Mincore(IntPtr addr, [NativeInteger] UIntPtr len, byte* vec);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mkstemp")]
		public unsafe static extern int MkSTemp(byte* template);

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int* __errno_location();

		public unsafe static int Errno
		{
			get
			{
				return *Unix.__errno_location();
			}
		}

		static Unix()
		{
			int errno = Unix.Errno;
		}

		[Nullable(1)]
		public const string LibC = "libc";

		[Flags]
		public enum PipeFlags
		{
			CloseOnExec = 524288
		}

		[Flags]
		public enum Protection
		{
			None = 0,
			Read = 1,
			Write = 2,
			Execute = 4
		}

		[Flags]
		public enum MmapFlags
		{
			Shared = 1,
			Private = 2,
			SharedValidate = 3,
			Fixed = 16,
			Anonymous = 32,
			GrowsDown = 256,
			DenyWrite = 2048,
			[Obsolete("Use Protection.Execute instead", true)]
			Executable = 4096,
			Locked = 8192,
			NoReserve = 16384,
			Populate = 32768,
			NonBlock = 65536,
			Stack = 131072,
			HugeTLB = 262144,
			Sync = 524288,
			FixedNoReplace = 1048576
		}

		public enum SysconfName
		{
			ArgMax,
			ChildMax,
			ClockTick,
			NGroupsMax,
			OpenMax,
			StreamMax,
			TZNameMax,
			JobControl,
			SavedIds,
			RealtimeSignals,
			PriorityScheduling,
			Timers,
			AsyncIO,
			PrioritizedIO,
			SynchronizedIO,
			FSync,
			MappedFiles,
			MemLock,
			MemLockRange,
			MemoryProtection,
			MessagePassing,
			Semaphores,
			SharedMemoryObjects,
			AIOListIOMax,
			AIOMax,
			AIOPrioDeltaMax,
			DelayTimerMax,
			MQOpenMax,
			MQPrioMax,
			Version,
			PageSize,
			RTSigMax,
			SemNSemsMax,
			SemValueMax,
			SigQueueMax,
			TimerMax
		}
	}
}
