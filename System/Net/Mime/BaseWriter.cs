using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Mail;

namespace System.Net.Mime
{
	internal abstract class BaseWriter
	{
		protected BaseWriter(Stream stream, bool shouldEncodeLeadingDots)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.stream = stream;
			this.shouldEncodeLeadingDots = shouldEncodeLeadingDots;
			this.onCloseHandler = new EventHandler(this.OnClose);
			this.bufferBuilder = new BufferBuilder();
			this.lineLength = BaseWriter.DefaultLineLength;
		}

		internal abstract void WriteHeaders(NameValueCollection headers, bool allowUnicode);

		internal void WriteHeader(string name, string value, bool allowUnicode)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.isInContent)
			{
				throw new InvalidOperationException(global::SR.GetString("This operation cannot be performed while in content."));
			}
			this.CheckBoundary();
			this.bufferBuilder.Append(name);
			this.bufferBuilder.Append(": ");
			this.WriteAndFold(value, name.Length + 2, allowUnicode);
			this.bufferBuilder.Append(BaseWriter.CRLF);
		}

		private void WriteAndFold(string value, int charsAlreadyOnLine, bool allowUnicode)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < value.Length; i++)
			{
				if (MailBnfHelper.IsFWSAt(value, i))
				{
					i += 2;
					this.bufferBuilder.Append(value, num2, i - num2, allowUnicode);
					num2 = i;
					num = i;
					charsAlreadyOnLine = 0;
				}
				else if (i - num2 > this.lineLength - charsAlreadyOnLine && num != num2)
				{
					this.bufferBuilder.Append(value, num2, num - num2, allowUnicode);
					this.bufferBuilder.Append(BaseWriter.CRLF);
					num2 = num;
					charsAlreadyOnLine = 0;
				}
				else if (value[i] == MailBnfHelper.Space || value[i] == MailBnfHelper.Tab)
				{
					num = i;
				}
			}
			if (value.Length - num2 > 0)
			{
				this.bufferBuilder.Append(value, num2, value.Length - num2, allowUnicode);
			}
		}

		internal Stream GetContentStream()
		{
			return this.GetContentStream(null);
		}

		private Stream GetContentStream(MultiAsyncResult multiResult)
		{
			if (this.isInContent)
			{
				throw new InvalidOperationException(global::SR.GetString("This operation cannot be performed while in content."));
			}
			this.isInContent = true;
			this.CheckBoundary();
			this.bufferBuilder.Append(BaseWriter.CRLF);
			this.Flush(multiResult);
			ClosableStream closableStream = new ClosableStream(new EightBitStream(this.stream, this.shouldEncodeLeadingDots), this.onCloseHandler);
			this.contentStream = closableStream;
			return closableStream;
		}

		internal IAsyncResult BeginGetContentStream(AsyncCallback callback, object state)
		{
			MultiAsyncResult multiAsyncResult = new MultiAsyncResult(this, callback, state);
			Stream stream = this.GetContentStream(multiAsyncResult);
			if (!(multiAsyncResult.Result is Exception))
			{
				multiAsyncResult.Result = stream;
			}
			multiAsyncResult.CompleteSequence();
			return multiAsyncResult;
		}

		internal Stream EndGetContentStream(IAsyncResult result)
		{
			object obj = MultiAsyncResult.End(result);
			if (obj is Exception)
			{
				throw (Exception)obj;
			}
			return (Stream)obj;
		}

		protected void Flush(MultiAsyncResult multiResult)
		{
			if (this.bufferBuilder.Length > 0)
			{
				if (multiResult != null)
				{
					multiResult.Enter();
					IAsyncResult asyncResult = this.stream.BeginWrite(this.bufferBuilder.GetBuffer(), 0, this.bufferBuilder.Length, BaseWriter.onWrite, multiResult);
					if (asyncResult.CompletedSynchronously)
					{
						this.stream.EndWrite(asyncResult);
						multiResult.Leave();
					}
				}
				else
				{
					this.stream.Write(this.bufferBuilder.GetBuffer(), 0, this.bufferBuilder.Length);
				}
				this.bufferBuilder.Reset();
			}
		}

		protected static void OnWrite(IAsyncResult result)
		{
			if (!result.CompletedSynchronously)
			{
				MultiAsyncResult multiAsyncResult = (MultiAsyncResult)result.AsyncState;
				BaseWriter baseWriter = (BaseWriter)multiAsyncResult.Context;
				try
				{
					baseWriter.stream.EndWrite(result);
					multiAsyncResult.Leave();
				}
				catch (Exception ex)
				{
					multiAsyncResult.Leave(ex);
				}
			}
		}

		internal abstract void Close();

		protected abstract void OnClose(object sender, EventArgs args);

		protected virtual void CheckBoundary()
		{
		}

		private static int DefaultLineLength = 76;

		private static AsyncCallback onWrite = new AsyncCallback(BaseWriter.OnWrite);

		protected static byte[] CRLF = new byte[] { 13, 10 };

		protected BufferBuilder bufferBuilder;

		protected Stream contentStream;

		protected bool isInContent;

		protected Stream stream;

		private int lineLength;

		private EventHandler onCloseHandler;

		private bool shouldEncodeLeadingDots;
	}
}
