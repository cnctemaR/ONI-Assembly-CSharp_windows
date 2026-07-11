using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Threading
{
	[ComVisible(true)]
	public class EventWaitHandle : WaitHandle
	{
		private EventWaitHandle(IntPtr handle)
		{
			this.Handle = handle;
		}

		public EventWaitHandle(bool initialState, EventResetMode mode)
		{
			bool flag = this.IsManualReset(mode);
			bool flag2;
			this.Handle = NativeEventCalls.CreateEvent_internal(flag, initialState, null, out flag2);
		}

		public EventWaitHandle(bool initialState, EventResetMode mode, string name)
		{
			bool flag = this.IsManualReset(mode);
			bool flag2;
			this.Handle = NativeEventCalls.CreateEvent_internal(flag, initialState, name, out flag2);
		}

		public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew)
		{
			bool flag = this.IsManualReset(mode);
			this.Handle = NativeEventCalls.CreateEvent_internal(flag, initialState, name, out createdNew);
		}

		[MonoTODO("Implement access control")]
		public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew, EventWaitHandleSecurity eventSecurity)
		{
			bool flag = this.IsManualReset(mode);
			this.Handle = NativeEventCalls.CreateEvent_internal(flag, initialState, name, out createdNew);
		}

		private bool IsManualReset(EventResetMode mode)
		{
			if (mode < EventResetMode.AutoReset || mode > EventResetMode.ManualReset)
			{
				throw new ArgumentException("mode");
			}
			return mode == EventResetMode.ManualReset;
		}

		[MonoTODO]
		public EventWaitHandleSecurity GetAccessControl()
		{
			throw new NotImplementedException();
		}

		public static EventWaitHandle OpenExisting(string name)
		{
			return EventWaitHandle.OpenExisting(name, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize);
		}

		public static EventWaitHandle OpenExisting(string name, EventWaitHandleRights rights)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0 || name.Length > 260)
			{
				throw new ArgumentException("name", Locale.GetText("Invalid length [1-260]."));
			}
			MonoIOError monoIOError;
			IntPtr intPtr = NativeEventCalls.OpenEvent_internal(name, rights, out monoIOError);
			if (!(intPtr == (IntPtr)null))
			{
				return new EventWaitHandle(intPtr);
			}
			if (monoIOError == MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				throw new WaitHandleCannotBeOpenedException(Locale.GetText("Named Event handle does not exist: ") + name);
			}
			if (monoIOError == MonoIOError.ERROR_ACCESS_DENIED)
			{
				throw new UnauthorizedAccessException();
			}
			throw new IOException(Locale.GetText("Win32 IO error: ") + monoIOError.ToString());
		}

		public bool Reset()
		{
			base.CheckDisposed();
			return NativeEventCalls.ResetEvent_internal(this.Handle);
		}

		public bool Set()
		{
			base.CheckDisposed();
			return NativeEventCalls.SetEvent_internal(this.Handle);
		}

		[MonoTODO]
		public void SetAccessControl(EventWaitHandleSecurity eventSecurity)
		{
			throw new NotImplementedException();
		}
	}
}
