using System;

namespace System.Diagnostics
{
	public struct CounterSample
	{
		public CounterSample(long rawValue, long baseValue, long counterFrequency, long systemFrequency, long timeStamp, long timeStamp100nSec, PerformanceCounterType counterType)
		{
			this = new CounterSample(rawValue, baseValue, counterFrequency, systemFrequency, timeStamp, timeStamp100nSec, counterType, 0L);
		}

		public CounterSample(long rawValue, long baseValue, long counterFrequency, long systemFrequency, long timeStamp, long timeStamp100nSec, PerformanceCounterType counterType, long counterTimeStamp)
		{
			this.rawValue = rawValue;
			this.baseValue = baseValue;
			this.counterFrequency = counterFrequency;
			this.systemFrequency = systemFrequency;
			this.timeStamp = timeStamp;
			this.timeStamp100nSec = timeStamp100nSec;
			this.counterType = counterType;
			this.counterTimeStamp = counterTimeStamp;
		}

		public long BaseValue
		{
			get
			{
				return this.baseValue;
			}
		}

		public long CounterFrequency
		{
			get
			{
				return this.counterFrequency;
			}
		}

		public long CounterTimeStamp
		{
			get
			{
				return this.counterTimeStamp;
			}
		}

		public PerformanceCounterType CounterType
		{
			get
			{
				return this.counterType;
			}
		}

		public long RawValue
		{
			get
			{
				return this.rawValue;
			}
		}

		public long SystemFrequency
		{
			get
			{
				return this.systemFrequency;
			}
		}

		public long TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
		}

		public long TimeStamp100nSec
		{
			get
			{
				return this.timeStamp100nSec;
			}
		}

		public static float Calculate(CounterSample counterSample)
		{
			return CounterSampleCalculator.ComputeCounterValue(counterSample);
		}

		public static float Calculate(CounterSample counterSample, CounterSample nextCounterSample)
		{
			return CounterSampleCalculator.ComputeCounterValue(counterSample, nextCounterSample);
		}

		public override bool Equals(object obj)
		{
			return obj is CounterSample && this.Equals((CounterSample)obj);
		}

		public bool Equals(CounterSample other)
		{
			return this.rawValue == other.rawValue && this.baseValue == other.counterFrequency && this.counterFrequency == other.counterFrequency && this.systemFrequency == other.systemFrequency && this.timeStamp == other.timeStamp && this.timeStamp100nSec == other.timeStamp100nSec && this.counterTimeStamp == other.counterTimeStamp && this.counterType == other.counterType;
		}

		public override int GetHashCode()
		{
			return (int)((this.rawValue << 28) ^ ((this.baseValue << 24) ^ ((this.counterFrequency << 20) ^ ((this.systemFrequency << 16) ^ ((this.timeStamp << 8) ^ ((this.timeStamp100nSec << 4) ^ (this.counterTimeStamp ^ (long)this.counterType)))))));
		}

		public static bool operator ==(CounterSample obj1, CounterSample obj2)
		{
			return obj1.Equals(obj2);
		}

		public static bool operator !=(CounterSample obj1, CounterSample obj2)
		{
			return !obj1.Equals(obj2);
		}

		private long rawValue;

		private long baseValue;

		private long counterFrequency;

		private long systemFrequency;

		private long timeStamp;

		private long timeStamp100nSec;

		private long counterTimeStamp;

		private PerformanceCounterType counterType;

		public static CounterSample Empty = new CounterSample(0L, 0L, 0L, 0L, 0L, 0L, PerformanceCounterType.NumberOfItems32, 0L);
	}
}
