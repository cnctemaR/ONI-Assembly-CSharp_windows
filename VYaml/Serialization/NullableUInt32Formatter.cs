using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableUInt32Formatter : IYamlFormatter<uint?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, uint? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteUInt32(value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public uint? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			uint scalarAsUInt = parser.GetScalarAsUInt32();
			parser.Read();
			return new uint?(scalarAsUInt);
		}

		[Nullable(1)]
		public static readonly NullableUInt32Formatter Instance = new NullableUInt32Formatter();
	}
}
