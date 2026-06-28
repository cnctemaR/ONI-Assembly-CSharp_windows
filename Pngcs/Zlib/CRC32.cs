using System;

namespace Hjg.Pngcs.Zlib
{
	public class CRC32
	{
		public CRC32()
			: this(3988292384U, uint.MaxValue)
		{
		}

		public CRC32(uint polynomial, uint seed)
		{
			this.table = CRC32.InitializeTable(polynomial);
			this.seed = seed;
			this.hash = seed;
		}

		public void Update(byte[] buffer)
		{
			this.Update(buffer, 0, buffer.Length);
		}

		public void Update(byte[] buffer, int start, int length)
		{
			int i = 0;
			int num = start;
			while (i < length)
			{
				this.hash = (this.hash >> 8) ^ this.table[(int)((UIntPtr)((uint)buffer[num] ^ (this.hash & 255U)))];
				i++;
				num++;
			}
		}

		public uint GetValue()
		{
			return ~this.hash;
		}

		public void Reset()
		{
			this.hash = this.seed;
		}

		private static uint[] InitializeTable(uint polynomial)
		{
			if (polynomial == 3988292384U && CRC32.defaultTable != null)
			{
				return CRC32.defaultTable;
			}
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					if ((num & 1U) == 1U)
					{
						num = (num >> 1) ^ polynomial;
					}
					else
					{
						num >>= 1;
					}
				}
				array[i] = num;
			}
			if (polynomial == 3988292384U)
			{
				CRC32.defaultTable = array;
			}
			return array;
		}

		private const uint defaultPolynomial = 3988292384U;

		private const uint defaultSeed = 4294967295U;

		private static uint[] defaultTable;

		private uint hash;

		private uint seed;

		private uint[] table;
	}
}
