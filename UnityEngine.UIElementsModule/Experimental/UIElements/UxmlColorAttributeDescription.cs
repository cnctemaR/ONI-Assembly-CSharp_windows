using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlColorAttributeDescription : UxmlAttributeDescription
	{
		public UxmlColorAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = new Color(0f, 0f, 0f, 1f);
		}

		public Color defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString();
			}
		}

		[Obsolete("Pass a creation context to the method.")]
		public Color GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public Color GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<Color>(bag, cc, new Func<string, Color, Color>(UxmlColorAttributeDescription.ConvertValueToColor), this.defaultValue);
		}

		private static Color ConvertValueToColor(string v, Color defaultValue)
		{
			Color color;
			Color color2;
			if (v == null || !ColorUtility.TryParseHtmlString(v, out color))
			{
				color2 = defaultValue;
			}
			else
			{
				color2 = color;
			}
			return color2;
		}
	}
}
