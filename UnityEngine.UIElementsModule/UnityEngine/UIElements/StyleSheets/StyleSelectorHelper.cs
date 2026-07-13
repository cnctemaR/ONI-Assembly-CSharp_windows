using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements.StyleSheets
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
					flag = string.Equals(element.typeName, parts[num2].value, StringComparison.Ordinal);
					break;
				case StyleSelectorType.Class:
					flag = element.ClassListContains(parts[num2].value);
					break;
				case StyleSelectorType.PseudoClass:
				{
					bool flag2 = selector.pseudoStateMask == -1 || selector.negatedPseudoStateMask == -1;
					if (flag2)
					{
						flag = false;
					}
					break;
				}
				case StyleSelectorType.RecursivePseudoClass:
					goto IL_00F1;
				case StyleSelectorType.ID:
					flag = string.Equals(element.name, parts[num2].value, StringComparison.Ordinal);
					break;
				case StyleSelectorType.Predicate:
				{
					UQuery.IVisualPredicateWrapper visualPredicateWrapper = parts[num2].tempData as UQuery.IVisualPredicateWrapper;
					flag = visualPredicateWrapper != null && visualPredicateWrapper.Predicate(element);
					break;
				}
				default:
					goto IL_00F1;
				}
				IL_00F5:
				num2++;
				continue;
				IL_00F1:
				flag = false;
				goto IL_00F5;
			}
			int num3 = 0;
			int num4 = 0;
			bool flag3 = flag;
			bool flag4 = flag3 && selector.pseudoStateMask != 0;
			if (flag4)
			{
				flag = (selector.pseudoStateMask & (int)element.pseudoStates) == selector.pseudoStateMask;
				bool flag5 = flag;
				if (flag5)
				{
					num4 = selector.pseudoStateMask;
				}
				else
				{
					num3 = selector.pseudoStateMask;
				}
			}
			bool flag6 = flag3 && selector.negatedPseudoStateMask != 0;
			if (flag6)
			{
				flag &= (selector.negatedPseudoStateMask & (int)(~(int)element.pseudoStates)) == selector.negatedPseudoStateMask;
				bool flag7 = flag;
				if (flag7)
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
				bool flag = visualElement == null;
				if (flag)
				{
					break;
				}
				MatchResultInfo matchResultInfo = StyleSelectorHelper.MatchesSelector(visualElement, complexSelector.selectors[i]);
				processResult(visualElement, matchResultInfo);
				bool flag2 = !matchResultInfo.success;
				if (flag2)
				{
					bool flag3 = i < complexSelector.selectors.Length - 1 && complexSelector.selectors[i + 1].previousRelationship == StyleSelectorRelationship.Descendent;
					if (flag3)
					{
						visualElement = visualElement.parent;
					}
					else
					{
						bool flag4 = visualElement2 != null;
						if (!flag4)
						{
							break;
						}
						visualElement = visualElement2;
						i = num;
					}
				}
				else
				{
					bool flag5 = i < complexSelector.selectors.Length - 1 && complexSelector.selectors[i + 1].previousRelationship == StyleSelectorRelationship.Descendent;
					if (flag5)
					{
						visualElement2 = visualElement.parent;
						num = i;
					}
					bool flag6 = --i < 0;
					if (flag6)
					{
						return true;
					}
					visualElement = visualElement.parent;
				}
			}
			return false;
		}

		private static void TestSelectorLinkedList(StyleComplexSelector currentComplexSelector, List<SelectorMatchRecord> matchedSelectors, StyleMatchingContext context, ref SelectorMatchRecord record)
		{
			while (currentComplexSelector != null)
			{
				bool flag = true;
				bool flag2 = false;
				bool flag3 = !currentComplexSelector.isSimple;
				if (flag3)
				{
					flag = context.ancestorFilter.IsCandidate(currentComplexSelector);
				}
				bool flag4 = flag;
				if (flag4)
				{
					flag2 = StyleSelectorHelper.MatchRightToLeft(context.currentElement, currentComplexSelector, context.processResult);
				}
				bool flag5 = flag2;
				if (flag5)
				{
					record.complexSelector = currentComplexSelector;
					matchedSelectors.Add(record);
				}
				currentComplexSelector = currentComplexSelector.nextInTable;
			}
		}

		private static void FastLookup(IDictionary<string, StyleComplexSelector> table, List<SelectorMatchRecord> matchedSelectors, StyleMatchingContext context, string input, ref SelectorMatchRecord record)
		{
			StyleComplexSelector styleComplexSelector;
			bool flag = table.TryGetValue(input, out styleComplexSelector);
			if (flag)
			{
				StyleSelectorHelper.TestSelectorLinkedList(styleComplexSelector, matchedSelectors, context, ref record);
			}
		}

		public static void FindMatches(StyleMatchingContext context, List<SelectorMatchRecord> matchedSelectors)
		{
			VisualElement currentElement = context.currentElement;
			int num = context.styleSheetCount - 1;
			bool flag = currentElement.styleSheetList != null;
			if (flag)
			{
				int num2 = currentElement.styleSheetList.Count;
				for (int i = 0; i < currentElement.styleSheetList.Count; i++)
				{
					StyleSheet styleSheet = currentElement.styleSheetList[i];
					bool flag2 = styleSheet.flattenedRecursiveImports != null;
					if (flag2)
					{
						num2 += styleSheet.flattenedRecursiveImports.Count;
					}
				}
				num -= num2;
			}
			StyleSelectorHelper.FindMatches(context, matchedSelectors, num);
		}

		public static void FindMatches(StyleMatchingContext context, List<SelectorMatchRecord> matchedSelectors, int parentSheetIndex)
		{
			Debug.Assert(matchedSelectors.Count == 0);
			Debug.Assert(context.currentElement != null, "context.currentElement != null");
			bool flag = false;
			HashSet<StyleSheet> hashSet = CollectionPool<HashSet<StyleSheet>, StyleSheet>.Get();
			List<StyleSelectorHelper.SelectorWorkItem> list = CollectionPool<List<StyleSelectorHelper.SelectorWorkItem>, StyleSelectorHelper.SelectorWorkItem>.Get();
			try
			{
				VisualElement currentElement = context.currentElement;
				list.Add(new StyleSelectorHelper.SelectorWorkItem(StyleSheet.OrderedSelectorType.Type, currentElement.typeName));
				bool flag2 = !string.IsNullOrEmpty(currentElement.name);
				if (flag2)
				{
					list.Add(new StyleSelectorHelper.SelectorWorkItem(StyleSheet.OrderedSelectorType.Name, currentElement.name));
				}
				List<string> classesForIteration = currentElement.GetClassesForIteration();
				int count = classesForIteration.Count;
				for (int i = 0; i < count; i++)
				{
					list.Add(new StyleSelectorHelper.SelectorWorkItem(StyleSheet.OrderedSelectorType.Class, classesForIteration[i]));
				}
				for (int j = context.styleSheetCount - 1; j >= 0; j--)
				{
					StyleSheet styleSheetAt = context.GetStyleSheetAt(j);
					bool flag3 = !hashSet.Add(styleSheetAt);
					if (!flag3)
					{
						styleSheetAt.RebuildIfNecessary();
						bool flag4 = j > parentSheetIndex;
						if (flag4)
						{
							currentElement.pseudoStates |= PseudoStates.Root;
							flag = true;
						}
						else
						{
							currentElement.pseudoStates &= ~PseudoStates.Root;
						}
						SelectorMatchRecord selectorMatchRecord = new SelectorMatchRecord(styleSheetAt, j);
						for (int k = 0; k < list.Count; k++)
						{
							StyleSelectorHelper.SelectorWorkItem selectorWorkItem = list[k];
							bool flag5 = (styleSheetAt.nonEmptyTablesMask & (1 << (int)selectorWorkItem.type)) == 0;
							if (!flag5)
							{
								Dictionary<string, StyleComplexSelector> dictionary = styleSheetAt.tables[(int)selectorWorkItem.type];
								StyleSelectorHelper.FastLookup(dictionary, matchedSelectors, context, selectorWorkItem.input, ref selectorMatchRecord);
							}
						}
						bool flag6 = flag;
						if (flag6)
						{
							StyleSelectorHelper.TestSelectorLinkedList(styleSheetAt.firstRootSelector, matchedSelectors, context, ref selectorMatchRecord);
						}
						StyleSelectorHelper.TestSelectorLinkedList(styleSheetAt.firstWildCardSelector, matchedSelectors, context, ref selectorMatchRecord);
					}
				}
				bool flag7 = flag;
				if (flag7)
				{
					currentElement.pseudoStates &= ~PseudoStates.Root;
				}
			}
			finally
			{
				CollectionPool<List<StyleSelectorHelper.SelectorWorkItem>, StyleSelectorHelper.SelectorWorkItem>.Release(list);
				CollectionPool<HashSet<StyleSheet>, StyleSheet>.Release(hashSet);
			}
		}

		private struct SelectorWorkItem
		{
			public SelectorWorkItem(StyleSheet.OrderedSelectorType type, string input)
			{
				this.type = type;
				this.input = input;
			}

			public StyleSheet.OrderedSelectorType type;

			public string input;
		}
	}
}
