using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreeStyleUpdaterTraversal : HierarchyTraversal
	{
		public float currentPixelsPerPoint { get; set; } = 1f;

		public void AddChangedElement(VisualElement ve)
		{
			this.m_UpdateList.Add(ve);
			this.PropagateToChildren(ve);
			this.PropagateToParents(ve);
		}

		public void Clear()
		{
			this.m_UpdateList.Clear();
			this.m_ParentList.Clear();
			this.m_TempMatchResults.Clear();
		}

		private void PropagateToChildren(VisualElement ve)
		{
			int childCount = ve.shadow.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.shadow[i];
				bool flag = this.m_UpdateList.Add(visualElement);
				if (flag)
				{
					this.PropagateToChildren(visualElement);
				}
			}
		}

		private void PropagateToParents(VisualElement ve)
		{
			for (VisualElement visualElement = ve.shadow.parent; visualElement != null; visualElement = visualElement.shadow.parent)
			{
				if (!this.m_ParentList.Add(visualElement))
				{
					break;
				}
			}
		}

		private static void OnProcessMatchResult(VisualElement current, MatchResultInfo info)
		{
			current.triggerPseudoMask |= info.triggerPseudoMask;
			current.dependencyPseudoMask |= info.dependencyPseudoMask;
		}

		public override void TraverseRecursive(VisualElement element, int depth)
		{
			if (!this.ShouldSkipElement(element))
			{
				if (this.m_UpdateList.Contains(element))
				{
					element.triggerPseudoMask = (PseudoStates)0;
					element.dependencyPseudoMask = (PseudoStates)0;
				}
				int count = this.m_StyleMatchingContext.styleSheetStack.Count;
				if (element.styleSheets != null)
				{
					for (int i = 0; i < element.styleSheets.Count; i++)
					{
						StyleSheet styleSheet = element.styleSheets[i];
						this.m_StyleMatchingContext.styleSheetStack.Add(styleSheet);
					}
				}
				if (this.m_UpdateList.Contains(element))
				{
					this.m_StyleMatchingContext.currentElement = element;
					StyleSelectorHelper.FindMatches(this.m_StyleMatchingContext, this.m_TempMatchResults);
					this.ProcessMatchedRules(element, this.m_TempMatchResults);
					this.m_StyleMatchingContext.currentElement = null;
					this.m_TempMatchResults.Clear();
				}
				base.Recurse(element, depth);
				if (this.m_StyleMatchingContext.styleSheetStack.Count > count)
				{
					this.m_StyleMatchingContext.styleSheetStack.RemoveRange(count, this.m_StyleMatchingContext.styleSheetStack.Count - count);
				}
			}
		}

		private bool ShouldSkipElement(VisualElement element)
		{
			return !this.m_ParentList.Contains(element) && !this.m_UpdateList.Contains(element);
		}

		private void ProcessMatchedRules(VisualElement element, List<SelectorMatchRecord> matchingSelectors)
		{
			matchingSelectors.Sort(new Comparison<SelectorMatchRecord>(SelectorMatchRecord.Compare));
			long num = (long)element.fullTypeName.GetHashCode();
			num = (num * 397L) ^ (long)this.currentPixelsPerPoint.GetHashCode();
			foreach (SelectorMatchRecord selectorMatchRecord in matchingSelectors)
			{
				StyleRule rule = selectorMatchRecord.complexSelector.rule;
				int specificity = selectorMatchRecord.complexSelector.specificity;
				num = (num * 397L) ^ (long)rule.GetHashCode();
				num = (num * 397L) ^ (long)specificity;
			}
			VisualElementStylesData visualElementStylesData;
			if (StyleCache.TryGetValue(num, out visualElementStylesData))
			{
				element.SetSharedStyles(visualElementStylesData);
			}
			else
			{
				visualElementStylesData = new VisualElementStylesData(true);
				foreach (SelectorMatchRecord selectorMatchRecord2 in matchingSelectors)
				{
					StylePropertyID[] propertyIDs = StyleSheetCache.GetPropertyIDs(selectorMatchRecord2.sheet, selectorMatchRecord2.complexSelector.ruleIndex);
					visualElementStylesData.ApplyRule(selectorMatchRecord2.sheet, selectorMatchRecord2.complexSelector.specificity, selectorMatchRecord2.complexSelector.rule, propertyIDs);
				}
				visualElementStylesData.ApplyLayoutValues();
				StyleCache.SetValue(num, visualElementStylesData);
				element.SetSharedStyles(visualElementStylesData);
			}
		}

		private HashSet<VisualElement> m_UpdateList = new HashSet<VisualElement>();

		private HashSet<VisualElement> m_ParentList = new HashSet<VisualElement>();

		private List<SelectorMatchRecord> m_TempMatchResults = new List<SelectorMatchRecord>();

		private StyleMatchingContext m_StyleMatchingContext = new StyleMatchingContext(new Action<VisualElement, MatchResultInfo>(VisualTreeStyleUpdaterTraversal.OnProcessMatchResult));
	}
}
