using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldTrimAttribute : Attribute
	{
		public char[] TrimChars { get; private set; }

		public TrimMode TrimMode { get; private set; }

		public FieldTrimAttribute(TrimMode mode)
			: this(mode, FieldTrimAttribute.WhitespaceChars)
		{
		}

		public FieldTrimAttribute(TrimMode mode, params char[] chars)
		{
			this.TrimMode = mode;
			Array.Sort<char>(chars);
			this.TrimChars = chars;
		}

		public FieldTrimAttribute(TrimMode mode, string trimChars)
			: this(mode, trimChars.ToCharArray())
		{
		}

		private static readonly char[] WhitespaceChars = new char[]
		{
			'\t', '\n', '\v', '\f', '\r', ' ', '\u00a0', '\u2000', '\u2001', '\u2002',
			'\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200a', '\u200b', '\u3000',
			'\ufeff'
		};
	}
}
