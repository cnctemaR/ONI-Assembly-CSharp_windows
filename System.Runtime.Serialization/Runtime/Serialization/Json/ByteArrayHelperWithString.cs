using System;
using System.Globalization;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class ByteArrayHelperWithString : ArrayHelper<string, byte>
	{
		internal void WriteArray(XmlWriter writer, byte[] array, int offset, int count)
		{
			XmlJsonReader.CheckArray(array, offset, count);
			writer.WriteAttributeString(string.Empty, "type", string.Empty, "array");
			for (int i = 0; i < count; i++)
			{
				writer.WriteStartElement("item", string.Empty);
				writer.WriteAttributeString(string.Empty, "type", string.Empty, "number");
				writer.WriteValue((int)array[offset + i]);
				writer.WriteEndElement();
			}
		}

		protected override int ReadArray(XmlDictionaryReader reader, string localName, string namespaceUri, byte[] array, int offset, int count)
		{
			XmlJsonReader.CheckArray(array, offset, count);
			int num = 0;
			while (num < count && reader.IsStartElement("item", string.Empty))
			{
				array[offset + num] = this.ToByte(reader.ReadElementContentAsInt());
				num++;
			}
			return num;
		}

		protected override void WriteArray(XmlDictionaryWriter writer, string prefix, string localName, string namespaceUri, byte[] array, int offset, int count)
		{
			this.WriteArray(writer, array, offset, count);
		}

		private void ThrowConversionException(string value, string type)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("The value '{0}' cannot be parsed as the type '{1}'.", new object[] { value, type })));
		}

		private byte ToByte(int value)
		{
			if (value < 0 || value > 255)
			{
				this.ThrowConversionException(value.ToString(NumberFormatInfo.CurrentInfo), "Byte");
			}
			return (byte)value;
		}

		public static readonly ByteArrayHelperWithString Instance = new ByteArrayHelperWithString();
	}
}
