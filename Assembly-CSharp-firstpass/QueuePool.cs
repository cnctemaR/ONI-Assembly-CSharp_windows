using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class QueuePool<ObjectType, PoolIdentifier>
{
	public static QueuePool<ObjectType, PoolIdentifier>.PooledQueue Allocate()
	{
		return QueuePool<ObjectType, PoolIdentifier>.pool.Allocate();
	}

	private static void Free(QueuePool<ObjectType, PoolIdentifier>.PooledQueue queue)
	{
		queue.Clear();
		QueuePool<ObjectType, PoolIdentifier>.pool.Free(queue);
	}

	public static ContainerPool GetPool()
	{
		return QueuePool<ObjectType, PoolIdentifier>.pool;
	}

	private static ContainerPool<QueuePool<ObjectType, PoolIdentifier>.PooledQueue, PoolIdentifier> pool = new ContainerPool<QueuePool<ObjectType, PoolIdentifier>.PooledQueue, PoolIdentifier>();

	[DebuggerDisplay("Count={Count}")]
	public class PooledQueue : Queue<ObjectType>, IDisposable
	{
		public void Recycle()
		{
			QueuePool<ObjectType, PoolIdentifier>.Free(this);
		}

		public void Dispose()
		{
			this.Recycle();
		}
	}
}
