using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class NullableByteFormatter : IYamlFormatter<byte?>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, byte? value, YamlSerializationContext context)
		{
			if (value != null)
			{
				emitter.WriteInt32((int)value.GetValueOrDefault());
				return;
			}
			emitter.WriteNull();
		}

		[NullableContext(1)]
		public byte? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			byte scalarAsUInt = (byte)parser.GetScalarAsUInt32();
			parser.Read();
			return new byte?(scalarAsUInt);
		}

		[Nullable(1)]
		public static readonly NullableByteFormatter Instance = new NullableByteFormatter();
	}
}
