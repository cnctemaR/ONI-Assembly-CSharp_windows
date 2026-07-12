using System;
using System.IO;
using System.Runtime.Serialization;

namespace System.Xml
{
	internal class MimeMessageReader
	{
		public MimeMessageReader(Stream stream)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			this.reader = new DelimittedStreamReader(stream);
			this.mimeHeaderReader = new MimeHeaderReader(this.reader.GetNextStream(MimeMessageReader.CRLFCRLF));
		}

		public Stream GetContentStream()
		{
			if (this.getContentStreamCalled)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("On MimeMessage, GetContentStream method is already called.")));
			}
			this.mimeHeaderReader.Close();
			Stream nextStream = this.reader.GetNextStream(null);
			this.getContentStreamCalled = true;
			return nextStream;
		}

		public MimeHeaders ReadHeaders(int maxBuffer, ref int remaining)
		{
			MimeHeaders mimeHeaders = new MimeHeaders();
			while (this.mimeHeaderReader.Read(maxBuffer, ref remaining))
			{
				mimeHeaders.Add(this.mimeHeaderReader.Name, this.mimeHeaderReader.Value, ref remaining);
			}
			return mimeHeaders;
		}

		private static byte[] CRLFCRLF = new byte[] { 13, 10, 13, 10 };

		private bool getContentStreamCalled;

		private MimeHeaderReader mimeHeaderReader;

		private DelimittedStreamReader reader;
	}
}
