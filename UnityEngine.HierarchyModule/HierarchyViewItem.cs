using System;
using System.Diagnostics;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule", "UnityEditor.UIToolkitAuthoringModule" })]
	internal sealed class HierarchyViewItem : VisualElement
	{
		internal VisualElement LeftContainer
		{
			get
			{
				return this.m_LeftContainer;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event HierarchyViewItem.ExpandedStateChangedEventHandler ExpandedStateChanged;

		public unsafe HierarchyNodeType NodeType
		{
			get
			{
				HierarchyNodeTypeHandler handler = this.m_Handler;
				return (handler != null) ? handler.GetNodeType() : (*HierarchyNodeType.Null);
			}
		}

		public readonly ref HierarchyNode Node
		{
			get
			{
				return ref this.m_Node;
			}
		}

		public Label Name
		{
			get
			{
				return this.m_Name.Label;
			}
		}

		public VisualElement Icon
		{
			get
			{
				return this.m_Icon;
			}
		}

		public VisualElement OverlayIcon
		{
			get
			{
				return this.m_OverlayIcon;
			}
		}

		public VisualElement LeftCustomContainer
		{
			get
			{
				return this.m_LeftCustomContainer;
			}
		}

		public VisualElement RightCustomContainer
		{
			get
			{
				return this.m_RightCustomContainer;
			}
		}

		public Button NavigateIntoButton
		{
			get
			{
				return this.m_NavigateIntoButton.Value;
			}
		}

		public VisualElement OverrideBarContainer
		{
			get
			{
				return this.m_OverrideBarContainer;
			}
		}

		public Toggle Toggle
		{
			get
			{
				return this.m_Toggle;
			}
		}

		public VisualElement RowContainer
		{
			get
			{
				VisualElement visualElement = base.parent;
				while (visualElement != null && !visualElement.ClassListContains("unity-multi-column-view__row-container"))
				{
					visualElement = visualElement.parent;
				}
				return visualElement;
			}
		}

		public HierarchyNodeTypeHandler Handler
		{
			get
			{
				return this.m_Handler;
			}
		}

		public HierarchyView View
		{
			get
			{
				return this.m_View;
			}
		}

		internal bool Bound
		{
			get
			{
				return (in this.m_Node) != HierarchyNode.Null || this.m_View != null;
			}
		}

		internal HierarchyViewItem()
		{
			base.name = "unity-tree-view__item";
			base.style.flexDirection = FlexDirection.Row;
			VisualElement visualElement = new VisualElement();
			visualElement.AddToClassList("hierarchy-item__container");
			base.hierarchy.Add(visualElement);
			this.m_LeftContainer = new VisualElement();
			this.m_LeftContainer.AddToClassList("hierarchy-item__left-container");
			this.m_OverrideBarContainer = new VisualElement();
			this.m_OverrideBarContainer.AddToClassList("hierarchy-item__override-bar-container");
			this.m_Toggle = new Toggle();
			this.m_Toggle.AddToClassList("unity-tree-view__item-toggle");
			this.m_Toggle.AddToClassList(Foldout.toggleUssClassName);
			this.m_Toggle.Q(null, "unity-toggle__checkmark").style.marginTop = 0f;
			this.m_Toggle.focusable = false;
			this.m_Icon = new VisualElement();
			this.m_Icon.AddToClassList("hierarchy-item__icon");
			this.m_OverlayIcon = new VisualElement();
			this.m_OverlayIcon.AddToClassList("hierarchy-item__overlay-icon");
			this.m_Name = new HierarchyViewItemName();
			this.m_Name.AddToClassList("hierarchy-item__name");
			this.m_LeftCustomContainer = new VisualElement();
			this.m_LeftCustomContainer.AddToClassList("hierarchy-item__left-custom-section");
			this.m_LeftContainer.Add(this.m_OverrideBarContainer);
			this.m_LeftContainer.Add(this.m_Toggle);
			this.m_LeftContainer.Add(this.m_Icon);
			this.m_LeftContainer.Add(this.m_OverlayIcon);
			this.m_LeftContainer.Add(this.m_Name);
			this.m_LeftContainer.Add(this.m_LeftCustomContainer);
			this.m_RightCustomContainer = new VisualElement();
			this.m_RightCustomContainer.AddToClassList("hierarchy-item__right-container");
			this.m_NavigateIntoButton = new Lazy<Button>(delegate
			{
				Button button = new Button();
				button.AddToClassList("hierarchy-item__right-arrow-button");
				button.RemoveFromClassList(Button.ussClassName);
				button.style.display = DisplayStyle.None;
				this.m_RightCustomContainer.Add(button);
				return button;
			});
			visualElement.Add(this.m_OverrideBarContainer);
			visualElement.Add(this.m_LeftContainer);
			visualElement.Add(this.m_RightCustomContainer);
			base.AddToClassList("unity-tree-view__item");
			base.AddToClassList("unity-list-view__item");
		}

		internal void Bind(in HierarchyNode node, HierarchyView view)
		{
			bool bound = this.Bound;
			if (bound)
			{
				throw new InvalidOperationException("Cannot bind a hierarchy view item that is already bound.");
			}
			this.m_Node = node;
			this.m_Handler = view.Source.GetNodeTypeHandler(in node);
			this.m_View = view;
			HierarchyViewModel viewModel = this.m_View.ViewModel;
			HierarchyNode root = viewModel.GetRoot();
			int depth = viewModel.GetDepth(in this.m_Node);
			int num = (((in root) == this.m_View.Source.Root) ? depth : (depth - viewModel.GetDepth(in root) - 1));
			bool flag = !this.m_View.Filtering;
			int num2 = (flag ? (num * 14) : 0);
			Translate value = this.m_LeftContainer.style.translate.value;
			this.m_LeftContainer.style.translate = new Translate(this.m_LeftContainer.CeilToPanelPixelSize((float)num2), value.y, value.z);
			bool flag2 = flag && viewModel.GetChildrenCount(in this.m_Node) > 0;
			this.m_Toggle.EnableInClassList("hierarchy-item__toggle--hidden", !flag2);
			bool flag3 = viewModel.HasAllFlags(in this.m_Node, HierarchyNodeFlags.Expanded);
			this.m_Toggle.SetValueWithoutNotify(flag2 && flag3);
			this.Icon.EnableInClassList("hierarchy-item__icon--cut", viewModel.HasAllFlags(in this.m_Node, HierarchyNodeFlags.Cut));
			IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = this.m_Handler as IHierarchyEditorNodeTypeHandler;
			bool flag4 = hierarchyEditorNodeTypeHandler != null;
			if (flag4)
			{
				this.m_Name.Text = hierarchyEditorNodeTypeHandler.GetDisplayName(this.m_View, in this.m_Node);
			}
			else
			{
				this.m_Name.Text = this.m_View.Source.GetName(in this.m_Node);
			}
			this.m_View.InvokeBindViewItem(this);
			this.m_Name.OnBeginRename += this.OnBeginRename;
			this.m_Name.OnEndRename += this.OnEndRename;
		}

		internal unsafe void Unbind()
		{
			bool flag = !this.Bound;
			if (!flag)
			{
				bool flag2 = this.RowContainer != null && this.RowContainer.ClassListContains("hierarchy - item__ping-base");
				if (flag2)
				{
					this.RowContainer.SendEvent(new TransitionEndEvent
					{
						target = this.RowContainer
					}, DispatchMode.Immediate);
					this.RowContainer.SendEvent(new TransitionEndEvent
					{
						target = this.RowContainer
					}, DispatchMode.Immediate);
				}
				this.m_Node = *HierarchyNode.Null;
				this.m_View.InvokeUnbindViewItem(this);
				this.m_Name.OnBeginRename -= this.OnBeginRename;
				this.m_Name.OnEndRename -= this.OnEndRename;
				this.m_Handler = null;
				this.m_View = null;
			}
		}

		[EventInterest(new Type[] { typeof(TooltipEvent) })]
		[EventInterest(new Type[] { typeof(ClickEvent) })]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			TooltipEvent tooltipEvent = evt as TooltipEvent;
			bool flag = tooltipEvent != null;
			if (flag)
			{
				bool filtering = this.m_View.Filtering;
				StringBuilder stringBuilder = new StringBuilder();
				this.m_View.InvokeGetTooltip(this, filtering, stringBuilder);
				bool flag2 = stringBuilder.Length == 0;
				if (!flag2)
				{
					tooltipEvent.rect = this.m_Name.worldBound;
					tooltipEvent.tooltip = stringBuilder.ToString();
				}
			}
			else
			{
				ClickEvent clickEvent = evt as ClickEvent;
				bool flag3 = clickEvent != null && this.m_Toggle.visible && this.m_Toggle.worldBound.Contains(clickEvent.position);
				if (flag3)
				{
					bool flag4 = !this.m_View.ViewModel.HasAllFlags(in this.m_Node, HierarchyNodeFlags.Expanded);
					HierarchyViewItem.ExpandedStateChangedEventHandler expandedStateChanged = this.ExpandedStateChanged;
					if (expandedStateChanged != null)
					{
						expandedStateChanged(in this.m_Node, flag4, clickEvent.altKey);
					}
					evt.StopPropagation();
				}
			}
		}

		[EventInterest(new Type[] { typeof(PointerDownEvent) })]
		protected override void HandleEventTrickleDown(EventBase evt)
		{
			PointerDownEvent pointerDownEvent = evt as PointerDownEvent;
			bool flag;
			if (pointerDownEvent != null)
			{
				HierarchyView view = this.m_View;
				if (view != null && view.m_IsRenamingItem)
				{
					flag = !this.m_Toggle.worldBound.Contains(pointerDownEvent.position);
					goto IL_003E;
				}
			}
			flag = true;
			IL_003E:
			bool flag2 = flag;
			if (!flag2)
			{
				pointerDownEvent.StopImmediatePropagation();
			}
		}

		public void BeginRename()
		{
			bool flag = (in this.m_Node) == HierarchyNode.Null;
			if (!flag)
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = this.m_Handler as IHierarchyEditorNodeTypeHandler;
				bool flag2 = hierarchyEditorNodeTypeHandler != null && !hierarchyEditorNodeTypeHandler.CanSetName(this.m_View, in this.m_Node);
				if (!flag2)
				{
					this.m_Name.BeginRename();
				}
			}
		}

		private void OnBeginRename()
		{
			this.m_View.SetRenamingItem(this);
		}

		private void OnEndRename(string text, bool canceled)
		{
			this.m_View.SetRenamingItem(null);
			if (!canceled)
			{
				bool flag = (in this.m_Node) == HierarchyNode.Null || string.IsNullOrEmpty(text);
				if (!flag)
				{
					IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = this.m_Handler as IHierarchyEditorNodeTypeHandler;
					bool flag2 = hierarchyEditorNodeTypeHandler != null;
					if (flag2)
					{
						hierarchyEditorNodeTypeHandler.OnSetName(this.m_View, in this.m_Node, text);
					}
					else
					{
						this.m_View.Source.SetName(in this.m_Node, text);
					}
				}
			}
		}

		private const string k_UnityListViewItem = "unity-list-view__item";

		private const string k_UnityTreeViewItem = "unity-tree-view__item";

		private const string k_UnityTreeViewItemToggle = "unity-tree-view__item-toggle";

		private const string k_UnityToggleCheckmark = "unity-toggle__checkmark";

		private const string k_HierarchyItemContainer = "hierarchy-item__container";

		private const string k_HierarchyItemOverrideBarContainer = "hierarchy-item__override-bar-container";

		private const string k_HierarchyItemIcon = "hierarchy-item__icon";

		private const string k_HierarchyItemIconCut = "hierarchy-item__icon--cut";

		private const string k_HierarchyItemOverlayIcon = "hierarchy-item__overlay-icon";

		private const string k_HierarchyItemName = "hierarchy-item__name";

		private const string k_HierarchyItemLeftContainer = "hierarchy-item__left-container";

		private const string k_HierarchyItemLeftCustomSection = "hierarchy-item__left-custom-section";

		private const string k_HierarchyItemRightContainer = "hierarchy-item__right-container";

		private const string k_HierarchyItemRightArrowButton = "hierarchy-item__right-arrow-button";

		private const string k_HierarchyItemToggleHidden = "hierarchy-item__toggle--hidden";

		internal const int k_IndentWidth = 14;

		private HierarchyNode m_Node;

		private HierarchyNodeTypeHandler m_Handler;

		private HierarchyView m_View;

		private readonly Toggle m_Toggle;

		private readonly VisualElement m_OverrideBarContainer;

		private readonly VisualElement m_Icon;

		private readonly VisualElement m_OverlayIcon;

		private readonly HierarchyViewItemName m_Name;

		private readonly Lazy<Button> m_NavigateIntoButton;

		private readonly VisualElement m_LeftCustomContainer;

		private readonly VisualElement m_RightCustomContainer;

		private readonly VisualElement m_LeftContainer;

		internal delegate void ExpandedStateChangedEventHandler(in HierarchyNode node, bool isExpanded, bool recursive);
	}
}
