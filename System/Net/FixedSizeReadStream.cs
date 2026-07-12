using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class FixedSizeReadStream : WebReadStream
	{
		public long ContentLength { get; }

		public FixedSizeReadStream(WebOperation operation, Stream innerStream, long contentLength)
			: base(operation, innerStream)
		{
			this.ContentLength = contentLength;
		}

		protected override async Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			long num = this.ContentLength - this.position;
			int num2;
			if (num == 0L)
			{
				num2 = 0;
			}
			else
			{
				int num3 = (int)Math.Min(num, (long)size);
				int num4 = await base.InnerStream.ReadAsync(buffer, offset, num3, cancellationToken).ConfigureAwait(false);
				if (num4 <= 0)
				{
					num2 = num4;
				}
				else
				{
					this.position += (long)num4;
					num2 = num4;
				}
			}
			return num2;
		}

		private long position;
	}
}
