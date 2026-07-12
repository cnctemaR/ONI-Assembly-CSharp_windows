using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class MonoChunkStream : WebReadStream
	{
		protected WebHeaderCollection Headers { get; }

		protected MonoChunkParser Decoder { get; }

		public MonoChunkStream(WebOperation operation, Stream innerStream, WebHeaderCollection headers)
			: base(operation, innerStream)
		{
			this.Headers = headers;
			this.Decoder = new MonoChunkParser(headers);
		}

		protected override async Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int num;
			if (this.Decoder.DataAvailable)
			{
				num = this.Decoder.Read(buffer, offset, size);
			}
			else
			{
				int num2 = 0;
				byte[] moreBytes = null;
				while (num2 == 0 && this.Decoder.WantMore)
				{
					int num3 = this.Decoder.ChunkLeft;
					if (num3 <= 0)
					{
						num3 = 1024;
					}
					else if (num3 > 16384)
					{
						num3 = 16384;
					}
					if (moreBytes == null || moreBytes.Length < num3)
					{
						moreBytes = new byte[num3];
					}
					num2 = await base.InnerStream.ReadAsync(moreBytes, 0, num3, cancellationToken).ConfigureAwait(false);
					if (num2 <= 0)
					{
						return num2;
					}
					this.Decoder.Write(moreBytes, 0, num2);
					num2 = this.Decoder.Read(buffer, offset, size);
				}
				num = num2;
			}
			return num;
		}

		internal override async Task FinishReading(CancellationToken cancellationToken)
		{
			await base.FinishReading(cancellationToken).ConfigureAwait(false);
			cancellationToken.ThrowIfCancellationRequested();
			if (this.Decoder.DataAvailable)
			{
				MonoChunkStream.ThrowExpectingChunkTrailer();
			}
			while (this.Decoder.WantMore)
			{
				byte[] buffer = new byte[256];
				int num = await base.InnerStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
				if (num <= 0)
				{
					MonoChunkStream.ThrowExpectingChunkTrailer();
				}
				this.Decoder.Write(buffer, 0, num);
				if (this.Decoder.Read(buffer, 0, 1) != 0)
				{
					MonoChunkStream.ThrowExpectingChunkTrailer();
				}
				buffer = null;
			}
		}

		private static void ThrowExpectingChunkTrailer()
		{
			throw new WebException("Expecting chunk trailer.", null, WebExceptionStatus.ServerProtocolViolation, null);
		}
	}
}
