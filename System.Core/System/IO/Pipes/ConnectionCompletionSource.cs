using System;

namespace System.IO.Pipes
{
	internal sealed class ConnectionCompletionSource : PipeCompletionSource<VoidResult>
	{
		internal ConnectionCompletionSource(NamedPipeServerStream server)
			: base(server._threadPoolBinding, ReadOnlyMemory<byte>.Empty)
		{
			this._serverStream = server;
		}

		internal override void SetCompletedSynchronously()
		{
			this._serverStream.State = PipeState.Connected;
			base.TrySetResult(default(VoidResult));
		}

		protected override void AsyncCallback(uint errorCode, uint numBytes)
		{
			if (errorCode == 535U)
			{
				errorCode = 0U;
			}
			base.AsyncCallback(errorCode, numBytes);
		}

		protected override void HandleError(int errorCode)
		{
			base.TrySetException(Win32Marshal.GetExceptionForWin32Error(errorCode, ""));
		}

		protected override void HandleUnexpectedCancellation()
		{
			base.TrySetException(Error.GetOperationAborted());
		}

		private readonly NamedPipeServerStream _serverStream;
	}
}
