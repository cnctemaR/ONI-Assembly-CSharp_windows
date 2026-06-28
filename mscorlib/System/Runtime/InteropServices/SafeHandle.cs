using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace System.Runtime.InteropServices
{
	public abstract class SafeHandle : CriticalFinalizerObject, IDisposable
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected SafeHandle(IntPtr invalidHandleValue, bool ownsHandle)
		{
			this.invalid_handle_value = invalidHandleValue;
			this.owns_handle = ownsHandle;
			this.refcount = 1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Close()
		{
			if (this.refcount == 0)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			int num;
			int num2;
			do
			{
				num = this.refcount;
				num2 = num - 1;
			}
			while (Interlocked.CompareExchange(ref this.refcount, num2, num) != num);
			if (num2 == 0 && this.owns_handle && !this.IsInvalid)
			{
				this.ReleaseHandle();
				this.handle = this.invalid_handle_value;
				this.refcount = -1;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void DangerousAddRef(ref bool success)
		{
			if (this.refcount <= 0)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			for (;;)
			{
				int num = this.refcount;
				int num2 = num + 1;
				if (num <= 0)
				{
					break;
				}
				if (Interlocked.CompareExchange(ref this.refcount, num2, num) == num)
				{
					goto Block_3;
				}
			}
			throw new ObjectDisposedException(base.GetType().FullName);
			Block_3:
			success = true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public IntPtr DangerousGetHandle()
		{
			if (this.refcount <= 0)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			return this.handle;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void DangerousRelease()
		{
			if (this.refcount <= 0)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			int num;
			int num2;
			do
			{
				num = this.refcount;
				num2 = num - 1;
			}
			while (Interlocked.CompareExchange(ref this.refcount, num2, num) != num);
			if (num2 == 0 && this.owns_handle && !this.IsInvalid)
			{
				this.ReleaseHandle();
				this.handle = this.invalid_handle_value;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void SetHandleAsInvalid()
		{
			this.handle = this.invalid_handle_value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected abstract bool ReleaseHandle();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected void SetHandle(IntPtr handle)
		{
			this.handle = handle;
		}

		public bool IsClosed
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.refcount <= 0;
			}
		}

		public abstract bool IsInvalid
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get;
		}

		~SafeHandle()
		{
			if (this.owns_handle && !this.IsInvalid)
			{
				this.ReleaseHandle();
				this.handle = this.invalid_handle_value;
			}
		}

		protected IntPtr handle;

		private IntPtr invalid_handle_value;

		private int refcount;

		private bool owns_handle;
	}
}
