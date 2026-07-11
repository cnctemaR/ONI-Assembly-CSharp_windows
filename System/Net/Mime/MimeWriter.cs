using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace System.Net.Mime
{
	internal class MimeWriter : BaseWriter
	{
		internal MimeWriter(Stream stream, string boundary)
			: base(stream, false)
		{
			if (boundary == null)
			{
				throw new ArgumentNullException("boundary");
			}
			this.boundaryBytes = Encoding.ASCII.GetBytes(boundary);
		}

		internal override void WriteHeaders(NameValueCollection headers, bool allowUnicode)
		{
			if (headers == null)
			{
				throw new ArgumentNullException("headers");
			}
			foreach (object obj in headers)
			{
				string text = (string)obj;
				base.WriteHeader(text, headers[text], allowUnicode);
			}
		}

		internal IAsyncResult BeginClose(AsyncCallback callback, object state)
		{
			MultiAsyncResult multiAsyncResult = new MultiAsyncResult(this, callback, state);
			this.Close(multiAsyncResult);
			multiAsyncResult.CompleteSequence();
			return multiAsyncResult;
		}

		internal void EndClose(IAsyncResult result)
		{
			MultiAsyncResult.End(result);
			this.stream.Close();
		}

		internal override void Close()
		{
			this.Close(null);
			this.stream.Close();
		}

		private void Close(MultiAsyncResult multiResult)
		{
			this.bufferBuilder.Append(BaseWriter.CRLF);
			this.bufferBuilder.Append(MimeWriter.DASHDASH);
			this.bufferBuilder.Append(this.boundaryBytes);
			this.bufferBuilder.Append(MimeWriter.DASHDASH);
			this.bufferBuilder.Append(BaseWriter.CRLF);
			base.Flush(multiResult);
		}

		protected override void OnClose(object sender, EventArgs args)
		{
			if (this.contentStream != sender)
			{
				return;
			}
			this.contentStream.Flush();
			this.contentStream = null;
			this.writeBoundary = true;
			this.isInContent = false;
		}

		protected override void CheckBoundary()
		{
			if (this.writeBoundary)
			{
				this.bufferBuilder.Append(BaseWriter.CRLF);
				this.bufferBuilder.Append(MimeWriter.DASHDASH);
				this.bufferBuilder.Append(this.boundaryBytes);
				this.bufferBuilder.Append(BaseWriter.CRLF);
				this.writeBoundary = false;
			}
		}

		private static byte[] DASHDASH = new byte[] { 45, 45 };

		private byte[] boundaryBytes;

		private bool writeBoundary = true;
	}
}
