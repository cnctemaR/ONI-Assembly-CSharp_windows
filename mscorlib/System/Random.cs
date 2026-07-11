using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class Random
	{
		public Random()
			: this(Environment.TickCount)
		{
		}

		public Random(int Seed)
		{
			int num = 161803398 - Math.Abs(Seed);
			this.SeedArray[55] = num;
			int num2 = 1;
			for (int i = 1; i < 55; i++)
			{
				int num3 = 21 * i % 55;
				this.SeedArray[num3] = num2;
				num2 = num - num2;
				if (num2 < 0)
				{
					num2 += int.MaxValue;
				}
				num = this.SeedArray[num3];
			}
			for (int j = 1; j < 5; j++)
			{
				for (int k = 1; k < 56; k++)
				{
					this.SeedArray[k] -= this.SeedArray[1 + (k + 30) % 55];
					if (this.SeedArray[k] < 0)
					{
						this.SeedArray[k] += int.MaxValue;
					}
				}
			}
			this.inext = 0;
			this.inextp = 31;
		}

		protected virtual double Sample()
		{
			if (++this.inext >= 56)
			{
				this.inext = 1;
			}
			if (++this.inextp >= 56)
			{
				this.inextp = 1;
			}
			int num = this.SeedArray[this.inext] - this.SeedArray[this.inextp];
			if (num < 0)
			{
				num += int.MaxValue;
			}
			this.SeedArray[this.inext] = num;
			return (double)num * 4.656612875245797E-10;
		}

		public virtual int Next()
		{
			return (int)(this.Sample() * 2147483647.0);
		}

		public virtual int Next(int maxValue)
		{
			if (maxValue < 0)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("Max value is less than min value."));
			}
			return (int)(this.Sample() * (double)maxValue);
		}

		public virtual int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("Min value is greater than max value."));
			}
			uint num = (uint)(maxValue - minValue);
			if (num <= 1U)
			{
				return minValue;
			}
			return (int)((ulong)((uint)(this.Sample() * num)) + (ulong)((long)minValue));
		}

		public virtual void NextBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = (byte)(this.Sample() * 256.0);
			}
		}

		public virtual double NextDouble()
		{
			return this.Sample();
		}

		private const int MBIG = 2147483647;

		private const int MSEED = 161803398;

		private const int MZ = 0;

		private int inext;

		private int inextp;

		private int[] SeedArray = new int[56];
	}
}
