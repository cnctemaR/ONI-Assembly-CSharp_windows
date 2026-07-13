using System;

namespace Iced.Intel.EncoderInternal
{
	[Flags]
	internal enum EncFlags3 : uint
	{
		None = 0U,
		EncodingShift = 0U,
		EncodingMask = 7U,
		OperandSizeShift = 3U,
		OperandSizeMask = 3U,
		AddressSizeShift = 5U,
		AddressSizeMask = 3U,
		TupleTypeShift = 7U,
		TupleTypeMask = 31U,
		DefaultOpSize64 = 4096U,
		HasRmGroupIndex = 8192U,
		IntelForceOpSize64 = 16384U,
		Fwait = 32768U,
		Bit16or32 = 65536U,
		Bit64 = 131072U,
		Lock = 262144U,
		Xacquire = 524288U,
		Xrelease = 1048576U,
		Rep = 2097152U,
		Repne = 4194304U,
		Bnd = 8388608U,
		HintTaken = 16777216U,
		Notrack = 33554432U,
		Broadcast = 67108864U,
		RoundingControl = 134217728U,
		SuppressAllExceptions = 268435456U,
		OpMaskRegister = 536870912U,
		ZeroingMasking = 1073741824U,
		RequireOpMaskRegister = 2147483648U
	}
}
