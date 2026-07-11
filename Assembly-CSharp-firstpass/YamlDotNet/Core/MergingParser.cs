using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public sealed class MergingParser : IParser
	{
		public MergingParser(IParser innerParser)
		{
			this._events = new MergingParser.ParsingEventCollection();
			this._merged = false;
			this._iterator = this._events.GetEnumerator();
			this._innerParser = innerParser;
		}

		public ParsingEvent Current
		{
			get
			{
				if (this._iterator.Current != null)
				{
					return this._iterator.Current.Value;
				}
				return null;
			}
		}

		public bool MoveNext()
		{
			if (!this._merged)
			{
				this.Merge();
				this._events.CleanMarked();
				this._iterator = this._events.GetEnumerator();
				this._merged = true;
			}
			return this._iterator.MoveNext();
		}

		private void Merge()
		{
			while (this._innerParser.MoveNext())
			{
				this._events.Add(this._innerParser.Current);
			}
			foreach (LinkedListNode<ParsingEvent> linkedListNode in this._events)
			{
				if (this.IsMergeToken(linkedListNode))
				{
					this._events.MarkDeleted(linkedListNode);
					if (!this.HandleMerge(linkedListNode.Next))
					{
						throw new SemanticErrorException(linkedListNode.Value.Start, linkedListNode.Value.End, "Unrecognized merge key pattern");
					}
				}
			}
		}

		private bool HandleMerge(LinkedListNode<ParsingEvent> node)
		{
			if (node == null)
			{
				return false;
			}
			if (node.Value is AnchorAlias)
			{
				return this.HandleAnchorAlias(node);
			}
			return node.Value is SequenceStart && this.HandleSequence(node);
		}

		private bool IsMergeToken(LinkedListNode<ParsingEvent> node)
		{
			if (node.Value is Scalar)
			{
				Scalar scalar = node.Value as Scalar;
				return scalar.Value == "<<";
			}
			return false;
		}

		private bool HandleAnchorAlias(LinkedListNode<ParsingEvent> node)
		{
			if (node == null || !(node.Value is AnchorAlias))
			{
				return false;
			}
			AnchorAlias anchorAlias = (AnchorAlias)node.Value;
			IEnumerable<ParsingEvent> mappingEvents = this.GetMappingEvents(anchorAlias.Value);
			this._events.AddAfter(node, mappingEvents);
			this._events.MarkDeleted(node);
			return true;
		}

		private bool HandleSequence(LinkedListNode<ParsingEvent> node)
		{
			if (node == null || !(node.Value is SequenceStart))
			{
				return false;
			}
			this._events.MarkDeleted(node);
			while (node != null)
			{
				if (node.Value is SequenceEnd)
				{
					this._events.MarkDeleted(node);
					return true;
				}
				LinkedListNode<ParsingEvent> next = node.Next;
				this.HandleMerge(next);
				node = next;
			}
			return true;
		}

		private IEnumerable<ParsingEvent> GetMappingEvents(string anchor)
		{
			MergingParser.ParsingEventCloner cloner = new MergingParser.ParsingEventCloner();
			int nesting = 0;
			return from e in (from e in this._events.FromAnchor(anchor)
					select e.Value).TakeWhile<ParsingEvent>((ParsingEvent e) => (nesting += e.NestingIncrease) >= 0)
				select cloner.Clone(e);
		}

		private readonly MergingParser.ParsingEventCollection _events;

		private readonly IParser _innerParser;

		private IEnumerator<LinkedListNode<ParsingEvent>> _iterator;

		private bool _merged;

		private sealed class ParsingEventCollection : IEnumerable<LinkedListNode<ParsingEvent>>, IEnumerable
		{
			public ParsingEventCollection()
			{
				this._events = new LinkedList<ParsingEvent>();
				this._deleted = new HashSet<LinkedListNode<ParsingEvent>>();
				this._references = new Dictionary<string, LinkedListNode<ParsingEvent>>();
			}

			public void AddAfter(LinkedListNode<ParsingEvent> node, IEnumerable<ParsingEvent> items)
			{
				foreach (ParsingEvent parsingEvent in items)
				{
					node = this._events.AddAfter(node, parsingEvent);
				}
			}

			public void Add(ParsingEvent item)
			{
				LinkedListNode<ParsingEvent> linkedListNode = this._events.AddLast(item);
				this.AddReference(item, linkedListNode);
			}

			public void MarkDeleted(LinkedListNode<ParsingEvent> node)
			{
				this._deleted.Add(node);
			}

			public void CleanMarked()
			{
				foreach (LinkedListNode<ParsingEvent> linkedListNode in this._deleted)
				{
					this._events.Remove(linkedListNode);
				}
			}

			public IEnumerable<LinkedListNode<ParsingEvent>> FromAnchor(string anchor)
			{
				LinkedListNode<ParsingEvent> node = this._references[anchor].Next;
				IEnumerator<LinkedListNode<ParsingEvent>> iterator = this.GetEnumerator(node);
				while (iterator.MoveNext())
				{
					LinkedListNode<ParsingEvent> linkedListNode = iterator.Current;
					yield return linkedListNode;
				}
				yield break;
			}

			public IEnumerator<LinkedListNode<ParsingEvent>> GetEnumerator()
			{
				return this.GetEnumerator(this._events.First);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private IEnumerator<LinkedListNode<ParsingEvent>> GetEnumerator(LinkedListNode<ParsingEvent> node)
			{
				while (node != null)
				{
					yield return node;
					node = node.Next;
				}
				yield break;
			}

			private void AddReference(ParsingEvent item, LinkedListNode<ParsingEvent> node)
			{
				if (!(item is MappingStart))
				{
					return;
				}
				MappingStart mappingStart = (MappingStart)item;
				string anchor = mappingStart.Anchor;
				if (!string.IsNullOrEmpty(anchor))
				{
					this._references[anchor] = node;
				}
			}

			private readonly LinkedList<ParsingEvent> _events;

			private readonly HashSet<LinkedListNode<ParsingEvent>> _deleted;

			private readonly Dictionary<string, LinkedListNode<ParsingEvent>> _references;
		}

		private sealed class ParsingEventCloner : IParsingEventVisitor
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
