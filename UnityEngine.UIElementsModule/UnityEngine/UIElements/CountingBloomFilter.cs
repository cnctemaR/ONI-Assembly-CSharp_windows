using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
	internal struct CountingBloomFilter
	{
		private unsafe void AdjustSlot(uint index, bool increment)
		{
			if (increment)
			{
				bool flag = *((ref this.m_Counters.FixedElementField) + (UIntPtr)index) != byte.MaxValue;
				if (flag)
				{
					ref byte ptr = (ref this.m_Counters.FixedElementField) + (UIntPtr)index;
					ptr += 1;
				}
			}
			else
			{
				bool flag2 = *((ref this.m_Counters.FixedElementField) + (UIntPtr)index) > 0;
				if (flag2)
				{
					ref byte ptr2 = (ref this.m_Counters.FixedElementField) + (UIntPtr)index;
					ptr2 -= 1;
				}
			}
		}

		private uint Hash1(uint hash)
		{
			return hash & 16383U;
		}

		private uint Hash2(uint hash)
		{
			return (hash >> 14) & 16383U;
		}

		private unsafe bool IsSlotEmpty(uint index)
		{
			return *((ref this.m_Counters.FixedElementField) + (UIntPtr)index) == 0;
		}

		public void InsertHash(uint hash)
		{
			this.AdjustSlot(this.Hash1(hash), true);
			this.AdjustSlot(this.Hash2(hash), true);
		}

		public void RemoveHash(uint hash)
		{
			this.AdjustSlot(this.Hash1(hash), false);
			this.AdjustSlot(this.Hash2(hash), false);
		}

		public bool ContainsHash(uint hash)
		{
			return !this.IsSlotEmpty(this.Hash1(hash)) && !this.IsSlotEmpty(this.Hash2(hash));
		}

		private const int KEY_SIZE = 14;

		private const uint ARRAY_SIZE = 16384U;

		private const int KEY_MASK = 16383;

		[FixedBuffer(typeof(byte), 16384)]
		private CountingBloomFilter.<m_Counters>e__FixedBuffer m_Counters;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16384)]
		public struct <m_Counters>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
