using System;

namespace System.Collections.Generic
{
	[Serializable]
	internal sealed class NonRandomizedStringEqualityComparer : EqualityComparer<string>
	{
		internal new static IEqualityComparer<string> Default
		{
			get
			{
				IEqualityComparer<string> equalityComparer;
				if ((equalityComparer = NonRandomizedStringEqualityComparer.s_nonRandomizedComparer) == null)
				{
					equalityComparer = (NonRandomizedStringEqualityComparer.s_nonRandomizedComparer = new NonRandomizedStringEqualityComparer());
				}
				return equalityComparer;
			}
		}

		public sealed override bool Equals(string x, string y)
		{
			return string.Equals(x, y);
		}

		public sealed override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetLegacyNonRandomizedHashCode();
		}

		private static volatile IEqualityComparer<string> s_nonRandomizedComparer;
	}
}
