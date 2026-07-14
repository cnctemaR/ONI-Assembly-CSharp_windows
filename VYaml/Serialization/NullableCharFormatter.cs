using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableCharFormatter : IYamlFormatter<char?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, char? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteInt32((int)value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public char? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			char scalarAsUInt = (char)parser.GetScalarAsUInt32();
			parser.Read();
			return new char?(scalarAsUInt);
		}

		[Nullable(1)]
		public static readonly NullableCharFormatter Instance = new NullableCharFormatter();
	}
}
