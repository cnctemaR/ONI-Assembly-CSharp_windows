using System;

namespace KSerialization
{
	public class Deserializer
	{
		public Deserializer(IReader reader)
		{
			this.reader = reader;
		}

		public bool Deserialize(object obj)
		{
			return Deserializer.Deserialize(obj, this.reader);
		}

		public static bool Deserialize(object obj, IReader reader)
		{
			string text = reader.ReadKleiString();
			Type type = obj.GetType();
			string ktypeString = type.GetKTypeString();
			return ktypeString == text && Deserializer.DeserializeTypeless(type, obj, reader);
		}

		public static bool DeserializeTypeless(Type type, object obj, IReader reader)
		{
			DeserializationMapping deserializationMapping = Manager.GetDeserializationMapping(type);
			bool flag = false;
			try
			{
				flag = deserializationMapping.Deserialize(obj, reader);
			}
			catch (Exception ex)
			{
				string text = string.Format("Exception occurred while attempting to deserialize object {0}({1}).\n{2}", obj, obj.GetType(), ex.ToString());
				DebugLog.Output(DebugLog.Level.Error, text);
				throw new Exception(text, ex);
			}
			return flag;
		}

		public static bool DeserializeTypeless(object obj, IReader reader)
		{
			Type type = obj.GetType();
			DeserializationMapping deserializationMapping = Manager.GetDeserializationMapping(type);
			bool flag;
			try
			{
				flag = deserializationMapping.Deserialize(obj, reader);
			}
			catch (Exception ex)
			{
				string text = string.Format("Exception occurred while attempting to deserialize object {0}({1}).\n{2}", obj, obj.GetType(), ex.ToString());
				DebugLog.Output(DebugLog.Level.Error, text);
				throw new Exception(text, ex);
			}
			return flag;
		}

		public IReader reader;
	}
}
