using System;

namespace System.Xml
{
	internal class BitStack
	{
		public BitStack()
		{
			this.curr = 1U;
		}

		public void PushBit(bool bit)
		{
			if ((this.curr & 2147483648U) != 0U)
			{
				this.PushCurr();
			}
			this.curr = (this.curr << 1) | (bit ? 1U : 0U);
		}

		public bool PopBit()
		{
			bool flag = (this.curr & 1U) > 0U;
			this.curr >>= 1;
			if (this.curr == 1U)
			{
				this.PopCurr();
			}
			return flag;
		}

		public bool PeekBit()
		{
			return (this.curr & 1U) > 0U;
		}

		public bool IsEmpty
		{
			get
			{
				return this.curr == 1U;
			}
		}

		private void PushCurr()
		{
			if (this.bitStack == null)
			{
				this.bitStack = new uint[16];
			}
			uint[] array = this.bitStack;
			int num = this.stackPos;
			this.stackPos = num + 1;
			array[num] = this.curr;
			this.curr = 1U;
			int num2 = this.bitStack.Length;
			if (this.stackPos >= num2)
			{
				uint[] array2 = new uint[2 * num2];
				Array.Copy(this.bitStack, array2, num2);
				this.bitStack = array2;
			}
		}

		private void PopCurr()
		{
			if (this.stackPos > 0)
			{
				uint[] array = this.bitStack;
				int num = this.stackPos - 1;
				this.stackPos = num;
				this.curr = array[num];
			}
		}

		private uint[] bitStack;

		private int stackPos;

		private uint curr;
	}
}
