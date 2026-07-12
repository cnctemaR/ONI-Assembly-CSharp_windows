using System;
using System.IO;
using System.Text;

namespace System.Net.Mime
{
	internal class EncodedStreamFactory
	{
		internal IEncodableStream GetEncoder(TransferEncoding encoding, Stream stream)
		{
			if (encoding == TransferEncoding.Base64)
			{
				return new Base64Stream(stream, new Base64WriteStateInfo());
			}
			if (encoding == TransferEncoding.QuotedPrintable)
			{
				return new QuotedPrintableStream(stream, true);
			}
			if (encoding == TransferEncoding.SevenBit || encoding == TransferEncoding.EightBit)
			{
				return new EightBitStream(stream);
			}
			throw new NotSupportedException();
		}

		internal IEncodableStream GetEncoderForHeader(Encoding encoding, bool useBase64Encoding, int headerTextLength)
		{
			byte[] array = this.CreateHeader(encoding, useBase64Encoding);
			byte[] array2 = this.CreateFooter();
			if (useBase64Encoding)
			{
				return new Base64Stream((Base64WriteStateInfo)new Base64WriteStateInfo(1024, array, array2, 70, headerTextLength));
			}
			return new QEncodedStream(new WriteStateInfoBase(1024, array, array2, 70, headerTextLength));
		}

		protected byte[] CreateHeader(Encoding encoding, bool useBase64Encoding)
		{
			return Encoding.ASCII.GetBytes("=?" + encoding.HeaderName + "?" + (useBase64Encoding ? "B?" : "Q?"));
		}

		protected byte[] CreateFooter()
		{
			return new byte[] { 63, 61 };
		}

		internal const int DefaultMaxLineLength = 70;

		private const int InitialBufferSize = 1024;
	}
}
