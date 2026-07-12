using System;

namespace System.IO.Compression
{
	internal sealed class InputBuffer
	{
		public int AvailableBits
		{
			get
			{
				return this._bitsInBuffer;
			}
		}

		public int AvailableBytes
		{
			get
			{
				return this._end - this._start + this._bitsInBuffer / 8;
			}
		}

		public bool EnsureBitsAvailable(int count)
		{
			if (this._bitsInBuffer < count)
			{
				if (this.NeedsInput())
				{
					return false;
				}
				uint bitBuffer = this._bitBuffer;
				byte[] buffer = this._buffer;
				int num = this._start;
				this._start = num + 1;
				this._bitBuffer = bitBuffer | (buffer[num] << (this._bitsInBuffer & 31));
				this._bitsInBuffer += 8;
				if (this._bitsInBuffer < count)
				{
					if (this.NeedsInput())
					{
						return false;
					}
					uint bitBuffer2 = this._bitBuffer;
					byte[] buffer2 = this._buffer;
					num = this._start;
					this._start = num + 1;
					this._bitBuffer = bitBuffer2 | (buffer2[num] << (this._bitsInBuffer & 31));
					this._bitsInBuffer += 8;
				}
			}
			return true;
		}

		public uint TryLoad16Bits()
		{
			if (this._bitsInBuffer < 8)
			{
				if (this._start < this._end)
				{
					uint bitBuffer = this._bitBuffer;
					byte[] buffer = this._buffer;
					int num = this._start;
					this._start = num + 1;
					this._bitBuffer = bitBuffer | (buffer[num] << (this._bitsInBuffer & 31));
					this._bitsInBuffer += 8;
				}
				if (this._start < this._end)
				{
					uint bitBuffer2 = this._bitBuffer;
					byte[] buffer2 = this._buffer;
					int num = this._start;
					this._start = num + 1;
					this._bitBuffer = bitBuffer2 | (buffer2[num] << (this._bitsInBuffer & 31));
					this._bitsInBuffer += 8;
				}
			}
			else if (this._bitsInBuffer < 16 && this._start < this._end)
			{
				uint bitBuffer3 = this._bitBuffer;
				byte[] buffer3 = this._buffer;
				int num = this._start;
				this._start = num + 1;
				this._bitBuffer = bitBuffer3 | (buffer3[num] << (this._bitsInBuffer & 31));
				this._bitsInBuffer += 8;
			}
			return this._bitBuffer;
		}

		private uint GetBitMask(int count)
		{
			return (1U << count) - 1U;
		}

		public int GetBits(int count)
		{
			if (!this.EnsureBitsAvailable(count))
			{
				return -1;
			}
			int num = (int)(this._bitBuffer & this.GetBitMask(count));
			this._bitBuffer >>= count;
			this._bitsInBuffer -= count;
			return num;
		}

		public int CopyTo(byte[] output, int offset, int length)
		{
			int num = 0;
			while (this._bitsInBuffer > 0 && length > 0)
			{
				output[offset++] = (byte)this._bitBuffer;
				this._bitBuffer >>= 8;
				this._bitsInBuffer -= 8;
				length--;
				num++;
			}
			if (length == 0)
			{
				return num;
			}
			int num2 = this._end - this._start;
			if (length > num2)
			{
				length = num2;
			}
			Array.Copy(this._buffer, this._start, output, offset, length);
			this._start += length;
			return num + length;
		}

		public bool NeedsInput()
		{
			return this._start == this._end;
		}

		public void SetInput(byte[] buffer, int offset, int length)
		{
			this._buffer = buffer;
			this._start = offset;
			this._end = offset + length;
		}

		public void SkipBits(int n)
		{
			this._bitBuffer >>= n;
			this._bitsInBuffer -= n;
		}

		public void SkipToByteBoundary()
		{
			this._bitBuffer >>= this._bitsInBuffer % 8;
			this._bitsInBuffer -= this._bitsInBuffer % 8;
		}

		private byte[] _buffer;

		private int _start;

		private int _end;

		private uint _bitBuffer;

		private int _bitsInBuffer;
	}
}
