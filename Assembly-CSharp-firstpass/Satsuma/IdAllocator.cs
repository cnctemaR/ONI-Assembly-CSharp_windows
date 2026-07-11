using System;

namespace Satsuma
{
	internal abstract class IdAllocator
	{
		public IdAllocator()
		{
			this.randomSeed = 205891132094649L;
			this.Rewind();
		}

		private long Random()
		{
			return this.randomSeed *= 3L;
		}

		protected abstract bool IsAllocated(long id);

		public void Rewind()
		{
			this.lastAllocated = 0L;
		}

		public long Allocate()
		{
			long num = this.lastAllocated + 1L;
			int num2 = 0;
			for (;;)
			{
				if (num == 0L)
				{
					num = 1L;
				}
				if (!this.IsAllocated(num))
				{
					break;
				}
				num += 1L;
				num2++;
				if (num2 >= 100)
				{
					num = this.Random();
					num2 = 0;
				}
			}
			this.lastAllocated = num;
			return num;
		}

		private long randomSeed;

		private long lastAllocated;
	}
}
