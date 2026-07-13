using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class AsyncInstantiateOperation<T> : AsyncInstantiateOperation
	{
		internal AsyncInstantiateOperation(IntPtr ptr, CancellationToken cancellationToken)
			: base(ptr, cancellationToken)
		{
		}

		public new T[] Result
		{
			get
			{
				return (T[])this.m_Result;
			}
		}

		internal override Object[] CreateResultArray(int size)
		{
			this.m_Result = (Object[])new T[size];
			return this.m_Result;
		}

		[ExcludeFromDocs]
		public AsyncInstantiateOperation<T>.Awaiter GetAwaiter()
		{
			return new AsyncInstantiateOperation<T>.Awaiter(this);
		}

		internal new static class BindingsMarshaller
		{
			public static AsyncInstantiateOperation<T> ConvertToManaged(IntPtr ptr)
			{
				return new AsyncInstantiateOperation<T>(ptr, CancellationToken.None);
			}

			public static IntPtr ConvertToNative(AsyncInstantiateOperation<T> obj)
			{
				return obj.m_Ptr;
			}
		}

		[ExcludeFromDocs]
		public struct Awaiter : INotifyCompletion
		{
			public Awaiter(AsyncInstantiateOperation<T> op)
			{
				this._awaitable = Awaitable.FromAsyncOperation(op, default(CancellationToken));
				this._op = op;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void OnCompleted(Action continuation)
			{
				this._awaitable.SetContinuation(continuation);
			}

			public bool IsCompleted
			{
				get
				{
					return this._awaitable.IsCompleted;
				}
			}

			public T[] GetResult()
			{
				this._awaitable.GetAwaiter().GetResult();
				return this._op.Result;
			}

			private readonly Awaitable _awaitable;

			private readonly AsyncInstantiateOperation<T> _op;
		}
	}
}
