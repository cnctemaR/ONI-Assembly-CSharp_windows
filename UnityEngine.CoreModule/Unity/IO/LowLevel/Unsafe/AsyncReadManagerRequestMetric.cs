using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.IO.LowLevel.Unsafe
{
	[RequiredByNativeCode]
	[NativeConditional("ENABLE_PROFILER")]
	public struct AsyncReadManagerRequestMetric
	{
		[NativeName("assetName")]
		public readonly string AssetName { get; }

		[NativeName("fileName")]
		public readonly string FileName { get; }

		[NativeName("offsetBytes")]
		public readonly ulong OffsetBytes { get; }

		[NativeName("sizeBytes")]
		public readonly ulong SizeBytes { get; }

		[NativeName("assetTypeId")]
		public readonly ulong AssetTypeId { get; }

		[NativeName("currentBytesRead")]
		public readonly ulong CurrentBytesRead { get; }

		[NativeName("batchReadCount")]
		public readonly uint BatchReadCount { get; }

		[NativeName("isBatchRead")]
		public readonly bool IsBatchRead { get; }

		[NativeName("state")]
		public readonly ProcessingState State { get; }

		[NativeName("readType")]
		public readonly FileReadType ReadType { get; }

		[NativeName("priorityLevel")]
		public readonly Priority PriorityLevel { get; }

		[NativeName("subsystem")]
		public readonly AssetLoadingSubsystem Subsystem { get; }

		[NativeName("requestTimeMicroseconds")]
		public readonly double RequestTimeMicroseconds { get; }

		[NativeName("timeInQueueMicroseconds")]
		public readonly double TimeInQueueMicroseconds { get; }

		[NativeName("totalTimeMicroseconds")]
		public readonly double TotalTimeMicroseconds { get; }
	}
}
