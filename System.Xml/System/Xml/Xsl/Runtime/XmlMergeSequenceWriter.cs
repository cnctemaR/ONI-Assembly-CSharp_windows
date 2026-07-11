using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlMergeSequenceWriter : XmlSequenceWriter
	{
		public XmlMergeSequenceWriter(XmlRawWriter xwrt)
		{
			this.xwrt = xwrt;
			this.lastItemWasAtomic = false;
		}

		public override XmlRawWriter StartTree(XPathNodeType rootType, IXmlNamespaceResolver nsResolver, XmlNameTable nameTable)
		{
			if (rootType == XPathNodeType.Attribute || rootType == XPathNodeType.Namespace)
			{
				throw new XslTransformException("XmlWriter cannot process the sequence returned by the query, because it contains an attribute or namespace node.", new string[] { string.Empty });
			}
			this.xwrt.NamespaceResolver = nsResolver;
			return this.xwrt;
		}

		public override void EndTree()
		{
			this.lastItemWasAtomic = false;
		}

		public override void WriteItem(XPathItem item)
		{
			if (!item.IsNode)
			{
				this.WriteString(item.Value);
				return;
			}
			XPathNavigator xpathNavigator = item as XPathNavigator;
			if (xpathNavigator.NodeType == XPathNodeType.Attribute || xpathNavigator.NodeType == XPathNodeType.Namespace)
			{
				throw new XslTransformException("XmlWriter cannot process the sequence returned by the query, because it contains an attribute or namespace node.", new string[] { string.Empty });
			}
			this.CopyNode(xpathNavigator);
			this.lastItemWasAtomic = false;
		}

		private void WriteString(string value)
		{
			if (this.lastItemWasAtomic)
			{
				this.xwrt.WriteWhitespace(" ");
			}
			else
			{
				this.lastItemWasAtomic = true;
			}
			this.xwrt.WriteString(value);
		}

		private void CopyNode(XPathNavigator nav)
		{
			int num = 0;
			for (;;)
			{
				IL_0002:
				if (this.CopyShallowNode(nav))
				{
					if (nav.NodeType == XPathNodeType.Element)
					{
						if (nav.MoveToFirstAttribute())
						{
							do
							{
								this.CopyShallowNode(nav);
							}
							while (nav.MoveToNextAttribute());
							nav.MoveToParent();
						}
						XPathNamespaceScope xpathNamespaceScope = ((num == 0) ? XPathNamespaceScope.ExcludeXml : XPathNamespaceScope.Local);
						if (nav.MoveToFirstNamespace(xpathNamespaceScope))
						{
							this.CopyNamespaces(nav, xpathNamespaceScope);
							nav.MoveToParent();
						}
						this.xwrt.StartElementContent();
					}
					if (nav.MoveToFirstChild())
					{
						num++;
						continue;
					}
					if (nav.NodeType == XPathNodeType.Element)
					{
						this.xwrt.WriteEndElement(nav.Prefix, nav.LocalName, nav.NamespaceURI);
					}
				}
				while (num != 0)
				{
					if (nav.MoveToNext())
					{
						goto IL_0002;
					}
					num--;
					nav.MoveToParent();
					if (nav.NodeType == XPathNodeType.Element)
					{
						this.xwrt.WriteFullEndElement(nav.Prefix, nav.LocalName, nav.NamespaceURI);
					}
				}
				break;
			}
		}

		private bool CopyShallowNode(XPathNavigator nav)
		{
			bool flag = false;
			switch (nav.NodeType)
			{
			case XPathNodeType.Root:
				flag = true;
				break;
			case XPathNodeType.Element:
				this.xwrt.WriteStartElement(nav.Prefix, nav.LocalName, nav.NamespaceURI);
				flag = true;
				break;
			case XPathNodeType.Attribute:
				this.xwrt.WriteStartAttribute(nav.Prefix, nav.LocalName, nav.NamespaceURI);
				this.xwrt.WriteString(nav.Value);
				this.xwrt.WriteEndAttribute();
				break;
			case XPathNodeType.Namespace:
				this.xwrt.WriteNamespaceDeclaration(nav.LocalName, nav.Value);
				break;
			case XPathNodeType.Text:
				this.xwrt.WriteString(nav.Value);
				break;
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
				this.xwrt.WriteWhitespace(nav.Value);
				break;
			case XPathNodeType.ProcessingInstruction:
				this.xwrt.WriteProcessingInstruction(nav.LocalName, nav.Value);
				break;
			case XPathNodeType.Comment:
				this.xwrt.WriteComment(nav.Value);
				break;
			}
			return flag;
		}

		private void CopyNamespaces(XPathNavigator nav, XPathNamespaceScope nsScope)
		{
			string localName = nav.LocalName;
			string value = nav.Value;
			if (nav.MoveToNextNamespace(nsScope))
			{
				this.CopyNamespaces(nav, nsScope);
			}
			this.xwrt.WriteNamespaceDeclaration(localName, value);
		}

		private XmlRawWriter xwrt;

		private bool lastItemWasAtomic;
	}
}
