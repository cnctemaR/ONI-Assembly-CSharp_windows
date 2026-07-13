using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Unity.Hierarchy;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements
{
	public abstract class CollectionViewController : IDisposable
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action itemsSourceChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, int> itemIndexChanged;

		public virtual IList itemsSource
		{
			get
			{
				return this.m_ItemsSource;
			}
			set
			{
				bool flag = this.m_ItemsSource == value;
				if (!flag)
				{
					this.m_ItemsSource = value;
					bool flag2 = this.m_View.GetProperty("__unity-collection-view-internal-binding") == null;
					if (flag2)
					{
						this.m_View.RefreshItems();
					}
					this.RaiseItemsSourceChanged();
				}
			}
		}

		protected void SetItemsSourceWithoutNotify(IList source)
		{
			this.m_ItemsSource = source;
		}

		private protected void SetHierarchyViewModelWithoutNotify(HierarchyViewModel source)
		{
			this.m_ItemsSource = new ReadOnlyHierarchyViewModelList(source);
		}

		protected BaseVerticalCollectionView view
		{
			get
			{
				return this.m_View;
			}
		}

		public void SetView(BaseVerticalCollectionView collectionView)
		{
			this.m_View = collectionView;
			this.PrepareView();
			Assert.IsNotNull<BaseVerticalCollectionView>(this.m_View, "View must not be null.");
		}

		protected virtual void PrepareView()
		{
		}

		public virtual void Dispose()
		{
			this.itemsSourceChanged = null;
			this.itemIndexChanged = null;
			this.m_View = null;
		}

		public virtual int GetItemsCount()
		{
			IList itemsSource = this.m_ItemsSource;
			return (itemsSource != null) ? itemsSource.Count : 0;
		}

		internal virtual int GetItemsMinCount()
		{
			return this.GetItemsCount();
		}

		public virtual int GetIndexForId(int id)
		{
			return id;
		}

		public virtual int GetIdForIndex(int index)
		{
			return index;
		}

		public virtual object GetItemForIndex(int index)
		{
			bool flag = this.m_ItemsSource == null;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				bool flag2 = index < 0 || index >= this.m_ItemsSource.Count;
				if (flag2)
				{
					obj = null;
				}
				else
				{
					obj = this.m_ItemsSource[index];
				}
			}
			return obj;
		}

		public virtual object GetItemForId(int id)
		{
			bool flag = this.m_ItemsSource == null;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				int indexForId = this.GetIndexForId(id);
				bool flag2 = indexForId < 0 || indexForId >= this.m_ItemsSource.Count;
				if (flag2)
				{
					obj = null;
				}
				else
				{
					obj = this.m_ItemsSource[indexForId];
				}
			}
			return obj;
		}

		internal virtual void InvokeMakeItem(ReusableCollectionItem reusableItem)
		{
			reusableItem.Init(this.MakeItem());
		}

		internal virtual void SetBindingContext(ReusableCollectionItem reusableItem, int index)
		{
		}

		internal virtual void InvokeBindItem(ReusableCollectionItem reusableItem, int index)
		{
			this.BindItem(reusableItem.bindableElement, index);
			this.SetBindingContext(reusableItem, index);
			reusableItem.SetSelected(this.m_View.selectedIndices.Contains(index));
			reusableItem.rootElement.pseudoStates &= ~PseudoStates.Hover;
			reusableItem.index = index;
		}

		internal virtual void InvokeUnbindItem(ReusableCollectionItem reusableItem, int index)
		{
			this.UnbindItem(reusableItem.bindableElement, index);
			reusableItem.index = -1;
		}

		internal virtual void InvokeDestroyItem(ReusableCollectionItem reusableItem)
		{
			this.DestroyItem(reusableItem.bindableElement);
		}

		internal virtual void PreRefresh()
		{
		}

		protected abstract VisualElement MakeItem();

		protected abstract void BindItem(VisualElement element, int index);

		protected abstract void UnbindItem(VisualElement element, int index);

		protected abstract void DestroyItem(VisualElement element);

		protected void RaiseItemsSourceChanged()
		{
			Action action = this.itemsSourceChanged;
			if (action != null)
			{
				action();
			}
		}

		protected void RaiseItemIndexChanged(int srcIndex, int dstIndex)
		{
			Action<int, int> action = this.itemIndexChanged;
			if (action != null)
			{
				action(srcIndex, dstIndex);
			}
		}

		private BaseVerticalCollectionView m_View;

		private IList m_ItemsSource;
	}
}
