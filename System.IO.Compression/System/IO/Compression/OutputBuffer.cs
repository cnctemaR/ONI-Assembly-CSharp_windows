using System;

namespace System.IO.Compression
{
	internal sealed class OutputBuffer
	{
		internal void UpdateBuffer(byte[] output)
		{
			this._byteBuffer = output;
			this._pos = 0;
		}

		internal int BytesWritten
		{
			get
			{
				return this._pos;
			}
		}

		internal int FreeBytes
		{
			get
			{
				return this._byteBuffer.Length - this._pos;
			}
		}

		internal void WriteUInt16(ushort value)
		{
			byte[] byteBuffer = this._byteBuffer;
			int num = this._pos;
			this._pos = num + 1;
			byteBuffer[num] = (byte)value;
			byte[] byteBuffer2 = this._byteBuffer;
			num = this._pos;
			this._pos = num + 1;
			byteBuffer2[num] = (byte)(value >> 8);
		}

		internal void WriteBits(int n, uint bits)
		{
			this._bitBuf |= bits << this._bitCount;
			this._bitCount += n;
			if (this._bitCount >= 16)
			{
				byte[] byteBuffer = this._byteBuffer;
				int num = this._pos;
				this._pos = num + 1;
				byteBuffer[num] = (byte)this._bitBuf;
				byte[] byteBuffer2 = this._byteBuffer;
				num = this._pos;
				this._pos = num + 1;
				byteBuffer2[num] = (byte)(this._bitBuf >> 8);
				this._bitCount -= 16;
				this._bitBuf >>= 16;
			}
		}

		internal void FlushBits()
		{
			while (this._bitCount >= 8)
			{
				byte[] byteBuffer = this._byteBuffer;
				int num = this._pos;
				this._pos = num + 1;
				byteBuffer[num] = (byte)this._bitBuf;
				this._bitCount -= 8;
				this._bitBuf >>= 8;
			}
			if (this._bitCount > 0)
			{
				byte[] byteBuffer2 = this._byteBuffer;
				int num = this._pos;
				this._pos = num + 1;
				byteBuffer2[num] = (byte)this._bitBuf;
				this._bitBuf = 0U;
				this._bitCount = 0;
			}
		}

		internal void WriteBytes(byte[] byteArray, int offset, int count)
		{
			if (this._bitCount == 0)
			{
				Array.Copy(byteArray, offset, this._byteBuffer, this._pos, count);
				this._pos += count;
				return;
			}
			this.WriteBytesUnaligned(byteArray, offset, count);
		}

		private void WriteBytesUnaligned(byte[] byteArray, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				byte b = byteArray[offset + i];
				this.WriteByteUnaligned(b);
			}
		}

		private void WriteByteUnaligned(byte b)
		{
			this.WriteBits(8, (uint)b);
		}

		internal int BitsInBuffer
		{
			get
			{
				return this._bitCount / 8 + 1;
			}
		}

		internal OutputBuffer.BufferState DumpState()
		{
			return new OutputBuffer.BufferState(this._pos, this._bitBuf, this._bitCount);
		}

		internal void RestoreState(OutputBuffer.BufferState state)
		{
			this._pos = state._pos;
			this._bitBuf = state._bitBuf;
			this._bitCount = state._bitCount;
		}

		private byte[] _byteBuffer;

		private int _pos;

		private uint _bitBuf;

		private int _bitCount;

		internal struct BufferState
		{
			internal BufferState(int pos, uint bitBuf, int bitCount)
			{
				this._pos = pos;
				this._bitBuf = bitBuf;
				this._bitCount = bitCount;
			}

			internal readonly int _pos;

			internal readonly uint _bitBuf;

			internal readonly int _bitCount;
		}
	}
}
