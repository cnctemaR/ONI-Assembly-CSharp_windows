using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class SliderInt : BaseSlider<int>
	{
		public SliderInt()
			: this(0, 10, SliderDirection.Horizontal, 0f)
		{
		}

		public SliderInt(int start, int end, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: base(start, end, direction, pageSize)
		{
		}

		public override float pageSize
		{
			get
			{
				return base.pageSize;
			}
			set
			{
				base.pageSize = (float)Mathf.RoundToInt(value);
			}
		}

		internal override int SliderLerpUnclamped(int a, int b, float interpolant)
		{
			return Mathf.RoundToInt(Mathf.LerpUnclamped((float)a, (float)b, interpolant));
		}

		internal override float SliderNormalizeValue(int currentValue, int lowerValue, int higherValue)
		{
			return ((float)currentValue - (float)lowerValue) / ((float)higherValue - (float)lowerValue);
		}

		internal override int SliderRange()
		{
			return Math.Abs(base.highValue - base.lowValue);
		}

		internal override void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
		{
			if (Mathf.Approximately(this.pageSize, 0f))
			{
				base.ComputeValueAndDirectionFromClick(sliderLength, dragElementLength, dragElementPos, dragElementLastPos);
			}
			else
			{
				float num = sliderLength - dragElementLength;
				if (Mathf.Abs(num) >= Mathf.Epsilon)
				{
					int num2 = (int)this.pageSize;
					if (base.lowValue > base.highValue)
					{
						num2 = -num2;
					}
					if (dragElementLastPos < dragElementPos && base.clampedDragger.dragDirection != ClampedDragger<int>.DragDirection.LowToHigh)
					{
						base.clampedDragger.dragDirection = ClampedDragger<int>.DragDirection.HighToLow;
						this.value -= num2;
					}
					else if (dragElementLastPos > dragElementPos + dragElementLength && base.clampedDragger.dragDirection != ClampedDragger<int>.DragDirection.HighToLow)
					{
						base.clampedDragger.dragDirection = ClampedDragger<int>.DragDirection.LowToHigh;
						this.value += num2;
					}
				}
			}
		}

		internal const int kDefaultHighValue = 10;

		public new class UxmlFactory : UxmlFactory<SliderInt, SliderInt.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<int>.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				SliderInt sliderInt = (SliderInt)ve;
				sliderInt.lowValue = this.m_LowValue.GetValueFromBag(bag, cc);
				sliderInt.highValue = this.m_HighValue.GetValueFromBag(bag, cc);
				sliderInt.direction = this.m_Direction.GetValueFromBag(bag, cc);
				sliderInt.pageSize = (float)this.m_PageSize.GetValueFromBag(bag, cc);
				sliderInt.SetValueWithoutNotify(this.m_Value.GetValueFromBag(bag, cc));
			}

			private UxmlIntAttributeDescription m_LowValue = new UxmlIntAttributeDescription
			{
				name = "low-value"
			};

			private UxmlIntAttributeDescription m_HighValue = new UxmlIntAttributeDescription
			{
				name = "high-value",
				defaultValue = 10
			};

			private UxmlIntAttributeDescription m_PageSize = new UxmlIntAttributeDescription
			{
				name = "page-size",
				defaultValue = 0
			};

			private UxmlEnumAttributeDescription<SliderDirection> m_Direction = new UxmlEnumAttributeDescription<SliderDirection>
			{
				name = "direction",
				defaultValue = SliderDirection.Vertical
			};

			private UxmlIntAttributeDescription m_Value = new UxmlIntAttributeDescription
			{
				name = "value"
			};
		}
	}
}
