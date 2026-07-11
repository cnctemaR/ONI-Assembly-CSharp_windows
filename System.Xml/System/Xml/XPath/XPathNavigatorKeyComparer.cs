using System;
using System.Collections;
using MS.Internal.Xml.Cache;

namespace System.Xml.XPath
{
	internal class XPathNavigatorKeyComparer : IEqualityComparer
	{
		bool IEqualityComparer.Equals(object obj1, object obj2)
		{
			XPathNavigator xpathNavigator = obj1 as XPathNavigator;
			XPathNavigator xpathNavigator2 = obj2 as XPathNavigator;
			return xpathNavigator != null && xpathNavigator2 != null && xpathNavigator.IsSamePosition(xpathNavigator2);
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			XPathDocumentNavigator xpathDocumentNavigator;
			int num;
			XPathNavigator xpathNavigator;
			if ((xpathDocumentNavigator = obj as XPathDocumentNavigator) != null)
			{
				num = xpathDocumentNavigator.GetPositionHashCode();
			}
			else if ((xpathNavigator = obj as XPathNavigator) != null)
			{
				object underlyingObject = xpathNavigator.UnderlyingObject;
				if (underlyingObject != null)
				{
					num = underlyingObject.GetHashCode();
				}
				else
				{
					num = (int)xpathNavigator.NodeType;
					num ^= xpathNavigator.LocalName.GetHashCode();
					num ^= xpathNavigator.Prefix.GetHashCode();
					num ^= xpathNavigator.NamespaceURI.GetHashCode();
				}
			}
			else
			{
				num = obj.GetHashCode();
			}
			return num;
		}
	}
}
