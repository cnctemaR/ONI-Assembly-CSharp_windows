using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace System.Xml.XPath
{
	public static class Extensions
	{
		public static XPathNavigator CreateNavigator(this XNode node)
		{
			return node.CreateNavigator(new NameTable());
		}

		public static XPathNavigator CreateNavigator(this XNode node, XmlNameTable nameTable)
		{
			return new XNodeNavigator(node, nameTable);
		}

		public static object XPathEvaluate(this XNode node, string expression)
		{
			return node.XPathEvaluate(expression, null);
		}

		public static object XPathEvaluate(this XNode node, string expression, IXmlNamespaceResolver nsResolver)
		{
			return node.CreateNavigator().Evaluate(expression, nsResolver);
		}

		public static XElement XPathSelectElement(this XNode node, string xpath)
		{
			return node.XPathSelectElement(xpath, null);
		}

		public static XElement XPathSelectElement(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
		{
			XPathNavigator xpathNavigator = node.CreateNavigator().SelectSingleNode(xpath, nsResolver);
			if (xpathNavigator == null)
			{
				return null;
			}
			return xpathNavigator.UnderlyingObject as XElement;
		}

		public static IEnumerable<XElement> XPathSelectElements(this XNode node, string xpath)
		{
			return node.XPathSelectElements(xpath, null);
		}

		public static IEnumerable<XElement> XPathSelectElements(this XNode node, string xpath, IXmlNamespaceResolver nsResolver)
		{
			XPathNodeIterator iter = node.CreateNavigator().Select(xpath, nsResolver);
			foreach (object obj in iter)
			{
				XPathNavigator nav = (XPathNavigator)obj;
				if (nav.UnderlyingObject is XElement)
				{
					yield return (XElement)nav.UnderlyingObject;
				}
			}
			yield break;
		}
	}
}
