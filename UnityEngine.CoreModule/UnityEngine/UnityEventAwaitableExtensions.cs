using System;
using UnityEngine.Events;

namespace UnityEngine
{
	public static class UnityEventAwaitableExtensions
	{
		public static Awaitable.Awaiter GetAwaiter(this UnityEvent ev)
		{
			Awaitable awaitable = Awaitable.NewManagedAwaitable();
			UnityAction cb = null;
			cb = delegate
			{
				ev.RemoveListener(cb);
				awaitable.RaiseManagedCompletion();
			};
			ev.AddListener(cb);
			return awaitable.GetAwaiter();
		}

		public static Awaitable<T>.Awaiter GetAwaiter<T>(this UnityEvent<T> ev)
		{
			Awaitable<T> awaitable = Awaitable<T>.GetManaged();
			UnityAction<T> cb = null;
			cb = delegate(T arg)
			{
				ev.RemoveListener(cb);
				awaitable.SetResultAndRaiseContinuation(arg);
			};
			ev.AddListener(cb);
			return awaitable.GetAwaiter();
		}

		public static Awaitable<ValueTuple<T0, T1>>.Awaiter GetAwaiter<T0, T1>(this UnityEvent<T0, T1> ev)
		{
			Awaitable<ValueTuple<T0, T1>> awaitable = Awaitable<ValueTuple<T0, T1>>.GetManaged();
			UnityAction<T0, T1> cb = null;
			cb = delegate(T0 arg0, T1 arg1)
			{
				ev.RemoveListener(cb);
				awaitable.SetResultAndRaiseContinuation(new ValueTuple<T0, T1>(arg0, arg1));
			};
			ev.AddListener(cb);
			return awaitable.GetAwaiter();
		}

		public static Awaitable<ValueTuple<T0, T1, T2>>.Awaiter GetAwaiter<T0, T1, T2>(this UnityEvent<T0, T1, T2> ev)
		{
			Awaitable<ValueTuple<T0, T1, T2>> awaitable = Awaitable<ValueTuple<T0, T1, T2>>.GetManaged();
			UnityAction<T0, T1, T2> cb = null;
			cb = delegate(T0 arg0, T1 arg1, T2 arg2)
			{
				ev.RemoveListener(cb);
				awaitable.SetResultAndRaiseContinuation(new ValueTuple<T0, T1, T2>(arg0, arg1, arg2));
			};
			ev.AddListener(cb);
			return awaitable.GetAwaiter();
		}

		public static Awaitable<ValueTuple<T0, T1, T2, T3>>.Awaiter GetAwaiter<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> ev)
		{
			Awaitable<ValueTuple<T0, T1, T2, T3>> awaitable = Awaitable<ValueTuple<T0, T1, T2, T3>>.GetManaged();
			UnityAction<T0, T1, T2, T3> cb = null;
			cb = delegate(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
			{
				ev.RemoveListener(cb);
				awaitable.SetResultAndRaiseContinuation(new ValueTuple<T0, T1, T2, T3>(arg0, arg1, arg2, arg3));
			};
			ev.AddListener(cb);
			return awaitable.GetAwaiter();
		}
	}
}
