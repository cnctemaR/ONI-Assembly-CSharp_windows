using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class Slider : BaseSlider<float>
	{
		public Slider()
			: this(0f, 10f, SliderDirection.Horizontal, 0f)
		{
		}

		public Slider(float start, float end, Action<float> valueChanged, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: this(start, end, direction, pageSize)
		{
			this.valueChanged = valueChanged;
		}

		public Slider(float start, float end, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
			: base(start, end, direction, pageSize)
		{
		}

		public Action<float> valueChanged
		{
			get
			{
				return this.m_ValueChanged;
			}
			set
			{
				if (value != null && this.m_ValueChanged == null)
				{
					base.OnValueChanged(new EventCallback<ChangeEvent<float>>(this.InternalOnValueChanged));
				}
				else if (value == null && this.m_ValueChanged != null)
				{
					base.UnregisterCallback<ChangeEvent<float>>(new EventCallback<ChangeEvent<float>>(this.InternalOnValueChanged), TrickleDown.NoTrickleDown);
				}
				this.m_ValueChanged = value;
			}
		}

		private void InternalOnValueChanged(ChangeEvent<float> evt)
		{
			if (this.m_ValueChanged != null)
			{
				this.m_ValueChanged(this.value);
			}
		}

		internal override float SliderLerpUnclamped(float a, float b, float interpolant)
		{
			return Mathf.LerpUnclamped(a, b, interpolant);
		}

		internal override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
		{
			return (currentValue - lowerValue) / (higherValue - lowerValue);
		}

		internal override float SliderRange()
		{
			return Math.Abs(base.highValue - base.lowValue);
		}

		private Action<float> m_ValueChanged;

		internal const float kDefaultHighValue = 10f;

		public new class UxmlFactory : UxmlFactory<Slider, Slider.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseField<float>.UxmlTraits
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
				Slider slider = (Slider)ve;
				slider.lowValue = this.m_LowValue.GetValueFromBag(bag, cc);
				slider.highValue = this.m_HighValue.GetValueFromBag(bag, cc);
				slider.direction = this.m_Direction.GetValueFromBag(bag, cc);
				slider.pageSize = this.m_PageSize.GetValueFromBag(bag, cc);
				slider.SetValueWithoutNotify(this.m_Value.GetValueFromBag(bag, cc));
			}

			private UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
			{
				name = "low-value",
				obsoleteNames = new string[] { "lowValue" }
			};

			private UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
			{
				name = "high-value",
				obsoleteNames = new string[] { "highValue" },
				defaultValue = 10f
			};

			private UxmlFloatAttributeDescription m_PageSize = new UxmlFloatAttributeDescription
			{
				name = "page-size",
				obsoleteNames = new string[] { "pageSize" },
				defaultValue = 0f
			};

			private UxmlEnumAttributeDescription<SliderDirection> m_Direction = new UxmlEnumAttributeDescription<SliderDirection>
			{
				name = "direction",
				defaultValue = SliderDirection.Vertical
			};

			private UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
			{
				name = "value"
			};
		}
	}
}
