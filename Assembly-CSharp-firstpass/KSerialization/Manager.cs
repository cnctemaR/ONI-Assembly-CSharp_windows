using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KSerialization
{
	public class Manager
	{
		public static Type GetType(string type_name)
		{
			Type type = Type.GetType(type_name);
			if (type == null)
			{
				foreach (Assembly assembly in Manager.assemblies)
				{
					type = assembly.GetType(type_name);
					if (type != null)
					{
						break;
					}
				}
			}
			if (type == null)
			{
				DebugLog.Output(DebugLog.Level.Warning, "Failed to find type named: " + type_name);
			}
			return type;
		}

		public static TypeInfo GetTypeInfo(Type type)
		{
			TypeInfo typeInfo;
			if (!Manager.typeInfoMap.TryGetValue(type, out typeInfo))
			{
				typeInfo = Manager.EncodeTypeInfo(type);
			}
			return typeInfo;
		}

		public static SerializationTemplate GetSerializationTemplate(Type type)
		{
			if (type == null)
			{
				throw new InvalidOperationException("Invalid type encountered when serializing");
			}
			SerializationTemplate serializationTemplate = null;
			if (!Manager.serializationTemplatesByType.TryGetValue(type, out serializationTemplate))
			{
				serializationTemplate = new SerializationTemplate(type);
				Manager.serializationTemplatesByType[type] = serializationTemplate;
				Manager.serializationTemplatesByTypeName[type.GetKTypeString()] = serializationTemplate;
			}
			return serializationTemplate;
		}

		public static SerializationTemplate GetSerializationTemplate(string type_name)
		{
			if (type_name == null || type_name == "")
			{
				throw new InvalidOperationException("Invalid type name encountered when serializing");
			}
			SerializationTemplate serializationTemplate = null;
			if (!Manager.serializationTemplatesByTypeName.TryGetValue(type_name, out serializationTemplate))
			{
				Type type = Manager.GetType(type_name);
				if (type != null)
				{
					serializationTemplate = new SerializationTemplate(type);
					Manager.serializationTemplatesByType[type] = serializationTemplate;
					Manager.serializationTemplatesByTypeName[type_name] = serializationTemplate;
				}
			}
			return serializationTemplate;
		}

		public static DeserializationTemplate GetDeserializationTemplate(Type type)
		{
			DeserializationTemplate deserializationTemplate = null;
			Manager.deserializationTemplatesByType.TryGetValue(type, out deserializationTemplate);
			return deserializationTemplate;
		}

		public static DeserializationTemplate GetDeserializationTemplate(string type_name)
		{
			DeserializationTemplate deserializationTemplate = null;
			Manager.deserializationTemplatesByTypeName.TryGetValue(type_name, out deserializationTemplate);
			return deserializationTemplate;
		}

		public static void SerializeDirectory(BinaryWriter writer)
		{
			writer.Write(Manager.serializationTemplatesByTypeName.Count);
			foreach (KeyValuePair<string, SerializationTemplate> keyValuePair in Manager.serializationTemplatesByTypeName)
			{
				string key = keyValuePair.Key;
				SerializationTemplate value = keyValuePair.Value;
				try
				{
					writer.WriteKleiString(key);
					value.SerializeTemplate(writer);
				}
				catch (Exception ex)
				{
					Output.LogError(new object[]
					{
						"Error serializing template " + key + "\n",
						ex.Message,
						ex.StackTrace
					});
				}
			}
		}

		public static void DeserializeDirectory(IReader reader)
		{
			Manager.deserializationTemplatesByTypeName.Clear();
			Manager.deserializationTemplatesByType.Clear();
			Manager.deserializationMappings.Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				try
				{
					DeserializationTemplate deserializationTemplate = new DeserializationTemplate(text, reader);
					Manager.deserializationTemplatesByTypeName[text] = deserializationTemplate;
					Type type = Manager.GetType(text);
					if (type != null)
					{
						Manager.deserializationTemplatesByType[type] = deserializationTemplate;
					}
				}
				catch (Exception ex)
				{
					string text2 = string.Concat(new string[] { "Error deserializing template ", text, "\n", ex.Message, "\n", ex.StackTrace });
					Output.LogError(new object[] { text2 });
					throw new Exception(text2, ex);
				}
			}
		}

		public static void Clear()
		{
			Manager.serializationTemplatesByTypeName.Clear();
			Manager.serializationTemplatesByType.Clear();
			Manager.deserializationTemplatesByTypeName.Clear();
			Manager.deserializationTemplatesByType.Clear();
			Manager.deserializationMappings.Clear();
			Manager.typeInfoMap.Clear();
		}

		public static bool HasDeserializationMapping(Type type)
		{
			return Manager.GetDeserializationTemplate(type) != null && Manager.GetSerializationTemplate(type) != null;
		}

		public static DeserializationMapping GetDeserializationMapping(Type type)
		{
			DeserializationTemplate deserializationTemplate = Manager.GetDeserializationTemplate(type);
			if (deserializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize a class named: " + type.GetKTypeString() + " but no such class exists");
			}
			SerializationTemplate serializationTemplate = Manager.GetSerializationTemplate(type);
			if (serializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize into a class named: " + type.GetKTypeString() + " but no such class exists");
			}
			return Manager.GetMapping(deserializationTemplate, serializationTemplate);
		}

		public static DeserializationMapping GetDeserializationMapping(string type_name)
		{
			DeserializationTemplate deserializationTemplate = Manager.GetDeserializationTemplate(type_name);
			if (deserializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize a class named: " + type_name + " but no such class exists");
			}
			SerializationTemplate serializationTemplate = Manager.GetSerializationTemplate(type_name);
			if (serializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize into a class named: " + type_name + " but no such class exists");
			}
			return Manager.GetMapping(deserializationTemplate, serializationTemplate);
		}

		private static DeserializationMapping GetMapping(DeserializationTemplate dtemplate, SerializationTemplate stemplate)
		{
			KeyValuePair<SerializationTemplate, DeserializationMapping> keyValuePair;
			DeserializationMapping deserializationMapping;
			if (Manager.deserializationMappings.TryGetValue(dtemplate, out keyValuePair))
			{
				deserializationMapping = keyValuePair.Value;
			}
			else
			{
				deserializationMapping = new DeserializationMapping(dtemplate, stemplate);
				keyValuePair = new KeyValuePair<SerializationTemplate, DeserializationMapping>(stemplate, deserializationMapping);
				Manager.deserializationMappings[dtemplate] = keyValuePair;
			}
			return deserializationMapping;
		}

		private static TypeInfo EncodeTypeInfo(Type type)
		{
			TypeInfo typeInfo = new TypeInfo();
			typeInfo.type = type;
			typeInfo.info = Helper.EncodeSerializationType(type);
			if (type.IsGenericType)
			{
				typeInfo.genericTypeArgs = type.GetGenericArguments();
				typeInfo.subTypes = new TypeInfo[typeInfo.genericTypeArgs.Length];
				for (int i = 0; i < typeInfo.genericTypeArgs.Length; i++)
				{
					typeInfo.subTypes[i] = Manager.GetTypeInfo(typeInfo.genericTypeArgs[i]);
				}
			}
			else if (typeof(Array).IsAssignableFrom(type))
			{
				Type elementType = type.GetElementType();
				typeInfo.subTypes = new TypeInfo[1];
				typeInfo.subTypes[0] = Manager.GetTypeInfo(elementType);
			}
			return typeInfo;
		}

		private static Dictionary<string, SerializationTemplate> serializationTemplatesByTypeName = new Dictionary<string, SerializationTemplate>();

		private static Dictionary<string, DeserializationTemplate> deserializationTemplatesByTypeName = new Dictionary<string, DeserializationTemplate>();

		private static Dictionary<Type, SerializationTemplate> serializationTemplatesByType = new Dictionary<Type, SerializationTemplate>();

		private static Dictionary<Type, DeserializationTemplate> deserializationTemplatesByType = new Dictionary<Type, DeserializationTemplate>();

		private static Dictionary<DeserializationTemplate, KeyValuePair<SerializationTemplate, DeserializationMapping>> deserializationMappings = new Dictionary<DeserializationTemplate, KeyValuePair<SerializationTemplate, DeserializationMapping>>();

		private static Dictionary<Type, TypeInfo> typeInfoMap = new Dictionary<Type, TypeInfo>();

		public static Assembly[] assemblies = null;
	}
}
