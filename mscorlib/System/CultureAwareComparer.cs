using System;
using System.Globalization;

namespace System
{
	[Serializable]
	internal sealed class CultureAwareComparer : StringComparer
	{
		public CultureAwareComparer(CultureInfo ci, bool ignore_case)
		{
			this._compareInfo = ci.CompareInfo;
			this._ignoreCase = ignore_case;
		}

		public override int Compare(string x, string y)
		{
			CompareOptions compareOptions = ((!this._ignoreCase) ? CompareOptions.None : CompareOptions.IgnoreCase);
			return this._compareInfo.Compare(x, y, compareOptions);
		}

		public override bool Equals(string x, string y)
		{
			return this.Compare(x, y) == 0;
		}

		public override int GetHashCode(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			CompareOptions compareOptions = ((!this._ignoreCase) ? CompareOptions.None : CompareOptions.IgnoreCase);
			return this._compareInfo.GetSortKey(s, compareOptions).GetHashCode();
		}

		private readonly bool _ignoreCase;

		private readonly CompareInfo _compareInfo;
	}
}
