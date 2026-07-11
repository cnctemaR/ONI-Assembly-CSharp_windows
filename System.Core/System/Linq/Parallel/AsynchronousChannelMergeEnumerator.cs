using System;

namespace System.Linq.Parallel
{
	internal sealed class AsynchronousChannelMergeEnumerator<T> : MergeEnumerator<T>
	{
		internal AsynchronousChannelMergeEnumerator(QueryTaskGroupState taskGroupState, AsynchronousChannel<T>[] channels, IntValueEvent consumerEvent)
			: base(taskGroupState)
		{
			this._channels = channels;
			this._channelIndex = -1;
			this._done = new bool[this._channels.Length];
			this._consumerEvent = consumerEvent;
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
			int num = this._channelIndex;
			if (num == -1)
			{
				num = (this._channelIndex = 0);
			}
			if (num == this._channels.Length)
			{
				return false;
			}
			if (!this._done[num] && this._channels[num].TryDequeue(ref this._currentElement))
			{
				this._channelIndex = (num + 1) % this._channels.Length;
				return true;
			}
			return this.MoveNextSlowPath();
		}

		private bool MoveNextSlowPath()
		{
			int num = 0;
			int num2 = this._channelIndex;
			int num3;
			while ((num3 = this._channelIndex) != this._channels.Length)
			{
				AsynchronousChannel<T> asynchronousChannel = this._channels[num3];
				bool flag = this._done[num3];
				if (!flag && asynchronousChannel.TryDequeue(ref this._currentElement))
				{
					this._channelIndex = (num3 + 1) % this._channels.Length;
					return true;
				}
				if (!flag && asynchronousChannel.IsDone)
				{
					if (!asynchronousChannel.IsChunkBufferEmpty)
					{
						asynchronousChannel.TryDequeue(ref this._currentElement);
						return true;
					}
					this._done[num3] = true;
					flag = true;
					asynchronousChannel.Dispose();
				}
				if (flag && ++num == this._channels.Length)
				{
					this._channelIndex = this._channels.Length;
					break;
				}
				num3 = (this._channelIndex = (num3 + 1) % this._channels.Length);
				if (num3 == num2)
				{
					try
					{
						num = 0;
						for (int i = 0; i < this._channels.Length; i++)
						{
							bool flag2 = false;
							if (!this._done[i] && this._channels[i].TryDequeue(ref this._currentElement, ref flag2))
							{
								return true;
							}
							if (flag2)
							{
								if (!this._done[i])
								{
									this._done[i] = true;
								}
								if (++num == this._channels.Length)
								{
									num3 = (this._channelIndex = this._channels.Length);
									break;
								}
							}
						}
						if (num3 == this._channels.Length)
						{
							break;
						}
						this._consumerEvent.Wait();
						num3 = (this._channelIndex = this._consumerEvent.Value);
						this._consumerEvent.Reset();
						num2 = num3;
						num = 0;
					}
					finally
					{
						for (int j = 0; j < this._channels.Length; j++)
						{
							if (!this._done[j])
							{
								this._channels[j].DoneWithDequeueWait();
							}
						}
					}
					continue;
				}
			}
			this._taskGroupState.QueryEnd(false);
			return false;
		}

		public override void Dispose()
		{
			if (this._consumerEvent != null)
			{
				base.Dispose();
				this._consumerEvent.Dispose();
				this._consumerEvent = null;
			}
		}

		private AsynchronousChannel<T>[] _channels;

		private IntValueEvent _consumerEvent;

		private bool[] _done;

		private int _channelIndex;

		private T _currentElement;
	}
}
