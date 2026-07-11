using System;

namespace System.Runtime
{
	internal class AsyncEventArgs<TArgument, TResult> : AsyncEventArgs<TArgument>
	{
		public TResult Result { get; set; }
	}
}
