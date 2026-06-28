using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlWhitespace : XmlCharacterData
	{
		protected internal XmlWhitespace(string strData, XmlDocument doc)
			: base(strData, doc)
		{
		}

		public override string LocalName
		{
			get
			{
				return "#whitespace";
			}
		}

		public override string Name
		{
			get
			{
				return "#whitespace";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Whitespace;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Whitespace;
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
				if (!XmlChar.IsWhitespace(value))
				{
					throw new ArgumentException("Invalid whitespace characters.");
				}
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
			return new XmlWhitespace(this.Data, this.OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteWhitespace(this.Data);
		}
	}
}
