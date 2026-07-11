using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public abstract class BaseSlider<TValueType> : BaseField<TValueType> where TValueType : IComparable<TValueType>
	{
		internal VisualElement dragElement { get; private set; }

		internal VisualElement dragBorderElement { get; private set; }

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
				}
			}
		}

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
				}
			}
		}

		public TValueType range
		{
			get
			{
				return this.SliderRange();
			}
		}

		public virtual float pageSize
		{
			get
			{
				return this.m_PageSize;
			}
			set
			{
				this.m_PageSize = value;
			}
		}

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
				TValueType clampedValue = this.GetClampedValue(value);
				base.value = clampedValue;
			}
		}

		public override void SetValueWithoutNotify(TValueType newValue)
		{
			TValueType clampedValue = this.GetClampedValue(newValue);
			base.SetValueWithoutNotify(clampedValue);
			this.UpdateDragElementPosition();
		}

		public SliderDirection direction
		{
			get
			{
				return this.m_Direction;
			}
			set
			{
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
			base.visualInput.pickingMode = PickingMode.Position;
			VisualElement visualElement = new VisualElement
			{
				name = "unity-tracker"
			};
			visualElement.AddToClassList(BaseSlider<TValueType>.trackerUssClassName);
			base.visualInput.Add(visualElement);
			this.dragBorderElement = new VisualElement
			{
				name = "unity-dragger-border"
			};
			this.dragBorderElement.AddToClassList(BaseSlider<TValueType>.draggerBorderUssClassName);
			base.visualInput.Add(this.dragBorderElement);
			this.dragElement = new VisualElement
			{
				name = "unity-dragger"
			};
			this.dragElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.UpdateDragElementPosition), TrickleDown.NoTrickleDown);
			this.dragElement.AddToClassList(BaseSlider<TValueType>.draggerUssClassName);
			base.visualInput.Add(this.dragElement);
			this.clampedDragger = new ClampedDragger<TValueType>(this, new Action(this.SetSliderValueFromClick), new Action(this.SetSliderValueFromDrag));
			base.visualInput.AddManipulator(this.clampedDragger);
		}

		private void ClampValue()
		{
			this.value = base.rawValue;
		}

		internal abstract TValueType SliderLerpUnclamped(TValueType a, TValueType b, float interpolant);

		internal abstract float SliderNormalizeValue(TValueType currentValue, TValueType lowerValue, TValueType higherValue);

		internal abstract TValueType SliderRange();

		private void SetSliderValueFromDrag()
		{
			bool flag = this.clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.Free;
			if (!flag)
			{
				Vector2 delta = this.clampedDragger.delta;
				bool flag2 = this.direction == SliderDirection.Horizontal;
				if (flag2)
				{
					this.ComputeValueAndDirectionFromDrag(base.visualInput.resolvedStyle.width, this.dragElement.resolvedStyle.width, this.m_DragElementStartPos.x + delta.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromDrag(base.visualInput.resolvedStyle.height, this.dragElement.resolvedStyle.height, this.m_DragElementStartPos.y + delta.y);
				}
			}
		}

		private void ComputeValueAndDirectionFromDrag(float sliderLength, float dragElementLength, float dragElementPos)
		{
			float num = sliderLength - dragElementLength;
			bool flag = Mathf.Abs(num) < Mathf.Epsilon;
			if (!flag)
			{
				float num2 = Mathf.Max(0f, Mathf.Min(dragElementPos, num)) / num;
				this.value = this.SliderLerpUnclamped(this.lowValue, this.highValue, num2);
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
						float num = ((this.direction == SliderDirection.Horizontal) ? (this.clampedDragger.startMousePosition.x - this.dragElement.resolvedStyle.width / 2f) : this.dragElement.transform.position.x);
						float num2 = ((this.direction == SliderDirection.Horizontal) ? this.dragElement.transform.position.y : (this.clampedDragger.startMousePosition.y - this.dragElement.resolvedStyle.height / 2f));
						Vector3 vector = new Vector3(num, num2, 0f);
						this.dragElement.transform.position = vector;
						this.dragBorderElement.transform.position = vector;
						this.m_DragElementStartPos = new Rect(num, num2, this.dragElement.resolvedStyle.width, this.dragElement.resolvedStyle.height);
						this.clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.Free;
						bool flag4 = this.direction == SliderDirection.Horizontal;
						if (flag4)
						{
							this.ComputeValueAndDirectionFromDrag(base.visualInput.resolvedStyle.width, this.dragElement.resolvedStyle.width, this.m_DragElementStartPos.x);
						}
						else
						{
							this.ComputeValueAndDirectionFromDrag(base.visualInput.resolvedStyle.height, this.dragElement.resolvedStyle.height, this.m_DragElementStartPos.y);
						}
						return;
					}
					this.m_DragElementStartPos = new Rect(this.dragElement.transform.position.x, this.dragElement.transform.position.y, this.dragElement.resolvedStyle.width, this.dragElement.resolvedStyle.height);
				}
				bool flag5 = this.direction == SliderDirection.Horizontal;
				if (flag5)
				{
					this.ComputeValueAndDirectionFromClick(base.visualInput.resolvedStyle.width, this.dragElement.resolvedStyle.width, this.dragElement.transform.position.x, this.clampedDragger.lastMousePosition.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromClick(base.visualInput.resolvedStyle.height, this.dragElement.resolvedStyle.height, this.dragElement.transform.position.y, this.clampedDragger.lastMousePosition.y);
				}
			}
		}

		internal virtual void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
		{
			float num = sliderLength - dragElementLength;
			bool flag = Mathf.Abs(num) < Mathf.Epsilon;
			if (!flag)
			{
				bool flag2 = dragElementLastPos < dragElementPos && this.clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.LowToHigh;
				if (flag2)
				{
					this.clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.HighToLow;
					float num2 = Mathf.Max(0f, Mathf.Min(dragElementPos - this.pageSize, num)) / num;
					this.value = this.SliderLerpUnclamped(this.lowValue, this.highValue, num2);
				}
				else
				{
					bool flag3 = dragElementLastPos > dragElementPos + dragElementLength && this.clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.HighToLow;
					if (flag3)
					{
						this.clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.LowToHigh;
						float num3 = Mathf.Max(0f, Mathf.Min(dragElementPos + this.pageSize, num)) / num;
						this.value = this.SliderLerpUnclamped(this.lowValue, this.highValue, num3);
					}
				}
			}
		}

		public void AdjustDragElement(float factor)
		{
			bool flag = factor < 1f;
			this.dragElement.visible = flag;
			bool flag2 = flag;
			if (flag2)
			{
				IStyle style = this.dragElement.style;
				this.dragElement.visible = true;
				bool flag3 = this.direction == SliderDirection.Horizontal;
				if (flag3)
				{
					float num = ((base.resolvedStyle.minWidth == StyleKeyword.Auto) ? 0f : base.resolvedStyle.minWidth.value);
					style.width = Mathf.Round(Mathf.Max(base.visualInput.layout.width * factor, num));
				}
				else
				{
					float num2 = ((base.resolvedStyle.minHeight == StyleKeyword.Auto) ? 0f : base.resolvedStyle.minHeight.value);
					style.height = Mathf.Round(Mathf.Max(base.visualInput.layout.height * factor, num2));
				}
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
				bool flag2 = this.direction == SliderDirection.Horizontal;
				if (flag2)
				{
					float width = this.dragElement.resolvedStyle.width;
					float num2 = -this.dragElement.resolvedStyle.marginLeft - this.dragElement.resolvedStyle.marginRight;
					float num3 = base.visualInput.layout.width - width + num2;
					float num4 = num * num3;
					bool flag3 = float.IsNaN(num4);
					if (!flag3)
					{
						float num5 = base.scaledPixelsPerPoint * 0.5f;
						float x = this.dragElement.transform.position.x;
						bool flag4 = !this.SameValues(x, num4, num5);
						if (flag4)
						{
							Vector3 vector = new Vector3(num4, 0f, 0f);
							this.dragElement.transform.position = vector;
							this.dragBorderElement.transform.position = vector;
						}
					}
				}
				else
				{
					float height = this.dragElement.resolvedStyle.height;
					float num6 = base.visualInput.resolvedStyle.height - height;
					float num7 = num * num6;
					bool flag5 = float.IsNaN(num7);
					if (!flag5)
					{
						float num8 = base.scaledPixelsPerPoint * 0.5f;
						float y = this.dragElement.transform.position.y;
						bool flag6 = !this.SameValues(y, num7, num8);
						if (flag6)
						{
							Vector3 vector2 = new Vector3(0f, num7, 0f);
							this.dragElement.transform.position = vector2;
							this.dragBorderElement.transform.position = vector2;
						}
					}
				}
			}
		}

		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
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

		[SerializeField]
		private TValueType m_LowValue;

		[SerializeField]
		private TValueType m_HighValue;

		private float m_PageSize;

		private Rect m_DragElementStartPos;

		private SliderDirection m_Direction;

		internal const float kDefaultPageSize = 0f;

		public new static readonly string ussClassName = "unity-base-slider";

		public new static readonly string labelUssClassName = BaseSlider<TValueType>.ussClassName + "__label";

		public new static readonly string inputUssClassName = BaseSlider<TValueType>.ussClassName + "__input";

		public static readonly string horizontalVariantUssClassName = BaseSlider<TValueType>.ussClassName + "--horizontal";

		public static readonly string verticalVariantUssClassName = BaseSlider<TValueType>.ussClassName + "--vertical";

		public static readonly string trackerUssClassName = BaseSlider<TValueType>.ussClassName + "__tracker";

		public static readonly string draggerUssClassName = BaseSlider<TValueType>.ussClassName + "__dragger";

		public static readonly string draggerBorderUssClassName = BaseSlider<TValueType>.ussClassName + "__dragger-border";
	}
}
