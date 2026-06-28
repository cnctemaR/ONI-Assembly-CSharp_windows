using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public sealed class MergingParser : IParser
	{
		public MergingParser(IParser innerParser)
		{
			this._innerParser = innerParser;
		}

		public ParsingEvent Current { get; private set; }

		public bool MoveNext()
		{
			if (this._currentIndex < 0)
			{
				while (this._innerParser.MoveNext())
				{
					this._allEvents.Add(this._innerParser.Current);
				}
				for (int i = this._allEvents.Count - 2; i >= 0; i--)
				{
					Scalar scalar = this._allEvents[i] as Scalar;
					if (scalar != null && scalar.Value == "<<")
					{
						AnchorAlias anchorAlias = this._allEvents[i + 1] as AnchorAlias;
						if (anchorAlias == null)
						{
							if (this._allEvents[i + 1] is SequenceStart)
							{
								List<IEnumerable<ParsingEvent>> list = new List<IEnumerable<ParsingEvent>>();
								bool flag = false;
								for (int j = i + 2; j < this._allEvents.Count; j++)
								{
									anchorAlias = this._allEvents[j] as AnchorAlias;
									if (anchorAlias != null)
									{
										list.Add(this.GetMappingEvents(anchorAlias.Value));
									}
									else if (this._allEvents[j] is SequenceEnd)
									{
										this._allEvents.RemoveRange(i, j - i + 1);
										this._allEvents.InsertRange(i, list.SelectMany<IEnumerable<ParsingEvent>, ParsingEvent>((IEnumerable<ParsingEvent> e) => e));
										flag = true;
										break;
									}
								}
								if (flag)
								{
									goto IL_019D;
								}
							}
							throw new SemanticErrorException(scalar.Start, scalar.End, "Unrecognized merge key pattern");
						}
						IEnumerable<ParsingEvent> mappingEvents = this.GetMappingEvents(anchorAlias.Value);
						this._allEvents.RemoveRange(i, 2);
						this._allEvents.InsertRange(i, mappingEvents);
					}
					IL_019D:;
				}
			}
			int num = this._currentIndex + 1;
			if (num < this._allEvents.Count)
			{
				this.Current = this._allEvents[num];
				this._currentIndex = num;
				return true;
			}
			return false;
		}

		private IEnumerable<ParsingEvent> GetMappingEvents(string mappingAlias)
		{
			MergingParser.ParsingEventCloner cloner = new MergingParser.ParsingEventCloner();
			int nesting = 0;
			return (from e in this._allEvents.SkipWhile<ParsingEvent>(delegate(ParsingEvent e)
				{
					MappingStart mappingStart = e as MappingStart;
					return mappingStart == null || mappingStart.Anchor != mappingAlias;
				}).Skip<ParsingEvent>(1).TakeWhile<ParsingEvent>((ParsingEvent e) => (nesting += e.NestingIncrease) >= 0)
				select cloner.Clone(e)).ToList<ParsingEvent>();
		}

		private readonly List<ParsingEvent> _allEvents = new List<ParsingEvent>();

		private readonly IParser _innerParser;

		private int _currentIndex = -1;

		private class ParsingEventCloner : IParsingEventVisitor
		{
			public ParsingEvent Clone(ParsingEvent e)
			{
				e.Accept(this);
				return this.clonedEvent;
			}

			void IParsingEventVisitor.Visit(AnchorAlias e)
			{
				this.clonedEvent = new AnchorAlias(e.Value, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(StreamStart e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(StreamEnd e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(DocumentStart e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(DocumentEnd e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(Scalar e)
			{
				this.clonedEvent = new Scalar(null, e.Tag, e.Value, e.Style, e.IsPlainImplicit, e.IsQuotedImplicit, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(SequenceStart e)
			{
				this.clonedEvent = new SequenceStart(null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(SequenceEnd e)
			{
				this.clonedEvent = new SequenceEnd(e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(MappingStart e)
			{
				this.clonedEvent = new MappingStart(null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(MappingEnd e)
			{
				this.clonedEvent = new MappingEnd(e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(Comment e)
			{
				throw new NotSupportedException();
			}

			private ParsingEvent clonedEvent;
		}
	}
}
