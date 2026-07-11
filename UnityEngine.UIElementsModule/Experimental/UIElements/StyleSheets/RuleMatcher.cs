using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal struct RuleMatcher
	{
		public override string ToString()
		{
			return this.complexSelector.ToString();
		}

		public StyleSheet sheet;

		public StyleComplexSelector complexSelector;
	}
}
