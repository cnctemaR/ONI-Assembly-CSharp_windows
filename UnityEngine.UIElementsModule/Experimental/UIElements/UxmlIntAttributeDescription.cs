using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlIntAttributeDescription : UxmlAttributeDescription
	{
		public UxmlIntAttributeDescription()
		{
			base.type = "int";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = 0;
		}

		public int defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString();
			}
		}

		[Obsolete("Pass a creation context to the method.")]
		public int GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public int GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<int>(bag, cc, new Func<string, int, int>(UxmlIntAttributeDescription.ConvertValueToInt), this.defaultValue);
		}

		private static int ConvertValueToInt(string v, int defaultValue)
		{
			int num;
			int num2;
			if (v == null || !int.TryParse(v, out num))
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
