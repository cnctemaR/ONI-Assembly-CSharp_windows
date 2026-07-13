using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.IO.Hashing
{
	public sealed class XxHash32 : NonCryptographicHashAlgorithm
	{
		public XxHash32()
			: this(0)
		{
		}

		public XxHash32(int seed)
			: base(4)
		{
			this._seed = (uint)seed;
			this.Reset();
		}

		public override void Reset()
		{
			this._state = new XxHash32.State(this._seed);
			this._length = 0;
		}

		public override void Append(ReadOnlySpan<byte> source)
		{
			int num = this._length & 15;
			if (num != 0)
			{
				int num2 = 16 - num;
				if (source.Length < num2)
				{
					source.CopyTo(MemoryExtensions.AsSpan<byte>(this._holdback, num));
					this._length += source.Length;
					return;
				}
				source.Slice(0, num2).CopyTo(MemoryExtensions.AsSpan<byte>(this._holdback, num));
				this._state.ProcessStripe(this._holdback);
				source = source.Slice(num2);
				this._length += num2;
			}
			while (source.Length >= 16)
			{
				this._state.ProcessStripe(source);
				source = source.Slice(16);
				this._length += 16;
			}
			if (source.Length > 0)
			{
				if (this._holdback == null)
				{
					this._holdback = new byte[16];
				}
				source.CopyTo(this._holdback);
				this._length += source.Length;
			}
		}

		protected override void GetCurrentHashCore(Span<byte> destination)
		{
			uint currentHashAsUInt = this.GetCurrentHashAsUInt32();
			BinaryPrimitives.WriteUInt32BigEndian(destination, currentHashAsUInt);
		}

		[CLSCompliant(false)]
		public uint GetCurrentHashAsUInt32()
		{
			int num = this._length & 15;
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
			return XxHash32.Hash(new ReadOnlySpan<byte>(source), 0);
		}

		[NullableContext(1)]
		public static byte[] Hash(byte[] source, int seed)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return XxHash32.Hash(new ReadOnlySpan<byte>(source), seed);
		}

		[return: Nullable(1)]
		public static byte[] Hash(ReadOnlySpan<byte> source, int seed = 0)
		{
			byte[] array = new byte[4];
			uint num = XxHash32.HashToUInt32(source, seed);
			BinaryPrimitives.WriteUInt32BigEndian(array, num);
			return array;
		}

		public static bool TryHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, int seed = 0)
		{
			if (destination.Length < 4)
			{
				bytesWritten = 0;
				return false;
			}
			uint num = XxHash32.HashToUInt32(source, seed);
			BinaryPrimitives.WriteUInt32BigEndian(destination, num);
			bytesWritten = 4;
			return true;
		}

		public static int Hash(ReadOnlySpan<byte> source, Span<byte> destination, int seed = 0)
		{
			if (destination.Length < 4)
			{
				NonCryptographicHashAlgorithm.ThrowDestinationTooShort();
			}
			uint num = XxHash32.HashToUInt32(source, seed);
			BinaryPrimitives.WriteUInt32BigEndian(destination, num);
			return 4;
		}

		[CLSCompliant(false)]
		public static uint HashToUInt32(ReadOnlySpan<byte> source, int seed = 0)
		{
			int length = source.Length;
			XxHash32.State state = new XxHash32.State((uint)seed);
			while (source.Length >= 16)
			{
				state.ProcessStripe(source);
				source = source.Slice(16);
			}
			return state.Complete(length, source);
		}

		private const int HashSize = 4;

		private const int StripeSize = 16;

		private readonly uint _seed;

		private XxHash32.State _state;

		private byte[] _holdback;

		private int _length;

		private struct State
		{
			internal State(uint seed)
			{
				this._acc1 = seed + 606290984U;
				this._acc2 = seed + 2246822519U;
				this._acc3 = seed;
				this._acc4 = seed - 2654435761U;
				this._smallAcc = seed + 374761393U;
				this._hadFullStripe = false;
			}

			internal void ProcessStripe(ReadOnlySpan<byte> source)
			{
				source = source.Slice(0, 16);
				this._acc1 = XxHash32.State.ApplyRound(this._acc1, source);
				this._acc2 = XxHash32.State.ApplyRound(this._acc2, source.Slice(4));
				this._acc3 = XxHash32.State.ApplyRound(this._acc3, source.Slice(8));
				this._acc4 = XxHash32.State.ApplyRound(this._acc4, source.Slice(12));
				this._hadFullStripe = true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private readonly uint Converge()
			{
				return BitOperations.RotateLeft(this._acc1, 1) + BitOperations.RotateLeft(this._acc2, 7) + BitOperations.RotateLeft(this._acc3, 12) + BitOperations.RotateLeft(this._acc4, 18);
			}

			private static uint ApplyRound(uint acc, ReadOnlySpan<byte> lane)
			{
				acc += BinaryPrimitives.ReadUInt32LittleEndian(lane) * 2246822519U;
				acc = BitOperations.RotateLeft(acc, 13);
				acc *= 2654435761U;
				return acc;
			}

			internal unsafe readonly uint Complete(int length, ReadOnlySpan<byte> remaining)
			{
				uint num = (this._hadFullStripe ? this.Converge() : this._smallAcc);
				num += (uint)length;
				while (remaining.Length >= 4)
				{
					uint num2 = BinaryPrimitives.ReadUInt32LittleEndian(remaining);
					num += num2 * 3266489917U;
					num = BitOperations.RotateLeft(num, 17);
					num *= 668265263U;
					remaining = remaining.Slice(4);
				}
				for (int i = 0; i < remaining.Length; i++)
				{
					uint num3 = (uint)(*remaining[i]);
					num += num3 * 374761393U;
					num = BitOperations.RotateLeft(num, 11);
					num *= 2654435761U;
				}
				num ^= num >> 15;
				num *= 2246822519U;
				num ^= num >> 13;
				num *= 3266489917U;
				return num ^ (num >> 16);
			}

			private uint _acc1;

			private uint _acc2;

			private uint _acc3;

			private uint _acc4;

			private readonly uint _smallAcc;

			private bool _hadFullStripe;
		}
	}
}
