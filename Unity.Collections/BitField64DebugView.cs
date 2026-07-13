using System;

namespace Unity.Collections
{
	internal sealed class BitField64DebugView
	{
		public BitField64DebugView(BitField64 data)
		{
			this.Data = data;
		}

		public bool[] Bits
		{
			get
			{
				bool[] array = new bool[64];
				for (int i = 0; i < 64; i++)
				{
					array[i] = this.Data.IsSet(i);
				}
				return array;
			}
		}

		private BitField64 Data;
	}
}
