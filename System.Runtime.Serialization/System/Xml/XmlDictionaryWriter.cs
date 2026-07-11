using System;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml
{
	public abstract class XmlDictionaryWriter : XmlWriter
	{
		internal virtual bool FastAsync
		{
			get
			{
				return false;
			}
		}

		internal virtual AsyncCompletionResult WriteBase64Async(AsyncEventArgs<XmlWriteBase64AsyncArguments> state)
		{
			throw FxTrace.Exception.AsError(new NotSupportedException());
		}

		public override Task WriteBase64Async(byte[] buffer, int index, int count)
		{
			return Task.Factory.FromAsync<byte[], int, int>(new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(this.BeginWriteBase64), new Action<IAsyncResult>(this.EndWriteBase64), buffer, index, count, null);
		}

		internal virtual IAsyncResult BeginWriteBase64(byte[] buffer, int index, int count, AsyncCallback callback, object state)
		{
			return new XmlDictionaryWriter.WriteBase64AsyncResult(buffer, index, count, this, callback, state);
		}

		internal virtual void EndWriteBase64(IAsyncResult result)
		{
			ScheduleActionItemAsyncResult.End(result);
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, null);
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary, null);
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary, session, true);
		}

		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
		{
			XmlBinaryWriter xmlBinaryWriter = new XmlBinaryWriter();
			xmlBinaryWriter.SetOutput(stream, dictionary, session, ownsStream);
			return xmlBinaryWriter;
		}

		public static XmlDictionaryWriter CreateTextWriter(Stream stream)
		{
			return XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, true);
		}

		public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding)
		{
			return XmlDictionaryWriter.CreateTextWriter(stream, encoding, true);
		}

		public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding, bool ownsStream)
		{
			XmlUTF8TextWriter xmlUTF8TextWriter = new XmlUTF8TextWriter();
			xmlUTF8TextWriter.SetOutput(stream, encoding, ownsStream);
			return xmlUTF8TextWriter;
		}

		public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo)
		{
			return XmlDictionaryWriter.CreateMtomWriter(stream, encoding, maxSizeInBytes, startInfo, null, null, true, true);
		}

		public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
		{
			XmlMtomWriter xmlMtomWriter = new XmlMtomWriter();
			xmlMtomWriter.SetOutput(stream, encoding, maxSizeInBytes, startInfo, boundary, startUri, writeMessageHeaders, ownsStream);
			return xmlMtomWriter;
		}

		public static XmlDictionaryWriter CreateDictionaryWriter(XmlWriter writer)
		{
			if (writer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
			}
			XmlDictionaryWriter xmlDictionaryWriter = writer as XmlDictionaryWriter;
			if (xmlDictionaryWriter == null)
			{
				xmlDictionaryWriter = new XmlDictionaryWriter.XmlWrappedWriter(writer);
			}
			return xmlDictionaryWriter;
		}

		public void WriteStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartElement(null, localName, namespaceUri);
		}

		public virtual void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartElement(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri));
		}

		public void WriteStartAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartAttribute(null, localName, namespaceUri);
		}

		public virtual void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartAttribute(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri));
		}

		public void WriteAttributeString(XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteAttributeString(null, localName, namespaceUri, value);
		}

		public virtual void WriteXmlnsAttribute(string prefix, string namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			if (prefix == null)
			{
				if (this.LookupPrefix(namespaceUri) != null)
				{
					return;
				}
				prefix = ((namespaceUri.Length == 0) ? string.Empty : ("d" + namespaceUri.Length.ToString(NumberFormatInfo.InvariantInfo)));
			}
			base.WriteAttributeString("xmlns", prefix, null, namespaceUri);
		}

		public virtual void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
		{
			this.WriteXmlnsAttribute(prefix, XmlDictionaryString.GetString(namespaceUri));
		}

		public virtual void WriteXmlAttribute(string localName, string value)
		{
			base.WriteAttributeString("xml", localName, null, value);
		}

		public virtual void WriteXmlAttribute(XmlDictionaryString localName, XmlDictionaryString value)
		{
			this.WriteXmlAttribute(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(value));
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

		public virtual void WriteString(XmlDictionaryString value)
		{
			this.WriteString(XmlDictionaryString.GetString(value));
		}

		public virtual void WriteQualifiedName(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (namespaceUri == null)
			{
				namespaceUri = XmlDictionaryString.Empty;
			}
			this.WriteQualifiedName(localName.Value, namespaceUri.Value);
		}

		public virtual void WriteValue(XmlDictionaryString value)
		{
			this.WriteValue(XmlDictionaryString.GetString(value));
		}

		public virtual void WriteValue(IStreamProvider value)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			Stream stream = value.GetStream();
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Stream returned by IStreamProvider cannot be null.")));
			}
			int num = 256;
			byte[] array = new byte[num];
			for (;;)
			{
				int num2 = stream.Read(array, 0, num);
				if (num2 <= 0)
				{
					break;
				}
				this.WriteBase64(array, 0, num2);
				if (num < 65536 && num2 == num)
				{
					num *= 16;
					array = new byte[num];
				}
			}
			value.ReleaseStream(stream);
		}

		public virtual Task WriteValueAsync(IStreamProvider value)
		{
			return Task.Factory.FromAsync<IStreamProvider>(new Func<IStreamProvider, AsyncCallback, object, IAsyncResult>(this.BeginWriteValue), new Action<IAsyncResult>(this.EndWriteValue), value, null);
		}

		internal virtual IAsyncResult BeginWriteValue(IStreamProvider value, AsyncCallback callback, object state)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (this.FastAsync)
			{
				return new XmlDictionaryWriter.WriteValueFastAsyncResult(this, value, callback, state);
			}
			return new XmlDictionaryWriter.WriteValueAsyncResult(this, value, callback, state);
		}

		internal virtual void EndWriteValue(IAsyncResult result)
		{
			if (this.FastAsync)
			{
				XmlDictionaryWriter.WriteValueFastAsyncResult.End(result);
				return;
			}
			XmlDictionaryWriter.WriteValueAsyncResult.End(result);
		}

		public virtual void WriteValue(UniqueId value)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			this.WriteString(value.ToString());
		}

		public virtual void WriteValue(Guid value)
		{
			this.WriteString(value.ToString());
		}

		public virtual void WriteValue(TimeSpan value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		public virtual bool CanCanonicalize
		{
			get
			{
				return false;
			}
		}

		public virtual void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		public virtual void EndCanonicalization()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		private void WriteElementNode(XmlDictionaryReader reader, bool defattr)
		{
			XmlDictionaryString xmlDictionaryString;
			XmlDictionaryString xmlDictionaryString2;
			if (reader.TryGetLocalNameAsDictionaryString(out xmlDictionaryString) && reader.TryGetNamespaceUriAsDictionaryString(out xmlDictionaryString2))
			{
				this.WriteStartElement(reader.Prefix, xmlDictionaryString, xmlDictionaryString2);
			}
			else
			{
				this.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			}
			if ((defattr || (!reader.IsDefault && (reader.SchemaInfo == null || !reader.SchemaInfo.IsDefault))) && reader.MoveToFirstAttribute())
			{
				do
				{
					if (reader.TryGetLocalNameAsDictionaryString(out xmlDictionaryString) && reader.TryGetNamespaceUriAsDictionaryString(out xmlDictionaryString2))
					{
						this.WriteStartAttribute(reader.Prefix, xmlDictionaryString, xmlDictionaryString2);
					}
					else
					{
						this.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
					}
					while (reader.ReadAttributeValue())
					{
						if (reader.NodeType == XmlNodeType.EntityReference)
						{
							this.WriteEntityRef(reader.Name);
						}
						else
						{
							this.WriteTextNode(reader, true);
						}
					}
					this.WriteEndAttribute();
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			if (reader.IsEmptyElement)
			{
				this.WriteEndElement();
			}
		}

		private void WriteArrayNode(XmlDictionaryReader reader, string prefix, string localName, string namespaceUri, Type type)
		{
			if (type == typeof(bool))
			{
				BooleanArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(short))
			{
				Int16ArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(int))
			{
				Int32ArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(long))
			{
				Int64ArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(float))
			{
				SingleArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(double))
			{
				DoubleArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(decimal))
			{
				DecimalArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(DateTime))
			{
				DateTimeArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(Guid))
			{
				GuidArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(TimeSpan))
			{
				TimeSpanArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			this.WriteElementNode(reader, false);
			reader.Read();
		}

		private void WriteArrayNode(XmlDictionaryReader reader, string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Type type)
		{
			if (type == typeof(bool))
			{
				BooleanArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(short))
			{
				Int16ArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(int))
			{
				Int32ArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(long))
			{
				Int64ArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(float))
			{
				SingleArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(double))
			{
				DoubleArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(decimal))
			{
				DecimalArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(DateTime))
			{
				DateTimeArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(Guid))
			{
				GuidArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(TimeSpan))
			{
				TimeSpanArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			this.WriteElementNode(reader, false);
			reader.Read();
		}

		private void WriteArrayNode(XmlDictionaryReader reader, Type type)
		{
			XmlDictionaryString xmlDictionaryString;
			XmlDictionaryString xmlDictionaryString2;
			if (reader.TryGetLocalNameAsDictionaryString(out xmlDictionaryString) && reader.TryGetNamespaceUriAsDictionaryString(out xmlDictionaryString2))
			{
				this.WriteArrayNode(reader, reader.Prefix, xmlDictionaryString, xmlDictionaryString2, type);
				return;
			}
			this.WriteArrayNode(reader, reader.Prefix, reader.LocalName, reader.NamespaceURI, type);
		}

		protected virtual void WriteTextNode(XmlDictionaryReader reader, bool isAttribute)
		{
			XmlDictionaryString xmlDictionaryString;
			if (reader.TryGetValueAsDictionaryString(out xmlDictionaryString))
			{
				this.WriteString(xmlDictionaryString);
			}
			else
			{
				this.WriteString(reader.Value);
			}
			if (!isAttribute)
			{
				reader.Read();
			}
		}

		public override void WriteNode(XmlReader reader, bool defattr)
		{
			XmlDictionaryReader xmlDictionaryReader = reader as XmlDictionaryReader;
			if (xmlDictionaryReader != null)
			{
				this.WriteNode(xmlDictionaryReader, defattr);
				return;
			}
			base.WriteNode(reader, defattr);
		}

		public virtual void WriteNode(XmlDictionaryReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("reader"));
			}
			int num = ((reader.NodeType == XmlNodeType.None) ? (-1) : reader.Depth);
			do
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					this.WriteTextNode(reader, false);
				}
				else
				{
					Type type;
					if (reader.Depth <= num || !reader.IsStartArray(out type))
					{
						switch (nodeType)
						{
						case XmlNodeType.Element:
							this.WriteElementNode(reader, defattr);
							break;
						case XmlNodeType.Attribute:
						case XmlNodeType.Text:
						case XmlNodeType.Entity:
						case XmlNodeType.Document:
							break;
						case XmlNodeType.CDATA:
							this.WriteCData(reader.Value);
							break;
						case XmlNodeType.EntityReference:
							this.WriteEntityRef(reader.Name);
							break;
						case XmlNodeType.ProcessingInstruction:
							goto IL_00C9;
						case XmlNodeType.Comment:
							this.WriteComment(reader.Value);
							break;
						case XmlNodeType.DocumentType:
							this.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
							break;
						default:
							if (nodeType != XmlNodeType.EndElement)
							{
								if (nodeType == XmlNodeType.XmlDeclaration)
								{
									goto IL_00C9;
								}
							}
							else
							{
								this.WriteFullEndElement();
							}
							break;
						}
						IL_011B:
						if (reader.Read())
						{
							goto IL_0123;
						}
						break;
						IL_00C9:
						this.WriteProcessingInstruction(reader.Name, reader.Value);
						goto IL_011B;
					}
					this.WriteArrayNode(reader, type);
				}
				IL_0123:;
			}
			while (num < reader.Depth || (num == reader.Depth && reader.NodeType == XmlNodeType.EndElement));
		}

		private void CheckArray(Array array, int offset, int count)
		{
			if (array == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("array"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > array.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { array.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > array.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { array.Length - offset })));
			}
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue((int)array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		private class WriteValueFastAsyncResult : AsyncResult
		{
			public WriteValueFastAsyncResult(XmlDictionaryWriter writer, IStreamProvider value, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.streamProvider = value;
				this.writer = writer;
				this.stream = value.GetStream();
				if (this.stream == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Stream returned by IStreamProvider cannot be null.")));
				}
				this.blockSize = 256;
				this.bytesRead = 0;
				this.block = new byte[this.blockSize];
				this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Read;
				this.ContinueWork(true, null);
			}

			private void CompleteAndReleaseStream(bool completedSynchronously, Exception completionException = null)
			{
				if (completionException == null)
				{
					this.streamProvider.ReleaseStream(this.stream);
					this.stream = null;
				}
				base.Complete(completedSynchronously, completionException);
			}

			private void ContinueWork(bool completedSynchronously, Exception completionException = null)
			{
				try
				{
					for (;;)
					{
						if (this.nextOperation == XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Read)
						{
							if (this.ReadAsync() != AsyncCompletionResult.Completed)
							{
								break;
							}
						}
						else if (this.nextOperation == XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Write)
						{
							if (this.WriteAsync() != AsyncCompletionResult.Completed)
							{
								break;
							}
						}
						else if (this.nextOperation == XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete)
						{
							goto Block_6;
						}
					}
					return;
					Block_6:;
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					if (completedSynchronously)
					{
						throw;
					}
					if (completionException == null)
					{
						completionException = ex;
					}
				}
				if (!this.completed)
				{
					this.completed = true;
					this.CompleteAndReleaseStream(completedSynchronously, completionException);
				}
			}

			private AsyncCompletionResult ReadAsync()
			{
				IAsyncResult asyncResult = this.stream.BeginRead(this.block, 0, this.blockSize, XmlDictionaryWriter.WriteValueFastAsyncResult.onReadComplete, this);
				if (asyncResult.CompletedSynchronously)
				{
					this.HandleReadComplete(asyncResult);
					return AsyncCompletionResult.Completed;
				}
				return AsyncCompletionResult.Queued;
			}

			private void HandleReadComplete(IAsyncResult result)
			{
				this.bytesRead = this.stream.EndRead(result);
				if (this.bytesRead > 0)
				{
					this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Write;
					return;
				}
				this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete;
			}

			private static void OnReadComplete(IAsyncResult result)
			{
				if (result.CompletedSynchronously)
				{
					return;
				}
				Exception ex = null;
				XmlDictionaryWriter.WriteValueFastAsyncResult writeValueFastAsyncResult = (XmlDictionaryWriter.WriteValueFastAsyncResult)result.AsyncState;
				bool flag = false;
				try
				{
					writeValueFastAsyncResult.HandleReadComplete(result);
					flag = true;
				}
				catch (Exception ex2)
				{
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
					ex = ex2;
				}
				if (!flag)
				{
					writeValueFastAsyncResult.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete;
				}
				writeValueFastAsyncResult.ContinueWork(false, ex);
			}

			private AsyncCompletionResult WriteAsync()
			{
				if (this.writerAsyncState == null)
				{
					this.writerAsyncArgs = new XmlWriteBase64AsyncArguments();
					this.writerAsyncState = new AsyncEventArgs<XmlWriteBase64AsyncArguments>();
				}
				if (XmlDictionaryWriter.WriteValueFastAsyncResult.onWriteComplete == null)
				{
					XmlDictionaryWriter.WriteValueFastAsyncResult.onWriteComplete = new AsyncEventArgsCallback(XmlDictionaryWriter.WriteValueFastAsyncResult.OnWriteComplete);
				}
				this.writerAsyncArgs.Buffer = this.block;
				this.writerAsyncArgs.Offset = 0;
				this.writerAsyncArgs.Count = this.bytesRead;
				this.writerAsyncState.Set(XmlDictionaryWriter.WriteValueFastAsyncResult.onWriteComplete, this.writerAsyncArgs, this);
				if (this.writer.WriteBase64Async(this.writerAsyncState) == AsyncCompletionResult.Completed)
				{
					this.HandleWriteComplete();
					this.writerAsyncState.Complete(true);
					return AsyncCompletionResult.Completed;
				}
				return AsyncCompletionResult.Queued;
			}

			private void HandleWriteComplete()
			{
				this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Read;
				if (this.blockSize < 65536 && this.bytesRead == this.blockSize)
				{
					this.blockSize *= 16;
					this.block = new byte[this.blockSize];
				}
			}

			private static void OnWriteComplete(IAsyncEventArgs asyncState)
			{
				XmlDictionaryWriter.WriteValueFastAsyncResult writeValueFastAsyncResult = (XmlDictionaryWriter.WriteValueFastAsyncResult)asyncState.AsyncState;
				Exception ex = null;
				bool flag = false;
				try
				{
					if (asyncState.Exception != null)
					{
						ex = asyncState.Exception;
					}
					else
					{
						writeValueFastAsyncResult.HandleWriteComplete();
						flag = true;
					}
				}
				catch (Exception ex2)
				{
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
					ex = ex2;
				}
				if (!flag)
				{
					writeValueFastAsyncResult.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete;
				}
				writeValueFastAsyncResult.ContinueWork(false, ex);
			}

			internal static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlDictionaryWriter.WriteValueFastAsyncResult>(result);
			}

			private bool completed;

			private int blockSize;

			private byte[] block;

			private int bytesRead;

			private Stream stream;

			private XmlDictionaryWriter.WriteValueFastAsyncResult.Operation nextOperation;

			private IStreamProvider streamProvider;

			private XmlDictionaryWriter writer;

			private AsyncEventArgs<XmlWriteBase64AsyncArguments> writerAsyncState;

			private XmlWriteBase64AsyncArguments writerAsyncArgs;

			private static AsyncCallback onReadComplete = Fx.ThunkCallback(new AsyncCallback(XmlDictionaryWriter.WriteValueFastAsyncResult.OnReadComplete));

			private static AsyncEventArgsCallback onWriteComplete;

			private enum Operation
			{
				Read,
				Write,
				Complete
			}
		}

		private class WriteValueAsyncResult : AsyncResult
		{
			public WriteValueAsyncResult(XmlDictionaryWriter writer, IStreamProvider value, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.streamProvider = value;
				this.writer = writer;
				this.writeBlockHandler = ((this.writer.Settings != null && this.writer.Settings.Async) ? XmlDictionaryWriter.WriteValueAsyncResult.handleWriteBlockAsync : XmlDictionaryWriter.WriteValueAsyncResult.handleWriteBlock);
				this.stream = value.GetStream();
				if (this.stream == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Stream returned by IStreamProvider cannot be null.")));
				}
				this.blockSize = 256;
				this.bytesRead = 0;
				this.block = new byte[this.blockSize];
				if (this.ContinueWork(null))
				{
					this.CompleteAndReleaseStream(true, null);
				}
			}

			private void AdjustBlockSize()
			{
				if (this.blockSize < 65536 && this.bytesRead == this.blockSize)
				{
					this.blockSize *= 16;
					this.block = new byte[this.blockSize];
				}
			}

			private void CompleteAndReleaseStream(bool completedSynchronously, Exception completionException)
			{
				if (completionException == null)
				{
					this.streamProvider.ReleaseStream(this.stream);
					this.stream = null;
				}
				base.Complete(completedSynchronously, completionException);
			}

			private bool ContinueWork(IAsyncResult result)
			{
				for (;;)
				{
					if (this.operation == XmlDictionaryWriter.WriteValueAsyncResult.Operation.Read)
					{
						if (!this.HandleReadBlock(result))
						{
							return false;
						}
						if (this.bytesRead <= 0)
						{
							break;
						}
						this.operation = XmlDictionaryWriter.WriteValueAsyncResult.Operation.Write;
					}
					else
					{
						if (!this.writeBlockHandler(result, this))
						{
							return false;
						}
						this.AdjustBlockSize();
						this.operation = XmlDictionaryWriter.WriteValueAsyncResult.Operation.Read;
					}
					result = null;
				}
				return true;
			}

			private bool HandleReadBlock(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.stream.BeginRead(this.block, 0, this.blockSize, XmlDictionaryWriter.WriteValueAsyncResult.onContinueWork, this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				this.bytesRead = this.stream.EndRead(result);
				return true;
			}

			private static bool HandleWriteBlock(IAsyncResult result, XmlDictionaryWriter.WriteValueAsyncResult thisPtr)
			{
				if (result == null)
				{
					result = thisPtr.writer.BeginWriteBase64(thisPtr.block, 0, thisPtr.bytesRead, XmlDictionaryWriter.WriteValueAsyncResult.onContinueWork, thisPtr);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				thisPtr.writer.EndWriteBase64(result);
				return true;
			}

			private static bool HandleWriteBlockAsync(IAsyncResult result, XmlDictionaryWriter.WriteValueAsyncResult thisPtr)
			{
				Task task = (Task)result;
				if (task == null)
				{
					task = thisPtr.writer.WriteBase64Async(thisPtr.block, 0, thisPtr.bytesRead);
					task.AsAsyncResult(XmlDictionaryWriter.WriteValueAsyncResult.onContinueWork, thisPtr);
					return false;
				}
				task.GetAwaiter().GetResult();
				return true;
			}

			private static void OnContinueWork(IAsyncResult result)
			{
				if (result.CompletedSynchronously && !(result is Task))
				{
					return;
				}
				Exception ex = null;
				XmlDictionaryWriter.WriteValueAsyncResult writeValueAsyncResult = (XmlDictionaryWriter.WriteValueAsyncResult)result.AsyncState;
				bool flag = false;
				try
				{
					flag = writeValueAsyncResult.ContinueWork(result);
				}
				catch (Exception ex2)
				{
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
					flag = true;
					ex = ex2;
				}
				if (flag)
				{
					writeValueAsyncResult.CompleteAndReleaseStream(false, ex);
				}
			}

			public static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlDictionaryWriter.WriteValueAsyncResult>(result);
			}

			private int blockSize;

			private byte[] block;

			private int bytesRead;

			private Stream stream;

			private XmlDictionaryWriter.WriteValueAsyncResult.Operation operation;

			private IStreamProvider streamProvider;

			private XmlDictionaryWriter writer;

			private Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool> writeBlockHandler;

			private static Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool> handleWriteBlock = new Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool>(XmlDictionaryWriter.WriteValueAsyncResult.HandleWriteBlock);

			private static Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool> handleWriteBlockAsync = new Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool>(XmlDictionaryWriter.WriteValueAsyncResult.HandleWriteBlockAsync);

			private static AsyncCallback onContinueWork = Fx.ThunkCallback(new AsyncCallback(XmlDictionaryWriter.WriteValueAsyncResult.OnContinueWork));

			private enum Operation
			{
				Read,
				Write
			}
		}

		private class WriteBase64AsyncResult : ScheduleActionItemAsyncResult
		{
			public WriteBase64AsyncResult(byte[] buffer, int index, int count, XmlDictionaryWriter writer, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.buffer = buffer;
				this.index = index;
				this.count = count;
				this.writer = writer;
				base.Schedule();
			}

			protected override void OnDoWork()
			{
				this.writer.WriteBase64(this.buffer, this.index, this.count);
			}

			private byte[] buffer;

			private int index;

			private int count;

			private XmlDictionaryWriter writer;
		}

		private class XmlWrappedWriter : XmlDictionaryWriter
		{
			public XmlWrappedWriter(XmlWriter writer)
			{
				this.writer = writer;
				this.depth = 0;
			}

			public override void Close()
			{
				this.writer.Close();
			}

			public override void Flush()
			{
				this.writer.Flush();
			}

			public override string LookupPrefix(string namespaceUri)
			{
				return this.writer.LookupPrefix(namespaceUri);
			}

			public override void WriteAttributes(XmlReader reader, bool defattr)
			{
				this.writer.WriteAttributes(reader, defattr);
			}

			public override void WriteBase64(byte[] buffer, int index, int count)
			{
				this.writer.WriteBase64(buffer, index, count);
			}

			public override void WriteBinHex(byte[] buffer, int index, int count)
			{
				this.writer.WriteBinHex(buffer, index, count);
			}

			public override void WriteCData(string text)
			{
				this.writer.WriteCData(text);
			}

			public override void WriteCharEntity(char ch)
			{
				this.writer.WriteCharEntity(ch);
			}

			public override void WriteChars(char[] buffer, int index, int count)
			{
				this.writer.WriteChars(buffer, index, count);
			}

			public override void WriteComment(string text)
			{
				this.writer.WriteComment(text);
			}

			public override void WriteDocType(string name, string pubid, string sysid, string subset)
			{
				this.writer.WriteDocType(name, pubid, sysid, subset);
			}

			public override void WriteEndAttribute()
			{
				this.writer.WriteEndAttribute();
			}

			public override void WriteEndDocument()
			{
				this.writer.WriteEndDocument();
			}

			public override void WriteEndElement()
			{
				this.writer.WriteEndElement();
				this.depth--;
			}

			public override void WriteEntityRef(string name)
			{
				this.writer.WriteEntityRef(name);
			}

			public override void WriteFullEndElement()
			{
				this.writer.WriteFullEndElement();
			}

			public override void WriteName(string name)
			{
				this.writer.WriteName(name);
			}

			public override void WriteNmToken(string name)
			{
				this.writer.WriteNmToken(name);
			}

			public override void WriteNode(XmlReader reader, bool defattr)
			{
				this.writer.WriteNode(reader, defattr);
			}

			public override void WriteProcessingInstruction(string name, string text)
			{
				this.writer.WriteProcessingInstruction(name, text);
			}

			public override void WriteQualifiedName(string localName, string namespaceUri)
			{
				this.writer.WriteQualifiedName(localName, namespaceUri);
			}

			public override void WriteRaw(char[] buffer, int index, int count)
			{
				this.writer.WriteRaw(buffer, index, count);
			}

			public override void WriteRaw(string data)
			{
				this.writer.WriteRaw(data);
			}

			public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
			{
				this.writer.WriteStartAttribute(prefix, localName, namespaceUri);
				this.prefix++;
			}

			public override void WriteStartDocument()
			{
				this.writer.WriteStartDocument();
			}

			public override void WriteStartDocument(bool standalone)
			{
				this.writer.WriteStartDocument(standalone);
			}

			public override void WriteStartElement(string prefix, string localName, string namespaceUri)
			{
				this.writer.WriteStartElement(prefix, localName, namespaceUri);
				this.depth++;
				this.prefix = 1;
			}

			public override WriteState WriteState
			{
				get
				{
					return this.writer.WriteState;
				}
			}

			public override void WriteString(string text)
			{
				this.writer.WriteString(text);
			}

			public override void WriteSurrogateCharEntity(char lowChar, char highChar)
			{
				this.writer.WriteSurrogateCharEntity(lowChar, highChar);
			}

			public override void WriteWhitespace(string whitespace)
			{
				this.writer.WriteWhitespace(whitespace);
			}

			public override void WriteValue(object value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(string value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(bool value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(DateTime value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(double value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(int value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(long value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteXmlnsAttribute(string prefix, string namespaceUri)
			{
				if (namespaceUri == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
				}
				if (prefix == null)
				{
					if (this.LookupPrefix(namespaceUri) != null)
					{
						return;
					}
					if (namespaceUri.Length == 0)
					{
						prefix = string.Empty;
					}
					else
					{
						string text = this.depth.ToString(NumberFormatInfo.InvariantInfo);
						string text2 = this.prefix.ToString(NumberFormatInfo.InvariantInfo);
						prefix = "d" + text + "p" + text2;
					}
				}
				base.WriteAttributeString("xmlns", prefix, null, namespaceUri);
			}

			public override string XmlLang
			{
				get
				{
					return this.writer.XmlLang;
				}
			}

			public override XmlSpace XmlSpace
			{
				get
				{
					return this.writer.XmlSpace;
				}
			}

			private XmlWriter writer;

			private int depth;

			private int prefix;
		}
	}
}
