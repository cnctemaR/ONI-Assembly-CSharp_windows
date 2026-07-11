using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnityEngine.UIElements
{
	public class ListView : BindableElement
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<object> onItemChosen;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<List<object>> onSelectionChanged;

		public IList itemsSource
		{
			get
			{
				return this.m_ItemsSource;
			}
			set
			{
				this.m_ItemsSource = value;
				this.Refresh();
			}
		}

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
					this.Refresh();
				}
			}
		}

		public Action<VisualElement, int> bindItem
		{
			get
			{
				return this.m_BindItem;
			}
			set
			{
				this.m_BindItem = value;
				this.Refresh();
			}
		}

		internal Func<int, int> getItemId
		{
			get
			{
				return this.m_GetItemId;
			}
			set
			{
				this.m_GetItemId = value;
				this.Refresh();
			}
		}

		public int itemHeight
		{
			get
			{
				return this.m_ItemHeight;
			}
			set
			{
				this.m_ItemHeightIsInline = true;
				this.m_ItemHeight = value;
				this.Refresh();
			}
		}

		internal List<int> currentSelectionIds
		{
			get
			{
				return this.m_SelectedIds;
			}
		}

		public int selectedIndex
		{
			get
			{
				return (this.m_SelectedIndices.Count == 0) ? (-1) : this.m_SelectedIndices.First<int>();
			}
			set
			{
				this.SetSelection(value);
			}
		}

		public object selectedItem
		{
			get
			{
				return (this.m_SelectedItems.Count == 0) ? null : this.m_SelectedItems.First<object>();
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ScrollView.contentContainer;
			}
		}

		public SelectionType selectionType { get; set; }

		public ListView()
		{
			base.AddToClassList(ListView.ussClassName);
			this.selectionType = SelectionType.Single;
			this.m_ScrollOffset = 0f;
			this.m_ScrollView = new ScrollView();
			this.m_ScrollView.viewDataKey = "list-view__scroll-view";
			this.m_ScrollView.StretchToParentSize();
			this.m_ScrollView.verticalScroller.valueChanged += this.OnScroll;
			base.hierarchy.Add(this.m_ScrollView);
			base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChanged), TrickleDown.NoTrickleDown);
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnClick), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.focusable = true;
			this.m_ScrollView.contentContainer.usageHints &= ~UsageHints.GroupTransform;
			base.focusable = true;
			base.isCompositeRoot = true;
			base.delegatesFocus = true;
		}

		public ListView(IList itemsSource, int itemHeight, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
			: this()
		{
			this.m_ItemsSource = itemsSource;
			this.m_ItemHeight = itemHeight;
			this.m_MakeItem = makeItem;
			this.m_BindItem = bindItem;
		}

		public void OnKeyDown(KeyDownEvent evt)
		{
			bool flag = evt == null || !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = true;
				bool flag3 = true;
				KeyCode keyCode = evt.keyCode;
				if (keyCode <= KeyCode.Escape)
				{
					if (keyCode == KeyCode.Return)
					{
						bool flag4 = this.onItemChosen != null;
						if (flag4)
						{
							this.onItemChosen(this.m_ItemsSource[this.selectedIndex]);
						}
						goto IL_01A8;
					}
					if (keyCode == KeyCode.Escape)
					{
						this.ClearSelection();
						flag3 = false;
						goto IL_01A8;
					}
				}
				else
				{
					if (keyCode == KeyCode.A)
					{
						bool actionKey = evt.actionKey;
						if (actionKey)
						{
							this.SelectAll();
							flag3 = false;
						}
						goto IL_01A8;
					}
					switch (keyCode)
					{
					case KeyCode.UpArrow:
					{
						bool flag5 = this.selectedIndex > 0;
						if (flag5)
						{
							this.selectedIndex--;
						}
						goto IL_01A8;
					}
					case KeyCode.DownArrow:
					{
						bool flag6 = this.selectedIndex + 1 < this.itemsSource.Count;
						if (flag6)
						{
							this.selectedIndex++;
						}
						goto IL_01A8;
					}
					case KeyCode.Home:
						this.selectedIndex = 0;
						goto IL_01A8;
					case KeyCode.End:
						this.selectedIndex = this.itemsSource.Count - 1;
						goto IL_01A8;
					case KeyCode.PageUp:
						this.selectedIndex = Math.Max(0, this.selectedIndex - (int)(this.m_LastHeight / (float)this.itemHeight));
						goto IL_01A8;
					case KeyCode.PageDown:
						this.selectedIndex = Math.Min(this.itemsSource.Count - 1, this.selectedIndex + (int)(this.m_LastHeight / (float)this.itemHeight));
						goto IL_01A8;
					}
				}
				flag2 = false;
				flag3 = false;
				IL_01A8:
				bool flag7 = flag2;
				if (flag7)
				{
					evt.StopPropagation();
				}
				bool flag8 = flag3;
				if (flag8)
				{
					this.ScrollToItem(this.selectedIndex);
				}
			}
		}

		public void ScrollToItem(int index)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (flag)
			{
				throw new InvalidOperationException("Can't scroll without valid source, bind method, or factory method.");
			}
			bool flag2 = this.m_VisibleItemCount == 0 || index < -1;
			if (!flag2)
			{
				bool flag3 = index == -1;
				if (flag3)
				{
					int num = (int)(this.m_LastHeight / (float)this.itemHeight);
					bool flag4 = this.itemsSource.Count < num;
					if (flag4)
					{
						this.m_ScrollView.scrollOffset = new Vector2(0f, 0f);
					}
					else
					{
						this.m_ScrollView.scrollOffset = new Vector2(0f, (float)(this.itemsSource.Count * this.itemHeight));
					}
				}
				else
				{
					bool flag5 = this.m_FirstVisibleIndex > index;
					if (flag5)
					{
						this.m_ScrollView.scrollOffset = Vector2.up * (float)this.itemHeight * (float)index;
					}
					else
					{
						int num2 = (int)(this.m_LastHeight / (float)this.itemHeight);
						bool flag6 = index < this.m_FirstVisibleIndex + num2;
						if (!flag6)
						{
							bool flag7 = (int)this.m_LastHeight % this.itemHeight != 0;
							int num3 = index - num2;
							bool flag8 = flag7;
							if (flag8)
							{
								num3++;
							}
							this.m_ScrollView.scrollOffset = Vector2.up * (float)this.itemHeight * (float)num3;
						}
					}
				}
			}
		}

		private void OnClick(MouseDownEvent evt)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = evt.button != 0;
				if (!flag2)
				{
					int num = (int)(evt.localMousePosition.y / (float)this.itemHeight);
					bool flag3 = num > this.m_ItemsSource.Count - 1;
					if (!flag3)
					{
						object obj = this.m_ItemsSource[num];
						int idFromIndex = this.GetIdFromIndex(num);
						int clickCount = evt.clickCount;
						if (clickCount != 1)
						{
							if (clickCount == 2)
							{
								bool flag4 = this.onItemChosen == null;
								if (!flag4)
								{
									this.onItemChosen(obj);
								}
							}
						}
						else
						{
							bool flag5 = this.selectionType == SelectionType.None;
							if (!flag5)
							{
								bool flag6 = this.selectionType == SelectionType.Multiple && evt.actionKey;
								if (flag6)
								{
									this.m_RangeSelectionOrigin = num;
									bool flag7 = this.m_SelectedIds.Contains(idFromIndex);
									if (flag7)
									{
										this.RemoveFromSelection(num);
									}
									else
									{
										this.AddToSelection(num);
									}
								}
								else
								{
									bool flag8 = this.selectionType == SelectionType.Multiple && evt.shiftKey;
									if (flag8)
									{
										bool flag9 = this.m_RangeSelectionOrigin == -1;
										if (flag9)
										{
											this.m_RangeSelectionOrigin = num;
											this.SetSelection(num);
										}
										else
										{
											foreach (ListView.RecycledItem recycledItem in this.m_Pool)
											{
												recycledItem.SetSelected(false);
											}
											this.m_SelectedIds.Clear();
											this.m_SelectedIndices.Clear();
											this.m_SelectedItems.Clear();
											bool flag10 = num < this.m_RangeSelectionOrigin;
											if (flag10)
											{
												for (int i = num; i <= this.m_RangeSelectionOrigin; i++)
												{
													this.AddToSelection(i);
												}
											}
											else
											{
												for (int j = this.m_RangeSelectionOrigin; j <= num; j++)
												{
													this.AddToSelection(j);
												}
											}
										}
									}
									else
									{
										this.m_RangeSelectionOrigin = num;
										this.SetSelection(num);
									}
								}
							}
						}
					}
				}
			}
		}

		internal void SelectAll()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = this.selectionType != SelectionType.Multiple;
				if (!flag2)
				{
					for (int i = 0; i < this.itemsSource.Count; i++)
					{
						int idFromIndex = this.GetIdFromIndex(i);
						object obj = this.m_ItemsSource[i];
						foreach (ListView.RecycledItem recycledItem in this.m_Pool)
						{
							bool flag3 = recycledItem.id == idFromIndex;
							if (flag3)
							{
								recycledItem.SetSelected(true);
							}
						}
						bool flag4 = !this.m_SelectedIds.Contains(idFromIndex);
						if (flag4)
						{
							this.m_SelectedIds.Add(idFromIndex);
							this.m_SelectedIndices.Add(i);
							this.m_SelectedItems.Add(obj);
						}
					}
					this.NotifyOfSelectionChange();
					base.SaveViewData();
				}
			}
		}

		private int GetIdFromIndex(int index)
		{
			bool flag = this.m_GetItemId == null;
			int num;
			if (flag)
			{
				num = index;
			}
			else
			{
				num = this.m_GetItemId(index);
			}
			return num;
		}

		protected void AddToSelection(int index)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				int idFromIndex = this.GetIdFromIndex(index);
				object obj = this.m_ItemsSource[index];
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					bool flag2 = recycledItem.id == idFromIndex;
					if (flag2)
					{
						recycledItem.SetSelected(true);
					}
				}
				bool flag3 = !this.m_SelectedIds.Contains(idFromIndex);
				if (flag3)
				{
					this.m_SelectedIds.Add(idFromIndex);
					this.m_SelectedIndices.Add(index);
					this.m_SelectedItems.Add(obj);
				}
				this.NotifyOfSelectionChange();
				base.SaveViewData();
			}
		}

		protected void RemoveFromSelection(int index)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				int idFromIndex = this.GetIdFromIndex(index);
				object obj = this.m_ItemsSource[index];
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					bool flag2 = recycledItem.id == idFromIndex;
					if (flag2)
					{
						recycledItem.SetSelected(false);
					}
				}
				bool flag3 = this.m_SelectedIds.Contains(idFromIndex);
				if (flag3)
				{
					this.m_SelectedIds.Remove(idFromIndex);
					this.m_SelectedIndices.Remove(index);
					this.m_SelectedItems.Remove(obj);
				}
				this.NotifyOfSelectionChange();
				base.SaveViewData();
			}
		}

		protected void SetSelection(int index)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = index < 0;
				if (flag2)
				{
					this.ClearSelection();
				}
				else
				{
					int idFromIndex = this.GetIdFromIndex(index);
					object obj = this.m_ItemsSource[index];
					foreach (ListView.RecycledItem recycledItem in this.m_Pool)
					{
						recycledItem.SetSelected(recycledItem.id == idFromIndex);
					}
					this.m_SelectedIds.Clear();
					this.m_SelectedIndices.Clear();
					this.m_SelectedItems.Clear();
					this.m_SelectedIds.Add(idFromIndex);
					this.m_SelectedIndices.Add(index);
					this.m_SelectedItems.Add(obj);
					this.NotifyOfSelectionChange();
					base.SaveViewData();
				}
			}
		}

		private void NotifyOfSelectionChange()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = this.onSelectionChanged == null;
				if (!flag2)
				{
					this.onSelectionChanged(this.m_SelectedItems);
				}
			}
		}

		protected void ClearSelection()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					recycledItem.SetSelected(false);
				}
				this.m_SelectedIds.Clear();
				this.m_SelectedIndices.Clear();
				this.m_SelectedItems.Clear();
				this.NotifyOfSelectionChange();
			}
		}

		public void ScrollTo(VisualElement visualElement)
		{
			this.m_ScrollView.ScrollTo(visualElement);
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
		}

		private void OnScroll(float offset)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				this.m_ScrollOffset = offset;
				int num = (int)(offset / (float)this.itemHeight);
				this.m_ScrollView.contentContainer.style.height = (float)(this.itemsSource.Count * this.itemHeight);
				bool flag2 = num != this.m_FirstVisibleIndex;
				if (flag2)
				{
					this.m_FirstVisibleIndex = num;
					bool flag3 = this.m_Pool.Count > 0;
					if (flag3)
					{
						bool flag4 = this.m_FirstVisibleIndex < this.m_Pool[0].index;
						if (flag4)
						{
							int num2 = this.m_Pool[0].index - this.m_FirstVisibleIndex;
							List<ListView.RecycledItem> scrollInsertionList = this.m_ScrollInsertionList;
							int num3 = 0;
							while (num3 < num2 && this.m_Pool.Count > 0)
							{
								ListView.RecycledItem recycledItem = this.m_Pool[this.m_Pool.Count - 1];
								scrollInsertionList.Add(recycledItem);
								this.m_Pool.RemoveAt(this.m_Pool.Count - 1);
								recycledItem.element.SendToBack();
								num3++;
							}
							this.m_ScrollInsertionList = this.m_Pool;
							this.m_Pool = scrollInsertionList;
							this.m_Pool.AddRange(this.m_ScrollInsertionList);
							this.m_ScrollInsertionList.Clear();
						}
						else
						{
							bool flag5 = this.m_FirstVisibleIndex < this.m_Pool[this.m_Pool.Count - 1].index;
							if (flag5)
							{
								List<ListView.RecycledItem> scrollInsertionList2 = this.m_ScrollInsertionList;
								int num4 = 0;
								while (this.m_FirstVisibleIndex > this.m_Pool[num4].index)
								{
									ListView.RecycledItem recycledItem2 = this.m_Pool[num4];
									scrollInsertionList2.Add(recycledItem2);
									num4++;
									recycledItem2.element.BringToFront();
								}
								this.m_Pool.RemoveRange(0, num4);
								this.m_Pool.AddRange(scrollInsertionList2);
								scrollInsertionList2.Clear();
							}
						}
						int num5 = 0;
						while (num5 < this.m_Pool.Count && num5 + this.m_FirstVisibleIndex < this.itemsSource.Count)
						{
							this.Setup(this.m_Pool[num5], num5 + this.m_FirstVisibleIndex);
							num5++;
						}
					}
				}
			}
		}

		private bool HasValidDataAndBindings()
		{
			return this.itemsSource != null && this.makeItem != null && this.bindItem != null;
		}

		public void Refresh()
		{
			foreach (ListView.RecycledItem recycledItem in this.m_Pool)
			{
				recycledItem.DetachElement();
			}
			this.m_Pool.Clear();
			this.m_ScrollView.Clear();
			this.m_SelectedIndices.Clear();
			this.m_SelectedItems.Clear();
			this.m_VisibleItemCount = 0;
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				this.m_LastHeight = this.m_ScrollView.layout.height;
				bool flag2 = float.IsNaN(this.m_LastHeight);
				if (!flag2)
				{
					this.ResizeHeight(this.m_LastHeight);
				}
			}
		}

		private void ResizeHeight(float height)
		{
			int num = this.itemsSource.Count * this.itemHeight;
			this.m_ScrollView.contentContainer.style.height = (float)num;
			float num2 = Mathf.Max(0f, (float)num - this.m_ScrollView.contentViewport.layout.height);
			this.m_ScrollView.verticalScroller.highValue = Mathf.Min(Mathf.Max(this.m_ScrollOffset, this.m_ScrollView.verticalScroller.highValue), num2);
			this.m_ScrollView.verticalScroller.value = Mathf.Min(this.m_ScrollOffset, this.m_ScrollView.verticalScroller.highValue);
			int num3 = Math.Min((int)(height / (float)this.itemHeight) + 2, this.itemsSource.Count);
			bool flag = this.m_VisibleItemCount != num3;
			if (flag)
			{
				bool flag2 = this.m_VisibleItemCount > num3;
				if (flag2)
				{
					int num4 = this.m_VisibleItemCount - num3;
					for (int i = 0; i < num4; i++)
					{
						int num5 = this.m_Pool.Count - 1;
						ListView.RecycledItem recycledItem = this.m_Pool[num5];
						recycledItem.element.RemoveFromHierarchy();
						recycledItem.DetachElement();
						this.m_Pool.RemoveAt(num5);
					}
				}
				else
				{
					int num6 = num3 - this.m_VisibleItemCount;
					for (int j = 0; j < num6; j++)
					{
						int num7 = j + this.m_FirstVisibleIndex + this.m_VisibleItemCount;
						VisualElement visualElement = this.makeItem();
						ListView.RecycledItem recycledItem2 = new ListView.RecycledItem(visualElement);
						this.m_Pool.Add(recycledItem2);
						visualElement.AddToClassList("unity-listview-item");
						visualElement.style.marginTop = 0f;
						visualElement.style.marginBottom = 0f;
						visualElement.style.position = Position.Absolute;
						visualElement.style.left = 0f;
						visualElement.style.right = 0f;
						visualElement.style.height = (float)this.itemHeight;
						bool flag3 = num7 < this.itemsSource.Count;
						if (flag3)
						{
							this.Setup(recycledItem2, num7);
						}
						else
						{
							visualElement.style.visibility = Visibility.Hidden;
						}
						base.Add(visualElement);
					}
				}
				this.m_VisibleItemCount = num3;
			}
			this.m_LastHeight = height;
			bool flag4 = this.m_SelectedIds.Count > 0;
			if (flag4)
			{
				for (int k = 0; k < this.m_ItemsSource.Count; k++)
				{
					bool flag5 = this.m_SelectedIds.Contains(this.GetIdFromIndex(k));
					if (flag5)
					{
						this.m_SelectedIndices.Add(k);
						this.m_SelectedItems.Add(this.m_ItemsSource[k]);
					}
				}
			}
		}

		private void Setup(ListView.RecycledItem recycledItem, int newIndex)
		{
			int idFromIndex = this.GetIdFromIndex(newIndex);
			recycledItem.element.style.visibility = Visibility.Visible;
			bool flag = recycledItem.index != newIndex;
			if (flag)
			{
				recycledItem.index = newIndex;
				recycledItem.id = idFromIndex;
				recycledItem.element.style.top = (float)(recycledItem.index * this.itemHeight);
				recycledItem.element.style.bottom = (float)((this.itemsSource.Count - recycledItem.index - 1) * this.itemHeight);
				this.bindItem(recycledItem.element, recycledItem.index);
				recycledItem.SetSelected(this.m_SelectedIds.Contains(idFromIndex));
			}
		}

		private void OnSizeChanged(GeometryChangedEvent evt)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = evt.newRect.height == evt.oldRect.height;
				if (!flag2)
				{
					this.ResizeHeight(evt.newRect.height);
				}
			}
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			int num = 0;
			bool flag = !this.m_ItemHeightIsInline && e.customStyle.TryGetValue(ListView.s_ItemHeightProperty, out num);
			if (flag)
			{
				this.itemHeight = num;
			}
		}

		private IList m_ItemsSource;

		private Func<VisualElement> m_MakeItem;

		private Action<VisualElement, int> m_BindItem;

		private Func<int, int> m_GetItemId;

		[SerializeField]
		internal int m_ItemHeight = ListView.s_DefaultItemHeight;

		[SerializeField]
		internal bool m_ItemHeightIsInline;

		[SerializeField]
		private float m_ScrollOffset;

		[SerializeField]
		private List<int> m_SelectedIds = new List<int>();

		private List<int> m_SelectedIndices = new List<int>();

		private List<object> m_SelectedItems = new List<object>();

		private int m_RangeSelectionOrigin = -1;

		internal static readonly int s_DefaultItemHeight = 30;

		internal static CustomStyleProperty<int> s_ItemHeightProperty = new CustomStyleProperty<int>("--unity-item-height");

		private int m_FirstVisibleIndex;

		private float m_LastHeight;

		private List<ListView.RecycledItem> m_Pool = new List<ListView.RecycledItem>();

		private ScrollView m_ScrollView;

		private List<ListView.RecycledItem> m_ScrollInsertionList = new List<ListView.RecycledItem>();

		private const int k_ExtraVisibleItems = 2;

		private int m_VisibleItemCount;

		public static readonly string ussClassName = "unity-list-view";

		public static readonly string itemUssClassName = ListView.ussClassName + "__item";

		public static readonly string itemSelectedVariantUssClassName = ListView.itemUssClassName + "--selected";

		public new class UxmlFactory : UxmlFactory<ListView, ListView.UxmlTraits>
		{
		}

		public new class UxmlTraits : BindableElement.UxmlTraits
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
				int num = 0;
				bool flag = this.m_ItemHeight.TryGetValueFromBag(bag, cc, ref num);
				if (flag)
				{
					((ListView)ve).itemHeight = num;
				}
			}

			private UxmlIntAttributeDescription m_ItemHeight = new UxmlIntAttributeDescription
			{
				name = "item-height",
				obsoleteNames = new string[] { "itemHeight" },
				defaultValue = ListView.s_DefaultItemHeight
			};
		}

		private class RecycledItem
		{
			public VisualElement element { get; private set; }

			public RecycledItem(VisualElement element)
			{
				this.element = element;
				this.index = (this.id = -1);
				element.AddToClassList(ListView.itemUssClassName);
			}

			public void DetachElement()
			{
				this.element.RemoveFromClassList(ListView.itemUssClassName);
				this.element = null;
			}

			public void SetSelected(bool selected)
			{
				bool flag = this.element != null;
				if (flag)
				{
					if (selected)
					{
						this.element.AddToClassList(ListView.itemSelectedVariantUssClassName);
						this.element.pseudoStates |= PseudoStates.Checked;
					}
					else
					{
						this.element.RemoveFromClassList(ListView.itemSelectedVariantUssClassName);
						this.element.pseudoStates &= ~PseudoStates.Checked;
					}
				}
			}

			public int index;

			public int id;
		}
	}
}
