using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	public abstract class BaseField<TValueType> : BindableElement, INotifyValueChanged<TValueType>, IMixedValueSupport, IPrefixLabel, IEditableElement
	{
		internal VisualElement visualInput
		{
			get
			{
				return this.m_VisualInput;
			}
			set
			{
				bool flag = this.m_VisualInput != null;
				if (flag)
				{
					bool flag2 = this.m_VisualInput.parent == this;
					if (flag2)
					{
						this.m_VisualInput.RemoveFromHierarchy();
					}
					this.m_VisualInput = null;
				}
				bool flag3 = value != null;
				if (flag3)
				{
					this.m_VisualInput = value;
				}
				else
				{
					this.m_VisualInput = new VisualElement
					{
						pickingMode = PickingMode.Ignore
					};
				}
				this.m_VisualInput.focusable = true;
				this.m_VisualInput.AddToClassList(BaseField<TValueType>.inputUssClassName);
				base.Add(this.m_VisualInput);
			}
		}

		protected TValueType rawValue
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Func<TValueType, TValueType> onValidateValue;

		public virtual TValueType value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				bool flag = !this.EqualsCurrentValue(value) || this.showMixedValue;
				if (flag)
				{
					TValueType value2 = this.m_Value;
					this.SetValueWithoutNotify(value);
					this.showMixedValue = false;
					bool flag2 = base.panel != null;
					if (flag2)
					{
						using (ChangeEvent<TValueType> pooled = ChangeEvent<TValueType>.GetPooled(value2, this.m_Value))
						{
							pooled.target = this;
							this.SendEvent(pooled);
						}
					}
				}
			}
		}

		public Label labelElement { get; private set; }

		public string label
		{
			get
			{
				return this.labelElement.text;
			}
			set
			{
				bool flag = this.labelElement.text != value;
				if (flag)
				{
					this.labelElement.text = value;
					bool flag2 = string.IsNullOrEmpty(this.labelElement.text);
					if (flag2)
					{
						base.AddToClassList(BaseField<TValueType>.noLabelVariantUssClassName);
						this.labelElement.RemoveFromHierarchy();
					}
					else
					{
						bool flag3 = !base.Contains(this.labelElement);
						if (flag3)
						{
							base.hierarchy.Insert(0, this.labelElement);
							base.RemoveFromClassList(BaseField<TValueType>.noLabelVariantUssClassName);
						}
					}
				}
			}
		}

		public bool showMixedValue
		{
			get
			{
				return this.m_ShowMixedValue;
			}
			set
			{
				bool flag = value == this.m_ShowMixedValue;
				if (!flag)
				{
					bool flag2 = value && !this.canSwitchToMixedValue;
					if (!flag2)
					{
						this.m_ShowMixedValue = value;
						this.UpdateMixedValueContent();
					}
				}
			}
		}

		private protected virtual bool canSwitchToMixedValue
		{
			get
			{
				return true;
			}
		}

		protected Label mixedValueLabel
		{
			get
			{
				bool flag = this.m_MixedValueLabel == null;
				if (flag)
				{
					this.m_MixedValueLabel = new Label(BaseField<TValueType>.mixedValueString)
					{
						focusable = true,
						tabIndex = -1
					};
					this.m_MixedValueLabel.AddToClassList(BaseField<TValueType>.labelUssClassName);
					this.m_MixedValueLabel.AddToClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
				}
				return this.m_MixedValueLabel;
			}
		}

		Action IEditableElement.editingStarted { get; set; }

		Action IEditableElement.editingEnded { get; set; }

		internal BaseField(string label)
		{
			base.isCompositeRoot = true;
			base.focusable = true;
			base.tabIndex = 0;
			base.excludeFromFocusRing = true;
			base.delegatesFocus = true;
			base.AddToClassList(BaseField<TValueType>.ussClassName);
			this.labelElement = new Label
			{
				focusable = true,
				tabIndex = -1
			};
			this.labelElement.AddToClassList(BaseField<TValueType>.labelUssClassName);
			bool flag = label != null;
			if (flag)
			{
				this.label = label;
			}
			else
			{
				base.AddToClassList(BaseField<TValueType>.noLabelVariantUssClassName);
			}
			base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
			base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
			this.m_VisualInput = null;
		}

		protected BaseField(string label, VisualElement visualInput)
			: this(label)
		{
			this.visualInput = visualInput;
		}

		internal virtual bool EqualsCurrentValue(TValueType value)
		{
			return EqualityComparer<TValueType>.Default.Equals(this.m_Value, value);
		}

		private void OnAttachToPanel(AttachToPanelEvent e)
		{
			this.RegisterEditingCallbacks();
			bool flag = e.destinationPanel == null;
			if (!flag)
			{
				bool flag2 = e.destinationPanel.contextType == ContextType.Player;
				if (!flag2)
				{
					for (VisualElement visualElement = base.parent; visualElement != null; visualElement = visualElement.parent)
					{
						bool flag3 = visualElement.ClassListContains("unity-inspector-element");
						if (flag3)
						{
							this.m_CachedInspectorElement = visualElement;
						}
						bool flag4 = visualElement.ClassListContains("unity-inspector-main-container");
						if (flag4)
						{
							this.m_CachedContextWidthElement = visualElement;
							break;
						}
					}
					bool flag5 = this.m_CachedInspectorElement == null;
					if (!flag5)
					{
						this.m_LabelWidthRatio = 0.45f;
						this.m_LabelExtraPadding = 37f;
						this.m_LabelBaseMinWidth = 123f;
						this.m_LabelExtraContextWidth = 1f;
						base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
						base.AddToClassList(BaseField<TValueType>.inspectorFieldUssClassName);
						base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnInspectorFieldGeometryChanged), TrickleDown.NoTrickleDown);
					}
				}
			}
		}

		private void OnDetachFromPanel(DetachFromPanelEvent e)
		{
			this.UnregisterEditingCallbacks();
			this.onValidateValue = null;
		}

		internal virtual void RegisterEditingCallbacks()
		{
			base.RegisterCallback<FocusInEvent>(new EventCallback<FocusInEvent>(this.StartEditing), TrickleDown.NoTrickleDown);
			base.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal virtual void UnregisterEditingCallbacks()
		{
			base.UnregisterCallback<FocusInEvent>(new EventCallback<FocusInEvent>(this.StartEditing), TrickleDown.NoTrickleDown);
			base.UnregisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal void StartEditing(EventBase e)
		{
			Action editingStarted = ((IEditableElement)this).editingStarted;
			if (editingStarted != null)
			{
				editingStarted();
			}
		}

		internal void EndEditing(EventBase e)
		{
			Action editingEnded = ((IEditableElement)this).editingEnded;
			if (editingEnded != null)
			{
				editingEnded();
			}
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
		{
			float num;
			bool flag = evt.customStyle.TryGetValue(BaseField<TValueType>.s_LabelWidthRatioProperty, out num);
			if (flag)
			{
				this.m_LabelWidthRatio = num;
			}
			float num2;
			bool flag2 = evt.customStyle.TryGetValue(BaseField<TValueType>.s_LabelExtraPaddingProperty, out num2);
			if (flag2)
			{
				this.m_LabelExtraPadding = num2;
			}
			float num3;
			bool flag3 = evt.customStyle.TryGetValue(BaseField<TValueType>.s_LabelBaseMinWidthProperty, out num3);
			if (flag3)
			{
				this.m_LabelBaseMinWidth = num3;
			}
			float num4;
			bool flag4 = evt.customStyle.TryGetValue(BaseField<TValueType>.s_LabelExtraContextWidthProperty, out num4);
			if (flag4)
			{
				this.m_LabelExtraContextWidth = num4;
			}
			this.AlignLabel();
		}

		private void OnInspectorFieldGeometryChanged(GeometryChangedEvent e)
		{
			this.AlignLabel();
		}

		private void AlignLabel()
		{
			bool flag = !base.ClassListContains(BaseField<TValueType>.alignedFieldUssClassName);
			if (!flag)
			{
				float num = this.m_LabelExtraPadding;
				float num2 = base.worldBound.x - this.m_CachedInspectorElement.worldBound.x - this.m_CachedInspectorElement.resolvedStyle.paddingLeft;
				num += num2;
				num += base.resolvedStyle.paddingLeft;
				float num3 = this.m_LabelBaseMinWidth - num2 - base.resolvedStyle.paddingLeft;
				VisualElement visualElement = this.m_CachedContextWidthElement ?? this.m_CachedInspectorElement;
				this.labelElement.style.minWidth = Mathf.Max(num3, 0f);
				float num4 = (visualElement.resolvedStyle.width + this.m_LabelExtraContextWidth) * this.m_LabelWidthRatio - num;
				bool flag2 = Mathf.Abs(this.labelElement.resolvedStyle.width - num4) > 1E-30f;
				if (flag2)
				{
					this.labelElement.style.width = Mathf.Max(0f, num4);
				}
			}
		}

		internal TValueType ValidatedValue(TValueType value)
		{
			bool flag = this.onValidateValue != null;
			TValueType tvalueType;
			if (flag)
			{
				tvalueType = this.onValidateValue(value);
			}
			else
			{
				tvalueType = value;
			}
			return tvalueType;
		}

		protected virtual void UpdateMixedValueContent()
		{
			throw new NotImplementedException();
		}

		public virtual void SetValueWithoutNotify(TValueType newValue)
		{
			bool skipValidation = this.m_SkipValidation;
			if (skipValidation)
			{
				this.m_Value = newValue;
			}
			else
			{
				this.m_Value = this.ValidatedValue(newValue);
			}
			bool flag = !string.IsNullOrEmpty(base.viewDataKey);
			if (flag)
			{
				base.SaveViewData();
			}
			base.MarkDirtyRepaint();
			bool showMixedValue = this.showMixedValue;
			if (showMixedValue)
			{
				this.UpdateMixedValueContent();
			}
		}

		internal void SetValueWithoutValidation(TValueType newValue)
		{
			this.m_SkipValidation = true;
			this.value = newValue;
			this.m_SkipValidation = false;
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			bool flag = this.m_VisualInput != null;
			if (flag)
			{
				string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
				TValueType value = this.m_Value;
				base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
				bool flag2 = !EqualityComparer<TValueType>.Default.Equals(value, this.m_Value);
				if (flag2)
				{
					using (ChangeEvent<TValueType> pooled = ChangeEvent<TValueType>.GetPooled(value, this.m_Value))
					{
						pooled.target = this;
						this.SetValueWithoutNotify(this.m_Value);
						this.SendEvent(pooled);
					}
				}
			}
		}

		internal override Rect GetTooltipRect()
		{
			return (!string.IsNullOrEmpty(this.label)) ? this.labelElement.worldBound : base.worldBound;
		}

		public static readonly string ussClassName = "unity-base-field";

		public static readonly string labelUssClassName = BaseField<TValueType>.ussClassName + "__label";

		public static readonly string inputUssClassName = BaseField<TValueType>.ussClassName + "__input";

		public static readonly string noLabelVariantUssClassName = BaseField<TValueType>.ussClassName + "--no-label";

		public static readonly string labelDraggerVariantUssClassName = BaseField<TValueType>.labelUssClassName + "--with-dragger";

		public static readonly string mixedValueLabelUssClassName = BaseField<TValueType>.labelUssClassName + "--mixed-value";

		public static readonly string alignedFieldUssClassName = BaseField<TValueType>.ussClassName + "__aligned";

		private static readonly string inspectorFieldUssClassName = BaseField<TValueType>.ussClassName + "__inspector-field";

		protected internal static readonly string mixedValueString = "—";

		protected internal static readonly PropertyName serializedPropertyCopyName = "SerializedPropertyCopyName";

		private static CustomStyleProperty<float> s_LabelWidthRatioProperty = new CustomStyleProperty<float>("--unity-property-field-label-width-ratio");

		private static CustomStyleProperty<float> s_LabelExtraPaddingProperty = new CustomStyleProperty<float>("--unity-property-field-label-extra-padding");

		private static CustomStyleProperty<float> s_LabelBaseMinWidthProperty = new CustomStyleProperty<float>("--unity-property-field-label-base-min-width");

		private static CustomStyleProperty<float> s_LabelExtraContextWidthProperty = new CustomStyleProperty<float>("--unity-base-field-extra-context-width");

		private float m_LabelWidthRatio;

		private float m_LabelExtraPadding;

		private float m_LabelBaseMinWidth;

		private float m_LabelExtraContextWidth;

		private VisualElement m_VisualInput;

		internal Action<ExpressionEvaluator.Expression> expressionEvaluated;

		[SerializeField]
		private TValueType m_Value;

		private bool m_ShowMixedValue;

		private Label m_MixedValueLabel;

		private bool m_SkipValidation;

		private VisualElement m_CachedContextWidthElement;

		private VisualElement m_CachedInspectorElement;

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public UxmlTraits()
			{
				base.focusIndex.defaultValue = 0;
				base.focusable.defaultValue = true;
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((BaseField<TValueType>)ve).label = this.m_Label.GetValueFromBag(bag, cc);
			}

			internal static List<string> ParseChoiceList(string choicesFromBag)
			{
				bool flag = string.IsNullOrEmpty(choicesFromBag.Trim());
				List<string> list;
				if (flag)
				{
					list = null;
				}
				else
				{
					string[] array = choicesFromBag.Split(',', StringSplitOptions.None);
					bool flag2 = array.Length != 0;
					if (flag2)
					{
						List<string> list2 = new List<string>();
						foreach (string text in array)
						{
							list2.Add(text.Trim());
						}
						list = list2;
					}
					else
					{
						list = null;
					}
				}
				return list;
			}

			private UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
			{
				name = "label"
			};
		}
	}
}
