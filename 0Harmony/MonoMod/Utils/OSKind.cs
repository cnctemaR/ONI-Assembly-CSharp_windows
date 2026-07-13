using System;

namespace MonoMod.Utils
{
	internal enum OSKind
	{
		Unknown,
		Posix,
		Linux = 9,
		Android = 41,
		OSX = 5,
		IOS = 37,
		BSD = 17,
		Windows = 2,
		Wine = 34
	}
}
