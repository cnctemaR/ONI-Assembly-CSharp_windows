using System;

namespace System.Xml.Serialization
{
	public class XmlAttributeEventArgs : EventArgs
	{
		internal XmlAttributeEventArgs(XmlAttribute attr, int lineNumber, int linePosition, object o, string qnames)
		{
			this.attr = attr;
			this.o = o;
			this.qnames = qnames;
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

		public XmlAttribute Attr
		{
			get
			{
				return this.attr;
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

		public string ExpectedAttributes
		{
			get
			{
				if (this.qnames != null)
				{
					return this.qnames;
				}
				return string.Empty;
			}
		}

		private object o;

		private XmlAttribute attr;

		private string qnames;

		private int lineNumber;

		private int linePosition;
	}
}
