using System;

namespace MonoMod.Logs
{
	internal interface IDebugFormattable
	{
		bool TryFormatInto(global::System.Span<char> span, out int wrote);
	}
}
