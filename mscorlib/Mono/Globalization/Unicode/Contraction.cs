using System;

namespace Mono.Globalization.Unicode
{
	internal class Contraction
	{
		public Contraction(int index, char[] source, string replacement, byte[] sortkey)
		{
			this.Index = index;
			this.Source = source;
			this.Replacement = replacement;
			this.SortKey = sortkey;
		}

		public int Index;

		public readonly char[] Source;

		public readonly string Replacement;

		public readonly byte[] SortKey;
	}
}
