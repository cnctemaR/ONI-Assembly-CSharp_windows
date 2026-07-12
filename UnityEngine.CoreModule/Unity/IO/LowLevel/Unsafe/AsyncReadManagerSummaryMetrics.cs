using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe
{
	[NativeConditional("ENABLE_PROFILER")]
	[NativeAsStruct]
	[StructLayout(LayoutKind.Sequential)]
	public class AsyncReadManagerSummaryMetrics
	{
		[NativeName("totalBytesRead")]
		public ulong TotalBytesRead { get; }

		[NativeName("averageBandwidthMBPerSecond")]
		public float AverageBandwidthMBPerSecond { get; }

		[NativeName("averageReadSizeInBytes")]
		public float AverageReadSizeInBytes { get; }

		[NativeName("averageWaitTimeMicroseconds")]
		public float AverageWaitTimeMicroseconds { get; }

		[NativeName("averageReadTimeMicroseconds")]
		public float AverageReadTimeMicroseconds { get; }

		[NativeName("averageTotalRequestTimeMicroseconds")]
		public float AverageTotalRequestTimeMicroseconds { get; }

		[NativeName("averageThroughputMBPerSecond")]
		public float AverageThroughputMBPerSecond { get; }

		[NativeName("longestWaitTimeMicroseconds")]
		public float LongestWaitTimeMicroseconds { get; }

		[NativeName("longestReadTimeMicroseconds")]
		public float LongestReadTimeMicroseconds { get; }

		[NativeName("longestReadAssetType")]
		public ulong LongestReadAssetType { get; }

		[NativeName("longestWaitAssetType")]
		public ulong LongestWaitAssetType { get; }

		[NativeName("longestReadSubsystem")]
		public AssetLoadingSubsystem LongestReadSubsystem { get; }

		[NativeName("longestWaitSubsystem")]
		public AssetLoadingSubsystem LongestWaitSubsystem { get; }

		[NativeName("numberOfInProgressRequests")]
		public int NumberOfInProgressRequests { get; }

		[NativeName("numberOfCompletedRequests")]
		public int NumberOfCompletedRequests { get; }

		[NativeName("numberOfFailedRequests")]
		public int NumberOfFailedRequests { get; }

		[NativeName("numberOfWaitingRequests")]
		public int NumberOfWaitingRequests { get; }

		[NativeName("numberOfCanceledRequests")]
		public int NumberOfCanceledRequests { get; }

		[NativeName("totalNumberOfRequests")]
		public int TotalNumberOfRequests { get; }

		[NativeName("numberOfCachedReads")]
		public int NumberOfCachedReads { get; }

		[NativeName("numberOfAsyncReads")]
		public int NumberOfAsyncReads { get; }

		[NativeName("numberOfSyncReads")]
		public int NumberOfSyncReads { get; }
	}
}
