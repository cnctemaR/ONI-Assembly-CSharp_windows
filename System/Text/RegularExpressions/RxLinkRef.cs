using System;

namespace System.Text.RegularExpressions
{
	internal class RxLinkRef : LinkRef
	{
		public RxLinkRef()
		{
			this.offsets = new int[8];
		}

		public void PushInstructionBase(int offset)
		{
			if ((this.current & 1) != 0)
			{
				throw new Exception();
			}
			if (this.current == this.offsets.Length)
			{
				int[] array = new int[this.offsets.Length * 2];
				Array.Copy(this.offsets, array, this.offsets.Length);
				this.offsets = array;
			}
			this.offsets[this.current++] = offset;
		}

		public void PushOffsetPosition(int offset)
		{
			if ((this.current & 1) == 0)
			{
				throw new Exception();
			}
			this.offsets[this.current++] = offset;
		}

		public int[] offsets;

		public int current;
	}
}
