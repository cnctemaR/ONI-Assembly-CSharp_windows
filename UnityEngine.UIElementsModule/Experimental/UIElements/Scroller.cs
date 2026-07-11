using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements
{
	public class Scroller : VisualElement
	{
		public Scroller()
			: this(0f, 0f, null, Slider.Direction.Vertical)
		{
		}

		public Scroller(float lowValue, float highValue, Action<float> valueChanged, Slider.Direction direction = Slider.Direction.Vertical)
		{
			this.direction = direction;
			this.valueChanged = valueChanged;
			this.slider = new Slider(lowValue, highValue, new Action<float>(this.OnSliderValueChange), direction, 10f)
			{
				name = "Slider",
				persistenceKey = "Slider"
			};
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

		public Slider.Direction direction
		{
			get
			{
				return (base.style.flexDirection != FlexDirection.Row) ? Slider.Direction.Vertical : Slider.Direction.Horizontal;
			}
			set
			{
				if (value == Slider.Direction.Horizontal)
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

		private void OnSliderValueChange(float newValue)
		{
			this.value = newValue;
			if (this.valueChanged != null)
			{
				this.valueChanged(this.slider.value);
			}
			base.Dirty(ChangeType.Repaint);
		}

		public void ScrollPageUp()
		{
			this.value -= this.slider.pageSize * ((this.slider.lowValue >= this.slider.highValue) ? (-1f) : 1f);
		}

		public void ScrollPageDown()
		{
			this.value += this.slider.pageSize * ((this.slider.lowValue >= this.slider.highValue) ? (-1f) : 1f);
		}

		/// <summary>
		///   <para>Instantiates a Scroller using the data read from a UXML file.</para>
		/// </summary>
		public class ScrollerFactory : UxmlFactory<Scroller, Scroller.ScrollerUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the Scroller.</para>
		/// </summary>
		public class ScrollerUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public ScrollerUxmlTraits()
			{
				this.m_LowValue = new UxmlFloatAttributeDescription
				{
					name = "lowValue"
				};
				this.m_HighValue = new UxmlFloatAttributeDescription
				{
					name = "highValue"
				};
				this.m_Direction = new UxmlEnumAttributeDescription<Slider.Direction>
				{
					name = "direction",
					defaultValue = Slider.Direction.Vertical
				};
				this.m_Value = new UxmlFloatAttributeDescription
				{
					name = "value"
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for Scroller properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_LowValue;
					yield return this.m_HighValue;
					yield return this.m_Direction;
					yield return this.m_Value;
					yield break;
				}
			}

			/// <summary>
			///   <para>Returns an empty enumerable, as scrollers do not have children.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize Scroller properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Scroller scroller = (Scroller)ve;
				scroller.slider.lowValue = this.m_LowValue.GetValueFromBag(bag);
				scroller.slider.highValue = this.m_HighValue.GetValueFromBag(bag);
				scroller.direction = this.m_Direction.GetValueFromBag(bag);
				scroller.value = this.m_Value.GetValueFromBag(bag);
			}

			private UxmlFloatAttributeDescription m_LowValue;

			private UxmlFloatAttributeDescription m_HighValue;

			private UxmlEnumAttributeDescription<Slider.Direction> m_Direction;

			private UxmlFloatAttributeDescription m_Value;
		}
	}
}
