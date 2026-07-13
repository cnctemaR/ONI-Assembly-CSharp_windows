using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;
using UnityEngine.Pool;

namespace UnityEngine
{
	[AsyncMethodBuilder(typeof(Awaitable.AwaitableAsyncMethodBuilder<>))]
	public class Awaitable<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ContinueWith(Action continuation)
		{
			this._awaitable.SetContinuation(continuation);
		}

		private T GetResult()
		{
			T result;
			try
			{
				this._awaitable.PropagateExceptionAndRelease();
				result = this._result;
			}
			finally
			{
				this._awaitable = null;
				this._result = default(T);
				Awaitable<T>._pool.Value.Release(this);
			}
			return result;
		}

		internal void SetResultAndRaiseContinuation(T result)
		{
			this._result = result;
			this._awaitable.RaiseManagedCompletion();
		}

		internal void SetExceptionAndRaiseContinuation(Exception exception)
		{
			this._awaitable.RaiseManagedCompletion(exception);
		}

		internal Awaitable.AwaiterCompletionThreadAffinity CompletionThreadAffinity
		{
			get
			{
				return this._awaitable.CompletionThreadAffinity;
			}
			set
			{
				this._awaitable.CompletionThreadAffinity = value;
			}
		}

		public void Cancel()
		{
			this._awaitable.Cancel();
		}

		private Awaitable()
		{
		}

		internal static Awaitable<T> GetManaged()
		{
			Awaitable awaitable = Awaitable.NewManagedAwaitable();
			Awaitable<T> awaitable2 = Awaitable<T>._pool.Value.Get();
			awaitable2._awaitable = awaitable;
			return awaitable2;
		}

		[ExcludeFromDocs]
		public Awaitable<T>.Awaiter GetAwaiter()
		{
			return new Awaitable<T>.Awaiter(this);
		}

		private static readonly ThreadLocal<ObjectPool<Awaitable<T>>> _pool = new ThreadLocal<ObjectPool<Awaitable<T>>>(() => new ObjectPool<Awaitable<T>>(() => new Awaitable<T>(), null, null, null, false, 10, 10000));

		private Awaitable _awaitable;

		private T _result;

		[ExcludeFromDocs]
		public struct Awaiter : INotifyCompletion
		{
			public Awaiter(Awaitable<T> coroutine)
			{
				this._coroutine = coroutine;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void OnCompleted(Action continuation)
			{
				this._coroutine.ContinueWith(continuation);
			}

			public bool IsCompleted
			{
				get
				{
					return this._coroutine._awaitable.IsCompleted;
				}
			}

			public T GetResult()
			{
				return this._coroutine.GetResult();
			}

			private readonly Awaitable<T> _coroutine;
		}
	}
}
