using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public class UxmlUnsignedLongAttributeDescription : TypedUxmlAttributeDescription<ulong>
	{
		public UxmlUnsignedLongAttributeDescription()
		{
			base.type = "unsignedLong";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = 0UL;
		}

		public override string defaultValueAsString
		{
			get
			{
				return base.defaultValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		public override ulong GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<ulong>(bag, cc, (string s, ulong l) => UxmlUnsignedLongAttributeDescription.ConvertValueToUlong(s, l), base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref ulong value)
		{
			return base.TryGetValueFromBag<ulong>(bag, cc, (string s, ulong l) => UxmlUnsignedLongAttributeDescription.ConvertValueToUlong(s, l), base.defaultValue, ref value);
		}

		private static ulong ConvertValueToUlong(string v, ulong defaultValue)
		{
			ulong num;
			bool flag = v == null || !ulong.TryParse(v, out num);
			ulong num2;
			if (flag)
			{
				num2 = defaultValue;
			}
			else
			{
				num2 = num;
			}
			return num2;
		}
	}
}
