using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	public abstract class XmlDictionaryReader : XmlReader
	{
		public static XmlDictionaryReader CreateDictionaryReader(XmlReader reader)
		{
			if (reader == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
			}
			XmlDictionaryReader xmlDictionaryReader = reader as XmlDictionaryReader;
			if (xmlDictionaryReader == null)
			{
				xmlDictionaryReader = new XmlDictionaryReader.XmlWrappedReader(reader, null);
			}
			return xmlDictionaryReader;
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			return XmlDictionaryReader.CreateBinaryReader(buffer, 0, buffer.Length, quotas);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(buffer, offset, count, null, quotas);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(buffer, offset, count, dictionary, quotas, null);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session)
		{
			return XmlDictionaryReader.CreateBinaryReader(buffer, offset, count, dictionary, quotas, session, null);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			XmlBinaryReader xmlBinaryReader = new XmlBinaryReader();
			xmlBinaryReader.SetInput(buffer, offset, count, dictionary, quotas, session, onClose);
			return xmlBinaryReader;
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(stream, null, quotas);
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(stream, dictionary, quotas, null);
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session)
		{
			return XmlDictionaryReader.CreateBinaryReader(stream, dictionary, quotas, session, null);
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			XmlBinaryReader xmlBinaryReader = new XmlBinaryReader();
			xmlBinaryReader.SetInput(stream, dictionary, quotas, session, onClose);
			return xmlBinaryReader;
		}

		public static XmlDictionaryReader CreateTextReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			return XmlDictionaryReader.CreateTextReader(buffer, 0, buffer.Length, quotas);
		}

		public static XmlDictionaryReader CreateTextReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateTextReader(buffer, offset, count, null, quotas, null);
		}

		public static XmlDictionaryReader CreateTextReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			XmlUTF8TextReader xmlUTF8TextReader = new XmlUTF8TextReader();
			xmlUTF8TextReader.SetInput(buffer, offset, count, encoding, quotas, onClose);
			return xmlUTF8TextReader;
		}

		public static XmlDictionaryReader CreateTextReader(Stream stream, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateTextReader(stream, null, quotas, null);
		}

		public static XmlDictionaryReader CreateTextReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			XmlUTF8TextReader xmlUTF8TextReader = new XmlUTF8TextReader();
			xmlUTF8TextReader.SetInput(stream, encoding, quotas, onClose);
			return xmlUTF8TextReader;
		}

		public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas)
		{
			if (encoding == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
			}
			return XmlDictionaryReader.CreateMtomReader(stream, new Encoding[] { encoding }, quotas);
		}

		public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding[] encodings, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateMtomReader(stream, encodings, null, quotas);
		}

		public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateMtomReader(stream, encodings, contentType, quotas, int.MaxValue, null);
		}

		public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
		{
			XmlMtomReader xmlMtomReader = new XmlMtomReader();
			xmlMtomReader.SetInput(stream, encodings, contentType, quotas, maxBufferSize, onClose);
			return xmlMtomReader;
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas)
		{
			if (encoding == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
			}
			return XmlDictionaryReader.CreateMtomReader(buffer, offset, count, new Encoding[] { encoding }, quotas);
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateMtomReader(buffer, offset, count, encodings, null, quotas);
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateMtomReader(buffer, offset, count, encodings, contentType, quotas, int.MaxValue, null);
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
		{
			XmlMtomReader xmlMtomReader = new XmlMtomReader();
			xmlMtomReader.SetInput(buffer, offset, count, encodings, contentType, quotas, maxBufferSize, onClose);
			return xmlMtomReader;
		}

		public virtual bool CanCanonicalize
		{
			get
			{
				return false;
			}
		}

		public virtual XmlDictionaryReaderQuotas Quotas
		{
			get
			{
				return XmlDictionaryReaderQuotas.Max;
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

		public virtual void MoveToStartElement()
		{
			if (!this.IsStartElement())
			{
				XmlExceptionHelper.ThrowStartElementExpected(this);
			}
		}

		public virtual void MoveToStartElement(string name)
		{
			if (!this.IsStartElement(name))
			{
				XmlExceptionHelper.ThrowStartElementExpected(this, name);
			}
		}

		public virtual void MoveToStartElement(string localName, string namespaceUri)
		{
			if (!this.IsStartElement(localName, namespaceUri))
			{
				XmlExceptionHelper.ThrowStartElementExpected(this, localName, namespaceUri);
			}
		}

		public virtual void MoveToStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (!this.IsStartElement(localName, namespaceUri))
			{
				XmlExceptionHelper.ThrowStartElementExpected(this, localName, namespaceUri);
			}
		}

		public virtual bool IsLocalName(string localName)
		{
			return this.LocalName == localName;
		}

		public virtual bool IsLocalName(XmlDictionaryString localName)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localName");
			}
			return this.IsLocalName(localName.Value);
		}

		public virtual bool IsNamespaceUri(string namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			return this.NamespaceURI == namespaceUri;
		}

		public virtual bool IsNamespaceUri(XmlDictionaryString namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			return this.IsNamespaceUri(namespaceUri.Value);
		}

		public virtual void ReadFullStartElement()
		{
			this.MoveToStartElement();
			if (this.IsEmptyElement)
			{
				XmlExceptionHelper.ThrowFullStartElementExpected(this);
			}
			this.Read();
		}

		public virtual void ReadFullStartElement(string name)
		{
			this.MoveToStartElement(name);
			if (this.IsEmptyElement)
			{
				XmlExceptionHelper.ThrowFullStartElementExpected(this, name);
			}
			this.Read();
		}

		public virtual void ReadFullStartElement(string localName, string namespaceUri)
		{
			this.MoveToStartElement(localName, namespaceUri);
			if (this.IsEmptyElement)
			{
				XmlExceptionHelper.ThrowFullStartElementExpected(this, localName, namespaceUri);
			}
			this.Read();
		}

		public virtual void ReadFullStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.MoveToStartElement(localName, namespaceUri);
			if (this.IsEmptyElement)
			{
				XmlExceptionHelper.ThrowFullStartElementExpected(this, localName, namespaceUri);
			}
			this.Read();
		}

		public virtual void ReadStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.MoveToStartElement(localName, namespaceUri);
			this.Read();
		}

		public virtual bool IsStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return this.IsStartElement(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri));
		}

		public virtual int IndexOfLocalName(string[] localNames, string namespaceUri)
		{
			if (localNames == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localNames");
			}
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			if (this.NamespaceURI == namespaceUri)
			{
				string localName = this.LocalName;
				for (int i = 0; i < localNames.Length; i++)
				{
					string text = localNames[i];
					if (text == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "localNames[{0}]", i));
					}
					if (localName == text)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public virtual int IndexOfLocalName(XmlDictionaryString[] localNames, XmlDictionaryString namespaceUri)
		{
			if (localNames == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localNames");
			}
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			if (this.NamespaceURI == namespaceUri.Value)
			{
				string localName = this.LocalName;
				for (int i = 0; i < localNames.Length; i++)
				{
					XmlDictionaryString xmlDictionaryString = localNames[i];
					if (xmlDictionaryString == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "localNames[{0}]", i));
					}
					if (localName == xmlDictionaryString.Value)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public virtual string GetAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return this.GetAttribute(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri));
		}

		public virtual bool TryGetBase64ContentLength(out int length)
		{
			length = 0;
			return false;
		}

		public virtual int ReadValueAsBase64(byte[] buffer, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		public virtual byte[] ReadContentAsBase64()
		{
			return this.ReadContentAsBase64(this.Quotas.MaxArrayLength, 65535);
		}

		internal byte[] ReadContentAsBase64(int maxByteArrayContentLength, int maxInitialCount)
		{
			int num;
			if (this.TryGetBase64ContentLength(out num))
			{
				if (num > maxByteArrayContentLength)
				{
					XmlExceptionHelper.ThrowMaxArrayLengthExceeded(this, maxByteArrayContentLength);
				}
				if (num <= maxInitialCount)
				{
					byte[] array = new byte[num];
					int num2;
					for (int i = 0; i < num; i += num2)
					{
						num2 = this.ReadContentAsBase64(array, i, num - i);
						if (num2 == 0)
						{
							XmlExceptionHelper.ThrowBase64DataExpected(this);
						}
					}
					return array;
				}
			}
			return this.ReadContentAsBytes(true, maxByteArrayContentLength);
		}

		public override string ReadContentAsString()
		{
			return this.ReadContentAsString(this.Quotas.MaxStringContentLength);
		}

		protected string ReadContentAsString(int maxStringContentLength)
		{
			StringBuilder stringBuilder = null;
			string text = string.Empty;
			bool flag = false;
			for (;;)
			{
				switch (this.NodeType)
				{
				case XmlNodeType.Element:
				case XmlNodeType.Entity:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentType:
				case XmlNodeType.DocumentFragment:
				case XmlNodeType.Notation:
				case XmlNodeType.EndElement:
					goto IL_00B4;
				case XmlNodeType.Attribute:
					text = this.Value;
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
				{
					string value = this.Value;
					if (text.Length == 0)
					{
						text = value;
					}
					else
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(text);
						}
						if (stringBuilder.Length > maxStringContentLength - value.Length)
						{
							XmlExceptionHelper.ThrowMaxStringContentLengthExceeded(this, maxStringContentLength);
						}
						stringBuilder.Append(value);
					}
					break;
				}
				case XmlNodeType.EntityReference:
					if (!this.CanResolveEntity)
					{
						goto IL_00B4;
					}
					this.ResolveEntity();
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.EndEntity:
					break;
				default:
					goto IL_00B4;
				}
				IL_00B6:
				if (flag)
				{
					break;
				}
				if (this.AttributeCount != 0)
				{
					this.ReadAttributeValue();
					continue;
				}
				this.Read();
				continue;
				IL_00B4:
				flag = true;
				goto IL_00B6;
			}
			if (stringBuilder != null)
			{
				text = stringBuilder.ToString();
			}
			if (text.Length > maxStringContentLength)
			{
				XmlExceptionHelper.ThrowMaxStringContentLengthExceeded(this, maxStringContentLength);
			}
			return text;
		}

		public override string ReadString()
		{
			return this.ReadString(this.Quotas.MaxStringContentLength);
		}

		protected string ReadString(int maxStringContentLength)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return string.Empty;
			}
			if (this.NodeType != XmlNodeType.Element)
			{
				this.MoveToElement();
			}
			if (this.NodeType == XmlNodeType.Element)
			{
				if (this.IsEmptyElement)
				{
					return string.Empty;
				}
				if (!this.Read())
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The reader cannot be advanced.")));
				}
				if (this.NodeType == XmlNodeType.EndElement)
				{
					return string.Empty;
				}
			}
			StringBuilder stringBuilder = null;
			string text = string.Empty;
			while (this.IsTextNode(this.NodeType))
			{
				string value = this.Value;
				if (text.Length == 0)
				{
					text = value;
				}
				else
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(text);
					}
					if (stringBuilder.Length > maxStringContentLength - value.Length)
					{
						XmlExceptionHelper.ThrowMaxStringContentLengthExceeded(this, maxStringContentLength);
					}
					stringBuilder.Append(value);
				}
				if (!this.Read())
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The reader cannot be advanced.")));
				}
			}
			if (stringBuilder != null)
			{
				text = stringBuilder.ToString();
			}
			if (text.Length > maxStringContentLength)
			{
				XmlExceptionHelper.ThrowMaxStringContentLengthExceeded(this, maxStringContentLength);
			}
			return text;
		}

		public virtual byte[] ReadContentAsBinHex()
		{
			return this.ReadContentAsBinHex(this.Quotas.MaxArrayLength);
		}

		protected byte[] ReadContentAsBinHex(int maxByteArrayContentLength)
		{
			return this.ReadContentAsBytes(false, maxByteArrayContentLength);
		}

		private byte[] ReadContentAsBytes(bool base64, int maxByteArrayContentLength)
		{
			byte[][] array = new byte[32][];
			int num = 384;
			int num2 = 0;
			int num3 = 0;
			byte[] array2;
			for (;;)
			{
				array2 = new byte[num];
				array[num2++] = array2;
				int i;
				int num4;
				for (i = 0; i < array2.Length; i += num4)
				{
					if (base64)
					{
						num4 = this.ReadContentAsBase64(array2, i, array2.Length - i);
					}
					else
					{
						num4 = this.ReadContentAsBinHex(array2, i, array2.Length - i);
					}
					if (num4 == 0)
					{
						break;
					}
				}
				if (num3 > maxByteArrayContentLength - i)
				{
					XmlExceptionHelper.ThrowMaxArrayLengthExceeded(this, maxByteArrayContentLength);
				}
				num3 += i;
				if (i < array2.Length)
				{
					break;
				}
				num *= 2;
			}
			array2 = new byte[num3];
			int num5 = 0;
			for (int j = 0; j < num2 - 1; j++)
			{
				Buffer.BlockCopy(array[j], 0, array2, num5, array[j].Length);
				num5 += array[j].Length;
			}
			Buffer.BlockCopy(array[num2 - 1], 0, array2, num5, num3 - num5);
			return array2;
		}

		protected bool IsTextNode(XmlNodeType nodeType)
		{
			return nodeType == XmlNodeType.Text || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Attribute;
		}

		public virtual int ReadContentAsChars(char[] chars, int offset, int count)
		{
			int num = 0;
			for (;;)
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (this.IsTextNode(nodeType))
				{
					num = this.ReadValueChunk(chars, offset, count);
					if (num > 0 || nodeType == XmlNodeType.Attribute)
					{
						break;
					}
					if (!this.Read())
					{
						break;
					}
				}
				else if (!this.Read())
				{
					break;
				}
			}
			return num;
		}

		public override object ReadContentAs(Type type, IXmlNamespaceResolver namespaceResolver)
		{
			if (type == typeof(Guid[]))
			{
				string[] array = (string[])this.ReadContentAs(typeof(string[]), namespaceResolver);
				Guid[] array2 = new Guid[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = XmlConverter.ToGuid(array[i]);
				}
				return array2;
			}
			if (type == typeof(UniqueId[]))
			{
				string[] array3 = (string[])this.ReadContentAs(typeof(string[]), namespaceResolver);
				UniqueId[] array4 = new UniqueId[array3.Length];
				for (int j = 0; j < array3.Length; j++)
				{
					array4[j] = XmlConverter.ToUniqueId(array3[j]);
				}
				return array4;
			}
			return base.ReadContentAs(type, namespaceResolver);
		}

		public virtual string ReadContentAsString(string[] strings, out int index)
		{
			if (strings == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("strings");
			}
			string text = this.ReadContentAsString();
			index = -1;
			for (int i = 0; i < strings.Length; i++)
			{
				string text2 = strings[i];
				if (text2 == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "strings[{0}]", i));
				}
				if (text2 == text)
				{
					index = i;
					return text2;
				}
			}
			return text;
		}

		public virtual string ReadContentAsString(XmlDictionaryString[] strings, out int index)
		{
			if (strings == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("strings");
			}
			string text = this.ReadContentAsString();
			index = -1;
			for (int i = 0; i < strings.Length; i++)
			{
				XmlDictionaryString xmlDictionaryString = strings[i];
				if (xmlDictionaryString == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "strings[{0}]", i));
				}
				if (xmlDictionaryString.Value == text)
				{
					index = i;
					return xmlDictionaryString.Value;
				}
			}
			return text;
		}

		public override decimal ReadContentAsDecimal()
		{
			return XmlConverter.ToDecimal(this.ReadContentAsString());
		}

		public override float ReadContentAsFloat()
		{
			return XmlConverter.ToSingle(this.ReadContentAsString());
		}

		public virtual UniqueId ReadContentAsUniqueId()
		{
			return XmlConverter.ToUniqueId(this.ReadContentAsString());
		}

		public virtual Guid ReadContentAsGuid()
		{
			return XmlConverter.ToGuid(this.ReadContentAsString());
		}

		public virtual TimeSpan ReadContentAsTimeSpan()
		{
			return XmlConverter.ToTimeSpan(this.ReadContentAsString());
		}

		public virtual void ReadContentAsQualifiedName(out string localName, out string namespaceUri)
		{
			string text;
			XmlConverter.ToQualifiedName(this.ReadContentAsString(), out text, out localName);
			namespaceUri = this.LookupNamespace(text);
			if (namespaceUri == null)
			{
				XmlExceptionHelper.ThrowUndefinedPrefix(this, text);
			}
		}

		public override string ReadElementContentAsString()
		{
			string text;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				text = string.Empty;
			}
			else
			{
				this.ReadStartElement();
				text = this.ReadContentAsString();
				this.ReadEndElement();
			}
			return text;
		}

		public override bool ReadElementContentAsBoolean()
		{
			bool flag;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				flag = XmlConverter.ToBoolean(string.Empty);
			}
			else
			{
				this.ReadStartElement();
				flag = this.ReadContentAsBoolean();
				this.ReadEndElement();
			}
			return flag;
		}

		public override int ReadElementContentAsInt()
		{
			int num;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				num = XmlConverter.ToInt32(string.Empty);
			}
			else
			{
				this.ReadStartElement();
				num = this.ReadContentAsInt();
				this.ReadEndElement();
			}
			return num;
		}

		public override long ReadElementContentAsLong()
		{
			long num;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				num = XmlConverter.ToInt64(string.Empty);
			}
			else
			{
				this.ReadStartElement();
				num = this.ReadContentAsLong();
				this.ReadEndElement();
			}
			return num;
		}

		public override float ReadElementContentAsFloat()
		{
			float num;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				num = XmlConverter.ToSingle(string.Empty);
			}
			else
			{
				this.ReadStartElement();
				num = this.ReadContentAsFloat();
				this.ReadEndElement();
			}
			return num;
		}

		public override double ReadElementContentAsDouble()
		{
			double num;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				num = XmlConverter.ToDouble(string.Empty);
			}
			else
			{
				this.ReadStartElement();
				num = this.ReadContentAsDouble();
				this.ReadEndElement();
			}
			return num;
		}

		public override decimal ReadElementContentAsDecimal()
		{
			decimal num;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				num = XmlConverter.ToDecimal(string.Empty);
			}
			else
			{
				this.ReadStartElement();
				num = this.ReadContentAsDecimal();
				this.ReadEndElement();
			}
			return num;
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				try
				{
					return DateTime.Parse(string.Empty, NumberFormatInfo.InvariantInfo);
				}
				catch (ArgumentException ex)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(string.Empty, "DateTime", ex));
				}
				catch (FormatException ex2)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(string.Empty, "DateTime", ex2));
				}
			}
			this.ReadStartElement();
			DateTime dateTime = this.ReadContentAsDateTime();
			this.ReadEndElement();
			return dateTime;
		}

		public virtual UniqueId ReadElementContentAsUniqueId()
		{
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				try
				{
					return new UniqueId(string.Empty);
				}
				catch (ArgumentException ex)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(string.Empty, "UniqueId", ex));
				}
				catch (FormatException ex2)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(string.Empty, "UniqueId", ex2));
				}
			}
			this.ReadStartElement();
			UniqueId uniqueId = this.ReadContentAsUniqueId();
			this.ReadEndElement();
			return uniqueId;
		}

		public virtual Guid ReadElementContentAsGuid()
		{
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				try
				{
					return Guid.Empty;
				}
				catch (ArgumentException ex)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(string.Empty, "Guid", ex));
				}
				catch (FormatException ex2)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(string.Empty, "Guid", ex2));
				}
				catch (OverflowException ex3)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(string.Empty, "Guid", ex3));
				}
			}
			this.ReadStartElement();
			Guid guid = this.ReadContentAsGuid();
			this.ReadEndElement();
			return guid;
		}

		public virtual TimeSpan ReadElementContentAsTimeSpan()
		{
			TimeSpan timeSpan;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				timeSpan = XmlConverter.ToTimeSpan(string.Empty);
			}
			else
			{
				this.ReadStartElement();
				timeSpan = this.ReadContentAsTimeSpan();
				this.ReadEndElement();
			}
			return timeSpan;
		}

		public virtual byte[] ReadElementContentAsBase64()
		{
			byte[] array;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				array = new byte[0];
			}
			else
			{
				this.ReadStartElement();
				array = this.ReadContentAsBase64();
				this.ReadEndElement();
			}
			return array;
		}

		public virtual byte[] ReadElementContentAsBinHex()
		{
			byte[] array;
			if (this.IsStartElement() && this.IsEmptyElement)
			{
				this.Read();
				array = new byte[0];
			}
			else
			{
				this.ReadStartElement();
				array = this.ReadContentAsBinHex();
				this.ReadEndElement();
			}
			return array;
		}

		public virtual void GetNonAtomizedNames(out string localName, out string namespaceUri)
		{
			localName = this.LocalName;
			namespaceUri = this.NamespaceURI;
		}

		public virtual bool TryGetLocalNameAsDictionaryString(out XmlDictionaryString localName)
		{
			localName = null;
			return false;
		}

		public virtual bool TryGetNamespaceUriAsDictionaryString(out XmlDictionaryString namespaceUri)
		{
			namespaceUri = null;
			return false;
		}

		public virtual bool TryGetValueAsDictionaryString(out XmlDictionaryString value)
		{
			value = null;
			return false;
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

		public virtual bool IsStartArray(out Type type)
		{
			type = null;
			return false;
		}

		public virtual bool TryGetArrayLength(out int count)
		{
			count = 0;
			return false;
		}

		public virtual bool[] ReadBooleanArray(string localName, string namespaceUri)
		{
			return BooleanArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual bool[] ReadBooleanArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return BooleanArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, bool[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsBoolean();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual short[] ReadInt16Array(string localName, string namespaceUri)
		{
			return Int16ArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual short[] ReadInt16Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return Int16ArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, short[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				int num2 = this.ReadElementContentAsInt();
				if (num2 < -32768 || num2 > 32767)
				{
					XmlExceptionHelper.ThrowConversionOverflow(this, num2.ToString(NumberFormatInfo.CurrentInfo), "Int16");
				}
				array[offset + num] = (short)num2;
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual int[] ReadInt32Array(string localName, string namespaceUri)
		{
			return Int32ArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int[] ReadInt32Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return Int32ArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, int[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsInt();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual long[] ReadInt64Array(string localName, string namespaceUri)
		{
			return Int64ArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual long[] ReadInt64Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return Int64ArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, long[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsLong();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual float[] ReadSingleArray(string localName, string namespaceUri)
		{
			return SingleArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual float[] ReadSingleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return SingleArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, float[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsFloat();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual double[] ReadDoubleArray(string localName, string namespaceUri)
		{
			return DoubleArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual double[] ReadDoubleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return DoubleArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, double[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsDouble();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual decimal[] ReadDecimalArray(string localName, string namespaceUri)
		{
			return DecimalArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual decimal[] ReadDecimalArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return DecimalArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, decimal[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsDecimal();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual DateTime[] ReadDateTimeArray(string localName, string namespaceUri)
		{
			return DateTimeArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual DateTime[] ReadDateTimeArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return DateTimeArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, DateTime[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsDateTime();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual Guid[] ReadGuidArray(string localName, string namespaceUri)
		{
			return GuidArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual Guid[] ReadGuidArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return GuidArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, Guid[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsGuid();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		public virtual TimeSpan[] ReadTimeSpanArray(string localName, string namespaceUri)
		{
			return TimeSpanArrayHelperWithString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual TimeSpan[] ReadTimeSpanArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			return TimeSpanArrayHelperWithDictionaryString.Instance.ReadArray(this, localName, namespaceUri, this.Quotas.MaxArrayLength);
		}

		public virtual int ReadArray(string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && this.IsStartElement(localName, namespaceUri))
			{
				array[offset + num] = this.ReadElementContentAsTimeSpan();
				num++;
			}
			return num;
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int count)
		{
			return this.ReadArray(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		internal const int MaxInitialArrayLength = 65535;

		private class XmlWrappedReader : XmlDictionaryReader, IXmlLineInfo
		{
			public XmlWrappedReader(XmlReader reader, XmlNamespaceManager nsMgr)
			{
				this.reader = reader;
				this.nsMgr = nsMgr;
			}

			public override int AttributeCount
			{
				get
				{
					return this.reader.AttributeCount;
				}
			}

			public override string BaseURI
			{
				get
				{
					return this.reader.BaseURI;
				}
			}

			public override bool CanReadBinaryContent
			{
				get
				{
					return this.reader.CanReadBinaryContent;
				}
			}

			public override bool CanReadValueChunk
			{
				get
				{
					return this.reader.CanReadValueChunk;
				}
			}

			public override void Close()
			{
				this.reader.Close();
				this.nsMgr = null;
			}

			public override int Depth
			{
				get
				{
					return this.reader.Depth;
				}
			}

			public override bool EOF
			{
				get
				{
					return this.reader.EOF;
				}
			}

			public override string GetAttribute(int index)
			{
				return this.reader.GetAttribute(index);
			}

			public override string GetAttribute(string name)
			{
				return this.reader.GetAttribute(name);
			}

			public override string GetAttribute(string name, string namespaceUri)
			{
				return this.reader.GetAttribute(name, namespaceUri);
			}

			public override bool HasValue
			{
				get
				{
					return this.reader.HasValue;
				}
			}

			public override bool IsDefault
			{
				get
				{
					return this.reader.IsDefault;
				}
			}

			public override bool IsEmptyElement
			{
				get
				{
					return this.reader.IsEmptyElement;
				}
			}

			public override bool IsStartElement(string name)
			{
				return this.reader.IsStartElement(name);
			}

			public override bool IsStartElement(string localName, string namespaceUri)
			{
				return this.reader.IsStartElement(localName, namespaceUri);
			}

			public override string LocalName
			{
				get
				{
					return this.reader.LocalName;
				}
			}

			public override string LookupNamespace(string namespaceUri)
			{
				return this.reader.LookupNamespace(namespaceUri);
			}

			public override void MoveToAttribute(int index)
			{
				this.reader.MoveToAttribute(index);
			}

			public override bool MoveToAttribute(string name)
			{
				return this.reader.MoveToAttribute(name);
			}

			public override bool MoveToAttribute(string name, string namespaceUri)
			{
				return this.reader.MoveToAttribute(name, namespaceUri);
			}

			public override bool MoveToElement()
			{
				return this.reader.MoveToElement();
			}

			public override bool MoveToFirstAttribute()
			{
				return this.reader.MoveToFirstAttribute();
			}

			public override bool MoveToNextAttribute()
			{
				return this.reader.MoveToNextAttribute();
			}

			public override string Name
			{
				get
				{
					return this.reader.Name;
				}
			}

			public override string NamespaceURI
			{
				get
				{
					return this.reader.NamespaceURI;
				}
			}

			public override XmlNameTable NameTable
			{
				get
				{
					return this.reader.NameTable;
				}
			}

			public override XmlNodeType NodeType
			{
				get
				{
					return this.reader.NodeType;
				}
			}

			public override string Prefix
			{
				get
				{
					return this.reader.Prefix;
				}
			}

			public override char QuoteChar
			{
				get
				{
					return this.reader.QuoteChar;
				}
			}

			public override bool Read()
			{
				return this.reader.Read();
			}

			public override bool ReadAttributeValue()
			{
				return this.reader.ReadAttributeValue();
			}

			public override string ReadElementString(string name)
			{
				return this.reader.ReadElementString(name);
			}

			public override string ReadElementString(string localName, string namespaceUri)
			{
				return this.reader.ReadElementString(localName, namespaceUri);
			}

			public override string ReadInnerXml()
			{
				return this.reader.ReadInnerXml();
			}

			public override string ReadOuterXml()
			{
				return this.reader.ReadOuterXml();
			}

			public override void ReadStartElement(string name)
			{
				this.reader.ReadStartElement(name);
			}

			public override void ReadStartElement(string localName, string namespaceUri)
			{
				this.reader.ReadStartElement(localName, namespaceUri);
			}

			public override void ReadEndElement()
			{
				this.reader.ReadEndElement();
			}

			public override string ReadString()
			{
				return this.reader.ReadString();
			}

			public override ReadState ReadState
			{
				get
				{
					return this.reader.ReadState;
				}
			}

			public override void ResolveEntity()
			{
				this.reader.ResolveEntity();
			}

			public override string this[int index]
			{
				get
				{
					return this.reader[index];
				}
			}

			public override string this[string name]
			{
				get
				{
					return this.reader[name];
				}
			}

			public override string this[string name, string namespaceUri]
			{
				get
				{
					return this.reader[name, namespaceUri];
				}
			}

			public override string Value
			{
				get
				{
					return this.reader.Value;
				}
			}

			public override string XmlLang
			{
				get
				{
					return this.reader.XmlLang;
				}
			}

			public override XmlSpace XmlSpace
			{
				get
				{
					return this.reader.XmlSpace;
				}
			}

			public override int ReadElementContentAsBase64(byte[] buffer, int offset, int count)
			{
				return this.reader.ReadElementContentAsBase64(buffer, offset, count);
			}

			public override int ReadContentAsBase64(byte[] buffer, int offset, int count)
			{
				return this.reader.ReadContentAsBase64(buffer, offset, count);
			}

			public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int count)
			{
				return this.reader.ReadElementContentAsBinHex(buffer, offset, count);
			}

			public override int ReadContentAsBinHex(byte[] buffer, int offset, int count)
			{
				return this.reader.ReadContentAsBinHex(buffer, offset, count);
			}

			public override int ReadValueChunk(char[] chars, int offset, int count)
			{
				return this.reader.ReadValueChunk(chars, offset, count);
			}

			public override Type ValueType
			{
				get
				{
					return this.reader.ValueType;
				}
			}

			public override bool ReadContentAsBoolean()
			{
				return this.reader.ReadContentAsBoolean();
			}

			public override DateTime ReadContentAsDateTime()
			{
				return this.reader.ReadContentAsDateTime();
			}

			public override decimal ReadContentAsDecimal()
			{
				return (decimal)this.reader.ReadContentAs(typeof(decimal), null);
			}

			public override double ReadContentAsDouble()
			{
				return this.reader.ReadContentAsDouble();
			}

			public override int ReadContentAsInt()
			{
				return this.reader.ReadContentAsInt();
			}

			public override long ReadContentAsLong()
			{
				return this.reader.ReadContentAsLong();
			}

			public override float ReadContentAsFloat()
			{
				return this.reader.ReadContentAsFloat();
			}

			public override string ReadContentAsString()
			{
				return this.reader.ReadContentAsString();
			}

			public override object ReadContentAs(Type type, IXmlNamespaceResolver namespaceResolver)
			{
				return this.reader.ReadContentAs(type, namespaceResolver);
			}

			public bool HasLineInfo()
			{
				IXmlLineInfo xmlLineInfo = this.reader as IXmlLineInfo;
				return xmlLineInfo != null && xmlLineInfo.HasLineInfo();
			}

			public int LineNumber
			{
				get
				{
					IXmlLineInfo xmlLineInfo = this.reader as IXmlLineInfo;
					if (xmlLineInfo == null)
					{
						return 1;
					}
					return xmlLineInfo.LineNumber;
				}
			}

			public int LinePosition
			{
				get
				{
					IXmlLineInfo xmlLineInfo = this.reader as IXmlLineInfo;
					if (xmlLineInfo == null)
					{
						return 1;
					}
					return xmlLineInfo.LinePosition;
				}
			}

			private XmlReader reader;

			private XmlNamespaceManager nsMgr;
		}
	}
}
