using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Utils
{
	internal sealed class BytePatternCollection : IEnumerable<BytePattern>, IEnumerable
	{
		public int MinLength { get; }

		public int MaxMinLength { get; }

		public int MaxAddressLength { get; }

		public BytePatternCollection([Nullable(new byte[] { 0, 2 })] global::System.ReadOnlyMemory<BytePattern> patterns)
		{
			int num;
			int num2;
			int num3;
			ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]> valueTuple = BytePatternCollection.ComputeLut(patterns, out num, out num2, out num3);
			this.patternCollections = valueTuple.Item1;
			this.emptyPatterns = valueTuple.Item2;
			this.MinLength = num;
			this.MaxMinLength = num2;
			this.MaxAddressLength = num3;
			Helpers.Assert(this.MinLength > 0, null, "MinLength > 0");
		}

		public BytePatternCollection([Nullable(new byte[] { 1, 2 })] params BytePattern[] patterns)
			: this(patterns.AsMemory<BytePattern>())
		{
		}

		[NullableContext(1)]
		public IEnumerator<BytePattern> GetEnumerator()
		{
			BytePatternCollection.<GetEnumerator>d__13 <GetEnumerator>d__ = new BytePatternCollection.<GetEnumerator>d__13(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		[NullableContext(1)]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[return: Nullable(new byte[] { 0, 1, 2, 1 })]
		private unsafe static ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]> ComputeLut([Nullable(new byte[] { 0, 2 })] global::System.ReadOnlyMemory<BytePattern> patterns, out int minLength, out int maxMinLength, out int maxAddrLength)
		{
			if (patterns.Length == 0)
			{
				minLength = 0;
				maxMinLength = 0;
				maxAddrLength = 0;
				return new ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]>(ArrayEx.Empty<BytePatternCollection.HomogenousPatternCollection>(), null);
			}
			global::System.Span<int> span = new global::System.Span<int>(stackalloc byte[(UIntPtr)1024], 256);
			minLength = int.MaxValue;
			maxMinLength = int.MinValue;
			maxAddrLength = 0;
			int[][] array = null;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < patterns.Length; i++)
			{
				BytePattern bytePattern = *patterns.Span[i];
				if (bytePattern != null)
				{
					if (bytePattern.MinLength < minLength)
					{
						minLength = bytePattern.MinLength;
					}
					if (bytePattern.MinLength > maxMinLength)
					{
						maxMinLength = bytePattern.MinLength;
					}
					if (bytePattern.AddressBytes > maxAddrLength)
					{
						maxAddrLength = bytePattern.AddressBytes;
					}
					ValueTuple<global::System.ReadOnlyMemory<byte>, int> firstLiteralSegment = bytePattern.FirstLiteralSegment;
					global::System.ReadOnlyMemory<byte> item = firstLiteralSegment.Item1;
					int item2 = firstLiteralSegment.Item2;
					if (item.Length == 0)
					{
						num++;
					}
					else
					{
						num2 = 1;
						if (item2 == 0)
						{
							(*span[(int)(*item.Span[0])])++;
						}
						else
						{
							if (array == null || array.Length < item2)
							{
								Array.Resize<int[]>(ref array, item2);
							}
							ref int[] ptr = ref array[item2 - 1];
							if (ptr == null)
							{
								ptr = new int[256];
							}
							ptr[(int)(*item.Span[0])]++;
						}
					}
				}
			}
			if (array != null)
			{
				int[][] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] != null)
					{
						num2++;
					}
				}
			}
			BytePattern[] array3 = ((num > 0) ? new BytePattern[num] : null);
			int num3 = 0;
			BytePatternCollection.HomogenousPatternCollection[] array4 = new BytePatternCollection.HomogenousPatternCollection[num2];
			int num4 = 1;
			array4[0] = new BytePatternCollection.HomogenousPatternCollection(0);
			for (int k = 0; k < patterns.Length; k++)
			{
				BytePattern bytePattern2 = *patterns.Span[k];
				if (bytePattern2 != null)
				{
					ValueTuple<global::System.ReadOnlyMemory<byte>, int> firstLiteralSegment2 = bytePattern2.FirstLiteralSegment;
					global::System.ReadOnlyMemory<byte> item3 = firstLiteralSegment2.Item1;
					int item4 = firstLiteralSegment2.Item2;
					if (item3.Length == 0)
					{
						array3[num3++] = bytePattern2;
					}
					else
					{
						int num5 = -1;
						for (int l = 0; l < array4.Length; l++)
						{
							if (array4[l].Offset == item4)
							{
								num5 = l;
								break;
							}
						}
						if (num5 == -1)
						{
							num5 = num4++;
							array4[num5] = new BytePatternCollection.HomogenousPatternCollection(item4);
						}
						global::System.ReadOnlySpan<int> readOnlySpan = ((item4 == 0) ? span : array[item4 - 1].AsSpan<int>());
						BytePatternCollection.<ComputeLut>g__AddToPatternCollection|15_0(ref array4[num5], readOnlySpan, bytePattern2);
						if (num5 > 0 && array4[num5 - 1].Offset > array4[num5].Offset)
						{
							Helpers.Swap<BytePatternCollection.HomogenousPatternCollection>(ref array4[num5 - 1], ref array4[num5]);
						}
					}
				}
			}
			return new ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]>(array4, array3);
		}

		public unsafe bool TryMatchAt(global::System.ReadOnlySpan<byte> data, out ulong address, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = 0;
				address = 0UL;
				matchingPattern = null;
				return false;
			}
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			bool flag = this.TryMatchAt(data, span, out matchingPattern, out length);
			address = Unsafe.ReadUnaligned<ulong>(span[0]);
			return flag;
		}

		public unsafe bool TryMatchAt(global::System.ReadOnlySpan<byte> data, global::System.Span<byte> addrBuf, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = 0;
				matchingPattern = null;
				return false;
			}
			for (int i = 0; i < this.patternCollections.Length; i++)
			{
				ref BytePatternCollection.HomogenousPatternCollection ptr = ref this.patternCollections[i];
				if (data.Length >= ptr.Offset + ptr.MinLength)
				{
					byte b = *data[ptr.Offset];
					BytePattern[] array = ptr.Lut[(int)b];
					if (array != null)
					{
						foreach (BytePattern bytePattern in array)
						{
							if (bytePattern.TryMatchAt(data, addrBuf, out length))
							{
								matchingPattern = bytePattern;
								return true;
							}
						}
					}
				}
			}
			if (this.emptyPatterns != null)
			{
				foreach (BytePattern bytePattern2 in this.emptyPatterns)
				{
					if (bytePattern2.TryMatchAt(data, addrBuf, out length))
					{
						matchingPattern = bytePattern2;
						return true;
					}
				}
			}
			matchingPattern = null;
			length = 0;
			return false;
		}

		public unsafe bool TryFindMatch(global::System.ReadOnlySpan<byte> data, out ulong address, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int offset, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = (offset = 0);
				address = 0UL;
				matchingPattern = null;
				return false;
			}
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			bool flag = this.TryFindMatch(data, span, out matchingPattern, out offset, out length);
			address = Unsafe.ReadUnaligned<ulong>(span[0]);
			return flag;
		}

		public unsafe bool TryFindMatch(global::System.ReadOnlySpan<byte> data, global::System.Span<byte> addrBuf, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int offset, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = (offset = 0);
				matchingPattern = null;
				return false;
			}
			global::System.ReadOnlySpan<byte> span = this.PossibleFirstBytes.Span;
			int num = 0;
			ref BytePatternCollection.HomogenousPatternCollection ptr;
			for (;;)
			{
				int num2 = data.Slice(num).IndexOfAny<byte>(span);
				if (num2 < 0)
				{
					goto IL_011B;
				}
				offset = num + num2;
				byte b = *data[offset];
				for (int i = 0; i < this.patternCollections.Length; i++)
				{
					ptr = ref this.patternCollections[i];
					if (offset >= ptr.Offset && data.Length >= offset + ptr.MinLength)
					{
						BytePattern[] array = ptr.Lut[(int)b];
						if (array != null)
						{
							foreach (BytePattern bytePattern in array)
							{
								if ((offset == 0 || !bytePattern.MustMatchAtStart) && bytePattern.TryMatchAt(data.Slice(offset - ptr.Offset), addrBuf, out length))
								{
									goto Block_7;
								}
							}
						}
					}
				}
				num = offset + 1;
			}
			Block_7:
			offset -= ptr.Offset;
			BytePattern bytePattern;
			matchingPattern = bytePattern;
			return true;
			IL_011B:
			if (this.emptyPatterns != null)
			{
				foreach (BytePattern bytePattern2 in this.emptyPatterns)
				{
					if (bytePattern2.TryFindMatch(data, addrBuf, out offset, out length))
					{
						matchingPattern = bytePattern2;
						return true;
					}
				}
			}
			matchingPattern = null;
			offset = 0;
			length = 0;
			return false;
		}

		private global::System.ReadOnlyMemory<byte> PossibleFirstBytes
		{
			get
			{
				global::System.ReadOnlyMemory<byte> readOnlyMemory = this.lazyPossibleFirstBytes.GetValueOrDefault();
				if (this.lazyPossibleFirstBytes == null)
				{
					readOnlyMemory = this.GetPossibleFirstBytes();
					this.lazyPossibleFirstBytes = new global::System.ReadOnlyMemory<byte>?(readOnlyMemory);
					return readOnlyMemory;
				}
				return readOnlyMemory;
			}
		}

		private global::System.ReadOnlyMemory<byte> GetPossibleFirstBytes()
		{
			global::System.Memory<byte> memory = new byte[512].AsMemory<byte>();
			BytePatternCollection.FirstByteCollection firstByteCollection = new BytePatternCollection.FirstByteCollection(memory.Span);
			for (int i = 0; i < this.patternCollections.Length; i++)
			{
				this.patternCollections[i].AddFirstBytes(ref firstByteCollection);
			}
			return memory.Slice(0, firstByteCollection.FirstBytes.Length);
		}

		[CompilerGenerated]
		internal unsafe static void <ComputeLut>g__AddToPatternCollection|15_0(ref BytePatternCollection.HomogenousPatternCollection collection, global::System.ReadOnlySpan<int> arrayCounts, [Nullable(1)] BytePattern pattern)
		{
			global::System.ReadOnlyMemory<byte> item = pattern.FirstLiteralSegment.Item1;
			if (collection.Lut == null)
			{
				BytePattern[][] array = new BytePattern[256][];
				for (int i = 0; i < arrayCounts.Length; i++)
				{
					if (*arrayCounts[i] > 0)
					{
						array[i] = new BytePattern[*arrayCounts[i]];
					}
				}
				collection.Lut = array;
			}
			BytePattern[] array2 = collection.Lut[(int)(*item.Span[0])];
			int num = Array.IndexOf<BytePattern>(array2, null);
			array2[num] = pattern;
			if (pattern.MinLength < collection.MinLength)
			{
				collection.MinLength = pattern.MinLength;
			}
		}

		[Nullable(1)]
		private readonly BytePatternCollection.HomogenousPatternCollection[] patternCollections;

		[Nullable(new byte[] { 2, 1 })]
		private readonly BytePattern[] emptyPatterns;

		private global::System.ReadOnlyMemory<byte>? lazyPossibleFirstBytes;

		private struct HomogenousPatternCollection
		{
			public HomogenousPatternCollection(int offs)
			{
				this.Offset = offs;
				this.Lut = null;
				this.MinLength = int.MaxValue;
			}

			public void AddFirstBytes(ref BytePatternCollection.FirstByteCollection bytes)
			{
				for (int i = 0; i < this.Lut.Length; i++)
				{
					if (this.Lut[i] != null)
					{
						bytes.Add((byte)i);
					}
				}
			}

			[Nullable(new byte[] { 1, 2, 1 })]
			public BytePattern[][] Lut;

			public readonly int Offset;

			public int MinLength;
		}

		private ref struct FirstByteCollection
		{
			public global::System.ReadOnlySpan<byte> FirstBytes
			{
				get
				{
					return this.firstByteStore.Slice(0, this.firstBytesRecorded);
				}
			}

			public FirstByteCollection(global::System.Span<byte> store)
			{
				this = new BytePatternCollection.FirstByteCollection(store.Slice(0, 256), store.Slice(256, 256));
			}

			public FirstByteCollection(global::System.Span<byte> store, global::System.Span<byte> indicies)
			{
				this.firstByteStore = store;
				this.byteIndicies = indicies;
				this.firstBytesRecorded = 0;
				this.byteIndicies.Fill(byte.MaxValue);
			}

			public unsafe void Add(byte value)
			{
				ref byte ptr = ref this.byteIndicies[(int)value];
				if (ptr == 255)
				{
					ptr = (byte)this.firstBytesRecorded;
					*this.firstByteStore[(int)ptr] = value;
					this.firstBytesRecorded = Math.Min(this.firstBytesRecorded + 1, 256);
				}
			}

			private global::System.Span<byte> firstByteStore;

			private global::System.Span<byte> byteIndicies;

			private int firstBytesRecorded;

			public const int SingleAllocationSize = 512;
		}
	}
}
