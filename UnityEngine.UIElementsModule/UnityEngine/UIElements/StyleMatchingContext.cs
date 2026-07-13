using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class StyleMatchingContext
	{
		public int styleSheetCount
		{
			get
			{
				return this.m_StyleSheetStack.Count;
			}
		}

		public StyleMatchingContext(Action<VisualElement, MatchResultInfo> processResult)
		{
			this.m_StyleSheetStack = new List<StyleSheet>();
			this.variableContext = StyleVariableContext.none;
			this.currentElement = null;
			this.processResult = processResult;
		}

		public void AddStyleSheet(StyleSheet sheet)
		{
			bool flag = sheet == null;
			if (!flag)
			{
				this.m_StyleSheetStack.Add(sheet);
			}
		}

		public void RemoveStyleSheetRange(int index, int count)
		{
			this.m_StyleSheetStack.RemoveRange(index, count);
		}

		public StyleSheet GetStyleSheetAt(int index)
		{
			return this.m_StyleSheetStack[index];
		}

		private List<StyleSheet> m_StyleSheetStack;

		public StyleVariableContext variableContext;

		public VisualElement currentElement;

		public Action<VisualElement, MatchResultInfo> processResult;

		public AncestorFilter ancestorFilter = new AncestorFilter();
	}
}
