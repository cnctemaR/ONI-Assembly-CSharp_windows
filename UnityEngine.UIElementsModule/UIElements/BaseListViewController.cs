using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
				int count = this.itemsSource.Count;
				List<int> list = CollectionPool<List<int>, int>.Get();
				try
				{
					bool isFixedSize = this.itemsSource.IsFixedSize;
					if (isFixedSize)
					{
						this.itemsSource = BaseListViewController.AddToArray((Array)this.itemsSource, itemCount);
						for (int i = 0; i < itemCount; i++)
						{
							list.Add(count + i);
						}
					}
					else
					{
						Type type = this.itemsSource.GetType();
						Type type2 = type.GetInterfaces().FirstOrDefault<Type>(new Func<Type, bool>(BaseListViewController.<AddItems>g__IsGenericList|15_0));
						bool flag2 = type2 != null && type2.GetGenericArguments()[0].IsValueType;
						if (flag2)
						{
							Type type3 = type2.GetGenericArguments()[0];
							for (int j = 0; j < itemCount; j++)
							{
								list.Add(count + j);
								this.itemsSource.Add(Activator.CreateInstance(type3));
							}
						}
						else
						{
							for (int k = 0; k < itemCount; k++)
							{
								list.Add(count + k);
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
			IList itemsSource = this.itemsSource;
			Type type = ((itemsSource != null) ? itemsSource.GetType() : null);
			bool flag = type != null && type.IsArray;
			bool flag2 = this.itemsSource == null || (this.itemsSource.IsFixedSize && !flag);
			if (flag2)
			{
				throw new InvalidOperationException("Cannot add or remove items from source, because it is null or its size is fixed.");
			}
		}

		[CompilerGenerated]
		internal static bool <AddItems>g__IsGenericList|15_0(Type t)
		{
			return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>);
		}
	}
}
