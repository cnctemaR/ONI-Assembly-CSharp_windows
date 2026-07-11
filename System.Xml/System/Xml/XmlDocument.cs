using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlDocument : XmlNode
	{
		public XmlDocument()
			: this(new XmlImplementation())
		{
		}

		public XmlDocument(XmlNameTable nt)
			: this(new XmlImplementation(nt))
		{
		}

		protected internal XmlDocument(XmlImplementation imp)
		{
			this.implementation = imp;
			this.domNameTable = new DomNameTable(this);
			XmlNameTable nameTable = this.NameTable;
			nameTable.Add(string.Empty);
			this.strDocumentName = nameTable.Add("#document");
			this.strDocumentFragmentName = nameTable.Add("#document-fragment");
			this.strCommentName = nameTable.Add("#comment");
			this.strTextName = nameTable.Add("#text");
			this.strCDataSectionName = nameTable.Add("#cdata-section");
			this.strEntityName = nameTable.Add("#entity");
			this.strID = nameTable.Add("id");
			this.strNonSignificantWhitespaceName = nameTable.Add("#whitespace");
			this.strSignificantWhitespaceName = nameTable.Add("#significant-whitespace");
			this.strXmlns = nameTable.Add("xmlns");
			this.strXml = nameTable.Add("xml");
			this.strSpace = nameTable.Add("space");
			this.strLang = nameTable.Add("lang");
			this.strReservedXmlns = nameTable.Add("http://www.w3.org/2000/xmlns/");
			this.strReservedXml = nameTable.Add("http://www.w3.org/XML/1998/namespace");
			this.strEmpty = nameTable.Add(string.Empty);
			this.baseURI = string.Empty;
			this.objLock = new object();
		}

		internal SchemaInfo DtdSchemaInfo
		{
			get
			{
				return this.schemaInfo;
			}
			set
			{
				this.schemaInfo = value;
			}
		}

		internal static void CheckName(string name)
		{
			int num = ValidateNames.ParseNmtoken(name, 0);
			if (num < name.Length)
			{
				throw new XmlException("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(name, num));
			}
		}

		internal XmlName AddXmlName(string prefix, string localName, string namespaceURI, IXmlSchemaInfo schemaInfo)
		{
			return this.domNameTable.AddName(prefix, localName, namespaceURI, schemaInfo);
		}

		internal XmlName GetXmlName(string prefix, string localName, string namespaceURI, IXmlSchemaInfo schemaInfo)
		{
			return this.domNameTable.GetName(prefix, localName, namespaceURI, schemaInfo);
		}

		internal XmlName AddAttrXmlName(string prefix, string localName, string namespaceURI, IXmlSchemaInfo schemaInfo)
		{
			XmlName xmlName = this.AddXmlName(prefix, localName, namespaceURI, schemaInfo);
			if (!this.IsLoading)
			{
				object prefix2 = xmlName.Prefix;
				object namespaceURI2 = xmlName.NamespaceURI;
				object localName2 = xmlName.LocalName;
				if ((prefix2 == this.strXmlns || (prefix2 == this.strEmpty && localName2 == this.strXmlns)) ^ (namespaceURI2 == this.strReservedXmlns))
				{
					throw new ArgumentException(Res.GetString("The namespace declaration attribute has an incorrect 'namespaceURI': '{0}'.", new object[] { namespaceURI }));
				}
			}
			return xmlName;
		}

		internal bool AddIdInfo(XmlName eleName, XmlName attrName)
		{
			if (this.htElementIDAttrDecl == null || this.htElementIDAttrDecl[eleName] == null)
			{
				if (this.htElementIDAttrDecl == null)
				{
					this.htElementIDAttrDecl = new Hashtable();
				}
				this.htElementIDAttrDecl.Add(eleName, attrName);
				return true;
			}
			return false;
		}

		private XmlName GetIDInfoByElement_(XmlName eleName)
		{
			XmlName xmlName = this.GetXmlName(eleName.Prefix, eleName.LocalName, string.Empty, null);
			if (xmlName != null)
			{
				return (XmlName)this.htElementIDAttrDecl[xmlName];
			}
			return null;
		}

		internal XmlName GetIDInfoByElement(XmlName eleName)
		{
			if (this.htElementIDAttrDecl == null)
			{
				return null;
			}
			return this.GetIDInfoByElement_(eleName);
		}

		private WeakReference GetElement(ArrayList elementList, XmlElement elem)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object obj in elementList)
			{
				WeakReference weakReference = (WeakReference)obj;
				if (!weakReference.IsAlive)
				{
					arrayList.Add(weakReference);
				}
				else if ((XmlElement)weakReference.Target == elem)
				{
					return weakReference;
				}
			}
			foreach (object obj2 in arrayList)
			{
				WeakReference weakReference2 = (WeakReference)obj2;
				elementList.Remove(weakReference2);
			}
			return null;
		}

		internal void AddElementWithId(string id, XmlElement elem)
		{
			if (this.htElementIdMap == null || !this.htElementIdMap.Contains(id))
			{
				if (this.htElementIdMap == null)
				{
					this.htElementIdMap = new Hashtable();
				}
				ArrayList arrayList = new ArrayList();
				arrayList.Add(new WeakReference(elem));
				this.htElementIdMap.Add(id, arrayList);
				return;
			}
			ArrayList arrayList2 = (ArrayList)this.htElementIdMap[id];
			if (this.GetElement(arrayList2, elem) == null)
			{
				arrayList2.Add(new WeakReference(elem));
			}
		}

		internal void RemoveElementWithId(string id, XmlElement elem)
		{
			if (this.htElementIdMap != null && this.htElementIdMap.Contains(id))
			{
				ArrayList arrayList = (ArrayList)this.htElementIdMap[id];
				WeakReference element = this.GetElement(arrayList, elem);
				if (element != null)
				{
					arrayList.Remove(element);
					if (arrayList.Count == 0)
					{
						this.htElementIdMap.Remove(id);
					}
				}
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument xmlDocument = this.Implementation.CreateDocument();
			xmlDocument.SetBaseURI(this.baseURI);
			if (deep)
			{
				xmlDocument.ImportChildren(this, xmlDocument, deep);
			}
			return xmlDocument;
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Document;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		public virtual XmlDocumentType DocumentType
		{
			get
			{
				return (XmlDocumentType)this.FindChild(XmlNodeType.DocumentType);
			}
		}

		internal virtual XmlDeclaration Declaration
		{
			get
			{
				if (this.HasChildNodes)
				{
					return this.FirstChild as XmlDeclaration;
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

		public override string Name
		{
			get
			{
				return this.strDocumentName;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.strDocumentName;
			}
		}

		public XmlElement DocumentElement
		{
			get
			{
				return (XmlElement)this.FindChild(XmlNodeType.Element);
			}
		}

		internal override bool IsContainer
		{
			get
			{
				return true;
			}
		}

		internal override XmlLinkedNode LastNode
		{
			get
			{
				return this.lastChild;
			}
			set
			{
				this.lastChild = value;
			}
		}

		public override XmlDocument OwnerDocument
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
					this.schemas = new XmlSchemaSet(this.NameTable);
				}
				return this.schemas;
			}
			set
			{
				this.schemas = value;
			}
		}

		internal bool CanReportValidity
		{
			get
			{
				return this.reportValidity;
			}
		}

		internal bool HasSetResolver
		{
			get
			{
				return this.bSetResolver;
			}
		}

		internal XmlResolver GetResolver()
		{
			return this.resolver;
		}

		public virtual XmlResolver XmlResolver
		{
			set
			{
				if (value != null)
				{
					try
					{
						new NamedPermissionSet("FullTrust").Demand();
					}
					catch (SecurityException ex)
					{
						throw new SecurityException(Res.GetString("XmlResolver can be set only by fully trusted code."), ex);
					}
				}
				this.resolver = value;
				if (!this.bSetResolver)
				{
					this.bSetResolver = true;
				}
				XmlDocumentType documentType = this.DocumentType;
				if (documentType != null)
				{
					documentType.DtdSchemaInfo = null;
				}
			}
		}

		internal override bool IsValidChildType(XmlNodeType type)
		{
			if (type != XmlNodeType.Element)
			{
				switch (type)
				{
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					return true;
				case XmlNodeType.DocumentType:
					if (this.DocumentType != null)
					{
						throw new InvalidOperationException(Res.GetString("This document already has a 'DocumentType' node."));
					}
					return true;
				case XmlNodeType.XmlDeclaration:
					if (this.Declaration != null)
					{
						throw new InvalidOperationException(Res.GetString("This document already has an 'XmlDeclaration' node."));
					}
					return true;
				}
				return false;
			}
			if (this.DocumentElement != null)
			{
				throw new InvalidOperationException(Res.GetString("This document already has a 'DocumentElement' node."));
			}
			return true;
		}

		private bool HasNodeTypeInPrevSiblings(XmlNodeType nt, XmlNode refNode)
		{
			if (refNode == null)
			{
				return false;
			}
			XmlNode xmlNode = null;
			if (refNode.ParentNode != null)
			{
				xmlNode = refNode.ParentNode.FirstChild;
			}
			while (xmlNode != null)
			{
				if (xmlNode.NodeType == nt)
				{
					return true;
				}
				if (xmlNode == refNode)
				{
					break;
				}
				xmlNode = xmlNode.NextSibling;
			}
			return false;
		}

		private bool HasNodeTypeInNextSiblings(XmlNodeType nt, XmlNode refNode)
		{
			for (XmlNode xmlNode = refNode; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == nt)
				{
					return true;
				}
			}
			return false;
		}

		internal override bool CanInsertBefore(XmlNode newChild, XmlNode refChild)
		{
			if (refChild == null)
			{
				refChild = this.FirstChild;
			}
			if (refChild == null)
			{
				return true;
			}
			XmlNodeType nodeType = newChild.NodeType;
			if (nodeType <= XmlNodeType.Comment)
			{
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType - XmlNodeType.ProcessingInstruction <= 1)
					{
						return refChild.NodeType != XmlNodeType.XmlDeclaration;
					}
				}
				else if (refChild.NodeType != XmlNodeType.XmlDeclaration)
				{
					return !this.HasNodeTypeInNextSiblings(XmlNodeType.DocumentType, refChild);
				}
			}
			else if (nodeType != XmlNodeType.DocumentType)
			{
				if (nodeType == XmlNodeType.XmlDeclaration)
				{
					return refChild == this.FirstChild;
				}
			}
			else if (refChild.NodeType != XmlNodeType.XmlDeclaration)
			{
				return !this.HasNodeTypeInPrevSiblings(XmlNodeType.Element, refChild.PreviousSibling);
			}
			return false;
		}

		internal override bool CanInsertAfter(XmlNode newChild, XmlNode refChild)
		{
			if (refChild == null)
			{
				refChild = this.LastChild;
			}
			if (refChild == null)
			{
				return true;
			}
			XmlNodeType nodeType = newChild.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				switch (nodeType)
				{
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					return true;
				case XmlNodeType.DocumentType:
					return !this.HasNodeTypeInPrevSiblings(XmlNodeType.Element, refChild);
				}
				return false;
			}
			return !this.HasNodeTypeInNextSiblings(XmlNodeType.DocumentType, refChild.NextSibling);
		}

		public XmlAttribute CreateAttribute(string name)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			XmlNode.SplitName(name, out empty, out empty2);
			this.SetDefaultNamespace(empty, empty2, ref empty3);
			return this.CreateAttribute(empty, empty2, empty3);
		}

		internal void SetDefaultNamespace(string prefix, string localName, ref string namespaceURI)
		{
			if (prefix == this.strXmlns || (prefix.Length == 0 && localName == this.strXmlns))
			{
				namespaceURI = this.strReservedXmlns;
				return;
			}
			if (prefix == this.strXml)
			{
				namespaceURI = this.strReservedXml;
			}
		}

		public virtual XmlCDataSection CreateCDataSection(string data)
		{
			this.fCDataNodesPresent = true;
			return new XmlCDataSection(data, this);
		}

		public virtual XmlComment CreateComment(string data)
		{
			return new XmlComment(data, this);
		}

		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		public virtual XmlDocumentType CreateDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XmlDocumentType(name, publicId, systemId, internalSubset, this);
		}

		public virtual XmlDocumentFragment CreateDocumentFragment()
		{
			return new XmlDocumentFragment(this);
		}

		public XmlElement CreateElement(string name)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			XmlNode.SplitName(name, out empty, out empty2);
			return this.CreateElement(empty, empty2, string.Empty);
		}

		internal void AddDefaultAttributes(XmlElement elem)
		{
			SchemaInfo dtdSchemaInfo = this.DtdSchemaInfo;
			SchemaElementDecl schemaElementDecl = this.GetSchemaElementDecl(elem);
			if (schemaElementDecl != null && schemaElementDecl.AttDefs != null)
			{
				IDictionaryEnumerator dictionaryEnumerator = schemaElementDecl.AttDefs.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					SchemaAttDef schemaAttDef = (SchemaAttDef)dictionaryEnumerator.Value;
					if (schemaAttDef.Presence == SchemaDeclBase.Use.Default || schemaAttDef.Presence == SchemaDeclBase.Use.Fixed)
					{
						string text = string.Empty;
						string name = schemaAttDef.Name.Name;
						string text2 = string.Empty;
						if (dtdSchemaInfo.SchemaType == SchemaType.DTD)
						{
							text = schemaAttDef.Name.Namespace;
						}
						else
						{
							text = schemaAttDef.Prefix;
							text2 = schemaAttDef.Name.Namespace;
						}
						XmlAttribute xmlAttribute = this.PrepareDefaultAttribute(schemaAttDef, text, name, text2);
						elem.SetAttributeNode(xmlAttribute);
					}
				}
			}
		}

		private SchemaElementDecl GetSchemaElementDecl(XmlElement elem)
		{
			SchemaInfo dtdSchemaInfo = this.DtdSchemaInfo;
			if (dtdSchemaInfo != null)
			{
				XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(elem.LocalName, (dtdSchemaInfo.SchemaType == SchemaType.DTD) ? elem.Prefix : elem.NamespaceURI);
				SchemaElementDecl schemaElementDecl;
				if (dtdSchemaInfo.ElementDecls.TryGetValue(xmlQualifiedName, out schemaElementDecl))
				{
					return schemaElementDecl;
				}
			}
			return null;
		}

		private XmlAttribute PrepareDefaultAttribute(SchemaAttDef attdef, string attrPrefix, string attrLocalname, string attrNamespaceURI)
		{
			this.SetDefaultNamespace(attrPrefix, attrLocalname, ref attrNamespaceURI);
			XmlAttribute xmlAttribute = this.CreateDefaultAttribute(attrPrefix, attrLocalname, attrNamespaceURI);
			xmlAttribute.InnerXml = attdef.DefaultValueRaw;
			XmlUnspecifiedAttribute xmlUnspecifiedAttribute = xmlAttribute as XmlUnspecifiedAttribute;
			if (xmlUnspecifiedAttribute != null)
			{
				xmlUnspecifiedAttribute.SetSpecified(false);
			}
			return xmlAttribute;
		}

		public virtual XmlEntityReference CreateEntityReference(string name)
		{
			return new XmlEntityReference(name, this);
		}

		public virtual XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
		{
			return new XmlProcessingInstruction(target, data, this);
		}

		public virtual XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new XmlDeclaration(version, encoding, standalone, this);
		}

		public virtual XmlText CreateTextNode(string text)
		{
			return new XmlText(text, this);
		}

		public virtual XmlSignificantWhitespace CreateSignificantWhitespace(string text)
		{
			return new XmlSignificantWhitespace(text, this);
		}

		public override XPathNavigator CreateNavigator()
		{
			return this.CreateNavigator(this);
		}

		protected internal virtual XPathNavigator CreateNavigator(XmlNode node)
		{
			switch (node.NodeType)
			{
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.SignificantWhitespace:
			{
				XmlNode xmlNode = node.ParentNode;
				if (xmlNode != null)
				{
					for (;;)
					{
						XmlNodeType xmlNodeType = xmlNode.NodeType;
						if (xmlNodeType == XmlNodeType.Attribute)
						{
							break;
						}
						if (xmlNodeType != XmlNodeType.EntityReference)
						{
							goto IL_0074;
						}
						xmlNode = xmlNode.ParentNode;
						if (xmlNode == null)
						{
							goto IL_0074;
						}
					}
					return null;
				}
				IL_0074:
				node = this.NormalizeText(node);
				break;
			}
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.DocumentType:
			case XmlNodeType.Notation:
			case XmlNodeType.XmlDeclaration:
				return null;
			case XmlNodeType.Whitespace:
			{
				XmlNode xmlNode = node.ParentNode;
				if (xmlNode != null)
				{
					for (;;)
					{
						XmlNodeType xmlNodeType = xmlNode.NodeType;
						if (xmlNodeType == XmlNodeType.Document || xmlNodeType == XmlNodeType.Attribute)
						{
							break;
						}
						if (xmlNodeType != XmlNodeType.EntityReference)
						{
							goto IL_00A9;
						}
						xmlNode = xmlNode.ParentNode;
						if (xmlNode == null)
						{
							goto IL_00A9;
						}
					}
					return null;
				}
				IL_00A9:
				node = this.NormalizeText(node);
				break;
			}
			}
			return new DocumentXPathNavigator(this, node);
		}

		internal static bool IsTextNode(XmlNodeType nt)
		{
			return nt - XmlNodeType.Text <= 1 || nt - XmlNodeType.Whitespace <= 1;
		}

		private XmlNode NormalizeText(XmlNode n)
		{
			XmlNode xmlNode = null;
			while (XmlDocument.IsTextNode(n.NodeType))
			{
				xmlNode = n;
				n = n.PreviousSibling;
				if (n == null)
				{
					XmlNode xmlNode2 = xmlNode;
					while (xmlNode2.ParentNode != null && xmlNode2.ParentNode.NodeType == XmlNodeType.EntityReference)
					{
						if (xmlNode2.ParentNode.PreviousSibling != null)
						{
							n = xmlNode2.ParentNode.PreviousSibling;
							break;
						}
						xmlNode2 = xmlNode2.ParentNode;
						if (xmlNode2 == null)
						{
							break;
						}
					}
				}
				if (n == null)
				{
					break;
				}
				while (n.NodeType == XmlNodeType.EntityReference)
				{
					n = n.LastChild;
				}
			}
			return xmlNode;
		}

		public virtual XmlWhitespace CreateWhitespace(string text)
		{
			return new XmlWhitespace(text, this);
		}

		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			return new XmlElementList(this, name);
		}

		public XmlAttribute CreateAttribute(string qualifiedName, string namespaceURI)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			XmlNode.SplitName(qualifiedName, out empty, out empty2);
			return this.CreateAttribute(empty, empty2, namespaceURI);
		}

		public XmlElement CreateElement(string qualifiedName, string namespaceURI)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			XmlNode.SplitName(qualifiedName, out empty, out empty2);
			return this.CreateElement(empty, empty2, namespaceURI);
		}

		public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
		{
			return new XmlElementList(this, localName, namespaceURI);
		}

		public virtual XmlElement GetElementById(string elementId)
		{
			if (this.htElementIdMap != null)
			{
				ArrayList arrayList = (ArrayList)this.htElementIdMap[elementId];
				if (arrayList != null)
				{
					foreach (object obj in arrayList)
					{
						XmlElement xmlElement = (XmlElement)((WeakReference)obj).Target;
						if (xmlElement != null && xmlElement.IsConnected())
						{
							return xmlElement;
						}
					}
				}
			}
			return null;
		}

		public virtual XmlNode ImportNode(XmlNode node, bool deep)
		{
			return this.ImportNodeInternal(node, deep);
		}

		private XmlNode ImportNodeInternal(XmlNode node, bool deep)
		{
			if (node == null)
			{
				throw new InvalidOperationException(Res.GetString("Cannot import a null node."));
			}
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
			{
				XmlNode xmlNode = this.CreateElement(node.Prefix, node.LocalName, node.NamespaceURI);
				this.ImportAttributes(node, xmlNode);
				if (deep)
				{
					this.ImportChildren(node, xmlNode, deep);
					return xmlNode;
				}
				return xmlNode;
			}
			case XmlNodeType.Attribute:
			{
				XmlNode xmlNode = this.CreateAttribute(node.Prefix, node.LocalName, node.NamespaceURI);
				this.ImportChildren(node, xmlNode, true);
				return xmlNode;
			}
			case XmlNodeType.Text:
				return this.CreateTextNode(node.Value);
			case XmlNodeType.CDATA:
				return this.CreateCDataSection(node.Value);
			case XmlNodeType.EntityReference:
				return this.CreateEntityReference(node.Name);
			case XmlNodeType.ProcessingInstruction:
				return this.CreateProcessingInstruction(node.Name, node.Value);
			case XmlNodeType.Comment:
				return this.CreateComment(node.Value);
			case XmlNodeType.DocumentType:
			{
				XmlDocumentType xmlDocumentType = (XmlDocumentType)node;
				return this.CreateDocumentType(xmlDocumentType.Name, xmlDocumentType.PublicId, xmlDocumentType.SystemId, xmlDocumentType.InternalSubset);
			}
			case XmlNodeType.DocumentFragment:
			{
				XmlNode xmlNode = this.CreateDocumentFragment();
				if (deep)
				{
					this.ImportChildren(node, xmlNode, deep);
					return xmlNode;
				}
				return xmlNode;
			}
			case XmlNodeType.Whitespace:
				return this.CreateWhitespace(node.Value);
			case XmlNodeType.SignificantWhitespace:
				return this.CreateSignificantWhitespace(node.Value);
			case XmlNodeType.XmlDeclaration:
			{
				XmlDeclaration xmlDeclaration = (XmlDeclaration)node;
				return this.CreateXmlDeclaration(xmlDeclaration.Version, xmlDeclaration.Encoding, xmlDeclaration.Standalone);
			}
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Res.GetString("Cannot import nodes of type '{0}'."), node.NodeType.ToString()));
		}

		private void ImportAttributes(XmlNode fromElem, XmlNode toElem)
		{
			int count = fromElem.Attributes.Count;
			for (int i = 0; i < count; i++)
			{
				if (fromElem.Attributes[i].Specified)
				{
					toElem.Attributes.SetNamedItem(this.ImportNodeInternal(fromElem.Attributes[i], true));
				}
			}
		}

		private void ImportChildren(XmlNode fromNode, XmlNode toNode, bool deep)
		{
			for (XmlNode xmlNode = fromNode.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				toNode.AppendChild(this.ImportNodeInternal(xmlNode, deep));
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.implementation.NameTable;
			}
		}

		public virtual XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return new XmlAttribute(this.AddAttrXmlName(prefix, localName, namespaceURI, null), this);
		}

		protected internal virtual XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
		{
			return new XmlUnspecifiedAttribute(prefix, localName, namespaceURI, this);
		}

		public virtual XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			XmlElement xmlElement = new XmlElement(this.AddXmlName(prefix, localName, namespaceURI, null), true, this);
			if (!this.IsLoading)
			{
				this.AddDefaultAttributes(xmlElement);
			}
			return xmlElement;
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

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal XmlNamedNodeMap Entities
		{
			get
			{
				if (this.entities == null)
				{
					this.entities = new XmlNamedNodeMap(this);
				}
				return this.entities;
			}
			set
			{
				this.entities = value;
			}
		}

		internal bool IsLoading
		{
			get
			{
				return this.isLoading;
			}
			set
			{
				this.isLoading = value;
			}
		}

		internal bool ActualLoadingStatus
		{
			get
			{
				return this.actualLoadingStatus;
			}
			set
			{
				this.actualLoadingStatus = value;
			}
		}

		public virtual XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
		{
			switch (type)
			{
			case XmlNodeType.Element:
				if (prefix != null)
				{
					return this.CreateElement(prefix, name, namespaceURI);
				}
				return this.CreateElement(name, namespaceURI);
			case XmlNodeType.Attribute:
				if (prefix != null)
				{
					return this.CreateAttribute(prefix, name, namespaceURI);
				}
				return this.CreateAttribute(name, namespaceURI);
			case XmlNodeType.Text:
				return this.CreateTextNode(string.Empty);
			case XmlNodeType.CDATA:
				return this.CreateCDataSection(string.Empty);
			case XmlNodeType.EntityReference:
				return this.CreateEntityReference(name);
			case XmlNodeType.ProcessingInstruction:
				return this.CreateProcessingInstruction(name, string.Empty);
			case XmlNodeType.Comment:
				return this.CreateComment(string.Empty);
			case XmlNodeType.Document:
				return new XmlDocument();
			case XmlNodeType.DocumentType:
				return this.CreateDocumentType(name, string.Empty, string.Empty, string.Empty);
			case XmlNodeType.DocumentFragment:
				return this.CreateDocumentFragment();
			case XmlNodeType.Whitespace:
				return this.CreateWhitespace(string.Empty);
			case XmlNodeType.SignificantWhitespace:
				return this.CreateSignificantWhitespace(string.Empty);
			case XmlNodeType.XmlDeclaration:
				return this.CreateXmlDeclaration("1.0", null, null);
			}
			throw new ArgumentException(Res.GetString("Cannot create node of type {0}.", new object[] { type }));
		}

		public virtual XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
		{
			return this.CreateNode(this.ConvertToNodeType(nodeTypeString), name, namespaceURI);
		}

		public virtual XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
		{
			return this.CreateNode(type, null, name, namespaceURI);
		}

		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		public virtual XmlNode ReadNode(XmlReader reader)
		{
			XmlNode xmlNode = null;
			try
			{
				this.IsLoading = true;
				xmlNode = new XmlLoader().ReadCurrentNode(this, reader);
			}
			finally
			{
				this.IsLoading = false;
			}
			return xmlNode;
		}

		internal XmlNodeType ConvertToNodeType(string nodeTypeString)
		{
			if (nodeTypeString == "element")
			{
				return XmlNodeType.Element;
			}
			if (nodeTypeString == "attribute")
			{
				return XmlNodeType.Attribute;
			}
			if (nodeTypeString == "text")
			{
				return XmlNodeType.Text;
			}
			if (nodeTypeString == "cdatasection")
			{
				return XmlNodeType.CDATA;
			}
			if (nodeTypeString == "entityreference")
			{
				return XmlNodeType.EntityReference;
			}
			if (nodeTypeString == "entity")
			{
				return XmlNodeType.Entity;
			}
			if (nodeTypeString == "processinginstruction")
			{
				return XmlNodeType.ProcessingInstruction;
			}
			if (nodeTypeString == "comment")
			{
				return XmlNodeType.Comment;
			}
			if (nodeTypeString == "document")
			{
				return XmlNodeType.Document;
			}
			if (nodeTypeString == "documenttype")
			{
				return XmlNodeType.DocumentType;
			}
			if (nodeTypeString == "documentfragment")
			{
				return XmlNodeType.DocumentFragment;
			}
			if (nodeTypeString == "notation")
			{
				return XmlNodeType.Notation;
			}
			if (nodeTypeString == "significantwhitespace")
			{
				return XmlNodeType.SignificantWhitespace;
			}
			if (nodeTypeString == "whitespace")
			{
				return XmlNodeType.Whitespace;
			}
			throw new ArgumentException(Res.GetString("'{0}' does not represent any 'XmlNodeType'.", new object[] { nodeTypeString }));
		}

		private XmlTextReader SetupReader(XmlTextReader tr)
		{
			tr.XmlValidatingReaderCompatibilityMode = true;
			tr.EntityHandling = EntityHandling.ExpandCharEntities;
			if (this.HasSetResolver)
			{
				tr.XmlResolver = this.GetResolver();
			}
			return tr;
		}

		public virtual void Load(string filename)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(filename, this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		public virtual void Load(Stream inStream)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(inStream, this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Impl.Close(false);
			}
		}

		public virtual void Load(TextReader txtReader)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(txtReader, this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Impl.Close(false);
			}
		}

		public virtual void Load(XmlReader reader)
		{
			try
			{
				this.IsLoading = true;
				this.actualLoadingStatus = true;
				this.RemoveAll();
				this.fEntRefNodesPresent = false;
				this.fCDataNodesPresent = false;
				this.reportValidity = true;
				new XmlLoader().Load(this, reader, this.preserveWhitespace);
			}
			finally
			{
				this.IsLoading = false;
				this.actualLoadingStatus = false;
				this.reportValidity = true;
			}
		}

		public virtual void LoadXml(string xml)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(new StringReader(xml), this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		internal Encoding TextEncoding
		{
			get
			{
				if (this.Declaration != null)
				{
					string encoding = this.Declaration.Encoding;
					if (encoding.Length > 0)
					{
						return global::System.Text.Encoding.GetEncoding(encoding);
					}
				}
				return null;
			}
		}

		public override string InnerText
		{
			set
			{
				throw new InvalidOperationException(Res.GetString("The 'InnerText' of a 'Document' node is read-only and cannot be set."));
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

		public virtual void Save(string filename)
		{
			if (this.DocumentElement == null)
			{
				throw new XmlException("Invalid XML document. {0}", Res.GetString("The document does not have a root element."));
			}
			XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(filename, this.TextEncoding);
			try
			{
				if (!this.preserveWhitespace)
				{
					xmlDOMTextWriter.Formatting = Formatting.Indented;
				}
				this.WriteTo(xmlDOMTextWriter);
				xmlDOMTextWriter.Flush();
			}
			finally
			{
				xmlDOMTextWriter.Close();
			}
		}

		public virtual void Save(Stream outStream)
		{
			XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(outStream, this.TextEncoding);
			if (!this.preserveWhitespace)
			{
				xmlDOMTextWriter.Formatting = Formatting.Indented;
			}
			this.WriteTo(xmlDOMTextWriter);
			xmlDOMTextWriter.Flush();
		}

		public virtual void Save(TextWriter writer)
		{
			XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(writer);
			if (!this.preserveWhitespace)
			{
				xmlDOMTextWriter.Formatting = Formatting.Indented;
			}
			this.Save(xmlDOMTextWriter);
		}

		public virtual void Save(XmlWriter w)
		{
			XmlNode xmlNode = this.FirstChild;
			if (xmlNode == null)
			{
				return;
			}
			if (w.WriteState == WriteState.Start)
			{
				if (xmlNode is XmlDeclaration)
				{
					if (this.Standalone.Length == 0)
					{
						w.WriteStartDocument();
					}
					else if (this.Standalone == "yes")
					{
						w.WriteStartDocument(true);
					}
					else if (this.Standalone == "no")
					{
						w.WriteStartDocument(false);
					}
					xmlNode = xmlNode.NextSibling;
				}
				else
				{
					w.WriteStartDocument();
				}
			}
			while (xmlNode != null)
			{
				xmlNode.WriteTo(w);
				xmlNode = xmlNode.NextSibling;
			}
			w.Flush();
		}

		public override void WriteTo(XmlWriter w)
		{
			this.WriteContentTo(w);
		}

		public override void WriteContentTo(XmlWriter xw)
		{
			foreach (object obj in this)
			{
				((XmlNode)obj).WriteTo(xw);
			}
		}

		public void Validate(ValidationEventHandler validationEventHandler)
		{
			this.Validate(validationEventHandler, this);
		}

		public void Validate(ValidationEventHandler validationEventHandler, XmlNode nodeToValidate)
		{
			if (this.schemas == null || this.schemas.Count == 0)
			{
				throw new InvalidOperationException(Res.GetString("The XmlSchemaSet on the document is either null or has no schemas in it. Provide schema information before calling Validate."));
			}
			if (nodeToValidate.Document != this)
			{
				throw new ArgumentException(Res.GetString("Cannot validate '{0}' because its owner document is not the current document.", new object[] { "nodeToValidate" }));
			}
			if (nodeToValidate == this)
			{
				this.reportValidity = false;
			}
			new DocumentSchemaValidator(this, this.schemas, validationEventHandler).Validate(nodeToValidate);
			if (nodeToValidate == this)
			{
				this.reportValidity = true;
			}
		}

		public event XmlNodeChangedEventHandler NodeInserting
		{
			add
			{
				this.onNodeInsertingDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeInsertingDelegate, value);
			}
			remove
			{
				this.onNodeInsertingDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeInsertingDelegate, value);
			}
		}

		public event XmlNodeChangedEventHandler NodeInserted
		{
			add
			{
				this.onNodeInsertedDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeInsertedDelegate, value);
			}
			remove
			{
				this.onNodeInsertedDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeInsertedDelegate, value);
			}
		}

		public event XmlNodeChangedEventHandler NodeRemoving
		{
			add
			{
				this.onNodeRemovingDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeRemovingDelegate, value);
			}
			remove
			{
				this.onNodeRemovingDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeRemovingDelegate, value);
			}
		}

		public event XmlNodeChangedEventHandler NodeRemoved
		{
			add
			{
				this.onNodeRemovedDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeRemovedDelegate, value);
			}
			remove
			{
				this.onNodeRemovedDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeRemovedDelegate, value);
			}
		}

		public event XmlNodeChangedEventHandler NodeChanging
		{
			add
			{
				this.onNodeChangingDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeChangingDelegate, value);
			}
			remove
			{
				this.onNodeChangingDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeChangingDelegate, value);
			}
		}

		public event XmlNodeChangedEventHandler NodeChanged
		{
			add
			{
				this.onNodeChangedDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeChangedDelegate, value);
			}
			remove
			{
				this.onNodeChangedDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeChangedDelegate, value);
			}
		}

		internal override XmlNodeChangedEventArgs GetEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
		{
			this.reportValidity = false;
			switch (action)
			{
			case XmlNodeChangedAction.Insert:
				if (this.onNodeInsertingDelegate == null && this.onNodeInsertedDelegate == null)
				{
					return null;
				}
				break;
			case XmlNodeChangedAction.Remove:
				if (this.onNodeRemovingDelegate == null && this.onNodeRemovedDelegate == null)
				{
					return null;
				}
				break;
			case XmlNodeChangedAction.Change:
				if (this.onNodeChangingDelegate == null && this.onNodeChangedDelegate == null)
				{
					return null;
				}
				break;
			}
			return new XmlNodeChangedEventArgs(node, oldParent, newParent, oldValue, newValue, action);
		}

		internal XmlNodeChangedEventArgs GetInsertEventArgsForLoad(XmlNode node, XmlNode newParent)
		{
			if (this.onNodeInsertingDelegate == null && this.onNodeInsertedDelegate == null)
			{
				return null;
			}
			string value = node.Value;
			return new XmlNodeChangedEventArgs(node, null, newParent, value, value, XmlNodeChangedAction.Insert);
		}

		internal override void BeforeEvent(XmlNodeChangedEventArgs args)
		{
			if (args != null)
			{
				switch (args.Action)
				{
				case XmlNodeChangedAction.Insert:
					if (this.onNodeInsertingDelegate != null)
					{
						this.onNodeInsertingDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Remove:
					if (this.onNodeRemovingDelegate != null)
					{
						this.onNodeRemovingDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Change:
					if (this.onNodeChangingDelegate != null)
					{
						this.onNodeChangingDelegate(this, args);
					}
					break;
				default:
					return;
				}
			}
		}

		internal override void AfterEvent(XmlNodeChangedEventArgs args)
		{
			if (args != null)
			{
				switch (args.Action)
				{
				case XmlNodeChangedAction.Insert:
					if (this.onNodeInsertedDelegate != null)
					{
						this.onNodeInsertedDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Remove:
					if (this.onNodeRemovedDelegate != null)
					{
						this.onNodeRemovedDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Change:
					if (this.onNodeChangedDelegate != null)
					{
						this.onNodeChangedDelegate(this, args);
					}
					break;
				default:
					return;
				}
			}
		}

		internal XmlAttribute GetDefaultAttribute(XmlElement elem, string attrPrefix, string attrLocalname, string attrNamespaceURI)
		{
			SchemaInfo dtdSchemaInfo = this.DtdSchemaInfo;
			SchemaElementDecl schemaElementDecl = this.GetSchemaElementDecl(elem);
			if (schemaElementDecl != null && schemaElementDecl.AttDefs != null)
			{
				IDictionaryEnumerator dictionaryEnumerator = schemaElementDecl.AttDefs.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					SchemaAttDef schemaAttDef = (SchemaAttDef)dictionaryEnumerator.Value;
					if ((schemaAttDef.Presence == SchemaDeclBase.Use.Default || schemaAttDef.Presence == SchemaDeclBase.Use.Fixed) && schemaAttDef.Name.Name == attrLocalname && ((dtdSchemaInfo.SchemaType == SchemaType.DTD && schemaAttDef.Name.Namespace == attrPrefix) || (dtdSchemaInfo.SchemaType != SchemaType.DTD && schemaAttDef.Name.Namespace == attrNamespaceURI)))
					{
						return this.PrepareDefaultAttribute(schemaAttDef, attrPrefix, attrLocalname, attrNamespaceURI);
					}
				}
			}
			return null;
		}

		internal string Version
		{
			get
			{
				XmlDeclaration declaration = this.Declaration;
				if (declaration != null)
				{
					return declaration.Version;
				}
				return null;
			}
		}

		internal string Encoding
		{
			get
			{
				XmlDeclaration declaration = this.Declaration;
				if (declaration != null)
				{
					return declaration.Encoding;
				}
				return null;
			}
		}

		internal string Standalone
		{
			get
			{
				XmlDeclaration declaration = this.Declaration;
				if (declaration != null)
				{
					return declaration.Standalone;
				}
				return null;
			}
		}

		internal XmlEntity GetEntityNode(string name)
		{
			if (this.DocumentType != null)
			{
				XmlNamedNodeMap xmlNamedNodeMap = this.DocumentType.Entities;
				if (xmlNamedNodeMap != null)
				{
					return (XmlEntity)xmlNamedNodeMap.GetNamedItem(name);
				}
			}
			return null;
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				if (this.reportValidity)
				{
					XmlElement documentElement = this.DocumentElement;
					if (documentElement != null)
					{
						XmlSchemaValidity validity = documentElement.SchemaInfo.Validity;
						if (validity == XmlSchemaValidity.Valid)
						{
							return XmlDocument.ValidSchemaInfo;
						}
						if (validity == XmlSchemaValidity.Invalid)
						{
							return XmlDocument.InvalidSchemaInfo;
						}
					}
				}
				return XmlDocument.NotKnownSchemaInfo;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.baseURI;
			}
		}

		internal void SetBaseURI(string inBaseURI)
		{
			this.baseURI = inBaseURI;
		}

		internal override XmlNode AppendChildForLoad(XmlNode newChild, XmlDocument doc)
		{
			if (!this.IsValidChildType(newChild.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("The specified node cannot be inserted as the valid child of this node, because the specified node is the wrong type."));
			}
			if (!this.CanInsertAfter(newChild, this.LastChild))
			{
				throw new InvalidOperationException(Res.GetString("Cannot insert the node in the specified location."));
			}
			XmlNodeChangedEventArgs insertEventArgsForLoad = this.GetInsertEventArgsForLoad(newChild, this);
			if (insertEventArgsForLoad != null)
			{
				this.BeforeEvent(insertEventArgsForLoad);
			}
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
			if (this.lastChild == null)
			{
				xmlLinkedNode.next = xmlLinkedNode;
			}
			else
			{
				xmlLinkedNode.next = this.lastChild.next;
				this.lastChild.next = xmlLinkedNode;
			}
			this.lastChild = xmlLinkedNode;
			xmlLinkedNode.SetParentForLoad(this);
			if (insertEventArgsForLoad != null)
			{
				this.AfterEvent(insertEventArgsForLoad);
			}
			return xmlLinkedNode;
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		internal bool HasEntityReferences
		{
			get
			{
				return this.fEntRefNodesPresent;
			}
		}

		internal XmlAttribute NamespaceXml
		{
			get
			{
				if (this.namespaceXml == null)
				{
					this.namespaceXml = new XmlAttribute(this.AddAttrXmlName(this.strXmlns, this.strXml, this.strReservedXmlns, null), this);
					this.namespaceXml.Value = this.strReservedXml;
				}
				return this.namespaceXml;
			}
		}

		private XmlImplementation implementation;

		private DomNameTable domNameTable;

		private XmlLinkedNode lastChild;

		private XmlNamedNodeMap entities;

		private Hashtable htElementIdMap;

		private Hashtable htElementIDAttrDecl;

		private SchemaInfo schemaInfo;

		private XmlSchemaSet schemas;

		private bool reportValidity;

		private bool actualLoadingStatus;

		private XmlNodeChangedEventHandler onNodeInsertingDelegate;

		private XmlNodeChangedEventHandler onNodeInsertedDelegate;

		private XmlNodeChangedEventHandler onNodeRemovingDelegate;

		private XmlNodeChangedEventHandler onNodeRemovedDelegate;

		private XmlNodeChangedEventHandler onNodeChangingDelegate;

		private XmlNodeChangedEventHandler onNodeChangedDelegate;

		internal bool fEntRefNodesPresent;

		internal bool fCDataNodesPresent;

		private bool preserveWhitespace;

		private bool isLoading;

		internal string strDocumentName;

		internal string strDocumentFragmentName;

		internal string strCommentName;

		internal string strTextName;

		internal string strCDataSectionName;

		internal string strEntityName;

		internal string strID;

		internal string strXmlns;

		internal string strXml;

		internal string strSpace;

		internal string strLang;

		internal string strEmpty;

		internal string strNonSignificantWhitespaceName;

		internal string strSignificantWhitespaceName;

		internal string strReservedXmlns;

		internal string strReservedXml;

		internal string baseURI;

		private XmlResolver resolver;

		internal bool bSetResolver;

		internal object objLock;

		private XmlAttribute namespaceXml;

		internal static EmptyEnumerator EmptyEnumerator = new EmptyEnumerator();

		internal static IXmlSchemaInfo NotKnownSchemaInfo = new XmlSchemaInfo(XmlSchemaValidity.NotKnown);

		internal static IXmlSchemaInfo ValidSchemaInfo = new XmlSchemaInfo(XmlSchemaValidity.Valid);

		internal static IXmlSchemaInfo InvalidSchemaInfo = new XmlSchemaInfo(XmlSchemaValidity.Invalid);
	}
}
