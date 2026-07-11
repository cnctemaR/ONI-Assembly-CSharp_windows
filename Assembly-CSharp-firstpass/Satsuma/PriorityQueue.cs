using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class PriorityQueue<TElement, TPriority> : IPriorityQueue<TElement, TPriority>, IReadOnlyPriorityQueue<TElement, TPriority>, IClearable where TPriority : IComparable<TPriority>
	{
		public void Clear()
		{
			this.payloads.Clear();
			this.priorities.Clear();
			this.positions.Clear();
		}

		public int Count
		{
			get
			{
				return this.payloads.Count;
			}
		}

		public IEnumerable<KeyValuePair<TElement, TPriority>> Items
		{
			get
			{
				int i = 0;
				int j = this.Count;
				while (i < j)
				{
					yield return new KeyValuePair<TElement, TPriority>(this.payloads[i], this.priorities[i]);
					int num = i;
					i = num + 1;
				}
				yield break;
			}
		}

		public TPriority this[TElement element]
		{
			get
			{
				return this.priorities[this.positions[element]];
			}
			set
			{
				int num;
				if (this.positions.TryGetValue(element, out num))
				{
					TPriority tpriority = this.priorities[num];
					this.priorities[num] = value;
					int num2 = value.CompareTo(tpriority);
					if (num2 < 0)
					{
						this.MoveUp(num);
						return;
					}
					if (num2 > 0)
					{
						this.MoveDown(num);
						return;
					}
				}
				else
				{
					this.payloads.Add(element);
					this.priorities.Add(value);
					num = this.Count - 1;
					this.positions[element] = num;
					this.MoveUp(num);
				}
			}
		}

		public bool Contains(TElement element)
		{
			return this.positions.ContainsKey(element);
		}

		public bool TryGetPriority(TElement element, out TPriority priority)
		{
			int num;
			if (!this.positions.TryGetValue(element, out num))
			{
				priority = default(TPriority);
				return false;
			}
			priority = this.priorities[num];
			return true;
		}

		private void RemoveAt(int pos)
		{
			int count = this.Count;
			TElement telement = this.payloads[pos];
			TPriority tpriority = this.priorities[pos];
			this.positions.Remove(telement);
			bool flag = count <= 1;
			if (!flag && pos != count - 1)
			{
				this.payloads[pos] = this.payloads[count - 1];
				this.priorities[pos] = this.priorities[count - 1];
				this.positions[this.payloads[pos]] = pos;
			}
			this.payloads.RemoveAt(count - 1);
			this.priorities.RemoveAt(count - 1);
			if (!flag && pos != count - 1)
			{
				TPriority tpriority2 = this.priorities[pos];
				int num = tpriority2.CompareTo(tpriority);
				if (num > 0)
				{
					this.MoveDown(pos);
					return;
				}
				if (num < 0)
				{
					this.MoveUp(pos);
				}
			}
		}

		public bool Remove(TElement element)
		{
			int num;
			bool flag = this.positions.TryGetValue(element, out num);
			if (flag)
			{
				this.RemoveAt(num);
			}
			return flag;
		}

		public TElement Peek()
		{
			return this.payloads[0];
		}

		public TElement Peek(out TPriority priority)
		{
			priority = this.priorities[0];
			return this.payloads[0];
		}

		public bool Pop()
		{
			if (this.Count == 0)
			{
				return false;
			}
			this.RemoveAt(0);
			return true;
		}

		private void MoveUp(int index)
		{
			TElement telement = this.payloads[index];
			TPriority tpriority = this.priorities[index];
			int i;
			int num;
			for (i = index; i > 0; i = num)
			{
				num = i / 2;
				if (tpriority.CompareTo(this.priorities[num]) >= 0)
				{
					break;
				}
				this.payloads[i] = this.payloads[num];
				this.priorities[i] = this.priorities[num];
				this.positions[this.payloads[i]] = i;
			}
			if (i != index)
			{
				this.payloads[i] = telement;
				this.priorities[i] = tpriority;
				this.positions[telement] = i;
			}
		}

		private void MoveDown(int index)
		{
			TElement telement = this.payloads[index];
			TPriority tpriority = this.priorities[index];
			int num = index;
			while (2 * num < this.Count)
			{
				int num2 = num;
				int num3 = 2 * num;
				if (tpriority.CompareTo(this.priorities[num3]) >= 0)
				{
					num2 = num3;
				}
				num3++;
				if (num3 < this.Count)
				{
					TPriority tpriority2 = this.priorities[num2];
					if (tpriority2.CompareTo(this.priorities[num3]) >= 0)
					{
						num2 = num3;
					}
				}
				if (num2 == num)
				{
					break;
				}
				this.payloads[num] = this.payloads[num2];
				this.priorities[num] = this.priorities[num2];
				this.positions[this.payloads[num]] = num;
				num = num2;
			}
			if (num != index)
			{
				this.payloads[num] = telement;
				this.priorities[num] = tpriority;
				this.positions[telement] = num;
			}
		}

		private List<TElement> payloads = new List<TElement>();

		private List<TPriority> priorities = new List<TPriority>();

		private Dictionary<TElement, int> positions = new Dictionary<TElement, int>();
	}
}
