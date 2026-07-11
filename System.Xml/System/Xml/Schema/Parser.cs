using System;
using System.Threading.Tasks;
using System.Xml.XmlConfiguration;

namespace System.Xml.Schema
{
	internal sealed class Parser
	{
		public Parser(SchemaType schemaType, XmlNameTable nameTable, SchemaNames schemaNames, ValidationEventHandler eventHandler)
		{
			this.schemaType = schemaType;
			this.nameTable = nameTable;
			this.schemaNames = schemaNames;
			this.eventHandler = eventHandler;
			this.xmlResolver = XmlReaderSection.CreateDefaultResolver();
			this.processMarkup = true;
			this.dummyDocument = new XmlDocument();
		}

		public SchemaType Parse(XmlReader reader, string targetNamespace)
		{
			this.StartParsing(reader, targetNamespace);
			while (this.ParseReaderNode() && reader.Read())
			{
			}
			return this.FinishParsing();
		}

		public void StartParsing(XmlReader reader, string targetNamespace)
		{
			this.reader = reader;
			this.positionInfo = PositionInfo.GetPositionInfo(reader);
			this.namespaceManager = reader.NamespaceManager;
			if (this.namespaceManager == null)
			{
				this.namespaceManager = new XmlNamespaceManager(this.nameTable);
				this.isProcessNamespaces = true;
			}
			else
			{
				this.isProcessNamespaces = false;
			}
			while (reader.NodeType != XmlNodeType.Element && reader.Read())
			{
			}
			this.markupDepth = int.MaxValue;
			this.schemaXmlDepth = reader.Depth;
			SchemaType schemaType = this.schemaNames.SchemaTypeFromRoot(reader.LocalName, reader.NamespaceURI);
			string text;
			if (!this.CheckSchemaRoot(schemaType, out text))
			{
				throw new XmlSchemaException(text, reader.BaseURI, this.positionInfo.LineNumber, this.positionInfo.LinePosition);
			}
			if (this.schemaType == SchemaType.XSD)
			{
				this.schema = new XmlSchema();
				this.schema.BaseUri = new Uri(reader.BaseURI, UriKind.RelativeOrAbsolute);
				this.builder = new XsdBuilder(reader, this.namespaceManager, this.schema, this.nameTable, this.schemaNames, this.eventHandler);
				return;
			}
			this.xdrSchema = new SchemaInfo();
			this.xdrSchema.SchemaType = SchemaType.XDR;
			this.builder = new XdrBuilder(reader, this.namespaceManager, this.xdrSchema, targetNamespace, this.nameTable, this.schemaNames, this.eventHandler);
			((XdrBuilder)this.builder).XmlResolver = this.xmlResolver;
		}

		private bool CheckSchemaRoot(SchemaType rootType, out string code)
		{
			code = null;
			if (this.schemaType == SchemaType.None)
			{
				this.schemaType = rootType;
			}
			switch (rootType)
			{
			case SchemaType.None:
			case SchemaType.DTD:
				code = "Expected schema root. Make sure the root element is <schema> and the namespace is 'http://www.w3.org/2001/XMLSchema' for an XSD schema or 'urn:schemas-microsoft-com:xml-data' for an XDR schema.";
				if (this.schemaType == SchemaType.XSD)
				{
					code = "The root element of a W3C XML Schema should be <schema> and its namespace should be 'http://www.w3.org/2001/XMLSchema'.";
				}
				return false;
			case SchemaType.XDR:
				if (this.schemaType == SchemaType.XSD)
				{
					code = "'XmlSchemaSet' can load only W3C XML Schemas.";
					return false;
				}
				if (this.schemaType != SchemaType.XDR)
				{
					code = "Different schema types cannot be mixed.";
					return false;
				}
				break;
			case SchemaType.XSD:
				if (this.schemaType != SchemaType.XSD)
				{
					code = "Different schema types cannot be mixed.";
					return false;
				}
				break;
			}
			return true;
		}

		public SchemaType FinishParsing()
		{
			return this.schemaType;
		}

		public XmlSchema XmlSchema
		{
			get
			{
				return this.schema;
			}
		}

		internal XmlResolver XmlResolver
		{
			set
			{
				this.xmlResolver = value;
			}
		}

		public SchemaInfo XdrSchema
		{
			get
			{
				return this.xdrSchema;
			}
		}

		public bool ParseReaderNode()
		{
			if (this.reader.Depth > this.markupDepth)
			{
				if (this.processMarkup)
				{
					this.ProcessAppInfoDocMarkup(false);
				}
				return true;
			}
			if (this.reader.NodeType == XmlNodeType.Element)
			{
				if (this.builder.ProcessElement(this.reader.Prefix, this.reader.LocalName, this.reader.NamespaceURI))
				{
					this.namespaceManager.PushScope();
					if (this.reader.MoveToFirstAttribute())
					{
						do
						{
							this.builder.ProcessAttribute(this.reader.Prefix, this.reader.LocalName, this.reader.NamespaceURI, this.reader.Value);
							if (Ref.Equal(this.reader.NamespaceURI, this.schemaNames.NsXmlNs) && this.isProcessNamespaces)
							{
								this.namespaceManager.AddNamespace((this.reader.Prefix.Length == 0) ? string.Empty : this.reader.LocalName, this.reader.Value);
							}
						}
						while (this.reader.MoveToNextAttribute());
						this.reader.MoveToElement();
					}
					this.builder.StartChildren();
					if (this.reader.IsEmptyElement)
					{
						this.namespaceManager.PopScope();
						this.builder.EndChildren();
						if (this.reader.Depth == this.schemaXmlDepth)
						{
							return false;
						}
					}
					else if (!this.builder.IsContentParsed())
					{
						this.markupDepth = this.reader.Depth;
						this.processMarkup = true;
						if (this.annotationNSManager == null)
						{
							this.annotationNSManager = new XmlNamespaceManager(this.nameTable);
							this.xmlns = this.nameTable.Add("xmlns");
						}
						this.ProcessAppInfoDocMarkup(true);
					}
				}
				else if (!this.reader.IsEmptyElement)
				{
					this.markupDepth = this.reader.Depth;
					this.processMarkup = false;
				}
			}
			else if (this.reader.NodeType == XmlNodeType.Text)
			{
				if (!this.xmlCharType.IsOnlyWhitespace(this.reader.Value))
				{
					this.builder.ProcessCData(this.reader.Value);
				}
			}
			else if (this.reader.NodeType == XmlNodeType.EntityReference || this.reader.NodeType == XmlNodeType.SignificantWhitespace || this.reader.NodeType == XmlNodeType.CDATA)
			{
				this.builder.ProcessCData(this.reader.Value);
			}
			else if (this.reader.NodeType == XmlNodeType.EndElement)
			{
				if (this.reader.Depth == this.markupDepth)
				{
					if (this.processMarkup)
					{
						XmlNodeList childNodes = this.parentNode.ChildNodes;
						XmlNode[] array = new XmlNode[childNodes.Count];
						for (int i = 0; i < childNodes.Count; i++)
						{
							array[i] = childNodes[i];
						}
						this.builder.ProcessMarkup(array);
						this.namespaceManager.PopScope();
						this.builder.EndChildren();
					}
					this.markupDepth = int.MaxValue;
				}
				else
				{
					this.namespaceManager.PopScope();
					this.builder.EndChildren();
				}
				if (this.reader.Depth == this.schemaXmlDepth)
				{
					return false;
				}
			}
			return true;
		}

		private void ProcessAppInfoDocMarkup(bool root)
		{
			XmlNode xmlNode = null;
			switch (this.reader.NodeType)
			{
			case XmlNodeType.Element:
				this.annotationNSManager.PushScope();
				xmlNode = this.LoadElementNode(root);
				return;
			case XmlNodeType.Text:
				xmlNode = this.dummyDocument.CreateTextNode(this.reader.Value);
				break;
			case XmlNodeType.CDATA:
				xmlNode = this.dummyDocument.CreateCDataSection(this.reader.Value);
				break;
			case XmlNodeType.EntityReference:
				xmlNode = this.dummyDocument.CreateEntityReference(this.reader.Name);
				break;
			case XmlNodeType.ProcessingInstruction:
				xmlNode = this.dummyDocument.CreateProcessingInstruction(this.reader.Name, this.reader.Value);
				break;
			case XmlNodeType.Comment:
				xmlNode = this.dummyDocument.CreateComment(this.reader.Value);
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.EndEntity:
				return;
			case XmlNodeType.SignificantWhitespace:
				xmlNode = this.dummyDocument.CreateSignificantWhitespace(this.reader.Value);
				break;
			case XmlNodeType.EndElement:
				this.annotationNSManager.PopScope();
				this.parentNode = this.parentNode.ParentNode;
				return;
			}
			this.parentNode.AppendChild(xmlNode);
		}

		private XmlElement LoadElementNode(bool root)
		{
			XmlReader xmlReader = this.reader;
			bool isEmptyElement = xmlReader.IsEmptyElement;
			XmlElement xmlElement = this.dummyDocument.CreateElement(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI);
			xmlElement.IsEmpty = isEmptyElement;
			if (root)
			{
				this.parentNode = xmlElement;
			}
			else
			{
				XmlAttributeCollection attributes = xmlElement.Attributes;
				if (xmlReader.MoveToFirstAttribute())
				{
					do
					{
						if (Ref.Equal(xmlReader.NamespaceURI, this.schemaNames.NsXmlNs))
						{
							this.annotationNSManager.AddNamespace((xmlReader.Prefix.Length == 0) ? string.Empty : this.reader.LocalName, this.reader.Value);
						}
						XmlAttribute xmlAttribute = this.LoadAttributeNode();
						attributes.Append(xmlAttribute);
					}
					while (xmlReader.MoveToNextAttribute());
				}
				xmlReader.MoveToElement();
				string text = this.annotationNSManager.LookupNamespace(xmlReader.Prefix);
				if (text == null)
				{
					XmlAttribute xmlAttribute2 = this.CreateXmlNsAttribute(xmlReader.Prefix, this.namespaceManager.LookupNamespace(xmlReader.Prefix));
					attributes.Append(xmlAttribute2);
				}
				else if (text.Length == 0)
				{
					string text2 = this.namespaceManager.LookupNamespace(xmlReader.Prefix);
					if (text2 != string.Empty)
					{
						XmlAttribute xmlAttribute3 = this.CreateXmlNsAttribute(xmlReader.Prefix, text2);
						attributes.Append(xmlAttribute3);
					}
				}
				while (xmlReader.MoveToNextAttribute())
				{
					if (xmlReader.Prefix.Length != 0 && this.annotationNSManager.LookupNamespace(xmlReader.Prefix) == null)
					{
						XmlAttribute xmlAttribute4 = this.CreateXmlNsAttribute(xmlReader.Prefix, this.namespaceManager.LookupNamespace(xmlReader.Prefix));
						attributes.Append(xmlAttribute4);
					}
				}
				xmlReader.MoveToElement();
				this.parentNode.AppendChild(xmlElement);
				if (!xmlReader.IsEmptyElement)
				{
					this.parentNode = xmlElement;
				}
			}
			return xmlElement;
		}

		private XmlAttribute CreateXmlNsAttribute(string prefix, string value)
		{
			XmlAttribute xmlAttribute;
			if (prefix.Length == 0)
			{
				xmlAttribute = this.dummyDocument.CreateAttribute(string.Empty, this.xmlns, "http://www.w3.org/2000/xmlns/");
			}
			else
			{
				xmlAttribute = this.dummyDocument.CreateAttribute(this.xmlns, prefix, "http://www.w3.org/2000/xmlns/");
			}
			xmlAttribute.AppendChild(this.dummyDocument.CreateTextNode(value));
			this.annotationNSManager.AddNamespace(prefix, value);
			return xmlAttribute;
		}

		private XmlAttribute LoadAttributeNode()
		{
			XmlReader xmlReader = this.reader;
			XmlAttribute xmlAttribute = this.dummyDocument.CreateAttribute(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI);
			while (xmlReader.ReadAttributeValue())
			{
				XmlNodeType nodeType = xmlReader.NodeType;
				if (nodeType != XmlNodeType.Text)
				{
					if (nodeType != XmlNodeType.EntityReference)
					{
						throw XmlLoader.UnexpectedNodeType(xmlReader.NodeType);
					}
					xmlAttribute.AppendChild(this.LoadEntityReferenceInAttribute());
				}
				else
				{
					xmlAttribute.AppendChild(this.dummyDocument.CreateTextNode(xmlReader.Value));
				}
			}
			return xmlAttribute;
		}

		private XmlEntityReference LoadEntityReferenceInAttribute()
		{
			XmlEntityReference xmlEntityReference = this.dummyDocument.CreateEntityReference(this.reader.LocalName);
			if (!this.reader.CanResolveEntity)
			{
				return xmlEntityReference;
			}
			this.reader.ResolveEntity();
			while (this.reader.ReadAttributeValue())
			{
				XmlNodeType nodeType = this.reader.NodeType;
				if (nodeType != XmlNodeType.Text)
				{
					if (nodeType != XmlNodeType.EntityReference)
					{
						if (nodeType != XmlNodeType.EndEntity)
						{
							throw XmlLoader.UnexpectedNodeType(this.reader.NodeType);
						}
						if (xmlEntityReference.ChildNodes.Count == 0)
						{
							xmlEntityReference.AppendChild(this.dummyDocument.CreateTextNode(string.Empty));
						}
						return xmlEntityReference;
					}
					else
					{
						xmlEntityReference.AppendChild(this.LoadEntityReferenceInAttribute());
					}
				}
				else
				{
					xmlEntityReference.AppendChild(this.dummyDocument.CreateTextNode(this.reader.Value));
				}
			}
			return xmlEntityReference;
		}

		public async Task<SchemaType> ParseAsync(XmlReader reader, string targetNamespace)
		{
			await this.StartParsingAsync(reader, targetNamespace).ConfigureAwait(false);
			bool flag;
			do
			{
				flag = this.ParseReaderNode();
				if (flag)
				{
					flag = await reader.ReadAsync().ConfigureAwait(false);
				}
			}
			while (flag);
			return this.FinishParsing();
		}

		public async Task StartParsingAsync(XmlReader reader, string targetNamespace)
		{
			this.reader = reader;
			this.positionInfo = PositionInfo.GetPositionInfo(reader);
			this.namespaceManager = reader.NamespaceManager;
			if (this.namespaceManager == null)
			{
				this.namespaceManager = new XmlNamespaceManager(this.nameTable);
				this.isProcessNamespaces = true;
			}
			else
			{
				this.isProcessNamespaces = false;
			}
			bool flag;
			do
			{
				flag = reader.NodeType != XmlNodeType.Element;
				if (flag)
				{
					flag = await reader.ReadAsync().ConfigureAwait(false);
				}
			}
			while (flag);
			this.markupDepth = int.MaxValue;
			this.schemaXmlDepth = reader.Depth;
			SchemaType schemaType = this.schemaNames.SchemaTypeFromRoot(reader.LocalName, reader.NamespaceURI);
			string text;
			if (!this.CheckSchemaRoot(schemaType, out text))
			{
				throw new XmlSchemaException(text, reader.BaseURI, this.positionInfo.LineNumber, this.positionInfo.LinePosition);
			}
			if (this.schemaType == SchemaType.XSD)
			{
				this.schema = new XmlSchema();
				this.schema.BaseUri = new Uri(reader.BaseURI, UriKind.RelativeOrAbsolute);
				this.builder = new XsdBuilder(reader, this.namespaceManager, this.schema, this.nameTable, this.schemaNames, this.eventHandler);
			}
			else
			{
				this.xdrSchema = new SchemaInfo();
				this.xdrSchema.SchemaType = SchemaType.XDR;
				this.builder = new XdrBuilder(reader, this.namespaceManager, this.xdrSchema, targetNamespace, this.nameTable, this.schemaNames, this.eventHandler);
				((XdrBuilder)this.builder).XmlResolver = this.xmlResolver;
			}
		}

		private SchemaType schemaType;

		private XmlNameTable nameTable;

		private SchemaNames schemaNames;

		private ValidationEventHandler eventHandler;

		private XmlNamespaceManager namespaceManager;

		private XmlReader reader;

		private PositionInfo positionInfo;

		private bool isProcessNamespaces;

		private int schemaXmlDepth;

		private int markupDepth;

		private SchemaBuilder builder;

		private XmlSchema schema;

		private SchemaInfo xdrSchema;

		private XmlResolver xmlResolver;

		private XmlDocument dummyDocument;

		private bool processMarkup;

		private XmlNode parentNode;

		private XmlNamespaceManager annotationNSManager;

		private string xmlns;

		private XmlCharType xmlCharType = XmlCharType.Instance;
	}
}
