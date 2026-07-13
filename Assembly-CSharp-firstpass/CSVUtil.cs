using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public static class CSVUtil
{
	public static bool IsValidColumn(string[,] grid, int col)
	{
		return grid[col, 0] != null && grid[col, 0] != "";
	}

	public static void ParseData<T>(object def, string[,] grid, int row)
	{
		int length = grid.GetLength(0);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < length; i++)
		{
			if (CSVUtil.IsValidColumn(grid, i))
			{
				try
				{
					string text = grid[i, 0];
					FieldInfo field = typeFromHandle.GetField(text);
					if (field != null)
					{
						string text2 = grid[i, row];
						if (text2 != null)
						{
							CSVUtil.ParseValue(field, text2, def, grid[0, row]);
						}
					}
				}
				catch
				{
				}
			}
		}
	}

	private static void ParseValue(FieldInfo field, string val, object target, string row_name)
	{
		if (field.FieldType.IsEnum)
		{
			object obj = null;
			if (val != null && val != "" && CSVUtil.EnumTryParse(field.FieldType, val, out obj))
			{
				field.SetValue(target, obj);
				return;
			}
		}
		else
		{
			if (field.FieldType == typeof(string))
			{
				field.SetValue(target, val);
				return;
			}
			if (field.FieldType == typeof(bool))
			{
				if (val.Contains("1"))
				{
					field.SetValue(target, true);
					return;
				}
				field.SetValue(target, val.ToLower() == "true");
				return;
			}
			else
			{
				if (field.FieldType == typeof(float))
				{
					field.SetValue(target, (val.Trim() == "") ? 0f : float.Parse(val));
					return;
				}
				if (field.FieldType == typeof(int))
				{
					field.SetValue(target, (val.Trim() == "") ? 0 : int.Parse(val));
					return;
				}
				if (field.FieldType == typeof(byte))
				{
					field.SetValue(target, byte.Parse(val));
					return;
				}
				if (field.FieldType == typeof(Tag))
				{
					field.SetValue(target, new Tag(val));
					return;
				}
				if (field.FieldType == typeof(CellOffset))
				{
					if (val == null || val == "")
					{
						field.SetValue(target, default(CellOffset));
						return;
					}
					string[] array = val.Split(',', StringSplitOptions.None);
					field.SetValue(target, new CellOffset(int.Parse(array[0]), int.Parse(array[1])));
					return;
				}
				else if (field.FieldType == typeof(Vector3))
				{
					if (val == null || val == "")
					{
						field.SetValue(target, Vector3.zero);
						return;
					}
					string[] array2 = val.Split(',', StringSplitOptions.None);
					field.SetValue(target, new Vector3(float.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2])));
					return;
				}
				else if (typeof(Array).IsAssignableFrom(field.FieldType))
				{
					string[] array3 = val.Split(CSVUtil._listSeparators);
					Type elementType = field.FieldType.GetElementType();
					Array array4 = Array.CreateInstance(elementType, array3.Length);
					int num = 0;
					for (int i = 0; i < array3.Length; i++)
					{
						if (array3[i].Trim() != "")
						{
							num++;
						}
					}
					array4 = Array.CreateInstance(elementType, num);
					num = 0;
					for (int j = 0; j < array3.Length; j++)
					{
						string text = array3[j].Trim();
						if (text != "")
						{
							object obj2 = Convert.ChangeType(text, elementType);
							array4.SetValue(obj2, num);
							num++;
						}
					}
					field.SetValue(target, array4);
				}
			}
		}
	}

	public static bool EnumTryParse(Type type, string input, out object value)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (!type.IsEnum)
		{
			throw new ArgumentException(null, "type");
		}
		if (input == null)
		{
			value = Activator.CreateInstance(type);
			return false;
		}
		input = input.Trim();
		if (input.Length == 0)
		{
			value = Activator.CreateInstance(type);
			return false;
		}
		string[] names = Enum.GetNames(type);
		if (names.Length == 0)
		{
			value = Activator.CreateInstance(type);
			return false;
		}
		Type underlyingType = Enum.GetUnderlyingType(type);
		Array values = Enum.GetValues(type);
		if (!type.IsDefined(typeof(FlagsAttribute), true) && input.IndexOfAny(CSVUtil._enumSeperators) < 0)
		{
			return CSVUtil.EnumToObject(type, underlyingType, names, values, input, out value);
		}
		string[] array = input.Split(CSVUtil._enumSeperators, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			value = Activator.CreateInstance(type);
			return false;
		}
		ulong num = 0UL;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array2[i].Trim();
			if (text.Length != 0)
			{
				object obj;
				if (!CSVUtil.EnumToObject(type, underlyingType, names, values, text, out obj))
				{
					value = Activator.CreateInstance(type);
					return false;
				}
				ulong num2;
				switch (Convert.GetTypeCode(obj))
				{
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					num2 = (ulong)Convert.ToInt64(obj, CultureInfo.InvariantCulture);
					break;
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
					goto IL_0137;
				default:
					goto IL_0137;
				}
				IL_0145:
				num |= num2;
				goto IL_014C;
				IL_0137:
				num2 = Convert.ToUInt64(obj, CultureInfo.InvariantCulture);
				goto IL_0145;
			}
			IL_014C:;
		}
		value = Enum.ToObject(type, num);
		return true;
	}

	private static object EnumToObject(Type underlyingType, string input)
	{
		int num;
		if (underlyingType == typeof(int) && int.TryParse(input, out num))
		{
			return num;
		}
		uint num2;
		if (underlyingType == typeof(uint) && uint.TryParse(input, out num2))
		{
			return num2;
		}
		ulong num3;
		if (underlyingType == typeof(ulong) && ulong.TryParse(input, out num3))
		{
			return num3;
		}
		long num4;
		if (underlyingType == typeof(long) && long.TryParse(input, out num4))
		{
			return num4;
		}
		short num5;
		if (underlyingType == typeof(short) && short.TryParse(input, out num5))
		{
			return num5;
		}
		ushort num6;
		if (underlyingType == typeof(ushort) && ushort.TryParse(input, out num6))
		{
			return num6;
		}
		byte b;
		if (underlyingType == typeof(byte) && byte.TryParse(input, out b))
		{
			return b;
		}
		sbyte b2;
		if (underlyingType == typeof(sbyte) && sbyte.TryParse(input, out b2))
		{
			return b2;
		}
		return null;
	}

	private static bool EnumToObject(Type type, Type underlyingType, string[] names, Array values, string input, out object value)
	{
		for (int i = 0; i < names.Length; i++)
		{
			if (string.Compare(names[i], input, StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = values.GetValue(i);
				return true;
			}
		}
		if (!char.IsDigit(input[0]) && input[0] != '-' && input[0] != '+')
		{
			value = Activator.CreateInstance(type);
			return false;
		}
		object obj = CSVUtil.EnumToObject(underlyingType, input);
		if (obj == null)
		{
			value = Activator.CreateInstance(type);
			return false;
		}
		value = obj;
		return true;
	}

	public static bool SetValue<T>(T src, ref T dest) where T : IComparable<T>
	{
		bool flag = false;
		if (!src.Equals(dest))
		{
			dest = src;
			flag = true;
		}
		return flag;
	}

	public static bool SetValue<T>(T[] src, ref T[] dest) where T : IComparable<T>, new()
	{
		bool flag = false;
		if (dest == null || src.Length != dest.Length)
		{
			flag = true;
			dest = new T[src.Length];
			Array.Copy(src, dest, src.Length);
		}
		else
		{
			for (int i = 0; i < src.Length; i++)
			{
				if (src[i].CompareTo(dest[i]) != 0)
				{
					flag = true;
					dest[i] = src[i];
				}
			}
		}
		return flag;
	}

	private static char[] _listSeparators = new char[] { ',', ';', '+', '|', '\n' };

	private static char[] _enumSeperators = new char[] { ',', ';', '+', '|', ' ' };
}
