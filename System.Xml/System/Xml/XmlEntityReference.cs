using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlEntityReference : XmlLinkedNode, IHasXmlChildNode
	{
		protected internal XmlEntityReference(string name, XmlDocument doc)
			: base(doc)
		{
			XmlConvert.VerifyName(name);
			this.entityName = doc.NameTable.Add(name);
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

		public override string BaseURI
		{
			get
			{
				return base.BaseURI;
			}
		}

		private XmlEntity Entity
		{
			get
			{
				XmlDocumentType documentType = this.OwnerDocument.DocumentType;
				if (documentType == null)
				{
					return null;
				}
				if (documentType.Entities == null)
				{
					return null;
				}
				return documentType.Entities.GetNamedItem(this.Name) as XmlEntity;
			}
		}

		internal override string ChildrenBaseURI
		{
			get
			{
				XmlEntity entity = this.Entity;
				if (entity == null)
				{
					return string.Empty;
				}
				if (entity.SystemId == null || entity.SystemId.Length == 0)
				{
					return entity.BaseURI;
				}
				if (entity.BaseURI == null || entity.BaseURI.Length == 0)
				{
					return entity.SystemId;
				}
				Uri uri = null;
				try
				{
					uri = new Uri(entity.BaseURI);
				}
				catch (UriFormatException)
				{
				}
				XmlResolver resolver = this.OwnerDocument.Resolver;
				if (resolver != null)
				{
					return resolver.ResolveUri(uri, entity.SystemId).ToString();
				}
				return new Uri(uri, entity.SystemId).ToString();
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
				return this.entityName;
			}
		}

		public override string Name
		{
			get
			{
				return this.entityName;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.EntityReference;
			}
		}

		public override string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new XmlException("entity reference cannot be set value.");
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Text;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlEntityReference(this.Name, this.OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (int i = 0; i < this.ChildNodes.Count; i++)
			{
				this.ChildNodes[i].WriteTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteRaw("&");
			w.WriteName(this.Name);
			w.WriteRaw(";");
		}

		internal void SetReferencedEntityContent()
		{
			if (this.FirstChild != null)
			{
				return;
			}
			if (this.OwnerDocument.DocumentType == null)
			{
				return;
			}
			XmlEntity entity = this.Entity;
			if (entity == null)
			{
				base.InsertBefore(this.OwnerDocument.CreateTextNode(string.Empty), null, false, true);
			}
			else
			{
				for (int i = 0; i < entity.ChildNodes.Count; i++)
				{
					base.InsertBefore(entity.ChildNodes[i].CloneNode(true), null, false, true);
				}
			}
		}

		private string entityName;

		private XmlLinkedNode lastLinkedChild;
	}
}
