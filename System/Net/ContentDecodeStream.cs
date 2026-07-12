using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class ContentDecodeStream : WebReadStream
	{
		public static ContentDecodeStream Create(WebOperation operation, Stream innerStream, ContentDecodeStream.Mode mode)
		{
			Stream stream;
			if (mode == ContentDecodeStream.Mode.GZip)
			{
				stream = new GZipStream(innerStream, CompressionMode.Decompress);
			}
			else
			{
				stream = new DeflateStream(innerStream, CompressionMode.Decompress);
			}
			return new ContentDecodeStream(operation, stream, innerStream);
		}

		private Stream OriginalInnerStream { get; }

		private ContentDecodeStream(WebOperation operation, Stream decodeStream, Stream originalInnerStream)
			: base(operation, decodeStream)
		{
			this.OriginalInnerStream = originalInnerStream;
		}

		protected override Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			return base.InnerStream.ReadAsync(buffer, offset, size, cancellationToken);
		}

		internal override Task FinishReading(CancellationToken cancellationToken)
		{
			WebReadStream webReadStream = this.OriginalInnerStream as WebReadStream;
			if (webReadStream != null)
			{
				return webReadStream.FinishReading(cancellationToken);
			}
			return Task.CompletedTask;
		}

		internal enum Mode
		{
			GZip,
			Deflate
		}
	}
}
