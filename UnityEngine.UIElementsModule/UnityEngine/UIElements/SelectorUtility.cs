using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class SelectorUtility
	{
		public static bool ExtractSelectorsAndSpecificityFromString(string complexSelectorStr, out StyleSelector[] selectors, out int specificity, out string error)
		{
			selectors = null;
			specificity = -1;
			string[] array = complexSelectorStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			int selectorSpecificity = CSSSpec.GetSelectorSpecificity(complexSelectorStr);
			bool flag = selectorSpecificity == 0;
			bool flag2;
			if (flag)
			{
				error = "Selector '" + complexSelectorStr + "' is invalid: failed to calculate selector specificity.";
				flag2 = false;
			}
			else
			{
				List<StyleSelector> list = new List<StyleSelector>();
				StyleSelectorRelationship styleSelectorRelationship = StyleSelectorRelationship.None;
				foreach (string text in array)
				{
					bool flag3 = text == ">";
					if (flag3)
					{
						styleSelectorRelationship = StyleSelectorRelationship.Child;
					}
					else
					{
						StyleSelectorPart[] array3;
						bool flag4 = !CSSSpec.ParseSelector(text, out array3);
						if (flag4)
						{
							error = "Selector '" + complexSelectorStr + "' is invalid: the selector could not be parsed.";
							return false;
						}
						foreach (StyleSelectorPart styleSelectorPart in array3)
						{
							StyleSelectorType type = styleSelectorPart.type;
							StyleSelectorType styleSelectorType = type;
							if (styleSelectorType == StyleSelectorType.Unknown)
							{
								error = "Selector '" + complexSelectorStr + "' is invalid: the selector contains unknown parts.";
								return false;
							}
							if (styleSelectorType == StyleSelectorType.RecursivePseudoClass)
							{
								error = "Selector '" + complexSelectorStr + "' is invalid: the selector contains recursive parts.";
								return false;
							}
						}
						StyleSelector styleSelector = new StyleSelector
						{
							parts = array3,
							previousRelationship = styleSelectorRelationship
						};
						list.Add(styleSelector);
						styleSelectorRelationship = StyleSelectorRelationship.Descendent;
					}
				}
				selectors = list.ToArray();
				specificity = selectorSpecificity;
				error = null;
				flag2 = true;
			}
			return flag2;
		}

		public static bool CompareSelectors(StyleComplexSelector lhs, StyleComplexSelector rhs)
		{
			bool flag = lhs.isSimple != rhs.isSimple || lhs.specificity != rhs.specificity || lhs.selectors.Length != rhs.selectors.Length;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < lhs.selectors.Length; i++)
				{
					StyleSelector styleSelector = lhs.selectors[i];
					StyleSelector styleSelector2 = rhs.selectors[i];
					bool flag3 = styleSelector.parts.Length != styleSelector2.parts.Length;
					if (flag3)
					{
						return false;
					}
					bool flag4 = styleSelector.previousRelationship != styleSelector2.previousRelationship;
					if (flag4)
					{
						return false;
					}
					for (int j = 0; j < styleSelector.parts.Length; j++)
					{
						bool flag5 = !EqualityComparer<StyleSelectorPart>.Default.Equals(styleSelector.parts[j], styleSelector2.parts[j]);
						if (flag5)
						{
							return false;
						}
					}
				}
				flag2 = true;
			}
			return flag2;
		}

		private const string k_DescendantSymbol = ">";
	}
}
