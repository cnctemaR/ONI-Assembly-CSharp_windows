using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[DefaultEvent("PositionChanged")]
	[DefaultProperty("Table")]
	[Designer("Microsoft.VSDesigner.Data.VS.DataViewDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[Editor("Microsoft.VSDesigner.Data.Design.DataSourceEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class DataView : MarshalByValueComponent, ICollection, IEnumerable, IList, IBindingList, IBindingListView, ISupportInitialize, ISupportInitializeNotification, ITypedList
	{
		public DataView()
		{
		}

		public DataView(DataTable table)
		{
		}

		public DataView(DataTable table, string RowFilter, string Sort, DataViewRowState RowState)
		{
		}

		[DefaultValue(true)]
		public bool AllowDelete
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(true)]
		public bool AllowEdit
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(true)]
		public bool AllowNew
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool ApplyDefaultSort
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public int Count
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		protected bool IsOpen
		{
			get
			{
				throw null;
			}
		}

		public DataRowView this[int recordIndex]
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue("")]
		public virtual string RowFilter
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(DataViewRowState.CurrentRows)]
		public DataViewRowState RowStateFilter
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue("")]
		public string Sort
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw null;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				throw null;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		object IList.this[int recordIndex]
		{
			get
			{
				throw null;
			}
			[MonoTODO]
			set
			{
			}
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				throw null;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				throw null;
			}
		}

		bool IBindingList.AllowRemove
		{
			[MonoTODO]
			get
			{
				throw null;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				throw null;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				throw null;
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				throw null;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				throw null;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				throw null;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				throw null;
			}
		}

		string IBindingListView.Filter
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		ListSortDescriptionCollection IBindingListView.SortDescriptions
		{
			get
			{
				throw null;
			}
		}

		bool IBindingListView.SupportsAdvancedSorting
		{
			get
			{
				throw null;
			}
		}

		bool IBindingListView.SupportsFiltering
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.DataTableTypeConverter")]
		public DataTable Table
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public event EventHandler Initialized
		{
			add
			{
			}
			remove
			{
			}
		}

		public event ListChangedEventHandler ListChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public virtual DataRowView AddNew()
		{
			throw null;
		}

		public void BeginInit()
		{
		}

		protected void Close()
		{
		}

		protected virtual void ColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
		}

		public void CopyTo(Array array, int index)
		{
		}

		public void Delete(int index)
		{
		}

		protected override void Dispose(bool disposing)
		{
		}

		public void EndInit()
		{
		}

		public virtual bool Equals(DataView view)
		{
			throw null;
		}

		public int Find(object key)
		{
			throw null;
		}

		public int Find(object[] key)
		{
			throw null;
		}

		public DataRowView[] FindRows(object key)
		{
			throw null;
		}

		public DataRowView[] FindRows(object[] key)
		{
			throw null;
		}

		public IEnumerator GetEnumerator()
		{
			throw null;
		}

		protected virtual void IndexListChanged(object sender, ListChangedEventArgs e)
		{
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
		}

		protected void Open()
		{
		}

		protected void Reset()
		{
		}

		int IList.Add(object value)
		{
			throw null;
		}

		void IList.Clear()
		{
		}

		bool IList.Contains(object value)
		{
			throw null;
		}

		int IList.IndexOf(object value)
		{
			throw null;
		}

		void IList.Insert(int index, object value)
		{
		}

		void IList.Remove(object value)
		{
		}

		void IList.RemoveAt(int index)
		{
		}

		[MonoTODO]
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		object IBindingList.AddNew()
		{
			throw null;
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw null;
		}

		[MonoTODO]
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.RemoveSort()
		{
		}

		[MonoTODO]
		void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
		{
		}

		void IBindingListView.RemoveFilter()
		{
		}

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			throw null;
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			throw null;
		}

		public DataTable ToTable()
		{
			throw null;
		}

		public DataTable ToTable(bool distinct, params string[] columnNames)
		{
			throw null;
		}

		public DataTable ToTable(string tableName)
		{
			throw null;
		}

		public DataTable ToTable(string tableName, bool distinct, params string[] columnNames)
		{
			throw null;
		}

		protected void UpdateIndex()
		{
		}

		protected virtual void UpdateIndex(bool force)
		{
		}
	}
}
