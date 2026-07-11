using System;

namespace UnityEngine.UIElements
{
	public class ScrollView : VisualElement
	{
		public bool showHorizontal
		{
			get
			{
				return this.m_ShowHorizontal;
			}
			set
			{
				this.m_ShowHorizontal = value;
				this.UpdateScrollers(this.m_ShowHorizontal, this.m_ShowVertical);
			}
		}

		public bool showVertical
		{
			get
			{
				return this.m_ShowVertical;
			}
			set
			{
				this.m_ShowVertical = value;
				this.UpdateScrollers(this.m_ShowHorizontal, this.m_ShowVertical);
			}
		}

		internal bool needsHorizontal
		{
			get
			{
				return this.showHorizontal || this.contentContainer.layout.width - base.layout.width > 0f;
			}
		}

		internal bool needsVertical
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
				bool flag = value != this.scrollOffset;
				if (flag)
				{
					this.horizontalScroller.value = value.x;
					this.verticalScroller.value = value.y;
					this.UpdateContentViewTransform();
				}
			}
		}

		public float horizontalPageSize
		{
			get
			{
				return this.horizontalScroller.slider.pageSize;
			}
			set
			{
				this.horizontalScroller.slider.pageSize = value;
			}
		}

		public float verticalPageSize
		{
			get
			{
				return this.verticalScroller.slider.pageSize;
			}
			set
			{
				this.verticalScroller.slider.pageSize = value;
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
			position.x = GUIUtility.RoundToPixelGrid(-scrollOffset.x);
			position.y = GUIUtility.RoundToPixelGrid(-scrollOffset.y);
			this.contentContainer.transform.position = position;
			base.IncrementVersion(VersionChangeType.Repaint);
		}

		public void ScrollTo(VisualElement child)
		{
			bool flag = child == null;
			if (flag)
			{
				throw new ArgumentNullException("child");
			}
			bool flag2 = !this.contentContainer.Contains(child);
			if (flag2)
			{
				throw new ArgumentException("Cannot scroll to a VisualElement that is not a child of the ScrollView content-container.");
			}
			float num = 0f;
			float num2 = 0f;
			bool flag3 = this.scrollableHeight > 0f;
			if (flag3)
			{
				num = this.GetYDeltaOffset(child);
				this.verticalScroller.value = this.scrollOffset.y + num;
			}
			bool flag4 = this.scrollableWidth > 0f;
			if (flag4)
			{
				num2 = this.GetXDeltaOffset(child);
				this.horizontalScroller.value = this.scrollOffset.x + num2;
			}
			bool flag5 = num == 0f && num2 == 0f;
			if (!flag5)
			{
				this.UpdateContentViewTransform();
			}
		}

		private float GetXDeltaOffset(VisualElement child)
		{
			float num = this.contentContainer.transform.position.x * -1f;
			Rect worldBound = this.contentViewport.worldBound;
			float num2 = worldBound.xMin + num;
			float num3 = worldBound.xMax + num;
			Rect worldBound2 = child.worldBound;
			float num4 = worldBound2.xMin + num;
			float num5 = worldBound2.xMax + num;
			bool flag = (num4 >= num2 && num5 <= num3) || float.IsNaN(num4) || float.IsNaN(num5);
			float num6;
			if (flag)
			{
				num6 = 0f;
			}
			else
			{
				float deltaDistance = this.GetDeltaDistance(num2, num3, num4, num5);
				num6 = deltaDistance * this.horizontalScroller.highValue / this.scrollableWidth;
			}
			return num6;
		}

		private float GetYDeltaOffset(VisualElement child)
		{
			float num = this.contentContainer.transform.position.y * -1f;
			Rect worldBound = this.contentViewport.worldBound;
			float num2 = worldBound.yMin + num;
			float num3 = worldBound.yMax + num;
			Rect worldBound2 = child.worldBound;
			float num4 = worldBound2.yMin + num;
			float num5 = worldBound2.yMax + num;
			bool flag = (num4 >= num2 && num5 <= num3) || float.IsNaN(num4) || float.IsNaN(num5);
			float num6;
			if (flag)
			{
				num6 = 0f;
			}
			else
			{
				float deltaDistance = this.GetDeltaDistance(num2, num3, num4, num5);
				num6 = deltaDistance * this.verticalScroller.highValue / this.scrollableHeight;
			}
			return num6;
		}

		private float GetDeltaDistance(float viewMin, float viewMax, float childBoundaryMin, float childBoundaryMax)
		{
			float num = childBoundaryMax - viewMax;
			bool flag = num < -1f;
			if (flag)
			{
				num = childBoundaryMin - viewMin;
			}
			return num;
		}

		public VisualElement contentViewport { get; private set; }

		public Scroller horizontalScroller { get; private set; }

		public Scroller verticalScroller { get; private set; }

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		public ScrollView()
			: this(ScrollViewMode.Vertical)
		{
		}

		public ScrollView(ScrollViewMode scrollViewMode)
		{
			base.AddToClassList(ScrollView.ussClassName);
			this.contentViewport = new VisualElement
			{
				name = "unity-content-viewport"
			};
			this.contentViewport.AddToClassList(ScrollView.viewportUssClassName);
			this.contentViewport.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			base.hierarchy.Add(this.contentViewport);
			this.m_ContentContainer = new VisualElement
			{
				name = "unity-content-container"
			};
			this.m_ContentContainer.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			this.m_ContentContainer.AddToClassList(ScrollView.contentUssClassName);
			this.m_ContentContainer.usageHints = UsageHints.GroupTransform;
			this.contentViewport.Add(this.m_ContentContainer);
			this.SetScrollViewMode(scrollViewMode);
			this.horizontalScroller = new Scroller(0f, 100f, delegate(float value)
			{
				this.scrollOffset = new Vector2(value, this.scrollOffset.y);
				this.UpdateContentViewTransform();
			}, SliderDirection.Horizontal)
			{
				viewDataKey = "HorizontalScroller",
				visible = false
			};
			this.horizontalScroller.AddToClassList(ScrollView.hScrollerUssClassName);
			base.hierarchy.Add(this.horizontalScroller);
			this.verticalScroller = new Scroller(0f, 100f, delegate(float value)
			{
				this.scrollOffset = new Vector2(this.scrollOffset.x, value);
				this.UpdateContentViewTransform();
			}, SliderDirection.Vertical)
			{
				viewDataKey = "VerticalScroller",
				visible = false
			};
			this.verticalScroller.AddToClassList(ScrollView.vScrollerUssClassName);
			base.hierarchy.Add(this.verticalScroller);
			base.RegisterCallback<WheelEvent>(new EventCallback<WheelEvent>(this.OnScrollWheel), TrickleDown.NoTrickleDown);
			this.scrollOffset = Vector2.zero;
		}

		internal void SetScrollViewMode(ScrollViewMode scrollViewMode)
		{
			base.RemoveFromClassList(ScrollView.verticalVariantUssClassName);
			base.RemoveFromClassList(ScrollView.horizontalVariantUssClassName);
			base.RemoveFromClassList(ScrollView.verticalHorizontalVariantUssClassName);
			base.RemoveFromClassList(ScrollView.scrollVariantUssClassName);
			switch (scrollViewMode)
			{
			case ScrollViewMode.Vertical:
				base.AddToClassList(ScrollView.verticalVariantUssClassName);
				base.AddToClassList(ScrollView.scrollVariantUssClassName);
				break;
			case ScrollViewMode.Horizontal:
				base.AddToClassList(ScrollView.horizontalVariantUssClassName);
				base.AddToClassList(ScrollView.scrollVariantUssClassName);
				break;
			case ScrollViewMode.VerticalAndHorizontal:
				base.AddToClassList(ScrollView.scrollVariantUssClassName);
				base.AddToClassList(ScrollView.verticalHorizontalVariantUssClassName);
				break;
			}
		}

		private void OnGeometryChanged(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				bool flag2 = this.needsVertical;
				bool flag3 = this.needsHorizontal;
				bool flag4 = evt.layoutPass > 0;
				if (flag4)
				{
					flag2 = flag2 || this.verticalScroller.visible;
					flag3 = flag3 || this.horizontalScroller.visible;
				}
				this.UpdateScrollers(flag3, flag2);
				this.UpdateContentViewTransform();
			}
		}

		private void UpdateScrollers(bool displayHorizontal, bool displayVertical)
		{
			float num = ((this.contentContainer.layout.width > Mathf.Epsilon) ? (this.contentViewport.layout.width / this.contentContainer.layout.width) : 1f);
			float num2 = ((this.contentContainer.layout.height > Mathf.Epsilon) ? (this.contentViewport.layout.height / this.contentContainer.layout.height) : 1f);
			this.horizontalScroller.Adjust(num);
			this.verticalScroller.Adjust(num2);
			this.horizontalScroller.SetEnabled(this.contentContainer.layout.width - this.contentViewport.layout.width > 0f);
			this.verticalScroller.SetEnabled(this.contentContainer.layout.height - this.contentViewport.layout.height > 0f);
			this.contentViewport.style.marginRight = (displayVertical ? this.verticalScroller.layout.width : 0f);
			this.horizontalScroller.style.right = (displayVertical ? this.verticalScroller.layout.width : 0f);
			this.contentViewport.style.marginBottom = (displayHorizontal ? this.horizontalScroller.layout.height : 0f);
			this.verticalScroller.style.bottom = (displayHorizontal ? this.horizontalScroller.layout.height : 0f);
			bool flag = displayHorizontal && this.scrollableWidth > 0f;
			if (flag)
			{
				this.horizontalScroller.lowValue = 0f;
				this.horizontalScroller.highValue = this.scrollableWidth;
			}
			else
			{
				this.horizontalScroller.value = 0f;
			}
			bool flag2 = displayVertical && this.scrollableHeight > 0f;
			if (flag2)
			{
				this.verticalScroller.lowValue = 0f;
				this.verticalScroller.highValue = this.scrollableHeight;
			}
			else
			{
				this.verticalScroller.value = 0f;
			}
			bool flag3 = this.horizontalScroller.visible != displayHorizontal;
			if (flag3)
			{
				this.horizontalScroller.visible = displayHorizontal;
			}
			bool flag4 = this.verticalScroller.visible != displayVertical;
			if (flag4)
			{
				this.verticalScroller.visible = displayVertical;
			}
		}

		private void OnScrollWheel(WheelEvent evt)
		{
			float value = this.verticalScroller.value;
			bool flag = this.contentContainer.layout.height - base.layout.height > 0f;
			if (flag)
			{
				bool flag2 = evt.delta.y < 0f;
				if (flag2)
				{
					this.verticalScroller.ScrollPageUp(Mathf.Abs(evt.delta.y));
				}
				else
				{
					bool flag3 = evt.delta.y > 0f;
					if (flag3)
					{
						this.verticalScroller.ScrollPageDown(Mathf.Abs(evt.delta.y));
					}
				}
			}
			bool flag4 = this.verticalScroller.value != value;
			if (flag4)
			{
				evt.StopPropagation();
			}
		}

		private bool m_ShowHorizontal;

		private bool m_ShowVertical;

		private VisualElement m_ContentContainer;

		public static readonly string ussClassName = "unity-scroll-view";

		public static readonly string viewportUssClassName = ScrollView.ussClassName + "__content-viewport";

		public static readonly string contentUssClassName = ScrollView.ussClassName + "__content-container";

		public static readonly string hScrollerUssClassName = ScrollView.ussClassName + "__horizontal-scroller";

		public static readonly string vScrollerUssClassName = ScrollView.ussClassName + "__vertical-scroller";

		public static readonly string horizontalVariantUssClassName = ScrollView.ussClassName + "--horizontal";

		public static readonly string verticalVariantUssClassName = ScrollView.ussClassName + "--vertical";

		public static readonly string verticalHorizontalVariantUssClassName = ScrollView.ussClassName + "--vertical-horizontal";

		public static readonly string scrollVariantUssClassName = ScrollView.ussClassName + "--scroll";

		public new class UxmlFactory : UxmlFactory<ScrollView, ScrollView.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				ScrollView scrollView = (ScrollView)ve;
				scrollView.SetScrollViewMode(this.m_ScrollViewMode.GetValueFromBag(bag, cc));
				scrollView.showHorizontal = this.m_ShowHorizontal.GetValueFromBag(bag, cc);
				scrollView.showVertical = this.m_ShowVertical.GetValueFromBag(bag, cc);
				scrollView.horizontalPageSize = this.m_HorizontalPageSize.GetValueFromBag(bag, cc);
				scrollView.verticalPageSize = this.m_VerticalPageSize.GetValueFromBag(bag, cc);
			}

			private UxmlEnumAttributeDescription<ScrollViewMode> m_ScrollViewMode = new UxmlEnumAttributeDescription<ScrollViewMode>
			{
				name = "mode",
				defaultValue = ScrollViewMode.Vertical
			};

			private UxmlBoolAttributeDescription m_ShowHorizontal = new UxmlBoolAttributeDescription
			{
				name = "show-horizontal-scroller"
			};

			private UxmlBoolAttributeDescription m_ShowVertical = new UxmlBoolAttributeDescription
			{
				name = "show-vertical-scroller"
			};

			private UxmlFloatAttributeDescription m_HorizontalPageSize = new UxmlFloatAttributeDescription
			{
				name = "horizontal-page-size",
				defaultValue = 20f
			};

			private UxmlFloatAttributeDescription m_VerticalPageSize = new UxmlFloatAttributeDescription
			{
				name = "vertical-page-size",
				defaultValue = 20f
			};
		}
	}
}
