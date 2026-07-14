using System;
using System.Buffers;
using System.Buffers.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using VYaml.Internal;

namespace VYaml.Emitter
{
	[NullableContext(1)]
	[Nullable(0)]
	public ref struct Utf8YamlEmitter
	{
		private unsafe EmitState CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ExpandBuffer<EmitState> expandBuffer = this.stateStack;
				return *expandBuffer[expandBuffer.Length - 1];
			}
		}

		private unsafe EmitState PreviousState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ExpandBuffer<EmitState> expandBuffer = this.stateStack;
				return *expandBuffer[expandBuffer.Length - 2];
			}
		}

		private bool IsFirstElement
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.currentElementCount <= 0;
			}
		}

		public Utf8YamlEmitter(IBufferWriter<byte> writer, [Nullable(2)] YamlEmitOptions options = null)
		{
			this.writer = writer;
			this.options = options ?? YamlEmitOptions.Default;
			this.currentIndentLevel = 0;
			ExpandBuffer<char> expandBuffer;
			if ((expandBuffer = Utf8YamlEmitter.stringBufferStatic) == null)
			{
				expandBuffer = (Utf8YamlEmitter.stringBufferStatic = new ExpandBuffer<char>(1024));
			}
			this.stringBuffer = expandBuffer;
			this.stringBuffer.Clear();
			ExpandBuffer<EmitState> expandBuffer2;
			if ((expandBuffer2 = Utf8YamlEmitter.stateBufferStatic) == null)
			{
				expandBuffer2 = (Utf8YamlEmitter.stateBufferStatic = new ExpandBuffer<EmitState>(16));
			}
			this.stateStack = expandBuffer2;
			this.stateStack.Clear();
			ExpandBuffer<int> expandBuffer3;
			if ((expandBuffer3 = Utf8YamlEmitter.elementCountBufferStatic) == null)
			{
				expandBuffer3 = (Utf8YamlEmitter.elementCountBufferStatic = new ExpandBuffer<int>(16));
			}
			this.elementCountStack = expandBuffer3;
			this.elementCountStack.Clear();
			this.stateStack.Add(EmitState.None);
			this.currentElementCount = 0;
			this.tagStack = new ExpandBuffer<string>(4);
		}

		internal readonly IBufferWriter<byte> GetWriter()
		{
			return this.writer;
		}

		public unsafe void BeginSequence(SequenceStyle style = SequenceStyle.Block)
		{
			if (style == SequenceStyle.Block)
			{
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					this.WriteBlockSequenceEntryHeader();
					break;
				case EmitState.BlockMappingKey:
					throw new YamlEmitterException("To start block-sequence in the mapping key is not supported.");
				case EmitState.FlowSequenceEntry:
					throw new YamlEmitterException("To start block-sequence in the flow-sequence is not supported.");
				}
				this.PushState(EmitState.BlockSequenceEntry);
				return;
			}
			if (style != SequenceStyle.Flow)
			{
				throw new ArgumentOutOfRangeException("style", style, null);
			}
			switch (this.CurrentState)
			{
			case EmitState.BlockSequenceEntry:
			{
				Span<byte> span = this.writer.GetSpan(this.currentIndentLevel * this.options.IndentWidth + 3);
				int num = 0;
				this.WriteBlockSequenceEntryHeader(span, ref num);
				*span[num++] = 32;
				*span[num++] = 91;
				this.writer.Advance(num);
				goto IL_01BA;
			}
			case EmitState.BlockMappingKey:
				throw new YamlEmitterException("To start flow-sequence in the mapping key is not supported.");
			case EmitState.FlowSequenceEntry:
			{
				Span<byte> span2 = this.writer.GetSpan(Utf8YamlEmitter.FlowSequenceSeparator.Length + 1);
				int num2 = 0;
				if (this.currentElementCount > 0)
				{
					Utf8YamlEmitter.FlowSequenceSeparator.CopyTo<byte>(span2);
					num2 += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				}
				*span2[num2++] = 91;
				this.writer.Advance(num2);
				goto IL_01BA;
			}
			}
			Span<byte> span3 = this.writer.GetSpan(this.GetTagLength() + 3);
			int num3 = 0;
			EmitState currentState = this.CurrentState;
			bool flag = currentState == EmitState.BlockMappingValue || currentState == EmitState.FlowMappingValue;
			if (flag)
			{
				*span3[num3++] = 32;
			}
			if (this.TryWriteTag(span3, false, ref num3))
			{
				*span3[num3++] = 32;
			}
			*span3[num3++] = 91;
			this.writer.Advance(num3);
			IL_01BA:
			this.PushState(EmitState.FlowSequenceEntry);
		}

		public unsafe void EndSequence()
		{
			EmitState currentState = this.CurrentState;
			if (currentState != EmitState.BlockSequenceEntry)
			{
				if (currentState != EmitState.FlowSequenceEntry)
				{
					throw new YamlEmitterException(string.Format("Current state is not sequence: {0}", this.CurrentState));
				}
				this.PopState();
				bool flag = false;
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					flag = true;
					this.currentElementCount++;
					break;
				case EmitState.BlockMappingValue:
					this.ReplaceCurrentState(EmitState.BlockMappingKey);
					flag = true;
					this.currentElementCount++;
					break;
				case EmitState.FlowSequenceEntry:
					this.currentElementCount++;
					break;
				case EmitState.FlowMappingValue:
					this.ReplaceCurrentState(EmitState.FlowMappingKey);
					this.currentElementCount++;
					break;
				}
				int num = 1;
				if (flag)
				{
					num++;
				}
				int num2 = 0;
				Span<byte> span = this.writer.GetSpan(num);
				*span[num2++] = 93;
				if (flag)
				{
					*span[num2++] = 10;
				}
				this.writer.Advance(num2);
				return;
			}
			else
			{
				bool flag2 = this.currentElementCount <= 0;
				this.PopState();
				if (flag2)
				{
					EmitState emitState = this.CurrentState;
					bool flag3 = emitState == EmitState.BlockSequenceEntry || emitState == EmitState.BlockMappingValue;
					bool flag4 = flag3;
					emitState = this.CurrentState;
					flag3 = emitState == EmitState.BlockSequenceEntry || emitState == EmitState.BlockMappingValue || emitState == EmitState.FlowMappingValue;
					this.WriteRaw(flag3 ? Utf8YamlEmitter.FlowSequenceEmptyWithSpace : Utf8YamlEmitter.FlowSequenceEmpty, false, flag4);
				}
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					if (!flag2)
					{
						this.DecreaseIndent();
					}
					this.currentElementCount++;
					return;
				case EmitState.BlockMappingKey:
					throw new YamlEmitterException("Complex key is not supported.");
				case EmitState.BlockMappingValue:
					this.ReplaceCurrentState(EmitState.BlockMappingKey);
					this.currentElementCount++;
					return;
				case EmitState.FlowSequenceEntry:
					this.currentElementCount++;
					return;
				default:
					return;
				}
			}
		}

		public unsafe void BeginMapping(MappingStyle style = MappingStyle.Block)
		{
			if (style == MappingStyle.Block)
			{
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					this.WriteBlockSequenceEntryHeader();
					break;
				case EmitState.BlockMappingKey:
					throw new YamlEmitterException("To start block-mapping in the mapping key is not supported.");
				case EmitState.FlowSequenceEntry:
					throw new YamlEmitterException("Cannot start block-mapping in the flow-sequence");
				}
				this.PushState(EmitState.BlockMappingKey);
				return;
			}
			if (style != MappingStyle.Flow)
			{
				throw new ArgumentOutOfRangeException("style", style, null);
			}
			switch (this.CurrentState)
			{
			case EmitState.BlockSequenceEntry:
			{
				Span<byte> span = this.writer.GetSpan(this.currentIndentLevel * this.options.IndentWidth + 3 + Utf8YamlEmitter.FlowMappingHeader.Length + this.GetTagLength() + 1);
				int num = 0;
				this.WriteBlockSequenceEntryHeader(span, ref num);
				*span[num++] = 32;
				if (this.TryWriteTag(span, false, ref num))
				{
					*span[num++] = 32;
				}
				*span[num++] = 123;
				this.writer.Advance(num);
				goto IL_01EE;
			}
			case EmitState.BlockMappingKey:
				throw new YamlEmitterException("To start flow-mapping in the mapping key is not supported.");
			case EmitState.FlowSequenceEntry:
			{
				Span<byte> span2 = this.writer.GetSpan(Utf8YamlEmitter.FlowSequenceSeparator.Length + Utf8YamlEmitter.FlowMappingHeader.Length);
				int num2 = 0;
				if (!this.IsFirstElement)
				{
					Utf8YamlEmitter.FlowSequenceSeparator.CopyTo<byte>(span2);
					num2 += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				}
				*span2[num2++] = 123;
				this.writer.Advance(num2);
				goto IL_01EE;
			}
			}
			Span<byte> span3 = this.writer.GetSpan(this.GetTagLength() + 2);
			int num3 = 0;
			EmitState currentState = this.CurrentState;
			bool flag = currentState == EmitState.BlockSequenceEntry || currentState == EmitState.BlockMappingValue || currentState == EmitState.FlowMappingValue;
			if (flag)
			{
				*span3[num3++] = 32;
			}
			if (this.TryWriteTag(span3, false, ref num3))
			{
				*span3[num3++] = 32;
			}
			*span3[num3++] = 123;
			this.writer.Advance(num3);
			IL_01EE:
			this.PushState(EmitState.FlowMappingKey);
		}

		public unsafe void EndMapping()
		{
			EmitState currentState = this.CurrentState;
			if (currentState != EmitState.BlockMappingKey)
			{
				if (currentState != EmitState.FlowMappingKey)
				{
					throw new YamlEmitterException(string.Format("Invalid mapping end: {0}", this.CurrentState));
				}
				bool flag = this.currentElementCount <= 0;
				this.PopState();
				bool flag2 = false;
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					flag2 = true;
					this.currentElementCount++;
					break;
				case EmitState.BlockMappingValue:
					this.ReplaceCurrentState(EmitState.BlockMappingKey);
					flag2 = true;
					this.currentElementCount++;
					break;
				case EmitState.FlowSequenceEntry:
					this.currentElementCount++;
					break;
				case EmitState.FlowMappingValue:
					this.ReplaceCurrentState(EmitState.FlowMappingKey);
					this.currentElementCount++;
					break;
				}
				int num = Utf8YamlEmitter.FlowMappingFooter.Length;
				if (flag2)
				{
					num++;
				}
				int num2 = 0;
				Span<byte> span = this.writer.GetSpan(num);
				if (!flag)
				{
					*span[num2++] = 32;
				}
				*span[num2++] = 125;
				if (flag2)
				{
					*span[num2++] = 10;
				}
				this.writer.Advance(num2);
				return;
			}
			else
			{
				bool flag3 = this.currentElementCount <= 0;
				this.PopState();
				EmitState emitState;
				if (flag3)
				{
					emitState = this.CurrentState;
					bool flag4 = emitState == EmitState.BlockSequenceEntry || emitState == EmitState.BlockMappingValue;
					bool flag5 = flag4;
					emitState = this.CurrentState;
					flag4 = emitState == EmitState.BlockSequenceEntry || emitState == EmitState.BlockMappingValue || emitState == EmitState.FlowMappingValue;
					bool flag6 = flag4;
					string text;
					if (this.tagStack.TryPop(out text))
					{
						byte[] bytes = StringEncoding.Utf8.GetBytes((flag6 ? " " : "") + text + " ");
						this.WriteRaw(bytes, Utf8YamlEmitter.FlowMappingEmpty, false, flag5);
					}
					else
					{
						this.WriteRaw(flag6 ? Utf8YamlEmitter.FlowMappingEmptyWithSpace : Utf8YamlEmitter.FlowMappingEmpty, false, flag5);
					}
				}
				emitState = this.CurrentState;
				if (emitState == EmitState.BlockSequenceEntry)
				{
					if (!flag3)
					{
						this.DecreaseIndent();
					}
					this.currentElementCount++;
					return;
				}
				if (emitState != EmitState.BlockMappingValue)
				{
					return;
				}
				if (!flag3)
				{
					this.DecreaseIndent();
				}
				this.ReplaceCurrentState(EmitState.BlockMappingKey);
				this.currentElementCount++;
				return;
			}
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteRaw(ReadOnlySpan<byte> value, bool indent, bool lineBreak)
		{
			int num = value.Length + (indent ? (this.currentIndentLevel * this.options.IndentWidth) : 0) + ((lineBreak > false) ? 1 : 0);
			int num2 = 0;
			Span<byte> span = this.writer.GetSpan(num);
			if (indent)
			{
				this.WriteIndent(span, ref num2, -1);
			}
			ref Span<byte> ptr = ref span;
			int num3 = num2;
			value.CopyTo(ptr.Slice(num3, ptr.Length - num3));
			if (lineBreak)
			{
				*span[num - 1] = 10;
			}
			this.writer.Advance(num);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteRaw(ReadOnlySpan<byte> value1, ReadOnlySpan<byte> value2, bool indent, bool lineBreak)
		{
			int num = value1.Length + value2.Length + (indent ? (this.currentIndentLevel * this.options.IndentWidth) : 0) + ((lineBreak > false) ? 1 : 0);
			int num2 = 0;
			Span<byte> span = this.writer.GetSpan(num);
			if (indent)
			{
				this.WriteIndent(span, ref num2, -1);
			}
			ref Span<byte> ptr = ref span;
			int num3 = num2;
			value1.CopyTo(ptr.Slice(num3, ptr.Length - num3));
			num2 += value1.Length;
			ptr = ref span;
			num3 = num2;
			value2.CopyTo(ptr.Slice(num3, ptr.Length - num3));
			if (lineBreak)
			{
				*span[num - 1] = 10;
			}
			this.writer.Advance(num);
		}

		public void Tag(string value)
		{
			this.tagStack.Add(value);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteScalar(ReadOnlySpan<byte> value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(value.Length));
			this.BeginScalar(span, ref num);
			ref Span<byte> ptr = ref span;
			int num2 = num;
			value.CopyTo(ptr.Slice(num2, ptr.Length - num2));
			num += value.Length;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteNull()
		{
			this.WriteScalar(YamlCodes.Null0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteBool(bool value)
		{
			this.WriteScalar(value ? YamlCodes.True0 : YamlCodes.False0);
		}

		public void WriteInt32(int value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(11));
			this.BeginScalar(span, ref num);
			ref Span<byte> ptr = ref span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, ptr.Slice(num2, ptr.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteUInt32(uint value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(10));
			this.BeginScalar(span, ref num);
			ref Span<byte> ptr = ref span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, ptr.Slice(num2, ptr.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteInt64(long value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(20));
			this.BeginScalar(span, ref num);
			ref Span<byte> ptr = ref span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, ptr.Slice(num2, ptr.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteUInt64(ulong value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(20));
			this.BeginScalar(span, ref num);
			ref Span<byte> ptr = ref span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, ptr.Slice(num2, ptr.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteFloat(float value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(12));
			this.BeginScalar(span, ref num);
			ref Span<byte> ptr = ref span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, ptr.Slice(num2, ptr.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteDouble(double value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(17));
			this.BeginScalar(span, ref num);
			ref Span<byte> ptr = ref span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, ptr.Slice(num2, ptr.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteString(string value, ScalarStyle style = ScalarStyle.Any)
		{
			this.WriteString(value.AsSpan(), style);
		}

		[NullableContext(0)]
		public unsafe void WriteString(char* value, int length, ScalarStyle style = ScalarStyle.Any)
		{
			ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>((void*)value, length);
			this.WriteString(readOnlySpan, style);
		}

		[NullableContext(0)]
		public void WriteString(ReadOnlySpan<char> value, ScalarStyle style = ScalarStyle.Any)
		{
			if (style == ScalarStyle.Any)
			{
				style = EmitStringAnalyzer.Analyze(value).SuggestScalarStyle(this.options);
			}
			switch (style)
			{
			case ScalarStyle.Plain:
				this.WritePlainScalar(value);
				return;
			case ScalarStyle.SingleQuoted:
				this.WriteQuotedScalar(value, false);
				return;
			case ScalarStyle.DoubleQuoted:
				this.WriteQuotedScalar(value, true);
				return;
			case ScalarStyle.Literal:
				this.WriteLiteralScalar(value);
				return;
			case ScalarStyle.Folded:
				throw new NotSupportedException();
			default:
				throw new ArgumentOutOfRangeException("style", style, null);
			}
		}

		[NullableContext(0)]
		private void WritePlainScalar(ReadOnlySpan<char> value)
		{
			int maxByteCount = StringEncoding.Utf8.GetMaxByteCount(value.Length);
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(maxByteCount));
			int num = 0;
			this.BeginScalar(span, ref num);
			int num2 = num;
			Encoding utf = StringEncoding.Utf8;
			ReadOnlySpan<char> readOnlySpan = value;
			ref Span<byte> ptr = ref span;
			int num3 = num;
			num = num2 + utf.GetBytes(readOnlySpan, ptr.Slice(num3, ptr.Length - num3));
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		[NullableContext(0)]
		private void WriteLiteralScalar(ReadOnlySpan<char> value)
		{
			int num = (this.currentIndentLevel + 1) * this.options.IndentWidth;
			StringBuilder stringBuilder = EmitStringAnalyzer.BuildLiteralScalar(value, num);
			Span<char> span = this.stringBuffer.AsSpan(stringBuilder.Length);
			stringBuilder.CopyTo(0, span, stringBuilder.Length);
			EmitState currentState = this.CurrentState;
			bool flag = currentState == EmitState.BlockSequenceEntry || currentState == EmitState.BlockMappingValue;
			if (flag)
			{
				ref Span<char> ptr = ref span;
				span = ptr.Slice(0, ptr.Length - 1);
			}
			int maxByteCount = StringEncoding.Utf8.GetMaxByteCount(span.Length);
			int num2 = 0;
			Span<byte> span2 = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(maxByteCount));
			this.BeginScalar(span2, ref num2);
			int num3 = num2;
			Encoding utf = StringEncoding.Utf8;
			ReadOnlySpan<char> readOnlySpan = span;
			ref Span<byte> ptr2 = ref span2;
			int num4 = num2;
			num2 = num3 + utf.GetBytes(readOnlySpan, ptr2.Slice(num4, ptr2.Length - num4));
			this.EndScalar(span2, ref num2);
			this.writer.Advance(num2);
		}

		[NullableContext(0)]
		private void WriteQuotedScalar(ReadOnlySpan<char> value, bool doubleQuote = true)
		{
			StringBuilder stringBuilder = EmitStringAnalyzer.BuildQuotedScalar(value, doubleQuote);
			Span<char> span = this.stringBuffer.AsSpan(stringBuilder.Length);
			stringBuilder.CopyTo(0, span, stringBuilder.Length);
			int maxByteCount = StringEncoding.Utf8.GetMaxByteCount(span.Length);
			int num = 0;
			Span<byte> span2 = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(maxByteCount));
			this.BeginScalar(span2, ref num);
			int num2 = num;
			Encoding utf = StringEncoding.Utf8;
			ReadOnlySpan<char> readOnlySpan = span;
			ref Span<byte> ptr = ref span2;
			int num3 = num;
			num = num2 + utf.GetBytes(readOnlySpan, ptr.Slice(num3, ptr.Length - num3));
			this.EndScalar(span2, ref num);
			this.writer.Advance(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteRaw1(byte value)
		{
			*this.writer.GetSpan(1)[0] = value;
			this.writer.Advance(1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void WriteBlockSequenceEntryHeader()
		{
			Span<byte> span = this.writer.GetSpan(2 + this.currentIndentLevel * this.options.IndentWidth + 2);
			int num = 0;
			this.WriteBlockSequenceEntryHeader(span, ref num);
			this.writer.Advance(num);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteBlockSequenceEntryHeader(Span<byte> output, ref int offset)
		{
			int num;
			if (this.IsFirstElement)
			{
				EmitState previousState = this.PreviousState;
				if (previousState != EmitState.BlockSequenceEntry)
				{
					if (previousState == EmitState.BlockMappingValue)
					{
						num = offset;
						offset = num + 1;
						*output[num] = 10;
					}
				}
				else
				{
					num = offset;
					offset = num + 1;
					*output[num] = 10;
					this.IncreaseIndent();
				}
			}
			this.WriteIndent(output, ref offset, -1);
			num = offset;
			offset = num + 1;
			*output[num] = 45;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void WriteIndent(Span<byte> output, ref int offset, int forceWidth = -1)
		{
			int num;
			if (forceWidth > -1)
			{
				if (forceWidth <= 0)
				{
					return;
				}
				num = forceWidth;
			}
			else
			{
				if (this.currentIndentLevel <= 0)
				{
					return;
				}
				num = this.currentIndentLevel * this.options.IndentWidth;
			}
			if (num > Utf8YamlEmitter.whiteSpaces.Length)
			{
				Utf8YamlEmitter.whiteSpaces = Enumerable.Repeat<byte>(32, num * 2).ToArray<byte>();
			}
			Span<byte> span = Utf8YamlEmitter.whiteSpaces.AsSpan<byte>(0, num);
			ref Span<byte> ptr = ref output;
			int num2 = offset;
			span.CopyTo(ptr.Slice(num2, ptr.Length - num2));
			offset += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int CalculateMaxScalarBufferLength(int length)
		{
			return length + (this.currentIndentLevel + 1) * this.options.IndentWidth + 3 + this.GetTagLength();
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void BeginScalar(Span<byte> output, ref int offset)
		{
			switch (this.CurrentState)
			{
			case EmitState.None:
				if (this.TryWriteTag(output, false, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			case EmitState.BlockSequenceEntry:
			{
				this.WriteBlockSequenceEntryHeader(output, ref offset);
				int num = offset;
				offset = num + 1;
				*output[num] = 32;
				if (this.TryWriteTag(output, false, ref offset))
				{
					num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			}
			case EmitState.BlockMappingKey:
			{
				if (!this.IsFirstElement)
				{
					this.WriteIndent(output, ref offset, -1);
					return;
				}
				EmitState previousState = this.PreviousState;
				if (previousState != EmitState.BlockSequenceEntry)
				{
					if (previousState != EmitState.BlockMappingValue)
					{
						this.WriteIndent(output, ref offset, -1);
					}
					else
					{
						this.IncreaseIndent();
						this.TryWriteTag(output, true, ref offset);
						int num = offset;
						offset = num + 1;
						*output[num] = 10;
						this.WriteIndent(output, ref offset, -1);
					}
				}
				else
				{
					this.IncreaseIndent();
					int num = offset;
					offset = num + 1;
					*output[num] = 32;
					string text;
					if (this.tagStack.TryPop(out text))
					{
						int num2 = offset;
						Encoding utf = StringEncoding.Utf8;
						ReadOnlySpan<char> readOnlySpan = text;
						ref Span<byte> ptr = ref output;
						num = offset;
						offset = num2 + utf.GetBytes(readOnlySpan, ptr.Slice(num, ptr.Length - num));
						num = offset;
						offset = num + 1;
						*output[num] = 10;
						this.WriteIndent(output, ref offset, -1);
					}
					else
					{
						this.WriteIndent(output, ref offset, this.options.IndentWidth - 2);
					}
				}
				if (this.TryWriteTag(output, false, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 10;
					this.WriteIndent(output, ref offset, -1);
					return;
				}
				break;
			}
			case EmitState.BlockMappingValue:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 32;
				if (this.TryWriteTag(output, false, ref offset))
				{
					num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			}
			case EmitState.FlowSequenceEntry:
				if (!this.IsFirstElement)
				{
					byte[] flowSequenceSeparator = Utf8YamlEmitter.FlowSequenceSeparator;
					ref Span<byte> ptr = ref output;
					int num = offset;
					flowSequenceSeparator.CopyTo<byte>(ptr.Slice(num, ptr.Length - num));
					offset += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				}
				if (this.TryWriteTag(output, false, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			case EmitState.FlowMappingKey:
			{
				int num;
				if (this.IsFirstElement)
				{
					num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				byte[] flowSequenceSeparator2 = Utf8YamlEmitter.FlowSequenceSeparator;
				ref Span<byte> ptr = ref output;
				num = offset;
				flowSequenceSeparator2.CopyTo<byte>(ptr.Slice(num, ptr.Length - num));
				offset += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				return;
			}
			case EmitState.FlowMappingValue:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 32;
				if (this.TryWriteTag(output, false, ref offset))
				{
					num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void EndScalar(Span<byte> output, ref int offset)
		{
			switch (this.CurrentState)
			{
			case EmitState.None:
				return;
			case EmitState.BlockSequenceEntry:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 10;
				this.currentElementCount++;
				return;
			}
			case EmitState.BlockMappingKey:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 58;
				this.ReplaceCurrentState(EmitState.BlockMappingValue);
				return;
			}
			case EmitState.BlockMappingValue:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 10;
				this.ReplaceCurrentState(EmitState.BlockMappingKey);
				this.currentElementCount++;
				return;
			}
			case EmitState.FlowSequenceEntry:
				this.currentElementCount++;
				return;
			case EmitState.FlowMappingKey:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 58;
				this.ReplaceCurrentState(EmitState.FlowMappingValue);
				return;
			}
			case EmitState.FlowMappingValue:
				this.ReplaceCurrentState(EmitState.FlowMappingKey);
				this.currentElementCount++;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void ReplaceCurrentState(EmitState newState)
		{
			ExpandBuffer<EmitState> expandBuffer = this.stateStack;
			*expandBuffer[expandBuffer.Length - 1] = newState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PushState(EmitState state)
		{
			this.stateStack.Add(state);
			this.elementCountStack.Add(this.currentElementCount);
			this.currentElementCount = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void PopState()
		{
			this.stateStack.Pop();
			this.currentElementCount = ((this.elementCountStack.Length > 0) ? (*this.elementCountStack.Pop()) : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void IncreaseIndent()
		{
			this.currentIndentLevel++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DecreaseIndent()
		{
			if (this.currentIndentLevel > 0)
			{
				this.currentIndentLevel--;
			}
		}

		[NullableContext(0)]
		private unsafe bool TryWriteTag(Span<byte> output, bool beforeSpace, ref int offset)
		{
			string text;
			if (this.tagStack.TryPop(out text))
			{
				int num;
				if (beforeSpace)
				{
					num = offset;
					offset = num + 1;
					*output[num] = 32;
				}
				int num2 = offset;
				Encoding utf = StringEncoding.Utf8;
				ReadOnlySpan<char> readOnlySpan = text;
				ref Span<byte> ptr = ref output;
				num = offset;
				offset = num2 + utf.GetBytes(readOnlySpan, ptr.Slice(num, ptr.Length - num));
				return true;
			}
			return false;
		}

		private unsafe int GetTagLength()
		{
			if (this.tagStack.Length <= 0)
			{
				return 0;
			}
			return StringEncoding.Utf8.GetMaxByteCount(this.tagStack.Peek()->Length);
		}

		private static byte[] whiteSpaces = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.F7180B7CF063FC7F94D3087EAA358729AABA1C4B9BA1012470B09A7D1D3A65BA), 32).ToArray();

		private static readonly byte[] FlowSequenceEmpty = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.9EE588BA2521E5A7D1EA261EA4CCAB4AD9B9724A8569CC2A32F812B985531E81), 2).ToArray();

		private static readonly byte[] FlowSequenceEmptyWithSpace = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.2B5C500DE592E35642236F8701C810F6A9513942115CCF0C0120972A2A216F9C), 3).ToArray();

		private static readonly byte[] FlowSequenceSeparator = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.3D757FF595408954CEC727B0D878225712CE19AEC0114F97F903407756702D14), 2).ToArray();

		private static readonly byte[] FlowMappingHeader = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.687B36781138401444E59B70564CAE17948F6E70413E319B97FDBAF7FAFC59FA), 2).ToArray();

		private static readonly byte[] FlowMappingFooter = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.4C398094BA7483A4945A202931E7DBBA38B5A7D626771A2B65A841C714556E80), 2).ToArray();

		private static readonly byte[] FlowMappingEmpty = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.68E9E86B6926CC2B37DF96B5E61BB8CABDAB276272BB73F6767E420C3EAD0663), 2).ToArray();

		private static readonly byte[] FlowMappingEmptyWithSpace = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.F5605E85B7F710D5D0053C09FE179FEF184713CA3573F7CCB6A2D9019AD9E747), 3).ToArray();

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<char> stringBufferStatic;

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<EmitState> stateBufferStatic;

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<int> elementCountBufferStatic;

		private readonly IBufferWriter<byte> writer;

		private readonly YamlEmitOptions options;

		private readonly ExpandBuffer<char> stringBuffer;

		private readonly ExpandBuffer<EmitState> stateStack;

		private readonly ExpandBuffer<int> elementCountStack;

		private readonly ExpandBuffer<string> tagStack;

		private int currentIndentLevel;

		private int currentElementCount;
	}
}
