using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlDocumentFragment : XmlNode
	{
		protected internal XmlDocumentFragment(XmlDocument ownerDocument)
		{
			if (ownerDocument == null)
			{
				throw new ArgumentException(Res.GetString("Cannot create a node without an owner document."));
			}
			this.parentNode = ownerDocument;
		}

		public override string Name
		{
			get
			{
				return this.OwnerDocument.strDocumentFragmentName;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.OwnerDocument.strDocumentFragmentName;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentFragment;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return (XmlDocument)this.parentNode;
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
				this.RemoveAll();
				new XmlLoader().ParsePartialContent(this, value, XmlNodeType.Element);
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument ownerDocument = this.OwnerDocument;
			XmlDocumentFragment xmlDocumentFragment = ownerDocument.CreateDocumentFragment();
			if (deep)
			{
				xmlDocumentFragment.CopyChildren(ownerDocument, this, deep);
			}
			return xmlDocumentFragment;
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

		internal override bool IsValidChildType(XmlNodeType type)
		{
			switch (type)
			{
			case XmlNodeType.Element:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.EntityReference:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				return true;
			case XmlNodeType.XmlDeclaration:
			{
				XmlNode firstChild = this.FirstChild;
				return firstChild == null || firstChild.NodeType != XmlNodeType.XmlDeclaration;
			}
			}
			return false;
		}

		internal override bool CanInsertAfter(XmlNode newChild, XmlNode refChild)
		{
			return newChild.NodeType != XmlNodeType.XmlDeclaration || (refChild == null && this.LastNode == null);
		}

		internal override bool CanInsertBefore(XmlNode newChild, XmlNode refChild)
		{
			return newChild.NodeType != XmlNodeType.XmlDeclaration || refChild == null || refChild == this.FirstChild;
		}

		public override void WriteTo(XmlWriter w)
		{
			this.WriteContentTo(w);
		}

		public override void WriteContentTo(XmlWriter w)
		{
			foreach (object obj in this)
			{
				((XmlNode)obj).WriteTo(w);
			}
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		private XmlLinkedNode lastChild;
	}
}
