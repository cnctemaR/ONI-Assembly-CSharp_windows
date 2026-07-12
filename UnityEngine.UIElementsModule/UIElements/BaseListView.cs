using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnityEngine.UIElements
{
	public abstract class BaseListView : BaseVerticalCollectionView
	{
		public bool showBoundCollectionSize
		{
			get
			{
				return this.m_ShowBoundCollectionSize;
			}
			set
			{
				bool flag = this.m_ShowBoundCollectionSize == value;
				if (!flag)
				{
					this.m_ShowBoundCollectionSize = value;
					this.SetupArraySizeField();
				}
			}
		}

		public bool showFoldoutHeader
		{
			get
			{
				return this.m_ShowFoldoutHeader;
			}
			set
			{
				bool flag = this.m_ShowFoldoutHeader == value;
				if (!flag)
				{
					this.m_ShowFoldoutHeader = value;
					base.EnableInClassList(BaseListView.listViewWithHeaderUssClassName, value);
					bool showFoldoutHeader = this.m_ShowFoldoutHeader;
					if (showFoldoutHeader)
					{
						bool flag2 = this.m_Foldout != null;
						if (flag2)
						{
							return;
						}
						this.m_Foldout = new Foldout
						{
							name = BaseListView.foldoutHeaderUssClassName,
							text = this.m_HeaderTitle
						};
						this.m_Foldout.toggle.tabIndex = 10;
						this.m_Foldout.toggle.m_Clickable.acceptClicksIfDisabled = true;
						this.m_Foldout.AddToClassList(BaseListView.foldoutHeaderUssClassName);
						this.m_Foldout.tabIndex = 1;
						base.hierarchy.Add(this.m_Foldout);
						this.m_Foldout.Add(base.scrollView);
					}
					else
					{
						bool flag3 = this.m_Foldout != null;
						if (flag3)
						{
							Foldout foldout = this.m_Foldout;
							if (foldout != null)
							{
								foldout.RemoveFromHierarchy();
							}
							this.m_Foldout = null;
							base.hierarchy.Add(base.scrollView);
						}
					}
					this.SetupArraySizeField();
					this.UpdateListViewLabel();
					bool showAddRemoveFooter = this.showAddRemoveFooter;
					if (showAddRemoveFooter)
					{
						this.EnableFooter(true);
					}
				}
			}
		}

		internal void SetupArraySizeField()
		{
			bool flag = !this.showBoundCollectionSize || (!this.showFoldoutHeader && base.GetProperty("__unity-collection-view-internal-binding") == null);
			if (flag)
			{
				TextField arraySizeField = this.m_ArraySizeField;
				if (arraySizeField != null)
				{
					arraySizeField.RemoveFromHierarchy();
				}
			}
			else
			{
				bool flag2 = this.m_ArraySizeField == null;
				if (flag2)
				{
					this.m_ArraySizeField = new TextField
					{
						name = BaseListView.arraySizeFieldUssClassName,
						tabIndex = 20
					};
					this.m_ArraySizeField.AddToClassList(BaseListView.arraySizeFieldUssClassName);
					this.m_ArraySizeField.RegisterValueChangedCallback<string>(new EventCallback<ChangeEvent<string>>(this.OnArraySizeFieldChanged));
					this.m_ArraySizeField.isDelayed = true;
					this.m_ArraySizeField.focusable = true;
				}
				this.m_ArraySizeField.EnableInClassList(BaseListView.arraySizeFieldWithFooterUssClassName, this.showAddRemoveFooter);
				this.m_ArraySizeField.EnableInClassList(BaseListView.arraySizeFieldWithHeaderUssClassName, this.showFoldoutHeader);
				bool showFoldoutHeader = this.showFoldoutHeader;
				if (showFoldoutHeader)
				{
					this.m_ArraySizeField.label = string.Empty;
					base.hierarchy.Add(this.m_ArraySizeField);
				}
				else
				{
					this.m_ArraySizeField.label = BaseListView.k_SizeFieldLabel;
					base.hierarchy.Insert(0, this.m_ArraySizeField);
				}
				this.UpdateArraySizeField();
			}
		}

		public string headerTitle
		{
			get
			{
				return this.m_HeaderTitle;
			}
			set
			{
				this.m_HeaderTitle = value;
				bool flag = this.m_Foldout != null;
				if (flag)
				{
					this.m_Foldout.text = this.m_HeaderTitle;
				}
			}
		}

		public bool showAddRemoveFooter
		{
			get
			{
				return this.m_Footer != null;
			}
			set
			{
				this.EnableFooter(value);
			}
		}

		internal Foldout headerFoldout
		{
			get
			{
				return this.m_Foldout;
			}
		}

		private void EnableFooter(bool enabled)
		{
			base.EnableInClassList(BaseListView.listViewWithFooterUssClassName, enabled);
			base.scrollView.EnableInClassList(BaseListView.scrollViewWithFooterUssClassName, enabled);
			bool flag = this.m_ArraySizeField != null;
			if (flag)
			{
				this.m_ArraySizeField.EnableInClassList(BaseListView.arraySizeFieldWithFooterUssClassName, enabled);
			}
			if (enabled)
			{
				bool flag2 = this.m_Footer == null;
				if (flag2)
				{
					this.m_Footer = new VisualElement
					{
						name = BaseListView.footerUssClassName
					};
					this.m_Footer.AddToClassList(BaseListView.footerUssClassName);
					this.m_AddButton = new Button(new Action(this.OnAddClicked))
					{
						name = BaseListView.footerAddButtonName,
						text = "+"
					};
					this.m_Footer.Add(this.m_AddButton);
					this.m_RemoveButton = new Button(new Action(this.OnRemoveClicked))
					{
						name = BaseListView.footerRemoveButtonName,
						text = "-"
					};
					this.m_Footer.Add(this.m_RemoveButton);
				}
				bool flag3 = this.m_Foldout != null;
				if (flag3)
				{
					this.m_Foldout.contentContainer.Add(this.m_Footer);
				}
				else
				{
					base.hierarchy.Add(this.m_Footer);
				}
			}
			else
			{
				Button removeButton = this.m_RemoveButton;
				if (removeButton != null)
				{
					removeButton.RemoveFromHierarchy();
				}
				Button addButton = this.m_AddButton;
				if (addButton != null)
				{
					addButton.RemoveFromHierarchy();
				}
				VisualElement footer = this.m_Footer;
				if (footer != null)
				{
					footer.RemoveFromHierarchy();
				}
				this.m_RemoveButton = null;
				this.m_AddButton = null;
				this.m_Footer = null;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<int>> itemsAdded;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<int>> itemsRemoved;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action itemsSourceSizeChanged;

		private void AddItems(int itemCount)
		{
			this.viewController.AddItems(itemCount);
		}

		private void RemoveItems(List<int> indices)
		{
			this.viewController.RemoveItems(indices);
		}

		private void OnArraySizeFieldChanged(ChangeEvent<string> evt)
		{
			bool flag = this.m_ArraySizeField.showMixedValue && BaseField<string>.mixedValueString == evt.newValue;
			if (!flag)
			{
				int num;
				bool flag2 = !int.TryParse(evt.newValue, out num) || num < 0;
				if (flag2)
				{
					this.m_ArraySizeField.SetValueWithoutNotify(evt.previousValue);
				}
				else
				{
					int itemsCount = this.viewController.GetItemsCount();
					bool flag3 = itemsCount == 0 && num == this.viewController.GetItemsMinCount();
					if (!flag3)
					{
						bool flag4 = num > itemsCount;
						if (flag4)
						{
							this.viewController.AddItems(num - itemsCount);
						}
						else
						{
							bool flag5 = num < itemsCount;
							if (flag5)
							{
								this.viewController.RemoveItems(itemsCount - num);
							}
							else
							{
								bool flag6 = num == 0;
								if (flag6)
								{
									this.viewController.ClearItems();
									this.m_IsOverMultiEditLimit = false;
								}
							}
						}
						this.UpdateListViewLabel();
					}
				}
			}
		}

		internal void UpdateArraySizeField()
		{
			bool flag = !this.HasValidDataAndBindings() || this.m_ArraySizeField == null;
			if (!flag)
			{
				bool flag2 = !this.m_ArraySizeField.showMixedValue;
				if (flag2)
				{
					this.m_ArraySizeField.SetValueWithoutNotify(this.viewController.GetItemsMinCount().ToString());
				}
				VisualElement footer = this.footer;
				if (footer != null)
				{
					footer.SetEnabled(!this.m_IsOverMultiEditLimit);
				}
			}
		}

		internal void UpdateListViewLabel()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = base.itemsSource.Count == 0;
				bool isOverMultiEditLimit = this.m_IsOverMultiEditLimit;
				if (isOverMultiEditLimit)
				{
					if (this.m_ListViewLabel == null)
					{
						this.m_ListViewLabel = new Label();
					}
					this.m_ListViewLabel.text = this.m_MaxMultiEditStr;
					base.scrollView.contentViewport.Add(this.m_ListViewLabel);
				}
				else
				{
					bool flag3 = flag2;
					if (flag3)
					{
						if (this.m_ListViewLabel == null)
						{
							this.m_ListViewLabel = new Label();
						}
						this.m_ListViewLabel.text = BaseListView.k_EmptyListStr;
						base.scrollView.contentViewport.Add(this.m_ListViewLabel);
					}
					else
					{
						Label listViewLabel = this.m_ListViewLabel;
						if (listViewLabel != null)
						{
							listViewLabel.RemoveFromHierarchy();
						}
						this.m_ListViewLabel = null;
					}
				}
				Label listViewLabel2 = this.m_ListViewLabel;
				if (listViewLabel2 != null)
				{
					listViewLabel2.EnableInClassList(BaseListView.emptyLabelUssClassName, flag2);
				}
				Label listViewLabel3 = this.m_ListViewLabel;
				if (listViewLabel3 != null)
				{
					listViewLabel3.EnableInClassList(BaseListView.overMaxMultiEditLimitClassName, this.m_IsOverMultiEditLimit);
				}
			}
		}

		private void OnAddClicked()
		{
			this.AddItems(1);
			bool flag = base.binding == null;
			if (flag)
			{
				base.SetSelection(base.itemsSource.Count - 1);
				base.ScrollToItem(-1);
			}
			else
			{
				base.schedule.Execute(delegate
				{
					base.SetSelection(base.itemsSource.Count - 1);
					base.ScrollToItem(-1);
				}).ExecuteLater(100L);
			}
			bool flag2 = this.HasValidDataAndBindings() && this.m_ArraySizeField != null;
			if (flag2)
			{
				this.m_ArraySizeField.showMixedValue = false;
			}
		}

		private void OnRemoveClicked()
		{
			bool flag = base.selectedIndices.Any<int>();
			if (flag)
			{
				this.viewController.RemoveItems(base.selectedIndices.ToList<int>());
				base.ClearSelection();
			}
			else
			{
				bool flag2 = base.itemsSource.Count > 0;
				if (flag2)
				{
					int num = base.itemsSource.Count - 1;
					this.viewController.RemoveItem(num);
				}
			}
			bool flag3 = this.HasValidDataAndBindings() && this.m_ArraySizeField != null;
			if (flag3)
			{
				this.m_ArraySizeField.showMixedValue = false;
			}
		}

		internal TextField arraySizeField
		{
			get
			{
				return this.m_ArraySizeField;
			}
		}

		internal void SetOverMaxMultiEditLimit(bool isOverLimit, int maxMultiEditCount)
		{
			this.m_IsOverMultiEditLimit = isOverLimit;
			this.m_MaxMultiEditCount = maxMultiEditCount;
			this.m_MaxMultiEditStr = string.Format("This field cannot display arrays with more than {0} elements when multiple objects are selected.", this.m_MaxMultiEditCount);
		}

		internal VisualElement footer
		{
			get
			{
				return this.m_Footer;
			}
		}

		public new BaseListViewController viewController
		{
			get
			{
				return base.viewController as BaseListViewController;
			}
		}

		private protected override void CreateVirtualizationController()
		{
			base.CreateVirtualizationController<ReusableListViewItem>();
		}

		public override void SetViewController(CollectionViewController controller)
		{
			if (this.m_ItemAddedCallback == null)
			{
				this.m_ItemAddedCallback = new Action<IEnumerable<int>>(this.OnItemAdded);
			}
			if (this.m_ItemRemovedCallback == null)
			{
				this.m_ItemRemovedCallback = new Action<IEnumerable<int>>(this.OnItemsRemoved);
			}
			if (this.m_ItemsSourceSizeChangedCallback == null)
			{
				this.m_ItemsSourceSizeChangedCallback = new Action(this.OnItemsSourceSizeChanged);
			}
			bool flag = this.viewController != null;
			if (flag)
			{
				this.viewController.itemsAdded -= this.m_ItemAddedCallback;
				this.viewController.itemsRemoved -= this.m_ItemRemovedCallback;
				this.viewController.itemsSourceSizeChanged -= this.m_ItemsSourceSizeChangedCallback;
			}
			base.SetViewController(controller);
			bool flag2 = this.viewController != null;
			if (flag2)
			{
				this.viewController.itemsAdded += this.m_ItemAddedCallback;
				this.viewController.itemsRemoved += this.m_ItemRemovedCallback;
				this.viewController.itemsSourceSizeChanged += this.m_ItemsSourceSizeChangedCallback;
			}
		}

		private void OnItemAdded(IEnumerable<int> indices)
		{
			Action<IEnumerable<int>> action = this.itemsAdded;
			if (action != null)
			{
				action(indices);
			}
		}

		private void OnItemsRemoved(IEnumerable<int> indices)
		{
			Action<IEnumerable<int>> action = this.itemsRemoved;
			if (action != null)
			{
				action(indices);
			}
		}

		private void OnItemsSourceSizeChanged()
		{
			bool flag = base.GetProperty("__unity-collection-view-internal-binding") == null;
			if (flag)
			{
				base.RefreshItems();
			}
			Action action = this.itemsSourceSizeChanged;
			if (action != null)
			{
				action();
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action reorderModeChanged;

		public ListViewReorderMode reorderMode
		{
			get
			{
				return this.m_ReorderMode;
			}
			set
			{
				bool flag = value != this.m_ReorderMode;
				if (flag)
				{
					this.m_ReorderMode = value;
					base.InitializeDragAndDropController(base.reorderable);
					Action action = this.reorderModeChanged;
					if (action != null)
					{
						action();
					}
					base.Rebuild();
				}
			}
		}

		internal override ListViewDragger CreateDragger()
		{
			bool flag = this.m_ReorderMode == ListViewReorderMode.Simple;
			ListViewDragger listViewDragger;
			if (flag)
			{
				listViewDragger = new ListViewDragger(this);
			}
			else
			{
				listViewDragger = new ListViewDraggerAnimated(this);
			}
			return listViewDragger;
		}

		internal override ICollectionDragAndDropController CreateDragAndDropController()
		{
			return new ListViewReorderableDragAndDropController(this);
		}

		public BaseListView()
		{
			base.AddToClassList(BaseListView.ussClassName);
			base.pickingMode = PickingMode.Ignore;
		}

		public BaseListView(IList itemsSource, float itemHeight = -1f)
			: base(itemsSource, itemHeight)
		{
			base.AddToClassList(BaseListView.ussClassName);
			base.pickingMode = PickingMode.Ignore;
		}

		private protected override void PostRefresh()
		{
			this.UpdateArraySizeField();
			this.UpdateListViewLabel();
			base.PostRefresh();
		}

		private protected override bool HandleItemNavigation(bool moveIn, bool altPressed)
		{
			bool flag = false;
			foreach (int num in base.selectedIndices)
			{
				foreach (ReusableCollectionItem reusableCollectionItem in base.activeItems)
				{
					bool flag2 = reusableCollectionItem.index == num && base.GetProperty("__unity-collection-view-internal-binding") != null;
					if (flag2)
					{
						Foldout foldout = reusableCollectionItem.bindableElement.Q<Foldout>(null, null);
						bool flag3 = foldout != null;
						if (flag3)
						{
							foldout.value = moveIn;
							flag = true;
						}
					}
				}
			}
			return flag;
		}

		private static readonly string k_SizeFieldLabel = "Size";

		private const int k_FoldoutTabIndex = 10;

		private const int k_ArraySizeFieldTabIndex = 20;

		private bool m_ShowBoundCollectionSize = true;

		private bool m_ShowFoldoutHeader;

		private string m_HeaderTitle;

		private Label m_ListViewLabel;

		private Foldout m_Foldout;

		private TextField m_ArraySizeField;

		private bool m_IsOverMultiEditLimit;

		private int m_MaxMultiEditCount;

		private VisualElement m_Footer;

		private Button m_AddButton;

		private Button m_RemoveButton;

		private Action<IEnumerable<int>> m_ItemAddedCallback;

		private Action<IEnumerable<int>> m_ItemRemovedCallback;

		private Action m_ItemsSourceSizeChangedCallback;

		private ListViewReorderMode m_ReorderMode;

		public new static readonly string ussClassName = "unity-list-view";

		public new static readonly string itemUssClassName = BaseListView.ussClassName + "__item";

		public static readonly string emptyLabelUssClassName = BaseListView.ussClassName + "__empty-label";

		public static readonly string overMaxMultiEditLimitClassName = BaseListView.ussClassName + "__over-max-multi-edit-limit-label";

		public static readonly string reorderableUssClassName = BaseListView.ussClassName + "__reorderable";

		public static readonly string reorderableItemUssClassName = BaseListView.reorderableUssClassName + "-item";

		public static readonly string reorderableItemContainerUssClassName = BaseListView.reorderableItemUssClassName + "__container";

		public static readonly string reorderableItemHandleUssClassName = BaseListView.reorderableUssClassName + "-handle";

		public static readonly string reorderableItemHandleBarUssClassName = BaseListView.reorderableItemHandleUssClassName + "-bar";

		public static readonly string footerUssClassName = BaseListView.ussClassName + "__footer";

		public static readonly string foldoutHeaderUssClassName = BaseListView.ussClassName + "__foldout-header";

		public static readonly string arraySizeFieldUssClassName = BaseListView.ussClassName + "__size-field";

		public static readonly string arraySizeFieldWithHeaderUssClassName = BaseListView.arraySizeFieldUssClassName + "--with-header";

		public static readonly string arraySizeFieldWithFooterUssClassName = BaseListView.arraySizeFieldUssClassName + "--with-footer";

		public static readonly string listViewWithHeaderUssClassName = BaseListView.ussClassName + "--with-header";

		public static readonly string listViewWithFooterUssClassName = BaseListView.ussClassName + "--with-footer";

		public static readonly string scrollViewWithFooterUssClassName = BaseListView.ussClassName + "__scroll-view--with-footer";

		public static readonly string footerAddButtonName = BaseListView.ussClassName + "__add-button";

		public static readonly string footerRemoveButtonName = BaseListView.ussClassName + "__remove-button";

		private string m_MaxMultiEditStr;

		private static readonly string k_EmptyListStr = "List is empty";

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
				BaseListView baseListView = (BaseListView)ve;
				baseListView.reorderMode = this.m_ReorderMode.GetValueFromBag(bag, cc);
				baseListView.showFoldoutHeader = this.m_ShowFoldoutHeader.GetValueFromBag(bag, cc);
				baseListView.headerTitle = this.m_HeaderTitle.GetValueFromBag(bag, cc);
				baseListView.showAddRemoveFooter = this.m_ShowAddRemoveFooter.GetValueFromBag(bag, cc);
				baseListView.showBoundCollectionSize = this.m_ShowBoundCollectionSize.GetValueFromBag(bag, cc);
			}

			protected UxmlTraits()
			{
				this.m_PickingMode.defaultValue = PickingMode.Ignore;
			}

			private readonly UxmlBoolAttributeDescription m_ShowFoldoutHeader = new UxmlBoolAttributeDescription
			{
				name = "show-foldout-header",
				defaultValue = false
			};

			private readonly UxmlStringAttributeDescription m_HeaderTitle = new UxmlStringAttributeDescription
			{
				name = "header-title",
				defaultValue = string.Empty
			};

			private readonly UxmlBoolAttributeDescription m_ShowAddRemoveFooter = new UxmlBoolAttributeDescription
			{
				name = "show-add-remove-footer",
				defaultValue = false
			};

			private readonly UxmlEnumAttributeDescription<ListViewReorderMode> m_ReorderMode = new UxmlEnumAttributeDescription<ListViewReorderMode>
			{
				name = "reorder-mode",
				defaultValue = ListViewReorderMode.Simple
			};

			private readonly UxmlBoolAttributeDescription m_ShowBoundCollectionSize = new UxmlBoolAttributeDescription
			{
				name = "show-bound-collection-size",
				defaultValue = true
			};
		}
	}
}
