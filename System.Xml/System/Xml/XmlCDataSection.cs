using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlCDataSection : XmlCharacterData
	{
		protected internal XmlCDataSection(string data, XmlDocument doc)
			: base(data, doc)
		{
		}

		public override string Name
		{
			get
			{
				return this.OwnerDocument.strCDataSectionName;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.OwnerDocument.strCDataSectionName;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.CDATA;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				XmlNodeType nodeType = this.parentNode.NodeType;
				if (nodeType - XmlNodeType.Text > 1)
				{
					if (nodeType == XmlNodeType.Document)
					{
						return null;
					}
					if (nodeType - XmlNodeType.Whitespace > 1)
					{
						return this.parentNode;
					}
				}
				XmlNode xmlNode = this.parentNode.parentNode;
				while (xmlNode.IsText)
				{
					xmlNode = xmlNode.parentNode;
				}
				return xmlNode;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateCDataSection(this.Data);
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteCData(this.Data);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.Text;
			}
		}

		internal override bool IsText
		{
			get
			{
				return true;
			}
		}

		public override XmlNode PreviousText
		{
			get
			{
				if (this.parentNode.IsText)
				{
					return this.parentNode;
				}
				return null;
			}
		}
	}
}
