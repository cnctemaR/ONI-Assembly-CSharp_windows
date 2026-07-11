using System;
using System.Collections.Generic;

namespace UnityEngine
{
	internal class IntervalTree<T> where T : class, IInterval
	{
		public bool dirty { get; internal set; }

		public void Add(T item)
		{
			if (item != null)
			{
				this.m_Entries.Add(new IntervalTree<T>.Entry
				{
					intervalStart = item.intervalStart,
					intervalEnd = item.intervalEnd,
					item = item
				});
				this.dirty = true;
			}
		}

		public void IntersectsWith(long value, int bitFlag, List<T> results)
		{
			if (this.m_Entries.Count != 0)
			{
				if (this.dirty)
				{
					this.Rebuild();
					this.dirty = false;
				}
				if (this.m_Nodes.Count > 0)
				{
					this.Query(this.m_Nodes[0], value, bitFlag, results);
				}
			}
		}

		public void UpdateIntervals()
		{
			bool flag = false;
			for (int i = 0; i < this.m_Entries.Count; i++)
			{
				IntervalTree<T>.Entry entry = this.m_Entries[i];
				long intervalStart = entry.item.intervalStart;
				long intervalEnd = entry.item.intervalEnd;
				flag |= entry.intervalStart != intervalStart;
				flag |= entry.intervalEnd != intervalEnd;
				this.m_Entries[i] = new IntervalTree<T>.Entry
				{
					intervalStart = intervalStart,
					intervalEnd = intervalEnd,
					item = entry.item
				};
			}
			this.dirty = this.dirty || flag;
		}

		private void Query(IntervalTreeNode node, long value, int bitflag, List<T> results)
		{
			for (int i = node.first; i <= node.last; i++)
			{
				IntervalTree<T>.Entry entry = this.m_Entries[i];
				if (value >= entry.intervalStart && value < entry.intervalEnd)
				{
					entry.item.intervalBit = bitflag;
					results.Add(entry.item);
				}
			}
			if (node.center != 9223372036854775807L)
			{
				if (node.left != -1 && value < node.center)
				{
					this.Query(this.m_Nodes[node.left], value, bitflag, results);
				}
				if (node.right != -1 && value > node.center)
				{
					this.Query(this.m_Nodes[node.right], value, bitflag, results);
				}
			}
		}

		private void Rebuild()
		{
			this.m_Nodes.Clear();
			this.m_Nodes.Capacity = this.m_Entries.Capacity;
			this.Rebuild(0, this.m_Entries.Count - 1);
		}

		private int Rebuild(int start, int end)
		{
			IntervalTreeNode intervalTreeNode = default(IntervalTreeNode);
			int num = end - start + 1;
			int num2;
			if (num < 10)
			{
				intervalTreeNode = new IntervalTreeNode
				{
					center = long.MaxValue,
					first = start,
					last = end,
					left = -1,
					right = -1
				};
				this.m_Nodes.Add(intervalTreeNode);
				num2 = this.m_Nodes.Count - 1;
			}
			else
			{
				long num3 = long.MaxValue;
				long num4 = long.MinValue;
				for (int i = start; i <= end; i++)
				{
					IntervalTree<T>.Entry entry = this.m_Entries[i];
					num3 = Math.Min(num3, entry.intervalStart);
					num4 = Math.Max(num4, entry.intervalEnd);
				}
				long num5 = (num4 + num3) / 2L;
				intervalTreeNode.center = num5;
				int num6 = start;
				int num7 = end;
				for (;;)
				{
					while (num6 <= end && this.m_Entries[num6].intervalEnd < num5)
					{
						num6++;
					}
					while (num7 >= start && this.m_Entries[num7].intervalEnd >= num5)
					{
						num7--;
					}
					if (num6 > num7)
					{
						break;
					}
					IntervalTree<T>.Entry entry2 = this.m_Entries[num6];
					IntervalTree<T>.Entry entry3 = this.m_Entries[num7];
					this.m_Entries[num7] = entry2;
					this.m_Entries[num6] = entry3;
				}
				intervalTreeNode.first = num6;
				num7 = end;
				for (;;)
				{
					while (num6 <= end && this.m_Entries[num6].intervalStart <= num5)
					{
						num6++;
					}
					while (num7 >= start && this.m_Entries[num7].intervalStart > num5)
					{
						num7--;
					}
					if (num6 > num7)
					{
						break;
					}
					IntervalTree<T>.Entry entry4 = this.m_Entries[num6];
					IntervalTree<T>.Entry entry5 = this.m_Entries[num7];
					this.m_Entries[num7] = entry4;
					this.m_Entries[num6] = entry5;
				}
				intervalTreeNode.last = num7;
				this.m_Nodes.Add(default(IntervalTreeNode));
				int num8 = this.m_Nodes.Count - 1;
				intervalTreeNode.left = -1;
				intervalTreeNode.right = -1;
				if (start < intervalTreeNode.first)
				{
					intervalTreeNode.left = this.Rebuild(start, intervalTreeNode.first - 1);
				}
				if (end > intervalTreeNode.last)
				{
					intervalTreeNode.right = this.Rebuild(intervalTreeNode.last + 1, end);
				}
				this.m_Nodes[num8] = intervalTreeNode;
				num2 = num8;
			}
			return num2;
		}

		private const int kMinNodeSize = 10;

		private const int kInvalidNode = -1;

		private const long kCenterUnknown = 9223372036854775807L;

		private readonly List<IntervalTree<T>.Entry> m_Entries = new List<IntervalTree<T>.Entry>();

		private readonly List<IntervalTreeNode> m_Nodes = new List<IntervalTreeNode>();

		internal struct Entry
		{
			public long intervalStart;

			public long intervalEnd;

			public T item;
		}
	}
}
