using System;

namespace System.IO.Compression
{
	internal sealed class OutputWindow
	{
		public void Write(byte b)
		{
			byte[] window = this._window;
			int end = this._end;
			this._end = end + 1;
			window[end] = b;
			this._end &= 262143;
			this._bytesUsed++;
		}

		public void WriteLengthDistance(int length, int distance)
		{
			this._bytesUsed += length;
			int num = (this._end - distance) & 262143;
			int num2 = 262144 - length;
			if (num > num2 || this._end >= num2)
			{
				while (length-- > 0)
				{
					byte[] window = this._window;
					int num3 = this._end;
					this._end = num3 + 1;
					window[num3] = this._window[num++];
					this._end &= 262143;
					num &= 262143;
				}
				return;
			}
			if (length <= distance)
			{
				Array.Copy(this._window, num, this._window, this._end, length);
				this._end += length;
				return;
			}
			while (length-- > 0)
			{
				byte[] window2 = this._window;
				int num3 = this._end;
				this._end = num3 + 1;
				window2[num3] = this._window[num++];
			}
		}

		public int CopyFrom(InputBuffer input, int length)
		{
			length = Math.Min(Math.Min(length, 262144 - this._bytesUsed), input.AvailableBytes);
			int num = 262144 - this._end;
			int num2;
			if (length > num)
			{
				num2 = input.CopyTo(this._window, this._end, num);
				if (num2 == num)
				{
					num2 += input.CopyTo(this._window, 0, length - num);
				}
			}
			else
			{
				num2 = input.CopyTo(this._window, this._end, length);
			}
			this._end = (this._end + num2) & 262143;
			this._bytesUsed += num2;
			return num2;
		}

		public int FreeBytes
		{
			get
			{
				return 262144 - this._bytesUsed;
			}
		}

		public int AvailableBytes
		{
			get
			{
				return this._bytesUsed;
			}
		}

		public int CopyTo(byte[] output, int offset, int length)
		{
			int num;
			if (length > this._bytesUsed)
			{
				num = this._end;
				length = this._bytesUsed;
			}
			else
			{
				num = (this._end - this._bytesUsed + length) & 262143;
			}
			int num2 = length;
			int num3 = length - num;
			if (num3 > 0)
			{
				Array.Copy(this._window, 262144 - num3, output, offset, num3);
				offset += num3;
				length = num;
			}
			Array.Copy(this._window, num - length, output, offset, length);
			this._bytesUsed -= num2;
			return num2;
		}

		private const int WindowSize = 262144;

		private const int WindowMask = 262143;

		private readonly byte[] _window = new byte[262144];

		private int _end;

		private int _bytesUsed;
	}
}
