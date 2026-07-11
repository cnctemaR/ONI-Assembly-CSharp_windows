using System;
using System.Xml;

namespace System.Data
{
	internal sealed class DataTextReader : XmlReader
	{
		internal static XmlReader CreateReader(XmlReader xr)
		{
			return new DataTextReader(xr);
		}

		private DataTextReader(XmlReader input)
		{
			this._xmlreader = input;
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return this._xmlreader.Settings;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this._xmlreader.NodeType;
			}
		}

		public override string Name
		{
			get
			{
				return this._xmlreader.Name;
			}
		}

		public override string LocalName
		{
			get
			{
				return this._xmlreader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this._xmlreader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				return this._xmlreader.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this._xmlreader.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				return this._xmlreader.Value;
			}
		}

		public override int Depth
		{
			get
			{
				return this._xmlreader.Depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this._xmlreader.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this._xmlreader.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this._xmlreader.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this._xmlreader.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this._xmlreader.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this._xmlreader.XmlLang;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this._xmlreader.AttributeCount;
			}
		}

		public override string GetAttribute(string name)
		{
			return this._xmlreader.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this._xmlreader.GetAttribute(localName, namespaceURI);
		}

		public override string GetAttribute(int i)
		{
			return this._xmlreader.GetAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			return this._xmlreader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return this._xmlreader.MoveToAttribute(localName, namespaceURI);
		}

		public override void MoveToAttribute(int i)
		{
			this._xmlreader.MoveToAttribute(i);
		}

		public override bool MoveToFirstAttribute()
		{
			return this._xmlreader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this._xmlreader.MoveToNextAttribute();
		}

		public override bool MoveToElement()
		{
			return this._xmlreader.MoveToElement();
		}

		public override bool ReadAttributeValue()
		{
			return this._xmlreader.ReadAttributeValue();
		}

		public override bool Read()
		{
			return this._xmlreader.Read();
		}

		public override bool EOF
		{
			get
			{
				return this._xmlreader.EOF;
			}
		}

		public override void Close()
		{
			this._xmlreader.Close();
		}

		public override ReadState ReadState
		{
			get
			{
				return this._xmlreader.ReadState;
			}
		}

		public override void Skip()
		{
			this._xmlreader.Skip();
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this._xmlreader.NameTable;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			return this._xmlreader.LookupNamespace(prefix);
		}

		public override bool CanResolveEntity
		{
			get
			{
				return this._xmlreader.CanResolveEntity;
			}
		}

		public override void ResolveEntity()
		{
			this._xmlreader.ResolveEntity();
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return this._xmlreader.CanReadBinaryContent;
			}
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			return this._xmlreader.ReadContentAsBase64(buffer, index, count);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			return this._xmlreader.ReadElementContentAsBase64(buffer, index, count);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			return this._xmlreader.ReadContentAsBinHex(buffer, index, count);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			return this._xmlreader.ReadElementContentAsBinHex(buffer, index, count);
		}

		public override bool CanReadValueChunk
		{
			get
			{
				return this._xmlreader.CanReadValueChunk;
			}
		}

		public override string ReadString()
		{
			return this._xmlreader.ReadString();
		}

		private XmlReader _xmlreader;
	}
}
