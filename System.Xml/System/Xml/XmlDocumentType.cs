using System;
using System.IO;
using Mono.Xml;
using Mono.Xml2;

namespace System.Xml
{
	public class XmlDocumentType : XmlLinkedNode
	{
		protected internal XmlDocumentType(string name, string publicId, string systemId, string internalSubset, XmlDocument doc)
			: base(doc)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(this.BaseURI, new StringReader(string.Empty), doc.NameTable);
			xmlTextReader.XmlResolver = doc.Resolver;
			xmlTextReader.GenerateDTDObjectModel(name, publicId, systemId, internalSubset);
			this.dtd = xmlTextReader.DTD;
			this.ImportFromDTD();
		}

		internal XmlDocumentType(DTDObjectModel dtd, XmlDocument doc)
			: base(doc)
		{
			this.dtd = dtd;
			this.ImportFromDTD();
		}

		private void ImportFromDTD()
		{
			this.entities = new XmlNamedNodeMap(this);
			this.notations = new XmlNamedNodeMap(this);
			foreach (DTDNode dtdnode in this.DTD.EntityDecls.Values)
			{
				DTDEntityDeclaration dtdentityDeclaration = (DTDEntityDeclaration)dtdnode;
				XmlNode xmlNode = new XmlEntity(dtdentityDeclaration.Name, dtdentityDeclaration.NotationName, dtdentityDeclaration.PublicId, dtdentityDeclaration.SystemId, this.OwnerDocument);
				this.entities.SetNamedItem(xmlNode);
			}
			foreach (DTDNode dtdnode2 in this.DTD.NotationDecls.Values)
			{
				DTDNotationDeclaration dtdnotationDeclaration = (DTDNotationDeclaration)dtdnode2;
				XmlNode xmlNode2 = new XmlNotation(dtdnotationDeclaration.LocalName, dtdnotationDeclaration.Prefix, dtdnotationDeclaration.PublicId, dtdnotationDeclaration.SystemId, this.OwnerDocument);
				this.notations.SetNamedItem(xmlNode2);
			}
		}

		internal DTDObjectModel DTD
		{
			get
			{
				return this.dtd;
			}
		}

		public XmlNamedNodeMap Entities
		{
			get
			{
				return this.entities;
			}
		}

		public string InternalSubset
		{
			get
			{
				return this.dtd.InternalSubset;
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
				return this.dtd.Name;
			}
		}

		public override string Name
		{
			get
			{
				return this.dtd.Name;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentType;
			}
		}

		public XmlNamedNodeMap Notations
		{
			get
			{
				return this.notations;
			}
		}

		public string PublicId
		{
			get
			{
				return this.dtd.PublicId;
			}
		}

		public string SystemId
		{
			get
			{
				return this.dtd.SystemId;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlDocumentType(this.dtd, this.OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteDocType(this.Name, this.PublicId, this.SystemId, this.InternalSubset);
		}

		internal XmlNamedNodeMap entities;

		internal XmlNamedNodeMap notations;

		private DTDObjectModel dtd;
	}
}
