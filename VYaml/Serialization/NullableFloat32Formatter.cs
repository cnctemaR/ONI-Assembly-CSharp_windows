using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableFloat32Formatter : IYamlFormatter<float?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, float? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteFloat(value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public float? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			float scalarAsFloat = parser.GetScalarAsFloat();
			parser.Read();
			return new float?(scalarAsFloat);
		}

		[Nullable(1)]
		public static readonly NullableFloat32Formatter Instance = new NullableFloat32Formatter();
	}
}
