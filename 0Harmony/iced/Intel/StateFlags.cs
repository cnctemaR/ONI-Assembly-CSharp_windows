using System;

namespace Iced.Intel
{
	[Flags]
	internal enum StateFlags : uint
	{
		IpRel64 = 1U,
		IpRel32 = 2U,
		HasRex = 8U,
		b = 16U,
		z = 32U,
		IsInvalid = 64U,
		W = 128U,
		NoImm = 256U,
		Addr64 = 512U,
		BranchImm8 = 1024U,
		Xbegin = 2048U,
		Lock = 4096U,
		AllowLock = 8192U,
		NoMoreBytes = 16384U,
		Has66 = 32768U,
		MvexSssMask = 7U,
		MvexSssShift = 16U,
		MvexEH = 524288U,
		EncodingMask = 7U,
		EncodingShift = 29U
	}
}
