using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class UInt32Formatter : IYamlFormatter<uint>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, uint value, YamlSerializationContext context)
		{
			emitter.WriteUInt32(value);
		}

		[NullableContext(1)]
		public uint Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			uint scalarAsUInt = parser.GetScalarAsUInt32();
			parser.Read();
			return scalarAsUInt;
		}

		[Nullable(1)]
		public static readonly UInt32Formatter Instance = new UInt32Formatter();
	}
}
