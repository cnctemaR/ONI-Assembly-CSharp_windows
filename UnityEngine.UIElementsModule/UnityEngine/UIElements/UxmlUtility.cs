using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class UxmlUtility
	{
		public static List<string> ParseStringListAttribute(string itemList)
		{
			bool flag = string.IsNullOrEmpty((itemList != null) ? itemList.Trim() : null);
			List<string> list;
			if (flag)
			{
				list = null;
			}
			else
			{
				string[] array = itemList.Split(',', StringSplitOptions.None);
				bool flag2 = array.Length != 0;
				if (flag2)
				{
					List<string> list2 = new List<string>();
					foreach (string text in array)
					{
						list2.Add(text.Trim());
					}
					list = list2;
				}
				else
				{
					list = null;
				}
			}
			return list;
		}

		public static string EncodeListItem(string item)
		{
			return (item == null) ? string.Empty : item.Replace(",", "%2C");
		}

		public static string DecodeListItem(string item)
		{
			return item.Replace("%2C", ",");
		}

		public static void MoveListItem(IList list, int src, int dst)
		{
			object obj = list[src];
			list.RemoveAt(src);
			list.Insert(dst, obj);
		}

		public static float ParseFloat(string value, float defaultValue = 0f)
		{
			float num;
			return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static byte ParseByte(string value, byte defaultValue = 0)
		{
			byte b;
			return byte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out b) ? b : defaultValue;
		}

		public static sbyte ParseSByte(string value, sbyte defaultValue = 0)
		{
			sbyte b;
			return sbyte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out b) ? b : defaultValue;
		}

		public static short ParseShort(string value, short defaultValue = 0)
		{
			short num;
			return short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static ushort ParseUShort(string value, ushort defaultValue = 0)
		{
			ushort num;
			return ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static int ParseInt(string value, int defaultValue = 0)
		{
			int num;
			return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static uint ParseUint(string value, uint defaultValue = 0U)
		{
			uint num;
			return uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static Angle ParseAngle(string value, Angle defaultValue = default(Angle))
		{
			Angle angle;
			return Angle.TryParseString(value, out angle) ? angle : defaultValue;
		}

		public static float TryParseFloatAttribute(string attributeName, IUxmlAttributes bag, ref int foundAttributeCounter)
		{
			string text;
			bool flag = bag.TryGetAttributeValue(attributeName, out text);
			float num;
			if (flag)
			{
				foundAttributeCounter++;
				num = UxmlUtility.ParseFloat(text, 0f);
			}
			else
			{
				num = 0f;
			}
			return num;
		}

		public static int TryParseIntAttribute(string attributeName, IUxmlAttributes bag, ref int foundAttributeCounter)
		{
			string text;
			bool flag = bag.TryGetAttributeValue(attributeName, out text);
			int num;
			if (flag)
			{
				foundAttributeCounter++;
				num = UxmlUtility.ParseInt(text, 0);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static Type ParseType(string value, Type defaultType = null)
		{
			try
			{
				bool flag = !string.IsNullOrEmpty(value);
				if (flag)
				{
					return Type.GetType(value, true);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return defaultType;
		}

		public static string ValidateUxmlName(string name)
		{
			bool flag = !char.IsLetter(name[0]) && name[0] != '_';
			string text;
			if (flag)
			{
				text = "Element names must start with a letter or underscore";
			}
			else
			{
				bool flag2 = name.StartsWith("xml", StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					text = "Element names cannot start with the letters xml (or XML, or Xml, etc)";
				}
				else
				{
					for (int i = 1; i < name.Length; i++)
					{
						char c = name[i];
						bool flag3 = char.IsWhiteSpace(c) || (!char.IsLetterOrDigit(c) && c != '-' && c != '_' && c != '.');
						if (flag3)
						{
							return string.Format("The character '{0}' is invalid. Element names can contain letters, digits, hyphens, underscores, and periods.", c);
						}
					}
					text = null;
				}
			}
			return text;
		}

		public static string TypeToString(Type value)
		{
			bool flag = value == null;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				text = value.FullName + ", " + value.Assembly.GetName().Name;
			}
			return text;
		}

		public static string ValueToString(Bounds value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3},{4},{5}", new object[]
			{
				value.center.x,
				value.center.y,
				value.center.z,
				value.size.x,
				value.size.y,
				value.size.z
			}));
		}

		public static string ValueToString(BoundsInt value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3},{4},{5}", new object[]
			{
				value.position.x,
				value.position.y,
				value.position.z,
				value.size.x,
				value.size.y,
				value.size.z
			}));
		}

		public static string ValueToString(Rect value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3}", new object[] { value.x, value.y, value.width, value.height }));
		}

		public static string ValueToString(RectInt value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3}", new object[] { value.x, value.y, value.width, value.height }));
		}

		public static string ValueToString(Vector2 value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1}", new object[] { value.x, value.y }));
		}

		public static string ValueToString(Vector2Int value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1}", new object[] { value.x, value.y }));
		}

		public static string ValueToString(Vector3 value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2}", new object[] { value.x, value.y, value.z }));
		}

		public static string ValueToString(Vector3Int value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2}", new object[] { value.x, value.y, value.z }));
		}

		public static string ValueToString(Vector4 value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3}", new object[] { value.x, value.y, value.z, value.w }));
		}

		public static object CloneObject(object value)
		{
			bool flag = value != null && !(value is string) && !(value is Type) && value.GetType().IsClass;
			object obj;
			if (flag)
			{
				obj = UxmlSerializedDataUtility.CopySerialized(value);
			}
			else
			{
				obj = value;
			}
			return obj;
		}

		public unsafe static int SplitValues(ReadOnlySpan<char> spanStr, Span<float> values, char separator)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i <= spanStr.Length; i++)
			{
				bool flag = i == spanStr.Length || *spanStr[i] == (ushort)separator;
				if (flag)
				{
					bool flag2 = num2 < i && num < values.Length;
					if (flag2)
					{
						float num3;
						bool flag3 = float.TryParse(spanStr.Slice(num2, i - num2), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out num3);
						if (flag3)
						{
							*values[num++] = num3;
						}
					}
					num2 = i + 1;
				}
			}
			return num;
		}

		private const string s_CommaEncoded = "%2C";
	}
}
