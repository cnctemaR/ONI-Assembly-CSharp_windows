using System;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;

namespace System.Xml
{
	internal abstract class XmlStreamNodeWriter : XmlNodeWriter
	{
		protected XmlStreamNodeWriter()
		{
			this.buffer = new byte[512];
			this.encoding = XmlStreamNodeWriter.UTF8Encoding;
		}

		protected void SetOutput(Stream stream, bool ownsStream, Encoding encoding)
		{
			this.stream = stream;
			this.ownsStream = ownsStream;
			this.offset = 0;
			if (encoding != null)
			{
				this.encoding = encoding;
			}
		}

		public Stream Stream
		{
			get
			{
				return this.stream;
			}
			set
			{
				this.stream = value;
			}
		}

		public byte[] StreamBuffer
		{
			get
			{
				return this.buffer;
			}
		}

		public int BufferOffset
		{
			get
			{
				return this.offset;
			}
		}

		public int Position
		{
			get
			{
				return (int)this.stream.Position + this.offset;
			}
		}

		protected byte[] GetBuffer(int count, out int offset)
		{
			int num = this.offset;
			if (num + count <= 512)
			{
				offset = num;
			}
			else
			{
				this.FlushBuffer();
				offset = 0;
			}
			return this.buffer;
		}

		internal AsyncCompletionResult GetBufferAsync(XmlStreamNodeWriter.GetBufferAsyncEventArgs getBufferState)
		{
			int count = getBufferState.Arguments.Count;
			int num = this.offset;
			int num2;
			if (num + count <= 512)
			{
				num2 = num;
			}
			else
			{
				if (XmlStreamNodeWriter.onGetFlushComplete == null)
				{
					XmlStreamNodeWriter.onGetFlushComplete = new AsyncEventArgsCallback(XmlStreamNodeWriter.GetBufferFlushComplete);
				}
				if (this.flushBufferState == null)
				{
					this.flushBufferState = new AsyncEventArgs<object>();
				}
				this.flushBufferState.Set(XmlStreamNodeWriter.onGetFlushComplete, getBufferState, this);
				if (this.FlushBufferAsync(this.flushBufferState) != AsyncCompletionResult.Completed)
				{
					return AsyncCompletionResult.Queued;
				}
				num2 = 0;
				this.flushBufferState.Complete(true);
			}
			getBufferState.Result = getBufferState.Result ?? new XmlStreamNodeWriter.GetBufferEventResult();
			getBufferState.Result.Buffer = this.buffer;
			getBufferState.Result.Offset = num2;
			return AsyncCompletionResult.Completed;
		}

		private static void GetBufferFlushComplete(IAsyncEventArgs completionState)
		{
			XmlStreamNodeWriter xmlStreamNodeWriter = (XmlStreamNodeWriter)completionState.AsyncState;
			XmlStreamNodeWriter.GetBufferAsyncEventArgs e = (XmlStreamNodeWriter.GetBufferAsyncEventArgs)xmlStreamNodeWriter.flushBufferState.Arguments;
			e.Result = e.Result ?? new XmlStreamNodeWriter.GetBufferEventResult();
			e.Result.Buffer = xmlStreamNodeWriter.buffer;
			e.Result.Offset = 0;
			e.Complete(false, completionState.Exception);
		}

		private AsyncCompletionResult FlushBufferAsync(AsyncEventArgs<object> state)
		{
			if (Interlocked.CompareExchange(ref this.hasPendingWrite, 1, 0) != 0)
			{
				throw FxTrace.Exception.AsError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Flush buffer is already in use.")));
			}
			if (this.offset != 0)
			{
				if (XmlStreamNodeWriter.onFlushBufferComplete == null)
				{
					XmlStreamNodeWriter.onFlushBufferComplete = new AsyncCallback(XmlStreamNodeWriter.OnFlushBufferCompete);
				}
				IAsyncResult asyncResult = this.stream.BeginWrite(this.buffer, 0, this.offset, XmlStreamNodeWriter.onFlushBufferComplete, this);
				if (!asyncResult.CompletedSynchronously)
				{
					return AsyncCompletionResult.Queued;
				}
				this.stream.EndWrite(asyncResult);
				this.offset = 0;
			}
			if (Interlocked.CompareExchange(ref this.hasPendingWrite, 0, 1) != 1)
			{
				throw FxTrace.Exception.AsError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("No async write operation is pending.")));
			}
			return AsyncCompletionResult.Completed;
		}

		private static void OnFlushBufferCompete(IAsyncResult result)
		{
			if (result.CompletedSynchronously)
			{
				return;
			}
			XmlStreamNodeWriter xmlStreamNodeWriter = (XmlStreamNodeWriter)result.AsyncState;
			Exception ex = null;
			try
			{
				xmlStreamNodeWriter.stream.EndWrite(result);
				xmlStreamNodeWriter.offset = 0;
				if (Interlocked.CompareExchange(ref xmlStreamNodeWriter.hasPendingWrite, 0, 1) != 1)
				{
					throw FxTrace.Exception.AsError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("No async write operation is pending.")));
				}
			}
			catch (Exception ex2)
			{
				if (Fx.IsFatal(ex2))
				{
					throw;
				}
				ex = ex2;
			}
			xmlStreamNodeWriter.flushBufferState.Complete(false, ex);
		}

		protected IAsyncResult BeginGetBuffer(int count, AsyncCallback callback, object state)
		{
			return new XmlStreamNodeWriter.GetBufferAsyncResult(count, this, callback, state);
		}

		protected byte[] EndGetBuffer(IAsyncResult result, out int offset)
		{
			return XmlStreamNodeWriter.GetBufferAsyncResult.End(result, out offset);
		}

		protected void Advance(int count)
		{
			this.offset += count;
		}

		private void EnsureByte()
		{
			if (this.offset >= 512)
			{
				this.FlushBuffer();
			}
		}

		protected void WriteByte(byte b)
		{
			this.EnsureByte();
			byte[] array = this.buffer;
			int num = this.offset;
			this.offset = num + 1;
			array[num] = b;
		}

		protected void WriteByte(char ch)
		{
			this.WriteByte((byte)ch);
		}

		protected void WriteBytes(byte b1, byte b2)
		{
			byte[] array = this.buffer;
			int num = this.offset;
			if (num + 1 >= 512)
			{
				this.FlushBuffer();
				num = 0;
			}
			array[num] = b1;
			array[num + 1] = b2;
			this.offset += 2;
		}

		protected void WriteBytes(char ch1, char ch2)
		{
			this.WriteBytes((byte)ch1, (byte)ch2);
		}

		public void WriteBytes(byte[] byteBuffer, int byteOffset, int byteCount)
		{
			if (byteCount < 512)
			{
				int num;
				byte[] array = this.GetBuffer(byteCount, out num);
				Buffer.BlockCopy(byteBuffer, byteOffset, array, num, byteCount);
				this.Advance(byteCount);
				return;
			}
			this.FlushBuffer();
			this.stream.Write(byteBuffer, byteOffset, byteCount);
		}

		public IAsyncResult BeginWriteBytes(byte[] byteBuffer, int byteOffset, int byteCount, AsyncCallback callback, object state)
		{
			return new XmlStreamNodeWriter.WriteBytesAsyncResult(byteBuffer, byteOffset, byteCount, this, callback, state);
		}

		public void EndWriteBytes(IAsyncResult result)
		{
			XmlStreamNodeWriter.WriteBytesAsyncResult.End(result);
		}

		[SecurityCritical]
		protected unsafe void UnsafeWriteBytes(byte* bytes, int byteCount)
		{
			this.FlushBuffer();
			byte[] array = this.buffer;
			while (byteCount > 512)
			{
				for (int i = 0; i < 512; i++)
				{
					array[i] = bytes[i];
				}
				this.stream.Write(array, 0, 512);
				bytes += 512;
				byteCount -= 512;
			}
			if (byteCount > 0)
			{
				for (int j = 0; j < byteCount; j++)
				{
					array[j] = bytes[j];
				}
				this.stream.Write(array, 0, byteCount);
			}
		}

		[SecuritySafeCritical]
		protected unsafe void WriteUTF8Char(int ch)
		{
			if (ch < 128)
			{
				this.WriteByte((byte)ch);
				return;
			}
			if (ch <= 65535)
			{
				char* ptr = stackalloc char[(UIntPtr)2];
				*ptr = (char)ch;
				this.UnsafeWriteUTF8Chars(ptr, 1);
				return;
			}
			SurrogateChar surrogateChar = new SurrogateChar(ch);
			char* ptr2 = stackalloc char[(UIntPtr)4];
			*ptr2 = surrogateChar.HighChar;
			ptr2[1] = surrogateChar.LowChar;
			this.UnsafeWriteUTF8Chars(ptr2, 2);
		}

		protected void WriteUTF8Chars(byte[] chars, int charOffset, int charCount)
		{
			if (charCount < 512)
			{
				int num;
				byte[] array = this.GetBuffer(charCount, out num);
				Buffer.BlockCopy(chars, charOffset, array, num, charCount);
				this.Advance(charCount);
				return;
			}
			this.FlushBuffer();
			this.stream.Write(chars, charOffset, charCount);
		}

		[SecuritySafeCritical]
		protected unsafe void WriteUTF8Chars(string value)
		{
			int length = value.Length;
			if (length > 0)
			{
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					this.UnsafeWriteUTF8Chars(ptr, length);
				}
			}
		}

		[SecurityCritical]
		protected unsafe void UnsafeWriteUTF8Chars(char* chars, int charCount)
		{
			while (charCount > 170)
			{
				int num = 170;
				if ((chars[num - 1] & 'ﰀ') == '\ud800')
				{
					num--;
				}
				int num2;
				byte[] array = this.GetBuffer(num * 3, out num2);
				this.Advance(this.UnsafeGetUTF8Chars(chars, num, array, num2));
				charCount -= num;
				chars += num;
			}
			if (charCount > 0)
			{
				int num3;
				byte[] array2 = this.GetBuffer(charCount * 3, out num3);
				this.Advance(this.UnsafeGetUTF8Chars(chars, charCount, array2, num3));
			}
		}

		[SecurityCritical]
		protected unsafe void UnsafeWriteUnicodeChars(char* chars, int charCount)
		{
			while (charCount > 256)
			{
				int num = 256;
				if ((chars[num - 1] & 'ﰀ') == '\ud800')
				{
					num--;
				}
				int num2;
				byte[] array = this.GetBuffer(num * 2, out num2);
				this.Advance(this.UnsafeGetUnicodeChars(chars, num, array, num2));
				charCount -= num;
				chars += num;
			}
			if (charCount > 0)
			{
				int num3;
				byte[] array2 = this.GetBuffer(charCount * 2, out num3);
				this.Advance(this.UnsafeGetUnicodeChars(chars, charCount, array2, num3));
			}
		}

		[SecurityCritical]
		protected unsafe int UnsafeGetUnicodeChars(char* chars, int charCount, byte[] buffer, int offset)
		{
			char* ptr = chars + charCount;
			while (chars < ptr)
			{
				char c = *(chars++);
				buffer[offset++] = (byte)c;
				c >>= 8;
				buffer[offset++] = (byte)c;
			}
			return charCount * 2;
		}

		[SecurityCritical]
		protected unsafe int UnsafeGetUTF8Length(char* chars, int charCount)
		{
			char* ptr = chars + charCount;
			while (chars < ptr && *chars < '\u0080')
			{
				chars++;
			}
			if (chars == ptr)
			{
				return charCount;
			}
			return (int)((long)(chars - (ptr - charCount))) + this.encoding.GetByteCount(chars, (int)((long)(ptr - chars)));
		}

		[SecurityCritical]
		protected unsafe int UnsafeGetUTF8Chars(char* chars, int charCount, byte[] buffer, int offset)
		{
			if (charCount > 0)
			{
				fixed (byte* ptr = &buffer[offset])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = ptr2;
					byte* ptr4 = ptr3 + (buffer.Length - offset);
					char* ptr5 = chars + charCount;
					do
					{
						if (chars < ptr5)
						{
							char c = *chars;
							if (c < '\u0080')
							{
								*ptr3 = (byte)c;
								ptr3++;
								chars++;
								continue;
							}
						}
						if (chars >= ptr5)
						{
							break;
						}
						char* ptr6 = chars;
						while (chars < ptr5 && *chars >= '\u0080')
						{
							chars++;
						}
						ptr3 += this.encoding.GetBytes(ptr6, (int)((long)(chars - ptr6)), ptr3, (int)((long)(ptr4 - ptr3)));
					}
					while (chars < ptr5);
					return (int)((long)(ptr3 - ptr2));
				}
			}
			return 0;
		}

		protected virtual void FlushBuffer()
		{
			if (this.offset != 0)
			{
				this.stream.Write(this.buffer, 0, this.offset);
				this.offset = 0;
			}
		}

		protected virtual IAsyncResult BeginFlushBuffer(AsyncCallback callback, object state)
		{
			return new XmlStreamNodeWriter.FlushBufferAsyncResult(this, callback, state);
		}

		protected virtual void EndFlushBuffer(IAsyncResult result)
		{
			XmlStreamNodeWriter.FlushBufferAsyncResult.End(result);
		}

		public override void Flush()
		{
			this.FlushBuffer();
			this.stream.Flush();
		}

		public override void Close()
		{
			if (this.stream != null)
			{
				if (this.ownsStream)
				{
					this.stream.Close();
				}
				this.stream = null;
			}
		}

		private Stream stream;

		private byte[] buffer;

		private int offset;

		private bool ownsStream;

		private const int bufferLength = 512;

		private const int maxEntityLength = 32;

		private const int maxBytesPerChar = 3;

		private Encoding encoding;

		private int hasPendingWrite;

		private AsyncEventArgs<object> flushBufferState;

		private static UTF8Encoding UTF8Encoding = new UTF8Encoding(false, true);

		private static AsyncCallback onFlushBufferComplete;

		private static AsyncEventArgsCallback onGetFlushComplete;

		private class GetBufferAsyncResult : AsyncResult
		{
			public GetBufferAsyncResult(int count, XmlStreamNodeWriter writer, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.count = count;
				this.writer = writer;
				int num = writer.offset;
				bool flag;
				if (num + count <= 512)
				{
					this.offset = num;
					flag = true;
				}
				else
				{
					IAsyncResult asyncResult = writer.BeginFlushBuffer(base.PrepareAsyncCompletion(XmlStreamNodeWriter.GetBufferAsyncResult.onComplete), this);
					flag = base.SyncContinue(asyncResult);
				}
				if (flag)
				{
					base.Complete(true);
				}
			}

			private static bool OnComplete(IAsyncResult result)
			{
				return ((XmlStreamNodeWriter.GetBufferAsyncResult)result.AsyncState).HandleFlushBuffer(result);
			}

			private bool HandleFlushBuffer(IAsyncResult result)
			{
				this.writer.EndFlushBuffer(result);
				this.offset = 0;
				return true;
			}

			public static byte[] End(IAsyncResult result, out int offset)
			{
				XmlStreamNodeWriter.GetBufferAsyncResult getBufferAsyncResult = AsyncResult.End<XmlStreamNodeWriter.GetBufferAsyncResult>(result);
				offset = getBufferAsyncResult.offset;
				return getBufferAsyncResult.writer.buffer;
			}

			private XmlStreamNodeWriter writer;

			private int offset;

			private int count;

			private static AsyncResult.AsyncCompletion onComplete = new AsyncResult.AsyncCompletion(XmlStreamNodeWriter.GetBufferAsyncResult.OnComplete);
		}

		private class WriteBytesAsyncResult : AsyncResult
		{
			public WriteBytesAsyncResult(byte[] byteBuffer, int byteOffset, int byteCount, XmlStreamNodeWriter writer, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.byteBuffer = byteBuffer;
				this.byteOffset = byteOffset;
				this.byteCount = byteCount;
				this.writer = writer;
				bool flag;
				if (byteCount < 512)
				{
					flag = this.HandleGetBuffer(null);
				}
				else
				{
					flag = this.HandleFlushBuffer(null);
				}
				if (flag)
				{
					base.Complete(true);
				}
			}

			private static bool OnHandleGetBufferComplete(IAsyncResult result)
			{
				return ((XmlStreamNodeWriter.WriteBytesAsyncResult)result.AsyncState).HandleGetBuffer(result);
			}

			private static bool OnHandleFlushBufferComplete(IAsyncResult result)
			{
				return ((XmlStreamNodeWriter.WriteBytesAsyncResult)result.AsyncState).HandleFlushBuffer(result);
			}

			private static bool OnHandleWrite(IAsyncResult result)
			{
				return ((XmlStreamNodeWriter.WriteBytesAsyncResult)result.AsyncState).HandleWrite(result);
			}

			private bool HandleGetBuffer(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.writer.BeginGetBuffer(this.byteCount, base.PrepareAsyncCompletion(XmlStreamNodeWriter.WriteBytesAsyncResult.onHandleGetBufferComplete), this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				int num;
				byte[] array = this.writer.EndGetBuffer(result, out num);
				Buffer.BlockCopy(this.byteBuffer, this.byteOffset, array, num, this.byteCount);
				this.writer.Advance(this.byteCount);
				return true;
			}

			private bool HandleFlushBuffer(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.writer.BeginFlushBuffer(base.PrepareAsyncCompletion(XmlStreamNodeWriter.WriteBytesAsyncResult.onHandleFlushBufferComplete), this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				this.writer.EndFlushBuffer(result);
				return this.HandleWrite(null);
			}

			private bool HandleWrite(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.writer.stream.BeginWrite(this.byteBuffer, this.byteOffset, this.byteCount, base.PrepareAsyncCompletion(XmlStreamNodeWriter.WriteBytesAsyncResult.onHandleWrite), this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				this.writer.stream.EndWrite(result);
				return true;
			}

			public static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlStreamNodeWriter.WriteBytesAsyncResult>(result);
			}

			private static AsyncResult.AsyncCompletion onHandleGetBufferComplete = new AsyncResult.AsyncCompletion(XmlStreamNodeWriter.WriteBytesAsyncResult.OnHandleGetBufferComplete);

			private static AsyncResult.AsyncCompletion onHandleFlushBufferComplete = new AsyncResult.AsyncCompletion(XmlStreamNodeWriter.WriteBytesAsyncResult.OnHandleFlushBufferComplete);

			private static AsyncResult.AsyncCompletion onHandleWrite = new AsyncResult.AsyncCompletion(XmlStreamNodeWriter.WriteBytesAsyncResult.OnHandleWrite);

			private byte[] byteBuffer;

			private int byteOffset;

			private int byteCount;

			private XmlStreamNodeWriter writer;
		}

		private class FlushBufferAsyncResult : AsyncResult
		{
			public FlushBufferAsyncResult(XmlStreamNodeWriter writer, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.writer = writer;
				bool flag = true;
				if (writer.offset != 0)
				{
					flag = this.HandleFlushBuffer(null);
				}
				if (flag)
				{
					base.Complete(true);
				}
			}

			private static bool OnComplete(IAsyncResult result)
			{
				return ((XmlStreamNodeWriter.FlushBufferAsyncResult)result.AsyncState).HandleFlushBuffer(result);
			}

			private bool HandleFlushBuffer(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.writer.stream.BeginWrite(this.writer.buffer, 0, this.writer.offset, base.PrepareAsyncCompletion(XmlStreamNodeWriter.FlushBufferAsyncResult.onComplete), this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				this.writer.stream.EndWrite(result);
				this.writer.offset = 0;
				return true;
			}

			public static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlStreamNodeWriter.FlushBufferAsyncResult>(result);
			}

			private static AsyncResult.AsyncCompletion onComplete = new AsyncResult.AsyncCompletion(XmlStreamNodeWriter.FlushBufferAsyncResult.OnComplete);

			private XmlStreamNodeWriter writer;
		}

		internal class GetBufferArgs
		{
			public int Count { get; set; }
		}

		internal class GetBufferEventResult
		{
			internal byte[] Buffer { get; set; }

			internal int Offset { get; set; }
		}

		internal class GetBufferAsyncEventArgs : AsyncEventArgs<XmlStreamNodeWriter.GetBufferArgs, XmlStreamNodeWriter.GetBufferEventResult>
		{
		}
	}
}
