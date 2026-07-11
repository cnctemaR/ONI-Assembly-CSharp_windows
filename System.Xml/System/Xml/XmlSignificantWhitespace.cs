using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlSignificantWhitespace : XmlCharacterData
	{
		protected internal XmlSignificantWhitespace(string strData, XmlDocument doc)
			: base(strData, doc)
		{
			if (!doc.IsLoading && !base.CheckOnData(strData))
			{
				throw new ArgumentException(Res.GetString("The string for white space contains an invalid character."));
			}
		}

		public override string Name
		{
			get
			{
				return this.OwnerDocument.strSignificantWhitespaceName;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.OwnerDocument.strSignificantWhitespaceName;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.SignificantWhitespace;
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
						return base.ParentNode;
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
			return this.OwnerDocument.CreateSignificantWhitespace(this.Data);
		}

		public override string Value
		{
			get
			{
				return this.Data;
			}
			set
			{
				if (base.CheckOnData(value))
				{
					this.Data = value;
					return;
				}
				throw new ArgumentException(Res.GetString("The string for white space contains an invalid character."));
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteString(this.Data);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				XPathNodeType xpathNodeType = XPathNodeType.SignificantWhitespace;
				base.DecideXPNodeTypeForTextNodes(this, ref xpathNodeType);
				return xpathNodeType;
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
