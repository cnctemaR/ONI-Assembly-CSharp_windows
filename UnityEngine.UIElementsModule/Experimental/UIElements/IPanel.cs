using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public interface IPanel : IDisposable
	{
		VisualElement visualTree { get; }

		EventDispatcher dispatcher { get; }

		ContextType contextType { get; }

		FocusController focusController { get; }

		VisualElement Pick(Vector2 point);

		VisualElement LoadTemplate(string path, Dictionary<string, VisualElement> slots = null);

		VisualElement PickAll(Vector2 point, List<VisualElement> picked);
	}
}
