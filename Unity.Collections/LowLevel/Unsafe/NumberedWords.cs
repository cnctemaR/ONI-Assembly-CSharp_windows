using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	public struct NumberedWords
	{
		private int LeadingZeroes
		{
			get
			{
				return (this.Suffix >> 29) & 7;
			}
			set
			{
				this.Suffix &= 536870911;
				this.Suffix |= (value & 7) << 29;
			}
		}

		private int PositiveNumericSuffix
		{
			get
			{
				return this.Suffix & 536870911;
			}
			set
			{
				this.Suffix &= -536870912;
				this.Suffix |= value & 536870911;
			}
		}

		private bool HasPositiveNumericSuffix
		{
			get
			{
				return this.PositiveNumericSuffix != 0;
			}
		}

		[NotBurstCompatible]
		private string NewString(char c, int count)
		{
			char[] array = new char[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = c;
			}
			return new string(array, 0, count);
		}

		[NotBurstCompatible]
		public unsafe int ToFixedString<T>(ref T result) where T : IUTF8Bytes, INativeList<byte>
		{
			int i = this.PositiveNumericSuffix;
			int leadingZeroes = this.LeadingZeroes;
			WordStorage.Instance.GetFixedString<T>(this.Index, ref result);
			if (i == 0 && leadingZeroes == 0)
			{
				return 0;
			}
			byte* ptr = stackalloc byte[(UIntPtr)17];
			int j = 17;
			while (i > 0)
			{
				ptr[--j] = (byte)(48 + i % 10);
				i /= 10;
			}
			while (leadingZeroes-- > 0)
			{
				ptr[--j] = 48;
			}
			byte* ptr2 = result.GetUnsafePtr() + result.Length;
			result.Length += 17 - j;
			while (j < 17)
			{
				*(ptr2++) = ptr[j++];
			}
			return 0;
		}

		[NotBurstCompatible]
		public override string ToString()
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			this.ToFixedString<FixedString512Bytes>(ref fixedString512Bytes);
			return fixedString512Bytes.ToString();
		}

		private bool IsDigit(byte b)
		{
			return b >= 48 && b <= 57;
		}

		[NotBurstCompatible]
		public void SetString<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			int num = value.Length;
			while (num > 0 && this.IsDigit(value[num - 1]))
			{
				num--;
			}
			int num2 = num;
			while (num2 < value.Length && value[num2] == 48)
			{
				num2++;
			}
			int num3 = num2 - num;
			if (num3 > 7)
			{
				int num4 = num3 - 7;
				num += num4;
				num3 -= num4;
			}
			this.PositiveNumericSuffix = 0;
			int num5 = 0;
			for (int i = num2; i < value.Length; i++)
			{
				num5 *= 10;
				num5 += (int)(value[i] - 48);
			}
			if (num5 <= 536870911)
			{
				this.PositiveNumericSuffix = num5;
			}
			else
			{
				num = value.Length;
				num3 = 0;
			}
			this.LeadingZeroes = num3;
			T t = value;
			int length = t.Length;
			if (num != t.Length)
			{
				t.Length = num;
			}
			this.Index = WordStorage.Instance.GetOrCreateIndex<T>(ref t);
		}

		[NotBurstCompatible]
		public void SetString(string value)
		{
			FixedString512Bytes fixedString512Bytes = value;
			this.SetString<FixedString512Bytes>(ref fixedString512Bytes);
		}

		private int Index;

		private int Suffix;

		private const int kPositiveNumericSuffixShift = 0;

		private const int kPositiveNumericSuffixBits = 29;

		private const int kMaxPositiveNumericSuffix = 536870911;

		private const int kPositiveNumericSuffixMask = 536870911;

		private const int kLeadingZeroesShift = 29;

		private const int kLeadingZeroesBits = 3;

		private const int kMaxLeadingZeroes = 7;

		private const int kLeadingZeroesMask = 7;
	}
}
