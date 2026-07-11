using System;

namespace System.Net
{
	[Flags]
	internal enum ThreadKinds
	{
		Unknown = 0,
		User = 1,
		System = 2,
		Sync = 4,
		Async = 8,
		Timer = 16,
		CompletionPort = 32,
		Worker = 64,
		Finalization = 128,
		Other = 256,
		OwnerMask = 3,
		SyncMask = 12,
		SourceMask = 496,
		SafeSources = 352,
		ThreadPool = 96
	}
}
