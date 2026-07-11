using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	internal abstract class XmlBaseReader : XmlDictionaryReader
	{
		protected XmlBaseReader()
		{
			this.bufferReader = new XmlBufferReader(this);
			this.nsMgr = new XmlBaseReader.NamespaceManager(this.bufferReader);
			this.quotas = new XmlDictionaryReaderQuotas();
			this.rootElementNode = new XmlBaseReader.XmlElementNode(this.bufferReader);
			this.atomicTextNode = new XmlBaseReader.XmlAtomicTextNode(this.bufferReader);
			this.node = XmlBaseReader.closedNode;
		}

		private static BinHexEncoding BinHexEncoding
		{
			get
			{
				if (XmlBaseReader.binhexEncoding == null)
				{
					XmlBaseReader.binhexEncoding = new BinHexEncoding();
				}
				return XmlBaseReader.binhexEncoding;
			}
		}

		private static Base64Encoding Base64Encoding
		{
			get
			{
				if (XmlBaseReader.base64Encoding == null)
				{
					XmlBaseReader.base64Encoding = new Base64Encoding();
				}
				return XmlBaseReader.base64Encoding;
			}
		}

		protected XmlBufferReader BufferReader
		{
			get
			{
				return this.bufferReader;
			}
		}

		public override XmlDictionaryReaderQuotas Quotas
		{
			get
			{
				return this.quotas;
			}
		}

		protected XmlBaseReader.XmlNode Node
		{
			get
			{
				return this.node;
			}
		}

		protected void MoveToNode(XmlBaseReader.XmlNode node)
		{
			this.node = node;
			this.ns = null;
			this.localName = null;
			this.prefix = null;
			this.value = null;
		}

		protected void MoveToInitial(XmlDictionaryReaderQuotas quotas)
		{
			if (quotas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("quotas");
			}
			quotas.InternalCopyTo(this.quotas);
			this.quotas.MakeReadOnly();
			this.nsMgr.Clear();
			this.depth = 0;
			this.attributeCount = 0;
			this.attributeStart = -1;
			this.attributeIndex = -1;
			this.rootElement = false;
			this.readingElement = false;
			this.signing = false;
			this.MoveToNode(XmlBaseReader.initialNode);
		}

		protected XmlBaseReader.XmlDeclarationNode MoveToDeclaration()
		{
			if (this.attributeCount < 1)
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Version not found in XML declaration.")));
			}
			if (this.attributeCount > 3)
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Malformed XML declaration.")));
			}
			if (!this.CheckDeclAttribute(0, "version", "1.0", false, "XML version must be '1.0'."))
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Version not found in XML declaration.")));
			}
			if (this.attributeCount > 1)
			{
				if (this.CheckDeclAttribute(1, "encoding", null, true, "XML encoding must be 'UTF-8'."))
				{
					if (this.attributeCount == 3 && !this.CheckStandalone(2))
					{
						XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Malformed XML declaration.")));
					}
				}
				else if (!this.CheckStandalone(1) || this.attributeCount > 2)
				{
					XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Malformed XML declaration.")));
				}
			}
			if (this.declarationNode == null)
			{
				this.declarationNode = new XmlBaseReader.XmlDeclarationNode(this.bufferReader);
			}
			this.MoveToNode(this.declarationNode);
			return this.declarationNode;
		}

		private bool CheckStandalone(int attr)
		{
			XmlBaseReader.XmlAttributeNode xmlAttributeNode = this.attributeNodes[attr];
			if (!xmlAttributeNode.Prefix.IsEmpty)
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Malformed XML declaration.")));
			}
			if (xmlAttributeNode.LocalName != "standalone")
			{
				return false;
			}
			if (!xmlAttributeNode.Value.Equals2("yes", false) && !xmlAttributeNode.Value.Equals2("no", false))
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("'standalone' value in declaration must be 'yes' or 'no'.")));
			}
			return true;
		}

		private bool CheckDeclAttribute(int index, string localName, string value, bool checkLower, string valueSR)
		{
			XmlBaseReader.XmlAttributeNode xmlAttributeNode = this.attributeNodes[index];
			if (!xmlAttributeNode.Prefix.IsEmpty)
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Malformed XML declaration.")));
			}
			if (xmlAttributeNode.LocalName != localName)
			{
				return false;
			}
			if (value != null && !xmlAttributeNode.Value.Equals2(value, checkLower))
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString(valueSR)));
			}
			return true;
		}

		protected XmlBaseReader.XmlCommentNode MoveToComment()
		{
			if (this.commentNode == null)
			{
				this.commentNode = new XmlBaseReader.XmlCommentNode(this.bufferReader);
			}
			this.MoveToNode(this.commentNode);
			return this.commentNode;
		}

		protected XmlBaseReader.XmlCDataNode MoveToCData()
		{
			if (this.cdataNode == null)
			{
				this.cdataNode = new XmlBaseReader.XmlCDataNode(this.bufferReader);
			}
			this.MoveToNode(this.cdataNode);
			return this.cdataNode;
		}

		protected XmlBaseReader.XmlAtomicTextNode MoveToAtomicText()
		{
			XmlBaseReader.XmlAtomicTextNode xmlAtomicTextNode = this.atomicTextNode;
			this.MoveToNode(xmlAtomicTextNode);
			return xmlAtomicTextNode;
		}

		protected XmlBaseReader.XmlComplexTextNode MoveToComplexText()
		{
			if (this.complexTextNode == null)
			{
				this.complexTextNode = new XmlBaseReader.XmlComplexTextNode(this.bufferReader);
			}
			this.MoveToNode(this.complexTextNode);
			return this.complexTextNode;
		}

		protected XmlBaseReader.XmlTextNode MoveToWhitespaceText()
		{
			if (this.whitespaceTextNode == null)
			{
				this.whitespaceTextNode = new XmlBaseReader.XmlWhitespaceTextNode(this.bufferReader);
			}
			if (this.nsMgr.XmlSpace == XmlSpace.Preserve)
			{
				this.whitespaceTextNode.NodeType = XmlNodeType.SignificantWhitespace;
			}
			else
			{
				this.whitespaceTextNode.NodeType = XmlNodeType.Whitespace;
			}
			this.MoveToNode(this.whitespaceTextNode);
			return this.whitespaceTextNode;
		}

		protected XmlBaseReader.XmlElementNode ElementNode
		{
			get
			{
				if (this.depth == 0)
				{
					return this.rootElementNode;
				}
				return this.elementNodes[this.depth];
			}
		}

		protected void MoveToEndElement()
		{
			if (this.depth == 0)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this);
			}
			XmlBaseReader.XmlElementNode xmlElementNode = this.elementNodes[this.depth];
			XmlBaseReader.XmlEndElementNode endElement = xmlElementNode.EndElement;
			endElement.Namespace = xmlElementNode.Namespace;
			this.MoveToNode(endElement);
		}

		protected void MoveToEndOfFile()
		{
			if (this.depth != 0)
			{
				XmlExceptionHelper.ThrowUnexpectedEndOfFile(this);
			}
			this.MoveToNode(XmlBaseReader.endOfFileNode);
		}

		protected XmlBaseReader.XmlElementNode EnterScope()
		{
			if (this.depth == 0)
			{
				if (this.rootElement)
				{
					XmlExceptionHelper.ThrowMultipleRootElements(this);
				}
				this.rootElement = true;
			}
			this.nsMgr.EnterScope();
			this.depth++;
			if (this.depth > this.quotas.MaxDepth)
			{
				XmlExceptionHelper.ThrowMaxDepthExceeded(this, this.quotas.MaxDepth);
			}
			if (this.elementNodes == null)
			{
				this.elementNodes = new XmlBaseReader.XmlElementNode[4];
			}
			else if (this.elementNodes.Length == this.depth)
			{
				XmlBaseReader.XmlElementNode[] array = new XmlBaseReader.XmlElementNode[this.depth * 2];
				Array.Copy(this.elementNodes, array, this.depth);
				this.elementNodes = array;
			}
			XmlBaseReader.XmlElementNode xmlElementNode = this.elementNodes[this.depth];
			if (xmlElementNode == null)
			{
				xmlElementNode = new XmlBaseReader.XmlElementNode(this.bufferReader);
				this.elementNodes[this.depth] = xmlElementNode;
			}
			this.attributeCount = 0;
			this.attributeStart = -1;
			this.attributeIndex = -1;
			this.MoveToNode(xmlElementNode);
			return xmlElementNode;
		}

		protected void ExitScope()
		{
			if (this.depth == 0)
			{
				XmlExceptionHelper.ThrowUnexpectedEndElement(this);
			}
			this.depth--;
			this.nsMgr.ExitScope();
		}

		private XmlBaseReader.XmlAttributeNode AddAttribute(XmlBaseReader.QNameType qnameType, bool isAtomicValue)
		{
			int num = this.attributeCount;
			if (this.attributeNodes == null)
			{
				this.attributeNodes = new XmlBaseReader.XmlAttributeNode[4];
			}
			else if (this.attributeNodes.Length == num)
			{
				XmlBaseReader.XmlAttributeNode[] array = new XmlBaseReader.XmlAttributeNode[num * 2];
				Array.Copy(this.attributeNodes, array, num);
				this.attributeNodes = array;
			}
			XmlBaseReader.XmlAttributeNode xmlAttributeNode = this.attributeNodes[num];
			if (xmlAttributeNode == null)
			{
				xmlAttributeNode = new XmlBaseReader.XmlAttributeNode(this.bufferReader);
				this.attributeNodes[num] = xmlAttributeNode;
			}
			xmlAttributeNode.QNameType = qnameType;
			xmlAttributeNode.IsAtomicValue = isAtomicValue;
			xmlAttributeNode.AttributeText.QNameType = qnameType;
			xmlAttributeNode.AttributeText.IsAtomicValue = isAtomicValue;
			this.attributeCount++;
			return xmlAttributeNode;
		}

		protected XmlBaseReader.Namespace AddNamespace()
		{
			return this.nsMgr.AddNamespace();
		}

		protected XmlBaseReader.XmlAttributeNode AddAttribute()
		{
			return this.AddAttribute(XmlBaseReader.QNameType.Normal, true);
		}

		protected XmlBaseReader.XmlAttributeNode AddXmlAttribute()
		{
			return this.AddAttribute(XmlBaseReader.QNameType.Normal, true);
		}

		protected XmlBaseReader.XmlAttributeNode AddXmlnsAttribute(XmlBaseReader.Namespace ns)
		{
			if (!ns.Prefix.IsEmpty && ns.Uri.IsEmpty)
			{
				XmlExceptionHelper.ThrowEmptyNamespace(this);
			}
			if (ns.Prefix.IsXml && ns.Uri != "http://www.w3.org/XML/1998/namespace")
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' can only be bound to the namespace '{1}'.", new object[] { "xml", "http://www.w3.org/XML/1998/namespace" })));
			}
			else if (ns.Prefix.IsXmlns && ns.Uri != "http://www.w3.org/2000/xmlns/")
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' can only be bound to the namespace '{1}'.", new object[] { "xmlns", "http://www.w3.org/2000/xmlns/" })));
			}
			this.nsMgr.Register(ns);
			XmlBaseReader.XmlAttributeNode xmlAttributeNode = this.AddAttribute(XmlBaseReader.QNameType.Xmlns, false);
			xmlAttributeNode.Namespace = ns;
			xmlAttributeNode.AttributeText.Namespace = ns;
			return xmlAttributeNode;
		}

		protected void FixXmlAttribute(XmlBaseReader.XmlAttributeNode attributeNode)
		{
			if (attributeNode.Prefix == "xml")
			{
				if (attributeNode.LocalName == "lang")
				{
					this.nsMgr.AddLangAttribute(attributeNode.Value.GetString());
					return;
				}
				if (attributeNode.LocalName == "space")
				{
					string @string = attributeNode.Value.GetString();
					if (@string == "preserve")
					{
						this.nsMgr.AddSpaceAttribute(XmlSpace.Preserve);
						return;
					}
					if (@string == "default")
					{
						this.nsMgr.AddSpaceAttribute(XmlSpace.Default);
					}
				}
			}
		}

		protected bool OutsideRootElement
		{
			get
			{
				return this.depth == 0;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override bool CanReadValueChunk
		{
			get
			{
				return true;
			}
		}

		public override string BaseURI
		{
			get
			{
				return string.Empty;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.node.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return false;
			}
		}

		public override string this[int index]
		{
			get
			{
				return this.GetAttribute(index);
			}
		}

		public override string this[string name]
		{
			get
			{
				return this.GetAttribute(name);
			}
		}

		public override string this[string localName, string namespaceUri]
		{
			get
			{
				return this.GetAttribute(localName, namespaceUri);
			}
		}

		public override int AttributeCount
		{
			get
			{
				if (this.node.CanGetAttribute)
				{
					return this.attributeCount;
				}
				return 0;
			}
		}

		public override void Close()
		{
			this.MoveToNode(XmlBaseReader.closedNode);
			this.nameTable = null;
			if (this.attributeNodes != null && this.attributeNodes.Length > 16)
			{
				this.attributeNodes = null;
			}
			if (this.elementNodes != null && this.elementNodes.Length > 16)
			{
				this.elementNodes = null;
			}
			this.nsMgr.Close();
			this.bufferReader.Close();
			if (this.signingWriter != null)
			{
				this.signingWriter.Close();
			}
			if (this.attributeSorter != null)
			{
				this.attributeSorter.Close();
			}
		}

		public sealed override int Depth
		{
			get
			{
				return this.depth + this.node.DepthDelta;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.node.ReadState == ReadState.EndOfFile;
			}
		}

		private XmlBaseReader.XmlAttributeNode GetAttributeNode(int index)
		{
			if (!this.node.CanGetAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("Only Element nodes have attributes.")));
			}
			if (index < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (index >= this.attributeCount)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { this.attributeCount })));
			}
			return this.attributeNodes[index];
		}

		private XmlBaseReader.XmlAttributeNode GetAttributeNode(string name)
		{
			if (name == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("name"));
			}
			if (!this.node.CanGetAttribute)
			{
				return null;
			}
			int num = name.IndexOf(':');
			string text;
			string text2;
			if (num == -1)
			{
				if (name == "xmlns")
				{
					text = "xmlns";
					text2 = string.Empty;
				}
				else
				{
					text = string.Empty;
					text2 = name;
				}
			}
			else
			{
				text = name.Substring(0, num);
				text2 = name.Substring(num + 1);
			}
			XmlBaseReader.XmlAttributeNode[] array = this.attributeNodes;
			int num2 = this.attributeCount;
			int num3 = this.attributeStart;
			for (int i = 0; i < num2; i++)
			{
				if (++num3 >= num2)
				{
					num3 = 0;
				}
				XmlBaseReader.XmlAttributeNode xmlAttributeNode = array[num3];
				if (xmlAttributeNode.IsPrefixAndLocalName(text, text2))
				{
					this.attributeStart = num3;
					return xmlAttributeNode;
				}
			}
			return null;
		}

		private XmlBaseReader.XmlAttributeNode GetAttributeNode(string localName, string namespaceUri)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (namespaceUri == null)
			{
				namespaceUri = string.Empty;
			}
			if (!this.node.CanGetAttribute)
			{
				return null;
			}
			XmlBaseReader.XmlAttributeNode[] array = this.attributeNodes;
			int num = this.attributeCount;
			int num2 = this.attributeStart;
			for (int i = 0; i < num; i++)
			{
				if (++num2 >= num)
				{
					num2 = 0;
				}
				XmlBaseReader.XmlAttributeNode xmlAttributeNode = array[num2];
				if (xmlAttributeNode.IsLocalNameAndNamespaceUri(localName, namespaceUri))
				{
					this.attributeStart = num2;
					return xmlAttributeNode;
				}
			}
			return null;
		}

		private XmlBaseReader.XmlAttributeNode GetAttributeNode(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (namespaceUri == null)
			{
				namespaceUri = XmlDictionaryString.Empty;
			}
			if (!this.node.CanGetAttribute)
			{
				return null;
			}
			XmlBaseReader.XmlAttributeNode[] array = this.attributeNodes;
			int num = this.attributeCount;
			int num2 = this.attributeStart;
			for (int i = 0; i < num; i++)
			{
				if (++num2 >= num)
				{
					num2 = 0;
				}
				XmlBaseReader.XmlAttributeNode xmlAttributeNode = array[num2];
				if (xmlAttributeNode.IsLocalNameAndNamespaceUri(localName, namespaceUri))
				{
					this.attributeStart = num2;
					return xmlAttributeNode;
				}
			}
			return null;
		}

		public override string GetAttribute(int index)
		{
			return this.GetAttributeNode(index).ValueAsString;
		}

		public override string GetAttribute(string name)
		{
			XmlBaseReader.XmlAttributeNode attributeNode = this.GetAttributeNode(name);
			if (attributeNode == null)
			{
				return null;
			}
			return attributeNode.ValueAsString;
		}

		public override string GetAttribute(string localName, string namespaceUri)
		{
			XmlBaseReader.XmlAttributeNode attributeNode = this.GetAttributeNode(localName, namespaceUri);
			if (attributeNode == null)
			{
				return null;
			}
			return attributeNode.ValueAsString;
		}

		public override string GetAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			XmlBaseReader.XmlAttributeNode attributeNode = this.GetAttributeNode(localName, namespaceUri);
			if (attributeNode == null)
			{
				return null;
			}
			return attributeNode.ValueAsString;
		}

		public sealed override bool IsEmptyElement
		{
			get
			{
				return this.node.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				if (this.localName == null)
				{
					this.localName = this.GetLocalName(true);
				}
				return this.localName;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			XmlBaseReader.Namespace @namespace = this.nsMgr.LookupNamespace(prefix);
			if (@namespace != null)
			{
				return @namespace.Uri.GetString(this.NameTable);
			}
			if (prefix == "xmlns")
			{
				return "http://www.w3.org/2000/xmlns/";
			}
			return null;
		}

		protected XmlBaseReader.Namespace LookupNamespace(PrefixHandleType prefix)
		{
			XmlBaseReader.Namespace @namespace = this.nsMgr.LookupNamespace(prefix);
			if (@namespace == null)
			{
				XmlExceptionHelper.ThrowUndefinedPrefix(this, PrefixHandle.GetString(prefix));
			}
			return @namespace;
		}

		protected XmlBaseReader.Namespace LookupNamespace(PrefixHandle prefix)
		{
			XmlBaseReader.Namespace @namespace = this.nsMgr.LookupNamespace(prefix);
			if (@namespace == null)
			{
				XmlExceptionHelper.ThrowUndefinedPrefix(this, prefix.GetString());
			}
			return @namespace;
		}

		protected void ProcessAttributes()
		{
			if (this.attributeCount > 0)
			{
				this.ProcessAttributes(this.attributeNodes, this.attributeCount);
			}
		}

		private void ProcessAttributes(XmlBaseReader.XmlAttributeNode[] attributeNodes, int attributeCount)
		{
			for (int i = 0; i < attributeCount; i++)
			{
				XmlBaseReader.XmlAttributeNode xmlAttributeNode = attributeNodes[i];
				if (xmlAttributeNode.QNameType == XmlBaseReader.QNameType.Normal)
				{
					PrefixHandle prefixHandle = xmlAttributeNode.Prefix;
					if (!prefixHandle.IsEmpty)
					{
						xmlAttributeNode.Namespace = this.LookupNamespace(prefixHandle);
					}
					else
					{
						xmlAttributeNode.Namespace = XmlBaseReader.NamespaceManager.EmptyNamespace;
					}
					xmlAttributeNode.AttributeText.Namespace = xmlAttributeNode.Namespace;
				}
			}
			if (attributeCount > 1)
			{
				if (attributeCount < 12)
				{
					for (int j = 0; j < attributeCount - 1; j++)
					{
						XmlBaseReader.XmlAttributeNode xmlAttributeNode2 = attributeNodes[j];
						if (xmlAttributeNode2.QNameType == XmlBaseReader.QNameType.Normal)
						{
							for (int k = j + 1; k < attributeCount; k++)
							{
								XmlBaseReader.XmlAttributeNode xmlAttributeNode3 = attributeNodes[k];
								if (xmlAttributeNode3.QNameType == XmlBaseReader.QNameType.Normal && xmlAttributeNode2.LocalName == xmlAttributeNode3.LocalName && xmlAttributeNode2.Namespace.Uri == xmlAttributeNode3.Namespace.Uri)
								{
									XmlExceptionHelper.ThrowDuplicateAttribute(this, xmlAttributeNode2.Prefix.GetString(), xmlAttributeNode3.Prefix.GetString(), xmlAttributeNode2.LocalName.GetString(), xmlAttributeNode2.Namespace.Uri.GetString());
								}
							}
						}
						else
						{
							for (int l = j + 1; l < attributeCount; l++)
							{
								XmlBaseReader.XmlAttributeNode xmlAttributeNode4 = attributeNodes[l];
								if (xmlAttributeNode4.QNameType == XmlBaseReader.QNameType.Xmlns && xmlAttributeNode2.Namespace.Prefix == xmlAttributeNode4.Namespace.Prefix)
								{
									XmlExceptionHelper.ThrowDuplicateAttribute(this, "xmlns", "xmlns", xmlAttributeNode2.Namespace.Prefix.GetString(), "http://www.w3.org/2000/xmlns/");
								}
							}
						}
					}
					return;
				}
				this.CheckAttributes(attributeNodes, attributeCount);
			}
		}

		private void CheckAttributes(XmlBaseReader.XmlAttributeNode[] attributeNodes, int attributeCount)
		{
			if (this.attributeSorter == null)
			{
				this.attributeSorter = new XmlBaseReader.AttributeSorter();
			}
			if (!this.attributeSorter.Sort(attributeNodes, attributeCount))
			{
				int num;
				int num2;
				this.attributeSorter.GetIndeces(out num, out num2);
				if (attributeNodes[num].QNameType == XmlBaseReader.QNameType.Xmlns)
				{
					XmlExceptionHelper.ThrowDuplicateXmlnsAttribute(this, attributeNodes[num].Namespace.Prefix.GetString(), "http://www.w3.org/2000/xmlns/");
					return;
				}
				XmlExceptionHelper.ThrowDuplicateAttribute(this, attributeNodes[num].Prefix.GetString(), attributeNodes[num2].Prefix.GetString(), attributeNodes[num].LocalName.GetString(), attributeNodes[num].Namespace.Uri.GetString());
			}
		}

		public override void MoveToAttribute(int index)
		{
			this.MoveToNode(this.GetAttributeNode(index));
			this.attributeIndex = index;
		}

		public override bool MoveToAttribute(string name)
		{
			XmlBaseReader.XmlNode attributeNode = this.GetAttributeNode(name);
			if (attributeNode == null)
			{
				return false;
			}
			this.MoveToNode(attributeNode);
			this.attributeIndex = this.attributeStart;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceUri)
		{
			XmlBaseReader.XmlNode attributeNode = this.GetAttributeNode(localName, namespaceUri);
			if (attributeNode == null)
			{
				return false;
			}
			this.MoveToNode(attributeNode);
			this.attributeIndex = this.attributeStart;
			return true;
		}

		public override bool MoveToElement()
		{
			if (!this.node.CanMoveToElement)
			{
				return false;
			}
			if (this.depth == 0)
			{
				this.MoveToDeclaration();
			}
			else
			{
				this.MoveToNode(this.elementNodes[this.depth]);
			}
			this.attributeIndex = -1;
			return true;
		}

		public override XmlNodeType MoveToContent()
		{
			do
			{
				if (this.node.HasContent)
				{
					if ((this.node.NodeType != XmlNodeType.Text && this.node.NodeType != XmlNodeType.CDATA) || this.trailByteCount > 0)
					{
						break;
					}
					if (this.value == null)
					{
						if (!this.node.Value.IsWhitespace())
						{
							break;
						}
					}
					else if (!XmlConverter.IsWhitespace(this.value))
					{
						break;
					}
				}
				else if (this.node.NodeType == XmlNodeType.Attribute)
				{
					goto Block_6;
				}
			}
			while (this.Read());
			goto IL_007C;
			Block_6:
			this.MoveToElement();
			IL_007C:
			return this.node.NodeType;
		}

		public override bool MoveToFirstAttribute()
		{
			if (!this.node.CanGetAttribute || this.attributeCount == 0)
			{
				return false;
			}
			this.MoveToNode(this.GetAttributeNode(0));
			this.attributeIndex = 0;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (!this.node.CanGetAttribute)
			{
				return false;
			}
			int num = this.attributeIndex + 1;
			if (num >= this.attributeCount)
			{
				return false;
			}
			this.MoveToNode(this.GetAttributeNode(num));
			this.attributeIndex = num;
			return true;
		}

		public override string NamespaceURI
		{
			get
			{
				if (this.ns == null)
				{
					this.ns = this.GetNamespaceUri(true);
				}
				return this.ns;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				if (this.nameTable == null)
				{
					this.nameTable = new XmlBaseReader.QuotaNameTable(this, this.quotas.MaxNameTableCharCount);
					this.nameTable.Add("xml");
					this.nameTable.Add("xmlns");
					this.nameTable.Add("http://www.w3.org/2000/xmlns/");
					this.nameTable.Add("http://www.w3.org/XML/1998/namespace");
					for (PrefixHandleType prefixHandleType = PrefixHandleType.A; prefixHandleType <= PrefixHandleType.Z; prefixHandleType++)
					{
						this.nameTable.Add(PrefixHandle.GetString(prefixHandleType));
					}
				}
				return this.nameTable;
			}
		}

		public sealed override XmlNodeType NodeType
		{
			get
			{
				return this.node.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				if (this.prefix == null)
				{
					XmlBaseReader.QNameType qnameType = this.node.QNameType;
					if (qnameType == XmlBaseReader.QNameType.Normal)
					{
						this.prefix = this.node.Prefix.GetString(this.NameTable);
					}
					else if (qnameType == XmlBaseReader.QNameType.Xmlns)
					{
						if (this.node.Namespace.Prefix.IsEmpty)
						{
							this.prefix = string.Empty;
						}
						else
						{
							this.prefix = "xmlns";
						}
					}
					else
					{
						this.prefix = "xml";
					}
				}
				return this.prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.node.QuoteChar;
			}
		}

		private string GetLocalName(bool enforceAtomization)
		{
			if (this.localName != null)
			{
				return this.localName;
			}
			if (this.node.QNameType == XmlBaseReader.QNameType.Normal)
			{
				if (enforceAtomization || this.nameTable != null)
				{
					return this.node.LocalName.GetString(this.NameTable);
				}
				return this.node.LocalName.GetString();
			}
			else
			{
				if (this.node.Namespace.Prefix.IsEmpty)
				{
					return "xmlns";
				}
				if (enforceAtomization || this.nameTable != null)
				{
					return this.node.Namespace.Prefix.GetString(this.NameTable);
				}
				return this.node.Namespace.Prefix.GetString();
			}
		}

		private string GetNamespaceUri(bool enforceAtomization)
		{
			if (this.ns != null)
			{
				return this.ns;
			}
			if (this.node.QNameType != XmlBaseReader.QNameType.Normal)
			{
				return "http://www.w3.org/2000/xmlns/";
			}
			if (enforceAtomization || this.nameTable != null)
			{
				return this.node.Namespace.Uri.GetString(this.NameTable);
			}
			return this.node.Namespace.Uri.GetString();
		}

		public override void GetNonAtomizedNames(out string localName, out string namespaceUri)
		{
			localName = this.GetLocalName(false);
			namespaceUri = this.GetNamespaceUri(false);
		}

		public override bool IsLocalName(string localName)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			return this.node.IsLocalName(localName);
		}

		public override bool IsLocalName(XmlDictionaryString localName)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			return this.node.IsLocalName(localName);
		}

		public override bool IsNamespaceUri(string namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			return this.node.IsNamespaceUri(namespaceUri);
		}

		public override bool IsNamespaceUri(XmlDictionaryString namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			return this.node.IsNamespaceUri(namespaceUri);
		}

		public sealed override bool IsStartElement()
		{
			XmlNodeType nodeType = this.node.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				return true;
			}
			if (nodeType == XmlNodeType.EndElement)
			{
				return false;
			}
			if (nodeType == XmlNodeType.None)
			{
				this.Read();
				if (this.node.NodeType == XmlNodeType.Element)
				{
					return true;
				}
			}
			return this.MoveToContent() == XmlNodeType.Element;
		}

		public override bool IsStartElement(string name)
		{
			if (name == null)
			{
				return false;
			}
			int num = name.IndexOf(':');
			string text;
			string text2;
			if (num == -1)
			{
				text = string.Empty;
				text2 = name;
			}
			else
			{
				text = name.Substring(0, num);
				text2 = name.Substring(num + 1);
			}
			return (this.node.NodeType == XmlNodeType.Element || this.IsStartElement()) && this.node.Prefix == text && this.node.LocalName == text2;
		}

		public override bool IsStartElement(string localName, string namespaceUri)
		{
			return localName != null && namespaceUri != null && ((this.node.NodeType == XmlNodeType.Element || this.IsStartElement()) && this.node.LocalName == localName) && this.node.IsNamespaceUri(namespaceUri);
		}

		public override bool IsStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localName");
			}
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			return (this.node.NodeType == XmlNodeType.Element || this.IsStartElement()) && this.node.LocalName == localName && this.node.IsNamespaceUri(namespaceUri);
		}

		public override int IndexOfLocalName(string[] localNames, string namespaceUri)
		{
			if (localNames == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localNames");
			}
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			XmlBaseReader.QNameType qnameType = this.node.QNameType;
			if (this.node.IsNamespaceUri(namespaceUri))
			{
				if (qnameType == XmlBaseReader.QNameType.Normal)
				{
					StringHandle stringHandle = this.node.LocalName;
					for (int i = 0; i < localNames.Length; i++)
					{
						string text = localNames[i];
						if (text == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "localNames[{0}]", i));
						}
						if (stringHandle == text)
						{
							return i;
						}
					}
				}
				else
				{
					PrefixHandle prefixHandle = this.node.Namespace.Prefix;
					for (int j = 0; j < localNames.Length; j++)
					{
						string text2 = localNames[j];
						if (text2 == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "localNames[{0}]", j));
						}
						if (prefixHandle == text2)
						{
							return j;
						}
					}
				}
			}
			return -1;
		}

		public override int IndexOfLocalName(XmlDictionaryString[] localNames, XmlDictionaryString namespaceUri)
		{
			if (localNames == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localNames");
			}
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			XmlBaseReader.QNameType qnameType = this.node.QNameType;
			if (this.node.IsNamespaceUri(namespaceUri))
			{
				if (qnameType == XmlBaseReader.QNameType.Normal)
				{
					StringHandle stringHandle = this.node.LocalName;
					for (int i = 0; i < localNames.Length; i++)
					{
						XmlDictionaryString xmlDictionaryString = localNames[i];
						if (xmlDictionaryString == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "localNames[{0}]", i));
						}
						if (stringHandle == xmlDictionaryString)
						{
							return i;
						}
					}
				}
				else
				{
					PrefixHandle prefixHandle = this.node.Namespace.Prefix;
					for (int j = 0; j < localNames.Length; j++)
					{
						XmlDictionaryString xmlDictionaryString2 = localNames[j];
						if (xmlDictionaryString2 == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "localNames[{0}]", j));
						}
						if (prefixHandle == xmlDictionaryString2)
						{
							return j;
						}
					}
				}
			}
			return -1;
		}

		public override int ReadValueChunk(char[] chars, int offset, int count)
		{
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > chars.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - offset })));
			}
			int num;
			if (this.value == null && this.node.QNameType == XmlBaseReader.QNameType.Normal && this.node.Value.TryReadChars(chars, offset, count, out num))
			{
				return num;
			}
			string text = this.Value;
			num = Math.Min(count, text.Length);
			text.CopyTo(0, chars, offset, num);
			this.value = text.Substring(num);
			return num;
		}

		public override int ReadValueAsBase64(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
			if (count == 0)
			{
				return 0;
			}
			int num;
			if (this.value == null && this.trailByteCount == 0 && this.trailCharCount == 0 && this.node.QNameType == XmlBaseReader.QNameType.Normal && this.node.Value.TryReadBase64(buffer, offset, count, out num))
			{
				return num;
			}
			return this.ReadBytes(XmlBaseReader.Base64Encoding, 3, 4, buffer, offset, Math.Min(count, 512), false);
		}

		public override string ReadElementContentAsString()
		{
			if (this.node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			if (this.node.IsEmptyElement)
			{
				this.Read();
				return string.Empty;
			}
			this.Read();
			string text = this.ReadContentAsString();
			this.ReadEndElement();
			return text;
		}

		public override string ReadElementString()
		{
			this.MoveToStartElement();
			if (this.IsEmptyElement)
			{
				this.Read();
				return string.Empty;
			}
			this.Read();
			string text = this.ReadString();
			this.ReadEndElement();
			return text;
		}

		public override string ReadElementString(string name)
		{
			this.MoveToStartElement(name);
			return this.ReadElementString();
		}

		public override string ReadElementString(string localName, string namespaceUri)
		{
			this.MoveToStartElement(localName, namespaceUri);
			return this.ReadElementString();
		}

		public override void ReadStartElement()
		{
			if (this.node.NodeType != XmlNodeType.Element)
			{
				this.MoveToStartElement();
			}
			this.Read();
		}

		public override void ReadStartElement(string name)
		{
			this.MoveToStartElement(name);
			this.Read();
		}

		public override void ReadStartElement(string localName, string namespaceUri)
		{
			this.MoveToStartElement(localName, namespaceUri);
			this.Read();
		}

		public override void ReadEndElement()
		{
			if (this.node.NodeType != XmlNodeType.EndElement && this.MoveToContent() != XmlNodeType.EndElement)
			{
				int num = ((this.node.NodeType == XmlNodeType.Element) ? (this.depth - 1) : this.depth);
				if (num == 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("No corresponding start element is open.")));
				}
				XmlBaseReader.XmlElementNode xmlElementNode = this.elementNodes[num];
				XmlExceptionHelper.ThrowEndElementExpected(this, xmlElementNode.LocalName.GetString(), xmlElementNode.Namespace.Uri.GetString());
			}
			this.Read();
		}

		public override bool ReadAttributeValue()
		{
			XmlBaseReader.XmlAttributeTextNode attributeText = this.node.AttributeText;
			if (attributeText == null)
			{
				return false;
			}
			this.MoveToNode(attributeText);
			return true;
		}

		public override ReadState ReadState
		{
			get
			{
				return this.node.ReadState;
			}
		}

		private void SkipValue(XmlBaseReader.XmlNode node)
		{
			if (node.SkipValue)
			{
				this.Read();
			}
		}

		public override bool TryGetBase64ContentLength(out int length)
		{
			if (this.trailByteCount == 0 && this.trailCharCount == 0 && this.value == null)
			{
				XmlBaseReader.XmlNode xmlNode = this.Node;
				if (xmlNode.IsAtomicValue)
				{
					return xmlNode.Value.TryGetByteArrayLength(out length);
				}
			}
			return base.TryGetBase64ContentLength(out length);
		}

		public override byte[] ReadContentAsBase64()
		{
			if (this.trailByteCount == 0 && this.trailCharCount == 0 && this.value == null)
			{
				XmlBaseReader.XmlNode xmlNode = this.Node;
				if (xmlNode.IsAtomicValue)
				{
					byte[] array = xmlNode.Value.ToByteArray();
					if (array.Length > this.quotas.MaxArrayLength)
					{
						XmlExceptionHelper.ThrowMaxArrayLengthExceeded(this, this.quotas.MaxArrayLength);
					}
					this.SkipValue(xmlNode);
					return array;
				}
			}
			if (!this.bufferReader.IsStreamed)
			{
				return base.ReadContentAsBase64(this.quotas.MaxArrayLength, this.bufferReader.Buffer.Length);
			}
			return base.ReadContentAsBase64(this.quotas.MaxArrayLength, 65535);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int count)
		{
			if (!this.readingElement)
			{
				if (this.IsEmptyElement)
				{
					this.Read();
					return 0;
				}
				this.ReadStartElement();
				this.readingElement = true;
			}
			int num = this.ReadContentAsBase64(buffer, offset, count);
			if (num == 0)
			{
				this.ReadEndElement();
				this.readingElement = false;
			}
			return num;
		}

		public override int ReadContentAsBase64(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
			if (count == 0)
			{
				return 0;
			}
			if (this.trailByteCount == 0 && this.trailCharCount == 0 && this.value == null && this.node.QNameType == XmlBaseReader.QNameType.Normal)
			{
				int num;
				while (this.node.NodeType != XmlNodeType.Comment && this.node.Value.TryReadBase64(buffer, offset, count, out num))
				{
					if (num != 0)
					{
						return num;
					}
					this.Read();
				}
			}
			XmlNodeType nodeType = this.node.NodeType;
			if (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.EndElement)
			{
				return 0;
			}
			return this.ReadBytes(XmlBaseReader.Base64Encoding, 3, 4, buffer, offset, Math.Min(count, 512), true);
		}

		public override byte[] ReadContentAsBinHex()
		{
			return base.ReadContentAsBinHex(this.quotas.MaxArrayLength);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int count)
		{
			if (!this.readingElement)
			{
				if (this.IsEmptyElement)
				{
					this.Read();
					return 0;
				}
				this.ReadStartElement();
				this.readingElement = true;
			}
			int num = this.ReadContentAsBinHex(buffer, offset, count);
			if (num == 0)
			{
				this.ReadEndElement();
				this.readingElement = false;
			}
			return num;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
			if (count == 0)
			{
				return 0;
			}
			return this.ReadBytes(XmlBaseReader.BinHexEncoding, 1, 2, buffer, offset, Math.Min(count, 512), true);
		}

		private int ReadBytes(Encoding encoding, int byteBlock, int charBlock, byte[] buffer, int offset, int byteCount, bool readContent)
		{
			if (this.trailByteCount > 0)
			{
				int num = Math.Min(this.trailByteCount, byteCount);
				Array.Copy(this.trailBytes, 0, buffer, offset, num);
				this.trailByteCount -= num;
				Array.Copy(this.trailBytes, num, this.trailBytes, 0, this.trailByteCount);
				return num;
			}
			XmlNodeType nodeType = this.node.NodeType;
			if (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.EndElement)
			{
				return 0;
			}
			int num2;
			if (byteCount < byteBlock)
			{
				num2 = charBlock;
			}
			else
			{
				num2 = byteCount / byteBlock * charBlock;
			}
			char[] charBuffer = this.GetCharBuffer(num2);
			int i = 0;
			int num5;
			for (;;)
			{
				if (this.trailCharCount > 0)
				{
					Array.Copy(this.trailChars, 0, charBuffer, i, this.trailCharCount);
					i += this.trailCharCount;
					this.trailCharCount = 0;
				}
				while (i < charBlock)
				{
					int num3;
					if (readContent)
					{
						num3 = this.ReadContentAsChars(charBuffer, i, num2 - i);
						if (num3 == 1 && charBuffer[i] == '\n')
						{
							continue;
						}
					}
					else
					{
						num3 = this.ReadValueChunk(charBuffer, i, num2 - i);
					}
					if (num3 == 0)
					{
						break;
					}
					i += num3;
				}
				if (i >= charBlock)
				{
					this.trailCharCount = i % charBlock;
					if (this.trailCharCount > 0)
					{
						if (this.trailChars == null)
						{
							this.trailChars = new char[4];
						}
						i -= this.trailCharCount;
						Array.Copy(charBuffer, i, this.trailChars, 0, this.trailCharCount);
					}
				}
				try
				{
					if (byteCount < byteBlock)
					{
						if (this.trailBytes == null)
						{
							this.trailBytes = new byte[3];
						}
						this.trailByteCount = encoding.GetBytes(charBuffer, 0, i, this.trailBytes, 0);
						int num4 = Math.Min(this.trailByteCount, byteCount);
						Array.Copy(this.trailBytes, 0, buffer, offset, num4);
						this.trailByteCount -= num4;
						Array.Copy(this.trailBytes, num4, this.trailBytes, 0, this.trailByteCount);
						num5 = num4;
					}
					else
					{
						num5 = encoding.GetBytes(charBuffer, 0, i, buffer, offset);
					}
				}
				catch (FormatException ex)
				{
					int num6 = 0;
					int num7 = 0;
					for (;;)
					{
						if (num7 >= i || !XmlConverter.IsWhitespace(charBuffer[num7]))
						{
							if (num7 == i)
							{
								break;
							}
							charBuffer[num6++] = charBuffer[num7++];
						}
						else
						{
							num7++;
						}
					}
					if (num6 == i)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(ex.Message, ex.InnerException));
					}
					i = num6;
					continue;
				}
				break;
			}
			return num5;
		}

		public override string ReadContentAsString()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (xmlNode.IsAtomicValue)
			{
				string @string;
				if (this.value != null)
				{
					@string = this.value;
					if (xmlNode.AttributeText == null)
					{
						this.value = string.Empty;
					}
				}
				else
				{
					@string = xmlNode.Value.GetString();
					this.SkipValue(xmlNode);
					if (@string.Length > this.quotas.MaxStringContentLength)
					{
						XmlExceptionHelper.ThrowMaxStringContentLengthExceeded(this, this.quotas.MaxStringContentLength);
					}
				}
				return @string;
			}
			return base.ReadContentAsString(this.quotas.MaxStringContentLength);
		}

		public override bool ReadContentAsBoolean()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				bool flag = xmlNode.Value.ToBoolean();
				this.SkipValue(xmlNode);
				return flag;
			}
			return XmlConverter.ToBoolean(this.ReadContentAsString());
		}

		public override long ReadContentAsLong()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				long num = xmlNode.Value.ToLong();
				this.SkipValue(xmlNode);
				return num;
			}
			return XmlConverter.ToInt64(this.ReadContentAsString());
		}

		public override int ReadContentAsInt()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				int num = xmlNode.Value.ToInt();
				this.SkipValue(xmlNode);
				return num;
			}
			return XmlConverter.ToInt32(this.ReadContentAsString());
		}

		public override DateTime ReadContentAsDateTime()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				DateTime dateTime = xmlNode.Value.ToDateTime();
				this.SkipValue(xmlNode);
				return dateTime;
			}
			return XmlConverter.ToDateTime(this.ReadContentAsString());
		}

		public override double ReadContentAsDouble()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				double num = xmlNode.Value.ToDouble();
				this.SkipValue(xmlNode);
				return num;
			}
			return XmlConverter.ToDouble(this.ReadContentAsString());
		}

		public override float ReadContentAsFloat()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				float num = xmlNode.Value.ToSingle();
				this.SkipValue(xmlNode);
				return num;
			}
			return XmlConverter.ToSingle(this.ReadContentAsString());
		}

		public override decimal ReadContentAsDecimal()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				decimal num = xmlNode.Value.ToDecimal();
				this.SkipValue(xmlNode);
				return num;
			}
			return XmlConverter.ToDecimal(this.ReadContentAsString());
		}

		public override UniqueId ReadContentAsUniqueId()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				UniqueId uniqueId = xmlNode.Value.ToUniqueId();
				this.SkipValue(xmlNode);
				return uniqueId;
			}
			return XmlConverter.ToUniqueId(this.ReadContentAsString());
		}

		public override TimeSpan ReadContentAsTimeSpan()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				TimeSpan timeSpan = xmlNode.Value.ToTimeSpan();
				this.SkipValue(xmlNode);
				return timeSpan;
			}
			return XmlConverter.ToTimeSpan(this.ReadContentAsString());
		}

		public override Guid ReadContentAsGuid()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				Guid guid = xmlNode.Value.ToGuid();
				this.SkipValue(xmlNode);
				return guid;
			}
			return XmlConverter.ToGuid(this.ReadContentAsString());
		}

		public override object ReadContentAsObject()
		{
			XmlBaseReader.XmlNode xmlNode = this.Node;
			if (this.value == null && xmlNode.IsAtomicValue)
			{
				object obj = xmlNode.Value.ToObject();
				this.SkipValue(xmlNode);
				return obj;
			}
			return this.ReadContentAsString();
		}

		public override object ReadContentAs(Type type, IXmlNamespaceResolver namespaceResolver)
		{
			if (type == typeof(ulong))
			{
				if (this.value == null && this.node.IsAtomicValue)
				{
					ulong num = this.node.Value.ToULong();
					this.SkipValue(this.node);
					return num;
				}
				return XmlConverter.ToUInt64(this.ReadContentAsString());
			}
			else
			{
				if (type == typeof(bool))
				{
					return this.ReadContentAsBoolean();
				}
				if (type == typeof(int))
				{
					return this.ReadContentAsInt();
				}
				if (type == typeof(long))
				{
					return this.ReadContentAsLong();
				}
				if (type == typeof(float))
				{
					return this.ReadContentAsFloat();
				}
				if (type == typeof(double))
				{
					return this.ReadContentAsDouble();
				}
				if (type == typeof(decimal))
				{
					return this.ReadContentAsDecimal();
				}
				if (type == typeof(DateTime))
				{
					return this.ReadContentAsDateTime();
				}
				if (type == typeof(UniqueId))
				{
					return this.ReadContentAsUniqueId();
				}
				if (type == typeof(Guid))
				{
					return this.ReadContentAsGuid();
				}
				if (type == typeof(TimeSpan))
				{
					return this.ReadContentAsTimeSpan();
				}
				if (type == typeof(object))
				{
					return this.ReadContentAsObject();
				}
				return base.ReadContentAs(type, namespaceResolver);
			}
		}

		public override void ResolveEntity()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The reader cannot be advanced.")));
		}

		public override void Skip()
		{
			if (this.node.ReadState != ReadState.Interactive)
			{
				return;
			}
			if ((this.node.NodeType == XmlNodeType.Element || this.MoveToElement()) && !this.IsEmptyElement)
			{
				int num = this.Depth;
				while (this.Read() && num < this.Depth)
				{
				}
				if (this.node.NodeType == XmlNodeType.EndElement)
				{
					this.Read();
					return;
				}
			}
			else
			{
				this.Read();
			}
		}

		public override string Value
		{
			get
			{
				if (this.value == null)
				{
					this.value = this.node.ValueAsString;
				}
				return this.value;
			}
		}

		public override Type ValueType
		{
			get
			{
				if (this.value == null && this.node.QNameType == XmlBaseReader.QNameType.Normal)
				{
					Type type = this.node.Value.ToType();
					if (this.node.IsAtomicValue)
					{
						return type;
					}
					if (type == typeof(byte[]))
					{
						return type;
					}
				}
				return typeof(string);
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.nsMgr.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.nsMgr.XmlSpace;
			}
		}

		public override bool TryGetLocalNameAsDictionaryString(out XmlDictionaryString localName)
		{
			return this.node.TryGetLocalNameAsDictionaryString(out localName);
		}

		public override bool TryGetNamespaceUriAsDictionaryString(out XmlDictionaryString localName)
		{
			return this.node.TryGetNamespaceUriAsDictionaryString(out localName);
		}

		public override bool TryGetValueAsDictionaryString(out XmlDictionaryString value)
		{
			return this.node.TryGetValueAsDictionaryString(out value);
		}

		public override short[] ReadInt16Array(string localName, string namespaceUri)
		{
			return Int16ArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override short[] ReadInt16Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return Int16ArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override int[] ReadInt32Array(string localName, string namespaceUri)
		{
			return Int32ArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override int[] ReadInt32Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return Int32ArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override long[] ReadInt64Array(string localName, string namespaceUri)
		{
			return Int64ArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override long[] ReadInt64Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return Int64ArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override float[] ReadSingleArray(string localName, string namespaceUri)
		{
			return SingleArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override float[] ReadSingleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return SingleArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override double[] ReadDoubleArray(string localName, string namespaceUri)
		{
			return DoubleArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override double[] ReadDoubleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return DoubleArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override decimal[] ReadDecimalArray(string localName, string namespaceUri)
		{
			return DecimalArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override decimal[] ReadDecimalArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return DecimalArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override DateTime[] ReadDateTimeArray(string localName, string namespaceUri)
		{
			return DateTimeArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override DateTime[] ReadDateTimeArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return DateTimeArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override Guid[] ReadGuidArray(string localName, string namespaceUri)
		{
			return GuidArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override Guid[] ReadGuidArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return GuidArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override TimeSpan[] ReadTimeSpanArray(string localName, string namespaceUri)
		{
			return TimeSpanArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public override TimeSpan[] ReadTimeSpanArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return TimeSpanArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.quotas.MaxArrayLength);
		}

		public string GetOpenElements()
		{
			string text = string.Empty;
			for (int i = this.depth; i > 0; i--)
			{
				string @string = this.elementNodes[i].LocalName.GetString();
				if (i != this.depth)
				{
					text += ", ";
				}
				text += @string;
			}
			return text;
		}

		private char[] GetCharBuffer(int count)
		{
			if (count > 1024)
			{
				return new char[count];
			}
			if (this.chars == null || this.chars.Length < count)
			{
				this.chars = new char[count];
			}
			return this.chars;
		}

		private void SignStartElement(XmlSigningNodeWriter writer)
		{
			int num;
			int num2;
			byte[] @string = this.node.Prefix.GetString(out num, out num2);
			int num3;
			int num4;
			byte[] string2 = this.node.LocalName.GetString(out num3, out num4);
			writer.WriteStartElement(@string, num, num2, string2, num3, num4);
		}

		private void SignAttribute(XmlSigningNodeWriter writer, XmlBaseReader.XmlAttributeNode attributeNode)
		{
			if (attributeNode.QNameType == XmlBaseReader.QNameType.Normal)
			{
				int num;
				int num2;
				byte[] @string = attributeNode.Prefix.GetString(out num, out num2);
				int num3;
				int num4;
				byte[] string2 = attributeNode.LocalName.GetString(out num3, out num4);
				writer.WriteStartAttribute(@string, num, num2, string2, num3, num4);
				attributeNode.Value.Sign(writer);
				writer.WriteEndAttribute();
				return;
			}
			int num5;
			int num6;
			byte[] string3 = attributeNode.Namespace.Prefix.GetString(out num5, out num6);
			int num7;
			int num8;
			byte[] string4 = attributeNode.Namespace.Uri.GetString(out num7, out num8);
			writer.WriteXmlnsAttribute(string3, num5, num6, string4, num7, num8);
		}

		private void SignEndElement(XmlSigningNodeWriter writer)
		{
			int num;
			int num2;
			byte[] @string = this.node.Prefix.GetString(out num, out num2);
			int num3;
			int num4;
			byte[] string2 = this.node.LocalName.GetString(out num3, out num4);
			writer.WriteEndElement(@string, num, num2, string2, num3, num4);
		}

		private void SignNode(XmlSigningNodeWriter writer)
		{
			XmlNodeType nodeType = this.node.NodeType;
			switch (nodeType)
			{
			case XmlNodeType.None:
				return;
			case XmlNodeType.Element:
			{
				this.SignStartElement(writer);
				for (int i = 0; i < this.attributeCount; i++)
				{
					this.SignAttribute(writer, this.attributeNodes[i]);
				}
				writer.WriteEndStartElement(this.node.IsEmptyElement);
				return;
			}
			case XmlNodeType.Attribute:
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.ProcessingInstruction:
				goto IL_00C6;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				break;
			case XmlNodeType.Comment:
				writer.WriteComment(this.node.Value.GetString());
				return;
			default:
				switch (nodeType)
				{
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					break;
				case XmlNodeType.EndElement:
					this.SignEndElement(writer);
					return;
				case XmlNodeType.EndEntity:
					goto IL_00C6;
				case XmlNodeType.XmlDeclaration:
					writer.WriteDeclaration();
					return;
				default:
					goto IL_00C6;
				}
				break;
			}
			this.node.Value.Sign(writer);
			return;
			IL_00C6:
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
		}

		public override bool CanCanonicalize
		{
			get
			{
				return true;
			}
		}

		protected bool Signing
		{
			get
			{
				return this.signing;
			}
		}

		protected void SignNode()
		{
			if (this.signing)
			{
				this.SignNode(this.signingWriter);
			}
		}

		public override void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			if (this.signing)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("XML canonicalization started")));
			}
			if (this.signingWriter == null)
			{
				this.signingWriter = this.CreateSigningNodeWriter();
			}
			this.signingWriter.SetOutput(XmlNodeWriter.Null, stream, includeComments, inclusivePrefixes);
			this.nsMgr.Sign(this.signingWriter);
			this.signing = true;
		}

		public override void EndCanonicalization()
		{
			if (!this.signing)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("XML canonicalization was not started.")));
			}
			this.signingWriter.Flush();
			this.signingWriter.Close();
			this.signing = false;
		}

		protected abstract XmlSigningNodeWriter CreateSigningNodeWriter();

		private XmlBufferReader bufferReader;

		private XmlBaseReader.XmlNode node;

		private XmlBaseReader.NamespaceManager nsMgr;

		private XmlBaseReader.XmlElementNode[] elementNodes;

		private XmlBaseReader.XmlAttributeNode[] attributeNodes;

		private XmlBaseReader.XmlAtomicTextNode atomicTextNode;

		private int depth;

		private int attributeCount;

		private int attributeStart;

		private XmlDictionaryReaderQuotas quotas;

		private XmlNameTable nameTable;

		private XmlBaseReader.XmlDeclarationNode declarationNode;

		private XmlBaseReader.XmlComplexTextNode complexTextNode;

		private XmlBaseReader.XmlWhitespaceTextNode whitespaceTextNode;

		private XmlBaseReader.XmlCDataNode cdataNode;

		private XmlBaseReader.XmlCommentNode commentNode;

		private XmlBaseReader.XmlElementNode rootElementNode;

		private int attributeIndex;

		private char[] chars;

		private string prefix;

		private string localName;

		private string ns;

		private string value;

		private int trailCharCount;

		private int trailByteCount;

		private char[] trailChars;

		private byte[] trailBytes;

		private bool rootElement;

		private bool readingElement;

		private XmlSigningNodeWriter signingWriter;

		private bool signing;

		private XmlBaseReader.AttributeSorter attributeSorter;

		private static XmlBaseReader.XmlInitialNode initialNode = new XmlBaseReader.XmlInitialNode(XmlBufferReader.Empty);

		private static XmlBaseReader.XmlEndOfFileNode endOfFileNode = new XmlBaseReader.XmlEndOfFileNode(XmlBufferReader.Empty);

		private static XmlBaseReader.XmlClosedNode closedNode = new XmlBaseReader.XmlClosedNode(XmlBufferReader.Empty);

		private static BinHexEncoding binhexEncoding;

		private static Base64Encoding base64Encoding;

		private const string xmlns = "xmlns";

		private const string xml = "xml";

		private const string xmlnsNamespace = "http://www.w3.org/2000/xmlns/";

		private const string xmlNamespace = "http://www.w3.org/XML/1998/namespace";

		protected enum QNameType
		{
			Normal,
			Xmlns
		}

		protected class XmlNode
		{
			protected XmlNode(XmlNodeType nodeType, PrefixHandle prefix, StringHandle localName, ValueHandle value, XmlBaseReader.XmlNode.XmlNodeFlags nodeFlags, ReadState readState, XmlBaseReader.XmlAttributeTextNode attributeTextNode, int depthDelta)
			{
				this.nodeType = nodeType;
				this.prefix = prefix;
				this.localName = localName;
				this.value = value;
				this.ns = XmlBaseReader.NamespaceManager.EmptyNamespace;
				this.hasValue = (nodeFlags & XmlBaseReader.XmlNode.XmlNodeFlags.HasValue) > XmlBaseReader.XmlNode.XmlNodeFlags.None;
				this.canGetAttribute = (nodeFlags & XmlBaseReader.XmlNode.XmlNodeFlags.CanGetAttribute) > XmlBaseReader.XmlNode.XmlNodeFlags.None;
				this.canMoveToElement = (nodeFlags & XmlBaseReader.XmlNode.XmlNodeFlags.CanMoveToElement) > XmlBaseReader.XmlNode.XmlNodeFlags.None;
				this.isAtomicValue = (nodeFlags & XmlBaseReader.XmlNode.XmlNodeFlags.AtomicValue) > XmlBaseReader.XmlNode.XmlNodeFlags.None;
				this.skipValue = (nodeFlags & XmlBaseReader.XmlNode.XmlNodeFlags.SkipValue) > XmlBaseReader.XmlNode.XmlNodeFlags.None;
				this.hasContent = (nodeFlags & XmlBaseReader.XmlNode.XmlNodeFlags.HasContent) > XmlBaseReader.XmlNode.XmlNodeFlags.None;
				this.readState = readState;
				this.attributeTextNode = attributeTextNode;
				this.exitScope = nodeType == XmlNodeType.EndElement;
				this.depthDelta = depthDelta;
				this.isEmptyElement = false;
				this.quoteChar = '"';
				this.qnameType = XmlBaseReader.QNameType.Normal;
			}

			public bool HasValue
			{
				get
				{
					return this.hasValue;
				}
			}

			public ReadState ReadState
			{
				get
				{
					return this.readState;
				}
			}

			public StringHandle LocalName
			{
				get
				{
					return this.localName;
				}
			}

			public PrefixHandle Prefix
			{
				get
				{
					return this.prefix;
				}
			}

			public bool CanGetAttribute
			{
				get
				{
					return this.canGetAttribute;
				}
			}

			public bool CanMoveToElement
			{
				get
				{
					return this.canMoveToElement;
				}
			}

			public XmlBaseReader.XmlAttributeTextNode AttributeText
			{
				get
				{
					return this.attributeTextNode;
				}
			}

			public bool SkipValue
			{
				get
				{
					return this.skipValue;
				}
			}

			public ValueHandle Value
			{
				get
				{
					return this.value;
				}
			}

			public int DepthDelta
			{
				get
				{
					return this.depthDelta;
				}
			}

			public bool HasContent
			{
				get
				{
					return this.hasContent;
				}
			}

			public XmlNodeType NodeType
			{
				get
				{
					return this.nodeType;
				}
				set
				{
					this.nodeType = value;
				}
			}

			public XmlBaseReader.QNameType QNameType
			{
				get
				{
					return this.qnameType;
				}
				set
				{
					this.qnameType = value;
				}
			}

			public XmlBaseReader.Namespace Namespace
			{
				get
				{
					return this.ns;
				}
				set
				{
					this.ns = value;
				}
			}

			public bool IsAtomicValue
			{
				get
				{
					return this.isAtomicValue;
				}
				set
				{
					this.isAtomicValue = value;
				}
			}

			public bool ExitScope
			{
				get
				{
					return this.exitScope;
				}
				set
				{
					this.exitScope = value;
				}
			}

			public bool IsEmptyElement
			{
				get
				{
					return this.isEmptyElement;
				}
				set
				{
					this.isEmptyElement = value;
				}
			}

			public char QuoteChar
			{
				get
				{
					return this.quoteChar;
				}
				set
				{
					this.quoteChar = value;
				}
			}

			public bool IsLocalName(string localName)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.LocalName == localName;
				}
				return this.Namespace.Prefix == localName;
			}

			public bool IsLocalName(XmlDictionaryString localName)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.LocalName == localName;
				}
				return this.Namespace.Prefix == localName;
			}

			public bool IsNamespaceUri(string ns)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.Namespace.IsUri(ns);
				}
				return ns == "http://www.w3.org/2000/xmlns/";
			}

			public bool IsNamespaceUri(XmlDictionaryString ns)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.Namespace.IsUri(ns);
				}
				return ns.Value == "http://www.w3.org/2000/xmlns/";
			}

			public bool IsLocalNameAndNamespaceUri(string localName, string ns)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.LocalName == localName && this.Namespace.IsUri(ns);
				}
				return this.Namespace.Prefix == localName && ns == "http://www.w3.org/2000/xmlns/";
			}

			public bool IsLocalNameAndNamespaceUri(XmlDictionaryString localName, XmlDictionaryString ns)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.LocalName == localName && this.Namespace.IsUri(ns);
				}
				return this.Namespace.Prefix == localName && ns.Value == "http://www.w3.org/2000/xmlns/";
			}

			public bool IsPrefixAndLocalName(string prefix, string localName)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.Prefix == prefix && this.LocalName == localName;
				}
				return prefix == "xmlns" && this.Namespace.Prefix == localName;
			}

			public bool TryGetLocalNameAsDictionaryString(out XmlDictionaryString localName)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.LocalName.TryGetDictionaryString(out localName);
				}
				localName = null;
				return false;
			}

			public bool TryGetNamespaceUriAsDictionaryString(out XmlDictionaryString ns)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.Namespace.Uri.TryGetDictionaryString(out ns);
				}
				ns = null;
				return false;
			}

			public bool TryGetValueAsDictionaryString(out XmlDictionaryString value)
			{
				if (this.qnameType == XmlBaseReader.QNameType.Normal)
				{
					return this.Value.TryGetDictionaryString(out value);
				}
				value = null;
				return false;
			}

			public string ValueAsString
			{
				get
				{
					if (this.qnameType == XmlBaseReader.QNameType.Normal)
					{
						return this.Value.GetString();
					}
					return this.Namespace.Uri.GetString();
				}
			}

			private XmlNodeType nodeType;

			private PrefixHandle prefix;

			private StringHandle localName;

			private ValueHandle value;

			private XmlBaseReader.Namespace ns;

			private bool hasValue;

			private bool canGetAttribute;

			private bool canMoveToElement;

			private ReadState readState;

			private XmlBaseReader.XmlAttributeTextNode attributeTextNode;

			private bool exitScope;

			private int depthDelta;

			private bool isAtomicValue;

			private bool skipValue;

			private XmlBaseReader.QNameType qnameType;

			private bool hasContent;

			private bool isEmptyElement;

			private char quoteChar;

			protected enum XmlNodeFlags
			{
				None,
				CanGetAttribute,
				CanMoveToElement,
				HasValue = 4,
				AtomicValue = 8,
				SkipValue = 16,
				HasContent = 32
			}
		}

		protected class XmlElementNode : XmlBaseReader.XmlNode
		{
			public XmlElementNode(XmlBufferReader bufferReader)
				: this(new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader))
			{
			}

			private XmlElementNode(PrefixHandle prefix, StringHandle localName, ValueHandle value)
				: base(XmlNodeType.Element, prefix, localName, value, (XmlBaseReader.XmlNode.XmlNodeFlags)33, ReadState.Interactive, null, -1)
			{
				this.endElementNode = new XmlBaseReader.XmlEndElementNode(prefix, localName, value);
			}

			public XmlBaseReader.XmlEndElementNode EndElement
			{
				get
				{
					return this.endElementNode;
				}
			}

			public int BufferOffset
			{
				get
				{
					return this.bufferOffset;
				}
				set
				{
					this.bufferOffset = value;
				}
			}

			private XmlBaseReader.XmlEndElementNode endElementNode;

			private int bufferOffset;

			public int NameOffset;

			public int NameLength;
		}

		protected class XmlAttributeNode : XmlBaseReader.XmlNode
		{
			public XmlAttributeNode(XmlBufferReader bufferReader)
				: this(new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader))
			{
			}

			private XmlAttributeNode(PrefixHandle prefix, StringHandle localName, ValueHandle value)
				: base(XmlNodeType.Attribute, prefix, localName, value, (XmlBaseReader.XmlNode.XmlNodeFlags)15, ReadState.Interactive, new XmlBaseReader.XmlAttributeTextNode(prefix, localName, value), 0)
			{
			}
		}

		protected class XmlEndElementNode : XmlBaseReader.XmlNode
		{
			public XmlEndElementNode(PrefixHandle prefix, StringHandle localName, ValueHandle value)
				: base(XmlNodeType.EndElement, prefix, localName, value, XmlBaseReader.XmlNode.XmlNodeFlags.HasContent, ReadState.Interactive, null, -1)
			{
			}
		}

		protected class XmlTextNode : XmlBaseReader.XmlNode
		{
			protected XmlTextNode(XmlNodeType nodeType, PrefixHandle prefix, StringHandle localName, ValueHandle value, XmlBaseReader.XmlNode.XmlNodeFlags nodeFlags, ReadState readState, XmlBaseReader.XmlAttributeTextNode attributeTextNode, int depthDelta)
				: base(nodeType, prefix, localName, value, nodeFlags, readState, attributeTextNode, depthDelta)
			{
			}
		}

		protected class XmlAtomicTextNode : XmlBaseReader.XmlTextNode
		{
			public XmlAtomicTextNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.Text, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), (XmlBaseReader.XmlNode.XmlNodeFlags)60, ReadState.Interactive, null, 0)
			{
			}
		}

		protected class XmlComplexTextNode : XmlBaseReader.XmlTextNode
		{
			public XmlComplexTextNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.Text, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), (XmlBaseReader.XmlNode.XmlNodeFlags)36, ReadState.Interactive, null, 0)
			{
			}
		}

		protected class XmlWhitespaceTextNode : XmlBaseReader.XmlTextNode
		{
			public XmlWhitespaceTextNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.Whitespace, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), XmlBaseReader.XmlNode.XmlNodeFlags.HasValue, ReadState.Interactive, null, 0)
			{
			}
		}

		protected class XmlCDataNode : XmlBaseReader.XmlTextNode
		{
			public XmlCDataNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.CDATA, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), (XmlBaseReader.XmlNode.XmlNodeFlags)36, ReadState.Interactive, null, 0)
			{
			}
		}

		protected class XmlAttributeTextNode : XmlBaseReader.XmlTextNode
		{
			public XmlAttributeTextNode(PrefixHandle prefix, StringHandle localName, ValueHandle value)
				: base(XmlNodeType.Text, prefix, localName, value, (XmlBaseReader.XmlNode.XmlNodeFlags)47, ReadState.Interactive, null, 1)
			{
			}
		}

		protected class XmlInitialNode : XmlBaseReader.XmlNode
		{
			public XmlInitialNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.None, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), XmlBaseReader.XmlNode.XmlNodeFlags.None, ReadState.Initial, null, 0)
			{
			}
		}

		protected class XmlDeclarationNode : XmlBaseReader.XmlNode
		{
			public XmlDeclarationNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.XmlDeclaration, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), XmlBaseReader.XmlNode.XmlNodeFlags.CanGetAttribute, ReadState.Interactive, null, 0)
			{
			}
		}

		protected class XmlCommentNode : XmlBaseReader.XmlNode
		{
			public XmlCommentNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.Comment, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), XmlBaseReader.XmlNode.XmlNodeFlags.HasValue, ReadState.Interactive, null, 0)
			{
			}
		}

		protected class XmlEndOfFileNode : XmlBaseReader.XmlNode
		{
			public XmlEndOfFileNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.None, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), XmlBaseReader.XmlNode.XmlNodeFlags.None, ReadState.EndOfFile, null, 0)
			{
			}
		}

		protected class XmlClosedNode : XmlBaseReader.XmlNode
		{
			public XmlClosedNode(XmlBufferReader bufferReader)
				: base(XmlNodeType.None, new PrefixHandle(bufferReader), new StringHandle(bufferReader), new ValueHandle(bufferReader), XmlBaseReader.XmlNode.XmlNodeFlags.None, ReadState.Closed, null, 0)
			{
			}
		}

		private class AttributeSorter : IComparer
		{
			public bool Sort(XmlBaseReader.XmlAttributeNode[] attributeNodes, int attributeCount)
			{
				this.attributeIndex1 = -1;
				this.attributeIndex2 = -1;
				this.attributeNodes = attributeNodes;
				this.attributeCount = attributeCount;
				bool flag = this.Sort();
				this.attributeNodes = null;
				this.attributeCount = 0;
				return flag;
			}

			public void GetIndeces(out int attributeIndex1, out int attributeIndex2)
			{
				attributeIndex1 = this.attributeIndex1;
				attributeIndex2 = this.attributeIndex2;
			}

			public void Close()
			{
				if (this.indeces != null && this.indeces.Length > 32)
				{
					this.indeces = null;
				}
			}

			private bool Sort()
			{
				if (this.indeces != null && this.indeces.Length == this.attributeCount && this.IsSorted())
				{
					return true;
				}
				object[] array = new object[this.attributeCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = i;
				}
				this.indeces = array;
				Array.Sort(this.indeces, 0, this.attributeCount, this);
				return this.IsSorted();
			}

			private bool IsSorted()
			{
				for (int i = 0; i < this.indeces.Length - 1; i++)
				{
					if (this.Compare(this.indeces[i], this.indeces[i + 1]) >= 0)
					{
						this.attributeIndex1 = (int)this.indeces[i];
						this.attributeIndex2 = (int)this.indeces[i + 1];
						return false;
					}
				}
				return true;
			}

			public int Compare(object obj1, object obj2)
			{
				int num = (int)obj1;
				int num2 = (int)obj2;
				XmlBaseReader.XmlAttributeNode xmlAttributeNode = this.attributeNodes[num];
				XmlBaseReader.XmlAttributeNode xmlAttributeNode2 = this.attributeNodes[num2];
				int num3 = this.CompareQNameType(xmlAttributeNode.QNameType, xmlAttributeNode2.QNameType);
				if (num3 == 0)
				{
					if (xmlAttributeNode.QNameType == XmlBaseReader.QNameType.Normal)
					{
						num3 = xmlAttributeNode.LocalName.CompareTo(xmlAttributeNode2.LocalName);
						if (num3 == 0)
						{
							num3 = xmlAttributeNode.Namespace.Uri.CompareTo(xmlAttributeNode2.Namespace.Uri);
						}
					}
					else
					{
						num3 = xmlAttributeNode.Namespace.Prefix.CompareTo(xmlAttributeNode2.Namespace.Prefix);
					}
				}
				return num3;
			}

			public int CompareQNameType(XmlBaseReader.QNameType type1, XmlBaseReader.QNameType type2)
			{
				return type1 - type2;
			}

			private object[] indeces;

			private XmlBaseReader.XmlAttributeNode[] attributeNodes;

			private int attributeCount;

			private int attributeIndex1;

			private int attributeIndex2;
		}

		private class NamespaceManager
		{
			public NamespaceManager(XmlBufferReader bufferReader)
			{
				this.bufferReader = bufferReader;
				this.shortPrefixUri = new XmlBaseReader.Namespace[28];
				this.shortPrefixUri[0] = XmlBaseReader.NamespaceManager.emptyNamespace;
				this.namespaces = null;
				this.nsCount = 0;
				this.attributes = null;
				this.attributeCount = 0;
				this.space = XmlSpace.None;
				this.lang = string.Empty;
				this.depth = 0;
			}

			public void Close()
			{
				if (this.namespaces != null && this.namespaces.Length > 32)
				{
					this.namespaces = null;
				}
				if (this.attributes != null && this.attributes.Length > 4)
				{
					this.attributes = null;
				}
				this.lang = string.Empty;
			}

			public static XmlBaseReader.Namespace XmlNamespace
			{
				get
				{
					if (XmlBaseReader.NamespaceManager.xmlNamespace == null)
					{
						byte[] array = new byte[]
						{
							120, 109, 108, 104, 116, 116, 112, 58, 47, 47,
							119, 119, 119, 46, 119, 51, 46, 111, 114, 103,
							47, 88, 77, 76, 47, 49, 57, 57, 56, 47,
							110, 97, 109, 101, 115, 112, 97, 99, 101
						};
						XmlBaseReader.Namespace @namespace = new XmlBaseReader.Namespace(new XmlBufferReader(array));
						@namespace.Prefix.SetValue(0, 3);
						@namespace.Uri.SetValue(3, array.Length - 3);
						XmlBaseReader.NamespaceManager.xmlNamespace = @namespace;
					}
					return XmlBaseReader.NamespaceManager.xmlNamespace;
				}
			}

			public static XmlBaseReader.Namespace EmptyNamespace
			{
				get
				{
					return XmlBaseReader.NamespaceManager.emptyNamespace;
				}
			}

			public string XmlLang
			{
				get
				{
					return this.lang;
				}
			}

			public XmlSpace XmlSpace
			{
				get
				{
					return this.space;
				}
			}

			public void Clear()
			{
				if (this.nsCount != 0)
				{
					if (this.shortPrefixUri != null)
					{
						for (int i = 0; i < this.shortPrefixUri.Length; i++)
						{
							this.shortPrefixUri[i] = null;
						}
					}
					this.shortPrefixUri[0] = XmlBaseReader.NamespaceManager.emptyNamespace;
					this.nsCount = 0;
				}
				this.attributeCount = 0;
				this.space = XmlSpace.None;
				this.lang = string.Empty;
				this.depth = 0;
			}

			public void EnterScope()
			{
				this.depth++;
			}

			public void ExitScope()
			{
				while (this.nsCount > 0)
				{
					XmlBaseReader.Namespace @namespace = this.namespaces[this.nsCount - 1];
					if (@namespace.Depth != this.depth)
					{
						IL_009A:
						while (this.attributeCount > 0)
						{
							XmlBaseReader.NamespaceManager.XmlAttribute xmlAttribute = this.attributes[this.attributeCount - 1];
							if (xmlAttribute.Depth != this.depth)
							{
								break;
							}
							this.space = xmlAttribute.XmlSpace;
							this.lang = xmlAttribute.XmlLang;
							this.attributeCount--;
						}
						this.depth--;
						return;
					}
					PrefixHandleType prefixHandleType;
					if (@namespace.Prefix.TryGetShortPrefix(out prefixHandleType))
					{
						this.shortPrefixUri[(int)prefixHandleType] = @namespace.OuterUri;
					}
					this.nsCount--;
				}
				goto IL_009A;
			}

			public void Sign(XmlSigningNodeWriter writer)
			{
				for (int i = 0; i < this.nsCount; i++)
				{
					PrefixHandle prefix = this.namespaces[i].Prefix;
					bool flag = false;
					for (int j = i + 1; j < this.nsCount; j++)
					{
						if (object.Equals(prefix, this.namespaces[j].Prefix))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						int num;
						int num2;
						byte[] @string = prefix.GetString(out num, out num2);
						int num3;
						int num4;
						byte[] string2 = this.namespaces[i].Uri.GetString(out num3, out num4);
						writer.WriteXmlnsAttribute(@string, num, num2, string2, num3, num4);
					}
				}
			}

			public void AddLangAttribute(string lang)
			{
				this.AddAttribute();
				this.lang = lang;
			}

			public void AddSpaceAttribute(XmlSpace space)
			{
				this.AddAttribute();
				this.space = space;
			}

			private void AddAttribute()
			{
				if (this.attributes == null)
				{
					this.attributes = new XmlBaseReader.NamespaceManager.XmlAttribute[1];
				}
				else if (this.attributes.Length == this.attributeCount)
				{
					XmlBaseReader.NamespaceManager.XmlAttribute[] array = new XmlBaseReader.NamespaceManager.XmlAttribute[this.attributeCount * 2];
					Array.Copy(this.attributes, array, this.attributeCount);
					this.attributes = array;
				}
				XmlBaseReader.NamespaceManager.XmlAttribute xmlAttribute = this.attributes[this.attributeCount];
				if (xmlAttribute == null)
				{
					xmlAttribute = new XmlBaseReader.NamespaceManager.XmlAttribute();
					this.attributes[this.attributeCount] = xmlAttribute;
				}
				xmlAttribute.XmlLang = this.lang;
				xmlAttribute.XmlSpace = this.space;
				xmlAttribute.Depth = this.depth;
				this.attributeCount++;
			}

			public void Register(XmlBaseReader.Namespace nameSpace)
			{
				PrefixHandleType prefixHandleType;
				if (nameSpace.Prefix.TryGetShortPrefix(out prefixHandleType))
				{
					nameSpace.OuterUri = this.shortPrefixUri[(int)prefixHandleType];
					this.shortPrefixUri[(int)prefixHandleType] = nameSpace;
					return;
				}
				nameSpace.OuterUri = null;
			}

			public XmlBaseReader.Namespace AddNamespace()
			{
				if (this.namespaces == null)
				{
					this.namespaces = new XmlBaseReader.Namespace[4];
				}
				else if (this.namespaces.Length == this.nsCount)
				{
					XmlBaseReader.Namespace[] array = new XmlBaseReader.Namespace[this.nsCount * 2];
					Array.Copy(this.namespaces, array, this.nsCount);
					this.namespaces = array;
				}
				XmlBaseReader.Namespace @namespace = this.namespaces[this.nsCount];
				if (@namespace == null)
				{
					@namespace = new XmlBaseReader.Namespace(this.bufferReader);
					this.namespaces[this.nsCount] = @namespace;
				}
				@namespace.Clear();
				@namespace.Depth = this.depth;
				this.nsCount++;
				return @namespace;
			}

			public XmlBaseReader.Namespace LookupNamespace(PrefixHandleType prefix)
			{
				return this.shortPrefixUri[(int)prefix];
			}

			public XmlBaseReader.Namespace LookupNamespace(PrefixHandle prefix)
			{
				PrefixHandleType prefixHandleType;
				if (prefix.TryGetShortPrefix(out prefixHandleType))
				{
					return this.LookupNamespace(prefixHandleType);
				}
				for (int i = this.nsCount - 1; i >= 0; i--)
				{
					XmlBaseReader.Namespace @namespace = this.namespaces[i];
					if (@namespace.Prefix == prefix)
					{
						return @namespace;
					}
				}
				if (prefix.IsXml)
				{
					return XmlBaseReader.NamespaceManager.XmlNamespace;
				}
				return null;
			}

			public XmlBaseReader.Namespace LookupNamespace(string prefix)
			{
				PrefixHandleType prefixHandleType;
				if (this.TryGetShortPrefix(prefix, out prefixHandleType))
				{
					return this.LookupNamespace(prefixHandleType);
				}
				for (int i = this.nsCount - 1; i >= 0; i--)
				{
					XmlBaseReader.Namespace @namespace = this.namespaces[i];
					if (@namespace.Prefix == prefix)
					{
						return @namespace;
					}
				}
				if (prefix == "xml")
				{
					return XmlBaseReader.NamespaceManager.XmlNamespace;
				}
				return null;
			}

			private bool TryGetShortPrefix(string s, out PrefixHandleType shortPrefix)
			{
				int length = s.Length;
				if (length == 0)
				{
					shortPrefix = PrefixHandleType.Empty;
					return true;
				}
				if (length == 1)
				{
					char c = s[0];
					if (c >= 'a' && c <= 'z')
					{
						shortPrefix = PrefixHandle.GetAlphaPrefix((int)(c - 'a'));
						return true;
					}
				}
				shortPrefix = PrefixHandleType.Empty;
				return false;
			}

			private XmlBufferReader bufferReader;

			private XmlBaseReader.Namespace[] namespaces;

			private int nsCount;

			private int depth;

			private XmlBaseReader.Namespace[] shortPrefixUri;

			private static XmlBaseReader.Namespace emptyNamespace = new XmlBaseReader.Namespace(XmlBufferReader.Empty);

			private static XmlBaseReader.Namespace xmlNamespace;

			private XmlBaseReader.NamespaceManager.XmlAttribute[] attributes;

			private int attributeCount;

			private XmlSpace space;

			private string lang;

			private class XmlAttribute
			{
				public int Depth
				{
					get
					{
						return this.depth;
					}
					set
					{
						this.depth = value;
					}
				}

				public string XmlLang
				{
					get
					{
						return this.lang;
					}
					set
					{
						this.lang = value;
					}
				}

				public XmlSpace XmlSpace
				{
					get
					{
						return this.space;
					}
					set
					{
						this.space = value;
					}
				}

				private XmlSpace space;

				private string lang;

				private int depth;
			}
		}

		protected class Namespace
		{
			public Namespace(XmlBufferReader bufferReader)
			{
				this.prefix = new PrefixHandle(bufferReader);
				this.uri = new StringHandle(bufferReader);
				this.outerUri = null;
				this.uriString = null;
			}

			public void Clear()
			{
				this.uriString = null;
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
				set
				{
					this.depth = value;
				}
			}

			public PrefixHandle Prefix
			{
				get
				{
					return this.prefix;
				}
			}

			public bool IsUri(string s)
			{
				if (s == this.uriString)
				{
					return true;
				}
				if (this.uri == s)
				{
					this.uriString = s;
					return true;
				}
				return false;
			}

			public bool IsUri(XmlDictionaryString s)
			{
				if (s.Value == this.uriString)
				{
					return true;
				}
				if (this.uri == s)
				{
					this.uriString = s.Value;
					return true;
				}
				return false;
			}

			public StringHandle Uri
			{
				get
				{
					return this.uri;
				}
			}

			public XmlBaseReader.Namespace OuterUri
			{
				get
				{
					return this.outerUri;
				}
				set
				{
					this.outerUri = value;
				}
			}

			private PrefixHandle prefix;

			private StringHandle uri;

			private int depth;

			private XmlBaseReader.Namespace outerUri;

			private string uriString;
		}

		private class QuotaNameTable : XmlNameTable
		{
			public QuotaNameTable(XmlDictionaryReader reader, int maxCharCount)
			{
				this.reader = reader;
				this.nameTable = new NameTable();
				this.maxCharCount = maxCharCount;
				this.charCount = 0;
			}

			public override string Get(char[] chars, int offset, int count)
			{
				return this.nameTable.Get(chars, offset, count);
			}

			public override string Get(string value)
			{
				return this.nameTable.Get(value);
			}

			private void Add(int charCount)
			{
				if (charCount > this.maxCharCount - this.charCount)
				{
					XmlExceptionHelper.ThrowMaxNameTableCharCountExceeded(this.reader, this.maxCharCount);
				}
				this.charCount += charCount;
			}

			public override string Add(char[] chars, int offset, int count)
			{
				string text = this.nameTable.Get(chars, offset, count);
				if (text != null)
				{
					return text;
				}
				this.Add(count);
				return this.nameTable.Add(chars, offset, count);
			}

			public override string Add(string value)
			{
				string text = this.nameTable.Get(value);
				if (text != null)
				{
					return text;
				}
				this.Add(value.Length);
				return this.nameTable.Add(value);
			}

			private XmlDictionaryReader reader;

			private XmlNameTable nameTable;

			private int maxCharCount;

			private int charCount;
		}
	}
}
