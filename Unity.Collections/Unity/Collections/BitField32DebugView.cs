using System;

namespace Unity.Collections
{
	internal sealed class BitField32DebugView
	{
		public BitField32DebugView(BitField32 bitfield)
		{
			this.BitField = bitfield;
		}

		public bool[] Bits
		{
			get
			{
				bool[] array = new bool[32];
				for (int i = 0; i < 32; i++)
				{
					array[i] = this.BitField.IsSet(i);
				}
				return array;
			}
		}

		private BitField32 BitField;
	}
}
