using System;

namespace Mono.Globalization.Unicode
{
	internal class Contraction
	{
		public Contraction(char[] source, string replacement, byte[] sortkey)
		{
			this.Source = source;
			this.Replacement = replacement;
			this.SortKey = sortkey;
		}

		public readonly char[] Source;

		public readonly string Replacement;

		public readonly byte[] SortKey;
	}
}
