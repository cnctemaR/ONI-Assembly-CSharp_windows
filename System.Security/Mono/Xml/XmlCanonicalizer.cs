using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Mono.Xml
{
	internal class XmlCanonicalizer
	{
		public XmlCanonicalizer(bool withComments, bool excC14N, Hashtable propagatedNamespaces)
		{
			this.res = new StringBuilder();
			this.comments = withComments;
			this.exclusive = excC14N;
			this.propagatedNss = propagatedNamespaces;
		}

		private void Initialize()
		{
			this.state = XmlCanonicalizer.XmlCanonicalizerState.BeforeDocElement;
			this.visibleNamespaces = new ArrayList();
			this.prevVisibleNamespacesStart = 0;
			this.prevVisibleNamespacesEnd = 0;
			this.res.Length = 0;
		}

		public Stream Canonicalize(XmlDocument doc)
		{
			if (doc == null)
			{
				throw new ArgumentNullException("doc");
			}
			this.Initialize();
			this.FillMissingPrefixes(doc, new XmlNamespaceManager(doc.NameTable), new ArrayList());
			this.WriteDocumentNode(doc);
			UTF8Encoding utf8Encoding = new UTF8Encoding();
			byte[] bytes = utf8Encoding.GetBytes(this.res.ToString());
			return new MemoryStream(bytes);
		}

		public Stream Canonicalize(XmlNodeList nodes)
		{
			this.xnl = nodes;
			if (nodes == null || nodes.Count < 1)
			{
				return new MemoryStream();
			}
			XmlNode xmlNode = nodes[0];
			return this.Canonicalize((xmlNode.NodeType != XmlNodeType.Document) ? xmlNode.OwnerDocument : (xmlNode as XmlDocument));
		}

		public string InclusiveNamespacesPrefixList
		{
			get
			{
				return this.inclusiveNamespacesPrefixList;
			}
			set
			{
				this.inclusiveNamespacesPrefixList = value;
			}
		}

		private XmlAttribute CreateXmlns(XmlNode n)
		{
			XmlAttribute xmlAttribute = ((n.Prefix.Length != 0) ? n.OwnerDocument.CreateAttribute("xmlns", n.Prefix, "http://www.w3.org/2000/xmlns/") : n.OwnerDocument.CreateAttribute("xmlns", "http://www.w3.org/2000/xmlns/"));
			xmlAttribute.Value = n.NamespaceURI;
			return xmlAttribute;
		}

		private void FillMissingPrefixes(XmlNode n, XmlNamespaceManager nsmgr, ArrayList tmpList)
		{
			if (n.Prefix.Length == 0 && this.propagatedNss != null)
			{
				foreach (object obj in this.propagatedNss)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if ((string)dictionaryEntry.Value == n.NamespaceURI)
					{
						n.Prefix = (string)dictionaryEntry.Key;
						break;
					}
				}
			}
			if (n.NodeType == XmlNodeType.Element && ((XmlElement)n).HasAttributes)
			{
				foreach (object obj2 in n.Attributes)
				{
					XmlAttribute xmlAttribute = (XmlAttribute)obj2;
					if (xmlAttribute.NamespaceURI == "http://www.w3.org/2000/xmlns/")
					{
						nsmgr.AddNamespace((xmlAttribute.Prefix.Length != 0) ? xmlAttribute.LocalName : string.Empty, xmlAttribute.Value);
					}
				}
				nsmgr.PushScope();
			}
			if (n.NamespaceURI.Length > 0 && nsmgr.LookupPrefix(n.NamespaceURI) == null)
			{
				tmpList.Add(this.CreateXmlns(n));
			}
			if (n.NodeType == XmlNodeType.Element && ((XmlElement)n).HasAttributes)
			{
				foreach (object obj3 in n.Attributes)
				{
					XmlAttribute xmlAttribute2 = (XmlAttribute)obj3;
					if (xmlAttribute2.NamespaceURI.Length > 0 && nsmgr.LookupNamespace(xmlAttribute2.Prefix) == null)
					{
						tmpList.Add(this.CreateXmlns(xmlAttribute2));
					}
				}
			}
			foreach (object obj4 in tmpList)
			{
				XmlAttribute xmlAttribute3 = (XmlAttribute)obj4;
				((XmlElement)n).SetAttributeNode(xmlAttribute3);
			}
			tmpList.Clear();
			if (n.HasChildNodes)
			{
				for (XmlNode xmlNode = n.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						this.FillMissingPrefixes(xmlNode, nsmgr, tmpList);
					}
				}
			}
			nsmgr.PopScope();
		}

		private void WriteNode(XmlNode node)
		{
			bool flag = this.IsNodeVisible(node);
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
				this.WriteElementNode(node, flag);
				break;
			case XmlNodeType.Attribute:
				throw new XmlException("Attribute node is impossible here", null);
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.SignificantWhitespace:
				this.WriteTextNode(node, flag);
				break;
			case XmlNodeType.EntityReference:
			{
				for (int i = 0; i < node.ChildNodes.Count; i++)
				{
					this.WriteNode(node.ChildNodes[i]);
				}
				break;
			}
			case XmlNodeType.ProcessingInstruction:
				this.WriteProcessingInstructionNode(node, flag);
				break;
			case XmlNodeType.Comment:
				this.WriteCommentNode(node, flag);
				break;
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
				this.WriteDocumentNode(node);
				break;
			case XmlNodeType.Whitespace:
				if (this.state == XmlCanonicalizer.XmlCanonicalizerState.InsideDocElement)
				{
					this.WriteTextNode(node, flag);
				}
				break;
			case XmlNodeType.EndElement:
				throw new XmlException("EndElement node is impossible here", null);
			case XmlNodeType.EndEntity:
				throw new XmlException("EndEntity node is impossible here", null);
			}
		}

		private void WriteDocumentNode(XmlNode node)
		{
			this.state = XmlCanonicalizer.XmlCanonicalizerState.BeforeDocElement;
			for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				this.WriteNode(xmlNode);
			}
		}

		private void WriteElementNode(XmlNode node, bool visible)
		{
			int num = this.prevVisibleNamespacesStart;
			int num2 = this.prevVisibleNamespacesEnd;
			int count = this.visibleNamespaces.Count;
			XmlCanonicalizer.XmlCanonicalizerState xmlCanonicalizerState = this.state;
			if (visible && this.state == XmlCanonicalizer.XmlCanonicalizerState.BeforeDocElement)
			{
				this.state = XmlCanonicalizer.XmlCanonicalizerState.InsideDocElement;
			}
			if (visible)
			{
				this.res.Append("<");
				this.res.Append(node.Name);
			}
			this.WriteNamespacesAxis(node, visible);
			this.WriteAttributesAxis(node);
			if (visible)
			{
				this.res.Append(">");
			}
			for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				this.WriteNode(xmlNode);
			}
			if (visible)
			{
				this.res.Append("</");
				this.res.Append(node.Name);
				this.res.Append(">");
			}
			if (visible && xmlCanonicalizerState == XmlCanonicalizer.XmlCanonicalizerState.BeforeDocElement)
			{
				this.state = XmlCanonicalizer.XmlCanonicalizerState.AfterDocElement;
			}
			this.prevVisibleNamespacesStart = num;
			this.prevVisibleNamespacesEnd = num2;
			if (this.visibleNamespaces.Count > count)
			{
				this.visibleNamespaces.RemoveRange(count, this.visibleNamespaces.Count - count);
			}
		}

		private void WriteNamespacesAxis(XmlNode node, bool visible)
		{
			XmlDocument ownerDocument = node.OwnerDocument;
			bool flag = false;
			ArrayList arrayList = new ArrayList();
			XmlNode xmlNode = node;
			while (xmlNode != null && xmlNode != ownerDocument)
			{
				foreach (object obj in xmlNode.Attributes)
				{
					XmlAttribute xmlAttribute = (XmlAttribute)obj;
					if (this.IsNamespaceNode(xmlAttribute))
					{
						string text = string.Empty;
						if (xmlAttribute.Prefix == "xmlns")
						{
							text = xmlAttribute.LocalName;
						}
						if (!(text == "xml") || !(xmlAttribute.Value == "http://www.w3.org/XML/1998/namespace"))
						{
							string namespaceOfPrefix = node.GetNamespaceOfPrefix(text);
							if (!(namespaceOfPrefix != xmlAttribute.Value))
							{
								if (this.IsNodeVisible(xmlAttribute))
								{
									bool flag2 = this.IsNamespaceRendered(text, xmlAttribute.Value);
									if (!this.exclusive || this.IsVisiblyUtilized(node as XmlElement, xmlAttribute))
									{
										if (visible)
										{
											this.visibleNamespaces.Add(xmlAttribute);
										}
										if (!flag2)
										{
											arrayList.Add(xmlAttribute);
										}
										if (text == string.Empty)
										{
											flag = true;
										}
									}
								}
							}
						}
					}
				}
				xmlNode = xmlNode.ParentNode;
			}
			if (visible && !flag && !this.IsNamespaceRendered(string.Empty, string.Empty) && node.NamespaceURI == string.Empty)
			{
				this.res.Append(" xmlns=\"\"");
			}
			arrayList.Sort(new XmlDsigC14NTransformNamespacesComparer());
			foreach (object obj2 in arrayList)
			{
				XmlNode xmlNode2 = obj2 as XmlNode;
				if (xmlNode2 != null)
				{
					this.res.Append(" ");
					this.res.Append(xmlNode2.Name);
					this.res.Append("=\"");
					this.res.Append(xmlNode2.Value);
					this.res.Append("\"");
				}
			}
			if (visible)
			{
				this.prevVisibleNamespacesStart = this.prevVisibleNamespacesEnd;
				this.prevVisibleNamespacesEnd = this.visibleNamespaces.Count;
			}
		}

		private void WriteAttributesAxis(XmlNode node)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object obj in node.Attributes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!this.IsNamespaceNode(xmlNode) && this.IsNodeVisible(xmlNode))
				{
					arrayList.Add(xmlNode);
				}
			}
			if (!this.exclusive && node.ParentNode != null && node.ParentNode.ParentNode != null && !this.IsNodeVisible(node.ParentNode.ParentNode))
			{
				for (XmlNode xmlNode2 = node.ParentNode; xmlNode2 != null; xmlNode2 = xmlNode2.ParentNode)
				{
					if (xmlNode2.Attributes != null)
					{
						foreach (object obj2 in xmlNode2.Attributes)
						{
							XmlNode xmlNode3 = (XmlNode)obj2;
							if (!(xmlNode3.Prefix != "xml"))
							{
								if (node.Attributes.GetNamedItem(xmlNode3.LocalName, xmlNode3.NamespaceURI) == null)
								{
									bool flag = false;
									foreach (object obj3 in arrayList)
									{
										XmlNode xmlNode4 = obj3 as XmlNode;
										if (xmlNode4.Prefix == "xml" && xmlNode4.LocalName == xmlNode3.LocalName)
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										arrayList.Add(xmlNode3);
									}
								}
							}
						}
					}
				}
			}
			arrayList.Sort(new XmlDsigC14NTransformAttributesComparer());
			foreach (object obj4 in arrayList)
			{
				XmlNode xmlNode5 = obj4 as XmlNode;
				if (xmlNode5 != null)
				{
					this.res.Append(" ");
					this.res.Append(xmlNode5.Name);
					this.res.Append("=\"");
					this.res.Append(this.NormalizeString(xmlNode5.Value, XmlNodeType.Attribute));
					this.res.Append("\"");
				}
			}
		}

		private void WriteTextNode(XmlNode node, bool visible)
		{
			if (visible)
			{
				this.res.Append(this.NormalizeString(node.Value, node.NodeType));
			}
		}

		private void WriteCommentNode(XmlNode node, bool visible)
		{
			if (visible && this.comments)
			{
				if (this.state == XmlCanonicalizer.XmlCanonicalizerState.AfterDocElement)
				{
					this.res.Append("\n<!--");
				}
				else
				{
					this.res.Append("<!--");
				}
				this.res.Append(this.NormalizeString(node.Value, XmlNodeType.Comment));
				if (this.state == XmlCanonicalizer.XmlCanonicalizerState.BeforeDocElement)
				{
					this.res.Append("-->\n");
				}
				else
				{
					this.res.Append("-->");
				}
			}
		}

		private void WriteProcessingInstructionNode(XmlNode node, bool visible)
		{
			if (visible)
			{
				if (this.state == XmlCanonicalizer.XmlCanonicalizerState.AfterDocElement)
				{
					this.res.Append("\n<?");
				}
				else
				{
					this.res.Append("<?");
				}
				this.res.Append(node.Name);
				if (node.Value.Length > 0)
				{
					this.res.Append(" ");
					this.res.Append(this.NormalizeString(node.Value, XmlNodeType.ProcessingInstruction));
				}
				if (this.state == XmlCanonicalizer.XmlCanonicalizerState.BeforeDocElement)
				{
					this.res.Append("?>\n");
				}
				else
				{
					this.res.Append("?>");
				}
			}
		}

		private bool IsNodeVisible(XmlNode node)
		{
			if (this.xnl == null)
			{
				return true;
			}
			foreach (object obj in this.xnl)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (node.Equals(xmlNode))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsVisiblyUtilized(XmlElement owner, XmlAttribute ns)
		{
			if (owner == null)
			{
				return false;
			}
			string text = ((!(ns.LocalName == "xmlns")) ? ns.LocalName : string.Empty);
			if (owner.Prefix == text && owner.NamespaceURI == ns.Value)
			{
				return true;
			}
			if (!owner.HasAttributes)
			{
				return false;
			}
			foreach (object obj in owner.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (!(xmlAttribute.Prefix == string.Empty))
				{
					if (!(xmlAttribute.Prefix != text) && !(xmlAttribute.NamespaceURI != ns.Value))
					{
						if (this.IsNodeVisible(xmlAttribute))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool IsNamespaceRendered(string prefix, string uri)
		{
			bool flag = prefix == string.Empty && uri == string.Empty;
			int num = ((!flag) ? this.prevVisibleNamespacesStart : 0);
			for (int i = this.visibleNamespaces.Count - 1; i >= num; i--)
			{
				XmlNode xmlNode = this.visibleNamespaces[i] as XmlNode;
				if (xmlNode != null)
				{
					string text = string.Empty;
					if (xmlNode.Prefix == "xmlns")
					{
						text = xmlNode.LocalName;
					}
					if (text == prefix)
					{
						return xmlNode.Value == uri;
					}
				}
			}
			return flag;
		}

		private bool IsNamespaceNode(XmlNode node)
		{
			return node != null && node.NodeType == XmlNodeType.Attribute && node.NamespaceURI == "http://www.w3.org/2000/xmlns/";
		}

		private bool IsTextNode(XmlNodeType type)
		{
			return type == XmlNodeType.Text || type == XmlNodeType.CDATA || type == XmlNodeType.Whitespace || type == XmlNodeType.SignificantWhitespace;
		}

		private string NormalizeString(string input, XmlNodeType type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in input)
			{
				if (c == '<' && (type == XmlNodeType.Attribute || this.IsTextNode(type)))
				{
					stringBuilder.Append("&lt;");
				}
				else if (c == '>' && this.IsTextNode(type))
				{
					stringBuilder.Append("&gt;");
				}
				else if (c == '&' && (type == XmlNodeType.Attribute || this.IsTextNode(type)))
				{
					stringBuilder.Append("&amp;");
				}
				else if (c == '"' && type == XmlNodeType.Attribute)
				{
					stringBuilder.Append("&quot;");
				}
				else if (c == '\t' && type == XmlNodeType.Attribute)
				{
					stringBuilder.Append("&#x9;");
				}
				else if (c == '\n' && type == XmlNodeType.Attribute)
				{
					stringBuilder.Append("&#xA;");
				}
				else if (c == '\r')
				{
					stringBuilder.Append("&#xD;");
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private bool comments;

		private bool exclusive;

		private string inclusiveNamespacesPrefixList;

		private XmlNodeList xnl;

		private StringBuilder res;

		private XmlCanonicalizer.XmlCanonicalizerState state;

		private ArrayList visibleNamespaces;

		private int prevVisibleNamespacesStart;

		private int prevVisibleNamespacesEnd;

		private Hashtable propagatedNss;

		private enum XmlCanonicalizerState
		{
			BeforeDocElement,
			InsideDocElement,
			AfterDocElement
		}
	}
}
