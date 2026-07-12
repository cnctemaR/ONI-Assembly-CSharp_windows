using System;
using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public class ByteArrayContent : HttpContent
	{
		public ByteArrayContent(byte[] content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			this.content = content;
			this.count = content.Length;
		}

		public ByteArrayContent(byte[] content, int offset, int count)
			: this(content)
		{
			if (offset < 0 || offset > this.count)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > this.count - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.offset = offset;
			this.count = count;
		}

		protected override Task<Stream> CreateContentReadStreamAsync()
		{
			return Task.FromResult<Stream>(new MemoryStream(this.content, this.offset, this.count));
		}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			return stream.WriteAsync(this.content, this.offset, this.count);
		}

		protected internal override bool TryComputeLength(out long length)
		{
			length = (long)this.count;
			return true;
		}

		private readonly byte[] content;

		private readonly int offset;

		private readonly int count;
	}
}
