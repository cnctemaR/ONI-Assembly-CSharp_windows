using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlText : XmlCharacterData
	{
		protected internal XmlText(string strData, XmlDocument doc)
			: base(strData, doc)
		{
		}

		public override string LocalName
		{
			get
			{
				return "#text";
			}
		}

		public override string Name
		{
			get
			{
				return "#text";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Text;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Text;
			}
		}

		public override string Value
		{
			get
			{
				return this.Data;
			}
			set
			{
				this.Data = value;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return base.ParentNode;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateTextNode(this.Data);
		}

		public virtual XmlText SplitText(int offset)
		{
			XmlText xmlText = this.OwnerDocument.CreateTextNode(this.Data.Substring(offset));
			this.DeleteData(offset, this.Data.Length - offset);
			this.ParentNode.InsertAfter(xmlText, this);
			return xmlText;
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteString(this.Data);
		}
	}
}
