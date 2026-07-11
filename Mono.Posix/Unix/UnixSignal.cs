using System;
using System.Runtime.InteropServices;
using System.Threading;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public class UnixSignal : WaitHandle
	{
		static UnixSignal()
		{
			Stdlib.VersionCheck();
		}

		public UnixSignal(Signum signum)
		{
			this.signum = NativeConvert.FromSignum(signum);
			this.signal_info = UnixSignal.install(this.signum);
			if (this.signal_info == IntPtr.Zero)
			{
				throw new ArgumentException("Unable to handle signal", "signum");
			}
		}

		public UnixSignal(RealTimeSignum rtsig)
		{
			this.signum = NativeConvert.FromRealTimeSignum(rtsig);
			this.signal_info = UnixSignal.install(this.signum);
			Errno lastError = Stdlib.GetLastError();
			if (!(this.signal_info == IntPtr.Zero))
			{
				return;
			}
			if (lastError == Errno.EADDRINUSE)
			{
				throw new ArgumentException("Signal registered outside of Mono.Posix", "signum");
			}
			throw new ArgumentException("Unable to handle signal", "signum");
		}

		public Signum Signum
		{
			get
			{
				if (this.IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is a RealTimeSignum");
				}
				return NativeConvert.ToSignum(this.signum);
			}
		}

		public RealTimeSignum RealTimeSignum
		{
			get
			{
				if (!this.IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is not a RealTimeSignum");
				}
				return NativeConvert.ToRealTimeSignum(this.signum - UnixSignal.GetSIGRTMIN());
			}
		}

		public bool IsRealTimeSignal
		{
			get
			{
				this.AssertValid();
				int sigrtmin = UnixSignal.GetSIGRTMIN();
				return sigrtmin != -1 && this.signum >= sigrtmin;
			}
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_install", SetLastError = true)]
		private static extern IntPtr install(int signum);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_uninstall")]
		private static extern int uninstall(IntPtr info);

		private static int RuntimeShuttingDownCallback()
		{
			if (!Environment.HasShutdownStarted)
			{
				return 0;
			}
			return 1;
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_WaitAny")]
		private static extern int WaitAny(IntPtr[] infos, int count, int timeout, UnixSignal.Mono_Posix_RuntimeIsShuttingDown shutting_down);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_SIGRTMIN")]
		internal static extern int GetSIGRTMIN();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_SIGRTMAX")]
		internal static extern int GetSIGRTMAX();

		private void AssertValid()
		{
			if (this.signal_info == IntPtr.Zero)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		private unsafe UnixSignal.SignalInfo* Info
		{
			get
			{
				this.AssertValid();
				return (UnixSignal.SignalInfo*)(void*)this.signal_info;
			}
		}

		public bool IsSet
		{
			get
			{
				return this.Count > 0;
			}
		}

		public unsafe bool Reset()
		{
			return Interlocked.Exchange(ref this.Info->count, 0) != 0;
		}

		public unsafe int Count
		{
			get
			{
				return this.Info->count;
			}
			set
			{
				Interlocked.Exchange(ref this.Info->count, value);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (this.signal_info == IntPtr.Zero)
			{
				return;
			}
			UnixSignal.uninstall(this.signal_info);
			this.signal_info = IntPtr.Zero;
		}

		public override bool WaitOne()
		{
			return this.WaitOne(-1, false);
		}

		public override bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.WaitOne((int)num, exitContext);
		}

		public override bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			this.AssertValid();
			if (exitContext)
			{
				throw new InvalidOperationException("exitContext is not supported");
			}
			if (millisecondsTimeout == 0)
			{
				return this.IsSet;
			}
			return UnixSignal.WaitAny(new UnixSignal[] { this }, millisecondsTimeout) == 0;
		}

		public static int WaitAny(UnixSignal[] signals)
		{
			return UnixSignal.WaitAny(signals, -1);
		}

		public static int WaitAny(UnixSignal[] signals, TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return UnixSignal.WaitAny(signals, (int)num);
		}

		public static int WaitAny(UnixSignal[] signals, int millisecondsTimeout)
		{
			if (signals == null)
			{
				throw new ArgumentNullException("signals");
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			IntPtr[] array = new IntPtr[signals.Length];
			for (int i = 0; i < signals.Length; i++)
			{
				array[i] = signals[i].signal_info;
				if (array[i] == IntPtr.Zero)
				{
					throw new InvalidOperationException("Disposed UnixSignal");
				}
			}
			return UnixSignal.WaitAny(array, array.Length, millisecondsTimeout, UnixSignal.ShuttingDown);
		}

		private int signum;

		private IntPtr signal_info;

		private static UnixSignal.Mono_Posix_RuntimeIsShuttingDown ShuttingDown = new UnixSignal.Mono_Posix_RuntimeIsShuttingDown(UnixSignal.RuntimeShuttingDownCallback);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int Mono_Posix_RuntimeIsShuttingDown();

		[Map]
		private struct SignalInfo
		{
			public int signum;

			public int count;

			public int read_fd;

			public int write_fd;

			public int pipecnt;

			public int pipelock;

			public int have_handler;

			public IntPtr handler;
		}
	}
}
