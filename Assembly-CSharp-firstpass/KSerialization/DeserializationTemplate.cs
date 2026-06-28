using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace KSerialization
{
	public class DeserializationTemplate
	{
		public DeserializationTemplate(string template_type_name, IReader reader)
		{
			this.typeName = template_type_name;
			DebugLog.Output(DebugLog.Level.Info, "Loading Deserialization Template: " + template_type_name);
			Type type = Manager.GetType(template_type_name);
			if (type != null)
			{
				type.GetSerializationMethods(typeof(OnDeserializingAttribute), typeof(OnDeserializedAttribute), out this.onDeserializing, out this.onDeserialized);
			}
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				DebugLog.Output(DebugLog.Level.Info, "Field " + i.ToString());
				string text = reader.ReadKleiString();
				DebugLog.Output(DebugLog.Level.Info, "Field " + i.ToString() + " == " + text);
				TypeInfo typeInfo = this.ReadType(reader);
				if (typeInfo.type == null)
				{
					string text2 = string.Format("Unknown type encountered while dserializing template {0} field {1} ({2}) at offset {3}", new object[] { template_type_name, i, text, reader.Position });
					DebugLog.Output(DebugLog.Level.Warning, text2);
				}
				this.serializedMembers.Add(new DeserializationTemplate.SerializedInfo
				{
					name = text,
					typeInfo = typeInfo
				});
			}
			for (int j = 0; j < num2; j++)
			{
				DebugLog.Output(DebugLog.Level.Info, "Property " + j.ToString());
				string text3 = reader.ReadKleiString();
				DebugLog.Output(DebugLog.Level.Info, "Property " + j.ToString() + " == " + text3);
				TypeInfo typeInfo2 = this.ReadType(reader);
				if (typeInfo2.type == null)
				{
					string text4 = string.Format("Unknown type encountered while dserializing template {0} property {1} ({2}) at offset {3}", new object[] { template_type_name, j, text3, reader.Position });
					DebugLog.Output(DebugLog.Level.Info, text4);
				}
				this.serializedMembers.Add(new DeserializationTemplate.SerializedInfo
				{
					name = text3,
					typeInfo = typeInfo2
				});
			}
			DebugLog.Output(DebugLog.Level.Info, "Finished loading template " + template_type_name);
		}

		private TypeInfo ReadType(IReader reader)
		{
			TypeInfo typeInfo = new TypeInfo();
			byte b = reader.ReadByte();
			typeInfo.info = (SerializationTypeInfo)b;
			SerializationTypeInfo serializationTypeInfo = typeInfo.info & SerializationTypeInfo.VALUE_MASK;
			if (Helper.IsGenericType(typeInfo.info))
			{
				Type type = null;
				switch (serializationTypeInfo)
				{
				case SerializationTypeInfo.Pair:
					type = typeof(KeyValuePair<, >);
					break;
				case SerializationTypeInfo.Dictionary:
					type = typeof(Dictionary<, >);
					break;
				case SerializationTypeInfo.List:
					type = typeof(List<>);
					break;
				case SerializationTypeInfo.HashSet:
					type = typeof(HashSet<>);
					break;
				default:
				{
					if (serializationTypeInfo != SerializationTypeInfo.UserDefined)
					{
						throw new ArgumentException("unknown type");
					}
					string text = reader.ReadKleiString();
					typeInfo.type = Manager.GetType(text);
					break;
				}
				}
				byte b2 = reader.ReadByte();
				Type[] array = new Type[(int)b2];
				typeInfo.subTypes = new TypeInfo[(int)b2];
				for (int i = 0; i < (int)b2; i++)
				{
					typeInfo.subTypes[i] = this.ReadType(reader);
					array[i] = typeInfo.subTypes[i].type;
				}
				if (type != null)
				{
					if (array == null || Array.IndexOf<Type>(array, null) != -1)
					{
						typeInfo.type = null;
						return typeInfo;
					}
					typeInfo.type = type.MakeGenericType(array);
				}
				else if (typeInfo.type != null)
				{
					Type[] genericArguments = typeInfo.type.GetGenericArguments();
					if (genericArguments.Length != (int)b2)
					{
						throw new InvalidOperationException("User defined generic type mismatch");
					}
					for (int j = 0; j < (int)b2; j++)
					{
						if (array[j] != genericArguments[j])
						{
							throw new InvalidOperationException("User defined generic type mismatch");
						}
					}
				}
			}
			else
			{
				switch (serializationTypeInfo)
				{
				case SerializationTypeInfo.UserDefined:
				case SerializationTypeInfo.Enumeration:
				{
					string text2 = reader.ReadKleiString();
					typeInfo.type = Manager.GetType(text2);
					goto IL_03FE;
				}
				case SerializationTypeInfo.SByte:
					typeInfo.type = typeof(sbyte);
					goto IL_03FE;
				case SerializationTypeInfo.Byte:
					typeInfo.type = typeof(byte);
					goto IL_03FE;
				case SerializationTypeInfo.Boolean:
					typeInfo.type = typeof(bool);
					goto IL_03FE;
				case SerializationTypeInfo.Int16:
					typeInfo.type = typeof(short);
					goto IL_03FE;
				case SerializationTypeInfo.UInt16:
					typeInfo.type = typeof(ushort);
					goto IL_03FE;
				case SerializationTypeInfo.Int32:
					typeInfo.type = typeof(int);
					goto IL_03FE;
				case SerializationTypeInfo.UInt32:
					typeInfo.type = typeof(uint);
					goto IL_03FE;
				case SerializationTypeInfo.Int64:
					typeInfo.type = typeof(long);
					goto IL_03FE;
				case SerializationTypeInfo.UInt64:
					typeInfo.type = typeof(ulong);
					goto IL_03FE;
				case SerializationTypeInfo.Single:
					typeInfo.type = typeof(float);
					goto IL_03FE;
				case SerializationTypeInfo.Double:
					typeInfo.type = typeof(double);
					goto IL_03FE;
				case SerializationTypeInfo.String:
					typeInfo.type = typeof(string);
					goto IL_03FE;
				case SerializationTypeInfo.Vector2I:
					typeInfo.type = typeof(Vector2I);
					goto IL_03FE;
				case SerializationTypeInfo.Vector2:
					typeInfo.type = typeof(Vector2);
					goto IL_03FE;
				case SerializationTypeInfo.Vector3:
					typeInfo.type = typeof(Vector3);
					goto IL_03FE;
				case SerializationTypeInfo.Array:
					typeInfo.subTypes = new TypeInfo[1];
					typeInfo.subTypes[0] = this.ReadType(reader);
					if (typeInfo.subTypes[0].type != null)
					{
						typeInfo.type = typeInfo.subTypes[0].type.MakeArrayType();
					}
					else
					{
						typeInfo.type = null;
					}
					goto IL_03FE;
				case SerializationTypeInfo.Colour:
					typeInfo.type = typeof(Color);
					goto IL_03FE;
				}
				throw new ArgumentException("unknown type");
				IL_03FE:;
			}
			return typeInfo;
		}

		public string typeName;

		public MethodInfo onDeserializing;

		public MethodInfo onDeserialized;

		public List<DeserializationTemplate.SerializedInfo> serializedMembers = new List<DeserializationTemplate.SerializedInfo>();

		public struct SerializedInfo
		{
			public string name;

			public TypeInfo typeInfo;
		}
	}
}
