using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class TwoPaneSplitView : VisualElement
	{
		public VisualElement fixedPane
		{
			get
			{
				return this.m_FixedPane;
			}
		}

		public VisualElement flexedPane
		{
			get
			{
				return this.m_FlexedPane;
			}
		}

		internal VisualElement dragLine
		{
			get
			{
				return this.m_DragLine;
			}
		}

		[CreateProperty]
		public int fixedPaneIndex
		{
			get
			{
				return this.m_FixedPaneIndex;
			}
			set
			{
				bool flag = value == this.m_FixedPaneIndex;
				if (!flag)
				{
					this.Init(value, this.m_FixedPaneInitialDimension, this.m_Orientation);
					base.NotifyPropertyChanged(in TwoPaneSplitView.fixedPaneIndexProperty);
				}
			}
		}

		[CreateProperty]
		public float fixedPaneInitialDimension
		{
			get
			{
				return this.m_FixedPaneInitialDimension;
			}
			set
			{
				bool flag = value == this.m_FixedPaneInitialDimension;
				if (!flag)
				{
					this.Init(this.m_FixedPaneIndex, value, this.m_Orientation);
					base.NotifyPropertyChanged(in TwoPaneSplitView.fixedPaneInitialDimensionProperty);
				}
			}
		}

		[CreateProperty]
		public TwoPaneSplitViewOrientation orientation
		{
			get
			{
				return this.m_Orientation;
			}
			set
			{
				bool flag = value == this.m_Orientation;
				if (!flag)
				{
					this.Init(this.m_FixedPaneIndex, this.m_FixedPaneInitialDimension, value);
					base.NotifyPropertyChanged(in TwoPaneSplitView.orientationProperty);
				}
			}
		}

		internal float fixedPaneDimension
		{
			get
			{
				return string.IsNullOrEmpty(base.viewDataKey) ? this.m_FixedPaneInitialDimension : this.m_FixedPaneDimension;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			set
			{
				bool flag = value == this.m_FixedPaneDimension;
				if (!flag)
				{
					this.m_FixedPaneDimension = value;
					base.SaveViewData();
				}
			}
		}

		public TwoPaneSplitView()
		{
			this.SetupSplitView();
			this.Init(this.m_FixedPaneIndex, this.m_FixedPaneInitialDimension, this.m_Orientation);
		}

		public TwoPaneSplitView(int fixedPaneIndex, float fixedPaneStartDimension, TwoPaneSplitViewOrientation orientation)
		{
			this.SetupSplitView();
			this.Init(fixedPaneIndex, fixedPaneStartDimension, orientation);
		}

		private void SetupSplitView()
		{
			base.AddToClassList(TwoPaneSplitView.s_UssClassName);
			this.m_Content = new VisualElement();
			this.m_Content.name = "unity-content-container";
			this.m_Content.AddToClassList(TwoPaneSplitView.s_ContentContainerClassName);
			base.hierarchy.Add(this.m_Content);
			this.m_DragLineAnchor = new VisualElement();
			this.m_DragLineAnchor.name = "unity-dragline-anchor";
			this.m_DragLineAnchor.AddToClassList(TwoPaneSplitView.s_HandleDragLineAnchorClassName);
			base.hierarchy.Add(this.m_DragLineAnchor);
			this.m_DragLine = new VisualElement();
			this.m_DragLine.name = "unity-dragline";
			this.m_DragLine.AddToClassList(TwoPaneSplitView.s_HandleDragLineClassName);
			this.m_DragLineAnchor.Add(this.m_DragLine);
		}

		public void CollapseChild(int index)
		{
			bool flag = index != 0 && index != 1;
			if (flag)
			{
				Debug.LogError("Invalid index. Must be 0 or 1.");
			}
			else
			{
				bool flag2 = this.m_LeftPane == null;
				if (flag2)
				{
					this.m_PendingCollapseToExecute = true;
					this.m_CollapsedChildIndex = index;
				}
				else
				{
					this.m_DragLine.style.display = DisplayStyle.None;
					this.m_DragLineAnchor.style.display = DisplayStyle.None;
					bool flag3 = index == 0;
					if (flag3)
					{
						this.m_RightPane.style.width = StyleKeyword.Initial;
						this.m_RightPane.style.height = StyleKeyword.Initial;
						this.m_RightPane.style.flexGrow = 1f;
						this.m_LeftPane.style.display = DisplayStyle.None;
					}
					else
					{
						this.m_LeftPane.style.width = StyleKeyword.Initial;
						this.m_LeftPane.style.height = StyleKeyword.Initial;
						this.m_LeftPane.style.flexGrow = 1f;
						this.m_RightPane.style.display = DisplayStyle.None;
					}
					this.m_CollapseMode = true;
					this.AdjustPanesBasedOnAnchor();
				}
			}
		}

		public void UnCollapse()
		{
			bool flag = this.m_LeftPane == null;
			if (!flag)
			{
				VisualElement visualElement = null;
				bool flag2 = this.m_LeftPane.style.display == DisplayStyle.None;
				if (flag2)
				{
					visualElement = this.m_LeftPane;
				}
				else
				{
					bool flag3 = this.m_RightPane.style.display == DisplayStyle.None;
					if (flag3)
					{
						visualElement = this.m_RightPane;
					}
				}
				bool flag4 = visualElement == null;
				if (!flag4)
				{
					this.m_LeftPane.style.display = DisplayStyle.Flex;
					this.m_RightPane.style.display = DisplayStyle.Flex;
					this.m_DragLine.style.display = DisplayStyle.Flex;
					this.m_DragLineAnchor.style.display = DisplayStyle.Flex;
					this.m_LeftPane.style.flexGrow = 0f;
					this.m_RightPane.style.flexGrow = 0f;
					this.m_CollapseMode = false;
					this.m_PendingCollapseToExecute = false;
					this.m_CollapsedChildIndex = -1;
					this.Init(this.m_FixedPaneIndex, this.m_FixedPaneInitialDimension, this.m_Orientation);
					this.AdjustPanesBasedOnAnchor();
					visualElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnUncollapsedPaneResized), TrickleDown.NoTrickleDown);
				}
			}
		}

		private void OnUncollapsedPaneResized(GeometryChangedEvent evt)
		{
			this.UpdateLayout(true, true);
			evt.elementTarget.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnUncollapsedPaneResized), TrickleDown.NoTrickleDown);
		}

		internal virtual void Init(int fixedPaneIndex, float fixedPaneInitialDimension, TwoPaneSplitViewOrientation orientation)
		{
			this.m_Orientation = orientation;
			this.m_FixedPaneIndex = fixedPaneIndex;
			this.m_FixedPaneInitialDimension = fixedPaneInitialDimension;
			this.m_Content.RemoveFromClassList(TwoPaneSplitView.s_HorizontalClassName);
			this.m_Content.RemoveFromClassList(TwoPaneSplitView.s_VerticalClassName);
			bool flag = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
			if (flag)
			{
				this.m_Content.AddToClassList(TwoPaneSplitView.s_HorizontalClassName);
			}
			else
			{
				this.m_Content.AddToClassList(TwoPaneSplitView.s_VerticalClassName);
			}
			this.m_DragLineAnchor.RemoveFromClassList(TwoPaneSplitView.s_HandleDragLineAnchorHorizontalClassName);
			this.m_DragLineAnchor.RemoveFromClassList(TwoPaneSplitView.s_HandleDragLineAnchorVerticalClassName);
			bool flag2 = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
			if (flag2)
			{
				this.m_DragLineAnchor.AddToClassList(TwoPaneSplitView.s_HandleDragLineAnchorHorizontalClassName);
			}
			else
			{
				this.m_DragLineAnchor.AddToClassList(TwoPaneSplitView.s_HandleDragLineAnchorVerticalClassName);
			}
			this.m_DragLine.RemoveFromClassList(TwoPaneSplitView.s_HandleDragLineHorizontalClassName);
			this.m_DragLine.RemoveFromClassList(TwoPaneSplitView.s_HandleDragLineVerticalClassName);
			bool flag3 = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
			if (flag3)
			{
				this.m_DragLine.AddToClassList(TwoPaneSplitView.s_HandleDragLineHorizontalClassName);
			}
			else
			{
				this.m_DragLine.AddToClassList(TwoPaneSplitView.s_HandleDragLineVerticalClassName);
			}
			bool flag4 = this.m_Resizer != null;
			if (flag4)
			{
				this.m_DragLineAnchor.RemoveManipulator(this.m_Resizer);
				this.m_Resizer = null;
			}
			bool flag5 = this.m_Content.childCount != 2;
			if (flag5)
			{
				base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnPostDisplaySetup), TrickleDown.NoTrickleDown);
			}
			else
			{
				this.PostDisplaySetup();
			}
		}

		private void OnPostDisplaySetup(GeometryChangedEvent evt)
		{
			bool flag = this.m_Content.childCount != 2;
			if (flag)
			{
				Debug.LogError("TwoPaneSplitView needs exactly 2 children.");
			}
			else
			{
				bool flag2 = this.m_LeftPane == null;
				this.PostDisplaySetup();
				bool flag3 = flag2 && this.m_PendingCollapseToExecute;
				if (flag3)
				{
					this.CollapseChild(this.m_CollapsedChildIndex);
					this.m_PendingCollapseToExecute = false;
				}
				base.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnPostDisplaySetup), TrickleDown.NoTrickleDown);
				this.AdjustPanesBasedOnAnchor();
			}
		}

		private void AdjustPanesBasedOnAnchor()
		{
			bool flag = this.m_LeftPane.style.display == DisplayStyle.None || this.m_RightPane.style.display == DisplayStyle.None;
			if (flag)
			{
				bool flag2 = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
				if (flag2)
				{
					this.m_RightPane.style.left = 0f;
					this.m_Content.style.paddingRight = 0f;
				}
				else
				{
					this.m_RightPane.style.top = 0f;
					this.m_Content.style.paddingBottom = 0f;
				}
			}
			else
			{
				bool flag3 = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
				if (flag3)
				{
					this.m_RightPane.style.left = this.m_DragLineAnchor.layout.width;
					this.m_Content.style.paddingRight = this.m_DragLineAnchor.layout.width;
				}
				else
				{
					this.m_RightPane.style.top = this.m_DragLineAnchor.layout.height;
					this.m_Content.style.paddingBottom = this.m_DragLineAnchor.layout.height;
				}
			}
		}

		private void IdentifyLeftAndRightPane()
		{
			this.m_LeftPane = this.m_Content[0];
			bool flag = this.m_FixedPaneIndex == 0;
			if (flag)
			{
				this.m_FixedPane = this.m_LeftPane;
			}
			else
			{
				this.m_FlexedPane = this.m_LeftPane;
			}
			this.m_RightPane = this.m_Content[1];
			bool flag2 = this.m_FixedPaneIndex == 1;
			if (flag2)
			{
				this.m_FixedPane = this.m_RightPane;
			}
			else
			{
				this.m_FlexedPane = this.m_RightPane;
			}
		}

		private void PostDisplaySetup()
		{
			bool flag = this.m_Content.childCount != 2;
			if (flag)
			{
				Debug.LogError("TwoPaneSplitView needs exactly 2 children.");
			}
			else
			{
				bool flag2 = this.fixedPaneDimension < 0f;
				if (flag2)
				{
					this.fixedPaneDimension = this.m_FixedPaneInitialDimension;
				}
				float fixedPaneDimension = this.fixedPaneDimension;
				this.IdentifyLeftAndRightPane();
				this.m_FixedPane.style.flexBasis = StyleKeyword.Null;
				this.m_FixedPane.style.flexShrink = StyleKeyword.Null;
				this.m_FixedPane.style.flexGrow = StyleKeyword.Null;
				this.m_FlexedPane.style.flexGrow = StyleKeyword.Null;
				this.m_FlexedPane.style.flexShrink = StyleKeyword.Null;
				this.m_FlexedPane.style.flexBasis = StyleKeyword.Null;
				this.m_FixedPane.style.width = StyleKeyword.Null;
				this.m_FixedPane.style.height = StyleKeyword.Null;
				this.m_FlexedPane.style.width = StyleKeyword.Null;
				this.m_FlexedPane.style.height = StyleKeyword.Null;
				bool flag3 = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
				if (flag3)
				{
					this.m_FixedPane.style.width = fixedPaneDimension;
					this.m_FixedPane.style.height = StyleKeyword.Null;
				}
				else
				{
					this.m_FixedPane.style.width = StyleKeyword.Null;
					this.m_FixedPane.style.height = fixedPaneDimension;
				}
				this.m_FixedPane.style.flexShrink = 0f;
				this.m_FixedPane.style.flexGrow = 0f;
				this.m_FlexedPane.style.flexGrow = 1f;
				this.m_FlexedPane.style.flexShrink = 0f;
				this.m_FlexedPane.style.flexBasis = 0f;
				this.m_DragLineAnchor.style.left = 0f;
				this.m_DragLineAnchor.style.top = 0f;
				bool flag4 = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
				if (flag4)
				{
					float num = this.m_FixedPane.resolvedStyle.marginLeft + this.m_FixedPane.resolvedStyle.marginRight;
					bool flag5 = this.m_FixedPaneIndex == 0;
					if (flag5)
					{
						this.m_DragLineAnchor.style.left = num + fixedPaneDimension;
					}
					else
					{
						this.m_DragLineAnchor.style.left = base.resolvedStyle.width - num - fixedPaneDimension - this.m_DragLineAnchor.resolvedStyle.width;
					}
				}
				else
				{
					float num2 = this.m_FixedPane.resolvedStyle.marginTop + this.m_FixedPane.resolvedStyle.marginBottom;
					bool flag6 = this.m_FixedPaneIndex == 0;
					if (flag6)
					{
						this.m_DragLineAnchor.style.top = num2 + fixedPaneDimension;
					}
					else
					{
						this.m_DragLineAnchor.style.top = base.resolvedStyle.height - num2 - fixedPaneDimension - this.m_DragLineAnchor.resolvedStyle.height;
					}
				}
				bool flag7 = this.m_FixedPaneIndex == 0;
				int num3;
				if (flag7)
				{
					num3 = 1;
				}
				else
				{
					num3 = -1;
				}
				bool flag8 = this.m_Resizer != null;
				if (flag8)
				{
					this.m_DragLineAnchor.RemoveManipulator(this.m_Resizer);
				}
				this.m_Resizer = new TwoPaneSplitViewResizer(this, num3);
				this.m_DragLineAnchor.AddManipulator(this.m_Resizer);
				base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChange), TrickleDown.NoTrickleDown);
			}
		}

		private void OnSizeChange(GeometryChangedEvent evt)
		{
			this.UpdateLayout(true, true);
		}

		private void UpdateDragLineAnchorOffset()
		{
			this.UpdateLayout(false, true);
		}

		private void UpdateLayout(bool updateFixedPane, bool updateDragLine)
		{
			bool collapseMode = this.m_CollapseMode;
			if (!collapseMode)
			{
				bool flag = base.resolvedStyle.display == DisplayStyle.None || base.resolvedStyle.visibility == Visibility.Hidden;
				if (!flag)
				{
					float num = base.resolvedStyle.width;
					float num2 = this.m_FixedPane.resolvedStyle.width;
					float num3 = this.m_FixedPane.resolvedStyle.marginLeft + this.m_FixedPane.resolvedStyle.marginRight;
					float num4 = this.m_FixedPane.resolvedStyle.minWidth.value;
					float num5 = this.m_FlexedPane.resolvedStyle.marginLeft + this.m_FlexedPane.resolvedStyle.marginRight;
					float num6 = this.m_FlexedPane.resolvedStyle.minWidth.value;
					bool flag2 = this.m_Orientation == TwoPaneSplitViewOrientation.Vertical;
					if (flag2)
					{
						num = base.resolvedStyle.height;
						num2 = this.m_FixedPane.resolvedStyle.height;
						num3 = this.m_FixedPane.resolvedStyle.marginTop + this.m_FixedPane.resolvedStyle.marginBottom;
						num4 = this.m_FixedPane.resolvedStyle.minHeight.value;
						num5 = this.m_FlexedPane.resolvedStyle.marginTop + this.m_FlexedPane.resolvedStyle.marginBottom;
						num6 = this.m_FlexedPane.resolvedStyle.minHeight.value;
					}
					bool flag3 = num >= num2 + num3 + num6 + num5;
					if (flag3)
					{
						if (updateDragLine)
						{
							this.SetDragLineOffset((this.m_FixedPaneIndex == 0) ? (num2 + num3) : (num - num2 - num3));
						}
					}
					else
					{
						bool flag4 = num >= num4 + num3 + num6 + num5;
						if (flag4)
						{
							float num7 = num - num6 - num5 - num3;
							float num8 = ((this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal) ? this.m_DragLineAnchor.layout.width : this.m_DragLineAnchor.layout.height);
							num7 -= num8;
							bool flag5 = num7 < num4;
							bool flag6 = num2 > num4;
							if (updateFixedPane)
							{
								bool flag7 = !flag5;
								if (flag7)
								{
									this.SetFixedPaneDimension(num7);
								}
								else
								{
									bool flag8 = flag6;
									if (flag8)
									{
										this.SetFixedPaneDimension(num4);
									}
								}
							}
							if (updateDragLine)
							{
								bool flag9 = flag5;
								if (flag9)
								{
									this.SetDragLineOffset((this.m_FixedPaneIndex == 0) ? num4 : (num - num4 - num3));
								}
								else
								{
									this.SetDragLineOffset((this.m_FixedPaneIndex == 0) ? (num7 + num3) : (num6 + num5));
								}
							}
						}
						else
						{
							if (updateFixedPane)
							{
								this.SetFixedPaneDimension(num4);
							}
							if (updateDragLine)
							{
								this.SetDragLineOffset((this.m_FixedPaneIndex == 0) ? (num4 + num3) : (num6 + num5));
							}
						}
					}
				}
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_Content;
			}
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			this.PostDisplaySetup();
		}

		private void SetDragLineOffset(float offset)
		{
			bool flag = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
			if (flag)
			{
				this.m_DragLineAnchor.style.left = offset;
			}
			else
			{
				this.m_DragLineAnchor.style.top = offset;
			}
		}

		private void SetFixedPaneDimension(float dimension)
		{
			bool flag = this.m_Orientation == TwoPaneSplitViewOrientation.Horizontal;
			if (flag)
			{
				this.m_FixedPane.style.width = dimension;
			}
			else
			{
				this.m_FixedPane.style.height = dimension;
			}
		}

		internal static readonly BindingId fixedPaneIndexProperty = "fixedPaneIndex";

		internal static readonly BindingId fixedPaneInitialDimensionProperty = "fixedPaneInitialDimension";

		internal static readonly BindingId orientationProperty = "orientation";

		private const float k_FixedPaneInitialDimension = 100f;

		private static readonly string s_UssClassName = "unity-two-pane-split-view";

		private static readonly string s_ContentContainerClassName = "unity-two-pane-split-view__content-container";

		private static readonly string s_HandleDragLineClassName = "unity-two-pane-split-view__dragline";

		private static readonly string s_HandleDragLineVerticalClassName = TwoPaneSplitView.s_HandleDragLineClassName + "--vertical";

		private static readonly string s_HandleDragLineHorizontalClassName = TwoPaneSplitView.s_HandleDragLineClassName + "--horizontal";

		private static readonly string s_HandleDragLineAnchorClassName = "unity-two-pane-split-view__dragline-anchor";

		private static readonly string s_HandleDragLineAnchorVerticalClassName = TwoPaneSplitView.s_HandleDragLineAnchorClassName + "--vertical";

		private static readonly string s_HandleDragLineAnchorHorizontalClassName = TwoPaneSplitView.s_HandleDragLineAnchorClassName + "--horizontal";

		private static readonly string s_VerticalClassName = "unity-two-pane-split-view--vertical";

		private static readonly string s_HorizontalClassName = "unity-two-pane-split-view--horizontal";

		private VisualElement m_LeftPane;

		private VisualElement m_RightPane;

		private VisualElement m_FixedPane;

		private VisualElement m_FlexedPane;

		[SerializeField]
		[DontCreateProperty]
		private float m_FixedPaneDimension = -1f;

		private VisualElement m_DragLine;

		private VisualElement m_DragLineAnchor;

		private bool m_CollapseMode;

		private bool m_PendingCollapseToExecute;

		private int m_CollapsedChildIndex = -1;

		private VisualElement m_Content;

		private TwoPaneSplitViewOrientation m_Orientation;

		private int m_FixedPaneIndex;

		private float m_FixedPaneInitialDimension = 100f;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal TwoPaneSplitViewResizer m_Resizer;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(TwoPaneSplitView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("fixedPaneIndex", "fixed-pane-index", null, Array.Empty<string>()),
					new UxmlAttributeNames("fixedPaneInitialDimension", "fixed-pane-initial-dimension", null, Array.Empty<string>()),
					new UxmlAttributeNames("orientation", "orientation", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new TwoPaneSplitView();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.fixedPaneIndex_UxmlAttributeFlags) || UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.fixedPaneInitialDimension_UxmlAttributeFlags) || UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.orientation_UxmlAttributeFlags);
				if (flag)
				{
					TwoPaneSplitView twoPaneSplitView = (TwoPaneSplitView)obj;
					twoPaneSplitView.Init(this.fixedPaneIndex, this.fixedPaneInitialDimension, this.orientation);
				}
			}

			[SerializeField]
			private int fixedPaneIndex;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags fixedPaneIndex_UxmlAttributeFlags;

			[SerializeField]
			private float fixedPaneInitialDimension;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags fixedPaneInitialDimension_UxmlAttributeFlags;

			[SerializeField]
			private TwoPaneSplitViewOrientation orientation;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags orientation_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<TwoPaneSplitView, TwoPaneSplitView.UxmlTraits>
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
				int valueFromBag = this.m_FixedPaneIndex.GetValueFromBag(bag, cc);
				int valueFromBag2 = this.m_FixedPaneInitialDimension.GetValueFromBag(bag, cc);
				TwoPaneSplitViewOrientation valueFromBag3 = this.m_Orientation.GetValueFromBag(bag, cc);
				((TwoPaneSplitView)ve).Init(valueFromBag, (float)valueFromBag2, valueFromBag3);
			}

			private UxmlIntAttributeDescription m_FixedPaneIndex = new UxmlIntAttributeDescription
			{
				name = "fixed-pane-index",
				defaultValue = 0
			};

			private UxmlIntAttributeDescription m_FixedPaneInitialDimension = new UxmlIntAttributeDescription
			{
				name = "fixed-pane-initial-dimension",
				defaultValue = 100
			};

			private UxmlEnumAttributeDescription<TwoPaneSplitViewOrientation> m_Orientation = new UxmlEnumAttributeDescription<TwoPaneSplitViewOrientation>
			{
				name = "orientation",
				defaultValue = TwoPaneSplitViewOrientation.Horizontal
			};
		}
	}
}
