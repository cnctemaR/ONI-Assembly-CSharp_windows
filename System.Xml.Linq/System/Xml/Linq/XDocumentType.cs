using System;

namespace System.Xml.Linq
{
	public class XDocumentType : XNode
	{
		public XDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			this.name = XmlConvert.VerifyName(name);
			this.publicId = publicId;
			this.systemId = systemId;
			this.internalSubset = internalSubset;
		}

		public XDocumentType(XDocumentType other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			this.publicId = other.publicId;
			this.systemId = other.systemId;
			this.internalSubset = other.internalSubset;
			this.dtdInfo = other.dtdInfo;
		}

		internal XDocumentType(XmlReader r)
		{
			this.name = r.Name;
			this.publicId = r.GetAttribute("PUBLIC");
			this.systemId = r.GetAttribute("SYSTEM");
			this.internalSubset = r.Value;
			this.dtdInfo = r.DtdInfo;
			r.Read();
		}

		internal XDocumentType(string name, string publicId, string systemId, string internalSubset, IDtdInfo dtdInfo)
			: this(name, publicId, systemId, internalSubset)
		{
			this.dtdInfo = dtdInfo;
		}

		public string InternalSubset
		{
			get
			{
				return this.internalSubset;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.internalSubset = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				value = XmlConvert.VerifyName(value);
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
				this.name = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Name);
				}
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentType;
			}
		}

		public string PublicId
		{
			get
			{
				return this.publicId;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.publicId = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		public string SystemId
		{
			get
			{
				return this.systemId;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.systemId = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		internal IDtdInfo DtdInfo
		{
			get
			{
				return this.dtdInfo;
			}
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteDocType(this.name, this.publicId, this.systemId, this.internalSubset);
		}

		internal override XNode CloneNode()
		{
			return new XDocumentType(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XDocumentType xdocumentType = node as XDocumentType;
			return xdocumentType != null && this.name == xdocumentType.name && this.publicId == xdocumentType.publicId && this.systemId == xdocumentType.SystemId && this.internalSubset == xdocumentType.internalSubset;
		}

		internal override int GetDeepHashCode()
		{
			return this.name.GetHashCode() ^ ((this.publicId != null) ? this.publicId.GetHashCode() : 0) ^ ((this.systemId != null) ? this.systemId.GetHashCode() : 0) ^ ((this.internalSubset != null) ? this.internalSubset.GetHashCode() : 0);
		}

		private string name;

		private string publicId;

		private string systemId;

		private string internalSubset;

		private IDtdInfo dtdInfo;
	}
}
