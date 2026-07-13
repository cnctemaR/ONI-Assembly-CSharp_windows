using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.Internal
{
	internal ref struct DataReader
	{
		public int Index
		{
			readonly get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		public readonly bool CanRead
		{
			get
			{
				return this.index < this.data.Length;
			}
		}

		public DataReader(global::System.ReadOnlySpan<byte> data)
		{
			this = new DataReader(data, 0);
		}

		public DataReader(global::System.ReadOnlySpan<byte> data, int maxStringLength)
		{
			this.data = data;
			this.stringData = ((maxStringLength == 0) ? Array2.Empty<char>() : new char[maxStringLength]);
			this.index = 0;
		}

		public unsafe byte ReadByte()
		{
			int num = this.index;
			this.index = num + 1;
			return *this.data[num];
		}

		public uint ReadCompressedUInt32()
		{
			uint num = 0U;
			for (int i = 0; i < 32; i += 7)
			{
				uint num2 = (uint)this.ReadByte();
				if ((num2 & 128U) == 0U)
				{
					return num | (num2 << i);
				}
				num |= (num2 & 127U) << i;
			}
			throw new InvalidOperationException();
		}

		[NullableContext(1)]
		public string ReadAsciiString()
		{
			int num = (int)this.ReadByte();
			char[] array = this.stringData;
			for (int i = 0; i < num; i++)
			{
				array[i] = (char)this.ReadByte();
			}
			return new string(array, 0, num);
		}

		private readonly global::System.ReadOnlySpan<byte> data;

		private readonly char[] stringData;

		private int index;
	}
}
