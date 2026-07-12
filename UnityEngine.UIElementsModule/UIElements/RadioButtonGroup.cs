using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class RadioButtonGroup : BaseField<int>, IGroupBox
	{
		public IEnumerable<string> choices
		{
			get
			{
				foreach (RadioButton radioButton in this.m_RadioButtons)
				{
					yield return radioButton.text;
					radioButton = null;
				}
				List<RadioButton>.Enumerator enumerator = default(List<RadioButton>.Enumerator);
				yield break;
				yield break;
			}
			set
			{
				bool flag = !value.HasValues();
				if (flag)
				{
					this.m_RadioButtonContainer.Clear();
					bool flag2 = base.panel != null;
					if (!flag2)
					{
						foreach (RadioButton radioButton in this.m_RadioButtons)
						{
							radioButton.UnregisterValueChangedCallback<bool>(this.m_RadioButtonValueChangedCallback);
						}
						this.m_RadioButtons.Clear();
					}
				}
				else
				{
					int num = 0;
					foreach (string text in value)
					{
						bool flag3 = num < this.m_RadioButtons.Count;
						if (flag3)
						{
							this.m_RadioButtons[num].text = text;
							this.m_RadioButtonContainer.Insert(num, this.m_RadioButtons[num]);
						}
						else
						{
							RadioButton radioButton2 = new RadioButton
							{
								text = text
							};
							radioButton2.RegisterValueChangedCallback<bool>(this.m_RadioButtonValueChangedCallback);
							this.m_RadioButtons.Add(radioButton2);
							this.m_RadioButtonContainer.Add(radioButton2);
						}
						num++;
					}
					int num2 = this.m_RadioButtons.Count - 1;
					for (int i = num2; i >= num; i--)
					{
						this.m_RadioButtons[i].RemoveFromHierarchy();
					}
					this.UpdateRadioButtons();
				}
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_RadioButtonContainer ?? this;
			}
		}

		public RadioButtonGroup()
			: this(null, null)
		{
		}

		public RadioButtonGroup(string label, List<string> radioButtonChoices = null)
			: base(label, null)
		{
			base.AddToClassList(RadioButtonGroup.ussClassName);
			VisualElement visualInput = base.visualInput;
			VisualElement visualElement = new VisualElement();
			visualElement.name = RadioButtonGroup.containerUssClassName;
			VisualElement visualElement2 = visualElement;
			this.m_RadioButtonContainer = visualElement;
			visualInput.Add(visualElement2);
			this.m_RadioButtonContainer.AddToClassList(RadioButtonGroup.containerUssClassName);
			this.m_RadioButtonValueChangedCallback = new EventCallback<ChangeEvent<bool>>(this.RadioButtonValueChangedCallback);
			this.choices = radioButtonChoices;
			this.value = -1;
			base.visualInput.focusable = false;
			base.delegatesFocus = true;
		}

		private void RadioButtonValueChangedCallback(ChangeEvent<bool> evt)
		{
			bool newValue = evt.newValue;
			if (newValue)
			{
				this.value = this.m_RadioButtons.IndexOf(evt.target as RadioButton);
				evt.StopPropagation();
			}
		}

		public override void SetValueWithoutNotify(int newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.UpdateRadioButtons();
		}

		private void UpdateRadioButtons()
		{
			bool flag = this.value >= 0 && this.value < this.m_RadioButtons.Count;
			if (flag)
			{
				this.m_RadioButtons[this.value].value = true;
			}
			else
			{
				foreach (RadioButton radioButton in this.m_RadioButtons)
				{
					radioButton.value = false;
				}
			}
		}

		void IGroupBox.OnOptionAdded(IGroupBoxOption option)
		{
			RadioButton radioButton = option as RadioButton;
			bool flag = radioButton == null;
			if (flag)
			{
				throw new ArgumentException("[UI Toolkit] Internal group box error. Expected a radio button element. Please report this using Help -> Report a bug...");
			}
			bool flag2 = this.m_RadioButtons.Contains(radioButton);
			if (!flag2)
			{
				radioButton.RegisterValueChangedCallback<bool>(this.m_RadioButtonValueChangedCallback);
				int num = this.m_RadioButtonContainer.IndexOf(radioButton);
				bool flag3 = num < 0 || num > this.m_RadioButtons.Count;
				if (flag3)
				{
					this.m_RadioButtons.Add(radioButton);
					this.m_RadioButtonContainer.Add(radioButton);
				}
				else
				{
					this.m_RadioButtons.Insert(num, radioButton);
				}
			}
		}

		void IGroupBox.OnOptionRemoved(IGroupBoxOption option)
		{
			RadioButton radioButton = option as RadioButton;
			bool flag = radioButton == null;
			if (flag)
			{
				throw new ArgumentException("[UI Toolkit] Internal group box error. Expected a radio button element. Please report this using Help -> Report a bug...");
			}
			int num = this.m_RadioButtons.IndexOf(radioButton);
			radioButton.UnregisterValueChangedCallback<bool>(this.m_RadioButtonValueChangedCallback);
			this.m_RadioButtons.Remove(radioButton);
			bool flag2 = this.value == num;
			if (flag2)
			{
				this.value = -1;
			}
		}

		public new static readonly string ussClassName = "unity-radio-button-group";

		public static readonly string containerUssClassName = RadioButtonGroup.ussClassName + "__container";

		private List<RadioButton> m_RadioButtons = new List<RadioButton>();

		private EventCallback<ChangeEvent<bool>> m_RadioButtonValueChangedCallback;

		private VisualElement m_RadioButtonContainer;

		public new class UxmlFactory : UxmlFactory<RadioButtonGroup, RadioButtonGroup.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseFieldTraits<int, UxmlIntAttributeDescription>
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				RadioButtonGroup radioButtonGroup = (RadioButtonGroup)ve;
				radioButtonGroup.choices = BaseField<int>.UxmlTraits.ParseChoiceList(this.m_Choices.GetValueFromBag(bag, cc));
			}

			private UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription
			{
				name = "choices"
			};
		}
	}
}
