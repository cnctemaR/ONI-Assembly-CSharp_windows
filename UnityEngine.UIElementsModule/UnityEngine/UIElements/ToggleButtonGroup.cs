using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlElement(null, new Type[] { typeof(Button) })]
	public class ToggleButtonGroup : BaseField<ToggleButtonGroupState>
	{
		[CreateProperty]
		public unsafe bool isMultipleSelection
		{
			get
			{
				return this.m_IsMultipleSelection;
			}
			set
			{
				bool flag = this.m_IsMultipleSelection == value;
				checked
				{
					if (!flag)
					{
						ToggleButtonGroupState value2 = this.value;
						int length = value2.length;
						Span<int> span = new Span<int>(stackalloc byte[unchecked((UIntPtr)length) * 4], length);
						Span<int> activeOptions = value2.GetActiveOptions(span);
						bool flag2 = activeOptions.Length > 1 && this.m_Buttons.Count > 0;
						if (flag2)
						{
							value2.ResetAllOptions();
							value2[*activeOptions[0]] = true;
							this.SetValueWithoutNotify(value2);
						}
						this.m_IsMultipleSelection = value;
						base.NotifyPropertyChanged(in ToggleButtonGroup.isMultipleSelectionProperty);
					}
				}
			}
		}

		[CreateProperty]
		public unsafe bool allowEmptySelection
		{
			get
			{
				return this.m_AllowEmptySelection;
			}
			set
			{
				bool flag = this.m_AllowEmptySelection == value;
				checked
				{
					if (!flag)
					{
						bool flag2 = !value;
						if (flag2)
						{
							ToggleButtonGroupState value2 = this.value;
							int length = value2.length;
							Span<int> span = new Span<int>(stackalloc byte[unchecked((UIntPtr)length) * 4], length);
							bool flag3 = value2.GetActiveOptions(span).Length == 0 && this.m_Buttons.Count > 0;
							if (flag3)
							{
								value2[0] = true;
								this.SetValueWithoutNotify(value2);
							}
						}
						this.m_AllowEmptySelection = value;
						base.NotifyPropertyChanged(in ToggleButtonGroup.allowEmptySelectionProperty);
					}
				}
			}
		}

		public ToggleButtonGroup()
			: this(null)
		{
		}

		public ToggleButtonGroup(string label)
			: this(label, new ToggleButtonGroupState(0UL, 64))
		{
		}

		public ToggleButtonGroup(ToggleButtonGroupState toggleButtonGroupState)
			: this(null, toggleButtonGroupState)
		{
		}

		public ToggleButtonGroup(string label, ToggleButtonGroupState toggleButtonGroupState)
			: base(label)
		{
			base.AddToClassList(ToggleButtonGroup.ussClassName);
			base.visualInput = new ToggleButtonGroup.ButtonGroupContainer(this)
			{
				name = ToggleButtonGroup.containerUssClassName,
				classList = { ToggleButtonGroup.buttonGroupClassName },
				delegatesFocus = true
			};
			this.m_ButtonGroupContainer = base.visualInput;
			this.SetValueWithoutNotify(toggleButtonGroupState);
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ButtonGroupContainer ?? this;
			}
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			this.UpdateButtonStates(this.value);
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				foreach (Button button in this.m_Buttons)
				{
					button.SetCheckedPseudoState(false);
					button.IncrementVersion(VersionChangeType.Styles);
				}
			}
			else
			{
				this.SetValueWithoutNotify(this.value);
			}
		}

		public override void SetValueWithoutNotify(ToggleButtonGroupState newValue)
		{
			bool flag = newValue.length == 0;
			if (flag)
			{
				newValue = new ToggleButtonGroupState(0UL, 0);
				if (this.m_EmptyLabel == null)
				{
					this.m_EmptyLabel = new Label("Group has no buttons.")
					{
						name = ToggleButtonGroup.emptyStateLabelClassName,
						classList = { ToggleButtonGroup.emptyStateLabelClassName }
					};
				}
				base.visualInput.Insert(0, this.m_EmptyLabel);
			}
			else
			{
				VisualElement emptyLabel = this.m_EmptyLabel;
				if (emptyLabel != null)
				{
					emptyLabel.RemoveFromHierarchy();
				}
			}
			base.SetValueWithoutNotify(newValue);
			this.UpdateButtonStates(newValue);
		}

		public Button GetButton(int index)
		{
			bool flag = index < 0 || index >= this.m_Buttons.Count;
			Button button;
			if (flag)
			{
				button = null;
			}
			else
			{
				button = this.m_Buttons[index];
			}
			return button;
		}

		private void OnButtonGroupContainerElementAdded(VisualElement ve)
		{
			Button button = ve as Button;
			bool flag = button == null;
			if (flag)
			{
				bool flag2 = ve == this.m_EmptyLabel;
				if (!flag2)
				{
					base.hierarchy.Add(ve);
				}
			}
			else
			{
				bool flag3 = this.m_Buttons.Count + 1 > 64;
				if (flag3)
				{
					Debug.LogWarning(ToggleButtonGroup.k_MaxToggleButtonGroupMessage);
				}
				else
				{
					button.AddToClassList(ToggleButtonGroup.buttonClassName);
					button.clickable.clickedWithEventInfo += this.OnOptionChange;
					this.m_Buttons = this.m_ButtonGroupContainer.Query<Button>(null, null).ToList();
					this.UpdateButtonsStyling();
					bool flag4 = false;
					ToggleButtonGroupState value = this.value;
					bool flag5 = this.m_Buttons.Count >= this.value.length && this.m_Buttons.Count <= 64;
					if (flag5)
					{
						value.length = this.m_Buttons.Count;
						flag4 = true;
					}
					bool flag6 = this.value.data == 0UL && !this.allowEmptySelection;
					if (flag6)
					{
						value[0] = true;
						flag4 = true;
					}
					bool flag7 = flag4;
					if (flag7)
					{
						this.value = value;
					}
				}
			}
		}

		private unsafe void OnButtonGroupContainerElementRemoved(VisualElement ve)
		{
			Button button = ve as Button;
			bool flag = button == null;
			checked
			{
				if (!flag)
				{
					ToggleButtonGroupState value = this.value;
					int num = this.m_Buttons.IndexOf(button);
					int length = value.length;
					Span<int> span = new Span<int>(stackalloc byte[unchecked((UIntPtr)length) * 4], length);
					Span<int> activeOptions = value.GetActiveOptions(span);
					bool flag2 = activeOptions.IndexOf(num) != -1;
					button.clickable.clickedWithEventInfo -= this.OnOptionChange;
					bool flag3 = flag2;
					if (flag3)
					{
						this.m_Buttons[num].SetCheckedPseudoState(false);
					}
					this.m_Buttons.Remove(button);
					this.UpdateButtonsStyling();
					value.length = this.m_Buttons.Count;
					bool flag4 = this.m_Buttons.Count == 0;
					if (flag4)
					{
						value.ResetAllOptions();
						this.SetValueWithoutNotify(value);
					}
					else
					{
						bool flag5 = flag2;
						if (flag5)
						{
							value[num] = false;
							bool flag6 = !this.allowEmptySelection && activeOptions.Length == 1;
							if (flag6)
							{
								value[0] = true;
							}
							this.value = value;
						}
					}
				}
			}
		}

		private unsafe void UpdateButtonStates(ToggleButtonGroupState options)
		{
			int length = this.value.length;
			Span<int> activeOptions;
			checked
			{
				Span<int> span = new Span<int>(stackalloc byte[unchecked((UIntPtr)length) * 4], length);
				activeOptions = options.GetActiveOptions(span);
			}
			for (int i = 0; i < this.m_Buttons.Count; i++)
			{
				bool flag = activeOptions.IndexOf(i) == -1;
				if (flag)
				{
					this.m_Buttons[i].SetCheckedPseudoState(false);
					this.m_Buttons[i].IncrementVersion(VersionChangeType.Styles);
				}
				else
				{
					this.m_Buttons[i].SetCheckedPseudoState(true);
					this.m_Buttons[i].IncrementVersion(VersionChangeType.Styles);
				}
			}
		}

		private unsafe void OnOptionChange(EventBase evt)
		{
			Button button = evt.target as Button;
			int num = this.m_Buttons.IndexOf(button);
			ToggleButtonGroupState value = this.value;
			int length = value.length;
			checked
			{
				Span<int> span = new Span<int>(stackalloc byte[unchecked((UIntPtr)length) * 4], length);
				Span<int> activeOptions = value.GetActiveOptions(span);
				bool showMixedValue = base.showMixedValue;
				if (showMixedValue)
				{
					ToggleButtonGroupState value2 = this.value;
					value2.ResetAllOptions();
					bool flag = this.value != value2;
					if (flag)
					{
						this.SetValueWithoutNotify(value2);
					}
				}
				bool isMultipleSelection = this.isMultipleSelection;
				if (isMultipleSelection)
				{
					bool flag2 = !this.allowEmptySelection && activeOptions.Length == 1 && value[num];
					if (flag2)
					{
						return;
					}
					bool flag3 = value[num];
					if (flag3)
					{
						value[num] = false;
					}
					else
					{
						value[num] = true;
					}
				}
				else
				{
					bool flag4 = this.allowEmptySelection && activeOptions.Length == 1 && value[*activeOptions[0]];
					if (flag4)
					{
						value[*activeOptions[0]] = false;
						bool flag5 = num != *activeOptions[0];
						if (flag5)
						{
							value[num] = true;
						}
					}
					else
					{
						value.ResetAllOptions();
						value[num] = true;
					}
				}
				this.value = value;
			}
		}

		private void UpdateButtonsStyling()
		{
			int count = this.m_Buttons.Count;
			for (int i = 0; i < count; i++)
			{
				Button button = this.m_Buttons[i];
				bool flag = count == 1;
				bool flag2 = i == 0 && !flag;
				bool flag3 = i == count - 1 && !flag;
				bool flag4 = !flag2 && !flag3 && !flag;
				button.EnableInClassList(ToggleButtonGroup.buttonStandaloneClassName, flag);
				button.EnableInClassList(ToggleButtonGroup.buttonLeftClassName, flag2);
				button.EnableInClassList(ToggleButtonGroup.buttonRightClassName, flag3);
				button.EnableInClassList(ToggleButtonGroup.buttonMidClassName, flag4);
			}
		}

		private static readonly string k_MaxToggleButtonGroupMessage = string.Format("The number of buttons added to ToggleButtonGroup exceeds the maximum allowed ({0}). The newly added button will not be treated as part of this control.", 64);

		internal static readonly BindingId isMultipleSelectionProperty = "isMultipleSelection";

		internal static readonly BindingId allowEmptySelectionProperty = "allowEmptySelection";

		public new static readonly string ussClassName = "unity-toggle-button-group";

		public static readonly string containerUssClassName = ToggleButtonGroup.ussClassName + "__container";

		public static readonly string buttonGroupClassName = "unity-button-group";

		public static readonly string buttonClassName = ToggleButtonGroup.buttonGroupClassName + "__button";

		public static readonly string buttonLeftClassName = ToggleButtonGroup.buttonClassName + "--left";

		public static readonly string buttonMidClassName = ToggleButtonGroup.buttonClassName + "--mid";

		public static readonly string buttonRightClassName = ToggleButtonGroup.buttonClassName + "--right";

		public static readonly string buttonStandaloneClassName = ToggleButtonGroup.buttonClassName + "--standalone";

		public static readonly string emptyStateLabelClassName = ToggleButtonGroup.buttonGroupClassName + "__empty-label";

		private VisualElement m_ButtonGroupContainer;

		private List<Button> m_Buttons = new List<Button>();

		private VisualElement m_EmptyLabel;

		private const string k_EmptyStateLabel = "Group has no buttons.";

		private bool m_IsMultipleSelection;

		private bool m_AllowEmptySelection;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseField<ToggleButtonGroupState>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<ToggleButtonGroupState>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(ToggleButtonGroup.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("isMultipleSelection", "is-multiple-selection", null, Array.Empty<string>()),
					new UxmlAttributeNames("allowEmptySelection", "allow-empty-selection", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new ToggleButtonGroup();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				ToggleButtonGroup toggleButtonGroup = (ToggleButtonGroup)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.isMultipleSelection_UxmlAttributeFlags);
				if (flag)
				{
					toggleButtonGroup.isMultipleSelection = this.isMultipleSelection;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.allowEmptySelection_UxmlAttributeFlags);
				if (flag2)
				{
					toggleButtonGroup.allowEmptySelection = this.allowEmptySelection;
				}
			}

			[SerializeField]
			private bool isMultipleSelection;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags isMultipleSelection_UxmlAttributeFlags;

			[SerializeField]
			private bool allowEmptySelection;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags allowEmptySelection_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<ToggleButtonGroup, ToggleButtonGroup.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<ToggleButtonGroupState>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				ToggleButtonGroup toggleButtonGroup = (ToggleButtonGroup)ve;
				toggleButtonGroup.isMultipleSelection = this.m_IsMultipleSelection.GetValueFromBag(bag, cc);
				toggleButtonGroup.allowEmptySelection = this.m_AllowEmptySelection.GetValueFromBag(bag, cc);
			}

			private UxmlBoolAttributeDescription m_IsMultipleSelection = new UxmlBoolAttributeDescription
			{
				name = "is-multiple-selection"
			};

			private UxmlBoolAttributeDescription m_AllowEmptySelection = new UxmlBoolAttributeDescription
			{
				name = "allow-empty-selection"
			};
		}

		private class ButtonGroupContainer : VisualElement
		{
			public ButtonGroupContainer(ToggleButtonGroup group)
			{
				this.m_Group = group;
			}

			internal override void OnChildAdded(VisualElement ve)
			{
				this.m_Group.OnButtonGroupContainerElementAdded(ve);
			}

			internal override void OnChildRemoved(VisualElement ve)
			{
				this.m_Group.OnButtonGroupContainerElementRemoved(ve);
			}

			private readonly ToggleButtonGroup m_Group;
		}
	}
}
