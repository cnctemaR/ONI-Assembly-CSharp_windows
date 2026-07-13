using System;
using System.Diagnostics;
using UnityEngine.Internal;

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
				base.visualInput.SetCheckedPseudoState(false);
				base.SetCheckedPseudoState(false);
				this.m_CheckMark.AddToClassList(Toggle.mixedValuesUssClassName);
			}
			else
			{
				this.m_CheckMark.RemoveFromClassList(Toggle.mixedValuesUssClassName);
				base.visualInput.SetCheckedPseudoState(this.value);
				base.SetCheckedPseudoState(this.value);
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

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseBoolField.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Toggle.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("text", "text", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Toggle();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.text_UxmlAttributeFlags);
				if (flag)
				{
					Toggle toggle = (Toggle)obj;
					toggle.text = this.text;
				}
			}

			[MultilineTextField]
			[SerializeField]
			private string text;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags text_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Toggle, Toggle.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
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
