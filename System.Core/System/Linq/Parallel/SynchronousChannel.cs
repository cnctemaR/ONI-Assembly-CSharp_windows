using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal sealed class SynchronousChannel<T>
	{
		internal SynchronousChannel()
		{
		}

		internal void Init()
		{
			this._queue = new Queue<T>();
		}

		internal void Enqueue(T item)
		{
			this._queue.Enqueue(item);
		}

		internal T Dequeue()
		{
			return this._queue.Dequeue();
		}

		internal void SetDone()
		{
		}

		internal void CopyTo(T[] array, int arrayIndex)
		{
			this._queue.CopyTo(array, arrayIndex);
		}

		internal int Count
		{
			get
			{
				return this._queue.Count;
			}
		}

		private Queue<T> _queue;
	}
}
