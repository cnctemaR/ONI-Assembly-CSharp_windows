using System;
using System.Globalization;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonReaderDelegator : XmlReaderDelegator
	{
		public JsonReaderDelegator(XmlReader reader)
			: base(reader)
		{
		}

		public JsonReaderDelegator(XmlReader reader, DateTimeFormat dateTimeFormat)
			: this(reader)
		{
			this.dateTimeFormat = dateTimeFormat;
		}

		internal XmlDictionaryReaderQuotas ReaderQuotas
		{
			get
			{
				if (this.dictionaryReader == null)
				{
					return null;
				}
				return this.dictionaryReader.Quotas;
			}
		}

		private JsonReaderDelegator.DateTimeArrayJsonHelperWithString DateTimeArrayHelper
		{
			get
			{
				if (this.dateTimeArrayHelper == null)
				{
					this.dateTimeArrayHelper = new JsonReaderDelegator.DateTimeArrayJsonHelperWithString(this.dateTimeFormat);
				}
				return this.dateTimeArrayHelper;
			}
		}

		internal static XmlQualifiedName ParseQualifiedName(string qname)
		{
			string text2;
			string text;
			if (string.IsNullOrEmpty(qname))
			{
				text = (text2 = string.Empty);
			}
			else
			{
				qname = qname.Trim();
				int num = qname.IndexOf(':');
				if (num >= 0)
				{
					text2 = qname.Substring(0, num);
					text = qname.Substring(num + 1);
				}
				else
				{
					text2 = qname;
					text = string.Empty;
				}
			}
			return new XmlQualifiedName(text2, text);
		}

		internal override char ReadContentAsChar()
		{
			return XmlConvert.ToChar(base.ReadContentAsString());
		}

		internal override XmlQualifiedName ReadContentAsQName()
		{
			return JsonReaderDelegator.ParseQualifiedName(base.ReadContentAsString());
		}

		internal override char ReadElementContentAsChar()
		{
			return XmlConvert.ToChar(base.ReadElementContentAsString());
		}

		internal override byte[] ReadContentAsBase64()
		{
			if (this.isEndOfEmptyElement)
			{
				return new byte[0];
			}
			byte[] array;
			if (this.dictionaryReader == null)
			{
				XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateDictionaryReader(this.reader);
				array = ByteArrayHelperWithString.Instance.ReadArray(xmlDictionaryReader, "item", string.Empty, xmlDictionaryReader.Quotas.MaxArrayLength);
			}
			else
			{
				array = ByteArrayHelperWithString.Instance.ReadArray(this.dictionaryReader, "item", string.Empty, this.dictionaryReader.Quotas.MaxArrayLength);
			}
			return array;
		}

		internal override byte[] ReadElementContentAsBase64()
		{
			if (this.isEndOfEmptyElement)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Start element expected. Found {0}.", new object[] { "EndElement" })));
			}
			byte[] array;
			if (this.reader.IsStartElement() && this.reader.IsEmptyElement)
			{
				this.reader.Read();
				array = new byte[0];
			}
			else
			{
				this.reader.ReadStartElement();
				array = this.ReadContentAsBase64();
				this.reader.ReadEndElement();
			}
			return array;
		}

		internal override DateTime ReadContentAsDateTime()
		{
			return JsonReaderDelegator.ParseJsonDate(base.ReadContentAsString(), this.dateTimeFormat);
		}

		internal static DateTime ParseJsonDate(string originalDateTimeValue, DateTimeFormat dateTimeFormat)
		{
			if (dateTimeFormat == null)
			{
				return JsonReaderDelegator.ParseJsonDateInDefaultFormat(originalDateTimeValue);
			}
			return DateTime.ParseExact(originalDateTimeValue, dateTimeFormat.FormatString, dateTimeFormat.FormatProvider, dateTimeFormat.DateTimeStyles);
		}

		internal static DateTime ParseJsonDateInDefaultFormat(string originalDateTimeValue)
		{
			string text;
			if (!string.IsNullOrEmpty(originalDateTimeValue))
			{
				text = originalDateTimeValue.Trim();
			}
			else
			{
				text = originalDateTimeValue;
			}
			if (string.IsNullOrEmpty(text) || !text.StartsWith("/Date(", StringComparison.Ordinal) || !text.EndsWith(")/", StringComparison.Ordinal))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Invalid JSON dateTime string is specified: original value '{0}', start guide writer: {1}, end guard writer: {2}.", new object[] { originalDateTimeValue, "\\/Date(", ")\\/" })));
			}
			string text2 = text.Substring(6, text.Length - 8);
			DateTimeKind dateTimeKind = DateTimeKind.Utc;
			int num = text2.IndexOf('+', 1);
			if (num == -1)
			{
				num = text2.IndexOf('-', 1);
			}
			if (num != -1)
			{
				dateTimeKind = DateTimeKind.Local;
				text2 = text2.Substring(0, num);
			}
			long num2;
			try
			{
				num2 = long.Parse(text2, CultureInfo.InvariantCulture);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text2, "Int64", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text2, "Int64", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text2, "Int64", ex3));
			}
			long num3 = num2 * 10000L + JsonGlobals.unixEpochTicks;
			DateTime dateTime2;
			try
			{
				DateTime dateTime = new DateTime(num3, DateTimeKind.Utc);
				switch (dateTimeKind)
				{
				case DateTimeKind.Unspecified:
					return DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
				case DateTimeKind.Local:
					return dateTime.ToLocalTime();
				}
				dateTime2 = dateTime;
			}
			catch (ArgumentException ex4)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text2, "DateTime", ex4));
			}
			return dateTime2;
		}

		internal override DateTime ReadElementContentAsDateTime()
		{
			return JsonReaderDelegator.ParseJsonDate(base.ReadElementContentAsString(), this.dateTimeFormat);
		}

		internal bool TryReadJsonDateTimeArray(XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, int arrayLength, out DateTime[] array)
		{
			if (this.dictionaryReader == null || arrayLength != -1)
			{
				array = null;
				return false;
			}
			array = this.DateTimeArrayHelper.ReadArray(this.dictionaryReader, XmlDictionaryString.GetString(itemName), XmlDictionaryString.GetString(itemNamespace), base.GetArrayLengthQuota(context));
			context.IncrementItemCount(array.Length);
			return true;
		}

		internal override ulong ReadContentAsUnsignedLong()
		{
			string text = this.reader.ReadContentAsString();
			if (text == null || text.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(XmlObjectSerializer.TryAddLineInfo(this, global::System.Runtime.Serialization.SR.GetString("The value '{0}' cannot be parsed as the type '{1}'.", new object[] { text, "UInt64" }))));
			}
			ulong num;
			try
			{
				num = ulong.Parse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "UInt64", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "UInt64", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "UInt64", ex3));
			}
			return num;
		}

		internal override ulong ReadElementContentAsUnsignedLong()
		{
			if (this.isEndOfEmptyElement)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Start element expected. Found {0}.", new object[] { "EndElement" })));
			}
			string text = this.reader.ReadElementContentAsString();
			if (text == null || text.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(XmlObjectSerializer.TryAddLineInfo(this, global::System.Runtime.Serialization.SR.GetString("The value '{0}' cannot be parsed as the type '{1}'.", new object[] { text, "UInt64" }))));
			}
			ulong num;
			try
			{
				num = ulong.Parse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "UInt64", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "UInt64", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "UInt64", ex3));
			}
			return num;
		}

		private DateTimeFormat dateTimeFormat;

		private JsonReaderDelegator.DateTimeArrayJsonHelperWithString dateTimeArrayHelper;

		private class DateTimeArrayJsonHelperWithString : ArrayHelper<string, DateTime>
		{
			public DateTimeArrayJsonHelperWithString(DateTimeFormat dateTimeFormat)
			{
				this.dateTimeFormat = dateTimeFormat;
			}

			protected override int ReadArray(XmlDictionaryReader reader, string localName, string namespaceUri, DateTime[] array, int offset, int count)
			{
				XmlJsonReader.CheckArray(array, offset, count);
				int num = 0;
				while (num < count && reader.IsStartElement("item", string.Empty))
				{
					array[offset + num] = JsonReaderDelegator.ParseJsonDate(reader.ReadElementContentAsString(), this.dateTimeFormat);
					num++;
				}
				return num;
			}

			protected override void WriteArray(XmlDictionaryWriter writer, string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int count)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
			}

			private DateTimeFormat dateTimeFormat;
		}
	}
}
