using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class BottleneckUtil
	{
		public static PerformanceBottleneck DetermineBottleneck(PerformanceBottleneck prevBottleneck, float averageCpuFrameTime, float averageGpuFrametime, float averageOverallFrametime, float targetFrameTime)
		{
			bool flag = BottleneckUtil.HittingFrameRateLimit(averageOverallFrametime, (prevBottleneck == PerformanceBottleneck.TargetFrameRate) ? 0.03f : 0.02f, targetFrameTime);
			PerformanceBottleneck performanceBottleneck;
			if (flag)
			{
				performanceBottleneck = PerformanceBottleneck.TargetFrameRate;
			}
			else
			{
				bool flag2 = averageGpuFrametime >= averageOverallFrametime;
				if (flag2)
				{
					performanceBottleneck = PerformanceBottleneck.GPU;
				}
				else
				{
					bool flag3 = averageCpuFrameTime >= averageOverallFrametime;
					if (flag3)
					{
						performanceBottleneck = PerformanceBottleneck.CPU;
					}
					else
					{
						bool flag4 = prevBottleneck == PerformanceBottleneck.GPU;
						bool flag5 = prevBottleneck == PerformanceBottleneck.CPU;
						float num = averageGpuFrametime / averageOverallFrametime;
						float num2 = averageCpuFrameTime / averageOverallFrametime;
						float num3 = (flag5 ? 0.87f : 0.9f);
						bool flag6 = num2 > num3;
						if (flag6)
						{
							performanceBottleneck = PerformanceBottleneck.CPU;
						}
						else
						{
							float num4 = (flag4 ? 0.87f : 0.9f);
							bool flag7 = averageGpuFrametime > num4;
							if (flag7)
							{
								performanceBottleneck = PerformanceBottleneck.GPU;
							}
							else
							{
								bool flag8 = averageGpuFrametime > averageCpuFrameTime;
								if (flag8)
								{
									float num5 = (flag4 ? 0.9f : 0.92f);
									bool flag9 = num > num5;
									if (flag9)
									{
										float num6 = (flag4 ? 0.92f : 0.9f);
										bool flag10 = averageGpuFrametime * num6 > averageCpuFrameTime;
										if (flag10)
										{
											return PerformanceBottleneck.GPU;
										}
									}
								}
								else
								{
									float num7 = (flag5 ? 0.5f : 0.52f);
									bool flag11 = num2 > num7 && averageGpuFrametime < averageCpuFrameTime;
									if (flag11)
									{
										float num8 = (flag5 ? 0.85f : 0.8f);
										bool flag12 = averageCpuFrameTime * num8 > averageGpuFrametime;
										if (flag12)
										{
											return PerformanceBottleneck.CPU;
										}
									}
								}
								performanceBottleneck = PerformanceBottleneck.Unknown;
							}
						}
					}
				}
			}
			return performanceBottleneck;
		}

		private static bool HittingFrameRateLimit(float actualFrameTime, float thresholdFactor, float targetFrameTime)
		{
			bool flag = targetFrameTime <= 0f;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = actualFrameTime <= targetFrameTime;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = actualFrameTime - targetFrameTime < thresholdFactor * targetFrameTime;
					flag2 = flag4;
				}
			}
			return flag2;
		}
	}
}
