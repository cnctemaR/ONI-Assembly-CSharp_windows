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
			SerializationTypeInfo serializationTypeInfo = type_info & SerializationTypeInfo.VALUE_MASK;
			return serializationTypeInfo == SerializationTypeInfo.Array;
		}

		public static bool IsGenericType(SerializationTypeInfo type_info)
		{
			return (byte)(type_info & SerializationTypeInfo.IS_GENERIC_TYPE) != 0;
		}

		public static bool IsValueType(SerializationTypeInfo type_info)
		{
			return (byte)(type_info & SerializationTypeInfo.IS_VALUE_TYPE) != 0;
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
					else if (Helper.IsValueType(typeInfo.info))
					{
						SerializationTemplate serializationTemplate2 = Manager.GetSerializationTemplate(typeInfo.type);
						for (int i = 0; i < array.Length; i++)
						{
							serializationTemplate2.SerializeData(array.GetValue(i), writer);
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
					IEnumerator enumerator = values.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							writer.WriteValue(typeInfo5, obj);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = enumerator as IDisposable) != null)
						{
							disposable.Dispose();
						}
					}
					IEnumerator enumerator2 = keys.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							writer.WriteValue(typeInfo4, obj2);
						}
					}
					finally
					{
						IDisposable disposable2;
						if ((disposable2 = enumerator2 as IDisposable) != null)
						{
							disposable2.Dispose();
						}
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
					if (Helper.IsPOD(typeInfo6.info))
					{
						Helper.WriteListPOD(writer, typeInfo6, collection);
					}
					else if (Helper.IsValueType(typeInfo6.info))
					{
						SerializationTemplate serializationTemplate3 = Manager.GetSerializationTemplate(typeInfo6.type);
						IEnumerator enumerator3 = collection.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								object obj3 = enumerator3.Current;
								serializationTemplate3.SerializeData(obj3, writer);
							}
						}
						finally
						{
							IDisposable disposable3;
							if ((disposable3 = enumerator3 as IDisposable) != null)
							{
								disposable3.Dispose();
							}
						}
					}
					else
					{
						IEnumerator enumerator4 = collection.GetEnumerator();
						try
						{
							while (enumerator4.MoveNext())
							{
								object obj4 = enumerator4.Current;
								writer.WriteValue(typeInfo6, obj4);
							}
						}
						finally
						{
							IDisposable disposable4;
							if ((disposable4 = enumerator4 as IDisposable) != null)
							{
								disposable4.Dispose();
							}
						}
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
					long position16 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(0);
					long position17 = writer.BaseStream.Position;
					int num6 = 0;
					IEnumerable enumerable = value as IEnumerable;
					if (Helper.IsValueType(typeInfo7.info))
					{
						SerializationTemplate serializationTemplate4 = Manager.GetSerializationTemplate(typeInfo7.type);
						IEnumerator enumerator5 = enumerable.GetEnumerator();
						try
						{
							while (enumerator5.MoveNext())
							{
								object obj5 = enumerator5.Current;
								serializationTemplate4.SerializeData(obj5, writer);
								num6++;
							}
						}
						finally
						{
							IDisposable disposable5;
							if ((disposable5 = enumerator5 as IDisposable) != null)
							{
								disposable5.Dispose();
							}
						}
					}
					else
					{
						IEnumerator enumerator6 = enumerable.GetEnumerator();
						try
						{
							while (enumerator6.MoveNext())
							{
								object obj6 = enumerator6.Current;
								writer.WriteValue(typeInfo7, obj6);
								num6++;
							}
						}
						finally
						{
							IDisposable disposable6;
							if ((disposable6 = enumerator6 as IDisposable) != null)
							{
								disposable6.Dispose();
							}
						}
					}
					long position18 = writer.BaseStream.Position;
					long num7 = position18 - position17;
					writer.BaseStream.Position = position16;
					writer.Write((int)num7);
					writer.Write(num6);
					writer.BaseStream.Position = position18;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case 22:
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
					else if (Helper.IsValueType(typeInfo8.info))
					{
						SerializationTemplate serializationTemplate5 = Manager.GetSerializationTemplate(typeInfo8.type);
						IEnumerator enumerator7 = collection2.GetEnumerator();
						try
						{
							while (enumerator7.MoveNext())
							{
								object obj7 = enumerator7.Current;
								serializationTemplate5.SerializeData(obj7, writer);
							}
						}
						finally
						{
							IDisposable disposable7;
							if ((disposable7 = enumerator7 as IDisposable) != null)
							{
								disposable7.Dispose();
							}
						}
					}
					else
					{
						IEnumerator enumerator8 = collection2.GetEnumerator();
						try
						{
							while (enumerator8.MoveNext())
							{
								object obj8 = enumerator8.Current;
								writer.WriteValue(typeInfo8, obj8);
							}
						}
						finally
						{
							IDisposable disposable8;
							if ((disposable8 = enumerator8 as IDisposable) != null)
							{
								disposable8.Dispose();
							}
						}
					}
					long position21 = writer.BaseStream.Position;
					long num8 = position21 - position20;
					writer.BaseStream.Position = position19;
					writer.Write((int)num8);
					writer.BaseStream.Position = position21;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case 23:
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

		private static void WriteListPOD(BinaryWriter writer, TypeInfo elem_type_info, ICollection collection)
		{
			switch (elem_type_info.info)
			{
			case SerializationTypeInfo.SByte:
			{
				IEnumerator enumerator = collection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						writer.Write((sbyte)obj);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = enumerator as IDisposable) != null)
					{
						disposable.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.Byte:
			{
				IEnumerator enumerator2 = collection.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj2 = enumerator2.Current;
						writer.Write((byte)obj2);
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = enumerator2 as IDisposable) != null)
					{
						disposable2.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.Int16:
			{
				IEnumerator enumerator3 = collection.GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						object obj3 = enumerator3.Current;
						writer.Write((short)obj3);
					}
				}
				finally
				{
					IDisposable disposable3;
					if ((disposable3 = enumerator3 as IDisposable) != null)
					{
						disposable3.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.UInt16:
			{
				IEnumerator enumerator4 = collection.GetEnumerator();
				try
				{
					while (enumerator4.MoveNext())
					{
						object obj4 = enumerator4.Current;
						writer.Write((ushort)obj4);
					}
				}
				finally
				{
					IDisposable disposable4;
					if ((disposable4 = enumerator4 as IDisposable) != null)
					{
						disposable4.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.Int32:
			{
				IEnumerator enumerator5 = collection.GetEnumerator();
				try
				{
					while (enumerator5.MoveNext())
					{
						object obj5 = enumerator5.Current;
						writer.Write((int)obj5);
					}
				}
				finally
				{
					IDisposable disposable5;
					if ((disposable5 = enumerator5 as IDisposable) != null)
					{
						disposable5.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.UInt32:
			{
				IEnumerator enumerator6 = collection.GetEnumerator();
				try
				{
					while (enumerator6.MoveNext())
					{
						object obj6 = enumerator6.Current;
						writer.Write((uint)obj6);
					}
				}
				finally
				{
					IDisposable disposable6;
					if ((disposable6 = enumerator6 as IDisposable) != null)
					{
						disposable6.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.Int64:
			{
				IEnumerator enumerator7 = collection.GetEnumerator();
				try
				{
					while (enumerator7.MoveNext())
					{
						object obj7 = enumerator7.Current;
						writer.Write((long)obj7);
					}
				}
				finally
				{
					IDisposable disposable7;
					if ((disposable7 = enumerator7 as IDisposable) != null)
					{
						disposable7.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.UInt64:
			{
				IEnumerator enumerator8 = collection.GetEnumerator();
				try
				{
					while (enumerator8.MoveNext())
					{
						object obj8 = enumerator8.Current;
						writer.Write((ulong)obj8);
					}
				}
				finally
				{
					IDisposable disposable8;
					if ((disposable8 = enumerator8 as IDisposable) != null)
					{
						disposable8.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.Single:
			{
				IEnumerator enumerator9 = collection.GetEnumerator();
				try
				{
					while (enumerator9.MoveNext())
					{
						object obj9 = enumerator9.Current;
						writer.Write((float)obj9);
					}
				}
				finally
				{
					IDisposable disposable9;
					if ((disposable9 = enumerator9 as IDisposable) != null)
					{
						disposable9.Dispose();
					}
				}
				break;
			}
			case SerializationTypeInfo.Double:
			{
				IEnumerator enumerator10 = collection.GetEnumerator();
				try
				{
					while (enumerator10.MoveNext())
					{
						object obj10 = enumerator10.Current;
						writer.Write((double)obj10);
					}
				}
				finally
				{
					IDisposable disposable10;
					if ((disposable10 = enumerator10 as IDisposable) != null)
					{
						disposable10.Dispose();
					}
				}
				break;
			}
			}
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
