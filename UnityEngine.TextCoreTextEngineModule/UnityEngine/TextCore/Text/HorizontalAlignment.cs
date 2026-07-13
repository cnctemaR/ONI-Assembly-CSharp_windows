using System;

namespace UnityEngine.TextCore.Text
{
	[Flags]
	internal enum HorizontalAlignment
	{
		Left = 1,
		Center = 2,
		Right = 4,
		Justified = 8,
		Flush = 16,
		Geometry = 32
	}
}
