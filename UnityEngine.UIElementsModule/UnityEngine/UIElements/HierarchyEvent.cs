using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal delegate void HierarchyEvent(VisualElement ve, HierarchyChangeType changeType, IReadOnlyList<VisualElement> additionalContext = null);
}
