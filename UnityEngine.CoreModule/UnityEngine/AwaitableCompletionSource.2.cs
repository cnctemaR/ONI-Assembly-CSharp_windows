using System;
using System.Threading;

namespace UnityEngine
{
	public class AwaitableCompletionSource<T>
	{
		public Awaitable<T> Awaitable { get; private set; } = Awaitable<T>.GetManaged();

		public void SetResult(in T value)
		{
			bool flag = !this.TrySetResult(in value);
			if (flag)
			{
				throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
			}
		}

		public void SetCanceled()
		{
			bool flag = !this.TrySetCanceled();
			if (flag)
			{
				throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
			}
		}

		public void SetException(Exception exception)
		{
			bool flag = !this.TrySetException(exception);
			if (flag)
			{
				throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
			}
		}

		private bool CheckAndAcquireCompletionState()
		{
			return Interlocked.CompareExchange(ref this._state, 1, 0) == 0;
		}

		public bool TrySetResult(in T value)
		{
			bool flag = !this.CheckAndAcquireCompletionState();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.Awaitable.SetResultAndRaiseContinuation(value);
				flag2 = true;
			}
			return flag2;
		}

		public bool TrySetCanceled()
		{
			bool flag = !this.CheckAndAcquireCompletionState();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.Awaitable.Cancel();
				flag2 = true;
			}
			return flag2;
		}

		public bool TrySetException(Exception exception)
		{
			bool flag = !this.CheckAndAcquireCompletionState();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.Awaitable.SetExceptionAndRaiseContinuation(exception);
				flag2 = true;
			}
			return flag2;
		}

		public void Reset()
		{
			this.Awaitable = Awaitable<T>.GetManaged();
			this._state = 0;
		}

		private volatile int _state;
	}
}
