using System;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements
{
	public class ScrollView : VisualElement
	{
		public ScrollView()
		{
			this.contentViewport = new VisualElement
			{
				name = "ContentViewport"
			};
			this.contentViewport.style.overflow = Overflow.Hidden;
			base.shadow.Add(this.contentViewport);
			this.AssignContentContainer(new VisualElement
			{
				name = "ContentView"
			});
			this.horizontalScroller = new Scroller(0f, 100f, delegate(float value)
			{
				this.scrollOffset = new Vector2(value, this.scrollOffset.y);
				this.UpdateContentViewTransform();
			}, SliderDirection.Horizontal)
			{
				name = "HorizontalScroller",
				persistenceKey = "HorizontalScroller",
				visible = false
			};
			base.shadow.Add(this.horizontalScroller);
			this.verticalScroller = new Scroller(0f, 100f, delegate(float value)
			{
				this.scrollOffset = new Vector2(this.scrollOffset.x, value);
				this.UpdateContentViewTransform();
			}, SliderDirection.Vertical)
			{
				name = "VerticalScroller",
				persistenceKey = "VerticalScroller",
				visible = false
			};
			base.shadow.Add(this.verticalScroller);
			base.RegisterCallback<WheelEvent>(new EventCallback<WheelEvent>(this.OnScrollWheel), TrickleDown.NoTrickleDown);
			this.scrollOffset = Vector2.zero;
		}

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
			position.x = -scrollOffset.x;
			position.y = -scrollOffset.y;
			this.contentContainer.transform.position = position;
			base.IncrementVersion(VersionChangeType.Repaint);
		}

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

		private void AssignContentContainer(VisualElement contents)
		{
			contents.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			contents.AddToClassList(ScrollView.contentViewClass);
			this.contentViewport.Add(contents);
			this.m_ContentContainer = contents;
		}

		public void SetContents(VisualElement contents)
		{
			if (contents != null && contents != this.m_ContentContainer)
			{
				if (this.m_ContentContainer != null)
				{
					this.m_ContentContainer.RemoveFromHierarchy();
					this.m_ContentContainer.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
					this.m_ContentContainer.RemoveFromClassList(ScrollView.contentViewClass);
				}
				this.AssignContentContainer(contents);
				this.scrollOffset = Vector2.zero;
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
						this.contentViewport.AddToClassList(ScrollView.horizontalScrollClass);
					}
					else
					{
						this.contentViewport.RemoveFromClassList(ScrollView.horizontalScrollClass);
					}
				}
				if (this.verticalScroller.visible != this.needsVertical)
				{
					this.verticalScroller.visible = this.needsVertical;
					if (this.needsVertical)
					{
						this.contentViewport.AddToClassList(ScrollView.verticalScrollClass);
					}
					else
					{
						this.contentViewport.RemoveFromClassList(ScrollView.verticalScrollClass);
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
					this.verticalScroller.ScrollPageUp(Mathf.Abs(evt.delta.y));
				}
				else if (evt.delta.y > 0f)
				{
					this.verticalScroller.ScrollPageDown(Mathf.Abs(evt.delta.y));
				}
			}
			evt.StopPropagation();
		}

		private bool m_StretchContentWidth = false;

		private VisualElement m_ContentContainer;

		private static readonly string contentViewClass = "content-view";

		private static readonly string horizontalScrollClass = "horizontal-scroll-visible";

		private static readonly string verticalScrollClass = "vertical-scroll-visible";

		public new class UxmlFactory : UxmlFactory<ScrollView, ScrollView.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				ScrollView scrollView = (ScrollView)ve;
				scrollView.showHorizontal = this.m_ShowHorizontal.GetValueFromBag(bag, cc);
				scrollView.showVertical = this.m_ShowVertical.GetValueFromBag(bag, cc);
				scrollView.stretchContentWidth = this.m_StretchContentWidth.GetValueFromBag(bag, cc);
				scrollView.horizontalPageSize = this.m_HorizontalPageSize.GetValueFromBag(bag, cc);
				scrollView.verticalPageSize = this.m_VerticalPageSize.GetValueFromBag(bag, cc);
			}

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

			private UxmlBoolAttributeDescription m_StretchContentWidth = new UxmlBoolAttributeDescription
			{
				name = "stretch-content-width"
			};
		}
	}
}
