using System;
using System.Xml.Linq;

namespace System.Xml.XPath
{
	public static class XDocumentExtensions
	{
		public static IXPathNavigable ToXPathNavigable(this XNode node)
		{
			return new XDocumentExtensions.XDocumentNavigable(node);
		}

		private class XDocumentNavigable : IXPathNavigable
		{
			public XDocumentNavigable(XNode n)
			{
				this._node = n;
			}

			public XPathNavigator CreateNavigator()
			{
				return this._node.CreateNavigator();
			}

			private XNode _node;
		}
	}
}
