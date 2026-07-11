using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlFloatAttributeDescription : UxmlAttributeDescription
	{
		public UxmlFloatAttributeDescription()
		{
			base.type = "float";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = 0f;
		}

		public float defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString();
			}
		}

		[Obsolete("Pass a creation context to the method.")]
		public float GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public float GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<float>(bag, cc, new Func<string, float, float>(UxmlFloatAttributeDescription.ConvertValueToFloat), this.defaultValue);
		}

		private static float ConvertValueToFloat(string v, float defaultValue)
		{
			float num;
			float num2;
			if (v == null || !float.TryParse(v, out num))
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
