using System;
using System.Collections.Generic;

namespace UnityEngine.Timeline
{
	internal class IntervalTree<T> where T : IInterval
	{
		public bool dirty { get; internal set; }

		public void Add(T item)
		{
			if (item == null)
			{
				return;
			}
			this.m_Entries.Add(new IntervalTree<T>.Entry
			{
				intervalStart = item.intervalStart,
				intervalEnd = item.intervalEnd,
				item = item
			});
			this.dirty = true;
		}

		public void IntersectsWith(long value, List<T> results)
		{
			if (this.m_Entries.Count == 0)
			{
				return;
			}
			if (this.dirty)
			{
				this.Rebuild();
				this.dirty = false;
			}
			if (this.m_Nodes.Count > 0)
			{
				this.Query(this.m_Nodes[0], value, results);
			}
		}

		public void IntersectsWithRange(long start, long end, List<T> results)
		{
			if (start > end)
			{
				return;
			}
			if (this.m_Entries.Count == 0)
			{
				return;
			}
			if (this.dirty)
			{
				this.Rebuild();
				this.dirty = false;
			}
			if (this.m_Nodes.Count > 0)
			{
				this.QueryRange(this.m_Nodes[0], start, end, results);
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

		private void Query(IntervalTreeNode intervalTreeNode, long value, List<T> results)
		{
			for (int i = intervalTreeNode.first; i <= intervalTreeNode.last; i++)
			{
				IntervalTree<T>.Entry entry = this.m_Entries[i];
				if (value >= entry.intervalStart && value < entry.intervalEnd)
				{
					results.Add(entry.item);
				}
			}
			if (intervalTreeNode.center == 9223372036854775807L)
			{
				return;
			}
			if (intervalTreeNode.left != -1 && value < intervalTreeNode.center)
			{
				this.Query(this.m_Nodes[intervalTreeNode.left], value, results);
			}
			if (intervalTreeNode.right != -1 && value > intervalTreeNode.center)
			{
				this.Query(this.m_Nodes[intervalTreeNode.right], value, results);
			}
		}

		private void QueryRange(IntervalTreeNode intervalTreeNode, long start, long end, List<T> results)
		{
			for (int i = intervalTreeNode.first; i <= intervalTreeNode.last; i++)
			{
				IntervalTree<T>.Entry entry = this.m_Entries[i];
				if (end >= entry.intervalStart && start < entry.intervalEnd)
				{
					results.Add(entry.item);
				}
			}
			if (intervalTreeNode.center == 9223372036854775807L)
			{
				return;
			}
			if (intervalTreeNode.left != -1 && start < intervalTreeNode.center)
			{
				this.QueryRange(this.m_Nodes[intervalTreeNode.left], start, end, results);
			}
			if (intervalTreeNode.right != -1 && end > intervalTreeNode.center)
			{
				this.QueryRange(this.m_Nodes[intervalTreeNode.right], start, end, results);
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
			if (end - start + 1 < 10)
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
				return this.m_Nodes.Count - 1;
			}
			long num = long.MaxValue;
			long num2 = long.MinValue;
			for (int i = start; i <= end; i++)
			{
				IntervalTree<T>.Entry entry = this.m_Entries[i];
				num = Math.Min(num, entry.intervalStart);
				num2 = Math.Max(num2, entry.intervalEnd);
			}
			long num3 = (num2 + num) / 2L;
			intervalTreeNode.center = num3;
			int num4 = start;
			int num5 = end;
			for (;;)
			{
				if (num4 <= end)
				{
					if (this.m_Entries[num4].intervalEnd < num3)
					{
						num4++;
						continue;
					}
				}
				while (num5 >= start && this.m_Entries[num5].intervalEnd >= num3)
				{
					num5--;
				}
				if (num4 > num5)
				{
					break;
				}
				IntervalTree<T>.Entry entry2 = this.m_Entries[num4];
				IntervalTree<T>.Entry entry3 = this.m_Entries[num5];
				this.m_Entries[num5] = entry2;
				this.m_Entries[num4] = entry3;
			}
			intervalTreeNode.first = num4;
			num5 = end;
			for (;;)
			{
				if (num4 <= end)
				{
					if (this.m_Entries[num4].intervalStart <= num3)
					{
						num4++;
						continue;
					}
				}
				while (num5 >= start && this.m_Entries[num5].intervalStart > num3)
				{
					num5--;
				}
				if (num4 > num5)
				{
					break;
				}
				IntervalTree<T>.Entry entry4 = this.m_Entries[num4];
				IntervalTree<T>.Entry entry5 = this.m_Entries[num5];
				this.m_Entries[num5] = entry4;
				this.m_Entries[num4] = entry5;
			}
			intervalTreeNode.last = num5;
			this.m_Nodes.Add(default(IntervalTreeNode));
			int num6 = this.m_Nodes.Count - 1;
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
			this.m_Nodes[num6] = intervalTreeNode;
			return num6;
		}

		public void Clear()
		{
			this.m_Entries.Clear();
			this.m_Nodes.Clear();
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
