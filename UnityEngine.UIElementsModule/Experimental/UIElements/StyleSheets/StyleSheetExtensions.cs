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
				StyleSheetApplicator.ApplyValue<T>(specificity, ref property, default(T));
			}
			else
			{
				applicatorFunc(sheet, handles, specificity, ref property);
			}
		}

		public static void ApplyShorthand(this StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData, ShorthandApplicatorFunction applicatorFunc)
		{
			applicatorFunc(sheet, handles, specificity, styleData);
		}

		public static string ReadAsString(this StyleSheet sheet, StyleValueHandle handle)
		{
			string empty = string.Empty;
			switch (handle.valueType)
			{
			case StyleValueType.Keyword:
				return sheet.ReadKeyword(handle).ToString();
			case StyleValueType.Float:
				return sheet.ReadFloat(handle).ToString();
			case StyleValueType.Color:
				return sheet.ReadColor(handle).ToString();
			case StyleValueType.ResourcePath:
				return sheet.ReadResourcePath(handle);
			case StyleValueType.Enum:
				return sheet.ReadEnum(handle);
			case StyleValueType.String:
				return sheet.ReadString(handle);
			}
			throw new ArgumentException("Unhandled type " + handle.valueType);
		}
	}
}
