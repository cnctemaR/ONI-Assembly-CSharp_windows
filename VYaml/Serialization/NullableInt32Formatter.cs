using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableInt32Formatter : IYamlFormatter<int?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, int? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteInt32(value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public int? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			int scalarAsInt = parser.GetScalarAsInt32();
			parser.Read();
			return new int?(scalarAsInt);
		}

		[Nullable(1)]
		public static readonly NullableInt32Formatter Instance = new NullableInt32Formatter();
	}
}
