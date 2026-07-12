using System;

namespace System.Threading.Tasks.Sources
{
	public interface IValueTaskSource<out TResult>
	{
		ValueTaskSourceStatus GetStatus(short token);

		void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags);

		TResult GetResult(short token);
	}
}
