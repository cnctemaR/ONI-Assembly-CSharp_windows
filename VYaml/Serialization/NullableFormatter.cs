using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableFormatter<T> : IYamlFormatter<T?>, IYamlFormatter where T : struct
	{
		public void Serialize(ref Utf8YamlEmitter emitter, T? value, [Nullable(1)] YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			context.Resolver.GetFormatterWithVerify<T>().Serialize(ref emitter, value.Value, context);
		}

		public T? Deserialize(ref YamlParser parser, [Nullable(1)] YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			return new T?(context.DeserializeWithAlias<T>(ref parser));
		}
	}
}
