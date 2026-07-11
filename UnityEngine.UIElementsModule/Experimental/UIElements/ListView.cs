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
	public class ListView : VisualElement
	{
		public ListView()
		{
			this.selectionType = SelectionType.Single;
			this.m_ScrollOffset = 0f;
			this.m_ScrollView = new ScrollView();
			this.m_ScrollView.StretchToParentSize();
			this.m_ScrollView.stretchContentWidth = true;
			this.m_ScrollView.verticalScroller.valueChanged += this.OnScroll;
			base.shadow.Add(this.m_ScrollView);
			base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChanged), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnClick), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.focusIndex = 0;
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
				if (!(this.m_MakeItem == value))
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
					this.selectedIndex = Math.Max(0, this.selectedIndex - (int)(this.m_LastHeight / (float)this.itemHeight));
					break;
				case KeyCode.PageDown:
					this.selectedIndex = Math.Min(this.itemsSource.Count - 1, this.selectedIndex + (int)(this.m_LastHeight / (float)this.itemHeight));
					break;
				}
				this.ScrollToItem(this.selectedIndex);
			}
		}

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
					int num = (int)(this.m_LastHeight / (float)this.itemHeight);
					if (index >= this.m_FirstVisibleIndex + num)
					{
						bool flag = (int)this.m_LastHeight % this.itemHeight != 0;
						int num2 = index - num;
						if (flag)
						{
							num2++;
						}
						this.m_ScrollView.scrollOffset = Vector2.up * (float)this.itemHeight * (float)num2;
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
					if (num <= this.itemsSource.Count - 1)
					{
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
							if (this.selectionType == SelectionType.Multiple && evt.actionKey)
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

		public void Refresh()
		{
			this.m_Pool.Clear();
			this.m_ScrollView.Clear();
			this.m_VisibleItemCount = 0;
			if (this.HasValidDataAndBindings())
			{
				this.m_LastHeight = this.m_ScrollView.layout.height;
				if (!float.IsNaN(this.m_LastHeight))
				{
					this.ResizeHeight(this.m_LastHeight);
				}
			}
		}

		private void ResizeHeight(float height)
		{
			this.m_ScrollView.contentContainer.style.height = (float)(this.itemsSource.Count * this.itemHeight);
			this.m_ScrollView.verticalScroller.highValue = Mathf.Max(this.m_ScrollOffset, this.m_ScrollView.verticalScroller.highValue);
			this.m_ScrollView.verticalScroller.value = this.m_ScrollOffset;
			int num = Math.Min((int)(height / (float)this.itemHeight) + 2, this.itemsSource.Count);
			if (this.m_VisibleItemCount != num)
			{
				if (this.m_VisibleItemCount > num)
				{
					int num2 = this.m_VisibleItemCount - num;
					for (int i = 0; i < num2; i++)
					{
						this.m_Pool.RemoveAt(this.m_Pool.Count - 1);
						this.m_ScrollView.RemoveAt(this.m_ScrollView.childCount - 1);
					}
				}
				else
				{
					int num3 = num - this.m_VisibleItemCount;
					for (int j = 0; j < num3; j++)
					{
						int num4 = j + this.m_FirstVisibleIndex + this.m_VisibleItemCount;
						VisualElement visualElement = this.makeItem();
						ListView.RecycledItem recycledItem = new ListView.RecycledItem(visualElement);
						this.m_Pool.Add(recycledItem);
						visualElement.style.marginTop = 0f;
						visualElement.style.marginBottom = 0f;
						visualElement.style.positionType = PositionType.Absolute;
						visualElement.style.positionLeft = 0f;
						visualElement.style.positionRight = 0f;
						visualElement.style.height = (float)this.itemHeight;
						if (num4 < this.itemsSource.Count)
						{
							this.Setup(recycledItem, num4);
						}
						else
						{
							visualElement.style.visibility = Visibility.Hidden;
						}
						this.m_ScrollView.Add(visualElement);
					}
				}
				this.m_VisibleItemCount = num;
			}
			this.m_LastHeight = height;
		}

		private void Setup(ListView.RecycledItem recycledItem, int newIndex)
		{
			Assert.IsTrue(newIndex < this.itemsSource.Count);
			recycledItem.element.style.visibility = Visibility.Visible;
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
				if (evt.newRect.height != evt.oldRect.height)
				{
					this.ResizeHeight(evt.newRect.height);
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

		private float m_LastHeight;

		private List<ListView.RecycledItem> m_Pool = new List<ListView.RecycledItem>();

		private ScrollView m_ScrollView;

		private const int m_ExtraVisibleItems = 2;

		private int m_VisibleItemCount;

		public new class UxmlFactory : UxmlFactory<ListView, ListView.UxmlTraits>
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
				((ListView)ve).itemHeight = this.m_ItemHeight.GetValueFromBag(bag, cc);
			}

			private UxmlIntAttributeDescription m_ItemHeight = new UxmlIntAttributeDescription
			{
				name = "item-height",
				obsoleteNames = new string[] { "itemHeight" },
				defaultValue = 30
			};
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
