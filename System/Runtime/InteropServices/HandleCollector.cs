using System;

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
				throw new ArgumentOutOfRangeException("initialThreshold", "initialThreshold must not be less than zero");
			}
			if (maximumThreshold < 0)
			{
				throw new ArgumentOutOfRangeException("maximumThreshold", "maximumThreshold must not be less than zero");
			}
			if (maximumThreshold < initialThreshold)
			{
				throw new ArgumentException("maximumThreshold must not be less than initialThreshold");
			}
			this.name = name;
			this.init = initialThreshold;
			this.max = maximumThreshold;
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public int InitialThreshold
		{
			get
			{
				return this.init;
			}
		}

		public int MaximumThreshold
		{
			get
			{
				return this.max;
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
			if (++this.count >= this.max)
			{
				GC.Collect(GC.MaxGeneration);
			}
			else if (this.count >= this.init && DateTime.Now - this.previous_collection > TimeSpan.FromSeconds(5.0))
			{
				GC.Collect(GC.MaxGeneration);
				this.previous_collection = DateTime.Now;
			}
		}

		public void Remove()
		{
			if (this.count == 0)
			{
				throw new InvalidOperationException("Cannot call Remove method when Count is 0");
			}
			this.count--;
		}

		private int count;

		private readonly int init;

		private readonly int max;

		private readonly string name;

		private DateTime previous_collection = DateTime.MinValue;
	}
}
