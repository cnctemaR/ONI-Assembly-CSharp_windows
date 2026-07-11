using System;

namespace UnityEngine.UIElements.UIR
{
	internal class Pool<T> where T : PoolItem, new()
	{
		public T Get()
		{
			bool flag = this.m_Pool == null;
			T t;
			if (flag)
			{
				t = new T();
			}
			else
			{
				Debug.Assert(this.m_Pool != null);
				T t2 = (T)((object)this.m_Pool);
				this.m_Pool = this.m_Pool.poolNext;
				t2.poolNext = null;
				t = t2;
			}
			return t;
		}

		public void Return(T obj)
		{
			obj.poolNext = this.m_Pool;
			this.m_Pool = obj;
		}

		private PoolItem m_Pool;
	}
}
