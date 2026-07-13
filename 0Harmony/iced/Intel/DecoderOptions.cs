using System;

namespace Iced.Intel
{
	[Flags]
	internal enum DecoderOptions : uint
	{
		None = 0U,
		NoInvalidCheck = 1U,
		AMD = 2U,
		ForceReservedNop = 4U,
		Umov = 8U,
		Xbts = 16U,
		Cmpxchg486A = 32U,
		OldFpu = 64U,
		Pcommit = 128U,
		Loadall286 = 256U,
		Loadall386 = 512U,
		Cl1invmb = 1024U,
		MovTr = 2048U,
		Jmpe = 4096U,
		NoPause = 8192U,
		NoWbnoinvd = 16384U,
		Udbg = 32768U,
		NoMPFX_0FBC = 65536U,
		NoMPFX_0FBD = 131072U,
		NoLahfSahf64 = 262144U,
		MPX = 524288U,
		Cyrix = 1048576U,
		Cyrix_SMINT_0F7E = 2097152U,
		Cyrix_DMI = 4194304U,
		ALTINST = 8388608U,
		KNC = 16777216U
	}
}
