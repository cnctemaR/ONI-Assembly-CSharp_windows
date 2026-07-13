using System;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class ScrollContainer : VisualElement
	{
		public override VisualElement contentContainer
		{
			get
			{
				return this.m_Container;
			}
		}

		public VisualElement viewport
		{
			get
			{
				return this.m_Viewport;
			}
		}

		public CollectionViewScroller verticalScroller
		{
			get
			{
				return this.m_VerticalScroller;
			}
			private set
			{
				this.m_VerticalScroller = value;
			}
		}

		public CollectionViewScroller horizontalScroller
		{
			get
			{
				return this.m_HorizontalScroller;
			}
			private set
			{
				this.m_HorizontalScroller = value;
			}
		}

		public Vector2 containerOffset
		{
			get
			{
				return this.m_ContainerOffset;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.m_ContainerOffset.x, value.x) || !Mathf.Approximately(this.m_ContainerOffset.y, value.y);
				if (flag)
				{
					this.m_ContainerOffset = value;
					this.m_Container.style.translate = new Vector3(-this.m_ContainerOffset.x, -this.m_ContainerOffset.y, 0f);
				}
			}
		}

		public ScrollContainer()
		{
			base.AddToClassList(ScrollContainer.ussClassName);
			this.m_Viewport = new VisualElement();
			this.m_Viewport.AddToClassList(ScrollContainer.contentViewportUssClassName);
			this.m_Container = new VisualElement();
			this.m_Container.AddToClassList(ScrollContainer.containerUssClassName);
			this.m_Container.RegisterCallback<WheelEvent>(new EventCallback<WheelEvent>(this.OnScrollWheel), TrickleDown.NoTrickleDown);
			this.verticalScroller = new CollectionViewScroller();
			this.verticalScroller.AddToClassList(ScrollContainer.verticalScrollerUssClassName);
			this.horizontalScroller = new CollectionViewScroller
			{
				direction = SliderDirection.Horizontal
			};
			this.horizontalScroller.AddToClassList(ScrollContainer.horizontalScrollerUssClassName);
			this.horizontalScroller.RegisterValueChangedCallback<double>(delegate(ChangeEvent<double> evt)
			{
				Vector2 containerOffset = this.containerOffset;
				containerOffset.x = (float)evt.newValue;
				this.containerOffset = containerOffset;
			});
			this.m_Viewport.Add(this.m_Container);
			VisualElement visualElement = new VisualElement();
			visualElement.AddToClassList(ScrollContainer.contentAndHorizontalScrollUssClassName);
			visualElement.Add(this.m_Viewport);
			visualElement.Add(this.horizontalScroller);
			base.hierarchy.Add(visualElement);
			base.hierarchy.Add(this.verticalScroller);
		}

		private void OnScrollWheel(WheelEvent evt)
		{
			this.verticalScroller.value += (double)(evt.delta.y * ((this.verticalScroller.lowValue < this.verticalScroller.highValue) ? 1f : (-1f)) * 18f);
		}

		private const float k_MouseScrollFactor = 18f;

		private VisualElement m_Container;

		private VisualElement m_Viewport;

		private CollectionViewScroller m_VerticalScroller;

		private CollectionViewScroller m_HorizontalScroller;

		private Vector2 m_ContainerOffset;

		public static readonly string ussClassName = "unity-collection-view-scroll-view";

		public static readonly string containerUssClassName = ScrollContainer.ussClassName + "__content-container";

		public static readonly string verticalScrollerUssClassName = ScrollContainer.ussClassName + "__vertical-scroller";

		public static readonly string horizontalScrollerUssClassName = ScrollContainer.ussClassName + "__horizontal-scroller";

		public static readonly string contentAndHorizontalScrollUssClassName = ScrollContainer.ussClassName + "__content-and-horizontal-scroll-container";

		public static readonly string contentViewportUssClassName = ScrollContainer.ussClassName + "__content-viewport";
	}
}
