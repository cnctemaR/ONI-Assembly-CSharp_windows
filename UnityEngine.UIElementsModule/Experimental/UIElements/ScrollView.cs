using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class ScrollView : VisualElement
	{
		public ScrollView()
			: this(ScrollView.kDefaultScrollerValues, ScrollView.kDefaultScrollerValues)
		{
		}

		public ScrollView(Vector2 horizontalScrollerValues, Vector2 verticalScrollerValues)
		{
			this.horizontalScrollerValues = horizontalScrollerValues;
			this.verticalScrollerValues = verticalScrollerValues;
			this.contentViewport = new VisualElement
			{
				name = "ContentViewport"
			};
			this.contentViewport.clippingOptions = VisualElement.ClippingOptions.ClipContents;
			base.shadow.Add(this.contentViewport);
			this.m_ContentContainer = new VisualElement
			{
				name = "ContentView"
			};
			this.contentViewport.Add(this.m_ContentContainer);
			this.horizontalScroller = new Scroller(horizontalScrollerValues.x, horizontalScrollerValues.y, delegate(float value)
			{
				this.scrollOffset = new Vector2(value, this.scrollOffset.y);
				this.UpdateContentViewTransform();
			}, Slider.Direction.Horizontal)
			{
				name = "HorizontalScroller",
				persistenceKey = "HorizontalScroller"
			};
			base.shadow.Add(this.horizontalScroller);
			this.verticalScroller = new Scroller(verticalScrollerValues.x, verticalScrollerValues.y, delegate(float value)
			{
				this.scrollOffset = new Vector2(this.scrollOffset.x, value);
				this.UpdateContentViewTransform();
			}, Slider.Direction.Vertical)
			{
				name = "VerticalScroller",
				persistenceKey = "VerticalScroller"
			};
			base.shadow.Add(this.verticalScroller);
			base.RegisterCallback<WheelEvent>(new EventCallback<WheelEvent>(this.OnScrollWheel), Capture.NoCapture);
			this.contentContainer.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), Capture.NoCapture);
		}

		/// <summary>
		///   <para>Indicates whether the content of ScrollView should fill the width of its viewport. The default value is false.</para>
		/// </summary>
		public bool stretchContentWidth
		{
			get
			{
				return this.m_StretchContentWidth;
			}
			set
			{
				if (this.m_StretchContentWidth != value)
				{
					this.m_StretchContentWidth = value;
					if (this.m_StretchContentWidth)
					{
						base.AddToClassList("stretchContentWidth");
					}
					else
					{
						base.RemoveFromClassList("stretchContentWidth");
					}
				}
			}
		}

		public Vector2 horizontalScrollerValues { get; set; }

		public Vector2 verticalScrollerValues { get; set; }

		public bool showHorizontal { get; set; }

		public bool showVertical { get; set; }

		public bool needsHorizontal
		{
			get
			{
				return this.showHorizontal || this.contentContainer.layout.width - base.layout.width > 0f;
			}
		}

		public bool needsVertical
		{
			get
			{
				return this.showVertical || this.contentContainer.layout.height - base.layout.height > 0f;
			}
		}

		public Vector2 scrollOffset
		{
			get
			{
				return new Vector2(this.horizontalScroller.value, this.verticalScroller.value);
			}
			set
			{
				if (value != this.scrollOffset)
				{
					this.horizontalScroller.value = value.x;
					this.verticalScroller.value = value.y;
					this.UpdateContentViewTransform();
				}
			}
		}

		private float scrollableWidth
		{
			get
			{
				return this.contentContainer.layout.width - this.contentViewport.layout.width;
			}
		}

		private float scrollableHeight
		{
			get
			{
				return this.contentContainer.layout.height - this.contentViewport.layout.height;
			}
		}

		private void UpdateContentViewTransform()
		{
			Vector3 position = this.contentContainer.transform.position;
			Vector2 scrollOffset = this.scrollOffset;
			position.x = -scrollOffset.x;
			position.y = -scrollOffset.y;
			this.contentContainer.transform.position = position;
			base.Dirty(ChangeType.Repaint);
		}

		/// <summary>
		///   <para>Scroll to a specific child element.</para>
		/// </summary>
		/// <param name="child">The child to scroll to.</param>
		public void ScrollTo(VisualElement child)
		{
			if (!this.contentContainer.Contains(child))
			{
				throw new ArgumentException("Cannot scroll to null child");
			}
			float num = this.contentContainer.layout.height - this.contentViewport.layout.height;
			float num2 = this.contentContainer.transform.position.y * -1f;
			float num3 = this.contentViewport.layout.yMin + num2;
			float num4 = this.contentViewport.layout.yMax + num2;
			float yMin = child.layout.yMin;
			float yMax = child.layout.yMax;
			if ((yMin < num3 || yMax > num4) && !float.IsNaN(yMin) && !float.IsNaN(yMax))
			{
				bool flag = false;
				float num5 = yMax - num4;
				if (num5 < -1f)
				{
					num5 = num3 - yMin;
					flag = true;
				}
				float num6 = num5 * this.verticalScroller.highValue / num;
				this.verticalScroller.value = this.scrollOffset.y + ((!flag) ? num6 : (-num6));
				this.UpdateContentViewTransform();
			}
		}

		public VisualElement contentViewport { get; private set; }

		[Obsolete("Please use contentContainer instead", false)]
		public VisualElement contentView
		{
			get
			{
				return this.contentContainer;
			}
		}

		public Scroller horizontalScroller { get; private set; }

		public Scroller verticalScroller { get; private set; }

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt.GetEventTypeId() == EventBase<GeometryChangedEvent>.TypeId())
			{
				this.OnGeometryChanged((GeometryChangedEvent)evt);
			}
		}

		private void OnGeometryChanged(GeometryChangedEvent evt)
		{
			if (!(evt.oldRect.size == evt.newRect.size))
			{
				if (this.contentContainer.layout.width > Mathf.Epsilon)
				{
					this.horizontalScroller.Adjust(this.contentViewport.layout.width / this.contentContainer.layout.width);
				}
				if (this.contentContainer.layout.height > Mathf.Epsilon)
				{
					this.verticalScroller.Adjust(this.contentViewport.layout.height / this.contentContainer.layout.height);
				}
				this.horizontalScroller.SetEnabled(this.contentContainer.layout.width - base.layout.width > 0f);
				this.verticalScroller.SetEnabled(this.contentContainer.layout.height - base.layout.height > 0f);
				this.contentViewport.style.positionRight = ((!this.needsVertical) ? 0f : this.verticalScroller.layout.width);
				this.horizontalScroller.style.positionRight = ((!this.needsVertical) ? 0f : this.verticalScroller.layout.width);
				this.contentViewport.style.positionBottom = ((!this.needsHorizontal) ? 0f : this.horizontalScroller.layout.height);
				this.verticalScroller.style.positionBottom = ((!this.needsHorizontal) ? 0f : this.horizontalScroller.layout.height);
				if (this.needsHorizontal && this.scrollableWidth > 0f)
				{
					this.horizontalScroller.lowValue = 0f;
					this.horizontalScroller.highValue = this.scrollableWidth;
				}
				else
				{
					this.horizontalScroller.value = 0f;
				}
				if (this.needsVertical && this.scrollableHeight > 0f)
				{
					this.verticalScroller.lowValue = 0f;
					this.verticalScroller.highValue = this.scrollableHeight;
				}
				else
				{
					this.verticalScroller.value = 0f;
				}
				if (this.horizontalScroller.visible != this.needsHorizontal)
				{
					this.horizontalScroller.visible = this.needsHorizontal;
					if (this.needsHorizontal)
					{
						this.contentViewport.AddToClassList("HorizontalScroll");
					}
					else
					{
						this.contentViewport.RemoveFromClassList("HorizontalScroll");
					}
				}
				if (this.verticalScroller.visible != this.needsVertical)
				{
					this.verticalScroller.visible = this.needsVertical;
					if (this.needsVertical)
					{
						this.contentViewport.AddToClassList("VerticalScroll");
					}
					else
					{
						this.contentViewport.RemoveFromClassList("VerticalScroll");
					}
				}
				this.UpdateContentViewTransform();
			}
		}

		private void OnScrollWheel(WheelEvent evt)
		{
			if (this.contentContainer.layout.height - base.layout.height > 0f)
			{
				if (evt.delta.y < 0f)
				{
					this.verticalScroller.ScrollPageUp();
				}
				else if (evt.delta.y > 0f)
				{
					this.verticalScroller.ScrollPageDown();
				}
			}
			evt.StopPropagation();
		}

		private bool m_StretchContentWidth = false;

		public static readonly Vector2 kDefaultScrollerValues = new Vector2(0f, 100f);

		private VisualElement m_ContentContainer;

		/// <summary>
		///   <para>Instantiates a ScrollView using the data read from a UXML file.</para>
		/// </summary>
		public class ScrollViewFactory : UxmlFactory<ScrollView, ScrollView.ScrollViewUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the ScrollView.</para>
		/// </summary>
		public class ScrollViewUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public ScrollViewUxmlTraits()
			{
				this.m_ShowHorizontal = new UxmlBoolAttributeDescription
				{
					name = "showHorizontalScroller"
				};
				this.m_ShowVertical = new UxmlBoolAttributeDescription
				{
					name = "showVerticalScroller"
				};
				this.m_HorizontalLowValue = new UxmlFloatAttributeDescription
				{
					name = "horizontalLowValue"
				};
				this.m_HorizontalHighValue = new UxmlFloatAttributeDescription
				{
					name = "horizontalHighValue"
				};
				this.m_HorizontalPageSize = new UxmlFloatAttributeDescription
				{
					name = "horizontalPageSize",
					defaultValue = 10f
				};
				this.m_HorizontalValue = new UxmlFloatAttributeDescription
				{
					name = "horizontalValue"
				};
				this.m_VerticalLowValue = new UxmlFloatAttributeDescription
				{
					name = "verticalLowValue"
				};
				this.m_VerticalHighValue = new UxmlFloatAttributeDescription
				{
					name = "verticalHighValue"
				};
				this.m_VerticalPageSize = new UxmlFloatAttributeDescription
				{
					name = "verticalPageSize",
					defaultValue = 10f
				};
				this.m_VerticalValue = new UxmlFloatAttributeDescription
				{
					name = "verticalValue"
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for ScrollView properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_ShowHorizontal;
					yield return this.m_ShowVertical;
					yield return this.m_HorizontalLowValue;
					yield return this.m_HorizontalHighValue;
					yield return this.m_HorizontalPageSize;
					yield return this.m_HorizontalValue;
					yield return this.m_VerticalLowValue;
					yield return this.m_VerticalHighValue;
					yield return this.m_VerticalPageSize;
					yield return this.m_VerticalValue;
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize ScrollView properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Vector2 vector = new Vector2(this.m_HorizontalLowValue.GetValueFromBag(bag), this.m_HorizontalHighValue.GetValueFromBag(bag));
				Vector2 vector2 = new Vector2(this.m_VerticalLowValue.GetValueFromBag(bag), this.m_VerticalHighValue.GetValueFromBag(bag));
				ScrollView scrollView = (ScrollView)ve;
				scrollView.horizontalScrollerValues = vector;
				scrollView.verticalScrollerValues = vector2;
				scrollView.showHorizontal = this.m_ShowHorizontal.GetValueFromBag(bag);
				scrollView.showVertical = this.m_ShowVertical.GetValueFromBag(bag);
				scrollView.scrollOffset = new Vector2(this.m_HorizontalValue.GetValueFromBag(bag), this.m_VerticalValue.GetValueFromBag(bag));
				scrollView.horizontalScroller.slider.pageSize = this.m_HorizontalPageSize.GetValueFromBag(bag);
				scrollView.verticalScroller.slider.pageSize = this.m_VerticalPageSize.GetValueFromBag(bag);
			}

			private UxmlBoolAttributeDescription m_ShowHorizontal;

			private UxmlBoolAttributeDescription m_ShowVertical;

			private UxmlFloatAttributeDescription m_HorizontalLowValue;

			private UxmlFloatAttributeDescription m_HorizontalHighValue;

			private UxmlFloatAttributeDescription m_HorizontalPageSize;

			private UxmlFloatAttributeDescription m_HorizontalValue;

			private UxmlFloatAttributeDescription m_VerticalLowValue;

			private UxmlFloatAttributeDescription m_VerticalHighValue;

			private UxmlFloatAttributeDescription m_VerticalPageSize;

			private UxmlFloatAttributeDescription m_VerticalValue;
		}
	}
}
