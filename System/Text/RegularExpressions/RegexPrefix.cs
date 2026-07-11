using System;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexPrefix
	{
		internal RegexPrefix(string prefix, bool ci)
		{
			this._prefix = prefix;
			this._caseInsensitive = ci;
		}

		internal string Prefix
		{
			get
			{
				return this._prefix;
			}
		}

		internal bool CaseInsensitive
		{
			get
			{
				return this._caseInsensitive;
			}
		}

		internal static RegexPrefix Empty
		{
			get
			{
				return RegexPrefix._empty;
			}
		}

		internal string _prefix;

		internal bool _caseInsensitive;

		internal static RegexPrefix _empty = new RegexPrefix(string.Empty, false);
	}
}
