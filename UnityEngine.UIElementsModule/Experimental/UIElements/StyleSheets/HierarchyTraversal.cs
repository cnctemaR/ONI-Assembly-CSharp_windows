using System;
using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal abstract class HierarchyTraversal : IHierarchyTraversal
	{
		public abstract bool ShouldSkipElement(VisualElement element);

		public abstract bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element);

		public virtual void OnBeginElementTest(VisualElement element, List<RuleMatcher> ruleMatchers)
		{
		}

		public void BeginElementTest(VisualElement element, List<RuleMatcher> ruleMatchers)
		{
			this.OnBeginElementTest(element, ruleMatchers);
		}

		public virtual void ProcessMatchedRules(VisualElement element)
		{
		}

		public virtual void OnProcessMatchResult(VisualElement element, ref RuleMatcher matcher, ref HierarchyTraversal.MatchResultInfo matchInfo)
		{
		}

		public virtual void Traverse(VisualElement element)
		{
			this.TraverseRecursive(element, 0, this.m_ruleMatchers);
			this.m_ruleMatchers.Clear();
		}

		public virtual void TraverseRecursive(VisualElement element, int depth, List<RuleMatcher> ruleMatchers)
		{
			if (!this.ShouldSkipElement(element))
			{
				int count = ruleMatchers.Count;
				this.BeginElementTest(element, ruleMatchers);
				int count2 = ruleMatchers.Count;
				for (int i = 0; i < count2; i++)
				{
					RuleMatcher ruleMatcher = ruleMatchers[i];
					if (this.MatchRightToLeft(element, ref ruleMatcher))
					{
						return;
					}
				}
				this.ProcessMatchedRules(element);
				this.Recurse(element, depth, ruleMatchers);
				if (ruleMatchers.Count > count)
				{
					ruleMatchers.RemoveRange(count, ruleMatchers.Count - count);
				}
			}
		}

		private bool MatchRightToLeft(VisualElement element, ref RuleMatcher matcher)
		{
			VisualElement visualElement = element;
			int i = matcher.complexSelector.selectors.Length - 1;
			VisualElement visualElement2 = null;
			int num = -1;
			while (i >= 0)
			{
				if (visualElement == null)
				{
					break;
				}
				HierarchyTraversal.MatchResultInfo matchResultInfo = this.Match(visualElement, ref matcher, i);
				this.OnProcessMatchResult(visualElement, ref matcher, ref matchResultInfo);
				if (!matchResultInfo.success)
				{
					if (i < matcher.complexSelector.selectors.Length - 1 && matcher.complexSelector.selectors[i + 1].previousRelationship == StyleSelectorRelationship.Descendent)
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
					if (i < matcher.complexSelector.selectors.Length - 1 && matcher.complexSelector.selectors[i + 1].previousRelationship == StyleSelectorRelationship.Descendent)
					{
						visualElement2 = visualElement.parent;
						num = i;
					}
					if (--i < 0)
					{
						if (this.OnRuleMatchedElement(matcher, element))
						{
							return true;
						}
					}
					visualElement = visualElement.parent;
				}
			}
			return false;
		}

		protected virtual void Recurse(VisualElement element, int depth, List<RuleMatcher> ruleMatchers)
		{
			int i = 0;
			while (i < element.shadow.childCount)
			{
				VisualElement visualElement = element.shadow[i];
				this.TraverseRecursive(visualElement, depth + 1, ruleMatchers);
				if (visualElement.shadow.parent == element)
				{
					i++;
				}
			}
		}

		protected virtual bool MatchSelectorPart(VisualElement element, StyleSelector selector, StyleSelectorPart part)
		{
			bool flag = true;
			switch (part.type)
			{
			case StyleSelectorType.Wildcard:
				return flag;
			case StyleSelectorType.Type:
				return element.typeName == part.value;
			case StyleSelectorType.Class:
				return element.ClassListContains(part.value);
			case StyleSelectorType.PseudoClass:
			{
				int pseudoStates = (int)element.pseudoStates;
				flag = (selector.pseudoStateMask & pseudoStates) == selector.pseudoStateMask;
				return flag & ((selector.negatedPseudoStateMask & ~pseudoStates) == selector.negatedPseudoStateMask);
			}
			case StyleSelectorType.ID:
				return element.name == part.value;
			}
			flag = false;
			return flag;
		}

		public virtual HierarchyTraversal.MatchResultInfo Match(VisualElement element, ref RuleMatcher matcher, int selectorIndex)
		{
			HierarchyTraversal.MatchResultInfo matchResultInfo;
			if (element == null)
			{
				matchResultInfo = default(HierarchyTraversal.MatchResultInfo);
			}
			else
			{
				bool flag = true;
				StyleSelector styleSelector = matcher.complexSelector.selectors[selectorIndex];
				int num = styleSelector.parts.Length;
				int num2 = 0;
				int num3 = 0;
				bool flag2 = true;
				for (int i = 0; i < num; i++)
				{
					bool flag3 = this.MatchSelectorPart(element, styleSelector, styleSelector.parts[i]);
					if (!flag3)
					{
						if (styleSelector.parts[i].type == StyleSelectorType.PseudoClass)
						{
							num2 |= styleSelector.pseudoStateMask;
							num3 |= styleSelector.negatedPseudoStateMask;
						}
						else
						{
							flag2 = false;
						}
					}
					else if (styleSelector.parts[i].type == StyleSelectorType.PseudoClass)
					{
						num3 |= styleSelector.pseudoStateMask;
						num2 |= styleSelector.negatedPseudoStateMask;
					}
					flag = flag && flag3;
				}
				HierarchyTraversal.MatchResultInfo matchResultInfo2 = new HierarchyTraversal.MatchResultInfo
				{
					success = flag
				};
				if (flag || flag2)
				{
					matchResultInfo2.triggerPseudoMask = (PseudoStates)num2;
					matchResultInfo2.dependencyPseudoMask = (PseudoStates)num3;
				}
				matchResultInfo = matchResultInfo2;
			}
			return matchResultInfo;
		}

		private List<RuleMatcher> m_ruleMatchers = new List<RuleMatcher>();

		public struct MatchResultInfo
		{
			public bool success;

			public PseudoStates triggerPseudoMask;

			public PseudoStates dependencyPseudoMask;
		}
	}
}
