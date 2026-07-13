using System;
using System.Collections.Generic;
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
			base.onIsReadOnlyChanged += this.OnIsReadOnlyChanged;
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

		public override TValueType value
		{
			get
			{
				return base.value;
			}
			set
			{
				base.value = value;
			}
		}

		internal override void UpdateValueFromText()
		{
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

		[EventInterest(new Type[]
		{
			typeof(BlurEvent),
			typeof(FocusEvent)
		})]
		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			bool flag = evt == null;
			if (!flag)
			{
				bool flag2 = evt.eventTypeId == EventBase<BlurEvent>.TypeId();
				if (flag2)
				{
					bool showMixedValue = base.showMixedValue;
					if (showMixedValue)
					{
						this.UpdateMixedValueContent();
					}
					else
					{
						bool flag3 = string.IsNullOrEmpty(base.text);
						if (flag3)
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
					bool flag4 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
					if (flag4)
					{
						bool flag5 = base.showMixedValue && base.textInputBase.textElement.hasFocus;
						if (flag5)
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

		private BaseFieldMouseDragger m_Dragger;

		private bool m_ForceUpdateDisplay;

		internal const int kMaxValueFieldLength = 1000;

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
				base.SelectNone();
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
				base.SelectAll();
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
