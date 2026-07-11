using System;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace System.Xml
{
	internal class XmlAsyncCheckReader : XmlReader
	{
		internal XmlReader CoreReader
		{
			get
			{
				return this.coreReader;
			}
		}

		public static XmlAsyncCheckReader CreateAsyncCheckWrapper(XmlReader reader)
		{
			if (reader is IXmlLineInfo)
			{
				if (!(reader is IXmlNamespaceResolver))
				{
					return new XmlAsyncCheckReaderWithLineInfo(reader);
				}
				if (reader is IXmlSchemaInfo)
				{
					return new XmlAsyncCheckReaderWithLineInfoNSSchema(reader);
				}
				return new XmlAsyncCheckReaderWithLineInfoNS(reader);
			}
			else
			{
				if (reader is IXmlNamespaceResolver)
				{
					return new XmlAsyncCheckReaderWithNS(reader);
				}
				return new XmlAsyncCheckReader(reader);
			}
		}

		public XmlAsyncCheckReader(XmlReader reader)
		{
			this.coreReader = reader;
		}

		private void CheckAsync()
		{
			if (!this.lastTask.IsCompleted)
			{
				throw new InvalidOperationException(Res.GetString("An asynchronous operation is already in progress."));
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings xmlReaderSettings = this.coreReader.Settings;
				if (xmlReaderSettings != null)
				{
					xmlReaderSettings = xmlReaderSettings.Clone();
				}
				else
				{
					xmlReaderSettings = new XmlReaderSettings();
				}
				xmlReaderSettings.Async = true;
				xmlReaderSettings.ReadOnly = true;
				return xmlReaderSettings;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.NodeType;
			}
		}

		public override string Name
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.Name;
			}
		}

		public override string LocalName
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.Value;
			}
		}

		public override int Depth
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.Depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.XmlLang;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.SchemaInfo;
			}
		}

		public override Type ValueType
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.ValueType;
			}
		}

		public override object ReadContentAsObject()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsObject();
		}

		public override bool ReadContentAsBoolean()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsBoolean();
		}

		public override DateTime ReadContentAsDateTime()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsDateTime();
		}

		public override double ReadContentAsDouble()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsDouble();
		}

		public override float ReadContentAsFloat()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsFloat();
		}

		public override decimal ReadContentAsDecimal()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsDecimal();
		}

		public override int ReadContentAsInt()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsInt();
		}

		public override long ReadContentAsLong()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsLong();
		}

		public override string ReadContentAsString()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsString();
		}

		public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAs(returnType, namespaceResolver);
		}

		public override object ReadElementContentAsObject()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsObject();
		}

		public override object ReadElementContentAsObject(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsObject(localName, namespaceURI);
		}

		public override bool ReadElementContentAsBoolean()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsBoolean();
		}

		public override bool ReadElementContentAsBoolean(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsBoolean(localName, namespaceURI);
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsDateTime();
		}

		public override DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsDateTime(localName, namespaceURI);
		}

		public override DateTimeOffset ReadContentAsDateTimeOffset()
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsDateTimeOffset();
		}

		public override double ReadElementContentAsDouble()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsDouble();
		}

		public override double ReadElementContentAsDouble(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsDouble(localName, namespaceURI);
		}

		public override float ReadElementContentAsFloat()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsFloat();
		}

		public override float ReadElementContentAsFloat(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsFloat(localName, namespaceURI);
		}

		public override decimal ReadElementContentAsDecimal()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsDecimal();
		}

		public override decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsDecimal(localName, namespaceURI);
		}

		public override int ReadElementContentAsInt()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsInt();
		}

		public override int ReadElementContentAsInt(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsInt(localName, namespaceURI);
		}

		public override long ReadElementContentAsLong()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsLong();
		}

		public override long ReadElementContentAsLong(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsLong(localName, namespaceURI);
		}

		public override string ReadElementContentAsString()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsString();
		}

		public override string ReadElementContentAsString(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsString(localName, namespaceURI);
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAs(returnType, namespaceResolver);
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAs(returnType, namespaceResolver, localName, namespaceURI);
		}

		public override int AttributeCount
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.AttributeCount;
			}
		}

		public override string GetAttribute(string name)
		{
			this.CheckAsync();
			return this.coreReader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.GetAttribute(name, namespaceURI);
		}

		public override string GetAttribute(int i)
		{
			this.CheckAsync();
			return this.coreReader.GetAttribute(i);
		}

		public override string this[int i]
		{
			get
			{
				this.CheckAsync();
				return this.coreReader[i];
			}
		}

		public override string this[string name]
		{
			get
			{
				this.CheckAsync();
				return this.coreReader[name];
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				this.CheckAsync();
				return this.coreReader[name, namespaceURI];
			}
		}

		public override bool MoveToAttribute(string name)
		{
			this.CheckAsync();
			return this.coreReader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			this.CheckAsync();
			return this.coreReader.MoveToAttribute(name, ns);
		}

		public override void MoveToAttribute(int i)
		{
			this.CheckAsync();
			this.coreReader.MoveToAttribute(i);
		}

		public override bool MoveToFirstAttribute()
		{
			this.CheckAsync();
			return this.coreReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			this.CheckAsync();
			return this.coreReader.MoveToNextAttribute();
		}

		public override bool MoveToElement()
		{
			this.CheckAsync();
			return this.coreReader.MoveToElement();
		}

		public override bool ReadAttributeValue()
		{
			this.CheckAsync();
			return this.coreReader.ReadAttributeValue();
		}

		public override bool Read()
		{
			this.CheckAsync();
			return this.coreReader.Read();
		}

		public override bool EOF
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.EOF;
			}
		}

		public override void Close()
		{
			this.CheckAsync();
			this.coreReader.Close();
		}

		public override ReadState ReadState
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.ReadState;
			}
		}

		public override void Skip()
		{
			this.CheckAsync();
			this.coreReader.Skip();
		}

		public override XmlNameTable NameTable
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.NameTable;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			this.CheckAsync();
			return this.coreReader.LookupNamespace(prefix);
		}

		public override bool CanResolveEntity
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.CanResolveEntity;
			}
		}

		public override void ResolveEntity()
		{
			this.CheckAsync();
			this.coreReader.ResolveEntity();
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.CanReadBinaryContent;
			}
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsBase64(buffer, index, count);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsBase64(buffer, index, count);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			return this.coreReader.ReadContentAsBinHex(buffer, index, count);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementContentAsBinHex(buffer, index, count);
		}

		public override bool CanReadValueChunk
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.CanReadValueChunk;
			}
		}

		public override int ReadValueChunk(char[] buffer, int index, int count)
		{
			this.CheckAsync();
			return this.coreReader.ReadValueChunk(buffer, index, count);
		}

		public override string ReadString()
		{
			this.CheckAsync();
			return this.coreReader.ReadString();
		}

		public override XmlNodeType MoveToContent()
		{
			this.CheckAsync();
			return this.coreReader.MoveToContent();
		}

		public override void ReadStartElement()
		{
			this.CheckAsync();
			this.coreReader.ReadStartElement();
		}

		public override void ReadStartElement(string name)
		{
			this.CheckAsync();
			this.coreReader.ReadStartElement(name);
		}

		public override void ReadStartElement(string localname, string ns)
		{
			this.CheckAsync();
			this.coreReader.ReadStartElement(localname, ns);
		}

		public override string ReadElementString()
		{
			this.CheckAsync();
			return this.coreReader.ReadElementString();
		}

		public override string ReadElementString(string name)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementString(name);
		}

		public override string ReadElementString(string localname, string ns)
		{
			this.CheckAsync();
			return this.coreReader.ReadElementString(localname, ns);
		}

		public override void ReadEndElement()
		{
			this.CheckAsync();
			this.coreReader.ReadEndElement();
		}

		public override bool IsStartElement()
		{
			this.CheckAsync();
			return this.coreReader.IsStartElement();
		}

		public override bool IsStartElement(string name)
		{
			this.CheckAsync();
			return this.coreReader.IsStartElement(name);
		}

		public override bool IsStartElement(string localname, string ns)
		{
			this.CheckAsync();
			return this.coreReader.IsStartElement(localname, ns);
		}

		public override bool ReadToFollowing(string name)
		{
			this.CheckAsync();
			return this.coreReader.ReadToFollowing(name);
		}

		public override bool ReadToFollowing(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadToFollowing(localName, namespaceURI);
		}

		public override bool ReadToDescendant(string name)
		{
			this.CheckAsync();
			return this.coreReader.ReadToDescendant(name);
		}

		public override bool ReadToDescendant(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadToDescendant(localName, namespaceURI);
		}

		public override bool ReadToNextSibling(string name)
		{
			this.CheckAsync();
			return this.coreReader.ReadToNextSibling(name);
		}

		public override bool ReadToNextSibling(string localName, string namespaceURI)
		{
			this.CheckAsync();
			return this.coreReader.ReadToNextSibling(localName, namespaceURI);
		}

		public override string ReadInnerXml()
		{
			this.CheckAsync();
			return this.coreReader.ReadInnerXml();
		}

		public override string ReadOuterXml()
		{
			this.CheckAsync();
			return this.coreReader.ReadOuterXml();
		}

		public override XmlReader ReadSubtree()
		{
			this.CheckAsync();
			return XmlAsyncCheckReader.CreateAsyncCheckWrapper(this.coreReader.ReadSubtree());
		}

		public override bool HasAttributes
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.HasAttributes;
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.CheckAsync();
			this.coreReader.Dispose();
		}

		internal override XmlNamespaceManager NamespaceManager
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.NamespaceManager;
			}
		}

		internal override IDtdInfo DtdInfo
		{
			get
			{
				this.CheckAsync();
				return this.coreReader.DtdInfo;
			}
		}

		public override Task<string> GetValueAsync()
		{
			this.CheckAsync();
			Task<string> valueAsync = this.coreReader.GetValueAsync();
			this.lastTask = valueAsync;
			return valueAsync;
		}

		public override Task<object> ReadContentAsObjectAsync()
		{
			this.CheckAsync();
			Task<object> task = this.coreReader.ReadContentAsObjectAsync();
			this.lastTask = task;
			return task;
		}

		public override Task<string> ReadContentAsStringAsync()
		{
			this.CheckAsync();
			Task<string> task = this.coreReader.ReadContentAsStringAsync();
			this.lastTask = task;
			return task;
		}

		public override Task<object> ReadContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			this.CheckAsync();
			Task<object> task = this.coreReader.ReadContentAsAsync(returnType, namespaceResolver);
			this.lastTask = task;
			return task;
		}

		public override Task<object> ReadElementContentAsObjectAsync()
		{
			this.CheckAsync();
			Task<object> task = this.coreReader.ReadElementContentAsObjectAsync();
			this.lastTask = task;
			return task;
		}

		public override Task<string> ReadElementContentAsStringAsync()
		{
			this.CheckAsync();
			Task<string> task = this.coreReader.ReadElementContentAsStringAsync();
			this.lastTask = task;
			return task;
		}

		public override Task<object> ReadElementContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			this.CheckAsync();
			Task<object> task = this.coreReader.ReadElementContentAsAsync(returnType, namespaceResolver);
			this.lastTask = task;
			return task;
		}

		public override Task<bool> ReadAsync()
		{
			this.CheckAsync();
			Task<bool> task = this.coreReader.ReadAsync();
			this.lastTask = task;
			return task;
		}

		public override Task SkipAsync()
		{
			this.CheckAsync();
			Task task = this.coreReader.SkipAsync();
			this.lastTask = task;
			return task;
		}

		public override Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task<int> task = this.coreReader.ReadContentAsBase64Async(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task<int> task = this.coreReader.ReadElementContentAsBase64Async(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task<int> task = this.coreReader.ReadContentAsBinHexAsync(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task<int> task = this.coreReader.ReadElementContentAsBinHexAsync(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task<int> ReadValueChunkAsync(char[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task<int> task = this.coreReader.ReadValueChunkAsync(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task<XmlNodeType> MoveToContentAsync()
		{
			this.CheckAsync();
			Task<XmlNodeType> task = this.coreReader.MoveToContentAsync();
			this.lastTask = task;
			return task;
		}

		public override Task<string> ReadInnerXmlAsync()
		{
			this.CheckAsync();
			Task<string> task = this.coreReader.ReadInnerXmlAsync();
			this.lastTask = task;
			return task;
		}

		public override Task<string> ReadOuterXmlAsync()
		{
			this.CheckAsync();
			Task<string> task = this.coreReader.ReadOuterXmlAsync();
			this.lastTask = task;
			return task;
		}

		private readonly XmlReader coreReader;

		private Task lastTask = AsyncHelper.DoneTask;
	}
}
