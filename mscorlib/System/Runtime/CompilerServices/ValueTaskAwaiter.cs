using System;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
	public struct ValueTaskAwaiter<TResult> : ICriticalNotifyCompletion, INotifyCompletion
	{
		internal ValueTaskAwaiter(ValueTask<TResult> value)
		{
			this._value = value;
		}

		public bool IsCompleted
		{
			get
			{
				return this._value.IsCompleted;
			}
		}

		public TResult GetResult()
		{
			if (this._value._task != null)
			{
				return this._value._task.GetAwaiter().GetResult();
			}
			return this._value._result;
		}

		public void OnCompleted(Action continuation)
		{
			this._value.AsTask().ConfigureAwait(true).GetAwaiter()
				.OnCompleted(continuation);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			this._value.AsTask().ConfigureAwait(true).GetAwaiter()
				.UnsafeOnCompleted(continuation);
		}

		private readonly ValueTask<TResult> _value;
	}
}
