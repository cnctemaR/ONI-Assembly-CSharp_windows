using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.Collections
{
	[MovedFrom(true, "Unity.Networking.Transport", null, null)]
	[GenerateTestsForBurstCompatibility]
	public struct DataStreamReader
	{
		public DataStreamReader(NativeArray<byte> array)
		{
			DataStreamReader.Initialize(out this, array);
		}

		private unsafe static void Initialize(out DataStreamReader self, NativeArray<byte> array)
		{
			self.m_BufferPtr = (byte*)array.GetUnsafeReadOnlyPtr<byte>();
			self.m_Length = array.Length;
			self.m_Context = default(DataStreamReader.Context);
		}

		public static bool IsLittleEndian
		{
			get
			{
				return DataStreamWriter.IsLittleEndian;
			}
		}

		private static short ByteSwap(short val)
		{
			return (short)(((int)(val & 255) << 8) | ((val >> 8) & 255));
		}

		private static int ByteSwap(int val)
		{
			return ((val & 255) << 24) | ((val & 65280) << 8) | ((val >> 8) & 65280) | ((val >> 24) & 255);
		}

		public readonly bool HasFailedReads
		{
			get
			{
				return this.m_Context.m_FailedReads > 0;
			}
		}

		public readonly int Length
		{
			get
			{
				return this.m_Length;
			}
		}

		public readonly bool IsCreated
		{
			get
			{
				return this.m_BufferPtr != null;
			}
		}

		private unsafe void ReadBytesInternal(byte* data, int length)
		{
			if (this.GetBytesRead() + length > this.m_Length)
			{
				this.m_Context.m_FailedReads = this.m_Context.m_FailedReads + 1;
				UnsafeUtility.MemClear((void*)data, (long)length);
				return;
			}
			this.Flush();
			UnsafeUtility.MemCpy((void*)data, (void*)(this.m_BufferPtr + this.m_Context.m_ReadByteIndex), (long)length);
			this.m_Context.m_ReadByteIndex = this.m_Context.m_ReadByteIndex + length;
		}

		public unsafe void ReadBytes(NativeArray<byte> array)
		{
			this.ReadBytesInternal((byte*)array.GetUnsafePtr<byte>(), array.Length);
		}

		public unsafe void ReadBytes(Span<byte> span)
		{
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				byte* ptr = pinnableReference;
				this.ReadBytesInternal(ptr, span.Length);
			}
		}

		public int GetBytesRead()
		{
			return this.m_Context.m_ReadByteIndex - (this.m_Context.m_BitIndex >> 3);
		}

		public int GetBitsRead()
		{
			return (this.m_Context.m_ReadByteIndex << 3) - this.m_Context.m_BitIndex;
		}

		public void SeekSet(int pos)
		{
			if (pos > this.m_Length)
			{
				this.m_Context.m_FailedReads = this.m_Context.m_FailedReads + 1;
				return;
			}
			this.m_Context.m_ReadByteIndex = pos;
			this.m_Context.m_BitIndex = 0;
			this.m_Context.m_BitBuffer = 0UL;
		}

		public unsafe byte ReadByte()
		{
			byte b;
			this.ReadBytesInternal(&b, 1);
			return b;
		}

		public unsafe short ReadShort()
		{
			short num;
			this.ReadBytesInternal((byte*)(&num), 2);
			return num;
		}

		public unsafe ushort ReadUShort()
		{
			ushort num;
			this.ReadBytesInternal((byte*)(&num), 2);
			return num;
		}

		public unsafe int ReadInt()
		{
			int num;
			this.ReadBytesInternal((byte*)(&num), 4);
			return num;
		}

		public unsafe uint ReadUInt()
		{
			uint num;
			this.ReadBytesInternal((byte*)(&num), 4);
			return num;
		}

		public unsafe long ReadLong()
		{
			long num;
			this.ReadBytesInternal((byte*)(&num), 8);
			return num;
		}

		public unsafe ulong ReadULong()
		{
			ulong num;
			this.ReadBytesInternal((byte*)(&num), 8);
			return num;
		}

		public void Flush()
		{
			this.m_Context.m_ReadByteIndex = this.m_Context.m_ReadByteIndex - (this.m_Context.m_BitIndex >> 3);
			this.m_Context.m_BitIndex = 0;
			this.m_Context.m_BitBuffer = 0UL;
		}

		public unsafe short ReadShortNetworkByteOrder()
		{
			short num;
			this.ReadBytesInternal((byte*)(&num), 2);
			if (!DataStreamReader.IsLittleEndian)
			{
				return num;
			}
			return DataStreamReader.ByteSwap(num);
		}

		public ushort ReadUShortNetworkByteOrder()
		{
			return (ushort)this.ReadShortNetworkByteOrder();
		}

		public unsafe int ReadIntNetworkByteOrder()
		{
			int num;
			this.ReadBytesInternal((byte*)(&num), 4);
			if (!DataStreamReader.IsLittleEndian)
			{
				return num;
			}
			return DataStreamReader.ByteSwap(num);
		}

		public uint ReadUIntNetworkByteOrder()
		{
			return (uint)this.ReadIntNetworkByteOrder();
		}

		public float ReadFloat()
		{
			return new UIntFloat
			{
				intValue = (uint)this.ReadInt()
			}.floatValue;
		}

		public double ReadDouble()
		{
			return new UIntFloat
			{
				longValue = (ulong)this.ReadLong()
			}.doubleValue;
		}

		public uint ReadPackedUInt(in StreamCompressionModel model)
		{
			return this.ReadPackedUIntInternal(6, in model);
		}

		private unsafe uint ReadPackedUIntInternal(int maxSymbolLength, in StreamCompressionModel model)
		{
			this.FillBitBuffer();
			uint num = (1U << maxSymbolLength) - 1U;
			uint num2 = (uint)this.m_Context.m_BitBuffer & num;
			ushort num3 = *((ref model.decodeTable.FixedElementField) + (IntPtr)num2 * 2);
			int num4 = num3 >> 8;
			int num5 = (int)(num3 & 255);
			if (this.m_Context.m_BitIndex < num5)
			{
				this.m_Context.m_FailedReads = this.m_Context.m_FailedReads + 1;
				return 0U;
			}
			this.m_Context.m_BitBuffer = this.m_Context.m_BitBuffer >> num5;
			this.m_Context.m_BitIndex = this.m_Context.m_BitIndex - num5;
			uint num6 = *((ref model.bucketOffsets.FixedElementField) + (IntPtr)num4 * 4);
			byte b = *((ref model.bucketSizes.FixedElementField) + num4);
			return this.ReadRawBitsInternal((int)b) + num6;
		}

		private unsafe void FillBitBuffer()
		{
			while (this.m_Context.m_BitIndex <= 56 && this.m_Context.m_ReadByteIndex < this.m_Length)
			{
				ulong bitBuffer = this.m_Context.m_BitBuffer;
				int bufferPtr = this.m_BufferPtr;
				int readByteIndex = this.m_Context.m_ReadByteIndex;
				this.m_Context.m_ReadByteIndex = readByteIndex + 1;
				this.m_Context.m_BitBuffer = bitBuffer | ((ulong)(*(bufferPtr + readByteIndex)) << this.m_Context.m_BitIndex);
				this.m_Context.m_BitIndex = this.m_Context.m_BitIndex + 8;
			}
		}

		private uint ReadRawBitsInternal(int numbits)
		{
			if (this.m_Context.m_BitIndex < numbits)
			{
				this.m_Context.m_FailedReads = this.m_Context.m_FailedReads + 1;
				return 0U;
			}
			uint num = (uint)(this.m_Context.m_BitBuffer & ((1UL << numbits) - 1UL));
			this.m_Context.m_BitBuffer = this.m_Context.m_BitBuffer >> numbits;
			this.m_Context.m_BitIndex = this.m_Context.m_BitIndex - numbits;
			return num;
		}

		public uint ReadRawBits(int numbits)
		{
			this.FillBitBuffer();
			return this.ReadRawBitsInternal(numbits);
		}

		public ulong ReadPackedULong(in StreamCompressionModel model)
		{
			return (ulong)this.ReadPackedUInt(in model) | ((ulong)this.ReadPackedUInt(in model) << 32);
		}

		public int ReadPackedInt(in StreamCompressionModel model)
		{
			uint num = this.ReadPackedUInt(in model);
			return (int)((num >> 1) ^ -(int)(num & 1U));
		}

		public long ReadPackedLong(in StreamCompressionModel model)
		{
			ulong num = this.ReadPackedULong(in model);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		public float ReadPackedFloat(in StreamCompressionModel model)
		{
			return this.ReadPackedFloatDelta(0f, in model);
		}

		public double ReadPackedDouble(in StreamCompressionModel model)
		{
			return this.ReadPackedDoubleDelta(0.0, in model);
		}

		public int ReadPackedIntDelta(int baseline, in StreamCompressionModel model)
		{
			int num = this.ReadPackedInt(in model);
			return baseline - num;
		}

		public uint ReadPackedUIntDelta(uint baseline, in StreamCompressionModel model)
		{
			uint num = (uint)this.ReadPackedInt(in model);
			return baseline - num;
		}

		public long ReadPackedLongDelta(long baseline, in StreamCompressionModel model)
		{
			long num = this.ReadPackedLong(in model);
			return baseline - num;
		}

		public ulong ReadPackedULongDelta(ulong baseline, in StreamCompressionModel model)
		{
			ulong num = (ulong)this.ReadPackedLong(in model);
			return baseline - num;
		}

		public float ReadPackedFloatDelta(float baseline, in StreamCompressionModel model)
		{
			this.FillBitBuffer();
			if (this.ReadRawBitsInternal(1) == 0U)
			{
				return baseline;
			}
			int num = 32;
			return new UIntFloat
			{
				intValue = this.ReadRawBitsInternal(num)
			}.floatValue;
		}

		public unsafe double ReadPackedDoubleDelta(double baseline, in StreamCompressionModel model)
		{
			this.FillBitBuffer();
			if (this.ReadRawBitsInternal(1) == 0U)
			{
				return baseline;
			}
			int num = 32;
			UIntFloat uintFloat = default(UIntFloat);
			uint* ptr = (uint*)(&uintFloat.longValue);
			*ptr = this.ReadRawBitsInternal(num);
			this.FillBitBuffer();
			ptr[1] |= this.ReadRawBitsInternal(num);
			return uintFloat.doubleValue;
		}

		public unsafe FixedString32Bytes ReadFixedString32()
		{
			FixedString32Bytes fixedString32Bytes;
			byte* ptr = (byte*)(&fixedString32Bytes) + 2;
			*(short*)(&fixedString32Bytes) = (short)this.ReadFixedStringInternal(ptr, fixedString32Bytes.Capacity);
			return fixedString32Bytes;
		}

		public unsafe FixedString64Bytes ReadFixedString64()
		{
			FixedString64Bytes fixedString64Bytes;
			byte* ptr = (byte*)(&fixedString64Bytes) + 2;
			*(short*)(&fixedString64Bytes) = (short)this.ReadFixedStringInternal(ptr, fixedString64Bytes.Capacity);
			return fixedString64Bytes;
		}

		public unsafe FixedString128Bytes ReadFixedString128()
		{
			FixedString128Bytes fixedString128Bytes;
			byte* ptr = (byte*)(&fixedString128Bytes) + 2;
			*(short*)(&fixedString128Bytes) = (short)this.ReadFixedStringInternal(ptr, fixedString128Bytes.Capacity);
			return fixedString128Bytes;
		}

		public unsafe FixedString512Bytes ReadFixedString512()
		{
			FixedString512Bytes fixedString512Bytes;
			byte* ptr = (byte*)(&fixedString512Bytes) + 2;
			*(short*)(&fixedString512Bytes) = (short)this.ReadFixedStringInternal(ptr, fixedString512Bytes.Capacity);
			return fixedString512Bytes;
		}

		public unsafe FixedString4096Bytes ReadFixedString4096()
		{
			FixedString4096Bytes fixedString4096Bytes;
			byte* ptr = (byte*)(&fixedString4096Bytes) + 2;
			*(short*)(&fixedString4096Bytes) = (short)this.ReadFixedStringInternal(ptr, fixedString4096Bytes.Capacity);
			return fixedString4096Bytes;
		}

		public unsafe ushort ReadFixedString(NativeArray<byte> array)
		{
			return this.ReadFixedStringInternal((byte*)array.GetUnsafePtr<byte>(), array.Length);
		}

		private unsafe ushort ReadFixedStringInternal(byte* data, int maxLength)
		{
			ushort num = this.ReadUShort();
			if ((int)num > maxLength)
			{
				return 0;
			}
			this.ReadBytesInternal(data, (int)num);
			return num;
		}

		public unsafe FixedString32Bytes ReadPackedFixedString32Delta(FixedString32Bytes baseline, in StreamCompressionModel model)
		{
			FixedString32Bytes fixedString32Bytes;
			byte* ptr = (byte*)(&fixedString32Bytes) + 2;
			*(short*)(&fixedString32Bytes) = (short)this.ReadPackedFixedStringDeltaInternal(ptr, fixedString32Bytes.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), in model);
			return fixedString32Bytes;
		}

		public unsafe FixedString64Bytes ReadPackedFixedString64Delta(FixedString64Bytes baseline, in StreamCompressionModel model)
		{
			FixedString64Bytes fixedString64Bytes;
			byte* ptr = (byte*)(&fixedString64Bytes) + 2;
			*(short*)(&fixedString64Bytes) = (short)this.ReadPackedFixedStringDeltaInternal(ptr, fixedString64Bytes.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), in model);
			return fixedString64Bytes;
		}

		public unsafe FixedString128Bytes ReadPackedFixedString128Delta(FixedString128Bytes baseline, in StreamCompressionModel model)
		{
			FixedString128Bytes fixedString128Bytes;
			byte* ptr = (byte*)(&fixedString128Bytes) + 2;
			*(short*)(&fixedString128Bytes) = (short)this.ReadPackedFixedStringDeltaInternal(ptr, fixedString128Bytes.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), in model);
			return fixedString128Bytes;
		}

		public unsafe FixedString512Bytes ReadPackedFixedString512Delta(FixedString512Bytes baseline, in StreamCompressionModel model)
		{
			FixedString512Bytes fixedString512Bytes;
			byte* ptr = (byte*)(&fixedString512Bytes) + 2;
			*(short*)(&fixedString512Bytes) = (short)this.ReadPackedFixedStringDeltaInternal(ptr, fixedString512Bytes.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), in model);
			return fixedString512Bytes;
		}

		public unsafe FixedString4096Bytes ReadPackedFixedString4096Delta(FixedString4096Bytes baseline, in StreamCompressionModel model)
		{
			FixedString4096Bytes fixedString4096Bytes;
			byte* ptr = (byte*)(&fixedString4096Bytes) + 2;
			*(short*)(&fixedString4096Bytes) = (short)this.ReadPackedFixedStringDeltaInternal(ptr, fixedString4096Bytes.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), in model);
			return fixedString4096Bytes;
		}

		public unsafe ushort ReadPackedFixedStringDelta(NativeArray<byte> data, NativeArray<byte> baseData, in StreamCompressionModel model)
		{
			return this.ReadPackedFixedStringDeltaInternal((byte*)data.GetUnsafePtr<byte>(), data.Length, (byte*)baseData.GetUnsafePtr<byte>(), (ushort)baseData.Length, in model);
		}

		private unsafe ushort ReadPackedFixedStringDeltaInternal(byte* data, int maxLength, byte* baseData, ushort baseLength, in StreamCompressionModel model)
		{
			uint num = this.ReadPackedUIntDelta((uint)baseLength, in model);
			if (num > (uint)maxLength)
			{
				return 0;
			}
			if (num <= (uint)baseLength)
			{
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					data[num2] = (byte)this.ReadPackedUIntDelta((uint)baseData[num2], in model);
					num2++;
				}
			}
			else
			{
				for (int i = 0; i < (int)baseLength; i++)
				{
					data[i] = (byte)this.ReadPackedUIntDelta((uint)baseData[i], in model);
				}
				int num3 = (int)baseLength;
				while ((long)num3 < (long)((ulong)num))
				{
					data[num3] = (byte)this.ReadPackedUInt(in model);
					num3++;
				}
			}
			return (ushort)num;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal readonly void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckBits(int numBits)
		{
			if (numBits < 0 || numBits > 32)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid number of bits specified: {0}! Valid range is (0, 32) inclusive.", numBits));
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe byte* m_BufferPtr;

		private DataStreamReader.Context m_Context;

		private int m_Length;

		private struct Context
		{
			public int m_ReadByteIndex;

			public int m_BitIndex;

			public ulong m_BitBuffer;

			public int m_FailedReads;
		}
	}
}
