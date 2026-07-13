using System;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal struct RingControl
	{
		internal RingControl(int capacity)
		{
			this.Capacity = capacity;
			this.Current = 0;
			this.Write = 0;
			this.Read = 0;
		}

		internal void Reset()
		{
			this.Current = 0;
			this.Write = 0;
			this.Read = 0;
		}

		internal int Distance(int from, int to)
		{
			int num = to - from;
			if (num >= 0)
			{
				return num;
			}
			return this.Capacity - math.abs(num);
		}

		internal int Available()
		{
			return this.Distance(this.Read, this.Current);
		}

		internal int Reserve(int count)
		{
			int num = this.Distance(this.Write, this.Read) - 1;
			int num2 = ((num < 0) ? (this.Capacity - 1) : num);
			count = ((math.abs(count) - num2 < 0) ? count : num2);
			this.Write = (this.Write + count) % this.Capacity;
			return count;
		}

		internal int Commit(int count)
		{
			int num = this.Distance(this.Current, this.Write);
			count = ((math.abs(count) - num < 0) ? count : num);
			this.Current = (this.Current + count) % this.Capacity;
			return count;
		}

		internal int Consume(int count)
		{
			int num = this.Distance(this.Read, this.Current);
			count = ((math.abs(count) - num < 0) ? count : num);
			this.Read = (this.Read + count) % this.Capacity;
			return count;
		}

		internal int Length
		{
			get
			{
				return this.Distance(this.Read, this.Write);
			}
		}

		internal readonly int Capacity;

		internal int Current;

		internal int Write;

		internal int Read;
	}
}
