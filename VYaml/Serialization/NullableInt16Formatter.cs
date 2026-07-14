using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableInt16Formatter : IYamlFormatter<short?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, short? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteInt32((int)value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public short? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			short scalarAsInt = (short)parser.GetScalarAsInt32();
			parser.Read();
			return new short?(scalarAsInt);
		}

		[Nullable(1)]
		public static readonly NullableInt16Formatter Instance = new NullableInt16Formatter();
	}
}
