using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization
{
	public sealed class Deserializer
	{
		public Deserializer()
			: this(new DeserializerBuilder().BuildValueDeserializer())
		{
		}

		private Deserializer(IValueDeserializer valueDeserializer)
		{
			if (valueDeserializer == null)
			{
				throw new ArgumentNullException("valueDeserializer");
			}
			this.valueDeserializer = valueDeserializer;
		}

		public static Deserializer FromValueDeserializer(IValueDeserializer valueDeserializer)
		{
			return new Deserializer(valueDeserializer);
		}

		public T Deserialize<T>(string input)
		{
			T t;
			using (StringReader stringReader = new StringReader(input))
			{
				t = (T)((object)this.Deserialize(stringReader, typeof(T)));
			}
			return t;
		}

		public T Deserialize<T>(TextReader input)
		{
			return (T)((object)this.Deserialize(input, typeof(T)));
		}

		public object Deserialize(TextReader input)
		{
			return this.Deserialize(input, typeof(object));
		}

		public object Deserialize(string input, Type type)
		{
			object obj;
			using (StringReader stringReader = new StringReader(input))
			{
				obj = this.Deserialize(stringReader, type);
			}
			return obj;
		}

		public object Deserialize(TextReader input, Type type)
		{
			return this.Deserialize(new Parser(input), type);
		}

		public T Deserialize<T>(IParser parser)
		{
			return (T)((object)this.Deserialize(parser, typeof(T)));
		}

		public object Deserialize(IParser parser)
		{
			return this.Deserialize(parser, typeof(object));
		}

		public object Deserialize(IParser parser, Type type)
		{
			if (parser == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = parser.Allow<StreamStart>() != null;
			bool flag2 = parser.Allow<DocumentStart>() != null;
			object obj = null;
			if (!parser.Accept<DocumentEnd>() && !parser.Accept<StreamEnd>())
			{
				using (SerializerState serializerState = new SerializerState())
				{
					obj = this.valueDeserializer.DeserializeValue(parser, type, serializerState, this.valueDeserializer);
					serializerState.OnDeserialization();
				}
			}
			if (flag2)
			{
				parser.Expect<DocumentEnd>();
			}
			if (flag)
			{
				parser.Expect<StreamEnd>();
			}
			return obj;
		}

		private readonly IValueDeserializer valueDeserializer;
	}
}
