using System;

namespace Microsoft.Cci.Pdb
{
	internal struct BitSet
	{
		internal BitSet(BitAccess bits)
		{
			bits.ReadInt32(out this.size);
			this.words = new uint[this.size];
			bits.ReadUInt32(this.words);
		}

		internal bool IsSet(int index)
		{
			int num = index / 32;
			return num < this.size && (this.words[num] & BitSet.GetBit(index)) > 0U;
		}

		private static uint GetBit(int index)
		{
			return 1U << index % 32;
		}

		internal bool IsEmpty
		{
			get
			{
				return this.size == 0;
			}
		}

		private int size;

		private uint[] words;
	}
}
