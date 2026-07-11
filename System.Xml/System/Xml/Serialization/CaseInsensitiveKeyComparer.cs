using System;
using System.Collections;
using System.Globalization;

namespace System.Xml.Serialization
{
	internal class CaseInsensitiveKeyComparer : CaseInsensitiveComparer, IEqualityComparer
	{
		public CaseInsensitiveKeyComparer()
			: base(CultureInfo.CurrentCulture)
		{
		}

		bool IEqualityComparer.Equals(object x, object y)
		{
			return base.Compare(x, y) == 0;
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			string text = obj as string;
			if (text == null)
			{
				throw new ArgumentException(null, "obj");
			}
			return text.ToUpper(CultureInfo.CurrentCulture).GetHashCode();
		}
	}
}
