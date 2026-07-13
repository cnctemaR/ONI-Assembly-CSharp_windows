using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.IO.Hashing
{
	internal static class XxHashShared
	{
		public unsafe static ReadOnlySpan<byte> DefaultSecret
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.2CF2F88BF9B71283059B6DF53E5BCDE20ADBFD9E8D6CE2C1AB106262BB283BED), 192);
			}
		}

		public unsafe static void Initialize(ref XxHashShared.State state, ulong seed)
		{
			state.Seed = seed;
			fixed (byte* ptr = &state.Secret.FixedElementField)
			{
				byte* ptr2 = ptr;
				if (seed == 0UL)
				{
					XxHashShared.DefaultSecret.CopyTo(new Span<byte>((void*)ptr2, 192));
				}
				else
				{
					XxHashShared.DeriveSecretFromSeed(ptr2, seed);
				}
			}
			XxHashShared.Reset(ref state);
		}

		public unsafe static void Reset(ref XxHashShared.State state)
		{
			state.BufferedCount = 0U;
			state.StripesProcessedInCurrentBlock = 0UL;
			state.TotalLength = 0UL;
			fixed (ulong* ptr = &state.Accumulators.FixedElementField)
			{
				XxHashShared.InitializeAccumulators(ptr);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Rrmxmx(ulong hash, uint length)
		{
			hash ^= BitOperations.RotateLeft(hash, 49) ^ BitOperations.RotateLeft(hash, 24);
			hash *= 11507291218515648293UL;
			hash ^= (hash >> 35) + (ulong)length;
			hash *= 11507291218515648293UL;
			return XxHashShared.XorShift(hash, 28);
		}

		public unsafe static void HashInternalLoop(ulong* accumulators, byte* source, uint length, byte* secret)
		{
			int num = (int)((length - 1U) / 1024U);
			XxHashShared.Accumulate(accumulators, source, secret, 16, true, num);
			int num2 = 1024 * num;
			int num3 = (int)(((ulong)(length - 1U) - (ulong)((long)num2)) / 64UL);
			XxHashShared.Accumulate(accumulators, source + num2, secret, num3, false, 1);
			XxHashShared.Accumulate512(accumulators, source + length - 64, secret + 121);
		}

		public unsafe static void ConsumeStripes(ulong* accumulators, ref ulong stripesSoFar, ulong stripesPerBlock, byte* source, ulong stripes, byte* secret)
		{
			ulong num = stripesPerBlock - stripesSoFar;
			if (num <= stripes)
			{
				ulong num2 = stripes - num;
				XxHashShared.Accumulate(accumulators, source, secret + (int)stripesSoFar * 8, (int)num, false, 1);
				XxHashShared.ScrambleAccumulators(accumulators, secret + 128);
				XxHashShared.Accumulate(accumulators, source + (int)num * 64, secret, (int)num2, false, 1);
				stripesSoFar = num2;
				return;
			}
			XxHashShared.Accumulate(accumulators, source, secret + (int)stripesSoFar * 8, (int)stripes, false, 1);
			stripesSoFar += stripes;
		}

		public unsafe static void Append(ref XxHashShared.State state, ReadOnlySpan<byte> source)
		{
			state.TotalLength += (ulong)source.Length;
			fixed (byte* ptr = &state.Buffer.FixedElementField)
			{
				byte* ptr2 = ptr;
				if ((long)source.Length <= (long)((ulong)(256U - state.BufferedCount)))
				{
					source.CopyTo(new Span<byte>((void*)(ptr2 + state.BufferedCount), source.Length));
					state.BufferedCount += (uint)source.Length;
					return;
				}
				fixed (byte* ptr3 = &state.Secret.FixedElementField)
				{
					byte* ptr4 = ptr3;
					fixed (ulong* ptr5 = &state.Accumulators.FixedElementField)
					{
						ulong* ptr6 = ptr5;
						fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
						{
							byte* ptr7 = reference;
							int num = 0;
							if (state.BufferedCount != 0U)
							{
								int num2 = (int)(256U - state.BufferedCount);
								source.Slice(0, num2).CopyTo(new Span<byte>((void*)(ptr2 + state.BufferedCount), num2));
								num = num2;
								XxHashShared.ConsumeStripes(ptr6, ref state.StripesProcessedInCurrentBlock, 16UL, ptr2, 4UL, ptr4);
								state.BufferedCount = 0U;
							}
							if (source.Length - num > 1024)
							{
								ulong num3 = (ulong)((long)(source.Length - num - 1) / 64L);
								ulong num4 = 16UL - state.StripesProcessedInCurrentBlock;
								XxHashShared.Accumulate(ptr6, ptr7 + num, ptr4 + (int)state.StripesProcessedInCurrentBlock * 8, (int)num4, false, 1);
								XxHashShared.ScrambleAccumulators(ptr6, ptr4 + 128);
								state.StripesProcessedInCurrentBlock = 0UL;
								num += (int)num4 * 64;
								for (num3 -= num4; num3 >= 16UL; num3 -= 16UL)
								{
									XxHashShared.Accumulate(ptr6, ptr7 + num, ptr4, 16, false, 1);
									XxHashShared.ScrambleAccumulators(ptr6, ptr4 + 128);
									num += 1024;
								}
								XxHashShared.Accumulate(ptr6, ptr7 + num, ptr4, (int)num3, false, 1);
								num += (int)num3 * 64;
								state.StripesProcessedInCurrentBlock = num3;
								source.Slice(num - 64, 64).CopyTo(new Span<byte>((void*)(ptr2 + 256 - 64), 64));
							}
							else if (source.Length - num > 256)
							{
								do
								{
									XxHashShared.ConsumeStripes(ptr6, ref state.StripesProcessedInCurrentBlock, 16UL, ptr7 + num, 4UL, ptr4);
									num += 256;
								}
								while (source.Length - num > 256);
								source.Slice(num - 64, 64).CopyTo(new Span<byte>((void*)(ptr2 + 256 - 64), 64));
							}
							Span<byte> span = new Span<byte>((void*)ptr2, source.Length - num);
							source.Slice(num).CopyTo(span);
							state.BufferedCount = (uint)span.Length;
						}
					}
				}
			}
		}

		public unsafe static void CopyAccumulators(ref XxHashShared.State state, ulong* accumulators)
		{
			fixed (ulong* ptr = &state.Accumulators.FixedElementField)
			{
				ulong* ptr2 = ptr;
				for (int i = 0; i < 8; i++)
				{
					accumulators[i] = ptr2[i];
				}
			}
		}

		public unsafe static void DigestLong(ref XxHashShared.State state, ulong* accumulators, byte* secret)
		{
			fixed (byte* ptr = &state.Buffer.FixedElementField)
			{
				byte* ptr2 = ptr;
				byte* ptr3;
				if (state.BufferedCount >= 64U)
				{
					uint num = (state.BufferedCount - 1U) / 64U;
					ulong stripesProcessedInCurrentBlock = state.StripesProcessedInCurrentBlock;
					XxHashShared.ConsumeStripes(accumulators, ref stripesProcessedInCurrentBlock, 16UL, ptr2, (ulong)num, secret);
					ptr3 = ptr2 + state.BufferedCount - 64;
				}
				else
				{
					byte* ptr4 = stackalloc byte[(UIntPtr)64];
					int num2 = (int)(64U - state.BufferedCount);
					new ReadOnlySpan<byte>((void*)(ptr2 + 256 - num2), num2).CopyTo(new Span<byte>((void*)ptr4, 64));
					new ReadOnlySpan<byte>((void*)ptr2, (int)state.BufferedCount).CopyTo(new Span<byte>((void*)(ptr4 + num2), (int)state.BufferedCount));
					ptr3 = ptr4;
				}
				XxHashShared.Accumulate512(accumulators, ptr3, secret + 121);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitializeAccumulators(ulong* accumulators)
		{
			*accumulators = (ulong)(-1028477379);
			accumulators[1] = 11400714785074694791UL;
			accumulators[2] = 14029467366897019727UL;
			accumulators[3] = 1609587929392839161UL;
			accumulators[4] = 9650029242287828579UL;
			accumulators[5] = (ulong)(-2048144777);
			accumulators[6] = 2870177450012600261UL;
			accumulators[7] = (ulong)(-1640531535);
		}

		public unsafe static ulong MergeAccumulators(ulong* accumulators, byte* secret, ulong start)
		{
			return XxHashShared.Avalanche(start + XxHashShared.Multiply64To128ThenFold(*accumulators ^ XxHashShared.ReadUInt64LE(secret), accumulators[1] ^ XxHashShared.ReadUInt64LE(secret + 8)) + XxHashShared.Multiply64To128ThenFold(accumulators[2] ^ XxHashShared.ReadUInt64LE(secret + 16), accumulators[3] ^ XxHashShared.ReadUInt64LE(secret + 24)) + XxHashShared.Multiply64To128ThenFold(accumulators[4] ^ XxHashShared.ReadUInt64LE(secret + 32), accumulators[5] ^ XxHashShared.ReadUInt64LE(secret + 40)) + XxHashShared.Multiply64To128ThenFold(accumulators[6] ^ XxHashShared.ReadUInt64LE(secret + 48), accumulators[7] ^ XxHashShared.ReadUInt64LE(secret + 56)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ulong Mix16Bytes(byte* source, ulong secretLow, ulong secretHigh, ulong seed)
		{
			return XxHashShared.Multiply64To128ThenFold(XxHashShared.ReadUInt64LE(source) ^ (secretLow + seed), XxHashShared.ReadUInt64LE(source + 8) ^ (secretHigh - seed));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Multiply32To64(uint v1, uint v2)
		{
			return (ulong)v1 * (ulong)v2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Avalanche(ulong hash)
		{
			hash = XxHashShared.XorShift(hash, 37);
			hash *= 1609587791953885689UL;
			hash = XxHashShared.XorShift(hash, 32);
			return hash;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Multiply64To128(ulong left, ulong right, out ulong lower)
		{
			ulong num = XxHashShared.Multiply32To64((uint)left, (uint)right);
			ulong num2 = XxHashShared.Multiply32To64((uint)(left >> 32), (uint)right);
			ulong num3 = XxHashShared.Multiply32To64((uint)left, (uint)(right >> 32));
			ulong num4 = XxHashShared.Multiply32To64((uint)(left >> 32), (uint)(right >> 32));
			ulong num5 = (num >> 32) + (num2 & (ulong)(-1)) + num3;
			ulong num6 = (num2 >> 32) + (num5 >> 32) + num4;
			lower = (num5 << 32) | (num & (ulong)(-1));
			return num6;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Multiply64To128ThenFold(ulong left, ulong right)
		{
			ulong num2;
			ulong num = XxHashShared.Multiply64To128(left, right, out num2);
			return num2 ^ num;
		}

		public unsafe static void DeriveSecretFromSeed(byte* destinationSecret, ulong seed)
		{
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(XxHashShared.DefaultSecret))
			{
				byte* ptr = reference;
				for (int i = 0; i < 192; i += 16)
				{
					XxHashShared.WriteUInt64LE(destinationSecret + i, XxHashShared.ReadUInt64LE(ptr + i) + seed);
					XxHashShared.WriteUInt64LE(destinationSecret + i + 8, XxHashShared.ReadUInt64LE(ptr + i + 8) - seed);
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe static void Accumulate(ulong* accumulators, byte* source, byte* secret, int stripesToProcess, bool scramble = false, int blockCount = 1)
		{
			byte* ptr = secret + 128;
			for (int i = 0; i < blockCount; i++)
			{
				for (int j = 0; j < stripesToProcess; j++)
				{
					XxHashShared.Accumulate512Inlined(accumulators, source, secret + j * 8);
					source += 64;
				}
				if (scramble)
				{
					XxHashShared.ScrambleAccumulators(accumulators, ptr);
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static void Accumulate512(ulong* accumulators, byte* source, byte* secret)
		{
			XxHashShared.Accumulate512Inlined(accumulators, source, secret);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Accumulate512Inlined(ulong* accumulators, byte* source, byte* secret)
		{
			for (int i = 0; i < 8; i++)
			{
				ulong num = XxHashShared.ReadUInt64LE(source + 8 * i);
				ulong num2 = num ^ XxHashShared.ReadUInt64LE(secret + i * 8);
				accumulators[i ^ 1] += num;
				accumulators[i] += XxHashShared.Multiply32To64((uint)num2, (uint)(num2 >> 32));
			}
		}

		private unsafe static void ScrambleAccumulators(ulong* accumulators, byte* secret)
		{
			for (int i = 0; i < 8; i++)
			{
				ulong num = XxHashShared.XorShift(*accumulators, 47) ^ XxHashShared.ReadUInt64LE(secret);
				*accumulators = num * (ulong)(-1640531535);
				accumulators++;
				secret += 8;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong XorShift(ulong value, int shift)
		{
			return value ^ (value >> shift);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint ReadUInt32LE(byte* data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<uint>((void*)data));
			}
			return Unsafe.ReadUnaligned<uint>((void*)data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ulong ReadUInt64LE(byte* data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>((void*)data));
			}
			return Unsafe.ReadUnaligned<ulong>((void*)data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void WriteUInt64LE(byte* data, ulong value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = BinaryPrimitives.ReverseEndianness(value);
			}
			Unsafe.WriteUnaligned<ulong>((void*)data, value);
		}

		public const int StripeLengthBytes = 64;

		public const int SecretLengthBytes = 192;

		public const int SecretSizeMin = 136;

		public const int SecretLastAccStartBytes = 7;

		public const int SecretConsumeRateBytes = 8;

		public const int SecretMergeAccsStartBytes = 11;

		public const int NumStripesPerBlock = 16;

		public const int AccumulatorCount = 8;

		public const int MidSizeMaxBytes = 240;

		public const int InternalBufferStripes = 4;

		public const int InternalBufferLengthBytes = 256;

		public const ulong DefaultSecretUInt64_0 = 13712233961653862072UL;

		public const ulong DefaultSecretUInt64_1 = 2066345149520216444UL;

		public const ulong DefaultSecretUInt64_2 = 15823274712020931806UL;

		public const ulong DefaultSecretUInt64_3 = 2262974939099578482UL;

		public const ulong DefaultSecretUInt64_4 = 8711581037947681227UL;

		public const ulong DefaultSecretUInt64_5 = 2410270004345854594UL;

		public const ulong DefaultSecretUInt64_6 = 10242386182634080440UL;

		public const ulong DefaultSecretUInt64_7 = 5487137525590930912UL;

		public const ulong DefaultSecretUInt64_8 = 14627906620379768892UL;

		public const ulong DefaultSecretUInt64_9 = 11758427054878871688UL;

		public const ulong DefaultSecretUInt64_10 = 5690594596133299313UL;

		public const ulong DefaultSecretUInt64_11 = 15613098826807580984UL;

		public const ulong DefaultSecretUInt64_12 = 4554437623014685352UL;

		public const ulong DefaultSecretUInt64_13 = 2111919702937427193UL;

		public const ulong DefaultSecretUInt64_14 = 3556072174620004746UL;

		public const ulong DefaultSecretUInt64_15 = 7238261902898274248UL;

		public const ulong DefaultSecret3UInt64_0 = 9295848262624092985UL;

		public const ulong DefaultSecret3UInt64_1 = 7914194659941938988UL;

		public const ulong DefaultSecret3UInt64_2 = 11835586108195898345UL;

		public const ulong DefaultSecret3UInt64_3 = 16607528436649670564UL;

		public const ulong DefaultSecret3UInt64_4 = 15013455763555273806UL;

		public const ulong DefaultSecret3UInt64_5 = 5046485836271438973UL;

		public const ulong DefaultSecret3UInt64_6 = 10391458616325699444UL;

		public const ulong DefaultSecret3UInt64_7 = 5920048007935066598UL;

		public const ulong DefaultSecret3UInt64_8 = 7336514198459093435UL;

		public const ulong DefaultSecret3UInt64_9 = 5216419214072683403UL;

		public const ulong DefaultSecret3UInt64_10 = 17228863761319568023UL;

		public const ulong DefaultSecret3UInt64_11 = 8573350489219836230UL;

		public const ulong DefaultSecret3UInt64_12 = 13536968629829821247UL;

		public const ulong DefaultSecret3UInt64_13 = 16163852396094277575UL;

		public const ulong Prime64_1 = 11400714785074694791UL;

		public const ulong Prime64_2 = 14029467366897019727UL;

		public const ulong Prime64_3 = 1609587929392839161UL;

		public const ulong Prime64_4 = 9650029242287828579UL;

		public const ulong Prime64_5 = 2870177450012600261UL;

		public const uint Prime32_1 = 2654435761U;

		public const uint Prime32_2 = 2246822519U;

		public const uint Prime32_3 = 3266489917U;

		public const uint Prime32_4 = 668265263U;

		public const uint Prime32_5 = 374761393U;

		[StructLayout(LayoutKind.Auto)]
		public struct State
		{
			[FixedBuffer(typeof(ulong), 8)]
			internal XxHashShared.State.<Accumulators>e__FixedBuffer Accumulators;

			[FixedBuffer(typeof(byte), 192)]
			internal XxHashShared.State.<Secret>e__FixedBuffer Secret;

			[FixedBuffer(typeof(byte), 256)]
			internal XxHashShared.State.<Buffer>e__FixedBuffer Buffer;

			internal uint BufferedCount;

			internal ulong StripesProcessedInCurrentBlock;

			internal ulong TotalLength;

			internal ulong Seed;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 64)]
			public struct <Accumulators>e__FixedBuffer
			{
				public ulong FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 256)]
			public struct <Buffer>e__FixedBuffer
			{
				public byte FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 192)]
			public struct <Secret>e__FixedBuffer
			{
				public byte FixedElementField;
			}
		}
	}
}
