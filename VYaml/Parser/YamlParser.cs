using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Internal;

namespace VYaml.Parser
{
	[NullableContext(2)]
	[Nullable(0)]
	public ref struct YamlParser
	{
		[NullableContext(0)]
		public static YamlParser FromBytes(Memory<byte> bytes)
		{
			return new YamlParser(new ReadOnlySequence<byte>(bytes));
		}

		[NullableContext(0)]
		public static YamlParser FromSequence(in ReadOnlySequence<byte> sequence)
		{
			return new YamlParser(sequence);
		}

		public ParseEventType CurrentEventType { readonly get; private set; }

		public bool UnityStrippedMark { readonly get; private set; }

		public readonly Marker CurrentMark
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Utf8YamlTokenizer utf8YamlTokenizer = this.tokenizer;
				return utf8YamlTokenizer.CurrentMark;
			}
		}

		public bool End
		{
			get
			{
				return this.CurrentEventType == ParseEventType.StreamEnd;
			}
		}

		private TokenType CurrentTokenType
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.tokenizer.CurrentTokenType;
			}
		}

		[NullableContext(0)]
		public YamlParser(ReadOnlySequence<byte> sequence)
		{
			this.tokenizer = new Utf8YamlTokenizer(sequence);
			this.currentState = ParseState.StreamStart;
			this.CurrentEventType = ParseEventType.Nothing;
			this.lastAnchorId = -1;
			Dictionary<string, int> dictionary;
			if ((dictionary = YamlParser.anchorsBufferStatic) == null)
			{
				dictionary = (YamlParser.anchorsBufferStatic = new Dictionary<string, int>());
			}
			this.anchors = dictionary;
			this.anchors.Clear();
			ExpandBuffer<ParseState> expandBuffer;
			if ((expandBuffer = YamlParser.stateStackBufferStatic) == null)
			{
				expandBuffer = (YamlParser.stateStackBufferStatic = new ExpandBuffer<ParseState>(16));
			}
			this.stateStack = expandBuffer;
			this.stateStack.Clear();
			this.currentScalar = null;
			this.currentTag = null;
			this.currentAnchor = null;
			this.UnityStrippedMark = false;
		}

		public YamlParser(ref Utf8YamlTokenizer tokenizer)
		{
			this.tokenizer = tokenizer;
			this.currentState = ParseState.StreamStart;
			this.CurrentEventType = ParseEventType.Nothing;
			this.lastAnchorId = -1;
			this.anchors = new Dictionary<string, int>();
			this.stateStack = new ExpandBuffer<ParseState>(16);
			this.currentScalar = null;
			this.currentTag = null;
			this.currentAnchor = null;
			this.UnityStrippedMark = false;
		}

		public bool Read()
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				ScalarPool.Shared.Return(scalar);
				this.currentScalar = null;
			}
			if (this.currentState == ParseState.End)
			{
				this.CurrentEventType = ParseEventType.StreamEnd;
				return false;
			}
			switch (this.currentState)
			{
			case ParseState.StreamStart:
				this.ParseStreamStart();
				return true;
			case ParseState.ImplicitDocumentStart:
				this.ParseDocumentStart(true);
				return true;
			case ParseState.DocumentStart:
				this.ParseDocumentStart(false);
				return true;
			case ParseState.DocumentContent:
				this.ParseDocumentContent();
				return true;
			case ParseState.DocumentEnd:
				this.ParseDocumentEnd();
				return true;
			case ParseState.BlockNode:
				this.ParseNode(true, false);
				return true;
			case ParseState.BlockSequenceFirstEntry:
				this.ParseBlockSequenceEntry(true);
				return true;
			case ParseState.BlockSequenceEntry:
				this.ParseBlockSequenceEntry(false);
				return true;
			case ParseState.IndentlessSequenceEntry:
				this.ParseIndentlessSequenceEntry();
				return true;
			case ParseState.BlockMappingFirstKey:
				this.ParseBlockMappingKey(true);
				return true;
			case ParseState.BlockMappingKey:
				this.ParseBlockMappingKey(false);
				return true;
			case ParseState.BlockMappingValue:
				this.ParseBlockMappingValue();
				return true;
			case ParseState.FlowSequenceFirstEntry:
				this.ParseFlowSequenceEntry(true);
				return true;
			case ParseState.FlowSequenceEntry:
				this.ParseFlowSequenceEntry(false);
				return true;
			case ParseState.FlowSequenceEntryMappingKey:
				this.ParseFlowSequenceEntryMappingKey();
				return true;
			case ParseState.FlowSequenceEntryMappingValue:
				this.ParseFlowSequenceEntryMappingValue();
				return true;
			case ParseState.FlowSequenceEntryMappingEnd:
				this.ParseFlowSequenceEntryMappingEnd();
				return true;
			case ParseState.FlowMappingFirstKey:
				this.ParseFlowMappingKey(true);
				return true;
			case ParseState.FlowMappingKey:
				this.ParseFlowMappingKey(false);
				return true;
			case ParseState.FlowMappingValue:
				this.ParseFlowMappingValue(false);
				return true;
			case ParseState.FlowMappingEmptyValue:
				this.ParseFlowMappingValue(true);
				return true;
			}
			throw new ArgumentOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadWithVerify(ParseEventType eventType)
		{
			if (this.CurrentEventType != eventType)
			{
				Marker currentMark = this.CurrentMark;
				throw new YamlParserException(in currentMark, string.Format("Did not find expected event : `{0}`", eventType));
			}
			this.Read();
		}

		public void SkipHeader()
		{
			bool flag;
			do
			{
				ParseEventType currentEventType = this.CurrentEventType;
				flag = currentEventType <= ParseEventType.StreamStart || currentEventType == ParseEventType.DocumentStart;
			}
			while (flag && this.Read());
		}

		public void SkipAfter(ParseEventType eventType)
		{
			while (this.CurrentEventType != eventType && this.Read())
			{
			}
			if (this.CurrentEventType == eventType)
			{
				this.Read();
			}
		}

		public void SkipCurrentNode()
		{
			switch (this.CurrentEventType)
			{
			case ParseEventType.Alias:
			case ParseEventType.Scalar:
				this.Read();
				return;
			case ParseEventType.SequenceStart:
			{
				int num = 1;
				while (this.Read())
				{
					ParseEventType parseEventType = this.CurrentEventType;
					if (parseEventType != ParseEventType.SequenceStart)
					{
						if (parseEventType == ParseEventType.SequenceEnd)
						{
							if (--num <= 0)
							{
								this.Read();
								return;
							}
						}
					}
					else
					{
						num++;
					}
				}
				return;
			}
			case ParseEventType.MappingStart:
			{
				int num2 = 1;
				while (this.Read())
				{
					ParseEventType parseEventType = this.CurrentEventType;
					if (parseEventType != ParseEventType.MappingStart)
					{
						if (parseEventType == ParseEventType.MappingEnd)
						{
							if (--num2 <= 0)
							{
								this.Read();
								return;
							}
						}
					}
					else
					{
						num2++;
					}
				}
				return;
			}
			}
			throw new ArgumentOutOfRangeException();
		}

		private void ParseStreamStart()
		{
			if (this.CurrentTokenType == TokenType.None)
			{
				this.tokenizer.Read();
			}
			this.ThrowIfCurrentTokenUnless(TokenType.StreamStart);
			this.currentState = ParseState.ImplicitDocumentStart;
			this.tokenizer.Read();
			this.CurrentEventType = ParseEventType.StreamStart;
		}

		private void ParseDocumentStart(bool implicitStarted)
		{
			if (!implicitStarted)
			{
				while (this.tokenizer.CurrentTokenType == TokenType.DocumentEnd)
				{
					this.tokenizer.Read();
				}
			}
			TokenType currentTokenType = this.tokenizer.CurrentTokenType;
			if (currentTokenType == TokenType.StreamEnd)
			{
				this.currentState = ParseState.End;
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.StreamEnd;
				return;
			}
			if (currentTokenType - TokenType.VersionDirective <= 2)
			{
				this.ParseExplicitDocumentStart();
				return;
			}
			if (implicitStarted)
			{
				this.ProcessDirectives();
				this.PushState(ParseState.DocumentEnd);
				this.currentState = ParseState.BlockNode;
				this.CurrentEventType = ParseEventType.DocumentStart;
				return;
			}
			this.ParseExplicitDocumentStart();
		}

		private void ParseExplicitDocumentStart()
		{
			this.ProcessDirectives();
			this.ThrowIfCurrentTokenUnless(TokenType.DocumentStart);
			this.PushState(ParseState.DocumentEnd);
			this.currentState = ParseState.DocumentContent;
			this.tokenizer.Read();
			this.CurrentEventType = ParseEventType.DocumentStart;
		}

		private void ParseDocumentContent()
		{
			TokenType currentTokenType = this.tokenizer.CurrentTokenType;
			if (currentTokenType - TokenType.StreamEnd <= 4)
			{
				this.PopState();
				this.EmptyScalar();
				return;
			}
			this.ParseNode(true, false);
		}

		private void ParseDocumentEnd()
		{
			if (this.CurrentTokenType == TokenType.DocumentEnd)
			{
				this.tokenizer.Read();
			}
			this.currentState = ParseState.DocumentStart;
			this.CurrentEventType = ParseEventType.DocumentEnd;
		}

		private void ParseNode(bool block, bool indentlessSequence)
		{
			this.currentAnchor = null;
			this.currentTag = null;
			this.UnityStrippedMark = false;
			switch (this.CurrentTokenType)
			{
			case TokenType.Alias:
			{
				this.PopState();
				string text = this.tokenizer.TakeCurrentTokenContent<Scalar>().ToString();
				this.tokenizer.Read();
				int num;
				if (this.anchors.TryGetValue(text, out num))
				{
					this.currentAnchor = new Anchor(text, num);
					this.CurrentEventType = ParseEventType.Alias;
					return;
				}
				Marker marker = this.CurrentMark;
				throw new YamlParserException(in marker, "While parsing node, found unknown anchor");
			}
			case TokenType.Anchor:
			{
				string text2 = this.tokenizer.TakeCurrentTokenContent<Scalar>().ToString();
				int num2 = this.RegisterAnchor(text2);
				this.currentAnchor = new Anchor(text2, num2);
				this.tokenizer.Read();
				if (this.CurrentTokenType == TokenType.Tag)
				{
					this.currentTag = this.tokenizer.TakeCurrentTokenContent<Tag>();
					this.tokenizer.Read();
				}
				break;
			}
			case TokenType.Tag:
				this.currentTag = this.tokenizer.TakeCurrentTokenContent<Tag>();
				this.tokenizer.Read();
				if (this.CurrentTokenType == TokenType.Anchor)
				{
					string text3 = this.tokenizer.TakeCurrentTokenContent<Scalar>().ToString();
					int num3 = this.RegisterAnchor(text3);
					this.currentAnchor = new Anchor(text3, num3);
					if (this.CurrentEventType == ParseEventType.DocumentStart)
					{
						Tag tag = this.currentTag;
						if (((tag != null) ? tag.Handle : null) == "!u!")
						{
							this.UnityStrippedMark = this.tokenizer.TrySkipUnityStrippedSymbol();
						}
					}
					this.tokenizer.Read();
				}
				break;
			}
			int num4;
			switch (this.CurrentTokenType)
			{
			case TokenType.BlockSequenceStart:
				if (block)
				{
					this.currentState = ParseState.BlockSequenceFirstEntry;
					this.CurrentEventType = ParseEventType.SequenceStart;
					return;
				}
				break;
			case TokenType.BlockMappingStart:
				if (block)
				{
					this.currentState = ParseState.BlockMappingFirstKey;
					this.CurrentEventType = ParseEventType.MappingStart;
					return;
				}
				break;
			case TokenType.FlowSequenceStart:
				this.currentState = ParseState.FlowSequenceFirstEntry;
				this.CurrentEventType = ParseEventType.SequenceStart;
				return;
			case TokenType.FlowMappingStart:
				this.currentState = ParseState.FlowMappingFirstKey;
				this.CurrentEventType = ParseEventType.MappingStart;
				return;
			case TokenType.BlockEntryStart:
				if (indentlessSequence)
				{
					this.currentState = ParseState.IndentlessSequenceEntry;
					this.CurrentEventType = ParseEventType.SequenceStart;
					return;
				}
				num4 = 1;
				goto IL_027D;
			case TokenType.PlainScalar:
			case TokenType.SingleQuotedScaler:
			case TokenType.DoubleQuotedScaler:
			case TokenType.LiteralScalar:
			case TokenType.FoldedScalar:
				this.PopState();
				this.currentScalar = this.tokenizer.TakeCurrentTokenContent<Scalar>();
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.Scalar;
				return;
			}
			num4 = 5;
			IL_027D:
			if (this.currentAnchor == null && this.currentTag == null)
			{
				if (num4 != 1)
				{
					if (num4 != 5)
					{
						goto IL_0297;
					}
				}
				else if (this.currentState == ParseState.IndentlessSequenceEntry)
				{
					this.PopState();
					this.EmptyScalar();
					return;
				}
				Marker marker = this.tokenizer.CurrentMark;
				throw new YamlTokenizerException(in marker, "while parsing a node, did not find expected node content");
			}
			IL_0297:
			this.PopState();
			this.EmptyScalar();
		}

		private void ParseBlockMappingKey(bool first)
		{
			if (first)
			{
				this.tokenizer.Read();
			}
			TokenType currentTokenType = this.CurrentTokenType;
			if (currentTokenType == TokenType.BlockEnd)
			{
				this.PopState();
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.MappingEnd;
				return;
			}
			if (currentTokenType != TokenType.KeyStart)
			{
				if (currentTokenType != TokenType.ValueStart)
				{
					Marker currentMark = this.CurrentMark;
					throw new YamlParserException(in currentMark, "while parsing a block mapping, did not find expected key");
				}
				this.currentState = ParseState.BlockMappingValue;
				this.EmptyScalar();
				return;
			}
			else
			{
				this.tokenizer.Read();
				TokenType currentTokenType2 = this.CurrentTokenType;
				bool flag = currentTokenType2 == TokenType.BlockEnd || currentTokenType2 - TokenType.KeyStart <= 1;
				if (flag)
				{
					this.currentState = ParseState.BlockMappingValue;
					this.EmptyScalar();
					return;
				}
				this.PushState(ParseState.BlockMappingValue);
				this.ParseNode(true, true);
				return;
			}
		}

		private void ParseBlockMappingValue()
		{
			if (this.CurrentTokenType != TokenType.ValueStart)
			{
				this.currentState = ParseState.BlockMappingKey;
				this.EmptyScalar();
				return;
			}
			this.tokenizer.Read();
			TokenType currentTokenType = this.CurrentTokenType;
			bool flag = currentTokenType == TokenType.BlockEnd || currentTokenType - TokenType.KeyStart <= 1;
			if (flag)
			{
				this.currentState = ParseState.BlockMappingKey;
				this.EmptyScalar();
				return;
			}
			this.PushState(ParseState.BlockMappingKey);
			this.ParseNode(true, true);
		}

		private void ParseBlockSequenceEntry(bool first)
		{
			if (first)
			{
				this.tokenizer.Read();
			}
			TokenType currentTokenType = this.CurrentTokenType;
			if (currentTokenType == TokenType.BlockEnd)
			{
				this.PopState();
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.SequenceEnd;
				return;
			}
			if (currentTokenType != TokenType.BlockEntryStart)
			{
				Marker currentMark = this.CurrentMark;
				throw new YamlParserException(in currentMark, "while parsing a block collection, did not find expected '-' indicator");
			}
			this.tokenizer.Read();
			TokenType currentTokenType2 = this.CurrentTokenType;
			bool flag = currentTokenType2 == TokenType.BlockEnd || currentTokenType2 == TokenType.BlockEntryStart;
			if (flag)
			{
				this.currentState = ParseState.BlockSequenceEntry;
				this.EmptyScalar();
				return;
			}
			this.PushState(ParseState.BlockSequenceEntry);
			this.ParseNode(true, false);
		}

		private void ParseFlowSequenceEntry(bool first)
		{
			if (first)
			{
				this.tokenizer.Read();
			}
			TokenType tokenType = this.CurrentTokenType;
			if (tokenType == TokenType.FlowSequenceEnd)
			{
				this.PopState();
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.SequenceEnd;
				return;
			}
			if (tokenType == TokenType.FlowEntryStart)
			{
				if (!first)
				{
					this.tokenizer.Read();
					goto IL_0064;
				}
			}
			if (!first)
			{
				Marker currentMark = this.CurrentMark;
				throw new YamlParserException(in currentMark, "while parsing a flow sequence, expected ',' or ']'");
			}
			IL_0064:
			tokenType = this.CurrentTokenType;
			if (tokenType == TokenType.FlowSequenceEnd)
			{
				this.PopState();
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.SequenceEnd;
				return;
			}
			if (tokenType != TokenType.KeyStart)
			{
				this.PushState(ParseState.FlowSequenceEntry);
				this.ParseNode(false, false);
				return;
			}
			this.currentState = ParseState.FlowSequenceEntryMappingKey;
			this.tokenizer.Read();
			this.CurrentEventType = ParseEventType.MappingStart;
		}

		private void ParseFlowMappingKey(bool first)
		{
			if (first)
			{
				this.tokenizer.Read();
			}
			if (this.CurrentTokenType == TokenType.FlowMappingEnd)
			{
				this.PopState();
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.MappingEnd;
				return;
			}
			if (!first)
			{
				if (this.CurrentTokenType != TokenType.FlowEntryStart)
				{
					Marker currentMark = this.CurrentMark;
					throw new YamlParserException(in currentMark, "While parsing a flow mapping, did not find expected ',' or '}'");
				}
				this.tokenizer.Read();
			}
			switch (this.CurrentTokenType)
			{
			case TokenType.FlowMappingEnd:
				this.PopState();
				this.tokenizer.Read();
				this.CurrentEventType = ParseEventType.MappingEnd;
				return;
			case TokenType.KeyStart:
			{
				this.tokenizer.Read();
				bool flag;
				switch (this.CurrentTokenType)
				{
				case TokenType.FlowMappingEnd:
				case TokenType.FlowEntryStart:
				case TokenType.ValueStart:
					flag = true;
					goto IL_00C4;
				}
				flag = false;
				IL_00C4:
				if (flag)
				{
					this.currentState = ParseState.FlowMappingValue;
					this.EmptyScalar();
					return;
				}
				this.PushState(ParseState.FlowMappingValue);
				this.ParseNode(false, false);
				return;
			}
			case TokenType.ValueStart:
				this.currentState = ParseState.FlowMappingValue;
				this.EmptyScalar();
				return;
			}
			this.PushState(ParseState.FlowMappingEmptyValue);
			this.ParseNode(false, false);
		}

		private void ParseFlowMappingValue(bool empty)
		{
			if (empty)
			{
				this.currentState = ParseState.FlowMappingKey;
				this.EmptyScalar();
				return;
			}
			if (this.CurrentTokenType == TokenType.ValueStart)
			{
				this.tokenizer.Read();
				if (this.CurrentTokenType != TokenType.FlowEntryStart && this.CurrentTokenType != TokenType.FlowMappingEnd)
				{
					this.PushState(ParseState.FlowMappingKey);
					this.ParseNode(false, false);
					return;
				}
			}
			this.currentState = ParseState.FlowMappingKey;
			this.EmptyScalar();
		}

		private void ParseIndentlessSequenceEntry()
		{
			if (this.CurrentTokenType != TokenType.BlockEntryStart)
			{
				this.PopState();
				this.CurrentEventType = ParseEventType.SequenceEnd;
				return;
			}
			this.tokenizer.Read();
			TokenType currentTokenType = this.CurrentTokenType;
			bool flag = currentTokenType == TokenType.BlockEnd || currentTokenType - TokenType.KeyStart <= 1;
			if (flag)
			{
				this.currentState = ParseState.IndentlessSequenceEntry;
				this.EmptyScalar();
				return;
			}
			this.PushState(ParseState.IndentlessSequenceEntry);
			this.ParseNode(true, false);
		}

		private void ParseFlowSequenceEntryMappingKey()
		{
			TokenType currentTokenType = this.CurrentTokenType;
			bool flag = currentTokenType == TokenType.FlowSequenceEnd || currentTokenType == TokenType.FlowEntryStart || currentTokenType == TokenType.ValueStart;
			if (flag)
			{
				this.tokenizer.Read();
				this.currentState = ParseState.FlowSequenceEntryMappingValue;
				this.EmptyScalar();
				return;
			}
			this.PushState(ParseState.FlowSequenceEntryMappingValue);
			this.ParseNode(false, false);
		}

		private void ParseFlowSequenceEntryMappingValue()
		{
			if (this.CurrentTokenType != TokenType.ValueStart)
			{
				this.currentState = ParseState.FlowSequenceEntryMappingEnd;
				this.EmptyScalar();
				return;
			}
			this.tokenizer.Read();
			this.currentState = ParseState.FlowSequenceEntryMappingValue;
			TokenType currentTokenType = this.CurrentTokenType;
			bool flag = currentTokenType == TokenType.FlowSequenceEnd || currentTokenType == TokenType.FlowEntryStart;
			if (flag)
			{
				this.currentState = ParseState.FlowSequenceEntryMappingEnd;
				this.EmptyScalar();
				return;
			}
			this.PushState(ParseState.FlowSequenceEntryMappingEnd);
			this.ParseNode(false, false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ParseFlowSequenceEntryMappingEnd()
		{
			this.currentState = ParseState.FlowSequenceEntry;
			this.CurrentEventType = ParseEventType.MappingEnd;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void PopState()
		{
			this.currentState = *this.stateStack.Pop();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PushState(ParseState state)
		{
			this.stateStack.Add(state);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EmptyScalar()
		{
			this.currentScalar = null;
			this.CurrentEventType = ParseEventType.Scalar;
		}

		private void ProcessDirectives()
		{
			for (;;)
			{
				TokenType currentTokenType = this.tokenizer.CurrentTokenType;
				if (currentTokenType != TokenType.VersionDirective && currentTokenType != TokenType.TagDirective)
				{
					break;
				}
				this.tokenizer.Read();
			}
		}

		[NullableContext(1)]
		private int RegisterAnchor(string anchorName)
		{
			int num = this.lastAnchorId + 1;
			this.lastAnchorId = num;
			int num2 = num;
			this.anchors[anchorName] = num2;
			return num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfCurrentTokenUnless(TokenType expectedTokenType)
		{
			if (this.CurrentTokenType != expectedTokenType)
			{
				Marker currentMark = this.tokenizer.CurrentMark;
				throw new YamlParserException(in currentMark, string.Format("Did not find expected token of  `{0}`", expectedTokenType));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool IsNullScalar()
		{
			return this.CurrentEventType == ParseEventType.Scalar && (this.currentScalar == null || this.currentScalar.IsNull());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string GetScalarAsString()
		{
			Scalar scalar = this.currentScalar;
			if (scalar == null)
			{
				return null;
			}
			return scalar.ToString();
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<byte> GetScalarAsUtf8()
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.AsUtf8();
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect a scalar value as utf8 : {0} {1}", this.CurrentEventType, this.currentScalar));
			return default(ReadOnlySpan<byte>);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsSpan(out ReadOnlySpan<byte> span)
		{
			if (this.currentScalar == null)
			{
				span = default(ReadOnlySpan<byte>);
				return false;
			}
			span = this.currentScalar.AsSpan();
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool GetScalarAsBool()
		{
			Scalar scalar = this.currentScalar;
			bool flag;
			if (scalar != null && scalar.TryGetBool(out flag))
			{
				return flag;
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect a scalar value as bool : {0} {1}", this.CurrentEventType, this.currentScalar));
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetScalarAsInt32()
		{
			Scalar scalar = this.currentScalar;
			int num;
			if (scalar != null && scalar.TryGetInt32(out num))
			{
				return num;
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect a scalar value as Int32: {0} {1}", this.CurrentEventType, this.currentScalar));
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly long GetScalarAsInt64()
		{
			Scalar scalar = this.currentScalar;
			long num;
			if (scalar != null && scalar.TryGetInt64(out num))
			{
				return num;
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect a scalar value as Int64: {0} {1}", this.CurrentEventType, this.currentScalar));
			return 0L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly uint GetScalarAsUInt32()
		{
			Scalar scalar = this.currentScalar;
			uint num;
			if (scalar != null && scalar.TryGetUInt32(out num))
			{
				return num;
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect a scalar value as UInt32 : {0} {1}", this.CurrentEventType, this.currentScalar));
			return 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ulong GetScalarAsUInt64()
		{
			Scalar scalar = this.currentScalar;
			ulong num;
			if (scalar != null && scalar.TryGetUInt64(out num))
			{
				return num;
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect a scalar value as UInt64 : {0} ({1})", this.CurrentEventType, this.currentScalar));
			return 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float GetScalarAsFloat()
		{
			Scalar scalar = this.currentScalar;
			float num;
			if (scalar != null && scalar.TryGetFloat(out num))
			{
				return num;
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect scalar value as float : {0} {1}", this.CurrentEventType, this.currentScalar));
			return 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly double GetScalarAsDouble()
		{
			Scalar scalar = this.currentScalar;
			double num;
			if (scalar != null && scalar.TryGetDouble(out num))
			{
				return num;
			}
			Marker currentMark = this.CurrentMark;
			YamlParserException.Throw(in currentMark, string.Format("Cannot detect a scalar value as double : {0} {1}", this.CurrentEventType, this.currentScalar));
			return 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ReadScalarAsString()
		{
			Scalar scalar = this.currentScalar;
			string text = ((scalar != null) ? scalar.ToString() : null);
			this.ReadWithVerify(ParseEventType.Scalar);
			return text;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ReadScalarAsBool()
		{
			bool scalarAsBool = this.GetScalarAsBool();
			this.ReadWithVerify(ParseEventType.Scalar);
			return scalarAsBool;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ReadScalarAsInt32()
		{
			int scalarAsInt = this.GetScalarAsInt32();
			this.ReadWithVerify(ParseEventType.Scalar);
			return scalarAsInt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public long ReadScalarAsInt64()
		{
			long scalarAsInt = this.GetScalarAsInt64();
			this.ReadWithVerify(ParseEventType.Scalar);
			return scalarAsInt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint ReadScalarAsUInt32()
		{
			uint scalarAsUInt = this.GetScalarAsUInt32();
			this.ReadWithVerify(ParseEventType.Scalar);
			return scalarAsUInt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ulong ReadScalarAsUInt64()
		{
			ulong scalarAsUInt = this.GetScalarAsUInt64();
			this.ReadWithVerify(ParseEventType.Scalar);
			return scalarAsUInt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float ReadScalarAsFloat()
		{
			float scalarAsFloat = this.GetScalarAsFloat();
			this.ReadWithVerify(ParseEventType.Scalar);
			return scalarAsFloat;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double ReadScalarAsDouble()
		{
			double scalarAsDouble = this.GetScalarAsDouble();
			this.ReadWithVerify(ParseEventType.Scalar);
			return scalarAsDouble;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsString(out string result)
		{
			if (this.CurrentEventType != ParseEventType.Scalar)
			{
				result = null;
				return false;
			}
			Scalar scalar = this.currentScalar;
			result = ((scalar != null) ? scalar.ToString() : null);
			this.ReadWithVerify(ParseEventType.Scalar);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsBool(out bool result)
		{
			if (this.TryGetScalarAsBool(out result))
			{
				this.ReadWithVerify(ParseEventType.Scalar);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsInt32(out int result)
		{
			if (this.TryGetScalarAsInt32(out result))
			{
				this.ReadWithVerify(ParseEventType.Scalar);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsInt64(out long result)
		{
			if (this.TryGetScalarAsInt64(out result))
			{
				this.ReadWithVerify(ParseEventType.Scalar);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsUInt32(out uint result)
		{
			if (this.TryGetScalarAsUInt32(out result))
			{
				this.ReadWithVerify(ParseEventType.Scalar);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsUInt64(out ulong result)
		{
			if (this.TryGetScalarAsUInt64(out result))
			{
				this.ReadWithVerify(ParseEventType.Scalar);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsFloat(out float result)
		{
			if (this.TryGetScalarAsFloat(out result))
			{
				this.ReadWithVerify(ParseEventType.Scalar);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadScalarAsDouble(out double result)
		{
			if (this.TryGetScalarAsDouble(out result))
			{
				this.ReadWithVerify(ParseEventType.Scalar);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsString(out string value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				value = (scalar.IsNull() ? null : scalar.ToString());
				return true;
			}
			value = null;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsBool(out bool value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.TryGetBool(out value);
			}
			value = false;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsInt32(out int value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.TryGetInt32(out value);
			}
			value = 0;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsUInt32(out uint value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.TryGetUInt32(out value);
			}
			value = 0U;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsInt64(out long value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.TryGetInt64(out value);
			}
			value = 0L;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsUInt64(out ulong value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.TryGetUInt64(out value);
			}
			value = 0UL;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsFloat(out float value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.TryGetFloat(out value);
			}
			value = 0f;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetScalarAsDouble(out double value)
		{
			Scalar scalar = this.currentScalar;
			if (scalar != null)
			{
				return scalar.TryGetDouble(out value);
			}
			value = 0.0;
			return false;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetCurrentTag(out Tag tag)
		{
			if (this.currentTag != null)
			{
				tag = this.currentTag;
				return true;
			}
			tag = null;
			return false;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetCurrentAnchor(out Anchor anchor)
		{
			if (this.currentAnchor != null)
			{
				anchor = this.currentAnchor;
				return true;
			}
			anchor = null;
			return false;
		}

		[Nullable(new byte[] { 2, 1 })]
		[ThreadStatic]
		private static Dictionary<string, int> anchorsBufferStatic;

		[ThreadStatic]
		private static ExpandBuffer<ParseState> stateStackBufferStatic;

		private Utf8YamlTokenizer tokenizer;

		private ParseState currentState;

		private Scalar currentScalar;

		private Tag currentTag;

		private Anchor currentAnchor;

		private int lastAnchorId;

		[Nullable(1)]
		private readonly Dictionary<string, int> anchors;

		[Nullable(1)]
		private readonly ExpandBuffer<ParseState> stateStack;
	}
}
