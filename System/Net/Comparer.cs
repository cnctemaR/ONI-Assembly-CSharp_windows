using System;
using System.Collections;

namespace System.Net
{
	internal class Comparer : IComparer
	{
		int IComparer.Compare(object ol, object or)
		{
			Cookie cookie = (Cookie)ol;
			Cookie cookie2 = (Cookie)or;
			int num;
			if ((num = string.Compare(cookie.Name, cookie2.Name, StringComparison.OrdinalIgnoreCase)) != 0)
			{
				return num;
			}
			if ((num = string.Compare(cookie.Domain, cookie2.Domain, StringComparison.OrdinalIgnoreCase)) != 0)
			{
				return num;
			}
			if ((num = string.Compare(cookie.Path, cookie2.Path, StringComparison.Ordinal)) != 0)
			{
				return num;
			}
			return 0;
		}
	}
}
