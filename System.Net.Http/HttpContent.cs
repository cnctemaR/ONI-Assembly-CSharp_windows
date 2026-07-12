using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public abstract class HttpContent : IDisposable
	{
		public HttpContentHeaders Headers
		{
			get
			{
				HttpContentHeaders httpContentHeaders;
				if ((httpContentHeaders = this.headers) == null)
				{
					httpContentHeaders = (this.headers = new HttpContentHeaders(this));
				}
				return httpContentHeaders;
			}
		}

		internal long? LoadedBufferLength
		{
			get
			{
				if (this.buffer != null)
				{
					return new long?(this.buffer.Length);
				}
				return null;
			}
		}

		internal void CopyTo(Stream stream)
		{
			this.CopyToAsync(stream).Wait();
		}

		public Task CopyToAsync(Stream stream)
		{
			return this.CopyToAsync(stream, null);
		}

		public Task CopyToAsync(Stream stream, TransportContext context)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (this.buffer != null)
			{
				return this.buffer.CopyToAsync(stream);
			}
			return this.SerializeToStreamAsync(stream, context);
		}

		protected virtual async Task<Stream> CreateContentReadStreamAsync()
		{
			await this.LoadIntoBufferAsync().ConfigureAwait(false);
			return this.buffer;
		}

		private static HttpContent.FixedMemoryStream CreateFixedMemoryStream(long maxBufferSize)
		{
			return new HttpContent.FixedMemoryStream(maxBufferSize);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (this.buffer != null)
				{
					this.buffer.Dispose();
				}
			}
		}

		public Task LoadIntoBufferAsync()
		{
			return this.LoadIntoBufferAsync(2147483647L);
		}

		public async Task LoadIntoBufferAsync(long maxBufferSize)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (this.buffer == null)
			{
				this.buffer = HttpContent.CreateFixedMemoryStream(maxBufferSize);
				await this.SerializeToStreamAsync(this.buffer, null).ConfigureAwait(false);
				this.buffer.Seek(0L, SeekOrigin.Begin);
			}
		}

		public async Task<Stream> ReadAsStreamAsync()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			Stream stream;
			if (this.buffer != null)
			{
				stream = new MemoryStream(this.buffer.GetBuffer(), 0, (int)this.buffer.Length, false);
			}
			else
			{
				if (this.stream == null)
				{
					Stream stream2 = await this.CreateContentReadStreamAsync().ConfigureAwait(false);
					this.stream = stream2;
				}
				stream = this.stream;
			}
			return stream;
		}

		public async Task<byte[]> ReadAsByteArrayAsync()
		{
			await this.LoadIntoBufferAsync().ConfigureAwait(false);
			return this.buffer.ToArray();
		}

		public async Task<string> ReadAsStringAsync()
		{
			await this.LoadIntoBufferAsync().ConfigureAwait(false);
			string text;
			if (this.buffer.Length == 0L)
			{
				text = string.Empty;
			}
			else
			{
				byte[] array = this.buffer.GetBuffer();
				int num = (int)this.buffer.Length;
				int num2 = 0;
				Encoding encoding;
				if (this.headers != null && this.headers.ContentType != null && this.headers.ContentType.CharSet != null)
				{
					encoding = Encoding.GetEncoding(this.headers.ContentType.CharSet);
					num2 = HttpContent.StartsWith(array, num, encoding.GetPreamble());
				}
				else
				{
					encoding = HttpContent.GetEncodingFromBuffer(array, num, ref num2) ?? Encoding.UTF8;
				}
				text = encoding.GetString(array, num2, num - num2);
			}
			return text;
		}

		private static Encoding GetEncodingFromBuffer(byte[] buffer, int length, ref int preambleLength)
		{
			foreach (Encoding encoding in new Encoding[]
			{
				Encoding.UTF8,
				Encoding.UTF32,
				Encoding.Unicode
			})
			{
				if ((preambleLength = HttpContent.StartsWith(buffer, length, encoding.GetPreamble())) != 0)
				{
					return encoding;
				}
			}
			return null;
		}

		private static int StartsWith(byte[] array, int length, byte[] value)
		{
			if (length < value.Length)
			{
				return 0;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (array[i] != value[i])
				{
					return 0;
				}
			}
			return value.Length;
		}

		internal Task SerializeToStreamAsync_internal(Stream stream, TransportContext context)
		{
			return this.SerializeToStreamAsync(stream, context);
		}

		protected abstract Task SerializeToStreamAsync(Stream stream, TransportContext context);

		protected internal abstract bool TryComputeLength(out long length);

		private HttpContent.FixedMemoryStream buffer;

		private Stream stream;

		private bool disposed;

		private HttpContentHeaders headers;

		private sealed class FixedMemoryStream : MemoryStream
		{
			public FixedMemoryStream(long maxSize)
			{
				this.maxSize = maxSize;
			}

			private void CheckOverflow(int count)
			{
				if (this.Length + (long)count > this.maxSize)
				{
					throw new HttpRequestException(string.Format("Cannot write more bytes to the buffer than the configured maximum buffer size: {0}", this.maxSize));
				}
			}

			public override void WriteByte(byte value)
			{
				this.CheckOverflow(1);
				base.WriteByte(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.CheckOverflow(count);
				base.Write(buffer, offset, count);
			}

			private readonly long maxSize;
		}
	}
}
