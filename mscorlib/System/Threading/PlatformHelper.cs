using System;

namespace System.Threading
{
	internal static class PlatformHelper
	{
		internal static int ProcessorCount
		{
			get
			{
				int tickCount = Environment.TickCount;
				int num = PlatformHelper.s_processorCount;
				if (num == 0 || tickCount - PlatformHelper.s_lastProcessorCountRefreshTicks >= 30000)
				{
					num = (PlatformHelper.s_processorCount = Environment.ProcessorCount);
					PlatformHelper.s_lastProcessorCountRefreshTicks = tickCount;
				}
				return num;
			}
		}

		private const int PROCESSOR_COUNT_REFRESH_INTERVAL_MS = 30000;

		private static volatile int s_processorCount;

		private static volatile int s_lastProcessorCountRefreshTicks;

		internal static readonly bool IsSingleProcessor = PlatformHelper.ProcessorCount == 1;
	}
}
