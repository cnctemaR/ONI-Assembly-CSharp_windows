using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	internal class StyleMatchingContext
	{
		public StyleMatchingContext(Action<VisualElement, MatchResultInfo> processResult)
		{
			this.styleSheetStack = new List<StyleSheet>();
			this.currentElement = null;
			this.processResult = processResult;
		}

		public List<StyleSheet> styleSheetStack;

		public VisualElement currentElement;

		public Action<VisualElement, MatchResultInfo> processResult;
	}
}
