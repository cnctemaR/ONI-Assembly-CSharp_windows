using System;

namespace UnityEngine.UIElements
{
	internal readonly struct BindingTarget
	{
		public BindingTarget(VisualElement element, in BindingId bindingId)
		{
			this.element = element;
			this.bindingId = bindingId;
		}

		public readonly VisualElement element;

		public readonly BindingId bindingId;
	}
}
