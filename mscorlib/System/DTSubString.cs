using System;

namespace System
{
	internal ref struct DTSubString
	{
		internal unsafe char this[int relativeIndex]
		{
			get
			{
				return (char)(*this.s[this.index + relativeIndex]);
			}
		}

		internal ReadOnlySpan<char> s;

		internal int index;

		internal int length;

		internal DTSubStringType type;

		internal int value;
	}
}
