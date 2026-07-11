using System;
using System.IO;

namespace KSerialization
{
	public class Serializer
	{
		public Serializer(BinaryWriter writer)
		{
			this.writer = writer;
		}

		public void Serialize(object obj)
		{
			Serializer.Serialize(obj, this.writer);
		}

		public static void Serialize(object obj, BinaryWriter writer)
		{
			SerializationTemplate serializationTemplate = Manager.GetSerializationTemplate(obj.GetType());
			string ktypeString = obj.GetType().GetKTypeString();
			writer.WriteKleiString(ktypeString);
			serializationTemplate.SerializeData(obj, writer);
		}

		public static void SerializeTypeless(object obj, BinaryWriter writer)
		{
			Manager.GetSerializationTemplate(obj.GetType()).SerializeData(obj, writer);
		}

		private BinaryWriter writer;
	}
}
