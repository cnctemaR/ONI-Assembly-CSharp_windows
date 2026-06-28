using System;
using System.IO;
using System.Text;

namespace System.Xml
{
	public abstract class XmlDictionaryWriter : XmlWriter
	{
		internal int Depth
		{
			get
			{
				return this.depth;
			}
			set
			{
				this.depth = value;
			}
		}

		public virtual bool CanCanonicalize
		{
			get
			{
				return false;
			}
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, null, null, false);
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary, null, false);
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary, session, false);
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
		{
			return new XmlBinaryDictionaryWriter(stream, dictionary, session, ownsStream);
		}

		public static XmlDictionaryWriter CreateDictionaryWriter(XmlWriter writer)
		{
			return new XmlSimpleDictionaryWriter(writer);
		}

		public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo)
		{
			return XmlDictionaryWriter.CreateMtomWriter(stream, encoding, maxSizeInBytes, startInfo, Guid.NewGuid() + "id=1", "http://tempuri.org/0/" + DateTime.Now.Ticks, true, false);
		}

		public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
		{
			return new XmlMtomDictionaryWriter(stream, encoding, maxSizeInBytes, startInfo, boundary, startUri, writeMessageHeaders, ownsStream);
		}

		public static XmlDictionaryWriter CreateTextWriter(Stream stream)
		{
			return XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8);
		}

		public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding)
		{
			return XmlDictionaryWriter.CreateTextWriter(stream, encoding, false);
		}

		public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding, bool ownsStream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			int codePage = encoding.CodePage;
			if (codePage != 1200 && codePage != 1201 && codePage != 65001)
			{
				throw new XmlException(string.Format("XML declaration is required for encoding code page {0} but this XmlWriter does not support XML declaration.", encoding.CodePage));
			}
			encoding = XmlDictionaryWriter.utf8_unmarked;
			return XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(stream, new XmlWriterSettings
			{
				Encoding = encoding,
				CloseOutput = ownsStream,
				OmitXmlDeclaration = true
			}));
		}

		public virtual void EndCanonicalization()
		{
			throw new NotSupportedException();
		}

		public virtual void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			throw new NotSupportedException();
		}

		public void WriteAttributeString(XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteAttributeString(null, localName, namespaceUri, value);
		}

		public void WriteAttributeString(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteStartAttribute(prefix, localName, namespaceUri);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		public void WriteElementString(XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteElementString(null, localName, namespaceUri, value);
		}

		public void WriteElementString(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteString(value);
			this.WriteEndElement();
		}

		public virtual void WriteNode(XmlDictionaryReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
			{
				XmlDictionaryString xmlDictionaryString;
				XmlDictionaryString xmlDictionaryString2;
				if (reader.TryGetLocalNameAsDictionaryString(out xmlDictionaryString) && reader.TryGetLocalNameAsDictionaryString(out xmlDictionaryString2))
				{
					this.WriteStartElement(reader.Prefix, xmlDictionaryString, xmlDictionaryString2);
				}
				else
				{
					this.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
				}
				if (reader.HasAttributes)
				{
					for (int i = 0; i < reader.AttributeCount; i++)
					{
						reader.MoveToAttribute(i);
						this.WriteAttribute(reader, defattr);
					}
					reader.MoveToElement();
				}
				if (reader.IsEmptyElement)
				{
					this.WriteEndElement();
				}
				else
				{
					int num = reader.Depth;
					reader.Read();
					if (reader.NodeType != XmlNodeType.EndElement)
					{
						do
						{
							this.WriteNode(reader, defattr);
						}
						while (num < reader.Depth);
					}
					this.WriteFullEndElement();
				}
				reader.Read();
				break;
			}
			case XmlNodeType.Attribute:
			case XmlNodeType.Text:
				this.WriteTextNode(reader, defattr);
				break;
			default:
				base.WriteNode(reader, defattr);
				break;
			}
		}

		private void WriteAttribute(XmlDictionaryReader reader, bool defattr)
		{
			if (!defattr && reader.IsDefault)
			{
				return;
			}
			XmlDictionaryString xmlDictionaryString;
			XmlDictionaryString xmlDictionaryString2;
			if (reader.TryGetLocalNameAsDictionaryString(out xmlDictionaryString) && reader.TryGetLocalNameAsDictionaryString(out xmlDictionaryString2))
			{
				this.WriteStartAttribute(reader.Prefix, xmlDictionaryString, xmlDictionaryString2);
			}
			else
			{
				this.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			}
			while (reader.ReadAttributeValue())
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Text:
					this.WriteTextNode(reader, true);
					break;
				case XmlNodeType.EntityReference:
					this.WriteEntityRef(reader.Name);
					break;
				}
			}
			this.WriteEndAttribute();
		}

		public override void WriteNode(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			XmlDictionaryReader xmlDictionaryReader = reader as XmlDictionaryReader;
			if (xmlDictionaryReader != null)
			{
				this.WriteNode(xmlDictionaryReader, defattr);
			}
			else
			{
				base.WriteNode(reader, defattr);
			}
		}

		public virtual void WriteQualifiedName(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteQualifiedName(localName.Value, namespaceUri.Value);
		}

		public void WriteStartAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			base.WriteStartAttribute(localName.Value, namespaceUri.Value);
		}

		public virtual void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartAttribute(prefix, localName.Value, namespaceUri.Value);
		}

		public void WriteStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartElement(null, localName, namespaceUri);
		}

		public virtual void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentException("localName must not be null.", "localName");
			}
			this.WriteStartElement(prefix, localName.Value, (namespaceUri == null) ? null : namespaceUri.Value);
		}

		public virtual void WriteString(XmlDictionaryString value)
		{
			this.WriteString(value.Value);
		}

		protected virtual void WriteTextNode(XmlDictionaryReader reader, bool isAttribute)
		{
			this.WriteString(reader.Value);
			if (!isAttribute)
			{
				reader.Read();
			}
		}

		public virtual void WriteValue(Guid guid)
		{
			this.WriteString(guid.ToString());
		}

		public virtual void WriteValue(IStreamProvider value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Stream stream = value.GetStream();
			byte[] array = new byte[Math.Min(2048L, (!stream.CanSeek) ? 2048L : stream.Length)];
			int num;
			while ((num = stream.Read(array, 0, array.Length)) > 0)
			{
				this.WriteBase64(array, 0, num);
			}
			value.ReleaseStream(stream);
		}

		public virtual void WriteValue(TimeSpan duration)
		{
			this.WriteString(XmlConvert.ToString(duration));
		}

		public virtual void WriteValue(UniqueId id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			this.WriteString(id.ToString());
		}

		public virtual void WriteValue(XmlDictionaryString value)
		{
			this.WriteValue(value.Value);
		}

		public virtual void WriteXmlAttribute(string localName, string value)
		{
			base.WriteAttributeString("xml", localName, "http://www.w3.org/XML/1998/namespace", value);
		}

		public virtual void WriteXmlAttribute(XmlDictionaryString localName, XmlDictionaryString value)
		{
			this.WriteXmlAttribute(localName.Value, value.Value);
		}

		public virtual void WriteXmlnsAttribute(string prefix, string namespaceUri)
		{
			if (prefix == null)
			{
				prefix = "d" + this.Depth + "p1";
			}
			if (prefix == string.Empty)
			{
				base.WriteAttributeString("xmlns", namespaceUri);
			}
			else
			{
				base.WriteAttributeString("xmlns", prefix, "http://www.w3.org/2000/xmlns/", namespaceUri);
			}
		}

		public virtual void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
		{
			this.WriteXmlnsAttribute(prefix, namespaceUri.Value);
		}

		private void CheckWriteArrayArguments(Array array, int offset, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset is negative");
			}
			if (offset > array.Length)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the length of the destination array");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length is negative");
			}
			if (length > array.Length - offset)
			{
				throw new ArgumentOutOfRangeException("length + offset exceeds the length of the destination array");
			}
		}

		private void CheckDictionaryStringArgs(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue((int)array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			this.WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int length)
		{
			this.CheckWriteArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		private static readonly Encoding utf8_unmarked = new UTF8Encoding(false);

		private int depth;
	}
}
