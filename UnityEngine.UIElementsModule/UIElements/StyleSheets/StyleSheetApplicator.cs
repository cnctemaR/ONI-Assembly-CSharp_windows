using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class StyleSheetApplicator
	{
		public static void ApplyAlign(IStylePropertyReader reader, ref StyleInt property)
		{
			bool flag = reader.IsKeyword(0, StyleValueKeyword.Auto);
			if (flag)
			{
				StyleInt styleInt = new StyleInt(0)
				{
					specificity = reader.specificity
				};
				property = styleInt;
			}
			else
			{
				bool flag2 = !reader.IsValueType(0, StyleValueType.Enum);
				if (flag2)
				{
					Debug.LogError("Invalid value for align property " + reader.ReadAsString(0));
				}
				else
				{
					property = reader.ReadStyleEnum<Align>(0);
				}
			}
		}

		public static void ApplyDisplay(IStylePropertyReader reader, ref StyleInt property)
		{
			bool flag = reader.IsKeyword(0, StyleValueKeyword.None);
			if (flag)
			{
				StyleInt styleInt = new StyleInt(1)
				{
					specificity = reader.specificity
				};
				property = styleInt;
			}
			else
			{
				bool flag2 = !reader.IsValueType(0, StyleValueType.Enum);
				if (flag2)
				{
					Debug.LogError("Invalid value for display property " + reader.ReadAsString(0));
				}
				else
				{
					property = reader.ReadStyleEnum<DisplayStyle>(0);
				}
			}
		}
	}
}
