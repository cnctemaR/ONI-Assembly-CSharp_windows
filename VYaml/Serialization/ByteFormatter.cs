using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ByteFormatter : IYamlFormatter<byte>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, byte value, YamlSerializationContext context)
		{
			emitter.WriteInt32((int)value);
		}

		[NullableContext(1)]
		public byte Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			byte scalarAsUInt = (byte)parser.GetScalarAsUInt32();
			parser.Read();
			return scalarAsUInt;
		}

		[Nullable(1)]
		public static readonly ByteFormatter Instance = new ByteFormatter();
	}
}
