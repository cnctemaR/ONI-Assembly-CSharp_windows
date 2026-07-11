using System;
using Mono.Xml;

namespace System.Xml
{
	public class XmlEntity : XmlNode, IHasXmlChildNode
	{
		internal XmlEntity(string name, string NDATA, string publicId, string systemId, XmlDocument doc)
			: base(doc)
		{
			this.name = doc.NameTable.Add(name);
			this.NDATA = NDATA;
			this.publicId = publicId;
			this.systemId = systemId;
			this.baseUri = doc.BaseURI;
		}

		XmlLinkedNode IHasXmlChildNode.LastLinkedChild
		{
			get
			{
				if (this.lastLinkedChild != null)
				{
					return this.lastLinkedChild;
				}
				if (!this.contentAlreadySet)
				{
					this.contentAlreadySet = true;
					this.SetEntityContent();
				}
				return this.lastLinkedChild;
			}
			set
			{
				this.lastLinkedChild = value;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.baseUri;
			}
		}

		public override string InnerText
		{
			get
			{
				return base.InnerText;
			}
			set
			{
				throw new InvalidOperationException("This operation is not supported.");
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
				throw new InvalidOperationException("This operation is not supported.");
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.name;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Entity;
			}
		}

		public string NotationName
		{
			get
			{
				if (this.NDATA == null)
				{
					return null;
				}
				return this.NDATA;
			}
		}

		public override string OuterXml
		{
			get
			{
				return string.Empty;
			}
		}

		public string PublicId
		{
			get
			{
				return this.publicId;
			}
		}

		public string SystemId
		{
			get
			{
				return this.systemId;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			throw new InvalidOperationException("This operation is not supported.");
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
		}

		private void SetEntityContent()
		{
			if (this.lastLinkedChild != null)
			{
				return;
			}
			XmlDocumentType documentType = this.OwnerDocument.DocumentType;
			if (documentType == null)
			{
				return;
			}
			DTDEntityDeclaration dtdentityDeclaration = documentType.DTD.EntityDecls[this.name];
			if (dtdentityDeclaration == null)
			{
				return;
			}
			XmlNamespaceManager xmlNamespaceManager = base.ConstructNamespaceManager();
			XmlParserContext xmlParserContext = new XmlParserContext(this.OwnerDocument.NameTable, xmlNamespaceManager, (documentType == null) ? null : documentType.DTD, this.BaseURI, this.XmlLang, this.XmlSpace, null);
			XmlTextReader xmlTextReader = new XmlTextReader(dtdentityDeclaration.EntityValue, XmlNodeType.Element, xmlParserContext);
			xmlTextReader.XmlResolver = this.OwnerDocument.Resolver;
			for (;;)
			{
				XmlNode xmlNode = this.OwnerDocument.ReadNode(xmlTextReader);
				if (xmlNode == null)
				{
					break;
				}
				base.InsertBefore(xmlNode, null, false, false);
			}
		}

		private string name;

		private string NDATA;

		private string publicId;

		private string systemId;

		private string baseUri;

		private XmlLinkedNode lastLinkedChild;

		private bool contentAlreadySet;
	}
}
