using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Klei
{
	public class YamlIO<T>
	{
		public void Save(string filename)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				SerializerBuilder serializerBuilder = new SerializerBuilder();
				Serializer serializer = serializerBuilder.Build();
				serializer.Serialize(streamWriter, this);
			}
		}

		public static T LoadFile(string filename)
		{
			string text = ((LayeredFileSystem.instance == null) ? File.ReadAllText(filename) : LayeredFileSystem.instance.ReadText(filename));
			text = text.Replace("\t", "    ");
			T t = YamlIO<T>.Parse(text, filename);
			if (t == null)
			{
				Debug.LogWarning("Exception while loading yaml file [" + filename + "]", null);
			}
			return t;
		}

		public static T Parse(string readText, string filename)
		{
			try
			{
				readText = readText.Replace("\t", "    ");
				DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
				deserializerBuilder.IgnoreUnmatchedProperties();
				Deserializer deserializer = deserializerBuilder.Build();
				StringReader stringReader = new StringReader(readText);
				return deserializer.Deserialize<T>(stringReader);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				Output.LogWarning(new object[] { "Exception while loading yaml data: " + message + "\n YAML FILE:\n" + readText });
			}
			return default(T);
		}
	}
}
