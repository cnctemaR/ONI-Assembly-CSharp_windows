using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Threading
{
	[ComVisible(false)]
	public sealed class Semaphore : WaitHandle
	{
		private Semaphore(IntPtr handle)
		{
			this.Handle = handle;
		}

		public Semaphore(int initialCount, int maximumCount)
			: this(initialCount, maximumCount, null)
		{
		}

		public Semaphore(int initialCount, int maximumCount, string name)
		{
			if (initialCount < 0)
			{
				throw new ArgumentOutOfRangeException("initialCount", "< 0");
			}
			if (maximumCount < 1)
			{
				throw new ArgumentOutOfRangeException("maximumCount", "< 1");
			}
			if (initialCount > maximumCount)
			{
				throw new ArgumentException("initialCount > maximumCount");
			}
			bool flag;
			this.Handle = Semaphore.CreateSemaphore_internal(initialCount, maximumCount, name, out flag);
		}

		public Semaphore(int initialCount, int maximumCount, string name, out bool createdNew)
			: this(initialCount, maximumCount, name, out createdNew, null)
		{
		}

		[global::System.MonoTODO("Does not support access control, semaphoreSecurity is ignored")]
		public Semaphore(int initialCount, int maximumCount, string name, out bool createdNew, global::System.Security.AccessControl.SemaphoreSecurity semaphoreSecurity)
		{
			if (initialCount < 0)
			{
				throw new ArgumentOutOfRangeException("initialCount", "< 0");
			}
			if (maximumCount < 1)
			{
				throw new ArgumentOutOfRangeException("maximumCount", "< 1");
			}
			if (initialCount > maximumCount)
			{
				throw new ArgumentException("initialCount > maximumCount");
			}
			this.Handle = Semaphore.CreateSemaphore_internal(initialCount, maximumCount, name, out createdNew);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateSemaphore_internal(int initialCount, int maximumCount, string name, out bool createdNew);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ReleaseSemaphore_internal(IntPtr handle, int releaseCount, out bool fail);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr OpenSemaphore_internal(string name, global::System.Security.AccessControl.SemaphoreRights rights, out global::System.IO.MonoIOError error);

		[global::System.MonoTODO]
		public global::System.Security.AccessControl.SemaphoreSecurity GetAccessControl()
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[PrePrepareMethod]
		public int Release()
		{
			return this.Release(1);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public int Release(int releaseCount)
		{
			if (releaseCount < 1)
			{
				throw new ArgumentOutOfRangeException("releaseCount");
			}
			bool flag;
			int num = Semaphore.ReleaseSemaphore_internal(this.Handle, releaseCount, out flag);
			if (flag)
			{
				throw new SemaphoreFullException();
			}
			return num;
		}

		[global::System.MonoTODO]
		public void SetAccessControl(global::System.Security.AccessControl.SemaphoreSecurity semaphoreSecurity)
		{
			if (semaphoreSecurity == null)
			{
				throw new ArgumentNullException("semaphoreSecurity");
			}
			throw new NotImplementedException();
		}

		public static Semaphore OpenExisting(string name)
		{
			return Semaphore.OpenExisting(name, global::System.Security.AccessControl.SemaphoreRights.Modify | global::System.Security.AccessControl.SemaphoreRights.Synchronize);
		}

		public static Semaphore OpenExisting(string name, global::System.Security.AccessControl.SemaphoreRights rights)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0 || name.Length > 260)
			{
				throw new ArgumentException("name", global::Locale.GetText("Invalid length [1-260]."));
			}
			global::System.IO.MonoIOError monoIOError;
			IntPtr intPtr = Semaphore.OpenSemaphore_internal(name, rights, out monoIOError);
			if (!(intPtr == (IntPtr)null))
			{
				return new Semaphore(intPtr);
			}
			if (monoIOError == global::System.IO.MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				throw new WaitHandleCannotBeOpenedException(global::Locale.GetText("Named Semaphore handle does not exist: ") + name);
			}
			if (monoIOError == global::System.IO.MonoIOError.ERROR_ACCESS_DENIED)
			{
				throw new UnauthorizedAccessException();
			}
			throw new IOException(global::Locale.GetText("Win32 IO error: ") + monoIOError.ToString());
		}
	}
}
