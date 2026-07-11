using System;

namespace System.Runtime
{
	internal class AsyncEventArgs<TArgument> : AsyncEventArgs
	{
		public TArgument Arguments { get; private set; }

		public virtual void Set(AsyncEventArgsCallback callback, TArgument arguments, object state)
		{
			base.SetAsyncState(callback, state);
			this.Arguments = arguments;
		}
	}
}
