using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlSignificantWhitespace : XmlCharacterData
	{
		protected internal XmlSignificantWhitespace(string strData, XmlDocument doc)
			: base(strData, doc)
		{
		}

		public override string LocalName
		{
			get
			{
				return "#significant-whitespace";
			}
		}

		public override string Name
		{
			get
			{
				return "#significant-whitespace";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.SignificantWhitespace;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.SignificantWhitespace;
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
				base.Data = value;
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
			return new XmlSignificantWhitespace(this.Data, this.OwnerDocument);
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
