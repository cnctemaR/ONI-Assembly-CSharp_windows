using System;

namespace System.Xml
{
	internal class XmlUnspecifiedAttribute : XmlAttribute
	{
		protected internal XmlUnspecifiedAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		public override bool Specified
		{
			get
			{
				return this.fSpecified;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument ownerDocument = this.OwnerDocument;
			XmlUnspecifiedAttribute xmlUnspecifiedAttribute = (XmlUnspecifiedAttribute)ownerDocument.CreateDefaultAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
			xmlUnspecifiedAttribute.CopyChildren(ownerDocument, this, true);
			xmlUnspecifiedAttribute.fSpecified = true;
			return xmlUnspecifiedAttribute;
		}

		public override string InnerText
		{
			set
			{
				base.InnerText = value;
				this.fSpecified = true;
			}
		}

		public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
		{
			XmlNode xmlNode = base.InsertBefore(newChild, refChild);
			this.fSpecified = true;
			return xmlNode;
		}

		public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
		{
			XmlNode xmlNode = base.InsertAfter(newChild, refChild);
			this.fSpecified = true;
			return xmlNode;
		}

		public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
		{
			XmlNode xmlNode = base.ReplaceChild(newChild, oldChild);
			this.fSpecified = true;
			return xmlNode;
		}

		public override XmlNode RemoveChild(XmlNode oldChild)
		{
			XmlNode xmlNode = base.RemoveChild(oldChild);
			this.fSpecified = true;
			return xmlNode;
		}

		public override XmlNode AppendChild(XmlNode newChild)
		{
			XmlNode xmlNode = base.AppendChild(newChild);
			this.fSpecified = true;
			return xmlNode;
		}

		public override void WriteTo(XmlWriter w)
		{
			if (this.fSpecified)
			{
				base.WriteTo(w);
			}
		}

		internal void SetSpecified(bool f)
		{
			this.fSpecified = f;
		}

		private bool fSpecified;
	}
}
