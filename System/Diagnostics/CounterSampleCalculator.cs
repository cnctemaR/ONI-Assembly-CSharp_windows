using System;

namespace System.Diagnostics
{
	public static class CounterSampleCalculator
	{
		public static float ComputeCounterValue(CounterSample newSample)
		{
			PerformanceCounterType counterType = newSample.CounterType;
			if (counterType <= PerformanceCounterType.NumberOfItemsHEX64)
			{
				if (counterType != PerformanceCounterType.NumberOfItemsHEX32 && counterType != PerformanceCounterType.NumberOfItemsHEX64)
				{
					goto IL_003E;
				}
			}
			else if (counterType != PerformanceCounterType.NumberOfItems32 && counterType != PerformanceCounterType.NumberOfItems64 && counterType != PerformanceCounterType.RawFraction)
			{
				goto IL_003E;
			}
			return (float)newSample.RawValue;
			IL_003E:
			return 0f;
		}

		[MonoTODO("What's the algorithm?")]
		public static float ComputeCounterValue(CounterSample oldSample, CounterSample newSample)
		{
			if (newSample.CounterType != oldSample.CounterType)
			{
				throw new Exception("The counter samples must be of the same type");
			}
			PerformanceCounterType counterType = newSample.CounterType;
			if (counterType <= PerformanceCounterType.RawFraction)
			{
				if (counterType <= PerformanceCounterType.CounterDelta32)
				{
					if (counterType <= PerformanceCounterType.NumberOfItemsHEX64)
					{
						if (counterType != PerformanceCounterType.NumberOfItemsHEX32 && counterType != PerformanceCounterType.NumberOfItemsHEX64)
						{
							goto IL_036C;
						}
					}
					else if (counterType != PerformanceCounterType.NumberOfItems32 && counterType != PerformanceCounterType.NumberOfItems64)
					{
						if (counterType != PerformanceCounterType.CounterDelta32)
						{
							goto IL_036C;
						}
						goto IL_01C3;
					}
				}
				else if (counterType <= PerformanceCounterType.CountPerTimeInterval64)
				{
					if (counterType == PerformanceCounterType.CounterDelta64)
					{
						goto IL_01C3;
					}
					if (counterType != PerformanceCounterType.CountPerTimeInterval32 && counterType != PerformanceCounterType.CountPerTimeInterval64)
					{
						goto IL_036C;
					}
					goto IL_0298;
				}
				else
				{
					if (counterType == PerformanceCounterType.RateOfCountsPerSecond32 || counterType == PerformanceCounterType.RateOfCountsPerSecond64)
					{
						return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp) * 10000000f;
					}
					if (counterType != PerformanceCounterType.RawFraction)
					{
						goto IL_036C;
					}
				}
				return (float)newSample.RawValue;
				IL_01C3:
				return (float)(newSample.RawValue - oldSample.RawValue);
			}
			if (counterType <= PerformanceCounterType.CounterMultiTimer)
			{
				if (counterType <= PerformanceCounterType.Timer100Ns)
				{
					if (counterType != PerformanceCounterType.CounterTimer)
					{
						if (counterType != PerformanceCounterType.Timer100Ns)
						{
							goto IL_036C;
						}
						return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp) * 100f;
					}
				}
				else
				{
					if (counterType == PerformanceCounterType.CounterTimerInverse)
					{
						return (1f - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec)) * 100f;
					}
					if (counterType == PerformanceCounterType.Timer100NsInverse)
					{
						return (1f - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp)) * 100f;
					}
					if (counterType != PerformanceCounterType.CounterMultiTimer)
					{
						goto IL_036C;
					}
					return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp) * 100f / (float)newSample.BaseValue;
				}
			}
			else if (counterType <= PerformanceCounterType.CounterMultiTimer100NsInverse)
			{
				if (counterType == PerformanceCounterType.CounterMultiTimer100Ns)
				{
					return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec) * 100f / (float)newSample.BaseValue;
				}
				if (counterType == PerformanceCounterType.CounterMultiTimerInverse)
				{
					return ((float)newSample.BaseValue - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp)) * 100f;
				}
				if (counterType != PerformanceCounterType.CounterMultiTimer100NsInverse)
				{
					goto IL_036C;
				}
				return ((float)newSample.BaseValue - (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec)) * 100f;
			}
			else
			{
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
					goto IL_036C;
				}
				return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.BaseValue - oldSample.BaseValue);
			}
			IL_0298:
			return (float)(newSample.RawValue - oldSample.RawValue) / (float)(newSample.TimeStamp - oldSample.TimeStamp);
			IL_036C:
			Console.WriteLine("Counter type {0} not handled", newSample.CounterType);
			return 0f;
		}
	}
}
