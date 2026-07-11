using System;
using System.Text;

namespace System.Xml
{
	internal class CharEntityEncoderFallback : EncoderFallback
	{
		internal CharEntityEncoderFallback()
		{
		}

		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			if (this.fallbackBuffer == null)
			{
				this.fallbackBuffer = new CharEntityEncoderFallbackBuffer(this);
			}
			return this.fallbackBuffer;
		}

		public override int MaxCharCount
		{
			get
			{
				return 12;
			}
		}

		internal int StartOffset
		{
			get
			{
				return this.startOffset;
			}
			set
			{
				this.startOffset = value;
			}
		}

		internal void Reset(int[] textContentMarks, int endMarkPos)
		{
			this.textContentMarks = textContentMarks;
			this.endMarkPos = endMarkPos;
			this.curMarkPos = 0;
		}

		internal bool CanReplaceAt(int index)
		{
			int num = this.curMarkPos;
			int num2 = this.startOffset + index;
			while (num < this.endMarkPos && num2 >= this.textContentMarks[num + 1])
			{
				num++;
			}
			this.curMarkPos = num;
			return (num & 1) != 0;
		}

		private CharEntityEncoderFallbackBuffer fallbackBuffer;

		private int[] textContentMarks;

		private int endMarkPos;

		private int curMarkPos;

		private int startOffset;
	}
}
