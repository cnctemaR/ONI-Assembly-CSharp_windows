using System;

namespace System.Xml
{
	internal class ByteStack
	{
		public ByteStack(int growthRate)
		{
			this.growthRate = growthRate;
			this.top = 0;
			this.stack = new byte[growthRate];
			this.size = growthRate;
		}

		public void Push(byte data)
		{
			if (this.size == this.top)
			{
				byte[] array = new byte[this.size + this.growthRate];
				if (this.top > 0)
				{
					Buffer.BlockCopy(this.stack, 0, array, 0, this.top);
				}
				this.stack = array;
				this.size += this.growthRate;
			}
			byte[] array2 = this.stack;
			int num = this.top;
			this.top = num + 1;
			array2[num] = data;
		}

		public byte Pop()
		{
			if (this.top > 0)
			{
				byte[] array = this.stack;
				int num = this.top - 1;
				this.top = num;
				return array[num];
			}
			return 0;
		}

		public byte Peek()
		{
			if (this.top > 0)
			{
				return this.stack[this.top - 1];
			}
			return 0;
		}

		public int Length
		{
			get
			{
				return this.top;
			}
		}

		private byte[] stack;

		private int growthRate;

		private int top;

		private int size;
	}
}
