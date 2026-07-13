using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	public struct BackgroundThreadAwaitable : INotifyCompletion
	{
		internal BackgroundThreadAwaitable(SynchronizationContext syncContext, int mainThreadId)
		{
			this._synchronizationContext = syncContext;
			this._mainThreadId = mainThreadId;
		}

		public BackgroundThreadAwaitable GetAwaiter()
		{
			return this;
		}

		public bool IsCompleted
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId != this._mainThreadId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			Task.Run(continuation);
		}

		private readonly SynchronizationContext _synchronizationContext;

		private readonly int _mainThreadId;
	}
}
