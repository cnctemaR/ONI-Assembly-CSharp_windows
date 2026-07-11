using System;

namespace System.Xml
{
	internal class NonInteractiveStateXmlReader : DummyStateXmlReader
	{
		public NonInteractiveStateXmlReader(string baseUri, XmlNameTable nameTable, ReadState readState)
			: base(baseUri, nameTable, readState)
		{
		}

		public override int Depth
		{
			get
			{
				return 0;
			}
		}

		public override bool HasValue
		{
			get
			{
				return false;
			}
		}

		public override string Value
		{
			get
			{
				return string.Empty;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.None;
			}
		}
	}
}
