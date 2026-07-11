using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements
{
	internal class TreeView : VisualElement
	{
		public Func<VisualElement> makeItem
		{
			get
			{
				return this.m_MakeItem;
			}
			set
			{
				bool flag = this.m_MakeItem == value;
				if (!flag)
				{
					this.m_MakeItem = value;
					this.ListViewRefresh();
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ITreeViewItem> onItemChosen;

		public IEnumerable<ITreeViewItem> currentSelection
		{
			get
			{
				bool flag = this.m_CurrentSelection != null;
				IEnumerable<ITreeViewItem> enumerable;
				if (flag)
				{
					enumerable = this.m_CurrentSelection;
				}
				else
				{
					this.m_CurrentSelection = new List<ITreeViewItem>();
					foreach (ITreeViewItem treeViewItem in this.items)
					{
						foreach (int num in this.m_ListView.currentSelectionIds)
						{
							bool flag2 = treeViewItem.id == num;
							if (flag2)
							{
								this.m_CurrentSelection.Add(treeViewItem);
							}
						}
					}
					enumerable = this.m_CurrentSelection;
				}
				return enumerable;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<List<ITreeViewItem>> onSelectionChanged;

		public Action<VisualElement, ITreeViewItem> bindItem
		{
			get
			{
				return this.m_BindItem;
			}
			set
			{
				this.m_BindItem = value;
				this.ListViewRefresh();
			}
		}

		public IList<ITreeViewItem> rootItems
		{
			get
			{
				return this.m_RootItems;
			}
			set
			{
				this.m_RootItems = value;
				this.Refresh();
			}
		}

		public IEnumerable<ITreeViewItem> items
		{
			get
			{
				return TreeView.GetAllItems(this.m_RootItems);
			}
		}

		public int itemHeight
		{
			get
			{
				return this.m_ListView.itemHeight;
			}
			set
			{
				this.m_ListView.itemHeight = value;
			}
		}

		public SelectionType selectionType
		{
			get
			{
				return this.m_ListView.selectionType;
			}
			set
			{
				this.m_ListView.selectionType = value;
			}
		}

		public TreeView()
		{
			this.m_CurrentSelection = null;
			this.m_ExpandedItemIds = new List<int>();
			this.m_ItemWrappers = new List<TreeView.TreeViewItemWrapper>();
			this.m_ListView = new ListView();
			this.m_ListView.name = TreeView.s_ListViewName;
			this.m_ListView.itemsSource = this.m_ItemWrappers;
			this.m_ListView.viewDataKey = TreeView.s_ListViewName;
			this.m_ListView.AddToClassList(TreeView.s_ListViewName);
			base.hierarchy.Add(this.m_ListView);
			this.m_ListView.makeItem = new Func<VisualElement>(this.MakeTreeItem);
			this.m_ListView.bindItem = new Action<VisualElement, int>(this.BindTreeItem);
			this.m_ListView.getItemId = new Func<int, int>(this.GetItemId);
			this.m_ListView.onItemChosen += this.OnItemChosen;
			this.m_ListView.onSelectionChanged += this.OnSelectionChanged;
			this.m_ScrollView = this.m_ListView.Q<ScrollView>(null, null);
			this.m_ScrollView.contentContainer.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
			base.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnTreeViewMouseUp), TrickleDown.TrickleDown);
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
		}

		public TreeView(IList<ITreeViewItem> items, int itemHeight, Func<VisualElement> makeItem, Action<VisualElement, ITreeViewItem> bindItem)
			: this()
		{
			this.m_ListView.itemHeight = itemHeight;
			this.m_MakeItem = makeItem;
			this.m_BindItem = bindItem;
			this.m_RootItems = items;
			this.Refresh();
		}

		public void Refresh()
		{
			this.RegenerateWrappers();
			this.ListViewRefresh();
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			this.Refresh();
		}

		public static IEnumerable<ITreeViewItem> GetAllItems(IEnumerable<ITreeViewItem> rootItems)
		{
			bool flag = rootItems == null;
			if (flag)
			{
				yield break;
			}
			Stack<IEnumerator<ITreeViewItem>> iteratorStack = new Stack<IEnumerator<ITreeViewItem>>();
			IEnumerator<ITreeViewItem> currentIterator = rootItems.GetEnumerator();
			for (;;)
			{
				bool hasNext = currentIterator.MoveNext();
				bool flag2 = !hasNext;
				if (flag2)
				{
					bool flag3 = iteratorStack.Count > 0;
					if (!flag3)
					{
						break;
					}
					currentIterator = iteratorStack.Pop();
				}
				else
				{
					ITreeViewItem currentItem = currentIterator.Current;
					yield return currentItem;
					bool hasChildren = currentItem.hasChildren;
					if (hasChildren)
					{
						iteratorStack.Push(currentIterator);
						currentIterator = currentItem.children.GetEnumerator();
					}
					currentItem = null;
				}
			}
			yield break;
		}

		public void OnKeyDown(KeyDownEvent evt)
		{
			int selectedIndex = this.m_ListView.selectedIndex;
			bool flag = true;
			KeyCode keyCode = evt.keyCode;
			if (keyCode != KeyCode.RightArrow)
			{
				if (keyCode != KeyCode.LeftArrow)
				{
					flag = false;
				}
				else
				{
					bool flag2 = this.IsExpandedByIndex(selectedIndex);
					if (flag2)
					{
						this.CollapseItemByIndex(selectedIndex);
					}
				}
			}
			else
			{
				bool flag3 = !this.IsExpandedByIndex(selectedIndex);
				if (flag3)
				{
					this.ExpandItemByIndex(selectedIndex);
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				evt.StopPropagation();
			}
		}

		public void SelectItem(int id)
		{
			ITreeViewItem treeViewItem = this.FindItem(id);
			bool flag = treeViewItem == null;
			if (flag)
			{
				throw new InvalidOperationException("id");
			}
			for (ITreeViewItem treeViewItem2 = treeViewItem.parent; treeViewItem2 != null; treeViewItem2 = treeViewItem2.parent)
			{
				bool flag2 = !this.m_ExpandedItemIds.Contains(treeViewItem2.id);
				if (flag2)
				{
					this.m_ExpandedItemIds.Add(treeViewItem2.id);
				}
			}
			this.Refresh();
			int i;
			for (i = 0; i < this.m_ItemWrappers.Count; i++)
			{
				bool flag3 = this.m_ItemWrappers[i].id == id;
				if (flag3)
				{
					break;
				}
			}
			this.m_ListView.selectedIndex = i;
			this.m_ListView.ScrollToItem(this.m_ListView.selectedIndex);
		}

		public void ClearSelection()
		{
			this.m_ListView.selectedIndex = -1;
		}

		public bool IsExpanded(int id)
		{
			return this.m_ExpandedItemIds.Contains(id);
		}

		public void CollapseItem(int id)
		{
			for (int i = 0; i < this.m_ItemWrappers.Count; i++)
			{
				bool flag = this.m_ItemWrappers[i].item.id == id;
				if (flag)
				{
					bool flag2 = this.IsExpandedByIndex(i);
					if (flag2)
					{
						this.CollapseItemByIndex(i);
						return;
					}
				}
			}
			bool flag3 = !this.m_ExpandedItemIds.Contains(id);
			if (flag3)
			{
				return;
			}
			this.m_ExpandedItemIds.Remove(id);
			this.Refresh();
		}

		public void ExpandItem(int id)
		{
			for (int i = 0; i < this.m_ItemWrappers.Count; i++)
			{
				bool flag = this.m_ItemWrappers[i].item.id == id;
				if (flag)
				{
					bool flag2 = !this.IsExpandedByIndex(i);
					if (flag2)
					{
						this.ExpandItemByIndex(i);
						return;
					}
				}
			}
			bool flag3 = this.FindItem(id) == null;
			if (flag3)
			{
				throw new InvalidOperationException("TreeView: Item id not found.");
			}
			bool flag4 = this.m_ExpandedItemIds.Contains(id);
			if (flag4)
			{
				return;
			}
			this.m_ExpandedItemIds.Add(id);
			this.Refresh();
		}

		public ITreeViewItem FindItem(int id)
		{
			foreach (ITreeViewItem treeViewItem in this.items)
			{
				bool flag = treeViewItem.id == id;
				if (flag)
				{
					return treeViewItem;
				}
			}
			return null;
		}

		private void ListViewRefresh()
		{
			this.m_ListView.Refresh();
		}

		private void OnItemChosen(object item)
		{
			bool flag = this.onItemChosen != null;
			if (flag)
			{
				TreeView.TreeViewItemWrapper treeViewItemWrapper = (TreeView.TreeViewItemWrapper)item;
				this.onItemChosen(treeViewItemWrapper.item);
			}
		}

		private void OnSelectionChanged(List<object> items)
		{
			bool flag = this.m_CurrentSelection == null;
			if (flag)
			{
				this.m_CurrentSelection = new List<ITreeViewItem>();
			}
			this.m_CurrentSelection.Clear();
			foreach (object obj in items)
			{
				this.m_CurrentSelection.Add(((TreeView.TreeViewItemWrapper)obj).item);
			}
			bool flag2 = this.onSelectionChanged != null;
			if (flag2)
			{
				this.onSelectionChanged(this.m_CurrentSelection);
			}
		}

		private void OnTreeViewMouseUp(MouseUpEvent evt)
		{
			this.m_ScrollView.contentContainer.Focus();
		}

		private void OnItemMouseUp(MouseUpEvent evt)
		{
			bool flag = (evt.modifiers & EventModifiers.Alt) == EventModifiers.None;
			if (!flag)
			{
				VisualElement visualElement = evt.currentTarget as VisualElement;
				Toggle toggle = visualElement.Q<Toggle>(TreeView.s_ItemToggleName, null);
				int num = (int)toggle.userData;
				ITreeViewItem item = this.m_ItemWrappers[num].item;
				bool flag2 = this.IsExpandedByIndex(num);
				bool flag3 = !item.hasChildren;
				if (!flag3)
				{
					HashSet<int> hashSet = new HashSet<int>(this.m_ExpandedItemIds);
					bool flag4 = flag2;
					if (flag4)
					{
						hashSet.Remove(item.id);
					}
					else
					{
						hashSet.Add(item.id);
					}
					foreach (ITreeViewItem treeViewItem in TreeView.GetAllItems(item.children))
					{
						bool hasChildren = treeViewItem.hasChildren;
						if (hasChildren)
						{
							bool flag5 = flag2;
							if (flag5)
							{
								hashSet.Remove(treeViewItem.id);
							}
							else
							{
								hashSet.Add(treeViewItem.id);
							}
						}
					}
					this.m_ExpandedItemIds = hashSet.ToList<int>();
					this.Refresh();
					evt.StopPropagation();
				}
			}
		}

		private VisualElement MakeTreeItem()
		{
			VisualElement visualElement = new VisualElement
			{
				name = TreeView.s_ItemName,
				style = 
				{
					flexDirection = FlexDirection.Row
				}
			};
			visualElement.AddToClassList(TreeView.s_ItemName);
			visualElement.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnItemMouseUp), TrickleDown.NoTrickleDown);
			VisualElement visualElement2 = new VisualElement
			{
				name = TreeView.s_ItemIndentsContainerName,
				style = 
				{
					flexDirection = FlexDirection.Row
				}
			};
			visualElement2.AddToClassList(TreeView.s_ItemIndentsContainerName);
			visualElement.hierarchy.Add(visualElement2);
			Toggle toggle = new Toggle
			{
				name = TreeView.s_ItemToggleName
			};
			toggle.AddToClassList(Foldout.toggleUssClassName);
			toggle.RegisterValueChangedCallback<bool>(new EventCallback<ChangeEvent<bool>>(this.ToggleExpandedState));
			visualElement.hierarchy.Add(toggle);
			VisualElement visualElement3 = new VisualElement
			{
				name = TreeView.s_ItemContentContainerName,
				style = 
				{
					flexGrow = 1f
				}
			};
			visualElement3.AddToClassList(TreeView.s_ItemContentContainerName);
			visualElement.Add(visualElement3);
			bool flag = this.m_MakeItem != null;
			if (flag)
			{
				visualElement3.Add(this.m_MakeItem());
			}
			return visualElement;
		}

		private void BindTreeItem(VisualElement element, int index)
		{
			ITreeViewItem item = this.m_ItemWrappers[index].item;
			VisualElement visualElement = element.Q(TreeView.s_ItemIndentsContainerName, null);
			visualElement.Clear();
			for (int i = 0; i < this.m_ItemWrappers[index].depth; i++)
			{
				VisualElement visualElement2 = new VisualElement();
				visualElement2.AddToClassList(TreeView.s_ItemIndentName);
				visualElement.Add(visualElement2);
			}
			Toggle toggle = element.Q<Toggle>(TreeView.s_ItemToggleName, null);
			toggle.SetValueWithoutNotify(this.IsExpandedByIndex(index));
			toggle.userData = index;
			bool hasChildren = item.hasChildren;
			if (hasChildren)
			{
				toggle.visible = true;
			}
			else
			{
				toggle.visible = false;
			}
			bool flag = this.m_BindItem == null;
			if (!flag)
			{
				VisualElement visualElement3 = element.Q(TreeView.s_ItemContentContainerName, null).ElementAt(0);
				this.m_BindItem(visualElement3, item);
			}
		}

		private int GetItemId(int index)
		{
			return this.m_ItemWrappers[index].id;
		}

		private bool IsExpandedByIndex(int index)
		{
			return this.m_ExpandedItemIds.Contains(this.m_ItemWrappers[index].id);
		}

		private void CollapseItemByIndex(int index)
		{
			bool flag = !this.m_ItemWrappers[index].item.hasChildren;
			if (!flag)
			{
				this.m_ExpandedItemIds.Remove(this.m_ItemWrappers[index].item.id);
				int num = 0;
				int num2 = index + 1;
				int depth = this.m_ItemWrappers[index].depth;
				while (num2 < this.m_ItemWrappers.Count && this.m_ItemWrappers[num2].depth > depth)
				{
					num++;
					num2++;
				}
				this.m_ItemWrappers.RemoveRange(index + 1, num);
				this.ListViewRefresh();
				base.SaveViewData();
			}
		}

		private void ExpandItemByIndex(int index)
		{
			bool flag = !this.m_ItemWrappers[index].item.hasChildren;
			if (!flag)
			{
				List<TreeView.TreeViewItemWrapper> list = new List<TreeView.TreeViewItemWrapper>();
				this.CreateWrappers(this.m_ItemWrappers[index].item.children, this.m_ItemWrappers[index].depth + 1, ref list);
				this.m_ItemWrappers.InsertRange(index + 1, list);
				this.m_ExpandedItemIds.Add(this.m_ItemWrappers[index].item.id);
				this.ListViewRefresh();
				base.SaveViewData();
			}
		}

		private void ToggleExpandedState(ChangeEvent<bool> evt)
		{
			Toggle toggle = evt.target as Toggle;
			int num = (int)toggle.userData;
			bool flag = this.IsExpandedByIndex(num);
			Assert.AreNotEqual<bool>(flag, evt.newValue);
			bool flag2 = flag;
			if (flag2)
			{
				this.CollapseItemByIndex(num);
			}
			else
			{
				this.ExpandItemByIndex(num);
			}
			this.m_ScrollView.contentContainer.Focus();
		}

		private void CreateWrappers(IEnumerable<ITreeViewItem> items, int depth, ref List<TreeView.TreeViewItemWrapper> wrappers)
		{
			int num = 0;
			foreach (ITreeViewItem treeViewItem in items)
			{
				TreeView.TreeViewItemWrapper treeViewItemWrapper = new TreeView.TreeViewItemWrapper
				{
					depth = depth,
					item = treeViewItem
				};
				wrappers.Add(treeViewItemWrapper);
				bool flag = this.m_ExpandedItemIds.Contains(treeViewItem.id) && treeViewItem.hasChildren;
				if (flag)
				{
					this.CreateWrappers(treeViewItem.children, depth + 1, ref wrappers);
				}
				num++;
			}
		}

		private void RegenerateWrappers()
		{
			this.m_ItemWrappers.Clear();
			bool flag = this.m_RootItems == null;
			if (!flag)
			{
				this.CreateWrappers(this.m_RootItems, 0, ref this.m_ItemWrappers);
			}
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			int itemHeight = this.m_ListView.itemHeight;
			int num = 0;
			bool flag = !this.m_ListView.m_ItemHeightIsInline && e.customStyle.TryGetValue(ListView.s_ItemHeightProperty, out num);
			if (flag)
			{
				this.m_ListView.m_ItemHeight = num;
			}
			bool flag2 = this.m_ListView.m_ItemHeight != itemHeight;
			if (flag2)
			{
				this.m_ListView.Refresh();
			}
		}

		private static readonly string s_ListViewName = "unity-tree-view__list-view";

		private static readonly string s_ItemName = "unity-tree-view__item";

		private static readonly string s_ItemToggleName = "unity-tree-view__item-toggle";

		private static readonly string s_ItemIndentsContainerName = "unity-tree-view__item-indents";

		private static readonly string s_ItemIndentName = "unity-tree-view__item-indent";

		private static readonly string s_ItemContentContainerName = "unity-tree-view__item-content";

		private Func<VisualElement> m_MakeItem;

		private List<ITreeViewItem> m_CurrentSelection;

		private Action<VisualElement, ITreeViewItem> m_BindItem;

		private IList<ITreeViewItem> m_RootItems;

		[SerializeField]
		private List<int> m_ExpandedItemIds;

		private List<TreeView.TreeViewItemWrapper> m_ItemWrappers;

		private ListView m_ListView;

		private ScrollView m_ScrollView;

		public new class UxmlFactory : UxmlFactory<TreeView, TreeView.UxmlTraits>
		{
		}

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
				((TreeView)ve).itemHeight = this.m_ItemHeight.GetValueFromBag(bag, cc);
			}

			private UxmlIntAttributeDescription m_ItemHeight = new UxmlIntAttributeDescription
			{
				name = "item-height",
				defaultValue = ListView.s_DefaultItemHeight
			};
		}

		private struct TreeViewItemWrapper
		{
			public int id
			{
				get
				{
					return this.item.id;
				}
			}

			public bool hasChildren
			{
				get
				{
					return this.item.hasChildren;
				}
			}

			public int depth;

			public ITreeViewItem item;
		}
	}
}
