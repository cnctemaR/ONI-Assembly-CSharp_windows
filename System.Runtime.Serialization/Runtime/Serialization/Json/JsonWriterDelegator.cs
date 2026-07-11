using System;
using System.Globalization;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonWriterDelegator : XmlWriterDelegator
	{
		public JsonWriterDelegator(XmlWriter writer)
			: base(writer)
		{
		}

		public JsonWriterDelegator(XmlWriter writer, DateTimeFormat dateTimeFormat)
			: this(writer)
		{
			this.dateTimeFormat = dateTimeFormat;
		}

		internal override void WriteChar(char value)
		{
			base.WriteString(XmlConvert.ToString(value));
		}

		internal override void WriteBase64(byte[] bytes)
		{
			if (bytes == null)
			{
				return;
			}
			ByteArrayHelperWithString.Instance.WriteArray(base.Writer, bytes, 0, bytes.Length);
		}

		internal override void WriteQName(XmlQualifiedName value)
		{
			if (value != XmlQualifiedName.Empty)
			{
				this.writer.WriteString(value.Name);
				this.writer.WriteString(":");
				this.writer.WriteString(value.Namespace);
			}
		}

		internal override void WriteUnsignedLong(ulong value)
		{
			this.WriteDecimal(value);
		}

		internal override void WriteDecimal(decimal value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteDecimal(value);
		}

		internal override void WriteDouble(double value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteDouble(value);
		}

		internal override void WriteFloat(float value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteFloat(value);
		}

		internal override void WriteLong(long value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteLong(value);
		}

		internal override void WriteSignedByte(sbyte value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteSignedByte(value);
		}

		internal override void WriteUnsignedInt(uint value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteUnsignedInt(value);
		}

		internal override void WriteUnsignedShort(ushort value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteUnsignedShort(value);
		}

		internal override void WriteUnsignedByte(byte value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteUnsignedByte(value);
		}

		internal override void WriteShort(short value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteShort(value);
		}

		internal override void WriteBoolean(bool value)
		{
			this.writer.WriteAttributeString("type", "boolean");
			base.WriteBoolean(value);
		}

		internal override void WriteInt(int value)
		{
			this.writer.WriteAttributeString("type", "number");
			base.WriteInt(value);
		}

		internal void WriteJsonBooleanArray(bool[] value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			for (int i = 0; i < value.Length; i++)
			{
				base.WriteBoolean(value[i], itemName, itemNamespace);
			}
		}

		internal void WriteJsonDateTimeArray(DateTime[] value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			for (int i = 0; i < value.Length; i++)
			{
				base.WriteDateTime(value[i], itemName, itemNamespace);
			}
		}

		internal void WriteJsonDecimalArray(decimal[] value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			for (int i = 0; i < value.Length; i++)
			{
				base.WriteDecimal(value[i], itemName, itemNamespace);
			}
		}

		internal void WriteJsonInt32Array(int[] value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			for (int i = 0; i < value.Length; i++)
			{
				base.WriteInt(value[i], itemName, itemNamespace);
			}
		}

		internal void WriteJsonInt64Array(long[] value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			for (int i = 0; i < value.Length; i++)
			{
				base.WriteLong(value[i], itemName, itemNamespace);
			}
		}

		internal override void WriteDateTime(DateTime value)
		{
			if (this.dateTimeFormat == null)
			{
				this.WriteDateTimeInDefaultFormat(value);
				return;
			}
			this.writer.WriteString(value.ToString(this.dateTimeFormat.FormatString, this.dateTimeFormat.FormatProvider));
		}

		private void WriteDateTimeInDefaultFormat(DateTime value)
		{
			if (value.Kind != DateTimeKind.Utc)
			{
				long num;
				if (!LocalAppContextSwitches.DoNotUseTimeZoneInfo)
				{
					num = value.Ticks - TimeZoneInfo.Local.GetUtcOffset(value).Ticks;
				}
				else
				{
					num = value.Ticks - TimeZone.CurrentTimeZone.GetUtcOffset(value).Ticks;
				}
				if (num > DateTime.MaxValue.Ticks || num < DateTime.MinValue.Ticks)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("JSON DateTime is out of range."), new ArgumentOutOfRangeException("value")));
				}
			}
			this.writer.WriteString("/Date(");
			this.writer.WriteValue((value.ToUniversalTime().Ticks - JsonGlobals.unixEpochTicks) / 10000L);
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
			case DateTimeKind.Local:
			{
				TimeSpan timeSpan;
				if (!LocalAppContextSwitches.DoNotUseTimeZoneInfo)
				{
					timeSpan = TimeZoneInfo.Local.GetUtcOffset(value.ToLocalTime());
				}
				else
				{
					timeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(value.ToLocalTime());
				}
				if (timeSpan.Ticks < 0L)
				{
					this.writer.WriteString("-");
				}
				else
				{
					this.writer.WriteString("+");
				}
				int num2 = Math.Abs(timeSpan.Hours);
				this.writer.WriteString((num2 < 10) ? ("0" + num2) : num2.ToString(CultureInfo.InvariantCulture));
				int num3 = Math.Abs(timeSpan.Minutes);
				this.writer.WriteString((num3 < 10) ? ("0" + num3) : num3.ToString(CultureInfo.InvariantCulture));
				break;
			}
			}
			this.writer.WriteString(")/");
		}

		internal void WriteJsonSingleArray(float[] value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			for (int i = 0; i < value.Length; i++)
			{
				base.WriteFloat(value[i], itemName, itemNamespace);
			}
		}

		internal void WriteJsonDoubleArray(double[] value, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
		{
			for (int i = 0; i < value.Length; i++)
			{
				base.WriteDouble(value[i], itemName, itemNamespace);
			}
		}

		internal override void WriteStartElement(string prefix, string localName, string ns)
		{
			if (localName != null && localName.Length == 0)
			{
				base.WriteStartElement("item", "item");
				base.WriteAttributeString(null, "item", null, localName);
				return;
			}
			base.WriteStartElement(prefix, localName, ns);
		}

		private DateTimeFormat dateTimeFormat;
	}
}
