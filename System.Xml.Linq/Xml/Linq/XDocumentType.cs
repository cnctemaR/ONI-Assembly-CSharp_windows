using System;

namespace System.Xml.Linq
{
	public class XDocumentType : XNode
	{
		public XDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			this.name = name;
			this.pubid = publicId;
			this.sysid = systemId;
			this.intSubset = internalSubset;
		}

		public XDocumentType(XDocumentType other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			this.pubid = other.pubid;
			this.sysid = other.sysid;
			this.intSubset = other.intSubset;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.name = value;
			}
		}

		public string PublicId
		{
			get
			{
				return this.pubid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.pubid = value;
			}
		}

		public string SystemId
		{
			get
			{
				return this.sysid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.sysid = value;
			}
		}

		public string InternalSubset
		{
			get
			{
				return this.intSubset;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.intSubset = value;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentType;
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			XDocument document = base.Document;
			XElement root = document.Root;
			if (root != null)
			{
				w.WriteDocType(root.Name.LocalName, this.pubid, this.sysid, this.intSubset);
			}
		}

		private string name;

		private string pubid;

		private string sysid;

		private string intSubset;
	}
}
