using System;
using System.Globalization;

namespace System
{
	[Serializable]
	internal sealed class OrdinalComparer : StringComparer
	{
		internal OrdinalComparer(bool ignoreCase)
		{
			this._ignoreCase = ignoreCase;
		}

		public override int Compare(string x, string y)
		{
			if (x == y)
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			if (this._ignoreCase)
			{
				return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
			}
			return string.CompareOrdinal(x, y);
		}

		public override bool Equals(string x, string y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (this._ignoreCase)
			{
				return x.Length == y.Length && string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
			}
			return x.Equals(y);
		}

		public override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (this._ignoreCase)
			{
				return TextInfo.GetHashCodeOrdinalIgnoreCase(obj);
			}
			return obj.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			OrdinalComparer ordinalComparer = obj as OrdinalComparer;
			return ordinalComparer != null && this._ignoreCase == ordinalComparer._ignoreCase;
		}

		public override int GetHashCode()
		{
			int hashCode = "OrdinalComparer".GetHashCode();
			if (!this._ignoreCase)
			{
				return hashCode;
			}
			return ~hashCode;
		}

		private bool _ignoreCase;
	}
}
