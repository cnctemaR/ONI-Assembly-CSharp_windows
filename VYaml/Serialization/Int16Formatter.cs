using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class Int16Formatter : IYamlFormatter<short>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, short value, YamlSerializationContext context)
		{
			emitter.WriteInt32((int)value);
		}

		[NullableContext(1)]
		public short Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			short scalarAsInt = (short)parser.GetScalarAsInt32();
			parser.Read();
			return scalarAsInt;
		}

		[Nullable(1)]
		public static readonly Int16Formatter Instance = new Int16Formatter();
	}
}
