using System;
using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleSelectorHelper
	{
		public static MatchResultInfo MatchesSelector(VisualElement element, StyleSelector selector)
		{
			bool flag = true;
			StyleSelectorPart[] parts = selector.parts;
			int num = parts.Length;
			int num2 = 0;
			while (num2 < num && flag)
			{
				switch (parts[num2].type)
				{
				case StyleSelectorType.Wildcard:
					break;
				case StyleSelectorType.Type:
					flag = element.typeName == parts[num2].value;
					break;
				case StyleSelectorType.Class:
					flag = element.ClassListContains(parts[num2].value);
					break;
				case StyleSelectorType.PseudoClass:
					break;
				case StyleSelectorType.RecursivePseudoClass:
					goto IL_00D5;
				case StyleSelectorType.ID:
					flag = element.name == parts[num2].value;
					break;
				case StyleSelectorType.Predicate:
				{
					UQuery.IVisualPredicateWrapper visualPredicateWrapper = parts[num2].tempData as UQuery.IVisualPredicateWrapper;
					flag = visualPredicateWrapper != null && visualPredicateWrapper.Predicate(element);
					break;
				}
				default:
					goto IL_00D5;
				}
				IL_00DC:
				num2++;
				continue;
				IL_00D5:
				flag = false;
				goto IL_00DC;
			}
			int num3 = 0;
			int num4 = 0;
			bool flag2 = flag;
			if (flag2 && selector.pseudoStateMask != 0)
			{
				flag = (selector.pseudoStateMask & (int)element.pseudoStates) == selector.pseudoStateMask;
				if (flag)
				{
					num4 = selector.pseudoStateMask;
				}
				else
				{
					num3 = selector.pseudoStateMask;
				}
			}
			if (flag2 && selector.negatedPseudoStateMask != 0)
			{
				flag &= (selector.negatedPseudoStateMask & (int)(~(int)element.pseudoStates)) == selector.negatedPseudoStateMask;
				if (flag)
				{
					num3 |= selector.negatedPseudoStateMask;
				}
				else
				{
					num4 |= selector.negatedPseudoStateMask;
				}
			}
			return new MatchResultInfo(flag, (PseudoStates)num3, (PseudoStates)num4);
		}

		public static bool MatchRightToLeft(VisualElement element, StyleComplexSelector complexSelector, Action<VisualElement, MatchResultInfo> processResult)
		{
			VisualElement visualElement = element;
			int i = complexSelector.selectors.Length - 1;
			VisualElement visualElement2 = null;
			int num = -1;
			while (i >= 0)
			{
				if (visualElement == null)
				{
					break;
				}
				MatchResultInfo matchResultInfo = StyleSelectorHelper.MatchesSelector(visualElement, complexSelector.selectors[i]);
				processResult(visualElement, matchResultInfo);
				if (!matchResultInfo.success)
				{
					if (i < complexSelector.selectors.Length - 1 && complexSelector.selectors[i + 1].previousRelationship == StyleSelectorRelationship.Descendent)
					{
						visualElement = visualElement.parent;
					}
					else
					{
						if (visualElement2 == null)
						{
							break;
						}
						visualElement = visualElement2;
						i = num;
					}
				}
				else
				{
					if (i < complexSelector.selectors.Length - 1 && complexSelector.selectors[i + 1].previousRelationship == StyleSelectorRelationship.Descendent)
					{
						visualElement2 = visualElement.parent;
						num = i;
					}
					if (--i < 0)
					{
						return true;
					}
					visualElement = visualElement.parent;
				}
			}
			return false;
		}

		private static void FastLookup(IDictionary<string, StyleComplexSelector> table, List<SelectorMatchRecord> matchedSelectors, StyleMatchingContext context, string input, ref SelectorMatchRecord record)
		{
			StyleComplexSelector nextInTable;
			if (table.TryGetValue(input, out nextInTable))
			{
				while (nextInTable != null)
				{
					if (StyleSelectorHelper.MatchRightToLeft(context.currentElement, nextInTable, context.processResult))
					{
						record.complexSelector = nextInTable;
						matchedSelectors.Add(record);
					}
					nextInTable = nextInTable.nextInTable;
				}
			}
		}

		public static void FindMatches(StyleMatchingContext context, List<SelectorMatchRecord> matchedSelectors)
		{
			Debug.Assert(matchedSelectors.Count == 0);
			Debug.Assert(context.currentElement != null, "context.currentElement != null");
			VisualElement currentElement = context.currentElement;
			for (int i = 0; i < context.styleSheetStack.Count; i++)
			{
				StyleSheet styleSheet = context.styleSheetStack[i];
				SelectorMatchRecord selectorMatchRecord = new SelectorMatchRecord(styleSheet, i);
				StyleSelectorHelper.FastLookup(styleSheet.orderedTypeSelectors, matchedSelectors, context, currentElement.typeName, ref selectorMatchRecord);
				StyleSelectorHelper.FastLookup(styleSheet.orderedTypeSelectors, matchedSelectors, context, "*", ref selectorMatchRecord);
				if (!string.IsNullOrEmpty(currentElement.name))
				{
					StyleSelectorHelper.FastLookup(styleSheet.orderedNameSelectors, matchedSelectors, context, currentElement.name, ref selectorMatchRecord);
				}
				foreach (string text in currentElement.classList)
				{
					StyleSelectorHelper.FastLookup(styleSheet.orderedClassSelectors, matchedSelectors, context, text, ref selectorMatchRecord);
				}
			}
		}
	}
}
