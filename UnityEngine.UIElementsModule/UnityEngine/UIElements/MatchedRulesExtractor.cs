using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class MatchedRulesExtractor
	{
		public IEnumerable<MatchedRule> GetMatchedRules()
		{
			return this.selectedElementRules;
		}

		public Func<StyleSheet, string> getStyleSheetPath
		{
			get
			{
				return this.m_GetStyleSheetPath ?? MatchedRulesExtractor.k_defaultGetPath;
			}
			set
			{
				this.m_GetStyleSheetPath = value;
			}
		}

		public MatchedRulesExtractor(Func<StyleSheet, string> getAssetPath)
		{
			this.getStyleSheetPath = getAssetPath;
		}

		private void SetupParents(VisualElement target, StyleMatchingContext matchingContext)
		{
			MatchedRulesExtractor.<>c__DisplayClass10_0 CS$<>8__locals1;
			CS$<>8__locals1.matchingContext = matchingContext;
			bool flag = target.hierarchy.parent != null;
			if (flag)
			{
				this.SetupParents(target.hierarchy.parent, CS$<>8__locals1.matchingContext);
			}
			CS$<>8__locals1.matchingContext.ancestorFilter.PushElement(target);
			bool flag2 = target.styleSheetList == null;
			if (!flag2)
			{
				foreach (StyleSheet styleSheet in target.styleSheetList)
				{
					bool flag3 = styleSheet == null;
					if (!flag3)
					{
						MatchedRulesExtractor.<>c__DisplayClass10_1 CS$<>8__locals2;
						CS$<>8__locals2.name = this.getStyleSheetPath(styleSheet);
						bool flag4 = string.IsNullOrEmpty(CS$<>8__locals2.name) || styleSheet.isDefaultStyleSheet;
						if (flag4)
						{
							CS$<>8__locals2.name = styleSheet.name;
						}
						MatchedRulesExtractor.<SetupParents>g__RecursivePrintStyleSheetNames|10_0(styleSheet, ref CS$<>8__locals1, ref CS$<>8__locals2);
						this.selectedElementStylesheets.Add(CS$<>8__locals2.name);
						CS$<>8__locals1.matchingContext.AddStyleSheet(styleSheet);
					}
				}
			}
		}

		public void FindMatchingRules(VisualElement target)
		{
			StyleMatchingContext styleMatchingContext = new StyleMatchingContext(delegate(VisualElement element, MatchResultInfo info)
			{
			})
			{
				currentElement = target
			};
			this.SetupParents(target, styleMatchingContext);
			this.matchRecords.Clear();
			StyleSelectorHelper.FindMatches(styleMatchingContext, this.matchRecords);
			this.matchRecords.Sort(new Comparison<SelectorMatchRecord>(SelectorMatchRecord.Compare));
			foreach (SelectorMatchRecord selectorMatchRecord in this.matchRecords)
			{
				this.selectedElementRules.Add(new MatchedRule(selectorMatchRecord, this.getStyleSheetPath(selectorMatchRecord.sheet)));
			}
		}

		public void Clear()
		{
			this.selectedElementRules.Clear();
			this.selectedElementStylesheets.Clear();
			this.matchRecords.Clear();
		}

		[CompilerGenerated]
		internal static void <SetupParents>g__RecursivePrintStyleSheetNames|10_0(StyleSheet importedSheet, ref MatchedRulesExtractor.<>c__DisplayClass10_0 A_1, ref MatchedRulesExtractor.<>c__DisplayClass10_1 A_2)
		{
			for (int i = 0; i < importedSheet.imports.Length; i++)
			{
				StyleSheet styleSheet = importedSheet.imports[i].styleSheet;
				bool flag = styleSheet != null;
				if (flag)
				{
					A_2.name = A_2.name + "\n(" + styleSheet.name + ")";
					A_1.matchingContext.AddStyleSheet(styleSheet);
					MatchedRulesExtractor.<SetupParents>g__RecursivePrintStyleSheetNames|10_0(styleSheet, ref A_1, ref A_2);
				}
			}
		}

		private static readonly Func<StyleSheet, string> k_defaultGetPath = (StyleSheet ss) => ss.name;

		private Func<StyleSheet, string> m_GetStyleSheetPath;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal HashSet<MatchedRule> selectedElementRules = new HashSet<MatchedRule>(MatchedRule.lineNumberFullPathComparer);

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal HashSet<string> selectedElementStylesheets = new HashSet<string>();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal List<SelectorMatchRecord> matchRecords = new List<SelectorMatchRecord>();
	}
}
