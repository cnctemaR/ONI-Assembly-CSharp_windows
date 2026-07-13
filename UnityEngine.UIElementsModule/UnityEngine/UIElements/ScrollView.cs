using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	public class ScrollView : VisualElement
	{
		[CreateProperty]
		public ScrollerVisibility horizontalScrollerVisibility
		{
			get
			{
				return this.m_HorizontalScrollerVisibility;
			}
			set
			{
				ScrollerVisibility horizontalScrollerVisibility = this.m_HorizontalScrollerVisibility;
				this.m_HorizontalScrollerVisibility = value;
				this.UpdateScrollers(this.needsHorizontal, this.needsVertical);
				bool flag = horizontalScrollerVisibility != this.m_HorizontalScrollerVisibility;
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.horizontalScrollerVisibilityProperty);
				}
			}
		}

		[CreateProperty]
		public ScrollerVisibility verticalScrollerVisibility
		{
			get
			{
				return this.m_VerticalScrollerVisibility;
			}
			set
			{
				ScrollerVisibility verticalScrollerVisibility = this.m_VerticalScrollerVisibility;
				this.m_VerticalScrollerVisibility = value;
				this.UpdateScrollers(this.needsHorizontal, this.needsVertical);
				bool flag = verticalScrollerVisibility != this.m_VerticalScrollerVisibility;
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.verticalScrollerVisibilityProperty);
				}
			}
		}

		[CreateProperty]
		public long elasticAnimationIntervalMs
		{
			get
			{
				return this.m_ElasticAnimationIntervalMs;
			}
			set
			{
				long elasticAnimationIntervalMs = this.m_ElasticAnimationIntervalMs;
				this.m_ElasticAnimationIntervalMs = value;
				bool flag = elasticAnimationIntervalMs != this.m_ElasticAnimationIntervalMs;
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.elasticAnimationIntervalMsProperty);
					this.m_PostPointerUpAnimation = base.schedule.Execute(new Action(this.PostPointerUpAnimation)).Every(this.m_ElasticAnimationIntervalMs);
				}
			}
		}

		[Obsolete("showHorizontal is obsolete. Use horizontalScrollerVisibility instead")]
		public bool showHorizontal
		{
			get
			{
				return this.horizontalScrollerVisibility == ScrollerVisibility.AlwaysVisible;
			}
			set
			{
				this.m_HorizontalScrollerVisibility = (value ? ScrollerVisibility.AlwaysVisible : ScrollerVisibility.Auto);
			}
		}

		[Obsolete("showVertical is obsolete. Use verticalScrollerVisibility instead")]
		public bool showVertical
		{
			get
			{
				return this.verticalScrollerVisibility == ScrollerVisibility.AlwaysVisible;
			}
			set
			{
				this.m_VerticalScrollerVisibility = (value ? ScrollerVisibility.AlwaysVisible : ScrollerVisibility.Auto);
			}
		}

		internal bool needsHorizontal
		{
			get
			{
				return (this.mode != ScrollViewMode.Vertical && this.horizontalScrollerVisibility == ScrollerVisibility.AlwaysVisible) || (this.horizontalScrollerVisibility == ScrollerVisibility.Auto && this.scrollableWidth > 0.001f);
			}
		}

		internal bool needsVertical
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return (this.mode != ScrollViewMode.Horizontal && this.verticalScrollerVisibility == ScrollerVisibility.AlwaysVisible) || (this.verticalScrollerVisibility == ScrollerVisibility.Auto && this.scrollableHeight > 0.001f);
			}
		}

		internal bool isVerticalScrollDisplayed
		{
			get
			{
				return this.verticalScroller.resolvedStyle.display == DisplayStyle.Flex;
			}
		}

		internal bool isHorizontalScrollDisplayed
		{
			get
			{
				return this.horizontalScroller.resolvedStyle.display == DisplayStyle.Flex;
			}
		}

		[CreateProperty]
		public Vector2 scrollOffset
		{
			get
			{
				return this.m_ScrollOffset;
			}
			set
			{
				bool flag = value != this.m_ScrollOffset;
				if (flag)
				{
					this.horizontalScroller.value = value.x;
					this.verticalScroller.value = value.y;
					this.m_ScrollOffset = new Vector2(this.horizontalScroller.value, this.verticalScroller.value);
					base.SaveViewData();
					bool flag2 = base.panel != null;
					if (flag2)
					{
						bool flag3 = !Mathf.Approximately(this.m_ContentAndVerticalScrollContainer.layout.height - this.contentViewport.layout.height, 0f);
						if (flag3)
						{
							this.UpdateScrollers(this.needsHorizontal, this.needsVertical);
						}
						this.UpdateContentViewTransform();
					}
					base.NotifyPropertyChanged(in ScrollView.scrollOffsetProperty);
				}
			}
		}

		[CreateProperty]
		public float horizontalPageSize
		{
			get
			{
				return this.m_HorizontalPageSize;
			}
			set
			{
				float horizontalPageSize = this.m_HorizontalPageSize;
				this.m_HorizontalPageSize = value;
				this.UpdateHorizontalSliderPageSize();
				bool flag = !Mathf.Approximately(horizontalPageSize, this.m_HorizontalPageSize);
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.horizontalPageSizeProperty);
				}
			}
		}

		[CreateProperty]
		public float verticalPageSize
		{
			get
			{
				return this.m_VerticalPageSize;
			}
			set
			{
				float verticalPageSize = this.m_VerticalPageSize;
				this.m_VerticalPageSize = value;
				this.UpdateVerticalSliderPageSize();
				bool flag = !Mathf.Approximately(verticalPageSize, this.m_VerticalPageSize);
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.verticalPageSizeProperty);
				}
			}
		}

		[CreateProperty]
		public float mouseWheelScrollSize
		{
			get
			{
				return this.m_MouseWheelScrollSize;
			}
			set
			{
				float mouseWheelScrollSize = this.m_MouseWheelScrollSize;
				bool flag = Math.Abs(this.m_MouseWheelScrollSize - value) > float.Epsilon;
				if (flag)
				{
					this.m_MouseWheelScrollSizeIsInline = true;
					this.m_MouseWheelScrollSize = value;
					base.NotifyPropertyChanged(in ScrollView.mouseWheelScrollSizeProperty);
				}
			}
		}

		internal float scrollableWidth
		{
			get
			{
				return this.contentContainer.boundingBox.width - this.contentViewport.layout.width;
			}
		}

		internal float scrollableHeight
		{
			get
			{
				return this.contentContainer.boundingBox.height - this.contentViewport.layout.height;
			}
		}

		private bool hasInertia
		{
			get
			{
				return this.scrollDecelerationRate > 0f;
			}
		}

		[CreateProperty]
		public float scrollDecelerationRate
		{
			get
			{
				return this.m_ScrollDecelerationRate;
			}
			set
			{
				float scrollDecelerationRate = this.m_ScrollDecelerationRate;
				this.m_ScrollDecelerationRate = Mathf.Max(0f, value);
				bool flag = !Mathf.Approximately(scrollDecelerationRate, this.m_ScrollDecelerationRate);
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.scrollDecelerationRateProperty);
				}
			}
		}

		[CreateProperty]
		public float elasticity
		{
			get
			{
				return this.m_Elasticity;
			}
			set
			{
				float elasticity = this.m_Elasticity;
				this.m_Elasticity = Mathf.Max(0f, value);
				bool flag = !Mathf.Approximately(elasticity, this.m_Elasticity);
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.elasticityProperty);
				}
			}
		}

		[CreateProperty]
		public ScrollView.TouchScrollBehavior touchScrollBehavior
		{
			get
			{
				return this.m_TouchScrollBehavior;
			}
			set
			{
				ScrollView.TouchScrollBehavior touchScrollBehavior = this.m_TouchScrollBehavior;
				this.m_TouchScrollBehavior = value;
				bool flag = this.m_TouchScrollBehavior == ScrollView.TouchScrollBehavior.Clamped;
				if (flag)
				{
					this.horizontalScroller.slider.clamped = true;
					this.verticalScroller.slider.clamped = true;
				}
				else
				{
					this.horizontalScroller.slider.clamped = false;
					this.verticalScroller.slider.clamped = false;
				}
				bool flag2 = touchScrollBehavior != this.m_TouchScrollBehavior;
				if (flag2)
				{
					base.NotifyPropertyChanged(in ScrollView.touchScrollBehaviorProperty);
				}
			}
		}

		[CreateProperty]
		public ScrollView.NestedInteractionKind nestedInteractionKind
		{
			get
			{
				return this.m_NestedInteractionKind;
			}
			set
			{
				ScrollView.NestedInteractionKind nestedInteractionKind = this.m_NestedInteractionKind;
				this.m_NestedInteractionKind = value;
				bool flag = nestedInteractionKind != this.m_NestedInteractionKind;
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.nestedInteractionKindProperty);
				}
			}
		}

		private void OnHorizontalScrollDragElementChanged(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				this.UpdateHorizontalSliderPageSize();
			}
		}

		private void OnVerticalScrollDragElementChanged(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				this.UpdateVerticalSliderPageSize();
			}
		}

		private void UpdateHorizontalSliderPageSize()
		{
			float width = this.horizontalScroller.resolvedStyle.width;
			float num = this.m_HorizontalPageSize;
			bool flag = width > 0f;
			if (flag)
			{
				bool flag2 = Mathf.Approximately(this.m_HorizontalPageSize, -1f);
				if (flag2)
				{
					float width2 = this.horizontalScroller.slider.dragElement.resolvedStyle.width;
					num = width2 * 0.9f;
				}
			}
			bool flag3 = num >= 0f;
			if (flag3)
			{
				this.horizontalScroller.slider.pageSize = num;
			}
		}

		private void UpdateVerticalSliderPageSize()
		{
			float height = this.verticalScroller.resolvedStyle.height;
			float num = this.m_VerticalPageSize;
			bool flag = height > 0f;
			if (flag)
			{
				bool flag2 = Mathf.Approximately(this.m_VerticalPageSize, -1f);
				if (flag2)
				{
					float height2 = this.verticalScroller.slider.dragElement.resolvedStyle.height;
					num = height2 * 0.9f;
				}
			}
			bool flag3 = num >= 0f;
			if (flag3)
			{
				this.verticalScroller.slider.pageSize = num;
			}
		}

		internal void UpdateContentViewTransform()
		{
			Vector3 translate = this.contentContainer.resolvedStyle.translate;
			Vector2 scrollOffset = this.scrollOffset;
			bool needsVertical = this.needsVertical;
			if (needsVertical)
			{
				scrollOffset.y += this.contentContainer.resolvedStyle.top;
			}
			translate.x = this.RoundToPanelPixelSize(-scrollOffset.x);
			translate.y = this.RoundToPanelPixelSize(-scrollOffset.y);
			this.contentContainer.style.translate = translate;
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
				throw new ArgumentException("Cannot scroll to a VisualElement that's not a child of the ScrollView content-container.");
			}
			bool flag3 = this.ShouldDeferScrollTo();
			if (flag3)
			{
				this.StartDeferredScrollTo(child);
			}
			else
			{
				this.StopDeferredScrollTo();
			}
			this.m_Velocity = Vector2.zero;
			float num = 0f;
			float num2 = 0f;
			bool flag4 = this.scrollableHeight > 0f;
			if (flag4)
			{
				num = this.GetYDeltaOffset(child);
				this.verticalScroller.value = this.scrollOffset.y + num;
			}
			bool flag5 = this.scrollableWidth > 0f;
			if (flag5)
			{
				num2 = this.GetXDeltaOffset(child);
				this.horizontalScroller.value = this.scrollOffset.x + num2;
			}
			bool flag6 = num == 0f && num2 == 0f;
			if (!flag6)
			{
				this.UpdateContentViewTransform();
			}
		}

		private bool ShouldDeferScrollTo()
		{
			return this.contentContainer.panel.isDirty;
		}

		private bool ShouldStopDeferredScrollTo()
		{
			return !this.ShouldDeferScrollTo();
		}

		private void StartDeferredScrollTo(VisualElement target)
		{
			this.m_DeferredScrollToElement = target;
			bool flag = this.m_DeferredScrollTo == null;
			if (flag)
			{
				this.m_DeferredScrollTo = base.schedule.Execute(new Action(this.PerformDeferredScrollTo)).Until(new Func<bool>(this.ShouldStopDeferredScrollTo));
			}
			else
			{
				bool flag2 = !this.m_DeferredScrollTo.isActive;
				if (flag2)
				{
					this.m_DeferredScrollTo.Resume();
				}
			}
		}

		private void StopDeferredScrollTo()
		{
			this.m_DeferredScrollToElement = null;
			bool flag = this.m_DeferredScrollTo == null;
			if (!flag)
			{
				bool isActive = this.m_DeferredScrollTo.isActive;
				if (isActive)
				{
					this.m_DeferredScrollTo.Pause();
				}
			}
		}

		private void PerformDeferredScrollTo()
		{
			bool flag = this.m_DeferredScrollToElement != null;
			if (flag)
			{
				bool flag2 = !this.contentContainer.Contains(this.m_DeferredScrollToElement);
				if (flag2)
				{
					this.StopDeferredScrollTo();
				}
				else
				{
					this.ScrollTo(this.m_DeferredScrollToElement);
				}
			}
			else
			{
				this.StopDeferredScrollTo();
			}
		}

		private float GetXDeltaOffset(VisualElement child)
		{
			float num = this.contentContainer.resolvedStyle.translate.x * -1f;
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
			float num = this.contentContainer.resolvedStyle.translate.y * -1f;
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
			float num = viewMax - viewMin;
			float num2 = childBoundaryMax - childBoundaryMin;
			bool flag = num2 > num;
			float num3;
			if (flag)
			{
				bool flag2 = viewMin > childBoundaryMin && childBoundaryMax > viewMax;
				if (flag2)
				{
					num3 = 0f;
				}
				else
				{
					num3 = ((childBoundaryMin > viewMin) ? (childBoundaryMin - viewMin) : (childBoundaryMax - viewMax));
				}
			}
			else
			{
				float num4 = childBoundaryMax - viewMax;
				bool flag3 = num4 < -1f;
				if (flag3)
				{
					num4 = childBoundaryMin - viewMin;
				}
				num3 = num4;
			}
			return num3;
		}

		public VisualElement contentViewport { get; }

		public Scroller horizontalScroller { get; }

		public Scroller verticalScroller { get; }

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
			this.m_ContentAndVerticalScrollContainer = new VisualElement
			{
				name = "unity-content-and-vertical-scroll-container"
			};
			this.m_ContentAndVerticalScrollContainer.AddToClassList(ScrollView.contentAndVerticalScrollUssClassName);
			base.hierarchy.Add(this.m_ContentAndVerticalScrollContainer);
			this.contentViewport = new VisualElement
			{
				name = "unity-content-viewport"
			};
			this.contentViewport.AddToClassList(ScrollView.viewportUssClassName);
			this.contentViewport.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			this.contentViewport.pickingMode = PickingMode.Ignore;
			this.m_ContentAndVerticalScrollContainer.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
			this.m_ContentAndVerticalScrollContainer.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
			this.m_ContentAndVerticalScrollContainer.Add(this.contentViewport);
			this.m_ContentContainer = new VisualElement
			{
				name = "unity-content-container"
			};
			this.m_ContentContainer.disableClipping = true;
			this.m_ContentContainer.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			this.m_ContentContainer.AddToClassList(ScrollView.contentUssClassName);
			this.m_ContentContainer.usageHints = UsageHints.GroupTransform;
			this.contentViewport.Add(this.m_ContentContainer);
			this.SetScrollViewMode(scrollViewMode);
			this.horizontalScroller = new Scroller(0f, 2.1474836E+09f, delegate(float value)
			{
				this.scrollOffset = new Vector2(value, this.scrollOffset.y);
				this.UpdateContentViewTransform();
			}, SliderDirection.Horizontal)
			{
				viewDataKey = "HorizontalScroller"
			};
			this.horizontalScroller.AddToClassList(ScrollView.hScrollerUssClassName);
			this.horizontalScroller.style.display = DisplayStyle.None;
			base.hierarchy.Add(this.horizontalScroller);
			this.verticalScroller = new Scroller(0f, 2.1474836E+09f, delegate(float value)
			{
				this.scrollOffset = new Vector2(this.scrollOffset.x, value);
				this.UpdateContentViewTransform();
			}, SliderDirection.Vertical)
			{
				viewDataKey = "VerticalScroller"
			};
			this.verticalScroller.slider.viewDataRestored += this.OnVerticalSliderViewDataRestored;
			this.horizontalScroller.slider.viewDataRestored += this.OnHorizontalSliderViewDataRestored;
			this.horizontalScroller.slider.onSetValueWithoutNotify += this.OnHorizontalScrollerSetValueWithoutNotify;
			this.verticalScroller.slider.onSetValueWithoutNotify += this.OnVerticalScrollerSetValueWithoutNotify;
			this.horizontalScroller.slider.clampedDragger.draggingEnded += this.UpdateElasticBehaviour;
			this.verticalScroller.slider.clampedDragger.draggingEnded += this.UpdateElasticBehaviour;
			this.horizontalScroller.lowButton.AddAction(new Action(this.UpdateElasticBehaviour));
			this.horizontalScroller.highButton.AddAction(new Action(this.UpdateElasticBehaviour));
			this.verticalScroller.lowButton.AddAction(new Action(this.UpdateElasticBehaviour));
			this.verticalScroller.highButton.AddAction(new Action(this.UpdateElasticBehaviour));
			this.verticalScroller.AddToClassList(ScrollView.vScrollerUssClassName);
			this.verticalScroller.style.display = DisplayStyle.None;
			this.m_ContentAndVerticalScrollContainer.Add(this.verticalScroller);
			this.touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;
			base.RegisterCallback<WheelEvent>(new EventCallback<WheelEvent>(this.OnScrollWheel), InvokePolicy.IncludeDisabled, TrickleDown.NoTrickleDown);
			this.verticalScroller.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnScrollersGeometryChanged), TrickleDown.NoTrickleDown);
			this.horizontalScroller.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnScrollersGeometryChanged), TrickleDown.NoTrickleDown);
			this.horizontalPageSize = -1f;
			this.verticalPageSize = -1f;
			this.horizontalScroller.slider.dragElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnHorizontalScrollDragElementChanged), TrickleDown.NoTrickleDown);
			this.verticalScroller.slider.dragElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnVerticalScrollDragElementChanged), TrickleDown.NoTrickleDown);
			this.m_CapturedTargetPointerMoveCallback = new EventCallback<PointerMoveEvent>(this.OnPointerMove);
			this.m_CapturedTargetPointerUpCallback = new EventCallback<PointerUpEvent>(this.OnPointerUp);
			this.scrollOffset = Vector2.zero;
			this.m_ContentContainer.receivesHierarchyGeometryChangedEvents = true;
		}

		[CreateProperty]
		public ScrollViewMode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				ScrollViewMode mode = this.m_Mode;
				this.SetScrollViewMode(value);
				bool flag = mode != this.m_Mode;
				if (flag)
				{
					base.NotifyPropertyChanged(in ScrollView.modeProperty);
				}
			}
		}

		private void SetScrollViewMode(ScrollViewMode mode)
		{
			this.m_Mode = mode;
			base.RemoveFromClassList(ScrollView.verticalVariantUssClassName);
			base.RemoveFromClassList(ScrollView.horizontalVariantUssClassName);
			base.RemoveFromClassList(ScrollView.verticalHorizontalVariantUssClassName);
			base.RemoveFromClassList(ScrollView.scrollVariantUssClassName);
			this.contentContainer.RemoveFromClassList(ScrollView.verticalVariantContentUssClassName);
			this.contentContainer.RemoveFromClassList(ScrollView.horizontalVariantContentUssClassName);
			this.contentContainer.RemoveFromClassList(ScrollView.verticalHorizontalVariantContentUssClassName);
			this.contentViewport.RemoveFromClassList(ScrollView.verticalVariantViewportUssClassName);
			this.contentViewport.RemoveFromClassList(ScrollView.horizontalVariantViewportUssClassName);
			this.contentViewport.RemoveFromClassList(ScrollView.verticalHorizontalVariantViewportUssClassName);
			switch (mode)
			{
			case ScrollViewMode.Vertical:
				base.AddToClassList(ScrollView.scrollVariantUssClassName);
				base.AddToClassList(ScrollView.verticalVariantUssClassName);
				this.contentViewport.AddToClassList(ScrollView.verticalVariantViewportUssClassName);
				this.contentContainer.AddToClassList(ScrollView.verticalVariantContentUssClassName);
				break;
			case ScrollViewMode.Horizontal:
				base.AddToClassList(ScrollView.scrollVariantUssClassName);
				base.AddToClassList(ScrollView.horizontalVariantUssClassName);
				this.contentViewport.AddToClassList(ScrollView.horizontalVariantViewportUssClassName);
				this.contentContainer.AddToClassList(ScrollView.horizontalVariantContentUssClassName);
				break;
			case ScrollViewMode.VerticalAndHorizontal:
				base.AddToClassList(ScrollView.scrollVariantUssClassName);
				base.AddToClassList(ScrollView.verticalHorizontalVariantUssClassName);
				this.contentViewport.AddToClassList(ScrollView.verticalHorizontalVariantViewportUssClassName);
				this.contentContainer.AddToClassList(ScrollView.verticalHorizontalVariantContentUssClassName);
				break;
			}
		}

		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			bool flag = evt.destinationPanel == null;
			if (!flag)
			{
				this.m_AttachedRootVisualContainer = base.GetRootVisualContainer(false) ?? evt.destinationPanel.visualTree;
				VisualElement attachedRootVisualContainer = this.m_AttachedRootVisualContainer;
				if (attachedRootVisualContainer != null)
				{
					attachedRootVisualContainer.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnRootCustomStyleResolved), TrickleDown.NoTrickleDown);
				}
				base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnRootCustomStyleResolved), TrickleDown.NoTrickleDown);
				this.MarkSingleLineHeightDirty();
				bool flag2 = evt.destinationPanel.contextType == ContextType.Player;
				if (flag2)
				{
					this.m_ContentAndVerticalScrollContainer.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
					this.contentContainer.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.TrickleDown);
					this.contentContainer.RegisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
					this.contentContainer.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.TrickleDown);
					this.contentContainer.RegisterCallback<PointerCaptureEvent>(new EventCallback<PointerCaptureEvent>(this.OnPointerCapture), TrickleDown.NoTrickleDown);
					this.contentContainer.RegisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), TrickleDown.NoTrickleDown);
					evt.destinationPanel.visualTree.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnRootPointerUp), TrickleDown.TrickleDown);
				}
			}
		}

		private void OnDetachFromPanel(DetachFromPanelEvent evt)
		{
			IVisualElementScheduledItem scheduledLayoutPassResetItem = this.m_ScheduledLayoutPassResetItem;
			if (scheduledLayoutPassResetItem != null)
			{
				scheduledLayoutPassResetItem.Pause();
			}
			this.ResetLayoutPass();
			bool flag = evt.originPanel == null;
			if (!flag)
			{
				VisualElement attachedRootVisualContainer = this.m_AttachedRootVisualContainer;
				if (attachedRootVisualContainer != null)
				{
					attachedRootVisualContainer.UnregisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnRootCustomStyleResolved), TrickleDown.NoTrickleDown);
				}
				this.m_AttachedRootVisualContainer = null;
				base.UnregisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnRootCustomStyleResolved), TrickleDown.NoTrickleDown);
				bool flag2 = evt.originPanel.contextType == ContextType.Player;
				if (flag2)
				{
					this.m_ContentAndVerticalScrollContainer.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
					this.contentContainer.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.TrickleDown);
					this.contentContainer.UnregisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
					this.contentContainer.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.TrickleDown);
					this.contentContainer.UnregisterCallback<PointerCaptureEvent>(new EventCallback<PointerCaptureEvent>(this.OnPointerCapture), TrickleDown.NoTrickleDown);
					this.contentContainer.UnregisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), TrickleDown.NoTrickleDown);
					evt.originPanel.visualTree.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnRootPointerUp), TrickleDown.TrickleDown);
				}
			}
		}

		private void OnPointerCapture(PointerCaptureEvent evt)
		{
			this.m_CapturedTarget = evt.elementTarget;
			bool flag = this.m_CapturedTarget == null;
			if (!flag)
			{
				this.m_TouchPointerMoveAllowed = true;
				this.m_CapturedTarget.RegisterCallback<PointerMoveEvent>(this.m_CapturedTargetPointerMoveCallback, TrickleDown.NoTrickleDown);
				this.m_CapturedTarget.RegisterCallback<PointerUpEvent>(this.m_CapturedTargetPointerUpCallback, TrickleDown.NoTrickleDown);
			}
		}

		private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
		{
			this.ReleaseScrolling(evt.pointerId, evt.target);
			bool flag = this.m_CapturedTarget == null;
			if (!flag)
			{
				this.m_CapturedTarget.UnregisterCallback<PointerMoveEvent>(this.m_CapturedTargetPointerMoveCallback, TrickleDown.NoTrickleDown);
				this.m_CapturedTarget.UnregisterCallback<PointerUpEvent>(this.m_CapturedTargetPointerUpCallback, TrickleDown.NoTrickleDown);
				this.m_CapturedTarget = null;
			}
		}

		private void OnGeometryChanged(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				bool flag2 = this.needsVertical;
				bool flag3 = this.needsHorizontal;
				bool flag4 = this.m_FirstLayoutPass == -1;
				if (flag4)
				{
					this.m_FirstLayoutPass = evt.layoutPass;
				}
				else
				{
					bool flag5 = evt.layoutPass - this.m_FirstLayoutPass > 5;
					if (flag5)
					{
						flag2 = flag2 || this.isVerticalScrollDisplayed;
						flag3 = flag3 || this.isHorizontalScrollDisplayed;
					}
				}
				this.UpdateScrollers(flag3, flag2);
				this.UpdateContentViewTransform();
				this.ScheduleResetLayoutPass();
			}
		}

		private void OnVerticalSliderViewDataRestored()
		{
			this.verticalScroller.highValue = (float.IsNaN(this.scrollableHeight) ? this.verticalScroller.highValue : this.scrollableHeight);
			this.UpdateContentViewTransform();
		}

		private void OnHorizontalSliderViewDataRestored()
		{
			this.horizontalScroller.highValue = (float.IsNaN(this.scrollableWidth) ? this.horizontalScroller.highValue : this.scrollableWidth);
			this.UpdateContentViewTransform();
		}

		private void OnVerticalScrollerSetValueWithoutNotify(float value)
		{
			this.m_ScrollOffset = new Vector2(this.scrollOffset.x, value);
			base.SaveViewData();
		}

		private void OnHorizontalScrollerSetValueWithoutNotify(float value)
		{
			this.m_ScrollOffset = new Vector2(value, this.scrollOffset.y);
			base.SaveViewData();
		}

		private void ScheduleResetLayoutPass()
		{
			bool flag = this.m_ScheduledLayoutPassResetItem == null;
			if (flag)
			{
				this.m_ScheduledLayoutPassResetItem = base.schedule.Execute(new Action(this.ResetLayoutPass));
			}
			else
			{
				this.m_ScheduledLayoutPassResetItem.Pause();
				this.m_ScheduledLayoutPassResetItem.Resume();
			}
		}

		private void ResetLayoutPass()
		{
			this.m_FirstLayoutPass = -1;
		}

		private static float ComputeElasticOffset(float deltaPointer, float initialScrollOffset, float lowLimit, float hardLowLimit, float highLimit, float hardHighLimit)
		{
			initialScrollOffset = Mathf.Max(initialScrollOffset, hardLowLimit * 0.95f);
			initialScrollOffset = Mathf.Min(initialScrollOffset, hardHighLimit * 0.95f);
			bool flag = initialScrollOffset < lowLimit && hardLowLimit < lowLimit;
			float num;
			float num3;
			if (flag)
			{
				num = lowLimit - hardLowLimit;
				float num2 = (lowLimit - initialScrollOffset) / num;
				num3 = num2 * num / (1f - num2);
				num3 += deltaPointer;
				initialScrollOffset = lowLimit;
			}
			else
			{
				bool flag2 = initialScrollOffset > highLimit && hardHighLimit > highLimit;
				if (flag2)
				{
					num = hardHighLimit - highLimit;
					float num4 = (initialScrollOffset - highLimit) / num;
					num3 = -1f * num4 * num / (1f - num4);
					num3 += deltaPointer;
					initialScrollOffset = highLimit;
				}
				else
				{
					num3 = deltaPointer;
				}
			}
			float num5 = initialScrollOffset - num3;
			bool flag3 = num5 < lowLimit;
			float num6;
			if (flag3)
			{
				num3 = lowLimit - num5;
				initialScrollOffset = lowLimit;
				num = lowLimit - hardLowLimit;
				num6 = 1f;
			}
			else
			{
				bool flag4 = num5 <= highLimit;
				if (flag4)
				{
					return num5;
				}
				num3 = num5 - highLimit;
				initialScrollOffset = highLimit;
				num = hardHighLimit - highLimit;
				num6 = -1f;
			}
			bool flag5 = Mathf.Abs(num3) < 1E-30f;
			float num7;
			if (flag5)
			{
				num7 = initialScrollOffset;
			}
			else
			{
				float num8 = num3 / (num3 + num);
				num8 *= num;
				num8 *= num6;
				num5 = initialScrollOffset - num8;
				num7 = num5;
			}
			return num7;
		}

		private void ComputeInitialSpringBackVelocity()
		{
			bool flag = this.touchScrollBehavior != ScrollView.TouchScrollBehavior.Elastic;
			if (flag)
			{
				this.m_SpringBackVelocity = Vector2.zero;
			}
			else
			{
				bool flag2 = this.scrollOffset.x < this.m_LowBounds.x;
				if (flag2)
				{
					this.m_SpringBackVelocity.x = this.m_LowBounds.x - this.scrollOffset.x;
				}
				else
				{
					bool flag3 = this.scrollOffset.x > this.m_HighBounds.x;
					if (flag3)
					{
						this.m_SpringBackVelocity.x = this.m_HighBounds.x - this.scrollOffset.x;
					}
					else
					{
						this.m_SpringBackVelocity.x = 0f;
					}
				}
				bool flag4 = this.scrollOffset.y < this.m_LowBounds.y;
				if (flag4)
				{
					this.m_SpringBackVelocity.y = this.m_LowBounds.y - this.scrollOffset.y;
				}
				else
				{
					bool flag5 = this.scrollOffset.y > this.m_HighBounds.y;
					if (flag5)
					{
						this.m_SpringBackVelocity.y = this.m_HighBounds.y - this.scrollOffset.y;
					}
					else
					{
						this.m_SpringBackVelocity.y = 0f;
					}
				}
			}
		}

		private void SpringBack()
		{
			bool flag = this.touchScrollBehavior != ScrollView.TouchScrollBehavior.Elastic;
			if (flag)
			{
				this.m_SpringBackVelocity = Vector2.zero;
			}
			else
			{
				Vector2 scrollOffset = this.scrollOffset;
				bool flag2 = scrollOffset.x < this.m_LowBounds.x;
				if (flag2)
				{
					scrollOffset.x = Mathf.SmoothDamp(scrollOffset.x, this.m_LowBounds.x, ref this.m_SpringBackVelocity.x, this.elasticity, float.PositiveInfinity, this.elapsedTimeSinceLastHorizontalTouchScroll);
					bool flag3 = Mathf.Abs(this.m_SpringBackVelocity.x) < base.scaledPixelsPerPoint;
					if (flag3)
					{
						this.m_SpringBackVelocity.x = 0f;
					}
				}
				else
				{
					bool flag4 = scrollOffset.x > this.m_HighBounds.x;
					if (flag4)
					{
						scrollOffset.x = Mathf.SmoothDamp(scrollOffset.x, this.m_HighBounds.x, ref this.m_SpringBackVelocity.x, this.elasticity, float.PositiveInfinity, this.elapsedTimeSinceLastHorizontalTouchScroll);
						bool flag5 = Mathf.Abs(this.m_SpringBackVelocity.x) < base.scaledPixelsPerPoint;
						if (flag5)
						{
							this.m_SpringBackVelocity.x = 0f;
						}
					}
					else
					{
						this.m_SpringBackVelocity.x = 0f;
					}
				}
				bool flag6 = scrollOffset.y < this.m_LowBounds.y;
				if (flag6)
				{
					scrollOffset.y = Mathf.SmoothDamp(scrollOffset.y, this.m_LowBounds.y, ref this.m_SpringBackVelocity.y, this.elasticity, float.PositiveInfinity, this.elapsedTimeSinceLastVerticalTouchScroll);
					bool flag7 = Mathf.Abs(this.m_SpringBackVelocity.y) < base.scaledPixelsPerPoint;
					if (flag7)
					{
						this.m_SpringBackVelocity.y = 0f;
					}
				}
				else
				{
					bool flag8 = scrollOffset.y > this.m_HighBounds.y;
					if (flag8)
					{
						scrollOffset.y = Mathf.SmoothDamp(scrollOffset.y, this.m_HighBounds.y, ref this.m_SpringBackVelocity.y, this.elasticity, float.PositiveInfinity, this.elapsedTimeSinceLastVerticalTouchScroll);
						bool flag9 = Mathf.Abs(this.m_SpringBackVelocity.y) < base.scaledPixelsPerPoint;
						if (flag9)
						{
							this.m_SpringBackVelocity.y = 0f;
						}
					}
					else
					{
						this.m_SpringBackVelocity.y = 0f;
					}
				}
				this.scrollOffset = scrollOffset;
			}
		}

		internal void ApplyScrollInertia()
		{
			bool flag = this.hasInertia && this.m_Velocity != Vector2.zero;
			if (flag)
			{
				Vector2 vector = Vector2.zero;
				float num = 0f;
				while (num < this.elapsedTimeSinceLastVerticalTouchScroll)
				{
					this.m_Velocity *= Mathf.Pow(this.scrollDecelerationRate, this.k_TouchScrollInertiaBaseTimeInterval);
					num += this.k_TouchScrollInertiaBaseTimeInterval;
					vector += this.m_Velocity * this.k_TouchScrollInertiaBaseTimeInterval;
				}
				float num2 = this.elapsedTimeSinceLastVerticalTouchScroll - num;
				bool flag2 = num2 > 0f && num2 < this.k_TouchScrollInertiaBaseTimeInterval;
				if (flag2)
				{
					this.m_Velocity *= Mathf.Pow(this.scrollDecelerationRate, num2);
					vector += this.m_Velocity * num2;
				}
				float num3 = base.scaledPixelsPerPoint * this.k_ScaledPixelsPerPointMultiplier;
				bool flag3 = Mathf.Abs(this.m_Velocity.x) <= num3 || (this.touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic && (this.scrollOffset.x < this.m_LowBounds.x || this.scrollOffset.x > this.m_HighBounds.x));
				if (flag3)
				{
					this.m_Velocity.x = 0f;
				}
				bool flag4 = Mathf.Abs(this.m_Velocity.y) <= num3 || (this.touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic && (this.scrollOffset.y < this.m_LowBounds.y || this.scrollOffset.y > this.m_HighBounds.y));
				if (flag4)
				{
					this.m_Velocity.y = 0f;
				}
				this.scrollOffset += vector;
			}
			else
			{
				this.m_Velocity = Vector2.zero;
			}
		}

		private void PostPointerUpAnimation()
		{
			this.elapsedTimeSinceLastVerticalTouchScroll = Time.unscaledTime - this.previousVerticalTouchScrollTimeStamp;
			this.previousVerticalTouchScrollTimeStamp = Time.unscaledTime;
			this.elapsedTimeSinceLastHorizontalTouchScroll = Time.unscaledTime - this.previousHorizontalTouchScrollTimeStamp;
			this.previousHorizontalTouchScrollTimeStamp = Time.unscaledTime;
			this.ApplyScrollInertia();
			this.SpringBack();
			bool flag = this.m_SpringBackVelocity == Vector2.zero && this.m_Velocity == Vector2.zero;
			if (flag)
			{
				this.m_PostPointerUpAnimation.Pause();
				this.elapsedTimeSinceLastVerticalTouchScroll = 0f;
				this.elapsedTimeSinceLastHorizontalTouchScroll = 0f;
				this.previousVerticalTouchScrollTimeStamp = 0f;
				this.previousHorizontalTouchScrollTimeStamp = 0f;
			}
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			bool flag = evt.pointerType == PointerType.mouse || !evt.isPrimary;
			if (!flag)
			{
				bool flag2 = evt.pointerId != PointerId.invalidPointerId;
				if (flag2)
				{
					this.ReleaseScrolling(evt.pointerId, evt.target);
				}
				IVisualElementScheduledItem postPointerUpAnimation = this.m_PostPointerUpAnimation;
				if (postPointerUpAnimation != null)
				{
					postPointerUpAnimation.Pause();
				}
				bool flag3 = Mathf.Abs(this.m_Velocity.x) > 10f || Mathf.Abs(this.m_Velocity.y) > 10f;
				this.m_TouchPointerMoveAllowed = true;
				this.m_StartedMoving = false;
				this.InitTouchScrolling(evt.position);
				bool flag4 = flag3;
				if (flag4)
				{
					this.contentContainer.CapturePointer(evt.pointerId);
					this.contentContainer.panel.PreventCompatibilityMouseEvents(evt.pointerId);
					evt.StopPropagation();
					this.m_TouchStoppedVelocity = true;
				}
			}
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			bool flag = evt.pointerType == PointerType.mouse || !evt.isPrimary || !this.m_TouchPointerMoveAllowed;
			if (!flag)
			{
				bool isHandledByDraggable = evt.isHandledByDraggable;
				if (isHandledByDraggable)
				{
					this.m_PointerStartPosition = evt.position;
					this.m_StartPosition = this.scrollOffset;
				}
				else
				{
					Vector2 vector = evt.position;
					Vector2 vector2 = vector - this.m_PointerStartPosition;
					bool flag2 = this.mode == ScrollViewMode.Horizontal;
					if (flag2)
					{
						vector2.y = 0f;
					}
					else
					{
						bool flag3 = this.mode == ScrollViewMode.Vertical;
						if (flag3)
						{
							vector2.x = 0f;
						}
					}
					bool flag4 = !this.m_TouchStoppedVelocity && !this.m_StartedMoving && vector2.sqrMagnitude < 100f;
					if (!flag4)
					{
						ScrollView.TouchScrollingResult touchScrollingResult = this.ComputeTouchScrolling(evt.position);
						bool flag5 = touchScrollingResult != ScrollView.TouchScrollingResult.Forward;
						if (flag5)
						{
							evt.isHandledByDraggable = true;
							evt.StopPropagation();
							bool flag6 = !this.contentContainer.HasPointerCapture(evt.pointerId);
							if (flag6)
							{
								this.contentContainer.CapturePointer(evt.pointerId);
							}
						}
						else
						{
							this.m_Velocity = Vector2.zero;
						}
					}
				}
			}
		}

		private void OnPointerCancel(PointerCancelEvent evt)
		{
			this.ReleaseScrolling(evt.pointerId, evt.target);
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			bool flag = this.ReleaseScrolling(evt.pointerId, evt.target);
			if (flag)
			{
				this.contentContainer.panel.PreventCompatibilityMouseEvents(evt.pointerId);
				evt.StopPropagation();
			}
		}

		internal void InitTouchScrolling(Vector2 position)
		{
			this.m_PointerStartPosition = position;
			this.m_StartPosition = this.scrollOffset;
			this.m_Velocity = Vector2.zero;
			this.m_SpringBackVelocity = Vector2.zero;
			this.m_LowBounds = new Vector2(Mathf.Min(this.horizontalScroller.lowValue, this.horizontalScroller.highValue), Mathf.Min(this.verticalScroller.lowValue, this.verticalScroller.highValue));
			this.m_HighBounds = new Vector2(Mathf.Max(this.horizontalScroller.lowValue, this.horizontalScroller.highValue), Mathf.Max(this.verticalScroller.lowValue, this.verticalScroller.highValue));
		}

		internal ScrollView.TouchScrollingResult ComputeTouchScrolling(Vector2 position)
		{
			bool flag = this.touchScrollBehavior == ScrollView.TouchScrollBehavior.Clamped;
			Vector2 vector;
			if (flag)
			{
				vector = this.m_StartPosition - (position - this.m_PointerStartPosition);
				vector = Vector2.Max(vector, this.m_LowBounds);
				vector = Vector2.Min(vector, this.m_HighBounds);
			}
			else
			{
				bool flag2 = this.touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic;
				if (flag2)
				{
					Vector2 vector2 = position - this.m_PointerStartPosition;
					vector.x = ScrollView.ComputeElasticOffset(vector2.x, this.m_StartPosition.x, this.m_LowBounds.x, this.m_LowBounds.x - this.contentViewport.resolvedStyle.width, this.m_HighBounds.x, this.m_HighBounds.x + this.contentViewport.resolvedStyle.width);
					vector.y = ScrollView.ComputeElasticOffset(vector2.y, this.m_StartPosition.y, this.m_LowBounds.y, this.m_LowBounds.y - this.contentViewport.resolvedStyle.height, this.m_HighBounds.y, this.m_HighBounds.y + this.contentViewport.resolvedStyle.height);
					this.previousVerticalTouchScrollTimeStamp = Time.unscaledTime;
					this.previousHorizontalTouchScrollTimeStamp = Time.unscaledTime;
				}
				else
				{
					vector = this.m_StartPosition - (position - this.m_PointerStartPosition);
				}
			}
			bool flag3 = this.mode == ScrollViewMode.Vertical;
			if (flag3)
			{
				vector.x = this.m_LowBounds.x;
			}
			else
			{
				bool flag4 = this.mode == ScrollViewMode.Horizontal;
				if (flag4)
				{
					vector.y = this.m_LowBounds.y;
				}
			}
			bool flag5 = this.scrollOffset != vector;
			bool flag6 = flag5;
			ScrollView.TouchScrollingResult touchScrollingResult;
			if (flag6)
			{
				touchScrollingResult = (this.ApplyTouchScrolling(vector) ? ScrollView.TouchScrollingResult.Apply : ScrollView.TouchScrollingResult.Forward);
			}
			else
			{
				touchScrollingResult = ((this.m_StartedMoving && this.nestedInteractionKind != ScrollView.NestedInteractionKind.ForwardScrolling) ? ScrollView.TouchScrollingResult.Block : ScrollView.TouchScrollingResult.Forward);
			}
			return touchScrollingResult;
		}

		private bool ApplyTouchScrolling(Vector2 newScrollOffset)
		{
			this.m_StartedMoving = true;
			bool hasInertia = this.hasInertia;
			if (hasInertia)
			{
				bool flag = newScrollOffset == this.m_LowBounds || newScrollOffset == this.m_HighBounds;
				if (flag)
				{
					this.m_Velocity = Vector2.zero;
					this.scrollOffset = newScrollOffset;
					return false;
				}
				bool flag2 = this.m_LastVelocityLerpTime > 0f;
				if (flag2)
				{
					float num = Time.unscaledTime - this.m_LastVelocityLerpTime;
					this.m_Velocity = Vector2.Lerp(this.m_Velocity, Vector2.zero, num * 10f);
				}
				this.m_LastVelocityLerpTime = Time.unscaledTime;
				float num2 = this.k_TouchScrollInertiaBaseTimeInterval;
				Vector2 vector = (newScrollOffset - this.scrollOffset) / num2;
				this.m_Velocity = Vector2.Lerp(this.m_Velocity, vector, num2 * 10f);
			}
			bool flag3 = this.scrollOffset != newScrollOffset;
			this.scrollOffset = newScrollOffset;
			return flag3;
		}

		private bool ReleaseScrolling(int pointerId, IEventHandler target)
		{
			this.m_TouchStoppedVelocity = false;
			this.m_StartedMoving = false;
			this.m_TouchPointerMoveAllowed = false;
			bool flag = target != this.contentContainer || !this.contentContainer.HasPointerCapture(pointerId);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.previousVerticalTouchScrollTimeStamp = Time.unscaledTime;
				this.previousHorizontalTouchScrollTimeStamp = Time.unscaledTime;
				bool flag3 = this.touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic || this.hasInertia;
				if (flag3)
				{
					this.ExecuteElasticSpringAnimation();
				}
				this.contentContainer.ReleasePointer(pointerId);
				flag2 = true;
			}
			return flag2;
		}

		private void ExecuteElasticSpringAnimation()
		{
			this.ComputeInitialSpringBackVelocity();
			bool flag = this.m_PostPointerUpAnimation == null;
			if (flag)
			{
				this.m_PostPointerUpAnimation = base.schedule.Execute(new Action(this.PostPointerUpAnimation)).Every(this.m_ElasticAnimationIntervalMs);
			}
			else
			{
				this.m_PostPointerUpAnimation.Resume();
			}
		}

		private void AdjustScrollers()
		{
			float num = ((this.contentContainer.boundingBox.width > 1E-30f) ? (this.contentViewport.layout.width / this.contentContainer.boundingBox.width) : 1f);
			float num2 = ((this.contentContainer.boundingBox.height > 1E-30f) ? (this.contentViewport.layout.height / this.contentContainer.boundingBox.height) : 1f);
			this.horizontalScroller.Adjust(num);
			this.verticalScroller.Adjust(num2);
		}

		internal void UpdateScrollers(bool displayHorizontal, bool displayVertical)
		{
			this.AdjustScrollers();
			bool flag = this.scrollableHeight > 0f;
			bool flag2 = this.scrollableWidth > 0f;
			this.horizontalScroller.SetEnabled(flag2);
			this.verticalScroller.SetEnabled(flag);
			this.verticalScroller.slider.clampedDragger.acceptClicksIfDisabled = flag;
			this.verticalScroller.lowButton.acceptClicksIfDisabled = flag;
			this.verticalScroller.highButton.acceptClicksIfDisabled = flag;
			this.horizontalScroller.slider.clampedDragger.acceptClicksIfDisabled = flag2;
			this.horizontalScroller.lowButton.acceptClicksIfDisabled = flag2;
			this.horizontalScroller.highButton.acceptClicksIfDisabled = flag2;
			bool flag3 = displayHorizontal && this.m_HorizontalScrollerVisibility != ScrollerVisibility.Hidden;
			bool flag4 = displayVertical && this.m_VerticalScrollerVisibility != ScrollerVisibility.Hidden;
			DisplayStyle displayStyle = (flag3 ? DisplayStyle.Flex : DisplayStyle.None);
			DisplayStyle displayStyle2 = (flag4 ? DisplayStyle.Flex : DisplayStyle.None);
			bool flag5 = displayStyle != this.horizontalScroller.style.display;
			if (flag5)
			{
				this.horizontalScroller.style.display = displayStyle;
			}
			bool flag6 = displayStyle2 != this.verticalScroller.style.display;
			if (flag6)
			{
				this.verticalScroller.style.display = displayStyle2;
			}
			this.verticalScroller.lowValue = 0f;
			this.verticalScroller.highValue = (float.IsNaN(this.scrollableHeight) ? this.verticalScroller.highValue : this.scrollableHeight);
			this.horizontalScroller.lowValue = 0f;
			this.horizontalScroller.highValue = (float.IsNaN(this.scrollableWidth) ? this.horizontalScroller.highValue : this.scrollableWidth);
		}

		private void OnScrollersGeometryChanged(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				bool flag2 = this.needsHorizontal && this.m_HorizontalScrollerVisibility != ScrollerVisibility.Hidden;
				bool flag3 = flag2;
				if (flag3)
				{
					this.horizontalScroller.style.marginRight = this.verticalScroller.layout.width;
				}
				this.AdjustScrollers();
			}
		}

		private void OnScrollWheel(WheelEvent evt)
		{
			bool flag = Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer;
			if (flag)
			{
				bool commandKey = evt.commandKey;
				if (commandKey)
				{
					return;
				}
			}
			else
			{
				bool ctrlKey = evt.ctrlKey;
				if (ctrlKey)
				{
					return;
				}
			}
			bool flag2 = false;
			bool flag3 = this.mode != ScrollViewMode.Horizontal && this.scrollableHeight > 0f;
			bool flag4 = this.mode != ScrollViewMode.Vertical && this.scrollableWidth > 0f;
			float num = ((flag4 && !flag3) ? evt.delta.y : evt.delta.x);
			bool flag5 = (flag4 || flag3) && !this.m_MouseWheelScrollSizeIsInline;
			if (flag5)
			{
				bool flag6;
				if (!this.m_SingleLineHeightDirtyFlag)
				{
					VisualElement parent = base.parent;
					flag6 = parent != null && parent.isRootVisualContainer && base.parent != this.m_AttachedRootVisualContainer;
				}
				else
				{
					flag6 = true;
				}
				bool flag7 = flag6;
				if (flag7)
				{
					this.ReadSingleLineHeight();
				}
			}
			float num2 = (this.m_MouseWheelScrollSizeIsInline ? this.mouseWheelScrollSize : this.m_SingleLineHeight);
			bool flag8 = flag3;
			if (flag8)
			{
				float value = this.verticalScroller.value;
				this.verticalScroller.value += evt.delta.y * ((this.verticalScroller.lowValue < this.verticalScroller.highValue) ? 1f : (-1f)) * num2;
				bool flag9 = this.nestedInteractionKind == ScrollView.NestedInteractionKind.StopScrolling || !Mathf.Approximately(this.verticalScroller.value, value);
				if (flag9)
				{
					evt.StopPropagation();
					flag2 = true;
				}
			}
			bool flag10 = flag4;
			if (flag10)
			{
				float value2 = this.horizontalScroller.value;
				this.horizontalScroller.value += num * ((this.horizontalScroller.lowValue < this.horizontalScroller.highValue) ? 1f : (-1f)) * num2;
				bool flag11 = this.nestedInteractionKind == ScrollView.NestedInteractionKind.StopScrolling || !Mathf.Approximately(this.horizontalScroller.value, value2);
				if (flag11)
				{
					evt.StopPropagation();
					flag2 = true;
				}
			}
			bool flag12 = flag2;
			if (flag12)
			{
				this.UpdateElasticBehaviour();
				this.UpdateContentViewTransform();
			}
		}

		private void OnRootCustomStyleResolved(CustomStyleResolvedEvent evt)
		{
			this.MarkSingleLineHeightDirty();
		}

		private void MarkSingleLineHeightDirty()
		{
			this.m_SingleLineHeightDirtyFlag = true;
		}

		private void OnRootPointerUp(PointerUpEvent evt)
		{
			this.m_TouchPointerMoveAllowed = false;
		}

		private unsafe void ReadSingleLineHeight()
		{
			StylePropertyValue stylePropertyValue;
			bool flag = base.computedStyle.customProperties != null && base.computedStyle.customProperties.TryGetValue("--unity-metrics-single_line-height", out stylePropertyValue);
			if (flag)
			{
				this.m_SingleLineHeightDirtyFlag = false;
				Dimension dimension;
				bool flag2 = stylePropertyValue.sheet.TryReadDimension(stylePropertyValue.handle, out dimension);
				if (flag2)
				{
					this.m_SingleLineHeight = dimension.value;
					return;
				}
			}
			VisualElement firstAncestorWhere = base.GetFirstAncestorWhere((VisualElement x) => x.isRootVisualContainer);
			bool flag3 = firstAncestorWhere != this.m_AttachedRootVisualContainer;
			if (flag3)
			{
				this.m_AttachedRootVisualContainer.UnregisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnRootCustomStyleResolved), TrickleDown.NoTrickleDown);
				this.m_AttachedRootVisualContainer = firstAncestorWhere;
				this.m_AttachedRootVisualContainer.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnRootCustomStyleResolved), TrickleDown.NoTrickleDown);
			}
			VisualElement attachedRootVisualContainer = this.m_AttachedRootVisualContainer;
			StylePropertyValue stylePropertyValue2;
			bool flag4 = attachedRootVisualContainer != null && attachedRootVisualContainer.computedStyle->customProperties != null && this.m_AttachedRootVisualContainer.computedStyle.customProperties.TryGetValue("--unity-metrics-single_line-height", out stylePropertyValue2);
			if (flag4)
			{
				this.m_SingleLineHeightDirtyFlag = false;
				Dimension dimension2;
				bool flag5 = stylePropertyValue2.sheet.TryReadDimension(stylePropertyValue2.handle, out dimension2);
				if (flag5)
				{
					this.m_SingleLineHeight = dimension2.value;
					return;
				}
			}
			this.m_SingleLineHeight = UIElementsUtility.singleLineHeight;
			this.m_SingleLineHeightDirtyFlag = false;
		}

		private void UpdateElasticBehaviour()
		{
			bool flag = this.touchScrollBehavior == ScrollView.TouchScrollBehavior.Elastic;
			if (flag)
			{
				this.m_LowBounds = new Vector2(Mathf.Min(this.horizontalScroller.lowValue, this.horizontalScroller.highValue), Mathf.Min(this.verticalScroller.lowValue, this.verticalScroller.highValue));
				this.m_HighBounds = new Vector2(Mathf.Max(this.horizontalScroller.lowValue, this.horizontalScroller.highValue), Mathf.Max(this.verticalScroller.lowValue, this.verticalScroller.highValue));
				this.ExecuteElasticSpringAnimation();
			}
		}

		internal void SetScrollOffsetWithoutNotify(Vector2 value)
		{
			this.horizontalScroller.slider.SetValueWithoutNotify(value.x);
			this.verticalScroller.slider.SetValueWithoutNotify(value.y);
			this.m_ScrollOffset = new Vector2(this.horizontalScroller.value, this.verticalScroller.value);
			base.SaveViewData();
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			bool flag = string.IsNullOrEmpty(this.verticalScroller.viewDataKey) && string.IsNullOrEmpty(this.verticalScroller.slider.viewDataKey) && string.IsNullOrEmpty(this.horizontalScroller.viewDataKey) && string.IsNullOrEmpty(this.horizontalScroller.slider.viewDataKey);
			if (!flag)
			{
				string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
				base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
				this.UpdateContentViewTransform();
			}
		}

		internal static readonly BindingId horizontalScrollerVisibilityProperty = "horizontalScrollerVisibility";

		internal static readonly BindingId verticalScrollerVisibilityProperty = "verticalScrollerVisibility";

		internal static readonly BindingId scrollOffsetProperty = "scrollOffset";

		internal static readonly BindingId horizontalPageSizeProperty = "horizontalPageSize";

		internal static readonly BindingId verticalPageSizeProperty = "verticalPageSize";

		internal static readonly BindingId mouseWheelScrollSizeProperty = "mouseWheelScrollSize";

		internal static readonly BindingId scrollDecelerationRateProperty = "scrollDecelerationRate";

		internal static readonly BindingId elasticityProperty = "elasticity";

		internal static readonly BindingId touchScrollBehaviorProperty = "touchScrollBehavior";

		internal static readonly BindingId nestedInteractionKindProperty = "nestedInteractionKind";

		internal static readonly BindingId modeProperty = "mode";

		internal static readonly BindingId elasticAnimationIntervalMsProperty = "elasticAnimationIntervalMs";

		private VisualElement m_DeferredScrollToElement;

		private IVisualElementScheduledItem m_DeferredScrollTo;

		private const int k_MaxLocalLayoutPassCount = 5;

		private int m_FirstLayoutPass = -1;

		private ScrollerVisibility m_HorizontalScrollerVisibility;

		private ScrollerVisibility m_VerticalScrollerVisibility;

		private long m_ElasticAnimationIntervalMs = 16L;

		private const float k_SizeThreshold = 0.001f;

		private VisualElement m_AttachedRootVisualContainer;

		private float m_SingleLineHeight = UIElementsUtility.singleLineHeight;

		private bool m_SingleLineHeightDirtyFlag;

		private const string k_SingleLineHeightPropertyName = "--unity-metrics-single_line-height";

		private const float k_ScrollPageOverlapFactor = 0.1f;

		internal const float k_UnsetPageSizeValue = -1f;

		internal const float k_MouseWheelScrollSizeDefaultValue = 18f;

		internal const float k_MouseWheelScrollSizeUnset = -1f;

		internal bool m_MouseWheelScrollSizeIsInline;

		[SerializeField]
		[DontCreateProperty]
		private Vector2 m_ScrollOffset;

		private float m_HorizontalPageSize;

		private float m_VerticalPageSize;

		private float m_MouseWheelScrollSize = 18f;

		private static readonly float k_DefaultScrollDecelerationRate = 0.135f;

		private float m_ScrollDecelerationRate = ScrollView.k_DefaultScrollDecelerationRate;

		private float k_ScaledPixelsPerPointMultiplier = 10f;

		private float k_TouchScrollInertiaBaseTimeInterval = 0.004167f;

		private static readonly float k_DefaultElasticity = 0.1f;

		private float m_Elasticity = ScrollView.k_DefaultElasticity;

		private ScrollView.TouchScrollBehavior m_TouchScrollBehavior;

		private ScrollView.NestedInteractionKind m_NestedInteractionKind;

		private VisualElement m_ContentContainer;

		private VisualElement m_ContentAndVerticalScrollContainer;

		private float previousVerticalTouchScrollTimeStamp = 0f;

		private float previousHorizontalTouchScrollTimeStamp = 0f;

		private float elapsedTimeSinceLastVerticalTouchScroll = 0f;

		private float elapsedTimeSinceLastHorizontalTouchScroll = 0f;

		public static readonly string ussClassName = "unity-scroll-view";

		public static readonly string viewportUssClassName = ScrollView.ussClassName + "__content-viewport";

		public static readonly string horizontalVariantViewportUssClassName = ScrollView.viewportUssClassName + "--horizontal";

		public static readonly string verticalVariantViewportUssClassName = ScrollView.viewportUssClassName + "--vertical";

		public static readonly string verticalHorizontalVariantViewportUssClassName = ScrollView.viewportUssClassName + "--vertical-horizontal";

		public static readonly string contentAndVerticalScrollUssClassName = ScrollView.ussClassName + "__content-and-vertical-scroll-container";

		public static readonly string contentUssClassName = ScrollView.ussClassName + "__content-container";

		public static readonly string horizontalVariantContentUssClassName = ScrollView.contentUssClassName + "--horizontal";

		public static readonly string verticalVariantContentUssClassName = ScrollView.contentUssClassName + "--vertical";

		public static readonly string verticalHorizontalVariantContentUssClassName = ScrollView.contentUssClassName + "--vertical-horizontal";

		public static readonly string hScrollerUssClassName = ScrollView.ussClassName + "__horizontal-scroller";

		public static readonly string vScrollerUssClassName = ScrollView.ussClassName + "__vertical-scroller";

		public static readonly string horizontalVariantUssClassName = ScrollView.ussClassName + "--horizontal";

		public static readonly string verticalVariantUssClassName = ScrollView.ussClassName + "--vertical";

		public static readonly string verticalHorizontalVariantUssClassName = ScrollView.ussClassName + "--vertical-horizontal";

		public static readonly string scrollVariantUssClassName = ScrollView.ussClassName + "--scroll";

		private ScrollViewMode m_Mode;

		private IVisualElementScheduledItem m_ScheduledLayoutPassResetItem;

		private const float k_VelocityLerpTimeFactor = 10f;

		internal const float ScrollThresholdSquared = 100f;

		private Vector2 m_StartPosition;

		private Vector2 m_PointerStartPosition;

		private Vector2 m_Velocity;

		private Vector2 m_SpringBackVelocity;

		private Vector2 m_LowBounds;

		private Vector2 m_HighBounds;

		private float m_LastVelocityLerpTime;

		private bool m_StartedMoving;

		private bool m_TouchPointerMoveAllowed;

		private bool m_TouchStoppedVelocity;

		private VisualElement m_CapturedTarget;

		private EventCallback<PointerMoveEvent> m_CapturedTargetPointerMoveCallback;

		private EventCallback<PointerUpEvent> m_CapturedTargetPointerUpCallback;

		internal IVisualElementScheduledItem m_PostPointerUpAnimation;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(ScrollView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("mode", "mode", null, Array.Empty<string>()),
					new UxmlAttributeNames("nestedInteractionKind", "nested-interaction-kind", null, Array.Empty<string>()),
					new UxmlAttributeNames("showHorizontal", "show-horizontal-scroller", null, Array.Empty<string>()),
					new UxmlAttributeNames("showVertical", "show-vertical-scroller", null, Array.Empty<string>()),
					new UxmlAttributeNames("horizontalScrollerVisibility", "horizontal-scroller-visibility", null, Array.Empty<string>()),
					new UxmlAttributeNames("verticalScrollerVisibility", "vertical-scroller-visibility", null, Array.Empty<string>()),
					new UxmlAttributeNames("horizontalPageSize", "horizontal-page-size", null, Array.Empty<string>()),
					new UxmlAttributeNames("verticalPageSize", "vertical-page-size", null, Array.Empty<string>()),
					new UxmlAttributeNames("mouseWheelScrollSize", "mouse-wheel-scroll-size", null, Array.Empty<string>()),
					new UxmlAttributeNames("touchScrollBehavior", "touch-scroll-type", null, Array.Empty<string>()),
					new UxmlAttributeNames("scrollDecelerationRate", "scroll-deceleration-rate", null, Array.Empty<string>()),
					new UxmlAttributeNames("elasticity", "elasticity", null, Array.Empty<string>()),
					new UxmlAttributeNames("elasticAnimationIntervalMs", "elastic-animation-interval-ms", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new ScrollView();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				ScrollView scrollView = (ScrollView)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.mode_UxmlAttributeFlags);
				if (flag)
				{
					scrollView.mode = this.mode;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.horizontalScrollerVisibility_UxmlAttributeFlags);
				if (flag2)
				{
					scrollView.horizontalScrollerVisibility = this.horizontalScrollerVisibility;
				}
				else
				{
					bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showHorizontal_UxmlAttributeFlags);
					if (flag3)
					{
						scrollView.showHorizontal = this.showHorizontal;
					}
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.verticalScrollerVisibility_UxmlAttributeFlags);
				if (flag4)
				{
					scrollView.verticalScrollerVisibility = this.verticalScrollerVisibility;
				}
				else
				{
					bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showVertical_UxmlAttributeFlags);
					if (flag5)
					{
						scrollView.showVertical = this.showVertical;
					}
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.nestedInteractionKind_UxmlAttributeFlags);
				if (flag6)
				{
					scrollView.nestedInteractionKind = this.nestedInteractionKind;
				}
				bool flag7 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.horizontalPageSize_UxmlAttributeFlags);
				if (flag7)
				{
					scrollView.horizontalPageSize = this.horizontalPageSize;
				}
				bool flag8 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.verticalPageSize_UxmlAttributeFlags);
				if (flag8)
				{
					scrollView.verticalPageSize = this.verticalPageSize;
				}
				bool flag9 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.mouseWheelScrollSize_UxmlAttributeFlags);
				if (flag9)
				{
					scrollView.mouseWheelScrollSize = this.mouseWheelScrollSize;
				}
				bool flag10 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.scrollDecelerationRate_UxmlAttributeFlags);
				if (flag10)
				{
					scrollView.scrollDecelerationRate = this.scrollDecelerationRate;
				}
				bool flag11 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.touchScrollBehavior_UxmlAttributeFlags);
				if (flag11)
				{
					scrollView.touchScrollBehavior = this.touchScrollBehavior;
				}
				bool flag12 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.elasticity_UxmlAttributeFlags);
				if (flag12)
				{
					scrollView.elasticity = this.elasticity;
				}
				bool flag13 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.elasticAnimationIntervalMs_UxmlAttributeFlags);
				if (flag13)
				{
					scrollView.elasticAnimationIntervalMs = this.elasticAnimationIntervalMs;
				}
			}

			[SerializeField]
			private long elasticAnimationIntervalMs;

			[SerializeField]
			private ScrollViewMode mode;

			[SerializeField]
			private ScrollView.NestedInteractionKind nestedInteractionKind;

			[SerializeField]
			private ScrollerVisibility horizontalScrollerVisibility;

			[SerializeField]
			private ScrollerVisibility verticalScrollerVisibility;

			[SerializeField]
			private float horizontalPageSize;

			[SerializeField]
			private float verticalPageSize;

			[SerializeField]
			private float mouseWheelScrollSize;

			[UxmlAttribute("touch-scroll-type")]
			[SerializeField]
			private ScrollView.TouchScrollBehavior touchScrollBehavior;

			[SerializeField]
			private float scrollDecelerationRate;

			[SerializeField]
			private float elasticity;

			[HideInInspector]
			[SerializeField]
			[UxmlAttribute("show-horizontal-scroller")]
			private bool showHorizontal;

			[SerializeField]
			[HideInInspector]
			[UxmlAttribute("show-vertical-scroller")]
			private bool showVertical;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags mode_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags nestedInteractionKind_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showHorizontal_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showVertical_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags horizontalScrollerVisibility_UxmlAttributeFlags;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags verticalScrollerVisibility_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags horizontalPageSize_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags verticalPageSize_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags mouseWheelScrollSize_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags touchScrollBehavior_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags scrollDecelerationRate_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags elasticity_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags elasticAnimationIntervalMs_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<ScrollView, ScrollView.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				ScrollView scrollView = (ScrollView)ve;
				scrollView.mode = this.m_ScrollViewMode.GetValueFromBag(bag, cc);
				ScrollerVisibility scrollerVisibility = ScrollerVisibility.Auto;
				bool flag = this.m_HorizontalScrollerVisibility.TryGetValueFromBag(bag, cc, ref scrollerVisibility);
				if (flag)
				{
					scrollView.horizontalScrollerVisibility = scrollerVisibility;
				}
				else
				{
					scrollView.showHorizontal = this.m_ShowHorizontal.GetValueFromBag(bag, cc);
				}
				ScrollerVisibility scrollerVisibility2 = ScrollerVisibility.Auto;
				bool flag2 = this.m_VerticalScrollerVisibility.TryGetValueFromBag(bag, cc, ref scrollerVisibility2);
				if (flag2)
				{
					scrollView.verticalScrollerVisibility = scrollerVisibility2;
				}
				else
				{
					scrollView.showVertical = this.m_ShowVertical.GetValueFromBag(bag, cc);
				}
				scrollView.nestedInteractionKind = this.m_NestedInteractionKind.GetValueFromBag(bag, cc);
				scrollView.horizontalPageSize = this.m_HorizontalPageSize.GetValueFromBag(bag, cc);
				scrollView.verticalPageSize = this.m_VerticalPageSize.GetValueFromBag(bag, cc);
				scrollView.mouseWheelScrollSize = this.m_MouseWheelScrollSize.GetValueFromBag(bag, cc);
				scrollView.scrollDecelerationRate = this.m_ScrollDecelerationRate.GetValueFromBag(bag, cc);
				scrollView.touchScrollBehavior = this.m_TouchScrollBehavior.GetValueFromBag(bag, cc);
				scrollView.elasticity = this.m_Elasticity.GetValueFromBag(bag, cc);
			}

			private UxmlEnumAttributeDescription<ScrollViewMode> m_ScrollViewMode = new UxmlEnumAttributeDescription<ScrollViewMode>
			{
				name = "mode",
				defaultValue = ScrollViewMode.Vertical
			};

			private UxmlEnumAttributeDescription<ScrollView.NestedInteractionKind> m_NestedInteractionKind = new UxmlEnumAttributeDescription<ScrollView.NestedInteractionKind>
			{
				name = "nested-interaction-kind",
				defaultValue = ScrollView.NestedInteractionKind.Default
			};

			private UxmlBoolAttributeDescription m_ShowHorizontal = new UxmlBoolAttributeDescription
			{
				name = "show-horizontal-scroller"
			};

			private UxmlBoolAttributeDescription m_ShowVertical = new UxmlBoolAttributeDescription
			{
				name = "show-vertical-scroller"
			};

			private UxmlEnumAttributeDescription<ScrollerVisibility> m_HorizontalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
			{
				name = "horizontal-scroller-visibility"
			};

			private UxmlEnumAttributeDescription<ScrollerVisibility> m_VerticalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
			{
				name = "vertical-scroller-visibility"
			};

			private UxmlFloatAttributeDescription m_HorizontalPageSize = new UxmlFloatAttributeDescription
			{
				name = "horizontal-page-size",
				defaultValue = -1f
			};

			private UxmlFloatAttributeDescription m_VerticalPageSize = new UxmlFloatAttributeDescription
			{
				name = "vertical-page-size",
				defaultValue = -1f
			};

			private UxmlFloatAttributeDescription m_MouseWheelScrollSize = new UxmlFloatAttributeDescription
			{
				name = "mouse-wheel-scroll-size",
				defaultValue = 18f
			};

			private UxmlEnumAttributeDescription<ScrollView.TouchScrollBehavior> m_TouchScrollBehavior = new UxmlEnumAttributeDescription<ScrollView.TouchScrollBehavior>
			{
				name = "touch-scroll-type",
				defaultValue = ScrollView.TouchScrollBehavior.Clamped
			};

			private UxmlFloatAttributeDescription m_ScrollDecelerationRate = new UxmlFloatAttributeDescription
			{
				name = "scroll-deceleration-rate",
				defaultValue = ScrollView.k_DefaultScrollDecelerationRate
			};

			private UxmlFloatAttributeDescription m_Elasticity = new UxmlFloatAttributeDescription
			{
				name = "elasticity",
				defaultValue = ScrollView.k_DefaultElasticity
			};
		}

		public enum TouchScrollBehavior
		{
			Unrestricted,
			Elastic,
			Clamped
		}

		public enum NestedInteractionKind
		{
			Default,
			StopScrolling,
			ForwardScrolling
		}

		internal enum TouchScrollingResult
		{
			Apply,
			Forward,
			Block
		}
	}
}
