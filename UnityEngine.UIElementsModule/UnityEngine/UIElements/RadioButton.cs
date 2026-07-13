using System;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class RadioButton : BaseBoolField, IGroupBoxOption
	{
		public override bool value
		{
			get
			{
				return base.value;
			}
			set
			{
				bool flag = base.value != value;
				if (flag)
				{
					base.value = value;
					this.UpdateCheckmark();
					if (value)
					{
						this.OnOptionSelected<RadioButton>();
					}
				}
			}
		}

		public RadioButton()
			: this(null)
		{
		}

		public RadioButton(string label)
			: base(label)
		{
			base.AddToClassList(RadioButton.ussClassName);
			base.visualInput.AddToClassList(RadioButton.inputUssClassName);
			base.labelElement.AddToClassList(RadioButton.labelUssClassName);
			this.m_CheckMark.RemoveFromHierarchy();
			this.m_CheckmarkBackground = new VisualElement
			{
				pickingMode = PickingMode.Ignore
			};
			this.m_CheckmarkBackground.Add(this.m_CheckMark);
			this.m_CheckmarkBackground.AddToClassList(RadioButton.checkmarkBackgroundUssClassName);
			this.m_CheckMark.AddToClassList(RadioButton.checkmarkUssClassName);
			base.visualInput.Add(this.m_CheckmarkBackground);
			this.UpdateCheckmark();
			base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnOptionAttachToPanel), TrickleDown.NoTrickleDown);
			base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnOptionDetachFromPanel), TrickleDown.NoTrickleDown);
		}

		private void OnOptionAttachToPanel(AttachToPanelEvent evt)
		{
			this.RegisterGroupBoxOption<RadioButton>();
		}

		private void OnOptionDetachFromPanel(DetachFromPanelEvent evt)
		{
			this.UnregisterGroupBoxOption<RadioButton>();
		}

		protected override void InitLabel()
		{
			base.InitLabel();
			this.m_Label.AddToClassList(RadioButton.textUssClassName);
		}

		protected override void ToggleValue()
		{
			bool flag = !this.value;
			if (flag)
			{
				this.value = true;
			}
		}

		[Obsolete("[UI Toolkit] Please set the value property instead.", false)]
		public void SetSelected(bool selected)
		{
			((IGroupBoxOption)this).SetSelected(selected);
		}

		void IGroupBoxOption.SetSelected(bool selected)
		{
			this.value = selected;
		}

		public override void SetValueWithoutNotify(bool newValue)
		{
			base.SetValueWithoutNotify(newValue);
			this.UpdateCheckmark();
		}

		private void UpdateCheckmark()
		{
			this.m_CheckMark.style.display = (this.value ? DisplayStyle.Flex : DisplayStyle.None);
		}

		protected override void UpdateMixedValueContent()
		{
			base.UpdateMixedValueContent();
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				this.m_CheckmarkBackground.RemoveFromHierarchy();
			}
			else
			{
				this.m_CheckmarkBackground.Add(this.m_CheckMark);
				base.visualInput.Add(this.m_CheckmarkBackground);
			}
		}

		public new static readonly string ussClassName = "unity-radio-button";

		public new static readonly string labelUssClassName = RadioButton.ussClassName + "__label";

		public new static readonly string inputUssClassName = RadioButton.ussClassName + "__input";

		public static readonly string checkmarkBackgroundUssClassName = RadioButton.ussClassName + "__checkmark-background";

		public static readonly string checkmarkUssClassName = RadioButton.ussClassName + "__checkmark";

		public static readonly string textUssClassName = RadioButton.ussClassName + "__text";

		private VisualElement m_CheckmarkBackground;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseBoolField.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(RadioButton.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("text", "text", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new RadioButton();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.text_UxmlAttributeFlags);
				if (flag)
				{
					RadioButton radioButton = (RadioButton)obj;
					radioButton.text = this.text;
				}
			}

			[SerializeField]
			[MultilineTextField]
			private string text;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags text_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<RadioButton, RadioButton.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription>
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((RadioButton)ve).text = this.m_Text.GetValueFromBag(bag, cc);
			}

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};
		}
	}
}
