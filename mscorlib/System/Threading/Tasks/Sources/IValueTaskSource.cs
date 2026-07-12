using System;

namespace System.Threading.Tasks.Sources
{
	public interface IValueTaskSource
	{
		ValueTaskSourceStatus GetStatus(short token);

		void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags);

		void GetResult(short token);
	}
}
