using System;
using System.ComponentModel;

namespace Ionic
{
	internal sealed class EnumUtil
	{
		private EnumUtil()
		{
		}

		internal static string GetDescription(Enum value)
		{
			DescriptionAttribute[] array = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (array.Length != 0)
			{
				return array[0].Description;
			}
			return value.ToString();
		}

		internal static object Parse(Type enumType, string stringRepresentation)
		{
			return EnumUtil.Parse(enumType, stringRepresentation, false);
		}

		internal static object Parse(Type enumType, string stringRepresentation, bool ignoreCase)
		{
			if (ignoreCase)
			{
				stringRepresentation = stringRepresentation.ToLower();
			}
			foreach (object obj in Enum.GetValues(enumType))
			{
				Enum @enum = (Enum)obj;
				string text = EnumUtil.GetDescription(@enum);
				if (ignoreCase)
				{
					text = text.ToLower();
				}
				if (text == stringRepresentation)
				{
					return @enum;
				}
			}
			return Enum.Parse(enumType, stringRepresentation, ignoreCase);
		}
	}
}
