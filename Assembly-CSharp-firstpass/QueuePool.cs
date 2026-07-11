using System;
using System.Collections.Generic;

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

	private static ContainerPool<QueuePool<ObjectType, PoolIdentifier>.PooledQueue, PoolIdentifier> pool = new ContainerPool<QueuePool<ObjectType, PoolIdentifier>.PooledQueue, PoolIdentifier>();

	public class PooledQueue : Queue<ObjectType>
	{
		public void Recycle()
		{
			QueuePool<ObjectType, PoolIdentifier>.Free(this);
		}
	}
}
