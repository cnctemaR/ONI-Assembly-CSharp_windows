using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

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

		[CreateProperty]
		public bool multiline
		{
			get
			{
				return this.textInput.multiline;
			}
			set
			{
				bool multiline = this.multiline;
				this.textInput.multiline = value;
				bool flag = multiline != this.multiline;
				if (flag)
				{
					base.NotifyPropertyChanged(in TextField.multilineProperty);
				}
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
			base.textEdition.isPassword = isPasswordField;
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
			string text = base.rawValue;
			bool flag = !this.multiline && base.rawValue != null;
			if (flag)
			{
				text = base.rawValue.Replace("\n", "");
			}
			((INotifyValueChanged<string>)this.textInput.textElement).SetValueWithoutNotify(text);
		}

		internal override void UpdateTextFromValue()
		{
			this.SetValueWithoutNotify(base.rawValue);
		}

		[EventInterest(new Type[] { typeof(FocusOutEvent) })]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool flag;
			if (base.isDelayed)
			{
				long? num = ((evt != null) ? new long?(evt.eventTypeId) : null);
				long num2 = EventBase<FocusOutEvent>.TypeId();
				flag = (num.GetValueOrDefault() == num2) & (num != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				DispatchMode dispatchMode = base.dispatchMode;
				try
				{
					base.dispatchMode = DispatchMode.Immediate;
					this.value = base.text;
				}
				finally
				{
					base.dispatchMode = dispatchMode;
				}
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

		internal static readonly BindingId multilineProperty = "multiline";

		public new static readonly string ussClassName = "unity-text-field";

		public new static readonly string labelUssClassName = TextField.ussClassName + "__label";

		public new static readonly string inputUssClassName = TextField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextInputBaseField<string>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextInputBaseField<string>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(TextField.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("multiline", "multiline", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new TextField();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				TextField textField = (TextField)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.multiline_UxmlAttributeFlags);
				if (flag)
				{
					textField.multiline = this.multiline;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.verticalScrollerVisibility_UxmlAttributeFlags);
				if (flag2)
				{
					textField.verticalScrollerVisibility = this.verticalScrollerVisibility;
				}
			}

			void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
			{
				string text;
				bool flag = bag.TryGetAttributeValue("text", out text);
				if (flag)
				{
					base.Value = text;
					handledAttributes.Add("value");
					UxmlAsset uxmlAsset = bag as UxmlAsset;
					bool flag2 = uxmlAsset != null;
					if (flag2)
					{
						uxmlAsset.RemoveAttribute("text");
						uxmlAsset.SetAttribute("value", base.Value);
					}
				}
			}

			[SerializeField]
			[MultilineDecorator]
			private bool multiline;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags multiline_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<TextField, TextField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextInputBaseField<string>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				TextField textField = (TextField)ve;
				base.Init(ve, bag, cc);
				string empty = string.Empty;
				bool flag = TextField.UxmlTraits.k_Value.TryGetValueFromBag(bag, cc, ref empty);
				if (flag)
				{
					textField.SetValueWithoutNotify(empty);
				}
				textField.multiline = this.m_Multiline.GetValueFromBag(bag, cc);
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
					bool flag = value || string.IsNullOrEmpty(base.text) || !base.text.Contains("\n");
					bool flag2 = flag && base.textEdition.multiline == value;
					if (!flag2)
					{
						base.textEdition.multiline = value;
						if (value)
						{
							base.text = this.parentTextField.rawValue;
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

			[Obsolete("isPasswordField is deprecated. Use textEdition.isPassword instead.")]
			public override bool isPasswordField
			{
				set
				{
					base.textEdition.isPassword = value;
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
