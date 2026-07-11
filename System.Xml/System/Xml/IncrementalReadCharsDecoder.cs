using System;

namespace System.Xml
{
	internal class IncrementalReadCharsDecoder : IncrementalReadDecoder
	{
		internal IncrementalReadCharsDecoder()
		{
		}

		internal override int DecodedCount
		{
			get
			{
				return this.curIndex - this.startIndex;
			}
		}

		internal override bool IsFull
		{
			get
			{
				return this.curIndex == this.endIndex;
			}
		}

		internal override int Decode(char[] chars, int startPos, int len)
		{
			int num = this.endIndex - this.curIndex;
			if (num > len)
			{
				num = len;
			}
			Buffer.BlockCopy(chars, startPos * 2, this.buffer, this.curIndex * 2, num * 2);
			this.curIndex += num;
			return num;
		}

		internal override int Decode(string str, int startPos, int len)
		{
			int num = this.endIndex - this.curIndex;
			if (num > len)
			{
				num = len;
			}
			str.CopyTo(startPos, this.buffer, this.curIndex, num);
			this.curIndex += num;
			return num;
		}

		internal override void Reset()
		{
		}

		internal override void SetNextOutputBuffer(Array buffer, int index, int count)
		{
			this.buffer = (char[])buffer;
			this.startIndex = index;
			this.curIndex = index;
			this.endIndex = index + count;
		}

		private char[] buffer;

		private int startIndex;

		private int curIndex;

		private int endIndex;
	}
}
