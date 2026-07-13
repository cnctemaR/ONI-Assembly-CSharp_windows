using System;

namespace Iced.Intel.EncoderInternal
{
	[Flags]
	internal enum EncoderFlags : uint
	{
		None = 0U,
		B = 1U,
		X = 2U,
		R = 4U,
		W = 8U,
		ModRM = 16U,
		Sib = 32U,
		REX = 64U,
		P66 = 128U,
		P67 = 256U,
		R2 = 512U,
		Broadcast = 1024U,
		HighLegacy8BitRegs = 2048U,
		Displ = 4096U,
		PF0 = 8192U,
		RegIsMemory = 16384U,
		MustUseSib = 32768U,
		VvvvvShift = 27U,
		VvvvvMask = 31U
	}
}
