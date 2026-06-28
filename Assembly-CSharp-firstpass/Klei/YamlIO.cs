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
				Serializer serializer = new Serializer(SerializationOptions.None, null);
				serializer.Serialize(streamWriter, this);
			}
		}

		public static T LoadFile(string filename)
		{
			string text = File.ReadAllText(filename);
			T t = YamlIO<T>.Parse(text);
			if (t == null)
			{
				Debug.LogError("Exception while loading yaml file [" + filename + "]", null);
			}
			return t;
		}

		public static T Parse(string readText)
		{
			try
			{
				Deserializer deserializer = new Deserializer(null, null, true);
				StringReader stringReader = new StringReader(readText);
				return deserializer.Deserialize<T>(stringReader);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				Output.LogError(new object[] { "Exception while loading yaml data: " + message });
			}
			return default(T);
		}
	}
}
