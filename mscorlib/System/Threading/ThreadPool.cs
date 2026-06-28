using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Threading
{
	public static class ThreadPool
	{
		[Obsolete("This method is obsolete, use BindHandle(SafeHandle) instead")]
		public static bool BindHandle(IntPtr osHandle)
		{
			return true;
		}

		public static bool BindHandle(SafeHandle osHandle)
		{
			if (osHandle == null)
			{
				throw new ArgumentNullException("osHandle");
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetAvailableThreads(out int workerThreads, out int completionPortThreads);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetMaxThreads(out int workerThreads, out int completionPortThreads);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetMinThreads(out int workerThreads, out int completionPortThreads);

		[MonoTODO("The min number of completion port threads is not evaluated.")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetMinThreads(int workerThreads, int completionPortThreads);

		[MonoTODO("The max number of threads cannot be decremented.")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlThread\"/>\n</PermissionSet>\n")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetMaxThreads(int workerThreads, int completionPortThreads);

		public static bool QueueUserWorkItem(WaitCallback callBack)
		{
			return ThreadPool.QueueUserWorkItem(callBack, null);
		}

		public static bool QueueUserWorkItem(WaitCallback callBack, object state)
		{
			if (callBack == null)
			{
				throw new ArgumentNullException("callBack");
			}
			return callBack.BeginInvoke(state, null, null) != null;
		}

		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, int millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			return ThreadPool.RegisterWaitForSingleObject(waitObject, callBack, state, (long)millisecondsTimeOutInterval, executeOnlyOnce);
		}

		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, long millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			if (millisecondsTimeOutInterval < -1L)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout < -1");
			}
			if (millisecondsTimeOutInterval > 2147483647L)
			{
				throw new NotSupportedException("Timeout is too big. Maximum is Int32.MaxValue");
			}
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int)millisecondsTimeOutInterval);
			RegisteredWaitHandle registeredWaitHandle = new RegisteredWaitHandle(waitObject, callBack, state, timeSpan, executeOnlyOnce);
			ThreadPool.QueueUserWorkItem(new WaitCallback(registeredWaitHandle.Wait), null);
			return registeredWaitHandle;
		}

		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, TimeSpan timeout, bool executeOnlyOnce)
		{
			return ThreadPool.RegisterWaitForSingleObject(waitObject, callBack, state, (long)timeout.TotalMilliseconds, executeOnlyOnce);
		}

		[CLSCompliant(false)]
		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, uint millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			return ThreadPool.RegisterWaitForSingleObject(waitObject, callBack, state, (long)((ulong)millisecondsTimeOutInterval), executeOnlyOnce);
		}

		[CLSCompliant(false)]
		public unsafe static bool UnsafeQueueNativeOverlapped(NativeOverlapped* overlapped)
		{
			throw new NotImplementedException();
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
		public static bool UnsafeQueueUserWorkItem(WaitCallback callBack, object state)
		{
			IAsyncResult asyncResult = null;
			try
			{
				if (!ExecutionContext.IsFlowSuppressed())
				{
					ExecutionContext.SuppressFlow();
				}
				asyncResult = callBack.BeginInvoke(state, null, null);
			}
			finally
			{
				if (ExecutionContext.IsFlowSuppressed())
				{
					ExecutionContext.RestoreFlow();
				}
			}
			return asyncResult != null;
		}

		[MonoTODO("Not implemented")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
		public static RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, int millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
		public static RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, long millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
		public static RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, TimeSpan timeout, bool executeOnlyOnce)
		{
			throw new NotImplementedException();
		}

		[CLSCompliant(false)]
		[MonoTODO("Not implemented")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
		public static RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, uint millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			throw new NotImplementedException();
		}
	}
}
