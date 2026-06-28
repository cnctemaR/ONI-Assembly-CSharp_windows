using System;

namespace FMOD
{
	[Flags]
	public enum TIMEUNIT : uint
	{
		MS = 1U,
		PCM = 2U,
		PCMBYTES = 4U,
		RAWBYTES = 8U,
		PCMFRACTION = 16U,
		MODORDER = 256U,
		MODROW = 512U,
		MODPATTERN = 1024U,
		BUFFERED = 268435456U
	}
}
