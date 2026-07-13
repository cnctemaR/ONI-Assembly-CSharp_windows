using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct SelectorMatchRecord : IEquatable<SelectorMatchRecord>
	{
		public SelectorMatchRecord(StyleSheet sheet, int styleSheetIndexInStack)
		{
			this = default(SelectorMatchRecord);
			this.sheet = sheet;
			this.styleSheetIndexInStack = styleSheetIndexInStack;
		}

		public static int Compare(SelectorMatchRecord a, SelectorMatchRecord b)
		{
			bool flag = a.sheet.isDefaultStyleSheet != b.sheet.isDefaultStyleSheet;
			int num;
			if (flag)
			{
				num = (a.sheet.isDefaultStyleSheet ? (-1) : 1);
			}
			else
			{
				int num2 = a.complexSelector.specificity.CompareTo(b.complexSelector.specificity);
				bool flag2 = num2 == 0;
				if (flag2)
				{
					num2 = a.styleSheetIndexInStack.CompareTo(b.styleSheetIndexInStack);
				}
				bool flag3 = num2 == 0;
				if (flag3)
				{
					num2 = a.complexSelector.orderInStyleSheet.CompareTo(b.complexSelector.orderInStyleSheet);
				}
				num = num2;
			}
			return num;
		}

		public bool Equals(SelectorMatchRecord other)
		{
			return object.Equals(this.sheet, other.sheet) && this.styleSheetIndexInStack == other.styleSheetIndexInStack && object.Equals(this.complexSelector, other.complexSelector);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is SelectorMatchRecord)
			{
				SelectorMatchRecord selectorMatchRecord = (SelectorMatchRecord)obj;
				flag = this.Equals(selectorMatchRecord);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<StyleSheet, int, StyleComplexSelector>(this.sheet, this.styleSheetIndexInStack, this.complexSelector);
		}

		public StyleSheet sheet;

		public int styleSheetIndexInStack;

		public StyleComplexSelector complexSelector;
	}
}
