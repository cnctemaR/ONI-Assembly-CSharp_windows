using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public abstract class BaseListView : BaseVerticalCollectionView
	{
		[CreateProperty]
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
					base.NotifyPropertyChanged(in BaseListView.showBoundCollectionSizeProperty);
				}
			}
		}

		[CreateProperty]
		public bool showFoldoutHeader
		{
			get
			{
				return this.m_ShowFoldoutHeader;
			}
			set
			{
				bool showFoldoutHeader = this.m_ShowFoldoutHeader;
				this.m_ShowFoldoutHeader = value;
				try
				{
					bool flag = this.makeHeader != null;
					if (!flag)
					{
						base.EnableInClassList(BaseListView.listViewWithHeaderUssClassName, value);
						bool showFoldoutHeader2 = this.m_ShowFoldoutHeader;
						if (showFoldoutHeader2)
						{
							this.AddFoldout();
						}
						else
						{
							bool flag2 = this.m_Foldout != null;
							if (flag2)
							{
								VisualElement visualElement = this.drawnFooter;
								if (visualElement != null)
								{
									visualElement.RemoveFromHierarchy();
								}
								this.RemoveFoldout();
							}
						}
						this.SetupArraySizeField();
						this.UpdateListViewLabel();
						bool flag3 = this.makeFooter == null;
						if (flag3)
						{
							bool showAddRemoveFooter = this.showAddRemoveFooter;
							if (showAddRemoveFooter)
							{
								this.EnableFooter(true);
							}
						}
						else
						{
							bool showFoldoutHeader3 = this.m_ShowFoldoutHeader;
							if (showFoldoutHeader3)
							{
								VisualElement visualElement2 = this.drawnFooter;
								if (visualElement2 != null)
								{
									visualElement2.RemoveFromHierarchy();
								}
								Foldout foldout = this.m_Foldout;
								if (foldout != null)
								{
									foldout.contentContainer.Add(this.drawnFooter);
								}
							}
							else
							{
								base.hierarchy.Add(this.drawnFooter);
								base.hierarchy.BringToFront(this.drawnFooter);
							}
						}
					}
				}
				finally
				{
					bool flag4 = showFoldoutHeader != this.m_ShowFoldoutHeader;
					if (flag4)
					{
						base.NotifyPropertyChanged(in BaseListView.showFoldoutHeaderProperty);
					}
				}
			}
		}

		private void AddFoldout()
		{
			bool flag = this.m_Foldout != null;
			if (!flag)
			{
				this.m_Foldout = new Foldout
				{
					name = BaseListView.foldoutHeaderUssClassName,
					text = this.m_HeaderTitle
				};
				this.m_Foldout.toggle.tabIndex = 10;
				this.m_Foldout.toggle.acceptClicksIfDisabled = true;
				this.m_Foldout.AddToClassList(BaseListView.foldoutHeaderUssClassName);
				base.hierarchy.Add(this.m_Foldout);
				this.m_Foldout.Add(base.scrollView);
			}
		}

		private void RemoveFoldout()
		{
			Foldout foldout = this.m_Foldout;
			if (foldout != null)
			{
				foldout.RemoveFromHierarchy();
			}
			this.m_Foldout = null;
			base.hierarchy.Add(base.scrollView);
		}

		internal void SetupArraySizeField()
		{
			bool flag = !this.showBoundCollectionSize || (!this.showFoldoutHeader && base.GetProperty("__unity-collection-view-internal-binding") == null) || this.drawnHeader != null;
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

		[CreateProperty]
		public string headerTitle
		{
			get
			{
				return this.m_HeaderTitle;
			}
			set
			{
				string headerTitle = this.m_HeaderTitle;
				this.m_HeaderTitle = value;
				bool flag = this.m_Foldout != null;
				if (flag)
				{
					this.m_Foldout.text = this.m_HeaderTitle;
				}
				bool flag2 = string.CompareOrdinal(headerTitle, this.m_HeaderTitle) != 0;
				if (flag2)
				{
					base.NotifyPropertyChanged(in BaseListView.headerTitleProperty);
				}
			}
		}

		[CreateProperty]
		public Func<VisualElement> makeHeader
		{
			get
			{
				return this.m_MakeHeader;
			}
			set
			{
				bool flag = value == this.m_MakeHeader;
				if (!flag)
				{
					this.RemoveFoldout();
					this.m_MakeHeader = value;
					bool flag2 = this.m_MakeHeader != null;
					if (flag2)
					{
						this.SetupArraySizeField();
						this.drawnHeader = this.m_MakeHeader();
						this.drawnHeader.tabIndex = 1;
						base.hierarchy.Add(this.drawnHeader);
						base.hierarchy.SendToBack(this.drawnHeader);
					}
					else
					{
						VisualElement visualElement = this.drawnHeader;
						if (visualElement != null)
						{
							visualElement.RemoveFromHierarchy();
						}
						this.drawnHeader = null;
						bool showFoldoutHeader = this.showFoldoutHeader;
						if (showFoldoutHeader)
						{
							this.AddFoldout();
							this.SetupArraySizeField();
							this.UpdateListViewLabel();
						}
					}
					bool flag3 = this.drawnFooter != null;
					if (flag3)
					{
						bool flag4 = this.m_Foldout != null;
						if (flag4)
						{
							this.drawnFooter.RemoveFromHierarchy();
							this.m_Foldout.contentContainer.hierarchy.Add(this.drawnFooter);
						}
						else
						{
							base.hierarchy.Add(this.drawnFooter);
							VisualElement visualElement2 = this.drawnFooter;
							if (visualElement2 != null)
							{
								visualElement2.BringToFront();
							}
						}
					}
					else
					{
						this.EnableFooter(this.showAddRemoveFooter);
					}
					base.NotifyPropertyChanged(in BaseListView.makeHeaderProperty);
				}
			}
		}

		[CreateProperty]
		public Func<VisualElement> makeFooter
		{
			get
			{
				return this.m_MakeFooter;
			}
			set
			{
				bool flag = value == this.m_MakeFooter;
				if (!flag)
				{
					this.m_MakeFooter = value;
					bool flag2 = this.m_MakeFooter != null;
					if (flag2)
					{
						VisualElement footer = this.m_Footer;
						if (footer != null)
						{
							footer.RemoveFromHierarchy();
						}
						this.m_Footer = null;
						this.drawnFooter = this.m_MakeFooter();
						bool flag3 = this.m_Foldout != null;
						if (flag3)
						{
							this.m_Foldout.contentContainer.Add(this.drawnFooter);
						}
						else
						{
							base.hierarchy.Add(this.drawnFooter);
							base.hierarchy.BringToFront(this.drawnFooter);
						}
						base.EnableInClassList(BaseListView.listViewWithFooterUssClassName, true);
						base.scrollView.EnableInClassList(BaseListView.scrollViewWithFooterUssClassName, true);
					}
					else
					{
						VisualElement visualElement = this.drawnFooter;
						if (visualElement != null)
						{
							visualElement.RemoveFromHierarchy();
						}
						this.drawnFooter = null;
						this.EnableFooter(this.m_ShowAddRemoveFooter);
					}
					base.NotifyPropertyChanged(in BaseListView.makeFooterProperty);
				}
			}
		}

		[CreateProperty]
		public bool showAddRemoveFooter
		{
			get
			{
				return this.m_Footer != null;
			}
			set
			{
				bool showAddRemoveFooter = this.showAddRemoveFooter;
				this.m_ShowAddRemoveFooter = value;
				bool flag = this.makeFooter == null;
				if (flag)
				{
					this.EnableFooter(value);
				}
				bool flag2 = value && this.m_ArraySizeField != null;
				if (flag2)
				{
					this.m_ArraySizeField.AddToClassList(BaseListView.arraySizeFieldWithFooterUssClassName);
				}
				bool flag3 = showAddRemoveFooter != this.showFoldoutHeader;
				if (flag3)
				{
					base.NotifyPropertyChanged(in BaseListView.showAddRemoveFooterProperty);
				}
			}
		}

		internal Foldout headerFoldout
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_Foldout;
			}
		}

		private void EnableFooter(bool enabled)
		{
			base.EnableInClassList(BaseListView.listViewWithFooterUssClassName, enabled);
			base.scrollView.EnableInClassList(BaseListView.scrollViewWithFooterUssClassName, enabled);
			if (enabled)
			{
				bool flag = this.m_Footer == null;
				if (flag)
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
					this.m_AddButton.SetEnabled(this.allowAdd);
					this.m_Footer.Add(this.m_AddButton);
					this.m_RemoveButton = new Button(new Action(this.OnRemoveClicked))
					{
						name = BaseListView.footerRemoveButtonName,
						text = "-"
					};
					this.m_Footer.Add(this.m_RemoveButton);
					this.UpdateRemoveButton();
				}
				bool flag2 = this.m_Foldout != null;
				if (flag2)
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

		private IVisualElementScheduledItem trackItemCount
		{
			get
			{
				bool flag = this.m_TrackedItem != null;
				IVisualElementScheduledItem visualElementScheduledItem;
				if (flag)
				{
					visualElementScheduledItem = this.m_TrackedItem;
				}
				else
				{
					this.m_TrackedItem = base.schedule.Execute(this.trackCount).Until(this.untilManualBindingSourceSelectionMode);
					visualElementScheduledItem = this.m_TrackedItem;
				}
				return visualElementScheduledItem;
			}
		}

		private Action trackCount
		{
			get
			{
				Action action;
				if ((action = this.m_TrackCount) == null)
				{
					action = (this.m_TrackCount = delegate
					{
						IList itemsSource = base.itemsSource;
						int? num = ((itemsSource != null) ? new int?(itemsSource.Count) : null);
						int previousRefreshedCount = this.m_PreviousRefreshedCount;
						bool flag = !((num.GetValueOrDefault() == previousRefreshedCount) & (num != null));
						if (flag)
						{
							base.RefreshItems();
						}
					});
				}
				return action;
			}
		}

		private Func<bool> untilManualBindingSourceSelectionMode
		{
			get
			{
				Func<bool> func;
				if ((func = this.m_WhileAutoAssign) == null)
				{
					func = (this.m_WhileAutoAssign = () => !this.autoAssignSource);
				}
				return func;
			}
		}

		[CreateProperty]
		public BindingSourceSelectionMode bindingSourceSelectionMode
		{
			get
			{
				return this.m_BindingSourceSelectionMode;
			}
			set
			{
				bool flag = this.m_BindingSourceSelectionMode == value;
				if (!flag)
				{
					this.m_BindingSourceSelectionMode = value;
					base.Rebuild();
					base.NotifyPropertyChanged(in BaseListView.bindingSourceSelectionModeProperty);
					bool autoAssignSource = this.autoAssignSource;
					if (autoAssignSource)
					{
						this.trackItemCount.Resume();
					}
				}
			}
		}

		internal bool autoAssignSource
		{
			get
			{
				return this.bindingSourceSelectionMode == BindingSourceSelectionMode.AutoAssign;
			}
		}

		private void AddItems(int itemCount)
		{
			BaseListViewController baseListViewController = base.GetOrCreateViewController() as BaseListViewController;
			bool flag = baseListViewController != null;
			if (flag)
			{
				baseListViewController.AddItems(itemCount);
			}
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

		private void UpdateRemoveButton()
		{
			Button removeButton = this.m_RemoveButton;
			if (removeButton != null)
			{
				bool flag;
				if (this.allowRemove)
				{
					BaseListViewController viewController = this.viewController;
					flag = viewController != null && viewController.GetItemsCount() > 0;
				}
				else
				{
					flag = false;
				}
				removeButton.SetEnabled(flag);
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
						bool flag4 = this.m_MakeNoneElement != null;
						if (flag4)
						{
							if (this.m_NoneElement == null)
							{
								this.m_NoneElement = this.m_MakeNoneElement();
							}
							base.scrollView.contentViewport.Add(this.m_NoneElement);
							Label listViewLabel = this.m_ListViewLabel;
							if (listViewLabel != null)
							{
								listViewLabel.RemoveFromHierarchy();
							}
							this.m_ListViewLabel = null;
						}
						else
						{
							if (this.m_ListViewLabel == null)
							{
								this.m_ListViewLabel = new Label();
							}
							this.m_ListViewLabel.text = BaseListView.k_EmptyListStr;
							base.scrollView.contentViewport.Add(this.m_ListViewLabel);
							VisualElement noneElement = this.m_NoneElement;
							if (noneElement != null)
							{
								noneElement.RemoveFromHierarchy();
							}
							this.m_NoneElement = null;
						}
					}
					else
					{
						VisualElement noneElement2 = this.m_NoneElement;
						if (noneElement2 != null)
						{
							noneElement2.RemoveFromHierarchy();
						}
						this.m_NoneElement = null;
						Label listViewLabel2 = this.m_ListViewLabel;
						if (listViewLabel2 != null)
						{
							listViewLabel2.RemoveFromHierarchy();
						}
						this.m_ListViewLabel = null;
					}
				}
				Label listViewLabel3 = this.m_ListViewLabel;
				if (listViewLabel3 != null)
				{
					listViewLabel3.EnableInClassList(BaseListView.emptyLabelUssClassName, flag2);
				}
				Label listViewLabel4 = this.m_ListViewLabel;
				if (listViewLabel4 != null)
				{
					listViewLabel4.EnableInClassList(BaseListView.overMaxMultiEditLimitClassName, this.m_IsOverMultiEditLimit);
				}
			}
		}

		private void OnAddClicked()
		{
			BaseListView.<>c__DisplayClass82_0 CS$<>8__locals1 = new BaseListView.<>c__DisplayClass82_0();
			CS$<>8__locals1.<>4__this = this;
			BaseListView.<>c__DisplayClass82_0 CS$<>8__locals2 = CS$<>8__locals1;
			IList itemsSource = base.itemsSource;
			CS$<>8__locals2.itemsCountPreCallback = ((itemsSource != null) ? itemsSource.Count : 0);
			bool flag = this.overridingAddButtonBehavior != null;
			if (flag)
			{
				this.overridingAddButtonBehavior(this, this.m_AddButton);
			}
			else
			{
				bool flag2 = this.onAdd != null;
				if (flag2)
				{
					this.onAdd(this);
				}
				else
				{
					this.AddItems(1);
				}
			}
			bool flag3 = base.GetProperty("__unity-collection-view-internal-binding") == null;
			if (flag3)
			{
				this.OnAfterAddClicked(CS$<>8__locals1.itemsCountPreCallback);
			}
			else
			{
				base.schedule.Execute(delegate
				{
					CS$<>8__locals1.<>4__this.OnAfterAddClicked(CS$<>8__locals1.itemsCountPreCallback);
				}).ExecuteLater(100L);
			}
			bool flag4 = this.HasValidDataAndBindings() && this.m_ArraySizeField != null;
			if (flag4)
			{
				this.m_ArraySizeField.showMixedValue = false;
			}
		}

		private void OnAfterAddClicked(int itemsCountPreCallback)
		{
			bool flag = base.itemsSource != null && itemsCountPreCallback != base.itemsSource.Count;
			if (flag)
			{
				this.OnItemsSourceSizeChanged();
				base.SetSelection(base.itemsSource.Count - 1);
				base.ScrollToItem(-1);
			}
		}

		private void OnRemoveClicked()
		{
			bool flag = this.onRemove != null;
			if (flag)
			{
				this.onRemove(this);
			}
			else
			{
				bool flag2 = base.selectedIndices.Any<int>();
				if (flag2)
				{
					this.viewController.RemoveItems(base.selectedIndices.ToList<int>());
					base.ClearSelection();
				}
				else
				{
					IList itemsSource = base.itemsSource;
					bool flag3 = itemsSource != null && itemsSource.Count > 0;
					if (flag3)
					{
						int num = base.itemsSource.Count - 1;
						this.viewController.RemoveItem(num);
					}
				}
			}
			bool flag4 = this.HasValidDataAndBindings() && this.m_ArraySizeField != null;
			if (flag4)
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

		[CreateProperty]
		public ListViewReorderMode reorderMode
		{
			get
			{
				return this.m_ReorderMode;
			}
			set
			{
				bool flag = value == this.m_ReorderMode;
				if (!flag)
				{
					this.m_ReorderMode = value;
					base.InitializeDragAndDropController(base.reorderable);
					Action action = this.reorderModeChanged;
					if (action != null)
					{
						action();
					}
					base.Rebuild();
					base.NotifyPropertyChanged(in BaseListView.reorderModeProperty);
				}
			}
		}

		[CreateProperty]
		public Func<VisualElement> makeNoneElement
		{
			get
			{
				return this.m_MakeNoneElement;
			}
			set
			{
				bool flag = value == this.m_MakeNoneElement;
				if (!flag)
				{
					this.m_MakeNoneElement = value;
					VisualElement noneElement = this.m_NoneElement;
					if (noneElement != null)
					{
						noneElement.RemoveFromHierarchy();
					}
					this.m_NoneElement = null;
					base.RefreshItems();
					base.NotifyPropertyChanged(in BaseListView.makeNoneElementProperty);
				}
			}
		}

		[CreateProperty]
		public bool allowAdd
		{
			get
			{
				return this.m_AllowAdd;
			}
			set
			{
				bool flag = value == this.m_AllowAdd;
				if (!flag)
				{
					this.m_AllowAdd = value;
					Button addButton = this.m_AddButton;
					if (addButton != null)
					{
						addButton.SetEnabled(this.m_AllowAdd);
					}
					base.NotifyPropertyChanged(in BaseListView.allowAddProperty);
				}
			}
		}

		[CreateProperty]
		public Action<BaseListView, Button> overridingAddButtonBehavior
		{
			get
			{
				return this.m_OverridingAddButtonBehavior;
			}
			set
			{
				bool flag = value == this.m_OverridingAddButtonBehavior;
				if (!flag)
				{
					this.m_OverridingAddButtonBehavior = value;
					base.RefreshItems();
					base.NotifyPropertyChanged(in BaseListView.overridingAddButtonBehaviorProperty);
				}
			}
		}

		[CreateProperty]
		public Action<BaseListView> onAdd
		{
			get
			{
				return this.m_OnAdd;
			}
			set
			{
				bool flag = value == this.m_OnAdd;
				if (!flag)
				{
					this.m_OnAdd = value;
					base.RefreshItems();
					base.NotifyPropertyChanged(in BaseListView.onAddProperty);
				}
			}
		}

		[CreateProperty]
		public bool allowRemove
		{
			get
			{
				return this.m_AllowRemove;
			}
			set
			{
				bool flag = value == this.m_AllowRemove;
				if (!flag)
				{
					this.m_AllowRemove = value;
					this.UpdateRemoveButton();
					base.NotifyPropertyChanged(in BaseListView.allowRemoveProperty);
				}
			}
		}

		[CreateProperty]
		public Action<BaseListView> onRemove
		{
			get
			{
				return this.m_OnRemove;
			}
			set
			{
				bool flag = value == this.m_OnRemove;
				if (!flag)
				{
					this.m_OnRemove = value;
					base.RefreshItems();
					base.NotifyPropertyChanged(in BaseListView.onRemoveProperty);
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
			: this(null, -1f)
		{
		}

		public BaseListView(IList itemsSource, float itemHeight = -1f)
			: base(itemsSource, itemHeight)
		{
			base.AddToClassList(BaseListView.ussClassName);
			base.pickingMode = PickingMode.Ignore;
			this.allowAdd = true;
			this.allowRemove = true;
		}

		private protected override void PostRefresh()
		{
			this.UpdateArraySizeField();
			this.UpdateRemoveButton();
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

		internal static readonly BindingId showBoundCollectionSizeProperty = "showBoundCollectionSize";

		internal static readonly BindingId showFoldoutHeaderProperty = "showFoldoutHeader";

		internal static readonly BindingId headerTitleProperty = "headerTitle";

		internal static readonly BindingId makeHeaderProperty = "makeHeader";

		internal static readonly BindingId makeFooterProperty = "makeFooter";

		internal static readonly BindingId showAddRemoveFooterProperty = "showAddRemoveFooter";

		internal static readonly BindingId bindingSourceSelectionModeProperty = "bindingSourceSelectionMode";

		internal static readonly BindingId reorderModeProperty = "reorderMode";

		internal static readonly BindingId makeNoneElementProperty = "makeNoneElement";

		internal static readonly BindingId allowAddProperty = "allowAdd";

		internal static readonly BindingId overridingAddButtonBehaviorProperty = "overridingAddButtonBehavior";

		internal static readonly BindingId onAddProperty = "onAdd";

		internal static readonly BindingId allowRemoveProperty = "allowRemove";

		internal static readonly BindingId onRemoveProperty = "onRemove";

		private const int k_FoldoutTabIndex = 10;

		private const int k_ArraySizeFieldTabIndex = 20;

		private bool m_ShowBoundCollectionSize = true;

		private bool m_ShowFoldoutHeader;

		private string m_HeaderTitle;

		private VisualElement drawnHeader;

		private Func<VisualElement> m_MakeHeader;

		private VisualElement drawnFooter;

		private Func<VisualElement> m_MakeFooter;

		private bool m_ShowAddRemoveFooter;

		private IVisualElementScheduledItem m_TrackedItem;

		private Action m_TrackCount;

		private Func<bool> m_WhileAutoAssign;

		private BindingSourceSelectionMode m_BindingSourceSelectionMode = BindingSourceSelectionMode.Manual;

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

		private VisualElement m_NoneElement;

		private Func<VisualElement> m_MakeNoneElement;

		private bool m_AllowAdd = true;

		private Action<BaseListView, Button> m_OverridingAddButtonBehavior;

		private Action<BaseListView> m_OnAdd;

		private bool m_AllowRemove = true;

		private Action<BaseListView> m_OnRemove;

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

		[ExcludeFromDocs]
		[Serializable]
		public new abstract class UxmlSerializedData : BaseVerticalCollectionView.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(BaseListView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("showFoldoutHeader", "show-foldout-header", null, Array.Empty<string>()),
					new UxmlAttributeNames("headerTitle", "header-title", null, Array.Empty<string>()),
					new UxmlAttributeNames("showAddRemoveFooter", "show-add-remove-footer", null, Array.Empty<string>()),
					new UxmlAttributeNames("allowAdd", "allow-add", null, Array.Empty<string>()),
					new UxmlAttributeNames("allowRemove", "allow-remove", null, Array.Empty<string>()),
					new UxmlAttributeNames("reorderMode", "reorder-mode", null, Array.Empty<string>()),
					new UxmlAttributeNames("showBoundCollectionSize", "show-bound-collection-size", null, Array.Empty<string>()),
					new UxmlAttributeNames("bindingSourceSelectionMode", "binding-source-selection-mode", null, Array.Empty<string>())
				}, false);
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				BaseListView baseListView = (BaseListView)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showFoldoutHeader_UxmlAttributeFlags);
				if (flag)
				{
					baseListView.showFoldoutHeader = this.showFoldoutHeader;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.headerTitle_UxmlAttributeFlags);
				if (flag2)
				{
					baseListView.headerTitle = this.headerTitle;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showAddRemoveFooter_UxmlAttributeFlags);
				if (flag3)
				{
					baseListView.showAddRemoveFooter = this.showAddRemoveFooter;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.allowAdd_UxmlAttributeFlags);
				if (flag4)
				{
					baseListView.allowAdd = this.allowAdd;
				}
				bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.allowRemove_UxmlAttributeFlags);
				if (flag5)
				{
					baseListView.allowRemove = this.allowRemove;
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.reorderMode_UxmlAttributeFlags);
				if (flag6)
				{
					baseListView.reorderMode = this.reorderMode;
				}
				bool flag7 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.showBoundCollectionSize_UxmlAttributeFlags);
				if (flag7)
				{
					baseListView.showBoundCollectionSize = this.showBoundCollectionSize;
				}
				bool flag8 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.bindingSourceSelectionMode_UxmlAttributeFlags);
				if (flag8)
				{
					baseListView.bindingSourceSelectionMode = this.bindingSourceSelectionMode;
				}
			}

			[SerializeField]
			private string headerTitle;

			[SerializeField]
			private ListViewReorderMode reorderMode;

			[SerializeField]
			private BindingSourceSelectionMode bindingSourceSelectionMode;

			[SerializeField]
			private bool showFoldoutHeader;

			[SerializeField]
			private bool showAddRemoveFooter;

			[SerializeField]
			private bool allowAdd;

			[SerializeField]
			private bool allowRemove;

			[SerializeField]
			private bool showBoundCollectionSize;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showFoldoutHeader_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags headerTitle_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showAddRemoveFooter_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags allowAdd_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags allowRemove_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags reorderMode_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags showBoundCollectionSize_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags bindingSourceSelectionMode_UxmlAttributeFlags;
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
				BaseListView baseListView = (BaseListView)ve;
				baseListView.reorderMode = this.m_ReorderMode.GetValueFromBag(bag, cc);
				baseListView.showFoldoutHeader = this.m_ShowFoldoutHeader.GetValueFromBag(bag, cc);
				baseListView.headerTitle = this.m_HeaderTitle.GetValueFromBag(bag, cc);
				baseListView.showAddRemoveFooter = this.m_ShowAddRemoveFooter.GetValueFromBag(bag, cc);
				baseListView.allowAdd = this.m_AllowAdd.GetValueFromBag(bag, cc);
				baseListView.allowRemove = this.m_AllowRemove.GetValueFromBag(bag, cc);
				baseListView.showBoundCollectionSize = this.m_ShowBoundCollectionSize.GetValueFromBag(bag, cc);
				baseListView.bindingSourceSelectionMode = this.m_BindingSourceSelectionMode.GetValueFromBag(bag, cc);
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

			private readonly UxmlBoolAttributeDescription m_AllowAdd = new UxmlBoolAttributeDescription
			{
				name = "allow-add",
				defaultValue = true
			};

			private readonly UxmlBoolAttributeDescription m_AllowRemove = new UxmlBoolAttributeDescription
			{
				name = "allow-remove",
				defaultValue = true
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

			private readonly UxmlEnumAttributeDescription<BindingSourceSelectionMode> m_BindingSourceSelectionMode = new UxmlEnumAttributeDescription<BindingSourceSelectionMode>
			{
				name = "binding-source-selection-mode",
				defaultValue = BindingSourceSelectionMode.Manual
			};
		}
	}
}
