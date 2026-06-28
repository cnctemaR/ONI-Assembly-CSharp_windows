using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoNode : KeyInfoClause
	{
		public KeyInfoNode()
		{
		}

		public KeyInfoNode(XmlElement node)
		{
			this.LoadXml(node);
		}

		public XmlElement Value
		{
			get
			{
				return this.Node;
			}
			set
			{
				this.Node = value;
			}
		}

		public override XmlElement GetXml()
		{
			return this.Node;
		}

		public override void LoadXml(XmlElement value)
		{
			this.Node = value;
		}

		private XmlElement Node;
	}
}
