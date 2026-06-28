using System;

namespace YamlDotNet.Core.Events
{
	public class Comment : ParsingEvent
	{
		public string Value { get; private set; }

		public bool IsInline { get; private set; }

		public Comment(string value, bool isInline)
			: this(value, isInline, Mark.Empty, Mark.Empty)
		{
		}

		public Comment(string value, bool isInline, Mark start, Mark end)
			: base(start, end)
		{
			this.Value = value;
			this.IsInline = isInline;
		}

		internal override EventType Type
		{
			get
			{
				return EventType.Comment;
			}
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
