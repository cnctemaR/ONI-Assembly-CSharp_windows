using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableSByteFormatter : IYamlFormatter<sbyte?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, sbyte? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteInt32((int)value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public sbyte? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			sbyte scalarAsInt = (sbyte)parser.GetScalarAsInt32();
			parser.Read();
			return new sbyte?(scalarAsInt);
		}

		[Nullable(1)]
		public static readonly NullableSByteFormatter Instance = new NullableSByteFormatter();
	}
}
