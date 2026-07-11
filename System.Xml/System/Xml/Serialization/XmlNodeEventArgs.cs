using System;

namespace System.Xml.Serialization
{
	public class XmlNodeEventArgs : EventArgs
	{
		internal XmlNodeEventArgs(XmlNode xmlNode, int lineNumber, int linePosition, object o)
		{
			this.o = o;
			this.xmlNode = xmlNode;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		public object ObjectBeingDeserialized
		{
			get
			{
				return this.o;
			}
		}

		public XmlNodeType NodeType
		{
			get
			{
				return this.xmlNode.NodeType;
			}
		}

		public string Name
		{
			get
			{
				return this.xmlNode.Name;
			}
		}

		public string LocalName
		{
			get
			{
				return this.xmlNode.LocalName;
			}
		}

		public string NamespaceURI
		{
			get
			{
				return this.xmlNode.NamespaceURI;
			}
		}

		public string Text
		{
			get
			{
				return this.xmlNode.Value;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		private object o;

		private XmlNode xmlNode;

		private int lineNumber;

		private int linePosition;
	}
}
