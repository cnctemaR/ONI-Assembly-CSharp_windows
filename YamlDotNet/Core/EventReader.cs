using System;
using System.Globalization;
using System.IO;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public class EventReader
	{
		public EventReader(IParser parser)
		{
			this.parser = parser;
			this.MoveNext();
		}

		public IParser Parser
		{
			get
			{
				return this.parser;
			}
		}

		public T Expect<T>() where T : ParsingEvent
		{
			T t = this.Allow<T>();
			if (t == null)
			{
				ParsingEvent parsingEvent = this.parser.Current;
				throw new YamlException(parsingEvent.Start, parsingEvent.End, string.Format(CultureInfo.InvariantCulture, "Expected '{0}', got '{1}' (at {2}).", new object[]
				{
					typeof(T).Name,
					parsingEvent.GetType().Name,
					parsingEvent.Start
				}));
			}
			return t;
		}

		public bool Accept<T>() where T : ParsingEvent
		{
			this.ThrowIfAtEndOfStream();
			return this.parser.Current is T;
		}

		private void ThrowIfAtEndOfStream()
		{
			if (this.endOfStream)
			{
				throw new EndOfStreamException();
			}
		}

		public T Allow<T>() where T : ParsingEvent
		{
			if (!this.Accept<T>())
			{
				return default(T);
			}
			T t = (T)((object)this.parser.Current);
			this.MoveNext();
			return t;
		}

		public T Peek<T>() where T : ParsingEvent
		{
			if (!this.Accept<T>())
			{
				return default(T);
			}
			return (T)((object)this.parser.Current);
		}

		public void SkipThisAndNestedEvents()
		{
			int num = 0;
			do
			{
				num += this.Peek<ParsingEvent>().NestingIncrease;
				this.MoveNext();
			}
			while (num > 0);
		}

		private void MoveNext()
		{
			this.endOfStream = !this.parser.MoveNext();
		}

		private readonly IParser parser;

		private bool endOfStream;
	}
}
