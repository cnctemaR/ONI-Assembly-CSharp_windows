using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class BufferedReadStream : WebReadStream
	{
		public BufferedReadStream(WebOperation operation, Stream innerStream, BufferOffsetSize readBuffer)
			: base(operation, innerStream)
		{
			this.readBuffer = readBuffer;
		}

		protected override async Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			BufferOffsetSize bufferOffsetSize = this.readBuffer;
			int num = ((bufferOffsetSize != null) ? bufferOffsetSize.Size : 0);
			int num3;
			if (num > 0)
			{
				int num2 = ((num > size) ? size : num);
				Buffer.BlockCopy(this.readBuffer.Buffer, this.readBuffer.Offset, buffer, offset, num2);
				this.readBuffer.Offset += num2;
				this.readBuffer.Size -= num2;
				offset += num2;
				size -= num2;
				num3 = num2;
			}
			else if (base.InnerStream == null)
			{
				num3 = 0;
			}
			else
			{
				num3 = await base.InnerStream.ReadAsync(buffer, offset, size, cancellationToken).ConfigureAwait(false);
			}
			return num3;
		}

		internal bool TryReadFromBuffer(byte[] buffer, int offset, int size, out int result)
		{
			BufferOffsetSize bufferOffsetSize = this.readBuffer;
			int num = ((bufferOffsetSize != null) ? bufferOffsetSize.Size : 0);
			if (num <= 0)
			{
				result = 0;
				return base.InnerStream == null;
			}
			int num2 = ((num > size) ? size : num);
			Buffer.BlockCopy(this.readBuffer.Buffer, this.readBuffer.Offset, buffer, offset, num2);
			this.readBuffer.Offset += num2;
			this.readBuffer.Size -= num2;
			offset += num2;
			size -= num2;
			result = num2;
			return true;
		}

		private readonly BufferOffsetSize readBuffer;
	}
}
