using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class Int64Formatter : IYamlFormatter<long>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, long value, YamlSerializationContext context)
		{
			emitter.WriteInt64(value);
		}

		[NullableContext(1)]
		public long Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			long scalarAsInt = parser.GetScalarAsInt64();
			parser.Read();
			return scalarAsInt;
		}

		[Nullable(1)]
		public static readonly Int64Formatter Instance = new Int64Formatter();
	}
}
