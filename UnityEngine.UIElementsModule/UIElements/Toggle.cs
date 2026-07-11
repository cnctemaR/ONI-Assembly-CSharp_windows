using System;

namespace UnityEngine.UIElements
{
	public class Toggle : BaseField<bool>
	{
		public Toggle()
			: this(null)
		{
		}

		public Toggle(string label)
			: base(label, null)
		{
			base.AddToClassList(Toggle.ussClassName);
			base.AddToClassList(Toggle.noTextVariantUssClassName);
			base.visualInput.AddToClassList(Toggle.inputUssClassName);
			base.labelElement.AddToClassList(Toggle.labelUssClassName);
			VisualElement visualElement = new VisualElement
			{
				name = "unity-checkmark",
				pickingMode = PickingMode.Ignore
			};
			visualElement.AddToClassList(Toggle.checkmarkUssClassName);
			base.visualInput.Add(visualElement);
			base.visualInput.pickingMode = PickingMode.Position;
			this.text = null;
			this.AddManipulator(new Clickable(new Action<EventBase>(this.OnClickEvent)));
		}

		public string text
		{
			get
			{
				Label label = this.m_Label;
				return (label != null) ? label.text : null;
			}
			set
			{
				bool flag = !string.IsNullOrEmpty(value);
				if (flag)
				{
					bool flag2 = this.m_Label == null;
					if (flag2)
					{
						this.m_Label = new Label
						{
							pickingMode = PickingMode.Ignore
						};
						this.m_Label.AddToClassList(Toggle.textUssClassName);
						base.RemoveFromClassList(Toggle.noTextVariantUssClassName);
						base.visualInput.Add(this.m_Label);
					}
					this.m_Label.text = value;
				}
				else
				{
					bool flag3 = this.m_Label != null;
					if (flag3)
					{
						base.Remove(this.m_Label);
						base.AddToClassList(Toggle.noTextVariantUssClassName);
						this.m_Label = null;
					}
				}
			}
		}

		public override void SetValueWithoutNotify(bool newValue)
		{
			if (newValue)
			{
				base.visualInput.pseudoStates |= PseudoStates.Checked;
				base.pseudoStates |= PseudoStates.Checked;
			}
			else
			{
				base.visualInput.pseudoStates &= ~PseudoStates.Checked;
				base.pseudoStates &= ~PseudoStates.Checked;
			}
			base.SetValueWithoutNotify(newValue);
		}

		private void OnClickEvent(EventBase evt)
		{
			MouseUpEvent mouseUpEvent = evt as MouseUpEvent;
			bool flag = mouseUpEvent != null && mouseUpEvent.button == 0;
			if (flag)
			{
				MouseUpEvent mouseUpEvent2 = (MouseUpEvent)evt;
				bool flag2 = base.visualInput.ContainsPoint(base.visualInput.WorldToLocal(mouseUpEvent2.mousePosition));
				if (flag2)
				{
					this.OnClick();
				}
			}
		}

		private void OnClick()
		{
			this.value = !this.value;
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			bool flag = evt == null;
			if (!flag)
			{
				KeyDownEvent keyDownEvent = evt as KeyDownEvent;
				bool flag2;
				if (keyDownEvent == null || keyDownEvent.keyCode != KeyCode.KeypadEnter)
				{
					KeyDownEvent keyDownEvent2 = evt as KeyDownEvent;
					flag2 = keyDownEvent2 != null && keyDownEvent2.keyCode == KeyCode.Return;
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					this.OnClick();
					evt.StopPropagation();
				}
			}
		}

		public new static readonly string ussClassName = "unity-toggle";

		public new static readonly string labelUssClassName = Toggle.ussClassName + "__label";

		public new static readonly string inputUssClassName = Toggle.ussClassName + "__input";

		public static readonly string noTextVariantUssClassName = Toggle.ussClassName + "--no-text";

		public static readonly string checkmarkUssClassName = Toggle.ussClassName + "__checkmark";

		public static readonly string textUssClassName = Toggle.ussClassName + "__text";

		private Label m_Label;

		public new class UxmlFactory : UxmlFactory<Toggle, Toggle.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription>
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((Toggle)ve).text = this.m_Text.GetValueFromBag(bag, cc);
			}

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};
		}
	}
}
