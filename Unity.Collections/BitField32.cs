using System;
using System.Diagnostics;
using Unity.Mathematics;

namespace Unity.Collections
{
	[DebuggerTypeProxy(typeof(BitField32DebugView))]
	[BurstCompatible]
	public struct BitField32
	{
		public BitField32(uint initialValue = 0U)
		{
			this.Value = initialValue;
		}

		public void Clear()
		{
			this.Value = 0U;
		}

		public void SetBits(int pos, bool value)
		{
			this.Value = Bitwise.SetBits(this.Value, pos, 1U, value);
		}

		public void SetBits(int pos, bool value, int numBits)
		{
			uint num = uint.MaxValue >> 32 - numBits;
			this.Value = Bitwise.SetBits(this.Value, pos, num, value);
		}

		public uint GetBits(int pos, int numBits = 1)
		{
			uint num = uint.MaxValue >> 32 - numBits;
			return Bitwise.ExtractBits(this.Value, pos, num);
		}

		public bool IsSet(int pos)
		{
			return this.GetBits(pos, 1) > 0U;
		}

		public bool TestNone(int pos, int numBits = 1)
		{
			return this.GetBits(pos, numBits) == 0U;
		}

		public bool TestAny(int pos, int numBits = 1)
		{
			return this.GetBits(pos, numBits) > 0U;
		}

		public bool TestAll(int pos, int numBits = 1)
		{
			uint num = uint.MaxValue >> 32 - numBits;
			return num == Bitwise.ExtractBits(this.Value, pos, num);
		}

		public int CountBits()
		{
			return math.countbits(this.Value);
		}

		public int CountLeadingZeros()
		{
			return math.lzcnt(this.Value);
		}

		public int CountTrailingZeros()
		{
			return math.tzcnt(this.Value);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckArgs(int pos, int numBits)
		{
			if (pos > 31 || numBits == 0 || numBits > 32 || pos + numBits > 32)
			{
				throw new ArgumentException(string.Format("BitField32 invalid arguments: pos {0} (must be 0-31), numBits {1} (must be 1-32).", pos, numBits));
			}
		}

		public uint Value;
	}
}
