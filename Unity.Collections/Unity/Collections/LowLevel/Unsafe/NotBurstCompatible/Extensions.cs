using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe.NotBurstCompatible
{
	public static class Extensions
	{
		public static T[] ToArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeParallelHashSet<T> set) where T : struct, ValueType, IEquatable<T>
		{
			NativeArray<T> nativeArray = set.ToNativeArray(Allocator.TempJob);
			T[] array = nativeArray.ToArray();
			nativeArray.Dispose();
			return array;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public unsafe static void AddNBC(this UnsafeAppendBuffer buffer, string value)
		{
			if (value != null)
			{
				buffer.Add<int>(value.Length);
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					buffer.Add((void*)ptr, 2 * value.Length);
				}
				return;
			}
			buffer.Add<int>(-1);
		}

		[ExcludeFromBurstCompatTesting("Returns managed array")]
		public unsafe static byte[] ToBytesNBC(this UnsafeAppendBuffer buffer)
		{
			byte[] array2;
			byte[] array = (array2 = new byte[buffer.Length]);
			byte* ptr;
			if (array == null || array2.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array2[0];
			}
			UnsafeUtility.MemCpy((void*)ptr, (void*)buffer.Ptr, (long)buffer.Length);
			array2 = null;
			return array;
		}

		[ExcludeFromBurstCompatTesting("Managed string out argument")]
		public unsafe static void ReadNextNBC(this UnsafeAppendBuffer.Reader reader, out string value)
		{
			int num;
			reader.ReadNext<int>(out num);
			if (num != -1)
			{
				value = new string('0', num);
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					int num2 = num * 2;
					UnsafeUtility.MemCpy((void*)ptr, reader.ReadNext(num2), (long)num2);
				}
				return;
			}
			value = null;
		}
	}
}
