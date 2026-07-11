using System;
using System.Globalization;
using System.IO;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public static class ParserExtensions
	{
		public static T Expect<T>(this IParser parser) where T : ParsingEvent
		{
			T t = parser.Allow<T>();
			if (t == null)
			{
				ParsingEvent parsingEvent = parser.Current;
				throw new YamlException(parsingEvent.Start, parsingEvent.End, string.Format(CultureInfo.InvariantCulture, "Expected '{0}', got '{1}' (at {2}).", typeof(T).Name, parsingEvent.GetType().Name, parsingEvent.Start));
			}
			return t;
		}

		public static bool Accept<T>(this IParser parser) where T : ParsingEvent
		{
			if (parser.Current == null && !parser.MoveNext())
			{
				throw new EndOfStreamException();
			}
			return parser.Current is T;
		}

		public static T Allow<T>(this IParser parser) where T : ParsingEvent
		{
			if (!parser.Accept<T>())
			{
				return default(T);
			}
			T t = (T)((object)parser.Current);
			parser.MoveNext();
			return t;
		}

		public static T Peek<T>(this IParser parser) where T : ParsingEvent
		{
			if (!parser.Accept<T>())
			{
				return default(T);
			}
			return (T)((object)parser.Current);
		}

		public static void SkipThisAndNestedEvents(this IParser parser)
		{
			int num = 0;
			do
			{
				num += parser.Peek<ParsingEvent>().NestingIncrease;
				parser.MoveNext();
			}
			while (num > 0);
		}
	}
}
