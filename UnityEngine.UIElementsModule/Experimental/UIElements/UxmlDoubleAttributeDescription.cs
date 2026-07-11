using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlDoubleAttributeDescription : UxmlAttributeDescription
	{
		public UxmlDoubleAttributeDescription()
		{
			base.type = "double";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = 0.0;
		}

		public double defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString();
			}
		}

		[Obsolete("Pass a creation context to the method.")]
		public double GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public double GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<double>(bag, cc, new Func<string, double, double>(UxmlDoubleAttributeDescription.ConvertValueToDouble), this.defaultValue);
		}

		private static double ConvertValueToDouble(string v, double defaultValue)
		{
			double num;
			double num2;
			if (v == null || !double.TryParse(v, out num))
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
