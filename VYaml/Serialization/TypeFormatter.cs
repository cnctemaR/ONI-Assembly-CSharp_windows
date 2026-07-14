using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class TypeFormatter : IYamlFormatter<Type>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] Type value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.WriteString(value.AssemblyQualifiedName, ScalarStyle.Any);
		}

		[return: Nullable(2)]
		public Type Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return null;
			}
			return Type.GetType(parser.ReadScalarAsString(), true);
		}

		public static readonly TypeFormatter Instance = new TypeFormatter();
	}
}
