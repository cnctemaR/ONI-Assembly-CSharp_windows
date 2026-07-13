using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	public abstract class BaseTreeView : BaseVerticalCollectionView
	{
		[CreateProperty(ReadOnly = true)]
		public new IList itemsSource
		{
			get
			{
				BaseTreeViewController viewController = this.viewController;
				return (viewController != null) ? viewController.itemsSource : null;
			}
			internal set
			{
				base.GetOrCreateViewController().itemsSource = value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<TreeViewExpansionChangedArgs> itemExpandedChanged;

		public void SetRootItems<T>(IList<TreeViewItemData<T>> rootItems)
		{
			this.SetRootItemsInternal<T>(rootItems);
		}

		internal abstract void SetRootItemsInternal<T>(IList<TreeViewItemData<T>> rootItems);

		public IEnumerable<int> GetRootIds()
		{
			return this.viewController.GetRootItemIds();
		}

		public int GetTreeCount()
		{
			return this.viewController.GetTreeItemsCount();
		}

		public new BaseTreeViewController viewController
		{
			get
			{
				return base.viewController as BaseTreeViewController;
			}
		}

		private protected override void CreateVirtualizationController()
		{
			base.CreateVirtualizationController<ReusableTreeViewItem>();
		}

		public override void SetViewController(CollectionViewController controller)
		{
			bool flag = this.viewController != null;
			if (flag)
			{
				this.viewController.itemIndexChanged -= this.OnItemIndexChanged;
				this.viewController.itemExpandedChanged -= this.OnItemExpandedChanged;
			}
			base.SetViewController(controller);
			bool flag2 = this.viewController != null;
			if (flag2)
			{
				this.viewController.itemIndexChanged += this.OnItemIndexChanged;
				this.viewController.itemExpandedChanged += this.OnItemExpandedChanged;
			}
		}

		private void OnItemIndexChanged(int srcIndex, int dstIndex)
		{
			base.RefreshItems();
		}

		private void OnItemExpandedChanged(TreeViewExpansionChangedArgs arg)
		{
			Action<TreeViewExpansionChangedArgs> action = this.itemExpandedChanged;
			if (action != null)
			{
				action(arg);
			}
		}

		internal override ICollectionDragAndDropController CreateDragAndDropController()
		{
			return new TreeViewReorderableDragAndDropController(this);
		}

		[CreateProperty]
		public bool autoExpand
		{
			get
			{
				return this.m_AutoExpand;
			}
			set
			{
				bool flag = this.m_AutoExpand == value;
				if (!flag)
				{
					this.m_AutoExpand = value;
					base.RefreshItems();
					base.NotifyPropertyChanged(in BaseTreeView.autoExpandProperty);
				}
			}
		}

		internal List<int> expandedItemIds
		{
			get
			{
				return this.m_ExpandedItemIds;
			}
			set
			{
				this.m_ExpandedItemIds = value;
			}
		}

		internal float? customIdent { get; private set; }

		public BaseTreeView()
			: this(-1)
		{
		}

		public BaseTreeView(int itemHeight)
			: base(null, (float)itemHeight)
		{
			this.m_ExpandedItemIds = new List<int>();
			base.AddToClassList(BaseTreeView.ussClassName);
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
		}

		public int GetIdForIndex(int index)
		{
			return this.viewController.GetIdForIndex(index);
		}

		public int GetParentIdForIndex(int index)
		{
			return this.viewController.GetParentId(this.GetIdForIndex(index));
		}

		public IEnumerable<int> GetChildrenIdsForIndex(int index)
		{
			return this.viewController.GetChildrenIdsByIndex(index);
		}

		public IEnumerable<TreeViewItemData<T>> GetSelectedItems<T>()
		{
			return this.GetSelectedItemsInternal<T>();
		}

		private protected abstract IEnumerable<TreeViewItemData<T>> GetSelectedItemsInternal<T>();

		public T GetItemDataForIndex<T>(int index)
		{
			return this.GetItemDataForIndexInternal<T>(index);
		}

		private protected abstract T GetItemDataForIndexInternal<T>(int index);

		public T GetItemDataForId<T>(int id)
		{
			return this.GetItemDataForIdInternal<T>(id);
		}

		private protected abstract T GetItemDataForIdInternal<T>(int id);

		public void AddItem<T>(TreeViewItemData<T> item, int parentId = -1, int childIndex = -1, bool rebuildTree = true)
		{
			this.AddItemInternal<T>(item, parentId, childIndex, rebuildTree);
		}

		private protected abstract void AddItemInternal<T>(TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree);

		public bool TryRemoveItem(int id, bool rebuildTree = true)
		{
			return this.viewController.TryRemoveItem(id, rebuildTree);
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
		{
			float num;
			bool flag = evt.customStyle.TryGetValue(BaseTreeView.s_TreeViewIndentProperty, out num);
			if (flag)
			{
				this.customIdent = new float?(num);
				CollectionVirtualizationController virtualizationController = base.virtualizationController;
				if (virtualizationController != null)
				{
					virtualizationController.Refresh(false);
				}
			}
			else
			{
				this.customIdent = null;
			}
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			bool flag = this.viewController != null;
			if (flag)
			{
				this.viewController.OnViewDataReadyUpdateNodes();
				base.RefreshItems();
			}
		}

		private protected override bool HandleItemNavigation(bool moveIn, bool altPressed)
		{
			int num = 1;
			bool flag = false;
			foreach (int num2 in base.selectedIds)
			{
				int indexForId = this.viewController.GetIndexForId(num2);
				bool flag2 = !this.viewController.HasChildrenByIndex(indexForId);
				if (flag2)
				{
					break;
				}
				bool flag3 = moveIn && !this.IsExpandedByIndex(indexForId);
				if (flag3)
				{
					this.ExpandItemByIndex(indexForId, altPressed);
					flag = true;
				}
				else
				{
					bool flag4 = !moveIn && this.IsExpandedByIndex(indexForId);
					if (flag4)
					{
						this.CollapseItemByIndex(indexForId, altPressed);
						flag = true;
					}
				}
			}
			bool flag5 = flag;
			bool flag6;
			if (flag5)
			{
				flag6 = true;
			}
			else
			{
				bool flag7 = !moveIn;
				if (flag7)
				{
					int idForIndex = this.viewController.GetIdForIndex(base.selectedIndex);
					int parentId = this.viewController.GetParentId(idForIndex);
					bool flag8 = parentId != -1;
					if (flag8)
					{
						this.SetSelectionById(parentId);
						base.ScrollToItemById(parentId);
						return true;
					}
					num = -1;
				}
				int num3 = base.selectedIndex;
				bool flag9;
				do
				{
					num3 += num;
					flag9 = this.viewController.HasChildrenByIndex(num3);
				}
				while (!flag9 && num3 >= 0 && num3 < this.itemsSource.Count);
				bool flag10 = flag9;
				if (flag10)
				{
					base.SetSelection(num3);
					base.ScrollToItem(num3);
					flag6 = true;
				}
				else
				{
					flag6 = false;
				}
			}
			return flag6;
		}

		public void SetSelectionById(int id)
		{
			this.SetSelectionById(new int[] { id });
		}

		public void SetSelectionById(IEnumerable<int> ids)
		{
			this.SetSelectionInternalById(ids, true);
		}

		public void SetSelectionByIdWithoutNotify(IEnumerable<int> ids)
		{
			this.SetSelectionInternalById(ids, false);
		}

		internal void SetSelectionInternalById(IEnumerable<int> ids, bool sendNotification)
		{
			bool flag = ids == null;
			if (!flag)
			{
				List<int> list = ids.Select<int, int>((int id) => this.GetItemIndex(id, true)).ToList<int>();
				base.SetSelectionInternal(list, sendNotification);
			}
		}

		public void AddToSelectionById(int id)
		{
			int itemIndex = this.GetItemIndex(id, true);
			base.AddToSelection(itemIndex);
		}

		public void RemoveFromSelectionById(int id)
		{
			int itemIndex = this.GetItemIndex(id, false);
			base.RemoveFromSelection(itemIndex);
		}

		private int GetItemIndex(int id, bool expand = false)
		{
			if (expand)
			{
				int num = this.viewController.GetParentId(id);
				List<int> list = CollectionPool<List<int>, int>.Get();
				this.viewController.GetExpandedItemIds(list);
				while (num != -1)
				{
					bool flag = !list.Contains(num);
					if (flag)
					{
						this.viewController.ExpandItem(num, false, true);
					}
					num = this.viewController.GetParentId(num);
				}
				CollectionPool<List<int>, int>.Release(list);
			}
			return this.viewController.GetIndexForId(id);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void CopyExpandedStates(int sourceId, int targetId)
		{
			bool flag = this.IsExpanded(sourceId);
			if (flag)
			{
				this.ExpandItem(targetId, false, true);
				bool flag2 = this.viewController.HasChildren(sourceId);
				if (flag2)
				{
					bool flag3 = this.viewController.GetChildrenIds(sourceId).Count<int>() != this.viewController.GetChildrenIds(targetId).Count<int>();
					if (flag3)
					{
						Debug.LogWarning("Source and target hierarchies are not the same");
					}
					else
					{
						for (int i = 0; i < this.viewController.GetChildrenIds(sourceId).Count<int>(); i++)
						{
							int num = this.viewController.GetChildrenIds(sourceId).ElementAt<int>(i);
							int num2 = this.viewController.GetChildrenIds(targetId).ElementAt<int>(i);
							this.CopyExpandedStates(num, num2);
						}
					}
				}
			}
			else
			{
				this.CollapseItem(targetId, false, true);
			}
		}

		public bool IsExpanded(int id)
		{
			return this.viewController.IsExpanded(id);
		}

		public void CollapseItem(int id, bool collapseAllChildren = false, bool refresh = true)
		{
			this.viewController.CollapseItem(id, collapseAllChildren, refresh);
		}

		public void ExpandItem(int id, bool expandAllChildren = false, bool refresh = true)
		{
			this.viewController.ExpandItem(id, expandAllChildren, refresh);
		}

		public void ExpandRootItems()
		{
			foreach (int num in this.viewController.GetRootItemIds())
			{
				this.viewController.ExpandItem(num, false, false);
			}
			base.RefreshItems();
		}

		public void ExpandAll()
		{
			this.viewController.ExpandAll();
		}

		public void CollapseAll()
		{
			this.viewController.CollapseAll();
		}

		private bool IsExpandedByIndex(int index)
		{
			return this.viewController.IsExpandedByIndex(index);
		}

		private void CollapseItemByIndex(int index, bool collapseAll)
		{
			bool flag = !this.viewController.HasChildrenByIndex(index);
			if (!flag)
			{
				this.viewController.CollapseItemByIndex(index, collapseAll, true);
				base.RefreshItems();
				base.SaveViewData();
			}
		}

		private void ExpandItemByIndex(int index, bool expandAll)
		{
			bool flag = !this.viewController.HasChildrenByIndex(index);
			if (!flag)
			{
				this.viewController.ExpandItemByIndex(index, expandAll, true);
				base.RefreshItems();
				base.SaveViewData();
			}
		}

		internal static readonly BindingId autoExpandProperty = "autoExpand";

		internal static CustomStyleProperty<float> s_TreeViewIndentProperty = new CustomStyleProperty<float>("--unity-tree-view-indent");

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static readonly int invalidId = -1;

		public new static readonly string ussClassName = "unity-tree-view";

		public new static readonly string itemUssClassName = BaseTreeView.ussClassName + "__item";

		public static readonly string itemToggleUssClassName = BaseTreeView.ussClassName + "__item-toggle";

		[Obsolete("Individual item indents are no longer used, see itemIndentUssClassName instead", false)]
		public static readonly string itemIndentsContainerUssClassName = BaseTreeView.ussClassName + "__item-indents";

		public static readonly string itemIndentUssClassName = BaseTreeView.ussClassName + "__item-indent";

		public static readonly string itemContentContainerUssClassName = BaseTreeView.ussClassName + "__item-content";

		private bool m_AutoExpand;

		[SerializeField]
		[DontCreateProperty]
		private List<int> m_ExpandedItemIds;

		[ExcludeFromDocs]
		[Serializable]
		public new abstract class UxmlSerializedData : BaseVerticalCollectionView.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(BaseTreeView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("autoExpand", "auto-expand", null, Array.Empty<string>())
				}, false);
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.autoExpand_UxmlAttributeFlags);
				if (flag)
				{
					BaseTreeView baseTreeView = (BaseTreeView)obj;
					baseTreeView.autoExpand = this.autoExpand;
				}
			}

			[SerializeField]
			private bool autoExpand;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags autoExpand_UxmlAttributeFlags;
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseVerticalCollectionView.UxmlTraits
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
				BaseTreeView baseTreeView = (BaseTreeView)ve;
				baseTreeView.autoExpand = this.m_AutoExpand.GetValueFromBag(bag, cc);
			}

			private readonly UxmlBoolAttributeDescription m_AutoExpand = new UxmlBoolAttributeDescription
			{
				name = "auto-expand",
				defaultValue = false
			};
		}
	}
}
