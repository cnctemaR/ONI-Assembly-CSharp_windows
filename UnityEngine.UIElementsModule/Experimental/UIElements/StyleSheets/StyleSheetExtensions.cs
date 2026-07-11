using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleSheetExtensions
	{
		public static void Apply<T>(this StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<T> property, HandlesApplicatorFunction<T> applicatorFunc)
		{
			if (handles[0].valueType == StyleValueType.Keyword && handles[0].valueIndex == 2)
			{
				StyleSheetApplicator.ApplyDefault<T>(specificity, ref property);
			}
			else
			{
				applicatorFunc(sheet, handles, specificity, ref property);
			}
		}

		public static void ApplyShorthand(this StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData, ShorthandApplicatorFunction applicatorFunc)
		{
			if (handles[0].valueType != StyleValueType.Keyword && handles[0].valueIndex != 2)
			{
				applicatorFunc(sheet, handles, specificity, styleData);
			}
		}

		public static string ReadAsString(this StyleSheet sheet, StyleValueHandle handle)
		{
			string text = string.Empty;
			switch (handle.valueType)
			{
			case StyleValueType.Keyword:
				text = sheet.ReadKeyword(handle).ToString();
				break;
			case StyleValueType.Float:
				text = sheet.ReadFloat(handle).ToString();
				break;
			case StyleValueType.Color:
				text = sheet.ReadColor(handle).ToString();
				break;
			case StyleValueType.ResourcePath:
				text = sheet.ReadResourcePath(handle);
				break;
			case StyleValueType.Enum:
				text = sheet.ReadEnum(handle);
				break;
			case StyleValueType.String:
				text = sheet.ReadString(handle);
				break;
			default:
				throw new ArgumentException("Unhandled type " + handle.valueType);
			}
			return text;
		}
	}
}
