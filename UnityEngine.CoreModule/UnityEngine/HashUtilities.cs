using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine
{
	public static class HashUtilities
	{
		public unsafe static void AppendHash(ref Hash128 inHash, ref Hash128 outHash)
		{
			fixed (Hash128* ptr = &outHash)
			{
				fixed (Hash128* ptr2 = &inHash)
				{
					HashUnsafeUtilities.ComputeHash128((void*)ptr2, (ulong)((long)sizeof(Hash128)), ptr);
				}
			}
		}

		public unsafe static void QuantisedMatrixHash(ref Matrix4x4 value, ref Hash128 hash)
		{
			fixed (Hash128* ptr = &hash)
			{
				int* ptr2 = stackalloc int[checked(16 * 4)];
				for (int i = 0; i < 16; i++)
				{
					ptr2[i] = (int)(value[i] * 1000f + 0.5f);
				}
				HashUnsafeUtilities.ComputeHash128((void*)ptr2, 64UL, ptr);
			}
		}

		public unsafe static void QuantisedVectorHash(ref Vector3 value, ref Hash128 hash)
		{
			fixed (Hash128* ptr = &hash)
			{
				int* ptr2 = stackalloc int[checked(3 * 4)];
				for (int i = 0; i < 3; i++)
				{
					ptr2[i] = (int)(value[i] * 1000f + 0.5f);
				}
				HashUnsafeUtilities.ComputeHash128((void*)ptr2, 12UL, ptr);
			}
		}

		public unsafe static void ComputeHash128<T>(ref T value, ref Hash128 hash) where T : struct
		{
			void* ptr = UnsafeUtility.AddressOf<T>(ref value);
			ulong num = (ulong)((long)UnsafeUtility.SizeOf<T>());
			Hash128* ptr2 = (Hash128*)UnsafeUtility.AddressOf<Hash128>(ref hash);
			HashUnsafeUtilities.ComputeHash128(ptr, num, ptr2);
		}
	}
}
