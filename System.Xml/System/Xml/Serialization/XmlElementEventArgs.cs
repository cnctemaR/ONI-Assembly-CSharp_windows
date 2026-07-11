using System;

namespace System.Xml.Serialization
{
	public class XmlElementEventArgs : EventArgs
	{
		internal XmlElementEventArgs(XmlElement elem, int lineNumber, int linePosition, object o, string qnames)
		{
			this.elem = elem;
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

		public XmlElement Element
		{
			get
			{
				return this.elem;
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

		public string ExpectedElements
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

		private XmlElement elem;

		private string qnames;

		private int lineNumber;

		private int linePosition;
	}
}
