using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class ChunkAllocatingArray<T>
	{
		public ChunkAllocatingArray()
		{
			this.m_Chunks = new List<T[]> { new T[2048] };
		}

		public T this[int index]
		{
			get
			{
				int num = index / 2048;
				int num2 = index % 2048;
				bool flag = num >= this.m_Chunks.Count;
				if (flag)
				{
					throw new IndexOutOfRangeException();
				}
				return this.m_Chunks[num][num2];
			}
			set
			{
				int i = index / 2048;
				int num = index % 2048;
				while (i >= this.m_Chunks.Count)
				{
					this.m_Chunks.Add(new T[2048]);
				}
				this.m_Chunks[i][num] = value;
			}
		}

		private const int k_ChunkSize = 2048;

		private readonly List<T[]> m_Chunks;
	}
}
