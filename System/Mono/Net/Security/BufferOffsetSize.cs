using System;

namespace Mono.Net.Security
{
	internal class BufferOffsetSize
	{
		public int EndOffset
		{
			get
			{
				return this.Offset + this.Size;
			}
		}

		public int Remaining
		{
			get
			{
				return this.Buffer.Length - this.Offset - this.Size;
			}
		}

		public BufferOffsetSize(byte[] buffer, int offset, int size)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			this.Buffer = buffer;
			this.Offset = offset;
			this.Size = size;
			this.Complete = false;
		}

		public override string ToString()
		{
			return string.Format("[BufferOffsetSize: {0} {1}]", this.Offset, this.Size);
		}

		public byte[] Buffer;

		public int Offset;

		public int Size;

		public int TotalBytes;

		public bool Complete;
	}
}
