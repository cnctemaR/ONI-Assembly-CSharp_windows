using System;

namespace YamlDotNet.Core.Events
{
	public abstract class ParsingEvent
	{
		public virtual int NestingIncrease
		{
			get
			{
				return 0;
			}
		}

		internal abstract EventType Type { get; }

		public Mark Start
		{
			get
			{
				return this.start;
			}
		}

		public Mark End
		{
			get
			{
				return this.end;
			}
		}

		public abstract void Accept(IParsingEventVisitor visitor);

		internal ParsingEvent(Mark start, Mark end)
		{
			this.start = start;
			this.end = end;
		}

		private readonly Mark start;

		private readonly Mark end;
	}
}
