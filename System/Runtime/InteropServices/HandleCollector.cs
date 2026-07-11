using System;
using System.Threading;

namespace System.Runtime.InteropServices
{
	public sealed class HandleCollector
	{
		public HandleCollector(string name, int initialThreshold)
			: this(name, initialThreshold, int.MaxValue)
		{
		}

		public HandleCollector(string name, int initialThreshold, int maximumThreshold)
		{
			if (initialThreshold < 0)
			{
				throw new ArgumentOutOfRangeException("initialThreshold", global::SR.GetString("Non-negative number required."));
			}
			if (maximumThreshold < 0)
			{
				throw new ArgumentOutOfRangeException("maximumThreshold", global::SR.GetString("Non-negative number required."));
			}
			if (initialThreshold > maximumThreshold)
			{
				throw new ArgumentException(global::SR.GetString("maximumThreshold cannot be less than initialThreshold."));
			}
			if (name != null)
			{
				this.name = name;
			}
			else
			{
				this.name = string.Empty;
			}
			this.initialThreshold = initialThreshold;
			this.maximumThreshold = maximumThreshold;
			this.threshold = initialThreshold;
			this.handleCount = 0;
		}

		public int Count
		{
			get
			{
				return this.handleCount;
			}
		}

		public int InitialThreshold
		{
			get
			{
				return this.initialThreshold;
			}
		}

		public int MaximumThreshold
		{
			get
			{
				return this.maximumThreshold;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public void Add()
		{
			int num = -1;
			Interlocked.Increment(ref this.handleCount);
			if (this.handleCount < 0)
			{
				throw new InvalidOperationException(global::SR.GetString("Handle collector count overflows or underflows."));
			}
			if (this.handleCount > this.threshold)
			{
				lock (this)
				{
					this.threshold = this.handleCount + this.handleCount / 10;
					num = this.gc_gen;
					if (this.gc_gen < 2)
					{
						this.gc_gen++;
					}
				}
			}
			if (num >= 0 && (num == 0 || this.gc_counts[num] == GC.CollectionCount(num)))
			{
				GC.Collect(num);
				Thread.Sleep(10 * num);
			}
			for (int i = 1; i < 3; i++)
			{
				this.gc_counts[i] = GC.CollectionCount(i);
			}
		}

		public void Remove()
		{
			Interlocked.Decrement(ref this.handleCount);
			if (this.handleCount < 0)
			{
				throw new InvalidOperationException(global::SR.GetString("Handle collector count overflows or underflows."));
			}
			int num = this.handleCount + this.handleCount / 10;
			if (num < this.threshold - this.threshold / 10)
			{
				lock (this)
				{
					if (num > this.initialThreshold)
					{
						this.threshold = num;
					}
					else
					{
						this.threshold = this.initialThreshold;
					}
					this.gc_gen = 0;
				}
			}
			for (int i = 1; i < 3; i++)
			{
				this.gc_counts[i] = GC.CollectionCount(i);
			}
		}

		private const int deltaPercent = 10;

		private string name;

		private int initialThreshold;

		private int maximumThreshold;

		private int threshold;

		private int handleCount;

		private int[] gc_counts = new int[3];

		private int gc_gen;
	}
}
