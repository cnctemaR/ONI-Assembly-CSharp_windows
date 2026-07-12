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
			this._boundaryBytes = Encoding.ASCII.GetBytes(boundary);
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
			this._stream.Close();
		}

		internal override void Close()
		{
			this.Close(null);
			this._stream.Close();
		}

		private void Close(MultiAsyncResult multiResult)
		{
			this._bufferBuilder.Append(BaseWriter.s_crlf);
			this._bufferBuilder.Append(MimeWriter.s_DASHDASH);
			this._bufferBuilder.Append(this._boundaryBytes);
			this._bufferBuilder.Append(MimeWriter.s_DASHDASH);
			this._bufferBuilder.Append(BaseWriter.s_crlf);
			base.Flush(multiResult);
		}

		protected override void OnClose(object sender, EventArgs args)
		{
			if (this._contentStream != sender)
			{
				return;
			}
			this._contentStream.Flush();
			this._contentStream = null;
			this._writeBoundary = true;
			this._isInContent = false;
		}

		protected override void CheckBoundary()
		{
			if (this._writeBoundary)
			{
				this._bufferBuilder.Append(BaseWriter.s_crlf);
				this._bufferBuilder.Append(MimeWriter.s_DASHDASH);
				this._bufferBuilder.Append(this._boundaryBytes);
				this._bufferBuilder.Append(BaseWriter.s_crlf);
				this._writeBoundary = false;
			}
		}

		private static byte[] s_DASHDASH = new byte[] { 45, 45 };

		private byte[] _boundaryBytes;

		private bool _writeBoundary = true;
	}
}
