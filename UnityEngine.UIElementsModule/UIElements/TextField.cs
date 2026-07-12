using System;

namespace UnityEngine.UIElements
{
	public class TextField : TextInputBaseField<string>
	{
		private TextField.TextInput textInput
		{
			get
			{
				return (TextField.TextInput)base.textInputBase;
			}
		}

		public bool multiline
		{
			get
			{
				return this.textInput.multiline;
			}
			set
			{
				this.textInput.multiline = value;
			}
		}

		public TextField()
			: this(null)
		{
		}

		public TextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
			: this(null, maxLength, multiline, isPasswordField, maskChar)
		{
		}

		public TextField(string label)
			: this(label, -1, false, false, '*')
		{
		}

		public TextField(string label, int maxLength, bool multiline, bool isPasswordField, char maskChar)
			: base(label, maxLength, maskChar, new TextField.TextInput())
		{
			base.AddToClassList(TextField.ussClassName);
			base.labelElement.AddToClassList(TextField.labelUssClassName);
			base.visualInput.AddToClassList(TextField.inputUssClassName);
			base.pickingMode = PickingMode.Ignore;
			this.SetValueWithoutNotify("");
			this.multiline = multiline;
			base.isPasswordField = isPasswordField;
		}

		public override string value
		{
			get
			{
				return base.value;
			}
			set
			{
				base.value = value;
				base.textEdition.UpdateText(base.rawValue);
			}
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			((INotifyValueChanged<string>)this.textInput.textElement).SetValueWithoutNotify(base.rawValue);
		}

		internal override void UpdateTextFromValue()
		{
			this.SetValueWithoutNotify(base.rawValue);
		}

		[EventInterest(new Type[] { typeof(BlurEvent) })]
		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			bool flag;
			if (base.isDelayed)
			{
				long? num = ((evt != null) ? new long?(evt.eventTypeId) : null);
				long num2 = EventBase<BlurEvent>.TypeId();
				flag = (num.GetValueOrDefault() == num2) & (num != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.value = base.text;
			}
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			base.text = base.rawValue;
		}

		protected override string ValueToString(string value)
		{
			return value;
		}

		protected override string StringToValue(string str)
		{
			return str;
		}

		public new static readonly string ussClassName = "unity-text-field";

		public new static readonly string labelUssClassName = TextField.ussClassName + "__label";

		public new static readonly string inputUssClassName = TextField.ussClassName + "__input";

		public new class UxmlFactory : UxmlFactory<TextField, TextField.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextInputBaseField<string>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				TextField textField = (TextField)ve;
				textField.multiline = this.m_Multiline.GetValueFromBag(bag, cc);
				base.Init(ve, bag, cc);
				string empty = string.Empty;
				bool flag = TextField.UxmlTraits.k_Value.TryGetValueFromBag(bag, cc, ref empty);
				if (flag)
				{
					textField.SetValueWithoutNotify(empty);
				}
			}

			private static readonly UxmlStringAttributeDescription k_Value = new UxmlStringAttributeDescription
			{
				name = "value",
				obsoleteNames = new string[] { "text" }
			};

			private UxmlBoolAttributeDescription m_Multiline = new UxmlBoolAttributeDescription
			{
				name = "multiline"
			};
		}

		private class TextInput : TextInputBaseField<string>.TextInputBase
		{
			private TextField parentTextField
			{
				get
				{
					return (TextField)base.parent;
				}
			}

			public bool multiline
			{
				get
				{
					return base.textEdition.multiline;
				}
				set
				{
					bool flag = base.textEdition.multiline == value;
					if (!flag)
					{
						base.textEdition.multiline = value;
						if (value)
						{
							base.SetMultiline();
						}
						else
						{
							base.text = base.text.Replace("\n", "");
							base.SetSingleLine();
						}
					}
				}
			}

			public override bool isPasswordField
			{
				set
				{
					base.isPasswordField = value;
					if (value)
					{
						this.multiline = false;
					}
				}
			}

			protected override string StringToValue(string str)
			{
				return str;
			}
		}
	}
}
