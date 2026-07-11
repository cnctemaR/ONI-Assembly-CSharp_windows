using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlBoolAttributeDescription : UxmlAttributeDescription
	{
		public UxmlBoolAttributeDescription()
		{
			base.type = "boolean";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = false;
		}

		public bool defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString().ToLower();
			}
		}

		[Obsolete("Pass a creation context to the method.")]
		public bool GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public bool GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<bool>(bag, cc, new Func<string, bool, bool>(UxmlBoolAttributeDescription.ConvertValueToBool), this.defaultValue);
		}

		private static bool ConvertValueToBool(string v, bool defaultValue)
		{
			bool flag;
			bool flag2;
			if (v == null || !bool.TryParse(v, out flag))
			{
				flag2 = defaultValue;
			}
			else
			{
				flag2 = flag;
			}
			return flag2;
		}
	}
}
