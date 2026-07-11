using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class ObjectPool<T> where T : new()
	{
		public ObjectPool(int maxSize = 100)
		{
			this.maxSize = maxSize;
		}

		public int maxSize
		{
			get
			{
				return this.m_MaxSize;
			}
			set
			{
				this.m_MaxSize = Math.Max(0, value);
				while (this.Size() > this.m_MaxSize)
				{
					this.Get();
				}
			}
		}

		public int Size()
		{
			return this.m_Stack.Count;
		}

		public void Clear()
		{
			this.m_Stack.Clear();
		}

		public T Get()
		{
			return (this.m_Stack.Count != 0) ? this.m_Stack.Pop() : new T();
		}

		public void Release(T element)
		{
			if (this.m_Stack.Count > 0 && object.ReferenceEquals(this.m_Stack.Peek(), element))
			{
				Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
			}
			if (this.m_Stack.Count < this.maxSize)
			{
				this.m_Stack.Push(element);
			}
		}

		private readonly Stack<T> m_Stack = new Stack<T>();

		private int m_MaxSize;
	}
}
