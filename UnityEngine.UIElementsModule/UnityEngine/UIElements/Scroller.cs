using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class Scroller : VisualElement
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<float> valueChanged;

		public Slider slider { get; }

		public RepeatButton lowButton { get; }

		public RepeatButton highButton { get; }

		[CreateProperty]
		public float value
		{
			get
			{
				return this.slider.value;
			}
			set
			{
				float value2 = this.slider.value;
				this.slider.value = value;
				bool flag = !Mathf.Approximately(value2, this.slider.value);
				if (flag)
				{
					base.NotifyPropertyChanged(in Scroller.valueProperty);
				}
			}
		}

		[CreateProperty]
		public float lowValue
		{
			get
			{
				return this.slider.lowValue;
			}
			set
			{
				float lowValue = this.slider.lowValue;
				this.slider.lowValue = value;
				bool flag = !Mathf.Approximately(lowValue, this.slider.lowValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in Scroller.lowValueProperty);
				}
			}
		}

		[CreateProperty]
		public float highValue
		{
			get
			{
				return this.slider.highValue;
			}
			set
			{
				float highValue = this.slider.highValue;
				this.slider.highValue = value;
				bool flag = !Mathf.Approximately(highValue, this.slider.highValue);
				if (flag)
				{
					base.NotifyPropertyChanged(in Scroller.highValueProperty);
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
					base.NotifyPropertyChanged(in Scroller.directionProperty);
				}
			}
		}

		public Scroller()
			: this(0f, 0f, null, SliderDirection.Vertical)
		{
		}

		public Scroller(float lowValue, float highValue, Action<float> valueChanged, SliderDirection direction = SliderDirection.Vertical)
		{
			base.AddToClassList(Scroller.ussClassName);
			this.slider = new Scroller.ScrollerSlider(lowValue, highValue, direction, 20f)
			{
				name = "unity-slider",
				viewDataKey = "Slider"
			};
			this.slider.AddToClassList(Scroller.sliderUssClassName);
			this.slider.RegisterValueChangedCallback<float>(new EventCallback<ChangeEvent<float>>(this.OnSliderValueChange));
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
			this.valueChanged = valueChanged;
		}

		public void Adjust(float factor)
		{
			base.SetEnabled(factor < 1f);
			this.slider.AdjustDragElement(factor);
		}

		private void OnSliderValueChange(ChangeEvent<float> evt)
		{
			this.value = evt.newValue;
			Action<float> action = this.valueChanged;
			if (action != null)
			{
				action(this.slider.value);
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
			this.value -= factor * (this.slider.pageSize * ((this.slider.lowValue < this.slider.highValue) ? 1f : (-1f)));
		}

		public void ScrollPageDown(float factor)
		{
			this.value += factor * (this.slider.pageSize * ((this.slider.lowValue < this.slider.highValue) ? 1f : (-1f)));
		}

		internal static readonly BindingId valueProperty = "value";

		internal static readonly BindingId lowValueProperty = "lowValue";

		internal static readonly BindingId highValueProperty = "highValue";

		internal static readonly BindingId directionProperty = "direction";

		internal const float kDefaultPageSize = 20f;

		public static readonly string ussClassName = "unity-scroller";

		public static readonly string horizontalVariantUssClassName = Scroller.ussClassName + "--horizontal";

		public static readonly string verticalVariantUssClassName = Scroller.ussClassName + "--vertical";

		public static readonly string sliderUssClassName = Scroller.ussClassName + "__slider";

		public static readonly string lowButtonUssClassName = Scroller.ussClassName + "__low-button";

		public static readonly string highButtonUssClassName = Scroller.ussClassName + "__high-button";

		private class ScrollerSlider : Slider
		{
			public ScrollerSlider(float start, float end, SliderDirection direction, float pageSize)
				: base(start, end, direction, pageSize)
			{
			}

			internal override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
			{
				return Mathf.Clamp(base.SliderNormalizeValue(currentValue, lowerValue, higherValue), 0f, 1f);
			}
		}

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Scroller.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("lowValue", "low-value", null, new string[] { "lowValue" }),
					new UxmlAttributeNames("highValue", "high-value", null, new string[] { "highValue" }),
					new UxmlAttributeNames("direction", "direction", null, Array.Empty<string>()),
					new UxmlAttributeNames("value", "value", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Scroller();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				Scroller scroller = (Scroller)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.lowValue_UxmlAttributeFlags);
				if (flag)
				{
					scroller.slider.lowValue = this.lowValue;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.highValue_UxmlAttributeFlags);
				if (flag2)
				{
					scroller.slider.highValue = this.highValue;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.direction_UxmlAttributeFlags);
				if (flag3)
				{
					scroller.direction = this.direction;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.value_UxmlAttributeFlags);
				if (flag4)
				{
					scroller.value = this.value;
				}
			}

			[UxmlAttribute("low-value", new string[] { "lowValue" })]
			[SerializeField]
			private float lowValue;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags lowValue_UxmlAttributeFlags;

			[SerializeField]
			[UxmlAttribute("high-value", new string[] { "highValue" })]
			private float highValue;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags highValue_UxmlAttributeFlags;

			[SerializeField]
			private SliderDirection direction;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags direction_UxmlAttributeFlags;

			[SerializeField]
			private float value;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags value_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Scroller, Scroller.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
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
