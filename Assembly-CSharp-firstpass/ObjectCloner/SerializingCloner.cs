using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectCloner
{
	public static class SerializingCloner
	{
		public static T Copy<T>(T obj)
		{
			T t;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, obj);
				memoryStream.Position = 0L;
				t = (T)((object)binaryFormatter.Deserialize(memoryStream));
			}
			return t;
		}
	}
}
