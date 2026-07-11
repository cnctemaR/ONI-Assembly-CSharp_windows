using System;

namespace System.Xml
{
	internal class MultiPartedXmlReader : DummyStateXmlReader
	{
		public MultiPartedXmlReader(XmlReader reader, MimeEncodedStream value)
			: base(reader.BaseURI, reader.NameTable, reader.ReadState)
		{
			this.owner = reader;
			this.value = value.CreateTextReader().ReadToEnd();
		}

		public override int Depth
		{
			get
			{
				return this.owner.Depth;
			}
		}

		public override bool HasValue
		{
			get
			{
				return true;
			}
		}

		public override string Value
		{
			get
			{
				return this.value;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Text;
			}
		}

		private XmlReader owner;

		private string value;
	}
}
