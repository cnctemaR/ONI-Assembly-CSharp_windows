using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace System.Net
{
	internal class WebCompletionSource
	{
		public WebCompletionSource()
		{
			this.completion = new TaskCompletionSource<WebCompletionSource.Result>();
		}

		public bool TrySetCompleted()
		{
			return this.completion.TrySetResult(new WebCompletionSource.Result(WebCompletionSource.State.Completed, null));
		}

		public bool TrySetCanceled()
		{
			OperationCanceledException ex = new OperationCanceledException();
			WebCompletionSource.Result result = new WebCompletionSource.Result(WebCompletionSource.State.Canceled, ExceptionDispatchInfo.Capture(ex));
			return this.completion.TrySetResult(result);
		}

		public bool TrySetException(Exception error)
		{
			WebCompletionSource.Result result = new WebCompletionSource.Result(WebCompletionSource.State.Faulted, ExceptionDispatchInfo.Capture(error));
			return this.completion.TrySetResult(result);
		}

		public bool IsCompleted
		{
			get
			{
				return this.completion.Task.IsCompleted;
			}
		}

		public void ThrowOnError()
		{
			if (!this.completion.Task.IsCompleted)
			{
				return;
			}
			ExceptionDispatchInfo error = this.completion.Task.Result.Error;
			if (error == null)
			{
				return;
			}
			error.Throw();
		}

		public async Task<bool> WaitForCompletion(bool throwOnError)
		{
			WebCompletionSource.Result result = await this.completion.Task.ConfigureAwait(false);
			bool flag;
			if (result.State == WebCompletionSource.State.Completed)
			{
				flag = true;
			}
			else
			{
				if (throwOnError)
				{
					result.Error.Throw();
				}
				flag = false;
			}
			return flag;
		}

		private TaskCompletionSource<WebCompletionSource.Result> completion;

		private enum State
		{
			Running,
			Completed,
			Canceled,
			Faulted
		}

		private class Result
		{
			public WebCompletionSource.State State { get; }

			public ExceptionDispatchInfo Error { get; }

			public Result(WebCompletionSource.State state, ExceptionDispatchInfo error)
			{
				this.State = state;
				this.Error = error;
			}
		}
	}
}
