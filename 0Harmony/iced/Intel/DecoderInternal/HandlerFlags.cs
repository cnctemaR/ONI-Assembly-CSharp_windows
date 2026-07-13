using System;

namespace Iced.Intel.DecoderInternal
{
	[Flags]
	internal enum HandlerFlags : uint
	{
		None = 0U,
		Xacquire = 1U,
		Xrelease = 2U,
		XacquireXreleaseNoLock = 4U,
		Lock = 8U
	}
}
