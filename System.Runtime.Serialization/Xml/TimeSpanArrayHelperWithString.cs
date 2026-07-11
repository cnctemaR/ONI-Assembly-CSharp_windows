using System;

namespace System.Xml
{
	internal class TimeSpanArrayHelperWithString : ArrayHelper<string, TimeSpan>
	{
		protected override int ReadArray(XmlDictionaryReader reader, string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			return reader.ReadArray(localName, namespaceUri, array, offset, count);
		}

		protected override void WriteArray(XmlDictionaryWriter writer, string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			writer.WriteArray(prefix, localName, namespaceUri, array, offset, count);
		}

		public static readonly TimeSpanArrayHelperWithString Instance = new TimeSpanArrayHelperWithString();
	}
}
