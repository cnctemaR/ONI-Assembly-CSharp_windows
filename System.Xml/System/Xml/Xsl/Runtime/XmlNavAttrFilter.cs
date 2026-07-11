using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlNavAttrFilter : XmlNavigatorFilter
	{
		public static XmlNavigatorFilter Create()
		{
			return XmlNavAttrFilter.Singleton;
		}

		private XmlNavAttrFilter()
		{
		}

		public override bool MoveToContent(XPathNavigator navigator)
		{
			return navigator.MoveToFirstChild();
		}

		public override bool MoveToNextContent(XPathNavigator navigator)
		{
			return navigator.MoveToNext();
		}

		public override bool MoveToFollowingSibling(XPathNavigator navigator)
		{
			return navigator.MoveToNext();
		}

		public override bool MoveToPreviousSibling(XPathNavigator navigator)
		{
			return navigator.MoveToPrevious();
		}

		public override bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navEnd)
		{
			return navigator.MoveToFollowing(XPathNodeType.All, navEnd);
		}

		public override bool IsFiltered(XPathNavigator navigator)
		{
			return navigator.NodeType == XPathNodeType.Attribute;
		}

		private static XmlNavigatorFilter Singleton = new XmlNavAttrFilter();
	}
}
