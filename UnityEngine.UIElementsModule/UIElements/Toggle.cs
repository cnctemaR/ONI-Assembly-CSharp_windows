using System;

namespace UnityEngine.UIElements
{
	public class Toggle : BaseBoolField
	{
		public Toggle()
			: this(null)
		{
		}

		public Toggle(string label)
			: base(label)
		{
			base.AddToClassList(Toggle.ussClassName);
			base.visualInput.AddToClassList(Toggle.inputUssClassName);
			base.labelElement.AddToClassList(Toggle.labelUssClassName);
			this.m_CheckMark.AddToClassList(Toggle.checkmarkUssClassName);
		}

		protected override void InitLabel()
		{
			base.InitLabel();
			this.m_Label.AddToClassList(Toggle.textUssClassName);
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				base.visualInput.pseudoStates &= ~PseudoStates.Checked;
				base.pseudoStates &= ~PseudoStates.Checked;
				this.m_CheckMark.AddToClassList(Toggle.mixedValuesUssClassName);
			}
			else
			{
				this.m_CheckMark.RemoveFromClassList(Toggle.mixedValuesUssClassName);
				bool value = this.value;
				if (value)
				{
					base.visualInput.pseudoStates |= PseudoStates.Checked;
					base.pseudoStates |= PseudoStates.Checked;
				}
				else
				{
					base.visualInput.pseudoStates &= ~PseudoStates.Checked;
					base.pseudoStates &= ~PseudoStates.Checked;
				}
			}
		}

		public new static readonly string ussClassName = "unity-toggle";

		public new static readonly string labelUssClassName = Toggle.ussClassName + "__label";

		public new static readonly string inputUssClassName = Toggle.ussClassName + "__input";

		[Obsolete]
		public static readonly string noTextVariantUssClassName = Toggle.ussClassName + "--no-text";

		public static readonly string checkmarkUssClassName = Toggle.ussClassName + "__checkmark";

		public static readonly string textUssClassName = Toggle.ussClassName + "__text";

		public static readonly string mixedValuesUssClassName = Toggle.ussClassName + "__mixed-values";

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
