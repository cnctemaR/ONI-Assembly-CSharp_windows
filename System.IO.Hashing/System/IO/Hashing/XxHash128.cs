using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.IO.Hashing
{
	public sealed class XxHash128 : NonCryptographicHashAlgorithm
	{
		public XxHash128()
			: this(0L)
		{
		}

		public XxHash128(long seed)
			: base(16)
		{
			XxHashShared.Initialize(ref this._state, (ulong)seed);
		}

		[NullableContext(1)]
		public static byte[] Hash(byte[] source)
		{
			return XxHash128.Hash(source, 0L);
		}

		[NullableContext(1)]
		public static byte[] Hash(byte[] source, long seed)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return XxHash128.Hash(new ReadOnlySpan<byte>(source), seed);
		}

		[return: Nullable(1)]
		public static byte[] Hash(ReadOnlySpan<byte> source, long seed = 0L)
		{
			byte[] array = new byte[16];
			XxHash128.Hash(source, array, seed);
			return array;
		}

		public static int Hash(ReadOnlySpan<byte> source, Span<byte> destination, long seed = 0L)
		{
			int num;
			if (!XxHash128.TryHash(source, destination, out num, seed))
			{
				NonCryptographicHashAlgorithm.ThrowDestinationTooShort();
			}
			return num;
		}

		public static bool TryHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, long seed = 0L)
		{
			if (destination.Length >= 16)
			{
				XxHash128.Hash128 hash = XxHash128.HashToHash128(source, seed);
				XxHash128.WriteBigEndian128(in hash, destination);
				bytesWritten = 16;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		private unsafe static XxHash128.Hash128 HashToHash128(ReadOnlySpan<byte> source, long seed = 0L)
		{
			uint length = (uint)source.Length;
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
			{
				byte* ptr = reference;
				if (length <= 16U)
				{
					return XxHash128.HashLength0To16(ptr, length, (ulong)seed);
				}
				if (length <= 128U)
				{
					return XxHash128.HashLength17To128(ptr, length, (ulong)seed);
				}
				if (length <= 240U)
				{
					return XxHash128.HashLength129To240(ptr, length, (ulong)seed);
				}
				return XxHash128.HashLengthOver240(ptr, length, (ulong)seed);
			}
		}

		public override void Reset()
		{
			XxHashShared.Reset(ref this._state);
		}

		public override void Append(ReadOnlySpan<byte> source)
		{
			XxHashShared.Append(ref this._state, source);
		}

		protected override void GetCurrentHashCore(Span<byte> destination)
		{
			XxHash128.Hash128 currentHashAsHash = this.GetCurrentHashAsHash128();
			XxHash128.WriteBigEndian128(in currentHashAsHash, destination);
		}

		private unsafe XxHash128.Hash128 GetCurrentHashAsHash128()
		{
			XxHash128.Hash128 hash;
			if (this._state.TotalLength > 240UL)
			{
				ulong* ptr = stackalloc ulong[(UIntPtr)64];
				XxHashShared.CopyAccumulators(ref this._state, ptr);
				fixed (byte* ptr2 = &this._state.Secret.FixedElementField)
				{
					byte* ptr3 = ptr2;
					XxHashShared.DigestLong(ref this._state, ptr, ptr3);
					hash = new XxHash128.Hash128(XxHashShared.MergeAccumulators(ptr, ptr3 + 11, this._state.TotalLength * 11400714785074694791UL), XxHashShared.MergeAccumulators(ptr, ptr3 + 192 - 64 - 11, ~(this._state.TotalLength * 14029467366897019727UL)));
				}
			}
			else
			{
				fixed (byte* ptr2 = &this._state.Buffer.FixedElementField)
				{
					hash = XxHash128.HashToHash128(new ReadOnlySpan<byte>((void*)ptr2, (int)this._state.TotalLength), (long)this._state.Seed);
				}
			}
			return hash;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteBigEndian128(in XxHash128.Hash128 hash, Span<byte> destination)
		{
			ulong num = hash.Low64;
			ulong num2 = hash.High64;
			if (BitConverter.IsLittleEndian)
			{
				num = BinaryPrimitives.ReverseEndianness(num);
				num2 = BinaryPrimitives.ReverseEndianness(num2);
			}
			ref byte reference = MemoryMarshal.GetReference<byte>(destination);
			Unsafe.WriteUnaligned<ulong>(ref reference, num2);
			Unsafe.WriteUnaligned<ulong>(Unsafe.AddByteOffset<byte>(ref reference, new IntPtr(8)), num);
		}

		private unsafe static XxHash128.Hash128 HashLength0To16(byte* source, uint length, ulong seed)
		{
			if (length > 8U)
			{
				return XxHash128.HashLength9To16(source, length, seed);
			}
			if (length >= 4U)
			{
				return XxHash128.HashLength4To8(source, length, seed);
			}
			if (length != 0U)
			{
				return XxHash128.HashLength1To3(source, length, seed);
			}
			return new XxHash128.Hash128(XxHash64.Avalanche(seed ^ 7507096552062056628UL), XxHash64.Avalanche(seed ^ 10832796526425111881UL));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static XxHash128.Hash128 HashLength1To3(byte* source, uint length, ulong seed)
		{
			int num = (int)(*source);
			byte b = source[length >> 1];
			byte b2 = source[length - 1U];
			int num2 = (num << 16) | ((int)b << 24) | (int)b2 | (int)((int)length << 8);
			uint num3 = BitOperations.RotateLeft(BinaryPrimitives.ReverseEndianness((uint)num2), 13);
			ulong num4 = (ulong)(-2027464037) + seed;
			ulong num5 = 808198283UL - seed;
			ulong num6 = (ulong)num2 ^ num4;
			ulong num7 = (ulong)num3 ^ num5;
			return new XxHash128.Hash128(XxHash64.Avalanche(num6), XxHash64.Avalanche(num7));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static XxHash128.Hash128 HashLength4To8(byte* source, uint length, ulong seed)
		{
			seed ^= (ulong)BinaryPrimitives.ReverseEndianness((uint)seed) << 32;
			ulong num = (ulong)XxHashShared.ReadUInt32LE(source);
			uint num2 = XxHashShared.ReadUInt32LE(source + length - 4);
			ulong num3 = num + ((ulong)num2 << 32);
			ulong num4 = 14190881133394760876UL + seed;
			ulong num6;
			ulong num5 = XxHashShared.Multiply64To128(num3 ^ num4, 11400714785074694791UL + (ulong)((ulong)length << 2), out num6);
			num5 += num6 << 1;
			num6 ^= num5 >> 3;
			num6 = XxHashShared.XorShift(num6, 35);
			num6 *= 11507291218515648293UL;
			num6 = XxHashShared.XorShift(num6, 28);
			num5 = XxHashShared.Avalanche(num5);
			return new XxHash128.Hash128(num6, num5);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static XxHash128.Hash128 HashLength9To16(byte* source, uint length, ulong seed)
		{
			ulong num = 6455697860950631241UL - seed;
			ulong num2 = 13979869743488056664UL + seed;
			ulong num3 = XxHashShared.ReadUInt64LE(source);
			ulong num4 = XxHashShared.ReadUInt64LE(source + length - 8);
			ulong num6;
			ulong num5 = XxHashShared.Multiply64To128(num3 ^ num4 ^ num, 11400714785074694791UL, out num6);
			num6 += (ulong)(length - 1U) << 54;
			num4 ^= num2;
			num5 += ((sizeof(void*) < 8) ? ((num4 & 18446744069414584320UL) + XxHashShared.Multiply32To64((uint)num4, 2246822519U)) : (num4 + XxHashShared.Multiply32To64((uint)num4, 2246822518U)));
			num6 ^= BinaryPrimitives.ReverseEndianness(num5);
			ulong num8;
			ulong num7 = XxHashShared.Multiply64To128(num6, 14029467366897019727UL, out num8);
			num7 += num5 * 14029467366897019727UL;
			num8 = XxHashShared.Avalanche(num8);
			num7 = XxHashShared.Avalanche(num7);
			return new XxHash128.Hash128(num8, num7);
		}

		private unsafe static XxHash128.Hash128 HashLength17To128(byte* source, uint length, ulong seed)
		{
			ulong num = (ulong)length * 11400714785074694791UL;
			ulong num2 = 0UL;
			switch ((length - 1U) / 32U)
			{
			case 0U:
				goto IL_00D4;
			case 1U:
				goto IL_009B;
			case 2U:
				break;
			default:
				XxHash128.Mix32Bytes(ref num, ref num2, source + 48, source + length - 64, 4554437623014685352UL, 2111919702937427193UL, 3556072174620004746UL, 7238261902898274248UL, seed);
				break;
			}
			XxHash128.Mix32Bytes(ref num, ref num2, source + 32, source + length - 48, 14627906620379768892UL, 11758427054878871688UL, 5690594596133299313UL, 15613098826807580984UL, seed);
			IL_009B:
			XxHash128.Mix32Bytes(ref num, ref num2, source + 16, source + length - 32, 8711581037947681227UL, 2410270004345854594UL, 10242386182634080440UL, 5487137525590930912UL, seed);
			IL_00D4:
			XxHash128.Mix32Bytes(ref num, ref num2, source, source + length - 16, 13712233961653862072UL, 2066345149520216444UL, 15823274712020931806UL, 2262974939099578482UL, seed);
			return XxHash128.AvalancheHash(num, num2, length, seed);
		}

		private unsafe static XxHash128.Hash128 HashLength129To240(byte* source, uint length, ulong seed)
		{
			ulong num = (ulong)length * 11400714785074694791UL;
			ulong num2 = 0UL;
			XxHash128.Mix32Bytes(ref num, ref num2, source, source + 16, 13712233961653862072UL, 2066345149520216444UL, 15823274712020931806UL, 2262974939099578482UL, seed);
			XxHash128.Mix32Bytes(ref num, ref num2, source + 32, source + 32 + 16, 8711581037947681227UL, 2410270004345854594UL, 10242386182634080440UL, 5487137525590930912UL, seed);
			XxHash128.Mix32Bytes(ref num, ref num2, source + 64, source + 64 + 16, 14627906620379768892UL, 11758427054878871688UL, 5690594596133299313UL, 15613098826807580984UL, seed);
			XxHash128.Mix32Bytes(ref num, ref num2, source + 96, source + 96 + 16, 4554437623014685352UL, 2111919702937427193UL, 3556072174620004746UL, 7238261902898274248UL, seed);
			num = XxHashShared.Avalanche(num);
			num2 = XxHashShared.Avalanche(num2);
			uint num3 = (length - 128U) / 32U;
			if (num3 != 0U)
			{
				XxHash128.Mix32Bytes(ref num, ref num2, source + 128, source + 128 + 16, 9295848262624092985UL, 7914194659941938988UL, 11835586108195898345UL, 16607528436649670564UL, seed);
				if (num3 >= 2U)
				{
					XxHash128.Mix32Bytes(ref num, ref num2, source + 160, source + 160 + 16, 15013455763555273806UL, 5046485836271438973UL, 10391458616325699444UL, 5920048007935066598UL, seed);
					if (num3 == 3U)
					{
						XxHash128.Mix32Bytes(ref num, ref num2, source + 192, source + 192 + 16, 7336514198459093435UL, 5216419214072683403UL, 17228863761319568023UL, 8573350489219836230UL, seed);
					}
				}
			}
			XxHash128.Mix32Bytes(ref num, ref num2, source + length - 16, source + length - 32, 5695865814404364607UL, 6464017090953185821UL, 8320639771003045937UL, 16992983559143025252UL, 0UL - seed);
			return XxHash128.AvalancheHash(num, num2, length, seed);
		}

		private unsafe static XxHash128.Hash128 HashLengthOver240(byte* source, uint length, ulong seed)
		{
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(XxHashShared.DefaultSecret))
			{
				byte* ptr = reference;
				if (seed != 0UL)
				{
					IntPtr intPtr = stackalloc byte[(UIntPtr)192];
					XxHashShared.DeriveSecretFromSeed(intPtr, seed);
					ptr = intPtr;
				}
				ulong* ptr2 = stackalloc ulong[(UIntPtr)64];
				XxHashShared.InitializeAccumulators(ptr2);
				XxHashShared.HashInternalLoop(ptr2, source, length, ptr);
				return new XxHash128.Hash128(XxHashShared.MergeAccumulators(ptr2, ptr + 11, (ulong)length * 11400714785074694791UL), XxHashShared.MergeAccumulators(ptr2, ptr + 192 - 64 - 11, ~((ulong)length * 14029467366897019727UL)));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static XxHash128.Hash128 AvalancheHash(ulong accLow, ulong accHigh, uint length, ulong seed)
		{
			ulong num = accLow + accHigh;
			ulong num2 = accLow * 11400714785074694791UL + accHigh * 9650029242287828579UL + ((ulong)length - seed) * 14029467366897019727UL;
			ulong num3 = XxHashShared.Avalanche(num);
			num2 = 0UL - XxHashShared.Avalanche(num2);
			return new XxHash128.Hash128(num3, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Mix32Bytes(ref ulong accLow, ref ulong accHigh, byte* input1, byte* input2, ulong secret1, ulong secret2, ulong secret3, ulong secret4, ulong seed)
		{
			accLow += XxHashShared.Mix16Bytes(input1, secret1, secret2, seed);
			accLow ^= XxHashShared.ReadUInt64LE(input2) + XxHashShared.ReadUInt64LE(input2 + 8);
			accHigh += XxHashShared.Mix16Bytes(input2, secret3, secret4, seed);
			accHigh ^= XxHashShared.ReadUInt64LE(input1) + XxHashShared.ReadUInt64LE(input1 + 8);
		}

		private new const int HashLengthInBytes = 16;

		private XxHashShared.State _state;

		[DebuggerDisplay("Low64 = {Low64}, High64 = {High64}")]
		private readonly struct Hash128
		{
			public Hash128(ulong low64, ulong high64)
			{
				this.Low64 = low64;
				this.High64 = high64;
			}

			public readonly ulong Low64;

			public readonly ulong High64;
		}
	}
}
