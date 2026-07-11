using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace System.Xml
{
	public abstract class XmlDictionaryReader : XmlReader
	{
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
				if (this.quotas == null)
				{
					this.quotas = new XmlDictionaryReaderQuotas();
				}
				return this.quotas;
			}
		}

		public virtual void EndCanonicalization()
		{
			throw new NotSupportedException();
		}

		public virtual string GetAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			return this.GetAttribute(localName.Value, namespaceUri.Value);
		}

		public virtual int IndexOfLocalName(string[] localNames, string namespaceUri)
		{
			if (localNames == null)
			{
				throw new ArgumentNullException("localNames");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			if (this.NamespaceURI != namespaceUri)
			{
				return -1;
			}
			for (int i = 0; i < localNames.Length; i++)
			{
				if (localNames[i] == this.LocalName)
				{
					return i;
				}
			}
			return -1;
		}

		public virtual int IndexOfLocalName(XmlDictionaryString[] localNames, XmlDictionaryString namespaceUri)
		{
			if (localNames == null)
			{
				throw new ArgumentNullException("localNames");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			if (this.NamespaceURI != namespaceUri.Value)
			{
				return -1;
			}
			XmlDictionaryString xmlDictionaryString;
			if (!this.TryGetLocalNameAsDictionaryString(out xmlDictionaryString))
			{
				return -1;
			}
			IXmlDictionary dictionary = xmlDictionaryString.Dictionary;
			for (int i = 0; i < localNames.Length; i++)
			{
				XmlDictionaryString xmlDictionaryString2;
				if (dictionary.TryLookup(localNames[i], out xmlDictionaryString2) && object.ReferenceEquals(xmlDictionaryString2, xmlDictionaryString))
				{
					return i;
				}
			}
			return -1;
		}

		public virtual bool IsArray(out Type type)
		{
			type = null;
			return false;
		}

		public virtual bool IsLocalName(string localName)
		{
			return this.LocalName == localName;
		}

		public virtual bool IsLocalName(XmlDictionaryString localName)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			return this.LocalName == localName.Value;
		}

		public virtual bool IsNamespaceUri(string namespaceUri)
		{
			return this.NamespaceURI == namespaceUri;
		}

		public virtual bool IsNamespaceUri(XmlDictionaryString namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			return this.NamespaceURI == namespaceUri.Value;
		}

		public virtual bool IsStartArray(out Type type)
		{
			type = null;
			return false;
		}

		public virtual bool IsStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			return this.IsStartElement(localName.Value, namespaceUri.Value);
		}

		protected bool IsTextNode(XmlNodeType nodeType)
		{
			switch (nodeType)
			{
			case XmlNodeType.Attribute:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				break;
			default:
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.SignificantWhitespace)
				{
					return false;
				}
				break;
			}
			return true;
		}

		private XmlException XmlError(string message)
		{
			IXmlLineInfo xmlLineInfo = this as IXmlLineInfo;
			if (xmlLineInfo == null || !xmlLineInfo.HasLineInfo())
			{
				return new XmlException(message);
			}
			return new XmlException(string.Format("{0} in {1} , at ({2},{3})", new object[] { message, this.BaseURI, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition }));
		}

		public virtual void MoveToStartElement()
		{
			this.MoveToContent();
			if (this.NodeType != XmlNodeType.Element)
			{
				throw this.XmlError(string.Format("Element node is expected, but got {0} node.", this.NodeType));
			}
		}

		public virtual void MoveToStartElement(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.MoveToStartElement();
			if (this.Name != name)
			{
				throw this.XmlError(string.Format("Element node '{0}' is expected, but got '{1}' element.", name, this.Name));
			}
		}

		public virtual void MoveToStartElement(string localName, string namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			this.MoveToStartElement();
			if (this.LocalName != localName || this.NamespaceURI != namespaceUri)
			{
				throw this.XmlError(string.Format("Element node '{0}' in namespace '{1}' is expected, but got '{2}' in namespace '{3}' element.", new object[] { localName, namespaceUri, this.LocalName, this.NamespaceURI }));
			}
		}

		public virtual void MoveToStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			this.MoveToStartElement(localName.Value, namespaceUri.Value);
		}

		public virtual void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			throw new NotSupportedException();
		}

		public virtual bool TryGetArrayLength(out int count)
		{
			count = -1;
			return false;
		}

		public virtual bool TryGetBase64ContentLength(out int count)
		{
			count = -1;
			return false;
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

		public override object ReadContentAs(Type type, IXmlNamespaceResolver nsResolver)
		{
			return base.ReadContentAs(type, nsResolver);
		}

		public virtual byte[] ReadContentAsBase64()
		{
			int num;
			if (!this.TryGetBase64ContentLength(out num))
			{
				return Convert.FromBase64String(this.ReadContentAsString());
			}
			byte[] array = new byte[num];
			this.ReadContentAsBase64(array, 0, num);
			return array;
		}

		private byte[] FromBinHexString(string s)
		{
			return (byte[])this.xmlconv_from_bin_hex.Invoke(null, new object[] { s });
		}

		public virtual byte[] ReadContentAsBinHex()
		{
			int num;
			if (!this.TryGetArrayLength(out num))
			{
				return this.FromBinHexString(this.ReadContentAsString());
			}
			return this.ReadContentAsBinHex(num);
		}

		protected byte[] ReadContentAsBinHex(int maxByteArrayContentLength)
		{
			byte[] array = new byte[maxByteArrayContentLength];
			this.ReadContentAsBinHex(array, 0, maxByteArrayContentLength);
			return array;
		}

		[MonoTODO]
		public virtual int ReadContentAsChars(char[] chars, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override decimal ReadContentAsDecimal()
		{
			return base.ReadContentAsDecimal();
		}

		public override float ReadContentAsFloat()
		{
			return base.ReadContentAsFloat();
		}

		public virtual Guid ReadContentAsGuid()
		{
			return XmlConvert.ToGuid(this.ReadContentAsString());
		}

		public virtual void ReadContentAsQualifiedName(out string localName, out string namespaceUri)
		{
			XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)this.ReadContentAs(typeof(XmlQualifiedName), this as IXmlNamespaceResolver);
			localName = xmlQualifiedName.Name;
			namespaceUri = xmlQualifiedName.Namespace;
		}

		public override string ReadContentAsString()
		{
			return this.ReadContentAsString(this.Quotas.MaxStringContentLength);
		}

		[MonoTODO]
		protected string ReadContentAsString(int maxStringContentLength)
		{
			return base.ReadContentAsString();
		}

		[MonoTODO("there is exactly no information on the web")]
		public virtual string ReadContentAsString(string[] strings, out int index)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("there is exactly no information on the web")]
		public virtual string ReadContentAsString(XmlDictionaryString[] strings, out int index)
		{
			throw new NotImplementedException();
		}

		public virtual TimeSpan ReadContentAsTimeSpan()
		{
			return XmlConvert.ToTimeSpan(this.ReadContentAsString());
		}

		public virtual UniqueId ReadContentAsUniqueId()
		{
			return new UniqueId(this.ReadContentAsString());
		}

		public virtual byte[] ReadElementContentAsBase64()
		{
			this.ReadStartElement();
			byte[] array = this.ReadContentAsBase64();
			this.ReadEndElement();
			return array;
		}

		public virtual byte[] ReadElementContentAsBinHex()
		{
			this.ReadStartElement();
			byte[] array = this.ReadContentAsBinHex();
			this.ReadEndElement();
			return array;
		}

		public virtual Guid ReadElementContentAsGuid()
		{
			this.ReadStartElement();
			Guid guid = this.ReadContentAsGuid();
			this.ReadEndElement();
			return guid;
		}

		public virtual TimeSpan ReadElementContentAsTimeSpan()
		{
			this.ReadStartElement();
			TimeSpan timeSpan = this.ReadContentAsTimeSpan();
			this.ReadEndElement();
			return timeSpan;
		}

		public virtual UniqueId ReadElementContentAsUniqueId()
		{
			this.ReadStartElement();
			UniqueId uniqueId = this.ReadContentAsUniqueId();
			this.ReadEndElement();
			return uniqueId;
		}

		public override string ReadElementContentAsString()
		{
			if (this.IsEmptyElement)
			{
				this.Read();
				return string.Empty;
			}
			this.ReadStartElement();
			string text;
			if (this.NodeType == XmlNodeType.EndElement)
			{
				text = string.Empty;
			}
			else
			{
				text = this.ReadContentAsString();
			}
			this.ReadEndElement();
			return text;
		}

		public virtual void ReadFullStartElement()
		{
			if (!this.IsStartElement())
			{
				throw new XmlException("Current node is not a start element");
			}
			this.ReadStartElement();
		}

		public virtual void ReadFullStartElement(string name)
		{
			if (!this.IsStartElement(name))
			{
				throw new XmlException(string.Format("Current node is not a start element '{0}'", name));
			}
			this.ReadStartElement(name);
		}

		public virtual void ReadFullStartElement(string localName, string namespaceUri)
		{
			if (!this.IsStartElement(localName, namespaceUri))
			{
				throw new XmlException(string.Format("Current node is not a start element '{0}' in namesapce '{1}'", localName, namespaceUri));
			}
			this.ReadStartElement(localName, namespaceUri);
		}

		public virtual void ReadFullStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (!this.IsStartElement(localName, namespaceUri))
			{
				throw new XmlException(string.Format("Current node is not a start element '{0}' in namesapce '{1}'", localName, namespaceUri));
			}
			this.ReadStartElement(localName.Value, namespaceUri.Value);
		}

		public virtual void ReadStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}
			this.ReadStartElement(localName.Value, namespaceUri.Value);
		}

		public override string ReadString()
		{
			return this.ReadString(this.Quotas.MaxStringContentLength);
		}

		[MonoTODO]
		protected string ReadString(int maxStringContentLength)
		{
			return base.ReadString();
		}

		public virtual int ReadValueAsBase64(byte[] bytes, int start, int length)
		{
			throw new NotSupportedException();
		}

		public virtual bool TryGetValueAsDictionaryString(out XmlDictionaryString value)
		{
			throw new NotSupportedException();
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(buffer, 0, buffer.Length, quotas);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(buffer, offset, count, new XmlDictionary(), quotas);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(buffer, offset, count, dictionary, quotas, new XmlBinaryReaderSession(), null);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session)
		{
			return XmlDictionaryReader.CreateBinaryReader(buffer, offset, count, dictionary, quotas, session, null);
		}

		public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			return new XmlBinaryDictionaryReader(buffer, offset, count, dictionary, quotas, session, onClose);
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(stream, new XmlDictionary(), quotas);
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateBinaryReader(stream, dictionary, quotas, new XmlBinaryReaderSession(), null);
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session)
		{
			return XmlDictionaryReader.CreateBinaryReader(stream, dictionary, quotas, session, null);
		}

		public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
		{
			return new XmlBinaryDictionaryReader(stream, dictionary, quotas, session, onClose);
		}

		public static XmlDictionaryReader CreateDictionaryReader(XmlReader reader)
		{
			return new XmlSimpleDictionaryReader(reader);
		}

		public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas)
		{
			return new XmlMtomDictionaryReader(stream, encoding, quotas);
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
			return new XmlMtomDictionaryReader(stream, encodings, contentType, quotas, maxBufferSize, onClose);
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateMtomReader(new MemoryStream(buffer, offset, count), encoding, quotas);
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateMtomReader(new MemoryStream(buffer, offset, count), encodings, quotas);
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateMtomReader(new MemoryStream(buffer, offset, count), encodings, contentType, quotas);
		}

		public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
		{
			return XmlDictionaryReader.CreateMtomReader(new MemoryStream(buffer, offset, count), encodings, contentType, quotas, maxBufferSize, onClose);
		}

		public static XmlDictionaryReader CreateTextReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateTextReader(buffer, 0, buffer.Length, quotas);
		}

		public static XmlDictionaryReader CreateTextReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateTextReader(buffer, offset, count, Encoding.UTF8, quotas, null);
		}

		public static XmlDictionaryReader CreateTextReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			return XmlDictionaryReader.CreateTextReader(new MemoryStream(buffer, offset, count), encoding, quotas, onClose);
		}

		public static XmlDictionaryReader CreateTextReader(Stream stream, XmlDictionaryReaderQuotas quotas)
		{
			return XmlDictionaryReader.CreateTextReader(stream, Encoding.UTF8, quotas, null);
		}

		public static XmlDictionaryReader CreateTextReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			XmlNameTable xmlNameTable = new NameTable();
			XmlParserContext xmlParserContext = new XmlParserContext(xmlNameTable, new XmlNamespaceManager(xmlNameTable), string.Empty, XmlSpace.None, encoding);
			return new XmlSimpleDictionaryReader(XmlReader.Create(stream, xmlReaderSettings, xmlParserContext), null, onClose)
			{
				quotas = quotas
			};
		}

		private void CheckReadArrayArguments(Array array, int offset, int length)
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

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, bool[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToBoolean(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual bool[] ReadBooleanArray(string localName, string namespaceUri)
		{
			List<bool> list = new List<bool>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToBoolean(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual bool[] ReadBooleanArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadBooleanArray(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, DateTime[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToDateTime(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual DateTime[] ReadDateTimeArray(string localName, string namespaceUri)
		{
			List<DateTime> list = new List<DateTime>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToDateTime(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual DateTime[] ReadDateTimeArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadDateTimeArray(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, decimal[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToDecimal(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual decimal[] ReadDecimalArray(string localName, string namespaceUri)
		{
			List<decimal> list = new List<decimal>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToDecimal(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual decimal[] ReadDecimalArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadDecimalArray(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, double[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToDouble(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual double[] ReadDoubleArray(string localName, string namespaceUri)
		{
			List<double> list = new List<double>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToDouble(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual double[] ReadDoubleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadDoubleArray(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, Guid[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToGuid(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual Guid[] ReadGuidArray(string localName, string namespaceUri)
		{
			List<Guid> list = new List<Guid>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToGuid(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual Guid[] ReadGuidArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadGuidArray(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, short[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToInt16(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual short[] ReadInt16Array(string localName, string namespaceUri)
		{
			List<short> list = new List<short>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToInt16(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual short[] ReadInt16Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadInt16Array(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, int[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToInt32(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual int[] ReadInt32Array(string localName, string namespaceUri)
		{
			List<int> list = new List<int>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToInt32(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual int[] ReadInt32Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadInt32Array(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, long[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToInt64(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual long[] ReadInt64Array(string localName, string namespaceUri)
		{
			List<long> list = new List<long>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToInt64(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual long[] ReadInt64Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadInt64Array(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, float[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToSingle(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual float[] ReadSingleArray(string localName, string namespaceUri)
		{
			List<float> list = new List<float>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToSingle(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual float[] ReadSingleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadSingleArray(localName.Value, namespaceUri.Value);
		}

		public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int length)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
		}

		public virtual int ReadArray(string localName, string namespaceUri, TimeSpan[] array, int offset, int length)
		{
			this.CheckReadArrayArguments(array, offset, length);
			for (int i = 0; i < length; i++)
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					return i;
				}
				this.ReadStartElement(localName, namespaceUri);
				array[offset + i] = XmlConvert.ToTimeSpan(this.ReadContentAsString());
				this.ReadEndElement();
			}
			return length;
		}

		public virtual TimeSpan[] ReadTimeSpanArray(string localName, string namespaceUri)
		{
			List<TimeSpan> list = new List<TimeSpan>();
			do
			{
				this.MoveToContent();
				if (this.NodeType != XmlNodeType.Element)
				{
					break;
				}
				this.ReadStartElement(localName, namespaceUri);
				list.Add(XmlConvert.ToTimeSpan(this.ReadContentAsString()));
				this.ReadEndElement();
			}
			while (list.Count != this.Quotas.MaxArrayLength);
			return list.ToArray();
		}

		public virtual TimeSpan[] ReadTimeSpanArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.CheckDictionaryStringArgs(localName, namespaceUri);
			return this.ReadTimeSpanArray(localName.Value, namespaceUri.Value);
		}

		public override bool ReadElementContentAsBoolean()
		{
			this.ReadStartElement(this.LocalName, this.NamespaceURI);
			bool flag = this.ReadContentAsBoolean();
			this.ReadEndElement();
			return flag;
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			this.ReadStartElement(this.LocalName, this.NamespaceURI);
			DateTime dateTime = this.ReadContentAsDateTime();
			this.ReadEndElement();
			return dateTime;
		}

		public override decimal ReadElementContentAsDecimal()
		{
			this.ReadStartElement(this.LocalName, this.NamespaceURI);
			decimal num = this.ReadContentAsDecimal();
			this.ReadEndElement();
			return num;
		}

		public override double ReadElementContentAsDouble()
		{
			this.ReadStartElement(this.LocalName, this.NamespaceURI);
			double num = this.ReadContentAsDouble();
			this.ReadEndElement();
			return num;
		}

		public override float ReadElementContentAsFloat()
		{
			this.ReadStartElement(this.LocalName, this.NamespaceURI);
			float num = this.ReadContentAsFloat();
			this.ReadEndElement();
			return num;
		}

		public override int ReadElementContentAsInt()
		{
			this.ReadStartElement(this.LocalName, this.NamespaceURI);
			int num = this.ReadContentAsInt();
			this.ReadEndElement();
			return num;
		}

		public override long ReadElementContentAsLong()
		{
			this.ReadStartElement(this.LocalName, this.NamespaceURI);
			long num = this.ReadContentAsLong();
			this.ReadEndElement();
			return num;
		}

		private XmlDictionaryReaderQuotas quotas;

		private MethodInfo xmlconv_from_bin_hex = typeof(XmlConvert).GetMethod("FromBinHexString", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);

		private static readonly char[] wsChars = new char[] { ' ', '\t', '\n', '\r' };
	}
}
