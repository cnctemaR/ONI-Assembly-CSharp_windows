using System;
using System.Threading;

namespace System.Net.Mime
{
	internal sealed class MultiAsyncResult : LazyAsyncResult
	{
		internal MultiAsyncResult(object context, AsyncCallback callback, object state)
			: base(context, state, callback)
		{
			this._context = context;
		}

		internal object Context
		{
			get
			{
				return this._context;
			}
		}

		internal void Enter()
		{
			this.Increment();
		}

		internal void Leave()
		{
			this.Decrement();
		}

		internal void Leave(object result)
		{
			base.Result = result;
			this.Decrement();
		}

		private void Decrement()
		{
			if (Interlocked.Decrement(ref this._outstanding) == -1)
			{
				base.InvokeCallback(base.Result);
			}
		}

		private void Increment()
		{
			Interlocked.Increment(ref this._outstanding);
		}

		internal void CompleteSequence()
		{
			this.Decrement();
		}

		internal static object End(IAsyncResult result)
		{
			MultiAsyncResult multiAsyncResult = (MultiAsyncResult)result;
			multiAsyncResult.InternalWaitForCompletion();
			return multiAsyncResult.Result;
		}

		private readonly object _context;

		private int _outstanding;
	}
}
