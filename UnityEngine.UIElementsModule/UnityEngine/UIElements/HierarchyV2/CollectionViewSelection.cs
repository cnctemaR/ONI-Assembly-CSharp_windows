using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal sealed class CollectionViewSelection
	{
		public int indexCount
		{
			get
			{
				return this.indices.Count;
			}
		}

		public int minIndex
		{
			get
			{
				bool flag = this.m_MinIndex == -1;
				if (flag)
				{
					this.m_MinIndex = int.MaxValue;
					foreach (int num in this.indices)
					{
						bool flag2 = num < this.m_MinIndex;
						if (flag2)
						{
							this.m_MinIndex = num;
						}
					}
				}
				return this.m_MinIndex;
			}
		}

		public int maxIndex
		{
			get
			{
				bool flag = this.m_MaxIndex == -1;
				if (flag)
				{
					foreach (int num in this.indices)
					{
						bool flag2 = num > this.m_MaxIndex;
						if (flag2)
						{
							this.m_MaxIndex = num;
						}
					}
				}
				return this.m_MaxIndex;
			}
		}

		public int capacity
		{
			get
			{
				return this.indices.Capacity;
			}
			set
			{
				this.indices.Capacity = value;
			}
		}

		public int FirstIndex()
		{
			return (this.indices.Count > 0) ? this.indices[0] : (-1);
		}

		public bool ContainsIndex(int index)
		{
			return this.m_IndexLookup.Contains(index);
		}

		public void AddIndex(int index)
		{
			this.m_IndexLookup.Add(index);
			this.indices.Add(index);
			bool flag = index < this.m_MinIndex;
			if (flag)
			{
				this.m_MinIndex = index;
			}
			bool flag2 = index > this.m_MaxIndex;
			if (flag2)
			{
				this.m_MaxIndex = index;
			}
		}

		public bool TryRemove(int index)
		{
			bool flag = !this.m_IndexLookup.Remove(index);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num = this.indices.IndexOf(index);
				bool flag3 = num >= 0;
				if (flag3)
				{
					this.indices.RemoveAt(num);
					bool flag4 = index == this.m_MinIndex;
					if (flag4)
					{
						this.m_MinIndex = -1;
					}
					bool flag5 = index == this.m_MaxIndex;
					if (flag5)
					{
						this.m_MaxIndex = -1;
					}
				}
				flag2 = true;
			}
			return flag2;
		}

		public void ClearIndices()
		{
			this.m_IndexLookup.Clear();
			this.indices.Clear();
			this.m_MinIndex = -1;
			this.m_MaxIndex = -1;
		}

		private readonly HashSet<int> m_IndexLookup = new HashSet<int>();

		private int m_MinIndex = -1;

		private int m_MaxIndex = -1;

		public readonly List<int> indices = new List<int>();
	}
}
