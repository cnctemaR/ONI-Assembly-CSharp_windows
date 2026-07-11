using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Threading;

namespace System.Data
{
	[DefaultProperty("Table")]
	[DefaultEvent("PositionChanged")]
	public class DataView : MarshalByValueComponent, IBindingListView, IBindingList, IList, ICollection, IEnumerable, ITypedList, ISupportInitializeNotification, ISupportInitialize
	{
		internal DataView(DataTable table, bool locked)
		{
			GC.SuppressFinalize(this);
			DataCommonEventSource.Log.Trace<int, int, bool>("<ds.DataView.DataView|INFO> {0}, table={1}, locked={2}", this.ObjectID, (table != null) ? table.ObjectID : 0, locked);
			this._dvListener = new DataViewListener(this);
			this._locked = locked;
			this._table = table;
			this._dvListener.RegisterMetaDataEvents(this._table);
		}

		public DataView()
			: this(null)
		{
			this.SetIndex2("", DataViewRowState.CurrentRows, null, true);
		}

		public DataView(DataTable table)
			: this(table, false)
		{
			this.SetIndex2("", DataViewRowState.CurrentRows, null, true);
		}

		public DataView(DataTable table, string RowFilter, string Sort, DataViewRowState RowState)
		{
			GC.SuppressFinalize(this);
			DataCommonEventSource.Log.Trace<int, int, string, string, DataViewRowState>("<ds.DataView.DataView|API> {0}, table={1}, RowFilter='{2}', Sort='{3}', RowState={4}", this.ObjectID, (table != null) ? table.ObjectID : 0, RowFilter, Sort, RowState);
			if (table == null)
			{
				throw ExceptionBuilder.CanNotUse();
			}
			this._dvListener = new DataViewListener(this);
			this._locked = false;
			this._table = table;
			this._dvListener.RegisterMetaDataEvents(this._table);
			if ((RowState & ~(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.ModifiedOriginal)) != DataViewRowState.None)
			{
				throw ExceptionBuilder.RecordStateRange();
			}
			if ((RowState & DataViewRowState.ModifiedOriginal) != DataViewRowState.None && (RowState & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
			{
				throw ExceptionBuilder.SetRowStateFilter();
			}
			if (Sort == null)
			{
				Sort = string.Empty;
			}
			if (RowFilter == null)
			{
				RowFilter = string.Empty;
			}
			DataExpression dataExpression = new DataExpression(table, RowFilter);
			this.SetIndex(Sort, RowState, dataExpression);
		}

		[DefaultValue(true)]
		public bool AllowDelete
		{
			get
			{
				return this._allowDelete;
			}
			set
			{
				if (this._allowDelete != value)
				{
					this._allowDelete = value;
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		public bool ApplyDefaultSort
		{
			get
			{
				return this._applyDefaultSort;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, bool>("<ds.DataView.set_ApplyDefaultSort|API> {0}, {1}", this.ObjectID, value);
				if (this._applyDefaultSort != value)
				{
					this._comparison = null;
					this._applyDefaultSort = value;
					this.UpdateIndex(true);
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		[DefaultValue(true)]
		public bool AllowEdit
		{
			get
			{
				return this._allowEdit;
			}
			set
			{
				if (this._allowEdit != value)
				{
					this._allowEdit = value;
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		[DefaultValue(true)]
		public bool AllowNew
		{
			get
			{
				return this._allowNew;
			}
			set
			{
				if (this._allowNew != value)
				{
					this._allowNew = value;
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		[Browsable(false)]
		public int Count
		{
			get
			{
				return this._rowViewCache.Count;
			}
		}

		private int CountFromIndex
		{
			get
			{
				return ((this._index != null) ? this._index.RecordCount : 0) + ((this._addNewRow != null) ? 1 : 0);
			}
		}

		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				return this._dataViewManager;
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return !this._fInitInProgress;
			}
		}

		[Browsable(false)]
		protected bool IsOpen
		{
			get
			{
				return this._open;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		[DefaultValue("")]
		public virtual string RowFilter
		{
			get
			{
				DataExpression dataExpression = this._rowFilter as DataExpression;
				if (dataExpression != null)
				{
					return dataExpression.Expression;
				}
				return "";
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				DataCommonEventSource.Log.Trace<int, string>("<ds.DataView.set_RowFilter|API> {0}, '{1}'", this.ObjectID, value);
				if (this._fInitInProgress)
				{
					this._delayedRowFilter = value;
					return;
				}
				CultureInfo cultureInfo = ((this._table != null) ? this._table.Locale : CultureInfo.CurrentCulture);
				if (this._rowFilter == null || string.Compare(this.RowFilter, value, false, cultureInfo) != 0)
				{
					DataExpression dataExpression = new DataExpression(this._table, value);
					this.SetIndex(this._sort, this._recordStates, dataExpression);
				}
			}
		}

		internal Predicate<DataRow> RowPredicate
		{
			get
			{
				DataView.RowPredicateFilter rowPredicateFilter = this.GetFilter() as DataView.RowPredicateFilter;
				if (rowPredicateFilter == null)
				{
					return null;
				}
				return rowPredicateFilter._predicateFilter;
			}
			set
			{
				if (this.RowPredicate != value)
				{
					this.SetIndex(this.Sort, this.RowStateFilter, (value != null) ? new DataView.RowPredicateFilter(value) : null);
				}
			}
		}

		[DefaultValue(DataViewRowState.CurrentRows)]
		public DataViewRowState RowStateFilter
		{
			get
			{
				return this._recordStates;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, DataViewRowState>("<ds.DataView.set_RowStateFilter|API> {0}, {1}", this.ObjectID, value);
				if (this._fInitInProgress)
				{
					this._delayedRecordStates = value;
					return;
				}
				if ((value & ~(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.ModifiedOriginal)) != DataViewRowState.None)
				{
					throw ExceptionBuilder.RecordStateRange();
				}
				if ((value & DataViewRowState.ModifiedOriginal) != DataViewRowState.None && (value & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
				{
					throw ExceptionBuilder.SetRowStateFilter();
				}
				if (this._recordStates != value)
				{
					this.SetIndex(this._sort, value, this._rowFilter);
				}
			}
		}

		[DefaultValue("")]
		public string Sort
		{
			get
			{
				if (this._sort.Length == 0 && this._applyDefaultSort && this._table != null && this._table._primaryIndex.Length != 0)
				{
					return this._table.FormatSortString(this._table._primaryIndex);
				}
				return this._sort;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				DataCommonEventSource.Log.Trace<int, string>("<ds.DataView.set_Sort|API> {0}, '{1}'", this.ObjectID, value);
				if (this._fInitInProgress)
				{
					this._delayedSort = value;
					return;
				}
				CultureInfo cultureInfo = ((this._table != null) ? this._table.Locale : CultureInfo.CurrentCulture);
				if (string.Compare(this._sort, value, false, cultureInfo) != 0 || this._comparison != null)
				{
					this.CheckSort(value);
					this._comparison = null;
					this.SetIndex(value, this._recordStates, this._rowFilter);
				}
			}
		}

		internal Comparison<DataRow> SortComparison
		{
			get
			{
				return this._comparison;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataView.set_SortComparison|API> {0}", this.ObjectID);
				if (this._comparison != value)
				{
					this._comparison = value;
					this.SetIndex("", this._recordStates, this._rowFilter);
				}
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		[TypeConverter(typeof(DataTableTypeConverter))]
		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		public DataTable Table
		{
			get
			{
				return this._table;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, int>("<ds.DataView.set_Table|API> {0}, {1}", this.ObjectID, (value != null) ? value.ObjectID : 0);
				if (this._fInitInProgress && value != null)
				{
					this._delayedTable = value;
					return;
				}
				if (this._locked)
				{
					throw ExceptionBuilder.SetTable();
				}
				if (this._dataViewManager != null)
				{
					throw ExceptionBuilder.CanNotSetTable();
				}
				if (value != null && value.TableName.Length == 0)
				{
					throw ExceptionBuilder.CanNotBindTable();
				}
				if (this._table != value)
				{
					this._dvListener.UnregisterMetaDataEvents();
					this._table = value;
					if (this._table != null)
					{
						this._dvListener.RegisterMetaDataEvents(this._table);
					}
					this.SetIndex2("", DataViewRowState.CurrentRows, null, false);
					if (this._table != null)
					{
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, new DataTablePropertyDescriptor(this._table)));
					}
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		object IList.this[int recordIndex]
		{
			get
			{
				return this[recordIndex];
			}
			set
			{
				throw ExceptionBuilder.SetIListObject();
			}
		}

		public DataRowView this[int recordIndex]
		{
			get
			{
				return this.GetRowView(this.GetRow(recordIndex));
			}
		}

		public virtual DataRowView AddNew()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataView.AddNew|API> {0}", this.ObjectID);
			DataRowView dataRowView2;
			try
			{
				this.CheckOpen();
				if (!this.AllowNew)
				{
					throw ExceptionBuilder.AddNewNotAllowNull();
				}
				if (this._addNewRow != null)
				{
					this._rowViewCache[this._addNewRow].EndEdit();
				}
				this._addNewRow = this._table.NewRow();
				DataRowView dataRowView = new DataRowView(this, this._addNewRow);
				this._rowViewCache.Add(this._addNewRow, dataRowView);
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, this.IndexOf(dataRowView)));
				dataRowView2 = dataRowView;
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataRowView2;
		}

		public void BeginInit()
		{
			this._fInitInProgress = true;
		}

		public void EndInit()
		{
			if (this._delayedTable != null && this._delayedTable.fInitInProgress)
			{
				this._delayedTable._delayedViews.Add(this);
				return;
			}
			this._fInitInProgress = false;
			this._fEndInitInProgress = true;
			if (this._delayedTable != null)
			{
				this.Table = this._delayedTable;
				this._delayedTable = null;
			}
			if (this._delayedSort != null)
			{
				this.Sort = this._delayedSort;
				this._delayedSort = null;
			}
			if (this._delayedRowFilter != null)
			{
				this.RowFilter = this._delayedRowFilter;
				this._delayedRowFilter = null;
			}
			if (this._delayedRecordStates != (DataViewRowState)(-1))
			{
				this.RowStateFilter = this._delayedRecordStates;
				this._delayedRecordStates = (DataViewRowState)(-1);
			}
			this._fEndInitInProgress = false;
			this.SetIndex(this.Sort, this.RowStateFilter, this._rowFilter);
			this.OnInitialized();
		}

		private void CheckOpen()
		{
			if (!this.IsOpen)
			{
				throw ExceptionBuilder.NotOpen();
			}
		}

		private void CheckSort(string sort)
		{
			if (this._table == null)
			{
				throw ExceptionBuilder.CanNotUse();
			}
			if (sort.Length == 0)
			{
				return;
			}
			this._table.ParseSortString(sort);
		}

		protected void Close()
		{
			this._shouldOpen = false;
			this.UpdateIndex();
			this._dvListener.UnregisterMetaDataEvents();
		}

		public void CopyTo(Array array, int index)
		{
			checked
			{
				if (this._index != null)
				{
					RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(0);
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						array.SetValue(this.GetRowView(num), index);
						index++;
					}
				}
				if (this._addNewRow != null)
				{
					array.SetValue(this._rowViewCache[this._addNewRow], index);
				}
			}
		}

		private void CopyTo(DataRowView[] array, int index)
		{
			checked
			{
				if (this._index != null)
				{
					RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(0);
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						array[index] = this.GetRowView(num);
						index++;
					}
				}
				if (this._addNewRow != null)
				{
					array[index] = this._rowViewCache[this._addNewRow];
				}
			}
		}

		public void Delete(int index)
		{
			this.Delete(this.GetRow(index));
		}

		internal void Delete(DataRow row)
		{
			if (row != null)
			{
				long num = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataView.Delete|API> {0}, row={1}", this.ObjectID, row._objectID);
				try
				{
					this.CheckOpen();
					if (row == this._addNewRow)
					{
						this.FinishAddNew(false);
					}
					else
					{
						if (!this.AllowDelete)
						{
							throw ExceptionBuilder.CanNotDelete();
						}
						row.Delete();
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(num);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
			base.Dispose(disposing);
		}

		public int Find(object key)
		{
			return this.FindByKey(key);
		}

		internal virtual int FindByKey(object key)
		{
			return this._index.FindRecordByKey(key);
		}

		public int Find(object[] key)
		{
			return this.FindByKey(key);
		}

		internal virtual int FindByKey(object[] key)
		{
			return this._index.FindRecordByKey(key);
		}

		public DataRowView[] FindRows(object key)
		{
			return this.FindRowsByKey(new object[] { key });
		}

		public DataRowView[] FindRows(object[] key)
		{
			return this.FindRowsByKey(key);
		}

		internal virtual DataRowView[] FindRowsByKey(object[] key)
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataView.FindRows|API> {0}", this.ObjectID);
			DataRowView[] dataRowViewFromRange;
			try
			{
				Range range = this._index.FindRecords(key);
				dataRowViewFromRange = this.GetDataRowViewFromRange(range);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return dataRowViewFromRange;
		}

		internal DataRowView[] GetDataRowViewFromRange(Range range)
		{
			if (range.IsNull)
			{
				return Array.Empty<DataRowView>();
			}
			DataRowView[] array = new DataRowView[range.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this[i + range.Min];
			}
			return array;
		}

		internal void FinishAddNew(bool success)
		{
			DataCommonEventSource.Log.Trace<int, bool>("<ds.DataView.FinishAddNew|INFO> {0}, success={1}", this.ObjectID, success);
			DataRow addNewRow = this._addNewRow;
			if (success)
			{
				if (DataRowState.Detached == addNewRow.RowState)
				{
					this._table.Rows.Add(addNewRow);
				}
				else
				{
					addNewRow.EndEdit();
				}
			}
			if (addNewRow == this._addNewRow)
			{
				this._rowViewCache.Remove(this._addNewRow);
				this._addNewRow = null;
				if (!success)
				{
					addNewRow.CancelEdit();
				}
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, this.Count));
			}
		}

		public IEnumerator GetEnumerator()
		{
			DataRowView[] array = new DataRowView[this.Count];
			this.CopyTo(array, 0);
			return array.GetEnumerator();
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		int IList.Add(object value)
		{
			if (value == null)
			{
				this.AddNew();
				return this.Count - 1;
			}
			throw ExceptionBuilder.AddExternalObject();
		}

		void IList.Clear()
		{
			throw ExceptionBuilder.CanNotClear();
		}

		bool IList.Contains(object value)
		{
			return 0 <= this.IndexOf(value as DataRowView);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf(value as DataRowView);
		}

		internal int IndexOf(DataRowView rowview)
		{
			if (rowview != null)
			{
				if (this._addNewRow == rowview.Row)
				{
					return this.Count - 1;
				}
				DataRowView dataRowView;
				if (this._index != null && DataRowState.Detached != rowview.Row.RowState && this._rowViewCache.TryGetValue(rowview.Row, out dataRowView) && dataRowView == rowview)
				{
					return this.IndexOfDataRowView(rowview);
				}
			}
			return -1;
		}

		private int IndexOfDataRowView(DataRowView rowview)
		{
			return this._index.GetIndex(rowview.Row.GetRecordFromVersion(rowview.Row.GetDefaultRowVersion(this.RowStateFilter) & (DataRowVersion)(-1025)));
		}

		void IList.Insert(int index, object value)
		{
			throw ExceptionBuilder.InsertExternalObject();
		}

		void IList.Remove(object value)
		{
			int num = this.IndexOf(value as DataRowView);
			if (0 <= num)
			{
				((IList)this).RemoveAt(num);
				return;
			}
			throw ExceptionBuilder.RemoveExternalObject();
		}

		void IList.RemoveAt(int index)
		{
			this.Delete(index);
		}

		internal Index GetFindIndex(string column, bool keepIndex)
		{
			if (this._findIndexes == null)
			{
				this._findIndexes = new Dictionary<string, Index>();
			}
			Index index;
			if (this._findIndexes.TryGetValue(column, out index))
			{
				if (!keepIndex)
				{
					this._findIndexes.Remove(column);
					index.RemoveRef();
					if (index.RefCount == 1)
					{
						index.RemoveRef();
					}
				}
			}
			else if (keepIndex)
			{
				index = this._table.GetIndex(column, this._recordStates, this.GetFilter());
				this._findIndexes[column] = index;
				index.AddRef();
			}
			return index;
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return this.AllowNew;
			}
		}

		object IBindingList.AddNew()
		{
			return this.AddNew();
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return this.AllowEdit;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return this.AllowDelete;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return this.Sort.Length != 0;
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				return this.GetSortProperty();
			}
		}

		internal PropertyDescriptor GetSortProperty()
		{
			if (this._table != null && this._index != null && this._index._indexFields.Length == 1)
			{
				return new DataColumnPropertyDescriptor(this._index._indexFields[0].Column);
			}
			return null;
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				if (this._index._indexFields.Length != 1 || !this._index._indexFields[0].IsDescending)
				{
					return ListSortDirection.Ascending;
				}
				return ListSortDirection.Descending;
			}
		}

		public event ListChangedEventHandler ListChanged
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataView.add_ListChanged|API> {0}", this.ObjectID);
				this._onListChanged = (ListChangedEventHandler)Delegate.Combine(this._onListChanged, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataView.remove_ListChanged|API> {0}", this.ObjectID);
				this._onListChanged = (ListChangedEventHandler)Delegate.Remove(this._onListChanged, value);
			}
		}

		public event EventHandler Initialized;

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			this.GetFindIndex(property.Name, true);
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			this.Sort = this.CreateSortString(property, direction);
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			if (property != null)
			{
				bool flag = false;
				Index index = null;
				try
				{
					if (this._findIndexes == null || !this._findIndexes.TryGetValue(property.Name, out index))
					{
						flag = true;
						index = this._table.GetIndex(property.Name, this._recordStates, this.GetFilter());
						index.AddRef();
					}
					Range range = index.FindRecords(key);
					if (!range.IsNull)
					{
						return this._index.GetIndex(index.GetRecord(range.Min));
					}
				}
				finally
				{
					if (flag && index != null)
					{
						index.RemoveRef();
						if (index.RefCount == 1)
						{
							index.RemoveRef();
						}
					}
				}
				return -1;
			}
			return -1;
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			this.GetFindIndex(property.Name, false);
		}

		void IBindingList.RemoveSort()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.DataView.RemoveSort|API> {0}", this.ObjectID);
			this.Sort = string.Empty;
		}

		void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
		{
			if (sorts == null)
			{
				throw ExceptionBuilder.ArgumentNull("sorts");
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (object obj in ((IEnumerable)sorts))
			{
				ListSortDescription listSortDescription = (ListSortDescription)obj;
				if (listSortDescription == null)
				{
					throw ExceptionBuilder.ArgumentContainsNull("sorts");
				}
				PropertyDescriptor propertyDescriptor = listSortDescription.PropertyDescriptor;
				if (propertyDescriptor == null)
				{
					throw ExceptionBuilder.ArgumentNull("PropertyDescriptor");
				}
				if (!this._table.Columns.Contains(propertyDescriptor.Name))
				{
					throw ExceptionBuilder.ColumnToSortIsOutOfRange(propertyDescriptor.Name);
				}
				ListSortDirection sortDirection = listSortDescription.SortDirection;
				if (flag)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(this.CreateSortString(propertyDescriptor, sortDirection));
				if (!flag)
				{
					flag = true;
				}
			}
			this.Sort = stringBuilder.ToString();
		}

		private string CreateSortString(PropertyDescriptor property, ListSortDirection direction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			stringBuilder.Append(property.Name);
			stringBuilder.Append(']');
			if (ListSortDirection.Descending == direction)
			{
				stringBuilder.Append(" DESC");
			}
			return stringBuilder.ToString();
		}

		void IBindingListView.RemoveFilter()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.DataView.RemoveFilter|API> {0}", this.ObjectID);
			this.RowFilter = string.Empty;
		}

		string IBindingListView.Filter
		{
			get
			{
				return this.RowFilter;
			}
			set
			{
				this.RowFilter = value;
			}
		}

		ListSortDescriptionCollection IBindingListView.SortDescriptions
		{
			get
			{
				return this.GetSortDescriptions();
			}
		}

		internal ListSortDescriptionCollection GetSortDescriptions()
		{
			ListSortDescription[] array = Array.Empty<ListSortDescription>();
			if (this._table != null && this._index != null && this._index._indexFields.Length != 0)
			{
				array = new ListSortDescription[this._index._indexFields.Length];
				for (int i = 0; i < this._index._indexFields.Length; i++)
				{
					DataColumnPropertyDescriptor dataColumnPropertyDescriptor = new DataColumnPropertyDescriptor(this._index._indexFields[i].Column);
					if (this._index._indexFields[i].IsDescending)
					{
						array[i] = new ListSortDescription(dataColumnPropertyDescriptor, ListSortDirection.Descending);
					}
					else
					{
						array[i] = new ListSortDescription(dataColumnPropertyDescriptor, ListSortDirection.Ascending);
					}
				}
			}
			return new ListSortDescriptionCollection(array);
		}

		bool IBindingListView.SupportsAdvancedSorting
		{
			get
			{
				return true;
			}
		}

		bool IBindingListView.SupportsFiltering
		{
			get
			{
				return true;
			}
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (this._table != null)
			{
				if (listAccessors == null || listAccessors.Length == 0)
				{
					return this._table.TableName;
				}
				DataSet dataSet = this._table.DataSet;
				if (dataSet != null)
				{
					DataTable dataTable = dataSet.FindTable(this._table, listAccessors, 0);
					if (dataTable != null)
					{
						return dataTable.TableName;
					}
				}
			}
			return string.Empty;
		}

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (this._table != null)
			{
				if (listAccessors == null || listAccessors.Length == 0)
				{
					return this._table.GetPropertyDescriptorCollection(null);
				}
				DataSet dataSet = this._table.DataSet;
				if (dataSet == null)
				{
					return new PropertyDescriptorCollection(null);
				}
				DataTable dataTable = dataSet.FindTable(this._table, listAccessors, 0);
				if (dataTable != null)
				{
					return dataTable.GetPropertyDescriptorCollection(null);
				}
			}
			return new PropertyDescriptorCollection(null);
		}

		internal virtual IFilter GetFilter()
		{
			return this._rowFilter;
		}

		private int GetRecord(int recordIndex)
		{
			if (this.Count <= recordIndex)
			{
				throw ExceptionBuilder.RowOutOfRange(recordIndex);
			}
			if (recordIndex != this._index.RecordCount)
			{
				return this._index.GetRecord(recordIndex);
			}
			return this._addNewRow.GetDefaultRecord();
		}

		internal DataRow GetRow(int index)
		{
			int count = this.Count;
			if (count <= index)
			{
				throw ExceptionBuilder.GetElementIndex(index);
			}
			if (index == count - 1 && this._addNewRow != null)
			{
				return this._addNewRow;
			}
			return this._table._recordManager[this.GetRecord(index)];
		}

		private DataRowView GetRowView(int record)
		{
			return this.GetRowView(this._table._recordManager[record]);
		}

		private DataRowView GetRowView(DataRow dr)
		{
			return this._rowViewCache[dr];
		}

		protected virtual void IndexListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType != ListChangedType.Reset)
			{
				this.OnListChanged(e);
			}
			if (this._addNewRow != null && this._index.RecordCount == 0)
			{
				this.FinishAddNew(false);
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				this.OnListChanged(e);
			}
		}

		internal void IndexListChangedInternal(ListChangedEventArgs e)
		{
			this._rowViewBuffer.Clear();
			if (ListChangedType.ItemAdded == e.ListChangedType && this._addNewMoved != null && this._addNewMoved.NewIndex != this._addNewMoved.OldIndex)
			{
				ListChangedEventArgs addNewMoved = this._addNewMoved;
				this._addNewMoved = null;
				this.IndexListChanged(this, addNewMoved);
			}
			this.IndexListChanged(this, e);
		}

		internal void MaintainDataView(ListChangedType changedType, DataRow row, bool trackAddRemove)
		{
			DataRowView dataRowView = null;
			switch (changedType)
			{
			case ListChangedType.Reset:
				this.ResetRowViewCache();
				break;
			case ListChangedType.ItemAdded:
				if (trackAddRemove && this._rowViewBuffer.TryGetValue(row, out dataRowView))
				{
					this._rowViewBuffer.Remove(row);
				}
				if (row == this._addNewRow)
				{
					int num = this.IndexOfDataRowView(this._rowViewCache[this._addNewRow]);
					this._addNewRow = null;
					this._addNewMoved = new ListChangedEventArgs(ListChangedType.ItemMoved, num, this.Count - 1);
					return;
				}
				if (!this._rowViewCache.ContainsKey(row))
				{
					this._rowViewCache.Add(row, dataRowView ?? new DataRowView(this, row));
					return;
				}
				break;
			case ListChangedType.ItemDeleted:
				if (trackAddRemove)
				{
					this._rowViewCache.TryGetValue(row, out dataRowView);
					if (dataRowView != null)
					{
						this._rowViewBuffer.Add(row, dataRowView);
					}
				}
				this._rowViewCache.Remove(row);
				return;
			case ListChangedType.ItemMoved:
			case ListChangedType.ItemChanged:
			case ListChangedType.PropertyDescriptorAdded:
			case ListChangedType.PropertyDescriptorDeleted:
			case ListChangedType.PropertyDescriptorChanged:
				break;
			default:
				return;
			}
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			DataCommonEventSource.Log.Trace<int, ListChangedType>("<ds.DataView.OnListChanged|INFO> {0}, ListChangedType={1}", this.ObjectID, e.ListChangedType);
			try
			{
				DataColumn dataColumn = null;
				string text = null;
				switch (e.ListChangedType)
				{
				case ListChangedType.ItemMoved:
				case ListChangedType.ItemChanged:
					if (0 <= e.NewIndex)
					{
						DataRow row = this.GetRow(e.NewIndex);
						if (row.HasPropertyChanged)
						{
							dataColumn = row.LastChangedColumn;
							text = ((dataColumn != null) ? dataColumn.ColumnName : string.Empty);
						}
					}
					break;
				}
				if (this._onListChanged != null)
				{
					if (dataColumn != null && e.NewIndex == e.OldIndex)
					{
						ListChangedEventArgs e2 = new ListChangedEventArgs(e.ListChangedType, e.NewIndex, new DataColumnPropertyDescriptor(dataColumn));
						this._onListChanged(this, e2);
					}
					else
					{
						this._onListChanged(this, e);
					}
				}
				if (text != null)
				{
					this[e.NewIndex].RaisePropertyChangedEvent(text);
				}
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
			}
		}

		private void OnInitialized()
		{
			EventHandler initialized = this.Initialized;
			if (initialized == null)
			{
				return;
			}
			initialized(this, EventArgs.Empty);
		}

		protected void Open()
		{
			this._shouldOpen = true;
			this.UpdateIndex();
			this._dvListener.RegisterMetaDataEvents(this._table);
		}

		protected void Reset()
		{
			if (this.IsOpen)
			{
				this._index.Reset();
			}
		}

		internal void ResetRowViewCache()
		{
			Dictionary<DataRow, DataRowView> dictionary = new Dictionary<DataRow, DataRowView>(this.CountFromIndex, DataView.DataRowReferenceComparer.s_default);
			if (this._index != null)
			{
				RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(0);
				while (enumerator.MoveNext())
				{
					int num = enumerator.Current;
					DataRow dataRow = this._table._recordManager[num];
					DataRowView dataRowView;
					if (!this._rowViewCache.TryGetValue(dataRow, out dataRowView))
					{
						dataRowView = new DataRowView(this, dataRow);
					}
					dictionary.Add(dataRow, dataRowView);
				}
			}
			if (this._addNewRow != null)
			{
				DataRowView dataRowView;
				this._rowViewCache.TryGetValue(this._addNewRow, out dataRowView);
				dictionary.Add(this._addNewRow, dataRowView);
			}
			this._rowViewCache = dictionary;
		}

		internal void SetDataViewManager(DataViewManager dataViewManager)
		{
			if (this._table == null)
			{
				throw ExceptionBuilder.CanNotUse();
			}
			if (this._dataViewManager != dataViewManager)
			{
				if (dataViewManager != null)
				{
					dataViewManager._nViews--;
				}
				this._dataViewManager = dataViewManager;
				if (dataViewManager != null)
				{
					dataViewManager._nViews++;
					DataViewSetting dataViewSetting = dataViewManager.DataViewSettings[this._table];
					try
					{
						this._applyDefaultSort = dataViewSetting.ApplyDefaultSort;
						DataExpression dataExpression = new DataExpression(this._table, dataViewSetting.RowFilter);
						this.SetIndex(dataViewSetting.Sort, dataViewSetting.RowStateFilter, dataExpression);
					}
					catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
					}
					this._locked = true;
					return;
				}
				this.SetIndex("", DataViewRowState.CurrentRows, null);
			}
		}

		internal virtual void SetIndex(string newSort, DataViewRowState newRowStates, IFilter newRowFilter)
		{
			this.SetIndex2(newSort, newRowStates, newRowFilter, true);
		}

		internal void SetIndex2(string newSort, DataViewRowState newRowStates, IFilter newRowFilter, bool fireEvent)
		{
			DataCommonEventSource.Log.Trace<int, string, DataViewRowState>("<ds.DataView.SetIndex|INFO> {0}, newSort='{1}', newRowStates={2}", this.ObjectID, newSort, newRowStates);
			this._sort = newSort;
			this._recordStates = newRowStates;
			this._rowFilter = newRowFilter;
			if (this._fEndInitInProgress)
			{
				return;
			}
			if (fireEvent)
			{
				this.UpdateIndex(true);
			}
			else
			{
				this.UpdateIndex(true, false);
			}
			if (this._findIndexes != null)
			{
				Dictionary<string, Index> findIndexes = this._findIndexes;
				this._findIndexes = null;
				foreach (KeyValuePair<string, Index> keyValuePair in findIndexes)
				{
					keyValuePair.Value.RemoveRef();
				}
			}
		}

		protected void UpdateIndex()
		{
			this.UpdateIndex(false);
		}

		protected virtual void UpdateIndex(bool force)
		{
			this.UpdateIndex(force, true);
		}

		internal void UpdateIndex(bool force, bool fireEvent)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataView.UpdateIndex|INFO> {0}, force={1}", this.ObjectID, force);
			try
			{
				if (this._open != this._shouldOpen || force)
				{
					this._open = this._shouldOpen;
					Index index = null;
					if (this._open && this._table != null)
					{
						if (this.SortComparison != null)
						{
							index = new Index(this._table, this.SortComparison, this._recordStates, this.GetFilter());
							index.AddRef();
						}
						else
						{
							index = this._table.GetIndex(this.Sort, this._recordStates, this.GetFilter());
						}
					}
					if (this._index != index)
					{
						if (this._index == null)
						{
							DataTable table = index.Table;
						}
						else
						{
							DataTable table2 = this._index.Table;
						}
						if (this._index != null)
						{
							this._dvListener.UnregisterListChangedEvent();
						}
						this._index = index;
						if (this._index != null)
						{
							this._dvListener.RegisterListChangedEvent(this._index);
						}
						this.ResetRowViewCache();
						if (fireEvent)
						{
							this.OnListChanged(DataView.s_resetEventArgs);
						}
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		internal void ChildRelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataRelationPropertyDescriptor dataRelationPropertyDescriptor = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, dataRelationPropertyDescriptor) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : null)));
		}

		internal void ParentRelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataRelationPropertyDescriptor dataRelationPropertyDescriptor = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, dataRelationPropertyDescriptor) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : null)));
		}

		protected virtual void ColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataColumnPropertyDescriptor dataColumnPropertyDescriptor = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataColumnPropertyDescriptor((DataColumn)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, dataColumnPropertyDescriptor) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataColumnPropertyDescriptor((DataColumn)e.Element)) : null)));
		}

		internal void ColumnCollectionChangedInternal(object sender, CollectionChangeEventArgs e)
		{
			this.ColumnCollectionChanged(sender, e);
		}

		public DataTable ToTable()
		{
			return this.ToTable(null, false, Array.Empty<string>());
		}

		public DataTable ToTable(string tableName)
		{
			return this.ToTable(tableName, false, Array.Empty<string>());
		}

		public DataTable ToTable(bool distinct, params string[] columnNames)
		{
			return this.ToTable(null, distinct, columnNames);
		}

		public DataTable ToTable(string tableName, bool distinct, params string[] columnNames)
		{
			DataCommonEventSource.Log.Trace<int, string, bool>("<ds.DataView.ToTable|API> {0}, TableName='{1}', distinct={2}", this.ObjectID, tableName, distinct);
			if (columnNames == null)
			{
				throw ExceptionBuilder.ArgumentNull("columnNames");
			}
			DataTable dataTable = new DataTable();
			dataTable.Locale = this._table.Locale;
			dataTable.CaseSensitive = this._table.CaseSensitive;
			dataTable.TableName = ((tableName != null) ? tableName : this._table.TableName);
			dataTable.Namespace = this._table.Namespace;
			dataTable.Prefix = this._table.Prefix;
			if (columnNames.Length == 0)
			{
				columnNames = new string[this.Table.Columns.Count];
				for (int i = 0; i < columnNames.Length; i++)
				{
					columnNames[i] = this.Table.Columns[i].ColumnName;
				}
			}
			int[] array = new int[columnNames.Length];
			List<object[]> list = new List<object[]>();
			for (int j = 0; j < columnNames.Length; j++)
			{
				DataColumn dataColumn = this.Table.Columns[columnNames[j]];
				if (dataColumn == null)
				{
					throw ExceptionBuilder.ColumnNotInTheUnderlyingTable(columnNames[j], this.Table.TableName);
				}
				dataTable.Columns.Add(dataColumn.Clone());
				array[j] = this.Table.Columns.IndexOf(dataColumn);
			}
			foreach (object obj in this)
			{
				DataRowView dataRowView = (DataRowView)obj;
				object[] array2 = new object[columnNames.Length];
				for (int k = 0; k < array.Length; k++)
				{
					array2[k] = dataRowView[array[k]];
				}
				if (!distinct || !this.RowExist(list, array2))
				{
					dataTable.Rows.Add(array2);
					list.Add(array2);
				}
			}
			return dataTable;
		}

		private bool RowExist(List<object[]> arraylist, object[] objectArray)
		{
			for (int i = 0; i < arraylist.Count; i++)
			{
				object[] array = arraylist[i];
				bool flag = true;
				for (int j = 0; j < objectArray.Length; j++)
				{
					flag &= array[j].Equals(objectArray[j]);
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool Equals(DataView view)
		{
			return view != null && this.Table == view.Table && this.Count == view.Count && string.Equals(this.RowFilter, view.RowFilter, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Sort, view.Sort, StringComparison.OrdinalIgnoreCase) && this.SortComparison == view.SortComparison && this.RowPredicate == view.RowPredicate && this.RowStateFilter == view.RowStateFilter && this.DataViewManager == view.DataViewManager && this.AllowDelete == view.AllowDelete && this.AllowNew == view.AllowNew && this.AllowEdit == view.AllowEdit;
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		private DataViewManager _dataViewManager;

		private DataTable _table;

		private bool _locked;

		private Index _index;

		private Dictionary<string, Index> _findIndexes;

		private string _sort = string.Empty;

		private Comparison<DataRow> _comparison;

		private IFilter _rowFilter;

		private DataViewRowState _recordStates = DataViewRowState.CurrentRows;

		private bool _shouldOpen = true;

		private bool _open;

		private bool _allowNew = true;

		private bool _allowEdit = true;

		private bool _allowDelete = true;

		private bool _applyDefaultSort;

		internal DataRow _addNewRow;

		private ListChangedEventArgs _addNewMoved;

		private ListChangedEventHandler _onListChanged;

		internal static ListChangedEventArgs s_resetEventArgs = new ListChangedEventArgs(ListChangedType.Reset, -1);

		private DataTable _delayedTable;

		private string _delayedRowFilter;

		private string _delayedSort;

		private DataViewRowState _delayedRecordStates = (DataViewRowState)(-1);

		private bool _fInitInProgress;

		private bool _fEndInitInProgress;

		private Dictionary<DataRow, DataRowView> _rowViewCache = new Dictionary<DataRow, DataRowView>(DataView.DataRowReferenceComparer.s_default);

		private readonly Dictionary<DataRow, DataRowView> _rowViewBuffer = new Dictionary<DataRow, DataRowView>(DataView.DataRowReferenceComparer.s_default);

		private DataViewListener _dvListener;

		private static int s_objectTypeCount;

		private readonly int _objectID = Interlocked.Increment(ref DataView.s_objectTypeCount);

		private sealed class DataRowReferenceComparer : IEqualityComparer<DataRow>
		{
			private DataRowReferenceComparer()
			{
			}

			public bool Equals(DataRow x, DataRow y)
			{
				return x == y;
			}

			public int GetHashCode(DataRow obj)
			{
				return obj._objectID;
			}

			internal static readonly DataView.DataRowReferenceComparer s_default = new DataView.DataRowReferenceComparer();
		}

		private sealed class RowPredicateFilter : IFilter
		{
			internal RowPredicateFilter(Predicate<DataRow> predicate)
			{
				this._predicateFilter = predicate;
			}

			bool IFilter.Invoke(DataRow row, DataRowVersion version)
			{
				return this._predicateFilter(row);
			}

			internal readonly Predicate<DataRow> _predicateFilter;
		}
	}
}
