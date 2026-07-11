using System;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
	[ComVisible(false)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public sealed class Semaphore : WaitHandle
	{
		[SecuritySafeCritical]
		public Semaphore(int initialCount, int maximumCount)
			: this(initialCount, maximumCount, null)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Semaphore(int initialCount, int maximumCount, string name)
		{
			if (initialCount < 0)
			{
				throw new ArgumentOutOfRangeException("initialCount", global::SR.GetString("Non-negative number required."));
			}
			if (maximumCount < 1)
			{
				throw new ArgumentOutOfRangeException("maximumCount", global::SR.GetString("Positive number required."));
			}
			if (initialCount > maximumCount)
			{
				throw new ArgumentException(global::SR.GetString("The initial count for the semaphore must be greater than or equal to zero and less than the maximum count."));
			}
			if (name != null && 260 < name.Length)
			{
				throw new ArgumentException(global::SR.GetString("The name can be no more than 260 characters in length."));
			}
			int num;
			SafeWaitHandle safeWaitHandle = new SafeWaitHandle(Semaphore.CreateSemaphore_internal(initialCount, maximumCount, name, out num), true);
			if (safeWaitHandle.IsInvalid)
			{
				if (name != null && name.Length != 0 && 6 == num)
				{
					throw new WaitHandleCannotBeOpenedException(global::SR.GetString("A WaitHandle with system-wide name '{0}' cannot be created. A WaitHandle of a different type might have the same name.", new object[] { name }));
				}
				InternalResources.WinIOError(num, "");
			}
			base.SafeWaitHandle = safeWaitHandle;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Semaphore(int initialCount, int maximumCount, string name, out bool createdNew)
			: this(initialCount, maximumCount, name, out createdNew, null)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Semaphore(int initialCount, int maximumCount, string name, out bool createdNew, SemaphoreSecurity semaphoreSecurity)
		{
			if (initialCount < 0)
			{
				throw new ArgumentOutOfRangeException("initialCount", global::SR.GetString("Non-negative number required."));
			}
			if (maximumCount < 1)
			{
				throw new ArgumentOutOfRangeException("maximumCount", global::SR.GetString("Non-negative number required."));
			}
			if (initialCount > maximumCount)
			{
				throw new ArgumentException(global::SR.GetString("The initial count for the semaphore must be greater than or equal to zero and less than the maximum count."));
			}
			if (name != null && 260 < name.Length)
			{
				throw new ArgumentException(global::SR.GetString("The name can be no more than 260 characters in length."));
			}
			int num;
			SafeWaitHandle safeWaitHandle = new SafeWaitHandle(Semaphore.CreateSemaphore_internal(initialCount, maximumCount, name, out num), true);
			if (safeWaitHandle.IsInvalid)
			{
				if (name != null && name.Length != 0 && 6 == num)
				{
					throw new WaitHandleCannotBeOpenedException(global::SR.GetString("A WaitHandle with system-wide name '{0}' cannot be created. A WaitHandle of a different type might have the same name.", new object[] { name }));
				}
				InternalResources.WinIOError(num, "");
			}
			createdNew = num != 183;
			base.SafeWaitHandle = safeWaitHandle;
		}

		private Semaphore(SafeWaitHandle handle)
		{
			base.SafeWaitHandle = handle;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static Semaphore OpenExisting(string name)
		{
			return Semaphore.OpenExisting(name, SemaphoreRights.Modify | SemaphoreRights.Synchronize);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static Semaphore OpenExisting(string name, SemaphoreRights rights)
		{
			Semaphore semaphore;
			switch (Semaphore.OpenExistingWorker(name, rights, out semaphore))
			{
			case Semaphore.OpenExistingResult.NameNotFound:
				throw new WaitHandleCannotBeOpenedException();
			case Semaphore.OpenExistingResult.PathNotFound:
				InternalResources.WinIOError(3, string.Empty);
				return semaphore;
			case Semaphore.OpenExistingResult.NameInvalid:
				throw new WaitHandleCannotBeOpenedException(global::SR.GetString("A WaitHandle with system-wide name '{0}' cannot be created. A WaitHandle of a different type might have the same name.", new object[] { name }));
			default:
				return semaphore;
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static bool TryOpenExisting(string name, out Semaphore result)
		{
			return Semaphore.OpenExistingWorker(name, SemaphoreRights.Modify | SemaphoreRights.Synchronize, out result) == Semaphore.OpenExistingResult.Success;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static bool TryOpenExisting(string name, SemaphoreRights rights, out Semaphore result)
		{
			return Semaphore.OpenExistingWorker(name, rights, out result) == Semaphore.OpenExistingResult.Success;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		private static Semaphore.OpenExistingResult OpenExistingWorker(string name, SemaphoreRights rights, out Semaphore result)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(global::SR.GetString("Argument {0} cannot be null or zero-length.", new object[] { "name" }), "name");
			}
			if (name != null && 260 < name.Length)
			{
				throw new ArgumentException(global::SR.GetString("The name can be no more than 260 characters in length."));
			}
			result = null;
			int num;
			SafeWaitHandle safeWaitHandle = new SafeWaitHandle(Semaphore.OpenSemaphore_internal(name, rights, out num), true);
			if (safeWaitHandle.IsInvalid)
			{
				if (2 == num || 123 == num)
				{
					return Semaphore.OpenExistingResult.NameNotFound;
				}
				if (3 == num)
				{
					return Semaphore.OpenExistingResult.PathNotFound;
				}
				if (name != null && name.Length != 0 && 6 == num)
				{
					return Semaphore.OpenExistingResult.NameInvalid;
				}
				InternalResources.WinIOError(num, "");
			}
			result = new Semaphore(safeWaitHandle);
			return Semaphore.OpenExistingResult.Success;
		}

		[PrePrepareMethod]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public int Release()
		{
			return this.Release(1);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public int Release(int releaseCount)
		{
			if (releaseCount < 1)
			{
				throw new ArgumentOutOfRangeException("releaseCount", global::SR.GetString("Non-negative number required."));
			}
			int num;
			if (!Semaphore.ReleaseSemaphore_internal(base.SafeWaitHandle.DangerousGetHandle(), releaseCount, out num))
			{
				throw new SemaphoreFullException();
			}
			return num;
		}

		public SemaphoreSecurity GetAccessControl()
		{
			return new SemaphoreSecurity(base.SafeWaitHandle, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public void SetAccessControl(SemaphoreSecurity semaphoreSecurity)
		{
			if (semaphoreSecurity == null)
			{
				throw new ArgumentNullException("semaphoreSecurity");
			}
			semaphoreSecurity.Persist(base.SafeWaitHandle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr CreateSemaphore_internal(int initialCount, int maximumCount, string name, out int errorCode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ReleaseSemaphore_internal(IntPtr handle, int releaseCount, out int previousCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr OpenSemaphore_internal(string name, SemaphoreRights rights, out int errorCode);

		private const int MAX_PATH = 260;

		private new enum OpenExistingResult
		{
			Success,
			NameNotFound,
			PathNotFound,
			NameInvalid
		}
	}
}
