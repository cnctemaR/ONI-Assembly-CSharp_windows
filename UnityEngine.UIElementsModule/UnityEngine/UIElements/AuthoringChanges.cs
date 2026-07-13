using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal class AuthoringChanges
	{
		public HashSet<VisualElement> addedOrMovedElements { get; } = new HashSet<VisualElement>();

		public HashSet<VisualElement> removedFromPanel { get; } = new HashSet<VisualElement>();

		public HashSet<VisualElement> styleChanged { get; } = new HashSet<VisualElement>();

		public HashSet<VisualElement> stylingContextChanged { get; } = new HashSet<VisualElement>();

		public HashSet<VisualElement> bindingContextChanged { get; } = new HashSet<VisualElement>();

		public bool ContainsChanges()
		{
			return this.addedOrMovedElements.Count > 0 || this.removedFromPanel.Count > 0 || this.styleChanged.Count > 0 || this.stylingContextChanged.Count > 0 || this.bindingContextChanged.Count > 0;
		}

		public void Clear()
		{
			this.addedOrMovedElements.Clear();
			this.removedFromPanel.Clear();
			this.styleChanged.Clear();
			this.stylingContextChanged.Clear();
			this.bindingContextChanged.Clear();
		}
	}
}
