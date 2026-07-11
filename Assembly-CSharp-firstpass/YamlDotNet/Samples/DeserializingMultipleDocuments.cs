using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;

namespace YamlDotNet.Samples
{
	public class DeserializingMultipleDocuments
	{
		public DeserializingMultipleDocuments(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Sample(Title = "Deserializing multiple documents", Description = "Explains how to load multiple YAML documents from a stream.")]
		public void Main()
		{
			TextReader textReader = new StringReader("---\n- Prisoner\n- Goblet\n- Phoenix\n---\n- Memoirs\n- Snow \n- Ghost\t\t\n...");
			Deserializer deserializer = new DeserializerBuilder().Build();
			Parser parser = new Parser(textReader);
			parser.Expect<StreamStart>();
			while (parser.Accept<DocumentStart>())
			{
				List<string> list = deserializer.Deserialize<List<string>>(parser);
				this.output.WriteLine("## Document");
				foreach (string text in list)
				{
					this.output.WriteLine(text);
				}
			}
		}

		private readonly ITestOutputHelper output;

		private const string Document = "---\n- Prisoner\n- Goblet\n- Phoenix\n---\n- Memoirs\n- Snow \n- Ghost\t\t\n...";
	}
}
