using System;

namespace System.Xml.Serialization
{
	internal class XmlSerializationPrimitiveWriter : XmlSerializationWriter
	{
		internal void Write_string(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("string", "");
				return;
			}
			base.TopLevelElement();
			base.WriteNullableStringLiteral("string", "", (string)o);
		}

		internal void Write_int(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("int", "");
				return;
			}
			base.WriteElementStringRaw("int", "", XmlConvert.ToString((int)o));
		}

		internal void Write_boolean(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("boolean", "");
				return;
			}
			base.WriteElementStringRaw("boolean", "", XmlConvert.ToString((bool)o));
		}

		internal void Write_short(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("short", "");
				return;
			}
			base.WriteElementStringRaw("short", "", XmlConvert.ToString((short)o));
		}

		internal void Write_long(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("long", "");
				return;
			}
			base.WriteElementStringRaw("long", "", XmlConvert.ToString((long)o));
		}

		internal void Write_float(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("float", "");
				return;
			}
			base.WriteElementStringRaw("float", "", XmlConvert.ToString((float)o));
		}

		internal void Write_double(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("double", "");
				return;
			}
			base.WriteElementStringRaw("double", "", XmlConvert.ToString((double)o));
		}

		internal void Write_decimal(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("decimal", "");
				return;
			}
			base.WriteElementStringRaw("decimal", "", XmlConvert.ToString((decimal)o));
		}

		internal void Write_dateTime(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("dateTime", "");
				return;
			}
			base.WriteElementStringRaw("dateTime", "", XmlSerializationWriter.FromDateTime((DateTime)o));
		}

		internal void Write_unsignedByte(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("unsignedByte", "");
				return;
			}
			base.WriteElementStringRaw("unsignedByte", "", XmlConvert.ToString((byte)o));
		}

		internal void Write_byte(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("byte", "");
				return;
			}
			base.WriteElementStringRaw("byte", "", XmlConvert.ToString((sbyte)o));
		}

		internal void Write_unsignedShort(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("unsignedShort", "");
				return;
			}
			base.WriteElementStringRaw("unsignedShort", "", XmlConvert.ToString((ushort)o));
		}

		internal void Write_unsignedInt(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("unsignedInt", "");
				return;
			}
			base.WriteElementStringRaw("unsignedInt", "", XmlConvert.ToString((uint)o));
		}

		internal void Write_unsignedLong(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("unsignedLong", "");
				return;
			}
			base.WriteElementStringRaw("unsignedLong", "", XmlConvert.ToString((ulong)o));
		}

		internal void Write_base64Binary(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("base64Binary", "");
				return;
			}
			base.TopLevelElement();
			base.WriteNullableStringLiteralRaw("base64Binary", "", XmlSerializationWriter.FromByteArrayBase64((byte[])o));
		}

		internal void Write_guid(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("guid", "");
				return;
			}
			base.WriteElementStringRaw("guid", "", XmlConvert.ToString((Guid)o));
		}

		internal void Write_TimeSpan(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("TimeSpan", "");
				return;
			}
			TimeSpan timeSpan = (TimeSpan)o;
			base.WriteElementStringRaw("TimeSpan", "", XmlConvert.ToString(timeSpan));
		}

		internal void Write_char(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("char", "");
				return;
			}
			base.WriteElementString("char", "", XmlSerializationWriter.FromChar((char)o));
		}

		internal void Write_QName(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("QName", "");
				return;
			}
			base.TopLevelElement();
			base.WriteNullableQualifiedNameLiteral("QName", "", (XmlQualifiedName)o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
