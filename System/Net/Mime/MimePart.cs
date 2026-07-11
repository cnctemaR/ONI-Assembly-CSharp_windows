using System;
using System.IO;
using System.Net.Mail;

namespace System.Net.Mime
{
	internal class MimePart : MimeBasePart, IDisposable
	{
		internal MimePart()
		{
		}

		public void Dispose()
		{
			if (this.stream != null)
			{
				this.stream.Close();
			}
		}

		internal Stream Stream
		{
			get
			{
				return this.stream;
			}
		}

		internal ContentDisposition ContentDisposition
		{
			get
			{
				return this.contentDisposition;
			}
			set
			{
				this.contentDisposition = value;
				if (value == null)
				{
					((HeaderCollection)base.Headers).InternalRemove(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition));
					return;
				}
				this.contentDisposition.PersistIfNeeded((HeaderCollection)base.Headers, true);
			}
		}

		internal TransferEncoding TransferEncoding
		{
			get
			{
				string text = base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)];
				if (text.Equals("base64", StringComparison.OrdinalIgnoreCase))
				{
					return TransferEncoding.Base64;
				}
				if (text.Equals("quoted-printable", StringComparison.OrdinalIgnoreCase))
				{
					return TransferEncoding.QuotedPrintable;
				}
				if (text.Equals("7bit", StringComparison.OrdinalIgnoreCase))
				{
					return TransferEncoding.SevenBit;
				}
				if (text.Equals("8bit", StringComparison.OrdinalIgnoreCase))
				{
					return TransferEncoding.EightBit;
				}
				return TransferEncoding.Unknown;
			}
			set
			{
				if (value == TransferEncoding.Base64)
				{
					base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "base64";
					return;
				}
				if (value == TransferEncoding.QuotedPrintable)
				{
					base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "quoted-printable";
					return;
				}
				if (value == TransferEncoding.SevenBit)
				{
					base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "7bit";
					return;
				}
				if (value == TransferEncoding.EightBit)
				{
					base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "8bit";
					return;
				}
				throw new NotSupportedException(global::SR.GetString("The MIME transfer encoding '{0}' is not supported.", new object[] { value }));
			}
		}

		internal void SetContent(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (this.streamSet)
			{
				this.stream.Close();
				this.stream = null;
				this.streamSet = false;
			}
			this.stream = stream;
			this.streamSet = true;
			this.streamUsedOnce = false;
			this.TransferEncoding = TransferEncoding.Base64;
		}

		internal void SetContent(Stream stream, string name, string mimeType)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (mimeType != null && mimeType != string.Empty)
			{
				this.contentType = new ContentType(mimeType);
			}
			if (name != null && name != string.Empty)
			{
				base.ContentType.Name = name;
			}
			this.SetContent(stream);
		}

		internal void SetContent(Stream stream, ContentType contentType)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.contentType = contentType;
			this.SetContent(stream);
		}

		internal void Complete(IAsyncResult result, Exception e)
		{
			MimePart.MimePartContext mimePartContext = (MimePart.MimePartContext)result.AsyncState;
			if (mimePartContext.completed)
			{
				throw e;
			}
			try
			{
				if (mimePartContext.outputStream != null)
				{
					mimePartContext.outputStream.Close();
				}
			}
			catch (Exception ex)
			{
				if (e == null)
				{
					e = ex;
				}
			}
			mimePartContext.completed = true;
			mimePartContext.result.InvokeCallback(e);
		}

		internal void ReadCallback(IAsyncResult result)
		{
			if (result.CompletedSynchronously)
			{
				return;
			}
			((MimePart.MimePartContext)result.AsyncState).completedSynchronously = false;
			try
			{
				this.ReadCallbackHandler(result);
			}
			catch (Exception ex)
			{
				this.Complete(result, ex);
			}
		}

		internal void ReadCallbackHandler(IAsyncResult result)
		{
			MimePart.MimePartContext mimePartContext = (MimePart.MimePartContext)result.AsyncState;
			mimePartContext.bytesLeft = this.Stream.EndRead(result);
			if (mimePartContext.bytesLeft > 0)
			{
				IAsyncResult asyncResult = mimePartContext.outputStream.BeginWrite(mimePartContext.buffer, 0, mimePartContext.bytesLeft, this.writeCallback, mimePartContext);
				if (asyncResult.CompletedSynchronously)
				{
					this.WriteCallbackHandler(asyncResult);
					return;
				}
			}
			else
			{
				this.Complete(result, null);
			}
		}

		internal void WriteCallback(IAsyncResult result)
		{
			if (result.CompletedSynchronously)
			{
				return;
			}
			((MimePart.MimePartContext)result.AsyncState).completedSynchronously = false;
			try
			{
				this.WriteCallbackHandler(result);
			}
			catch (Exception ex)
			{
				this.Complete(result, ex);
			}
		}

		internal void WriteCallbackHandler(IAsyncResult result)
		{
			MimePart.MimePartContext mimePartContext = (MimePart.MimePartContext)result.AsyncState;
			mimePartContext.outputStream.EndWrite(result);
			IAsyncResult asyncResult = this.Stream.BeginRead(mimePartContext.buffer, 0, mimePartContext.buffer.Length, this.readCallback, mimePartContext);
			if (asyncResult.CompletedSynchronously)
			{
				this.ReadCallbackHandler(asyncResult);
			}
		}

		internal Stream GetEncodedStream(Stream stream)
		{
			Stream stream2 = stream;
			if (this.TransferEncoding == TransferEncoding.Base64)
			{
				stream2 = new Base64Stream(stream2, new Base64WriteStateInfo());
			}
			else if (this.TransferEncoding == TransferEncoding.QuotedPrintable)
			{
				stream2 = new QuotedPrintableStream(stream2, true);
			}
			else if (this.TransferEncoding == TransferEncoding.SevenBit || this.TransferEncoding == TransferEncoding.EightBit)
			{
				stream2 = new EightBitStream(stream2);
			}
			return stream2;
		}

		internal void ContentStreamCallbackHandler(IAsyncResult result)
		{
			MimePart.MimePartContext mimePartContext = (MimePart.MimePartContext)result.AsyncState;
			Stream stream = mimePartContext.writer.EndGetContentStream(result);
			mimePartContext.outputStream = this.GetEncodedStream(stream);
			this.readCallback = new AsyncCallback(this.ReadCallback);
			this.writeCallback = new AsyncCallback(this.WriteCallback);
			IAsyncResult asyncResult = this.Stream.BeginRead(mimePartContext.buffer, 0, mimePartContext.buffer.Length, this.readCallback, mimePartContext);
			if (asyncResult.CompletedSynchronously)
			{
				this.ReadCallbackHandler(asyncResult);
			}
		}

		internal void ContentStreamCallback(IAsyncResult result)
		{
			if (result.CompletedSynchronously)
			{
				return;
			}
			((MimePart.MimePartContext)result.AsyncState).completedSynchronously = false;
			try
			{
				this.ContentStreamCallbackHandler(result);
			}
			catch (Exception ex)
			{
				this.Complete(result, ex);
			}
		}

		internal override IAsyncResult BeginSend(BaseWriter writer, AsyncCallback callback, bool allowUnicode, object state)
		{
			base.PrepareHeaders(allowUnicode);
			writer.WriteHeaders(base.Headers, allowUnicode);
			MimeBasePart.MimePartAsyncResult mimePartAsyncResult = new MimeBasePart.MimePartAsyncResult(this, state, callback);
			MimePart.MimePartContext mimePartContext = new MimePart.MimePartContext(writer, mimePartAsyncResult);
			this.ResetStream();
			this.streamUsedOnce = true;
			IAsyncResult asyncResult = writer.BeginGetContentStream(new AsyncCallback(this.ContentStreamCallback), mimePartContext);
			if (asyncResult.CompletedSynchronously)
			{
				this.ContentStreamCallbackHandler(asyncResult);
			}
			return mimePartAsyncResult;
		}

		internal override void Send(BaseWriter writer, bool allowUnicode)
		{
			if (this.Stream != null)
			{
				byte[] array = new byte[17408];
				base.PrepareHeaders(allowUnicode);
				writer.WriteHeaders(base.Headers, allowUnicode);
				Stream stream = writer.GetContentStream();
				stream = this.GetEncodedStream(stream);
				this.ResetStream();
				this.streamUsedOnce = true;
				int num;
				while ((num = this.Stream.Read(array, 0, 17408)) > 0)
				{
					stream.Write(array, 0, num);
				}
				stream.Close();
			}
		}

		internal void ResetStream()
		{
			if (!this.streamUsedOnce)
			{
				return;
			}
			if (this.Stream.CanSeek)
			{
				this.Stream.Seek(0L, SeekOrigin.Begin);
				this.streamUsedOnce = false;
				return;
			}
			throw new InvalidOperationException(global::SR.GetString("One of the streams has already been used and can't be reset to the origin."));
		}

		private Stream stream;

		private bool streamSet;

		private bool streamUsedOnce;

		private AsyncCallback readCallback;

		private AsyncCallback writeCallback;

		private const int maxBufferSize = 17408;

		internal class MimePartContext
		{
			internal MimePartContext(BaseWriter writer, LazyAsyncResult result)
			{
				this.writer = writer;
				this.result = result;
				this.buffer = new byte[17408];
			}

			internal Stream outputStream;

			internal LazyAsyncResult result;

			internal int bytesLeft;

			internal BaseWriter writer;

			internal byte[] buffer;

			internal bool completed;

			internal bool completedSynchronously = true;
		}
	}
}
