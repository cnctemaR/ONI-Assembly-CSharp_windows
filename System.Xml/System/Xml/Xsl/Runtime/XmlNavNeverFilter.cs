using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlNavNeverFilter : XmlNavigatorFilter
	{
		public static XmlNavigatorFilter Create()
		{
			return XmlNavNeverFilter.Singleton;
		}

		private XmlNavNeverFilter()
		{
		}

		public override bool MoveToContent(XPathNavigator navigator)
		{
			return XmlNavNeverFilter.MoveToFirstAttributeContent(navigator);
		}

		public override bool MoveToNextContent(XPathNavigator navigator)
		{
			return XmlNavNeverFilter.MoveToNextAttributeContent(navigator);
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
			return false;
		}

		public static bool MoveToFirstAttributeContent(XPathNavigator navigator)
		{
			return navigator.MoveToFirstAttribute() || navigator.MoveToFirstChild();
		}

		public static bool MoveToNextAttributeContent(XPathNavigator navigator)
		{
			if (navigator.NodeType == XPathNodeType.Attribute)
			{
				if (!navigator.MoveToNextAttribute())
				{
					navigator.MoveToParent();
					if (!navigator.MoveToFirstChild())
					{
						navigator.MoveToFirstAttribute();
						while (navigator.MoveToNextAttribute())
						{
						}
						return false;
					}
				}
				return true;
			}
			return navigator.MoveToNext();
		}

		private static XmlNavigatorFilter Singleton = new XmlNavNeverFilter();
	}
}
