using System;
using System.Runtime.InteropServices;
using Unity;

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
			bool flag = false;
			try
			{
				this._waitObject.SafeWaitHandle.DangerousAddRef(ref flag);
				RegisteredWaitHandle registeredWaitHandle;
				try
				{
					WaitHandle[] array = new WaitHandle[] { this._waitObject, this._cancelEvent };
					do
					{
						int num = WaitHandle.WaitAny(array, this._timeout, false);
						if (!this._unregistered)
						{
							registeredWaitHandle = this;
							lock (registeredWaitHandle)
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
				registeredWaitHandle = this;
				lock (registeredWaitHandle)
				{
					this._unregistered = true;
					if (this._callsInProcess == 0 && this._finalEvent != null)
					{
						NativeEventCalls.SetEvent(this._finalEvent.SafeWaitHandle);
						this._finalEvent = null;
					}
				}
			}
			catch (ObjectDisposedException)
			{
				if (flag)
				{
					throw;
				}
			}
			finally
			{
				if (flag)
				{
					this._waitObject.SafeWaitHandle.DangerousRelease();
				}
			}
		}

		private void DoCallBack(object timedOut)
		{
			try
			{
				if (this._callback != null)
				{
					this._callback(this._state, (bool)timedOut);
				}
			}
			finally
			{
				lock (this)
				{
					this._callsInProcess--;
					if (this._unregistered && this._callsInProcess == 0 && this._finalEvent != null)
					{
						NativeEventCalls.SetEvent(this._finalEvent.SafeWaitHandle);
						this._finalEvent = null;
					}
				}
			}
		}

		[ComVisible(true)]
		public bool Unregister(WaitHandle waitObject)
		{
			bool flag2;
			lock (this)
			{
				if (this._unregistered)
				{
					flag2 = false;
				}
				else
				{
					this._finalEvent = waitObject;
					this._unregistered = true;
					this._cancelEvent.Set();
					flag2 = true;
				}
			}
			return flag2;
		}

		internal RegisteredWaitHandle()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private WaitHandle _waitObject;

		private WaitOrTimerCallback _callback;

		private object _state;

		private WaitHandle _finalEvent;

		private ManualResetEvent _cancelEvent;

		private TimeSpan _timeout;

		private int _callsInProcess;

		private bool _executeOnlyOnce;

		private bool _unregistered;
	}
}
