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
			return (type_info & SerializationTypeInfo.VALUE_MASK) == SerializationTypeInfo.UserDefined;
		}

		public static bool IsArray(SerializationTypeInfo type_info)
		{
			return (type_info & SerializationTypeInfo.VALUE_MASK) == SerializationTypeInfo.Array;
		}

		public static bool IsGenericType(SerializationTypeInfo type_info)
		{
			return (type_info & SerializationTypeInfo.IS_GENERIC_TYPE) > SerializationTypeInfo.UserDefined;
		}

		public static bool IsValueType(SerializationTypeInfo type_info)
		{
			return (type_info & SerializationTypeInfo.IS_VALUE_TYPE) > SerializationTypeInfo.UserDefined;
		}

		public static SerializationTypeInfo EncodeSerializationType(Type type)
		{
			SerializationTypeInfo serializationTypeInfo;
			if (type == typeof(sbyte))
			{
				serializationTypeInfo = SerializationTypeInfo.SByte;
			}
			else if (type == typeof(byte))
			{
				serializationTypeInfo = SerializationTypeInfo.Byte;
			}
			else if (type == typeof(bool))
			{
				serializationTypeInfo = SerializationTypeInfo.Boolean;
			}
			else if (type == typeof(short))
			{
				serializationTypeInfo = SerializationTypeInfo.Int16;
			}
			else if (type == typeof(ushort))
			{
				serializationTypeInfo = SerializationTypeInfo.UInt16;
			}
			else if (type == typeof(int))
			{
				serializationTypeInfo = SerializationTypeInfo.Int32;
			}
			else if (type == typeof(uint))
			{
				serializationTypeInfo = SerializationTypeInfo.UInt32;
			}
			else if (type == typeof(long))
			{
				serializationTypeInfo = SerializationTypeInfo.Int64;
			}
			else if (type == typeof(ulong))
			{
				serializationTypeInfo = SerializationTypeInfo.UInt64;
			}
			else if (type == typeof(float))
			{
				serializationTypeInfo = SerializationTypeInfo.Single;
			}
			else if (type == typeof(double))
			{
				serializationTypeInfo = SerializationTypeInfo.Double;
			}
			else if (type == typeof(string))
			{
				serializationTypeInfo = SerializationTypeInfo.String;
			}
			else if (type == typeof(Vector2I))
			{
				serializationTypeInfo = SerializationTypeInfo.Vector2I;
			}
			else if (type == typeof(Vector2))
			{
				serializationTypeInfo = SerializationTypeInfo.Vector2;
			}
			else if (type == typeof(Vector3))
			{
				serializationTypeInfo = SerializationTypeInfo.Vector3;
			}
			else if (type == typeof(Color))
			{
				serializationTypeInfo = SerializationTypeInfo.Colour;
			}
			else if (typeof(Array).IsAssignableFrom(type))
			{
				serializationTypeInfo = SerializationTypeInfo.Array;
			}
			else if (type.IsEnum)
			{
				serializationTypeInfo = SerializationTypeInfo.Enumeration;
			}
			else if (type.IsGenericType)
			{
				serializationTypeInfo = SerializationTypeInfo.IS_GENERIC_TYPE;
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
				else if (genericTypeDefinition == typeof(Queue<>))
				{
					serializationTypeInfo |= SerializationTypeInfo.Queue;
				}
				else
				{
					serializationTypeInfo |= SerializationTypeInfo.UserDefined;
				}
			}
			else
			{
				serializationTypeInfo = SerializationTypeInfo.UserDefined;
				if (type.IsValueType)
				{
					serializationTypeInfo |= SerializationTypeInfo.IS_VALUE_TYPE;
				}
			}
			return serializationTypeInfo & Helper.TYPE_INFO_MASK;
		}

		public static void WriteValue(this BinaryWriter writer, TypeInfo type_info, object value)
		{
			switch (type_info.info & SerializationTypeInfo.VALUE_MASK)
			{
			case SerializationTypeInfo.UserDefined:
				if (value != null)
				{
					long position = writer.BaseStream.Position;
					writer.Write(0);
					long position2 = writer.BaseStream.Position;
					Manager.GetSerializationTemplate(type_info.type).SerializeData(value, writer);
					long position3 = writer.BaseStream.Position;
					long num = position3 - position2;
					writer.BaseStream.Position = position;
					writer.Write((int)num);
					writer.BaseStream.Position = position3;
					return;
				}
				writer.Write(-1);
				return;
			case SerializationTypeInfo.SByte:
				writer.Write((sbyte)value);
				return;
			case SerializationTypeInfo.Byte:
				writer.Write((byte)value);
				return;
			case SerializationTypeInfo.Boolean:
				writer.Write(((bool)value) ? 1 : 0);
				return;
			case SerializationTypeInfo.Int16:
				writer.Write((short)value);
				return;
			case SerializationTypeInfo.UInt16:
				writer.Write((ushort)value);
				return;
			case SerializationTypeInfo.Int32:
				writer.Write((int)value);
				return;
			case SerializationTypeInfo.UInt32:
				writer.Write((uint)value);
				return;
			case SerializationTypeInfo.Int64:
				writer.Write((long)value);
				return;
			case SerializationTypeInfo.UInt64:
				writer.Write((ulong)value);
				return;
			case SerializationTypeInfo.Single:
				writer.WriteSingleFast((float)value);
				return;
			case SerializationTypeInfo.Double:
				writer.Write((double)value);
				return;
			case SerializationTypeInfo.String:
				writer.WriteKleiString((string)value);
				return;
			case SerializationTypeInfo.Enumeration:
				writer.Write((int)value);
				return;
			case SerializationTypeInfo.Vector2I:
			{
				Vector2I vector2I = (Vector2I)value;
				writer.Write(vector2I.x);
				writer.Write(vector2I.y);
				return;
			}
			case SerializationTypeInfo.Vector2:
			{
				Vector2 vector = (Vector2)value;
				writer.WriteSingleFast(vector.x);
				writer.WriteSingleFast(vector.y);
				return;
			}
			case SerializationTypeInfo.Vector3:
			{
				Vector3 vector2 = (Vector3)value;
				writer.WriteSingleFast(vector2.x);
				writer.WriteSingleFast(vector2.y);
				writer.WriteSingleFast(vector2.z);
				return;
			}
			case SerializationTypeInfo.Array:
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
					else if (Helper.IsValueType(typeInfo.info))
					{
						SerializationTemplate serializationTemplate = Manager.GetSerializationTemplate(typeInfo.type);
						for (int i = 0; i < array.Length; i++)
						{
							serializationTemplate.SerializeData(array.GetValue(i), writer);
						}
					}
					else
					{
						for (int j = 0; j < array.Length; j++)
						{
							writer.WriteValue(typeInfo, array.GetValue(j));
						}
					}
					long position6 = writer.BaseStream.Position;
					long num2 = position6 - position5;
					writer.BaseStream.Position = position4;
					writer.Write((int)num2);
					writer.BaseStream.Position = position6;
					return;
				}
				writer.Write(4);
				writer.Write(-1);
				return;
			case SerializationTypeInfo.Pair:
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
					return;
				}
				writer.Write(4);
				writer.Write(-1);
				return;
			case SerializationTypeInfo.Dictionary:
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
					return;
				}
				writer.Write(4);
				writer.Write(-1);
				return;
			case SerializationTypeInfo.List:
				if (value != null)
				{
					TypeInfo typeInfo6 = type_info.subTypes[0];
					ICollection collection = value as ICollection;
					long position13 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(collection.Count);
					long position14 = writer.BaseStream.Position;
					if (Helper.IsPOD(typeInfo6.info))
					{
						Helper.WriteListPOD(writer, typeInfo6, collection);
					}
					else
					{
						if (Helper.IsValueType(typeInfo6.info))
						{
							SerializationTemplate serializationTemplate2 = Manager.GetSerializationTemplate(typeInfo6.type);
							using (IEnumerator enumerator = collection.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									object obj3 = enumerator.Current;
									serializationTemplate2.SerializeData(obj3, writer);
								}
								goto IL_05A4;
							}
						}
						foreach (object obj4 in collection)
						{
							writer.WriteValue(typeInfo6, obj4);
						}
					}
					IL_05A4:
					long position15 = writer.BaseStream.Position;
					long num5 = position15 - position14;
					writer.BaseStream.Position = position13;
					writer.Write((int)num5);
					writer.BaseStream.Position = position15;
					return;
				}
				writer.Write(4);
				writer.Write(-1);
				return;
			case SerializationTypeInfo.HashSet:
				if (value != null)
				{
					TypeInfo typeInfo7 = type_info.subTypes[0];
					long position16 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(0);
					long position17 = writer.BaseStream.Position;
					int num6 = 0;
					IEnumerable enumerable = value as IEnumerable;
					if (Helper.IsValueType(typeInfo7.info))
					{
						SerializationTemplate serializationTemplate3 = Manager.GetSerializationTemplate(typeInfo7.type);
						using (IEnumerator enumerator = enumerable.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj5 = enumerator.Current;
								serializationTemplate3.SerializeData(obj5, writer);
								num6++;
							}
							goto IL_045A;
						}
					}
					foreach (object obj6 in enumerable)
					{
						writer.WriteValue(typeInfo7, obj6);
						num6++;
					}
					IL_045A:
					long position18 = writer.BaseStream.Position;
					long num7 = position18 - position17;
					writer.BaseStream.Position = position16;
					writer.Write((int)num7);
					writer.Write(num6);
					writer.BaseStream.Position = position18;
					return;
				}
				writer.Write(4);
				writer.Write(-1);
				return;
			case SerializationTypeInfo.Queue:
				if (value != null)
				{
					TypeInfo typeInfo8 = type_info.subTypes[0];
					ICollection collection2 = value as ICollection;
					long position19 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(collection2.Count);
					long position20 = writer.BaseStream.Position;
					if (Helper.IsPOD(typeInfo8.info))
					{
						Helper.WriteListPOD(writer, typeInfo8, collection2);
					}
					else
					{
						if (Helper.IsValueType(typeInfo8.info))
						{
							SerializationTemplate serializationTemplate4 = Manager.GetSerializationTemplate(typeInfo8.type);
							using (IEnumerator enumerator = collection2.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									object obj7 = enumerator.Current;
									serializationTemplate4.SerializeData(obj7, writer);
								}
								goto IL_08D2;
							}
						}
						foreach (object obj8 in collection2)
						{
							writer.WriteValue(typeInfo8, obj8);
						}
					}
					IL_08D2:
					long position21 = writer.BaseStream.Position;
					long num8 = position21 - position20;
					writer.BaseStream.Position = position19;
					writer.Write((int)num8);
					writer.BaseStream.Position = position21;
					return;
				}
				writer.Write(4);
				writer.Write(-1);
				return;
			case SerializationTypeInfo.Colour:
			{
				Color color = (Color)value;
				writer.Write((byte)(color.r * 255f));
				writer.Write((byte)(color.g * 255f));
				writer.Write((byte)(color.b * 255f));
				writer.Write((byte)(color.a * 255f));
				return;
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
					writer.WriteSingleFast(array9[num2]);
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

		private static void WriteListPOD(BinaryWriter writer, TypeInfo elem_type_info, ICollection collection)
		{
			switch (elem_type_info.info)
			{
			case SerializationTypeInfo.SByte:
			{
				using (IEnumerator enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						writer.Write((sbyte)obj);
					}
					return;
				}
				break;
			}
			case SerializationTypeInfo.Byte:
				break;
			case SerializationTypeInfo.Boolean:
				return;
			case SerializationTypeInfo.Int16:
				goto IL_00B2;
			case SerializationTypeInfo.UInt16:
				goto IL_00EE;
			case SerializationTypeInfo.Int32:
				goto IL_012A;
			case SerializationTypeInfo.UInt32:
				goto IL_0166;
			case SerializationTypeInfo.Int64:
				goto IL_01A2;
			case SerializationTypeInfo.UInt64:
				goto IL_01DE;
			case SerializationTypeInfo.Single:
				goto IL_021A;
			case SerializationTypeInfo.Double:
				goto IL_0253;
			default:
				return;
			}
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj2 = enumerator.Current;
					writer.Write((byte)obj2);
				}
				return;
			}
			IL_00B2:
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj3 = enumerator.Current;
					writer.Write((short)obj3);
				}
				return;
			}
			IL_00EE:
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj4 = enumerator.Current;
					writer.Write((ushort)obj4);
				}
				return;
			}
			IL_012A:
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj5 = enumerator.Current;
					writer.Write((int)obj5);
				}
				return;
			}
			IL_0166:
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj6 = enumerator.Current;
					writer.Write((uint)obj6);
				}
				return;
			}
			IL_01A2:
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj7 = enumerator.Current;
					writer.Write((long)obj7);
				}
				return;
			}
			IL_01DE:
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj8 = enumerator.Current;
					writer.Write((ulong)obj8);
				}
				return;
			}
			IL_021A:
			using (IEnumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj9 = enumerator.Current;
					writer.WriteSingleFast((float)obj9);
				}
				return;
			}
			IL_0253:
			foreach (object obj10 in collection)
			{
				writer.Write((double)obj10);
			}
		}

		public static void GetSerializationMethods(this Type type, Type type_a, Type type_b, Type type_c, out MethodInfo method_a, out MethodInfo method_b, out MethodInfo method_c)
		{
			method_a = null;
			method_b = null;
			method_c = null;
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
					else if (customAttributes[j].GetType() == type_c)
					{
						method_c = methodInfo;
					}
				}
			}
		}

		public static bool IsPOD(SerializationTypeInfo info)
		{
			return info - SerializationTypeInfo.SByte <= 1 || info - SerializationTypeInfo.Int16 <= 7;
		}

		public static bool IsPOD(Type type)
		{
			return type == typeof(int) || type == typeof(uint) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(float) || type == typeof(double) || type == typeof(short) || type == typeof(ushort) || type == typeof(long) || type == typeof(ulong);
		}

		public static string GetKTypeString(this Type type)
		{
			return type.FullName;
		}

		public static void ClearTypeInfoMask()
		{
			Helper.TYPE_INFO_MASK = (SerializationTypeInfo)255;
		}

		public static void SetTypeInfoMask(SerializationTypeInfo mask)
		{
			Helper.TYPE_INFO_MASK = mask;
		}

		private static SerializationTypeInfo TYPE_INFO_MASK = (SerializationTypeInfo)255;
	}
}
