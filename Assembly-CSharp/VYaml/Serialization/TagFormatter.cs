using System;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class TagFormatter : IYamlFormatter<global::Tag>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, global::Tag value, YamlSerializationContext context)
		{
			emitter.BeginSequence(SequenceStyle.Flow);
			emitter.WriteString(value.Name, ScalarStyle.Any);
			emitter.EndSequence();
		}

		public global::Tag Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return default(global::Tag);
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			string text = parser.ReadScalarAsString();
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return TagManager.Create(text);
		}

		public static readonly TagFormatter Instance = new TagFormatter();
	}
}
