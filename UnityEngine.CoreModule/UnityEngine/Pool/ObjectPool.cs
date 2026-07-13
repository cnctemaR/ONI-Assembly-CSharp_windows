using System;
using System.Collections.Generic;

namespace UnityEngine.Pool
{
	public class ObjectPool<T> : IDisposable, IPool, IObjectPool<T> where T : class
	{
		public int CountAll { get; private set; }

		public int CountActive
		{
			get
			{
				return this.CountAll - this.CountInactive;
			}
		}

		public int CountInactive
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public ObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
		{
			bool flag = createFunc == null;
			if (flag)
			{
				throw new ArgumentNullException("createFunc");
			}
			bool flag2 = maxSize <= 0;
			if (flag2)
			{
				throw new ArgumentException("Max Size must be greater than 0", "maxSize");
			}
			this.m_List = new List<T>(defaultCapacity);
			this.m_CreateFunc = createFunc;
			this.m_MaxSize = maxSize;
			this.m_ActionOnGet = actionOnGet;
			this.m_ActionOnRelease = actionOnRelease;
			this.m_ActionOnDestroy = actionOnDestroy;
			this.m_CollectionCheck = collectionCheck;
			PoolManager.Register(this);
		}

		public T Get()
		{
			bool flag = this.m_List.Count == 0;
			T t;
			if (flag)
			{
				t = this.m_CreateFunc();
				int countAll = this.CountAll;
				this.CountAll = countAll + 1;
			}
			else
			{
				int num = this.m_List.Count - 1;
				t = this.m_List[num];
				this.m_List.RemoveAt(num);
			}
			Action<T> actionOnGet = this.m_ActionOnGet;
			if (actionOnGet != null)
			{
				actionOnGet(t);
			}
			return t;
		}

		public PooledObject<T> Get(out T v)
		{
			return new PooledObject<T>(v = this.Get(), this);
		}

		public void Release(T element)
		{
			Action<T> actionOnRelease = this.m_ActionOnRelease;
			if (actionOnRelease != null)
			{
				actionOnRelease(element);
			}
			bool flag = this.CountInactive < this.m_MaxSize;
			if (flag)
			{
				this.m_List.Add(element);
			}
			else
			{
				int countAll = this.CountAll;
				this.CountAll = countAll - 1;
				Action<T> actionOnDestroy = this.m_ActionOnDestroy;
				if (actionOnDestroy != null)
				{
					actionOnDestroy(element);
				}
			}
		}

		public void Clear()
		{
			bool flag = this.m_ActionOnDestroy != null;
			if (flag)
			{
				foreach (T t in this.m_List)
				{
					this.m_ActionOnDestroy(t);
				}
			}
			this.m_List.Clear();
			this.CountAll = 0;
		}

		public void Dispose()
		{
			this.Clear();
		}

		internal readonly List<T> m_List;

		private readonly Func<T> m_CreateFunc;

		private readonly Action<T> m_ActionOnGet;

		private readonly Action<T> m_ActionOnRelease;

		private readonly Action<T> m_ActionOnDestroy;

		private readonly int m_MaxSize;

		internal bool m_CollectionCheck;
	}
}
