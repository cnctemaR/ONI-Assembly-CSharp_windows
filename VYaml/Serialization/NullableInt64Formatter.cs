using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableInt64Formatter : IYamlFormatter<long?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, long? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteInt64(value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public long? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			long scalarAsInt = parser.GetScalarAsInt64();
			parser.Read();
			return new long?(scalarAsInt);
		}

		[Nullable(1)]
		public static readonly NullableInt64Formatter Instance = new NullableInt64Formatter();
	}
}
