using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Serializable]
	public class CaseInsensitiveComparer : IComparer
	{
		public CaseInsensitiveComparer()
		{
			this.culture = CultureInfo.CurrentCulture;
		}

		private CaseInsensitiveComparer(bool invariant)
		{
		}

		public CaseInsensitiveComparer(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID != CultureInfo.InvariantCulture.LCID)
			{
				this.culture = culture;
			}
		}

		public static CaseInsensitiveComparer Default
		{
			get
			{
				return CaseInsensitiveComparer.defaultComparer;
			}
		}

		public static CaseInsensitiveComparer DefaultInvariant
		{
			get
			{
				return CaseInsensitiveComparer.defaultInvariantComparer;
			}
		}

		public int Compare(object a, object b)
		{
			string text = a as string;
			string text2 = b as string;
			if (text == null || text2 == null)
			{
				return Comparer.Default.Compare(a, b);
			}
			if (this.culture != null)
			{
				return this.culture.CompareInfo.Compare(text, text2, CompareOptions.IgnoreCase);
			}
			return CultureInfo.InvariantCulture.CompareInfo.Compare(text, text2, CompareOptions.IgnoreCase);
		}

		private static CaseInsensitiveComparer defaultComparer = new CaseInsensitiveComparer();

		private static CaseInsensitiveComparer defaultInvariantComparer = new CaseInsensitiveComparer(true);

		private CultureInfo culture;
	}
}
