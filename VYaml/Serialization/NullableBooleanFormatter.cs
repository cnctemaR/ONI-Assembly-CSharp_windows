using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableBooleanFormatter : IYamlFormatter<bool?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, bool? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteBool(value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public bool? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			bool scalarAsBool = parser.GetScalarAsBool();
			parser.Read();
			return new bool?(scalarAsBool);
		}

		[Nullable(1)]
		public static readonly NullableBooleanFormatter Instance = new NullableBooleanFormatter();
	}
}
