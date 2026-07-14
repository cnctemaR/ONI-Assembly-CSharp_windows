using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class SByteFormatter : IYamlFormatter<sbyte>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, sbyte value, YamlSerializationContext context)
		{
			emitter.WriteInt32((int)value);
		}

		[NullableContext(1)]
		public sbyte Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			sbyte scalarAsInt = (sbyte)parser.GetScalarAsInt32();
			parser.Read();
			return scalarAsInt;
		}

		[Nullable(1)]
		public static readonly SByteFormatter Instance = new SByteFormatter();
	}
}
