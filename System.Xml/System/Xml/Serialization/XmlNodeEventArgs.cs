using System;

namespace System.Xml.Serialization
{
	public class XmlNodeEventArgs : EventArgs
	{
		internal XmlNodeEventArgs(int linenumber, int lineposition, string localname, string name, string nsuri, XmlNodeType nodetype, object source, string text)
		{
			this.linenumber = linenumber;
			this.lineposition = lineposition;
			this.localname = localname;
			this.name = name;
			this.nsuri = nsuri;
			this.nodetype = nodetype;
			this.source = source;
			this.text = text;
		}

		public int LineNumber
		{
			get
			{
				return this.linenumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.lineposition;
			}
		}

		public string LocalName
		{
			get
			{
				return this.localname;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string NamespaceURI
		{
			get
			{
				return this.nsuri;
			}
		}

		public XmlNodeType NodeType
		{
			get
			{
				return this.nodetype;
			}
		}

		public object ObjectBeingDeserialized
		{
			get
			{
				return this.source;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		private int linenumber;

		private int lineposition;

		private string localname;

		private string name;

		private string nsuri;

		private XmlNodeType nodetype;

		private object source;

		private string text;
	}
}
