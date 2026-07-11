using System;
using System.Threading;

namespace System.Xml.Linq
{
	internal sealed class XHashtable<TValue>
	{
		public XHashtable(XHashtable<TValue>.ExtractKeyDelegate extractKey, int capacity)
		{
			this.state = new XHashtable<TValue>.XHashtableState(extractKey, capacity);
		}

		public bool TryGetValue(string key, int index, int count, out TValue value)
		{
			return this.state.TryGetValue(key, index, count, out value);
		}

		public TValue Add(TValue value)
		{
			TValue tvalue;
			while (!this.state.TryAdd(value, out tvalue))
			{
				lock (this)
				{
					XHashtable<TValue>.XHashtableState xhashtableState = this.state.Resize();
					Thread.MemoryBarrier();
					this.state = xhashtableState;
				}
			}
			return tvalue;
		}

		private XHashtable<TValue>.XHashtableState state;

		private const int StartingHash = 352654597;

		public delegate string ExtractKeyDelegate(TValue value);

		private sealed class XHashtableState
		{
			public XHashtableState(XHashtable<TValue>.ExtractKeyDelegate extractKey, int capacity)
			{
				this.buckets = new int[capacity];
				this.entries = new XHashtable<TValue>.XHashtableState.Entry[capacity];
				this.extractKey = extractKey;
			}

			public XHashtable<TValue>.XHashtableState Resize()
			{
				if (this.numEntries < this.buckets.Length)
				{
					return this;
				}
				int num = 0;
				for (int i = 0; i < this.buckets.Length; i++)
				{
					int j = this.buckets[i];
					if (j == 0)
					{
						j = Interlocked.CompareExchange(ref this.buckets[i], -1, 0);
					}
					while (j > 0)
					{
						if (this.extractKey(this.entries[j].Value) != null)
						{
							num++;
						}
						if (this.entries[j].Next == 0)
						{
							j = Interlocked.CompareExchange(ref this.entries[j].Next, -1, 0);
						}
						else
						{
							j = this.entries[j].Next;
						}
					}
				}
				if (num < this.buckets.Length / 2)
				{
					num = this.buckets.Length;
				}
				else
				{
					num = this.buckets.Length * 2;
					if (num < 0)
					{
						throw new OverflowException();
					}
				}
				XHashtable<TValue>.XHashtableState xhashtableState = new XHashtable<TValue>.XHashtableState(this.extractKey, num);
				for (int k = 0; k < this.buckets.Length; k++)
				{
					for (int l = this.buckets[k]; l > 0; l = this.entries[l].Next)
					{
						TValue tvalue;
						xhashtableState.TryAdd(this.entries[l].Value, out tvalue);
					}
				}
				return xhashtableState;
			}

			public bool TryGetValue(string key, int index, int count, out TValue value)
			{
				int num = XHashtable<TValue>.XHashtableState.ComputeHashCode(key, index, count);
				int num2 = 0;
				if (this.FindEntry(num, key, index, count, ref num2))
				{
					value = this.entries[num2].Value;
					return true;
				}
				value = default(TValue);
				return false;
			}

			public bool TryAdd(TValue value, out TValue newValue)
			{
				newValue = value;
				string text = this.extractKey(value);
				if (text == null)
				{
					return true;
				}
				int num = XHashtable<TValue>.XHashtableState.ComputeHashCode(text, 0, text.Length);
				int num2 = Interlocked.Increment(ref this.numEntries);
				if (num2 < 0 || num2 >= this.buckets.Length)
				{
					return false;
				}
				this.entries[num2].Value = value;
				this.entries[num2].HashCode = num;
				Thread.MemoryBarrier();
				int num3 = 0;
				while (!this.FindEntry(num, text, 0, text.Length, ref num3))
				{
					if (num3 == 0)
					{
						num3 = Interlocked.CompareExchange(ref this.buckets[num & (this.buckets.Length - 1)], num2, 0);
					}
					else
					{
						num3 = Interlocked.CompareExchange(ref this.entries[num3].Next, num2, 0);
					}
					if (num3 <= 0)
					{
						return num3 == 0;
					}
				}
				newValue = this.entries[num3].Value;
				return true;
			}

			private bool FindEntry(int hashCode, string key, int index, int count, ref int entryIndex)
			{
				int num = entryIndex;
				int i;
				if (num == 0)
				{
					i = this.buckets[hashCode & (this.buckets.Length - 1)];
				}
				else
				{
					i = num;
				}
				while (i > 0)
				{
					if (this.entries[i].HashCode == hashCode)
					{
						string text = this.extractKey(this.entries[i].Value);
						if (text == null)
						{
							if (this.entries[i].Next > 0)
							{
								this.entries[i].Value = default(TValue);
								i = this.entries[i].Next;
								if (num == 0)
								{
									this.buckets[hashCode & (this.buckets.Length - 1)] = i;
									continue;
								}
								this.entries[num].Next = i;
								continue;
							}
						}
						else if (count == text.Length && string.CompareOrdinal(key, index, text, 0, count) == 0)
						{
							entryIndex = i;
							return true;
						}
					}
					num = i;
					i = this.entries[i].Next;
				}
				entryIndex = num;
				return false;
			}

			private static int ComputeHashCode(string key, int index, int count)
			{
				int num = 352654597;
				int num2 = index + count;
				for (int i = index; i < num2; i++)
				{
					num += (num << 7) ^ (int)key[i];
				}
				num -= num >> 17;
				num -= num >> 11;
				num -= num >> 5;
				return num & int.MaxValue;
			}

			private int[] buckets;

			private XHashtable<TValue>.XHashtableState.Entry[] entries;

			private int numEntries;

			private XHashtable<TValue>.ExtractKeyDelegate extractKey;

			private const int EndOfList = 0;

			private const int FullList = -1;

			private struct Entry
			{
				public TValue Value;

				public int HashCode;

				public int Next;
			}
		}
	}
}
