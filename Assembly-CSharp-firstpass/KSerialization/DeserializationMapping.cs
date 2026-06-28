using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace KSerialization
{
	public class DeserializationMapping
	{
		public DeserializationMapping(DeserializationTemplate in_template, SerializationTemplate out_template)
		{
			this.template = in_template;
			foreach (DeserializationTemplate.SerializedInfo serializedInfo in in_template.serializedMembers)
			{
				DeserializationMapping.DeserializationInfo deserializationInfo = default(DeserializationMapping.DeserializationInfo);
				deserializationInfo.valid = false;
				for (int i = 0; i < out_template.serializableFields.Count; i++)
				{
					if (out_template.serializableFields[i].field.Name == serializedInfo.name)
					{
						TypeInfo typeInfo = out_template.serializableFields[i].typeInfo;
						if (serializedInfo.typeInfo.Equals(typeInfo))
						{
							deserializationInfo.field = out_template.serializableFields[i].field;
							deserializationInfo.typeInfo = serializedInfo.typeInfo;
							deserializationInfo.valid = true;
							break;
						}
					}
				}
				if (!deserializationInfo.valid)
				{
					for (int j = 0; j < out_template.serializableProperties.Count; j++)
					{
						if (out_template.serializableProperties[j].property.Name == serializedInfo.name)
						{
							TypeInfo typeInfo2 = out_template.serializableProperties[j].typeInfo;
							if (serializedInfo.typeInfo.Equals(typeInfo2))
							{
								PropertyInfo property = out_template.serializableProperties[j].property;
								deserializationInfo.property = property;
								deserializationInfo.typeInfo = serializedInfo.typeInfo;
								deserializationInfo.valid = true;
								break;
							}
						}
					}
				}
				deserializationInfo.valid = deserializationInfo.valid && deserializationInfo.typeInfo.type != null;
				if (deserializationInfo.valid)
				{
					deserializationInfo.typeInfo.BuildGenericArgs();
				}
				else
				{
					deserializationInfo.typeInfo = serializedInfo.typeInfo;
				}
				if (deserializationInfo.typeInfo.type == null)
				{
					DebugLog.Output(DebugLog.Level.Warning, string.Format("Tried to deserialize field '{0}' on type {1} but it no longer exists", serializedInfo.name, in_template.typeName));
				}
				this.deserializationInfo.Add(deserializationInfo);
			}
		}

		public bool Deserialize(object obj, IReader reader)
		{
			if (obj == null)
			{
				throw new ArgumentException("obj cannot be null");
			}
			if (this.template.onDeserializing != null)
			{
				this.template.onDeserializing.Invoke(obj, null);
			}
			foreach (DeserializationMapping.DeserializationInfo deserializationInfo in this.deserializationInfo)
			{
				if (deserializationInfo.valid)
				{
					if (deserializationInfo.field != null)
					{
						try
						{
							object value = deserializationInfo.field.GetValue(obj);
							object obj2 = this.ReadValue(deserializationInfo.typeInfo, reader, value);
							deserializationInfo.field.SetValue(obj, obj2);
						}
						catch (Exception ex)
						{
							string text = string.Format("Exception occurred while attempting to deserialize field {0}({1}) on object {2}({3}).\n{4}", new object[]
							{
								deserializationInfo.field,
								deserializationInfo.field.FieldType,
								obj,
								obj.GetType(),
								ex.ToString()
							});
							DebugLog.Output(DebugLog.Level.Error, text);
							throw new Exception(text, ex);
						}
					}
					else
					{
						if (deserializationInfo.property == null)
						{
							throw new Exception("????");
						}
						try
						{
							object value2 = deserializationInfo.property.GetValue(obj, null);
							object obj3 = this.ReadValue(deserializationInfo.typeInfo, reader, value2);
							deserializationInfo.property.SetValue(obj, obj3, null);
						}
						catch (Exception ex2)
						{
							string text2 = string.Format("Exception occurred while attempting to deserialize property {0}({1}) on object {2}({3}).\n{4}", new object[]
							{
								deserializationInfo.property,
								deserializationInfo.property.PropertyType,
								obj,
								obj.GetType(),
								ex2.ToString()
							});
							DebugLog.Output(DebugLog.Level.Error, text2);
							throw new Exception(text2, ex2);
						}
					}
				}
				else
				{
					SerializationTypeInfo serializationTypeInfo = deserializationInfo.typeInfo.info & SerializationTypeInfo.VALUE_MASK;
					switch (serializationTypeInfo)
					{
					case SerializationTypeInfo.Array:
					{
						int num = reader.ReadInt32();
						reader.ReadInt32();
						if (num > 0)
						{
							reader.SkipBytes(num);
						}
						continue;
					}
					case SerializationTypeInfo.Pair:
						break;
					case SerializationTypeInfo.Dictionary:
					case SerializationTypeInfo.List:
					case SerializationTypeInfo.HashSet:
					{
						int num2 = reader.ReadInt32();
						reader.ReadInt32();
						reader.SkipBytes(num2);
						continue;
					}
					default:
						if (serializationTypeInfo != SerializationTypeInfo.UserDefined)
						{
							this.SkipValue(serializationTypeInfo, reader);
							continue;
						}
						break;
					}
					int num3 = reader.ReadInt32();
					if (num3 > 0)
					{
						reader.SkipBytes(num3);
					}
				}
			}
			if (this.template.onDeserialized != null)
			{
				this.template.onDeserialized.Invoke(obj, null);
			}
			return true;
		}

		private object ReadValue(TypeInfo type_info, IReader reader, object base_value)
		{
			object obj = null;
			SerializationTypeInfo serializationTypeInfo = type_info.info & SerializationTypeInfo.VALUE_MASK;
			Type type = type_info.type;
			switch (serializationTypeInfo)
			{
			case SerializationTypeInfo.UserDefined:
			{
				int num = reader.ReadInt32();
				if (num >= 0)
				{
					Type type2 = type_info.type;
					if (base_value == null)
					{
						ConstructorInfo constructor = type2.GetConstructor(Type.EmptyTypes);
						if (constructor != null)
						{
							obj = Activator.CreateInstance(type2);
						}
						else
						{
							obj = FormatterServices.GetUninitializedObject(type2);
						}
					}
					else
					{
						obj = base_value;
					}
					DeserializationMapping deserializationMapping = Manager.GetDeserializationMapping(type2);
					deserializationMapping.Deserialize(obj, reader);
				}
				break;
			}
			case SerializationTypeInfo.SByte:
				obj = reader.ReadSByte();
				break;
			case SerializationTypeInfo.Byte:
				obj = reader.ReadByte();
				break;
			case SerializationTypeInfo.Boolean:
				obj = reader.ReadByte() == 1;
				break;
			case SerializationTypeInfo.Int16:
				obj = reader.ReadInt16();
				break;
			case SerializationTypeInfo.UInt16:
				obj = reader.ReadUInt16();
				break;
			case SerializationTypeInfo.Int32:
				obj = reader.ReadInt32();
				break;
			case SerializationTypeInfo.UInt32:
				obj = reader.ReadUInt32();
				break;
			case SerializationTypeInfo.Int64:
				obj = reader.ReadInt64();
				break;
			case SerializationTypeInfo.UInt64:
				obj = reader.ReadUInt64();
				break;
			case SerializationTypeInfo.Single:
				obj = reader.ReadSingle();
				break;
			case SerializationTypeInfo.Double:
				obj = reader.ReadDouble();
				break;
			case SerializationTypeInfo.String:
				obj = reader.ReadKleiString();
				break;
			case SerializationTypeInfo.Enumeration:
			{
				int num2 = reader.ReadInt32();
				obj = Enum.ToObject(type_info.type, num2);
				break;
			}
			case SerializationTypeInfo.Vector2I:
				obj = reader.ReadVector2I();
				break;
			case SerializationTypeInfo.Vector2:
				obj = reader.ReadVector2();
				break;
			case SerializationTypeInfo.Vector3:
				obj = reader.ReadVector3();
				break;
			case SerializationTypeInfo.Array:
			{
				reader.ReadInt32();
				int num3 = reader.ReadInt32();
				if (num3 >= 0)
				{
					obj = Activator.CreateInstance(type, new object[] { num3 });
					Array array = obj as Array;
					TypeInfo typeInfo = type_info.subTypes[0];
					if (Helper.IsPOD(typeInfo.info))
					{
						this.ReadArrayFast(array, typeInfo, reader);
					}
					else
					{
						for (int i = 0; i < num3; i++)
						{
							object obj2 = this.ReadValue(typeInfo, reader, null);
							array.SetValue(obj2, i);
						}
					}
				}
				break;
			}
			case SerializationTypeInfo.Pair:
			{
				int num4 = reader.ReadInt32();
				if (num4 >= 0)
				{
					TypeInfo typeInfo2 = type_info.subTypes[0];
					TypeInfo typeInfo3 = type_info.subTypes[1];
					object obj3 = this.ReadValue(typeInfo2, reader, null);
					object obj4 = this.ReadValue(typeInfo3, reader, null);
					obj = Activator.CreateInstance(type_info.genericInstantiationType, new object[] { obj3, obj4 });
				}
				break;
			}
			case SerializationTypeInfo.Dictionary:
			{
				reader.ReadInt32();
				int num5 = reader.ReadInt32();
				if (num5 >= 0)
				{
					obj = Activator.CreateInstance(type_info.genericInstantiationType);
					IDictionary dictionary = obj as IDictionary;
					TypeInfo typeInfo4 = type_info.subTypes[1];
					Array array2 = Array.CreateInstance(typeInfo4.type, num5);
					for (int j = 0; j < num5; j++)
					{
						object obj5 = this.ReadValue(typeInfo4, reader, null);
						array2.SetValue(obj5, j);
					}
					TypeInfo typeInfo5 = type_info.subTypes[0];
					Array array3 = Array.CreateInstance(typeInfo5.type, num5);
					for (int k = 0; k < num5; k++)
					{
						object obj6 = this.ReadValue(typeInfo5, reader, null);
						array3.SetValue(obj6, k);
					}
					for (int l = 0; l < num5; l++)
					{
						dictionary.Add(array3.GetValue(l), array2.GetValue(l));
					}
				}
				break;
			}
			case SerializationTypeInfo.List:
			case SerializationTypeInfo.HashSet:
			{
				reader.ReadInt32();
				int num6 = reader.ReadInt32();
				if (num6 >= 0)
				{
					TypeInfo typeInfo6 = type_info.subTypes[0];
					Type type3 = typeInfo6.type;
					Array array4 = Array.CreateInstance(type3, num6);
					if (Helper.IsPOD(typeInfo6.info))
					{
						this.ReadArrayFast(array4, typeInfo6, reader);
					}
					else
					{
						for (int m = 0; m < num6; m++)
						{
							object obj7 = this.ReadValue(typeInfo6, reader, null);
							array4.SetValue(obj7, m);
						}
					}
					obj = Activator.CreateInstance(type_info.genericInstantiationType, new object[] { array4 });
				}
				break;
			}
			case SerializationTypeInfo.Colour:
				obj = reader.ReadColour();
				break;
			default:
				throw new ArgumentException("unknown type");
			}
			return obj;
		}

		private void ReadArrayFast(Array dest_array, TypeInfo elem_type_info, IReader reader)
		{
			byte[] array = reader.RawBytes();
			int position = reader.Position;
			int length = dest_array.Length;
			int num;
			switch (elem_type_info.info)
			{
			case SerializationTypeInfo.SByte:
			case SerializationTypeInfo.Byte:
				num = length;
				goto IL_0084;
			case SerializationTypeInfo.Int16:
			case SerializationTypeInfo.UInt16:
				num = length * 2;
				goto IL_0084;
			case SerializationTypeInfo.Int32:
			case SerializationTypeInfo.UInt32:
			case SerializationTypeInfo.Single:
				num = length * 4;
				goto IL_0084;
			case SerializationTypeInfo.Int64:
			case SerializationTypeInfo.UInt64:
			case SerializationTypeInfo.Double:
				num = length * 8;
				goto IL_0084;
			}
			throw new Exception("unknown pod type");
			IL_0084:
			Buffer.BlockCopy(array, position, dest_array, 0, num);
			reader.SkipBytes(num);
		}

		private void SkipValue(SerializationTypeInfo type_info, IReader reader)
		{
			switch (type_info)
			{
			case SerializationTypeInfo.SByte:
			case SerializationTypeInfo.Byte:
			case SerializationTypeInfo.Boolean:
				reader.SkipBytes(1);
				return;
			case SerializationTypeInfo.Int16:
			case SerializationTypeInfo.UInt16:
				reader.SkipBytes(2);
				return;
			case SerializationTypeInfo.Int32:
			case SerializationTypeInfo.UInt32:
			case SerializationTypeInfo.Single:
			case SerializationTypeInfo.Enumeration:
				reader.SkipBytes(4);
				return;
			case SerializationTypeInfo.Int64:
			case SerializationTypeInfo.UInt64:
			case SerializationTypeInfo.Double:
			case SerializationTypeInfo.Vector2I:
			case SerializationTypeInfo.Vector2:
				reader.SkipBytes(8);
				return;
			case SerializationTypeInfo.String:
			{
				int num = reader.ReadInt32();
				if (num > 0)
				{
					reader.SkipBytes(num);
				}
				return;
			}
			case SerializationTypeInfo.Vector3:
				reader.SkipBytes(12);
				return;
			case SerializationTypeInfo.Colour:
				reader.SkipBytes(4);
				return;
			}
			throw new ArgumentException("Unhandled type. Not sure how to skip by");
		}

		private DeserializationTemplate template;

		private List<DeserializationMapping.DeserializationInfo> deserializationInfo = new List<DeserializationMapping.DeserializationInfo>();

		private struct DeserializationInfo
		{
			public bool valid;

			public FieldInfo field;

			public PropertyInfo property;

			public TypeInfo typeInfo;
		}
	}
}
