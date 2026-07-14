using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using VYaml.Internal;

namespace VYaml.Parser
{
	[NullableContext(1)]
	[Nullable(0)]
	public ref struct Utf8YamlTokenizer
	{
		public TokenType CurrentTokenType
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.currentToken.Type;
			}
		}

		public Marker CurrentMark
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.mark;
			}
		}

		[NullableContext(0)]
		public Utf8YamlTokenizer(ReadOnlySequence<byte> sequence)
		{
			this.reader = new SequenceReader<byte>(sequence);
			this.mark = new Marker(0, 1, 0);
			this.indent = -1;
			this.flowLevel = 0;
			this.adjacentValueAllowedAt = 0;
			this.tokensParsed = 0;
			this.simpleKeyAllowed = false;
			this.streamStartProduced = false;
			this.streamEndProduced = false;
			this.tokenAvailable = false;
			this.currentToken = default(Token);
			InsertionQueue<Token> insertionQueue;
			if ((insertionQueue = Utf8YamlTokenizer.tokensBufferStatic) == null)
			{
				insertionQueue = (Utf8YamlTokenizer.tokensBufferStatic = new InsertionQueue<Token>(16));
			}
			this.tokens = insertionQueue;
			this.tokens.Clear();
			ExpandBuffer<SimpleKeyState> expandBuffer;
			if ((expandBuffer = Utf8YamlTokenizer.simpleKeyBufferStatic) == null)
			{
				expandBuffer = (Utf8YamlTokenizer.simpleKeyBufferStatic = new ExpandBuffer<SimpleKeyState>(16));
			}
			this.simpleKeyCandidates = expandBuffer;
			this.simpleKeyCandidates.Clear();
			ExpandBuffer<int> expandBuffer2;
			if ((expandBuffer2 = Utf8YamlTokenizer.indentsBufferStatic) == null)
			{
				expandBuffer2 = (Utf8YamlTokenizer.indentsBufferStatic = new ExpandBuffer<int>(16));
			}
			this.indents = expandBuffer2;
			this.indents.Clear();
			this.reader.TryPeek(out this.currentCode);
		}

		public bool Read()
		{
			if (this.streamEndProduced)
			{
				return false;
			}
			if (!this.tokenAvailable)
			{
				this.ConsumeMoreTokens();
			}
			Scalar scalar = this.currentToken.Content as Scalar;
			if (scalar != null)
			{
				ScalarPool.Shared.Return(scalar);
			}
			this.currentToken = this.tokens.Dequeue();
			this.tokenAvailable = false;
			this.tokensParsed++;
			if (this.currentToken.Type == TokenType.StreamEnd)
			{
				this.streamEndProduced = true;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal T TakeCurrentTokenContent<[Nullable(0)] T>() where T : ITokenContent
		{
			Token token = this.currentToken;
			Scalar scalar = token.Content as Scalar;
			if (scalar != null)
			{
				scalar.Type = this.currentToken.Type;
			}
			this.currentToken = default(Token);
			return (T)((object)token.Content);
		}

		internal bool TrySkipUnityStrippedSymbol()
		{
			while (this.currentCode == 32)
			{
				this.Advance(1);
			}
			if (this.reader.IsNext(YamlCodes.UnityStrippedSymbol, false))
			{
				this.Advance(YamlCodes.UnityStrippedSymbol.Length);
				return true;
			}
			return false;
		}

		private void ConsumeMoreTokens()
		{
			for (;;)
			{
				bool flag = this.tokens.Count <= 0;
				if (!flag)
				{
					this.StaleSimpleKeyCandidates();
					for (int i = 0; i < this.simpleKeyCandidates.Length; i++)
					{
						ref SimpleKeyState ptr = ref this.simpleKeyCandidates[i];
						if (ptr.Possible && ptr.TokenNumber == this.tokensParsed)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
				this.ConsumeNextToken();
			}
			this.tokenAvailable = true;
		}

		private void ConsumeNextToken()
		{
			if (!this.streamStartProduced)
			{
				this.ConsumeStreamStart();
				return;
			}
			this.SkipToNextToken();
			this.StaleSimpleKeyCandidates();
			this.UnrollIndent(this.mark.Col);
			if (this.reader.End)
			{
				this.ConsumeStreamEnd();
				return;
			}
			byte b;
			if (this.mark.Col == 0)
			{
				b = this.currentCode;
				if (b == 37)
				{
					this.ConsumeDirective();
					return;
				}
				if (b != 45)
				{
					if (b == 46)
					{
						if (this.reader.IsNext(YamlCodes.DocStart, false) && this.IsEmptyNext(YamlCodes.DocStart.Length))
						{
							this.ConsumeDocumentIndicator(TokenType.DocumentEnd);
							return;
						}
					}
				}
				else if (this.reader.IsNext(YamlCodes.StreamStart, false) && this.IsEmptyNext(YamlCodes.StreamStart.Length))
				{
					this.ConsumeDocumentIndicator(TokenType.DocumentStart);
					return;
				}
			}
			b = this.currentCode;
			if (b <= 91)
			{
				switch (b)
				{
				case 33:
					this.ConsumeTag();
					return;
				case 34:
					this.ConsumeFlowScaler(false);
					return;
				case 35:
				case 36:
				case 40:
				case 41:
				case 43:
					goto IL_02BD;
				case 37:
					break;
				case 38:
					this.ConsumeAnchor(false);
					return;
				case 39:
					this.ConsumeFlowScaler(true);
					return;
				case 42:
					this.ConsumeAnchor(true);
					return;
				case 44:
					this.ConsumeFlowEntryStart();
					return;
				case 45:
				{
					byte b2;
					if (!this.TryPeek(1L, out b2) || YamlCodes.IsEmpty(b2))
					{
						this.ConsumeBlockEntry();
						return;
					}
					byte b3;
					if (!this.TryPeek(1L, out b3) || YamlCodes.IsBlank(b3))
					{
						this.ConsumePlainScalar();
						return;
					}
					goto IL_02BD;
				}
				default:
				{
					switch (b)
					{
					case 58:
					{
						byte b4;
						if ((this.TryPeek(1L, out b4) && YamlCodes.IsEmpty(b4)) || (this.flowLevel > 0 && (YamlCodes.IsAnyFlowSymbol(b4) || this.mark.Position == this.adjacentValueAllowedAt)))
						{
							this.ConsumeValueStart();
							return;
						}
						break;
					}
					case 59:
					case 60:
					case 61:
						goto IL_02BD;
					case 62:
						if (this.flowLevel == 0)
						{
							this.ConsumeBlockScaler(false);
							return;
						}
						goto IL_02BD;
					case 63:
					{
						byte b5;
						if (!this.TryPeek(1L, out b5) || YamlCodes.IsEmpty(b5))
						{
							this.ConsumeComplexKeyStart();
							return;
						}
						break;
					}
					case 64:
						goto IL_029C;
					default:
						if (b != 91)
						{
							goto IL_02BD;
						}
						this.ConsumeFlowCollectionStart(TokenType.FlowSequenceStart);
						return;
					}
					byte b6;
					if (this.flowLevel == 0 && (!this.TryPeek(1L, out b6) || YamlCodes.IsBlank(b6)))
					{
						this.ConsumePlainScalar();
						return;
					}
					goto IL_02BD;
				}
				}
			}
			else
			{
				if (b == 93)
				{
					this.ConsumeFlowCollectionEnd(TokenType.FlowSequenceEnd);
					return;
				}
				if (b != 96)
				{
					switch (b)
					{
					case 123:
						this.ConsumeFlowCollectionStart(TokenType.FlowMappingStart);
						return;
					case 124:
						if (this.flowLevel == 0)
						{
							this.ConsumeBlockScaler(true);
							return;
						}
						goto IL_02BD;
					case 125:
						this.ConsumeFlowCollectionEnd(TokenType.FlowMappingEnd);
						return;
					default:
						goto IL_02BD;
					}
				}
			}
			IL_029C:
			throw new YamlTokenizerException(in this.mark, string.Format("Unexpected character: '{0}'", this.currentCode));
			IL_02BD:
			this.ConsumePlainScalar();
		}

		private void ConsumeStreamStart()
		{
			this.indent = -1;
			this.streamStartProduced = true;
			this.simpleKeyAllowed = true;
			this.tokens.Enqueue(new Token(TokenType.StreamStart, null));
			this.simpleKeyCandidates.Add(default(SimpleKeyState));
		}

		private void ConsumeStreamEnd()
		{
			if (this.mark.Col != 0)
			{
				this.mark.Col = 0;
				this.mark.Line = this.mark.Line + 1;
			}
			this.UnrollIndent(-1);
			this.RemoveSimpleKeyCandidate();
			this.simpleKeyAllowed = false;
			this.tokens.Enqueue(new Token(TokenType.StreamEnd, null));
		}

		private bool ConsumeBom()
		{
			if (!this.reader.IsNext(YamlCodes.Utf8Bom, false))
			{
				return false;
			}
			bool flag = this.mark.Position == 0;
			this.Advance(YamlCodes.Utf8Bom.Length);
			this.mark.Col = 0;
			if (!flag && this.CurrentTokenType != TokenType.DocumentEnd && (this.tokens.Count <= 0 || this.tokens.Peek().Type != TokenType.DocumentEnd) && !this.reader.IsNext(YamlCodes.DocStart, false))
			{
				Marker currentMark = this.CurrentMark;
				throw new YamlTokenizerException(in currentMark, "BOM must be at the beginning of the stream or document.");
			}
			return true;
		}

		private void ConsumeDirective()
		{
			this.UnrollIndent(-1);
			this.RemoveSimpleKeyCandidate();
			this.simpleKeyAllowed = false;
			this.Advance(1);
			Scalar scalar = ScalarPool.Shared.Rent();
			try
			{
				this.ConsumeDirectiveName(scalar);
				if (scalar.SequenceEqual(YamlCodes.YamlDirectiveName))
				{
					this.ConsumeVersionDirectiveValue();
					goto IL_00A9;
				}
				if (scalar.SequenceEqual(YamlCodes.TagDirectiveName))
				{
					this.ConsumeTagDirectiveValue();
					goto IL_00A9;
				}
				while (!this.reader.End && !YamlCodes.IsLineBreak(this.currentCode))
				{
					this.Advance(1);
				}
				this.tokens.Enqueue(new Token(TokenType.TagDirective, null));
				goto IL_00A9;
			}
			finally
			{
				ScalarPool.Shared.Return(scalar);
			}
			IL_00A2:
			this.Advance(1);
			IL_00A9:
			if (YamlCodes.IsBlank(this.currentCode))
			{
				goto IL_00A2;
			}
			if (this.currentCode == 35)
			{
				while (!this.reader.End && !YamlCodes.IsLineBreak(this.currentCode))
				{
					this.Advance(1);
				}
			}
			if (!this.reader.End && !YamlCodes.IsLineBreak(this.currentCode))
			{
				Marker currentMark = this.CurrentMark;
				throw new YamlTokenizerException(in currentMark, "While scanning a directive, did not find expected comment or line break");
			}
			if (YamlCodes.IsLineBreak(this.currentCode))
			{
				this.ConsumeLineBreaks();
			}
		}

		private void ConsumeDirectiveName(Scalar result)
		{
			while (YamlCodes.IsAlphaNumericDashOrUnderscore(this.currentCode))
			{
				result.Write(this.currentCode);
				this.Advance(1);
			}
			if (result.Length <= 0)
			{
				Marker marker = this.CurrentMark;
				throw new YamlTokenizerException(in marker, "While scanning a directive, could not find expected directive name");
			}
			if (!this.reader.End && !YamlCodes.IsBlank(this.currentCode))
			{
				Marker marker = this.CurrentMark;
				throw new YamlTokenizerException(in marker, "While scanning a directive, found unexpected non-alphabetical character");
			}
		}

		private void ConsumeVersionDirectiveValue()
		{
			while (YamlCodes.IsBlank(this.currentCode))
			{
				this.Advance(1);
			}
			int num = this.ConsumeVersionDirectiveNumber();
			if (this.currentCode != 46)
			{
				Marker currentMark = this.CurrentMark;
				throw new YamlTokenizerException(in currentMark, "while scanning a YAML directive, did not find expected digit or '.' character");
			}
			this.Advance(1);
			int num2 = this.ConsumeVersionDirectiveNumber();
			this.tokens.Enqueue(new Token(TokenType.VersionDirective, new VersionDirective(num, num2)));
		}

		private int ConsumeVersionDirectiveNumber()
		{
			int num = 0;
			int num2 = 0;
			while (YamlCodes.IsNumber(this.currentCode))
			{
				if (num2 + 1 > 9)
				{
					Marker marker = this.CurrentMark;
					throw new YamlTokenizerException(in marker, "While scanning a YAML directive, found exteremely long version number");
				}
				num2++;
				num = num * 10 + (int)YamlCodes.AsHex(this.currentCode);
				this.Advance(1);
			}
			if (num2 == 0)
			{
				Marker marker = this.CurrentMark;
				throw new YamlTokenizerException(in marker, "While scanning a YAML directive, did not find expected version number");
			}
			return num;
		}

		private void ConsumeTagDirectiveValue()
		{
			Scalar scalar = ScalarPool.Shared.Rent();
			Scalar scalar2 = ScalarPool.Shared.Rent();
			try
			{
				while (YamlCodes.IsBlank(this.currentCode))
				{
					this.Advance(1);
				}
				this.ConsumeTagHandle(true, scalar);
				if (!YamlCodes.IsBlank(this.currentCode))
				{
					Marker marker = this.CurrentMark;
					throw new YamlTokenizerException(in marker, "While scanning a TAG directive, did not find expected whitespace after tag handle.");
				}
				while (YamlCodes.IsBlank(this.currentCode))
				{
					this.Advance(1);
				}
				this.ConsumeTagPrefix(scalar2);
				if (!YamlCodes.IsEmpty(this.currentCode) && !this.reader.End)
				{
					Marker marker = this.CurrentMark;
					throw new YamlTokenizerException(in marker, "While scanning TAG, did not find expected whitespace or line break");
				}
				this.tokens.Enqueue(new Token(TokenType.TagDirective, new Tag(scalar.ToString(), scalar2.ToString())));
			}
			finally
			{
				ScalarPool.Shared.Return(scalar);
				ScalarPool.Shared.Return(scalar2);
			}
		}

		private void ConsumeDocumentIndicator(TokenType tokenType)
		{
			this.UnrollIndent(-1);
			this.RemoveSimpleKeyCandidate();
			this.simpleKeyAllowed = false;
			this.Advance(3);
			this.tokens.Enqueue(new Token(tokenType, null));
		}

		private void ConsumeFlowCollectionStart(TokenType tokenType)
		{
			this.SaveSimpleKeyCandidate();
			this.IncreaseFlowLevel();
			this.simpleKeyAllowed = true;
			this.Advance(1);
			this.tokens.Enqueue(new Token(tokenType, null));
		}

		private void ConsumeFlowCollectionEnd(TokenType tokenType)
		{
			this.RemoveSimpleKeyCandidate();
			this.DecreaseFlowLevel();
			this.simpleKeyAllowed = false;
			this.Advance(1);
			this.tokens.Enqueue(new Token(tokenType, null));
		}

		private void ConsumeFlowEntryStart()
		{
			this.RemoveSimpleKeyCandidate();
			this.simpleKeyAllowed = true;
			this.Advance(1);
			this.tokens.Enqueue(new Token(TokenType.FlowEntryStart, null));
		}

		private void ConsumeBlockEntry()
		{
			if (this.flowLevel != 0)
			{
				throw new YamlTokenizerException(in this.mark, "'-' is only valid inside a block");
			}
			if (!this.simpleKeyAllowed)
			{
				throw new YamlTokenizerException(in this.mark, "Block sequence entries are not allowed in this context");
			}
			int col = this.mark.Col;
			Token token = new Token(TokenType.BlockSequenceStart, null);
			this.RollIndent(col, in token, -1);
			this.RemoveSimpleKeyCandidate();
			this.simpleKeyAllowed = true;
			this.Advance(1);
			this.tokens.Enqueue(new Token(TokenType.BlockEntryStart, null));
		}

		private void ConsumeComplexKeyStart()
		{
			if (this.flowLevel == 0)
			{
				if (!this.simpleKeyAllowed)
				{
					throw new YamlTokenizerException(in this.mark, "Mapping keys are not allowed in this context");
				}
				int col = this.mark.Col;
				Token token = new Token(TokenType.BlockMappingStart, null);
				this.RollIndent(col, in token, -1);
			}
			this.RemoveSimpleKeyCandidate();
			this.simpleKeyAllowed = this.flowLevel == 0;
			this.Advance(1);
			this.tokens.Enqueue(new Token(TokenType.KeyStart, null));
		}

		private void ConsumeValueStart()
		{
			ExpandBuffer<SimpleKeyState> expandBuffer = this.simpleKeyCandidates;
			ref SimpleKeyState ptr = ref expandBuffer[expandBuffer.Length - 1];
			if (ptr.Possible)
			{
				Token token = new Token(TokenType.KeyStart, null);
				this.tokens.Insert(ptr.TokenNumber - this.tokensParsed, token);
				int col = ptr.Start.Col;
				Token token2 = new Token(TokenType.BlockMappingStart, null);
				this.RollIndent(col, in token2, ptr.TokenNumber);
				ExpandBuffer<SimpleKeyState> expandBuffer2 = this.simpleKeyCandidates;
				expandBuffer2[expandBuffer2.Length - 1].Possible = false;
				this.simpleKeyAllowed = false;
			}
			else
			{
				if (this.flowLevel == 0)
				{
					if (!this.simpleKeyAllowed)
					{
						throw new YamlTokenizerException(in this.mark, "Mapping values are not allowed in this context");
					}
					int col2 = this.mark.Col;
					Token token2 = new Token(TokenType.BlockMappingStart, null);
					this.RollIndent(col2, in token2, -1);
				}
				this.simpleKeyAllowed = this.flowLevel == 0;
			}
			this.Advance(1);
			this.tokens.Enqueue(new Token(TokenType.ValueStart, null));
		}

		private void ConsumeAnchor(bool alias)
		{
			this.SaveSimpleKeyCandidate();
			this.simpleKeyAllowed = false;
			Scalar scalar = ScalarPool.Shared.Rent();
			this.Advance(1);
			while (YamlCodes.IsAlphaNumericDashOrUnderscore(this.currentCode))
			{
				scalar.Write(this.currentCode);
				this.Advance(1);
			}
			if (scalar.Length <= 0)
			{
				throw new YamlTokenizerException(in this.mark, "while scanning an anchor or alias, did not find expected alphabetic or numeric character");
			}
			if (!YamlCodes.IsEmpty(this.currentCode) && !this.reader.End && this.currentCode != 63 && this.currentCode != 58 && this.currentCode != 44 && this.currentCode != 93 && this.currentCode != 125 && this.currentCode != 37 && this.currentCode != 64 && this.currentCode != 96)
			{
				throw new YamlTokenizerException(in this.mark, "while scanning an anchor or alias, did not find expected alphabetic or numeric character");
			}
			this.tokens.Enqueue(alias ? new Token(TokenType.Alias, scalar) : new Token(TokenType.Anchor, scalar));
		}

		private unsafe void ConsumeTag()
		{
			this.SaveSimpleKeyCandidate();
			this.simpleKeyAllowed = false;
			Scalar scalar = ScalarPool.Shared.Rent();
			Scalar scalar2 = ScalarPool.Shared.Rent();
			try
			{
				byte b;
				if (this.TryPeek(1L, out b) && b == 60)
				{
					this.Advance(2);
					while (this.TryConsumeUriChar(scalar2))
					{
					}
					if (scalar2.Length <= 0)
					{
						throw new YamlTokenizerException(in this.mark, "While scanning a verbatim tag, did not find valid characters.");
					}
					if (this.currentCode != 62)
					{
						throw new YamlTokenizerException(in this.mark, "While scanning a tag, did not find the expected '>'");
					}
					this.Advance(1);
				}
				else
				{
					this.ConsumeTagHandle(false, scalar);
					Span<byte> span;
					if (scalar.Length >= 2 && *scalar.AsSpan()[span.Length - 1] == 33)
					{
						while (this.TryConsumeTagChar(scalar2))
						{
						}
						if (scalar2.Length <= 0)
						{
							throw new YamlTokenizerException(in this.mark, "While scanning a tag, did not find any tag-shorthand suffix.");
						}
					}
					else
					{
						scalar2.Write(scalar.AsSpan(1, scalar.Length - 1));
						scalar.Clear();
						scalar.Write(33);
						while (this.TryConsumeTagChar(scalar2))
						{
						}
						if (scalar2.Length <= 0)
						{
							Scalar scalar3 = scalar2;
							scalar2 = scalar;
							scalar = scalar3;
						}
					}
				}
				if (!YamlCodes.IsEmpty(this.currentCode) && !this.reader.End && !YamlCodes.IsAnyFlowSymbol(this.currentCode))
				{
					throw new YamlTokenizerException(in this.mark, "While scanning a tag, did not find expected whitespace or line break or flow");
				}
				this.tokens.Enqueue(new Token(TokenType.Tag, new Tag(scalar.ToString(), scalar2.ToString())));
			}
			finally
			{
				ScalarPool.Shared.Return(scalar);
				ScalarPool.Shared.Return(scalar2);
			}
		}

		private unsafe void ConsumeTagHandle(bool directive, Scalar buf)
		{
			if (this.currentCode != 33)
			{
				throw new YamlTokenizerException(in this.mark, "While scanning a tag, did not find expected '!'");
			}
			buf.Write(this.currentCode);
			this.Advance(1);
			while (YamlCodes.IsWordChar(this.currentCode))
			{
				buf.Write(this.currentCode);
				this.Advance(1);
			}
			if (this.currentCode == 33)
			{
				buf.Write(this.currentCode);
				this.Advance(1);
				return;
			}
			if (directive)
			{
				IntPtr intPtr = stackalloc byte[(UIntPtr)1];
				*intPtr = 33;
				Span<byte> span = new Span<byte>(intPtr, 1);
				if (!buf.SequenceEqual(span))
				{
					throw new YamlTokenizerException(in this.mark, "While parsing a tag directive, did not find expected '!'");
				}
			}
		}

		private void ConsumeTagPrefix(Scalar prefix)
		{
			if (this.currentCode == 33)
			{
				prefix.Write(this.currentCode);
				this.Advance(1);
				while (this.TryConsumeUriChar(prefix))
				{
				}
				return;
			}
			if (YamlCodes.IsTagChar(this.currentCode))
			{
				prefix.Write(this.currentCode);
				this.Advance(1);
				while (this.TryConsumeUriChar(prefix))
				{
				}
				return;
			}
			throw new YamlTokenizerException(in this.mark, "While parsing a tag, did not find expected tag prefix");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryConsumeUriChar(Scalar scalar)
		{
			if (this.currentCode == 37)
			{
				scalar.WriteUnicodeCodepoint(this.ConsumeUriEscapes());
				return true;
			}
			if (YamlCodes.IsUriChar(this.currentCode))
			{
				scalar.Write(this.currentCode);
				this.Advance(1);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryConsumeTagChar(Scalar scalar)
		{
			if (this.currentCode == 37)
			{
				scalar.WriteUnicodeCodepoint(this.ConsumeUriEscapes());
				return true;
			}
			if (YamlCodes.IsTagChar(this.currentCode))
			{
				scalar.Write(this.currentCode);
				this.Advance(1);
				return true;
			}
			return false;
		}

		private int ConsumeUriEscapes()
		{
			int num = 0;
			int num2 = 0;
			while (!this.reader.End)
			{
				byte b;
				this.TryPeek(1L, out b);
				byte b2;
				this.TryPeek(2L, out b2);
				if (this.currentCode != 37 || !YamlCodes.IsHex(b) || !YamlCodes.IsHex(b2))
				{
					throw new YamlTokenizerException(in this.mark, "While parsing a tag, did not find URI escaped octet");
				}
				int num3 = ((int)YamlCodes.AsHex(b) << 4) + (int)YamlCodes.AsHex(b2);
				if (num == 0)
				{
					int num4;
					if ((num3 & 128) == 0)
					{
						num4 = 1;
					}
					else if ((num3 & 224) == 192)
					{
						num4 = 2;
					}
					else if ((num3 & 240) == 224)
					{
						num4 = 3;
					}
					else
					{
						if ((num3 & 248) != 240)
						{
							throw new YamlTokenizerException(in this.mark, "While parsing a tag, found an incorrect leading utf8 octet");
						}
						num4 = 4;
					}
					num = num4;
					num2 = num3;
				}
				else
				{
					if ((num3 & 192) != 128)
					{
						throw new YamlTokenizerException(in this.mark, "While parsing a tag, found an incorrect trailing utf8 octet");
					}
					num2 = ((int)this.currentCode << 8) + num3;
				}
				this.Advance(3);
				num--;
				if (num == 0)
				{
					break;
				}
			}
			return num2;
		}

		private void ConsumeBlockScaler(bool literal)
		{
			this.SaveSimpleKeyCandidate();
			this.simpleKeyAllowed = true;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			LineBreakState lineBreakState = LineBreakState.None;
			Scalar scalar = ScalarPool.Shared.Rent();
			if (Utf8YamlTokenizer.lineBreaksBufferStatic == null)
			{
				Utf8YamlTokenizer.lineBreaksBufferStatic = new ExpandBuffer<byte>(64);
			}
			Utf8YamlTokenizer.lineBreaksBufferStatic.Clear();
			this.Advance(1);
			byte b = this.currentCode;
			bool flag2 = b == 43 || b == 45;
			if (flag2)
			{
				num = ((this.currentCode == 43) ? 1 : (-1));
				this.Advance(1);
				if (YamlCodes.IsNumber(this.currentCode))
				{
					if (this.currentCode == 48)
					{
						throw new YamlTokenizerException(in this.mark, "While scanning a block scalar, found an indentation indicator equal to 0");
					}
					num2 = (int)YamlCodes.AsHex(this.currentCode);
					this.Advance(1);
				}
			}
			else if (YamlCodes.IsNumber(this.currentCode))
			{
				if (this.currentCode == 48)
				{
					throw new YamlTokenizerException(in this.mark, "While scanning a block scalar, found an indentation indicator equal to 0");
				}
				num2 = (int)YamlCodes.AsHex(this.currentCode);
				this.Advance(1);
				b = this.currentCode;
				flag2 = b == 43 || b == 45;
				if (flag2)
				{
					num = ((this.currentCode == 43) ? 1 : (-1));
					this.Advance(1);
				}
			}
			while (YamlCodes.IsBlank(this.currentCode))
			{
				this.Advance(1);
			}
			if (this.currentCode == 35)
			{
				while (!this.reader.End && !YamlCodes.IsLineBreak(this.currentCode))
				{
					this.Advance(1);
				}
			}
			if (!this.reader.End && !YamlCodes.IsLineBreak(this.currentCode))
			{
				throw new YamlTokenizerException(in this.mark, "While scanning a block scalar, did not find expected commnet or line break");
			}
			if (YamlCodes.IsLineBreak(this.currentCode))
			{
				this.ConsumeLineBreaks();
			}
			if (num2 > 0)
			{
				num3 = ((this.indent >= 0) ? (this.indent + num2) : num2);
			}
			this.ConsumeBlockScalarBreaks(ref num3, ref Utf8YamlTokenizer.lineBreaksBufferStatic);
			while (this.mark.Col == num3)
			{
				bool flag3 = YamlCodes.IsBlank(this.currentCode);
				if (!literal && lineBreakState != LineBreakState.None && !flag && !flag3)
				{
					if (Utf8YamlTokenizer.lineBreaksBufferStatic.Length <= 0)
					{
						scalar.Write(32);
					}
				}
				else
				{
					scalar.Write(lineBreakState);
				}
				scalar.Write(Utf8YamlTokenizer.lineBreaksBufferStatic.AsSpan());
				flag = YamlCodes.IsBlank(this.currentCode);
				Utf8YamlTokenizer.lineBreaksBufferStatic.Clear();
				while (!this.reader.End && !YamlCodes.IsLineBreak(this.currentCode))
				{
					scalar.Write(this.currentCode);
					this.Advance(1);
				}
				if (this.reader.End)
				{
					lineBreakState = LineBreakState.Lf;
					break;
				}
				lineBreakState = this.ConsumeLineBreaks();
				this.ConsumeBlockScalarBreaks(ref num3, ref Utf8YamlTokenizer.lineBreaksBufferStatic);
			}
			if (num != -1)
			{
				scalar.Write(lineBreakState);
			}
			if (num == 1)
			{
				scalar.Write(Utf8YamlTokenizer.lineBreaksBufferStatic.AsSpan());
			}
			TokenType tokenType = (literal ? TokenType.LiteralScalar : TokenType.FoldedScalar);
			this.tokens.Enqueue(new Token(tokenType, scalar));
		}

		private void ConsumeBlockScalarBreaks(ref int blockIndent, ref ExpandBuffer<byte> blockLineBreaks)
		{
			int num = 0;
			for (;;)
			{
				if ((blockIndent != 0 && this.mark.Col >= blockIndent) || this.currentCode != 32)
				{
					if (this.mark.Col > num)
					{
						num = this.mark.Col;
					}
					if ((blockIndent == 0 || this.mark.Col < blockIndent) && this.currentCode == 9)
					{
						break;
					}
					if (!YamlCodes.IsLineBreak(this.currentCode))
					{
						goto IL_00D0;
					}
					switch (this.ConsumeLineBreaks())
					{
					case LineBreakState.Lf:
						blockLineBreaks.Add(10);
						break;
					case LineBreakState.CrLf:
						blockLineBreaks.Add(13);
						blockLineBreaks.Add(10);
						break;
					case LineBreakState.Cr:
						blockLineBreaks.Add(13);
						break;
					}
				}
				else
				{
					this.Advance(1);
				}
			}
			throw new YamlTokenizerException(in this.mark, "while scanning a block scalar, found a tab character where an indentation space is expected");
			IL_00D0:
			if (blockIndent == 0)
			{
				blockIndent = num;
				if (blockIndent < this.indent + 1)
				{
					blockIndent = this.indent + 1;
					return;
				}
				if (blockIndent < 1)
				{
					blockIndent = 1;
				}
			}
		}

		private unsafe void ConsumeFlowScaler(bool singleQuote)
		{
			this.SaveSimpleKeyCandidate();
			this.simpleKeyAllowed = false;
			LineBreakState lineBreakState = LineBreakState.None;
			LineBreakState lineBreakState2 = LineBreakState.None;
			Scalar scalar = ScalarPool.Shared.Rent();
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)32], 32);
			int num = 0;
			this.Advance(1);
			byte b;
			while (this.mark.Col != 0 || (!this.reader.IsNext(YamlCodes.StreamStart, false) && !this.reader.IsNext(YamlCodes.DocStart, false)) || this.TryPeek(3L, out b))
			{
				if (this.reader.End)
				{
					throw new YamlTokenizerException(in this.mark, "while scanning a quoted scalar, found unexpected end of stream");
				}
				bool flag = false;
				while (!this.reader.End)
				{
					if (YamlCodes.IsEmpty(this.currentCode))
					{
						break;
					}
					b = this.currentCode;
					if (b != 34)
					{
						if (b != 39)
						{
							if (b == 92)
							{
								byte b2;
								if (!singleQuote && this.TryPeek(1L, out b2) && YamlCodes.IsLineBreak(b2))
								{
									this.Advance(1);
									this.ConsumeLineBreaks();
									flag = true;
									continue;
								}
								if (!singleQuote)
								{
									int num2 = 0;
									byte b3;
									this.TryPeek(1L, out b3);
									if (b3 <= 48)
									{
										if (b3 <= 34)
										{
											if (b3 != 32)
											{
												if (b3 != 34)
												{
													goto IL_030F;
												}
												scalar.Write(34);
											}
											else
											{
												scalar.Write(32);
											}
										}
										else if (b3 != 39)
										{
											if (b3 != 48)
											{
												goto IL_030F;
											}
											scalar.Write(0);
										}
										else
										{
											scalar.Write(39);
										}
									}
									else if (b3 <= 85)
									{
										switch (b3)
										{
										case 76:
											scalar.WriteUnicodeCodepoint(8232);
											break;
										case 77:
										case 79:
											goto IL_030F;
										case 78:
											scalar.WriteUnicodeCodepoint(133);
											break;
										case 80:
											scalar.WriteUnicodeCodepoint(8233);
											break;
										default:
											if (b3 != 85)
											{
												goto IL_030F;
											}
											num2 = 8;
											break;
										}
									}
									else
									{
										switch (b3)
										{
										case 92:
											scalar.Write(92);
											break;
										case 93:
										case 94:
										case 96:
										case 99:
										case 100:
											goto IL_030F;
										case 95:
											scalar.WriteUnicodeCodepoint(160);
											break;
										case 97:
											scalar.Write(7);
											break;
										case 98:
											scalar.Write(8);
											break;
										case 101:
											scalar.Write(27);
											break;
										case 102:
											scalar.Write(12);
											break;
										default:
											switch (b3)
											{
											case 110:
												scalar.Write(10);
												break;
											case 111:
											case 112:
											case 113:
											case 115:
											case 119:
												goto IL_030F;
											case 114:
												scalar.Write(13);
												break;
											case 116:
												scalar.Write(9);
												break;
											case 117:
												num2 = 4;
												break;
											case 118:
												scalar.Write(11);
												break;
											case 120:
												num2 = 2;
												break;
											default:
												goto IL_030F;
											}
											break;
										}
									}
									this.Advance(2);
									if (num2 > 0)
									{
										int num3 = 0;
										for (int i = 0; i < num2; i++)
										{
											byte b4;
											if (!this.TryPeek((long)i, out b4) || !YamlCodes.IsHex(b4))
											{
												throw new YamlTokenizerException(in this.mark, "While parsing a quoted scalar, did not find expected hexadecimal number");
											}
											num3 = (num3 << 4) + (int)YamlCodes.AsHex(b4);
										}
										scalar.WriteUnicodeCodepoint(num3);
									}
									this.Advance(num2);
									continue;
									IL_030F:
									throw new YamlTokenizerException(in this.mark, "while parsing a quoted scalar, found unknown escape character");
								}
							}
						}
						else
						{
							byte b5;
							if (this.TryPeek(1L, out b5) && b5 == 39 && singleQuote)
							{
								scalar.Write(39);
								this.Advance(2);
								continue;
							}
							if (singleQuote)
							{
								goto IL_0489;
							}
						}
					}
					else if (!singleQuote)
					{
						goto IL_0489;
					}
					scalar.Write(this.currentCode);
					this.Advance(1);
					continue;
					IL_0489:
					this.Advance(1);
					this.simpleKeyAllowed = flag;
					this.adjacentValueAllowedAt = this.mark.Position;
					this.tokens.Enqueue(new Token(singleQuote ? TokenType.SingleQuotedScaler : TokenType.DoubleQuotedScaler, scalar));
					return;
				}
				while (YamlCodes.IsBlank(this.currentCode) || YamlCodes.IsLineBreak(this.currentCode))
				{
					if (YamlCodes.IsBlank(this.currentCode))
					{
						if (!flag)
						{
							if (span.Length <= num)
							{
								span = new byte[span.Length * 2];
							}
							*span[num++] = this.currentCode;
						}
						this.Advance(1);
					}
					else if (flag)
					{
						lineBreakState2 = this.ConsumeLineBreaks();
					}
					else
					{
						num = 0;
						lineBreakState = this.ConsumeLineBreaks();
						flag = true;
					}
				}
				if (flag)
				{
					if (lineBreakState == LineBreakState.None)
					{
						scalar.Write(lineBreakState2);
						lineBreakState2 = LineBreakState.None;
					}
					else
					{
						if (lineBreakState2 == LineBreakState.None)
						{
							scalar.Write(32);
						}
						else
						{
							scalar.Write(lineBreakState2);
							lineBreakState2 = LineBreakState.None;
						}
						lineBreakState = LineBreakState.None;
					}
				}
				else
				{
					scalar.Write(span.Slice(0, num));
					num = 0;
				}
			}
			throw new YamlTokenizerException(in this.mark, "while scanning a quoted scalar, found unexpected document indicator");
		}

		private unsafe void ConsumePlainScalar()
		{
			this.SaveSimpleKeyCandidate();
			this.simpleKeyAllowed = false;
			int num = this.indent + 1;
			LineBreakState lineBreakState = LineBreakState.None;
			LineBreakState lineBreakState2 = LineBreakState.None;
			bool flag = false;
			Scalar scalar = ScalarPool.Shared.Rent();
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)16], 16);
			int num2 = 0;
			while (this.mark.Col != 0 || ((this.currentCode != 45 || !this.reader.IsNext(YamlCodes.StreamStart, false) || !this.IsEmptyNext(YamlCodes.StreamStart.Length)) && (this.currentCode != 46 || !this.reader.IsNext(YamlCodes.DocStart, false) || !this.IsEmptyNext(YamlCodes.DocStart.Length))))
			{
				if (this.currentCode == 35)
				{
					break;
				}
				while (!this.reader.End && !YamlCodes.IsEmpty(this.currentCode))
				{
					if (this.currentCode == 58)
					{
						byte b;
						if (!this.TryPeek(1L, out b) || YamlCodes.IsEmpty(b))
						{
							break;
						}
						if (this.flowLevel > 0 && YamlCodes.IsAnyFlowSymbol(b))
						{
							break;
						}
					}
					else if (this.flowLevel > 0 && YamlCodes.IsAnyFlowSymbol(this.currentCode))
					{
						break;
					}
					if (flag || num2 > 0)
					{
						if (flag)
						{
							if (lineBreakState == LineBreakState.None)
							{
								scalar.Write(lineBreakState2);
								lineBreakState2 = LineBreakState.None;
							}
							else
							{
								if (lineBreakState2 == LineBreakState.None)
								{
									scalar.Write(32);
								}
								else
								{
									scalar.Write(lineBreakState2);
									lineBreakState2 = LineBreakState.None;
								}
								lineBreakState = LineBreakState.None;
							}
							flag = false;
						}
						else
						{
							scalar.Write(span.Slice(0, num2));
							num2 = 0;
						}
					}
					scalar.Write(this.currentCode);
					this.Advance(1);
				}
				if (!YamlCodes.IsEmpty(this.currentCode))
				{
					break;
				}
				while (YamlCodes.IsEmpty(this.currentCode))
				{
					if (YamlCodes.IsBlank(this.currentCode))
					{
						if (flag && this.mark.Col < num && this.currentCode == 9)
						{
							throw new YamlTokenizerException(in this.mark, "While scanning a plain scaler, found a tab");
						}
						if (!flag)
						{
							if (num2 >= span.Length)
							{
								span = new byte[span.Length * 2];
							}
							*span[num2++] = this.currentCode;
						}
						this.Advance(1);
					}
					else if (flag)
					{
						lineBreakState2 = this.ConsumeLineBreaks();
					}
					else
					{
						lineBreakState = this.ConsumeLineBreaks();
						flag = true;
						num2 = 0;
					}
				}
				if (this.flowLevel == 0 && this.mark.Col < num)
				{
					break;
				}
			}
			this.simpleKeyAllowed = flag;
			this.tokens.Enqueue(new Token(TokenType.PlainScalar, scalar));
		}

		private void SkipToNextToken()
		{
			for (;;)
			{
				byte b = this.currentCode;
				if (b <= 32)
				{
					switch (b)
					{
					case 9:
						if (this.flowLevel <= 0 && this.simpleKeyAllowed)
						{
							return;
						}
						this.Advance(1);
						break;
					case 10:
					case 13:
						this.ConsumeLineBreaks();
						if (this.flowLevel == 0)
						{
							this.simpleKeyAllowed = true;
						}
						break;
					case 11:
					case 12:
						return;
					default:
						if (b != 32)
						{
							return;
						}
						this.Advance(1);
						break;
					}
				}
				else if (b != 35)
				{
					if (b != 239)
					{
						return;
					}
					if (!this.ConsumeBom())
					{
						return;
					}
				}
				else
				{
					while (!this.reader.End)
					{
						if (YamlCodes.IsLineBreak(this.currentCode))
						{
							break;
						}
						this.Advance(1);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Advance(int offset)
		{
			for (int i = 0; i < offset; i++)
			{
				this.mark.Position = this.mark.Position + 1;
				if (this.currentCode == 10)
				{
					this.mark.Line = this.mark.Line + 1;
					this.mark.Col = 0;
				}
				else
				{
					this.mark.Col = this.mark.Col + 1;
				}
				this.reader.Advance(1L);
				this.reader.TryPeek(out this.currentCode);
			}
		}

		private LineBreakState ConsumeLineBreaks()
		{
			if (this.reader.End)
			{
				return LineBreakState.None;
			}
			byte b = this.currentCode;
			if (b == 10)
			{
				this.Advance(1);
				return LineBreakState.Lf;
			}
			if (b != 13)
			{
				return LineBreakState.None;
			}
			byte b2;
			if (this.TryPeek(1L, out b2) && b2 == 10)
			{
				this.Advance(2);
				return LineBreakState.CrLf;
			}
			this.Advance(1);
			return LineBreakState.Cr;
		}

		private void StaleSimpleKeyCandidates()
		{
			for (int i = 0; i < this.simpleKeyCandidates.Length; i++)
			{
				ref SimpleKeyState ptr = ref this.simpleKeyCandidates[i];
				if (ptr.Possible && (ptr.Start.Line < this.mark.Line || ptr.Start.Position + 1024 < this.mark.Position))
				{
					if (ptr.Required)
					{
						throw new YamlTokenizerException(in this.mark, "Simple key expect ':'");
					}
					ptr.Possible = false;
				}
			}
		}

		private unsafe void SaveSimpleKeyCandidate()
		{
			if (!this.simpleKeyAllowed)
			{
				return;
			}
			ExpandBuffer<SimpleKeyState> expandBuffer = this.simpleKeyCandidates;
			SimpleKeyState simpleKeyState = *expandBuffer[expandBuffer.Length - 1];
			if (simpleKeyState.Possible && simpleKeyState.Required)
			{
				throw new YamlTokenizerException(in this.mark, "Simple key expected");
			}
			ExpandBuffer<SimpleKeyState> expandBuffer2 = this.simpleKeyCandidates;
			ref SimpleKeyState ptr = ref expandBuffer2[expandBuffer2.Length - 1];
			simpleKeyState = new SimpleKeyState
			{
				Start = this.mark,
				Possible = true,
				Required = (this.flowLevel > 0 && this.indent == this.mark.Col),
				TokenNumber = this.tokensParsed + this.tokens.Count
			};
			ptr = simpleKeyState;
		}

		private void RemoveSimpleKeyCandidate()
		{
			ExpandBuffer<SimpleKeyState> expandBuffer = this.simpleKeyCandidates;
			ref SimpleKeyState ptr = ref expandBuffer[expandBuffer.Length - 1];
			SimpleKeyState simpleKeyState = ptr;
			if (simpleKeyState.Possible && simpleKeyState.Required)
			{
				throw new YamlTokenizerException(in this.mark, "Simple key expected");
			}
			ptr.Possible = false;
		}

		private void RollIndent(int colTo, in Token nextToken, int insertNumber = -1)
		{
			if (this.flowLevel > 0 || this.indent >= colTo)
			{
				return;
			}
			this.indents.Add(this.indent);
			this.indent = colTo;
			if (insertNumber >= 0)
			{
				this.tokens.Insert(insertNumber - this.tokensParsed, nextToken);
				return;
			}
			this.tokens.Enqueue(nextToken);
		}

		private unsafe void UnrollIndent(int col)
		{
			if (this.flowLevel > 0)
			{
				return;
			}
			while (this.indent > col)
			{
				this.tokens.Enqueue(new Token(TokenType.BlockEnd, null));
				this.indent = *this.indents.Pop();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void IncreaseFlowLevel()
		{
			this.simpleKeyCandidates.Add(default(SimpleKeyState));
			this.flowLevel++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DecreaseFlowLevel()
		{
			if (this.flowLevel <= 0)
			{
				return;
			}
			this.flowLevel--;
			this.simpleKeyCandidates.Pop();
		}

		private unsafe readonly bool IsEmptyNext(int offset)
		{
			if (this.reader.End || this.reader.Remaining <= (long)offset)
			{
				return true;
			}
			if (this.reader.CurrentSpanIndex + offset <= this.reader.CurrentSpan.Length - 1)
			{
				return YamlCodes.IsEmpty(*this.reader.CurrentSpan[this.reader.CurrentSpanIndex + offset]);
			}
			int num = offset;
			SequencePosition position = this.reader.Position;
			ReadOnlyMemory<byte> readOnlyMemory;
			while (this.reader.Sequence.TryGet(ref position, out readOnlyMemory, true))
			{
				if (readOnlyMemory.Length > 0)
				{
					if (num < readOnlyMemory.Length)
					{
						break;
					}
					num -= readOnlyMemory.Length;
				}
			}
			return YamlCodes.IsEmpty(*readOnlyMemory.Span[num]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe readonly bool TryPeek(long offset, out byte value)
		{
			if (this.reader.End || this.reader.Remaining <= offset)
			{
				value = 0;
				return false;
			}
			if ((long)this.reader.CurrentSpanIndex + offset <= (long)(this.reader.CurrentSpan.Length - 1))
			{
				value = *this.reader.CurrentSpan[this.reader.CurrentSpanIndex + (int)offset];
				return true;
			}
			long num = offset;
			SequencePosition position = this.reader.Position;
			ReadOnlyMemory<byte> readOnlyMemory;
			while (this.reader.Sequence.TryGet(ref position, out readOnlyMemory, true))
			{
				if (readOnlyMemory.Length > 0)
				{
					if (num < (long)readOnlyMemory.Length)
					{
						break;
					}
					num -= (long)readOnlyMemory.Length;
				}
			}
			value = *readOnlyMemory.Span[(int)num];
			return true;
		}

		[Nullable(2)]
		[ThreadStatic]
		private static InsertionQueue<Token> tokensBufferStatic;

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<SimpleKeyState> simpleKeyBufferStatic;

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<int> indentsBufferStatic;

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<byte> lineBreaksBufferStatic;

		[Nullable(0)]
		private SequenceReader<byte> reader;

		private Marker mark;

		private Token currentToken;

		private bool streamStartProduced;

		private bool streamEndProduced;

		private byte currentCode;

		private int indent;

		private bool simpleKeyAllowed;

		private int adjacentValueAllowedAt;

		private int flowLevel;

		private int tokensParsed;

		private bool tokenAvailable;

		private readonly InsertionQueue<Token> tokens;

		private readonly ExpandBuffer<SimpleKeyState> simpleKeyCandidates;

		private readonly ExpandBuffer<int> indents;
	}
}
