using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
	[ComVisible(true)]
	public abstract class WaitHandle : MarshalByRefObject, IDisposable
	{
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WaitAll_internal(WaitHandle[] handles, int ms, bool exitContext);

		private static void CheckArray(WaitHandle[] handles, bool waitAll)
		{
			if (handles == null)
			{
				throw new ArgumentNullException("waitHandles");
			}
			int num = handles.Length;
			if (num > 64)
			{
				throw new NotSupportedException("Too many handles");
			}
			foreach (WaitHandle waitHandle in handles)
			{
				if (waitHandle == null)
				{
					throw new ArgumentNullException("waitHandles", "null handle");
				}
				if (waitHandle.safe_wait_handle == null)
				{
					throw new ArgumentException("null element found", "waitHandle");
				}
			}
		}

		public static bool WaitAll(WaitHandle[] waitHandles)
		{
			WaitHandle.CheckArray(waitHandles, true);
			return WaitHandle.WaitAll_internal(waitHandles, -1, false);
		}

		public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
		{
			WaitHandle.CheckArray(waitHandles, true);
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			bool flag;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				flag = WaitHandle.WaitAll_internal(waitHandles, millisecondsTimeout, false);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
			return flag;
		}

		public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
		{
			WaitHandle.CheckArray(waitHandles, true);
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			bool flag;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				flag = WaitHandle.WaitAll_internal(waitHandles, (int)num, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int WaitAny_internal(WaitHandle[] handles, int ms, bool exitContext);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles)
		{
			WaitHandle.CheckArray(waitHandles, false);
			return WaitHandle.WaitAny_internal(waitHandles, -1, false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
		{
			WaitHandle.CheckArray(waitHandles, false);
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			int num;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				num = WaitHandle.WaitAny_internal(waitHandles, millisecondsTimeout, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
			return num;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout)
		{
			return WaitHandle.WaitAny(waitHandles, timeout, false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout)
		{
			return WaitHandle.WaitAny(waitHandles, millisecondsTimeout, false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
		{
			WaitHandle.CheckArray(waitHandles, false);
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			int num2;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				num2 = WaitHandle.WaitAny_internal(waitHandles, (int)num, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
			return num2;
		}

		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[Obsolete("In the profiles > 2.x, use SafeHandle instead of Handle")]
		public virtual IntPtr Handle
		{
			get
			{
				return this.safe_wait_handle.DangerousGetHandle();
			}
			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
			[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
			set
			{
				if (value == WaitHandle.InvalidHandle)
				{
					this.safe_wait_handle = new SafeWaitHandle(WaitHandle.InvalidHandle, false);
				}
				else
				{
					this.safe_wait_handle = new SafeWaitHandle(value, true);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool WaitOne_internal(IntPtr handle, int ms, bool exitContext);

		protected virtual void Dispose(bool explicitDisposing)
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.safe_wait_handle == null)
				{
					return;
				}
				lock (this)
				{
					if (this.safe_wait_handle != null)
					{
						this.safe_wait_handle.Dispose();
					}
				}
			}
		}

		public SafeWaitHandle SafeWaitHandle
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				return this.safe_wait_handle;
			}
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			set
			{
				if (value == null)
				{
					this.safe_wait_handle = new SafeWaitHandle(WaitHandle.InvalidHandle, false);
				}
				else
				{
					this.safe_wait_handle = value;
				}
			}
		}

		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn)
		{
			return WaitHandle.SignalAndWait(toSignal, toWaitOn, -1, false);
		}

		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, int millisecondsTimeout, bool exitContext)
		{
			if (toSignal == null)
			{
				throw new ArgumentNullException("toSignal");
			}
			if (toWaitOn == null)
			{
				throw new ArgumentNullException("toWaitOn");
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			return WaitHandle.SignalAndWait_Internal(toSignal.Handle, toWaitOn.Handle, millisecondsTimeout, exitContext);
		}

		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, TimeSpan timeout, bool exitContext)
		{
			double totalMilliseconds = timeout.TotalMilliseconds;
			if (totalMilliseconds > 2147483647.0)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return WaitHandle.SignalAndWait(toSignal, toWaitOn, Convert.ToInt32(totalMilliseconds), false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SignalAndWait_Internal(IntPtr toSignal, IntPtr toWaitOn, int ms, bool exitContext);

		public virtual bool WaitOne()
		{
			this.CheckDisposed();
			bool flag = false;
			bool flag2;
			try
			{
				this.safe_wait_handle.DangerousAddRef(ref flag);
				flag2 = this.WaitOne_internal(this.safe_wait_handle.DangerousGetHandle(), -1, false);
			}
			finally
			{
				if (flag)
				{
					this.safe_wait_handle.DangerousRelease();
				}
			}
			return flag2;
		}

		public virtual bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			this.CheckDisposed();
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			bool flag = false;
			bool flag2;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				this.safe_wait_handle.DangerousAddRef(ref flag);
				flag2 = this.WaitOne_internal(this.safe_wait_handle.DangerousGetHandle(), millisecondsTimeout, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
				if (flag)
				{
					this.safe_wait_handle.DangerousRelease();
				}
			}
			return flag2;
		}

		public virtual bool WaitOne(int millisecondsTimeout)
		{
			return this.WaitOne(millisecondsTimeout, false);
		}

		public virtual bool WaitOne(TimeSpan timeout)
		{
			return this.WaitOne(timeout, false);
		}

		public virtual bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			this.CheckDisposed();
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			bool flag = false;
			bool flag2;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				this.safe_wait_handle.DangerousAddRef(ref flag);
				flag2 = this.WaitOne_internal(this.safe_wait_handle.DangerousGetHandle(), (int)num, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
				if (flag)
				{
					this.safe_wait_handle.DangerousRelease();
				}
			}
			return flag2;
		}

		internal void CheckDisposed()
		{
			if (this.disposed || this.safe_wait_handle == null)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout)
		{
			return WaitHandle.WaitAll(waitHandles, millisecondsTimeout, false);
		}

		public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout)
		{
			return WaitHandle.WaitAll(waitHandles, timeout, false);
		}

		~WaitHandle()
		{
			this.Dispose(false);
		}

		public const int WaitTimeout = 258;

		private SafeWaitHandle safe_wait_handle;

		protected static readonly IntPtr InvalidHandle = (IntPtr)(-1);

		private bool disposed;
	}
}
