using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafeBitArrayDebugView
	{
		public UnsafeBitArrayDebugView(UnsafeBitArray data)
		{
			this.Data = data;
		}

		public bool[] Bits
		{
			get
			{
				bool[] array = new bool[this.Data.Length];
				for (int i = 0; i < this.Data.Length; i++)
				{
					array[i] = this.Data.IsSet(i);
				}
				return array;
			}
		}

		private UnsafeBitArray Data;
	}
}
