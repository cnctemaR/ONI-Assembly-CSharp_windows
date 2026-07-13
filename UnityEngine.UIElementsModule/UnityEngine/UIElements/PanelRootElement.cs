using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal class PanelRootElement : VisualElement
	{
		public PanelRootElement()
		{
			base.name = VisualElementUtils.GetUniqueName("unity-panel-container");
			base.viewDataKey = "PanelContainer";
			base.pickingMode = PickingMode.Ignore;
			base.SetAsNextParentWithEventInterests();
		}
	}
}
