using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public static class DataStreamExtensions
	{
		public unsafe static DataStreamWriter Create(byte* data, int length)
		{
			return new DataStreamWriter(NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)data, length, Allocator.None));
		}

		public unsafe static bool WriteBytesUnsafe(this DataStreamWriter writer, byte* data, int bytes)
		{
			NativeArray<byte> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)data, bytes, Allocator.None);
			return writer.WriteBytes(nativeArray);
		}

		public unsafe static void ReadBytesUnsafe(this DataStreamReader reader, byte* data, int length)
		{
			NativeArray<byte> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)data, length, Allocator.None);
			reader.ReadBytes(nativeArray);
		}

		public unsafe static ushort ReadFixedStringUnsafe(this DataStreamReader reader, byte* data, int maxLength)
		{
			NativeArray<byte> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)data, maxLength, Allocator.Temp);
			return reader.ReadFixedString(nativeArray);
		}

		public unsafe static ushort ReadPackedFixedStringDeltaUnsafe(this DataStreamReader reader, byte* data, int maxLength, byte* baseData, ushort baseLength, StreamCompressionModel model)
		{
			NativeArray<byte> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)data, maxLength, Allocator.Temp);
			NativeArray<byte> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)baseData, (int)baseLength, Allocator.Temp);
			return reader.ReadPackedFixedStringDelta(nativeArray, nativeArray2, in model);
		}

		public unsafe static void* GetUnsafeReadOnlyPtr(this DataStreamReader reader)
		{
			return (void*)reader.m_BufferPtr;
		}
	}
}
