using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;
using Mono.Xml;
using Mono.Xml.XPath;

namespace System.Xml
{
	public class XmlDocument : XmlNode, IHasXmlChildNode
	{
		public XmlDocument()
			: this(null, null)
		{
		}

		protected internal XmlDocument(XmlImplementation imp)
			: this(imp, null)
		{
		}

		public XmlDocument(XmlNameTable nt)
			: this(null, nt)
		{
		}

		private XmlDocument(XmlImplementation impl, XmlNameTable nt)
			: base(null)
		{
			if (impl == null)
			{
				this.implementation = new XmlImplementation();
			}
			else
			{
				this.implementation = impl;
			}
			this.nameTable = ((nt == null) ? this.implementation.InternalNameTable : nt);
			this.nameCache = new XmlNameEntryCache(this.nameTable);
			this.AddDefaultNameTableKeys();
			this.resolver = new XmlUrlResolver();
			Type type = base.GetType();
			this.optimal_create_element = type.GetMethod("CreateElement", XmlDocument.optimal_create_types).DeclaringType == typeof(XmlDocument);
			this.optimal_create_attribute = type.GetMethod("CreateAttribute", XmlDocument.optimal_create_types).DeclaringType == typeof(XmlDocument);
		}

		public event XmlNodeChangedEventHandler NodeChanged;

		public event XmlNodeChangedEventHandler NodeChanging;

		public event XmlNodeChangedEventHandler NodeInserted;

		public event XmlNodeChangedEventHandler NodeInserting;

		public event XmlNodeChangedEventHandler NodeRemoved;

		public event XmlNodeChangedEventHandler NodeRemoving;

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

		internal XmlAttribute NsNodeXml
		{
			get
			{
				if (this.nsNodeXml == null)
				{
					this.nsNodeXml = this.CreateAttribute("xmlns", "xml", "http://www.w3.org/2000/xmlns/");
					this.nsNodeXml.Value = "http://www.w3.org/XML/1998/namespace";
				}
				return this.nsNodeXml;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.baseURI;
			}
		}

		public XmlElement DocumentElement
		{
			get
			{
				XmlNode xmlNode;
				for (xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode is XmlElement)
					{
						break;
					}
				}
				return (xmlNode == null) ? null : (xmlNode as XmlElement);
			}
		}

		public virtual XmlDocumentType DocumentType
		{
			get
			{
				for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.NodeType == XmlNodeType.DocumentType)
					{
						return (XmlDocumentType)xmlNode;
					}
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						return null;
					}
				}
				return null;
			}
		}

		public XmlImplementation Implementation
		{
			get
			{
				return this.implementation;
			}
		}

		public override string InnerXml
		{
			get
			{
				return base.InnerXml;
			}
			set
			{
				this.LoadXml(value);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal bool IsStandalone
		{
			get
			{
				return this.FirstChild != null && this.FirstChild.NodeType == XmlNodeType.XmlDeclaration && ((XmlDeclaration)this.FirstChild).Standalone == "yes";
			}
		}

		public override string LocalName
		{
			get
			{
				return "#document";
			}
		}

		public override string Name
		{
			get
			{
				return "#document";
			}
		}

		internal XmlNameEntryCache NameCache
		{
			get
			{
				return this.nameCache;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Document;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return null;
			}
		}

		public bool PreserveWhitespace
		{
			get
			{
				return this.preserveWhitespace;
			}
			set
			{
				this.preserveWhitespace = value;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return this.resolver;
			}
		}

		internal override string XmlLang
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual XmlResolver XmlResolver
		{
			set
			{
				this.resolver = value;
			}
		}

		internal override XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		internal Encoding TextEncoding
		{
			get
			{
				XmlDeclaration xmlDeclaration = this.FirstChild as XmlDeclaration;
				if (xmlDeclaration == null || xmlDeclaration.Encoding == string.Empty)
				{
					return null;
				}
				return Encoding.GetEncoding(xmlDeclaration.Encoding);
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				if (this.schemas == null)
				{
					this.schemas = new XmlSchemaSet();
				}
				return this.schemas;
			}
			set
			{
				this.schemas = value;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this.schemaInfo;
			}
			internal set
			{
				this.schemaInfo = value;
			}
		}

		internal void AddIdenticalAttribute(XmlAttribute attr)
		{
			this.idTable[attr.Value] = attr;
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument xmlDocument = ((this.implementation == null) ? new XmlDocument() : this.implementation.CreateDocument());
			xmlDocument.baseURI = this.baseURI;
			if (deep)
			{
				for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					xmlDocument.AppendChild(xmlDocument.ImportNode(xmlNode, deep), false);
				}
			}
			return xmlDocument;
		}

		public XmlAttribute CreateAttribute(string name)
		{
			string text = string.Empty;
			string text2;
			string text3;
			this.ParseName(name, out text2, out text3);
			if (text2 == "xmlns" || (text2 == string.Empty && text3 == "xmlns"))
			{
				text = "http://www.w3.org/2000/xmlns/";
			}
			else if (text2 == "xml")
			{
				text = "http://www.w3.org/XML/1998/namespace";
			}
			return this.CreateAttribute(text2, text3, text);
		}

		public XmlAttribute CreateAttribute(string qualifiedName, string namespaceURI)
		{
			string text;
			string text2;
			this.ParseName(qualifiedName, out text, out text2);
			return this.CreateAttribute(text, text2, namespaceURI);
		}

		public virtual XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			if (localName == null || localName == string.Empty)
			{
				throw new ArgumentException("The attribute local name cannot be empty.");
			}
			return new XmlAttribute(prefix, localName, namespaceURI, this, false, true);
		}

		internal XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI, bool atomizedNames, bool checkNamespace)
		{
			if (this.optimal_create_attribute)
			{
				return new XmlAttribute(prefix, localName, namespaceURI, this, atomizedNames, checkNamespace);
			}
			return this.CreateAttribute(prefix, localName, namespaceURI);
		}

		public virtual XmlCDataSection CreateCDataSection(string data)
		{
			return new XmlCDataSection(data, this);
		}

		public virtual XmlComment CreateComment(string data)
		{
			return new XmlComment(data, this);
		}

		protected internal virtual XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
		{
			XmlAttribute xmlAttribute = this.CreateAttribute(prefix, localName, namespaceURI);
			xmlAttribute.isDefault = true;
			return xmlAttribute;
		}

		public virtual XmlDocumentFragment CreateDocumentFragment()
		{
			return new XmlDocumentFragment(this);
		}

		[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public virtual XmlDocumentType CreateDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XmlDocumentType(name, publicId, systemId, internalSubset, this);
		}

		private XmlDocumentType CreateDocumentType(DTDObjectModel dtd)
		{
			return new XmlDocumentType(dtd, this);
		}

		public XmlElement CreateElement(string name)
		{
			return this.CreateElement(name, string.Empty);
		}

		public XmlElement CreateElement(string qualifiedName, string namespaceURI)
		{
			string text;
			string text2;
			this.ParseName(qualifiedName, out text, out text2);
			return this.CreateElement(text, text2, namespaceURI);
		}

		public virtual XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			return new XmlElement((prefix == null) ? string.Empty : prefix, localName, (namespaceURI == null) ? string.Empty : namespaceURI, this, false);
		}

		internal XmlElement CreateElement(string prefix, string localName, string namespaceURI, bool nameAtomized)
		{
			if (localName == null || localName == string.Empty)
			{
				throw new ArgumentException("The local name for elements or attributes cannot be null or an empty string.");
			}
			if (this.optimal_create_element)
			{
				return new XmlElement((prefix == null) ? string.Empty : prefix, localName, (namespaceURI == null) ? string.Empty : namespaceURI, this, nameAtomized);
			}
			return this.CreateElement(prefix, localName, namespaceURI);
		}

		public virtual XmlEntityReference CreateEntityReference(string name)
		{
			return new XmlEntityReference(name, this);
		}

		public override XPathNavigator CreateNavigator()
		{
			return this.CreateNavigator(this);
		}

		protected internal virtual XPathNavigator CreateNavigator(XmlNode node)
		{
			return new XPathEditableDocument(node).CreateNavigator();
		}

		public virtual XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
		{
			return this.CreateNode(this.GetNodeTypeFromString(nodeTypeString), name, namespaceURI);
		}

		public virtual XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
		{
			string text = null;
			string text2 = name;
			if (type == XmlNodeType.Attribute || type == XmlNodeType.Element || type == XmlNodeType.EntityReference)
			{
				this.ParseName(name, out text, out text2);
			}
			return this.CreateNode(type, text, text2, namespaceURI);
		}

		public virtual XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
		{
			switch (type)
			{
			case XmlNodeType.Element:
				return this.CreateElement(prefix, name, namespaceURI);
			case XmlNodeType.Attribute:
				return this.CreateAttribute(prefix, name, namespaceURI);
			case XmlNodeType.Text:
				return this.CreateTextNode(null);
			case XmlNodeType.CDATA:
				return this.CreateCDataSection(null);
			case XmlNodeType.EntityReference:
				return this.CreateEntityReference(null);
			case XmlNodeType.ProcessingInstruction:
				return this.CreateProcessingInstruction(null, null);
			case XmlNodeType.Comment:
				return this.CreateComment(null);
			case XmlNodeType.Document:
				return new XmlDocument();
			case XmlNodeType.DocumentType:
				return this.CreateDocumentType(null, null, null, null);
			case XmlNodeType.DocumentFragment:
				return this.CreateDocumentFragment();
			case XmlNodeType.Whitespace:
				return this.CreateWhitespace(string.Empty);
			case XmlNodeType.SignificantWhitespace:
				return this.CreateSignificantWhitespace(string.Empty);
			case XmlNodeType.XmlDeclaration:
				return this.CreateXmlDeclaration("1.0", null, null);
			}
			throw new ArgumentException(string.Format("{0}\nParameter name: {1}", "Specified argument was out of the range of valid values", type.ToString()));
		}

		public virtual XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
		{
			return new XmlProcessingInstruction(target, data, this);
		}

		public virtual XmlSignificantWhitespace CreateSignificantWhitespace(string text)
		{
			if (!XmlChar.IsWhitespace(text))
			{
				throw new ArgumentException("Invalid whitespace characters.");
			}
			return new XmlSignificantWhitespace(text, this);
		}

		public virtual XmlText CreateTextNode(string text)
		{
			return new XmlText(text, this);
		}

		public virtual XmlWhitespace CreateWhitespace(string text)
		{
			if (!XmlChar.IsWhitespace(text))
			{
				throw new ArgumentException("Invalid whitespace characters.");
			}
			return new XmlWhitespace(text, this);
		}

		public virtual XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			if (version != "1.0")
			{
				throw new ArgumentException("version string is not correct.");
			}
			if (standalone != null && standalone != string.Empty && !(standalone == "yes") && !(standalone == "no"))
			{
				throw new ArgumentException("standalone string is not correct.");
			}
			return new XmlDeclaration(version, encoding, standalone, this);
		}

		public virtual XmlElement GetElementById(string elementId)
		{
			XmlAttribute identicalAttribute = this.GetIdenticalAttribute(elementId);
			return (identicalAttribute == null) ? null : identicalAttribute.OwnerElement;
		}

		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			ArrayList arrayList = new ArrayList();
			base.SearchDescendantElements(name, name == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
		{
			ArrayList arrayList = new ArrayList();
			base.SearchDescendantElements(localName, localName == "*", namespaceURI, namespaceURI == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		private XmlNodeType GetNodeTypeFromString(string nodeTypeString)
		{
			if (nodeTypeString == null)
			{
				throw new ArgumentNullException("nodeTypeString");
			}
			switch (nodeTypeString)
			{
			case "attribute":
				return XmlNodeType.Attribute;
			case "cdatasection":
				return XmlNodeType.CDATA;
			case "comment":
				return XmlNodeType.Comment;
			case "document":
				return XmlNodeType.Document;
			case "documentfragment":
				return XmlNodeType.DocumentFragment;
			case "documenttype":
				return XmlNodeType.DocumentType;
			case "element":
				return XmlNodeType.Element;
			case "entityreference":
				return XmlNodeType.EntityReference;
			case "processinginstruction":
				return XmlNodeType.ProcessingInstruction;
			case "significantwhitespace":
				return XmlNodeType.SignificantWhitespace;
			case "text":
				return XmlNodeType.Text;
			case "whitespace":
				return XmlNodeType.Whitespace;
			}
			throw new ArgumentException(string.Format("The string doesn't represent any node type : {0}.", nodeTypeString));
		}

		internal XmlAttribute GetIdenticalAttribute(string id)
		{
			XmlAttribute xmlAttribute = this.idTable[id] as XmlAttribute;
			if (xmlAttribute == null)
			{
				return null;
			}
			if (xmlAttribute.OwnerElement == null || !xmlAttribute.OwnerElement.IsRooted)
			{
				return null;
			}
			return xmlAttribute;
		}

		public virtual XmlNode ImportNode(XmlNode node, bool deep)
		{
			if (node == null)
			{
				throw new NullReferenceException("Null node cannot be imported.");
			}
			switch (node.NodeType)
			{
			case XmlNodeType.None:
				throw new XmlException("Illegal ImportNode call for NodeType.None");
			case XmlNodeType.Element:
			{
				XmlElement xmlElement = (XmlElement)node;
				XmlElement xmlElement2 = this.CreateElement(xmlElement.Prefix, xmlElement.LocalName, xmlElement.NamespaceURI);
				for (int i = 0; i < xmlElement.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = xmlElement.Attributes[i];
					if (xmlAttribute.Specified)
					{
						xmlElement2.SetAttributeNode((XmlAttribute)this.ImportNode(xmlAttribute, deep));
					}
				}
				if (deep)
				{
					for (XmlNode xmlNode = xmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
					{
						xmlElement2.AppendChild(this.ImportNode(xmlNode, deep));
					}
				}
				return xmlElement2;
			}
			case XmlNodeType.Attribute:
			{
				XmlAttribute xmlAttribute2 = node as XmlAttribute;
				XmlAttribute xmlAttribute3 = this.CreateAttribute(xmlAttribute2.Prefix, xmlAttribute2.LocalName, xmlAttribute2.NamespaceURI);
				for (XmlNode xmlNode2 = xmlAttribute2.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
				{
					xmlAttribute3.AppendChild(this.ImportNode(xmlNode2, deep));
				}
				return xmlAttribute3;
			}
			case XmlNodeType.Text:
				return this.CreateTextNode(node.Value);
			case XmlNodeType.CDATA:
				return this.CreateCDataSection(node.Value);
			case XmlNodeType.EntityReference:
				return this.CreateEntityReference(node.Name);
			case XmlNodeType.ProcessingInstruction:
			{
				XmlProcessingInstruction xmlProcessingInstruction = node as XmlProcessingInstruction;
				return this.CreateProcessingInstruction(xmlProcessingInstruction.Target, xmlProcessingInstruction.Data);
			}
			case XmlNodeType.Comment:
				return this.CreateComment(node.Value);
			case XmlNodeType.Document:
				throw new XmlException("Document cannot be imported.");
			case XmlNodeType.DocumentType:
				throw new XmlException("DocumentType cannot be imported.");
			case XmlNodeType.DocumentFragment:
			{
				XmlDocumentFragment xmlDocumentFragment = this.CreateDocumentFragment();
				if (deep)
				{
					for (XmlNode xmlNode3 = node.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
					{
						xmlDocumentFragment.AppendChild(this.ImportNode(xmlNode3, deep));
					}
				}
				return xmlDocumentFragment;
			}
			case XmlNodeType.Whitespace:
				return this.CreateWhitespace(node.Value);
			case XmlNodeType.SignificantWhitespace:
				return this.CreateSignificantWhitespace(node.Value);
			case XmlNodeType.EndElement:
				throw new XmlException("Illegal ImportNode call for NodeType.EndElement");
			case XmlNodeType.EndEntity:
				throw new XmlException("Illegal ImportNode call for NodeType.EndEntity");
			case XmlNodeType.XmlDeclaration:
			{
				XmlDeclaration xmlDeclaration = node as XmlDeclaration;
				return this.CreateXmlDeclaration(xmlDeclaration.Version, xmlDeclaration.Encoding, xmlDeclaration.Standalone);
			}
			}
			throw new InvalidOperationException("Cannot import specified node type: " + node.NodeType);
		}

		public virtual void Load(Stream inStream)
		{
			this.Load(new XmlValidatingReader(new XmlTextReader(inStream, this.NameTable)
			{
				XmlResolver = this.resolver
			})
			{
				EntityHandling = EntityHandling.ExpandCharEntities,
				ValidationType = ValidationType.None
			});
		}

		public virtual void Load(string filename)
		{
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(filename, this.NameTable);
				xmlTextReader.XmlResolver = this.resolver;
				this.Load(new XmlValidatingReader(xmlTextReader)
				{
					EntityHandling = EntityHandling.ExpandCharEntities,
					ValidationType = ValidationType.None
				});
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		public virtual void Load(TextReader txtReader)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(txtReader, this.NameTable);
			XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
			xmlValidatingReader.EntityHandling = EntityHandling.ExpandCharEntities;
			xmlValidatingReader.ValidationType = ValidationType.None;
			xmlTextReader.XmlResolver = this.resolver;
			this.Load(xmlValidatingReader);
		}

		public virtual void Load(XmlReader xmlReader)
		{
			this.RemoveAll();
			this.baseURI = xmlReader.BaseURI;
			try
			{
				this.loadMode = true;
				for (;;)
				{
					XmlNode xmlNode = this.ReadNode(xmlReader);
					if (xmlNode == null)
					{
						break;
					}
					if (this.preserveWhitespace || xmlNode.NodeType != XmlNodeType.Whitespace)
					{
						base.AppendChild(xmlNode, false);
					}
				}
				if (xmlReader.Settings != null)
				{
					this.schemas = xmlReader.Settings.Schemas;
				}
			}
			finally
			{
				this.loadMode = false;
			}
		}

		public virtual void LoadXml(string xml)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(xml, XmlNodeType.Document, new XmlParserContext(this.NameTable, new XmlNamespaceManager(this.NameTable), null, XmlSpace.None));
			try
			{
				xmlTextReader.XmlResolver = this.resolver;
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		internal void onNodeChanged(XmlNode node, XmlNode parent, string oldValue, string newValue)
		{
			if (this.NodeChanged != null)
			{
				this.NodeChanged(node, new XmlNodeChangedEventArgs(node, parent, parent, oldValue, newValue, XmlNodeChangedAction.Change));
			}
		}

		internal void onNodeChanging(XmlNode node, XmlNode parent, string oldValue, string newValue)
		{
			if (node.IsReadOnly)
			{
				throw new ArgumentException("Node is read-only.");
			}
			if (this.NodeChanging != null)
			{
				this.NodeChanging(node, new XmlNodeChangedEventArgs(node, parent, parent, oldValue, newValue, XmlNodeChangedAction.Change));
			}
		}

		internal void onNodeInserted(XmlNode node, XmlNode newParent)
		{
			if (this.NodeInserted != null)
			{
				this.NodeInserted(node, new XmlNodeChangedEventArgs(node, null, newParent, null, null, XmlNodeChangedAction.Insert));
			}
		}

		internal void onNodeInserting(XmlNode node, XmlNode newParent)
		{
			if (this.NodeInserting != null)
			{
				this.NodeInserting(node, new XmlNodeChangedEventArgs(node, null, newParent, null, null, XmlNodeChangedAction.Insert));
			}
		}

		internal void onNodeRemoved(XmlNode node, XmlNode oldParent)
		{
			if (this.NodeRemoved != null)
			{
				this.NodeRemoved(node, new XmlNodeChangedEventArgs(node, oldParent, null, null, null, XmlNodeChangedAction.Remove));
			}
		}

		internal void onNodeRemoving(XmlNode node, XmlNode oldParent)
		{
			if (this.NodeRemoving != null)
			{
				this.NodeRemoving(node, new XmlNodeChangedEventArgs(node, oldParent, null, null, null, XmlNodeChangedAction.Remove));
			}
		}

		private void ParseName(string name, out string prefix, out string localName)
		{
			int num = name.IndexOf(':');
			if (num != -1)
			{
				prefix = name.Substring(0, num);
				localName = name.Substring(num + 1);
			}
			else
			{
				prefix = string.Empty;
				localName = name;
			}
		}

		private XmlAttribute ReadAttributeNode(XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				reader.MoveToFirstAttribute();
			}
			else if (reader.NodeType != XmlNodeType.Attribute)
			{
				throw new InvalidOperationException(this.MakeReaderErrorMessage("bad position to read attribute.", reader));
			}
			XmlAttribute xmlAttribute = this.CreateAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			if (reader.SchemaInfo != null)
			{
				this.SchemaInfo = new XmlSchemaInfo(reader.SchemaInfo);
			}
			bool isDefault = reader.IsDefault;
			this.ReadAttributeNodeValue(reader, xmlAttribute);
			if (isDefault)
			{
				xmlAttribute.SetDefault();
			}
			return xmlAttribute;
		}

		internal void ReadAttributeNodeValue(XmlReader reader, XmlAttribute attribute)
		{
			while (reader.ReadAttributeValue())
			{
				if (reader.NodeType == XmlNodeType.EntityReference)
				{
					attribute.AppendChild(this.CreateEntityReference(reader.Name), false);
				}
				else
				{
					attribute.AppendChild(this.CreateTextNode(reader.Value), false);
				}
			}
		}

		[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public virtual XmlNode ReadNode(XmlReader reader)
		{
			if (this.PreserveWhitespace)
			{
				return this.ReadNodeCore(reader);
			}
			XmlTextReader xmlTextReader = reader as XmlTextReader;
			if (xmlTextReader != null && xmlTextReader.WhitespaceHandling == WhitespaceHandling.All)
			{
				try
				{
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
					return this.ReadNodeCore(reader);
				}
				finally
				{
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
				}
			}
			return this.ReadNodeCore(reader);
		}

		private XmlNode ReadNodeCore(XmlReader reader)
		{
			ReadState readState = reader.ReadState;
			if (readState != ReadState.Initial)
			{
				if (readState != ReadState.Interactive)
				{
					return null;
				}
			}
			else
			{
				if (reader.SchemaInfo != null)
				{
					this.SchemaInfo = new XmlSchemaInfo(reader.SchemaInfo);
				}
				reader.Read();
			}
			XmlNode xmlNode;
			switch (reader.NodeType)
			{
			case XmlNodeType.None:
				return null;
			case XmlNodeType.Element:
			{
				XmlElement xmlElement = this.CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.NameTable == this.NameTable);
				if (reader.SchemaInfo != null)
				{
					this.SchemaInfo = new XmlSchemaInfo(reader.SchemaInfo);
				}
				xmlElement.IsEmpty = reader.IsEmptyElement;
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					xmlElement.SetAttributeNode(this.ReadAttributeNode(reader));
					reader.MoveToElement();
				}
				reader.MoveToElement();
				int depth = reader.Depth;
				if (reader.IsEmptyElement)
				{
					xmlNode = xmlElement;
					goto IL_036A;
				}
				reader.Read();
				while (reader.Depth > depth)
				{
					xmlNode = this.ReadNodeCore(reader);
					if (this.preserveWhitespace || xmlNode.NodeType != XmlNodeType.Whitespace)
					{
						xmlElement.AppendChild(xmlNode, false);
					}
				}
				xmlNode = xmlElement;
				goto IL_036A;
			}
			case XmlNodeType.Attribute:
			{
				string localName = reader.LocalName;
				string namespaceURI = reader.NamespaceURI;
				xmlNode = this.ReadAttributeNode(reader);
				reader.MoveToAttribute(localName, namespaceURI);
				return xmlNode;
			}
			case XmlNodeType.Text:
				xmlNode = this.CreateTextNode(reader.Value);
				goto IL_036A;
			case XmlNodeType.CDATA:
				xmlNode = this.CreateCDataSection(reader.Value);
				goto IL_036A;
			case XmlNodeType.EntityReference:
				if (this.loadMode && this.DocumentType != null && this.DocumentType.Entities.GetNamedItem(reader.Name) == null)
				{
					throw new XmlException("Reference to undeclared entity was found.");
				}
				xmlNode = this.CreateEntityReference(reader.Name);
				if (reader.CanResolveEntity)
				{
					reader.ResolveEntity();
					reader.Read();
					XmlNode xmlNode2;
					while (reader.NodeType != XmlNodeType.EndEntity && (xmlNode2 = this.ReadNode(reader)) != null)
					{
						xmlNode.InsertBefore(xmlNode2, null, false, false);
					}
				}
				goto IL_036A;
			case XmlNodeType.ProcessingInstruction:
				xmlNode = this.CreateProcessingInstruction(reader.Name, reader.Value);
				goto IL_036A;
			case XmlNodeType.Comment:
				xmlNode = this.CreateComment(reader.Value);
				goto IL_036A;
			case XmlNodeType.DocumentType:
			{
				DTDObjectModel dtdobjectModel = null;
				IHasXmlParserContext hasXmlParserContext = reader as IHasXmlParserContext;
				if (hasXmlParserContext != null)
				{
					dtdobjectModel = hasXmlParserContext.ParserContext.Dtd;
				}
				if (dtdobjectModel != null)
				{
					xmlNode = this.CreateDocumentType(dtdobjectModel);
				}
				else
				{
					xmlNode = this.CreateDocumentType(reader.Name, reader["PUBLIC"], reader["SYSTEM"], reader.Value);
				}
				goto IL_036A;
			}
			case XmlNodeType.Whitespace:
				xmlNode = this.CreateWhitespace(reader.Value);
				goto IL_036A;
			case XmlNodeType.SignificantWhitespace:
				xmlNode = this.CreateSignificantWhitespace(reader.Value);
				goto IL_036A;
			case XmlNodeType.XmlDeclaration:
				xmlNode = this.CreateXmlDeclaration("1.0", string.Empty, string.Empty);
				xmlNode.Value = reader.Value;
				goto IL_036A;
			}
			throw new NullReferenceException("Unexpected node type " + reader.NodeType + ".");
			IL_036A:
			reader.Read();
			return xmlNode;
		}

		private string MakeReaderErrorMessage(string message, XmlReader reader)
		{
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			if (xmlLineInfo != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} Line number = {1}, Inline position = {2}.", new object[] { message, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition });
			}
			return message;
		}

		internal void RemoveIdenticalAttribute(string id)
		{
			this.idTable.Remove(id);
		}

		public virtual void Save(Stream outStream)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(outStream, this.TextEncoding);
			if (!this.PreserveWhitespace)
			{
				xmlTextWriter.Formatting = Formatting.Indented;
			}
			this.WriteContentTo(xmlTextWriter);
			xmlTextWriter.Flush();
		}

		public virtual void Save(string filename)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(filename, this.TextEncoding);
			try
			{
				if (!this.PreserveWhitespace)
				{
					xmlTextWriter.Formatting = Formatting.Indented;
				}
				this.WriteContentTo(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.Close();
			}
		}

		public virtual void Save(TextWriter writer)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
			if (!this.PreserveWhitespace)
			{
				xmlTextWriter.Formatting = Formatting.Indented;
			}
			if (this.FirstChild != null && this.FirstChild.NodeType != XmlNodeType.XmlDeclaration)
			{
				xmlTextWriter.WriteStartDocument();
			}
			this.WriteContentTo(xmlTextWriter);
			xmlTextWriter.WriteEndDocument();
			xmlTextWriter.Flush();
		}

		public virtual void Save(XmlWriter xmlWriter)
		{
			bool flag = this.FirstChild != null && this.FirstChild.NodeType != XmlNodeType.XmlDeclaration;
			if (flag)
			{
				xmlWriter.WriteStartDocument();
			}
			this.WriteContentTo(xmlWriter);
			if (flag)
			{
				xmlWriter.WriteEndDocument();
			}
			xmlWriter.Flush();
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				xmlNode.WriteTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			this.WriteContentTo(w);
		}

		private void AddDefaultNameTableKeys()
		{
			this.nameTable.Add("#text");
			this.nameTable.Add("xml");
			this.nameTable.Add("xmlns");
			this.nameTable.Add("#entity");
			this.nameTable.Add("#document-fragment");
			this.nameTable.Add("#comment");
			this.nameTable.Add("space");
			this.nameTable.Add("id");
			this.nameTable.Add("#whitespace");
			this.nameTable.Add("http://www.w3.org/2000/xmlns/");
			this.nameTable.Add("#cdata-section");
			this.nameTable.Add("lang");
			this.nameTable.Add("#document");
			this.nameTable.Add("#significant-whitespace");
		}

		internal void CheckIdTableUpdate(XmlAttribute attr, string oldValue, string newValue)
		{
			if (this.idTable[oldValue] == attr)
			{
				this.idTable.Remove(oldValue);
				this.idTable[newValue] = attr;
			}
		}

		public void Validate(ValidationEventHandler handler)
		{
			this.Validate(handler, this, XmlSchemaValidationFlags.ProcessIdentityConstraints);
		}

		public void Validate(ValidationEventHandler handler, XmlNode node)
		{
			this.Validate(handler, node, XmlSchemaValidationFlags.ProcessIdentityConstraints);
		}

		private void Validate(ValidationEventHandler handler, XmlNode node, XmlSchemaValidationFlags flags)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.NameTable = this.NameTable;
			xmlReaderSettings.Schemas = this.schemas;
			xmlReaderSettings.Schemas.XmlResolver = this.resolver;
			xmlReaderSettings.XmlResolver = this.resolver;
			xmlReaderSettings.ValidationFlags = flags;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			XmlReader xmlReader = XmlReader.Create(new XmlNodeReader(node), xmlReaderSettings);
			while (!xmlReader.EOF)
			{
				xmlReader.Read();
			}
		}

		private static readonly Type[] optimal_create_types = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		};

		private bool optimal_create_element;

		private bool optimal_create_attribute;

		private XmlNameTable nameTable;

		private string baseURI = string.Empty;

		private XmlImplementation implementation;

		private bool preserveWhitespace;

		private XmlResolver resolver;

		private Hashtable idTable = new Hashtable();

		private XmlNameEntryCache nameCache;

		private XmlLinkedNode lastLinkedChild;

		private XmlAttribute nsNodeXml;

		private XmlSchemaSet schemas;

		private IXmlSchemaInfo schemaInfo;

		private bool loadMode;
	}
}
