using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class UInt16Formatter : IYamlFormatter<ushort>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, ushort value, YamlSerializationContext context)
		{
			emitter.WriteUInt32((uint)value);
		}

		[NullableContext(1)]
		public ushort Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			ushort scalarAsUInt = (ushort)parser.GetScalarAsUInt32();
			parser.Read();
			return scalarAsUInt;
		}

		[Nullable(1)]
		public static readonly UInt16Formatter Instance = new UInt16Formatter();
	}
}
