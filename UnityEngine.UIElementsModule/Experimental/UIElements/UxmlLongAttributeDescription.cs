using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlLongAttributeDescription : UxmlAttributeDescription
	{
		public UxmlLongAttributeDescription()
		{
			base.type = "long";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = 0L;
		}

		public long defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString();
			}
		}

		[Obsolete("Pass a creation context to the method.")]
		public long GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public long GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<long>(bag, cc, new Func<string, long, long>(UxmlLongAttributeDescription.ConvertValueToLong), this.defaultValue);
		}

		private static long ConvertValueToLong(string v, long defaultValue)
		{
			long num;
			long num2;
			if (v == null || !long.TryParse(v, out num))
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
