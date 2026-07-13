using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.IO.Hashing
{
	public sealed class XxHash64 : NonCryptographicHashAlgorithm
	{
		public XxHash64()
			: this(0L)
		{
		}

		public XxHash64(long seed)
			: base(8)
		{
			this._seed = (ulong)seed;
			this.Reset();
		}

		public override void Reset()
		{
			this._state = new XxHash64.State(this._seed);
			this._length = 0L;
		}

		public override void Append(ReadOnlySpan<byte> source)
		{
			int num = (int)this._length & 31;
			if (num != 0)
			{
				int num2 = 32 - num;
				if (source.Length < num2)
				{
					source.CopyTo(MemoryExtensions.AsSpan<byte>(this._holdback, num));
					this._length += (long)source.Length;
					return;
				}
				source.Slice(0, num2).CopyTo(MemoryExtensions.AsSpan<byte>(this._holdback, num));
				this._state.ProcessStripe(this._holdback);
				source = source.Slice(num2);
				this._length += (long)num2;
			}
			while (source.Length >= 32)
			{
				this._state.ProcessStripe(source);
				source = source.Slice(32);
				this._length += 32L;
			}
			if (source.Length > 0)
			{
				if (this._holdback == null)
				{
					this._holdback = new byte[32];
				}
				source.CopyTo(this._holdback);
				this._length += (long)source.Length;
			}
		}

		protected override void GetCurrentHashCore(Span<byte> destination)
		{
			ulong currentHashAsUInt = this.GetCurrentHashAsUInt64();
			BinaryPrimitives.WriteUInt64BigEndian(destination, currentHashAsUInt);
		}

		[CLSCompliant(false)]
		public ulong GetCurrentHashAsUInt64()
		{
			int num = (int)this._length & 31;
			ReadOnlySpan<byte> empty = ReadOnlySpan<byte>.Empty;
			if (num > 0)
			{
				empty = new ReadOnlySpan<byte>(this._holdback, 0, num);
			}
			return this._state.Complete(this._length, empty);
		}

		[NullableContext(1)]
		public static byte[] Hash(byte[] source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return XxHash64.Hash(new ReadOnlySpan<byte>(source), 0L);
		}

		[NullableContext(1)]
		public static byte[] Hash(byte[] source, long seed)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return XxHash64.Hash(new ReadOnlySpan<byte>(source), seed);
		}

		[return: Nullable(1)]
		public static byte[] Hash(ReadOnlySpan<byte> source, long seed = 0L)
		{
			byte[] array = new byte[8];
			ulong num = XxHash64.HashToUInt64(source, seed);
			BinaryPrimitives.WriteUInt64BigEndian(array, num);
			return array;
		}

		public static bool TryHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, long seed = 0L)
		{
			if (destination.Length < 8)
			{
				bytesWritten = 0;
				return false;
			}
			ulong num = XxHash64.HashToUInt64(source, seed);
			BinaryPrimitives.WriteUInt64BigEndian(destination, num);
			bytesWritten = 8;
			return true;
		}

		public static int Hash(ReadOnlySpan<byte> source, Span<byte> destination, long seed = 0L)
		{
			if (destination.Length < 8)
			{
				NonCryptographicHashAlgorithm.ThrowDestinationTooShort();
			}
			ulong num = XxHash64.HashToUInt64(source, seed);
			BinaryPrimitives.WriteUInt64BigEndian(destination, num);
			return 8;
		}

		[CLSCompliant(false)]
		public static ulong HashToUInt64(ReadOnlySpan<byte> source, long seed = 0L)
		{
			int length = source.Length;
			XxHash64.State state = new XxHash64.State((ulong)seed);
			while (source.Length >= 32)
			{
				state.ProcessStripe(source);
				source = source.Slice(32);
			}
			return state.Complete((long)((ulong)length), source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ulong Avalanche(ulong hash)
		{
			hash ^= hash >> 33;
			hash *= 14029467366897019727UL;
			hash ^= hash >> 29;
			hash *= 1609587929392839161UL;
			hash ^= hash >> 32;
			return hash;
		}

		private const int HashSize = 8;

		private const int StripeSize = 32;

		private readonly ulong _seed;

		private XxHash64.State _state;

		private byte[] _holdback;

		private long _length;

		private struct State
		{
			internal State(ulong seed)
			{
				this._acc1 = seed + 6983438078262162902UL;
				this._acc2 = seed + 14029467366897019727UL;
				this._acc3 = seed;
				this._acc4 = seed - 11400714785074694791UL;
				this._smallAcc = seed + 2870177450012600261UL;
				this._hadFullStripe = false;
			}

			internal void ProcessStripe(ReadOnlySpan<byte> source)
			{
				source = source.Slice(0, 32);
				this._acc1 = XxHash64.State.ApplyRound(this._acc1, source);
				this._acc2 = XxHash64.State.ApplyRound(this._acc2, source.Slice(8));
				this._acc3 = XxHash64.State.ApplyRound(this._acc3, source.Slice(16));
				this._acc4 = XxHash64.State.ApplyRound(this._acc4, source.Slice(24));
				this._hadFullStripe = true;
			}

			private static ulong MergeAccumulator(ulong acc, ulong accN)
			{
				acc ^= XxHash64.State.ApplyRound(0UL, accN);
				acc *= 11400714785074694791UL;
				acc += 9650029242287828579UL;
				return acc;
			}

			private readonly ulong Converge()
			{
				return XxHash64.State.MergeAccumulator(XxHash64.State.MergeAccumulator(XxHash64.State.MergeAccumulator(XxHash64.State.MergeAccumulator(BitOperations.RotateLeft(this._acc1, 1) + BitOperations.RotateLeft(this._acc2, 7) + BitOperations.RotateLeft(this._acc3, 12) + BitOperations.RotateLeft(this._acc4, 18), this._acc1), this._acc2), this._acc3), this._acc4);
			}

			private static ulong ApplyRound(ulong acc, ReadOnlySpan<byte> lane)
			{
				return XxHash64.State.ApplyRound(acc, BinaryPrimitives.ReadUInt64LittleEndian(lane));
			}

			private static ulong ApplyRound(ulong acc, ulong lane)
			{
				acc += lane * 14029467366897019727UL;
				acc = BitOperations.RotateLeft(acc, 31);
				acc *= 11400714785074694791UL;
				return acc;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			internal unsafe readonly ulong Complete(long length, ReadOnlySpan<byte> remaining)
			{
				ulong num = (this._hadFullStripe ? this.Converge() : this._smallAcc);
				num += (ulong)length;
				while (remaining.Length >= 8)
				{
					ulong num2 = BinaryPrimitives.ReadUInt64LittleEndian(remaining);
					num ^= XxHash64.State.ApplyRound(0UL, num2);
					num = BitOperations.RotateLeft(num, 27);
					num *= 11400714785074694791UL;
					num += 9650029242287828579UL;
					remaining = remaining.Slice(8);
				}
				if (remaining.Length >= 4)
				{
					ulong num3 = (ulong)BinaryPrimitives.ReadUInt32LittleEndian(remaining);
					num ^= num3 * 11400714785074694791UL;
					num = BitOperations.RotateLeft(num, 23);
					num *= 14029467366897019727UL;
					num += 1609587929392839161UL;
					remaining = remaining.Slice(4);
				}
				for (int i = 0; i < remaining.Length; i++)
				{
					ulong num4 = (ulong)(*remaining[i]);
					num ^= num4 * 2870177450012600261UL;
					num = BitOperations.RotateLeft(num, 11);
					num *= 11400714785074694791UL;
				}
				return XxHash64.Avalanche(num);
			}

			private ulong _acc1;

			private ulong _acc2;

			private ulong _acc3;

			private ulong _acc4;

			private readonly ulong _smallAcc;

			private bool _hadFullStripe;
		}
	}
}
