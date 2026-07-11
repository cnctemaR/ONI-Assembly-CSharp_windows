using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	internal struct RuleMatcher
	{
		public RuleMatcher(StyleSheet sheet, StyleComplexSelector complexSelector, int styleSheetIndexInStack)
		{
			this.sheet = sheet;
			this.complexSelector = complexSelector;
		}

		public override string ToString()
		{
			return this.complexSelector.ToString();
		}

		public StyleSheet sheet;

		public StyleComplexSelector complexSelector;
	}
}
