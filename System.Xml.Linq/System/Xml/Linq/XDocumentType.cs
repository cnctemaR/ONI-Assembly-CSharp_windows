using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
	public class XDocumentType : XNode
	{
		public XDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			this._name = XmlConvert.VerifyName(name);
			this._publicId = publicId;
			this._systemId = systemId;
			this._internalSubset = internalSubset;
		}

		public XDocumentType(XDocumentType other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this._name = other._name;
			this._publicId = other._publicId;
			this._systemId = other._systemId;
			this._internalSubset = other._internalSubset;
		}

		internal XDocumentType(XmlReader r)
		{
			this._name = r.Name;
			this._publicId = r.GetAttribute("PUBLIC");
			this._systemId = r.GetAttribute("SYSTEM");
			this._internalSubset = r.Value;
			r.Read();
		}

		public string InternalSubset
		{
			get
			{
				return this._internalSubset;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this._internalSubset = value;
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
				return this._name;
			}
			set
			{
				value = XmlConvert.VerifyName(value);
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
				this._name = value;
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
				return this._publicId;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this._publicId = value;
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
				return this._systemId;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this._systemId = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteDocType(this._name, this._publicId, this._systemId, this._internalSubset);
		}

		public override Task WriteToAsync(XmlWriter writer, CancellationToken cancellationToken)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			return writer.WriteDocTypeAsync(this._name, this._publicId, this._systemId, this._internalSubset);
		}

		internal override XNode CloneNode()
		{
			return new XDocumentType(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XDocumentType xdocumentType = node as XDocumentType;
			return xdocumentType != null && this._name == xdocumentType._name && this._publicId == xdocumentType._publicId && this._systemId == xdocumentType.SystemId && this._internalSubset == xdocumentType._internalSubset;
		}

		internal override int GetDeepHashCode()
		{
			return this._name.GetHashCode() ^ ((this._publicId != null) ? this._publicId.GetHashCode() : 0) ^ ((this._systemId != null) ? this._systemId.GetHashCode() : 0) ^ ((this._internalSubset != null) ? this._internalSubset.GetHashCode() : 0);
		}

		private string _name;

		private string _publicId;

		private string _systemId;

		private string _internalSubset;
	}
}
