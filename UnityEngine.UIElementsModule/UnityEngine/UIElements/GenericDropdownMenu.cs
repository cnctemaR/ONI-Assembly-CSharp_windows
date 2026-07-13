using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	public class GenericDropdownMenu : AbstractGenericMenu
	{
		internal List<GenericDropdownMenu.MenuItem> items
		{
			get
			{
				return this.m_Items;
			}
		}

		internal VisualElement menuContainer
		{
			get
			{
				return this.m_MenuContainer;
			}
		}

		internal VisualElement outerContainer
		{
			get
			{
				return this.m_OuterContainer;
			}
		}

		internal ScrollView scrollView
		{
			get
			{
				return this.m_ScrollView;
			}
		}

		internal bool isSingleSelectionDropdown { get; set; }

		internal bool closeOnParentResize { get; set; }

		public VisualElement contentContainer
		{
			get
			{
				return this.m_ScrollView.contentContainer;
			}
		}

		public VisualElement targetElement
		{
			get
			{
				return this.m_TargetElement;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action onOpen;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action onClose;

		public GenericDropdownMenu()
		{
			this.m_MenuContainer = new VisualElement();
			this.m_MenuContainer.AddToClassList(GenericDropdownMenu.ussClassName);
			this.m_OuterContainer = new VisualElement();
			this.m_OuterContainer.AddToClassList(GenericDropdownMenu.containerOuterUssClassName);
			this.m_MenuContainer.Add(this.m_OuterContainer);
			this.m_ScrollView = new ScrollView();
			this.m_ScrollView.AddToClassList(GenericDropdownMenu.containerInnerUssClassName);
			this.m_ScrollView.pickingMode = PickingMode.Position;
			this.m_ScrollView.contentContainer.focusable = true;
			this.m_ScrollView.touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;
			this.m_ScrollView.mode = ScrollViewMode.VerticalAndHorizontal;
			this.m_OuterContainer.hierarchy.Add(this.m_ScrollView);
			this.m_MenuContainer.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
			this.m_MenuContainer.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
			this.isSingleSelectionDropdown = true;
			this.closeOnParentResize = true;
		}

		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			bool flag = evt.destinationPanel == null;
			if (!flag)
			{
				this.contentContainer.AddManipulator(this.m_NavigationManipulator = new KeyboardNavigationManipulator(new Action<KeyboardNavigationOperation, EventBase>(this.Apply)));
				this.m_MenuContainer.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				this.m_MenuContainer.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				this.m_MenuContainer.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
				evt.destinationPanel.visualTree.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnParentResized), TrickleDown.NoTrickleDown);
				this.m_ScrollView.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnInitialDisplay), InvokePolicy.Once, TrickleDown.NoTrickleDown);
				this.m_ScrollView.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnContainerGeometryChanged), TrickleDown.NoTrickleDown);
				this.m_ScrollView.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnFocusOut), TrickleDown.NoTrickleDown);
			}
		}

		private void OnDetachFromPanel(DetachFromPanelEvent evt)
		{
			bool flag = evt.originPanel == null;
			if (!flag)
			{
				this.contentContainer.RemoveManipulator(this.m_NavigationManipulator);
				this.m_MenuContainer.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				this.m_MenuContainer.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				this.m_MenuContainer.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
				evt.originPanel.visualTree.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnParentResized), TrickleDown.NoTrickleDown);
				this.m_ScrollView.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnContainerGeometryChanged), TrickleDown.NoTrickleDown);
				this.m_ScrollView.UnregisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnFocusOut), TrickleDown.NoTrickleDown);
			}
		}

		private void Hide(bool giveFocusBack = false)
		{
			this.m_MenuContainer.RemoveFromHierarchy();
			bool flag = this.m_TargetElement != null;
			if (flag)
			{
				this.m_TargetElement.UnregisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnTargetElementDetachFromPanel), TrickleDown.NoTrickleDown);
				this.m_TargetElement.pseudoStates ^= PseudoStates.Active;
				bool flag2 = giveFocusBack && this.m_TargetElement.canGrabFocus;
				if (flag2)
				{
					this.m_TargetElement.Focus();
				}
			}
			Action action = this.onClose;
			if (action != null)
			{
				action();
			}
			this.contentContainer.userData = null;
			this.m_TargetElement = null;
		}

		private void Apply(KeyboardNavigationOperation op, EventBase sourceEvent)
		{
			bool flag = this.Apply(op);
			if (flag)
			{
				sourceEvent.StopPropagation();
			}
		}

		private bool Apply(KeyboardNavigationOperation op)
		{
			GenericDropdownMenu.<>c__DisplayClass56_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.selectedIndex = this.GetSelectedIndex();
			switch (op)
			{
			case KeyboardNavigationOperation.Cancel:
				this.Hide(true);
				return true;
			case KeyboardNavigationOperation.Submit:
			{
				GenericDropdownMenu.MenuItem menuItem = ((CS$<>8__locals1.selectedIndex != -1) ? this.m_Items[CS$<>8__locals1.selectedIndex] : null);
				bool flag = CS$<>8__locals1.selectedIndex >= 0 && menuItem.element.enabledSelf;
				if (flag)
				{
					Action action = menuItem.action;
					if (action != null)
					{
						action();
					}
					Action<object> actionUserData = menuItem.actionUserData;
					if (actionUserData != null)
					{
						actionUserData(menuItem.element.userData);
					}
				}
				this.Hide(true);
				return true;
			}
			case KeyboardNavigationOperation.Previous:
				this.<Apply>g__UpdateSelectionUp|56_1((CS$<>8__locals1.selectedIndex < 0) ? (this.m_Items.Count - 1) : (CS$<>8__locals1.selectedIndex - 1), ref CS$<>8__locals1);
				return true;
			case KeyboardNavigationOperation.Next:
				this.<Apply>g__UpdateSelectionDown|56_0(CS$<>8__locals1.selectedIndex + 1, ref CS$<>8__locals1);
				return true;
			case KeyboardNavigationOperation.PageUp:
			case KeyboardNavigationOperation.Begin:
				this.<Apply>g__UpdateSelectionDown|56_0(0, ref CS$<>8__locals1);
				return true;
			case KeyboardNavigationOperation.PageDown:
			case KeyboardNavigationOperation.End:
				this.<Apply>g__UpdateSelectionUp|56_1(this.m_Items.Count - 1, ref CS$<>8__locals1);
				return true;
			}
			return false;
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			this.m_MousePosition = this.m_ScrollView.WorldToLocal(evt.position);
			this.UpdateSelection(evt.elementTarget);
			bool flag = evt.pointerId != PointerId.mousePointerId;
			if (flag)
			{
				this.m_MenuContainer.panel.PreventCompatibilityMouseEvents(evt.pointerId);
			}
			evt.StopPropagation();
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			this.m_MousePosition = this.m_ScrollView.WorldToLocal(evt.position);
			this.UpdateSelection(evt.elementTarget);
			bool flag = evt.pointerId != PointerId.mousePointerId;
			if (flag)
			{
				this.m_MenuContainer.panel.PreventCompatibilityMouseEvents(evt.pointerId);
			}
			evt.StopPropagation();
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			int selectedIndex = this.GetSelectedIndex();
			bool flag = selectedIndex != -1;
			if (flag)
			{
				GenericDropdownMenu.MenuItem menuItem = this.m_Items[selectedIndex];
				Action action = menuItem.action;
				if (action != null)
				{
					action();
				}
				Action<object> actionUserData = menuItem.actionUserData;
				if (actionUserData != null)
				{
					actionUserData(menuItem.element.userData);
				}
				bool isSingleSelectionDropdown = this.isSingleSelectionDropdown;
				if (isSingleSelectionDropdown)
				{
					this.Hide(true);
				}
			}
			bool flag2 = evt.pointerId != PointerId.mousePointerId;
			if (flag2)
			{
				this.m_MenuContainer.panel.PreventCompatibilityMouseEvents(evt.pointerId);
			}
			evt.StopPropagation();
		}

		private void OnFocusOut(FocusOutEvent evt)
		{
			bool flag = !this.m_ScrollView.ContainsPoint(this.m_MousePosition);
			if (flag)
			{
				this.Hide(false);
			}
			else
			{
				this.m_MenuContainer.schedule.Execute(new Action(this.contentContainer.Focus));
			}
		}

		private void OnParentResized(GeometryChangedEvent evt)
		{
			bool closeOnParentResize = this.closeOnParentResize;
			if (closeOnParentResize)
			{
				this.Hide(true);
			}
		}

		private void UpdateSelection(VisualElement target)
		{
			bool flag = !this.m_ScrollView.ContainsPoint(this.m_MousePosition);
			if (flag)
			{
				int selectedIndex = this.GetSelectedIndex();
				bool flag2 = selectedIndex >= 0;
				if (flag2)
				{
					this.m_Items[selectedIndex].element.pseudoStates &= ~PseudoStates.Hover;
				}
			}
			else
			{
				bool flag3 = target == null;
				if (!flag3)
				{
					bool flag4 = (target.pseudoStates & PseudoStates.Hover) != PseudoStates.Hover;
					if (flag4)
					{
						int selectedIndex2 = this.GetSelectedIndex();
						bool flag5 = selectedIndex2 >= 0;
						if (flag5)
						{
							this.m_Items[selectedIndex2].element.pseudoStates &= ~PseudoStates.Hover;
						}
						target.pseudoStates |= PseudoStates.Hover;
					}
				}
			}
		}

		private void ChangeSelectedIndex(int newIndex, int previousIndex)
		{
			bool flag = previousIndex >= 0 && previousIndex < this.m_Items.Count;
			if (flag)
			{
				this.m_Items[previousIndex].element.pseudoStates &= ~PseudoStates.Hover;
			}
			bool flag2 = newIndex >= 0 && newIndex < this.m_Items.Count;
			if (flag2)
			{
				this.m_Items[newIndex].element.pseudoStates |= PseudoStates.Hover;
				this.m_ScrollView.ScrollTo(this.m_Items[newIndex].element);
			}
		}

		private int GetSelectedIndex()
		{
			for (int i = 0; i < this.m_Items.Count; i++)
			{
				bool flag = (this.m_Items[i].element.pseudoStates & PseudoStates.Hover) == PseudoStates.Hover;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		public override void AddItem(string itemName, bool isChecked, Action action)
		{
			GenericDropdownMenu.MenuItem menuItem = this.AddItem(itemName, isChecked, true, null);
			bool flag = menuItem != null;
			if (flag)
			{
				menuItem.action = action;
			}
		}

		public override void AddItem(string itemName, bool isChecked, Action<object> action, object data)
		{
			GenericDropdownMenu.MenuItem menuItem = this.AddItem(itemName, isChecked, true, data);
			bool flag = menuItem != null;
			if (flag)
			{
				menuItem.actionUserData = action;
			}
		}

		public override void AddDisabledItem(string itemName, bool isChecked)
		{
			this.AddItem(itemName, isChecked, false, null);
		}

		public override void AddSeparator(string path)
		{
			VisualElement visualElement = new VisualElement();
			visualElement.AddToClassList(GenericDropdownMenu.separatorUssClassName);
			visualElement.pickingMode = PickingMode.Ignore;
			this.m_ScrollView.Add(visualElement);
		}

		private GenericDropdownMenu.MenuItem AddItem(string itemName, bool isChecked, bool isEnabled, object data = null)
		{
			bool flag = string.IsNullOrEmpty(itemName) || itemName.EndsWith("/");
			GenericDropdownMenu.MenuItem menuItem;
			if (flag)
			{
				this.AddSeparator(itemName);
				menuItem = null;
			}
			else
			{
				for (int i = 0; i < this.m_Items.Count; i++)
				{
					bool flag2 = itemName == this.m_Items[i].name;
					if (flag2)
					{
						return null;
					}
				}
				VisualElement visualElement = new VisualElement();
				visualElement.AddToClassList(GenericDropdownMenu.itemUssClassName);
				visualElement.SetEnabled(isEnabled);
				visualElement.userData = data;
				VisualElement visualElement2 = new VisualElement
				{
					pickingMode = PickingMode.Ignore
				};
				visualElement2.AddToClassList(GenericDropdownMenu.itemContentUssClassName);
				VisualElement visualElement3 = new VisualElement();
				visualElement3.AddToClassList(GenericDropdownMenu.checkmarkUssClassName);
				visualElement3.pickingMode = PickingMode.Ignore;
				visualElement2.Add(visualElement3);
				if (isChecked)
				{
					visualElement.SetCheckedPseudoState(true);
				}
				Label label = new Label(itemName);
				label.AddToClassList(GenericDropdownMenu.labelUssClassName);
				label.pickingMode = PickingMode.Ignore;
				visualElement2.Add(label);
				visualElement.Add(visualElement2);
				this.m_ScrollView.Add(visualElement);
				GenericDropdownMenu.MenuItem menuItem2 = new GenericDropdownMenu.MenuItem
				{
					name = itemName,
					element = visualElement
				};
				this.m_Items.Add(menuItem2);
				menuItem = menuItem2;
			}
			return menuItem;
		}

		internal void UpdateItem(string itemName, bool isChecked)
		{
			GenericDropdownMenu.MenuItem menuItem = this.m_Items.Find((GenericDropdownMenu.MenuItem x) => x.name == itemName);
			bool flag = menuItem == null;
			if (!flag)
			{
				menuItem.element.SetCheckedPseudoState(isChecked);
			}
		}

		[Obsolete("This version of Dropdown is deprecated. To ensure the dropdown is positioned correctly, please provide a reference to the targetElement.", false)]
		public void DropDown(Rect position)
		{
			this.DropDown(position, null, DropdownMenuSizeMode.Content);
		}

		[Obsolete("This version of Dropdown is deprecated. Please use DropDown(Rect position, VisualElement targetElement, DropdownMenuSizeMode dropdownMenuSizeMode).", false)]
		public void DropDown(Rect position, VisualElement targetElement, bool anchored = false)
		{
			this.DropDown(position, targetElement, anchored ? DropdownMenuSizeMode.Fixed : DropdownMenuSizeMode.Content);
		}

		[Obsolete("This version of Dropdown is deprecated. Please use DropDown(Rect position, VisualElement targetElement, DropdownMenuSizeMode dropdownMenuSizeMode).", false)]
		public void DropDown(Rect position, VisualElement targetElement, bool anchored = false, bool fitContentWidthIfAnchored = false)
		{
			bool flag = anchored && fitContentWidthIfAnchored;
			if (flag)
			{
				this.DropDown(position, targetElement, DropdownMenuSizeMode.Auto);
			}
			else if (anchored)
			{
				this.DropDown(position, targetElement, DropdownMenuSizeMode.Fixed);
			}
			else
			{
				this.DropDown(position, targetElement, DropdownMenuSizeMode.Content);
			}
		}

		public override void DropDown(Rect position, VisualElement targetElement, DropdownMenuSizeMode dropdownMenuSizeMode = DropdownMenuSizeMode.Auto)
		{
			bool flag = false;
			switch (dropdownMenuSizeMode)
			{
			case DropdownMenuSizeMode.Auto:
				this.SetFitContentWidth(true);
				flag = true;
				break;
			case DropdownMenuSizeMode.Fixed:
				this.SetFitContentWidth(false);
				flag = true;
				break;
			case DropdownMenuSizeMode.Content:
				this.SetFitContentWidth(false);
				flag = false;
				break;
			}
			this.DoDropDown(position, targetElement, flag);
		}

		private void DoDropDown(Rect position, VisualElement targetElement, bool anchored)
		{
			bool flag = targetElement == null;
			if (flag)
			{
				Debug.LogError("VisualElement Generic Menu needs a target to find a root to attach to.");
			}
			else
			{
				this.m_TargetElement = targetElement;
				this.m_TargetElement.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnTargetElementDetachFromPanel), TrickleDown.NoTrickleDown);
				Action action = this.onOpen;
				if (action != null)
				{
					action();
				}
				bool flag2 = this.m_TargetElement.panel != null && this.m_TargetElement.panel.contextType == ContextType.Player;
				if (flag2)
				{
					UIDocument uidocument = UIDocument.FindRootUIDocument(this.m_TargetElement);
					bool flag3 = uidocument != null && uidocument.panelSettings != null && uidocument.panelSettings.renderMode == PanelRenderMode.WorldSpace;
					if (flag3)
					{
						this.m_PanelRootVisualContainer = uidocument.rootVisualElement;
					}
					else
					{
						this.m_PanelRootVisualContainer = this.m_TargetElement.GetRootVisualContainer(false);
					}
				}
				else
				{
					this.m_PanelRootVisualContainer = this.m_TargetElement.GetRootVisualContainer(false);
				}
				bool flag4 = this.m_PanelRootVisualContainer == null;
				if (flag4)
				{
					Debug.LogError("Could not find rootVisualContainer...");
				}
				else
				{
					this.m_PanelRootVisualContainer.Add(this.m_MenuContainer);
					this.m_MenuContainer.style.left = this.m_PanelRootVisualContainer.layout.x;
					this.m_MenuContainer.style.top = this.m_PanelRootVisualContainer.layout.y;
					this.m_MenuContainer.style.width = this.m_PanelRootVisualContainer.layout.width;
					this.m_MenuContainer.style.height = this.m_PanelRootVisualContainer.layout.height;
					this.m_MenuContainer.style.fontSize = this.m_TargetElement.computedStyle.fontSize;
					this.m_MenuContainer.style.unityFont = this.m_TargetElement.computedStyle.unityFont;
					this.m_MenuContainer.style.unityFontDefinition = this.m_TargetElement.computedStyle.unityFontDefinition;
					Rect rect = this.m_PanelRootVisualContainer.WorldToLocal(position);
					this.m_PositionTop = rect.y + rect.height - this.m_PanelRootVisualContainer.layout.y;
					this.m_PositionLeft = rect.x - this.m_PanelRootVisualContainer.layout.x;
					this.m_OuterContainer.style.left = this.m_PositionLeft;
					this.m_OuterContainer.style.top = this.m_PositionTop;
					this.m_OuterContainer.style.maxHeight = Length.None();
					this.m_OuterContainer.style.maxWidth = Length.None();
					this.m_DesiredRect = (anchored ? rect : Rect.zero);
					this.m_MenuContainer.schedule.Execute(new Action(this.contentContainer.Focus));
					this.m_ShownAboveTarget = false;
					this.EnsureVisibilityInParent();
					targetElement.SetActivePseudoState(true);
					this.contentContainer.userData = this;
				}
			}
		}

		private void SetFitContentWidth(bool fit)
		{
			this.m_FitContentWidth = fit;
			this.m_OuterContainer.EnableInClassList(GenericDropdownMenu.contentWidthUssClassName, this.m_FitContentWidth);
		}

		private void OnTargetElementDetachFromPanel(DetachFromPanelEvent evt)
		{
			this.Hide(false);
		}

		private void OnContainerGeometryChanged(GeometryChangedEvent evt)
		{
			this.EnsureVisibilityInParent();
		}

		private void OnInitialDisplay(GeometryChangedEvent evt)
		{
			this.m_ContentWidth = this.GetLargestItemWidth() + 20f;
		}

		private void EnsureVisibilityInParent()
		{
			bool flag = this.m_PanelRootVisualContainer != null && !float.IsNaN(this.m_OuterContainer.layout.width) && !float.IsNaN(this.m_OuterContainer.layout.height);
			if (flag)
			{
				bool flag2 = this.m_DesiredRect == Rect.zero;
				if (flag2)
				{
					float num = Math.Max(0f, Mathf.Min(this.m_PositionLeft, this.m_PanelRootVisualContainer.layout.width - this.m_OuterContainer.layout.width));
					float num2 = Mathf.Min(this.m_PositionTop, Mathf.Max(0f, this.m_PanelRootVisualContainer.layout.height - this.m_OuterContainer.layout.height));
					this.m_OuterContainer.style.left = num;
					this.m_OuterContainer.style.top = num2;
				}
				else
				{
					float num3 = this.m_ContentWidth;
					bool isVerticalScrollDisplayed = this.m_ScrollView.isVerticalScrollDisplayed;
					if (isVerticalScrollDisplayed)
					{
						num3 += Mathf.Ceil(this.m_ScrollView.verticalScroller.computedStyle.width.value);
					}
					num3 = (this.m_FitContentWidth ? num3 : this.m_DesiredRect.width);
					this.m_OuterContainer.style.width = num3;
					float num4 = this.m_PanelRootVisualContainer.layout.width - this.m_PositionLeft;
					bool flag3 = num4 <= num3;
					if (flag3)
					{
						this.m_PositionLeft -= num3 - num4 + 2f;
					}
					this.m_PositionLeft = Math.Max(this.m_PositionLeft, 0f);
					bool flag4 = this.m_PositionLeft == 0f;
					if (flag4)
					{
						this.m_OuterContainer.style.maxWidth = Math.Min(this.m_PanelRootVisualContainer.layout.width, num3);
					}
					this.m_OuterContainer.style.left = this.m_PositionLeft;
				}
				Rect rect = this.m_MenuContainer.WorldToLocal(this.m_TargetElement.worldBound);
				float num5 = ((this.m_Items.Count == 0) ? 20f : (this.m_Items[0].element.layout.height + 20f));
				float height = this.m_OuterContainer.layout.height;
				float y = rect.y;
				Vector2 vector = this.m_PanelRootVisualContainer.WorldToLocal(new Vector2(this.m_OuterContainer.worldBound.x, this.m_OuterContainer.worldBound.y));
				float y2 = vector.y;
				float num6 = (this.m_ShownAboveTarget ? (y - y2) : (this.m_PanelRootVisualContainer.layout.height - y2));
				float num7 = (this.m_ShownAboveTarget ? (this.m_PanelRootVisualContainer.layout.height - y2) : y);
				bool flag5 = num6 < height;
				bool flag6 = flag5 && num7 > num6;
				if (flag6)
				{
					this.m_PositionTop = this.m_OuterContainer.RoundToPanelPixelSize(Math.Max(y - height, 0f));
					this.m_OuterContainer.style.maxHeight = ((this.m_PositionTop == 0f) ? Math.Max(y, num5) : Length.None());
					this.m_OuterContainer.style.top = this.m_PositionTop;
					this.m_ShownAboveTarget = true;
				}
				else
				{
					bool flag7 = flag5;
					if (flag7)
					{
						bool flag8 = num6 < num5;
						if (flag8)
						{
							this.m_OuterContainer.style.maxHeight = num5;
							this.m_PositionTop = this.m_PanelRootVisualContainer.worldBound.height - num5;
						}
						else
						{
							this.m_OuterContainer.style.maxHeight = num6;
						}
						this.m_OuterContainer.style.top = this.m_PositionTop;
					}
				}
			}
		}

		private float GetLargestItemWidth()
		{
			float num = 0f;
			bool flag = this.m_Items.Count == 0 && this.m_ScrollView.contentContainer.childCount > 0;
			float num2;
			if (flag)
			{
				List<GenericDropdownMenu.MenuItem> list = CollectionPool<List<GenericDropdownMenu.MenuItem>, GenericDropdownMenu.MenuItem>.Get();
				foreach (VisualElement visualElement in this.m_ScrollView.contentContainer.Children())
				{
					list.Add(new GenericDropdownMenu.MenuItem
					{
						element = visualElement
					});
					num = Math.Max(num, visualElement.layout.width);
				}
				this.m_Items.AddRange(list);
				CollectionPool<List<GenericDropdownMenu.MenuItem>, GenericDropdownMenu.MenuItem>.Release(list);
				num2 = num;
			}
			else
			{
				foreach (GenericDropdownMenu.MenuItem menuItem in this.m_Items)
				{
					VisualElement visualElement2 = menuItem.element.Q(null, new string[] { GenericDropdownMenu.itemContentUssClassName });
					float num3 = ((visualElement2 != null) ? visualElement2.layout.width : menuItem.element.layout.width);
					num = Math.Max(num, num3);
				}
				num2 = num;
			}
			return num2;
		}

		[CompilerGenerated]
		private void <Apply>g__UpdateSelectionDown|56_0(int newIndex, ref GenericDropdownMenu.<>c__DisplayClass56_0 A_2)
		{
			while (newIndex < this.m_Items.Count)
			{
				bool enabledSelf = this.m_Items[newIndex].element.enabledSelf;
				if (enabledSelf)
				{
					this.ChangeSelectedIndex(newIndex, A_2.selectedIndex);
					break;
				}
				newIndex++;
			}
		}

		[CompilerGenerated]
		private void <Apply>g__UpdateSelectionUp|56_1(int newIndex, ref GenericDropdownMenu.<>c__DisplayClass56_0 A_2)
		{
			while (newIndex >= 0)
			{
				bool enabledSelf = this.m_Items[newIndex].element.enabledSelf;
				if (enabledSelf)
				{
					this.ChangeSelectedIndex(newIndex, A_2.selectedIndex);
					break;
				}
				newIndex--;
			}
		}

		public static readonly string ussClassName = "unity-base-dropdown";

		public static readonly string itemUssClassName = GenericDropdownMenu.ussClassName + "__item";

		public static readonly string itemContentUssClassName = GenericDropdownMenu.ussClassName + "__item-content";

		public static readonly string labelUssClassName = GenericDropdownMenu.ussClassName + "__label";

		public static readonly string containerInnerUssClassName = GenericDropdownMenu.ussClassName + "__container-inner";

		public static readonly string containerOuterUssClassName = GenericDropdownMenu.ussClassName + "__container-outer";

		public static readonly string checkmarkUssClassName = GenericDropdownMenu.ussClassName + "__checkmark";

		public static readonly string separatorUssClassName = GenericDropdownMenu.ussClassName + "__separator";

		public static readonly string contentWidthUssClassName = GenericDropdownMenu.ussClassName + "--content-width-menu";

		private const float k_MenuItemPadding = 20f;

		private const float k_MenuPadding = 2f;

		private List<GenericDropdownMenu.MenuItem> m_Items = new List<GenericDropdownMenu.MenuItem>();

		private VisualElement m_MenuContainer;

		private VisualElement m_OuterContainer;

		private ScrollView m_ScrollView;

		private VisualElement m_PanelRootVisualContainer;

		private VisualElement m_TargetElement;

		private Rect m_DesiredRect;

		private KeyboardNavigationManipulator m_NavigationManipulator;

		private float m_PositionTop;

		private float m_PositionLeft;

		private float m_ContentWidth;

		private bool m_FitContentWidth;

		private bool m_ShownAboveTarget;

		private Vector2 m_MousePosition;

		internal class MenuItem
		{
			public string name;

			public VisualElement element;

			public Action action;

			public Action<object> actionUserData;
		}
	}
}
