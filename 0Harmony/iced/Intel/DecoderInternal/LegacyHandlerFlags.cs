using System;

namespace Iced.Intel.DecoderInternal
{
	[Flags]
	internal enum LegacyHandlerFlags : uint
	{
		HandlerReg = 1U,
		HandlerMem = 2U,
		Handler66Reg = 4U,
		Handler66Mem = 8U,
		HandlerF3Reg = 16U,
		HandlerF3Mem = 32U,
		HandlerF2Reg = 64U,
		HandlerF2Mem = 128U
	}
}
