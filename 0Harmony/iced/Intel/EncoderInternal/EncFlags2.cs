using System;

namespace Iced.Intel.EncoderInternal
{
	[Flags]
	internal enum EncFlags2 : uint
	{
		None = 0U,
		OpCodeShift = 0U,
		OpCodeIs2Bytes = 65536U,
		TableShift = 17U,
		TableMask = 7U,
		MandatoryPrefixShift = 20U,
		MandatoryPrefixMask = 3U,
		WBitShift = 22U,
		WBitMask = 3U,
		LBitShift = 24U,
		LBitMask = 7U,
		GroupIndexShift = 27U,
		GroupIndexMask = 7U,
		HasMandatoryPrefix = 1073741824U,
		HasGroupIndex = 2147483648U
	}
}
