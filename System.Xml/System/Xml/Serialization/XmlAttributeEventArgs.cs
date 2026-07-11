using System;

namespace System.Xml.Serialization
{
	public class XmlAttributeEventArgs : EventArgs
	{
		internal XmlAttributeEventArgs(XmlAttribute attr, int lineNum, int linePos, object source)
		{
			this.attr = attr;
			this.lineNumber = lineNum;
			this.linePosition = linePos;
			this.obj = source;
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

		public object ObjectBeingDeserialized
		{
			get
			{
				return this.obj;
			}
		}

		public string ExpectedAttributes
		{
			get
			{
				return this.expectedAttributes;
			}
			internal set
			{
				this.expectedAttributes = value;
			}
		}

		private XmlAttribute attr;

		private int lineNumber;

		private int linePosition;

		private object obj;

		private string expectedAttributes;
	}
}
