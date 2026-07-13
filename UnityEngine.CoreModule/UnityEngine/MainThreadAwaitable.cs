using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	public struct MainThreadAwaitable : INotifyCompletion
	{
		internal MainThreadAwaitable(SynchronizationContext syncContext, int mainThreadId)
		{
			this._synchronizationContext = syncContext;
			this._mainThreadId = mainThreadId;
		}

		public MainThreadAwaitable GetAwaiter()
		{
			return this;
		}

		public bool IsCompleted
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId == this._mainThreadId;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			this._synchronizationContext.Post(new SendOrPostCallback(MainThreadAwaitable.DoOnCompleted), continuation);
		}

		private static void DoOnCompleted(object continuation)
		{
			Action action = continuation as Action;
			if (action != null)
			{
				action();
			}
		}

		private readonly SynchronizationContext _synchronizationContext;

		private readonly int _mainThreadId;
	}
}
