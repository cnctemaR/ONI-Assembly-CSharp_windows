using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public readonly struct BindingInfo
	{
		public VisualElement targetElement { get; }

		public BindingId bindingId { get; }

		public Binding binding { get; }

		private BindingInfo(VisualElement targetElement, in BindingId bindingId, Binding binding)
		{
			this.targetElement = targetElement;
			this.bindingId = bindingId;
			this.binding = binding;
		}

		internal static BindingInfo FromRequest(VisualElement target, in PropertyPath targetPath, Binding binding)
		{
			BindingId bindingId = in targetPath;
			return new BindingInfo(target, in bindingId, binding);
		}

		internal static BindingInfo FromBindingData(in DataBindingManager.BindingData bindingData)
		{
			return new BindingInfo(bindingData.target.element, in bindingData.target.bindingId, bindingData.binding);
		}
	}
}
