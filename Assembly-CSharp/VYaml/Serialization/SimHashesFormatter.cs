using System;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class SimHashesFormatter : IYamlFormatter<SimHashes>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, SimHashes value, YamlSerializationContext context)
		{
			emitter.BeginSequence(SequenceStyle.Flow);
			emitter.WriteString(value.ToString(), ScalarStyle.Any);
			emitter.EndSequence();
		}

		public SimHashes Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return (SimHashes)0;
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			Element element = ElementLoader.FindElementByName(parser.ReadScalarAsString());
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			if (element == null)
			{
				Debug.LogWarning("SimHashesFormatter: Could not find element, defaulting to Unobtanium");
				return SimHashes.Unobtanium;
			}
			return element.id;
		}

		public static readonly SimHashesFormatter Instance = new SimHashesFormatter();
	}
}
