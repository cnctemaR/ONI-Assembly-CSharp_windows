using System;

namespace UnityEngine.UIElements
{
	public class BindableElement : VisualElement, IBindable
	{
		public IBinding binding { get; set; }

		public string bindingPath { get; set; }

		public new class UxmlFactory : UxmlFactory<BindableElement, BindableElement.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public UxmlTraits()
			{
				this.m_PropertyPath = new UxmlStringAttributeDescription
				{
					name = "binding-path"
				};
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				string valueFromBag = this.m_PropertyPath.GetValueFromBag(bag, cc);
				IBindable bindable = ve as IBindable;
				bool flag = bindable != null;
				if (flag)
				{
					bindable.bindingPath = (string.IsNullOrEmpty(valueFromBag) ? string.Empty : valueFromBag);
				}
			}

			private UxmlStringAttributeDescription m_PropertyPath;
		}
	}
}
