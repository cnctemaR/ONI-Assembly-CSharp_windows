using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public class UxmlUnsignedIntAttributeDescription : TypedUxmlAttributeDescription<uint>
	{
		public UxmlUnsignedIntAttributeDescription()
		{
			base.type = "unsignedInt";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = 0U;
		}

		public override string defaultValueAsString
		{
			get
			{
				return base.defaultValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		public override uint GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<uint>(bag, cc, (string s, uint i) => UxmlUnsignedIntAttributeDescription.ConvertValueToUInt(s, i), base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref uint value)
		{
			return base.TryGetValueFromBag<uint>(bag, cc, (string s, uint i) => UxmlUnsignedIntAttributeDescription.ConvertValueToUInt(s, i), base.defaultValue, ref value);
		}

		private static uint ConvertValueToUInt(string v, uint defaultValue)
		{
			uint num;
			bool flag = v == null || !uint.TryParse(v, out num);
			uint num2;
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
