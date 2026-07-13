using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MonoMod.Utils;

namespace MonoMod.Core.Utils
{
	internal sealed class BytePattern
	{
		public int AddressBytes { get; }

		public int MinLength { get; }

		public AddressMeaning AddressMeaning { get; }

		public bool MustMatchAtStart { get; }

		[NullableContext(1)]
		public BytePattern(AddressMeaning meaning, params ushort[] pattern)
			: this(meaning, false, pattern.AsMemory<ushort>())
		{
		}

		[NullableContext(1)]
		public BytePattern(AddressMeaning meaning, bool mustMatchAtStart, params ushort[] pattern)
			: this(meaning, mustMatchAtStart, pattern.AsMemory<ushort>())
		{
		}

		public BytePattern(AddressMeaning meaning, global::System.ReadOnlyMemory<ushort> pattern)
			: this(meaning, false, pattern)
		{
		}

		public unsafe BytePattern(AddressMeaning meaning, bool mustMatchAtStart, global::System.ReadOnlyMemory<ushort> pattern)
		{
			this.AddressMeaning = meaning;
			this.MustMatchAtStart = mustMatchAtStart;
			BytePattern.PatternSegment[] array;
			int num;
			int num2;
			BytePattern.ComputeSegmentsFromShort(pattern).Deconstruct(out array, out num, out num2);
			this.segments = array;
			this.MinLength = num;
			this.AddressBytes = num2;
			global::System.Memory<byte> memory = new byte[pattern.Length * 2].AsMemory<byte>();
			global::System.Memory<byte> memory2 = memory.Slice(0, pattern.Length);
			global::System.Memory<byte> memory3 = memory.Slice(pattern.Length);
			for (int i = 0; i < pattern.Length; i++)
			{
				ushort num3 = *pattern.Span[i];
				byte b = (byte)((num3 & 65280) >> 8);
				byte b2 = (byte)((int)num3 & -65281);
				bool flag = b == 0 || b == byte.MaxValue;
				if (flag)
				{
					b = ~b;
				}
				*memory2.Span[i] = b2 & b;
				*memory3.Span[i] = b;
			}
			this.pattern = memory2;
			this.bitmask = memory3;
		}

		public BytePattern(AddressMeaning meaning, global::System.ReadOnlyMemory<byte> mask, global::System.ReadOnlyMemory<byte> pattern)
			: this(meaning, false, mask, pattern)
		{
		}

		public BytePattern(AddressMeaning meaning, bool mustMatchAtStart, global::System.ReadOnlyMemory<byte> mask, global::System.ReadOnlyMemory<byte> pattern)
		{
			this.AddressMeaning = meaning;
			this.MustMatchAtStart = mustMatchAtStart;
			BytePattern.PatternSegment[] array;
			int num;
			int num2;
			BytePattern.ComputeSegmentsFromMaskPattern(mask, pattern).Deconstruct(out array, out num, out num2);
			this.segments = array;
			this.MinLength = num;
			this.AddressBytes = num2;
			this.pattern = pattern;
			this.bitmask = mask;
		}

		private static BytePattern.ComputeSegmentsResult ComputeSegmentsFromShort(global::System.ReadOnlyMemory<ushort> pattern)
		{
			return BytePattern.ComputeSegmentsCore<global::System.ReadOnlyMemory<ushort>>(ldftn(<ComputeSegmentsFromShort>g__KindForShort|31_0), pattern.Length, pattern);
		}

		private static BytePattern.ComputeSegmentsResult ComputeSegmentsFromMaskPattern(global::System.ReadOnlyMemory<byte> mask, global::System.ReadOnlyMemory<byte> pattern)
		{
			if (mask.Length < pattern.Length)
			{
				throw new ArgumentException("Mask buffer shorter than pattern", "mask");
			}
			return BytePattern.ComputeSegmentsCore<ValueTuple<global::System.ReadOnlyMemory<byte>, global::System.ReadOnlyMemory<byte>>>(ldftn(<ComputeSegmentsFromMaskPattern>g__KindForIdx|32_0), pattern.Length, new ValueTuple<global::System.ReadOnlyMemory<byte>, global::System.ReadOnlyMemory<byte>>(mask, pattern));
		}

		[NullableContext(1)]
		private unsafe static BytePattern.ComputeSegmentsResult ComputeSegmentsCore<[Nullable(2)] TPattern>([Nullable(new byte[] { 0, 1 })] delegate*<TPattern, int, BytePattern.SegmentKind> kindForIdx, int patternLength, TPattern pattern)
		{
			if (patternLength == 0)
			{
				throw new ArgumentException("Pattern cannot be empty", "pattern");
			}
			int num = 0;
			BytePattern.SegmentKind segmentKind = BytePattern.SegmentKind.AnyRepeating;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = -1;
			for (int i = 0; i < patternLength; i++)
			{
				BytePattern.SegmentKind segmentKind2 = calli(MonoMod.Core.Utils.BytePattern/SegmentKind(TPattern,System.Int32), pattern, i, kindForIdx);
				int num6 = num4;
				int num7;
				switch (segmentKind2)
				{
				case BytePattern.SegmentKind.Literal:
					num7 = 1;
					break;
				case BytePattern.SegmentKind.MaskedLiteral:
					num7 = 1;
					break;
				case BytePattern.SegmentKind.Any:
					num7 = 1;
					break;
				case BytePattern.SegmentKind.AnyRepeating:
					num7 = 0;
					break;
				case BytePattern.SegmentKind.Address:
					num7 = 1;
					break;
				default:
					num7 = 0;
					break;
				}
				num4 = num6 + num7;
				if (segmentKind2 != segmentKind)
				{
					if (num5 < 0)
					{
						num5 = i;
					}
					num++;
					num2 = 1;
				}
				else
				{
					num2++;
				}
				if (segmentKind2 == BytePattern.SegmentKind.Address)
				{
					num3++;
				}
				segmentKind = segmentKind2;
			}
			if (num > 0 && segmentKind == BytePattern.SegmentKind.AnyRepeating)
			{
				num--;
			}
			if (num == 0 || num4 <= 0)
			{
				throw new ArgumentException("Pattern has no meaningful segments", "pattern");
			}
			BytePattern.PatternSegment[] array = new BytePattern.PatternSegment[num];
			num = 0;
			segmentKind = BytePattern.SegmentKind.AnyRepeating;
			num2 = 0;
			int num8 = num5;
			while (num8 < patternLength && num <= array.Length)
			{
				BytePattern.SegmentKind segmentKind3 = calli(MonoMod.Core.Utils.BytePattern/SegmentKind(TPattern,System.Int32), pattern, num8, kindForIdx);
				if (segmentKind3 != segmentKind)
				{
					if (num > 0)
					{
						array[num - 1] = new BytePattern.PatternSegment(num8 - num2, num2, segmentKind);
						if (num > 1 && segmentKind == BytePattern.SegmentKind.Any && array[num - 2].Kind == BytePattern.SegmentKind.AnyRepeating)
						{
							Helpers.Swap<BytePattern.PatternSegment>(ref array[num - 2], ref array[num - 1]);
						}
					}
					num++;
					num2 = 1;
				}
				else
				{
					num2++;
				}
				segmentKind = segmentKind3;
				num8++;
			}
			if (segmentKind != BytePattern.SegmentKind.AnyRepeating && num > 0)
			{
				array[num - 1] = new BytePattern.PatternSegment(patternLength - num2, num2, segmentKind);
			}
			return new BytePattern.ComputeSegmentsResult(array, num4, num3);
		}

		public unsafe bool TryMatchAt(global::System.ReadOnlySpan<byte> data, out ulong address, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = 0;
				address = 0UL;
				return false;
			}
			global::System.ReadOnlySpan<byte> span = this.pattern.Span;
			global::System.Span<byte> span2 = new global::System.Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			bool flag = this.TryMatchAtImpl(span, data, span2, out length, 0);
			address = Unsafe.ReadUnaligned<ulong>(span2[0]);
			return flag;
		}

		public bool TryMatchAt(global::System.ReadOnlySpan<byte> data, global::System.Span<byte> addrBuf, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = 0;
				return false;
			}
			global::System.ReadOnlySpan<byte> span = this.pattern.Span;
			return this.TryMatchAtImpl(span, data, addrBuf, out length, 0);
		}

		private bool TryMatchAtImpl(global::System.ReadOnlySpan<byte> patternSpan, global::System.ReadOnlySpan<byte> data, global::System.Span<byte> addrBuf, out int length, int startAtSegment)
		{
			int num = 0;
			int i = startAtSegment;
			while (i < this.segments.Length)
			{
				BytePattern.PatternSegment patternSegment = this.segments[i];
				switch (patternSegment.Kind)
				{
				case BytePattern.SegmentKind.Literal:
				{
					if (data.Length - num < patternSegment.Length)
					{
						goto IL_01AA;
					}
					global::System.ReadOnlySpan<byte> readOnlySpan = patternSegment.SliceOf<byte>(patternSpan);
					if (!readOnlySpan.SequenceEqual<byte>(data.Slice(num, readOnlySpan.Length)))
					{
						goto IL_01AA;
					}
					num += patternSegment.Length;
					break;
				}
				case BytePattern.SegmentKind.MaskedLiteral:
				{
					if (data.Length - num < patternSegment.Length)
					{
						goto IL_01AA;
					}
					global::System.ReadOnlySpan<byte> readOnlySpan2 = patternSegment.SliceOf<byte>(patternSpan);
					global::System.ReadOnlySpan<byte> readOnlySpan3 = patternSegment.SliceOf<byte>(this.bitmask.Span);
					if (!Helpers.MaskedSequenceEqual(readOnlySpan2, data.Slice(num, readOnlySpan2.Length), readOnlySpan3))
					{
						goto IL_01AA;
					}
					num += patternSegment.Length;
					break;
				}
				case BytePattern.SegmentKind.Any:
					if (data.Length - num < patternSegment.Length)
					{
						goto IL_01AA;
					}
					num += patternSegment.Length;
					break;
				case BytePattern.SegmentKind.AnyRepeating:
				{
					int num2;
					int num3;
					bool flag = this.ScanForNextLiteral(patternSpan, data.Slice(num), addrBuf, out num2, out num3, i);
					length = num + num2 + num3;
					return flag;
				}
				case BytePattern.SegmentKind.Address:
				{
					if (data.Length - num < patternSegment.Length)
					{
						goto IL_01AA;
					}
					global::System.ReadOnlySpan<byte> readOnlySpan4 = data.Slice(num, Math.Min(patternSegment.Length, addrBuf.Length));
					readOnlySpan4.CopyTo(addrBuf);
					addrBuf = addrBuf.Slice(Math.Min(addrBuf.Length, readOnlySpan4.Length));
					num += patternSegment.Length;
					break;
				}
				default:
					throw new InvalidOperationException();
				}
				i++;
				continue;
				IL_01AA:
				length = 0;
				return false;
			}
			length = num;
			return true;
		}

		public unsafe bool TryFindMatch(global::System.ReadOnlySpan<byte> data, out ulong address, out int offset, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = (offset = 0);
				address = 0UL;
				return false;
			}
			global::System.ReadOnlySpan<byte> span = this.pattern.Span;
			global::System.Span<byte> span2 = new global::System.Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			bool flag;
			if (this.MustMatchAtStart)
			{
				offset = 0;
				flag = this.TryMatchAtImpl(span, data, span2, out length, 0);
			}
			else
			{
				flag = this.ScanForNextLiteral(span, data, span2, out offset, out length, 0);
			}
			address = Unsafe.ReadUnaligned<ulong>(span2[0]);
			return flag;
		}

		public bool TryFindMatch(global::System.ReadOnlySpan<byte> data, global::System.Span<byte> addrBuf, out int offset, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = (offset = 0);
				return false;
			}
			global::System.ReadOnlySpan<byte> span = this.pattern.Span;
			if (this.MustMatchAtStart)
			{
				offset = 0;
				return this.TryMatchAtImpl(span, data, addrBuf, out length, 0);
			}
			return this.ScanForNextLiteral(span, data, addrBuf, out offset, out length, 0);
		}

		private bool ScanForNextLiteral(global::System.ReadOnlySpan<byte> patternSpan, global::System.ReadOnlySpan<byte> data, global::System.Span<byte> addrBuf, out int offset, out int length, int segmentIndex)
		{
			ValueTuple<BytePattern.PatternSegment, int> nextLiteralSegment = this.GetNextLiteralSegment(segmentIndex);
			BytePattern.PatternSegment item = nextLiteralSegment.Item1;
			int item2 = nextLiteralSegment.Item2;
			if (item2 + item.Length > data.Length)
			{
				offset = (length = 0);
				return false;
			}
			int num = 0;
			for (;;)
			{
				int num2 = data.Slice(item2 + num).IndexOf<byte>(item.SliceOf<byte>(patternSpan));
				if (num2 < 0)
				{
					break;
				}
				if (this.TryMatchAtImpl(patternSpan, data.Slice(offset = num + num2), addrBuf, out length, segmentIndex))
				{
					return true;
				}
				num += num2 + 1;
			}
			offset = (length = 0);
			return false;
		}

		[TupleElementNames(new string[] { "Bytes", "Offset" })]
		public ValueTuple<global::System.ReadOnlyMemory<byte>, int> FirstLiteralSegment
		{
			[return: TupleElementNames(new string[] { "Bytes", "Offset" })]
			get
			{
				ValueTuple<global::System.ReadOnlyMemory<byte>, int> valueTuple = this.lazyFirstLiteralSegment.GetValueOrDefault();
				if (this.lazyFirstLiteralSegment == null)
				{
					valueTuple = this.GetFirstLiteralSegment();
					this.lazyFirstLiteralSegment = new ValueTuple<global::System.ReadOnlyMemory<byte>, int>?(valueTuple);
					return valueTuple;
				}
				return valueTuple;
			}
		}

		[return: TupleElementNames(new string[] { "Bytes", "Offset" })]
		private ValueTuple<global::System.ReadOnlyMemory<byte>, int> GetFirstLiteralSegment()
		{
			ValueTuple<BytePattern.PatternSegment, int> nextLiteralSegment = this.GetNextLiteralSegment(0);
			BytePattern.PatternSegment item = nextLiteralSegment.Item1;
			int item2 = nextLiteralSegment.Item2;
			return new ValueTuple<global::System.ReadOnlyMemory<byte>, int>(item.SliceOf<byte>(this.pattern), item2);
		}

		[return: TupleElementNames(new string[] { "Segment", "LiteralOffset" })]
		private ValueTuple<BytePattern.PatternSegment, int> GetNextLiteralSegment(int segmentIndexId)
		{
			if (segmentIndexId < 0 || segmentIndexId >= this.segments.Length)
			{
				throw new ArgumentOutOfRangeException("segmentIndexId");
			}
			int num = 0;
			while (segmentIndexId < this.segments.Length)
			{
				BytePattern.PatternSegment patternSegment = this.segments[segmentIndexId];
				if (patternSegment.Kind == BytePattern.SegmentKind.Literal)
				{
					return new ValueTuple<BytePattern.PatternSegment, int>(patternSegment, num);
				}
				BytePattern.SegmentKind kind = patternSegment.Kind;
				bool flag = kind - BytePattern.SegmentKind.MaskedLiteral <= 1 || kind == BytePattern.SegmentKind.Address;
				if (flag)
				{
					num += patternSegment.Length;
				}
				else if (patternSegment.Kind != BytePattern.SegmentKind.AnyRepeating)
				{
					throw new InvalidOperationException("Unknown segment kind");
				}
				segmentIndexId++;
			}
			return new ValueTuple<BytePattern.PatternSegment, int>(default(BytePattern.PatternSegment), num);
		}

		[CompilerGenerated]
		internal unsafe static BytePattern.SegmentKind <ComputeSegmentsFromShort>g__KindForShort|31_0(global::System.ReadOnlyMemory<ushort> pattern, int idx)
		{
			ushort num = *pattern.Span[idx];
			int num2 = (int)(num & 65280);
			BytePattern.SegmentKind segmentKind;
			if (num2 != 0)
			{
				if (num2 != 65280)
				{
					segmentKind = BytePattern.SegmentKind.MaskedLiteral;
				}
				else
				{
					int num3 = (int)(num & 255);
					BytePattern.SegmentKind segmentKind2;
					switch (num3)
					{
					case 0:
						segmentKind2 = BytePattern.SegmentKind.Any;
						break;
					case 1:
						segmentKind2 = BytePattern.SegmentKind.AnyRepeating;
						break;
					case 2:
						segmentKind2 = BytePattern.SegmentKind.Address;
						break;
					default:
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Pattern contained unknown special value ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num3, "x2");
						throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "pattern");
					}
					}
					segmentKind = segmentKind2;
				}
			}
			else
			{
				segmentKind = BytePattern.SegmentKind.Literal;
			}
			return segmentKind;
		}

		[CompilerGenerated]
		internal unsafe static BytePattern.SegmentKind <ComputeSegmentsFromMaskPattern>g__KindForIdx|32_0([TupleElementNames(new string[] { "mask", "pattern" })] ValueTuple<global::System.ReadOnlyMemory<byte>, global::System.ReadOnlyMemory<byte>> t, int idx)
		{
			byte b = *t.Item1.Span[idx];
			BytePattern.SegmentKind segmentKind;
			if (b != 0)
			{
				if (b != 255)
				{
					segmentKind = BytePattern.SegmentKind.MaskedLiteral;
				}
				else
				{
					segmentKind = BytePattern.SegmentKind.Literal;
				}
			}
			else
			{
				byte b2 = *t.Item2.Span[idx];
				BytePattern.SegmentKind segmentKind2;
				switch (b2)
				{
				case 0:
					segmentKind2 = BytePattern.SegmentKind.Any;
					break;
				case 1:
					segmentKind2 = BytePattern.SegmentKind.AnyRepeating;
					break;
				case 2:
					segmentKind2 = BytePattern.SegmentKind.Address;
					break;
				default:
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Pattern contained unknown special value ");
					defaultInterpolatedStringHandler.AppendFormatted<byte>(b2, "x2");
					throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "pattern");
				}
				}
				segmentKind = segmentKind2;
			}
			return segmentKind;
		}

		private const ushort MaskMask = 65280;

		public const byte BAnyValue = 0;

		public const ushort SAnyValue = 65280;

		public const byte BAnyRepeatingValue = 1;

		public const ushort SAnyRepeatingValue = 65281;

		public const byte BAddressValue = 2;

		public const ushort SAddressValue = 65282;

		private readonly global::System.ReadOnlyMemory<byte> pattern;

		private readonly global::System.ReadOnlyMemory<byte> bitmask;

		[Nullable(1)]
		private readonly BytePattern.PatternSegment[] segments;

		[TupleElementNames(new string[] { "Bytes", "Offset" })]
		private ValueTuple<global::System.ReadOnlyMemory<byte>, int>? lazyFirstLiteralSegment;

		private enum SegmentKind
		{
			Literal,
			MaskedLiteral,
			Any,
			AnyRepeating,
			Address
		}

		private struct PatternSegment : IEquatable<BytePattern.PatternSegment>
		{
			public PatternSegment(int Start, int Length, BytePattern.SegmentKind Kind)
			{
				this.Start = Start;
				this.Length = Length;
				this.Kind = Kind;
			}

			public int Start { readonly get; set; }

			public int Length { readonly get; set; }

			public BytePattern.SegmentKind Kind { readonly get; set; }

			[NullableContext(2)]
			[return: Nullable(new byte[] { 0, 1 })]
			public global::System.ReadOnlySpan<T> SliceOf<T>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> span)
			{
				return span.Slice(this.Start, this.Length);
			}

			[NullableContext(2)]
			[return: Nullable(new byte[] { 0, 1 })]
			public global::System.ReadOnlyMemory<T> SliceOf<T>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> mem)
			{
				return mem.Slice(this.Start, this.Length);
			}

			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("PatternSegment");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Start = ");
				builder.Append(this.Start.ToString());
				builder.Append(", Length = ");
				builder.Append(this.Length.ToString());
				builder.Append(", Kind = ");
				builder.Append(this.Kind.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(BytePattern.PatternSegment left, BytePattern.PatternSegment right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(BytePattern.PatternSegment left, BytePattern.PatternSegment right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<int>.Default.GetHashCode(this.<Start>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<Length>k__BackingField)) * -1521134295 + EqualityComparer<BytePattern.SegmentKind>.Default.GetHashCode(this.<Kind>k__BackingField);
			}

			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is BytePattern.PatternSegment && this.Equals((BytePattern.PatternSegment)obj);
			}

			[CompilerGenerated]
			public readonly bool Equals(BytePattern.PatternSegment other)
			{
				return EqualityComparer<int>.Default.Equals(this.<Start>k__BackingField, other.<Start>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<Length>k__BackingField, other.<Length>k__BackingField) && EqualityComparer<BytePattern.SegmentKind>.Default.Equals(this.<Kind>k__BackingField, other.<Kind>k__BackingField);
			}

			[CompilerGenerated]
			public readonly void Deconstruct(out int Start, out int Length, out BytePattern.SegmentKind Kind)
			{
				Start = this.Start;
				Length = this.Length;
				Kind = this.Kind;
			}
		}

		[NullableContext(1)]
		[Nullable(0)]
		private readonly struct ComputeSegmentsResult : IEquatable<BytePattern.ComputeSegmentsResult>
		{
			public ComputeSegmentsResult(BytePattern.PatternSegment[] Segments, int MinLen, int AddrBytes)
			{
				this.Segments = Segments;
				this.MinLen = MinLen;
				this.AddrBytes = AddrBytes;
			}

			public BytePattern.PatternSegment[] Segments { get; set; }

			public int MinLen { get; set; }

			public int AddrBytes { get; set; }

			[NullableContext(0)]
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("ComputeSegmentsResult");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[NullableContext(0)]
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Segments = ");
				builder.Append(this.Segments);
				builder.Append(", MinLen = ");
				builder.Append(this.MinLen.ToString());
				builder.Append(", AddrBytes = ");
				builder.Append(this.AddrBytes.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(BytePattern.ComputeSegmentsResult left, BytePattern.ComputeSegmentsResult right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(BytePattern.ComputeSegmentsResult left, BytePattern.ComputeSegmentsResult right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<BytePattern.PatternSegment[]>.Default.GetHashCode(this.<Segments>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<MinLen>k__BackingField)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<AddrBytes>k__BackingField);
			}

			[NullableContext(0)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return obj is BytePattern.ComputeSegmentsResult && this.Equals((BytePattern.ComputeSegmentsResult)obj);
			}

			[CompilerGenerated]
			public bool Equals(BytePattern.ComputeSegmentsResult other)
			{
				return EqualityComparer<BytePattern.PatternSegment[]>.Default.Equals(this.<Segments>k__BackingField, other.<Segments>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<MinLen>k__BackingField, other.<MinLen>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<AddrBytes>k__BackingField, other.<AddrBytes>k__BackingField);
			}

			[CompilerGenerated]
			public void Deconstruct(out BytePattern.PatternSegment[] Segments, out int MinLen, out int AddrBytes)
			{
				Segments = this.Segments;
				MinLen = this.MinLen;
				AddrBytes = this.AddrBytes;
			}
		}
	}
}
