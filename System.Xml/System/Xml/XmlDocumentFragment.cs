using System;
using System.Text;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlDocumentFragment : XmlNode, IHasXmlChildNode
	{
		protected internal XmlDocumentFragment(XmlDocument doc)
			: base(doc)
		{
		}

		XmlLinkedNode IHasXmlChildNode.LastLinkedChild
		{
			get
			{
				return this.lastLinkedChild;
			}
			set
			{
				this.lastLinkedChild = value;
			}
		}

		public override string InnerXml
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.ChildNodes.Count; i++)
				{
					stringBuilder.Append(this.ChildNodes[i].OuterXml);
				}
				return stringBuilder.ToString();
			}
			set
			{
				for (int i = 0; i < this.ChildNodes.Count; i++)
				{
					this.RemoveChild(this.ChildNodes[i]);
				}
				XmlNamespaceManager xmlNamespaceManager = base.ConstructNamespaceManager();
				XmlParserContext xmlParserContext = new XmlParserContext(this.OwnerDocument.NameTable, xmlNamespaceManager, (this.OwnerDocument.DocumentType == null) ? null : this.OwnerDocument.DocumentType.DTD, this.BaseURI, this.XmlLang, this.XmlSpace, null);
				XmlTextReader xmlTextReader = new XmlTextReader(value, XmlNodeType.Element, xmlParserContext);
				xmlTextReader.XmlResolver = this.OwnerDocument.Resolver;
				for (;;)
				{
					XmlNode xmlNode = this.OwnerDocument.ReadNode(xmlTextReader);
					if (xmlNode == null)
					{
						break;
					}
					this.AppendChild(xmlNode);
				}
			}
		}

		public override string LocalName
		{
			get
			{
				return "#document-fragment";
			}
		}

		public override string Name
		{
			get
			{
				return "#document-fragment";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentFragment;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			if (deep)
			{
				XmlNode xmlNode = this.FirstChild;
				while (xmlNode != null && xmlNode.HasChildNodes)
				{
					this.AppendChild(xmlNode.NextSibling.CloneNode(false));
					xmlNode = xmlNode.NextSibling;
				}
				return xmlNode;
			}
			return new XmlDocumentFragment(this.OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (int i = 0; i < this.ChildNodes.Count; i++)
			{
				this.ChildNodes[i].WriteContentTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			for (int i = 0; i < this.ChildNodes.Count; i++)
			{
				this.ChildNodes[i].WriteTo(w);
			}
		}

		private XmlLinkedNode lastLinkedChild;
	}
}
