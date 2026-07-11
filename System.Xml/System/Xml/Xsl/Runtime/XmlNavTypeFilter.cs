using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlNavTypeFilter : XmlNavigatorFilter
	{
		static XmlNavTypeFilter()
		{
			XmlNavTypeFilter.TypeFilters[1] = new XmlNavTypeFilter(XPathNodeType.Element);
			XmlNavTypeFilter.TypeFilters[4] = new XmlNavTypeFilter(XPathNodeType.Text);
			XmlNavTypeFilter.TypeFilters[7] = new XmlNavTypeFilter(XPathNodeType.ProcessingInstruction);
			XmlNavTypeFilter.TypeFilters[8] = new XmlNavTypeFilter(XPathNodeType.Comment);
		}

		public static XmlNavigatorFilter Create(XPathNodeType nodeType)
		{
			return XmlNavTypeFilter.TypeFilters[(int)nodeType];
		}

		private XmlNavTypeFilter(XPathNodeType nodeType)
		{
			this.nodeType = nodeType;
			this.mask = XPathNavigator.GetContentKindMask(nodeType);
		}

		public override bool MoveToContent(XPathNavigator navigator)
		{
			return navigator.MoveToChild(this.nodeType);
		}

		public override bool MoveToNextContent(XPathNavigator navigator)
		{
			return navigator.MoveToNext(this.nodeType);
		}

		public override bool MoveToFollowingSibling(XPathNavigator navigator)
		{
			return navigator.MoveToNext(this.nodeType);
		}

		public override bool MoveToPreviousSibling(XPathNavigator navigator)
		{
			return navigator.MoveToPrevious(this.nodeType);
		}

		public override bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navEnd)
		{
			return navigator.MoveToFollowing(this.nodeType, navEnd);
		}

		public override bool IsFiltered(XPathNavigator navigator)
		{
			return ((1 << (int)navigator.NodeType) & this.mask) == 0;
		}

		private static XmlNavigatorFilter[] TypeFilters = new XmlNavigatorFilter[9];

		private XPathNodeType nodeType;

		private int mask;
	}
}
