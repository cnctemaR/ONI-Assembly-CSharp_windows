using System;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlRawWriterBase64Encoder : Base64Encoder
	{
		internal XmlRawWriterBase64Encoder(XmlRawWriter rawWriter)
		{
			this.rawWriter = rawWriter;
		}

		internal override void WriteChars(char[] chars, int index, int count)
		{
			this.rawWriter.WriteRaw(chars, index, count);
		}

		internal override Task WriteCharsAsync(char[] chars, int index, int count)
		{
			return this.rawWriter.WriteRawAsync(chars, index, count);
		}

		private XmlRawWriter rawWriter;
	}
}
