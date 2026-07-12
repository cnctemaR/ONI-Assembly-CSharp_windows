using System;
using System.Threading;

namespace System.Xml.Linq
{
	internal sealed class XHashtable<TValue>
	{
		public XHashtable(XHashtable<TValue>.ExtractKeyDelegate extractKey, int capacity)
		{
			this._state = new XHashtable<TValue>.XHashtableState(extractKey, capacity);
		}

		public bool TryGetValue(string key, int index, int count, out TValue value)
		{
			return this._state.TryGetValue(key, index, count, out value);
		}

		public TValue Add(TValue value)
		{
			TValue tvalue;
			while (!this._state.TryAdd(value, out tvalue))
			{
				lock (this)
				{
					XHashtable<TValue>.XHashtableState xhashtableState = this._state.Resize();
					Thread.MemoryBarrier();
					this._state = xhashtableState;
				}
			}
			return tvalue;
		}

		private XHashtable<TValue>.XHashtableState _state;

		private const int StartingHash = 352654597;

		public delegate string ExtractKeyDelegate(TValue value);

		private sealed class XHashtableState
		{
			public XHashtableState(XHashtable<TValue>.ExtractKeyDelegate extractKey, int capacity)
			{
				this._buckets = new int[capacity];
				this._entries = new XHashtable<TValue>.XHashtableState.Entry[capacity];
				this._extractKey = extractKey;
			}

			public XHashtable<TValue>.XHashtableState Resize()
			{
				if (this._numEntries < this._buckets.Length)
				{
					return this;
				}
				int num = 0;
				for (int i = 0; i < this._buckets.Length; i++)
				{
					int j = this._buckets[i];
					if (j == 0)
					{
						j = Interlocked.CompareExchange(ref this._buckets[i], -1, 0);
					}
					while (j > 0)
					{
						if (this._extractKey(this._entries[j].Value) != null)
						{
							num++;
						}
						if (this._entries[j].Next == 0)
						{
							j = Interlocked.CompareExchange(ref this._entries[j].Next, -1, 0);
						}
						else
						{
							j = this._entries[j].Next;
						}
					}
				}
				if (num < this._buckets.Length / 2)
				{
					num = this._buckets.Length;
				}
				else
				{
					num = this._buckets.Length * 2;
					if (num < 0)
					{
						throw new OverflowException();
					}
				}
				XHashtable<TValue>.XHashtableState xhashtableState = new XHashtable<TValue>.XHashtableState(this._extractKey, num);
				for (int k = 0; k < this._buckets.Length; k++)
				{
					for (int l = this._buckets[k]; l > 0; l = this._entries[l].Next)
					{
						TValue tvalue;
						xhashtableState.TryAdd(this._entries[l].Value, out tvalue);
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
					value = this._entries[num2].Value;
					return true;
				}
				value = default(TValue);
				return false;
			}

			public bool TryAdd(TValue value, out TValue newValue)
			{
				newValue = value;
				string text = this._extractKey(value);
				if (text == null)
				{
					return true;
				}
				int num = XHashtable<TValue>.XHashtableState.ComputeHashCode(text, 0, text.Length);
				int num2 = Interlocked.Increment(ref this._numEntries);
				if (num2 < 0 || num2 >= this._buckets.Length)
				{
					return false;
				}
				this._entries[num2].Value = value;
				this._entries[num2].HashCode = num;
				Thread.MemoryBarrier();
				int num3 = 0;
				while (!this.FindEntry(num, text, 0, text.Length, ref num3))
				{
					if (num3 == 0)
					{
						num3 = Interlocked.CompareExchange(ref this._buckets[num & (this._buckets.Length - 1)], num2, 0);
					}
					else
					{
						num3 = Interlocked.CompareExchange(ref this._entries[num3].Next, num2, 0);
					}
					if (num3 <= 0)
					{
						return num3 == 0;
					}
				}
				newValue = this._entries[num3].Value;
				return true;
			}

			private bool FindEntry(int hashCode, string key, int index, int count, ref int entryIndex)
			{
				int num = entryIndex;
				int i;
				if (num == 0)
				{
					i = this._buckets[hashCode & (this._buckets.Length - 1)];
				}
				else
				{
					i = num;
				}
				while (i > 0)
				{
					if (this._entries[i].HashCode == hashCode)
					{
						string text = this._extractKey(this._entries[i].Value);
						if (text == null)
						{
							if (this._entries[i].Next > 0)
							{
								this._entries[i].Value = default(TValue);
								i = this._entries[i].Next;
								if (num == 0)
								{
									this._buckets[hashCode & (this._buckets.Length - 1)] = i;
									continue;
								}
								this._entries[num].Next = i;
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
					i = this._entries[i].Next;
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

			private int[] _buckets;

			private XHashtable<TValue>.XHashtableState.Entry[] _entries;

			private int _numEntries;

			private XHashtable<TValue>.ExtractKeyDelegate _extractKey;

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
