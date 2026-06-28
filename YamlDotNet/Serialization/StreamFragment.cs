using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
	public sealed class StreamFragment : IYamlConvertible
	{
		public IList<ParsingEvent> Events
		{
			get
			{
				return this.events;
			}
		}

		void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
		{
			this.events.Clear();
			int num = 0;
			while (parser.MoveNext())
			{
				ParsingEvent parsingEvent = parser.Current;
				this.events.Add(parsingEvent);
				num += parser.Current.NestingIncrease;
				if (num <= 0)
				{
					return;
				}
			}
			throw new InvalidOperationException("The parser has reached the end before deserialization completed.");
		}

		void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
		{
			foreach (ParsingEvent parsingEvent in this.events)
			{
				emitter.Emit(parsingEvent);
			}
		}

		private readonly List<ParsingEvent> events = new List<ParsingEvent>();
	}
}
