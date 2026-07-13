using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.IO.Hashing
{
	public sealed class XxHash3 : NonCryptographicHashAlgorithm
	{
		public XxHash3()
			: this(0L)
		{
		}

		public XxHash3(long seed)
			: base(8)
		{
			XxHashShared.Initialize(ref this._state, (ulong)seed);
		}

		[NullableContext(1)]
		public static byte[] Hash(byte[] source)
		{
			return XxHash3.Hash(source, 0L);
		}

		[NullableContext(1)]
		public static byte[] Hash(byte[] source, long seed)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return XxHash3.Hash(new ReadOnlySpan<byte>(source), seed);
		}

		[return: Nullable(1)]
		public static byte[] Hash(ReadOnlySpan<byte> source, long seed = 0L)
		{
			byte[] array = new byte[8];
			ulong num = XxHash3.HashToUInt64(source, seed);
			BinaryPrimitives.WriteUInt64BigEndian(array, num);
			return array;
		}

		public static int Hash(ReadOnlySpan<byte> source, Span<byte> destination, long seed = 0L)
		{
			int num;
			if (!XxHash3.TryHash(source, destination, out num, seed))
			{
				NonCryptographicHashAlgorithm.ThrowDestinationTooShort();
			}
			return num;
		}

		public static bool TryHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, long seed = 0L)
		{
			if (destination.Length >= 8)
			{
				ulong num = XxHash3.HashToUInt64(source, seed);
				if (BitConverter.IsLittleEndian)
				{
					num = BinaryPrimitives.ReverseEndianness(num);
				}
				Unsafe.WriteUnaligned<ulong>(MemoryMarshal.GetReference<byte>(destination), num);
				bytesWritten = 8;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		[CLSCompliant(false)]
		public unsafe static ulong HashToUInt64(ReadOnlySpan<byte> source, long seed = 0L)
		{
			uint length = (uint)source.Length;
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
			{
				byte* ptr = reference;
				if (length <= 16U)
				{
					return XxHash3.HashLength0To16(ptr, length, (ulong)seed);
				}
				if (length <= 128U)
				{
					return XxHash3.HashLength17To128(ptr, length, (ulong)seed);
				}
				if (length <= 240U)
				{
					return XxHash3.HashLength129To240(ptr, length, (ulong)seed);
				}
				return XxHash3.HashLengthOver240(ptr, length, (ulong)seed);
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
			ulong currentHashAsUInt = this.GetCurrentHashAsUInt64();
			BinaryPrimitives.WriteUInt64BigEndian(destination, currentHashAsUInt);
		}

		[CLSCompliant(false)]
		public unsafe ulong GetCurrentHashAsUInt64()
		{
			ulong num;
			if (this._state.TotalLength > 240UL)
			{
				ulong* ptr = stackalloc ulong[(UIntPtr)64];
				XxHashShared.CopyAccumulators(ref this._state, ptr);
				fixed (byte* ptr2 = &this._state.Secret.FixedElementField)
				{
					byte* ptr3 = ptr2;
					XxHashShared.DigestLong(ref this._state, ptr, ptr3);
					num = XxHashShared.MergeAccumulators(ptr, ptr3 + 11, this._state.TotalLength * 11400714785074694791UL);
				}
			}
			else
			{
				fixed (byte* ptr2 = &this._state.Buffer.FixedElementField)
				{
					num = XxHash3.HashToUInt64(new ReadOnlySpan<byte>((void*)ptr2, (int)this._state.TotalLength), (long)this._state.Seed);
				}
			}
			return num;
		}

		private unsafe static ulong HashLength0To16(byte* source, uint length, ulong seed)
		{
			if (length > 8U)
			{
				return XxHash3.HashLength9To16(source, length, seed);
			}
			if (length >= 4U)
			{
				return XxHash3.HashLength4To8(source, length, seed);
			}
			if (length != 0U)
			{
				return XxHash3.HashLength1To3(source, length, seed);
			}
			return XxHash64.Avalanche(seed ^ 9738745092923071964UL);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong HashLength1To3(byte* source, uint length, ulong seed)
		{
			int num = (int)(*source);
			byte b = source[length >> 1];
			byte b2 = source[length - 1U];
			return XxHash64.Avalanche((ulong)((num << 16) | ((int)b << 24) | (int)b2 | (int)((int)length << 8)) ^ ((ulong)(-2027464037) + seed));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong HashLength4To8(byte* source, uint length, ulong seed)
		{
			seed ^= (ulong)BinaryPrimitives.ReverseEndianness((uint)seed) << 32;
			uint num = XxHashShared.ReadUInt32LE(source);
			ulong num2 = (ulong)XxHashShared.ReadUInt32LE(source + length - 4);
			ulong num3 = 14355981877291832738UL - seed;
			return XxHashShared.Rrmxmx((num2 + ((ulong)num << 32)) ^ num3, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong HashLength9To16(byte* source, uint length, ulong seed)
		{
			ulong num = 7458650908927343033UL + seed;
			ulong num2 = 12634492766384443962UL - seed;
			ulong num3 = XxHashShared.ReadUInt64LE(source) ^ num;
			ulong num4 = XxHashShared.ReadUInt64LE(source + length - 8) ^ num2;
			return XxHashShared.Avalanche((ulong)length + BinaryPrimitives.ReverseEndianness(num3) + num4 + XxHashShared.Multiply64To128ThenFold(num3, num4));
		}

		private unsafe static ulong HashLength17To128(byte* source, uint length, ulong seed)
		{
			ulong num = (ulong)length * 11400714785074694791UL;
			switch ((length - 1U) / 32U)
			{
			case 0U:
				goto IL_00E9;
			case 1U:
				goto IL_00A8;
			case 2U:
				break;
			default:
				num += XxHashShared.Mix16Bytes(source + 48, 4554437623014685352UL, 2111919702937427193UL, seed);
				num += XxHashShared.Mix16Bytes(source + length - 64, 3556072174620004746UL, 7238261902898274248UL, seed);
				break;
			}
			num += XxHashShared.Mix16Bytes(source + 32, 14627906620379768892UL, 11758427054878871688UL, seed);
			num += XxHashShared.Mix16Bytes(source + length - 48, 5690594596133299313UL, 15613098826807580984UL, seed);
			IL_00A8:
			num += XxHashShared.Mix16Bytes(source + 16, 8711581037947681227UL, 2410270004345854594UL, seed);
			num += XxHashShared.Mix16Bytes(source + length - 32, 10242386182634080440UL, 5487137525590930912UL, seed);
			IL_00E9:
			num += XxHashShared.Mix16Bytes(source, 13712233961653862072UL, 2066345149520216444UL, seed);
			num += XxHashShared.Mix16Bytes(source + length - 16, 15823274712020931806UL, 2262974939099578482UL, seed);
			return XxHashShared.Avalanche(num);
		}

		private unsafe static ulong HashLength129To240(byte* source, uint length, ulong seed)
		{
			ulong num = (ulong)length * 11400714785074694791UL;
			num += XxHashShared.Mix16Bytes(source, 13712233961653862072UL, 2066345149520216444UL, seed);
			num += XxHashShared.Mix16Bytes(source + 16, 15823274712020931806UL, 2262974939099578482UL, seed);
			num += XxHashShared.Mix16Bytes(source + 32, 8711581037947681227UL, 2410270004345854594UL, seed);
			num += XxHashShared.Mix16Bytes(source + 48, 10242386182634080440UL, 5487137525590930912UL, seed);
			num += XxHashShared.Mix16Bytes(source + 64, 14627906620379768892UL, 11758427054878871688UL, seed);
			num += XxHashShared.Mix16Bytes(source + 80, 5690594596133299313UL, 15613098826807580984UL, seed);
			num += XxHashShared.Mix16Bytes(source + 96, 4554437623014685352UL, 2111919702937427193UL, seed);
			num += XxHashShared.Mix16Bytes(source + 112, 3556072174620004746UL, 7238261902898274248UL, seed);
			num = XxHashShared.Avalanche(num);
			switch ((length - 128U) / 16U)
			{
			case 0U:
				goto IL_0224;
			case 1U:
				goto IL_0202;
			case 2U:
				goto IL_01E0;
			case 3U:
				goto IL_01BE;
			case 4U:
				goto IL_019C;
			case 5U:
				goto IL_017A;
			case 6U:
				break;
			default:
				num += XxHashShared.Mix16Bytes(source + 224, 13536968629829821247UL, 16163852396094277575UL, seed);
				break;
			}
			num += XxHashShared.Mix16Bytes(source + 208, 17228863761319568023UL, 8573350489219836230UL, seed);
			IL_017A:
			num += XxHashShared.Mix16Bytes(source + 192, 7336514198459093435UL, 5216419214072683403UL, seed);
			IL_019C:
			num += XxHashShared.Mix16Bytes(source + 176, 10391458616325699444UL, 5920048007935066598UL, seed);
			IL_01BE:
			num += XxHashShared.Mix16Bytes(source + 160, 15013455763555273806UL, 5046485836271438973UL, seed);
			IL_01E0:
			num += XxHashShared.Mix16Bytes(source + 144, 11835586108195898345UL, 16607528436649670564UL, seed);
			IL_0202:
			num += XxHashShared.Mix16Bytes(source + 128, 9295848262624092985UL, 7914194659941938988UL, seed);
			IL_0224:
			num += XxHashShared.Mix16Bytes(source + length - 16, 8320639771003045937UL, 16992983559143025252UL, seed);
			return XxHashShared.Avalanche(num);
		}

		private unsafe static ulong HashLengthOver240(byte* source, uint length, ulong seed)
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
				IntPtr intPtr2 = stackalloc byte[(UIntPtr)64];
				XxHashShared.InitializeAccumulators(intPtr2);
				XxHashShared.HashInternalLoop(intPtr2, source, length, ptr);
				return XxHashShared.MergeAccumulators(intPtr2, ptr + 11, (ulong)length * 11400714785074694791UL);
			}
		}

		private new const int HashLengthInBytes = 8;

		private XxHashShared.State _state;
	}
}
