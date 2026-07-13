using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal readonly struct UxmlData
	{
		public UxmlData(StyleProperty p, BindingInfo b, SelectorMatchRecord s)
		{
			this.inlineProperty = p;
			this.bindingInfo = b;
			this.selector = s;
		}

		public static UxmlData WithProperty(in UxmlData data, StyleProperty property)
		{
			return new UxmlData(property, data.bindingInfo, data.selector);
		}

		public static UxmlData WithBindingInfo(in UxmlData data, BindingInfo bindingInfo)
		{
			return new UxmlData(data.inlineProperty, bindingInfo, data.selector);
		}

		public static UxmlData WithSelector(in UxmlData data, SelectorMatchRecord selector)
		{
			return new UxmlData(data.inlineProperty, data.bindingInfo, selector);
		}

		public readonly StyleProperty inlineProperty;

		public readonly BindingInfo bindingInfo;

		public readonly SelectorMatchRecord selector;
	}
}
