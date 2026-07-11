using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	internal sealed class CultureAwareComparer : StringComparer
	{
		internal CultureAwareComparer(CultureInfo culture, bool ignoreCase)
		{
			this._compareInfo = culture.CompareInfo;
			this._ignoreCase = ignoreCase;
			this._options = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		internal CultureAwareComparer(CompareInfo compareInfo, bool ignoreCase)
		{
			this._compareInfo = compareInfo;
			this._ignoreCase = ignoreCase;
			this._options = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		internal CultureAwareComparer(CompareInfo compareInfo, CompareOptions options)
		{
			this._compareInfo = compareInfo;
			this._options = options;
			this._ignoreCase = (options & CompareOptions.IgnoreCase) == CompareOptions.IgnoreCase || (options & CompareOptions.OrdinalIgnoreCase) == CompareOptions.OrdinalIgnoreCase;
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
			return this._compareInfo.Compare(x, y, this._options);
		}

		public override bool Equals(string x, string y)
		{
			return x == y || (x != null && y != null && this._compareInfo.Compare(x, y, this._options) == 0);
		}

		public override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return this._compareInfo.GetHashCodeOfString(obj, this._options);
		}

		public override bool Equals(object obj)
		{
			CultureAwareComparer cultureAwareComparer = obj as CultureAwareComparer;
			return cultureAwareComparer != null && this._ignoreCase == cultureAwareComparer._ignoreCase && this._compareInfo.Equals(cultureAwareComparer._compareInfo) && this._options == cultureAwareComparer._options;
		}

		public override int GetHashCode()
		{
			int hashCode = this._compareInfo.GetHashCode();
			if (!this._ignoreCase)
			{
				return hashCode;
			}
			return ~hashCode;
		}

		private CompareInfo _compareInfo;

		private bool _ignoreCase;

		[OptionalField]
		private CompareOptions _options;
	}
}
