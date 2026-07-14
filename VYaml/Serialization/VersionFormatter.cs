using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class VersionFormatter : IYamlFormatter<Version>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] Version value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.WriteString(value.ToString(), ScalarStyle.Any);
		}

		[return: Nullable(2)]
		public Version Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (!parser.IsNullScalar())
			{
				return new Version(parser.ReadScalarAsString());
			}
			return null;
		}

		public static readonly VersionFormatter Instance = new VersionFormatter();
	}
}
