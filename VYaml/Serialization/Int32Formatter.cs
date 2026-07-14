using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class Int32Formatter : IYamlFormatter<int>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, int value, YamlSerializationContext context)
		{
			emitter.WriteInt32(value);
		}

		[NullableContext(1)]
		public int Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			int scalarAsInt = parser.GetScalarAsInt32();
			parser.Read();
			return scalarAsInt;
		}

		[Nullable(1)]
		public static readonly Int32Formatter Instance = new Int32Formatter();
	}
}
