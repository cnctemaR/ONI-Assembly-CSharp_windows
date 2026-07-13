using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.Collections
{
	[MovedFrom(true, "Unity.Networking.Transport", "Unity.Networking.Transport", null)]
	[GenerateTestsForBurstCompatibility]
	public struct DataStreamWriter
	{
		public unsafe static bool IsLittleEndian
		{
			get
			{
				uint num = 1U;
				byte* ptr = (byte*)(&num);
				return *ptr == 1;
			}
		}

		public DataStreamWriter(int length, AllocatorManager.AllocatorHandle allocator)
		{
			DataStreamWriter.Initialize(out this, CollectionHelper.CreateNativeArray<byte>(length, allocator, NativeArrayOptions.ClearMemory));
		}

		public DataStreamWriter(NativeArray<byte> data)
		{
			DataStreamWriter.Initialize(out this, data);
		}

		public unsafe DataStreamWriter(byte* data, int length)
		{
			NativeArray<byte> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)data, length, Allocator.Invalid);
			DataStreamWriter.Initialize(out this, nativeArray);
		}

		public unsafe NativeArray<byte> AsNativeArray()
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)this.m_Data.buffer, this.Length, Allocator.Invalid);
		}

		private unsafe static void Initialize(out DataStreamWriter self, NativeArray<byte> data)
		{
			self.m_SendHandleData = IntPtr.Zero;
			self.m_Data.capacity = data.Length;
			self.m_Data.length = 0;
			self.m_Data.buffer = (byte*)data.GetUnsafePtr<byte>();
			self.m_Data.bitBuffer = 0UL;
			self.m_Data.bitIndex = 0;
			self.m_Data.failedWrites = 0;
		}

		private static short ByteSwap(short val)
		{
			return (short)(((int)(val & 255) << 8) | ((val >> 8) & 255));
		}

		private static int ByteSwap(int val)
		{
			return ((val & 255) << 24) | ((val & 65280) << 8) | ((val >> 8) & 65280) | ((val >> 24) & 255);
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.buffer != null;
			}
		}

		public readonly bool HasFailedWrites
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.failedWrites > 0;
			}
		}

		public readonly int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.capacity;
			}
		}

		public int Length
		{
			get
			{
				this.SyncBitData();
				return this.m_Data.length + (this.m_Data.bitIndex + 7 >> 3);
			}
		}

		public int LengthInBits
		{
			get
			{
				this.SyncBitData();
				return this.m_Data.length * 8 + this.m_Data.bitIndex;
			}
		}

		private unsafe void SyncBitData()
		{
			int i = this.m_Data.bitIndex;
			if (i <= 0)
			{
				return;
			}
			ulong num = this.m_Data.bitBuffer;
			int num2 = 0;
			while (i > 0)
			{
				this.m_Data.buffer[this.m_Data.length + num2] = (byte)num;
				i -= 8;
				num >>= 8;
				num2++;
			}
		}

		public unsafe void Flush()
		{
			while (this.m_Data.bitIndex > 0)
			{
				ref byte buffer = ref *this.m_Data.buffer;
				int length = this.m_Data.length;
				this.m_Data.length = length + 1;
				*((ref buffer) + length) = (byte)this.m_Data.bitBuffer;
				this.m_Data.bitIndex = this.m_Data.bitIndex - 8;
				this.m_Data.bitBuffer = this.m_Data.bitBuffer >> 8;
			}
			this.m_Data.bitIndex = 0;
		}

		private unsafe bool WriteBytesInternal(byte* data, int bytes)
		{
			if (this.m_Data.length + (this.m_Data.bitIndex + 7 >> 3) + bytes > this.m_Data.capacity)
			{
				this.m_Data.failedWrites = this.m_Data.failedWrites + 1;
				return false;
			}
			this.Flush();
			UnsafeUtility.MemCpy((void*)(this.m_Data.buffer + this.m_Data.length), (void*)data, (long)bytes);
			this.m_Data.length = this.m_Data.length + bytes;
			return true;
		}

		public unsafe bool WriteByte(byte value)
		{
			return this.WriteBytesInternal(&value, 1);
		}

		public unsafe bool WriteBytes(NativeArray<byte> value)
		{
			return this.WriteBytesInternal((byte*)value.GetUnsafeReadOnlyPtr<byte>(), value.Length);
		}

		public unsafe bool WriteBytes(Span<byte> value)
		{
			fixed (byte* pinnableReference = value.GetPinnableReference())
			{
				byte* ptr = pinnableReference;
				return this.WriteBytesInternal(ptr, value.Length);
			}
		}

		public unsafe bool WriteShort(short value)
		{
			return this.WriteBytesInternal((byte*)(&value), 2);
		}

		public unsafe bool WriteUShort(ushort value)
		{
			return this.WriteBytesInternal((byte*)(&value), 2);
		}

		public unsafe bool WriteInt(int value)
		{
			return this.WriteBytesInternal((byte*)(&value), 4);
		}

		public unsafe bool WriteUInt(uint value)
		{
			return this.WriteBytesInternal((byte*)(&value), 4);
		}

		public unsafe bool WriteLong(long value)
		{
			return this.WriteBytesInternal((byte*)(&value), 8);
		}

		public unsafe bool WriteULong(ulong value)
		{
			return this.WriteBytesInternal((byte*)(&value), 8);
		}

		public unsafe bool WriteShortNetworkByteOrder(short value)
		{
			short num = (DataStreamWriter.IsLittleEndian ? DataStreamWriter.ByteSwap(value) : value);
			return this.WriteBytesInternal((byte*)(&num), 2);
		}

		public bool WriteUShortNetworkByteOrder(ushort value)
		{
			return this.WriteShortNetworkByteOrder((short)value);
		}

		public unsafe bool WriteIntNetworkByteOrder(int value)
		{
			int num = (DataStreamWriter.IsLittleEndian ? DataStreamWriter.ByteSwap(value) : value);
			return this.WriteBytesInternal((byte*)(&num), 4);
		}

		public bool WriteUIntNetworkByteOrder(uint value)
		{
			return this.WriteIntNetworkByteOrder((int)value);
		}

		public bool WriteFloat(float value)
		{
			return this.WriteInt((int)new UIntFloat
			{
				floatValue = value
			}.intValue);
		}

		public bool WriteDouble(double value)
		{
			return this.WriteLong((long)new UIntFloat
			{
				doubleValue = value
			}.longValue);
		}

		private unsafe void FlushBits()
		{
			while (this.m_Data.bitIndex >= 8)
			{
				ref byte buffer = ref *this.m_Data.buffer;
				int length = this.m_Data.length;
				this.m_Data.length = length + 1;
				*((ref buffer) + length) = (byte)this.m_Data.bitBuffer;
				this.m_Data.bitIndex = this.m_Data.bitIndex - 8;
				this.m_Data.bitBuffer = this.m_Data.bitBuffer >> 8;
			}
		}

		private void WriteRawBitsInternal(uint value, int numbits)
		{
			this.m_Data.bitBuffer = this.m_Data.bitBuffer | ((ulong)value << this.m_Data.bitIndex);
			this.m_Data.bitIndex = this.m_Data.bitIndex + numbits;
		}

		public bool WriteRawBits(uint value, int numbits)
		{
			if (this.m_Data.length + (this.m_Data.bitIndex + numbits + 7 >> 3) > this.m_Data.capacity)
			{
				this.m_Data.failedWrites = this.m_Data.failedWrites + 1;
				return false;
			}
			this.WriteRawBitsInternal(value, numbits);
			this.FlushBits();
			return true;
		}

		public unsafe bool WritePackedUInt(uint value, in StreamCompressionModel model)
		{
			int num = model.CalculateBucket(value);
			uint num2 = *((ref model.bucketOffsets.FixedElementField) + (IntPtr)num * 4);
			int num3 = (int)(*((ref model.bucketSizes.FixedElementField) + num));
			ushort num4 = *((ref model.encodeTable.FixedElementField) + (IntPtr)num * 2);
			if (this.m_Data.length + (this.m_Data.bitIndex + (int)(num4 & 255) + num3 + 7 >> 3) > this.m_Data.capacity)
			{
				this.m_Data.failedWrites = this.m_Data.failedWrites + 1;
				return false;
			}
			this.WriteRawBitsInternal((uint)(num4 >> 8), (int)(num4 & 255));
			this.WriteRawBitsInternal(value - num2, num3);
			this.FlushBits();
			return true;
		}

		public unsafe bool WritePackedULong(ulong value, in StreamCompressionModel model)
		{
			uint* ptr = (uint*)(&value);
			return this.WritePackedUInt(*ptr, in model) & this.WritePackedUInt(ptr[1], in model);
		}

		public bool WritePackedInt(int value, in StreamCompressionModel model)
		{
			uint num = (uint)((value >> 31) ^ (value << 1));
			return this.WritePackedUInt(num, in model);
		}

		public bool WritePackedLong(long value, in StreamCompressionModel model)
		{
			ulong num = (ulong)((value >> 63) ^ (value << 1));
			return this.WritePackedULong(num, in model);
		}

		public bool WritePackedFloat(float value, in StreamCompressionModel model)
		{
			return this.WritePackedFloatDelta(value, 0f, in model);
		}

		public bool WritePackedDouble(double value, in StreamCompressionModel model)
		{
			return this.WritePackedDoubleDelta(value, 0.0, in model);
		}

		public bool WritePackedUIntDelta(uint value, uint baseline, in StreamCompressionModel model)
		{
			int num = (int)(baseline - value);
			return this.WritePackedInt(num, in model);
		}

		public bool WritePackedIntDelta(int value, int baseline, in StreamCompressionModel model)
		{
			int num = baseline - value;
			return this.WritePackedInt(num, in model);
		}

		public bool WritePackedLongDelta(long value, long baseline, in StreamCompressionModel model)
		{
			long num = baseline - value;
			return this.WritePackedLong(num, in model);
		}

		public bool WritePackedULongDelta(ulong value, ulong baseline, in StreamCompressionModel model)
		{
			long num = (long)(baseline - value);
			return this.WritePackedLong(num, in model);
		}

		public bool WritePackedFloatDelta(float value, float baseline, in StreamCompressionModel model)
		{
			int num = 0;
			if (value != baseline)
			{
				num = 32;
			}
			if (this.m_Data.length + (this.m_Data.bitIndex + 1 + num + 7 >> 3) > this.m_Data.capacity)
			{
				this.m_Data.failedWrites = this.m_Data.failedWrites + 1;
				return false;
			}
			if (num == 0)
			{
				this.WriteRawBitsInternal(0U, 1);
			}
			else
			{
				this.WriteRawBitsInternal(1U, 1);
				this.WriteRawBitsInternal(new UIntFloat
				{
					floatValue = value
				}.intValue, num);
			}
			this.FlushBits();
			return true;
		}

		public unsafe bool WritePackedDoubleDelta(double value, double baseline, in StreamCompressionModel model)
		{
			int num = 0;
			if (value != baseline)
			{
				num = 64;
			}
			if (this.m_Data.length + (this.m_Data.bitIndex + 1 + num + 7 >> 3) > this.m_Data.capacity)
			{
				this.m_Data.failedWrites = this.m_Data.failedWrites + 1;
				return false;
			}
			if (num == 0)
			{
				this.WriteRawBitsInternal(0U, 1);
			}
			else
			{
				this.WriteRawBitsInternal(1U, 1);
				UIntFloat uintFloat = default(UIntFloat);
				uintFloat.doubleValue = value;
				uint* ptr = (uint*)(&uintFloat.longValue);
				this.WriteRawBitsInternal(*ptr, 32);
				this.FlushBits();
				this.WriteRawBitsInternal(ptr[1], 32);
			}
			this.FlushBits();
			return true;
		}

		public unsafe bool WriteFixedString32(FixedString32Bytes str)
		{
			int num = (int)(*(ushort*)(&str) + 2);
			byte* ptr = (byte*)(&str);
			return this.WriteBytesInternal(ptr, num);
		}

		public unsafe bool WriteFixedString64(FixedString64Bytes str)
		{
			int num = (int)(*(ushort*)(&str) + 2);
			byte* ptr = (byte*)(&str);
			return this.WriteBytesInternal(ptr, num);
		}

		public unsafe bool WriteFixedString128(FixedString128Bytes str)
		{
			int num = (int)(*(ushort*)(&str) + 2);
			byte* ptr = (byte*)(&str);
			return this.WriteBytesInternal(ptr, num);
		}

		public unsafe bool WriteFixedString512(FixedString512Bytes str)
		{
			int num = (int)(*(ushort*)(&str) + 2);
			byte* ptr = (byte*)(&str);
			return this.WriteBytesInternal(ptr, num);
		}

		public unsafe bool WriteFixedString4096(FixedString4096Bytes str)
		{
			int num = (int)(*(ushort*)(&str) + 2);
			byte* ptr = (byte*)(&str);
			return this.WriteBytesInternal(ptr, num);
		}

		public unsafe bool WritePackedFixedString32Delta(FixedString32Bytes str, FixedString32Bytes baseline, in StreamCompressionModel model)
		{
			ushort num = *(ushort*)(&str);
			byte* ptr = (byte*)(&str) + 2;
			return this.WritePackedFixedStringDelta(ptr, (uint)num, (byte*)(&baseline) + 2, (uint)(*(ushort*)(&baseline)), in model);
		}

		public unsafe bool WritePackedFixedString64Delta(FixedString64Bytes str, FixedString64Bytes baseline, in StreamCompressionModel model)
		{
			ushort num = *(ushort*)(&str);
			byte* ptr = (byte*)(&str) + 2;
			return this.WritePackedFixedStringDelta(ptr, (uint)num, (byte*)(&baseline) + 2, (uint)(*(ushort*)(&baseline)), in model);
		}

		public unsafe bool WritePackedFixedString128Delta(FixedString128Bytes str, FixedString128Bytes baseline, in StreamCompressionModel model)
		{
			ushort num = *(ushort*)(&str);
			byte* ptr = (byte*)(&str) + 2;
			return this.WritePackedFixedStringDelta(ptr, (uint)num, (byte*)(&baseline) + 2, (uint)(*(ushort*)(&baseline)), in model);
		}

		public unsafe bool WritePackedFixedString512Delta(FixedString512Bytes str, FixedString512Bytes baseline, in StreamCompressionModel model)
		{
			ushort num = *(ushort*)(&str);
			byte* ptr = (byte*)(&str) + 2;
			return this.WritePackedFixedStringDelta(ptr, (uint)num, (byte*)(&baseline) + 2, (uint)(*(ushort*)(&baseline)), in model);
		}

		public unsafe bool WritePackedFixedString4096Delta(FixedString4096Bytes str, FixedString4096Bytes baseline, in StreamCompressionModel model)
		{
			ushort num = *(ushort*)(&str);
			byte* ptr = (byte*)(&str) + 2;
			return this.WritePackedFixedStringDelta(ptr, (uint)num, (byte*)(&baseline) + 2, (uint)(*(ushort*)(&baseline)), in model);
		}

		private unsafe bool WritePackedFixedStringDelta(byte* data, uint length, byte* baseData, uint baseLength, in StreamCompressionModel model)
		{
			DataStreamWriter.StreamData data2 = this.m_Data;
			if (!this.WritePackedUIntDelta(length, baseLength, in model))
			{
				return false;
			}
			bool flag = false;
			if (length <= baseLength)
			{
				for (uint num = 0U; num < length; num += 1U)
				{
					flag |= !this.WritePackedUIntDelta((uint)data[num], (uint)baseData[num], in model);
				}
			}
			else
			{
				for (uint num2 = 0U; num2 < baseLength; num2 += 1U)
				{
					flag |= !this.WritePackedUIntDelta((uint)data[num2], (uint)baseData[num2], in model);
				}
				for (uint num3 = baseLength; num3 < length; num3 += 1U)
				{
					flag |= !this.WritePackedUInt((uint)data[num3], in model);
				}
			}
			if (flag)
			{
				this.m_Data = data2;
				this.m_Data.failedWrites = this.m_Data.failedWrites + 1;
			}
			return !flag;
		}

		public void Clear()
		{
			this.m_Data.length = 0;
			this.m_Data.bitIndex = 0;
			this.m_Data.bitBuffer = 0UL;
			this.m_Data.failedWrites = 0;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckWrite()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckAllocator(AllocatorManager.AllocatorHandle allocator)
		{
			if (allocator.ToAllocator != Allocator.Temp)
			{
				throw new InvalidOperationException("DataStreamWriters can only be created with temp memory");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckBits(uint value, int numBits)
		{
			if (numBits < 0 || numBits > 32)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid number of bits specified: {0}! Valid range is (0, 32) inclusive.", numBits));
			}
			ulong num = 1UL << numBits;
			if ((ulong)value >= num)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value {0} does not fit in the specified number of bits: {1}! Range (inclusive) is (0, {2})!", value, numBits, num - 1UL));
			}
		}

		[NativeDisableUnsafePtrRestriction]
		private DataStreamWriter.StreamData m_Data;

		public IntPtr m_SendHandleData;

		private struct StreamData
		{
			public unsafe byte* buffer;

			public int length;

			public int capacity;

			public ulong bitBuffer;

			public int bitIndex;

			public int failedWrites;
		}
	}
}
