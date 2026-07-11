using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.ComponentModel
{
	[Serializable]
	public class BindingList<T> : Collection<T>, IList, ICollection, IEnumerable, IBindingList, ICancelAddNew, IRaiseItemChangedEvents
	{
		public BindingList(IList<T> list)
			: base(list)
		{
			this.CheckType();
		}

		public BindingList()
		{
			this.CheckType();
		}

		public event AddingNewEventHandler AddingNew;

		public event ListChangedEventHandler ListChanged;

		void IBindingList.AddIndex(PropertyDescriptor index)
		{
		}

		object IBindingList.AddNew()
		{
			return this.AddNew();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			this.ApplySortCore(property, direction);
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			return this.FindCore(property, key);
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.RemoveSort()
		{
			this.RemoveSortCore();
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return this.IsSortedCore;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				return this.SortDirectionCore;
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				return this.SortPropertyCore;
			}
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return this.AllowEdit;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return this.AllowNew;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return this.AllowRemove;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return this.SupportsChangeNotificationCore;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return this.SupportsSearchingCore;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return this.SupportsSortingCore;
			}
		}

		bool IRaiseItemChangedEvents.RaisesItemChangedEvents
		{
			get
			{
				return this.type_raises_item_changed_events;
			}
		}

		private void CheckType()
		{
			ConstructorInfo constructor = typeof(T).GetConstructor(Type.EmptyTypes);
			this.type_has_default_ctor = constructor != null;
			this.type_raises_item_changed_events = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T));
		}

		public bool AllowEdit
		{
			get
			{
				return this.allow_edit;
			}
			set
			{
				if (this.allow_edit != value)
				{
					this.allow_edit = value;
					if (this.raise_list_changed_events)
					{
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
					}
				}
			}
		}

		public bool AllowNew
		{
			get
			{
				if (this.allow_new_set)
				{
					return this.allow_new;
				}
				return this.type_has_default_ctor || this.AddingNew != null;
			}
			set
			{
				if (this.AllowNew != value)
				{
					this.allow_new_set = true;
					this.allow_new = value;
					if (this.raise_list_changed_events)
					{
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
					}
				}
			}
		}

		public bool AllowRemove
		{
			get
			{
				return this.allow_remove;
			}
			set
			{
				if (this.allow_remove != value)
				{
					this.allow_remove = value;
					if (this.raise_list_changed_events)
					{
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
					}
				}
			}
		}

		protected virtual bool IsSortedCore
		{
			get
			{
				return false;
			}
		}

		public bool RaiseListChangedEvents
		{
			get
			{
				return this.raise_list_changed_events;
			}
			set
			{
				this.raise_list_changed_events = value;
			}
		}

		protected virtual ListSortDirection SortDirectionCore
		{
			get
			{
				return ListSortDirection.Ascending;
			}
		}

		protected virtual PropertyDescriptor SortPropertyCore
		{
			get
			{
				return null;
			}
		}

		protected virtual bool SupportsChangeNotificationCore
		{
			get
			{
				return true;
			}
		}

		protected virtual bool SupportsSearchingCore
		{
			get
			{
				return false;
			}
		}

		protected virtual bool SupportsSortingCore
		{
			get
			{
				return false;
			}
		}

		public T AddNew()
		{
			return (T)((object)this.AddNewCore());
		}

		protected virtual object AddNewCore()
		{
			if (!this.AllowNew)
			{
				throw new InvalidOperationException();
			}
			AddingNewEventArgs e = new AddingNewEventArgs();
			this.OnAddingNew(e);
			T t = (T)((object)e.NewObject);
			if (t == null)
			{
				if (!this.type_has_default_ctor)
				{
					throw new InvalidOperationException();
				}
				t = (T)((object)Activator.CreateInstance(typeof(T)));
			}
			this.Add(t);
			this.pending_add_index = this.IndexOf(t);
			this.add_pending = true;
			return t;
		}

		protected virtual void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		public virtual void CancelNew(int itemIndex)
		{
			if (!this.add_pending)
			{
				return;
			}
			if (itemIndex != this.pending_add_index)
			{
				return;
			}
			this.add_pending = false;
			base.RemoveItem(itemIndex);
			if (this.raise_list_changed_events)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, itemIndex));
			}
		}

		protected override void ClearItems()
		{
			this.EndNew(this.pending_add_index);
			base.ClearItems();
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public virtual void EndNew(int itemIndex)
		{
			if (!this.add_pending)
			{
				return;
			}
			if (itemIndex != this.pending_add_index)
			{
				return;
			}
			this.add_pending = false;
		}

		protected virtual int FindCore(PropertyDescriptor prop, object key)
		{
			throw new NotSupportedException();
		}

		protected override void InsertItem(int index, T item)
		{
			this.EndNew(this.pending_add_index);
			base.InsertItem(index, item);
			if (this.raise_list_changed_events)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
			}
		}

		protected virtual void OnAddingNew(AddingNewEventArgs e)
		{
			if (this.AddingNew != null)
			{
				this.AddingNew(this, e);
			}
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			if (this.ListChanged != null)
			{
				this.ListChanged(this, e);
			}
		}

		protected override void RemoveItem(int index)
		{
			if (!this.AllowRemove)
			{
				throw new NotSupportedException();
			}
			this.EndNew(this.pending_add_index);
			base.RemoveItem(index);
			if (this.raise_list_changed_events)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
			}
		}

		protected virtual void RemoveSortCore()
		{
			throw new NotSupportedException();
		}

		public void ResetBindings()
		{
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public void ResetItem(int position)
		{
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, position));
		}

		protected override void SetItem(int index, T item)
		{
			base.SetItem(index, item);
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}

		private bool allow_edit = true;

		private bool allow_remove = true;

		private bool allow_new;

		private bool allow_new_set;

		private bool raise_list_changed_events = true;

		private bool type_has_default_ctor;

		private bool type_raises_item_changed_events;

		private bool add_pending;

		private int pending_add_index;
	}
}
