using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public class StreamContent : HttpContent
	{
		public StreamContent(Stream content)
			: this(content, 16384)
		{
		}

		public StreamContent(Stream content, int bufferSize)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			this.content = content;
			this.bufferSize = bufferSize;
			if (content.CanSeek)
			{
				this.startPosition = content.Position;
			}
		}

		internal StreamContent(Stream content, CancellationToken cancellationToken)
			: this(content)
		{
			this.cancellationToken = cancellationToken;
		}

		protected override Task<Stream> CreateContentReadStreamAsync()
		{
			return Task.FromResult<Stream>(this.content);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.content.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			if (this.contentCopied)
			{
				if (!this.content.CanSeek)
				{
					throw new InvalidOperationException("The stream was already consumed. It cannot be read again.");
				}
				this.content.Seek(this.startPosition, SeekOrigin.Begin);
			}
			else
			{
				this.contentCopied = true;
			}
			return this.content.CopyToAsync(stream, this.bufferSize, this.cancellationToken);
		}

		protected internal override bool TryComputeLength(out long length)
		{
			if (!this.content.CanSeek)
			{
				length = 0L;
				return false;
			}
			length = this.content.Length - this.startPosition;
			return true;
		}

		private readonly Stream content;

		private readonly int bufferSize;

		private readonly CancellationToken cancellationToken;

		private readonly long startPosition;

		private bool contentCopied;
	}
}
