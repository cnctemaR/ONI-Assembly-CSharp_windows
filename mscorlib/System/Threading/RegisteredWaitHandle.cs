using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public sealed class RegisteredWaitHandle : MarshalByRefObject
	{
		internal RegisteredWaitHandle(WaitHandle waitObject, WaitOrTimerCallback callback, object state, TimeSpan timeout, bool executeOnlyOnce)
		{
			this._waitObject = waitObject;
			this._callback = callback;
			this._state = state;
			this._timeout = timeout;
			this._executeOnlyOnce = executeOnlyOnce;
			this._finalEvent = null;
			this._cancelEvent = new ManualResetEvent(false);
			this._callsInProcess = 0;
			this._unregistered = false;
		}

		internal void Wait(object state)
		{
			try
			{
				WaitHandle[] array = new WaitHandle[] { this._waitObject, this._cancelEvent };
				do
				{
					int num = WaitHandle.WaitAny(array, this._timeout, false);
					if (!this._unregistered)
					{
						lock (this)
						{
							this._callsInProcess++;
						}
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.DoCallBack), num == 258);
					}
				}
				while (!this._unregistered && !this._executeOnlyOnce);
			}
			catch
			{
			}
			lock (this)
			{
				this._unregistered = true;
				if (this._callsInProcess == 0 && this._finalEvent != null)
				{
					NativeEventCalls.SetEvent_internal(this._finalEvent.Handle);
				}
			}
		}

		private void DoCallBack(object timedOut)
		{
			if (this._callback != null)
			{
				this._callback(this._state, (bool)timedOut);
			}
			lock (this)
			{
				this._callsInProcess--;
				if (this._unregistered && this._callsInProcess == 0 && this._finalEvent != null)
				{
					NativeEventCalls.SetEvent_internal(this._finalEvent.Handle);
				}
			}
		}

		[ComVisible(true)]
		public bool Unregister(WaitHandle waitObject)
		{
			bool flag;
			lock (this)
			{
				if (this._unregistered)
				{
					flag = false;
				}
				else
				{
					this._finalEvent = waitObject;
					this._unregistered = true;
					this._cancelEvent.Set();
					flag = true;
				}
			}
			return flag;
		}

		private WaitHandle _waitObject;

		private WaitOrTimerCallback _callback;

		private TimeSpan _timeout;

		private object _state;

		private bool _executeOnlyOnce;

		private WaitHandle _finalEvent;

		private ManualResetEvent _cancelEvent;

		private int _callsInProcess;

		private bool _unregistered;
	}
}
