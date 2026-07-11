using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>A vertically scrollable area that only creates visual elements for visible items while allowing the binding of many more items. As the user scrolls, visual elements are recycled and re-bound to new data items.</para>
	/// </summary>
	public class ListView : VisualElement
	{
		public ListView()
		{
			this.selectionType = SelectionType.Single;
			this.m_ScrollOffset = 0f;
			this.m_ScrollView = new ScrollView();
			this.m_ScrollView.StretchToParentSize();
			this.m_ScrollView.verticalScroller.valueChanged += this.OnScroll;
			base.shadow.Add(this.m_ScrollView);
			base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChanged), Capture.NoCapture);
			this.m_ScrollView.contentContainer.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnClick), Capture.NoCapture);
			this.m_ScrollView.contentContainer.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), Capture.NoCapture);
			this.m_ScrollView.contentContainer.focusIndex = 0;
			base.schedule.Execute(delegate
			{
				base.Dirty(ChangeType.Layout);
				this.m_ScrollView.Focus();
			}).StartingIn(1L);
		}

		public ListView(IList itemsSource, int itemHeight, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
			: this()
		{
			this.m_ItemsSource = itemsSource;
			this.m_ItemHeight = itemHeight;
			this.m_MakeItem = makeItem;
			this.m_BindItem = bindItem;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<object> onItemChosen;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<List<object>> onSelectionChanged;

		/// <summary>
		///   <para>The items data source. This property must be set for the list view to function.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Callback for constructing the VisualElement that will serve as the template for each recycled and re-bound element in the list. This property must be set for the list view to function.</para>
		/// </summary>
		public Func<VisualElement> makeItem
		{
			get
			{
				return this.m_MakeItem;
			}
			set
			{
				this.m_MakeItem = value;
				this.Refresh();
			}
		}

		/// <summary>
		///   <para>Callback for binding a data item to the visual element.</para>
		/// </summary>
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

		/// <summary>
		///   <para>ListView requires all visual elements to have the same height so that it can calculate a sensible scroller size. This property must be set for the list view to function.</para>
		/// </summary>
		public int itemHeight
		{
			get
			{
				return this.m_ItemHeight.GetSpecifiedValueOrDefault(30);
			}
			set
			{
				this.m_ItemHeight = value;
				this.Refresh();
			}
		}

		/// <summary>
		///   <para>Currently selected item index in the items source. If multiple items are selected, this will return the first selected item's index.</para>
		/// </summary>
		public int selectedIndex
		{
			get
			{
				return (this.m_SelectedIndices.Count != 0) ? this.m_SelectedIndices.First<int>() : (-1);
			}
			set
			{
				this.SetSelection(value);
			}
		}

		/// <summary>
		///   <para>The currently selected item from the items source. If multiple items are selected, this will return the first selected item.</para>
		/// </summary>
		public object selectedItem
		{
			get
			{
				return (this.m_ItemsSource != null) ? this.m_ItemsSource[this.selectedIndex] : null;
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ScrollView.contentContainer;
			}
		}

		/// <summary>
		///   <para>Controls the selection state, whether: selections are disabled, there is only one selectable item, or if there are multiple selectable items.</para>
		/// </summary>
		public SelectionType selectionType { get; set; }

		public void OnKeyDown(KeyDownEvent evt)
		{
			if (this.HasValidDataAndBindings())
			{
				switch (evt.keyCode)
				{
				case KeyCode.UpArrow:
					if (this.selectedIndex > 0)
					{
						this.selectedIndex--;
					}
					break;
				case KeyCode.DownArrow:
					if (this.selectedIndex + 1 < this.itemsSource.Count)
					{
						this.selectedIndex++;
					}
					break;
				case KeyCode.Home:
					this.selectedIndex = 0;
					break;
				case KeyCode.End:
					this.selectedIndex = this.itemsSource.Count - 1;
					break;
				case KeyCode.PageUp:
					this.selectedIndex = Math.Max(0, this.selectedIndex - (int)(this.m_LastSize.height / (float)this.itemHeight));
					break;
				case KeyCode.PageDown:
					this.selectedIndex = Math.Min(this.itemsSource.Count - 1, this.selectedIndex + (int)(this.m_LastSize.height / (float)this.itemHeight));
					break;
				}
				this.ScrollToItem(this.selectedIndex);
			}
		}

		/// <summary>
		///   <para>Scroll so that a specific item index from the items source is visible.</para>
		/// </summary>
		/// <param name="index">Item index to scroll to.</param>
		public void ScrollToItem(int index)
		{
			if (!this.HasValidDataAndBindings())
			{
				throw new InvalidOperationException("Can't scroll without valid source, bind method, or factory method.");
			}
			if (this.m_VisibleItemCount != 0)
			{
				if (this.m_FirstVisibleIndex > index)
				{
					this.m_ScrollView.scrollOffset = Vector2.up * (float)this.itemHeight * (float)index;
				}
				else
				{
					int num = (int)(this.m_LastSize.height / (float)this.itemHeight);
					if (index >= this.m_FirstVisibleIndex + num)
					{
						this.m_ScrollView.scrollOffset = Vector2.up * (float)this.itemHeight * (float)(index - num);
					}
				}
			}
		}

		private void OnClick(MouseDownEvent evt)
		{
			if (this.HasValidDataAndBindings())
			{
				if (evt.button == 0)
				{
					int num = (int)(evt.localMousePosition.y / (float)this.itemHeight);
					int clickCount = evt.clickCount;
					if (clickCount != 1)
					{
						if (clickCount == 2)
						{
							if (this.onItemChosen != null)
							{
								this.onItemChosen(this.itemsSource[num]);
							}
						}
					}
					else if (this.selectionType != SelectionType.None)
					{
						if (this.selectionType == SelectionType.Multiple && evt.ctrlKey)
						{
							if (this.m_SelectedIndices.Contains(num))
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
							this.SetSelection(num);
						}
					}
				}
			}
		}

		protected void AddToSelection(int index)
		{
			if (this.HasValidDataAndBindings())
			{
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					if (recycledItem.index == index)
					{
						recycledItem.SetSelected(true);
					}
				}
				if (!this.m_SelectedIndices.Contains(index))
				{
					this.m_SelectedIndices.Add(index);
				}
				this.SelectionChanged();
				base.SavePersistentData();
			}
		}

		protected void RemoveFromSelection(int index)
		{
			if (this.HasValidDataAndBindings())
			{
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					if (recycledItem.index == index)
					{
						recycledItem.SetSelected(false);
					}
				}
				if (this.m_SelectedIndices.Contains(index))
				{
					this.m_SelectedIndices.Remove(index);
				}
				this.SelectionChanged();
				base.SavePersistentData();
			}
		}

		protected void SetSelection(int index)
		{
			if (this.HasValidDataAndBindings())
			{
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					recycledItem.SetSelected(recycledItem.index == index);
				}
				this.m_SelectedIndices.Clear();
				if (index >= 0)
				{
					this.m_SelectedIndices.Add(index);
				}
				this.SelectionChanged();
				base.SavePersistentData();
			}
		}

		private void SelectionChanged()
		{
			if (this.HasValidDataAndBindings())
			{
				if (this.onSelectionChanged != null)
				{
					List<object> list = new List<object>();
					foreach (int num in this.m_SelectedIndices)
					{
						list.Add(this.itemsSource[num]);
					}
					this.onSelectionChanged(list);
				}
			}
		}

		protected void ClearSelection()
		{
			if (this.HasValidDataAndBindings())
			{
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					recycledItem.SetSelected(false);
				}
				this.m_SelectedIndices.Clear();
				this.SelectionChanged();
			}
		}

		/// <summary>
		///   <para>Scroll to a specific visual element.</para>
		/// </summary>
		/// <param name="visualElement">Element to scroll to.</param>
		public void ScrollTo(VisualElement visualElement)
		{
			this.m_ScrollView.ScrollTo(visualElement);
		}

		public override void OnPersistentDataReady()
		{
			base.OnPersistentDataReady();
			string fullHierarchicalPersistenceKey = base.GetFullHierarchicalPersistenceKey();
			base.OverwriteFromPersistedData(this, fullHierarchicalPersistenceKey);
		}

		private void OnScroll(float offset)
		{
			if (this.HasValidDataAndBindings())
			{
				this.m_ScrollOffset = offset;
				this.m_FirstVisibleIndex = (int)(offset / (float)this.itemHeight);
				this.m_ScrollView.contentContainer.style.height = (float)(this.itemsSource.Count * this.itemHeight);
				int num = 0;
				while (num < this.m_Pool.Count && num + this.m_FirstVisibleIndex < this.itemsSource.Count)
				{
					this.Setup(this.m_Pool[num], num + this.m_FirstVisibleIndex);
					num++;
				}
			}
		}

		private bool HasValidDataAndBindings()
		{
			return this.itemsSource != null && this.makeItem != null && this.bindItem != null;
		}

		/// <summary>
		///   <para>Clear, recreate all visible visual elements, and rebind all items. This should be called whenever the items source changes.</para>
		/// </summary>
		public void Refresh()
		{
			this.m_Pool.Clear();
			this.m_ScrollView.Clear();
			this.m_ScrollView.contentContainer.style.width = this.m_ScrollView.contentViewport.layout.width;
			this.m_ScrollView.contentContainer.style.flex = 0f;
			if (this.HasValidDataAndBindings())
			{
				this.m_ScrollView.contentContainer.style.height = (float)(this.itemsSource.Count * this.itemHeight);
				this.m_ScrollView.verticalScroller.highValue = Mathf.Max(this.m_ScrollOffset, this.m_ScrollView.verticalScroller.highValue);
				this.m_ScrollView.verticalScroller.value = this.m_ScrollOffset;
				if (this.m_LastSize != this.m_ScrollView.layout)
				{
					this.m_LastSize = this.m_ScrollView.layout;
				}
				if (!float.IsNaN(this.m_LastSize.height))
				{
					this.m_ScrollView.contentContainer.style.height = (float)(this.itemsSource.Count * this.itemHeight);
					this.m_VisibleItemCount = (int)(this.m_LastSize.height / (float)this.itemHeight) + 2;
					for (int i = this.m_FirstVisibleIndex; i < this.m_VisibleItemCount + this.m_FirstVisibleIndex; i++)
					{
						VisualElement visualElement = this.makeItem();
						ListView.RecycledItem recycledItem = new ListView.RecycledItem(visualElement);
						this.m_Pool.Add(recycledItem);
						visualElement.style.marginTop = 0f;
						visualElement.style.marginBottom = 0f;
						visualElement.style.positionType = PositionType.Absolute;
						visualElement.style.positionLeft = 0f;
						visualElement.style.positionRight = 0f;
						visualElement.style.height = (float)this.itemHeight;
						if (i < this.itemsSource.Count)
						{
							visualElement.style.visibility = Visibility.Visible;
							this.Setup(recycledItem, i);
						}
						else
						{
							visualElement.style.visibility = Visibility.Hidden;
						}
						this.m_ScrollView.Add(visualElement);
					}
					base.schedule.Execute(delegate
					{
						base.Dirty(ChangeType.Layout);
					});
				}
			}
		}

		private void Setup(ListView.RecycledItem recycledItem, int newIndex)
		{
			Assert.IsTrue(newIndex < this.itemsSource.Count);
			recycledItem.index = newIndex;
			recycledItem.element.style.positionTop = (float)(recycledItem.index * this.itemHeight);
			recycledItem.element.style.positionBottom = (float)((this.itemsSource.Count - recycledItem.index - 1) * this.itemHeight);
			this.bindItem(recycledItem.element, recycledItem.index);
			recycledItem.SetSelected(this.m_SelectedIndices.Contains(newIndex));
		}

		private void OnSizeChanged(GeometryChangedEvent evt)
		{
			if (this.HasValidDataAndBindings())
			{
				this.m_ScrollView.contentContainer.style.height = (float)(this.itemsSource.Count * this.itemHeight);
				if (!(this.m_LastSize == this.m_ScrollView.layout))
				{
					this.Refresh();
				}
			}
		}

		protected override void OnStyleResolved(ICustomStyle styles)
		{
			base.OnStyleResolved(styles);
			styles.ApplyCustomProperty("-unity-item-height", ref this.m_ItemHeight);
			this.Refresh();
		}

		private IList m_ItemsSource;

		private Func<VisualElement> m_MakeItem;

		private Action<VisualElement, int> m_BindItem;

		private StyleValue<int> m_ItemHeight;

		[SerializeField]
		private float m_ScrollOffset;

		[SerializeField]
		private List<int> m_SelectedIndices = new List<int>();

		private const int k_DefaultItemHeight = 30;

		private const string k_ItemHeightProperty = "-unity-item-height";

		private int m_FirstVisibleIndex;

		private Rect m_LastSize;

		private List<ListView.RecycledItem> m_Pool = new List<ListView.RecycledItem>();

		private ScrollView m_ScrollView;

		private const int m_ExtraVisibleItems = 2;

		private int m_VisibleItemCount;

		/// <summary>
		///   <para>Instantiates a ListView using the data read from a UXML file.</para>
		/// </summary>
		public class ListViewFactory : UxmlFactory<ListView, ListView.ListViewUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the ListView.</para>
		/// </summary>
		public class ListViewUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public ListViewUxmlTraits()
			{
				this.m_ItemHeight = new UxmlIntAttributeDescription
				{
					name = "itemHeight",
					defaultValue = 30
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for ListView properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_ItemHeight;
					yield break;
				}
			}

			/// <summary>
			///   <para>Returns an empty enumerable, as list views generally do not have children.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize ListView properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				((ListView)ve).itemHeight = this.m_ItemHeight.GetValueFromBag(bag);
			}

			private UxmlIntAttributeDescription m_ItemHeight;
		}

		private class RecycledItem
		{
			public RecycledItem(VisualElement element)
			{
				this.element = element;
			}

			internal void SetSelected(bool selected)
			{
				if (selected)
				{
					this.element.pseudoStates |= PseudoStates.Selected;
				}
				else
				{
					this.element.pseudoStates &= ~PseudoStates.Selected;
				}
			}

			public readonly VisualElement element;

			public int index;
		}
	}
}
