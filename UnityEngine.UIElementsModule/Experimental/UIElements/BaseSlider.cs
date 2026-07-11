using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class BaseSlider<T> : BaseField<T> where T : IComparable<T>
	{
		public BaseSlider(T start, T end, SliderDirection direction, float pageSize = 0f)
		{
			this.direction = direction;
			this.pageSize = pageSize;
			this.lowValue = start;
			this.highValue = end;
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
			this.clampedDragger = new ClampedDragger<T>(this, new Action(this.SetSliderValueFromClick), new Action(this.SetSliderValueFromDrag));
			this.AddManipulator(this.clampedDragger);
		}

		internal VisualElement dragElement { get; private set; }

		public T lowValue
		{
			get
			{
				return this.m_LowValue;
			}
			set
			{
				if (!EqualityComparer<T>.Default.Equals(this.m_LowValue, value))
				{
					this.m_LowValue = value;
					this.ClampValue();
					this.UpdateDragElementPosition();
				}
			}
		}

		public T highValue
		{
			get
			{
				return this.m_HighValue;
			}
			set
			{
				if (!EqualityComparer<T>.Default.Equals(this.m_HighValue, value))
				{
					this.m_HighValue = value;
					this.ClampValue();
					this.UpdateDragElementPosition();
				}
			}
		}

		public T range
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

		internal ClampedDragger<T> clampedDragger { get; private set; }

		private T Clamp(T value, T lowBound, T highBound)
		{
			T t = value;
			if (lowBound.CompareTo(value) > 0)
			{
				t = lowBound;
			}
			else if (highBound.CompareTo(value) < 0)
			{
				t = highBound;
			}
			return t;
		}

		public override T value
		{
			get
			{
				return base.value;
			}
			set
			{
				T t = this.lowValue;
				T t2 = this.highValue;
				if (t.CompareTo(t2) > 0)
				{
					T t3 = t;
					t = t2;
					t2 = t3;
				}
				T t4 = this.Clamp(value, t, t2);
				base.value = t4;
				this.UpdateDragElementPosition();
			}
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
				if (this.m_Direction == SliderDirection.Horizontal)
				{
					base.RemoveFromClassList("vertical");
					base.AddToClassList("horizontal");
				}
				else
				{
					base.RemoveFromClassList("horizontal");
					base.AddToClassList("vertical");
				}
			}
		}

		private void ClampValue()
		{
			this.value = this.m_Value;
		}

		internal abstract T SliderLerpUnclamped(T a, T b, float interpolant);

		internal abstract float SliderNormalizeValue(T currentValue, T lowerValue, T higherValue);

		internal abstract T SliderRange();

		private void SetSliderValueFromDrag()
		{
			if (this.clampedDragger.dragDirection == ClampedDragger<T>.DragDirection.Free)
			{
				Vector2 delta = this.clampedDragger.delta;
				if (this.direction == SliderDirection.Horizontal)
				{
					this.ComputeValueAndDirectionFromDrag(base.layout.width, this.dragElement.style.width, this.m_DragElementStartPos.x + delta.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromDrag(base.layout.height, this.dragElement.style.height, this.m_DragElementStartPos.y + delta.y);
				}
			}
		}

		private void ComputeValueAndDirectionFromDrag(float sliderLength, float dragElementLength, float dragElementPos)
		{
			float num = sliderLength - dragElementLength;
			if (Mathf.Abs(num) >= Mathf.Epsilon)
			{
				float num2 = Mathf.Max(0f, Mathf.Min(dragElementPos, num)) / num;
				this.value = this.SliderLerpUnclamped(this.lowValue, this.highValue, num2);
			}
		}

		private void SetSliderValueFromClick()
		{
			if (this.clampedDragger.dragDirection != ClampedDragger<T>.DragDirection.Free)
			{
				if (this.clampedDragger.dragDirection == ClampedDragger<T>.DragDirection.None)
				{
					if (Mathf.Approximately(this.pageSize, 0f))
					{
						float num = ((this.direction != SliderDirection.Horizontal) ? this.dragElement.style.positionLeft.value : (this.clampedDragger.startMousePosition.x - this.dragElement.style.width / 2f));
						float num2 = ((this.direction != SliderDirection.Horizontal) ? (this.clampedDragger.startMousePosition.y - this.dragElement.style.height / 2f) : this.dragElement.style.positionTop.value);
						this.dragElement.style.positionLeft = num;
						this.dragElement.style.positionTop = num2;
						this.m_DragElementStartPos = new Rect(num, num2, this.dragElement.style.width, this.dragElement.style.height);
						this.clampedDragger.dragDirection = ClampedDragger<T>.DragDirection.Free;
						if (this.direction == SliderDirection.Horizontal)
						{
							this.ComputeValueAndDirectionFromDrag(base.layout.width, this.dragElement.style.width, this.m_DragElementStartPos.x);
						}
						else
						{
							this.ComputeValueAndDirectionFromDrag(base.layout.height, this.dragElement.style.height, this.m_DragElementStartPos.y);
						}
						return;
					}
					this.m_DragElementStartPos = new Rect(this.dragElement.style.positionLeft, this.dragElement.style.positionTop, this.dragElement.style.width, this.dragElement.style.height);
				}
				if (this.direction == SliderDirection.Horizontal)
				{
					this.ComputeValueAndDirectionFromClick(base.layout.width, this.dragElement.style.width, this.dragElement.style.positionLeft, this.clampedDragger.lastMousePosition.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromClick(base.layout.height, this.dragElement.style.height, this.dragElement.style.positionTop, this.clampedDragger.lastMousePosition.y);
				}
			}
		}

		internal virtual void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
		{
			float num = sliderLength - dragElementLength;
			if (Mathf.Abs(num) >= Mathf.Epsilon)
			{
				if (dragElementLastPos < dragElementPos && this.clampedDragger.dragDirection != ClampedDragger<T>.DragDirection.LowToHigh)
				{
					this.clampedDragger.dragDirection = ClampedDragger<T>.DragDirection.HighToLow;
					float num2 = Mathf.Max(0f, Mathf.Min(dragElementPos - this.pageSize, num)) / num;
					this.value = this.SliderLerpUnclamped(this.lowValue, this.highValue, num2);
				}
				else if (dragElementLastPos > dragElementPos + dragElementLength && this.clampedDragger.dragDirection != ClampedDragger<T>.DragDirection.HighToLow)
				{
					this.clampedDragger.dragDirection = ClampedDragger<T>.DragDirection.LowToHigh;
					float num3 = Mathf.Max(0f, Mathf.Min(dragElementPos + this.pageSize, num)) / num;
					this.value = this.SliderLerpUnclamped(this.lowValue, this.highValue, num3);
				}
			}
		}

		public void AdjustDragElement(float factor)
		{
			bool flag = factor < 1f;
			this.dragElement.visible = flag;
			if (flag)
			{
				IStyle style = this.dragElement.style;
				this.dragElement.visible = true;
				if (this.direction == SliderDirection.Horizontal)
				{
					float specifiedValueOrDefault = style.minWidth.GetSpecifiedValueOrDefault(0f);
					style.width = Mathf.Max(base.layout.width * factor, specifiedValueOrDefault);
				}
				else
				{
					float specifiedValueOrDefault2 = style.minHeight.GetSpecifiedValueOrDefault(0f);
					style.height = Mathf.Max(base.layout.height * factor, specifiedValueOrDefault2);
				}
			}
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt)
		{
			if (!(evt.oldRect.size == evt.newRect.size))
			{
				this.UpdateDragElementPosition();
			}
		}

		public override void OnPersistentDataReady()
		{
			base.OnPersistentDataReady();
			this.UpdateDragElementPosition();
		}

		private void UpdateDragElementPosition()
		{
			if (base.panel != null)
			{
				float num = this.SliderNormalizeValue(this.value, this.lowValue, this.highValue);
				float num2 = this.dragElement.style.width;
				float num3 = this.dragElement.style.height;
				if (this.direction == SliderDirection.Horizontal)
				{
					float num4 = base.layout.width - num2;
					this.dragElement.style.positionLeft = num * num4;
				}
				else
				{
					float num5 = base.layout.height - num3;
					this.dragElement.style.positionTop = num * num5;
				}
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt.GetEventTypeId() == EventBase<GeometryChangedEvent>.TypeId())
			{
				this.UpdateDragElementPosition((GeometryChangedEvent)evt);
			}
		}

		private T m_LowValue;

		private T m_HighValue;

		private float m_PageSize;

		private Rect m_DragElementStartPos;

		private SliderDirection m_Direction;

		internal const float kDefaultPageSize = 0f;
	}
}
