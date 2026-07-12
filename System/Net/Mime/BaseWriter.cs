using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Mail;
using System.Runtime.ExceptionServices;

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
			this._stream = stream;
			this._shouldEncodeLeadingDots = shouldEncodeLeadingDots;
			this._onCloseHandler = new EventHandler(this.OnClose);
			this._bufferBuilder = new BufferBuilder();
			this._lineLength = 76;
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
			if (this._isInContent)
			{
				throw new InvalidOperationException("This operation cannot be performed while in content.");
			}
			this.CheckBoundary();
			this._bufferBuilder.Append(name);
			this._bufferBuilder.Append(": ");
			this.WriteAndFold(value, name.Length + 2, allowUnicode);
			this._bufferBuilder.Append(BaseWriter.s_crlf);
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
					this._bufferBuilder.Append(value, num2, i - num2, allowUnicode);
					num2 = i;
					num = i;
					charsAlreadyOnLine = 0;
				}
				else if (i - num2 > this._lineLength - charsAlreadyOnLine && num != num2)
				{
					this._bufferBuilder.Append(value, num2, num - num2, allowUnicode);
					this._bufferBuilder.Append(BaseWriter.s_crlf);
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
				this._bufferBuilder.Append(value, num2, value.Length - num2, allowUnicode);
			}
		}

		internal Stream GetContentStream()
		{
			return this.GetContentStream(null);
		}

		private Stream GetContentStream(MultiAsyncResult multiResult)
		{
			if (this._isInContent)
			{
				throw new InvalidOperationException("This operation cannot be performed while in content.");
			}
			this._isInContent = true;
			this.CheckBoundary();
			this._bufferBuilder.Append(BaseWriter.s_crlf);
			this.Flush(multiResult);
			ClosableStream closableStream = new ClosableStream(new EightBitStream(this._stream, this._shouldEncodeLeadingDots), this._onCloseHandler);
			this._contentStream = closableStream;
			return closableStream;
		}

		internal IAsyncResult BeginGetContentStream(AsyncCallback callback, object state)
		{
			MultiAsyncResult multiAsyncResult = new MultiAsyncResult(this, callback, state);
			Stream contentStream = this.GetContentStream(multiAsyncResult);
			if (!(multiAsyncResult.Result is Exception))
			{
				multiAsyncResult.Result = contentStream;
			}
			multiAsyncResult.CompleteSequence();
			return multiAsyncResult;
		}

		internal Stream EndGetContentStream(IAsyncResult result)
		{
			object obj = MultiAsyncResult.End(result);
			Exception ex = obj as Exception;
			if (ex != null)
			{
				ExceptionDispatchInfo.Throw(ex);
			}
			return (Stream)obj;
		}

		protected void Flush(MultiAsyncResult multiResult)
		{
			if (this._bufferBuilder.Length > 0)
			{
				if (multiResult != null)
				{
					multiResult.Enter();
					IAsyncResult asyncResult = this._stream.BeginWrite(this._bufferBuilder.GetBuffer(), 0, this._bufferBuilder.Length, BaseWriter.s_onWrite, multiResult);
					if (asyncResult.CompletedSynchronously)
					{
						this._stream.EndWrite(asyncResult);
						multiResult.Leave();
					}
				}
				else
				{
					this._stream.Write(this._bufferBuilder.GetBuffer(), 0, this._bufferBuilder.Length);
				}
				this._bufferBuilder.Reset();
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
					baseWriter._stream.EndWrite(result);
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

		private const int DefaultLineLength = 76;

		private static readonly AsyncCallback s_onWrite = new AsyncCallback(BaseWriter.OnWrite);

		protected static readonly byte[] s_crlf = new byte[] { 13, 10 };

		protected readonly BufferBuilder _bufferBuilder;

		protected readonly Stream _stream;

		private readonly EventHandler _onCloseHandler;

		private readonly bool _shouldEncodeLeadingDots;

		private int _lineLength;

		protected Stream _contentStream;

		protected bool _isInContent;
	}
}
