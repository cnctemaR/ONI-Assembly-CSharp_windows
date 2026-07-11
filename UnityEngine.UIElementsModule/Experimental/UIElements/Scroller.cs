using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements
{
	public class Scroller : VisualElement
	{
		public Scroller()
			: this(0f, 0f, null, SliderDirection.Vertical)
		{
		}

		public Scroller(float lowValue, float highValue, Action<float> valueChanged, SliderDirection direction = SliderDirection.Vertical)
		{
			this.direction = direction;
			this.valueChanged = valueChanged;
			this.slider = new Slider(lowValue, highValue, direction, 20f)
			{
				name = "Slider",
				persistenceKey = "Slider"
			};
			this.slider.OnValueChanged(new EventCallback<ChangeEvent<float>>(this.OnSliderValueChange));
			base.Add(this.slider);
			this.lowButton = new ScrollerButton(new Action(this.ScrollPageUp), 250L, 30L)
			{
				name = "LowButton"
			};
			base.Add(this.lowButton);
			this.highButton = new ScrollerButton(new Action(this.ScrollPageDown), 250L, 30L)
			{
				name = "HighButton"
			};
			base.Add(this.highButton);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<float> valueChanged;

		public Slider slider { get; private set; }

		public ScrollerButton lowButton { get; private set; }

		public ScrollerButton highButton { get; private set; }

		public float value
		{
			get
			{
				return this.slider.value;
			}
			set
			{
				this.slider.value = value;
			}
		}

		public float lowValue
		{
			get
			{
				return this.slider.lowValue;
			}
			set
			{
				this.slider.lowValue = value;
			}
		}

		public float highValue
		{
			get
			{
				return this.slider.highValue;
			}
			set
			{
				this.slider.highValue = value;
			}
		}

		public SliderDirection direction
		{
			get
			{
				return (base.style.flexDirection != FlexDirection.Row) ? SliderDirection.Vertical : SliderDirection.Horizontal;
			}
			set
			{
				if (value == SliderDirection.Horizontal)
				{
					base.style.flexDirection = FlexDirection.Row;
					base.AddToClassList("horizontal");
				}
				else
				{
					base.style.flexDirection = FlexDirection.Column;
					base.AddToClassList("vertical");
				}
			}
		}

		public void Adjust(float factor)
		{
			base.SetEnabled(factor < 1f);
			this.slider.AdjustDragElement(factor);
		}

		private void OnSliderValueChange(ChangeEvent<float> evt)
		{
			this.value = evt.newValue;
			if (this.valueChanged != null)
			{
				this.valueChanged(this.slider.value);
			}
			base.IncrementVersion(VersionChangeType.Repaint);
		}

		public void ScrollPageUp()
		{
			this.ScrollPageUp(1f);
		}

		public void ScrollPageDown()
		{
			this.ScrollPageDown(1f);
		}

		public void ScrollPageUp(float factor)
		{
			this.value -= factor * (this.slider.pageSize * ((this.slider.lowValue >= this.slider.highValue) ? (-1f) : 1f));
		}

		public void ScrollPageDown(float factor)
		{
			this.value += factor * (this.slider.pageSize * ((this.slider.lowValue >= this.slider.highValue) ? (-1f) : 1f));
		}

		internal const float kDefaultPageSize = 20f;

		public new class UxmlFactory : UxmlFactory<Scroller, Scroller.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
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
				Scroller scroller = (Scroller)ve;
				scroller.slider.lowValue = this.m_LowValue.GetValueFromBag(bag, cc);
				scroller.slider.highValue = this.m_HighValue.GetValueFromBag(bag, cc);
				scroller.direction = this.m_Direction.GetValueFromBag(bag, cc);
				scroller.value = this.m_Value.GetValueFromBag(bag, cc);
			}

			private UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
			{
				name = "low-value",
				obsoleteNames = new string[] { "lowValue" }
			};

			private UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
			{
				name = "high-value",
				obsoleteNames = new string[] { "highValue" }
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
