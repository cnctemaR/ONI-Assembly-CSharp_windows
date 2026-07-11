using System;

namespace System.Linq.Parallel
{
	internal sealed class SynchronousChannelMergeEnumerator<T> : MergeEnumerator<T>
	{
		internal SynchronousChannelMergeEnumerator(QueryTaskGroupState taskGroupState, SynchronousChannel<T>[] channels)
			: base(taskGroupState)
		{
			this._channels = channels;
			this._channelIndex = -1;
		}

		public override T Current
		{
			get
			{
				if (this._channelIndex == -1 || this._channelIndex == this._channels.Length)
				{
					throw new InvalidOperationException("Enumeration has not started. MoveNext must be called to initiate enumeration.");
				}
				return this._currentElement;
			}
		}

		public override bool MoveNext()
		{
			if (this._channelIndex == -1)
			{
				this._channelIndex = 0;
			}
			while (this._channelIndex != this._channels.Length)
			{
				SynchronousChannel<T> synchronousChannel = this._channels[this._channelIndex];
				if (synchronousChannel.Count != 0)
				{
					this._currentElement = synchronousChannel.Dequeue();
					return true;
				}
				this._channelIndex++;
			}
			return false;
		}

		private SynchronousChannel<T>[] _channels;

		private int _channelIndex;

		private T _currentElement;
	}
}
