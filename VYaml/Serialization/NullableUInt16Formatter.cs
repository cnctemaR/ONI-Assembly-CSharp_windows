using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableUInt16Formatter : IYamlFormatter<ushort?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, ushort? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteUInt32((uint)value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public ushort? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			ushort scalarAsUInt = (ushort)parser.GetScalarAsUInt32();
			parser.Read();
			return new ushort?(scalarAsUInt);
		}

		[Nullable(1)]
		public static readonly NullableUInt16Formatter Instance = new NullableUInt16Formatter();
	}
}
