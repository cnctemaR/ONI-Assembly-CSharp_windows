using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableUInt64Formatter : IYamlFormatter<ulong?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, ulong? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteUInt64(value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public ulong? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			ulong scalarAsUInt = parser.GetScalarAsUInt64();
			parser.Read();
			return new ulong?(scalarAsUInt);
		}

		[Nullable(1)]
		public static readonly NullableUInt64Formatter Instance = new NullableUInt64Formatter();
	}
}
