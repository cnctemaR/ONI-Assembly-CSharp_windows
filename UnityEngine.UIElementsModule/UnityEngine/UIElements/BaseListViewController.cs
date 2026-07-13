using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	public abstract class BaseListViewController : CollectionViewController
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action itemsSourceSizeChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<int>> itemsAdded;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<int>> itemsRemoved;

		protected BaseListView baseListView
		{
			get
			{
				return base.view as BaseListView;
			}
		}

		internal override void InvokeMakeItem(ReusableCollectionItem reusableItem)
		{
			ReusableListViewItem reusableListViewItem = reusableItem as ReusableListViewItem;
			bool flag = reusableListViewItem != null;
			if (flag)
			{
				reusableListViewItem.Init(this.MakeItem(), this.baseListView.reorderable && this.baseListView.reorderMode == ListViewReorderMode.Animated);
				this.PostInitRegistration(reusableListViewItem);
			}
		}

		internal void PostInitRegistration(ReusableListViewItem listItem)
		{
			listItem.bindableElement.style.position = Position.Relative;
			listItem.bindableElement.style.flexBasis = StyleKeyword.Initial;
			listItem.bindableElement.style.marginTop = 0f;
			listItem.bindableElement.style.marginBottom = 0f;
			listItem.bindableElement.style.paddingTop = 0f;
			listItem.bindableElement.style.flexGrow = 0f;
			listItem.bindableElement.style.flexShrink = 0f;
		}

		internal override void SetBindingContext(ReusableCollectionItem reusableItem, int index)
		{
			base.SetBindingContext(reusableItem, index);
			bool autoAssignSource = this.baseListView.autoAssignSource;
			if (autoAssignSource)
			{
				reusableItem.rootElement.dataSource = this.itemsSource;
				reusableItem.rootElement.dataSourcePath = PropertyPath.FromIndex(index);
			}
		}

		internal override void InvokeBindItem(ReusableCollectionItem reusableItem, int index)
		{
			ReusableListViewItem reusableListViewItem = reusableItem as ReusableListViewItem;
			bool flag = reusableListViewItem != null;
			if (flag)
			{
				bool flag2 = this.baseListView.reorderable && this.baseListView.reorderMode == ListViewReorderMode.Animated;
				reusableListViewItem.UpdateDragHandle(flag2 && this.NeedsDragHandle(index));
			}
			base.InvokeBindItem(reusableItem, index);
		}

		public virtual bool NeedsDragHandle(int index)
		{
			return true;
		}

		public virtual void AddItems(int itemCount)
		{
			bool flag = itemCount <= 0;
			if (!flag)
			{
				this.EnsureItemSourceCanBeResized();
				int itemsCount = this.GetItemsCount();
				List<int> list = CollectionPool<List<int>, int>.Get();
				try
				{
					bool isFixedSize = this.itemsSource.IsFixedSize;
					if (isFixedSize)
					{
						this.itemsSource = BaseListViewController.AddToArray((Array)this.itemsSource, itemCount);
						for (int i = 0; i < itemCount; i++)
						{
							list.Add(itemsCount + i);
						}
					}
					else
					{
						Type type = this.itemsSource.GetType();
						Type type2 = type.GetInterfaces().FirstOrDefault<Type>(new Func<Type, bool>(BaseListViewController.<AddItems>g__IsGenericList|19_0));
						bool flag2 = type2 != null && type2.GetGenericArguments()[0].IsValueType;
						if (flag2)
						{
							Type type3 = type2.GetGenericArguments()[0];
							for (int j = 0; j < itemCount; j++)
							{
								list.Add(itemsCount + j);
								this.itemsSource.Add(Activator.CreateInstance(type3));
							}
						}
						else
						{
							for (int k = 0; k < itemCount; k++)
							{
								list.Add(itemsCount + k);
								this.itemsSource.Add(null);
							}
						}
					}
					this.RaiseItemsAdded(list);
				}
				finally
				{
					CollectionPool<List<int>, int>.Release(list);
				}
				this.RaiseOnSizeChanged();
			}
		}

		public virtual void Move(int index, int newIndex)
		{
			bool flag = this.itemsSource == null;
			if (!flag)
			{
				bool flag2 = index == newIndex;
				if (!flag2)
				{
					int num = Mathf.Min(index, newIndex);
					int num2 = Mathf.Max(index, newIndex);
					bool flag3 = num < 0 || num2 >= this.itemsSource.Count;
					if (!flag3)
					{
						int num3 = newIndex;
						int num4 = ((newIndex < index) ? 1 : (-1));
						while (Mathf.Min(index, newIndex) < Mathf.Max(index, newIndex))
						{
							this.Swap(index, newIndex);
							newIndex += num4;
						}
						base.RaiseItemIndexChanged(index, num3);
					}
				}
			}
		}

		public virtual void RemoveItem(int index)
		{
			List<int> list;
			using (CollectionPool<List<int>, int>.Get(out list))
			{
				list.Add(index);
				this.RemoveItems(list);
			}
		}

		public virtual void RemoveItems(List<int> indices)
		{
			this.EnsureItemSourceCanBeResized();
			bool flag = indices == null;
			if (!flag)
			{
				indices.Sort();
				this.RaiseItemsRemoved(indices);
				bool isFixedSize = this.itemsSource.IsFixedSize;
				if (isFixedSize)
				{
					this.itemsSource = BaseListViewController.RemoveFromArray((Array)this.itemsSource, indices);
				}
				else
				{
					for (int i = indices.Count - 1; i >= 0; i--)
					{
						this.itemsSource.RemoveAt(indices[i]);
					}
				}
				this.RaiseOnSizeChanged();
			}
		}

		internal virtual void RemoveItems(int itemCount)
		{
			bool flag = itemCount <= 0;
			if (!flag)
			{
				int itemsCount = this.GetItemsCount();
				List<int> list = CollectionPool<List<int>, int>.Get();
				try
				{
					int num = itemsCount - itemCount;
					for (int i = num; i < itemsCount; i++)
					{
						list.Add(i);
					}
					this.RemoveItems(list);
				}
				finally
				{
					CollectionPool<List<int>, int>.Release(list);
				}
			}
		}

		public virtual void ClearItems()
		{
			bool flag = this.itemsSource == null;
			if (!flag)
			{
				this.EnsureItemSourceCanBeResized();
				IEnumerable<int> enumerable = Enumerable.Range(0, this.itemsSource.Count - 1);
				this.itemsSource.Clear();
				this.RaiseItemsRemoved(enumerable);
				this.RaiseOnSizeChanged();
			}
		}

		protected void RaiseOnSizeChanged()
		{
			Action action = this.itemsSourceSizeChanged;
			if (action != null)
			{
				action();
			}
		}

		protected void RaiseItemsAdded(IEnumerable<int> indices)
		{
			Action<IEnumerable<int>> action = this.itemsAdded;
			if (action != null)
			{
				action(indices);
			}
		}

		protected void RaiseItemsRemoved(IEnumerable<int> indices)
		{
			Action<IEnumerable<int>> action = this.itemsRemoved;
			if (action != null)
			{
				action(indices);
			}
		}

		private static Array AddToArray(Array source, int itemCount)
		{
			Type elementType = source.GetType().GetElementType();
			bool flag = elementType == null;
			if (flag)
			{
				throw new InvalidOperationException("Cannot resize source, because its size is fixed.");
			}
			Array array = Array.CreateInstance(elementType, source.Length + itemCount);
			Array.Copy(source, array, source.Length);
			return array;
		}

		private static Array RemoveFromArray(Array source, List<int> indicesToRemove)
		{
			int length = source.Length;
			int num = length - indicesToRemove.Count;
			bool flag = num < 0;
			if (flag)
			{
				throw new InvalidOperationException("Cannot remove more items than the current count from source.");
			}
			Type elementType = source.GetType().GetElementType();
			bool flag2 = num == 0;
			Array array;
			if (flag2)
			{
				array = Array.CreateInstance(elementType, 0);
			}
			else
			{
				Array array2 = Array.CreateInstance(elementType, num);
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < source.Length; i++)
				{
					bool flag3 = num3 < indicesToRemove.Count && indicesToRemove[num3] == i;
					if (flag3)
					{
						num3++;
					}
					else
					{
						array2.SetValue(source.GetValue(i), num2);
						num2++;
					}
				}
				array = array2;
			}
			return array;
		}

		private void Swap(int lhs, int rhs)
		{
			IList itemsSource = this.itemsSource;
			IList itemsSource2 = this.itemsSource;
			object obj = this.itemsSource[rhs];
			object obj2 = this.itemsSource[lhs];
			itemsSource[lhs] = obj;
			itemsSource2[rhs] = obj2;
		}

		private void EnsureItemSourceCanBeResized()
		{
			bool flag = this.itemsSource == null;
			if (flag)
			{
				throw new InvalidOperationException("Unable to add or remove items because the source is not defined. Please assign a valid items source.");
			}
			bool flag2 = this.itemsSource.IsFixedSize && !this.itemsSource.GetType().IsArray;
			if (flag2)
			{
				throw new InvalidOperationException("Unable to add or remove items because the items source is a fixed size.");
			}
		}

		[CompilerGenerated]
		internal static bool <AddItems>g__IsGenericList|19_0(Type t)
		{
			return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>);
		}

		internal const string k_NullItemSourceError = "Unable to add or remove items because the source is not defined. Please assign a valid items source.";

		internal const string k_FixedItemSourceError = "Unable to add or remove items because the items source is a fixed size.";

		internal class SerializedObjectListControllerImpl
		{
			private ISerializedObjectList serializedObjectList
			{
				get
				{
					Func<ISerializedObjectList> serializedObjectListGetter = this.m_SerializedObjectListGetter;
					return (serializedObjectListGetter != null) ? serializedObjectListGetter() : null;
				}
			}

			internal SerializedObjectListControllerImpl(BaseListViewController baseListViewController, Func<ISerializedObjectList> serializedObjectListGetter)
			{
				this.m_BaseListViewController = baseListViewController;
				this.m_SerializedObjectListGetter = serializedObjectListGetter;
			}

			public int GetItemsCount()
			{
				ISerializedObjectList serializedObjectList = this.serializedObjectList;
				return (serializedObjectList != null) ? serializedObjectList.Count : 0;
			}

			internal int GetItemsMinCount()
			{
				ISerializedObjectList serializedObjectList = this.serializedObjectList;
				return (serializedObjectList != null) ? serializedObjectList.minArraySize : 0;
			}

			public void AddItems(int itemCount)
			{
				int itemsCount = this.GetItemsCount();
				this.serializedObjectList.arraySize += itemCount;
				this.serializedObjectList.ApplyChanges();
				List<int> list = CollectionPool<List<int>, int>.Get();
				try
				{
					for (int i = 0; i < itemCount; i++)
					{
						list.Add(itemsCount + i);
					}
					this.m_BaseListViewController.RaiseItemsAdded(list);
				}
				finally
				{
					CollectionPool<List<int>, int>.Release(list);
				}
				this.m_BaseListViewController.RaiseOnSizeChanged();
			}

			internal void RemoveItems(int itemCount)
			{
				int itemsCount = this.GetItemsCount();
				this.serializedObjectList.arraySize -= itemCount;
				List<int> list = CollectionPool<List<int>, int>.Get();
				try
				{
					for (int i = itemsCount - itemCount; i < itemsCount; i++)
					{
						list.Add(i);
					}
					this.m_BaseListViewController.RaiseItemsRemoved(list);
				}
				finally
				{
					CollectionPool<List<int>, int>.Release(list);
				}
				this.serializedObjectList.ApplyChanges();
				this.m_BaseListViewController.RaiseOnSizeChanged();
			}

			public void RemoveItems(List<int> indices)
			{
				indices.Sort();
				this.m_BaseListViewController.RaiseItemsRemoved(indices);
				int num = this.serializedObjectList.Count;
				for (int i = indices.Count - 1; i >= 0; i--)
				{
					int num2 = indices[i];
					this.serializedObjectList.RemoveAt(num2, num);
					num--;
				}
				this.serializedObjectList.ApplyChanges();
				this.m_BaseListViewController.RaiseOnSizeChanged();
			}

			public void RemoveItem(int index)
			{
				List<int> list;
				using (CollectionPool<List<int>, int>.Get(out list))
				{
					list.Add(index);
					this.m_BaseListViewController.RaiseItemsRemoved(list);
				}
				this.serializedObjectList.RemoveAt(index);
				this.serializedObjectList.ApplyChanges();
				this.m_BaseListViewController.RaiseOnSizeChanged();
			}

			public void ClearItems()
			{
				IEnumerable<int> enumerable = Enumerable.Range(0, this.GetItemsMinCount() - 1);
				this.serializedObjectList.arraySize = 0;
				this.serializedObjectList.ApplyChanges();
				this.m_BaseListViewController.RaiseItemsRemoved(enumerable);
				this.m_BaseListViewController.RaiseOnSizeChanged();
			}

			public void Move(int srcIndex, int destIndex)
			{
				bool flag = srcIndex == destIndex;
				if (!flag)
				{
					this.serializedObjectList.Move(srcIndex, destIndex);
					this.serializedObjectList.ApplyChanges();
					this.m_BaseListViewController.RaiseItemIndexChanged(srcIndex, destIndex);
				}
			}

			private Func<ISerializedObjectList> m_SerializedObjectListGetter;

			private BaseListViewController m_BaseListViewController;
		}
	}
}
