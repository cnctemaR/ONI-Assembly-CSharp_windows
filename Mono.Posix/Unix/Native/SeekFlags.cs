using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum SeekFlags : short
	{
		SEEK_SET,
		SEEK_CUR,
		SEEK_END,
		L_SET = 0,
		L_INCR,
		L_XTND
	}
}
