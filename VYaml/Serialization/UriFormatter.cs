using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class UriFormatter : IYamlFormatter<Uri>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, Uri value, YamlSerializationContext context)
		{
			emitter.WriteString(value.ToString(), ScalarStyle.Any);
		}

		public Uri Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			string text;
			if (parser.TryGetScalarAsString(out text) && text != null)
			{
				Uri uri = new Uri(text, UriKind.RelativeOrAbsolute);
				parser.Read();
				return uri;
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of Uri : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		public static readonly UriFormatter Instance = new UriFormatter();
	}
}
