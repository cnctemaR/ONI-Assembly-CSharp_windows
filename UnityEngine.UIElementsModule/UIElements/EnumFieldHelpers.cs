using System;

namespace UnityEngine.UIElements
{
	internal static class EnumFieldHelpers
	{
		internal static bool ExtractValue(IUxmlAttributes bag, CreationContext cc, out Type resEnumType, out Enum resEnumValue, out bool resIncludeObsoleteValues)
		{
			resIncludeObsoleteValues = false;
			resEnumValue = null;
			resEnumType = EnumFieldHelpers.type.GetValueFromBag(bag, cc);
			bool flag = resEnumType == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				string text = null;
				object obj = null;
				bool flag3 = EnumFieldHelpers.value.TryGetValueFromBag(bag, cc, ref text) && !Enum.TryParse(resEnumType, text, false, out obj);
				if (flag3)
				{
					Debug.LogErrorFormat("EnumField: Could not parse value of '{0}', because it isn't defined in the {1} enum.", new object[] { text, resEnumType.FullName });
					flag2 = false;
				}
				else
				{
					resEnumValue = ((text != null && obj != null) ? ((Enum)obj) : ((Enum)Enum.ToObject(resEnumType, 0)));
					resIncludeObsoleteValues = EnumFieldHelpers.includeObsoleteValues.GetValueFromBag(bag, cc);
					flag2 = true;
				}
			}
			return flag2;
		}

		internal static readonly UxmlTypeAttributeDescription<Enum> type = new UxmlTypeAttributeDescription<Enum>
		{
			name = "type"
		};

		internal static readonly UxmlStringAttributeDescription value = new UxmlStringAttributeDescription
		{
			name = "value"
		};

		internal static readonly UxmlBoolAttributeDescription includeObsoleteValues = new UxmlBoolAttributeDescription
		{
			name = "include-obsolete-values",
			defaultValue = false
		};
	}
}
