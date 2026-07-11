using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	public class Parser : IParser
	{
		public Parser(TextReader input)
			: this(new Scanner(input, true))
		{
		}

		public Parser(IScanner scanner)
		{
			this.scanner = scanner;
		}

		private Token GetCurrentToken()
		{
			if (this.currentToken == null)
			{
				while (this.scanner.MoveNextWithoutConsuming())
				{
					this.currentToken = this.scanner.Current;
					YamlDotNet.Core.Tokens.Comment comment = this.currentToken as YamlDotNet.Core.Tokens.Comment;
					if (comment == null)
					{
						break;
					}
					this.pendingEvents.Enqueue(new YamlDotNet.Core.Events.Comment(comment.Value, comment.IsInline, comment.Start, comment.End));
					this.scanner.ConsumeCurrent();
				}
			}
			return this.currentToken;
		}

		public ParsingEvent Current
		{
			get
			{
				return this.currentEvent;
			}
		}

		public bool MoveNext()
		{
			if (this.state == ParserState.StreamEnd)
			{
				this.currentEvent = null;
				return false;
			}
			if (this.pendingEvents.Count == 0)
			{
				this.pendingEvents.Enqueue(this.StateMachine());
			}
			this.currentEvent = this.pendingEvents.Dequeue();
			return true;
		}

		private ParsingEvent StateMachine()
		{
			switch (this.state)
			{
			case ParserState.StreamStart:
				return this.ParseStreamStart();
			case ParserState.ImplicitDocumentStart:
				return this.ParseDocumentStart(true);
			case ParserState.DocumentStart:
				return this.ParseDocumentStart(false);
			case ParserState.DocumentContent:
				return this.ParseDocumentContent();
			case ParserState.DocumentEnd:
				return this.ParseDocumentEnd();
			case ParserState.BlockNode:
				return this.ParseNode(true, false);
			case ParserState.BlockNodeOrIndentlessSequence:
				return this.ParseNode(true, true);
			case ParserState.FlowNode:
				return this.ParseNode(false, false);
			case ParserState.BlockSequenceFirstEntry:
				return this.ParseBlockSequenceEntry(true);
			case ParserState.BlockSequenceEntry:
				return this.ParseBlockSequenceEntry(false);
			case ParserState.IndentlessSequenceEntry:
				return this.ParseIndentlessSequenceEntry();
			case ParserState.BlockMappingFirstKey:
				return this.ParseBlockMappingKey(true);
			case ParserState.BlockMappingKey:
				return this.ParseBlockMappingKey(false);
			case ParserState.BlockMappingValue:
				return this.ParseBlockMappingValue();
			case ParserState.FlowSequenceFirstEntry:
				return this.ParseFlowSequenceEntry(true);
			case ParserState.FlowSequenceEntry:
				return this.ParseFlowSequenceEntry(false);
			case ParserState.FlowSequenceEntryMappingKey:
				return this.ParseFlowSequenceEntryMappingKey();
			case ParserState.FlowSequenceEntryMappingValue:
				return this.ParseFlowSequenceEntryMappingValue();
			case ParserState.FlowSequenceEntryMappingEnd:
				return this.ParseFlowSequenceEntryMappingEnd();
			case ParserState.FlowMappingFirstKey:
				return this.ParseFlowMappingKey(true);
			case ParserState.FlowMappingKey:
				return this.ParseFlowMappingKey(false);
			case ParserState.FlowMappingValue:
				return this.ParseFlowMappingValue(false);
			case ParserState.FlowMappingEmptyValue:
				return this.ParseFlowMappingValue(true);
			}
			Debug.Assert(false, "Invalid state");
			throw new InvalidOperationException();
		}

		private void Skip()
		{
			if (this.currentToken != null)
			{
				this.currentToken = null;
				this.scanner.ConsumeCurrent();
			}
		}

		private ParsingEvent ParseStreamStart()
		{
			YamlDotNet.Core.Tokens.StreamStart streamStart = this.GetCurrentToken() as YamlDotNet.Core.Tokens.StreamStart;
			if (streamStart == null)
			{
				Token token = this.GetCurrentToken();
				throw new SemanticErrorException(token.Start, token.End, "Did not find expected <stream-start>.");
			}
			this.Skip();
			this.state = ParserState.ImplicitDocumentStart;
			return new YamlDotNet.Core.Events.StreamStart(streamStart.Start, streamStart.End);
		}

		private ParsingEvent ParseDocumentStart(bool isImplicit)
		{
			if (!isImplicit)
			{
				while (this.GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentEnd)
				{
					this.Skip();
				}
			}
			if (isImplicit && !(this.GetCurrentToken() is VersionDirective) && !(this.GetCurrentToken() is TagDirective) && !(this.GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentStart) && !(this.GetCurrentToken() is YamlDotNet.Core.Tokens.StreamEnd))
			{
				TagDirectiveCollection tagDirectiveCollection = new TagDirectiveCollection();
				this.ProcessDirectives(tagDirectiveCollection);
				this.states.Push(ParserState.DocumentEnd);
				this.state = ParserState.BlockNode;
				return new YamlDotNet.Core.Events.DocumentStart(null, tagDirectiveCollection, true, this.GetCurrentToken().Start, this.GetCurrentToken().End);
			}
			if (!(this.GetCurrentToken() is YamlDotNet.Core.Tokens.StreamEnd))
			{
				Mark start = this.GetCurrentToken().Start;
				TagDirectiveCollection tagDirectiveCollection2 = new TagDirectiveCollection();
				VersionDirective versionDirective = this.ProcessDirectives(tagDirectiveCollection2);
				Token token = this.GetCurrentToken();
				if (!(token is YamlDotNet.Core.Tokens.DocumentStart))
				{
					throw new SemanticErrorException(token.Start, token.End, "Did not find expected <document start>.");
				}
				this.states.Push(ParserState.DocumentEnd);
				this.state = ParserState.DocumentContent;
				ParsingEvent parsingEvent = new YamlDotNet.Core.Events.DocumentStart(versionDirective, tagDirectiveCollection2, false, start, token.End);
				this.Skip();
				return parsingEvent;
			}
			else
			{
				this.state = ParserState.StreamEnd;
				ParsingEvent parsingEvent2 = new YamlDotNet.Core.Events.StreamEnd(this.GetCurrentToken().Start, this.GetCurrentToken().End);
				if (this.scanner.MoveNextWithoutConsuming())
				{
					throw new InvalidOperationException("The scanner should contain no more tokens.");
				}
				return parsingEvent2;
			}
		}

		private VersionDirective ProcessDirectives(TagDirectiveCollection tags)
		{
			VersionDirective versionDirective = null;
			bool flag = false;
			VersionDirective versionDirective2;
			TagDirective tagDirective;
			for (;;)
			{
				if ((versionDirective2 = this.GetCurrentToken() as VersionDirective) != null)
				{
					if (versionDirective != null)
					{
						break;
					}
					if (versionDirective2.Version.Major != 1 || versionDirective2.Version.Minor != 1)
					{
						goto IL_0055;
					}
					versionDirective = versionDirective2;
					flag = true;
				}
				else
				{
					if ((tagDirective = this.GetCurrentToken() as TagDirective) == null)
					{
						goto IL_00BD;
					}
					if (tags.Contains(tagDirective.Handle))
					{
						goto Block_5;
					}
					tags.Add(tagDirective);
					flag = true;
				}
				this.Skip();
			}
			throw new SemanticErrorException(versionDirective2.Start, versionDirective2.End, "Found duplicate %YAML directive.");
			IL_0055:
			throw new SemanticErrorException(versionDirective2.Start, versionDirective2.End, "Found incompatible YAML document.");
			Block_5:
			throw new SemanticErrorException(tagDirective.Start, tagDirective.End, "Found duplicate %TAG directive.");
			IL_00BD:
			Parser.AddTagDirectives(tags, Constants.DefaultTagDirectives);
			if (flag)
			{
				this.tagDirectives.Clear();
			}
			Parser.AddTagDirectives(this.tagDirectives, tags);
			return versionDirective;
		}

		private static void AddTagDirectives(TagDirectiveCollection directives, IEnumerable<TagDirective> source)
		{
			foreach (TagDirective tagDirective in source)
			{
				if (!directives.Contains(tagDirective))
				{
					directives.Add(tagDirective);
				}
			}
		}

		private ParsingEvent ParseDocumentContent()
		{
			if (this.GetCurrentToken() is VersionDirective || this.GetCurrentToken() is TagDirective || this.GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentStart || this.GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentEnd || this.GetCurrentToken() is YamlDotNet.Core.Tokens.StreamEnd)
			{
				this.state = this.states.Pop();
				return Parser.ProcessEmptyScalar(this.scanner.CurrentPosition);
			}
			return this.ParseNode(true, false);
		}

		private static ParsingEvent ProcessEmptyScalar(Mark position)
		{
			return new YamlDotNet.Core.Events.Scalar(null, null, string.Empty, ScalarStyle.Plain, true, false, position, position);
		}

		private ParsingEvent ParseNode(bool isBlock, bool isIndentlessSequence)
		{
			YamlDotNet.Core.Tokens.AnchorAlias anchorAlias = this.GetCurrentToken() as YamlDotNet.Core.Tokens.AnchorAlias;
			if (anchorAlias != null)
			{
				this.state = this.states.Pop();
				ParsingEvent parsingEvent = new YamlDotNet.Core.Events.AnchorAlias(anchorAlias.Value, anchorAlias.Start, anchorAlias.End);
				this.Skip();
				return parsingEvent;
			}
			Mark start = this.GetCurrentToken().Start;
			Anchor anchor = null;
			YamlDotNet.Core.Tokens.Tag tag = null;
			for (;;)
			{
				if (anchor == null && (anchor = this.GetCurrentToken() as Anchor) != null)
				{
					this.Skip();
				}
				else
				{
					if (tag != null || (tag = this.GetCurrentToken() as YamlDotNet.Core.Tokens.Tag) == null)
					{
						break;
					}
					this.Skip();
				}
			}
			string text = null;
			if (tag != null)
			{
				if (string.IsNullOrEmpty(tag.Handle))
				{
					text = tag.Suffix;
				}
				else
				{
					if (!this.tagDirectives.Contains(tag.Handle))
					{
						throw new SemanticErrorException(tag.Start, tag.End, "While parsing a node, find undefined tag handle.");
					}
					text = this.tagDirectives[tag.Handle].Prefix + tag.Suffix;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = null;
			}
			string text2 = ((anchor == null) ? null : ((!string.IsNullOrEmpty(anchor.Value)) ? anchor.Value : null));
			bool flag = string.IsNullOrEmpty(text);
			if (isIndentlessSequence && this.GetCurrentToken() is BlockEntry)
			{
				this.state = ParserState.IndentlessSequenceEntry;
				return new SequenceStart(text2, text, flag, SequenceStyle.Block, start, this.GetCurrentToken().End);
			}
			YamlDotNet.Core.Tokens.Scalar scalar = this.GetCurrentToken() as YamlDotNet.Core.Tokens.Scalar;
			if (scalar != null)
			{
				bool flag2 = false;
				bool flag3 = false;
				if ((scalar.Style == ScalarStyle.Plain && text == null) || text == "!")
				{
					flag2 = true;
				}
				else if (text == null)
				{
					flag3 = true;
				}
				this.state = this.states.Pop();
				ParsingEvent parsingEvent2 = new YamlDotNet.Core.Events.Scalar(text2, text, scalar.Value, scalar.Style, flag2, flag3, start, scalar.End);
				this.Skip();
				return parsingEvent2;
			}
			FlowSequenceStart flowSequenceStart = this.GetCurrentToken() as FlowSequenceStart;
			if (flowSequenceStart != null)
			{
				this.state = ParserState.FlowSequenceFirstEntry;
				return new SequenceStart(text2, text, flag, SequenceStyle.Flow, start, flowSequenceStart.End);
			}
			FlowMappingStart flowMappingStart = this.GetCurrentToken() as FlowMappingStart;
			if (flowMappingStart != null)
			{
				this.state = ParserState.FlowMappingFirstKey;
				return new MappingStart(text2, text, flag, MappingStyle.Flow, start, flowMappingStart.End);
			}
			if (isBlock)
			{
				BlockSequenceStart blockSequenceStart = this.GetCurrentToken() as BlockSequenceStart;
				if (blockSequenceStart != null)
				{
					this.state = ParserState.BlockSequenceFirstEntry;
					return new SequenceStart(text2, text, flag, SequenceStyle.Block, start, blockSequenceStart.End);
				}
				BlockMappingStart blockMappingStart = this.GetCurrentToken() as BlockMappingStart;
				if (blockMappingStart != null)
				{
					this.state = ParserState.BlockMappingFirstKey;
					return new MappingStart(text2, text, flag, MappingStyle.Block, start, this.GetCurrentToken().End);
				}
			}
			if (text2 != null || tag != null)
			{
				this.state = this.states.Pop();
				return new YamlDotNet.Core.Events.Scalar(text2, text, string.Empty, ScalarStyle.Plain, flag, false, start, this.GetCurrentToken().End);
			}
			Token token = this.GetCurrentToken();
			throw new SemanticErrorException(token.Start, token.End, "While parsing a node, did not find expected node content.");
		}

		private ParsingEvent ParseDocumentEnd()
		{
			bool flag = true;
			Mark start = this.GetCurrentToken().Start;
			Mark mark = start;
			if (this.GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentEnd)
			{
				mark = this.GetCurrentToken().End;
				this.Skip();
				flag = false;
			}
			this.state = ParserState.DocumentStart;
			return new YamlDotNet.Core.Events.DocumentEnd(flag, start, mark);
		}

		private ParsingEvent ParseBlockSequenceEntry(bool isFirst)
		{
			if (isFirst)
			{
				this.GetCurrentToken();
				this.Skip();
			}
			if (this.GetCurrentToken() is BlockEntry)
			{
				Mark end = this.GetCurrentToken().End;
				this.Skip();
				if (!(this.GetCurrentToken() is BlockEntry) && !(this.GetCurrentToken() is BlockEnd))
				{
					this.states.Push(ParserState.BlockSequenceEntry);
					return this.ParseNode(true, false);
				}
				this.state = ParserState.BlockSequenceEntry;
				return Parser.ProcessEmptyScalar(end);
			}
			else
			{
				if (this.GetCurrentToken() is BlockEnd)
				{
					this.state = this.states.Pop();
					ParsingEvent parsingEvent = new SequenceEnd(this.GetCurrentToken().Start, this.GetCurrentToken().End);
					this.Skip();
					return parsingEvent;
				}
				Token token = this.GetCurrentToken();
				throw new SemanticErrorException(token.Start, token.End, "While parsing a block collection, did not find expected '-' indicator.");
			}
		}

		private ParsingEvent ParseIndentlessSequenceEntry()
		{
			if (!(this.GetCurrentToken() is BlockEntry))
			{
				this.state = this.states.Pop();
				return new SequenceEnd(this.GetCurrentToken().Start, this.GetCurrentToken().End);
			}
			Mark end = this.GetCurrentToken().End;
			this.Skip();
			if (!(this.GetCurrentToken() is BlockEntry) && !(this.GetCurrentToken() is Key) && !(this.GetCurrentToken() is Value) && !(this.GetCurrentToken() is BlockEnd))
			{
				this.states.Push(ParserState.IndentlessSequenceEntry);
				return this.ParseNode(true, false);
			}
			this.state = ParserState.IndentlessSequenceEntry;
			return Parser.ProcessEmptyScalar(end);
		}

		private ParsingEvent ParseBlockMappingKey(bool isFirst)
		{
			if (isFirst)
			{
				this.GetCurrentToken();
				this.Skip();
			}
			if (this.GetCurrentToken() is Key)
			{
				Mark end = this.GetCurrentToken().End;
				this.Skip();
				if (!(this.GetCurrentToken() is Key) && !(this.GetCurrentToken() is Value) && !(this.GetCurrentToken() is BlockEnd))
				{
					this.states.Push(ParserState.BlockMappingValue);
					return this.ParseNode(true, true);
				}
				this.state = ParserState.BlockMappingValue;
				return Parser.ProcessEmptyScalar(end);
			}
			else
			{
				if (this.GetCurrentToken() is BlockEnd)
				{
					this.state = this.states.Pop();
					ParsingEvent parsingEvent = new MappingEnd(this.GetCurrentToken().Start, this.GetCurrentToken().End);
					this.Skip();
					return parsingEvent;
				}
				Token token = this.GetCurrentToken();
				throw new SemanticErrorException(token.Start, token.End, "While parsing a block mapping, did not find expected key.");
			}
		}

		private ParsingEvent ParseBlockMappingValue()
		{
			if (!(this.GetCurrentToken() is Value))
			{
				this.state = ParserState.BlockMappingKey;
				return Parser.ProcessEmptyScalar(this.GetCurrentToken().Start);
			}
			Mark end = this.GetCurrentToken().End;
			this.Skip();
			if (!(this.GetCurrentToken() is Key) && !(this.GetCurrentToken() is Value) && !(this.GetCurrentToken() is BlockEnd))
			{
				this.states.Push(ParserState.BlockMappingKey);
				return this.ParseNode(true, true);
			}
			this.state = ParserState.BlockMappingKey;
			return Parser.ProcessEmptyScalar(end);
		}

		private ParsingEvent ParseFlowSequenceEntry(bool isFirst)
		{
			if (isFirst)
			{
				this.GetCurrentToken();
				this.Skip();
			}
			ParsingEvent parsingEvent;
			if (!(this.GetCurrentToken() is FlowSequenceEnd))
			{
				if (!isFirst)
				{
					if (!(this.GetCurrentToken() is FlowEntry))
					{
						Token token = this.GetCurrentToken();
						throw new SemanticErrorException(token.Start, token.End, "While parsing a flow sequence, did not find expected ',' or ']'.");
					}
					this.Skip();
				}
				if (this.GetCurrentToken() is Key)
				{
					this.state = ParserState.FlowSequenceEntryMappingKey;
					parsingEvent = new MappingStart(null, null, true, MappingStyle.Flow);
					this.Skip();
					return parsingEvent;
				}
				if (!(this.GetCurrentToken() is FlowSequenceEnd))
				{
					this.states.Push(ParserState.FlowSequenceEntry);
					return this.ParseNode(false, false);
				}
			}
			this.state = this.states.Pop();
			parsingEvent = new SequenceEnd(this.GetCurrentToken().Start, this.GetCurrentToken().End);
			this.Skip();
			return parsingEvent;
		}

		private ParsingEvent ParseFlowSequenceEntryMappingKey()
		{
			if (!(this.GetCurrentToken() is Value) && !(this.GetCurrentToken() is FlowEntry) && !(this.GetCurrentToken() is FlowSequenceEnd))
			{
				this.states.Push(ParserState.FlowSequenceEntryMappingValue);
				return this.ParseNode(false, false);
			}
			Mark end = this.GetCurrentToken().End;
			this.Skip();
			this.state = ParserState.FlowSequenceEntryMappingValue;
			return Parser.ProcessEmptyScalar(end);
		}

		private ParsingEvent ParseFlowSequenceEntryMappingValue()
		{
			if (this.GetCurrentToken() is Value)
			{
				this.Skip();
				if (!(this.GetCurrentToken() is FlowEntry) && !(this.GetCurrentToken() is FlowSequenceEnd))
				{
					this.states.Push(ParserState.FlowSequenceEntryMappingEnd);
					return this.ParseNode(false, false);
				}
			}
			this.state = ParserState.FlowSequenceEntryMappingEnd;
			return Parser.ProcessEmptyScalar(this.GetCurrentToken().Start);
		}

		private ParsingEvent ParseFlowSequenceEntryMappingEnd()
		{
			this.state = ParserState.FlowSequenceEntry;
			return new MappingEnd(this.GetCurrentToken().Start, this.GetCurrentToken().End);
		}

		private ParsingEvent ParseFlowMappingKey(bool isFirst)
		{
			if (isFirst)
			{
				this.GetCurrentToken();
				this.Skip();
			}
			if (!(this.GetCurrentToken() is FlowMappingEnd))
			{
				if (!isFirst)
				{
					if (!(this.GetCurrentToken() is FlowEntry))
					{
						Token token = this.GetCurrentToken();
						throw new SemanticErrorException(token.Start, token.End, "While parsing a flow mapping,  did not find expected ',' or '}'.");
					}
					this.Skip();
				}
				if (this.GetCurrentToken() is Key)
				{
					this.Skip();
					if (!(this.GetCurrentToken() is Value) && !(this.GetCurrentToken() is FlowEntry) && !(this.GetCurrentToken() is FlowMappingEnd))
					{
						this.states.Push(ParserState.FlowMappingValue);
						return this.ParseNode(false, false);
					}
					this.state = ParserState.FlowMappingValue;
					return Parser.ProcessEmptyScalar(this.GetCurrentToken().Start);
				}
				else if (!(this.GetCurrentToken() is FlowMappingEnd))
				{
					this.states.Push(ParserState.FlowMappingEmptyValue);
					return this.ParseNode(false, false);
				}
			}
			this.state = this.states.Pop();
			ParsingEvent parsingEvent = new MappingEnd(this.GetCurrentToken().Start, this.GetCurrentToken().End);
			this.Skip();
			return parsingEvent;
		}

		private ParsingEvent ParseFlowMappingValue(bool isEmpty)
		{
			if (isEmpty)
			{
				this.state = ParserState.FlowMappingKey;
				return Parser.ProcessEmptyScalar(this.GetCurrentToken().Start);
			}
			if (this.GetCurrentToken() is Value)
			{
				this.Skip();
				if (!(this.GetCurrentToken() is FlowEntry) && !(this.GetCurrentToken() is FlowMappingEnd))
				{
					this.states.Push(ParserState.FlowMappingKey);
					return this.ParseNode(false, false);
				}
			}
			this.state = ParserState.FlowMappingKey;
			return Parser.ProcessEmptyScalar(this.GetCurrentToken().Start);
		}

		private readonly Stack<ParserState> states = new Stack<ParserState>();

		private readonly TagDirectiveCollection tagDirectives = new TagDirectiveCollection();

		private ParserState state;

		private readonly IScanner scanner;

		private ParsingEvent currentEvent;

		private Token currentToken;

		private readonly Parser.EventQueue pendingEvents = new Parser.EventQueue();

		private class EventQueue
		{
			public void Enqueue(ParsingEvent @event)
			{
				EventType type = @event.Type;
				if (type != EventType.StreamStart && type != EventType.DocumentStart)
				{
					this.normalPriorityEvents.Enqueue(@event);
				}
				else
				{
					this.highPriorityEvents.Enqueue(@event);
				}
			}

			public ParsingEvent Dequeue()
			{
				return (this.highPriorityEvents.Count <= 0) ? this.normalPriorityEvents.Dequeue() : this.highPriorityEvents.Dequeue();
			}

			public int Count
			{
				get
				{
					return this.highPriorityEvents.Count + this.normalPriorityEvents.Count;
				}
			}

			private readonly Queue<ParsingEvent> highPriorityEvents = new Queue<ParsingEvent>();

			private readonly Queue<ParsingEvent> normalPriorityEvents = new Queue<ParsingEvent>();
		}
	}
}
