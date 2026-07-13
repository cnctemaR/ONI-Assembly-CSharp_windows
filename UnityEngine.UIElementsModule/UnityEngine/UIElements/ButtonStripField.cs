using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	internal class ButtonStripField : BaseField<int>
	{
		public void AddButton(string text, string name = "")
		{
			Button button = this.CreateButton(name);
			button.text = text;
			base.Add(button);
		}

		public void AddButton(Background icon, string name = "")
		{
			Button button = this.CreateButton(name);
			VisualElement visualElement = new VisualElement();
			visualElement.AddToClassList("unity-button-strip-field__button-icon");
			visualElement.style.backgroundImage = icon;
			button.Add(visualElement);
			base.Add(button);
		}

		private Button CreateButton(string name)
		{
			Button button = new Button
			{
				name = name
			};
			button.AddToClassList("unity-button-strip-field__button");
			button.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnButtonDetachFromPanel), TrickleDown.NoTrickleDown);
			button.clicked += delegate
			{
				this.value = this.m_Buttons.IndexOf(button);
			};
			this.m_Buttons.Add(button);
			base.Add(button);
			this.RefreshButtonsStyling();
			return button;
		}

		private void OnButtonDetachFromPanel(DetachFromPanelEvent evt)
		{
			VisualElement visualElement = evt.currentTarget as VisualElement;
			ButtonStripField buttonStripField;
			bool flag;
			if (visualElement != null)
			{
				buttonStripField = visualElement.parent as ButtonStripField;
				flag = buttonStripField != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				buttonStripField.RefreshButtonsStyling();
				buttonStripField.EnsureValueIsValid();
			}
		}

		private void RefreshButtonsStyling()
		{
			for (int i = 0; i < this.m_Buttons.Count; i++)
			{
				Button button = this.m_Buttons[i];
				bool flag = this.m_Buttons.Count == 1;
				bool flag2 = i == 0;
				bool flag3 = i == this.m_Buttons.Count - 1;
				button.EnableInClassList("unity-button-strip-field__button--alone", flag);
				button.EnableInClassList("unity-button-strip-field__button--left", !flag && flag2);
				button.EnableInClassList("unity-button-strip-field__button--right", !flag && flag3);
				button.EnableInClassList("unity-button-strip-field__button--middle", !flag && !flag2 && !flag3);
			}
		}

		public ButtonStripField()
			: base(null)
		{
		}

		public ButtonStripField(string label)
			: base(label)
		{
			base.AddToClassList("unity-button-strip-field");
		}

		public override void SetValueWithoutNotify(int newValue)
		{
			newValue = Mathf.Clamp(newValue, 0, this.m_Buttons.Count - 1);
			base.SetValueWithoutNotify(newValue);
			this.RefreshButtonsState();
		}

		private void EnsureValueIsValid()
		{
			this.SetValueWithoutNotify(Mathf.Clamp(this.value, 0, this.m_Buttons.Count - 1));
		}

		private void RefreshButtonsState()
		{
			for (int i = 0; i < this.m_Buttons.Count; i++)
			{
				this.m_Buttons[i].SetCheckedPseudoState(i == this.value);
			}
		}

		public const string className = "unity-button-strip-field";

		private const string k_ButtonClass = "unity-button-strip-field__button";

		private const string k_IconClass = "unity-button-strip-field__button-icon";

		private const string k_ButtonLeftClass = "unity-button-strip-field__button--left";

		private const string k_ButtonMiddleClass = "unity-button-strip-field__button--middle";

		private const string k_ButtonRightClass = "unity-button-strip-field__button--right";

		private const string k_ButtonAloneClass = "unity-button-strip-field__button--alone";

		private readonly List<Button> m_Buttons = new List<Button>();

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<int>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<int>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new ButtonStripField();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<ButtonStripField, ButtonStripField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<int>.UxmlTraits
		{
		}
	}
}
