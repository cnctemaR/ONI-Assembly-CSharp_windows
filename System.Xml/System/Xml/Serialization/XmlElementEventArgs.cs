using System;

namespace System.Xml.Serialization
{
	public class XmlElementEventArgs : EventArgs
	{
		internal XmlElementEventArgs(XmlElement attr, int lineNum, int linePos, object source)
		{
			this.attr = attr;
			this.lineNumber = lineNum;
			this.linePosition = linePos;
			this.obj = source;
		}

		public XmlElement Element
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

		public string ExpectedElements
		{
			get
			{
				return this.expectedElements;
			}
			internal set
			{
				this.expectedElements = value;
			}
		}

		private XmlElement attr;

		private int lineNumber;

		private int linePosition;

		private object obj;

		private string expectedElements;
	}
}
