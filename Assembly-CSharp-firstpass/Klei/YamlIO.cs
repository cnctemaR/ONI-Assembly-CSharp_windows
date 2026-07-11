using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Klei
{
	public class YamlIO<T>
	{
		public void Save(string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				SerializerBuilder serializerBuilder = new SerializerBuilder();
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tuple in tagMappings)
					{
						serializerBuilder = serializerBuilder.WithTagMapping(tuple.first, tuple.second);
					}
				}
				Serializer serializer = serializerBuilder.Build();
				serializer.Serialize(streamWriter, this);
			}
		}

		public static T LoadFile(string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			string text = ((LayeredFileSystem.instance == null) ? File.ReadAllText(filename) : LayeredFileSystem.instance.ReadText(filename));
			T t = YamlIO<T>.Parse(text, tagMappings);
			if (t == null)
			{
				Debug.LogWarning("Exception while loading yaml file [" + filename + "]");
			}
			return t;
		}

		public static T Parse(string readText, List<Tuple<string, Type>> tagMappings = null)
		{
			try
			{
				readText = readText.Replace("\t", "    ");
				DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
				deserializerBuilder.IgnoreUnmatchedProperties();
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tuple in tagMappings)
					{
						deserializerBuilder = deserializerBuilder.WithTagMapping(tuple.first, tuple.second);
					}
				}
				Deserializer deserializer = deserializerBuilder.Build();
				StringReader stringReader = new StringReader(readText);
				return deserializer.Deserialize<T>(stringReader);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				DebugUtil.DevLogError("Exception while loading yaml data: " + message + "\n YAML FILE:\n" + readText);
			}
			return default(T);
		}
	}
}
