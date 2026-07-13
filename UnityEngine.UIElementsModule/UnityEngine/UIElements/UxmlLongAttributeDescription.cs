using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public class UxmlLongAttributeDescription : TypedUxmlAttributeDescription<long>
	{
		public UxmlLongAttributeDescription()
		{
			base.type = "long";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = 0L;
		}

		public override string defaultValueAsString
		{
			get
			{
				return base.defaultValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		public override long GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<long>(bag, cc, (string s, long l) => UxmlLongAttributeDescription.ConvertValueToLong(s, l), base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref long value)
		{
			return base.TryGetValueFromBag<long>(bag, cc, (string s, long l) => UxmlLongAttributeDescription.ConvertValueToLong(s, l), base.defaultValue, ref value);
		}

		private static long ConvertValueToLong(string v, long defaultValue)
		{
			long num;
			bool flag = v == null || !long.TryParse(v, out num);
			long num2;
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
