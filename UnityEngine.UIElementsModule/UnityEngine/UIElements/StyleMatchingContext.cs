using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal class StyleMatchingContext
	{
		public StyleMatchingContext(Action<VisualElement, MatchResultInfo> processResult)
		{
			this.styleSheetStack = new List<StyleSheet>();
			this.variableContext = StyleVariableContext.none;
			this.currentElement = null;
			this.processResult = processResult;
		}

		public List<StyleSheet> styleSheetStack;

		public StyleVariableContext variableContext;

		public VisualElement currentElement;

		public Action<VisualElement, MatchResultInfo> processResult;
	}
}
