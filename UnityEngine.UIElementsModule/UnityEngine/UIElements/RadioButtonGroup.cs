using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Internal;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	public class RadioButtonGroup : BaseField<int>, IGroupBox
	{
		[CreateProperty]
		public IEnumerable<string> choices
		{
			get
			{
				RadioButtonGroup.<get_choices>d__17 <get_choices>d__ = new RadioButtonGroup.<get_choices>d__17(-2);
				<get_choices>d__.<>4__this = this;
				return <get_choices>d__;
			}
			set
			{
				bool flag = (value != null && RadioButtonGroup.<set_choices>g__AreListEqual|18_0(this.m_Choices, value)) || (value == null && this.m_Choices.Count == 0);
				if (!flag)
				{
					this.m_Choices.Clear();
					bool flag2 = value != null;
					if (flag2)
					{
						this.m_Choices.AddRange(value);
					}
					this.RebuildRadioButtonsFromChoices();
					base.NotifyPropertyChanged(in RadioButtonGroup.choicesProperty);
				}
			}
		}

		private void RebuildRadioButtonsFromChoices()
		{
			bool flag = this.m_Choices.Count == 0;
			if (flag)
			{
				this.m_ChoiceRadioButtonContainer.Clear();
			}
			else
			{
				int num = 0;
				foreach (string text in this.m_Choices)
				{
					bool flag2 = num < this.m_ChoiceRadioButtonContainer.childCount;
					if (flag2)
					{
						(this.m_ChoiceRadioButtonContainer[num] as RadioButton).text = text;
						this.ScheduleRadioButtons();
					}
					else
					{
						RadioButton radioButton = new RadioButton
						{
							text = text
						};
						this.m_ChoiceRadioButtonContainer.Add(radioButton);
					}
					num++;
				}
				int num2 = this.m_ChoiceRadioButtonContainer.childCount - 1;
				for (int i = num2; i >= num; i--)
				{
					this.m_ChoiceRadioButtonContainer[i].RemoveFromHierarchy();
				}
			}
		}

		internal List<string> choicesList
		{
			get
			{
				return this.m_Choices;
			}
			set
			{
				this.choices = value;
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer ?? this;
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
			visualElement.name = RadioButtonGroup.choicesContainerName;
			VisualElement visualElement2 = visualElement;
			this.m_ChoiceRadioButtonContainer = visualElement;
			visualInput.Add(visualElement2);
			this.m_ChoiceRadioButtonContainer.AddToClassList(RadioButtonGroup.containerUssClassName);
			VisualElement visualInput2 = base.visualInput;
			VisualElement visualElement3 = new VisualElement();
			visualElement3.name = RadioButtonGroup.containerName;
			visualElement2 = visualElement3;
			this.m_ContentContainer = visualElement3;
			visualInput2.Add(visualElement2);
			this.m_ContentContainer.AddToClassList(RadioButtonGroup.containerUssClassName);
			this.m_GetAllRadioButtonsQuery = this.Query<RadioButton>(null, null);
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
				RadioButton radioButton = evt.target as RadioButton;
				List<RadioButton> list;
				using (CollectionPool<List<RadioButton>, RadioButton>.Get(out list))
				{
					this.GetAllRadioButtons(list);
					this.value = list.IndexOf(radioButton);
					evt.StopPropagation();
				}
			}
		}

		public override void SetValueWithoutNotify(int newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.UpdateRadioButtons(true);
		}

		private void GetAllRadioButtons(List<RadioButton> radioButtons)
		{
			radioButtons.Clear();
			this.m_GetAllRadioButtonsQuery.ForEach(new Action<RadioButton>(radioButtons.Add));
		}

		private void UpdateRadioButtons(bool notify)
		{
			bool flag = base.panel == null;
			if (!flag)
			{
				List<RadioButton> list;
				using (CollectionPool<List<RadioButton>, RadioButton>.Get(out list))
				{
					this.GetAllRadioButtons(list);
					bool flag2 = this.value >= 0 && this.value < list.Count;
					if (flag2)
					{
						this.m_SelectedRadioButton = list[this.value];
						if (notify)
						{
							this.m_SelectedRadioButton.value = true;
						}
						else
						{
							this.m_SelectedRadioButton.SetValueWithoutNotify(true);
						}
						foreach (RadioButton radioButton in list)
						{
							bool flag3 = radioButton != this.m_SelectedRadioButton;
							if (flag3)
							{
								if (notify)
								{
									radioButton.value = false;
								}
								else
								{
									radioButton.SetValueWithoutNotify(false);
								}
							}
						}
					}
					else
					{
						foreach (RadioButton radioButton2 in this.m_RegisteredRadioButtons)
						{
							if (notify)
							{
								radioButton2.value = false;
							}
							else
							{
								radioButton2.SetValueWithoutNotify(false);
							}
						}
					}
					this.m_UpdatingButtons = false;
				}
			}
		}

		private void ScheduleRadioButtons()
		{
			bool updatingButtons = this.m_UpdatingButtons;
			if (!updatingButtons)
			{
				base.schedule.Execute(delegate
				{
					this.UpdateRadioButtons(false);
				});
				this.m_UpdatingButtons = true;
			}
		}

		private void RegisterRadioButton(RadioButton radioButton)
		{
			bool flag = this.m_RegisteredRadioButtons.Contains(radioButton);
			if (!flag)
			{
				this.m_RegisteredRadioButtons.Add(radioButton);
				radioButton.RegisterValueChangedCallback<bool>(this.m_RadioButtonValueChangedCallback);
				bool flag2 = this.value == -1 && radioButton.value;
				if (flag2)
				{
					List<RadioButton> list;
					using (CollectionPool<List<RadioButton>, RadioButton>.Get(out list))
					{
						this.GetAllRadioButtons(list);
						this.SetValueWithoutNotify(list.IndexOf(radioButton));
					}
				}
				this.ScheduleRadioButtons();
			}
		}

		private void UnregisterRadioButton(RadioButton radioButton)
		{
			bool flag = !this.m_RegisteredRadioButtons.Contains(radioButton);
			if (!flag)
			{
				this.m_RegisteredRadioButtons.Remove(radioButton);
				radioButton.UnregisterValueChangedCallback<bool>(this.m_RadioButtonValueChangedCallback);
				this.UpdateRadioButtons(false);
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
			this.RegisterRadioButton(radioButton);
		}

		void IGroupBox.OnOptionRemoved(IGroupBoxOption option)
		{
			RadioButton radioButton = option as RadioButton;
			bool flag = radioButton == null;
			if (flag)
			{
				throw new ArgumentException("[UI Toolkit] Internal group box error. Expected a radio button element. Please report this using Help -> Report a bug...");
			}
			this.UnregisterRadioButton(radioButton);
			bool flag2 = this.m_SelectedRadioButton == radioButton;
			if (flag2)
			{
				this.m_SelectedRadioButton = null;
				this.value = -1;
			}
		}

		[CompilerGenerated]
		internal static bool <set_choices>g__AreListEqual|18_0(List<string> list1, IEnumerable<string> list2)
		{
			int num = 0;
			using (IEnumerator<string> enumerator = list2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
				}
			}
			bool flag = list1.Count != num;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num2 = 0;
				foreach (string text in list2)
				{
					bool flag3 = !string.Equals(list1[num2], text);
					if (flag3)
					{
						return false;
					}
					num2++;
				}
				flag2 = true;
			}
			return flag2;
		}

		internal static readonly BindingId choicesProperty = "choices";

		public new static readonly string ussClassName = "unity-radio-button-group";

		public static readonly string containerUssClassName = RadioButtonGroup.ussClassName + "__container";

		internal static readonly string containerName = "contentContainer";

		internal static readonly string choicesContainerName = "choicesContentContainer";

		private VisualElement m_ChoiceRadioButtonContainer;

		private VisualElement m_ContentContainer;

		private UQueryBuilder<RadioButton> m_GetAllRadioButtonsQuery;

		private readonly List<RadioButton> m_RegisteredRadioButtons = new List<RadioButton>();

		private RadioButton m_SelectedRadioButton;

		private EventCallback<ChangeEvent<bool>> m_RadioButtonValueChangedCallback;

		private bool m_UpdatingButtons;

		private List<string> m_Choices = new List<string>();

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<int>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<int>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(RadioButtonGroup.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("choicesList", "choices", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new RadioButtonGroup();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.choicesList_UxmlAttributeFlags);
				if (flag)
				{
					RadioButtonGroup radioButtonGroup = (RadioButtonGroup)obj;
					radioButtonGroup.choicesList = this.choicesList;
				}
			}

			[UxmlAttribute("choices")]
			[UxmlAttributeBindingPath("choices")]
			[SerializeField]
			private List<string> choicesList;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags choicesList_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<RadioButtonGroup, RadioButtonGroup.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseFieldTraits<int, UxmlIntAttributeDescription>
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				RadioButtonGroup radioButtonGroup = (RadioButtonGroup)ve;
				radioButtonGroup.choicesList = UxmlUtility.ParseStringListAttribute(this.m_Choices.GetValueFromBag(bag, cc));
			}

			private UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription
			{
				name = "choices"
			};
		}
	}
}
