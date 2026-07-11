using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	internal sealed class QueueDebugView<T>
	{
		public QueueDebugView(Queue<T> queue)
		{
			if (queue == null)
			{
				throw new ArgumentNullException("queue");
			}
			this._queue = queue;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._queue.ToArray();
			}
		}

		private readonly Queue<T> _queue;
	}
}
