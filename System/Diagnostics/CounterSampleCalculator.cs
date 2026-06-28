using System;

namespace System.Diagnostics
{
	public static class CounterSampleCalculator
	{
		public static float ComputeCounterValue(CounterSample newSample)
		{
			PerformanceCounterType counterType = newSample.CounterType;
			if (counterType != PerformanceCounterType.NumberOfItemsHEX32 && counterType != PerformanceCounterType.NumberOfItemsHEX64 && counterType != PerformanceCounterType.NumberOfItems32 && counterType != PerformanceCounterType.NumberOfItems64 && counterType != PerformanceCounterType.RawFraction)
			{
				return 0f;
			}
			return (float)newSample.RawValue;
		}

		[global::System.MonoTODO("What's the algorithm?")]
		public static float ComputeCounterValue(CounterSample oldSample, CounterSample newSample)
		{
			if (newSample.CounterType != oldSample.CounterType)
			{
				throw new Exception("The counter samples must be of the same type");
			}
			PerformanceCounterType counterType = newSample.CounterType;
			if (counterType != PerformanceCounterType.NumberOfItemsHEX32 && counterType != PerformanceCounterType.NumberOfItemsHEX64 && counterType != PerformanceCounterType.NumberOfItems32 && counterType != PerformanceCounterType.NumberOfItems64)
			{
				if (counterType != PerformanceCounterType.CounterDelta32 && counterType != PerformanceCounterType.CounterDelta64)
				{
					if (counterType != PerformanceCounterType.CountPerTimeInterval32 && counterType != PerformanceCounterType.CountPerTimeInterval64)
					{
						if (counterType == PerformanceCounterType.RateOfCountsPerSecond32 || counterType == PerformanceCounterType.RateOfCountsPerSecond64)
						{
							return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp) * 10000000f;
						}
						if (counterType == PerformanceCounterType.RawFraction)
						{
							goto IL_0118;
						}
						if (counterType != PerformanceCounterType.CounterTimer)
						{
							if (counterType == PerformanceCounterType.Timer100Ns)
							{
								return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp) * 100f;
							}
							if (counterType == PerformanceCounterType.CounterTimerInverse)
							{
								return (1f - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec)) * 100f;
							}
							if (counterType == PerformanceCounterType.Timer100NsInverse)
							{
								return (1f - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp)) * 100f;
							}
							if (counterType == PerformanceCounterType.CounterMultiTimer)
							{
								return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp) * 100f / (float)newSample.BaseValue;
							}
							if (counterType == PerformanceCounterType.CounterMultiTimer100Ns)
							{
								return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec) * 100f / (float)newSample.BaseValue;
							}
							if (counterType == PerformanceCounterType.CounterMultiTimerInverse)
							{
								return ((float)newSample.BaseValue - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp)) * 100f;
							}
							if (counterType == PerformanceCounterType.CounterMultiTimer100NsInverse)
							{
								return ((float)newSample.BaseValue - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec)) * 100f;
							}
							if (counterType == PerformanceCounterType.AverageTimer32)
							{
								return (float)(newSample.RawValue - oldSample.RawValue) / (float)newSample.SystemFrequency / (float)(newSample.BaseValue - oldSample.BaseValue);
							}
							if (counterType == PerformanceCounterType.ElapsedTime)
							{
								return 0f;
							}
							if (counterType != PerformanceCounterType.AverageCount64)
							{
								Console.WriteLine("Counter type {0} not handled", newSample.CounterType);
								return 0f;
							}
							return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.BaseValue - oldSample.BaseValue);
						}
					}
					return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp);
				}
				return (float)(newSample.RawValue - oldSample.RawValue);
			}
			IL_0118:
			return (float)newSample.RawValue;
		}
	}
}
