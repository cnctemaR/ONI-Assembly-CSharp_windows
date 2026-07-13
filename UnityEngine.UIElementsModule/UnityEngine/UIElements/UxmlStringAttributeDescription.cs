using System;

namespace UnityEngine.UIElements
{
	public class UxmlStringAttributeDescription : TypedUxmlAttributeDescription<string>
	{
		public UxmlStringAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = "";
		}

		public override string defaultValueAsString
		{
			get
			{
				return base.defaultValue;
			}
		}

		public override string GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<string>(bag, cc, (string s, string t) => s, base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref string value)
		{
			return base.TryGetValueFromBag<string>(bag, cc, (string s, string t) => s, base.defaultValue, ref value);
		}
	}
}
