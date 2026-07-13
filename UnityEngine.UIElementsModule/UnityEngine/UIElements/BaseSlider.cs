using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public abstract class BaseSlider<TValueType> : BaseField<TValueType>, IValueField<TValueType> where TValueType : IComparable<TValueType>
	{
		internal VisualElement dragContainer { get; private set; }

		internal VisualElement dragElement { get; private set; }

		internal VisualElement trackElement { get; private set; }

		internal VisualElement dragBorderElement { get; private set; }

		internal TextField inputTextField { get; private set; }

		internal VisualElement fillElement { get; private set; }

		private protected override bool canSwitchToMixedValue
		{
			get
			{
				bool flag = this.inputTextField == null;
				return flag || !this.inputTextField.textInputBase.textElement.hasFocus;
			}
		}

		internal TValueType valueOverride
		{
			get
			{
				return this.value;
			}
			set
			{
				this.SetValueWithoutNotify(value);
			}
		}

		[CreateProperty]
		public TValueType lowValue
		{
			get
			{
				return this.m_LowValue;
			}
			set
			{
				bool flag = !EqualityComparer<TValueType>.Default.Equals(this.m_LowValue, value);
				if (flag)
				{
					this.m_LowValue = value;
					this.ClampValue();
					this.UpdateDragElementPosition();
					base.SaveViewData();
					base.NotifyPropertyChanged(in BaseSlider<TValueType>.lowValueProperty);
				}
			}
		}

		[CreateProperty]
		public TValueType highValue
		{
			get
			{
				return this.m_HighValue;
			}
			set
			{
				bool flag = !EqualityComparer<TValueType>.Default.Equals(this.m_HighValue, value);
				if (flag)
				{
					this.m_HighValue = value;
					this.ClampValue();
					this.UpdateDragElementPosition();
					base.SaveViewData();
					base.NotifyPropertyChanged(in BaseSlider<TValueType>.highValueProperty);
				}
			}
		}

		internal void SetHighValueWithoutNotify(TValueType newHighValue)
		{
			this.m_HighValue = newHighValue;
			TValueType tvalueType = (this.clamped ? this.GetClampedValue(this.value) : this.value);
			this.SetValueWithoutNotify(tvalueType);
			this.UpdateDragElementPosition();
			base.SaveViewData();
		}

		[CreateProperty(ReadOnly = true)]
		public TValueType range
		{
			get
			{
				return this.SliderRange();
			}
		}

		[CreateProperty]
		public virtual float pageSize
		{
			get
			{
				return this.m_PageSize;
			}
			set
			{
				bool flag = this.m_PageSize == value;
				if (!flag)
				{
					this.m_PageSize = value;
					base.NotifyPropertyChanged(in BaseSlider<TValueType>.pageSizeProperty);
				}
			}
		}

		[CreateProperty]
		public virtual bool showInputField
		{
			get
			{
				return this.m_ShowInputField;
			}
			set
			{
				bool flag = this.m_ShowInputField != value;
				if (flag)
				{
					this.m_ShowInputField = value;
					this.UpdateTextFieldVisibility();
					base.NotifyPropertyChanged(in BaseSlider<TValueType>.showInputFieldProperty);
				}
			}
		}

		[CreateProperty]
		public bool fill
		{
			get
			{
				return this.m_Fill;
			}
			set
			{
				bool flag = this.m_Fill == value;
				if (!flag)
				{
					this.m_Fill = value;
					if (value)
					{
						this.UpdateDragElementPosition();
					}
					else
					{
						bool flag2 = this.fillElement != null;
						if (flag2)
						{
							this.fillElement.RemoveFromHierarchy();
							this.fillElement = null;
						}
					}
					base.NotifyPropertyChanged(in BaseSlider<TValueType>.fillProperty);
				}
			}
		}

		internal bool clamped { get; set; } = true;

		internal ClampedDragger<TValueType> clampedDragger { get; private set; }

		private TValueType Clamp(TValueType value, TValueType lowBound, TValueType highBound)
		{
			TValueType tvalueType = value;
			bool flag = lowBound.CompareTo(value) > 0;
			if (flag)
			{
				tvalueType = lowBound;
			}
			else
			{
				bool flag2 = highBound.CompareTo(value) < 0;
				if (flag2)
				{
					tvalueType = highBound;
				}
			}
			return tvalueType;
		}

		private TValueType GetClampedValue(TValueType newValue)
		{
			TValueType tvalueType = this.lowValue;
			TValueType tvalueType2 = this.highValue;
			bool flag = tvalueType.CompareTo(tvalueType2) > 0;
			if (flag)
			{
				TValueType tvalueType3 = tvalueType;
				tvalueType = tvalueType2;
				tvalueType2 = tvalueType3;
			}
			return this.Clamp(newValue, tvalueType, tvalueType2);
		}

		public override TValueType value
		{
			get
			{
				return base.value;
			}
			set
			{
				TValueType tvalueType = (this.clamped ? this.GetClampedValue(value) : value);
				base.value = tvalueType;
			}
		}

		public virtual void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TValueType startValue)
		{
		}

		void IValueField<TValueType>.StartDragging()
		{
		}

		void IValueField<TValueType>.StopDragging()
		{
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<TValueType> onSetValueWithoutNotify;

		public override void SetValueWithoutNotify(TValueType newValue)
		{
			TValueType tvalueType = (this.clamped ? this.GetClampedValue(newValue) : newValue);
			base.SetValueWithoutNotify(tvalueType);
			Action<TValueType> action = this.onSetValueWithoutNotify;
			if (action != null)
			{
				action(tvalueType);
			}
			this.UpdateDragElementPosition();
			this.UpdateTextFieldValue();
		}

		[CreateProperty]
		public SliderDirection direction
		{
			get
			{
				return this.m_Direction;
			}
			set
			{
				SliderDirection direction = this.m_Direction;
				this.m_Direction = value;
				bool flag = this.m_Direction == SliderDirection.Horizontal;
				if (flag)
				{
					base.RemoveFromClassList(BaseSlider<TValueType>.verticalVariantUssClassName);
					base.AddToClassList(BaseSlider<TValueType>.horizontalVariantUssClassName);
				}
				else
				{
					base.RemoveFromClassList(BaseSlider<TValueType>.horizontalVariantUssClassName);
					base.AddToClassList(BaseSlider<TValueType>.verticalVariantUssClassName);
				}
				bool flag2 = direction != this.m_Direction;
				if (flag2)
				{
					base.NotifyPropertyChanged(in BaseSlider<TValueType>.directionProperty);
				}
			}
		}

		[CreateProperty]
		public bool inverted
		{
			get
			{
				return this.m_Inverted;
			}
			set
			{
				bool flag = this.m_Inverted != value;
				if (flag)
				{
					this.m_Inverted = value;
					this.UpdateDragElementPosition();
					base.NotifyPropertyChanged(in BaseSlider<TValueType>.invertedProperty);
				}
			}
		}

		internal BaseSlider(string label, TValueType start, TValueType end, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: base(label, null)
		{
			base.AddToClassList(BaseSlider<TValueType>.ussClassName);
			base.labelElement.AddToClassList(BaseSlider<TValueType>.labelUssClassName);
			base.visualInput.AddToClassList(BaseSlider<TValueType>.inputUssClassName);
			this.direction = direction;
			this.pageSize = pageSize;
			this.lowValue = start;
			this.highValue = end;
			base.pickingMode = PickingMode.Ignore;
			this.dragContainer = new VisualElement
			{
				name = "unity-drag-container"
			};
			this.dragContainer.AddToClassList(BaseSlider<TValueType>.dragContainerUssClassName);
			this.dragContainer.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.UpdateDragElementPosition), TrickleDown.NoTrickleDown);
			base.visualInput.Add(this.dragContainer);
			this.trackElement = new VisualElement
			{
				name = "unity-tracker",
				usageHints = UsageHints.DynamicColor
			};
			this.trackElement.AddToClassList(BaseSlider<TValueType>.trackerUssClassName);
			this.dragContainer.Add(this.trackElement);
			this.dragBorderElement = new VisualElement
			{
				name = "unity-dragger-border"
			};
			this.dragBorderElement.AddToClassList(BaseSlider<TValueType>.draggerBorderUssClassName);
			this.dragContainer.Add(this.dragBorderElement);
			this.dragElement = new VisualElement
			{
				name = "unity-dragger",
				usageHints = UsageHints.DynamicTransform
			};
			this.dragElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.UpdateDragElementPosition), TrickleDown.NoTrickleDown);
			this.dragElement.AddToClassList(BaseSlider<TValueType>.draggerUssClassName);
			this.dragContainer.Add(this.dragElement);
			this.clampedDragger = new ClampedDragger<TValueType>(this, new Action(this.SetSliderValueFromClick), new Action(this.SetSliderValueFromDrag));
			this.dragContainer.pickingMode = PickingMode.Position;
			this.dragContainer.AddManipulator(this.clampedDragger);
			base.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
			base.RegisterCallback<FocusInEvent>(new EventCallback<FocusInEvent>(this.OnFocusIn), TrickleDown.NoTrickleDown);
			base.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnFocusOut), TrickleDown.NoTrickleDown);
			base.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit), TrickleDown.NoTrickleDown);
			base.RegisterCallback<NavigationMoveEvent>(new EventCallback<NavigationMoveEvent>(this.OnNavigationMove), TrickleDown.NoTrickleDown);
			this.UpdateTextFieldVisibility();
			FieldMouseDragger<TValueType> fieldMouseDragger = new FieldMouseDragger<TValueType>(this);
			fieldMouseDragger.SetDragZone(base.labelElement);
			base.labelElement.AddToClassList(BaseField<TValueType>.labelDraggerVariantUssClassName);
		}

		protected internal static float GetClosestPowerOfTen(float positiveNumber)
		{
			bool flag = positiveNumber <= 0f;
			float num;
			if (flag)
			{
				num = 1f;
			}
			else
			{
				num = Mathf.Pow(10f, (float)Mathf.RoundToInt(Mathf.Log10(positiveNumber)));
			}
			return num;
		}

		protected internal static float RoundToMultipleOf(float value, float roundingValue)
		{
			bool flag = roundingValue == 0f;
			float num;
			if (flag)
			{
				num = value;
			}
			else
			{
				num = Mathf.Round(value / roundingValue) * roundingValue;
			}
			return num;
		}

		private void ClampValue()
		{
			this.value = base.rawValue;
		}

		internal abstract TValueType SliderLerpUnclamped(TValueType a, TValueType b, float interpolant);

		internal abstract float SliderNormalizeValue(TValueType currentValue, TValueType lowerValue, TValueType higherValue);

		internal abstract TValueType SliderRange();

		internal abstract TValueType ParseStringToValue(string previousValue, string newValue);

		internal abstract void ComputeValueFromKey(BaseSlider<TValueType>.SliderKey sliderKey, bool isShift);

		private TValueType SliderLerpDirectionalUnclamped(TValueType a, TValueType b, float positionInterpolant)
		{
			float num = ((this.direction == SliderDirection.Vertical) ? (1f - positionInterpolant) : positionInterpolant);
			bool inverted = this.inverted;
			TValueType tvalueType;
			if (inverted)
			{
				tvalueType = this.SliderLerpUnclamped(b, a, num);
			}
			else
			{
				tvalueType = this.SliderLerpUnclamped(a, b, num);
			}
			return tvalueType;
		}

		private void SetSliderValueFromDrag()
		{
			bool flag = this.clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.Free;
			if (!flag)
			{
				Vector2 delta = this.clampedDragger.delta;
				bool flag2 = this.direction == SliderDirection.Horizontal;
				if (flag2)
				{
					this.ComputeValueAndDirectionFromDrag(this.dragContainer.resolvedStyle.width, this.dragElement.resolvedStyle.width, this.m_DragElementStartPos.x + delta.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromDrag(this.dragContainer.resolvedStyle.height, this.dragElement.resolvedStyle.height, this.m_DragElementStartPos.y + delta.y);
				}
			}
		}

		private void ComputeValueAndDirectionFromDrag(float sliderLength, float dragElementLength, float dragElementPos)
		{
			float num = sliderLength - dragElementLength;
			bool flag = Mathf.Abs(num) < 1E-30f;
			if (!flag)
			{
				bool clamped = this.clamped;
				float num2;
				if (clamped)
				{
					num2 = Mathf.Max(0f, Mathf.Min(dragElementPos, num)) / num;
				}
				else
				{
					num2 = dragElementPos / num;
				}
				TValueType value = this.value;
				this.value = this.SliderLerpDirectionalUnclamped(this.lowValue, this.highValue, num2);
				bool flag2 = EqualityComparer<TValueType>.Default.Equals(this.value, value);
				if (flag2)
				{
					this.UpdateDragElementPosition();
				}
			}
		}

		private void SetSliderValueFromClick()
		{
			bool flag = this.clampedDragger.dragDirection == ClampedDragger<TValueType>.DragDirection.Free;
			if (!flag)
			{
				bool flag2 = this.clampedDragger.dragDirection == ClampedDragger<TValueType>.DragDirection.None;
				if (flag2)
				{
					bool flag3 = Mathf.Approximately(this.pageSize, 0f);
					if (flag3)
					{
						bool flag4 = this.direction == SliderDirection.Horizontal;
						float num;
						float num2;
						float num5;
						float num6;
						float num7;
						if (flag4)
						{
							num = this.dragContainer.resolvedStyle.width;
							num2 = this.dragElement.resolvedStyle.width;
							float num3 = num - num2;
							float num4 = this.clampedDragger.startMousePosition.x - num2 / 2f;
							num5 = Mathf.Max(0f, Mathf.Min(num4, num3));
							num6 = this.dragElement.resolvedStyle.translate.y;
							num7 = num5;
						}
						else
						{
							num = this.dragContainer.resolvedStyle.height;
							num2 = this.dragElement.resolvedStyle.height;
							float num8 = num - num2;
							float num9 = this.clampedDragger.startMousePosition.y - num2 / 2f;
							num5 = this.dragElement.resolvedStyle.translate.x;
							num6 = Mathf.Max(0f, Mathf.Min(num9, num8));
							num7 = num6;
						}
						Vector3 vector = new Vector3(num5, num6, 0f);
						this.dragElement.style.translate = vector;
						this.dragBorderElement.style.translate = vector;
						this.m_DragElementStartPos = new Rect(num5, num6, this.dragElement.resolvedStyle.width, this.dragElement.resolvedStyle.height);
						this.clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.Free;
						this.ComputeValueAndDirectionFromDrag(num, num2, num7);
						return;
					}
					this.m_DragElementStartPos = new Rect(this.dragElement.resolvedStyle.translate.x, this.dragElement.resolvedStyle.translate.y, this.dragElement.resolvedStyle.width, this.dragElement.resolvedStyle.height);
				}
				bool flag5 = this.direction == SliderDirection.Horizontal;
				if (flag5)
				{
					this.ComputeValueAndDirectionFromClick(this.dragContainer.resolvedStyle.width, this.dragElement.resolvedStyle.width, this.dragElement.resolvedStyle.translate.x, this.clampedDragger.lastMousePosition.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromClick(this.dragContainer.resolvedStyle.height, this.dragElement.resolvedStyle.height, this.dragElement.resolvedStyle.translate.y, this.clampedDragger.lastMousePosition.y);
				}
			}
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			BaseSlider<TValueType>.SliderKey sliderKey = BaseSlider<TValueType>.SliderKey.None;
			bool flag = this.direction == SliderDirection.Horizontal;
			bool flag2 = (flag && evt.keyCode == KeyCode.Home) || (!flag && evt.keyCode == KeyCode.End);
			if (flag2)
			{
				sliderKey = (this.inverted ? BaseSlider<TValueType>.SliderKey.Highest : BaseSlider<TValueType>.SliderKey.Lowest);
			}
			else
			{
				bool flag3 = (flag && evt.keyCode == KeyCode.End) || (!flag && evt.keyCode == KeyCode.Home);
				if (flag3)
				{
					sliderKey = (this.inverted ? BaseSlider<TValueType>.SliderKey.Lowest : BaseSlider<TValueType>.SliderKey.Highest);
				}
				else
				{
					bool flag4 = (flag && evt.keyCode == KeyCode.PageUp) || (!flag && evt.keyCode == KeyCode.PageDown);
					if (flag4)
					{
						sliderKey = (this.inverted ? BaseSlider<TValueType>.SliderKey.HigherPage : BaseSlider<TValueType>.SliderKey.LowerPage);
					}
					else
					{
						bool flag5 = (flag && evt.keyCode == KeyCode.PageDown) || (!flag && evt.keyCode == KeyCode.PageUp);
						if (flag5)
						{
							sliderKey = (this.inverted ? BaseSlider<TValueType>.SliderKey.LowerPage : BaseSlider<TValueType>.SliderKey.HigherPage);
						}
					}
				}
			}
			bool flag6 = sliderKey == BaseSlider<TValueType>.SliderKey.None;
			if (!flag6)
			{
				this.ComputeValueFromKey(sliderKey, evt.shiftKey);
				evt.StopPropagation();
			}
		}

		private void OnNavigationMove(NavigationMoveEvent evt)
		{
			bool flag = !this.dragElement.ClassListContains(BaseSlider<TValueType>.movableUssClassName);
			if (!flag)
			{
				BaseSlider<TValueType>.SliderKey sliderKey = BaseSlider<TValueType>.SliderKey.None;
				bool flag2 = this.direction == SliderDirection.Horizontal;
				bool flag3 = evt.direction == (flag2 ? NavigationMoveEvent.Direction.Left : NavigationMoveEvent.Direction.Down);
				if (flag3)
				{
					sliderKey = (this.inverted ? BaseSlider<TValueType>.SliderKey.Higher : BaseSlider<TValueType>.SliderKey.Lower);
				}
				else
				{
					bool flag4 = evt.direction == (flag2 ? NavigationMoveEvent.Direction.Right : NavigationMoveEvent.Direction.Up);
					if (flag4)
					{
						sliderKey = (this.inverted ? BaseSlider<TValueType>.SliderKey.Lower : BaseSlider<TValueType>.SliderKey.Higher);
					}
				}
				bool flag5 = sliderKey == BaseSlider<TValueType>.SliderKey.None;
				if (!flag5)
				{
					this.ComputeValueFromKey(sliderKey, evt.shiftKey);
					evt.StopPropagation();
					FocusController focusController = this.focusController;
					if (focusController != null)
					{
						focusController.IgnoreEvent(evt);
					}
				}
			}
		}

		private void OnNavigationSubmit(NavigationSubmitEvent evt)
		{
			bool isEditingTextField = this.m_IsEditingTextField;
			if (!isEditingTextField)
			{
				this.dragElement.EnableInClassList(BaseSlider<TValueType>.movableUssClassName, !this.dragElement.ClassListContains(BaseSlider<TValueType>.movableUssClassName));
			}
		}

		internal virtual void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
		{
			float num = sliderLength - dragElementLength;
			bool flag = Mathf.Abs(num) < 1E-30f;
			if (!flag)
			{
				bool flag2 = dragElementLastPos < dragElementPos;
				bool flag3 = dragElementLastPos > dragElementPos + dragElementLength;
				bool flag4 = (this.inverted ? flag3 : flag2);
				bool flag5 = (this.inverted ? flag2 : flag3);
				this.m_AdjustedPageSizeFromClick = (this.inverted ? (this.m_AdjustedPageSizeFromClick - this.pageSize) : (this.m_AdjustedPageSizeFromClick + this.pageSize));
				bool flag6 = flag4 && this.clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.LowToHigh;
				if (flag6)
				{
					this.clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.HighToLow;
					float num2 = Mathf.Max(0f, Mathf.Min(dragElementPos - this.m_AdjustedPageSizeFromClick, num)) / num;
					this.value = this.SliderLerpDirectionalUnclamped(this.lowValue, this.highValue, num2);
				}
				else
				{
					bool flag7 = flag5 && this.clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.HighToLow;
					if (flag7)
					{
						this.clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.LowToHigh;
						float num3 = Mathf.Max(0f, Mathf.Min(dragElementPos + this.m_AdjustedPageSizeFromClick, num)) / num;
						this.value = this.SliderLerpDirectionalUnclamped(this.lowValue, this.highValue, num3);
					}
				}
			}
		}

		public void AdjustDragElement(float factor)
		{
			bool flag = factor < 1f;
			bool flag2 = flag;
			if (flag2)
			{
				this.dragElement.style.visibility = new StyleEnum<Visibility>(Visibility.Visible, StyleKeyword.Null);
				IStyle style = this.dragElement.style;
				bool flag3 = this.direction == SliderDirection.Horizontal;
				if (flag3)
				{
					float num = ((base.resolvedStyle.minWidth == StyleKeyword.Auto) ? 0f : base.resolvedStyle.minWidth.value);
					style.width = Mathf.Round(Mathf.Max(this.dragContainer.layout.width * factor, num));
				}
				else
				{
					float num2 = ((base.resolvedStyle.minHeight == StyleKeyword.Auto) ? 0f : base.resolvedStyle.minHeight.value);
					style.height = Mathf.Round(Mathf.Max(this.dragContainer.layout.height * factor, num2));
				}
			}
			else
			{
				this.dragElement.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden, StyleKeyword.Undefined);
			}
			this.dragBorderElement.visible = this.dragElement.visible;
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				this.UpdateDragElementPosition();
			}
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			this.UpdateDragElementPosition();
		}

		private bool SameValues(float a, float b, float epsilon)
		{
			return Mathf.Abs(b - a) < epsilon;
		}

		private void UpdateDragElementPosition()
		{
			bool flag = base.panel == null;
			if (!flag)
			{
				float num = this.SliderNormalizeValue(this.value, this.lowValue, this.highValue);
				float num2 = (this.inverted ? (1f - num) : num);
				float num3 = base.scaledPixelsPerPoint * 0.5f;
				bool flag2 = this.direction == SliderDirection.Horizontal;
				if (flag2)
				{
					float width = this.dragElement.resolvedStyle.width;
					float num4 = -this.dragElement.resolvedStyle.marginLeft - this.dragElement.resolvedStyle.marginRight;
					float num5 = this.dragContainer.layout.width - width + num4;
					float num6 = num2 * num5;
					bool flag3 = float.IsNaN(num6);
					if (flag3)
					{
						return;
					}
					float x = this.dragElement.resolvedStyle.translate.x;
					bool flag4 = !this.SameValues(x, num6, num3);
					if (flag4)
					{
						Vector3 vector = new Vector3(num6, 0f, 0f);
						this.dragElement.style.translate = vector;
						this.dragBorderElement.style.translate = vector;
						this.m_AdjustedPageSizeFromClick = 0f;
					}
				}
				else
				{
					float height = this.dragElement.resolvedStyle.height;
					float num7 = this.dragContainer.resolvedStyle.height - height;
					float num8 = (1f - num2) * num7;
					bool flag5 = float.IsNaN(num8);
					if (flag5)
					{
						return;
					}
					float y = this.dragElement.resolvedStyle.translate.y;
					bool flag6 = !this.SameValues(y, num8, num3);
					if (flag6)
					{
						Vector3 vector2 = new Vector3(0f, num8, 0f);
						this.dragElement.style.translate = vector2;
						this.dragBorderElement.style.translate = vector2;
						this.m_AdjustedPageSizeFromClick = 0f;
					}
				}
				this.UpdateFill(num);
			}
		}

		private void UpdateFill(float normalizedValue)
		{
			bool flag = !this.fill;
			if (!flag)
			{
				bool flag2 = this.fillElement == null;
				if (flag2)
				{
					this.fillElement = new VisualElement
					{
						name = "unity-fill",
						usageHints = UsageHints.DynamicColor
					};
					this.fillElement.AddToClassList(BaseSlider<TValueType>.fillUssClassName);
					this.trackElement.Add(this.fillElement);
				}
				float num = 1f - normalizedValue;
				Length length = Length.Percent(num * 100f);
				bool flag3 = this.direction == SliderDirection.Vertical;
				if (flag3)
				{
					this.fillElement.style.right = 0f;
					this.fillElement.style.left = 0f;
					this.fillElement.style.bottom = (this.inverted ? length : 0f);
					this.fillElement.style.top = (this.inverted ? 0f : length);
				}
				else
				{
					this.fillElement.style.top = 0f;
					this.fillElement.style.bottom = 0f;
					this.fillElement.style.left = (this.inverted ? length : 0f);
					this.fillElement.style.right = (this.inverted ? 0f : length);
				}
			}
		}

		[EventInterest(new Type[] { typeof(GeometryChangedEvent) })]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool flag = evt == null;
			if (!flag)
			{
				bool flag2 = evt.eventTypeId == EventBase<GeometryChangedEvent>.TypeId();
				if (flag2)
				{
					this.UpdateDragElementPosition((GeometryChangedEvent)evt);
				}
			}
		}

		[Obsolete("ExecuteDefaultAction override has been removed because default event handling was migrated to HandleEventBubbleUp. Please use HandleEventBubbleUp.", false)]
		[EventInterest(EventInterestOptions.Inherit)]
		protected override void ExecuteDefaultAction(EventBase evt)
		{
		}

		private void UpdateTextFieldVisibility()
		{
			bool showInputField = this.showInputField;
			if (showInputField)
			{
				bool flag = this.inputTextField == null;
				if (flag)
				{
					this.inputTextField = new TextField
					{
						name = "unity-text-field"
					};
					this.inputTextField.AddToClassList(BaseSlider<TValueType>.textFieldClassName);
					this.inputTextField.RegisterValueChangedCallback<string>(new EventCallback<ChangeEvent<string>>(this.OnTextFieldValueChange));
					this.inputTextField.RegisterCallback<FocusInEvent>(new EventCallback<FocusInEvent>(this.OnTextFieldFocusIn), TrickleDown.NoTrickleDown);
					this.inputTextField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnTextFieldFocusOut), TrickleDown.NoTrickleDown);
					base.visualInput.Add(this.inputTextField);
					this.UpdateTextFieldValue();
				}
			}
			else
			{
				bool flag2 = this.inputTextField != null && this.inputTextField.panel != null;
				if (flag2)
				{
					bool flag3 = this.inputTextField.panel != null;
					if (flag3)
					{
						this.inputTextField.RemoveFromHierarchy();
					}
					this.inputTextField.UnregisterValueChangedCallback<string>(new EventCallback<ChangeEvent<string>>(this.OnTextFieldValueChange));
					this.inputTextField.UnregisterCallback<FocusInEvent>(new EventCallback<FocusInEvent>(this.OnTextFieldFocusIn), TrickleDown.NoTrickleDown);
					this.inputTextField.UnregisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnTextFieldFocusOut), TrickleDown.NoTrickleDown);
					this.inputTextField = null;
				}
			}
		}

		private void UpdateTextFieldValue()
		{
			bool flag = this.inputTextField == null || this.m_IsEditingTextField;
			if (!flag)
			{
				this.inputTextField.SetValueWithoutNotify(string.Format(CultureInfo.InvariantCulture, "{0:g7}", this.value));
			}
		}

		private void OnFocusIn(FocusInEvent evt)
		{
			this.dragElement.AddToClassList(BaseSlider<TValueType>.movableUssClassName);
		}

		private void OnFocusOut(FocusOutEvent evt)
		{
			this.dragElement.RemoveFromClassList(BaseSlider<TValueType>.movableUssClassName);
		}

		private void OnTextFieldFocusIn(FocusInEvent evt)
		{
			this.m_IsEditingTextField = true;
		}

		private void OnTextFieldFocusOut(FocusOutEvent evt)
		{
			this.m_IsEditingTextField = false;
			this.UpdateTextFieldValue();
		}

		private void OnTextFieldValueChange(ChangeEvent<string> evt)
		{
			TValueType clampedValue = this.GetClampedValue(this.ParseStringToValue(evt.previousValue, evt.newValue));
			bool flag = !EqualityComparer<TValueType>.Default.Equals(clampedValue, this.value);
			if (flag)
			{
				this.value = clampedValue;
				evt.StopPropagation();
				bool flag2 = base.elementPanel != null;
				if (flag2)
				{
					this.OnViewDataReady();
				}
			}
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				VisualElement dragElement = this.dragElement;
				if (dragElement != null)
				{
					dragElement.RemoveFromHierarchy();
				}
				bool flag = this.inputTextField != null;
				if (flag)
				{
					this.inputTextField.showMixedValue = true;
				}
			}
			else
			{
				this.dragContainer.Add(this.dragElement);
				bool flag2 = this.inputTextField != null;
				if (flag2)
				{
					this.inputTextField.showMixedValue = false;
				}
			}
		}

		internal override void RegisterEditingCallbacks()
		{
			base.labelElement.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			this.dragContainer.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			this.dragContainer.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal override void UnregisterEditingCallbacks()
		{
			base.labelElement.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			this.dragContainer.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(base.StartEditing), TrickleDown.TrickleDown);
			this.dragContainer.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(base.EndEditing), TrickleDown.NoTrickleDown);
		}

		internal static readonly BindingId lowValueProperty = "lowValue";

		internal static readonly BindingId highValueProperty = "highValue";

		internal static readonly BindingId rangeProperty = "range";

		internal static readonly BindingId pageSizeProperty = "pageSize";

		internal static readonly BindingId showInputFieldProperty = "showInputField";

		internal static readonly BindingId directionProperty = "direction";

		internal static readonly BindingId invertedProperty = "inverted";

		internal static readonly BindingId fillProperty = "fill";

		private float m_AdjustedPageSizeFromClick = 0f;

		private bool m_IsEditingTextField;

		private bool m_Fill;

		[SerializeField]
		[DontCreateProperty]
		private TValueType m_LowValue;

		[DontCreateProperty]
		[SerializeField]
		private TValueType m_HighValue;

		private float m_PageSize;

		private bool m_ShowInputField = false;

		private Rect m_DragElementStartPos;

		private SliderDirection m_Direction;

		private bool m_Inverted = false;

		internal const float kDefaultPageSize = 0f;

		internal const bool kDefaultShowInputField = false;

		internal const bool kDefaultInverted = false;

		public new static readonly string ussClassName = "unity-base-slider";

		public new static readonly string labelUssClassName = BaseSlider<TValueType>.ussClassName + "__label";

		public new static readonly string inputUssClassName = BaseSlider<TValueType>.ussClassName + "__input";

		public static readonly string horizontalVariantUssClassName = BaseSlider<TValueType>.ussClassName + "--horizontal";

		public static readonly string verticalVariantUssClassName = BaseSlider<TValueType>.ussClassName + "--vertical";

		public static readonly string dragContainerUssClassName = BaseSlider<TValueType>.ussClassName + "__drag-container";

		public static readonly string trackerUssClassName = BaseSlider<TValueType>.ussClassName + "__tracker";

		public static readonly string draggerUssClassName = BaseSlider<TValueType>.ussClassName + "__dragger";

		public static readonly string draggerBorderUssClassName = BaseSlider<TValueType>.ussClassName + "__dragger-border";

		public static readonly string textFieldClassName = BaseSlider<TValueType>.ussClassName + "__text-field";

		public static readonly string fillUssClassName = BaseSlider<TValueType>.ussClassName + "__fill";

		public static readonly string movableUssClassName = BaseSlider<TValueType>.ussClassName + "--movable";

		internal const string k_FillElementName = "unity-fill";

		[ExcludeFromDocs]
		[Serializable]
		public new abstract class UxmlSerializedData : BaseField<TValueType>.UxmlSerializedData
		{
			public new static void Register()
			{
				BaseField<TValueType>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(BaseSlider<TValueType>.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("valueOverride", "value", null, Array.Empty<string>()),
					new UxmlAttributeNames("lowValue", "low-value", null, Array.Empty<string>()),
					new UxmlAttributeNames("highValue", "high-value", null, Array.Empty<string>()),
					new UxmlAttributeNames("fill", "fill", null, Array.Empty<string>())
				}, false);
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				BaseSlider<TValueType> baseSlider = (BaseSlider<TValueType>)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.lowValue_UxmlAttributeFlags);
				if (flag)
				{
					baseSlider.lowValue = this.lowValue;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.highValue_UxmlAttributeFlags);
				if (flag2)
				{
					baseSlider.highValue = this.highValue;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.valueOverride_UxmlAttributeFlags);
				if (flag3)
				{
					baseSlider.valueOverride = this.valueOverride;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.fill_UxmlAttributeFlags);
				if (flag4)
				{
					baseSlider.fill = this.fill;
				}
			}

			[UxmlAttributeBindingPath("value")]
			[Delayed]
			[UxmlAttribute("value")]
			[SerializeField]
			private TValueType valueOverride;

			[Delayed]
			[SerializeField]
			private TValueType lowValue;

			[Delayed]
			[SerializeField]
			private TValueType highValue;

			[SerializeField]
			private bool fill;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags valueOverride_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags lowValue_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags highValue_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags fill_UxmlAttributeFlags;
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseField<TValueType>.UxmlTraits
		{
		}

		[Obsolete("UxmlTraits<TValueUxmlAttributeType> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public class UxmlTraits<TValueUxmlAttributeType> : BaseFieldTraits<TValueType, TValueUxmlAttributeType> where TValueUxmlAttributeType : TypedUxmlAttributeDescription<TValueType>, new()
		{
			public UxmlTraits()
			{
				this.m_PickingMode.defaultValue = PickingMode.Ignore;
			}
		}

		internal enum SliderKey
		{
			None,
			Lowest,
			LowerPage,
			Lower,
			Higher,
			HigherPage,
			Highest
		}
	}
}
