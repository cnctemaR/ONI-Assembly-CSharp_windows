using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	internal static class MemorySizeExtensions
	{
		public static bool IsBroadcast(this MemorySize memorySize)
		{
			return memorySize >= MemorySize.Broadcast32_Float16;
		}

		private unsafe static MemorySizeInfo[] GetMemorySizeInfos()
		{
			global::System.ReadOnlySpan<byte> readOnlySpan = new global::System.ReadOnlySpan<byte>((void*)(&<b37590d4-39fb-478a-88de-d293f3364852><PrivateImplementationDetails>.9F8A914BAE36A263FB33BFC11BF247BB2AACC1F65E65F5B3AB22B8DC6991FD68), 486);
			ushort[] array = new ushort[]
			{
				0, 1, 2, 4, 6, 8, 10, 14, 16, 28,
				32, 48, 64, 94, 108, 512
			};
			MemorySizeInfo[] array2 = new MemorySizeInfo[162];
			int i = 0;
			int num = 0;
			while (i < array2.Length)
			{
				MemorySize memorySize = (MemorySize)(*readOnlySpan[num]);
				uint num2 = (uint)(((int)(*readOnlySpan[num + 2]) << 8) | (int)(*readOnlySpan[num + 1]));
				ushort num3 = array[(int)(num2 & 31U)];
				ushort num4 = array[(int)((num2 >> 5) & 31U)];
				array2[i] = new MemorySizeInfo((MemorySize)i, (int)num3, (int)num4, memorySize, (num2 & 32768U) > 0U, i >= 112);
				i++;
				num += 3;
			}
			return array2;
		}

		public static MemorySizeInfo GetInfo(this MemorySize memorySize)
		{
			MemorySizeInfo[] memorySizeInfos = MemorySizeExtensions.MemorySizeInfos;
			if (memorySize >= (MemorySize)memorySizeInfos.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_memorySize();
			}
			return memorySizeInfos[(int)memorySize];
		}

		public static int GetSize(this MemorySize memorySize)
		{
			return memorySize.GetInfo().Size;
		}

		public static int GetElementSize(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementSize;
		}

		public static MemorySize GetElementType(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementType;
		}

		public static MemorySizeInfo GetElementTypeInfo(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementType.GetInfo();
		}

		public static bool IsSigned(this MemorySize memorySize)
		{
			return memorySize.GetInfo().IsSigned;
		}

		public static bool IsPacked(this MemorySize memorySize)
		{
			return memorySize.GetInfo().IsPacked;
		}

		public static int GetElementCount(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementCount;
		}

		[Nullable(1)]
		internal static readonly MemorySizeInfo[] MemorySizeInfos = MemorySizeExtensions.GetMemorySizeInfos();
	}
}
