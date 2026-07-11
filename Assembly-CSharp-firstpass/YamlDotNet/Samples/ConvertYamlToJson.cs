using System;
using System.IO;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;

namespace YamlDotNet.Samples
{
	public class ConvertYamlToJson
	{
		public ConvertYamlToJson(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Sample(Title = "Convert YAML to JSON", Description = "Shows how to convert a YAML document to JSON.")]
		public void Main()
		{
			StringReader stringReader = new StringReader("\nscalar: a scalar\nsequence:\n  - one\n  - two\n");
			object obj = new DeserializerBuilder().Build().Deserialize(stringReader);
			string text = new SerializerBuilder().JsonCompatible().Build().Serialize(obj);
			this.output.WriteLine(text);
		}

		private readonly ITestOutputHelper output;
	}
}
