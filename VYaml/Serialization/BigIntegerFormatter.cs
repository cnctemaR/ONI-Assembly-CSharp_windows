using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class BigIntegerFormatter : IYamlFormatter<BigInteger>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, BigInteger value, YamlSerializationContext context)
		{
			emitter.WriteString(value.ToString(), ScalarStyle.Any);
		}

		[NullableContext(1)]
		public BigInteger Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return default(BigInteger);
			}
			return BigInteger.Parse(parser.ReadScalarAsString());
		}

		[Nullable(1)]
		public static readonly BigIntegerFormatter Instance = new BigIntegerFormatter();
	}
}
