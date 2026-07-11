using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlComment : XmlCharacterData
	{
		protected internal XmlComment(string comment, XmlDocument doc)
			: base(comment, doc)
		{
		}

		public override string LocalName
		{
			get
			{
				return "#comment";
			}
		}

		public override string Name
		{
			get
			{
				return "#comment";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Comment;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Comment;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlComment(this.Value, this.OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteComment(this.Data);
		}
	}
}
