using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using SimpleJson.Reflection;

namespace SimpleJson
{
	[GeneratedCode("simple-json", "1.0.0")]
	internal static class SimpleJson
	{
		public static object DeserializeObject(string json)
		{
			object obj;
			if (SimpleJson.TryDeserializeObject(json, out obj))
			{
				return obj;
			}
			throw new SerializationException("Invalid JSON string");
		}

		public static bool TryDeserializeObject(string json, out object obj)
		{
			bool flag = true;
			if (json != null)
			{
				char[] array = json.ToCharArray();
				int num = 0;
				obj = SimpleJson.ParseValue(array, ref num, ref flag);
			}
			else
			{
				obj = null;
			}
			return flag;
		}

		public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			object obj = SimpleJson.DeserializeObject(json);
			return (type != null && (obj == null || !ReflectionUtils.IsAssignableFrom(obj.GetType(), type))) ? (jsonSerializerStrategy ?? SimpleJson.CurrentJsonSerializerStrategy).DeserializeObject(obj, type) : obj;
		}

		public static object DeserializeObject(string json, Type type)
		{
			return SimpleJson.DeserializeObject(json, type, null);
		}

		public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			return (T)((object)SimpleJson.DeserializeObject(json, typeof(T), jsonSerializerStrategy));
		}

		public static T DeserializeObject<T>(string json)
		{
			return (T)((object)SimpleJson.DeserializeObject(json, typeof(T), null));
		}

		public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			bool flag = SimpleJson.SerializeValue(jsonSerializerStrategy, json, stringBuilder);
			return (!flag) ? null : stringBuilder.ToString();
		}

		public static string SerializeObject(object json)
		{
			return SimpleJson.SerializeObject(json, SimpleJson.CurrentJsonSerializerStrategy);
		}

		public static string EscapeToJavascriptString(string jsonString)
		{
			string text;
			if (string.IsNullOrEmpty(jsonString))
			{
				text = jsonString;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				int i = 0;
				while (i < jsonString.Length)
				{
					char c = jsonString[i++];
					if (c == '\\')
					{
						int num = jsonString.Length - i;
						if (num >= 2)
						{
							char c2 = jsonString[i];
							if (c2 == '\\')
							{
								stringBuilder.Append('\\');
								i++;
							}
							else if (c2 == '"')
							{
								stringBuilder.Append("\"");
								i++;
							}
							else if (c2 == 't')
							{
								stringBuilder.Append('\t');
								i++;
							}
							else if (c2 == 'b')
							{
								stringBuilder.Append('\b');
								i++;
							}
							else if (c2 == 'n')
							{
								stringBuilder.Append('\n');
								i++;
							}
							else if (c2 == 'r')
							{
								stringBuilder.Append('\r');
								i++;
							}
						}
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				text = stringBuilder.ToString();
			}
			return text;
		}

		private static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
		{
			IDictionary<string, object> dictionary = new JsonObject();
			SimpleJson.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = SimpleJson.LookAhead(json, index);
				if (num != 0)
				{
					if (num == 6)
					{
						SimpleJson.NextToken(json, ref index);
					}
					else
					{
						if (num == 2)
						{
							SimpleJson.NextToken(json, ref index);
							return dictionary;
						}
						string text = SimpleJson.ParseString(json, ref index, ref success);
						if (!success)
						{
							success = false;
							return null;
						}
						num = SimpleJson.NextToken(json, ref index);
						if (num != 5)
						{
							success = false;
							return null;
						}
						object obj = SimpleJson.ParseValue(json, ref index, ref success);
						if (!success)
						{
							success = false;
							return null;
						}
						dictionary[text] = obj;
					}
					continue;
				}
				success = false;
				return null;
			}
			return dictionary;
		}

		private static JsonArray ParseArray(char[] json, ref int index, ref bool success)
		{
			JsonArray jsonArray = new JsonArray();
			SimpleJson.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = SimpleJson.LookAhead(json, index);
				if (num != 0)
				{
					if (num == 6)
					{
						SimpleJson.NextToken(json, ref index);
					}
					else
					{
						if (num == 4)
						{
							SimpleJson.NextToken(json, ref index);
							break;
						}
						object obj = SimpleJson.ParseValue(json, ref index, ref success);
						if (!success)
						{
							return null;
						}
						jsonArray.Add(obj);
					}
					continue;
				}
				success = false;
				return null;
			}
			return jsonArray;
		}

		private static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (SimpleJson.LookAhead(json, index))
			{
			case 1:
				return SimpleJson.ParseObject(json, ref index, ref success);
			case 3:
				return SimpleJson.ParseArray(json, ref index, ref success);
			case 7:
				return SimpleJson.ParseString(json, ref index, ref success);
			case 8:
				return SimpleJson.ParseNumber(json, ref index, ref success);
			case 9:
				SimpleJson.NextToken(json, ref index);
				return true;
			case 10:
				SimpleJson.NextToken(json, ref index);
				return false;
			case 11:
				SimpleJson.NextToken(json, ref index);
				return null;
			}
			success = false;
			return null;
		}

		private static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			SimpleJson.EatWhitespace(json, ref index);
			char c = json[index++];
			bool flag = false;
			while (!flag)
			{
				if (index == json.Length)
				{
					break;
				}
				c = json[index++];
				if (c == '"')
				{
					flag = true;
					break;
				}
				if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}
					c = json[index++];
					if (c == '"')
					{
						stringBuilder.Append('"');
					}
					else if (c == '\\')
					{
						stringBuilder.Append('\\');
					}
					else if (c == '/')
					{
						stringBuilder.Append('/');
					}
					else if (c == 'b')
					{
						stringBuilder.Append('\b');
					}
					else if (c == 'f')
					{
						stringBuilder.Append('\f');
					}
					else if (c == 'n')
					{
						stringBuilder.Append('\n');
					}
					else if (c == 'r')
					{
						stringBuilder.Append('\r');
					}
					else if (c == 't')
					{
						stringBuilder.Append('\t');
					}
					else if (c == 'u')
					{
						int num = json.Length - index;
						if (num >= 4)
						{
							uint num2;
							string text;
							if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2)))
							{
								text = "";
							}
							else
							{
								if (55296U > num2 || num2 > 56319U)
								{
									stringBuilder.Append(SimpleJson.ConvertFromUtf32((int)num2));
									index += 4;
									continue;
								}
								index += 4;
								num = json.Length - index;
								if (num >= 6)
								{
									uint num3;
									if (new string(json, index, 2) == "\\u" && uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num3))
									{
										if (56320U <= num3 && num3 <= 57343U)
										{
											stringBuilder.Append((char)num2);
											stringBuilder.Append((char)num3);
											index += 6;
											continue;
										}
									}
								}
								success = false;
								text = "";
							}
							return text;
						}
						break;
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (!flag)
			{
				success = false;
				return null;
			}
			return stringBuilder.ToString();
		}

		private static string ConvertFromUtf32(int utf32)
		{
			if (utf32 < 0 || utf32 > 1114111)
			{
				throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
			}
			if (55296 <= utf32 && utf32 <= 57343)
			{
				throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
			}
			string text;
			if (utf32 < 65536)
			{
				text = new string((char)utf32, 1);
			}
			else
			{
				utf32 -= 65536;
				text = new string(new char[]
				{
					(char)((utf32 >> 10) + 55296),
					(char)(utf32 % 1024 + 56320)
				});
			}
			return text;
		}

		private static object ParseNumber(char[] json, ref int index, ref bool success)
		{
			SimpleJson.EatWhitespace(json, ref index);
			int lastIndexOfNumber = SimpleJson.GetLastIndexOfNumber(json, index);
			int num = lastIndexOfNumber - index + 1;
			string text = new string(json, index, num);
			object obj;
			if (text.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || text.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
			{
				double num2;
				success = double.TryParse(new string(json, index, num), NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
				obj = num2;
			}
			else
			{
				long num3;
				success = long.TryParse(new string(json, index, num), NumberStyles.Any, CultureInfo.InvariantCulture, out num3);
				obj = num3;
			}
			index = lastIndexOfNumber + 1;
			return obj;
		}

		private static int GetLastIndexOfNumber(char[] json, int index)
		{
			int i;
			for (i = index; i < json.Length; i++)
			{
				if ("0123456789+-.eE".IndexOf(json[i]) == -1)
				{
					break;
				}
			}
			return i - 1;
		}

		private static void EatWhitespace(char[] json, ref int index)
		{
			while (index < json.Length)
			{
				if (" \t\n\r\b\f".IndexOf(json[index]) == -1)
				{
					break;
				}
				index++;
			}
		}

		private static int LookAhead(char[] json, int index)
		{
			int num = index;
			return SimpleJson.NextToken(json, ref num);
		}

		private static int NextToken(char[] json, ref int index)
		{
			SimpleJson.EatWhitespace(json, ref index);
			int num;
			if (index == json.Length)
			{
				num = 0;
			}
			else
			{
				char c = json[index];
				index++;
				switch (c)
				{
				case ',':
					num = 6;
					break;
				case '-':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					num = 8;
					break;
				default:
					switch (c)
					{
					case '[':
						num = 3;
						break;
					default:
						switch (c)
						{
						case '{':
							num = 1;
							break;
						default:
							if (c != '"')
							{
								index--;
								int num2 = json.Length - index;
								if (num2 >= 5)
								{
									if (json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
									{
										index += 5;
										num = 10;
										break;
									}
								}
								if (num2 >= 4)
								{
									if (json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
									{
										index += 4;
										num = 9;
										break;
									}
								}
								if (num2 >= 4)
								{
									if (json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
									{
										index += 4;
										num = 11;
										break;
									}
								}
								num = 0;
							}
							else
							{
								num = 7;
							}
							break;
						case '}':
							num = 2;
							break;
						}
						break;
					case ']':
						num = 4;
						break;
					}
					break;
				case ':':
					num = 5;
					break;
				}
			}
			return num;
		}

		private static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder)
		{
			bool flag = true;
			string text = value as string;
			if (text != null)
			{
				flag = SimpleJson.SerializeString(text, builder);
			}
			else
			{
				IDictionary<string, object> dictionary = value as IDictionary<string, object>;
				if (dictionary != null)
				{
					flag = SimpleJson.SerializeObject(jsonSerializerStrategy, dictionary.Keys, dictionary.Values, builder);
				}
				else
				{
					IDictionary<string, string> dictionary2 = value as IDictionary<string, string>;
					if (dictionary2 != null)
					{
						flag = SimpleJson.SerializeObject(jsonSerializerStrategy, dictionary2.Keys, dictionary2.Values, builder);
					}
					else
					{
						IEnumerable enumerable = value as IEnumerable;
						if (enumerable != null)
						{
							flag = SimpleJson.SerializeArray(jsonSerializerStrategy, enumerable, builder);
						}
						else if (SimpleJson.IsNumeric(value))
						{
							flag = SimpleJson.SerializeNumber(value, builder);
						}
						else if (value is bool)
						{
							builder.Append((!(bool)value) ? "false" : "true");
						}
						else if (value == null)
						{
							builder.Append("null");
						}
						else
						{
							object obj;
							flag = jsonSerializerStrategy.TrySerializeNonPrimitiveObject(value, out obj);
							if (flag)
							{
								SimpleJson.SerializeValue(jsonSerializerStrategy, obj, builder);
							}
						}
					}
				}
			}
			return flag;
		}

		private static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder)
		{
			builder.Append("{");
			IEnumerator enumerator = keys.GetEnumerator();
			IEnumerator enumerator2 = values.GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				object obj = enumerator.Current;
				object obj2 = enumerator2.Current;
				if (!flag)
				{
					builder.Append(",");
				}
				string text = obj as string;
				if (text != null)
				{
					SimpleJson.SerializeString(text, builder);
				}
				else if (!SimpleJson.SerializeValue(jsonSerializerStrategy, obj2, builder))
				{
					return false;
				}
				builder.Append(":");
				if (SimpleJson.SerializeValue(jsonSerializerStrategy, obj2, builder))
				{
					flag = false;
					continue;
				}
				return false;
			}
			builder.Append("}");
			return true;
		}

		private static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder)
		{
			builder.Append("[");
			bool flag = true;
			IEnumerator enumerator = anArray.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					if (!flag)
					{
						builder.Append(",");
					}
					if (!SimpleJson.SerializeValue(jsonSerializerStrategy, obj, builder))
					{
						return false;
					}
					flag = false;
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
			builder.Append("]");
			return true;
		}

		private static bool SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");
			foreach (char c in aString.ToCharArray())
			{
				if (c == '"')
				{
					builder.Append("\\\"");
				}
				else if (c == '\\')
				{
					builder.Append("\\\\");
				}
				else if (c == '\b')
				{
					builder.Append("\\b");
				}
				else if (c == '\f')
				{
					builder.Append("\\f");
				}
				else if (c == '\n')
				{
					builder.Append("\\n");
				}
				else if (c == '\r')
				{
					builder.Append("\\r");
				}
				else if (c == '\t')
				{
					builder.Append("\\t");
				}
				else
				{
					builder.Append(c);
				}
			}
			builder.Append("\"");
			return true;
		}

		private static bool SerializeNumber(object number, StringBuilder builder)
		{
			if (number is long)
			{
				builder.Append(((long)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is ulong)
			{
				builder.Append(((ulong)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is int)
			{
				builder.Append(((int)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is uint)
			{
				builder.Append(((uint)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is decimal)
			{
				builder.Append(((decimal)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is float)
			{
				builder.Append(((float)number).ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
			}
			return true;
		}

		private static bool IsNumeric(object value)
		{
			return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
		}

		public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
		{
			get
			{
				IJsonSerializerStrategy jsonSerializerStrategy;
				if ((jsonSerializerStrategy = SimpleJson._currentJsonSerializerStrategy) == null)
				{
					jsonSerializerStrategy = (SimpleJson._currentJsonSerializerStrategy = SimpleJson.PocoJsonSerializerStrategy);
				}
				return jsonSerializerStrategy;
			}
			set
			{
				SimpleJson._currentJsonSerializerStrategy = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PocoJsonSerializerStrategy PocoJsonSerializerStrategy
		{
			get
			{
				PocoJsonSerializerStrategy pocoJsonSerializerStrategy;
				if ((pocoJsonSerializerStrategy = SimpleJson._pocoJsonSerializerStrategy) == null)
				{
					pocoJsonSerializerStrategy = (SimpleJson._pocoJsonSerializerStrategy = new PocoJsonSerializerStrategy());
				}
				return pocoJsonSerializerStrategy;
			}
		}

		private const int TOKEN_NONE = 0;

		private const int TOKEN_CURLY_OPEN = 1;

		private const int TOKEN_CURLY_CLOSE = 2;

		private const int TOKEN_SQUARED_OPEN = 3;

		private const int TOKEN_SQUARED_CLOSE = 4;

		private const int TOKEN_COLON = 5;

		private const int TOKEN_COMMA = 6;

		private const int TOKEN_STRING = 7;

		private const int TOKEN_NUMBER = 8;

		private const int TOKEN_TRUE = 9;

		private const int TOKEN_FALSE = 10;

		private const int TOKEN_NULL = 11;

		private const int BUILDER_CAPACITY = 2000;

		private static IJsonSerializerStrategy _currentJsonSerializerStrategy;

		private static PocoJsonSerializerStrategy _pocoJsonSerializerStrategy;
	}
}
