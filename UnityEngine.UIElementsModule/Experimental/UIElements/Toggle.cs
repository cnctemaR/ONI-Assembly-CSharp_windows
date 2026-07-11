using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Toggle : BaseField<bool>
	{
		public Toggle()
		{
			VisualElement visualElement = new VisualElement
			{
				name = "Checkmark",
				pickingMode = PickingMode.Ignore
			};
			base.Add(visualElement);
			this.text = null;
			this.AddManipulator(new Clickable(new Action(this.OnClick)));
		}

		[Obsolete("Use Toggle() with OnValueChanged() instead.", false)]
		public Toggle(Action clickEvent)
			: this()
		{
			this.OnToggle(clickEvent);
		}

		public string text
		{
			get
			{
				return (this.m_Label != null) ? this.m_Label.text : null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (this.m_Label == null)
					{
						this.m_Label = new Label();
						this.m_Label.pickingMode = PickingMode.Ignore;
						base.Add(this.m_Label);
					}
					this.m_Label.text = value;
				}
				else if (this.m_Label != null)
				{
					base.Remove(this.m_Label);
					this.m_Label = null;
				}
			}
		}

		public override void SetValueWithoutNotify(bool newValue)
		{
			if (newValue)
			{
				base.pseudoStates |= PseudoStates.Checked;
			}
			else
			{
				base.pseudoStates &= ~PseudoStates.Checked;
			}
			base.SetValueWithoutNotify(newValue);
		}

		[Obsolete("Use OnValueChanged() instead.", false)]
		public void OnToggle(Action clickEvent)
		{
			if (clickEvent != null && this.m_ClickEvent == null)
			{
				base.OnValueChanged(new EventCallback<ChangeEvent<bool>>(this.InternalOnValueChanged));
			}
			else if (clickEvent == null && this.m_ClickEvent != null)
			{
				base.UnregisterCallback<ChangeEvent<bool>>(new EventCallback<ChangeEvent<bool>>(this.InternalOnValueChanged), TrickleDown.NoTrickleDown);
			}
			this.m_ClickEvent = clickEvent;
		}

		private void InternalOnValueChanged(ChangeEvent<bool> evt)
		{
			if (this.m_ClickEvent != null)
			{
				this.m_ClickEvent();
			}
		}

		private void OnClick()
		{
			this.value = !this.value;
		}

		protected internal override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			KeyDownEvent keyDownEvent = evt as KeyDownEvent;
			char? c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
			if (!(((c == null) ? null : new int?((int)c.Value)) == 10))
			{
				KeyDownEvent keyDownEvent2 = evt as KeyDownEvent;
				char? c2 = ((keyDownEvent2 != null) ? new char?(keyDownEvent2.character) : null);
				if (!(((c2 == null) ? null : new int?((int)c2.Value)) == 32))
				{
					return;
				}
			}
			this.OnClick();
			evt.StopPropagation();
		}

		private Action m_ClickEvent;

		private Label m_Label;

		public new class UxmlFactory : UxmlFactory<Toggle, Toggle.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<bool>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((Toggle)ve).text = this.m_Label.GetValueFromBag(bag, cc);
				((Toggle)ve).SetValueWithoutNotify(this.m_Value.GetValueFromBag(bag, cc));
			}

			private UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
			{
				name = "label"
			};

			private UxmlBoolAttributeDescription m_Value = new UxmlBoolAttributeDescription
			{
				name = "value"
			};
		}
	}
}
