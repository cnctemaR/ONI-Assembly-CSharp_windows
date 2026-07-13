using System;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR
{
	internal class EntryPool
	{
		public EntryPool(int maxCapacity = 1024)
		{
			this.m_ThreadEntries = new Stack<Entry>[JobsUtility.ThreadIndexCount];
			int i = 0;
			int threadIndexCount = JobsUtility.ThreadIndexCount;
			while (i < threadIndexCount)
			{
				this.m_ThreadEntries[i] = new Stack<Entry>(128);
				i++;
			}
			this.m_SharedPool = new ImplicitPool<Entry>(EntryPool.k_CreateAction, EntryPool.k_ResetAction, 128, maxCapacity);
		}

		public Entry Get()
		{
			Stack<Entry> stack = this.m_ThreadEntries[UIRUtility.GetThreadIndex()];
			bool flag = stack.Count == 0;
			if (flag)
			{
				ImplicitPool<Entry> sharedPool = this.m_SharedPool;
				lock (sharedPool)
				{
					for (int i = 0; i < 128; i++)
					{
						stack.Push(this.m_SharedPool.Get());
					}
				}
			}
			return stack.Pop();
		}

		public void ReturnAll()
		{
			int i = 0;
			int num = this.m_ThreadEntries.Length;
			while (i < num)
			{
				this.m_ThreadEntries[i].Clear();
				i++;
			}
			this.m_SharedPool.ReturnAll();
		}

		private const int k_StackSize = 128;

		private Stack<Entry>[] m_ThreadEntries;

		private ImplicitPool<Entry> m_SharedPool;

		private static readonly Func<Entry> k_CreateAction = () => new Entry();

		private static readonly Action<Entry> k_ResetAction = delegate(Entry e)
		{
			e.Reset();
		};
	}
}
