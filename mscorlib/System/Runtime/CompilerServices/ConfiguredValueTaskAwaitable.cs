using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
	[StructLayout(LayoutKind.Auto)]
	public struct ConfiguredValueTaskAwaitable<TResult>
	{
		internal ConfiguredValueTaskAwaitable(ValueTask<TResult> value, bool continueOnCapturedContext)
		{
			this._value = value;
			this._continueOnCapturedContext = continueOnCapturedContext;
		}

		public ConfiguredValueTaskAwaitable<TResult>.ConfiguredValueTaskAwaiter GetAwaiter()
		{
			return new ConfiguredValueTaskAwaitable<TResult>.ConfiguredValueTaskAwaiter(this._value, this._continueOnCapturedContext);
		}

		private readonly ValueTask<TResult> _value;

		private readonly bool _continueOnCapturedContext;

		[StructLayout(LayoutKind.Auto)]
		public struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			internal ConfiguredValueTaskAwaiter(ValueTask<TResult> value, bool continueOnCapturedContext)
			{
				this._value = value;
				this._continueOnCapturedContext = continueOnCapturedContext;
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
				this._value.AsTask().ConfigureAwait(this._continueOnCapturedContext).GetAwaiter()
					.OnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				this._value.AsTask().ConfigureAwait(this._continueOnCapturedContext).GetAwaiter()
					.UnsafeOnCompleted(continuation);
			}

			private readonly ValueTask<TResult> _value;

			private readonly bool _continueOnCapturedContext;
		}
	}
}
