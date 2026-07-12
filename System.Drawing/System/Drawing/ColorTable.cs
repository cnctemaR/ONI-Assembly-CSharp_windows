using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Drawing
{
	internal static class ColorTable
	{
		private static Dictionary<string, Color> GetColors()
		{
			Dictionary<string, Color> dictionary = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
			ColorTable.FillConstants(dictionary, typeof(Color));
			ColorTable.FillConstants(dictionary, typeof(SystemColors));
			return dictionary;
		}

		internal static Dictionary<string, Color> Colors
		{
			get
			{
				return ColorTable.s_colorConstants.Value;
			}
		}

		private static void FillConstants(Dictionary<string, Color> colors, Type enumType)
		{
			foreach (PropertyInfo propertyInfo in enumType.GetProperties())
			{
				if (propertyInfo.PropertyType == typeof(Color))
				{
					colors[propertyInfo.Name] = (Color)propertyInfo.GetValue(null, null);
				}
			}
		}

		internal static bool TryGetNamedColor(string name, out Color result)
		{
			return ColorTable.Colors.TryGetValue(name, out result);
		}

		internal static bool IsKnownNamedColor(string name)
		{
			Color color;
			return ColorTable.Colors.TryGetValue(name, out color);
		}

		private static readonly Lazy<Dictionary<string, Color>> s_colorConstants = new Lazy<Dictionary<string, Color>>(new Func<Dictionary<string, Color>>(ColorTable.GetColors));
	}
}
