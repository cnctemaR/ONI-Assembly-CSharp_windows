using System;

namespace System.Diagnostics
{
	public enum ThreadWaitReason
	{
		EventPairHigh = 7,
		EventPairLow,
		ExecutionDelay = 4,
		Executive = 0,
		FreePage,
		LpcReceive = 9,
		LpcReply,
		PageIn = 2,
		PageOut = 12,
		Suspended = 5,
		SystemAllocation = 3,
		Unknown = 13,
		UserRequest = 6,
		VirtualMemory = 11
	}
}
