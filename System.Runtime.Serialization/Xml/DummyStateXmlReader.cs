using System;

namespace System.Xml
{
	internal abstract class DummyStateXmlReader : XmlReader
	{
		protected DummyStateXmlReader(string baseUri, XmlNameTable nameTable, ReadState readState)
		{
			this.base_uri = baseUri;
			this.name_table = nameTable;
			this.read_state = readState;
		}

		public override string BaseURI
		{
			get
			{
				return this.base_uri;
			}
		}

		public override bool EOF
		{
			get
			{
				return false;
			}
		}

		public override void Close()
		{
			throw new NotSupportedException();
		}

		public override bool Read()
		{
			throw new NotSupportedException();
		}

		public override int AttributeCount
		{
			get
			{
				return 0;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return false;
			}
		}

		public override string LocalName
		{
			get
			{
				return string.Empty;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return string.Empty;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.name_table;
			}
		}

		public override string Prefix
		{
			get
			{
				return string.Empty;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.read_state;
			}
		}

		public override bool MoveToElement()
		{
			return false;
		}

		public override string GetAttribute(int index)
		{
			return null;
		}

		public override string GetAttribute(string name)
		{
			return null;
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return null;
		}

		public override void MoveToAttribute(int index)
		{
			throw new ArgumentOutOfRangeException();
		}

		public override bool MoveToAttribute(string name)
		{
			return false;
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			return false;
		}

		public override string LookupNamespace(string prefix)
		{
			return null;
		}

		public override bool ReadAttributeValue()
		{
			return false;
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException();
		}

		private string base_uri;

		private XmlNameTable name_table;

		private ReadState read_state;
	}
}
