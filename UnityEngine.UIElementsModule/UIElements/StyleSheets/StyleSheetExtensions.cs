using System;
using System.Globalization;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class StyleSheetExtensions
	{
		public static string ReadAsString(this StyleSheet sheet, StyleValueHandle handle)
		{
			string text = string.Empty;
			switch (handle.valueType)
			{
			case StyleValueType.Keyword:
				text = sheet.ReadKeyword(handle).ToUssString();
				break;
			case StyleValueType.Float:
				text = sheet.ReadFloat(handle).ToString(CultureInfo.InvariantCulture.NumberFormat);
				break;
			case StyleValueType.Dimension:
				text = sheet.ReadDimension(handle).ToString();
				break;
			case StyleValueType.Color:
				text = sheet.ReadColor(handle).ToString();
				break;
			case StyleValueType.ResourcePath:
				text = sheet.ReadResourcePath(handle);
				break;
			case StyleValueType.AssetReference:
				text = sheet.ReadAssetReference(handle).ToString();
				break;
			case StyleValueType.Enum:
				text = sheet.ReadEnum(handle);
				break;
			case StyleValueType.Variable:
				text = sheet.ReadVariable(handle);
				break;
			case StyleValueType.String:
				text = sheet.ReadString(handle);
				break;
			case StyleValueType.Function:
				text = sheet.ReadFunctionName(handle);
				break;
			case StyleValueType.CommaSeparator:
				text = ",";
				break;
			case StyleValueType.ScalableImage:
				text = sheet.ReadScalableImage(handle).ToString();
				break;
			default:
				text = "Error reading value type (" + handle.valueType.ToString() + ") at index " + handle.valueIndex.ToString();
				break;
			}
			return text;
		}

		public static bool IsVarFunction(this StyleValueHandle handle)
		{
			return handle.valueType == StyleValueType.Function && handle.valueIndex == 1;
		}
	}
}
