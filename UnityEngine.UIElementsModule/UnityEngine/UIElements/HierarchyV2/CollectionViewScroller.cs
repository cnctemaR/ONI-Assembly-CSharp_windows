using System;
using Unity.Properties;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class CollectionViewScroller : VisualElement, INotifyValueChanged<double>
	{
		private ScrollerSlider slider { get; }

		private RepeatButton lowButton { get; }

		private RepeatButton highButton { get; }

		public void SetValueWithoutNotify(double newValue)
		{
			this.slider.SetValueWithoutNotify(newValue);
		}

		double INotifyValueChanged<double>.value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		[CreateProperty]
		public double value
		{
			get
			{
				return this.slider.value;
			}
			set
			{
				bool flag = !base.enabledSelf;
				if (!flag)
				{
					double value2 = this.slider.value;
					this.slider.value = value;
					bool flag2 = !this.Approximately(value2, this.slider.value);
					if (flag2)
					{
						base.NotifyPropertyChanged(in CollectionViewScroller.valueProperty);
					}
				}
			}
		}

		[CreateProperty]
		public double lowValue
		{
			get
			{
				return this.slider.lowValue;
			}
			set
			{
				double lowValue = this.slider.lowValue;
				this.slider.lowValue = value;
				bool flag = !this.Approximately(lowValue, this.slider.lowValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in CollectionViewScroller.lowValueProperty);
				}
			}
		}

		[CreateProperty]
		public double highValue
		{
			get
			{
				return this.slider.highValue;
			}
			set
			{
				double highValue = this.slider.highValue;
				this.slider.highValue = value;
				bool flag = !this.Approximately(highValue, this.slider.highValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in CollectionViewScroller.highValueProperty);
				}
			}
		}

		[CreateProperty]
		public SliderDirection direction
		{
			get
			{
				return (base.resolvedStyle.flexDirection == FlexDirection.Row) ? SliderDirection.Horizontal : SliderDirection.Vertical;
			}
			set
			{
				SliderDirection direction = this.slider.direction;
				this.slider.direction = value;
				this.slider.inverted = value == SliderDirection.Vertical;
				bool flag = value == SliderDirection.Horizontal;
				if (flag)
				{
					base.style.flexDirection = FlexDirection.Row;
					base.AddToClassList(Scroller.horizontalVariantUssClassName);
					base.RemoveFromClassList(Scroller.verticalVariantUssClassName);
				}
				else
				{
					base.style.flexDirection = FlexDirection.Column;
					base.AddToClassList(Scroller.verticalVariantUssClassName);
					base.RemoveFromClassList(Scroller.horizontalVariantUssClassName);
				}
				bool flag2 = direction != this.slider.direction;
				if (flag2)
				{
					base.NotifyPropertyChanged(in CollectionViewScroller.directionProperty);
				}
			}
		}

		public CollectionViewScroller()
			: this(0.0, 0.0, SliderDirection.Vertical)
		{
		}

		public CollectionViewScroller(double lowValue, double highValue, SliderDirection direction = SliderDirection.Vertical)
		{
			this.scrollSize = 20.0;
			base.AddToClassList(Scroller.ussClassName);
			this.slider = new ScrollerSlider(lowValue, highValue, direction, 20f)
			{
				name = "unity-slider",
				viewDataKey = "Slider"
			};
			this.slider.AddToClassList(Scroller.sliderUssClassName);
			this.lowButton = new RepeatButton(new Action(this.ScrollPageUp), 250L, 30L)
			{
				name = "unity-low-button"
			};
			this.lowButton.AddToClassList(Scroller.lowButtonUssClassName);
			base.Add(this.lowButton);
			this.highButton = new RepeatButton(new Action(this.ScrollPageDown), 250L, 30L)
			{
				name = "unity-high-button"
			};
			this.highButton.AddToClassList(Scroller.highButtonUssClassName);
			base.Add(this.highButton);
			base.Add(this.slider);
			this.direction = direction;
		}

		public void Adjust(float factor)
		{
			base.SetEnabled(factor < 1f);
			this.slider.AdjustDragElement(factor);
		}

		public double scrollSize { get; set; }

		public void ScrollPageUp()
		{
			this.ScrollPage(-1.0);
		}

		public void ScrollPageDown()
		{
			this.ScrollPage(1.0);
		}

		public void ScrollPage(double factor)
		{
			this.value += factor * (this.scrollSize * (double)((this.slider.lowValue < this.slider.highValue) ? 1f : (-1f)));
		}

		public bool Approximately(double a, double b)
		{
			double num = Math.Abs(a - b);
			return num < 8E-323;
		}

		internal static readonly BindingId valueProperty = "value";

		internal static readonly BindingId lowValueProperty = "lowValue";

		internal static readonly BindingId highValueProperty = "highValue";

		internal static readonly BindingId directionProperty = "direction";

		private const float k_DefaultPageSize = 20f;

		private const double k_closeEnoughEpsilon = 8E-323;
	}
}
