using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal class XmlReaderDelegator
	{
		public XmlReaderDelegator(XmlReader reader)
		{
			XmlObjectSerializer.CheckNull(reader, "reader");
			this.reader = reader;
			this.dictionaryReader = reader as XmlDictionaryReader;
		}

		internal XmlReader UnderlyingReader
		{
			get
			{
				return this.reader;
			}
		}

		internal ExtensionDataReader UnderlyingExtensionDataReader
		{
			get
			{
				return this.reader as ExtensionDataReader;
			}
		}

		internal int AttributeCount
		{
			get
			{
				if (!this.isEndOfEmptyElement)
				{
					return this.reader.AttributeCount;
				}
				return 0;
			}
		}

		internal string GetAttribute(string name)
		{
			if (!this.isEndOfEmptyElement)
			{
				return this.reader.GetAttribute(name);
			}
			return null;
		}

		internal string GetAttribute(string name, string namespaceUri)
		{
			if (!this.isEndOfEmptyElement)
			{
				return this.reader.GetAttribute(name, namespaceUri);
			}
			return null;
		}

		internal string GetAttribute(int i)
		{
			if (this.isEndOfEmptyElement)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("i", global::System.Runtime.Serialization.SR.GetString("Only Element nodes have attributes.")));
			}
			return this.reader.GetAttribute(i);
		}

		internal bool IsEmptyElement
		{
			get
			{
				return false;
			}
		}

		internal bool IsNamespaceURI(string ns)
		{
			if (this.dictionaryReader == null)
			{
				return ns == this.reader.NamespaceURI;
			}
			return this.dictionaryReader.IsNamespaceUri(ns);
		}

		internal bool IsLocalName(string localName)
		{
			if (this.dictionaryReader == null)
			{
				return localName == this.reader.LocalName;
			}
			return this.dictionaryReader.IsLocalName(localName);
		}

		internal bool IsNamespaceUri(XmlDictionaryString ns)
		{
			if (this.dictionaryReader == null)
			{
				return ns.Value == this.reader.NamespaceURI;
			}
			return this.dictionaryReader.IsNamespaceUri(ns);
		}

		internal bool IsLocalName(XmlDictionaryString localName)
		{
			if (this.dictionaryReader == null)
			{
				return localName.Value == this.reader.LocalName;
			}
			return this.dictionaryReader.IsLocalName(localName);
		}

		internal int IndexOfLocalName(XmlDictionaryString[] localNames, XmlDictionaryString ns)
		{
			if (this.dictionaryReader != null)
			{
				return this.dictionaryReader.IndexOfLocalName(localNames, ns);
			}
			if (this.reader.NamespaceURI == ns.Value)
			{
				string localName = this.LocalName;
				for (int i = 0; i < localNames.Length; i++)
				{
					if (localName == localNames[i].Value)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public bool IsStartElement()
		{
			return !this.isEndOfEmptyElement && this.reader.IsStartElement();
		}

		internal bool IsStartElement(string localname, string ns)
		{
			return !this.isEndOfEmptyElement && this.reader.IsStartElement(localname, ns);
		}

		public bool IsStartElement(XmlDictionaryString localname, XmlDictionaryString ns)
		{
			if (this.dictionaryReader == null)
			{
				return !this.isEndOfEmptyElement && this.reader.IsStartElement(localname.Value, ns.Value);
			}
			return !this.isEndOfEmptyElement && this.dictionaryReader.IsStartElement(localname, ns);
		}

		internal bool MoveToAttribute(string name)
		{
			return !this.isEndOfEmptyElement && this.reader.MoveToAttribute(name);
		}

		internal bool MoveToAttribute(string name, string ns)
		{
			return !this.isEndOfEmptyElement && this.reader.MoveToAttribute(name, ns);
		}

		internal void MoveToAttribute(int i)
		{
			if (this.isEndOfEmptyElement)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("i", global::System.Runtime.Serialization.SR.GetString("Only Element nodes have attributes.")));
			}
			this.reader.MoveToAttribute(i);
		}

		internal bool MoveToElement()
		{
			return !this.isEndOfEmptyElement && this.reader.MoveToElement();
		}

		internal bool MoveToFirstAttribute()
		{
			return !this.isEndOfEmptyElement && this.reader.MoveToFirstAttribute();
		}

		internal bool MoveToNextAttribute()
		{
			return !this.isEndOfEmptyElement && this.reader.MoveToNextAttribute();
		}

		public XmlNodeType NodeType
		{
			get
			{
				if (!this.isEndOfEmptyElement)
				{
					return this.reader.NodeType;
				}
				return XmlNodeType.EndElement;
			}
		}

		internal bool Read()
		{
			this.reader.MoveToElement();
			if (!this.reader.IsEmptyElement)
			{
				return this.reader.Read();
			}
			if (this.isEndOfEmptyElement)
			{
				this.isEndOfEmptyElement = false;
				return this.reader.Read();
			}
			this.isEndOfEmptyElement = true;
			return true;
		}

		internal XmlNodeType MoveToContent()
		{
			if (this.isEndOfEmptyElement)
			{
				return XmlNodeType.EndElement;
			}
			return this.reader.MoveToContent();
		}

		internal bool ReadAttributeValue()
		{
			return !this.isEndOfEmptyElement && this.reader.ReadAttributeValue();
		}

		public void ReadEndElement()
		{
			if (this.isEndOfEmptyElement)
			{
				this.Read();
				return;
			}
			this.reader.ReadEndElement();
		}

		private Exception CreateInvalidPrimitiveTypeException(Type type)
		{
			return new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString(type.IsInterface ? "Interface type '{0}' cannot be created. Consider replacing with a non-interface serializable type." : "Type '{0}' is not a valid serializable type.", new object[] { DataContract.GetClrTypeFullName(type) }));
		}

		public object ReadElementContentAsAnyType(Type valueType)
		{
			this.Read();
			object obj = this.ReadContentAsAnyType(valueType);
			this.ReadEndElement();
			return obj;
		}

		internal object ReadContentAsAnyType(Type valueType)
		{
			switch (Type.GetTypeCode(valueType))
			{
			case TypeCode.Boolean:
				return this.ReadContentAsBoolean();
			case TypeCode.Char:
				return this.ReadContentAsChar();
			case TypeCode.SByte:
				return this.ReadContentAsSignedByte();
			case TypeCode.Byte:
				return this.ReadContentAsUnsignedByte();
			case TypeCode.Int16:
				return this.ReadContentAsShort();
			case TypeCode.UInt16:
				return this.ReadContentAsUnsignedShort();
			case TypeCode.Int32:
				return this.ReadContentAsInt();
			case TypeCode.UInt32:
				return this.ReadContentAsUnsignedInt();
			case TypeCode.Int64:
				return this.ReadContentAsLong();
			case TypeCode.UInt64:
				return this.ReadContentAsUnsignedLong();
			case TypeCode.Single:
				return this.ReadContentAsSingle();
			case TypeCode.Double:
				return this.ReadContentAsDouble();
			case TypeCode.Decimal:
				return this.ReadContentAsDecimal();
			case TypeCode.DateTime:
				return this.ReadContentAsDateTime();
			case TypeCode.String:
				return this.ReadContentAsString();
			}
			if (valueType == Globals.TypeOfByteArray)
			{
				return this.ReadContentAsBase64();
			}
			if (valueType == Globals.TypeOfObject)
			{
				return new object();
			}
			if (valueType == Globals.TypeOfTimeSpan)
			{
				return this.ReadContentAsTimeSpan();
			}
			if (valueType == Globals.TypeOfGuid)
			{
				return this.ReadContentAsGuid();
			}
			if (valueType == Globals.TypeOfUri)
			{
				return this.ReadContentAsUri();
			}
			if (valueType == Globals.TypeOfXmlQualifiedName)
			{
				return this.ReadContentAsQName();
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateInvalidPrimitiveTypeException(valueType));
		}

		internal IDataNode ReadExtensionData(Type valueType)
		{
			switch (Type.GetTypeCode(valueType))
			{
			case TypeCode.Boolean:
				return new DataNode<bool>(this.ReadContentAsBoolean());
			case TypeCode.Char:
				return new DataNode<char>(this.ReadContentAsChar());
			case TypeCode.SByte:
				return new DataNode<sbyte>(this.ReadContentAsSignedByte());
			case TypeCode.Byte:
				return new DataNode<byte>(this.ReadContentAsUnsignedByte());
			case TypeCode.Int16:
				return new DataNode<short>(this.ReadContentAsShort());
			case TypeCode.UInt16:
				return new DataNode<ushort>(this.ReadContentAsUnsignedShort());
			case TypeCode.Int32:
				return new DataNode<int>(this.ReadContentAsInt());
			case TypeCode.UInt32:
				return new DataNode<uint>(this.ReadContentAsUnsignedInt());
			case TypeCode.Int64:
				return new DataNode<long>(this.ReadContentAsLong());
			case TypeCode.UInt64:
				return new DataNode<ulong>(this.ReadContentAsUnsignedLong());
			case TypeCode.Single:
				return new DataNode<float>(this.ReadContentAsSingle());
			case TypeCode.Double:
				return new DataNode<double>(this.ReadContentAsDouble());
			case TypeCode.Decimal:
				return new DataNode<decimal>(this.ReadContentAsDecimal());
			case TypeCode.DateTime:
				return new DataNode<DateTime>(this.ReadContentAsDateTime());
			case TypeCode.String:
				return new DataNode<string>(this.ReadContentAsString());
			}
			if (valueType == Globals.TypeOfByteArray)
			{
				return new DataNode<byte[]>(this.ReadContentAsBase64());
			}
			if (valueType == Globals.TypeOfObject)
			{
				return new DataNode<object>(new object());
			}
			if (valueType == Globals.TypeOfTimeSpan)
			{
				return new DataNode<TimeSpan>(this.ReadContentAsTimeSpan());
			}
			if (valueType == Globals.TypeOfGuid)
			{
				return new DataNode<Guid>(this.ReadContentAsGuid());
			}
			if (valueType == Globals.TypeOfUri)
			{
				return new DataNode<Uri>(this.ReadContentAsUri());
			}
			if (valueType == Globals.TypeOfXmlQualifiedName)
			{
				return new DataNode<XmlQualifiedName>(this.ReadContentAsQName());
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateInvalidPrimitiveTypeException(valueType));
		}

		private void ThrowConversionException(string value, string type)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(XmlObjectSerializer.TryAddLineInfo(this, global::System.Runtime.Serialization.SR.GetString("The value '{0}' cannot be parsed as the type '{1}'.", new object[] { value, type }))));
		}

		private void ThrowNotAtElement()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Start element expected. Found {0}.", new object[] { "EndElement" })));
		}

		internal virtual char ReadElementContentAsChar()
		{
			return this.ToChar(this.ReadElementContentAsInt());
		}

		internal virtual char ReadContentAsChar()
		{
			return this.ToChar(this.ReadContentAsInt());
		}

		private char ToChar(int value)
		{
			if (value < 0 || value > 65535)
			{
				this.ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "Char");
			}
			return (char)value;
		}

		public string ReadElementContentAsString()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsString();
		}

		internal string ReadContentAsString()
		{
			if (!this.isEndOfEmptyElement)
			{
				return this.reader.ReadContentAsString();
			}
			return string.Empty;
		}

		public bool ReadElementContentAsBoolean()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsBoolean();
		}

		internal bool ReadContentAsBoolean()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowConversionException(string.Empty, "Boolean");
			}
			return this.reader.ReadContentAsBoolean();
		}

		public float ReadElementContentAsFloat()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsFloat();
		}

		internal float ReadContentAsSingle()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowConversionException(string.Empty, "Float");
			}
			return this.reader.ReadContentAsFloat();
		}

		public double ReadElementContentAsDouble()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsDouble();
		}

		internal double ReadContentAsDouble()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowConversionException(string.Empty, "Double");
			}
			return this.reader.ReadContentAsDouble();
		}

		public decimal ReadElementContentAsDecimal()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsDecimal();
		}

		internal decimal ReadContentAsDecimal()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowConversionException(string.Empty, "Decimal");
			}
			return this.reader.ReadContentAsDecimal();
		}

		internal virtual byte[] ReadElementContentAsBase64()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			if (this.dictionaryReader == null)
			{
				return this.ReadContentAsBase64(this.reader.ReadElementContentAsString());
			}
			return this.dictionaryReader.ReadElementContentAsBase64();
		}

		internal virtual byte[] ReadContentAsBase64()
		{
			if (this.isEndOfEmptyElement)
			{
				return new byte[0];
			}
			if (this.dictionaryReader == null)
			{
				return this.ReadContentAsBase64(this.reader.ReadContentAsString());
			}
			return this.dictionaryReader.ReadContentAsBase64();
		}

		internal byte[] ReadContentAsBase64(string str)
		{
			if (str == null)
			{
				return null;
			}
			str = str.Trim();
			if (str.Length == 0)
			{
				return new byte[0];
			}
			byte[] array;
			try
			{
				array = Convert.FromBase64String(str);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "byte[]", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(str, "byte[]", ex2));
			}
			return array;
		}

		internal virtual DateTime ReadElementContentAsDateTime()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsDateTime();
		}

		internal virtual DateTime ReadContentAsDateTime()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowConversionException(string.Empty, "DateTime");
			}
			return this.reader.ReadContentAsDateTime();
		}

		public int ReadElementContentAsInt()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsInt();
		}

		internal int ReadContentAsInt()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowConversionException(string.Empty, "Int32");
			}
			return this.reader.ReadContentAsInt();
		}

		public long ReadElementContentAsLong()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return this.reader.ReadElementContentAsLong();
		}

		internal long ReadContentAsLong()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowConversionException(string.Empty, "Int64");
			}
			return this.reader.ReadContentAsLong();
		}

		public short ReadElementContentAsShort()
		{
			return this.ToShort(this.ReadElementContentAsInt());
		}

		internal short ReadContentAsShort()
		{
			return this.ToShort(this.ReadContentAsInt());
		}

		private short ToShort(int value)
		{
			if (value < -32768 || value > 32767)
			{
				this.ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "Int16");
			}
			return (short)value;
		}

		public byte ReadElementContentAsUnsignedByte()
		{
			return this.ToByte(this.ReadElementContentAsInt());
		}

		internal byte ReadContentAsUnsignedByte()
		{
			return this.ToByte(this.ReadContentAsInt());
		}

		private byte ToByte(int value)
		{
			if (value < 0 || value > 255)
			{
				this.ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "Byte");
			}
			return (byte)value;
		}

		public sbyte ReadElementContentAsSignedByte()
		{
			return this.ToSByte(this.ReadElementContentAsInt());
		}

		internal sbyte ReadContentAsSignedByte()
		{
			return this.ToSByte(this.ReadContentAsInt());
		}

		private sbyte ToSByte(int value)
		{
			if (value < -128 || value > 127)
			{
				this.ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "SByte");
			}
			return (sbyte)value;
		}

		public uint ReadElementContentAsUnsignedInt()
		{
			return this.ToUInt32(this.ReadElementContentAsLong());
		}

		internal uint ReadContentAsUnsignedInt()
		{
			return this.ToUInt32(this.ReadContentAsLong());
		}

		private uint ToUInt32(long value)
		{
			if (value < 0L || value > (long)((ulong)(-1)))
			{
				this.ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "UInt32");
			}
			return (uint)value;
		}

		internal virtual ulong ReadElementContentAsUnsignedLong()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			string text = this.reader.ReadElementContentAsString();
			if (text == null || text.Length == 0)
			{
				this.ThrowConversionException(string.Empty, "UInt64");
			}
			return XmlConverter.ToUInt64(text);
		}

		internal virtual ulong ReadContentAsUnsignedLong()
		{
			string text = this.reader.ReadContentAsString();
			if (text == null || text.Length == 0)
			{
				this.ThrowConversionException(string.Empty, "UInt64");
			}
			return XmlConverter.ToUInt64(text);
		}

		public ushort ReadElementContentAsUnsignedShort()
		{
			return this.ToUInt16(this.ReadElementContentAsInt());
		}

		internal ushort ReadContentAsUnsignedShort()
		{
			return this.ToUInt16(this.ReadContentAsInt());
		}

		private ushort ToUInt16(int value)
		{
			if (value < 0 || value > 65535)
			{
				this.ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "UInt16");
			}
			return (ushort)value;
		}

		public TimeSpan ReadElementContentAsTimeSpan()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			return XmlConverter.ToTimeSpan(this.reader.ReadElementContentAsString());
		}

		internal TimeSpan ReadContentAsTimeSpan()
		{
			return XmlConverter.ToTimeSpan(this.reader.ReadContentAsString());
		}

		public Guid ReadElementContentAsGuid()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			string text = this.reader.ReadElementContentAsString();
			Guid guid;
			try
			{
				guid = Guid.Parse(text);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Guid", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Guid", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Guid", ex3));
			}
			return guid;
		}

		internal Guid ReadContentAsGuid()
		{
			string text = this.reader.ReadContentAsString();
			Guid guid;
			try
			{
				guid = Guid.Parse(text);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Guid", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Guid", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Guid", ex3));
			}
			return guid;
		}

		public Uri ReadElementContentAsUri()
		{
			if (this.isEndOfEmptyElement)
			{
				this.ThrowNotAtElement();
			}
			string text = this.ReadElementContentAsString();
			Uri uri;
			try
			{
				uri = new Uri(text, UriKind.RelativeOrAbsolute);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Uri", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Uri", ex2));
			}
			return uri;
		}

		internal Uri ReadContentAsUri()
		{
			string text = this.ReadContentAsString();
			Uri uri;
			try
			{
				uri = new Uri(text, UriKind.RelativeOrAbsolute);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Uri", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Uri", ex2));
			}
			return uri;
		}

		public XmlQualifiedName ReadElementContentAsQName()
		{
			this.Read();
			XmlQualifiedName xmlQualifiedName = this.ReadContentAsQName();
			this.ReadEndElement();
			return xmlQualifiedName;
		}

		internal virtual XmlQualifiedName ReadContentAsQName()
		{
			return this.ParseQualifiedName(this.ReadContentAsString());
		}

		private XmlQualifiedName ParseQualifiedName(string str)
		{
			string empty;
			string text;
			if (str == null || str.Length == 0)
			{
				text = (empty = string.Empty);
			}
			else
			{
				string text2;
				XmlObjectSerializerReadContext.ParseQualifiedName(str, this, out empty, out text, out text2);
			}
			return new XmlQualifiedName(empty, text);
		}

		private void CheckExpectedArrayLength(XmlObjectSerializerReadContext context, int arrayLength)
		{
			context.IncrementItemCount(arrayLength);
		}

		protected int GetArrayLengthQuota(XmlObjectSerializerReadContext context)
		{
			if (this.dictionaryReader.Quotas == null)
			{
				return context.RemainingItemCount;
			}
			return Math.Min(context.RemainingItemCount, this.dictionaryReader.Quotas.MaxArrayLength);
		}

		private void CheckActualArrayLength(int expectedLength, int actualLength, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			if (expectedLength != actualLength)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Array length '{0}' provided by Size attribute is not equal to the number of array elements '{1}' from namespace '{2}' found.", new object[] { expectedLength, itemName.Value, itemNamespace.Value })));
			}
		}

		internal bool TryReadBooleanArray(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out bool[] array)
		{
			if (this.dictionaryReader == null)
			{
				array = null;
				return false;
			}
			if (arrayLength != -1)
			{
				this.CheckExpectedArrayLength(context, arrayLength);
				array = new bool[arrayLength];
				int num = 0;
				int num2;
				while ((num2 = this.dictionaryReader.ReadArray(itemName, itemNamespace, array, num, arrayLength - num)) > 0)
				{
					num += num2;
				}
				this.CheckActualArrayLength(arrayLength, num, itemName, itemNamespace);
			}
			else
			{
				array = BooleanArrayHelperWithDictionaryString.Instance.ReadArray(this.dictionaryReader, itemName, itemNamespace, this.GetArrayLengthQuota(context));
				context.IncrementItemCount(array.Length);
			}
			return true;
		}

		internal bool TryReadDateTimeArray(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out DateTime[] array)
		{
			if (this.dictionaryReader == null)
			{
				array = null;
				return false;
			}
			if (arrayLength != -1)
			{
				this.CheckExpectedArrayLength(context, arrayLength);
				array = new DateTime[arrayLength];
				int num = 0;
				int num2;
				while ((num2 = this.dictionaryReader.ReadArray(itemName, itemNamespace, array, num, arrayLength - num)) > 0)
				{
					num += num2;
				}
				this.CheckActualArrayLength(arrayLength, num, itemName, itemNamespace);
			}
			else
			{
				array = DateTimeArrayHelperWithDictionaryString.Instance.ReadArray(this.dictionaryReader, itemName, itemNamespace, this.GetArrayLengthQuota(context));
				context.IncrementItemCount(array.Length);
			}
			return true;
		}

		internal bool TryReadDecimalArray(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out decimal[] array)
		{
			if (this.dictionaryReader == null)
			{
				array = null;
				return false;
			}
			if (arrayLength != -1)
			{
				this.CheckExpectedArrayLength(context, arrayLength);
				array = new decimal[arrayLength];
				int num = 0;
				int num2;
				while ((num2 = this.dictionaryReader.ReadArray(itemName, itemNamespace, array, num, arrayLength - num)) > 0)
				{
					num += num2;
				}
				this.CheckActualArrayLength(arrayLength, num, itemName, itemNamespace);
			}
			else
			{
				array = DecimalArrayHelperWithDictionaryString.Instance.ReadArray(this.dictionaryReader, itemName, itemNamespace, this.GetArrayLengthQuota(context));
				context.IncrementItemCount(array.Length);
			}
			return true;
		}

		internal bool TryReadInt32Array(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out int[] array)
		{
			if (this.dictionaryReader == null)
			{
				array = null;
				return false;
			}
			if (arrayLength != -1)
			{
				this.CheckExpectedArrayLength(context, arrayLength);
				array = new int[arrayLength];
				int num = 0;
				int num2;
				while ((num2 = this.dictionaryReader.ReadArray(itemName, itemNamespace, array, num, arrayLength - num)) > 0)
				{
					num += num2;
				}
				this.CheckActualArrayLength(arrayLength, num, itemName, itemNamespace);
			}
			else
			{
				array = Int32ArrayHelperWithDictionaryString.Instance.ReadArray(this.dictionaryReader, itemName, itemNamespace, this.GetArrayLengthQuota(context));
				context.IncrementItemCount(array.Length);
			}
			return true;
		}

		internal bool TryReadInt64Array(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out long[] array)
		{
			if (this.dictionaryReader == null)
			{
				array = null;
				return false;
			}
			if (arrayLength != -1)
			{
				this.CheckExpectedArrayLength(context, arrayLength);
				array = new long[arrayLength];
				int num = 0;
				int num2;
				while ((num2 = this.dictionaryReader.ReadArray(itemName, itemNamespace, array, num, arrayLength - num)) > 0)
				{
					num += num2;
				}
				this.CheckActualArrayLength(arrayLength, num, itemName, itemNamespace);
			}
			else
			{
				array = Int64ArrayHelperWithDictionaryString.Instance.ReadArray(this.dictionaryReader, itemName, itemNamespace, this.GetArrayLengthQuota(context));
				context.IncrementItemCount(array.Length);
			}
			return true;
		}

		internal bool TryReadSingleArray(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out float[] array)
		{
			if (this.dictionaryReader == null)
			{
				array = null;
				return false;
			}
			if (arrayLength != -1)
			{
				this.CheckExpectedArrayLength(context, arrayLength);
				array = new float[arrayLength];
				int num = 0;
				int num2;
				while ((num2 = this.dictionaryReader.ReadArray(itemName, itemNamespace, array, num, arrayLength - num)) > 0)
				{
					num += num2;
				}
				this.CheckActualArrayLength(arrayLength, num, itemName, itemNamespace);
			}
			else
			{
				array = SingleArrayHelperWithDictionaryString.Instance.ReadArray(this.dictionaryReader, itemName, itemNamespace, this.GetArrayLengthQuota(context));
				context.IncrementItemCount(array.Length);
			}
			return true;
		}

		internal bool TryReadDoubleArray(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out double[] array)
		{
			if (this.dictionaryReader == null)
			{
				array = null;
				return false;
			}
			if (arrayLength != -1)
			{
				this.CheckExpectedArrayLength(context, arrayLength);
				array = new double[arrayLength];
				int num = 0;
				int num2;
				while ((num2 = this.dictionaryReader.ReadArray(itemName, itemNamespace, array, num, arrayLength - num)) > 0)
				{
					num += num2;
				}
				this.CheckActualArrayLength(arrayLength, num, itemName, itemNamespace);
			}
			else
			{
				array = DoubleArrayHelperWithDictionaryString.Instance.ReadArray(this.dictionaryReader, itemName, itemNamespace, this.GetArrayLengthQuota(context));
				context.IncrementItemCount(array.Length);
			}
			return true;
		}

		internal IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			if (!(this.reader is IXmlNamespaceResolver))
			{
				return null;
			}
			return ((IXmlNamespaceResolver)this.reader).GetNamespacesInScope(scope);
		}

		internal bool HasLineInfo()
		{
			IXmlLineInfo xmlLineInfo = this.reader as IXmlLineInfo;
			return xmlLineInfo != null && xmlLineInfo.HasLineInfo();
		}

		internal int LineNumber
		{
			get
			{
				IXmlLineInfo xmlLineInfo = this.reader as IXmlLineInfo;
				if (xmlLineInfo != null)
				{
					return xmlLineInfo.LineNumber;
				}
				return 0;
			}
		}

		internal int LinePosition
		{
			get
			{
				IXmlLineInfo xmlLineInfo = this.reader as IXmlLineInfo;
				if (xmlLineInfo != null)
				{
					return xmlLineInfo.LinePosition;
				}
				return 0;
			}
		}

		internal bool Normalized
		{
			get
			{
				XmlTextReader xmlTextReader = this.reader as XmlTextReader;
				if (xmlTextReader == null)
				{
					IXmlTextParser xmlTextParser = this.reader as IXmlTextParser;
					return xmlTextParser != null && xmlTextParser.Normalized;
				}
				return xmlTextReader.Normalization;
			}
			set
			{
				XmlTextReader xmlTextReader = this.reader as XmlTextReader;
				if (xmlTextReader == null)
				{
					IXmlTextParser xmlTextParser = this.reader as IXmlTextParser;
					if (xmlTextParser != null)
					{
						xmlTextParser.Normalized = value;
						return;
					}
				}
				else
				{
					xmlTextReader.Normalization = value;
				}
			}
		}

		internal WhitespaceHandling WhitespaceHandling
		{
			get
			{
				XmlTextReader xmlTextReader = this.reader as XmlTextReader;
				if (xmlTextReader != null)
				{
					return xmlTextReader.WhitespaceHandling;
				}
				IXmlTextParser xmlTextParser = this.reader as IXmlTextParser;
				if (xmlTextParser != null)
				{
					return xmlTextParser.WhitespaceHandling;
				}
				return WhitespaceHandling.None;
			}
			set
			{
				XmlTextReader xmlTextReader = this.reader as XmlTextReader;
				if (xmlTextReader == null)
				{
					IXmlTextParser xmlTextParser = this.reader as IXmlTextParser;
					if (xmlTextParser != null)
					{
						xmlTextParser.WhitespaceHandling = value;
						return;
					}
				}
				else
				{
					xmlTextReader.WhitespaceHandling = value;
				}
			}
		}

		internal string Name
		{
			get
			{
				return this.reader.Name;
			}
		}

		public string LocalName
		{
			get
			{
				return this.reader.LocalName;
			}
		}

		internal string NamespaceURI
		{
			get
			{
				return this.reader.NamespaceURI;
			}
		}

		internal string Value
		{
			get
			{
				return this.reader.Value;
			}
		}

		internal Type ValueType
		{
			get
			{
				return this.reader.ValueType;
			}
		}

		internal int Depth
		{
			get
			{
				return this.reader.Depth;
			}
		}

		internal string LookupNamespace(string prefix)
		{
			return this.reader.LookupNamespace(prefix);
		}

		internal bool EOF
		{
			get
			{
				return this.reader.EOF;
			}
		}

		internal void Skip()
		{
			this.reader.Skip();
			this.isEndOfEmptyElement = false;
		}

		protected XmlReader reader;

		protected XmlDictionaryReader dictionaryReader;

		protected bool isEndOfEmptyElement;
	}
}
