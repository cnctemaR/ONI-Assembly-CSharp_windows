using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Scanner : IScanner
	{
		public bool SkipComments { get; private set; }

		public Token Current { get; private set; }

		public Scanner(TextReader input, bool skipComments = true)
		{
			this.analyzer = new CharacterAnalyzer<LookAheadBuffer>(new LookAheadBuffer(input, 8));
			this.cursor = new Cursor();
			this.SkipComments = skipComments;
		}

		public Mark CurrentPosition
		{
			get
			{
				return this.cursor.Mark();
			}
		}

		public bool MoveNext()
		{
			if (this.Current != null)
			{
				this.ConsumeCurrent();
			}
			return this.MoveNextWithoutConsuming();
		}

		public bool MoveNextWithoutConsuming()
		{
			if (!this.tokenAvailable && !this.streamEndProduced)
			{
				this.FetchMoreTokens();
			}
			if (this.tokens.Count > 0)
			{
				this.Current = this.tokens.Dequeue();
				this.tokenAvailable = false;
				return true;
			}
			this.Current = null;
			return false;
		}

		public void ConsumeCurrent()
		{
			this.tokensParsed++;
			this.tokenAvailable = false;
			this.previous = this.Current;
			this.Current = null;
		}

		private char ReadCurrentCharacter()
		{
			char c = this.analyzer.Peek(0);
			this.Skip();
			return c;
		}

		private char ReadLine()
		{
			if (this.analyzer.Check("\r\n\u0085", 0))
			{
				this.SkipLine();
				return '\n';
			}
			char c = this.analyzer.Peek(0);
			this.SkipLine();
			return c;
		}

		private void FetchMoreTokens()
		{
			for (;;)
			{
				bool flag = false;
				if (this.tokens.Count == 0)
				{
					flag = true;
				}
				else
				{
					this.StaleSimpleKeys();
					foreach (SimpleKey simpleKey in this.simpleKeys)
					{
						if (simpleKey.IsPossible && simpleKey.TokenNumber == this.tokensParsed)
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
				this.FetchNextToken();
			}
			this.tokenAvailable = true;
		}

		private static bool StartsWith(StringBuilder what, char start)
		{
			return what.Length > 0 && what[0] == start;
		}

		private void StaleSimpleKeys()
		{
			foreach (SimpleKey simpleKey in this.simpleKeys)
			{
				if (simpleKey.IsPossible && (simpleKey.Line < this.cursor.Line || simpleKey.Index + 1024 < this.cursor.Index))
				{
					if (simpleKey.IsRequired)
					{
						Mark mark = this.cursor.Mark();
						throw new SyntaxErrorException(mark, mark, "While scanning a simple key, could not find expected ':'.");
					}
					simpleKey.IsPossible = false;
				}
			}
		}

		private void FetchNextToken()
		{
			if (!this.streamStartProduced)
			{
				this.FetchStreamStart();
				return;
			}
			this.ScanToNextToken();
			this.StaleSimpleKeys();
			this.UnrollIndent(this.cursor.LineOffset);
			this.analyzer.Buffer.Cache(4);
			if (this.analyzer.Buffer.EndOfInput)
			{
				this.FetchStreamEnd();
				return;
			}
			if (this.cursor.LineOffset == 0 && this.analyzer.Check('%', 0))
			{
				this.FetchDirective();
				return;
			}
			if (this.cursor.LineOffset == 0 && this.analyzer.Check('-', 0) && this.analyzer.Check('-', 1) && this.analyzer.Check('-', 2) && this.analyzer.IsWhiteBreakOrZero(3))
			{
				this.FetchDocumentIndicator(true);
				return;
			}
			if (this.cursor.LineOffset == 0 && this.analyzer.Check('.', 0) && this.analyzer.Check('.', 1) && this.analyzer.Check('.', 2) && this.analyzer.IsWhiteBreakOrZero(3))
			{
				this.FetchDocumentIndicator(false);
				return;
			}
			if (this.analyzer.Check('[', 0))
			{
				this.FetchFlowCollectionStart(true);
				return;
			}
			if (this.analyzer.Check('{', 0))
			{
				this.FetchFlowCollectionStart(false);
				return;
			}
			if (this.analyzer.Check(']', 0))
			{
				this.FetchFlowCollectionEnd(true);
				return;
			}
			if (this.analyzer.Check('}', 0))
			{
				this.FetchFlowCollectionEnd(false);
				return;
			}
			if (this.analyzer.Check(',', 0))
			{
				this.FetchFlowEntry();
				return;
			}
			if (this.analyzer.Check('-', 0) && this.analyzer.IsWhiteBreakOrZero(1))
			{
				this.FetchBlockEntry();
				return;
			}
			if (this.analyzer.Check('?', 0) && (this.flowLevel > 0 || this.analyzer.IsWhiteBreakOrZero(1)))
			{
				this.FetchKey();
				return;
			}
			if (this.analyzer.Check(':', 0) && (this.flowLevel > 0 || this.analyzer.IsWhiteBreakOrZero(1)))
			{
				this.FetchValue();
				return;
			}
			if (this.analyzer.Check('*', 0))
			{
				this.FetchAnchor(true);
				return;
			}
			if (this.analyzer.Check('&', 0))
			{
				this.FetchAnchor(false);
				return;
			}
			if (this.analyzer.Check('!', 0))
			{
				this.FetchTag();
				return;
			}
			if (this.analyzer.Check('|', 0) && this.flowLevel == 0)
			{
				this.FetchBlockScalar(true);
				return;
			}
			if (this.analyzer.Check('>', 0) && this.flowLevel == 0)
			{
				this.FetchBlockScalar(false);
				return;
			}
			if (this.analyzer.Check('\'', 0))
			{
				this.FetchFlowScalar(true);
				return;
			}
			if (this.analyzer.Check('"', 0))
			{
				this.FetchFlowScalar(false);
				return;
			}
			if ((!this.analyzer.IsWhiteBreakOrZero(0) && !this.analyzer.Check("-?:,[]{}#&*!|>'\"%@`", 0)) || (this.analyzer.Check('-', 0) && !this.analyzer.IsWhite(1)) || (this.flowLevel == 0 && this.analyzer.Check("?:", 0) && !this.analyzer.IsWhiteBreakOrZero(1)))
			{
				this.FetchPlainScalar();
				return;
			}
			Mark mark = this.cursor.Mark();
			this.Skip();
			Mark mark2 = this.cursor.Mark();
			throw new SyntaxErrorException(mark, mark2, "While scanning for the next token, find character that cannot start any token.");
		}

		private bool CheckWhiteSpace()
		{
			return this.analyzer.Check(' ', 0) || ((this.flowLevel > 0 || !this.simpleKeyAllowed) && this.analyzer.Check('\t', 0));
		}

		private bool IsDocumentIndicator()
		{
			if (this.cursor.LineOffset == 0 && this.analyzer.IsWhiteBreakOrZero(3))
			{
				bool flag = this.analyzer.Check('-', 0) && this.analyzer.Check('-', 1) && this.analyzer.Check('-', 2);
				bool flag2 = this.analyzer.Check('.', 0) && this.analyzer.Check('.', 1) && this.analyzer.Check('.', 2);
				return flag || flag2;
			}
			return false;
		}

		private void Skip()
		{
			this.cursor.Skip();
			this.analyzer.Buffer.Skip(1);
		}

		private void SkipLine()
		{
			if (this.analyzer.IsCrLf(0))
			{
				this.cursor.SkipLineByOffset(2);
				this.analyzer.Buffer.Skip(2);
				return;
			}
			if (this.analyzer.IsBreak(0))
			{
				this.cursor.SkipLineByOffset(1);
				this.analyzer.Buffer.Skip(1);
				return;
			}
			if (!this.analyzer.IsZero(0))
			{
				throw new InvalidOperationException("Not at a break.");
			}
		}

		private void ScanToNextToken()
		{
			for (;;)
			{
				if (!this.CheckWhiteSpace())
				{
					this.ProcessComment();
					if (!this.analyzer.IsBreak(0))
					{
						break;
					}
					this.SkipLine();
					if (this.flowLevel == 0)
					{
						this.simpleKeyAllowed = true;
					}
				}
				else
				{
					this.Skip();
				}
			}
		}

		private void ProcessComment()
		{
			if (this.analyzer.Check('#', 0))
			{
				Mark mark = this.cursor.Mark();
				this.Skip();
				while (this.analyzer.IsSpace(0))
				{
					this.Skip();
				}
				StringBuilder stringBuilder = new StringBuilder();
				while (!this.analyzer.IsBreakOrZero(0))
				{
					stringBuilder.Append(this.ReadCurrentCharacter());
				}
				if (!this.SkipComments)
				{
					bool flag = this.previous != null && this.previous.End.Line == mark.Line && !(this.previous is StreamStart);
					this.tokens.Enqueue(new Comment(stringBuilder.ToString(), flag, mark, this.cursor.Mark()));
				}
			}
		}

		private void FetchStreamStart()
		{
			this.simpleKeys.Push(new SimpleKey());
			this.simpleKeyAllowed = true;
			this.streamStartProduced = true;
			Mark mark = this.cursor.Mark();
			this.tokens.Enqueue(new StreamStart(mark, mark));
		}

		private void UnrollIndent(int column)
		{
			if (this.flowLevel != 0)
			{
				return;
			}
			while (this.indent > column)
			{
				Mark mark = this.cursor.Mark();
				this.tokens.Enqueue(new BlockEnd(mark, mark));
				this.indent = this.indents.Pop();
			}
		}

		private void FetchStreamEnd()
		{
			this.cursor.ForceSkipLineAfterNonBreak();
			this.UnrollIndent(-1);
			this.RemoveSimpleKey();
			this.simpleKeyAllowed = false;
			this.streamEndProduced = true;
			Mark mark = this.cursor.Mark();
			this.tokens.Enqueue(new StreamEnd(mark, mark));
		}

		private void FetchDirective()
		{
			this.UnrollIndent(-1);
			this.RemoveSimpleKey();
			this.simpleKeyAllowed = false;
			Token token = this.ScanDirective();
			this.tokens.Enqueue(token);
		}

		private Token ScanDirective()
		{
			Mark mark = this.cursor.Mark();
			this.Skip();
			string text = this.ScanDirectiveName(mark);
			Token token;
			if (!(text == "YAML"))
			{
				if (!(text == "TAG"))
				{
					throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a directive, find uknown directive name.");
				}
				token = this.ScanTagDirectiveValue(mark);
			}
			else
			{
				token = this.ScanVersionDirectiveValue(mark);
			}
			while (this.analyzer.IsWhite(0))
			{
				this.Skip();
			}
			this.ProcessComment();
			if (!this.analyzer.IsBreakOrZero(0))
			{
				throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a directive, did not find expected comment or line break.");
			}
			if (this.analyzer.IsBreak(0))
			{
				this.SkipLine();
			}
			return token;
		}

		private void FetchDocumentIndicator(bool isStartToken)
		{
			this.UnrollIndent(-1);
			this.RemoveSimpleKey();
			this.simpleKeyAllowed = false;
			Mark mark = this.cursor.Mark();
			this.Skip();
			this.Skip();
			this.Skip();
			Token token = (isStartToken ? new DocumentStart(mark, this.cursor.Mark()) : new DocumentEnd(mark, mark));
			this.tokens.Enqueue(token);
		}

		private void FetchFlowCollectionStart(bool isSequenceToken)
		{
			this.SaveSimpleKey();
			this.IncreaseFlowLevel();
			this.simpleKeyAllowed = true;
			Mark mark = this.cursor.Mark();
			this.Skip();
			Token token;
			if (isSequenceToken)
			{
				token = new FlowSequenceStart(mark, mark);
			}
			else
			{
				token = new FlowMappingStart(mark, mark);
			}
			this.tokens.Enqueue(token);
		}

		private void IncreaseFlowLevel()
		{
			this.simpleKeys.Push(new SimpleKey());
			this.flowLevel++;
		}

		private void FetchFlowCollectionEnd(bool isSequenceToken)
		{
			this.RemoveSimpleKey();
			this.DecreaseFlowLevel();
			this.simpleKeyAllowed = false;
			Mark mark = this.cursor.Mark();
			this.Skip();
			Token token;
			if (isSequenceToken)
			{
				token = new FlowSequenceEnd(mark, mark);
			}
			else
			{
				token = new FlowMappingEnd(mark, mark);
			}
			this.tokens.Enqueue(token);
		}

		private void DecreaseFlowLevel()
		{
			if (this.flowLevel > 0)
			{
				this.flowLevel--;
				this.simpleKeys.Pop();
			}
		}

		private void FetchFlowEntry()
		{
			this.RemoveSimpleKey();
			this.simpleKeyAllowed = true;
			Mark mark = this.cursor.Mark();
			this.Skip();
			this.tokens.Enqueue(new FlowEntry(mark, this.cursor.Mark()));
		}

		private void FetchBlockEntry()
		{
			if (this.flowLevel == 0)
			{
				if (!this.simpleKeyAllowed)
				{
					Mark mark = this.cursor.Mark();
					throw new SyntaxErrorException(mark, mark, "Block sequence entries are not allowed in this context.");
				}
				this.RollIndent(this.cursor.LineOffset, -1, true, this.cursor.Mark());
			}
			this.RemoveSimpleKey();
			this.simpleKeyAllowed = true;
			Mark mark2 = this.cursor.Mark();
			this.Skip();
			this.tokens.Enqueue(new BlockEntry(mark2, this.cursor.Mark()));
		}

		private void FetchKey()
		{
			if (this.flowLevel == 0)
			{
				if (!this.simpleKeyAllowed)
				{
					Mark mark = this.cursor.Mark();
					throw new SyntaxErrorException(mark, mark, "Mapping keys are not allowed in this context.");
				}
				this.RollIndent(this.cursor.LineOffset, -1, false, this.cursor.Mark());
			}
			this.RemoveSimpleKey();
			this.simpleKeyAllowed = this.flowLevel == 0;
			Mark mark2 = this.cursor.Mark();
			this.Skip();
			this.tokens.Enqueue(new Key(mark2, this.cursor.Mark()));
		}

		private void FetchValue()
		{
			SimpleKey simpleKey = this.simpleKeys.Peek();
			if (simpleKey.IsPossible)
			{
				this.tokens.Insert(simpleKey.TokenNumber - this.tokensParsed, new Key(simpleKey.Mark, simpleKey.Mark));
				this.RollIndent(simpleKey.LineOffset, simpleKey.TokenNumber, false, simpleKey.Mark);
				simpleKey.IsPossible = false;
				this.simpleKeyAllowed = false;
			}
			else
			{
				if (this.flowLevel == 0)
				{
					if (!this.simpleKeyAllowed)
					{
						Mark mark = this.cursor.Mark();
						throw new SyntaxErrorException(mark, mark, "Mapping values are not allowed in this context.");
					}
					this.RollIndent(this.cursor.LineOffset, -1, false, this.cursor.Mark());
				}
				this.simpleKeyAllowed = this.flowLevel == 0;
			}
			Mark mark2 = this.cursor.Mark();
			this.Skip();
			this.tokens.Enqueue(new Value(mark2, this.cursor.Mark()));
		}

		private void RollIndent(int column, int number, bool isSequence, Mark position)
		{
			if (this.flowLevel > 0)
			{
				return;
			}
			if (this.indent < column)
			{
				this.indents.Push(this.indent);
				this.indent = column;
				Token token;
				if (isSequence)
				{
					token = new BlockSequenceStart(position, position);
				}
				else
				{
					token = new BlockMappingStart(position, position);
				}
				if (number == -1)
				{
					this.tokens.Enqueue(token);
					return;
				}
				this.tokens.Insert(number - this.tokensParsed, token);
			}
		}

		private void FetchAnchor(bool isAlias)
		{
			this.SaveSimpleKey();
			this.simpleKeyAllowed = false;
			this.tokens.Enqueue(this.ScanAnchor(isAlias));
		}

		private Token ScanAnchor(bool isAlias)
		{
			Mark mark = this.cursor.Mark();
			this.Skip();
			StringBuilder stringBuilder = new StringBuilder();
			while (this.analyzer.IsAlphaNumericDashOrUnderscore(0))
			{
				stringBuilder.Append(this.ReadCurrentCharacter());
			}
			if (stringBuilder.Length == 0 || (!this.analyzer.IsWhiteBreakOrZero(0) && !this.analyzer.Check("?:,]}%@`", 0)))
			{
				throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning an anchor or alias, did not find expected alphabetic or numeric character.");
			}
			if (isAlias)
			{
				return new AnchorAlias(stringBuilder.ToString(), mark, this.cursor.Mark());
			}
			return new Anchor(stringBuilder.ToString(), mark, this.cursor.Mark());
		}

		private void FetchTag()
		{
			this.SaveSimpleKey();
			this.simpleKeyAllowed = false;
			this.tokens.Enqueue(this.ScanTag());
		}

		private Token ScanTag()
		{
			Mark mark = this.cursor.Mark();
			string text;
			string text2;
			if (this.analyzer.Check('<', 1))
			{
				text = string.Empty;
				this.Skip();
				this.Skip();
				text2 = this.ScanTagUri(null, mark);
				if (!this.analyzer.Check('>', 0))
				{
					throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a tag, did not find the expected '>'.");
				}
				this.Skip();
			}
			else
			{
				string text3 = this.ScanTagHandle(false, mark);
				if (text3.Length > 1 && text3[0] == '!' && text3[text3.Length - 1] == '!')
				{
					text = text3;
					text2 = this.ScanTagUri(null, mark);
				}
				else
				{
					text2 = this.ScanTagUri(text3, mark);
					text = "!";
					if (text2.Length == 0)
					{
						text2 = text;
						text = string.Empty;
					}
				}
			}
			if (!this.analyzer.IsWhiteBreakOrZero(0))
			{
				throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a tag, did not find expected whitespace or line break.");
			}
			return new Tag(text, text2, mark, this.cursor.Mark());
		}

		private void FetchBlockScalar(bool isLiteral)
		{
			this.RemoveSimpleKey();
			this.simpleKeyAllowed = true;
			this.tokens.Enqueue(this.ScanBlockScalar(isLiteral));
		}

		private Token ScanBlockScalar(bool isLiteral)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			Mark mark = this.cursor.Mark();
			this.Skip();
			if (this.analyzer.Check("+-", 0))
			{
				num = (this.analyzer.Check('+', 0) ? 1 : (-1));
				this.Skip();
				if (this.analyzer.IsDigit(0))
				{
					if (this.analyzer.Check('0', 0))
					{
						throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a block scalar, find an intendation indicator equal to 0.");
					}
					num2 = this.analyzer.AsDigit(0);
					this.Skip();
				}
			}
			else if (this.analyzer.IsDigit(0))
			{
				if (this.analyzer.Check('0', 0))
				{
					throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a block scalar, find an intendation indicator equal to 0.");
				}
				num2 = this.analyzer.AsDigit(0);
				this.Skip();
				if (this.analyzer.Check("+-", 0))
				{
					num = (this.analyzer.Check('+', 0) ? 1 : (-1));
					this.Skip();
				}
			}
			while (this.analyzer.IsWhite(0))
			{
				this.Skip();
			}
			this.ProcessComment();
			if (!this.analyzer.IsBreakOrZero(0))
			{
				throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a block scalar, did not find expected comment or line break.");
			}
			if (this.analyzer.IsBreak(0))
			{
				this.SkipLine();
			}
			Mark mark2 = this.cursor.Mark();
			if (num2 != 0)
			{
				num3 = ((this.indent >= 0) ? (this.indent + num2) : num2);
			}
			num3 = this.ScanBlockScalarBreaks(num3, stringBuilder3, mark, ref mark2);
			while (this.cursor.LineOffset == num3 && !this.analyzer.IsZero(0))
			{
				bool flag2 = this.analyzer.IsWhite(0);
				if (!isLiteral && Scanner.StartsWith(stringBuilder2, '\n') && !flag && !flag2)
				{
					if (stringBuilder3.Length == 0)
					{
						stringBuilder.Append(' ');
					}
					stringBuilder2.Length = 0;
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
					stringBuilder2.Length = 0;
				}
				stringBuilder.Append(stringBuilder3.ToString());
				stringBuilder3.Length = 0;
				flag = this.analyzer.IsWhite(0);
				while (!this.analyzer.IsBreakOrZero(0))
				{
					stringBuilder.Append(this.ReadCurrentCharacter());
				}
				char c = this.ReadLine();
				if (c != '\0')
				{
					stringBuilder2.Append(c);
				}
				num3 = this.ScanBlockScalarBreaks(num3, stringBuilder3, mark, ref mark2);
			}
			if (num != -1)
			{
				stringBuilder.Append(stringBuilder2);
			}
			if (num == 1)
			{
				stringBuilder.Append(stringBuilder3);
			}
			ScalarStyle scalarStyle = (isLiteral ? ScalarStyle.Literal : ScalarStyle.Folded);
			return new Scalar(stringBuilder.ToString(), scalarStyle, mark, mark2);
		}

		private int ScanBlockScalarBreaks(int currentIndent, StringBuilder breaks, Mark start, ref Mark end)
		{
			int num = 0;
			end = this.cursor.Mark();
			for (;;)
			{
				if ((currentIndent != 0 && this.cursor.LineOffset >= currentIndent) || !this.analyzer.IsSpace(0))
				{
					if (this.cursor.LineOffset > num)
					{
						num = this.cursor.LineOffset;
					}
					if ((currentIndent == 0 || this.cursor.LineOffset < currentIndent) && this.analyzer.IsTab(0))
					{
						break;
					}
					if (!this.analyzer.IsBreak(0))
					{
						goto IL_00B5;
					}
					breaks.Append(this.ReadLine());
					end = this.cursor.Mark();
				}
				else
				{
					this.Skip();
				}
			}
			throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a block scalar, find a tab character where an intendation space is expected.");
			IL_00B5:
			if (currentIndent == 0)
			{
				currentIndent = Math.Max(num, Math.Max(this.indent + 1, 1));
			}
			return currentIndent;
		}

		private void FetchFlowScalar(bool isSingleQuoted)
		{
			this.SaveSimpleKey();
			this.simpleKeyAllowed = false;
			this.tokens.Enqueue(this.ScanFlowScalar(isSingleQuoted));
		}

		private Token ScanFlowScalar(bool isSingleQuoted)
		{
			Mark mark = this.cursor.Mark();
			this.Skip();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			while (!this.IsDocumentIndicator())
			{
				if (this.analyzer.IsZero(0))
				{
					throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a quoted scalar, find unexpected end of stream.");
				}
				bool flag = false;
				while (!this.analyzer.IsWhiteBreakOrZero(0))
				{
					if (isSingleQuoted && this.analyzer.Check('\'', 0) && this.analyzer.Check('\'', 1))
					{
						stringBuilder.Append('\'');
						this.Skip();
						this.Skip();
					}
					else
					{
						if (this.analyzer.Check(isSingleQuoted ? '\'' : '"', 0))
						{
							break;
						}
						if (!isSingleQuoted && this.analyzer.Check('\\', 0) && this.analyzer.IsBreak(1))
						{
							this.Skip();
							this.SkipLine();
							flag = true;
							break;
						}
						if (!isSingleQuoted && this.analyzer.Check('\\', 0))
						{
							int num = 0;
							char c = this.analyzer.Peek(1);
							if (c != 'U')
							{
								if (c != 'u')
								{
									if (c == 'x')
									{
										num = 2;
									}
									else
									{
										char c2;
										if (!Scanner.simpleEscapeCodes.TryGetValue(c, out c2))
										{
											throw new SyntaxErrorException(mark, this.cursor.Mark(), "While parsing a quoted scalar, find unknown escape character.");
										}
										stringBuilder.Append(c2);
									}
								}
								else
								{
									num = 4;
								}
							}
							else
							{
								num = 8;
							}
							this.Skip();
							this.Skip();
							if (num > 0)
							{
								int num2 = 0;
								for (int i = 0; i < num; i++)
								{
									if (!this.analyzer.IsHex(i))
									{
										throw new SyntaxErrorException(mark, this.cursor.Mark(), "While parsing a quoted scalar, did not find expected hexdecimal number.");
									}
									num2 = (num2 << 4) + this.analyzer.AsHex(i);
								}
								if ((num2 >= 55296 && num2 <= 57343) || num2 > 1114111)
								{
									throw new SyntaxErrorException(mark, this.cursor.Mark(), "While parsing a quoted scalar, find invalid Unicode character escape code.");
								}
								stringBuilder.Append(char.ConvertFromUtf32(num2));
								for (int j = 0; j < num; j++)
								{
									this.Skip();
								}
							}
						}
						else
						{
							stringBuilder.Append(this.ReadCurrentCharacter());
						}
					}
				}
				if (this.analyzer.Check(isSingleQuoted ? '\'' : '"', 0))
				{
					this.Skip();
					return new Scalar(stringBuilder.ToString(), isSingleQuoted ? ScalarStyle.SingleQuoted : ScalarStyle.DoubleQuoted, mark, this.cursor.Mark());
				}
				while (this.analyzer.IsWhite(0) || this.analyzer.IsBreak(0))
				{
					if (this.analyzer.IsWhite(0))
					{
						if (!flag)
						{
							stringBuilder2.Append(this.ReadCurrentCharacter());
						}
						else
						{
							this.Skip();
						}
					}
					else if (!flag)
					{
						stringBuilder2.Length = 0;
						stringBuilder3.Append(this.ReadLine());
						flag = true;
					}
					else
					{
						stringBuilder4.Append(this.ReadLine());
					}
				}
				if (flag)
				{
					if (Scanner.StartsWith(stringBuilder3, '\n'))
					{
						if (stringBuilder4.Length == 0)
						{
							stringBuilder.Append(' ');
						}
						else
						{
							stringBuilder.Append(stringBuilder4.ToString());
						}
					}
					else
					{
						stringBuilder.Append(stringBuilder3.ToString());
						stringBuilder.Append(stringBuilder4.ToString());
					}
					stringBuilder3.Length = 0;
					stringBuilder4.Length = 0;
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
					stringBuilder2.Length = 0;
				}
			}
			throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a quoted scalar, find unexpected document indicator.");
		}

		private void FetchPlainScalar()
		{
			this.SaveSimpleKey();
			this.simpleKeyAllowed = false;
			this.tokens.Enqueue(this.ScanPlainScalar());
		}

		private Token ScanPlainScalar()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			bool flag = false;
			int num = this.indent + 1;
			Mark mark = this.cursor.Mark();
			Mark mark2 = mark;
			while (!this.IsDocumentIndicator())
			{
				if (this.analyzer.Check('#', 0))
				{
					break;
				}
				while (!this.analyzer.IsWhiteBreakOrZero(0))
				{
					if (this.flowLevel > 0 && this.analyzer.Check(':', 0) && !this.analyzer.IsWhiteBreakOrZero(1))
					{
						throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a plain scalar, find unexpected ':'.");
					}
					if ((this.analyzer.Check(':', 0) && this.analyzer.IsWhiteBreakOrZero(1)) || (this.flowLevel > 0 && this.analyzer.Check(",:?[]{}", 0)))
					{
						break;
					}
					if (flag || stringBuilder2.Length > 0)
					{
						if (flag)
						{
							if (Scanner.StartsWith(stringBuilder3, '\n'))
							{
								if (stringBuilder4.Length == 0)
								{
									stringBuilder.Append(' ');
								}
								else
								{
									stringBuilder.Append(stringBuilder4);
								}
							}
							else
							{
								stringBuilder.Append(stringBuilder3);
								stringBuilder.Append(stringBuilder4);
							}
							stringBuilder3.Length = 0;
							stringBuilder4.Length = 0;
							flag = false;
						}
						else
						{
							stringBuilder.Append(stringBuilder2);
							stringBuilder2.Length = 0;
						}
					}
					stringBuilder.Append(this.ReadCurrentCharacter());
					mark2 = this.cursor.Mark();
				}
				if (!this.analyzer.IsWhite(0) && !this.analyzer.IsBreak(0))
				{
					break;
				}
				while (this.analyzer.IsWhite(0) || this.analyzer.IsBreak(0))
				{
					if (this.analyzer.IsWhite(0))
					{
						if (flag && this.cursor.LineOffset < num && this.analyzer.IsTab(0))
						{
							throw new SyntaxErrorException(mark, this.cursor.Mark(), "While scanning a plain scalar, find a tab character that violate intendation.");
						}
						if (!flag)
						{
							stringBuilder2.Append(this.ReadCurrentCharacter());
						}
						else
						{
							this.Skip();
						}
					}
					else if (!flag)
					{
						stringBuilder2.Length = 0;
						stringBuilder3.Append(this.ReadLine());
						flag = true;
					}
					else
					{
						stringBuilder4.Append(this.ReadLine());
					}
				}
				if (this.flowLevel == 0 && this.cursor.LineOffset < num)
				{
					break;
				}
			}
			if (flag)
			{
				this.simpleKeyAllowed = true;
			}
			return new Scalar(stringBuilder.ToString(), ScalarStyle.Plain, mark, mark2);
		}

		private void RemoveSimpleKey()
		{
			SimpleKey simpleKey = this.simpleKeys.Peek();
			if (simpleKey.IsPossible && simpleKey.IsRequired)
			{
				throw new SyntaxErrorException(simpleKey.Mark, simpleKey.Mark, "While scanning a simple key, could not find expected ':'.");
			}
			simpleKey.IsPossible = false;
		}

		private string ScanDirectiveName(Mark start)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (this.analyzer.IsAlphaNumericDashOrUnderscore(0))
			{
				stringBuilder.Append(this.ReadCurrentCharacter());
			}
			if (stringBuilder.Length == 0)
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a directive, could not find expected directive name.");
			}
			if (!this.analyzer.IsWhiteBreakOrZero(0))
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a directive, find unexpected non-alphabetical character.");
			}
			return stringBuilder.ToString();
		}

		private void SkipWhitespaces()
		{
			while (this.analyzer.IsWhite(0))
			{
				this.Skip();
			}
		}

		private Token ScanVersionDirectiveValue(Mark start)
		{
			this.SkipWhitespaces();
			int num = this.ScanVersionDirectiveNumber(start);
			if (!this.analyzer.Check('.', 0))
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %YAML directive, did not find expected digit or '.' character.");
			}
			this.Skip();
			int num2 = this.ScanVersionDirectiveNumber(start);
			return new VersionDirective(new Version(num, num2), start, start);
		}

		private Token ScanTagDirectiveValue(Mark start)
		{
			this.SkipWhitespaces();
			string text = this.ScanTagHandle(true, start);
			if (!this.analyzer.IsWhite(0))
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %TAG directive, did not find expected whitespace.");
			}
			this.SkipWhitespaces();
			string text2 = this.ScanTagUri(null, start);
			if (!this.analyzer.IsWhiteBreakOrZero(0))
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %TAG directive, did not find expected whitespace or line break.");
			}
			return new TagDirective(text, text2, start, start);
		}

		private string ScanTagUri(string head, Mark start)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (head != null && head.Length > 1)
			{
				stringBuilder.Append(head.Substring(1));
			}
			while (this.analyzer.IsAlphaNumericDashOrUnderscore(0) || this.analyzer.Check(";/?:@&=+$,.!~*'()[]%", 0))
			{
				if (this.analyzer.Check('%', 0))
				{
					stringBuilder.Append(this.ScanUriEscapes(start));
				}
				else if (this.analyzer.Check('+', 0))
				{
					stringBuilder.Append(' ');
					this.Skip();
				}
				else
				{
					stringBuilder.Append(this.ReadCurrentCharacter());
				}
			}
			if (stringBuilder.Length == 0)
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, did not find expected tag URI.");
			}
			return stringBuilder.ToString();
		}

		private string ScanUriEscapes(Mark start)
		{
			byte[] array = null;
			int num = 0;
			int num2 = 0;
			while (this.analyzer.Check('%', 0) && this.analyzer.IsHex(1) && this.analyzer.IsHex(2))
			{
				int num3 = (this.analyzer.AsHex(1) << 4) + this.analyzer.AsHex(2);
				if (num2 == 0)
				{
					num2 = (((num3 & 128) == 0) ? 1 : (((num3 & 224) == 192) ? 2 : (((num3 & 240) == 224) ? 3 : (((num3 & 248) == 240) ? 4 : 0))));
					if (num2 == 0)
					{
						throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, find an incorrect leading UTF-8 octet.");
					}
					array = new byte[num2];
				}
				else if ((num3 & 192) != 128)
				{
					throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, find an incorrect trailing UTF-8 octet.");
				}
				array[num++] = (byte)num3;
				this.Skip();
				this.Skip();
				this.Skip();
				if (--num2 <= 0)
				{
					string @string = Encoding.UTF8.GetString(array, 0, num);
					if (@string.Length == 0 || @string.Length > 2)
					{
						throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, find an incorrect UTF-8 sequence.");
					}
					return @string;
				}
			}
			throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, did not find URI escaped octet.");
		}

		private string ScanTagHandle(bool isDirective, Mark start)
		{
			if (!this.analyzer.Check('!', 0))
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a tag, did not find expected '!'.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.ReadCurrentCharacter());
			while (this.analyzer.IsAlphaNumericDashOrUnderscore(0))
			{
				stringBuilder.Append(this.ReadCurrentCharacter());
			}
			if (this.analyzer.Check('!', 0))
			{
				stringBuilder.Append(this.ReadCurrentCharacter());
			}
			else if (isDirective && (stringBuilder.Length != 1 || stringBuilder[0] != '!'))
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag directive, did not find expected '!'.");
			}
			return stringBuilder.ToString();
		}

		private int ScanVersionDirectiveNumber(Mark start)
		{
			int num = 0;
			int num2 = 0;
			while (this.analyzer.IsDigit(0))
			{
				if (++num2 > 9)
				{
					throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %YAML directive, find extremely long version number.");
				}
				num = num * 10 + this.analyzer.AsDigit(0);
				this.Skip();
			}
			if (num2 == 0)
			{
				throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %YAML directive, did not find expected version number.");
			}
			return num;
		}

		private void SaveSimpleKey()
		{
			bool flag = this.flowLevel == 0 && this.indent == this.cursor.LineOffset;
			if (this.simpleKeyAllowed)
			{
				SimpleKey simpleKey = new SimpleKey(true, flag, this.tokensParsed + this.tokens.Count, this.cursor);
				this.RemoveSimpleKey();
				this.simpleKeys.Pop();
				this.simpleKeys.Push(simpleKey);
			}
		}

		private const int MaxVersionNumberLength = 9;

		private const int MaxBufferLength = 8;

		private static readonly IDictionary<char, char> simpleEscapeCodes = new SortedDictionary<char, char>
		{
			{ '0', '\0' },
			{ 'a', '\a' },
			{ 'b', '\b' },
			{ 't', '\t' },
			{ '\t', '\t' },
			{ 'n', '\n' },
			{ 'v', '\v' },
			{ 'f', '\f' },
			{ 'r', '\r' },
			{ 'e', '\u001b' },
			{ ' ', ' ' },
			{ '"', '"' },
			{ '\'', '\'' },
			{ '\\', '\\' },
			{ 'N', '\u0085' },
			{ '_', '\u00a0' },
			{ 'L', '\u2028' },
			{ 'P', '\u2029' }
		};

		private readonly Stack<int> indents = new Stack<int>();

		private readonly InsertionQueue<Token> tokens = new InsertionQueue<Token>();

		private readonly Stack<SimpleKey> simpleKeys = new Stack<SimpleKey>();

		private readonly CharacterAnalyzer<LookAheadBuffer> analyzer;

		private readonly Cursor cursor;

		private bool streamStartProduced;

		private bool streamEndProduced;

		private int indent = -1;

		private bool simpleKeyAllowed;

		private int flowLevel;

		private int tokensParsed;

		private bool tokenAvailable;

		private Token previous;
	}
}
