using System;

namespace UnityEngine.Experimental.UIElements
{
	public class MinMaxSlider : BaseField<Vector2>
	{
		public MinMaxSlider()
			: this(0f, 10f, float.MinValue, float.MaxValue)
		{
		}

		public MinMaxSlider(float minValue, float maxValue, float minLimit, float maxLimit)
		{
			this.m_DragState = MinMaxSlider.DragState.NoThumb;
			base.Add(new VisualElement
			{
				name = "TrackElement"
			});
			this.dragElement = new VisualElement
			{
				name = "DragElement"
			};
			this.dragElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.UpdateDragElementPosition), TrickleDown.NoTrickleDown);
			base.Add(this.dragElement);
			this.dragMinThumb = new VisualElement();
			this.dragMaxThumb = new VisualElement();
			this.dragMinThumb.AddToClassList("thumbelement");
			this.dragMaxThumb.AddToClassList("thumbelement");
			this.dragElement.Add(this.dragMinThumb);
			this.dragElement.Add(this.dragMaxThumb);
			this.clampedDragger = new ClampedDragger<float>(null, new Action(this.SetSliderValueFromClick), new Action(this.SetSliderValueFromDrag));
			this.AddManipulator(this.clampedDragger);
			this.m_MinLimit = minLimit;
			this.m_MaxLimit = maxLimit;
			this.m_Value = this.ClampValues(new Vector2(minValue, maxValue));
			this.UpdateDragElementPosition();
		}

		internal VisualElement dragElement { get; private set; }

		private VisualElement dragMinThumb { get; set; }

		private VisualElement dragMaxThumb { get; set; }

		internal ClampedDragger<float> clampedDragger { get; private set; }

		public float minValue
		{
			get
			{
				return this.value.x;
			}
			set
			{
				base.value = this.ClampValues(new Vector2(value, this.m_Value.y));
			}
		}

		public float maxValue
		{
			get
			{
				return this.value.y;
			}
			set
			{
				base.value = this.ClampValues(new Vector2(this.m_Value.x, value));
			}
		}

		public override Vector2 value
		{
			get
			{
				return base.value;
			}
			set
			{
				base.value = this.ClampValues(value);
			}
		}

		public override void SetValueWithoutNotify(Vector2 newValue)
		{
			base.SetValueWithoutNotify(this.ClampValues(newValue));
			this.UpdateDragElementPosition();
		}

		public float range
		{
			get
			{
				return Math.Abs(this.highLimit - this.lowLimit);
			}
		}

		public float lowLimit
		{
			get
			{
				return this.m_MinLimit;
			}
			set
			{
				if (!Mathf.Approximately(this.m_MinLimit, value))
				{
					if (value > this.m_MaxLimit)
					{
						throw new ArgumentException("lowLimit is greater than highLimit");
					}
					this.m_MinLimit = value;
					this.value = this.m_Value;
					this.UpdateDragElementPosition();
					if (!string.IsNullOrEmpty(base.persistenceKey))
					{
						base.SavePersistentData();
					}
				}
			}
		}

		public float highLimit
		{
			get
			{
				return this.m_MaxLimit;
			}
			set
			{
				if (!Mathf.Approximately(this.m_MaxLimit, value))
				{
					if (value < this.m_MinLimit)
					{
						throw new ArgumentException("highLimit is smaller than lowLimit");
					}
					this.m_MaxLimit = value;
					this.value = this.m_Value;
					this.UpdateDragElementPosition();
					if (!string.IsNullOrEmpty(base.persistenceKey))
					{
						base.SavePersistentData();
					}
				}
			}
		}

		private Vector2 ClampValues(Vector2 valueToClamp)
		{
			if (this.m_MinLimit > this.m_MaxLimit)
			{
				this.m_MinLimit = this.m_MaxLimit;
			}
			Vector2 vector = default(Vector2);
			if (valueToClamp.y > this.m_MaxLimit)
			{
				valueToClamp.y = this.m_MaxLimit;
			}
			vector.x = Mathf.Clamp(valueToClamp.x, this.m_MinLimit, valueToClamp.y);
			vector.y = Mathf.Clamp(valueToClamp.y, valueToClamp.x, this.m_MaxLimit);
			return vector;
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt)
		{
			if (!(evt.oldRect.size == evt.newRect.size))
			{
				this.UpdateDragElementPosition();
			}
		}

		private void UpdateDragElementPosition()
		{
			if (base.panel != null)
			{
				int num = this.dragElement.style.sliceLeft + this.dragElement.style.sliceRight;
				float num2 = Mathf.Round(this.SliderLerpUnclamped((float)this.dragElement.style.sliceLeft, base.layout.width - (float)this.dragElement.style.sliceRight, this.SliderNormalizeValue(this.minValue, this.lowLimit, this.highLimit)) - (float)this.dragElement.style.sliceLeft);
				float num3 = Mathf.Round(this.SliderLerpUnclamped((float)this.dragElement.style.sliceLeft, base.layout.width - (float)this.dragElement.style.sliceRight, this.SliderNormalizeValue(this.maxValue, this.lowLimit, this.highLimit)) + (float)this.dragElement.style.sliceRight);
				this.dragElement.style.width = Mathf.Max((float)num, num3 - num2);
				this.dragElement.style.positionLeft = num2;
				this.m_DragMinThumbRect = new Rect(this.dragElement.style.positionLeft, this.dragElement.layout.yMin, (float)this.dragElement.style.sliceLeft, this.dragElement.style.height);
				this.m_DragMaxThumbRect = new Rect(this.dragElement.style.positionLeft + (this.dragElement.style.width - (float)this.dragElement.style.sliceRight), this.dragElement.layout.yMin, (float)this.dragElement.style.sliceRight, this.dragElement.style.height);
				this.dragMaxThumb.style.positionLeft = this.dragElement.style.width - (float)this.dragElement.style.sliceRight;
				this.dragMaxThumb.style.positionTop = 0f;
				this.dragMinThumb.style.width = this.m_DragMinThumbRect.width;
				this.dragMinThumb.style.height = this.m_DragMinThumbRect.height;
				this.dragMinThumb.style.positionLeft = 0f;
				this.dragMinThumb.style.positionTop = 0f;
				this.dragMaxThumb.style.width = this.m_DragMaxThumbRect.width;
				this.dragMaxThumb.style.height = this.m_DragMaxThumbRect.height;
			}
		}

		internal float SliderLerpUnclamped(float a, float b, float interpolant)
		{
			return Mathf.LerpUnclamped(a, b, interpolant);
		}

		internal float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
		{
			return (currentValue - lowerValue) / (higherValue - lowerValue);
		}

		private float ComputeValueFromPosition(float positionToConvert)
		{
			float num = this.SliderNormalizeValue(positionToConvert, (float)this.dragElement.style.sliceLeft, base.layout.width - (float)this.dragElement.style.sliceRight);
			return this.SliderLerpUnclamped(this.lowLimit, this.highLimit, num);
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt.GetEventTypeId() == EventBase<GeometryChangedEvent>.TypeId())
			{
				this.UpdateDragElementPosition((GeometryChangedEvent)evt);
			}
		}

		private void SetSliderValueFromDrag()
		{
			if (this.clampedDragger.dragDirection == ClampedDragger<float>.DragDirection.Free)
			{
				float x = this.m_DragElementStartPos.x;
				float num = x + this.clampedDragger.delta.x;
				this.ComputeValueFromDraggingThumb(x, num);
			}
		}

		private void SetSliderValueFromClick()
		{
			if (this.clampedDragger.dragDirection != ClampedDragger<float>.DragDirection.Free)
			{
				if (this.m_DragMinThumbRect.Contains(this.clampedDragger.startMousePosition))
				{
					this.m_DragState = MinMaxSlider.DragState.MinThumb;
				}
				else if (this.m_DragMaxThumbRect.Contains(this.clampedDragger.startMousePosition))
				{
					this.m_DragState = MinMaxSlider.DragState.MaxThumb;
				}
				else if (this.dragElement.layout.Contains(this.clampedDragger.startMousePosition))
				{
					this.m_DragState = MinMaxSlider.DragState.MiddleThumb;
				}
				else
				{
					this.m_DragState = MinMaxSlider.DragState.NoThumb;
				}
				if (this.m_DragState == MinMaxSlider.DragState.NoThumb)
				{
					this.m_DragElementStartPos = new Vector2(this.clampedDragger.startMousePosition.x, this.dragElement.style.positionTop.value);
					this.clampedDragger.dragDirection = ClampedDragger<float>.DragDirection.Free;
					this.ComputeValueDragStateNoThumb((float)this.dragElement.style.sliceLeft, base.layout.width - (float)this.dragElement.style.sliceRight, this.m_DragElementStartPos.x);
					this.m_DragState = MinMaxSlider.DragState.MiddleThumb;
					this.m_ValueStartPos = this.value;
				}
				else
				{
					this.m_ValueStartPos = this.value;
					this.clampedDragger.dragDirection = ClampedDragger<float>.DragDirection.Free;
					this.m_DragElementStartPos = this.clampedDragger.startMousePosition;
				}
			}
		}

		private void ComputeValueDragStateNoThumb(float lowLimitPosition, float highLimitPosition, float dragElementPos)
		{
			float num;
			if (dragElementPos < lowLimitPosition)
			{
				num = this.lowLimit;
			}
			else if (dragElementPos > highLimitPosition)
			{
				num = this.highLimit;
			}
			else
			{
				num = this.ComputeValueFromPosition(dragElementPos);
			}
			float num2 = this.maxValue - this.minValue;
			float num3 = num - num2;
			float num4 = num;
			if (num3 < this.lowLimit)
			{
				num3 = this.lowLimit;
				num4 = num3 + num2;
			}
			this.value = new Vector2(num3, num4);
		}

		private void ComputeValueFromDraggingThumb(float dragElementStartPos, float dragElementEndPos)
		{
			float num = this.ComputeValueFromPosition(dragElementStartPos);
			float num2 = this.ComputeValueFromPosition(dragElementEndPos);
			float num3 = num2 - num;
			MinMaxSlider.DragState dragState = this.m_DragState;
			if (dragState != MinMaxSlider.DragState.MiddleThumb)
			{
				if (dragState != MinMaxSlider.DragState.MinThumb)
				{
					if (dragState == MinMaxSlider.DragState.MaxThumb)
					{
						float num4 = this.m_ValueStartPos.y + num3;
						if (num4 < this.minValue)
						{
							num4 = this.minValue;
						}
						else if (num4 > this.highLimit)
						{
							num4 = this.highLimit;
						}
						this.value = new Vector2(this.minValue, num4);
					}
				}
				else
				{
					float num5 = this.m_ValueStartPos.x + num3;
					if (num5 > this.maxValue)
					{
						num5 = this.maxValue;
					}
					else if (num5 < this.lowLimit)
					{
						num5 = this.lowLimit;
					}
					this.value = new Vector2(num5, this.maxValue);
				}
			}
			else
			{
				Vector2 value = this.value;
				value.x = this.m_ValueStartPos.x + num3;
				value.y = this.m_ValueStartPos.y + num3;
				float num6 = this.m_ValueStartPos.y - this.m_ValueStartPos.x;
				if (value.x < this.lowLimit)
				{
					value.x = this.lowLimit;
					value.y = this.lowLimit + num6;
				}
				else if (value.y > this.highLimit)
				{
					value.y = this.highLimit;
					value.x = this.highLimit - num6;
				}
				this.value = value;
			}
		}

		private Vector2 m_DragElementStartPos;

		private Vector2 m_ValueStartPos;

		private Rect m_DragMinThumbRect;

		private Rect m_DragMaxThumbRect;

		private MinMaxSlider.DragState m_DragState;

		private float m_MinLimit;

		private float m_MaxLimit;

		internal const float kDefaultHighValue = 10f;

		public new class UxmlFactory : UxmlFactory<MinMaxSlider, MinMaxSlider.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<Vector2>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				MinMaxSlider minMaxSlider = (MinMaxSlider)ve;
				minMaxSlider.SetValueWithoutNotify(new Vector2(this.m_MinValue.GetValueFromBag(bag, cc), this.m_MaxValue.GetValueFromBag(bag, cc)));
				minMaxSlider.lowLimit = this.m_LowLimit.GetValueFromBag(bag, cc);
				minMaxSlider.highLimit = this.m_HighLimit.GetValueFromBag(bag, cc);
			}

			private UxmlFloatAttributeDescription m_MinValue = new UxmlFloatAttributeDescription
			{
				name = "min-value",
				defaultValue = 0f
			};

			private UxmlFloatAttributeDescription m_MaxValue = new UxmlFloatAttributeDescription
			{
				name = "max-value",
				defaultValue = 10f
			};

			private UxmlFloatAttributeDescription m_LowLimit = new UxmlFloatAttributeDescription
			{
				name = "low-limit",
				defaultValue = float.MinValue
			};

			private UxmlFloatAttributeDescription m_HighLimit = new UxmlFloatAttributeDescription
			{
				name = "high-limit",
				defaultValue = float.MaxValue
			};
		}

		private enum DragState
		{
			NoThumb,
			MinThumb,
			MiddleThumb,
			MaxThumb
		}
	}
}
