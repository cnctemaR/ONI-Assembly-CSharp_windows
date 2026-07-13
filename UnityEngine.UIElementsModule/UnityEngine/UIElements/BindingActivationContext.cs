using System;

namespace UnityEngine.UIElements
{
	public readonly struct BindingActivationContext
	{
		public VisualElement targetElement
		{
			get
			{
				return this.m_TargetElement;
			}
		}

		public BindingId bindingId
		{
			get
			{
				return this.m_BindingId;
			}
		}

		internal BindingActivationContext(VisualElement element, in BindingId property)
		{
			this.m_TargetElement = element;
			this.m_BindingId = property;
		}

		private readonly VisualElement m_TargetElement;

		private readonly BindingId m_BindingId;
	}
}
