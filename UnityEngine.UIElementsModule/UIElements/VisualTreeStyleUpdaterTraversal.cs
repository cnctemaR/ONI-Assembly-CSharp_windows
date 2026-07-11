using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal class VisualTreeStyleUpdaterTraversal : HierarchyTraversal
	{
		private float currentPixelsPerPoint { get; set; } = 1f;

		public void PrepareTraversal(float pixelsPerPoint)
		{
			this.currentPixelsPerPoint = pixelsPerPoint;
			this.m_StyleMatchingContext.inheritedStyle = VisualTreeStyleUpdaterTraversal.s_DefaultInheritedStyles;
		}

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
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.hierarchy[i];
				bool flag = this.m_UpdateList.Add(visualElement);
				bool flag2 = flag;
				if (flag2)
				{
					this.PropagateToChildren(visualElement);
				}
			}
		}

		private void PropagateToParents(VisualElement ve)
		{
			for (VisualElement visualElement = ve.hierarchy.parent; visualElement != null; visualElement = visualElement.hierarchy.parent)
			{
				bool flag = !this.m_ParentList.Add(visualElement);
				if (flag)
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
			bool flag = this.ShouldSkipElement(element);
			if (!flag)
			{
				bool flag2 = this.m_UpdateList.Contains(element);
				bool flag3 = flag2;
				if (flag3)
				{
					element.triggerPseudoMask = (PseudoStates)0;
					element.dependencyPseudoMask = (PseudoStates)0;
				}
				int count = this.m_StyleMatchingContext.styleSheetStack.Count;
				bool flag4 = element.styleSheetList != null;
				if (flag4)
				{
					for (int i = 0; i < element.styleSheetList.Count; i++)
					{
						StyleSheet styleSheet = element.styleSheetList[i];
						this.m_StyleMatchingContext.styleSheetStack.Add(styleSheet);
					}
				}
				int customPropertiesCount = element.specifiedStyle.customPropertiesCount;
				InheritedStylesData inheritedStyle = this.m_StyleMatchingContext.inheritedStyle;
				bool flag5 = flag2;
				if (flag5)
				{
					this.m_StyleMatchingContext.currentElement = element;
					element.specifiedStyle.dpiScaling = element.scaledPixelsPerPoint;
					StyleSelectorHelper.FindMatches(this.m_StyleMatchingContext, this.m_TempMatchResults);
					this.ProcessMatchedRules(element, this.m_TempMatchResults);
					this.ResolveInheritance(element);
					this.m_StyleMatchingContext.currentElement = null;
					this.m_TempMatchResults.Clear();
				}
				else
				{
					this.m_StyleMatchingContext.inheritedStyle = element.propagatedStyle;
					this.m_StyleMatchingContext.variableContext = element.variableContext;
				}
				bool flag6 = flag2 && (customPropertiesCount > 0 || element.specifiedStyle.customPropertiesCount > 0);
				if (flag6)
				{
					using (CustomStyleResolvedEvent pooled = EventBase<CustomStyleResolvedEvent>.GetPooled())
					{
						pooled.target = element;
						element.SendEvent(pooled);
					}
				}
				base.Recurse(element, depth);
				this.m_StyleMatchingContext.inheritedStyle = inheritedStyle;
				bool flag7 = this.m_StyleMatchingContext.styleSheetStack.Count > count;
				if (flag7)
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
			matchingSelectors.Sort((SelectorMatchRecord a, SelectorMatchRecord b) => SelectorMatchRecord.Compare(a, b));
			long num = (long)element.fullTypeName.GetHashCode();
			num = (num * 397L) ^ (long)this.currentPixelsPerPoint.GetHashCode();
			int variableHash = this.m_StyleMatchingContext.variableContext.GetVariableHash();
			int num2 = 0;
			foreach (SelectorMatchRecord selectorMatchRecord in matchingSelectors)
			{
				num2 += selectorMatchRecord.complexSelector.rule.customPropertiesCount;
			}
			bool flag = num2 > 0;
			if (flag)
			{
				this.m_ProcessVarContext.AddInitialRange(this.m_StyleMatchingContext.variableContext);
			}
			foreach (SelectorMatchRecord selectorMatchRecord2 in matchingSelectors)
			{
				StyleRule rule = selectorMatchRecord2.complexSelector.rule;
				int specificity = selectorMatchRecord2.complexSelector.specificity;
				num = (num * 397L) ^ (long)rule.GetHashCode();
				num = (num * 397L) ^ (long)specificity;
				bool flag2 = rule.customPropertiesCount > 0;
				if (flag2)
				{
					this.ProcessMatchedVariables(selectorMatchRecord2.sheet, rule);
				}
			}
			int num3 = variableHash;
			bool flag3 = num2 > 0;
			if (flag3)
			{
				num3 = this.m_ProcessVarContext.GetVariableHash();
			}
			num = (num * 397L) ^ (long)num3;
			bool flag4 = variableHash != num3;
			if (flag4)
			{
				StyleVariableContext styleVariableContext;
				bool flag5 = !StyleCache.TryGetValue(num3, out styleVariableContext);
				if (flag5)
				{
					styleVariableContext = new StyleVariableContext(this.m_ProcessVarContext);
					StyleCache.SetValue(num3, styleVariableContext);
				}
				this.m_StyleMatchingContext.variableContext = styleVariableContext;
			}
			element.variableContext = this.m_StyleMatchingContext.variableContext;
			this.m_ProcessVarContext.Clear();
			VisualElementStylesData visualElementStylesData;
			bool flag6 = StyleCache.TryGetValue(num, out visualElementStylesData);
			if (flag6)
			{
				element.SetSharedStyles(visualElementStylesData);
			}
			else
			{
				visualElementStylesData = new VisualElementStylesData(true);
				float dpiScaling = element.specifiedStyle.dpiScaling;
				foreach (SelectorMatchRecord selectorMatchRecord3 in matchingSelectors)
				{
					this.m_StylePropertyReader.SetContext(selectorMatchRecord3.sheet, selectorMatchRecord3.complexSelector, this.m_StyleMatchingContext.variableContext, dpiScaling);
					visualElementStylesData.ApplyProperties(this.m_StylePropertyReader, this.m_StyleMatchingContext.inheritedStyle);
				}
				visualElementStylesData.ApplyLayoutValues();
				StyleCache.SetValue(num, visualElementStylesData);
				element.SetSharedStyles(visualElementStylesData);
			}
		}

		private void ProcessMatchedVariables(StyleSheet sheet, StyleRule rule)
		{
			foreach (StyleProperty styleProperty in rule.properties)
			{
				bool isCustomProperty = styleProperty.isCustomProperty;
				if (isCustomProperty)
				{
					StyleVariable styleVariable = new StyleVariable(styleProperty.name, sheet, styleProperty.values);
					this.m_ProcessVarContext.Add(styleVariable);
				}
			}
		}

		private void ResolveInheritance(VisualElement element)
		{
			VisualElementStylesData specifiedStyle = element.specifiedStyle;
			InheritedStylesData inheritedStyle = this.m_StyleMatchingContext.inheritedStyle;
			element.inheritedStyle = inheritedStyle;
			this.m_ResolveInheritData.CopyFrom(inheritedStyle);
			bool flag = specifiedStyle.color.specificity != 0;
			if (flag)
			{
				this.m_ResolveInheritData.color = specifiedStyle.color;
			}
			bool flag2 = specifiedStyle.unityFont.specificity != 0;
			if (flag2)
			{
				this.m_ResolveInheritData.font = specifiedStyle.unityFont;
			}
			bool flag3 = specifiedStyle.fontSize.specificity != 0;
			if (flag3)
			{
				this.m_ResolveInheritData.fontSize = new StyleLength(ComputedStyle.CalculatePixelFontSize(element));
				this.m_ResolveInheritData.fontSize.specificity = specifiedStyle.fontSize.specificity;
			}
			bool flag4 = specifiedStyle.unityFontStyleAndWeight.specificity != 0;
			if (flag4)
			{
				this.m_ResolveInheritData.unityFontStyle = specifiedStyle.unityFontStyleAndWeight;
			}
			bool flag5 = specifiedStyle.unityTextAlign.specificity != 0;
			if (flag5)
			{
				this.m_ResolveInheritData.unityTextAlign = specifiedStyle.unityTextAlign;
			}
			bool flag6 = specifiedStyle.visibility.specificity != 0;
			if (flag6)
			{
				this.m_ResolveInheritData.visibility = specifiedStyle.visibility;
			}
			bool flag7 = specifiedStyle.whiteSpace.specificity != 0;
			if (flag7)
			{
				this.m_ResolveInheritData.whiteSpace = specifiedStyle.whiteSpace;
			}
			bool flag8 = !this.m_ResolveInheritData.Equals(inheritedStyle);
			if (flag8)
			{
				InheritedStylesData inheritedStylesData = null;
				int hashCode = this.m_ResolveInheritData.GetHashCode();
				bool flag9 = !StyleCache.TryGetValue(hashCode, out inheritedStylesData);
				if (flag9)
				{
					inheritedStylesData = new InheritedStylesData(this.m_ResolveInheritData);
					StyleCache.SetValue(hashCode, inheritedStylesData);
				}
				this.m_StyleMatchingContext.inheritedStyle = inheritedStylesData;
			}
			element.propagatedStyle = this.m_StyleMatchingContext.inheritedStyle;
		}

		private static readonly InheritedStylesData s_DefaultInheritedStyles = new InheritedStylesData();

		private InheritedStylesData m_ResolveInheritData = new InheritedStylesData();

		private StyleVariableContext m_ProcessVarContext = new StyleVariableContext();

		private HashSet<VisualElement> m_UpdateList = new HashSet<VisualElement>();

		private HashSet<VisualElement> m_ParentList = new HashSet<VisualElement>();

		private List<SelectorMatchRecord> m_TempMatchResults = new List<SelectorMatchRecord>();

		private StyleMatchingContext m_StyleMatchingContext = new StyleMatchingContext(new Action<VisualElement, MatchResultInfo>(VisualTreeStyleUpdaterTraversal.OnProcessMatchResult));

		private StylePropertyReader m_StylePropertyReader = new StylePropertyReader();
	}
}
