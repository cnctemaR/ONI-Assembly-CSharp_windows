using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class ObjectPool<T> where T : new()
	{
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

		public ObjectPool(Func<T> CreateFunc, int maxSize = 100)
		{
			this.maxSize = maxSize;
			bool flag = CreateFunc == null;
			if (flag)
			{
				this.CreateFunc = () => new T();
			}
			else
			{
				this.CreateFunc = CreateFunc;
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
			return (this.m_Stack.Count == 0) ? this.CreateFunc() : this.m_Stack.Pop();
		}

		public void Release(T element)
		{
			bool flag = this.m_Stack.Count > 0 && this.m_Stack.Peek() == element;
			if (flag)
			{
				Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
			}
			bool flag2 = this.m_Stack.Count < this.maxSize;
			if (flag2)
			{
				this.m_Stack.Push(element);
			}
		}

		private readonly Stack<T> m_Stack = new Stack<T>();

		private int m_MaxSize;

		internal Func<T> CreateFunc;
	}
}
