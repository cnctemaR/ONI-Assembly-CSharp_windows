using System;
using System.Collections;

namespace System.Net
{
	internal sealed class InterlockedStack
	{
		internal InterlockedStack()
		{
		}

		internal void Push(object pooledStream)
		{
			if (pooledStream == null)
			{
				throw new ArgumentNullException("pooledStream");
			}
			object syncRoot = this._stack.SyncRoot;
			lock (syncRoot)
			{
				this._stack.Push(pooledStream);
			}
		}

		internal object Pop()
		{
			object syncRoot = this._stack.SyncRoot;
			object obj2;
			lock (syncRoot)
			{
				object obj = null;
				if (0 < this._stack.Count)
				{
					obj = this._stack.Pop();
				}
				obj2 = obj;
			}
			return obj2;
		}

		private readonly Stack _stack = new Stack();
	}
}
