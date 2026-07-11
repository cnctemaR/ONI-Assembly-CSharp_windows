using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class DocumentOrderComparer : IComparer<XPathNavigator>
	{
		public int Compare(XPathNavigator navThis, XPathNavigator navThat)
		{
			switch (navThis.ComparePosition(navThat))
			{
			case XmlNodeOrder.Before:
				return -1;
			case XmlNodeOrder.After:
				return 1;
			case XmlNodeOrder.Same:
				return 0;
			default:
				if (this.roots == null)
				{
					this.roots = new List<XPathNavigator>();
				}
				if (this.GetDocumentIndex(navThis) >= this.GetDocumentIndex(navThat))
				{
					return 1;
				}
				return -1;
			}
		}

		public int GetDocumentIndex(XPathNavigator nav)
		{
			if (this.roots == null)
			{
				this.roots = new List<XPathNavigator>();
			}
			XPathNavigator xpathNavigator = nav.Clone();
			xpathNavigator.MoveToRoot();
			for (int i = 0; i < this.roots.Count; i++)
			{
				if (xpathNavigator.IsSamePosition(this.roots[i]))
				{
					return i;
				}
			}
			this.roots.Add(xpathNavigator);
			return this.roots.Count - 1;
		}

		private List<XPathNavigator> roots;
	}
}
