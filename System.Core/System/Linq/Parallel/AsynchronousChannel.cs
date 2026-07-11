using System;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class AsynchronousChannel<T> : IDisposable
	{
		internal AsynchronousChannel(int index, int chunkSize, CancellationToken cancellationToken, IntValueEvent consumerEvent)
			: this(index, 512, chunkSize, cancellationToken, consumerEvent)
		{
		}

		internal AsynchronousChannel(int index, int capacity, int chunkSize, CancellationToken cancellationToken, IntValueEvent consumerEvent)
		{
			if (chunkSize == 0)
			{
				chunkSize = Scheduling.GetDefaultChunkSize<T>();
			}
			this._index = index;
			this._buffer = new T[capacity + 1][];
			this._producerBufferIndex = 0;
			this._consumerBufferIndex = 0;
			this._producerEvent = new ManualResetEventSlim();
			this._consumerEvent = consumerEvent;
			this._chunkSize = chunkSize;
			this._producerChunk = new T[chunkSize];
			this._producerChunkIndex = 0;
			this._cancellationToken = cancellationToken;
		}

		internal bool IsFull
		{
			get
			{
				int producerBufferIndex = this._producerBufferIndex;
				int consumerBufferIndex = this._consumerBufferIndex;
				return producerBufferIndex == consumerBufferIndex - 1 || (consumerBufferIndex == 0 && producerBufferIndex == this._buffer.Length - 1);
			}
		}

		internal bool IsChunkBufferEmpty
		{
			get
			{
				return this._producerBufferIndex == this._consumerBufferIndex;
			}
		}

		internal bool IsDone
		{
			get
			{
				return this._done;
			}
		}

		internal void FlushBuffers()
		{
			this.FlushCachedChunk();
		}

		internal void SetDone()
		{
			this._done = true;
			lock (this)
			{
				if (this._consumerEvent != null)
				{
					this._consumerEvent.Set(this._index);
				}
			}
		}

		internal void Enqueue(T item)
		{
			int producerChunkIndex = this._producerChunkIndex;
			this._producerChunk[producerChunkIndex] = item;
			if (producerChunkIndex == this._chunkSize - 1)
			{
				this.EnqueueChunk(this._producerChunk);
				this._producerChunk = new T[this._chunkSize];
			}
			this._producerChunkIndex = (producerChunkIndex + 1) % this._chunkSize;
		}

		private void EnqueueChunk(T[] chunk)
		{
			if (this.IsFull)
			{
				this.WaitUntilNonFull();
			}
			int producerBufferIndex = this._producerBufferIndex;
			this._buffer[producerBufferIndex] = chunk;
			Interlocked.Exchange(ref this._producerBufferIndex, (producerBufferIndex + 1) % this._buffer.Length);
			if (this._consumerIsWaiting == 1 && !this.IsChunkBufferEmpty)
			{
				this._consumerIsWaiting = 0;
				this._consumerEvent.Set(this._index);
			}
		}

		private void WaitUntilNonFull()
		{
			do
			{
				this._producerEvent.Reset();
				Interlocked.Exchange(ref this._producerIsWaiting, 1);
				if (this.IsFull)
				{
					this._producerEvent.Wait(this._cancellationToken);
				}
				else
				{
					this._producerIsWaiting = 0;
				}
			}
			while (this.IsFull);
		}

		private void FlushCachedChunk()
		{
			if (this._producerChunk != null && this._producerChunkIndex != 0)
			{
				T[] array = new T[this._producerChunkIndex];
				Array.Copy(this._producerChunk, 0, array, 0, this._producerChunkIndex);
				this.EnqueueChunk(array);
				this._producerChunk = null;
			}
		}

		internal bool TryDequeue(ref T item)
		{
			if (this._consumerChunk == null)
			{
				if (!this.TryDequeueChunk(ref this._consumerChunk))
				{
					return false;
				}
				this._consumerChunkIndex = 0;
			}
			item = this._consumerChunk[this._consumerChunkIndex];
			this._consumerChunkIndex++;
			if (this._consumerChunkIndex == this._consumerChunk.Length)
			{
				this._consumerChunk = null;
			}
			return true;
		}

		private bool TryDequeueChunk(ref T[] chunk)
		{
			if (this.IsChunkBufferEmpty)
			{
				return false;
			}
			chunk = this.InternalDequeueChunk();
			return true;
		}

		internal bool TryDequeue(ref T item, ref bool isDone)
		{
			isDone = false;
			if (this._consumerChunk == null)
			{
				if (!this.TryDequeueChunk(ref this._consumerChunk, ref isDone))
				{
					return false;
				}
				this._consumerChunkIndex = 0;
			}
			item = this._consumerChunk[this._consumerChunkIndex];
			this._consumerChunkIndex++;
			if (this._consumerChunkIndex == this._consumerChunk.Length)
			{
				this._consumerChunk = null;
			}
			return true;
		}

		private bool TryDequeueChunk(ref T[] chunk, ref bool isDone)
		{
			isDone = false;
			while (this.IsChunkBufferEmpty)
			{
				if (this.IsDone && this.IsChunkBufferEmpty)
				{
					isDone = true;
					return false;
				}
				Interlocked.Exchange(ref this._consumerIsWaiting, 1);
				if (this.IsChunkBufferEmpty && !this.IsDone)
				{
					return false;
				}
				this._consumerIsWaiting = 0;
			}
			chunk = this.InternalDequeueChunk();
			return true;
		}

		private T[] InternalDequeueChunk()
		{
			int consumerBufferIndex = this._consumerBufferIndex;
			T[] array = this._buffer[consumerBufferIndex];
			this._buffer[consumerBufferIndex] = null;
			Interlocked.Exchange(ref this._consumerBufferIndex, (consumerBufferIndex + 1) % this._buffer.Length);
			if (this._producerIsWaiting == 1 && !this.IsFull)
			{
				this._producerIsWaiting = 0;
				this._producerEvent.Set();
			}
			return array;
		}

		internal void DoneWithDequeueWait()
		{
			this._consumerIsWaiting = 0;
		}

		public void Dispose()
		{
			lock (this)
			{
				this._producerEvent.Dispose();
				this._producerEvent = null;
				this._consumerEvent = null;
			}
		}

		private T[][] _buffer;

		private readonly int _index;

		private volatile int _producerBufferIndex;

		private volatile int _consumerBufferIndex;

		private volatile bool _done;

		private T[] _producerChunk;

		private int _producerChunkIndex;

		private T[] _consumerChunk;

		private int _consumerChunkIndex;

		private int _chunkSize;

		private ManualResetEventSlim _producerEvent;

		private IntValueEvent _consumerEvent;

		private volatile int _producerIsWaiting;

		private volatile int _consumerIsWaiting;

		private CancellationToken _cancellationToken;
	}
}
