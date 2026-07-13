using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	[GenerateTestsForBurstCompatibility]
	[BurstCompile]
	[GenerateTestsForBurstCompatibility]
	public static class xxHash3
	{
		internal unsafe static void Avx2HashLongInternalLoop(ulong* acc, byte* input, byte* dest, long length, byte* secret, int isHash64)
		{
			if (X86.Avx2.IsAvx2Supported)
			{
				long num = (length - 1L) / 1024L;
				int num2 = 0;
				while ((long)num2 < num)
				{
					xxHash3.Avx2Accumulate(acc, input + num2 * 1024, (dest == null) ? null : (dest + num2 * 1024), secret, 16L, isHash64);
					xxHash3.Avx2ScrambleAcc(acc, secret + 192 - 64);
					num2++;
				}
				long num3 = (length - 1L - 1024L * num) / 64L;
				xxHash3.Avx2Accumulate(acc, input + num * 1024L, (dest == null) ? null : (dest + num * 1024L), secret, num3, isHash64);
				byte* ptr = input + length - 64;
				xxHash3.Avx2Accumulate512(acc, ptr, null, secret + 192 - 64 - 7);
				if (dest != null)
				{
					long num4 = length % 64L;
					if (num4 != 0L)
					{
						UnsafeUtility.MemCpy((void*)(dest + length - num4), (void*)(input + length - num4), num4);
					}
				}
			}
		}

		internal unsafe static void Avx2ScrambleAcc(ulong* acc, byte* secret)
		{
			if (X86.Avx2.IsAvx2Supported)
			{
				v256 v = X86.Avx.mm256_set1_epi32(-1640531535);
				v256 v2 = *(v256*)acc;
				v256 v3 = X86.Avx2.mm256_srli_epi64(v2, 47);
				v256 v4 = X86.Avx2.mm256_xor_si256(v2, v3);
				v256 v5 = X86.Avx.mm256_loadu_si256((void*)secret);
				v256 v6 = X86.Avx2.mm256_xor_si256(v4, v5);
				v256 v7 = X86.Avx2.mm256_shuffle_epi32(v6, X86.Sse.SHUFFLE(0, 3, 0, 1));
				v256 v8 = X86.Avx2.mm256_mul_epu32(v6, v);
				v256 v9 = X86.Avx2.mm256_mul_epu32(v7, v);
				*(v256*)acc = X86.Avx2.mm256_add_epi64(v8, X86.Avx2.mm256_slli_epi64(v9, 32));
				v256 v10 = *(v256*)(acc + sizeof(v256) / 8);
				v3 = X86.Avx2.mm256_srli_epi64(v10, 47);
				v256 v11 = X86.Avx2.mm256_xor_si256(v10, v3);
				v5 = X86.Avx.mm256_loadu_si256((void*)(secret + sizeof(v256)));
				v256 v12 = X86.Avx2.mm256_xor_si256(v11, v5);
				v7 = X86.Avx2.mm256_shuffle_epi32(v12, X86.Sse.SHUFFLE(0, 3, 0, 1));
				v8 = X86.Avx2.mm256_mul_epu32(v12, v);
				v9 = X86.Avx2.mm256_mul_epu32(v7, v);
				*(v256*)(acc + sizeof(v256) / 8) = X86.Avx2.mm256_add_epi64(v8, X86.Avx2.mm256_slli_epi64(v9, 32));
			}
		}

		internal unsafe static void Avx2Accumulate(ulong* acc, byte* input, byte* dest, byte* secret, long nbStripes, int isHash64)
		{
			if (X86.Avx2.IsAvx2Supported)
			{
				int num = 0;
				while ((long)num < nbStripes)
				{
					byte* ptr = input + num * 64;
					xxHash3.Avx2Accumulate512(acc, ptr, (dest == null) ? null : (dest + num * 64), secret + num * 8);
					num++;
				}
			}
		}

		internal unsafe static void Avx2Accumulate512(ulong* acc, byte* input, byte* dest, byte* secret)
		{
			if (X86.Avx2.IsAvx2Supported)
			{
				v256 v = X86.Avx.mm256_loadu_si256((void*)input);
				v256 v2 = X86.Avx.mm256_loadu_si256((void*)secret);
				v256 v3 = X86.Avx2.mm256_xor_si256(v, v2);
				if (dest != null)
				{
					X86.Avx.mm256_storeu_si256((void*)dest, v);
				}
				v256 v4 = X86.Avx2.mm256_shuffle_epi32(v3, X86.Sse.SHUFFLE(0, 3, 0, 1));
				v256 v5 = X86.Avx2.mm256_mul_epu32(v3, v4);
				v256 v6 = X86.Avx2.mm256_shuffle_epi32(v, X86.Sse.SHUFFLE(1, 0, 3, 2));
				v256 v7 = X86.Avx2.mm256_add_epi64(*(v256*)acc, v6);
				*(v256*)acc = X86.Avx2.mm256_add_epi64(v5, v7);
				v = X86.Avx.mm256_loadu_si256((void*)(input + sizeof(v256)));
				v2 = X86.Avx.mm256_loadu_si256((void*)(secret + sizeof(v256)));
				v256 v8 = X86.Avx2.mm256_xor_si256(v, v2);
				if (dest != null)
				{
					X86.Avx.mm256_storeu_si256((void*)(dest + 32), v);
				}
				v4 = X86.Avx2.mm256_shuffle_epi32(v8, X86.Sse.SHUFFLE(0, 3, 0, 1));
				v5 = X86.Avx2.mm256_mul_epu32(v8, v4);
				v6 = X86.Avx2.mm256_shuffle_epi32(v, X86.Sse.SHUFFLE(1, 0, 3, 2));
				v7 = X86.Avx2.mm256_add_epi64(*(v256*)(acc + sizeof(v256) / 8), v6);
				*(v256*)(acc + sizeof(v256) / 8) = X86.Avx2.mm256_add_epi64(v5, v7);
			}
		}

		public unsafe static uint2 Hash64(void* input, long length)
		{
			byte[] kSecret;
			void* ptr;
			if ((kSecret = xxHashDefaultKey.kSecret) == null || kSecret.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = (void*)(&kSecret[0]);
			}
			return xxHash3.ToUint2(xxHash3.Hash64Internal((byte*)input, null, length, (byte*)ptr, 0UL));
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static uint2 Hash64<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(in T input) where T : struct, ValueType
		{
			return xxHash3.Hash64(UnsafeUtilityExtensions.AddressOf<T>(in input), (long)UnsafeUtility.SizeOf<T>());
		}

		public unsafe static uint2 Hash64(void* input, long length, ulong seed)
		{
			byte[] kSecret;
			byte* ptr;
			if ((kSecret = xxHashDefaultKey.kSecret) == null || kSecret.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &kSecret[0];
			}
			return xxHash3.ToUint2(xxHash3.Hash64Internal((byte*)input, null, length, ptr, seed));
		}

		public unsafe static uint4 Hash128(void* input, long length)
		{
			byte[] kSecret;
			void* ptr;
			if ((kSecret = xxHashDefaultKey.kSecret) == null || kSecret.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = (void*)(&kSecret[0]);
			}
			uint4 @uint;
			xxHash3.Hash128Internal((byte*)input, null, length, (byte*)ptr, 0UL, out @uint);
			return @uint;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static uint4 Hash128<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(in T input) where T : struct, ValueType
		{
			return xxHash3.Hash128(UnsafeUtilityExtensions.AddressOf<T>(in input), (long)UnsafeUtility.SizeOf<T>());
		}

		public unsafe static uint4 Hash128(void* input, void* destination, long length)
		{
			byte[] kSecret;
			byte* ptr;
			if ((kSecret = xxHashDefaultKey.kSecret) == null || kSecret.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &kSecret[0];
			}
			uint4 @uint;
			xxHash3.Hash128Internal((byte*)input, (byte*)destination, length, ptr, 0UL, out @uint);
			return @uint;
		}

		public unsafe static uint4 Hash128(void* input, long length, ulong seed)
		{
			byte[] kSecret;
			byte* ptr;
			if ((kSecret = xxHashDefaultKey.kSecret) == null || kSecret.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &kSecret[0];
			}
			uint4 @uint;
			xxHash3.Hash128Internal((byte*)input, null, length, ptr, seed, out @uint);
			return @uint;
		}

		public unsafe static uint4 Hash128(void* input, void* destination, long length, ulong seed)
		{
			byte[] kSecret;
			byte* ptr;
			if ((kSecret = xxHashDefaultKey.kSecret) == null || kSecret.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &kSecret[0];
			}
			uint4 @uint;
			xxHash3.Hash128Internal((byte*)input, (byte*)destination, length, ptr, seed, out @uint);
			return @uint;
		}

		internal unsafe static ulong Hash64Internal(byte* input, byte* dest, long length, byte* secret, ulong seed)
		{
			if (dest != null && length < 240L)
			{
				UnsafeUtility.MemCpy((void*)dest, (void*)input, length);
			}
			if (length <= 16L)
			{
				return xxHash3.Hash64Len0To16(input, length, secret, seed);
			}
			if (length <= 128L)
			{
				return xxHash3.Hash64Len17To128(input, length, secret, seed);
			}
			if (length <= 240L)
			{
				return xxHash3.Hash64Len129To240(input, length, secret, seed);
			}
			if (seed != 0UL)
			{
				byte* ptr = (stackalloc byte[(UIntPtr)223] + 31L) & -32L;
				xxHash3.EncodeSecretKey(ptr, secret, seed);
				return xxHash3.Hash64Long(input, dest, length, ptr);
			}
			return xxHash3.Hash64Long(input, dest, length, secret);
		}

		internal unsafe static void Hash128Internal(byte* input, byte* dest, long length, byte* secret, ulong seed, out uint4 result)
		{
			if (dest != null && length < 240L)
			{
				UnsafeUtility.MemCpy((void*)dest, (void*)input, length);
			}
			if (length <= 16L)
			{
				xxHash3.Hash128Len0To16(input, length, secret, seed, out result);
				return;
			}
			if (length <= 128L)
			{
				xxHash3.Hash128Len17To128(input, length, secret, seed, out result);
				return;
			}
			if (length <= 240L)
			{
				xxHash3.Hash128Len129To240(input, length, secret, seed, out result);
				return;
			}
			if (seed != 0UL)
			{
				byte* ptr = (stackalloc byte[(UIntPtr)223] + 31L) & -32L;
				xxHash3.EncodeSecretKey(ptr, secret, seed);
				xxHash3.Hash128Long(input, dest, length, ptr, out result);
				return;
			}
			xxHash3.Hash128Long(input, dest, length, secret, out result);
		}

		private unsafe static ulong Hash64Len1To3(byte* input, long len, byte* secret, ulong seed)
		{
			ulong num = (ulong)(*input);
			byte b = input[len >> 1];
			byte b2 = input[len - 1L];
			ulong num2 = (num << 16) | (ulong)((ulong)b << 24) | (ulong)b2 | (ulong)((ulong)((uint)len) << 8);
			ulong num3 = (ulong)(xxHash3.Read32LE((void*)secret) ^ xxHash3.Read32LE((void*)(secret + 4))) + seed;
			return xxHash3.AvalancheH64(num2 ^ num3);
		}

		private unsafe static ulong Hash64Len4To8(byte* input, long length, byte* secret, ulong seed)
		{
			seed ^= (ulong)xxHash3.Swap32((uint)seed) << 32;
			uint num = xxHash3.Read32LE((void*)input);
			ulong num2 = (ulong)xxHash3.Read32LE((void*)(input + length - 4));
			ulong num3 = (xxHash3.Read64LE((void*)(secret + 8)) ^ xxHash3.Read64LE((void*)(secret + 16))) - seed;
			return xxHash3.rrmxmx((num2 + ((ulong)num << 32)) ^ num3, (ulong)length);
		}

		private unsafe static ulong Hash64Len9To16(byte* input, long length, byte* secret, ulong seed)
		{
			ulong num = (xxHash3.Read64LE((void*)(secret + 24)) ^ xxHash3.Read64LE((void*)(secret + 32))) + seed;
			ulong num2 = (xxHash3.Read64LE((void*)(secret + 40)) ^ xxHash3.Read64LE((void*)(secret + 48))) - seed;
			ulong num3 = xxHash3.Read64LE((void*)input) ^ num;
			ulong num4 = xxHash3.Read64LE((void*)(input + length - 8)) ^ num2;
			return xxHash3.Avalanche((ulong)(length + (long)xxHash3.Swap64(num3) + (long)num4 + (long)xxHash3.Mul128Fold64(num3, num4)));
		}

		private unsafe static ulong Hash64Len0To16(byte* input, long length, byte* secret, ulong seed)
		{
			if (length > 8L)
			{
				return xxHash3.Hash64Len9To16(input, length, secret, seed);
			}
			if (length >= 4L)
			{
				return xxHash3.Hash64Len4To8(input, length, secret, seed);
			}
			if (length > 0L)
			{
				return xxHash3.Hash64Len1To3(input, length, secret, seed);
			}
			return xxHash3.AvalancheH64(seed ^ (xxHash3.Read64LE((void*)(secret + 56)) ^ xxHash3.Read64LE((void*)(secret + 64))));
		}

		private unsafe static ulong Hash64Len17To128(byte* input, long length, byte* secret, ulong seed)
		{
			ulong num = (ulong)(length * -7046029288634856825L);
			if (length > 32L)
			{
				if (length > 64L)
				{
					if (length > 96L)
					{
						num += xxHash3.Mix16(input + 48, secret + 96, seed);
						num += xxHash3.Mix16(input + length - 64, secret + 112, seed);
					}
					num += xxHash3.Mix16(input + 32, secret + 64, seed);
					num += xxHash3.Mix16(input + length - 48, secret + 80, seed);
				}
				num += xxHash3.Mix16(input + 16, secret + 32, seed);
				num += xxHash3.Mix16(input + length - 32, secret + 48, seed);
			}
			num += xxHash3.Mix16(input, secret, seed);
			num += xxHash3.Mix16(input + length - 16, secret + 16, seed);
			return xxHash3.Avalanche(num);
		}

		private unsafe static ulong Hash64Len129To240(byte* input, long length, byte* secret, ulong seed)
		{
			ulong num = (ulong)(length * -7046029288634856825L);
			int num2 = (int)length / 16;
			for (int i = 0; i < 8; i++)
			{
				num += xxHash3.Mix16(input + 16 * i, secret + 16 * i, seed);
			}
			num = xxHash3.Avalanche(num);
			for (int j = 8; j < num2; j++)
			{
				num += xxHash3.Mix16(input + 16 * j, secret + 16 * (j - 8) + 3, seed);
			}
			num += xxHash3.Mix16(input + length - 16, secret + 136 - 17, seed);
			return xxHash3.Avalanche(num);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(xxHash3.Hash64Long_00000A6B$PostfixBurstDelegate))]
		private unsafe static ulong Hash64Long(byte* input, byte* dest, long length, byte* secret)
		{
			return xxHash3.Hash64Long_00000A6B$BurstDirectCall.Invoke(input, dest, length, secret);
		}

		private unsafe static void Hash128Len1To3(byte* input, long length, byte* secret, ulong seed, out uint4 result)
		{
			int num = (int)(*input);
			byte b = input[length >> 1];
			byte b2 = input[length - 1L];
			int num2 = (num << 16) + ((int)b << 24) + (int)b2 + (int)((int)((uint)length) << 8);
			uint num3 = xxHash3.RotL32(xxHash3.Swap32((uint)num2), 13);
			ulong num4 = (ulong)(xxHash3.Read32LE((void*)secret) ^ xxHash3.Read32LE((void*)(secret + 4))) + seed;
			ulong num5 = (ulong)(xxHash3.Read32LE((void*)(secret + 8)) ^ xxHash3.Read32LE((void*)(secret + 12))) - seed;
			ulong num6 = (ulong)num2 ^ num4;
			ulong num7 = (ulong)num3 ^ num5;
			result = xxHash3.ToUint4(xxHash3.AvalancheH64(num6), xxHash3.AvalancheH64(num7));
		}

		private unsafe static void Hash128Len4To8(byte* input, long len, byte* secret, ulong seed, out uint4 result)
		{
			seed ^= (ulong)xxHash3.Swap32((uint)seed) << 32;
			ulong num = (ulong)xxHash3.Read32LE((void*)input);
			uint num2 = xxHash3.Read32LE((void*)(input + len - 4));
			ulong num3 = num + ((ulong)num2 << 32);
			ulong num4 = (xxHash3.Read64LE((void*)(secret + 16)) ^ xxHash3.Read64LE((void*)(secret + 24))) + seed;
			ulong num6;
			ulong num5 = Common.umul128(num3 ^ num4, (ulong)(-7046029288634856825L + (len << 2)), out num6);
			num6 += num5 << 1;
			num5 ^= num6 >> 3;
			num5 = xxHash3.XorShift64(num5, 35);
			num5 *= 11507291218515648293UL;
			num5 = xxHash3.XorShift64(num5, 28);
			num6 = xxHash3.Avalanche(num6);
			result = xxHash3.ToUint4(num5, num6);
		}

		private unsafe static void Hash128Len9To16(byte* input, long len, byte* secret, ulong seed, out uint4 result)
		{
			ulong num = (xxHash3.Read64LE((void*)(secret + 32)) ^ xxHash3.Read64LE((void*)(secret + 40))) - seed;
			ulong num2 = (xxHash3.Read64LE((void*)(secret + 48)) ^ xxHash3.Read64LE((void*)(secret + 56))) + seed;
			ulong num3 = xxHash3.Read64LE((void*)input);
			ulong num4 = xxHash3.Read64LE((void*)(input + len - 8));
			ulong num6;
			ulong num5 = Common.umul128(num3 ^ num4 ^ num, 11400714785074694791UL, out num6) + (ulong)((ulong)(len - 1L) << 54);
			num4 ^= num2;
			num6 += num4 + xxHash3.Mul32To64((uint)num4, 2246822518U);
			ulong num8;
			ulong num7 = Common.umul128(num5 ^ xxHash3.Swap64(num6), 14029467366897019727UL, out num8);
			num8 += num6 * 14029467366897019727UL;
			result = xxHash3.ToUint4(xxHash3.Avalanche(num7), xxHash3.Avalanche(num8));
		}

		private unsafe static void Hash128Len0To16(byte* input, long length, byte* secret, ulong seed, out uint4 result)
		{
			if (length > 8L)
			{
				xxHash3.Hash128Len9To16(input, length, secret, seed, out result);
				return;
			}
			if (length >= 4L)
			{
				xxHash3.Hash128Len4To8(input, length, secret, seed, out result);
				return;
			}
			if (length > 0L)
			{
				xxHash3.Hash128Len1To3(input, length, secret, seed, out result);
				return;
			}
			ulong num = xxHash3.Read64LE((void*)(secret + 64)) ^ xxHash3.Read64LE((void*)(secret + 72));
			ulong num2 = xxHash3.Read64LE((void*)(secret + 80)) ^ xxHash3.Read64LE((void*)(secret + 88));
			ulong num3 = xxHash3.AvalancheH64(seed ^ num);
			ulong num4 = xxHash3.AvalancheH64(seed ^ num2);
			result = xxHash3.ToUint4(num3, num4);
		}

		private unsafe static void Hash128Len17To128(byte* input, long length, byte* secret, ulong seed, out uint4 result)
		{
			xxHash3.ulong2 @ulong = new xxHash3.ulong2((ulong)(length * -7046029288634856825L), 0UL);
			if (length > 32L)
			{
				if (length > 64L)
				{
					if (length > 96L)
					{
						@ulong = xxHash3.Mix32(@ulong, input + 48, input + length - 64, secret + 96, seed);
					}
					@ulong = xxHash3.Mix32(@ulong, input + 32, input + length - 48, secret + 64, seed);
				}
				@ulong = xxHash3.Mix32(@ulong, input + 16, input + length - 32, secret + 32, seed);
			}
			@ulong = xxHash3.Mix32(@ulong, input, input + length - 16, secret, seed);
			ulong num = @ulong.x + @ulong.y;
			ulong num2 = @ulong.x * 11400714785074694791UL + @ulong.y * 9650029242287828579UL + (ulong)((length - (long)seed) * -4417276706812531889L);
			result = xxHash3.ToUint4(xxHash3.Avalanche(num), 0UL - xxHash3.Avalanche(num2));
		}

		private unsafe static void Hash128Len129To240(byte* input, long length, byte* secret, ulong seed, out uint4 result)
		{
			xxHash3.ulong2 @ulong = new xxHash3.ulong2((ulong)(length * -7046029288634856825L), 0UL);
			long num = length / 32L;
			int i;
			for (i = 0; i < 4; i++)
			{
				@ulong = xxHash3.Mix32(@ulong, input + 32 * i, input + 32 * i + 16, secret + 32 * i, seed);
			}
			@ulong.x = xxHash3.Avalanche(@ulong.x);
			@ulong.y = xxHash3.Avalanche(@ulong.y);
			i = 4;
			while ((long)i < num)
			{
				@ulong = xxHash3.Mix32(@ulong, input + 32 * i, input + 32 * i + 16, secret + 3 + 32 * (i - 4), seed);
				i++;
			}
			@ulong = xxHash3.Mix32(@ulong, input + length - 16, input + length - 32, secret + 136 - 17 - 16, 0UL - seed);
			ulong num2 = @ulong.x + @ulong.y;
			ulong num3 = @ulong.x * 11400714785074694791UL + @ulong.y * 9650029242287828579UL + (ulong)((length - (long)seed) * -4417276706812531889L);
			result = xxHash3.ToUint4(xxHash3.Avalanche(num2), 0UL - xxHash3.Avalanche(num3));
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(xxHash3.Hash128Long_00000A72$PostfixBurstDelegate))]
		private unsafe static void Hash128Long(byte* input, byte* dest, long length, byte* secret, out uint4 result)
		{
			xxHash3.Hash128Long_00000A72$BurstDirectCall.Invoke(input, dest, length, secret, out result);
		}

		internal static uint2 ToUint2(ulong u)
		{
			return new uint2((uint)(u & (ulong)(-1)), (uint)(u >> 32));
		}

		internal static uint4 ToUint4(ulong ul0, ulong ul1)
		{
			return new uint4((uint)(ul0 & (ulong)(-1)), (uint)(ul0 >> 32), (uint)(ul1 & (ulong)(-1)), (uint)(ul1 >> 32));
		}

		internal unsafe static void EncodeSecretKey(byte* dst, byte* secret, ulong seed)
		{
			int num = 12;
			for (int i = 0; i < num; i++)
			{
				xxHash3.Write64LE((void*)(dst + 16 * i), xxHash3.Read64LE((void*)(secret + 16 * i)) + seed);
				xxHash3.Write64LE((void*)(dst + 16 * i + 8), xxHash3.Read64LE((void*)(secret + 16 * i + 8)) - seed);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong Read64LE(void* addr)
		{
			return (ulong)(*(long*)addr);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static uint Read32LE(void* addr)
		{
			return *(uint*)addr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Write64LE(void* addr, ulong value)
		{
			*(long*)addr = (long)value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Read32LE(void* addr, uint value)
		{
			*(int*)addr = (int)value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Mul32To64(uint x, uint y)
		{
			return (ulong)x * (ulong)y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Swap64(ulong x)
		{
			return ((x << 56) & 18374686479671623680UL) | ((x << 40) & 71776119061217280UL) | ((x << 24) & 280375465082880UL) | ((x << 8) & 1095216660480UL) | ((x >> 8) & (ulong)(-16777216)) | ((x >> 24) & 16711680UL) | ((x >> 40) & 65280UL) | ((x >> 56) & 255UL);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Swap32(uint x)
		{
			return ((x << 24) & 4278190080U) | ((x << 8) & 16711680U) | ((x >> 8) & 65280U) | ((x >> 24) & 255U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint RotL32(uint x, int r)
		{
			return (x << r) | (x >> 32 - r);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong RotL64(ulong x, int r)
		{
			return (x << r) | (x >> 64 - r);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong XorShift64(ulong v64, int shift)
		{
			return v64 ^ (v64 >> shift);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Mul128Fold64(ulong lhs, ulong rhs)
		{
			ulong num;
			return Common.umul128(lhs, rhs, out num) ^ num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong Mix16(byte* input, byte* secret, ulong seed)
		{
			ulong num = xxHash3.Read64LE((void*)input);
			ulong num2 = xxHash3.Read64LE((void*)(input + 8));
			return xxHash3.Mul128Fold64(num ^ (xxHash3.Read64LE((void*)secret) + seed), num2 ^ (xxHash3.Read64LE((void*)(secret + 8)) - seed));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static xxHash3.ulong2 Mix32(xxHash3.ulong2 acc, byte* input_1, byte* input_2, byte* secret, ulong seed)
		{
			ulong num = (acc.x + xxHash3.Mix16(input_1, secret, seed)) ^ (xxHash3.Read64LE((void*)input_2) + xxHash3.Read64LE((void*)(input_2 + 8)));
			ulong num2 = acc.y + xxHash3.Mix16(input_2, secret + 16, seed);
			num2 ^= xxHash3.Read64LE((void*)input_1) + xxHash3.Read64LE((void*)(input_1 + 8));
			return new xxHash3.ulong2(num, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Avalanche(ulong h64)
		{
			h64 = xxHash3.XorShift64(h64, 37);
			h64 *= 1609587791953885689UL;
			h64 = xxHash3.XorShift64(h64, 32);
			return h64;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong AvalancheH64(ulong h64)
		{
			h64 ^= h64 >> 33;
			h64 *= 14029467366897019727UL;
			h64 ^= h64 >> 29;
			h64 *= 1609587929392839161UL;
			h64 ^= h64 >> 32;
			return h64;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong rrmxmx(ulong h64, ulong length)
		{
			h64 ^= xxHash3.RotL64(h64, 49) ^ xxHash3.RotL64(h64, 24);
			h64 *= 11507291218515648293UL;
			h64 ^= (h64 >> 35) + length;
			h64 *= 11507291218515648293UL;
			return xxHash3.XorShift64(h64, 28);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong Mix2Acc(ulong acc0, ulong acc1, byte* secret)
		{
			return xxHash3.Mul128Fold64(acc0 ^ xxHash3.Read64LE((void*)secret), acc1 ^ xxHash3.Read64LE((void*)(secret + 8)));
		}

		internal unsafe static ulong MergeAcc(ulong* acc, byte* secret, ulong start)
		{
			return xxHash3.Avalanche(start + xxHash3.Mix2Acc(*acc, acc[1], secret) + xxHash3.Mix2Acc(acc[2], acc[3], secret + 16) + xxHash3.Mix2Acc(acc[4], acc[5], secret + 32) + xxHash3.Mix2Acc(acc[6], acc[7], secret + 48));
		}

		private unsafe static void DefaultHashLongInternalLoop(ulong* acc, byte* input, byte* dest, long length, byte* secret, int isHash64)
		{
			long num = (length - 1L) / 1024L;
			int num2 = 0;
			while ((long)num2 < num)
			{
				xxHash3.DefaultAccumulate(acc, input + num2 * 1024, (dest == null) ? null : (dest + num2 * 1024), secret, 16L, isHash64);
				xxHash3.DefaultScrambleAcc(acc, secret + 192 - 64);
				num2++;
			}
			long num3 = (length - 1L - 1024L * num) / 64L;
			xxHash3.DefaultAccumulate(acc, input + num * 1024L, (dest == null) ? null : (dest + num * 1024L), secret, num3, isHash64);
			byte* ptr = input + length - 64;
			xxHash3.DefaultAccumulate512(acc, ptr, null, secret + 192 - 64 - 7, isHash64);
			if (dest != null)
			{
				long num4 = length % 64L;
				if (num4 != 0L)
				{
					UnsafeUtility.MemCpy((void*)(dest + length - num4), (void*)(input + length - num4), num4);
				}
			}
		}

		internal unsafe static void DefaultAccumulate(ulong* acc, byte* input, byte* dest, byte* secret, long nbStripes, int isHash64)
		{
			int num = 0;
			while ((long)num < nbStripes)
			{
				xxHash3.DefaultAccumulate512(acc, input + num * 64, (dest == null) ? null : (dest + num * 64), secret + num * 8, isHash64);
				num++;
			}
		}

		internal unsafe static void DefaultAccumulate512(ulong* acc, byte* input, byte* dest, byte* secret, int isHash64)
		{
			int num = 8;
			for (int i = 0; i < num; i++)
			{
				ulong num2 = xxHash3.Read64LE((void*)(input + 8 * i));
				ulong num3 = num2 ^ xxHash3.Read64LE((void*)(secret + i * 8));
				if (dest != null)
				{
					xxHash3.Write64LE((void*)(dest + 8 * i), num2);
				}
				acc[i ^ 1] += num2;
				acc[i] += xxHash3.Mul32To64((uint)(num3 & (ulong)(-1)), (uint)(num3 >> 32));
			}
		}

		internal unsafe static void DefaultScrambleAcc(ulong* acc, byte* secret)
		{
			for (int i = 0; i < 8; i++)
			{
				ulong num = xxHash3.Read64LE((void*)(secret + 8 * i));
				ulong num2 = acc[i];
				num2 = xxHash3.XorShift64(num2, 47);
				num2 ^= num;
				num2 *= (ulong)(-1640531535);
				acc[i] = num2;
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static ulong Hash64Long$BurstManaged(byte* input, byte* dest, long length, byte* secret)
		{
			ulong* ptr = (stackalloc ulong[(UIntPtr)95] + 31L / 8L) & -32L;
			*ptr = (ulong)(-1028477379);
			ptr[1] = 11400714785074694791UL;
			ptr[2] = 14029467366897019727UL;
			ptr[3] = 1609587929392839161UL;
			ptr[4] = 9650029242287828579UL;
			ptr[5] = (ulong)(-2048144777);
			ptr[6] = 2870177450012600261UL;
			ptr[7] = (ulong)(-1640531535);
			if (X86.Avx2.IsAvx2Supported)
			{
				xxHash3.Avx2HashLongInternalLoop(ptr, input, dest, length, secret, 1);
			}
			else
			{
				xxHash3.DefaultHashLongInternalLoop(ptr, input, dest, length, secret, 1);
			}
			return xxHash3.MergeAcc(ptr, secret + 11, (ulong)(length * -7046029288634856825L));
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void Hash128Long$BurstManaged(byte* input, byte* dest, long length, byte* secret, out uint4 result)
		{
			ulong* ptr = (stackalloc ulong[(UIntPtr)95] + 31L / 8L) & -32L;
			*ptr = (ulong)(-1028477379);
			ptr[1] = 11400714785074694791UL;
			ptr[2] = 14029467366897019727UL;
			ptr[3] = 1609587929392839161UL;
			ptr[4] = 9650029242287828579UL;
			ptr[5] = (ulong)(-2048144777);
			ptr[6] = 2870177450012600261UL;
			ptr[7] = (ulong)(-1640531535);
			if (X86.Avx2.IsAvx2Supported)
			{
				xxHash3.Avx2HashLongInternalLoop(ptr, input, dest, length, secret, 0);
			}
			else
			{
				xxHash3.DefaultHashLongInternalLoop(ptr, input, dest, length, secret, 0);
			}
			ulong num = xxHash3.MergeAcc(ptr, secret + 11, (ulong)(length * -7046029288634856825L));
			ulong num2 = xxHash3.MergeAcc(ptr, secret + 192 - 64 - 11, (ulong)(~(ulong)(length * -4417276706812531889L)));
			result = xxHash3.ToUint4(num, num2);
		}

		private const int STRIPE_LEN = 64;

		private const int ACC_NB = 8;

		private const int SECRET_CONSUME_RATE = 8;

		private const int SECRET_KEY_SIZE = 192;

		private const int SECRET_KEY_MIN_SIZE = 136;

		private const int SECRET_LASTACC_START = 7;

		private const int NB_ROUNDS = 16;

		private const int BLOCK_LEN = 1024;

		private const uint PRIME32_1 = 2654435761U;

		private const uint PRIME32_2 = 2246822519U;

		private const uint PRIME32_3 = 3266489917U;

		private const uint PRIME32_5 = 374761393U;

		private const ulong PRIME64_1 = 11400714785074694791UL;

		private const ulong PRIME64_2 = 14029467366897019727UL;

		private const ulong PRIME64_3 = 1609587929392839161UL;

		private const ulong PRIME64_4 = 9650029242287828579UL;

		private const ulong PRIME64_5 = 2870177450012600261UL;

		private const int MIDSIZE_MAX = 240;

		private const int MIDSIZE_STARTOFFSET = 3;

		private const int MIDSIZE_LASTOFFSET = 17;

		private const int SECRET_MERGEACCS_START = 11;

		[GenerateTestsForBurstCompatibility]
		public struct StreamingState
		{
			public StreamingState(bool isHash64, ulong seed = 0UL)
			{
				this.State = default(xxHash3.StreamingState.StreamingStateData);
				this.Reset(isHash64, seed);
			}

			public unsafe void Reset(bool isHash64, ulong seed = 0UL)
			{
				int num = UnsafeUtility.SizeOf<xxHash3.StreamingState.StreamingStateData>();
				UnsafeUtility.MemClear(UnsafeUtility.AddressOf<xxHash3.StreamingState.StreamingStateData>(ref this.State), (long)num);
				this.State.IsHash64 = (isHash64 ? 1 : 0);
				ulong* acc = this.Acc;
				*acc = (ulong)(-1028477379);
				acc[1] = 11400714785074694791UL;
				acc[2] = 14029467366897019727UL;
				acc[3] = 1609587929392839161UL;
				acc[4] = 9650029242287828579UL;
				acc[5] = (ulong)(-2048144777);
				acc[6] = 2870177450012600261UL;
				acc[7] = (ulong)(-1640531535);
				this.State.Seed = seed;
				byte[] array;
				byte* ptr;
				if ((array = xxHashDefaultKey.kSecret) == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				if (seed != 0UL)
				{
					xxHash3.EncodeSecretKey(this.SecretKey, ptr, seed);
				}
				else
				{
					UnsafeUtility.MemCpy((void*)this.SecretKey, (void*)ptr, 192L);
				}
				array = null;
			}

			public unsafe void Update(void* input, int length)
			{
				byte* ptr = (byte*)input;
				byte* ptr2 = ptr + length;
				int isHash = this.State.IsHash64;
				byte* secretKey = this.SecretKey;
				this.State.TotalLength = this.State.TotalLength + (long)length;
				if (this.State.BufferedSize + length <= xxHash3.StreamingState.INTERNAL_BUFFER_SIZE)
				{
					UnsafeUtility.MemCpy((void*)(this.Buffer + this.State.BufferedSize), (void*)ptr, (long)length);
					this.State.BufferedSize = this.State.BufferedSize + length;
					return;
				}
				if (this.State.BufferedSize != 0)
				{
					int num = xxHash3.StreamingState.INTERNAL_BUFFER_SIZE - this.State.BufferedSize;
					UnsafeUtility.MemCpy((void*)(this.Buffer + this.State.BufferedSize), (void*)ptr, (long)num);
					ptr += num;
					this.ConsumeStripes(this.Acc, ref this.State.NbStripesSoFar, this.Buffer, (long)xxHash3.StreamingState.INTERNAL_BUFFER_STRIPES, secretKey, isHash);
					this.State.BufferedSize = 0;
				}
				if (ptr + xxHash3.StreamingState.INTERNAL_BUFFER_SIZE < ptr2)
				{
					byte* ptr3 = ptr2 - xxHash3.StreamingState.INTERNAL_BUFFER_SIZE;
					do
					{
						this.ConsumeStripes(this.Acc, ref this.State.NbStripesSoFar, ptr, (long)xxHash3.StreamingState.INTERNAL_BUFFER_STRIPES, secretKey, isHash);
						ptr += xxHash3.StreamingState.INTERNAL_BUFFER_SIZE;
					}
					while (ptr < ptr3);
					UnsafeUtility.MemCpy((void*)(this.Buffer + xxHash3.StreamingState.INTERNAL_BUFFER_SIZE - 64), (void*)(ptr - 64), 64L);
				}
				if (ptr < ptr2)
				{
					long num2 = (long)(ptr2 - ptr);
					UnsafeUtility.MemCpy((void*)this.Buffer, (void*)ptr, num2);
					this.State.BufferedSize = (int)num2;
				}
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public void Update<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(in T input) where T : struct, ValueType
			{
				this.Update(UnsafeUtilityExtensions.AddressOf<T>(in input), UnsafeUtility.SizeOf<T>());
			}

			public unsafe uint4 DigestHash128()
			{
				byte* secretKey = this.SecretKey;
				uint4 @uint;
				if (this.State.TotalLength > 240L)
				{
					ulong* ptr = stackalloc ulong[(UIntPtr)64];
					this.DigestLong(ptr, secretKey, 0);
					ulong num = xxHash3.MergeAcc(ptr, secretKey + 11, (ulong)(this.State.TotalLength * -7046029288634856825L));
					ulong num2 = xxHash3.MergeAcc(ptr, secretKey + xxHash3.StreamingState.SECRET_LIMIT - 11, (ulong)(~(ulong)(this.State.TotalLength * -4417276706812531889L)));
					@uint = xxHash3.ToUint4(num, num2);
				}
				else
				{
					@uint = xxHash3.Hash128((void*)this.Buffer, this.State.TotalLength, this.State.Seed);
				}
				this.Reset(this.State.IsHash64 == 1, this.State.Seed);
				return @uint;
			}

			public unsafe uint2 DigestHash64()
			{
				byte* secretKey = this.SecretKey;
				uint2 @uint;
				if (this.State.TotalLength > 240L)
				{
					ulong* ptr = stackalloc ulong[(UIntPtr)64];
					this.DigestLong(ptr, secretKey, 1);
					@uint = xxHash3.ToUint2(xxHash3.MergeAcc(ptr, secretKey + 11, (ulong)(this.State.TotalLength * -7046029288634856825L)));
				}
				else
				{
					@uint = xxHash3.Hash64((void*)this.Buffer, this.State.TotalLength, this.State.Seed);
				}
				this.Reset(this.State.IsHash64 == 1, this.State.Seed);
				return @uint;
			}

			private unsafe ulong* Acc
			{
				[DebuggerStepThrough]
				get
				{
					return (ulong*)UnsafeUtility.AddressOf<ulong>(ref this.State.Acc);
				}
			}

			private unsafe byte* Buffer
			{
				[DebuggerStepThrough]
				get
				{
					return (byte*)UnsafeUtility.AddressOf<byte>(ref this.State.Buffer);
				}
			}

			private unsafe byte* SecretKey
			{
				[DebuggerStepThrough]
				get
				{
					return (byte*)UnsafeUtility.AddressOf<byte>(ref this.State.SecretKey);
				}
			}

			private unsafe void DigestLong(ulong* acc, byte* secret, int isHash64)
			{
				UnsafeUtility.MemCpy((void*)acc, (void*)this.Acc, 64L);
				if (this.State.BufferedSize >= 64)
				{
					int num = (this.State.BufferedSize - 1) / 64;
					this.ConsumeStripes(acc, ref this.State.NbStripesSoFar, this.Buffer, (long)num, secret, isHash64);
					if (X86.Avx2.IsAvx2Supported)
					{
						xxHash3.Avx2Accumulate512(acc, this.Buffer + this.State.BufferedSize - 64, null, secret + xxHash3.StreamingState.SECRET_LIMIT - 7);
						return;
					}
					xxHash3.DefaultAccumulate512(acc, this.Buffer + this.State.BufferedSize - 64, null, secret + xxHash3.StreamingState.SECRET_LIMIT - 7, isHash64);
					return;
				}
				else
				{
					byte* ptr = stackalloc byte[(UIntPtr)64];
					int num2 = 64 - this.State.BufferedSize;
					UnsafeUtility.MemCpy((void*)ptr, (void*)(this.Buffer + xxHash3.StreamingState.INTERNAL_BUFFER_SIZE - num2), (long)num2);
					UnsafeUtility.MemCpy((void*)(ptr + num2), (void*)this.Buffer, (long)this.State.BufferedSize);
					if (X86.Avx2.IsAvx2Supported)
					{
						xxHash3.Avx2Accumulate512(acc, ptr, null, secret + xxHash3.StreamingState.SECRET_LIMIT - 7);
						return;
					}
					xxHash3.DefaultAccumulate512(acc, ptr, null, secret + xxHash3.StreamingState.SECRET_LIMIT - 7, isHash64);
					return;
				}
			}

			private unsafe void ConsumeStripes(ulong* acc, ref int nbStripesSoFar, byte* input, long totalStripes, byte* secret, int isHash64)
			{
				if ((long)(xxHash3.StreamingState.NB_STRIPES_PER_BLOCK - nbStripesSoFar) <= totalStripes)
				{
					int num = xxHash3.StreamingState.NB_STRIPES_PER_BLOCK - nbStripesSoFar;
					if (X86.Avx2.IsAvx2Supported)
					{
						xxHash3.Avx2Accumulate(acc, input, null, secret + nbStripesSoFar * 8, (long)num, isHash64);
						xxHash3.Avx2ScrambleAcc(acc, secret + xxHash3.StreamingState.SECRET_LIMIT);
						xxHash3.Avx2Accumulate(acc, input + num * 64, null, secret, totalStripes - (long)num, isHash64);
					}
					else
					{
						xxHash3.DefaultAccumulate(acc, input, null, secret + nbStripesSoFar * 8, (long)num, isHash64);
						xxHash3.DefaultScrambleAcc(acc, secret + xxHash3.StreamingState.SECRET_LIMIT);
						xxHash3.DefaultAccumulate(acc, input + num * 64, null, secret, totalStripes - (long)num, isHash64);
					}
					nbStripesSoFar = (int)totalStripes - num;
					return;
				}
				if (X86.Avx2.IsAvx2Supported)
				{
					xxHash3.Avx2Accumulate(acc, input, null, secret + nbStripesSoFar * 8, totalStripes, isHash64);
				}
				else
				{
					xxHash3.DefaultAccumulate(acc, input, null, secret + nbStripesSoFar * 8, totalStripes, isHash64);
				}
				nbStripesSoFar += (int)totalStripes;
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckKeySize(int isHash64)
			{
				if (this.State.IsHash64 != isHash64)
				{
					string text = ((this.State.IsHash64 != 0) ? "64" : "128");
					throw new InvalidOperationException("The streaming state was create for " + text + " bits hash key, the calling method doesn't support this key size, please use the appropriate API");
				}
			}

			private static readonly int SECRET_LIMIT = 128;

			private static readonly int NB_STRIPES_PER_BLOCK = xxHash3.StreamingState.SECRET_LIMIT / 8;

			private static readonly int INTERNAL_BUFFER_SIZE = 256;

			private static readonly int INTERNAL_BUFFER_STRIPES = xxHash3.StreamingState.INTERNAL_BUFFER_SIZE / 64;

			private xxHash3.StreamingState.StreamingStateData State;

			[StructLayout(LayoutKind.Explicit)]
			private struct StreamingStateData
			{
				[FieldOffset(0)]
				public ulong Acc;

				[FieldOffset(64)]
				public byte Buffer;

				[FieldOffset(320)]
				public int IsHash64;

				[FieldOffset(324)]
				public int BufferedSize;

				[FieldOffset(328)]
				public int NbStripesSoFar;

				[FieldOffset(336)]
				public long TotalLength;

				[FieldOffset(344)]
				public ulong Seed;

				[FieldOffset(352)]
				public byte SecretKey;

				[FieldOffset(540)]
				public byte _PadEnd;
			}
		}

		private struct ulong2
		{
			public ulong2(ulong x, ulong y)
			{
				this.x = x;
				this.y = y;
			}

			public ulong x;

			public ulong y;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate ulong Hash64Long_00000A6B$PostfixBurstDelegate(byte* input, byte* dest, long length, byte* secret);

		internal static class Hash64Long_00000A6B$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (xxHash3.Hash64Long_00000A6B$BurstDirectCall.Pointer == 0)
				{
					xxHash3.Hash64Long_00000A6B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<xxHash3.Hash64Long_00000A6B$PostfixBurstDelegate>(new xxHash3.Hash64Long_00000A6B$PostfixBurstDelegate(xxHash3.Hash64Long)).Value;
				}
				A_0 = xxHash3.Hash64Long_00000A6B$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				xxHash3.Hash64Long_00000A6B$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			public unsafe static ulong Invoke(byte* input, byte* dest, long length, byte* secret)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = xxHash3.Hash64Long_00000A6B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.UInt64(System.Byte*,System.Byte*,System.Int64,System.Byte*), input, dest, length, secret, functionPointer);
					}
				}
				return xxHash3.Hash64Long$BurstManaged(input, dest, length, secret);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate void Hash128Long_00000A72$PostfixBurstDelegate(byte* input, byte* dest, long length, byte* secret, out uint4 result);

		internal static class Hash128Long_00000A72$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (xxHash3.Hash128Long_00000A72$BurstDirectCall.Pointer == 0)
				{
					xxHash3.Hash128Long_00000A72$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<xxHash3.Hash128Long_00000A72$PostfixBurstDelegate>(new xxHash3.Hash128Long_00000A72$PostfixBurstDelegate(xxHash3.Hash128Long)).Value;
				}
				A_0 = xxHash3.Hash128Long_00000A72$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				xxHash3.Hash128Long_00000A72$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			public unsafe static void Invoke(byte* input, byte* dest, long length, byte* secret, out uint4 result)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = xxHash3.Hash128Long_00000A72$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Byte*,System.Byte*,System.Int64,System.Byte*,Unity.Mathematics.uint4&), input, dest, length, secret, ref result, functionPointer);
						return;
					}
				}
				xxHash3.Hash128Long$BurstManaged(input, dest, length, secret, out result);
			}

			private static IntPtr Pointer;
		}
	}
}
