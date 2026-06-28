using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Drawing
{
	public class ColorConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		internal static Color StaticConvertFromString(ITypeDescriptorContext context, string s, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = CultureInfo.InvariantCulture;
			}
			s = s.Trim();
			if (s.Length == 0)
			{
				return Color.Empty;
			}
			if (char.IsLetter(s[0]))
			{
				KnownColor knownColor;
				try
				{
					knownColor = (KnownColor)((int)Enum.Parse(typeof(KnownColor), s, true));
				}
				catch (Exception ex)
				{
					string text = Locale.GetText("Invalid color name '{0}'.", new object[] { s });
					throw new Exception(text, new FormatException(text, ex));
				}
				return KnownColors.FromKnownColor(knownColor);
			}
			string listSeparator = culture.TextInfo.ListSeparator;
			Color color = Color.Empty;
			if (s.IndexOf(listSeparator) == -1)
			{
				bool flag = s[0] == '#';
				int num = ((!flag) ? 0 : 1);
				bool flag2 = false;
				if (s.Length > num + 1 && s[num] == '0')
				{
					flag2 = s[num + 1] == 'x' || s[num + 1] == 'X';
					if (flag2)
					{
						num += 2;
					}
				}
				if (flag || flag2)
				{
					s = s.Substring(num);
					int num2;
					try
					{
						num2 = int.Parse(s, NumberStyles.HexNumber);
					}
					catch (Exception ex2)
					{
						string text2 = Locale.GetText("Invalid Int32 value '{0}'.", new object[] { s });
						throw new Exception(text2, ex2);
					}
					if (s.Length < 6 || (s.Length == 6 && flag && flag2))
					{
						num2 &= 16777215;
					}
					else if (num2 >> 24 == 0)
					{
						num2 |= -16777216;
					}
					color = Color.FromArgb(num2);
				}
			}
			if (color.IsEmpty)
			{
				Int32Converter int32Converter = new Int32Converter();
				string[] array = s.Split(listSeparator.ToCharArray());
				int[] array2 = new int[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = (int)int32Converter.ConvertFrom(context, culture, array[i]);
				}
				switch (array.Length)
				{
				case 1:
					color = Color.FromArgb(array2[0]);
					goto IL_028B;
				case 3:
					color = Color.FromArgb(array2[0], array2[1], array2[2]);
					goto IL_028B;
				case 4:
					color = Color.FromArgb(array2[0], array2[1], array2[2], array2[3]);
					goto IL_028B;
				}
				throw new ArgumentException(s + " is not a valid color value.");
			}
			IL_028B:
			if (!color.IsEmpty)
			{
				Color color2 = KnownColors.FindColorMatch(color);
				if (!color2.IsEmpty)
				{
					return color2;
				}
			}
			return color;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			return ColorConverter.StaticConvertFromString(context, text, culture);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is Color)
			{
				Color color = (Color)value;
				if (destinationType == typeof(string))
				{
					if (color == Color.Empty)
					{
						return string.Empty;
					}
					if (color.IsKnownColor || color.IsNamedColor)
					{
						return color.Name;
					}
					string listSeparator = culture.TextInfo.ListSeparator;
					StringBuilder stringBuilder = new StringBuilder();
					if (color.A != 255)
					{
						stringBuilder.Append(color.A);
						stringBuilder.Append(listSeparator);
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(color.R);
					stringBuilder.Append(listSeparator);
					stringBuilder.Append(" ");
					stringBuilder.Append(color.G);
					stringBuilder.Append(listSeparator);
					stringBuilder.Append(" ");
					stringBuilder.Append(color.B);
					return stringBuilder.ToString();
				}
				else if (destinationType == typeof(InstanceDescriptor))
				{
					if (color.IsEmpty)
					{
						return new InstanceDescriptor(typeof(Color).GetField("Empty"), null);
					}
					if (color.IsSystemColor)
					{
						return new InstanceDescriptor(typeof(SystemColors).GetProperty(color.Name), null);
					}
					if (color.IsKnownColor)
					{
						return new InstanceDescriptor(typeof(Color).GetProperty(color.Name), null);
					}
					MethodInfo method = typeof(Color).GetMethod("FromArgb", new Type[]
					{
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(int)
					});
					return new InstanceDescriptor(method, new object[] { color.A, color.R, color.G, color.B });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			object obj = ColorConverter.creatingCached;
			lock (obj)
			{
				if (ColorConverter.cached != null)
				{
					return ColorConverter.cached;
				}
				Array array = Array.CreateInstance(typeof(Color), KnownColors.ArgbValues.Length - 1);
				for (int i = 1; i < KnownColors.ArgbValues.Length; i++)
				{
					array.SetValue(KnownColors.FromKnownColor((KnownColor)i), i - 1);
				}
				Array.Sort(array, 0, array.Length, new ColorConverter.CompareColors());
				ColorConverter.cached = new TypeConverter.StandardValuesCollection(array);
			}
			return ColorConverter.cached;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private static TypeConverter.StandardValuesCollection cached;

		private static object creatingCached = new object();

		private sealed class CompareColors : IComparer
		{
			public int Compare(object x, object y)
			{
				return string.Compare(((Color)x).Name, ((Color)y).Name);
			}
		}
	}
}
