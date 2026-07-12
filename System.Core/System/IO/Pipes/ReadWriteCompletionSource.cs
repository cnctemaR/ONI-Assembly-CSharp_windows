using System;

namespace System.IO.Pipes
{
	internal sealed class ReadWriteCompletionSource : PipeCompletionSource<int>
	{
		internal ReadWriteCompletionSource(PipeStream stream, ReadOnlyMemory<byte> bufferToPin, bool isWrite)
			: base(stream._threadPoolBinding, bufferToPin)
		{
			this._pipeStream = stream;
			this._isWrite = isWrite;
			this._isMessageComplete = true;
		}

		internal override void SetCompletedSynchronously()
		{
			if (!this._isWrite)
			{
				this._pipeStream.UpdateMessageCompletion(this._isMessageComplete);
			}
			base.TrySetResult(this._numBytes);
		}

		protected override void AsyncCallback(uint errorCode, uint numBytes)
		{
			this._numBytes = (int)numBytes;
			if (!this._isWrite && (errorCode == 109U || errorCode - 232U <= 1U))
			{
				errorCode = 0U;
			}
			if (errorCode == 234U)
			{
				errorCode = 0U;
				this._isMessageComplete = false;
			}
			else
			{
				this._isMessageComplete = true;
			}
			base.AsyncCallback(errorCode, numBytes);
		}

		protected override void HandleError(int errorCode)
		{
			base.TrySetException(this._pipeStream.WinIOError(errorCode));
		}

		private readonly bool _isWrite;

		private readonly PipeStream _pipeStream;

		private bool _isMessageComplete;

		private int _numBytes;
	}
}
