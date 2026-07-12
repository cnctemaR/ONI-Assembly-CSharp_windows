using System;
using System.Xml.Linq;

namespace System.Xml.XPath
{
	internal static class XObjectExtensions
	{
		public static XContainer GetParent(this XObject obj)
		{
			XContainer xcontainer = obj.Parent;
			if (xcontainer == null)
			{
				xcontainer = obj.Document;
			}
			if (xcontainer == obj)
			{
				return null;
			}
			return xcontainer;
		}
	}
}
