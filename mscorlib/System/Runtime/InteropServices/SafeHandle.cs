using System;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Runtime.InteropServices
{
	[SecurityCritical]
	[SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class SafeHandle : CriticalFinalizerObject, IDisposable
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected SafeHandle(IntPtr invalidHandleValue, bool ownsHandle)
		{
			this.handle = invalidHandleValue;
			this._state = 4;
			this._ownsHandle = ownsHandle;
			if (!ownsHandle)
			{
				GC.SuppressFinalize(this);
			}
			this._fullyInitialized = true;
		}

		[SecuritySafeCritical]
		~SafeHandle()
		{
			this.Dispose(false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected void SetHandle(IntPtr handle)
		{
			this.handle = handle;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public IntPtr DangerousGetHandle()
		{
			return this.handle;
		}

		public bool IsClosed
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return (this._state & 1) == 1;
			}
		}

		public abstract bool IsInvalid
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get;
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Close()
		{
			this.Dispose(true);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecuritySafeCritical]
		public void Dispose()
		{
			this.Dispose(true);
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.InternalDispose();
				return;
			}
			this.InternalFinalize();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected abstract bool ReleaseHandle();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void SetHandleAsInvalid()
		{
			try
			{
			}
			finally
			{
				int state;
				int num;
				do
				{
					state = this._state;
					num = state | 1;
				}
				while (Interlocked.CompareExchange(ref this._state, num, state) != state);
				GC.SuppressFinalize(this);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void DangerousAddRef(ref bool success)
		{
			try
			{
			}
			finally
			{
				if (!this._fullyInitialized)
				{
					throw new InvalidOperationException();
				}
				for (;;)
				{
					int state = this._state;
					if ((state & 1) != 0)
					{
						break;
					}
					int num = state + 4;
					if (Interlocked.CompareExchange(ref this._state, num, state) == state)
					{
						goto Block_5;
					}
				}
				throw new ObjectDisposedException(null, "Safe handle has been closed");
				Block_5:
				success = true;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void DangerousRelease()
		{
			this.DangerousReleaseInternal(false);
		}

		private void InternalDispose()
		{
			if (!this._fullyInitialized)
			{
				throw new InvalidOperationException();
			}
			this.DangerousReleaseInternal(true);
			GC.SuppressFinalize(this);
		}

		private void InternalFinalize()
		{
			if (this._fullyInitialized)
			{
				this.DangerousReleaseInternal(true);
			}
		}

		private void DangerousReleaseInternal(bool dispose)
		{
			try
			{
			}
			finally
			{
				if (!this._fullyInitialized)
				{
					throw new InvalidOperationException();
				}
				bool flag;
				for (;;)
				{
					int state = this._state;
					if (dispose && (state & 2) != 0)
					{
						break;
					}
					if ((state & 2147483644) == 0)
					{
						goto Block_6;
					}
					flag = (state & 2147483644) == 4 && (state & 1) == 0 && this._ownsHandle && !this.IsInvalid;
					int num = (state & 2147483644) - 4;
					if ((state & 2147483644) == 4)
					{
						num |= 1;
					}
					if (dispose)
					{
						num |= 2;
					}
					if (Interlocked.CompareExchange(ref this._state, num, state) == state)
					{
						goto IL_00A0;
					}
				}
				flag = false;
				goto IL_00A0;
				Block_6:
				throw new ObjectDisposedException(null, "Safe handle has been closed");
				IL_00A0:
				if (flag)
				{
					this.ReleaseHandle();
				}
			}
		}

		protected IntPtr handle;

		private int _state;

		private bool _ownsHandle;

		private bool _fullyInitialized;

		private const int RefCount_Mask = 2147483644;

		private const int RefCount_One = 4;

		private enum State
		{
			Closed = 1,
			Disposed
		}
	}
}
