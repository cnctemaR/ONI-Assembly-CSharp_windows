using System;

namespace UnityEngine.UIElements
{
	public class UxmlBoolAttributeDescription : TypedUxmlAttributeDescription<bool>
	{
		public UxmlBoolAttributeDescription()
		{
			base.type = "boolean";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = false;
		}

		public override string defaultValueAsString
		{
			get
			{
				return base.defaultValue.ToString().ToLowerInvariant();
			}
		}

		public override bool GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<bool>(bag, cc, (string s, bool b) => UxmlBoolAttributeDescription.ConvertValueToBool(s, b), base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref bool value)
		{
			return base.TryGetValueFromBag<bool>(bag, cc, (string s, bool b) => UxmlBoolAttributeDescription.ConvertValueToBool(s, b), base.defaultValue, ref value);
		}

		private static bool ConvertValueToBool(string v, bool defaultValue)
		{
			bool flag2;
			bool flag = v == null || !bool.TryParse(v, out flag2);
			bool flag3;
			if (flag)
			{
				flag3 = defaultValue;
			}
			else
			{
				flag3 = flag2;
			}
			return flag3;
		}
	}
}
