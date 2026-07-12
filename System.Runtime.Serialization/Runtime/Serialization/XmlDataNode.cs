using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlDataNode : DataNode<object>
	{
		internal XmlDataNode()
		{
			this.dataType = Globals.TypeOfXmlDataNode;
		}

		internal IList<XmlAttribute> XmlAttributes
		{
			get
			{
				return this.xmlAttributes;
			}
			set
			{
				this.xmlAttributes = value;
			}
		}

		internal IList<XmlNode> XmlChildNodes
		{
			get
			{
				return this.xmlChildNodes;
			}
			set
			{
				this.xmlChildNodes = value;
			}
		}

		internal XmlDocument OwnerDocument
		{
			get
			{
				return this.ownerDocument;
			}
			set
			{
				this.ownerDocument = value;
			}
		}

		public override void Clear()
		{
			base.Clear();
			this.xmlAttributes = null;
			this.xmlChildNodes = null;
			this.ownerDocument = null;
		}

		private IList<XmlAttribute> xmlAttributes;

		private IList<XmlNode> xmlChildNodes;

		private XmlDocument ownerDocument;
	}
}
