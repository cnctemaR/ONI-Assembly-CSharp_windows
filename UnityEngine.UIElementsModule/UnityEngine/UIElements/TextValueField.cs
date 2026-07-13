using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public abstract class TextValueField<TValueType> : TextInputBaseField<TValueType>, IValueField<TValueType>
	{
		private TextValueField<TValueType>.TextValueInput textValueInput
		{
			get
			{
				return (TextValueField<TValueType>.TextValueInput)base.textInputBase;
			}
		}

		internal bool forceUpdateDisplay
		{
			set
			{
				this.m_ForceUpdateDisplay = value;
			}
		}

		[CreateProperty]
		public bool supportExpressions
		{
			get
			{
				return this.m_SupportExpressions;
			}
			set
			{
				bool flag = this.m_SupportExpressions != value;
				if (flag)
				{
					this.m_SupportExpressions = value;
					base.NotifyPropertyChanged(in TextValueField<TValueType>.supportExpressionsProperty);
				}
			}
		}

		[CreateProperty]
		public string formatString
		{
			get
			{
				return this.textValueInput.formatString;
			}
			set
			{
				bool flag = this.textValueInput.formatString != value;
				if (flag)
				{
					this.textValueInput.formatString = value;
					base.textEdition.UpdateText(this.ValueToString(base.rawValue));
					base.NotifyPropertyChanged(in TextValueField<TValueType>.formatStringProperty);
				}
			}
		}

		protected TextValueField(int maxLength, TextValueField<TValueType>.TextValueInput textValueInput)
			: this(null, maxLength, textValueInput)
		{
		}

		protected TextValueField(string label, int maxLength, TextValueField<TValueType>.TextValueInput textValueInput)
			: base(label, maxLength, '\0', textValueInput)
		{
			base.textEdition.UpdateText(this.ValueToString(base.rawValue));
			base.onIsReadOnlyChanged = (Action<bool>)Delegate.Combine(base.onIsReadOnlyChanged, new Action<bool>(this.OnIsReadOnlyChanged));
		}

		public abstract void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TValueType startValue);

		public void StartDragging()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				this.value = default(TValueType);
			}
			this.textValueInput.StartDragging();
		}

		public void StopDragging()
		{
			this.textValueInput.StopDragging();
		}

		internal override void UpdateValueFromText()
		{
			base.UpdatePlaceholderClassList(null);
			this.m_UpdateTextFromValue = false;
			try
			{
				this.value = this.StringToValue(base.text);
			}
			finally
			{
				this.m_UpdateTextFromValue = true;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal override void UpdateTextFromValue()
		{
			bool updateTextFromValue = this.m_UpdateTextFromValue;
			if (updateTextFromValue)
			{
				base.text = this.ValueToString(base.rawValue);
			}
		}

		private void OnIsReadOnlyChanged(bool newValue)
		{
			this.EnableLabelDragger(!newValue);
		}

		internal virtual bool CanTryParse(string textString)
		{
			return false;
		}

		protected void AddLabelDragger<TDraggerType>()
		{
			this.m_Dragger = new FieldMouseDragger<TDraggerType>((IValueField<TDraggerType>)this);
			this.EnableLabelDragger(!base.isReadOnly);
		}

		private void EnableLabelDragger(bool enable)
		{
			bool flag = this.m_Dragger != null;
			if (flag)
			{
				this.m_Dragger.SetDragZone(enable ? base.labelElement : null);
				base.labelElement.EnableInClassList(BaseField<TValueType>.labelDraggerVariantUssClassName, enable);
			}
		}

		public override void SetValueWithoutNotify(TValueType newValue)
		{
			bool flag = this.m_ForceUpdateDisplay || (this.m_UpdateTextFromValue && !EqualityComparer<TValueType>.Default.Equals(base.rawValue, newValue));
			base.SetValueWithoutNotify(newValue);
			bool flag2 = flag;
			if (flag2)
			{
				base.textEdition.UpdateText(this.ValueToString(base.rawValue));
			}
			this.m_ForceUpdateDisplay = false;
		}

		public void ClearValue()
		{
			base.text = string.Empty;
			this.UpdateValueFromText();
		}

		[EventInterest(new Type[]
		{
			typeof(BlurEvent),
			typeof(FocusEvent)
		})]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool flag = string.IsNullOrEmpty(base.text) && !string.IsNullOrEmpty(base.textEdition.placeholder);
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = evt.eventTypeId == EventBase<BlurEvent>.TypeId();
				if (flag3)
				{
					bool showMixedValue = base.showMixedValue;
					if (showMixedValue)
					{
						this.UpdateMixedValueContent();
					}
					else
					{
						bool flag4 = string.IsNullOrEmpty(base.text);
						if (flag4)
						{
							base.textInputBase.UpdateTextFromValue();
						}
						else
						{
							base.textInputBase.UpdateValueFromText();
							base.textInputBase.UpdateTextFromValue();
						}
					}
				}
				else
				{
					bool flag5 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
					if (flag5)
					{
						bool flag6 = base.showMixedValue && base.textInputBase.textElement.hasFocus;
						if (flag6)
						{
							base.textInputBase.text = "";
						}
					}
				}
			}
		}

		internal override void OnViewDataReady()
		{
			this.m_ForceUpdateDisplay = true;
			base.OnViewDataReady();
		}

		internal override void RegisterEditingCallbacks()
		{
			base.RegisterEditingCallbacks();
			base.labelElement.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			base.labelElement.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal override void UnregisterEditingCallbacks()
		{
			base.UnregisterEditingCallbacks();
			base.labelElement.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			base.labelElement.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal static readonly BindingId formatStringProperty = "formatString";

		internal static readonly BindingId supportExpressionsProperty = "supportExpressions";

		private BaseFieldMouseDragger m_Dragger;

		private bool m_ForceUpdateDisplay;

		private bool m_SupportExpressions = true;

		internal const int kMaxValueFieldLength = 1000;

		[ExcludeFromDocs]
		[Serializable]
		public new abstract class UxmlSerializedData : TextInputBaseField<TValueType>.UxmlSerializedData
		{
			public new static void Register()
			{
				TextInputBaseField<TValueType>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(TextValueField<TValueType>.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("supportExpressions", "support-expressions", null, Array.Empty<string>())
				}, false);
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				TextValueField<TValueType> textValueField = (TextValueField<TValueType>)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.supportExpressions_UxmlAttributeFlags);
				if (flag)
				{
					textValueField.supportExpressions = this.supportExpressions;
				}
			}

			[Tooltip("Indicates whether the field supports expressions that can be evaluated into a value.")]
			[SerializeField]
			private bool supportExpressions;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags supportExpressions_UxmlAttributeFlags;
		}

		protected abstract class TextValueInput : TextInputBaseField<TValueType>.TextInputBase
		{
			private TextValueField<TValueType> textValueFieldParent
			{
				get
				{
					return (TextValueField<TValueType>)base.parent;
				}
			}

			protected TextValueInput()
			{
				base.textEdition.AcceptCharacter = new Func<char, bool>(this.AcceptCharacter);
			}

			internal override bool AcceptCharacter(char c)
			{
				return base.AcceptCharacter(c) && c != '\0' && this.allowedCharacters.IndexOf(c) != -1;
			}

			protected abstract string allowedCharacters { get; }

			public string formatString { get; set; }

			public abstract void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TValueType startValue);

			public void StartDragging()
			{
				base.isDragging = true;
				base.textSelection.SelectNone();
				base.MarkDirtyRepaint();
			}

			public void StopDragging()
			{
				bool isDelayed = this.textValueFieldParent.isDelayed;
				if (isDelayed)
				{
					base.UpdateValueFromText();
				}
				base.isDragging = false;
				base.textSelection.SelectAll();
				base.MarkDirtyRepaint();
			}

			protected abstract string ValueToString(TValueType value);

			protected override TValueType StringToValue(string str)
			{
				return base.StringToValue(str);
			}
		}
	}
}
