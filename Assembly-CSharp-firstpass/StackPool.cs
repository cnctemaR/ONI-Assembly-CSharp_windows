using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class StackPool<ObjectType, PoolIdentifier>
{
	public static StackPool<ObjectType, PoolIdentifier>.PooledStack Allocate()
	{
		return StackPool<ObjectType, PoolIdentifier>.pool.Allocate();
	}

	private static void Free(StackPool<ObjectType, PoolIdentifier>.PooledStack queue)
	{
		queue.Clear();
		StackPool<ObjectType, PoolIdentifier>.pool.Free(queue);
	}

	public static ContainerPool GetPool()
	{
		return StackPool<ObjectType, PoolIdentifier>.pool;
	}

	private static ContainerPool<StackPool<ObjectType, PoolIdentifier>.PooledStack, PoolIdentifier> pool = new ContainerPool<StackPool<ObjectType, PoolIdentifier>.PooledStack, PoolIdentifier>();

	[DebuggerDisplay("Count={Count}")]
	public class PooledStack : Stack<ObjectType>, IDisposable
	{
		public void Recycle()
		{
			StackPool<ObjectType, PoolIdentifier>.Free(this);
		}

		public void Dispose()
		{
			this.Recycle();
		}
	}
}
