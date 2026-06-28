using System;
using System.IO;

namespace Ionic.BZip2
{
	internal class BitWriter
	{
		public BitWriter(Stream s)
		{
			this.output = s;
		}

		public byte RemainingBits
		{
			get
			{
				return (byte)((this.accumulator >> 32 - this.nAccumulatedBits) & 255U);
			}
		}

		public int NumRemainingBits
		{
			get
			{
				return this.nAccumulatedBits;
			}
		}

		public int TotalBytesWrittenOut
		{
			get
			{
				return this.totalBytesWrittenOut;
			}
		}

		public void Reset()
		{
			this.accumulator = 0U;
			this.nAccumulatedBits = 0;
			this.totalBytesWrittenOut = 0;
			this.output.Seek(0L, SeekOrigin.Begin);
			this.output.SetLength(0L);
		}

		public void WriteBits(int nbits, uint value)
		{
			int i = this.nAccumulatedBits;
			uint num = this.accumulator;
			while (i >= 8)
			{
				this.output.WriteByte((byte)((num >> 24) & 255U));
				this.totalBytesWrittenOut++;
				num <<= 8;
				i -= 8;
			}
			this.accumulator = num | (value << 32 - i - nbits);
			this.nAccumulatedBits = i + nbits;
		}

		public void WriteByte(byte b)
		{
			this.WriteBits(8, (uint)b);
		}

		public void WriteInt(uint u)
		{
			this.WriteBits(8, (u >> 24) & 255U);
			this.WriteBits(8, (u >> 16) & 255U);
			this.WriteBits(8, (u >> 8) & 255U);
			this.WriteBits(8, u & 255U);
		}

		public void Flush()
		{
			this.WriteBits(0, 0U);
		}

		public void FinishAndPad()
		{
			this.Flush();
			if (this.NumRemainingBits > 0)
			{
				byte b = (byte)((this.accumulator >> 24) & 255U);
				this.output.WriteByte(b);
				this.totalBytesWrittenOut++;
			}
		}

		private uint accumulator;

		private int nAccumulatedBits;

		private Stream output;

		private int totalBytesWrittenOut;
	}
}
