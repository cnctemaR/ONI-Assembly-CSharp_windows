using System;

namespace System.Text.RegularExpressions
{
	internal readonly struct RegexPrefix
	{
		internal RegexPrefix(string prefix, bool ci)
		{
			this.Prefix = prefix;
			this.CaseInsensitive = ci;
		}

		internal bool CaseInsensitive { get; }

		internal static RegexPrefix Empty { get; } = new RegexPrefix(string.Empty, false);

		internal string Prefix { get; }
	}
}
