using System;
using System.Security;

namespace System.Threading
{
	internal static class ThreadPoolGlobals
	{
		public static bool tpHosted
		{
			get
			{
				return ThreadPool.IsThreadPoolHosted();
			}
		}

		public const uint tpQuantum = 30U;

		public static int processorCount = Environment.ProcessorCount;

		public static volatile bool vmTpInitialized;

		public static bool enableWorkerTracking;

		[SecurityCritical]
		public static readonly ThreadPoolWorkQueue workQueue = new ThreadPoolWorkQueue();
	}
}
