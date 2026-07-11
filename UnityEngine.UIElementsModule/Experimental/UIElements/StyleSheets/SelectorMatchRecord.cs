using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal struct SelectorMatchRecord
	{
		public SelectorMatchRecord(StyleSheet sheet, int styleSheetIndexInStack)
		{
			this = default(SelectorMatchRecord);
			this.sheet = sheet;
			this.styleSheetIndexInStack = styleSheetIndexInStack;
		}

		public static int Compare(SelectorMatchRecord a, SelectorMatchRecord b)
		{
			int num = a.styleSheetIndexInStack.CompareTo(b.styleSheetIndexInStack);
			if (num == 0)
			{
				num = a.complexSelector.orderInStyleSheet.CompareTo(b.complexSelector.orderInStyleSheet);
			}
			return num;
		}

		public StyleSheet sheet;

		public int styleSheetIndexInStack;

		public StyleComplexSelector complexSelector;
	}
}
