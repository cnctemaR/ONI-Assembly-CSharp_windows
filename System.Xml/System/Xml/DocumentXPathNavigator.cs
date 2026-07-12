using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	internal sealed class DocumentXPathNavigator : XPathNavigator, IHasXmlNode
	{
		public DocumentXPathNavigator(XmlDocument document, XmlNode node)
		{
			this.document = document;
			this.ResetPosition(node);
		}

		public DocumentXPathNavigator(DocumentXPathNavigator other)
		{
			this.document = other.document;
			this.source = other.source;
			this.attributeIndex = other.attributeIndex;
			this.namespaceParent = other.namespaceParent;
		}

		public override XPathNavigator Clone()
		{
			return new DocumentXPathNavigator(this);
		}

		public override void SetValue(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			XmlNode xmlNode = this.source;
			switch (xmlNode.NodeType)
			{
			case XmlNodeType.Element:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
				break;
			case XmlNodeType.Attribute:
				if (!((XmlAttribute)xmlNode).IsNamespace)
				{
					xmlNode.InnerText = value;
					return;
				}
				goto IL_00B8;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
			{
				this.CalibrateText();
				xmlNode = this.source;
				XmlNode xmlNode2 = this.TextEnd(xmlNode);
				if (xmlNode != xmlNode2)
				{
					if (xmlNode.IsReadOnly)
					{
						throw new InvalidOperationException(Res.GetString("This node is read-only. It cannot be modified."));
					}
					DocumentXPathNavigator.DeleteToFollowingSibling(xmlNode.NextSibling, xmlNode2);
				}
				break;
			}
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentType:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				goto IL_00B8;
			default:
				goto IL_00B8;
			}
			xmlNode.InnerText = value;
			return;
			IL_00B8:
			throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.document.NameTable;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				this.CalibrateText();
				return this.source.XPNodeType;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.source.XPLocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				XmlAttribute xmlAttribute = this.source as XmlAttribute;
				if (xmlAttribute != null && xmlAttribute.IsNamespace)
				{
					return string.Empty;
				}
				return this.source.NamespaceURI;
			}
		}

		public override string Name
		{
			get
			{
				XmlNodeType nodeType = this.source.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType != XmlNodeType.ProcessingInstruction)
						{
							return string.Empty;
						}
					}
					else
					{
						if (!((XmlAttribute)this.source).IsNamespace)
						{
							return this.source.Name;
						}
						string localName = this.source.LocalName;
						if (Ref.Equal(localName, this.document.strXmlns))
						{
							return string.Empty;
						}
						return localName;
					}
				}
				return this.source.Name;
			}
		}

		public override string Prefix
		{
			get
			{
				XmlAttribute xmlAttribute = this.source as XmlAttribute;
				if (xmlAttribute != null && xmlAttribute.IsNamespace)
				{
					return string.Empty;
				}
				return this.source.Prefix;
			}
		}

		public override string Value
		{
			get
			{
				XmlNodeType nodeType = this.source.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType - XmlNodeType.Text > 1)
					{
						switch (nodeType)
						{
						case XmlNodeType.Document:
							return this.ValueDocument;
						case XmlNodeType.DocumentFragment:
							goto IL_0039;
						case XmlNodeType.Whitespace:
						case XmlNodeType.SignificantWhitespace:
							goto IL_004C;
						}
						return this.source.Value;
					}
					IL_004C:
					return this.ValueText;
				}
				IL_0039:
				return this.source.InnerText;
			}
		}

		private string ValueDocument
		{
			get
			{
				XmlElement documentElement = this.document.DocumentElement;
				if (documentElement != null)
				{
					return documentElement.InnerText;
				}
				return string.Empty;
			}
		}

		private string ValueText
		{
			get
			{
				this.CalibrateText();
				string text = this.source.Value;
				XmlNode xmlNode = this.NextSibling(this.source);
				if (xmlNode != null && xmlNode.IsText)
				{
					StringBuilder stringBuilder = new StringBuilder(text);
					do
					{
						stringBuilder.Append(xmlNode.Value);
						xmlNode = this.NextSibling(xmlNode);
					}
					while (xmlNode != null && xmlNode.IsText);
					text = stringBuilder.ToString();
				}
				return text;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.source.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				XmlElement xmlElement = this.source as XmlElement;
				return xmlElement != null && xmlElement.IsEmpty;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.source.XmlLang;
			}
		}

		public override object UnderlyingObject
		{
			get
			{
				this.CalibrateText();
				return this.source;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				XmlElement xmlElement = this.source as XmlElement;
				if (xmlElement != null && xmlElement.HasAttributes)
				{
					XmlAttributeCollection attributes = xmlElement.Attributes;
					for (int i = 0; i < attributes.Count; i++)
					{
						if (!attributes[i].IsNamespace)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this.source.GetXPAttribute(localName, namespaceURI);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			XmlElement xmlElement = this.source as XmlElement;
			if (xmlElement != null && xmlElement.HasAttributes)
			{
				XmlAttributeCollection attributes = xmlElement.Attributes;
				int i = 0;
				while (i < attributes.Count)
				{
					XmlAttribute xmlAttribute = attributes[i];
					if (xmlAttribute.LocalName == localName && xmlAttribute.NamespaceURI == namespaceURI)
					{
						if (!xmlAttribute.IsNamespace)
						{
							this.source = xmlAttribute;
							this.attributeIndex = i;
							return true;
						}
						return false;
					}
					else
					{
						i++;
					}
				}
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			XmlElement xmlElement = this.source as XmlElement;
			if (xmlElement != null && xmlElement.HasAttributes)
			{
				XmlAttributeCollection attributes = xmlElement.Attributes;
				for (int i = 0; i < attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = attributes[i];
					if (!xmlAttribute.IsNamespace)
					{
						this.source = xmlAttribute;
						this.attributeIndex = i;
						return true;
					}
				}
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			XmlAttribute xmlAttribute = this.source as XmlAttribute;
			if (xmlAttribute == null || xmlAttribute.IsNamespace)
			{
				return false;
			}
			XmlAttributeCollection xmlAttributeCollection;
			if (!DocumentXPathNavigator.CheckAttributePosition(xmlAttribute, out xmlAttributeCollection, this.attributeIndex) && !DocumentXPathNavigator.ResetAttributePosition(xmlAttribute, xmlAttributeCollection, out this.attributeIndex))
			{
				return false;
			}
			for (int i = this.attributeIndex + 1; i < xmlAttributeCollection.Count; i++)
			{
				xmlAttribute = xmlAttributeCollection[i];
				if (!xmlAttribute.IsNamespace)
				{
					this.source = xmlAttribute;
					this.attributeIndex = i;
					return true;
				}
			}
			return false;
		}

		public override string GetNamespace(string name)
		{
			XmlNode xmlNode = this.source;
			while (xmlNode != null && xmlNode.NodeType != XmlNodeType.Element)
			{
				XmlAttribute xmlAttribute = xmlNode as XmlAttribute;
				if (xmlAttribute != null)
				{
					xmlNode = xmlAttribute.OwnerElement;
				}
				else
				{
					xmlNode = xmlNode.ParentNode;
				}
			}
			XmlElement xmlElement = xmlNode as XmlElement;
			if (xmlElement != null)
			{
				string text;
				if (name != null && name.Length != 0)
				{
					text = name;
				}
				else
				{
					text = this.document.strXmlns;
				}
				string strReservedXmlns = this.document.strReservedXmlns;
				XmlAttribute attributeNode;
				for (;;)
				{
					attributeNode = xmlElement.GetAttributeNode(text, strReservedXmlns);
					if (attributeNode != null)
					{
						break;
					}
					xmlElement = xmlElement.ParentNode as XmlElement;
					if (xmlElement == null)
					{
						goto IL_0087;
					}
				}
				return attributeNode.Value;
			}
			IL_0087:
			if (name == this.document.strXml)
			{
				return this.document.strReservedXml;
			}
			if (name == this.document.strXmlns)
			{
				return this.document.strReservedXmlns;
			}
			return string.Empty;
		}

		public override bool MoveToNamespace(string name)
		{
			if (name == this.document.strXmlns)
			{
				return false;
			}
			XmlElement xmlElement = this.source as XmlElement;
			if (xmlElement != null)
			{
				string text;
				if (name != null && name.Length != 0)
				{
					text = name;
				}
				else
				{
					text = this.document.strXmlns;
				}
				string strReservedXmlns = this.document.strReservedXmlns;
				XmlAttribute attributeNode;
				for (;;)
				{
					attributeNode = xmlElement.GetAttributeNode(text, strReservedXmlns);
					if (attributeNode != null)
					{
						break;
					}
					xmlElement = xmlElement.ParentNode as XmlElement;
					if (xmlElement == null)
					{
						goto Block_6;
					}
				}
				this.namespaceParent = (XmlElement)this.source;
				this.source = attributeNode;
				return true;
				Block_6:
				if (name == this.document.strXml)
				{
					this.namespaceParent = (XmlElement)this.source;
					this.source = this.document.NamespaceXml;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
		{
			XmlElement xmlElement = this.source as XmlElement;
			if (xmlElement == null)
			{
				return false;
			}
			int maxValue = int.MaxValue;
			switch (scope)
			{
			case XPathNamespaceScope.All:
			{
				XmlAttributeCollection xmlAttributeCollection = xmlElement.Attributes;
				if (!DocumentXPathNavigator.MoveToFirstNamespaceGlobal(ref xmlAttributeCollection, ref maxValue))
				{
					this.source = this.document.NamespaceXml;
				}
				else
				{
					this.source = xmlAttributeCollection[maxValue];
					this.attributeIndex = maxValue;
				}
				this.namespaceParent = xmlElement;
				break;
			}
			case XPathNamespaceScope.ExcludeXml:
			{
				XmlAttributeCollection xmlAttributeCollection = xmlElement.Attributes;
				if (!DocumentXPathNavigator.MoveToFirstNamespaceGlobal(ref xmlAttributeCollection, ref maxValue))
				{
					return false;
				}
				XmlAttribute xmlAttribute = xmlAttributeCollection[maxValue];
				while (Ref.Equal(xmlAttribute.LocalName, this.document.strXml))
				{
					if (!DocumentXPathNavigator.MoveToNextNamespaceGlobal(ref xmlAttributeCollection, ref maxValue))
					{
						return false;
					}
					xmlAttribute = xmlAttributeCollection[maxValue];
				}
				this.source = xmlAttribute;
				this.attributeIndex = maxValue;
				this.namespaceParent = xmlElement;
				break;
			}
			case XPathNamespaceScope.Local:
			{
				if (!xmlElement.HasAttributes)
				{
					return false;
				}
				XmlAttributeCollection xmlAttributeCollection = xmlElement.Attributes;
				if (!DocumentXPathNavigator.MoveToFirstNamespaceLocal(xmlAttributeCollection, ref maxValue))
				{
					return false;
				}
				this.source = xmlAttributeCollection[maxValue];
				this.attributeIndex = maxValue;
				this.namespaceParent = xmlElement;
				break;
			}
			default:
				return false;
			}
			return true;
		}

		private static bool MoveToFirstNamespaceLocal(XmlAttributeCollection attributes, ref int index)
		{
			for (int i = attributes.Count - 1; i >= 0; i--)
			{
				if (attributes[i].IsNamespace)
				{
					index = i;
					return true;
				}
			}
			return false;
		}

		private static bool MoveToFirstNamespaceGlobal(ref XmlAttributeCollection attributes, ref int index)
		{
			if (DocumentXPathNavigator.MoveToFirstNamespaceLocal(attributes, ref index))
			{
				return true;
			}
			for (XmlElement xmlElement = attributes.parent.ParentNode as XmlElement; xmlElement != null; xmlElement = xmlElement.ParentNode as XmlElement)
			{
				if (xmlElement.HasAttributes)
				{
					attributes = xmlElement.Attributes;
					if (DocumentXPathNavigator.MoveToFirstNamespaceLocal(attributes, ref index))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			XmlAttribute xmlAttribute = this.source as XmlAttribute;
			if (xmlAttribute == null || !xmlAttribute.IsNamespace)
			{
				return false;
			}
			int num = this.attributeIndex;
			XmlAttributeCollection xmlAttributeCollection;
			if (!DocumentXPathNavigator.CheckAttributePosition(xmlAttribute, out xmlAttributeCollection, num) && !DocumentXPathNavigator.ResetAttributePosition(xmlAttribute, xmlAttributeCollection, out num))
			{
				return false;
			}
			switch (scope)
			{
			case XPathNamespaceScope.All:
				while (DocumentXPathNavigator.MoveToNextNamespaceGlobal(ref xmlAttributeCollection, ref num))
				{
					xmlAttribute = xmlAttributeCollection[num];
					if (!this.PathHasDuplicateNamespace(xmlAttribute.OwnerElement, this.namespaceParent, xmlAttribute.LocalName))
					{
						this.source = xmlAttribute;
						this.attributeIndex = num;
						return true;
					}
				}
				if (this.PathHasDuplicateNamespace(null, this.namespaceParent, this.document.strXml))
				{
					return false;
				}
				this.source = this.document.NamespaceXml;
				return true;
			case XPathNamespaceScope.ExcludeXml:
				while (DocumentXPathNavigator.MoveToNextNamespaceGlobal(ref xmlAttributeCollection, ref num))
				{
					xmlAttribute = xmlAttributeCollection[num];
					string localName = xmlAttribute.LocalName;
					if (!this.PathHasDuplicateNamespace(xmlAttribute.OwnerElement, this.namespaceParent, localName) && !Ref.Equal(localName, this.document.strXml))
					{
						this.source = xmlAttribute;
						this.attributeIndex = num;
						return true;
					}
				}
				return false;
			case XPathNamespaceScope.Local:
				if (xmlAttribute.OwnerElement != this.namespaceParent)
				{
					return false;
				}
				if (!DocumentXPathNavigator.MoveToNextNamespaceLocal(xmlAttributeCollection, ref num))
				{
					return false;
				}
				this.source = xmlAttributeCollection[num];
				this.attributeIndex = num;
				break;
			default:
				return false;
			}
			return true;
		}

		private static bool MoveToNextNamespaceLocal(XmlAttributeCollection attributes, ref int index)
		{
			for (int i = index - 1; i >= 0; i--)
			{
				if (attributes[i].IsNamespace)
				{
					index = i;
					return true;
				}
			}
			return false;
		}

		private static bool MoveToNextNamespaceGlobal(ref XmlAttributeCollection attributes, ref int index)
		{
			if (DocumentXPathNavigator.MoveToNextNamespaceLocal(attributes, ref index))
			{
				return true;
			}
			for (XmlElement xmlElement = attributes.parent.ParentNode as XmlElement; xmlElement != null; xmlElement = xmlElement.ParentNode as XmlElement)
			{
				if (xmlElement.HasAttributes)
				{
					attributes = xmlElement.Attributes;
					if (DocumentXPathNavigator.MoveToFirstNamespaceLocal(attributes, ref index))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool PathHasDuplicateNamespace(XmlElement top, XmlElement bottom, string localName)
		{
			string strReservedXmlns = this.document.strReservedXmlns;
			while (bottom != null && bottom != top)
			{
				if (bottom.GetAttributeNode(localName, strReservedXmlns) != null)
				{
					return true;
				}
				bottom = bottom.ParentNode as XmlElement;
			}
			return false;
		}

		public override string LookupNamespace(string prefix)
		{
			string text = base.LookupNamespace(prefix);
			if (text != null)
			{
				text = this.NameTable.Add(text);
			}
			return text;
		}

		public override bool MoveToNext()
		{
			XmlNode xmlNode = this.NextSibling(this.source);
			if (xmlNode == null)
			{
				return false;
			}
			if (xmlNode.IsText && this.source.IsText)
			{
				xmlNode = this.NextSibling(this.TextEnd(xmlNode));
				if (xmlNode == null)
				{
					return false;
				}
			}
			XmlNode xmlNode2 = this.ParentNode(xmlNode);
			while (!DocumentXPathNavigator.IsValidChild(xmlNode2, xmlNode))
			{
				xmlNode = this.NextSibling(xmlNode);
				if (xmlNode == null)
				{
					return false;
				}
			}
			this.source = xmlNode;
			return true;
		}

		public override bool MoveToPrevious()
		{
			XmlNode xmlNode = this.PreviousSibling(this.source);
			if (xmlNode == null)
			{
				return false;
			}
			if (xmlNode.IsText)
			{
				if (this.source.IsText)
				{
					xmlNode = this.PreviousSibling(this.TextStart(xmlNode));
					if (xmlNode == null)
					{
						return false;
					}
				}
				else
				{
					xmlNode = this.TextStart(xmlNode);
				}
			}
			XmlNode xmlNode2 = this.ParentNode(xmlNode);
			while (!DocumentXPathNavigator.IsValidChild(xmlNode2, xmlNode))
			{
				xmlNode = this.PreviousSibling(xmlNode);
				if (xmlNode == null)
				{
					return false;
				}
			}
			this.source = xmlNode;
			return true;
		}

		public override bool MoveToFirst()
		{
			if (this.source.NodeType == XmlNodeType.Attribute)
			{
				return false;
			}
			XmlNode xmlNode = this.ParentNode(this.source);
			if (xmlNode == null)
			{
				return false;
			}
			XmlNode xmlNode2 = this.FirstChild(xmlNode);
			while (!DocumentXPathNavigator.IsValidChild(xmlNode, xmlNode2))
			{
				xmlNode2 = this.NextSibling(xmlNode2);
				if (xmlNode2 == null)
				{
					return false;
				}
			}
			this.source = xmlNode2;
			return true;
		}

		public override bool MoveToFirstChild()
		{
			XmlNodeType nodeType = this.source.NodeType;
			XmlNode xmlNode;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Document && nodeType != XmlNodeType.DocumentFragment)
				{
					return false;
				}
				xmlNode = this.FirstChild(this.source);
				if (xmlNode == null)
				{
					return false;
				}
				while (!DocumentXPathNavigator.IsValidChild(this.source, xmlNode))
				{
					xmlNode = this.NextSibling(xmlNode);
					if (xmlNode == null)
					{
						return false;
					}
				}
			}
			else
			{
				xmlNode = this.FirstChild(this.source);
				if (xmlNode == null)
				{
					return false;
				}
			}
			this.source = xmlNode;
			return true;
		}

		public override bool MoveToParent()
		{
			XmlNode xmlNode = this.ParentNode(this.source);
			if (xmlNode != null)
			{
				this.source = xmlNode;
				return true;
			}
			XmlAttribute xmlAttribute = this.source as XmlAttribute;
			if (xmlAttribute != null)
			{
				xmlNode = (xmlAttribute.IsNamespace ? this.namespaceParent : xmlAttribute.OwnerElement);
				if (xmlNode != null)
				{
					this.source = xmlNode;
					this.namespaceParent = null;
					return true;
				}
			}
			return false;
		}

		public override void MoveToRoot()
		{
			for (;;)
			{
				XmlNode xmlNode = this.source.ParentNode;
				if (xmlNode == null)
				{
					XmlAttribute xmlAttribute = this.source as XmlAttribute;
					if (xmlAttribute == null)
					{
						break;
					}
					xmlNode = (xmlAttribute.IsNamespace ? this.namespaceParent : xmlAttribute.OwnerElement);
					if (xmlNode == null)
					{
						break;
					}
				}
				this.source = xmlNode;
			}
			this.namespaceParent = null;
		}

		public override bool MoveTo(XPathNavigator other)
		{
			DocumentXPathNavigator documentXPathNavigator = other as DocumentXPathNavigator;
			if (documentXPathNavigator != null && this.document == documentXPathNavigator.document)
			{
				this.source = documentXPathNavigator.source;
				this.attributeIndex = documentXPathNavigator.attributeIndex;
				this.namespaceParent = documentXPathNavigator.namespaceParent;
				return true;
			}
			return false;
		}

		public override bool MoveToId(string id)
		{
			XmlElement elementById = this.document.GetElementById(id);
			if (elementById != null)
			{
				this.source = elementById;
				this.namespaceParent = null;
				return true;
			}
			return false;
		}

		public override bool MoveToChild(string localName, string namespaceUri)
		{
			if (this.source.NodeType == XmlNodeType.Attribute)
			{
				return false;
			}
			XmlNode xmlNode = this.FirstChild(this.source);
			if (xmlNode != null)
			{
				while (xmlNode.NodeType != XmlNodeType.Element || !(xmlNode.LocalName == localName) || !(xmlNode.NamespaceURI == namespaceUri))
				{
					xmlNode = this.NextSibling(xmlNode);
					if (xmlNode == null)
					{
						return false;
					}
				}
				this.source = xmlNode;
				return true;
			}
			return false;
		}

		public override bool MoveToChild(XPathNodeType type)
		{
			if (this.source.NodeType == XmlNodeType.Attribute)
			{
				return false;
			}
			XmlNode xmlNode = this.FirstChild(this.source);
			if (xmlNode != null)
			{
				int contentKindMask = XPathNavigator.GetContentKindMask(type);
				if (contentKindMask == 0)
				{
					return false;
				}
				while (((1 << (int)xmlNode.XPNodeType) & contentKindMask) == 0)
				{
					xmlNode = this.NextSibling(xmlNode);
					if (xmlNode == null)
					{
						return false;
					}
				}
				this.source = xmlNode;
				return true;
			}
			return false;
		}

		public override bool MoveToFollowing(string localName, string namespaceUri, XPathNavigator end)
		{
			XmlNode xmlNode = null;
			DocumentXPathNavigator documentXPathNavigator = end as DocumentXPathNavigator;
			if (documentXPathNavigator != null)
			{
				if (this.document != documentXPathNavigator.document)
				{
					return false;
				}
				if (documentXPathNavigator.source.NodeType == XmlNodeType.Attribute)
				{
					documentXPathNavigator = (DocumentXPathNavigator)documentXPathNavigator.Clone();
					if (!documentXPathNavigator.MoveToNonDescendant())
					{
						return false;
					}
				}
				xmlNode = documentXPathNavigator.source;
			}
			XmlNode xmlNode2 = this.source;
			if (xmlNode2.NodeType == XmlNodeType.Attribute)
			{
				xmlNode2 = ((XmlAttribute)xmlNode2).OwnerElement;
				if (xmlNode2 == null)
				{
					return false;
				}
			}
			for (;;)
			{
				XmlNode firstChild = xmlNode2.FirstChild;
				if (firstChild != null)
				{
					xmlNode2 = firstChild;
				}
				else
				{
					XmlNode nextSibling;
					for (;;)
					{
						nextSibling = xmlNode2.NextSibling;
						if (nextSibling != null)
						{
							break;
						}
						XmlNode parentNode = xmlNode2.ParentNode;
						if (parentNode == null)
						{
							return false;
						}
						xmlNode2 = parentNode;
					}
					xmlNode2 = nextSibling;
				}
				if (xmlNode2 == xmlNode)
				{
					return false;
				}
				if (xmlNode2.NodeType == XmlNodeType.Element && !(xmlNode2.LocalName != localName) && !(xmlNode2.NamespaceURI != namespaceUri))
				{
					goto Block_13;
				}
			}
			return false;
			Block_13:
			this.source = xmlNode2;
			return true;
		}

		public override bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
		{
			XmlNode xmlNode = null;
			DocumentXPathNavigator documentXPathNavigator = end as DocumentXPathNavigator;
			if (documentXPathNavigator != null)
			{
				if (this.document != documentXPathNavigator.document)
				{
					return false;
				}
				if (documentXPathNavigator.source.NodeType == XmlNodeType.Attribute)
				{
					documentXPathNavigator = (DocumentXPathNavigator)documentXPathNavigator.Clone();
					if (!documentXPathNavigator.MoveToNonDescendant())
					{
						return false;
					}
				}
				xmlNode = documentXPathNavigator.source;
			}
			int contentKindMask = XPathNavigator.GetContentKindMask(type);
			if (contentKindMask == 0)
			{
				return false;
			}
			XmlNode xmlNode2 = this.source;
			XmlNodeType nodeType = xmlNode2.NodeType;
			if (nodeType != XmlNodeType.Attribute)
			{
				if (nodeType - XmlNodeType.Text <= 1 || nodeType - XmlNodeType.Whitespace <= 1)
				{
					xmlNode2 = this.TextEnd(xmlNode2);
				}
			}
			else
			{
				xmlNode2 = ((XmlAttribute)xmlNode2).OwnerElement;
				if (xmlNode2 == null)
				{
					return false;
				}
			}
			for (;;)
			{
				XmlNode firstChild = xmlNode2.FirstChild;
				if (firstChild != null)
				{
					xmlNode2 = firstChild;
				}
				else
				{
					XmlNode nextSibling;
					for (;;)
					{
						nextSibling = xmlNode2.NextSibling;
						if (nextSibling != null)
						{
							break;
						}
						XmlNode parentNode = xmlNode2.ParentNode;
						if (parentNode == null)
						{
							return false;
						}
						xmlNode2 = parentNode;
					}
					xmlNode2 = nextSibling;
				}
				if (xmlNode2 == xmlNode)
				{
					return false;
				}
				if (((1 << (int)xmlNode2.XPNodeType) & contentKindMask) != 0)
				{
					goto Block_14;
				}
			}
			return false;
			Block_14:
			this.source = xmlNode2;
			return true;
		}

		public override bool MoveToNext(string localName, string namespaceUri)
		{
			XmlNode xmlNode = this.NextSibling(this.source);
			if (xmlNode == null)
			{
				return false;
			}
			while (xmlNode.NodeType != XmlNodeType.Element || !(xmlNode.LocalName == localName) || !(xmlNode.NamespaceURI == namespaceUri))
			{
				xmlNode = this.NextSibling(xmlNode);
				if (xmlNode == null)
				{
					return false;
				}
			}
			this.source = xmlNode;
			return true;
		}

		public override bool MoveToNext(XPathNodeType type)
		{
			XmlNode xmlNode = this.NextSibling(this.source);
			if (xmlNode == null)
			{
				return false;
			}
			if (xmlNode.IsText && this.source.IsText)
			{
				xmlNode = this.NextSibling(this.TextEnd(xmlNode));
				if (xmlNode == null)
				{
					return false;
				}
			}
			int contentKindMask = XPathNavigator.GetContentKindMask(type);
			if (contentKindMask == 0)
			{
				return false;
			}
			while (((1 << (int)xmlNode.XPNodeType) & contentKindMask) == 0)
			{
				xmlNode = this.NextSibling(xmlNode);
				if (xmlNode == null)
				{
					return false;
				}
			}
			this.source = xmlNode;
			return true;
		}

		public override bool HasChildren
		{
			get
			{
				XmlNodeType nodeType = this.source.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					return this.FirstChild(this.source) != null;
				}
				if (nodeType != XmlNodeType.Document && nodeType != XmlNodeType.DocumentFragment)
				{
					return false;
				}
				XmlNode xmlNode = this.FirstChild(this.source);
				if (xmlNode == null)
				{
					return false;
				}
				while (!DocumentXPathNavigator.IsValidChild(this.source, xmlNode))
				{
					xmlNode = this.NextSibling(xmlNode);
					if (xmlNode == null)
					{
						return false;
					}
				}
				return true;
			}
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			DocumentXPathNavigator documentXPathNavigator = other as DocumentXPathNavigator;
			if (documentXPathNavigator != null)
			{
				this.CalibrateText();
				documentXPathNavigator.CalibrateText();
				return this.source == documentXPathNavigator.source && this.namespaceParent == documentXPathNavigator.namespaceParent;
			}
			return false;
		}

		public override bool IsDescendant(XPathNavigator other)
		{
			DocumentXPathNavigator documentXPathNavigator = other as DocumentXPathNavigator;
			return documentXPathNavigator != null && DocumentXPathNavigator.IsDescendant(this.source, documentXPathNavigator.source);
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this.source.SchemaInfo;
			}
		}

		public override bool CheckValidity(XmlSchemaSet schemas, ValidationEventHandler validationEventHandler)
		{
			XmlDocument xmlDocument;
			if (this.source.NodeType == XmlNodeType.Document)
			{
				xmlDocument = (XmlDocument)this.source;
			}
			else
			{
				xmlDocument = this.source.OwnerDocument;
				if (schemas != null)
				{
					throw new ArgumentException(Res.GetString("An XmlSchemaSet is only allowed as a parameter on the Root node.", null));
				}
			}
			if (schemas == null && xmlDocument != null)
			{
				schemas = xmlDocument.Schemas;
			}
			if (schemas == null || schemas.Count == 0)
			{
				throw new InvalidOperationException(Res.GetString("The XmlSchemaSet on the document is either null or has no schemas in it. Provide schema information before calling Validate."));
			}
			return new DocumentSchemaValidator(xmlDocument, schemas, validationEventHandler)
			{
				PsviAugmentation = false
			}.Validate(this.source);
		}

		private static XmlNode OwnerNode(XmlNode node)
		{
			XmlNode parentNode = node.ParentNode;
			if (parentNode != null)
			{
				return parentNode;
			}
			XmlAttribute xmlAttribute = node as XmlAttribute;
			if (xmlAttribute != null)
			{
				return xmlAttribute.OwnerElement;
			}
			return null;
		}

		private static int GetDepth(XmlNode node)
		{
			int num = 0;
			for (XmlNode xmlNode = DocumentXPathNavigator.OwnerNode(node); xmlNode != null; xmlNode = DocumentXPathNavigator.OwnerNode(xmlNode))
			{
				num++;
			}
			return num;
		}

		private XmlNodeOrder Compare(XmlNode node1, XmlNode node2)
		{
			if (node1.XPNodeType == XPathNodeType.Attribute)
			{
				if (node2.XPNodeType == XPathNodeType.Attribute)
				{
					XmlElement ownerElement = ((XmlAttribute)node1).OwnerElement;
					if (ownerElement.HasAttributes)
					{
						XmlAttributeCollection attributes = ownerElement.Attributes;
						for (int i = 0; i < attributes.Count; i++)
						{
							XmlAttribute xmlAttribute = attributes[i];
							if (xmlAttribute == node1)
							{
								return XmlNodeOrder.Before;
							}
							if (xmlAttribute == node2)
							{
								return XmlNodeOrder.After;
							}
						}
					}
					return XmlNodeOrder.Unknown;
				}
				return XmlNodeOrder.Before;
			}
			else
			{
				if (node2.XPNodeType == XPathNodeType.Attribute)
				{
					return XmlNodeOrder.After;
				}
				XmlNode xmlNode = node1.NextSibling;
				while (xmlNode != null && xmlNode != node2)
				{
					xmlNode = xmlNode.NextSibling;
				}
				if (xmlNode == null)
				{
					return XmlNodeOrder.After;
				}
				return XmlNodeOrder.Before;
			}
		}

		public override XmlNodeOrder ComparePosition(XPathNavigator other)
		{
			DocumentXPathNavigator documentXPathNavigator = other as DocumentXPathNavigator;
			if (documentXPathNavigator == null)
			{
				return XmlNodeOrder.Unknown;
			}
			this.CalibrateText();
			documentXPathNavigator.CalibrateText();
			if (this.source == documentXPathNavigator.source && this.namespaceParent == documentXPathNavigator.namespaceParent)
			{
				return XmlNodeOrder.Same;
			}
			if (this.namespaceParent != null || documentXPathNavigator.namespaceParent != null)
			{
				return base.ComparePosition(other);
			}
			XmlNode xmlNode = this.source;
			XmlNode xmlNode2 = documentXPathNavigator.source;
			XmlNode xmlNode3 = DocumentXPathNavigator.OwnerNode(xmlNode);
			XmlNode xmlNode4 = DocumentXPathNavigator.OwnerNode(xmlNode2);
			if (xmlNode3 != xmlNode4)
			{
				int num = DocumentXPathNavigator.GetDepth(xmlNode);
				int num2 = DocumentXPathNavigator.GetDepth(xmlNode2);
				if (num2 > num)
				{
					while (xmlNode2 != null && num2 > num)
					{
						xmlNode2 = DocumentXPathNavigator.OwnerNode(xmlNode2);
						num2--;
					}
					if (xmlNode == xmlNode2)
					{
						return XmlNodeOrder.Before;
					}
					xmlNode4 = DocumentXPathNavigator.OwnerNode(xmlNode2);
				}
				else if (num > num2)
				{
					while (xmlNode != null && num > num2)
					{
						xmlNode = DocumentXPathNavigator.OwnerNode(xmlNode);
						num--;
					}
					if (xmlNode == xmlNode2)
					{
						return XmlNodeOrder.After;
					}
					xmlNode3 = DocumentXPathNavigator.OwnerNode(xmlNode);
				}
				while (xmlNode3 != null && xmlNode4 != null)
				{
					if (xmlNode3 == xmlNode4)
					{
						return this.Compare(xmlNode, xmlNode2);
					}
					xmlNode = xmlNode3;
					xmlNode2 = xmlNode4;
					xmlNode3 = DocumentXPathNavigator.OwnerNode(xmlNode);
					xmlNode4 = DocumentXPathNavigator.OwnerNode(xmlNode2);
				}
				return XmlNodeOrder.Unknown;
			}
			if (xmlNode3 == null)
			{
				return XmlNodeOrder.Unknown;
			}
			return this.Compare(xmlNode, xmlNode2);
		}

		XmlNode IHasXmlNode.GetNode()
		{
			return this.source;
		}

		public override XPathNodeIterator SelectDescendants(string localName, string namespaceURI, bool matchSelf)
		{
			string text = this.document.NameTable.Get(namespaceURI);
			if (text == null || this.source.NodeType == XmlNodeType.Attribute)
			{
				return new DocumentXPathNodeIterator_Empty(this);
			}
			string text2 = this.document.NameTable.Get(localName);
			if (text2 == null)
			{
				return new DocumentXPathNodeIterator_Empty(this);
			}
			if (text2.Length == 0)
			{
				if (matchSelf)
				{
					return new DocumentXPathNodeIterator_ElemChildren_AndSelf_NoLocalName(this, text);
				}
				return new DocumentXPathNodeIterator_ElemChildren_NoLocalName(this, text);
			}
			else
			{
				if (matchSelf)
				{
					return new DocumentXPathNodeIterator_ElemChildren_AndSelf(this, text2, text);
				}
				return new DocumentXPathNodeIterator_ElemChildren(this, text2, text);
			}
		}

		public override XPathNodeIterator SelectDescendants(XPathNodeType nt, bool includeSelf)
		{
			if (nt != XPathNodeType.Element)
			{
				return base.SelectDescendants(nt, includeSelf);
			}
			XmlNodeType nodeType = this.source.NodeType;
			if (nodeType != XmlNodeType.Document && nodeType != XmlNodeType.Element)
			{
				return new DocumentXPathNodeIterator_Empty(this);
			}
			if (includeSelf)
			{
				return new DocumentXPathNodeIterator_AllElemChildren_AndSelf(this);
			}
			return new DocumentXPathNodeIterator_AllElemChildren(this);
		}

		public override bool CanEdit
		{
			get
			{
				return true;
			}
		}

		public override XmlWriter PrependChild()
		{
			XmlNodeType nodeType = this.source.NodeType;
			if (nodeType != XmlNodeType.Element && nodeType != XmlNodeType.Document && nodeType != XmlNodeType.DocumentFragment)
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			DocumentXmlWriter documentXmlWriter = new DocumentXmlWriter(DocumentXmlWriterType.PrependChild, this.source, this.document);
			documentXmlWriter.NamespaceManager = DocumentXPathNavigator.GetNamespaceManager(this.source, this.document);
			return new XmlWellFormedWriter(documentXmlWriter, documentXmlWriter.Settings);
		}

		public override XmlWriter AppendChild()
		{
			XmlNodeType nodeType = this.source.NodeType;
			if (nodeType != XmlNodeType.Element && nodeType != XmlNodeType.Document && nodeType != XmlNodeType.DocumentFragment)
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			DocumentXmlWriter documentXmlWriter = new DocumentXmlWriter(DocumentXmlWriterType.AppendChild, this.source, this.document);
			documentXmlWriter.NamespaceManager = DocumentXPathNavigator.GetNamespaceManager(this.source, this.document);
			return new XmlWellFormedWriter(documentXmlWriter, documentXmlWriter.Settings);
		}

		public override XmlWriter InsertAfter()
		{
			XmlNode xmlNode = this.source;
			switch (xmlNode.NodeType)
			{
			case XmlNodeType.Attribute:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				xmlNode = this.TextEnd(xmlNode);
				break;
			}
			DocumentXmlWriter documentXmlWriter = new DocumentXmlWriter(DocumentXmlWriterType.InsertSiblingAfter, xmlNode, this.document);
			documentXmlWriter.NamespaceManager = DocumentXPathNavigator.GetNamespaceManager(xmlNode.ParentNode, this.document);
			return new XmlWellFormedWriter(documentXmlWriter, documentXmlWriter.Settings);
		}

		public override XmlWriter InsertBefore()
		{
			switch (this.source.NodeType)
			{
			case XmlNodeType.Attribute:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.CalibrateText();
				break;
			}
			DocumentXmlWriter documentXmlWriter = new DocumentXmlWriter(DocumentXmlWriterType.InsertSiblingBefore, this.source, this.document);
			documentXmlWriter.NamespaceManager = DocumentXPathNavigator.GetNamespaceManager(this.source.ParentNode, this.document);
			return new XmlWellFormedWriter(documentXmlWriter, documentXmlWriter.Settings);
		}

		public override XmlWriter CreateAttributes()
		{
			if (this.source.NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			DocumentXmlWriter documentXmlWriter = new DocumentXmlWriter(DocumentXmlWriterType.AppendAttribute, this.source, this.document);
			documentXmlWriter.NamespaceManager = DocumentXPathNavigator.GetNamespaceManager(this.source, this.document);
			return new XmlWellFormedWriter(documentXmlWriter, documentXmlWriter.Settings);
		}

		public override XmlWriter ReplaceRange(XPathNavigator lastSiblingToReplace)
		{
			DocumentXPathNavigator documentXPathNavigator = lastSiblingToReplace as DocumentXPathNavigator;
			if (documentXPathNavigator != null)
			{
				this.CalibrateText();
				documentXPathNavigator.CalibrateText();
				XmlNode xmlNode = this.source;
				XmlNode xmlNode2 = documentXPathNavigator.source;
				if (xmlNode == xmlNode2)
				{
					switch (xmlNode.NodeType)
					{
					case XmlNodeType.Attribute:
					case XmlNodeType.Document:
					case XmlNodeType.DocumentFragment:
						throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						xmlNode2 = documentXPathNavigator.TextEnd(xmlNode2);
						break;
					}
				}
				else
				{
					if (xmlNode2.IsText)
					{
						xmlNode2 = documentXPathNavigator.TextEnd(xmlNode2);
					}
					if (!DocumentXPathNavigator.IsFollowingSibling(xmlNode, xmlNode2))
					{
						throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
					}
				}
				DocumentXmlWriter documentXmlWriter = new DocumentXmlWriter(DocumentXmlWriterType.ReplaceToFollowingSibling, xmlNode, this.document);
				documentXmlWriter.NamespaceManager = DocumentXPathNavigator.GetNamespaceManager(xmlNode.ParentNode, this.document);
				documentXmlWriter.Navigator = this;
				documentXmlWriter.EndNode = xmlNode2;
				return new XmlWellFormedWriter(documentXmlWriter, documentXmlWriter.Settings);
			}
			if (lastSiblingToReplace == null)
			{
				throw new ArgumentNullException("lastSiblingToReplace");
			}
			throw new NotSupportedException();
		}

		public override void DeleteRange(XPathNavigator lastSiblingToDelete)
		{
			DocumentXPathNavigator documentXPathNavigator = lastSiblingToDelete as DocumentXPathNavigator;
			if (documentXPathNavigator != null)
			{
				this.CalibrateText();
				documentXPathNavigator.CalibrateText();
				XmlNode xmlNode = this.source;
				XmlNode xmlNode2 = documentXPathNavigator.source;
				if (xmlNode == xmlNode2)
				{
					XmlNode xmlNode3;
					switch (xmlNode.NodeType)
					{
					case XmlNodeType.Element:
					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.Comment:
						break;
					case XmlNodeType.Attribute:
					{
						XmlAttribute xmlAttribute = (XmlAttribute)xmlNode;
						if (xmlAttribute.IsNamespace)
						{
							goto IL_00E1;
						}
						xmlNode3 = DocumentXPathNavigator.OwnerNode(xmlAttribute);
						DocumentXPathNavigator.DeleteAttribute(xmlAttribute, this.attributeIndex);
						if (xmlNode3 != null)
						{
							this.ResetPosition(xmlNode3);
							return;
						}
						return;
					}
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						xmlNode2 = documentXPathNavigator.TextEnd(xmlNode2);
						break;
					case XmlNodeType.EntityReference:
					case XmlNodeType.Entity:
					case XmlNodeType.Document:
					case XmlNodeType.DocumentType:
					case XmlNodeType.DocumentFragment:
					case XmlNodeType.Notation:
						goto IL_00E1;
					default:
						goto IL_00E1;
					}
					xmlNode3 = DocumentXPathNavigator.OwnerNode(xmlNode);
					DocumentXPathNavigator.DeleteToFollowingSibling(xmlNode, xmlNode2);
					if (xmlNode3 != null)
					{
						this.ResetPosition(xmlNode3);
						return;
					}
					return;
					IL_00E1:
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
				}
				if (xmlNode2.IsText)
				{
					xmlNode2 = documentXPathNavigator.TextEnd(xmlNode2);
				}
				if (!DocumentXPathNavigator.IsFollowingSibling(xmlNode, xmlNode2))
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
				}
				XmlNode xmlNode4 = DocumentXPathNavigator.OwnerNode(xmlNode);
				DocumentXPathNavigator.DeleteToFollowingSibling(xmlNode, xmlNode2);
				if (xmlNode4 != null)
				{
					this.ResetPosition(xmlNode4);
				}
				return;
			}
			if (lastSiblingToDelete == null)
			{
				throw new ArgumentNullException("lastSiblingToDelete");
			}
			throw new NotSupportedException();
		}

		public override void DeleteSelf()
		{
			XmlNode xmlNode = this.source;
			XmlNode xmlNode2 = xmlNode;
			XmlNode xmlNode3;
			switch (xmlNode.NodeType)
			{
			case XmlNodeType.Element:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
				break;
			case XmlNodeType.Attribute:
			{
				XmlAttribute xmlAttribute = (XmlAttribute)xmlNode;
				if (xmlAttribute.IsNamespace)
				{
					goto IL_00AF;
				}
				xmlNode3 = DocumentXPathNavigator.OwnerNode(xmlAttribute);
				DocumentXPathNavigator.DeleteAttribute(xmlAttribute, this.attributeIndex);
				if (xmlNode3 != null)
				{
					this.ResetPosition(xmlNode3);
					return;
				}
				return;
			}
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.CalibrateText();
				xmlNode = this.source;
				xmlNode2 = this.TextEnd(xmlNode);
				break;
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentType:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				goto IL_00AF;
			default:
				goto IL_00AF;
			}
			xmlNode3 = DocumentXPathNavigator.OwnerNode(xmlNode);
			DocumentXPathNavigator.DeleteToFollowingSibling(xmlNode, xmlNode2);
			if (xmlNode3 != null)
			{
				this.ResetPosition(xmlNode3);
				return;
			}
			return;
			IL_00AF:
			throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
		}

		private static void DeleteAttribute(XmlAttribute attribute, int index)
		{
			XmlAttributeCollection xmlAttributeCollection;
			if (!DocumentXPathNavigator.CheckAttributePosition(attribute, out xmlAttributeCollection, index) && !DocumentXPathNavigator.ResetAttributePosition(attribute, xmlAttributeCollection, out index))
			{
				throw new InvalidOperationException(Res.GetString("The current position of the navigator is missing a valid parent."));
			}
			if (attribute.IsReadOnly)
			{
				throw new InvalidOperationException(Res.GetString("This node is read-only. It cannot be modified."));
			}
			xmlAttributeCollection.RemoveAt(index);
		}

		internal static void DeleteToFollowingSibling(XmlNode node, XmlNode end)
		{
			XmlNode parentNode = node.ParentNode;
			if (parentNode == null)
			{
				throw new InvalidOperationException(Res.GetString("The current position of the navigator is missing a valid parent."));
			}
			if (node.IsReadOnly || end.IsReadOnly)
			{
				throw new InvalidOperationException(Res.GetString("This node is read-only. It cannot be modified."));
			}
			while (node != end)
			{
				XmlNode xmlNode = node;
				node = node.NextSibling;
				parentNode.RemoveChild(xmlNode);
			}
			parentNode.RemoveChild(node);
		}

		private static XmlNamespaceManager GetNamespaceManager(XmlNode node, XmlDocument document)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(document.NameTable);
			List<XmlElement> list = new List<XmlElement>();
			while (node != null)
			{
				XmlElement xmlElement = node as XmlElement;
				if (xmlElement != null && xmlElement.HasAttributes)
				{
					list.Add(xmlElement);
				}
				node = node.ParentNode;
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				xmlNamespaceManager.PushScope();
				XmlAttributeCollection attributes = list[i].Attributes;
				for (int j = 0; j < attributes.Count; j++)
				{
					XmlAttribute xmlAttribute = attributes[j];
					if (xmlAttribute.IsNamespace)
					{
						string text = ((xmlAttribute.Prefix.Length == 0) ? string.Empty : xmlAttribute.LocalName);
						xmlNamespaceManager.AddNamespace(text, xmlAttribute.Value);
					}
				}
			}
			return xmlNamespaceManager;
		}

		internal void ResetPosition(XmlNode node)
		{
			this.source = node;
			XmlAttribute xmlAttribute = node as XmlAttribute;
			if (xmlAttribute != null)
			{
				XmlElement ownerElement = xmlAttribute.OwnerElement;
				if (ownerElement != null)
				{
					DocumentXPathNavigator.ResetAttributePosition(xmlAttribute, ownerElement.Attributes, out this.attributeIndex);
					if (xmlAttribute.IsNamespace)
					{
						this.namespaceParent = ownerElement;
					}
				}
			}
		}

		private static bool ResetAttributePosition(XmlAttribute attribute, XmlAttributeCollection attributes, out int index)
		{
			if (attributes != null)
			{
				for (int i = 0; i < attributes.Count; i++)
				{
					if (attribute == attributes[i])
					{
						index = i;
						return true;
					}
				}
			}
			index = 0;
			return false;
		}

		private static bool CheckAttributePosition(XmlAttribute attribute, out XmlAttributeCollection attributes, int index)
		{
			XmlElement ownerElement = attribute.OwnerElement;
			if (ownerElement != null)
			{
				attributes = ownerElement.Attributes;
				if (index >= 0 && index < attributes.Count && attribute == attributes[index])
				{
					return true;
				}
			}
			else
			{
				attributes = null;
			}
			return false;
		}

		private void CalibrateText()
		{
			for (XmlNode xmlNode = this.PreviousText(this.source); xmlNode != null; xmlNode = this.PreviousText(xmlNode))
			{
				this.ResetPosition(xmlNode);
			}
		}

		private XmlNode ParentNode(XmlNode node)
		{
			XmlNode parentNode = node.ParentNode;
			if (!this.document.HasEntityReferences)
			{
				return parentNode;
			}
			return this.ParentNodeTail(parentNode);
		}

		private XmlNode ParentNodeTail(XmlNode parent)
		{
			while (parent != null && parent.NodeType == XmlNodeType.EntityReference)
			{
				parent = parent.ParentNode;
			}
			return parent;
		}

		private XmlNode FirstChild(XmlNode node)
		{
			XmlNode firstChild = node.FirstChild;
			if (!this.document.HasEntityReferences)
			{
				return firstChild;
			}
			return this.FirstChildTail(firstChild);
		}

		private XmlNode FirstChildTail(XmlNode child)
		{
			while (child != null && child.NodeType == XmlNodeType.EntityReference)
			{
				child = child.FirstChild;
			}
			return child;
		}

		private XmlNode NextSibling(XmlNode node)
		{
			XmlNode nextSibling = node.NextSibling;
			if (!this.document.HasEntityReferences)
			{
				return nextSibling;
			}
			return this.NextSiblingTail(node, nextSibling);
		}

		private XmlNode NextSiblingTail(XmlNode node, XmlNode sibling)
		{
			while (sibling == null)
			{
				node = node.ParentNode;
				if (node == null || node.NodeType != XmlNodeType.EntityReference)
				{
					return null;
				}
				sibling = node.NextSibling;
			}
			while (sibling != null && sibling.NodeType == XmlNodeType.EntityReference)
			{
				sibling = sibling.FirstChild;
			}
			return sibling;
		}

		private XmlNode PreviousSibling(XmlNode node)
		{
			XmlNode previousSibling = node.PreviousSibling;
			if (!this.document.HasEntityReferences)
			{
				return previousSibling;
			}
			return this.PreviousSiblingTail(node, previousSibling);
		}

		private XmlNode PreviousSiblingTail(XmlNode node, XmlNode sibling)
		{
			while (sibling == null)
			{
				node = node.ParentNode;
				if (node == null || node.NodeType != XmlNodeType.EntityReference)
				{
					return null;
				}
				sibling = node.PreviousSibling;
			}
			while (sibling != null && sibling.NodeType == XmlNodeType.EntityReference)
			{
				sibling = sibling.LastChild;
			}
			return sibling;
		}

		private XmlNode PreviousText(XmlNode node)
		{
			XmlNode previousText = node.PreviousText;
			if (!this.document.HasEntityReferences)
			{
				return previousText;
			}
			return this.PreviousTextTail(node, previousText);
		}

		private XmlNode PreviousTextTail(XmlNode node, XmlNode text)
		{
			if (text != null)
			{
				return text;
			}
			if (!node.IsText)
			{
				return null;
			}
			XmlNode xmlNode;
			for (xmlNode = node.PreviousSibling; xmlNode == null; xmlNode = node.PreviousSibling)
			{
				node = node.ParentNode;
				if (node == null || node.NodeType != XmlNodeType.EntityReference)
				{
					return null;
				}
			}
			while (xmlNode != null)
			{
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType - XmlNodeType.Text > 1)
				{
					if (nodeType == XmlNodeType.EntityReference)
					{
						xmlNode = xmlNode.LastChild;
						continue;
					}
					if (nodeType - XmlNodeType.Whitespace > 1)
					{
						return null;
					}
				}
				return xmlNode;
			}
			return null;
		}

		internal static bool IsFollowingSibling(XmlNode left, XmlNode right)
		{
			do
			{
				left = left.NextSibling;
				if (left == null)
				{
					return false;
				}
			}
			while (left != right);
			return true;
		}

		private static bool IsDescendant(XmlNode top, XmlNode bottom)
		{
			do
			{
				XmlNode xmlNode = bottom.ParentNode;
				if (xmlNode == null)
				{
					XmlAttribute xmlAttribute = bottom as XmlAttribute;
					if (xmlAttribute == null)
					{
						return false;
					}
					xmlNode = xmlAttribute.OwnerElement;
					if (xmlNode == null)
					{
						return false;
					}
				}
				bottom = xmlNode;
			}
			while (top != bottom);
			return true;
		}

		private static bool IsValidChild(XmlNode parent, XmlNode child)
		{
			XmlNodeType nodeType = parent.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Document)
				{
					if (nodeType == XmlNodeType.DocumentFragment)
					{
						XmlNodeType xmlNodeType = child.NodeType;
						switch (xmlNodeType)
						{
						case XmlNodeType.Element:
						case XmlNodeType.Text:
						case XmlNodeType.CDATA:
						case XmlNodeType.ProcessingInstruction:
						case XmlNodeType.Comment:
							break;
						case XmlNodeType.Attribute:
						case XmlNodeType.EntityReference:
						case XmlNodeType.Entity:
							return false;
						default:
							if (xmlNodeType - XmlNodeType.Whitespace > 1)
							{
								return false;
							}
							break;
						}
						return true;
					}
				}
				else
				{
					XmlNodeType xmlNodeType = child.NodeType;
					if (xmlNodeType == XmlNodeType.Element || xmlNodeType - XmlNodeType.ProcessingInstruction <= 1)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private XmlNode TextStart(XmlNode node)
		{
			XmlNode xmlNode;
			do
			{
				xmlNode = node;
				node = this.PreviousSibling(node);
			}
			while (node != null && node.IsText);
			return xmlNode;
		}

		private XmlNode TextEnd(XmlNode node)
		{
			XmlNode xmlNode;
			do
			{
				xmlNode = node;
				node = this.NextSibling(node);
			}
			while (node != null && node.IsText);
			return xmlNode;
		}

		private XmlDocument document;

		private XmlNode source;

		private int attributeIndex;

		private XmlElement namespaceParent;
	}
}
