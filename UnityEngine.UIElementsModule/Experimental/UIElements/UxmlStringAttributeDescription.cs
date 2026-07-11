using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlStringAttributeDescription : UxmlAttributeDescription
	{
		public UxmlStringAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = "";
		}

		public string defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue;
			}
		}

		[Obsolete("Pass a creation context to the method.")]
		public string GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public string GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<string>(bag, cc, new Func<string, string, string>(UxmlStringAttributeDescription.ConvertValueToString), this.defaultValue);
		}

		private static string ConvertValueToString(string v, string defaultValue)
		{
			return v;
		}
	}
}
