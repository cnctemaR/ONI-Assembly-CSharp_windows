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
			this._node = node;
		}

		public XmlElement Value
		{
			get
			{
				return this._node;
			}
			set
			{
				this._node = value;
			}
		}

		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal override XmlElement GetXml(XmlDocument xmlDocument)
		{
			return xmlDocument.ImportNode(this._node, true) as XmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			this._node = value;
		}

		private XmlElement _node;
	}
}
