using System;

namespace Iced.Intel.EncoderInternal
{
	[Flags]
	internal enum EncFlags1 : uint
	{
		None = 0U,
		Legacy_OpMask = 127U,
		Legacy_Op0Shift = 0U,
		Legacy_Op1Shift = 7U,
		Legacy_Op2Shift = 14U,
		Legacy_Op3Shift = 21U,
		VEX_OpMask = 63U,
		VEX_Op0Shift = 0U,
		VEX_Op1Shift = 6U,
		VEX_Op2Shift = 12U,
		VEX_Op3Shift = 18U,
		VEX_Op4Shift = 24U,
		XOP_OpMask = 31U,
		XOP_Op0Shift = 0U,
		XOP_Op1Shift = 5U,
		XOP_Op2Shift = 10U,
		XOP_Op3Shift = 15U,
		EVEX_OpMask = 31U,
		EVEX_Op0Shift = 0U,
		EVEX_Op1Shift = 5U,
		EVEX_Op2Shift = 10U,
		EVEX_Op3Shift = 15U,
		MVEX_OpMask = 15U,
		MVEX_Op0Shift = 0U,
		MVEX_Op1Shift = 4U,
		MVEX_Op2Shift = 8U,
		MVEX_Op3Shift = 12U,
		IgnoresRoundingControl = 1073741824U,
		AmdLockRegBit = 2147483648U
	}
}
