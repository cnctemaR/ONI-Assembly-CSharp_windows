using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Threading.Tasks
{
	[AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder<>))]
	[StructLayout(LayoutKind.Auto)]
	public struct ValueTask<TResult> : IEquatable<ValueTask<TResult>>
	{
		public ValueTask(TResult result)
		{
			this._task = null;
			this._result = result;
		}

		public ValueTask(Task<TResult> task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			this._task = task;
			this._result = default(TResult);
		}

		public override int GetHashCode()
		{
			if (this._task != null)
			{
				return this._task.GetHashCode();
			}
			if (this._result == null)
			{
				return 0;
			}
			TResult result = this._result;
			return result.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is ValueTask<TResult> && this.Equals((ValueTask<TResult>)obj);
		}

		public bool Equals(ValueTask<TResult> other)
		{
			if (this._task == null && other._task == null)
			{
				return EqualityComparer<TResult>.Default.Equals(this._result, other._result);
			}
			return this._task == other._task;
		}

		public static bool operator ==(ValueTask<TResult> left, ValueTask<TResult> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ValueTask<TResult> left, ValueTask<TResult> right)
		{
			return !left.Equals(right);
		}

		public Task<TResult> AsTask()
		{
			return this._task ?? Task.FromResult<TResult>(this._result);
		}

		public bool IsCompleted
		{
			get
			{
				return this._task == null || this._task.IsCompleted;
			}
		}

		public bool IsCompletedSuccessfully
		{
			get
			{
				return this._task == null || this._task.Status == TaskStatus.RanToCompletion;
			}
		}

		public bool IsFaulted
		{
			get
			{
				return this._task != null && this._task.IsFaulted;
			}
		}

		public bool IsCanceled
		{
			get
			{
				return this._task != null && this._task.IsCanceled;
			}
		}

		public TResult Result
		{
			get
			{
				if (this._task != null)
				{
					return this._task.GetAwaiter().GetResult();
				}
				return this._result;
			}
		}

		public ValueTaskAwaiter<TResult> GetAwaiter()
		{
			return new ValueTaskAwaiter<TResult>(this);
		}

		public ConfiguredValueTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
		{
			return new ConfiguredValueTaskAwaitable<TResult>(this, continueOnCapturedContext);
		}

		public override string ToString()
		{
			if (this._task != null)
			{
				if (this._task.Status != TaskStatus.RanToCompletion || this._task.Result == null)
				{
					return string.Empty;
				}
				TResult tresult = this._task.Result;
				return tresult.ToString();
			}
			else
			{
				if (this._result == null)
				{
					return string.Empty;
				}
				TResult tresult = this._result;
				return tresult.ToString();
			}
		}

		public static AsyncValueTaskMethodBuilder<TResult> CreateAsyncMethodBuilder()
		{
			return AsyncValueTaskMethodBuilder<TResult>.Create();
		}

		internal readonly Task<TResult> _task;

		internal readonly TResult _result;
	}
}
