using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Math/ColorUtility.h")]
	public class ColorUtility
	{
		[FreeFunction("TryParseHtmlColor", true)]
		internal unsafe static bool DoTryParseHtmlColor(string htmlString, out Color32 color)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(htmlString, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = htmlString.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = ColorUtility.DoTryParseHtmlColor_Injected(ref managedSpanWrapper, out color);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		private static ReadOnlySpan<Color32> HtmlColorValues
		{
			get
			{
				return new Color32[]
				{
					new Color32(byte.MaxValue, 0, 0, byte.MaxValue),
					new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue),
					new Color32(0, 0, byte.MaxValue, byte.MaxValue),
					new Color32(0, 0, 139, byte.MaxValue),
					new Color32(173, 216, 230, byte.MaxValue),
					new Color32(128, 0, 128, byte.MaxValue),
					new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue),
					new Color32(0, byte.MaxValue, 0, byte.MaxValue),
					new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue),
					new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
					new Color32(192, 192, 192, byte.MaxValue),
					new Color32(128, 128, 128, byte.MaxValue),
					new Color32(0, 0, 0, byte.MaxValue),
					new Color32(byte.MaxValue, 165, 0, byte.MaxValue),
					new Color32(165, 42, 42, byte.MaxValue),
					new Color32(128, 0, 0, byte.MaxValue),
					new Color32(0, 128, 0, byte.MaxValue),
					new Color32(128, 128, 0, byte.MaxValue),
					new Color32(0, 0, 128, byte.MaxValue),
					new Color32(0, 128, 128, byte.MaxValue),
					new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue),
					new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue),
					new Color32(0, 0, 0, 0)
				};
			}
		}

		private static ReadOnlySpan<string> HtmlColorNames
		{
			get
			{
				return new string[]
				{
					"red", "cyan", "blue", "darkblue", "lightblue", "purple", "yellow", "lime", "fuchsia", "white",
					"silver", "grey", "black", "orange", "brown", "maroon", "green", "olive", "navy", "teal",
					"aqua", "magenta", "transparent"
				};
			}
		}

		public static bool TryParseHtmlString(string htmlString, out Color color)
		{
			Color32 color2;
			bool flag = ColorUtility.DoTryParseHtmlColor(htmlString, out color2);
			color = color2;
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToHtmlStringRGB(Color color)
		{
			return ColorUtility.ToHtmlStringRGB(in color);
		}

		public static string ToHtmlStringRGB(in Color color)
		{
			Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), 1);
			return string.Format("{0:X2}{1:X2}{2:X2}", color2.r, color2.g, color2.b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToHtmlStringRGBA(Color color)
		{
			return ColorUtility.ToHtmlStringRGBA(in color);
		}

		public static string ToHtmlStringRGBA(in Color color)
		{
			Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255));
			return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { color2.r, color2.g, color2.b, color2.a });
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		internal unsafe static bool TryParseHtmlString(ReadOnlySpan<char> input, out Color color)
		{
			color = Color.white;
			input = input.Trim();
			bool flag = input.Length == 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = *input[0] == 35;
				if (flag3)
				{
					bool flag4 = input.Length > 9;
					if (flag4)
					{
						return false;
					}
					bool flag5 = !ColorUtility.IsHexString(input.Slice(1));
					if (flag5)
					{
						return false;
					}
					bool flag6 = input.Length == 4 || input.Length == 5;
					if (flag6)
					{
						int num = (input.Length - 1) * 2;
						Span<char> span2;
						int i;
						int num2;
						checked
						{
							Span<char> span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num) * 2], num);
							span2 = span;
							i = 1;
							num2 = 0;
						}
						while (i < input.Length)
						{
							*span2[num2++] = (char)(*input[i]);
							*span2[num2++] = (char)(*input[i]);
							i++;
						}
						return ColorUtility.TryParseHexColor(span2, out color);
					}
					bool flag7 = input.Length == 7 || input.Length == 9;
					if (flag7)
					{
						return ColorUtility.TryParseHexColor(input.Slice(1), out color);
					}
				}
				else
				{
					for (int j = 0; j < ColorUtility.HtmlColorNames.Length; j++)
					{
						bool flag8 = input.Equals(*ColorUtility.HtmlColorNames[j], StringComparison.OrdinalIgnoreCase);
						if (flag8)
						{
							color = *ColorUtility.HtmlColorValues[j];
							return true;
						}
					}
				}
				flag2 = false;
			}
			return flag2;
		}

		private unsafe static bool IsHexString(ReadOnlySpan<char> span)
		{
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = (char)(*readOnlySpan[i]);
				bool flag = !Uri.IsHexDigit(c);
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		private static bool TryParseHexColor(ReadOnlySpan<char> hex, out Color color)
		{
			color = Color.white;
			bool flag = hex.Length != 6 && hex.Length != 8;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				byte b;
				bool flag3 = !ColorUtility.TryHexToByte(hex.Slice(0, 2), out b);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					byte b2;
					bool flag4 = !ColorUtility.TryHexToByte(hex.Slice(2, 2), out b2);
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						byte b3;
						bool flag5 = !ColorUtility.TryHexToByte(hex.Slice(4, 2), out b3);
						if (flag5)
						{
							flag2 = false;
						}
						else
						{
							byte maxValue = byte.MaxValue;
							bool flag6 = hex.Length == 8;
							if (flag6)
							{
								bool flag7 = !ColorUtility.TryHexToByte(hex.Slice(6, 2), out maxValue);
								if (flag7)
								{
									return false;
								}
							}
							color = new Color32(b, b2, b3, maxValue);
							flag2 = true;
						}
					}
				}
			}
			return flag2;
		}

		private unsafe static bool TryHexToByte(ReadOnlySpan<char> span, out byte result)
		{
			result = 0;
			bool flag = span.Length != 2;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num = ColorUtility.HexDigitValue((char)(*span[0]));
				int num2 = ColorUtility.HexDigitValue((char)(*span[1]));
				bool flag3 = num == -1 || num2 == -1;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					result = (byte)((num << 4) | num2);
					flag2 = true;
				}
			}
			return flag2;
		}

		private static int HexDigitValue(char c)
		{
			bool flag = c >= '0' && c <= '9';
			int num;
			if (flag)
			{
				num = (int)(c - '0');
			}
			else
			{
				bool flag2 = c >= 'a' && c <= 'f';
				if (flag2)
				{
					num = (int)(c - 'a' + '\n');
				}
				else
				{
					bool flag3 = c >= 'A' && c <= 'F';
					if (flag3)
					{
						num = (int)(c - 'A' + '\n');
					}
					else
					{
						num = -1;
					}
				}
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoTryParseHtmlColor_Injected(ref ManagedSpanWrapper htmlString, out Color32 color);
	}
}
