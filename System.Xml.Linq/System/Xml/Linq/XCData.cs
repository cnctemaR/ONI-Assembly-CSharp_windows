using System;

namespace System.Xml.Linq
{
	public class XCData : XText
	{
		public XCData(string value)
			: base(value)
		{
		}

		public XCData(XCData other)
			: base(other)
		{
		}

		internal XCData(XmlReader r)
			: base(r)
		{
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.CDATA;
			}
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteCData(this.text);
		}

		internal override XNode CloneNode()
		{
			return new XCData(this);
		}
	}
}
