using System;

namespace System.Threading.Tasks
{
	[Flags]
	internal enum InternalTaskOptions
	{
		None = 0,
		InternalOptionsMask = 65280,
		ContinuationTask = 512,
		PromiseTask = 1024,
		LazyCancellation = 4096,
		QueuedByRuntime = 8192,
		DoNotDispose = 16384
	}
}
