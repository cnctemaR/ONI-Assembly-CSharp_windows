using System;
using System.Diagnostics;
using Unity.Mathematics;

namespace Unity.Collections
{
	[DebuggerTypeProxy(typeof(BitField64DebugView))]
	[GenerateTestsForBurstCompatibility]
	public struct BitField64
	{
		public BitField64(ulong initialValue = 0UL)
		{
			this.Value = initialValue;
		}

		public void Clear()
		{
			this.Value = 0UL;
		}

		public void SetBits(int pos, bool value)
		{
			this.Value = Bitwise.SetBits(this.Value, pos, 1UL, value);
		}

		public void SetBits(int pos, bool value, int numBits = 1)
		{
			ulong num = ulong.MaxValue >> 64 - numBits;
			this.Value = Bitwise.SetBits(this.Value, pos, num, value);
		}

		public ulong GetBits(int pos, int numBits = 1)
		{
			ulong num = ulong.MaxValue >> 64 - numBits;
			return Bitwise.ExtractBits(this.Value, pos, num);
		}

		public bool IsSet(int pos)
		{
			return this.GetBits(pos, 1) > 0UL;
		}

		public bool TestNone(int pos, int numBits = 1)
		{
			return this.GetBits(pos, numBits) == 0UL;
		}

		public bool TestAny(int pos, int numBits = 1)
		{
			return this.GetBits(pos, numBits) > 0UL;
		}

		public bool TestAll(int pos, int numBits = 1)
		{
			ulong num = ulong.MaxValue >> 64 - numBits;
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
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckArgs(int pos, int numBits)
		{
			if (pos > 63 || numBits == 0 || numBits > 64 || pos + numBits > 64)
			{
				throw new ArgumentException(string.Format("BitField32 invalid arguments: pos {0} (must be 0-63), numBits {1} (must be 1-64).", pos, numBits));
			}
		}

		public ulong Value;
	}
}
