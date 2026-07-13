using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	[Obsolete("TextValueFieldTraits<TValueType, TValueUxmlAttributeType> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class TextValueFieldTraits<TValueType, TValueUxmlAttributeType> : BaseFieldTraits<TValueType, TValueUxmlAttributeType> where TValueUxmlAttributeType : TypedUxmlAttributeDescription<TValueType>, new()
	{
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)ve;
			bool flag = textInputBaseField != null;
			if (flag)
			{
				textInputBaseField.textEdition.placeholder = this.m_PlaceholderText.GetValueFromBag(bag, cc);
				textInputBaseField.textEdition.hidePlaceholderOnFocus = this.m_HidePlaceholderOnFocus.GetValueFromBag(bag, cc);
				textInputBaseField.isReadOnly = this.m_IsReadOnly.GetValueFromBag(bag, cc);
				textInputBaseField.isDelayed = this.m_IsDelayed.GetValueFromBag(bag, cc);
			}
		}

		private UxmlStringAttributeDescription m_PlaceholderText = new UxmlStringAttributeDescription
		{
			name = "placeholder-text"
		};

		private UxmlBoolAttributeDescription m_HidePlaceholderOnFocus = new UxmlBoolAttributeDescription
		{
			name = "hide-placeholder-on-focus"
		};

		private UxmlBoolAttributeDescription m_IsReadOnly = new UxmlBoolAttributeDescription
		{
			name = "readonly"
		};

		private UxmlBoolAttributeDescription m_IsDelayed = new UxmlBoolAttributeDescription
		{
			name = "is-delayed"
		};
	}
}
