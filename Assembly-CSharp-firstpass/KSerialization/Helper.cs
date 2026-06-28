using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KSerialization
{
	public static class Helper
	{
		public static bool IsUserDefinedType(SerializationTypeInfo type_info)
		{
			return (byte)(type_info & SerializationTypeInfo.VALUE_MASK) == 0;
		}

		public static bool IsArray(SerializationTypeInfo type_info)
		{
			return (byte)(type_info & SerializationTypeInfo.VALUE_MASK) == 17;
		}

		public static bool IsGenericType(SerializationTypeInfo type_info)
		{
			return (byte)(type_info & SerializationTypeInfo.IS_GENERIC_TYPE) != 0;
		}

		public static SerializationTypeInfo EncodeSerializationType(Type type)
		{
			if (type == typeof(sbyte))
			{
				return SerializationTypeInfo.SByte;
			}
			if (type == typeof(byte))
			{
				return SerializationTypeInfo.Byte;
			}
			if (type == typeof(bool))
			{
				return SerializationTypeInfo.Boolean;
			}
			if (type == typeof(short))
			{
				return SerializationTypeInfo.Int16;
			}
			if (type == typeof(ushort))
			{
				return SerializationTypeInfo.UInt16;
			}
			if (type == typeof(int))
			{
				return SerializationTypeInfo.Int32;
			}
			if (type == typeof(uint))
			{
				return SerializationTypeInfo.UInt32;
			}
			if (type == typeof(long))
			{
				return SerializationTypeInfo.Int64;
			}
			if (type == typeof(ulong))
			{
				return SerializationTypeInfo.UInt64;
			}
			if (type == typeof(float))
			{
				return SerializationTypeInfo.Single;
			}
			if (type == typeof(double))
			{
				return SerializationTypeInfo.Double;
			}
			if (type == typeof(string))
			{
				return SerializationTypeInfo.String;
			}
			if (type == typeof(Vector2I))
			{
				return SerializationTypeInfo.Vector2I;
			}
			if (type == typeof(Vector2))
			{
				return SerializationTypeInfo.Vector2;
			}
			if (type == typeof(Vector3))
			{
				return SerializationTypeInfo.Vector3;
			}
			if (type == typeof(Color))
			{
				return SerializationTypeInfo.Colour;
			}
			if (typeof(Array).IsAssignableFrom(type))
			{
				return SerializationTypeInfo.Array;
			}
			if (type.IsEnum)
			{
				return SerializationTypeInfo.Enumeration;
			}
			if (type.IsGenericType)
			{
				SerializationTypeInfo serializationTypeInfo = SerializationTypeInfo.IS_GENERIC_TYPE;
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(List<>))
				{
					serializationTypeInfo |= SerializationTypeInfo.List;
				}
				else if (genericTypeDefinition == typeof(Dictionary<, >))
				{
					serializationTypeInfo |= SerializationTypeInfo.Dictionary;
				}
				else if (genericTypeDefinition == typeof(HashSet<>))
				{
					serializationTypeInfo |= SerializationTypeInfo.HashSet;
				}
				else if (genericTypeDefinition == typeof(KeyValuePair<, >))
				{
					serializationTypeInfo |= SerializationTypeInfo.Pair;
				}
				else
				{
					serializationTypeInfo |= SerializationTypeInfo.UserDefined;
				}
				return serializationTypeInfo;
			}
			return SerializationTypeInfo.UserDefined;
		}

		public static void WriteValue(this BinaryWriter writer, TypeInfo type_info, object value)
		{
			switch ((byte)(type_info.info & SerializationTypeInfo.VALUE_MASK))
			{
			case 0:
				if (value != null)
				{
					long position = writer.BaseStream.Position;
					writer.Write(0);
					long position2 = writer.BaseStream.Position;
					SerializationTemplate serializationTemplate = Manager.GetSerializationTemplate(type_info.type);
					serializationTemplate.SerializeData(value, writer);
					long position3 = writer.BaseStream.Position;
					long num = position3 - position2;
					writer.BaseStream.Position = position;
					writer.Write((int)num);
					writer.BaseStream.Position = position3;
				}
				else
				{
					writer.Write(-1);
				}
				break;
			case 1:
				writer.Write((sbyte)value);
				break;
			case 2:
				writer.Write((byte)value);
				break;
			case 3:
				writer.Write((!(bool)value) ? 0 : 1);
				break;
			case 4:
				writer.Write((short)value);
				break;
			case 5:
				writer.Write((ushort)value);
				break;
			case 6:
				writer.Write((int)value);
				break;
			case 7:
				writer.Write((uint)value);
				break;
			case 8:
				writer.Write((long)value);
				break;
			case 9:
				writer.Write((ulong)value);
				break;
			case 10:
				writer.Write((float)value);
				break;
			case 11:
				writer.Write((double)value);
				break;
			case 12:
				writer.WriteKleiString((string)value);
				break;
			case 13:
				writer.Write((int)value);
				break;
			case 14:
			{
				Vector2I vector2I = (Vector2I)value;
				writer.Write(vector2I.x);
				writer.Write(vector2I.y);
				break;
			}
			case 15:
			{
				Vector2 vector = (Vector2)value;
				writer.Write(vector.x);
				writer.Write(vector.y);
				break;
			}
			case 16:
			{
				Vector3 vector2 = (Vector3)value;
				writer.Write(vector2.x);
				writer.Write(vector2.y);
				writer.Write(vector2.z);
				break;
			}
			case 17:
				if (value != null)
				{
					Array array = value as Array;
					TypeInfo typeInfo = type_info.subTypes[0];
					long position4 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(array.Length);
					long position5 = writer.BaseStream.Position;
					if (Helper.IsPOD(typeInfo.info))
					{
						Helper.WriteArrayFast(writer, typeInfo, array);
					}
					else
					{
						for (int i = 0; i < array.Length; i++)
						{
							writer.WriteValue(typeInfo, array.GetValue(i));
						}
					}
					long position6 = writer.BaseStream.Position;
					long num2 = position6 - position5;
					writer.BaseStream.Position = position4;
					writer.Write((int)num2);
					writer.BaseStream.Position = position6;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case 18:
				if (value != null)
				{
					PropertyInfo property = type_info.type.GetProperty("Key");
					PropertyInfo property2 = type_info.type.GetProperty("Value");
					object value2 = property.GetValue(value, null);
					object value3 = property2.GetValue(value, null);
					TypeInfo typeInfo2 = type_info.subTypes[0];
					TypeInfo typeInfo3 = type_info.subTypes[1];
					long position7 = writer.BaseStream.Position;
					writer.Write(0);
					long position8 = writer.BaseStream.Position;
					writer.WriteValue(typeInfo2, value2);
					writer.WriteValue(typeInfo3, value3);
					long position9 = writer.BaseStream.Position;
					long num3 = position9 - position8;
					writer.BaseStream.Position = position7;
					writer.Write((int)num3);
					writer.BaseStream.Position = position9;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case 19:
				if (value != null)
				{
					TypeInfo typeInfo4 = type_info.subTypes[0];
					TypeInfo typeInfo5 = type_info.subTypes[1];
					IDictionary dictionary = value as IDictionary;
					ICollection keys = dictionary.Keys;
					ICollection values = dictionary.Values;
					long position10 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(values.Count);
					long position11 = writer.BaseStream.Position;
					foreach (object obj in values)
					{
						writer.WriteValue(typeInfo5, obj);
					}
					foreach (object obj2 in keys)
					{
						writer.WriteValue(typeInfo4, obj2);
					}
					long position12 = writer.BaseStream.Position;
					long num4 = position12 - position11;
					writer.BaseStream.Position = position10;
					writer.Write((int)num4);
					writer.BaseStream.Position = position12;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case 20:
				if (value != null)
				{
					TypeInfo typeInfo6 = type_info.subTypes[0];
					ICollection collection = value as ICollection;
					long position13 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(collection.Count);
					long position14 = writer.BaseStream.Position;
					foreach (object obj3 in collection)
					{
						writer.WriteValue(typeInfo6, obj3);
					}
					long position15 = writer.BaseStream.Position;
					long num5 = position15 - position14;
					writer.BaseStream.Position = position13;
					writer.Write((int)num5);
					writer.BaseStream.Position = position15;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case 21:
				if (value != null)
				{
					TypeInfo typeInfo7 = type_info.subTypes[0];
					int num6 = (int)type_info.type.GetProperty("Count").GetValue(value, null);
					IEnumerable enumerable = value as IEnumerable;
					long position16 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(num6);
					long position17 = writer.BaseStream.Position;
					foreach (object obj4 in enumerable)
					{
						writer.WriteValue(typeInfo7, obj4);
					}
					long position18 = writer.BaseStream.Position;
					long num7 = position18 - position17;
					writer.BaseStream.Position = position16;
					writer.Write((int)num7);
					writer.BaseStream.Position = position18;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case 22:
			{
				Color color = (Color)value;
				writer.Write((byte)(color.r * 255f));
				writer.Write((byte)(color.g * 255f));
				writer.Write((byte)(color.b * 255f));
				writer.Write((byte)(color.a * 255f));
				break;
			}
			default:
				throw new ArgumentException("Don't know how to serialize type: " + type_info.type.ToString());
			}
		}

		private static void WriteArrayFast(BinaryWriter writer, TypeInfo elem_type_info, Array array)
		{
			switch (elem_type_info.info)
			{
			case SerializationTypeInfo.SByte:
			{
				sbyte[] array2 = (sbyte[])array;
				for (int i = 0; i < array.Length; i++)
				{
					writer.Write(array2[i]);
				}
				return;
			}
			case SerializationTypeInfo.Byte:
				writer.Write((byte[])array);
				return;
			case SerializationTypeInfo.Int16:
			{
				short[] array3 = (short[])array;
				for (int j = 0; j < array.Length; j++)
				{
					writer.Write(array3[j]);
				}
				return;
			}
			case SerializationTypeInfo.UInt16:
			{
				ushort[] array4 = (ushort[])array;
				for (int k = 0; k < array.Length; k++)
				{
					writer.Write(array4[k]);
				}
				return;
			}
			case SerializationTypeInfo.Int32:
			{
				int[] array5 = (int[])array;
				for (int l = 0; l < array.Length; l++)
				{
					writer.Write(array5[l]);
				}
				return;
			}
			case SerializationTypeInfo.UInt32:
			{
				uint[] array6 = (uint[])array;
				for (int m = 0; m < array.Length; m++)
				{
					writer.Write(array6[m]);
				}
				return;
			}
			case SerializationTypeInfo.Int64:
			{
				long[] array7 = (long[])array;
				for (int n = 0; n < array.Length; n++)
				{
					writer.Write(array7[n]);
				}
				return;
			}
			case SerializationTypeInfo.UInt64:
			{
				ulong[] array8 = (ulong[])array;
				for (int num = 0; num < array.Length; num++)
				{
					writer.Write(array8[num]);
				}
				return;
			}
			case SerializationTypeInfo.Single:
			{
				float[] array9 = (float[])array;
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					writer.Write(array9[num2]);
				}
				return;
			}
			case SerializationTypeInfo.Double:
			{
				double[] array10 = (double[])array;
				for (int num3 = 0; num3 < array.Length; num3++)
				{
					writer.Write(array10[num3]);
				}
				return;
			}
			}
			throw new Exception("unknown pod type");
		}

		public static void GetSerializationMethods(this Type type, Type type_a, Type type_b, out MethodInfo method_a, out MethodInfo method_b)
		{
			method_a = null;
			method_b = null;
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object[] customAttributes = methodInfo.GetCustomAttributes(false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					if (customAttributes[j].GetType() == type_a)
					{
						method_a = methodInfo;
					}
					else if (customAttributes[j].GetType() == type_b)
					{
						method_b = methodInfo;
					}
				}
			}
		}

		public static bool IsPOD(SerializationTypeInfo info)
		{
			switch (info)
			{
			case SerializationTypeInfo.SByte:
			case SerializationTypeInfo.Byte:
			case SerializationTypeInfo.Int16:
			case SerializationTypeInfo.UInt16:
			case SerializationTypeInfo.Int32:
			case SerializationTypeInfo.UInt32:
			case SerializationTypeInfo.Int64:
			case SerializationTypeInfo.UInt64:
			case SerializationTypeInfo.Single:
			case SerializationTypeInfo.Double:
				return true;
			}
			return false;
		}

		public static string GetKTypeString(this Type type)
		{
			return type.FullName;
		}
	}
}
