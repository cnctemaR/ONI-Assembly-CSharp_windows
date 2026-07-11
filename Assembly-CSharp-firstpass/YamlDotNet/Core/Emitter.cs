using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	public class Emitter : IEmitter
	{
		public Emitter(TextWriter output)
			: this(output, 2)
		{
		}

		public Emitter(TextWriter output, int bestIndent)
			: this(output, bestIndent, int.MaxValue)
		{
		}

		public Emitter(TextWriter output, int bestIndent, int bestWidth)
			: this(output, bestIndent, bestWidth, false)
		{
		}

		public Emitter(TextWriter output, int bestIndent, int bestWidth, bool isCanonical)
		{
			if (bestIndent < 2 || bestIndent > 9)
			{
				throw new ArgumentOutOfRangeException("bestIndent", string.Format(CultureInfo.InvariantCulture, "The bestIndent parameter must be between {0} and {1}.", new object[] { 2, 9 }));
			}
			this.bestIndent = bestIndent;
			if (bestWidth <= bestIndent * 2)
			{
				throw new ArgumentOutOfRangeException("bestWidth", "The bestWidth parameter must be greater than bestIndent * 2.");
			}
			this.bestWidth = bestWidth;
			this.isCanonical = isCanonical;
			this.output = output;
			this.outputUsesUnicodeEncoding = this.IsUnicode(output.Encoding);
		}

		public void Emit(ParsingEvent @event)
		{
			this.events.Enqueue(@event);
			while (!this.NeedMoreEvents())
			{
				ParsingEvent parsingEvent = this.events.Peek();
				try
				{
					this.AnalyzeEvent(parsingEvent);
					this.StateMachine(parsingEvent);
				}
				finally
				{
					this.events.Dequeue();
				}
			}
		}

		private bool NeedMoreEvents()
		{
			if (this.events.Count == 0)
			{
				return true;
			}
			EventType type = this.events.Peek().Type;
			int num;
			switch (type)
			{
			case EventType.SequenceStart:
				num = 2;
				break;
			default:
				if (type != EventType.DocumentStart)
				{
					return false;
				}
				num = 1;
				break;
			case EventType.MappingStart:
				num = 3;
				break;
			}
			if (this.events.Count > num)
			{
				return false;
			}
			int num2 = 0;
			foreach (ParsingEvent parsingEvent in this.events)
			{
				switch (parsingEvent.Type)
				{
				case EventType.DocumentStart:
				case EventType.SequenceStart:
				case EventType.MappingStart:
					num2++;
					break;
				case EventType.DocumentEnd:
				case EventType.SequenceEnd:
				case EventType.MappingEnd:
					num2--;
					break;
				}
				if (num2 == 0)
				{
					return false;
				}
			}
			return true;
		}

		private void AnalyzeEvent(ParsingEvent evt)
		{
			this.anchorData.anchor = null;
			this.tagData.handle = null;
			this.tagData.suffix = null;
			YamlDotNet.Core.Events.AnchorAlias anchorAlias = evt as YamlDotNet.Core.Events.AnchorAlias;
			if (anchorAlias != null)
			{
				this.AnalyzeAnchor(anchorAlias.Value, true);
				return;
			}
			NodeEvent nodeEvent = evt as NodeEvent;
			if (nodeEvent != null)
			{
				YamlDotNet.Core.Events.Scalar scalar = evt as YamlDotNet.Core.Events.Scalar;
				if (scalar != null)
				{
					this.AnalyzeScalar(scalar);
				}
				this.AnalyzeAnchor(nodeEvent.Anchor, false);
				if (!string.IsNullOrEmpty(nodeEvent.Tag) && (this.isCanonical || nodeEvent.IsCanonical))
				{
					this.AnalyzeTag(nodeEvent.Tag);
				}
			}
		}

		private void AnalyzeAnchor(string anchor, bool isAlias)
		{
			this.anchorData.anchor = anchor;
			this.anchorData.isAlias = isAlias;
		}

		private void AnalyzeScalar(YamlDotNet.Core.Events.Scalar scalar)
		{
			string value = scalar.Value;
			this.scalarData.value = value;
			if (value.Length == 0)
			{
				if (scalar.Tag == "tag:yaml.org,2002:null")
				{
					this.scalarData.isMultiline = false;
					this.scalarData.isFlowPlainAllowed = false;
					this.scalarData.isBlockPlainAllowed = true;
					this.scalarData.isSingleQuotedAllowed = false;
					this.scalarData.isBlockAllowed = false;
				}
				else
				{
					this.scalarData.isMultiline = false;
					this.scalarData.isFlowPlainAllowed = false;
					this.scalarData.isBlockPlainAllowed = false;
					this.scalarData.isSingleQuotedAllowed = true;
					this.scalarData.isBlockAllowed = false;
				}
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (value.StartsWith("---", StringComparison.Ordinal) || value.StartsWith("...", StringComparison.Ordinal))
			{
				flag = true;
				flag2 = true;
			}
			CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
			bool flag3 = true;
			bool flag4 = characterAnalyzer.IsWhiteBreakOrZero(1);
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			bool flag11 = false;
			bool flag12 = false;
			bool flag13 = false;
			bool flag14 = false;
			bool flag15 = !this.ValueIsRepresentableInOutputEncoding(value);
			bool flag16 = false;
			bool flag17 = true;
			while (!characterAnalyzer.EndOfInput)
			{
				if (flag17)
				{
					if (characterAnalyzer.Check("#,[]{}&*!|>\\\"%@`'", 0))
					{
						flag = true;
						flag2 = true;
						flag9 = characterAnalyzer.Check('\'', 0);
						flag16 |= characterAnalyzer.Check('\'', 0);
					}
					if (characterAnalyzer.Check("?:", 0))
					{
						flag = true;
						if (flag4)
						{
							flag2 = true;
						}
					}
					if (characterAnalyzer.Check('-', 0) && flag4)
					{
						flag = true;
						flag2 = true;
					}
				}
				else
				{
					if (characterAnalyzer.Check(",?[]{}", 0))
					{
						flag = true;
					}
					if (characterAnalyzer.Check(':', 0))
					{
						flag = true;
						if (flag4)
						{
							flag2 = true;
						}
					}
					if (characterAnalyzer.Check('#', 0) && flag3)
					{
						flag = true;
						flag2 = true;
					}
					flag16 |= characterAnalyzer.Check('\'', 0);
				}
				if (!flag15 && !characterAnalyzer.IsPrintable(0))
				{
					flag15 = true;
				}
				if (characterAnalyzer.IsBreak(0))
				{
					flag14 = true;
				}
				if (characterAnalyzer.IsSpace(0))
				{
					if (flag17)
					{
						flag5 = true;
					}
					if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
					{
						flag7 = true;
					}
					if (flag13)
					{
						flag10 = true;
					}
					flag12 = true;
					flag13 = false;
				}
				else if (characterAnalyzer.IsBreak(0))
				{
					if (flag17)
					{
						flag6 = true;
					}
					if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
					{
						flag8 = true;
					}
					if (flag12)
					{
						flag11 = true;
					}
					flag12 = false;
					flag13 = true;
				}
				else
				{
					flag12 = false;
					flag13 = false;
				}
				flag3 = characterAnalyzer.IsWhiteBreakOrZero(0);
				characterAnalyzer.Skip(1);
				if (!characterAnalyzer.EndOfInput)
				{
					flag4 = characterAnalyzer.IsWhiteBreakOrZero(1);
				}
				flag17 = false;
			}
			this.scalarData.isFlowPlainAllowed = true;
			this.scalarData.isBlockPlainAllowed = true;
			this.scalarData.isSingleQuotedAllowed = true;
			this.scalarData.isBlockAllowed = true;
			if (flag5 || flag6 || flag7 || flag8 || flag9)
			{
				this.scalarData.isFlowPlainAllowed = false;
				this.scalarData.isBlockPlainAllowed = false;
			}
			if (flag7)
			{
				this.scalarData.isBlockAllowed = false;
			}
			if (flag10)
			{
				this.scalarData.isFlowPlainAllowed = false;
				this.scalarData.isBlockPlainAllowed = false;
				this.scalarData.isSingleQuotedAllowed = false;
			}
			if (flag11 || flag15)
			{
				this.scalarData.isFlowPlainAllowed = false;
				this.scalarData.isBlockPlainAllowed = false;
				this.scalarData.isSingleQuotedAllowed = false;
				this.scalarData.isBlockAllowed = false;
			}
			this.scalarData.isMultiline = flag14;
			if (flag14)
			{
				this.scalarData.isFlowPlainAllowed = false;
				this.scalarData.isBlockPlainAllowed = false;
			}
			if (flag)
			{
				this.scalarData.isFlowPlainAllowed = false;
			}
			if (flag2)
			{
				this.scalarData.isBlockPlainAllowed = false;
			}
			this.scalarData.hasSingleQuotes = flag16;
		}

		private bool ValueIsRepresentableInOutputEncoding(string value)
		{
			if (this.outputUsesUnicodeEncoding)
			{
				return true;
			}
			bool flag;
			try
			{
				byte[] bytes = this.output.Encoding.GetBytes(value);
				string @string = this.output.Encoding.GetString(bytes, 0, bytes.Length);
				flag = @string.Equals(value);
			}
			catch (EncoderFallbackException)
			{
				flag = false;
			}
			catch (ArgumentOutOfRangeException)
			{
				flag = false;
			}
			return flag;
		}

		private bool IsUnicode(Encoding encoding)
		{
			return encoding is UTF8Encoding || encoding is UnicodeEncoding || encoding is UTF7Encoding;
		}

		private void AnalyzeTag(string tag)
		{
			this.tagData.handle = tag;
			foreach (TagDirective tagDirective in this.tagDirectives)
			{
				if (tag.StartsWith(tagDirective.Prefix, StringComparison.Ordinal))
				{
					this.tagData.handle = tagDirective.Handle;
					this.tagData.suffix = tag.Substring(tagDirective.Prefix.Length);
					break;
				}
			}
		}

		private void StateMachine(ParsingEvent evt)
		{
			YamlDotNet.Core.Events.Comment comment = evt as YamlDotNet.Core.Events.Comment;
			if (comment != null)
			{
				this.EmitComment(comment);
				return;
			}
			switch (this.state)
			{
			case EmitterState.StreamStart:
				this.EmitStreamStart(evt);
				break;
			case EmitterState.StreamEnd:
				throw new YamlException("Expected nothing after STREAM-END");
			case EmitterState.FirstDocumentStart:
				this.EmitDocumentStart(evt, true);
				break;
			case EmitterState.DocumentStart:
				this.EmitDocumentStart(evt, false);
				break;
			case EmitterState.DocumentContent:
				this.EmitDocumentContent(evt);
				break;
			case EmitterState.DocumentEnd:
				this.EmitDocumentEnd(evt);
				break;
			case EmitterState.FlowSequenceFirstItem:
				this.EmitFlowSequenceItem(evt, true);
				break;
			case EmitterState.FlowSequenceItem:
				this.EmitFlowSequenceItem(evt, false);
				break;
			case EmitterState.FlowMappingFirstKey:
				this.EmitFlowMappingKey(evt, true);
				break;
			case EmitterState.FlowMappingKey:
				this.EmitFlowMappingKey(evt, false);
				break;
			case EmitterState.FlowMappingSimpleValue:
				this.EmitFlowMappingValue(evt, true);
				break;
			case EmitterState.FlowMappingValue:
				this.EmitFlowMappingValue(evt, false);
				break;
			case EmitterState.BlockSequenceFirstItem:
				this.EmitBlockSequenceItem(evt, true);
				break;
			case EmitterState.BlockSequenceItem:
				this.EmitBlockSequenceItem(evt, false);
				break;
			case EmitterState.BlockMappingFirstKey:
				this.EmitBlockMappingKey(evt, true);
				break;
			case EmitterState.BlockMappingKey:
				this.EmitBlockMappingKey(evt, false);
				break;
			case EmitterState.BlockMappingSimpleValue:
				this.EmitBlockMappingValue(evt, true);
				break;
			case EmitterState.BlockMappingValue:
				this.EmitBlockMappingValue(evt, false);
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		private void EmitComment(YamlDotNet.Core.Events.Comment comment)
		{
			if (comment.IsInline)
			{
				this.Write(' ');
			}
			else
			{
				this.WriteIndent();
			}
			this.Write("# ");
			this.Write(comment.Value);
			this.WriteBreak('\n');
			this.isIndentation = true;
		}

		private void EmitStreamStart(ParsingEvent evt)
		{
			if (!(evt is YamlDotNet.Core.Events.StreamStart))
			{
				throw new ArgumentException("Expected STREAM-START.", "evt");
			}
			this.indent = -1;
			this.column = 0;
			this.isWhitespace = true;
			this.isIndentation = true;
			this.state = EmitterState.FirstDocumentStart;
		}

		private void EmitDocumentStart(ParsingEvent evt, bool isFirst)
		{
			YamlDotNet.Core.Events.DocumentStart documentStart = evt as YamlDotNet.Core.Events.DocumentStart;
			if (documentStart != null)
			{
				bool flag = documentStart.IsImplicit && isFirst && !this.isCanonical;
				TagDirectiveCollection tagDirectiveCollection = this.NonDefaultTagsAmong(documentStart.Tags);
				if (!isFirst && !this.isDocumentEndWritten && (documentStart.Version != null || tagDirectiveCollection.Count > 0))
				{
					this.isDocumentEndWritten = false;
					this.WriteIndicator("...", true, false, false);
					this.WriteIndent();
				}
				if (documentStart.Version != null)
				{
					this.AnalyzeVersionDirective(documentStart.Version);
					flag = false;
					this.WriteIndicator("%YAML", true, false, false);
					this.WriteIndicator(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { 1, 1 }), true, false, false);
					this.WriteIndent();
				}
				foreach (TagDirective tagDirective in tagDirectiveCollection)
				{
					Emitter.AppendTagDirectiveTo(tagDirective, false, this.tagDirectives);
				}
				foreach (TagDirective tagDirective2 in Constants.DefaultTagDirectives)
				{
					Emitter.AppendTagDirectiveTo(tagDirective2, true, this.tagDirectives);
				}
				if (tagDirectiveCollection.Count > 0)
				{
					flag = false;
					foreach (TagDirective tagDirective3 in Constants.DefaultTagDirectives)
					{
						Emitter.AppendTagDirectiveTo(tagDirective3, true, tagDirectiveCollection);
					}
					foreach (TagDirective tagDirective4 in tagDirectiveCollection)
					{
						this.WriteIndicator("%TAG", true, false, false);
						this.WriteTagHandle(tagDirective4.Handle);
						this.WriteTagContent(tagDirective4.Prefix, true);
						this.WriteIndent();
					}
				}
				if (this.CheckEmptyDocument())
				{
					flag = false;
				}
				if (!flag)
				{
					this.WriteIndent();
					this.WriteIndicator("---", true, false, false);
					if (this.isCanonical)
					{
						this.WriteIndent();
					}
				}
				this.state = EmitterState.DocumentContent;
			}
			else
			{
				if (!(evt is YamlDotNet.Core.Events.StreamEnd))
				{
					throw new YamlException("Expected DOCUMENT-START or STREAM-END");
				}
				if (this.isOpenEnded)
				{
					this.WriteIndicator("...", true, false, false);
					this.WriteIndent();
				}
				this.state = EmitterState.StreamEnd;
			}
		}

		private TagDirectiveCollection NonDefaultTagsAmong(IEnumerable<TagDirective> tagCollection)
		{
			TagDirectiveCollection tagDirectiveCollection = new TagDirectiveCollection();
			if (tagCollection == null)
			{
				return tagDirectiveCollection;
			}
			foreach (TagDirective tagDirective in tagCollection)
			{
				Emitter.AppendTagDirectiveTo(tagDirective, false, tagDirectiveCollection);
			}
			foreach (TagDirective tagDirective2 in Constants.DefaultTagDirectives)
			{
				tagDirectiveCollection.Remove(tagDirective2);
			}
			return tagDirectiveCollection;
		}

		private void AnalyzeVersionDirective(VersionDirective versionDirective)
		{
			if (versionDirective.Version.Major != 1 || versionDirective.Version.Minor != 1)
			{
				throw new YamlException("Incompatible %YAML directive");
			}
		}

		private static void AppendTagDirectiveTo(TagDirective value, bool allowDuplicates, TagDirectiveCollection tagDirectives)
		{
			if (tagDirectives.Contains(value))
			{
				if (!allowDuplicates)
				{
					throw new YamlException("Duplicate %TAG directive.");
				}
			}
			else
			{
				tagDirectives.Add(value);
			}
		}

		private void EmitDocumentContent(ParsingEvent evt)
		{
			this.states.Push(EmitterState.DocumentEnd);
			this.EmitNode(evt, true, false, false);
		}

		private void EmitNode(ParsingEvent evt, bool isRoot, bool isMapping, bool isSimpleKey)
		{
			this.isRootContext = isRoot;
			this.isMappingContext = isMapping;
			this.isSimpleKeyContext = isSimpleKey;
			switch (evt.Type)
			{
			case EventType.Alias:
				this.EmitAlias();
				return;
			case EventType.Scalar:
				this.EmitScalar(evt);
				return;
			case EventType.SequenceStart:
				this.EmitSequenceStart(evt);
				return;
			case EventType.MappingStart:
				this.EmitMappingStart(evt);
				return;
			}
			throw new YamlException(string.Format("Expected SCALAR, SEQUENCE-START, MAPPING-START, or ALIAS, got {0}", evt.Type));
		}

		private void EmitAlias()
		{
			this.ProcessAnchor();
			this.state = this.states.Pop();
		}

		private void EmitScalar(ParsingEvent evt)
		{
			this.SelectScalarStyle(evt);
			this.ProcessAnchor();
			this.ProcessTag();
			this.IncreaseIndent(true, false);
			this.ProcessScalar();
			this.indent = this.indents.Pop();
			this.state = this.states.Pop();
		}

		private void SelectScalarStyle(ParsingEvent evt)
		{
			YamlDotNet.Core.Events.Scalar scalar = (YamlDotNet.Core.Events.Scalar)evt;
			ScalarStyle scalarStyle = scalar.Style;
			bool flag = this.tagData.handle == null && this.tagData.suffix == null;
			if (flag && !scalar.IsPlainImplicit && !scalar.IsQuotedImplicit)
			{
				throw new YamlException("Neither tag nor isImplicit flags are specified.");
			}
			if (scalarStyle == ScalarStyle.Any)
			{
				scalarStyle = ((!this.scalarData.isMultiline) ? ScalarStyle.Plain : ScalarStyle.Folded);
			}
			if (this.isCanonical)
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			if (this.isSimpleKeyContext && this.scalarData.isMultiline)
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			if (scalarStyle == ScalarStyle.Plain)
			{
				if ((this.flowLevel != 0 && !this.scalarData.isFlowPlainAllowed) || (this.flowLevel == 0 && !this.scalarData.isBlockPlainAllowed))
				{
					scalarStyle = ((!this.scalarData.isSingleQuotedAllowed || this.scalarData.hasSingleQuotes) ? ScalarStyle.DoubleQuoted : ScalarStyle.SingleQuoted);
				}
				if (string.IsNullOrEmpty(this.scalarData.value) && (this.flowLevel != 0 || this.isSimpleKeyContext))
				{
					scalarStyle = ScalarStyle.SingleQuoted;
				}
				if (flag && !scalar.IsPlainImplicit)
				{
					scalarStyle = ScalarStyle.SingleQuoted;
				}
			}
			if (scalarStyle == ScalarStyle.SingleQuoted && !this.scalarData.isSingleQuotedAllowed)
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			if ((scalarStyle == ScalarStyle.Literal || scalarStyle == ScalarStyle.Folded) && (!this.scalarData.isBlockAllowed || this.flowLevel != 0 || this.isSimpleKeyContext))
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			this.scalarData.style = scalarStyle;
		}

		private void ProcessScalar()
		{
			switch (this.scalarData.style)
			{
			case ScalarStyle.Plain:
				this.WritePlainScalar(this.scalarData.value, !this.isSimpleKeyContext);
				break;
			case ScalarStyle.SingleQuoted:
				this.WriteSingleQuotedScalar(this.scalarData.value, !this.isSimpleKeyContext);
				break;
			case ScalarStyle.DoubleQuoted:
				this.WriteDoubleQuotedScalar(this.scalarData.value, !this.isSimpleKeyContext);
				break;
			case ScalarStyle.Literal:
				this.WriteLiteralScalar(this.scalarData.value);
				break;
			case ScalarStyle.Folded:
				this.WriteFoldedScalar(this.scalarData.value);
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		private void WritePlainScalar(string value, bool allowBreaks)
		{
			if (!this.isWhitespace)
			{
				this.Write(' ');
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				char c2;
				if (Emitter.IsSpace(c))
				{
					if (allowBreaks && !flag && this.column > this.bestWidth && i + 1 < value.Length && value[i + 1] != ' ')
					{
						this.WriteIndent();
					}
					else
					{
						this.Write(c);
					}
					flag = true;
				}
				else if (Emitter.IsBreak(c, out c2))
				{
					if (!flag2 && c == '\n')
					{
						this.WriteBreak('\n');
					}
					this.WriteBreak(c2);
					this.isIndentation = true;
					flag2 = true;
				}
				else
				{
					if (flag2)
					{
						this.WriteIndent();
					}
					this.Write(c);
					this.isIndentation = false;
					flag = false;
					flag2 = false;
				}
			}
			this.isWhitespace = false;
			this.isIndentation = false;
			if (this.isRootContext)
			{
				this.isOpenEnded = true;
			}
		}

		private void WriteSingleQuotedScalar(string value, bool allowBreaks)
		{
			this.WriteIndicator("'", true, false, false);
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				char c2;
				if (c == ' ')
				{
					if (allowBreaks && !flag && this.column > this.bestWidth && i != 0 && i + 1 < value.Length && value[i + 1] != ' ')
					{
						this.WriteIndent();
					}
					else
					{
						this.Write(c);
					}
					flag = true;
				}
				else if (Emitter.IsBreak(c, out c2))
				{
					if (!flag2 && c == '\n')
					{
						this.WriteBreak('\n');
					}
					this.WriteBreak(c2);
					this.isIndentation = true;
					flag2 = true;
				}
				else
				{
					if (flag2)
					{
						this.WriteIndent();
					}
					if (c == '\'')
					{
						this.Write(c);
					}
					this.Write(c);
					this.isIndentation = false;
					flag = false;
					flag2 = false;
				}
			}
			this.WriteIndicator("'", false, false, false);
			this.isWhitespace = false;
			this.isIndentation = false;
		}

		private void WriteDoubleQuotedScalar(string value, bool allowBreaks)
		{
			this.WriteIndicator("\"", true, false, false);
			bool flag = false;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				char c2;
				if (!Emitter.IsPrintable(c) || Emitter.IsBreak(c, out c2) || c == '"' || c == '\\')
				{
					this.Write('\\');
					switch (c)
					{
					case '\a':
						this.Write('a');
						break;
					case '\b':
						this.Write('b');
						break;
					case '\t':
						this.Write('t');
						break;
					case '\n':
						this.Write('n');
						break;
					case '\v':
						this.Write('v');
						break;
					case '\f':
						this.Write('f');
						break;
					case '\r':
						this.Write('r');
						break;
					default:
						if (c != '\u2028')
						{
							if (c != '\u2029')
							{
								if (c != '\0')
								{
									if (c != '\u001b')
									{
										if (c != '"')
										{
											if (c != '\\')
											{
												if (c != '\u0085')
												{
													if (c != '\u00a0')
													{
														ushort num = (ushort)c;
														if (num <= 255)
														{
															this.Write('x');
															this.Write(num.ToString("X02", CultureInfo.InvariantCulture));
														}
														else if (Emitter.IsHighSurrogate(c))
														{
															if (i + 1 >= value.Length || !Emitter.IsLowSurrogate(value[i + 1]))
															{
																throw new SyntaxErrorException("While writing a quoted scalar, found an orphaned high surrogate.");
															}
															this.Write('U');
															this.Write(char.ConvertToUtf32(c, value[i + 1]).ToString("X08", CultureInfo.InvariantCulture));
															i++;
														}
														else
														{
															this.Write('u');
															this.Write(num.ToString("X04", CultureInfo.InvariantCulture));
														}
													}
													else
													{
														this.Write('_');
													}
												}
												else
												{
													this.Write('N');
												}
											}
											else
											{
												this.Write('\\');
											}
										}
										else
										{
											this.Write('"');
										}
									}
									else
									{
										this.Write('e');
									}
								}
								else
								{
									this.Write('0');
								}
							}
							else
							{
								this.Write('P');
							}
						}
						else
						{
							this.Write('L');
						}
						break;
					}
					flag = false;
				}
				else if (c == ' ')
				{
					if (allowBreaks && !flag && this.column > this.bestWidth && i > 0 && i + 1 < value.Length)
					{
						this.WriteIndent();
						if (value[i + 1] == ' ')
						{
							this.Write('\\');
						}
					}
					else
					{
						this.Write(c);
					}
					flag = true;
				}
				else
				{
					this.Write(c);
					flag = false;
				}
			}
			this.WriteIndicator("\"", false, false, false);
			this.isWhitespace = false;
			this.isIndentation = false;
		}

		private void WriteLiteralScalar(string value)
		{
			bool flag = true;
			this.WriteIndicator("|", true, false, false);
			this.WriteBlockScalarHints(value);
			this.WriteBreak('\n');
			this.isIndentation = true;
			this.isWhitespace = true;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (c != '\r' || i + 1 >= value.Length || value[i + 1] != '\n')
				{
					char c2;
					if (Emitter.IsBreak(c, out c2))
					{
						this.WriteBreak(c2);
						this.isIndentation = true;
						flag = true;
					}
					else
					{
						if (flag)
						{
							this.WriteIndent();
						}
						this.Write(c);
						this.isIndentation = false;
						flag = false;
					}
				}
			}
		}

		private void WriteFoldedScalar(string value)
		{
			bool flag = true;
			bool flag2 = true;
			this.WriteIndicator(">", true, false, false);
			this.WriteBlockScalarHints(value);
			this.WriteBreak('\n');
			this.isIndentation = true;
			this.isWhitespace = true;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				char c2;
				if (Emitter.IsBreak(c, out c2))
				{
					if (!flag && !flag2 && c == '\n')
					{
						int num = 0;
						char c3;
						while (i + num < value.Length && Emitter.IsBreak(value[i + num], out c3))
						{
							num++;
						}
						if (i + num < value.Length && !Emitter.IsBlank(value[i + num]) && !Emitter.IsBreak(value[i + num], out c3))
						{
							this.WriteBreak('\n');
						}
					}
					this.WriteBreak(c2);
					this.isIndentation = true;
					flag = true;
				}
				else
				{
					if (flag)
					{
						this.WriteIndent();
						flag2 = Emitter.IsBlank(c);
					}
					if (!flag && c == ' ' && i + 1 < value.Length && value[i + 1] != ' ' && this.column > this.bestWidth)
					{
						this.WriteIndent();
					}
					else
					{
						this.Write(c);
					}
					this.isIndentation = false;
					flag = false;
				}
			}
		}

		private static bool IsSpace(char character)
		{
			return character == ' ';
		}

		private static bool IsBreak(char character, out char breakChar)
		{
			switch (character)
			{
			case '\n':
			case '\r':
				break;
			default:
				if (character == '\u2028' || character == '\u2029')
				{
					breakChar = character;
					return true;
				}
				if (character != '\u0085')
				{
					breakChar = '\0';
					return false;
				}
				break;
			}
			breakChar = '\n';
			return true;
		}

		private static bool IsBlank(char character)
		{
			return character == ' ' || character == '\t';
		}

		private static bool IsPrintable(char character)
		{
			return character == '\t' || character == '\n' || character == '\r' || (character >= ' ' && character <= '~') || character == '\u0085' || (character >= '\u00a0' && character <= '\ud7ff') || (character >= '\ue000' && character <= '\ufffd');
		}

		private static bool IsHighSurrogate(char c)
		{
			return '\ud800' <= c && c <= '\udbff';
		}

		private static bool IsLowSurrogate(char c)
		{
			return '\udc00' <= c && c <= '\udfff';
		}

		private void EmitSequenceStart(ParsingEvent evt)
		{
			this.ProcessAnchor();
			this.ProcessTag();
			SequenceStart sequenceStart = (SequenceStart)evt;
			if (this.flowLevel != 0 || this.isCanonical || sequenceStart.Style == SequenceStyle.Flow || this.CheckEmptySequence())
			{
				this.state = EmitterState.FlowSequenceFirstItem;
			}
			else
			{
				this.state = EmitterState.BlockSequenceFirstItem;
			}
		}

		private void EmitMappingStart(ParsingEvent evt)
		{
			this.ProcessAnchor();
			this.ProcessTag();
			MappingStart mappingStart = (MappingStart)evt;
			if (this.flowLevel != 0 || this.isCanonical || mappingStart.Style == MappingStyle.Flow || this.CheckEmptyMapping())
			{
				this.state = EmitterState.FlowMappingFirstKey;
			}
			else
			{
				this.state = EmitterState.BlockMappingFirstKey;
			}
		}

		private void ProcessAnchor()
		{
			if (this.anchorData.anchor != null)
			{
				this.WriteIndicator((!this.anchorData.isAlias) ? "&" : "*", true, false, false);
				this.WriteAnchor(this.anchorData.anchor);
			}
		}

		private void ProcessTag()
		{
			if (this.tagData.handle == null && this.tagData.suffix == null)
			{
				return;
			}
			if (this.tagData.handle != null)
			{
				this.WriteTagHandle(this.tagData.handle);
				if (this.tagData.suffix != null)
				{
					this.WriteTagContent(this.tagData.suffix, false);
				}
			}
			else
			{
				this.WriteIndicator("!<", true, false, false);
				this.WriteTagContent(this.tagData.suffix, false);
				this.WriteIndicator(">", false, false, false);
			}
		}

		private void EmitDocumentEnd(ParsingEvent evt)
		{
			YamlDotNet.Core.Events.DocumentEnd documentEnd = evt as YamlDotNet.Core.Events.DocumentEnd;
			if (documentEnd != null)
			{
				this.WriteIndent();
				if (!documentEnd.IsImplicit)
				{
					this.WriteIndicator("...", true, false, false);
					this.WriteIndent();
					this.isDocumentEndWritten = true;
				}
				this.state = EmitterState.DocumentStart;
				this.tagDirectives.Clear();
				return;
			}
			throw new YamlException("Expected DOCUMENT-END.");
		}

		private void EmitFlowSequenceItem(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				this.WriteIndicator("[", true, true, false);
				this.IncreaseIndent(true, false);
				this.flowLevel++;
			}
			if (evt is SequenceEnd)
			{
				this.flowLevel--;
				this.indent = this.indents.Pop();
				if (this.isCanonical && !isFirst)
				{
					this.WriteIndicator(",", false, false, false);
					this.WriteIndent();
				}
				this.WriteIndicator("]", false, false, false);
				this.state = this.states.Pop();
				return;
			}
			if (!isFirst)
			{
				this.WriteIndicator(",", false, false, false);
			}
			if (this.isCanonical || this.column > this.bestWidth)
			{
				this.WriteIndent();
			}
			this.states.Push(EmitterState.FlowSequenceItem);
			this.EmitNode(evt, false, false, false);
		}

		private void EmitFlowMappingKey(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				this.WriteIndicator("{", true, true, false);
				this.IncreaseIndent(true, false);
				this.flowLevel++;
			}
			if (evt is MappingEnd)
			{
				this.flowLevel--;
				this.indent = this.indents.Pop();
				if (this.isCanonical && !isFirst)
				{
					this.WriteIndicator(",", false, false, false);
					this.WriteIndent();
				}
				this.WriteIndicator("}", false, false, false);
				this.state = this.states.Pop();
				return;
			}
			if (!isFirst)
			{
				this.WriteIndicator(",", false, false, false);
			}
			if (this.isCanonical || this.column > this.bestWidth)
			{
				this.WriteIndent();
			}
			if (!this.isCanonical && this.CheckSimpleKey())
			{
				this.states.Push(EmitterState.FlowMappingSimpleValue);
				this.EmitNode(evt, false, true, true);
			}
			else
			{
				this.WriteIndicator("?", true, false, false);
				this.states.Push(EmitterState.FlowMappingValue);
				this.EmitNode(evt, false, true, false);
			}
		}

		private void EmitFlowMappingValue(ParsingEvent evt, bool isSimple)
		{
			if (isSimple)
			{
				this.WriteIndicator(":", false, false, false);
			}
			else
			{
				if (this.isCanonical || this.column > this.bestWidth)
				{
					this.WriteIndent();
				}
				this.WriteIndicator(":", true, false, false);
			}
			this.states.Push(EmitterState.FlowMappingKey);
			this.EmitNode(evt, false, true, false);
		}

		private void EmitBlockSequenceItem(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				this.IncreaseIndent(false, this.isMappingContext && !this.isIndentation);
			}
			if (evt is SequenceEnd)
			{
				this.indent = this.indents.Pop();
				this.state = this.states.Pop();
				return;
			}
			this.WriteIndent();
			this.WriteIndicator("-", true, false, true);
			this.states.Push(EmitterState.BlockSequenceItem);
			this.EmitNode(evt, false, false, false);
		}

		private void EmitBlockMappingKey(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				this.IncreaseIndent(false, false);
			}
			if (evt is MappingEnd)
			{
				this.indent = this.indents.Pop();
				this.state = this.states.Pop();
				return;
			}
			this.WriteIndent();
			if (this.CheckSimpleKey())
			{
				this.states.Push(EmitterState.BlockMappingSimpleValue);
				this.EmitNode(evt, false, true, true);
			}
			else
			{
				this.WriteIndicator("?", true, false, true);
				this.states.Push(EmitterState.BlockMappingValue);
				this.EmitNode(evt, false, true, false);
			}
		}

		private void EmitBlockMappingValue(ParsingEvent evt, bool isSimple)
		{
			if (isSimple)
			{
				this.WriteIndicator(":", false, false, false);
			}
			else
			{
				this.WriteIndent();
				this.WriteIndicator(":", true, false, true);
			}
			this.states.Push(EmitterState.BlockMappingKey);
			this.EmitNode(evt, false, true, false);
		}

		private void IncreaseIndent(bool isFlow, bool isIndentless)
		{
			this.indents.Push(this.indent);
			if (this.indent < 0)
			{
				this.indent = ((!isFlow) ? 0 : this.bestIndent);
			}
			else if (!isIndentless)
			{
				this.indent += this.bestIndent;
			}
		}

		private bool CheckEmptyDocument()
		{
			int num = 0;
			foreach (ParsingEvent parsingEvent in this.events)
			{
				num++;
				if (num == 2)
				{
					YamlDotNet.Core.Events.Scalar scalar = parsingEvent as YamlDotNet.Core.Events.Scalar;
					if (scalar != null)
					{
						return string.IsNullOrEmpty(scalar.Value);
					}
					break;
				}
			}
			return false;
		}

		private bool CheckSimpleKey()
		{
			if (this.events.Count < 1)
			{
				return false;
			}
			int num;
			switch (this.events.Peek().Type)
			{
			case EventType.Alias:
				num = this.SafeStringLength(this.anchorData.anchor);
				goto IL_014D;
			case EventType.Scalar:
				if (this.scalarData.isMultiline)
				{
					return false;
				}
				num = this.SafeStringLength(this.anchorData.anchor) + this.SafeStringLength(this.tagData.handle) + this.SafeStringLength(this.tagData.suffix) + this.SafeStringLength(this.scalarData.value);
				goto IL_014D;
			case EventType.SequenceStart:
				if (!this.CheckEmptySequence())
				{
					return false;
				}
				num = this.SafeStringLength(this.anchorData.anchor) + this.SafeStringLength(this.tagData.handle) + this.SafeStringLength(this.tagData.suffix);
				goto IL_014D;
			case EventType.MappingStart:
				if (!this.CheckEmptySequence())
				{
					return false;
				}
				num = this.SafeStringLength(this.anchorData.anchor) + this.SafeStringLength(this.tagData.handle) + this.SafeStringLength(this.tagData.suffix);
				goto IL_014D;
			}
			return false;
			IL_014D:
			return num <= 128;
		}

		private int SafeStringLength(string value)
		{
			return (value != null) ? value.Length : 0;
		}

		private bool CheckEmptySequence()
		{
			if (this.events.Count < 2)
			{
				return false;
			}
			FakeList<ParsingEvent> fakeList = new FakeList<ParsingEvent>(this.events);
			return fakeList[0] is SequenceStart && fakeList[1] is SequenceEnd;
		}

		private bool CheckEmptyMapping()
		{
			if (this.events.Count < 2)
			{
				return false;
			}
			FakeList<ParsingEvent> fakeList = new FakeList<ParsingEvent>(this.events);
			return fakeList[0] is MappingStart && fakeList[1] is MappingEnd;
		}

		private void WriteBlockScalarHints(string value)
		{
			CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
			if (characterAnalyzer.IsSpace(0) || characterAnalyzer.IsBreak(0))
			{
				string text = string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { this.bestIndent });
				this.WriteIndicator(text, false, false, false);
			}
			this.isOpenEnded = false;
			string text2 = null;
			if (value.Length == 0 || !characterAnalyzer.IsBreak(value.Length - 1))
			{
				text2 = "-";
			}
			else if (value.Length >= 2 && characterAnalyzer.IsBreak(value.Length - 2))
			{
				text2 = "+";
				this.isOpenEnded = true;
			}
			if (text2 != null)
			{
				this.WriteIndicator(text2, false, false, false);
			}
		}

		private void WriteIndicator(string indicator, bool needWhitespace, bool whitespace, bool indentation)
		{
			if (needWhitespace && !this.isWhitespace)
			{
				this.Write(' ');
			}
			this.Write(indicator);
			this.isWhitespace = whitespace;
			this.isIndentation = this.isIndentation && indentation;
			this.isOpenEnded = false;
		}

		private void WriteIndent()
		{
			int num = Math.Max(this.indent, 0);
			if (!this.isIndentation || this.column > num || (this.column == num && !this.isWhitespace))
			{
				this.WriteBreak('\n');
			}
			while (this.column < num)
			{
				this.Write(' ');
			}
			this.isWhitespace = true;
			this.isIndentation = true;
		}

		private void WriteAnchor(string value)
		{
			this.Write(value);
			this.isWhitespace = false;
			this.isIndentation = false;
		}

		private void WriteTagHandle(string value)
		{
			if (!this.isWhitespace)
			{
				this.Write(' ');
			}
			this.Write(value);
			this.isWhitespace = false;
			this.isIndentation = false;
		}

		private void WriteTagContent(string value, bool needsWhitespace)
		{
			if (needsWhitespace && !this.isWhitespace)
			{
				this.Write(' ');
			}
			this.Write(this.UrlEncode(value));
			this.isWhitespace = false;
			this.isIndentation = false;
		}

		private string UrlEncode(string text)
		{
			return Emitter.uriReplacer.Replace(text, delegate(Match match)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in Encoding.UTF8.GetBytes(match.Value))
				{
					stringBuilder.AppendFormat("%{0:X02}", b);
				}
				return stringBuilder.ToString();
			});
		}

		private void Write(char value)
		{
			this.output.Write(value);
			this.column++;
		}

		private void Write(string value)
		{
			this.output.Write(value);
			this.column += value.Length;
		}

		private void WriteBreak(char breakCharacter = '\n')
		{
			if (breakCharacter == '\n')
			{
				this.output.WriteLine();
			}
			else
			{
				this.output.Write(breakCharacter);
			}
			this.column = 0;
		}

		private const int MinBestIndent = 2;

		private const int MaxBestIndent = 9;

		private const int MaxAliasLength = 128;

		private static readonly Regex uriReplacer = new Regex("[^0-9A-Za-z_\\-;?@=$~\\\\\\)\\]/:&+,\\.\\*\\(\\[!]", RegexOptions.Compiled | RegexOptions.Singleline);

		private readonly TextWriter output;

		private readonly bool outputUsesUnicodeEncoding;

		private readonly bool isCanonical;

		private readonly int bestIndent;

		private readonly int bestWidth;

		private EmitterState state;

		private readonly Stack<EmitterState> states = new Stack<EmitterState>();

		private readonly Queue<ParsingEvent> events = new Queue<ParsingEvent>();

		private readonly Stack<int> indents = new Stack<int>();

		private readonly TagDirectiveCollection tagDirectives = new TagDirectiveCollection();

		private int indent;

		private int flowLevel;

		private bool isMappingContext;

		private bool isSimpleKeyContext;

		private bool isRootContext;

		private int column;

		private bool isWhitespace;

		private bool isIndentation;

		private bool isOpenEnded;

		private bool isDocumentEndWritten;

		private readonly Emitter.AnchorData anchorData = new Emitter.AnchorData();

		private readonly Emitter.TagData tagData = new Emitter.TagData();

		private readonly Emitter.ScalarData scalarData = new Emitter.ScalarData();

		private class AnchorData
		{
			public string anchor;

			public bool isAlias;
		}

		private class TagData
		{
			public string handle;

			public string suffix;
		}

		private class ScalarData
		{
			public string value;

			public bool isMultiline;

			public bool isFlowPlainAllowed;

			public bool isBlockPlainAllowed;

			public bool isSingleQuotedAllowed;

			public bool isBlockAllowed;

			public bool hasSingleQuotes;

			public ScalarStyle style;
		}
	}
}
