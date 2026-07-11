using System;
using System.IO;
using System.Text;

namespace System.Xml
{
	public interface IXmlTextReaderInitializer
	{
		void SetInput(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quota, OnXmlDictionaryReaderClose onClose);

		void SetInput(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quota, OnXmlDictionaryReaderClose onClose);
	}
}
