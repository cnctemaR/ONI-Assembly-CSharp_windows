using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;
using Mono.Data.SqlExpressions;

namespace System.Data
{
	[DefaultEvent("PositionChanged")]
	[Editor("Microsoft.VSDesigner.Data.Design.DataSourceEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Designer("Microsoft.VSDesigner.Data.VS.DataViewDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[DefaultProperty("Table")]
	public class DataView : MarshalByValueComponent, IList, IEnumerable, ITypedList, IBindingListView, IBindingList, ICollection, ISupportInitialize, ISupportInitializeNotification
	{
		public DataView()
		{
			this.rowState = DataViewRowState.CurrentRows;
			this.Open();
		}

		public DataView(DataTable table)
			: this(table, null)
		{
		}

		internal DataView(DataTable table, DataViewManager manager)
		{
			this.dataTable = table;
			this.rowState = DataViewRowState.CurrentRows;
			this.dataViewManager = manager;
			this.Open();
		}

		public DataView(DataTable table, string RowFilter, string Sort, DataViewRowState RowState)
			: this(table, null, RowFilter, Sort, RowState)
		{
		}

		internal DataView(DataTable table, DataViewManager manager, string RowFilter, string Sort, DataViewRowState RowState)
		{
			this.dataTable = table;
			this.dataViewManager = manager;
			this.rowState = DataViewRowState.CurrentRows;
			this.RowFilter = RowFilter;
			this.Sort = Sort;
			this.rowState = RowState;
			this.Open();
		}

		[DataCategory("Data")]
		public event ListChangedEventHandler ListChanged;

		public event EventHandler Initialized;

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (this.dataTable == null)
			{
				return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			}
			PropertyDescriptor[] array = new PropertyDescriptor[this.dataTable.Columns.Count + this.dataTable.ChildRelations.Count];
			int num = 0;
			for (int i = 0; i < this.dataTable.Columns.Count; i++)
			{
				DataColumn dataColumn = this.dataTable.Columns[i];
				DataColumnPropertyDescriptor dataColumnPropertyDescriptor = new DataColumnPropertyDescriptor(dataColumn.ColumnName, i, null);
				dataColumnPropertyDescriptor.SetComponentType(typeof(DataRowView));
				dataColumnPropertyDescriptor.SetPropertyType(dataColumn.DataType);
				dataColumnPropertyDescriptor.SetReadOnly(dataColumn.ReadOnly);
				dataColumnPropertyDescriptor.SetBrowsable(dataColumn.ColumnMapping != MappingType.Hidden);
				array[num++] = dataColumnPropertyDescriptor;
			}
			for (int j = 0; j < this.dataTable.ChildRelations.Count; j++)
			{
				DataRelation dataRelation = this.dataTable.ChildRelations[j];
				DataRelationPropertyDescriptor dataRelationPropertyDescriptor = new DataRelationPropertyDescriptor(dataRelation);
				array[num++] = dataRelationPropertyDescriptor;
			}
			return new PropertyDescriptorCollection(array);
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (this.dataTable != null)
			{
				return this.dataTable.TableName;
			}
			return string.Empty;
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int recordIndex]
		{
			get
			{
				return this[recordIndex];
			}
			[MonoTODO]
			set
			{
				throw new InvalidOperationException();
			}
		}

		int IList.Add(object value)
		{
			throw new ArgumentException("Cannot add external objects to this list.");
		}

		void IList.Clear()
		{
			throw new ArgumentException("Cannot clear this list.");
		}

		bool IList.Contains(object value)
		{
			DataRowView dataRowView = value as DataRowView;
			return dataRowView != null && dataRowView.DataView == this;
		}

		int IList.IndexOf(object value)
		{
			DataRowView dataRowView = value as DataRowView;
			if (dataRowView != null && dataRowView.DataView == this)
			{
				return dataRowView.Index;
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			throw new ArgumentException("Cannot insert external objects to this list.");
		}

		void IList.Remove(object value)
		{
			DataRowView dataRowView = value as DataRowView;
			if (dataRowView != null && dataRowView.DataView == this)
			{
				((IList)this).RemoveAt(dataRowView.Index);
			}
			throw new ArgumentException("Cannot remove external objects to this list.");
		}

		void IList.RemoveAt(int index)
		{
			this.Delete(index);
		}

		[MonoTODO]
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}

		object IBindingList.AddNew()
		{
			return this.AddNew();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			if (!(property is DataColumnPropertyDescriptor))
			{
				throw new ArgumentException("Dataview accepts only DataColumnPropertyDescriptors", "property");
			}
			this.sortProperty = property;
			string text = string.Format("[{0}]", property.Name);
			if (direction == ListSortDirection.Descending)
			{
				text += " DESC";
			}
			this.Sort = text;
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			DataColumn dataColumn = this.Table.Columns[property.Name];
			Index index = this.Table.FindIndex(new DataColumn[] { dataColumn }, this.sortOrder, this.RowStateFilter, this.FilterExpression);
			if (index == null)
			{
				index = new Index(new Key(this.Table, new DataColumn[] { dataColumn }, this.sortOrder, this.RowStateFilter, this.FilterExpression));
			}
			return index.FindIndex(new object[] { key });
		}

		[MonoTODO]
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}

		void IBindingList.RemoveSort()
		{
			this.sortProperty = null;
			this.Sort = string.Empty;
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
			[MonoTODO]
			get
			{
				return this.AllowDelete;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return this.Sort != null && this.Sort.Length != 0;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				if (this.sortOrder != null && this.sortOrder.Length > 0)
				{
					return this.sortOrder[0];
				}
				return ListSortDirection.Ascending;
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				if (this.sortProperty == null && this.sortColumns != null && this.sortColumns.Length > 0)
				{
					PropertyDescriptorCollection itemProperties = ((ITypedList)this).GetItemProperties(null);
					return itemProperties.Find(this.sortColumns[0].ColumnName, false);
				}
				return this.sortProperty;
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
				ListSortDescriptionCollection listSortDescriptionCollection = new ListSortDescriptionCollection();
				for (int i = 0; i < this.sortColumns.Length; i++)
				{
					ListSortDescription listSortDescription = new ListSortDescription(new DataColumnPropertyDescriptor(this.sortColumns[i]), this.sortOrder[i]);
					((IList)listSortDescriptionCollection).Add(listSortDescription);
				}
				return listSortDescriptionCollection;
			}
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

		[MonoTODO]
		void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in ((IEnumerable)sorts))
			{
				ListSortDescription listSortDescription = (ListSortDescription)obj;
				stringBuilder.AppendFormat("[{0}]{1},", listSortDescription.PropertyDescriptor.Name, (listSortDescription.SortDirection != ListSortDirection.Descending) ? string.Empty : " DESC");
			}
			this.Sort = stringBuilder.ToString(0, stringBuilder.Length - 1);
		}

		void IBindingListView.RemoveFilter()
		{
			((IBindingListView)this).Filter = string.Empty;
		}

		[DefaultValue(true)]
		[DataCategory("Data")]
		public bool AllowDelete
		{
			get
			{
				return this.allowDelete;
			}
			set
			{
				this.allowDelete = value;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(true)]
		public bool AllowEdit
		{
			get
			{
				return this.allowEdit;
			}
			set
			{
				this.allowEdit = value;
			}
		}

		[DefaultValue(true)]
		[DataCategory("Data")]
		public bool AllowNew
		{
			get
			{
				return this.allowNew;
			}
			set
			{
				this.allowNew = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DataCategory("Data")]
		[DefaultValue(false)]
		public bool ApplyDefaultSort
		{
			get
			{
				return this.applyDefaultSort;
			}
			set
			{
				if (this.isInitPhase)
				{
					this.initApplyDefaultSort = value;
					return;
				}
				if (this.applyDefaultSort == value)
				{
					return;
				}
				this.applyDefaultSort = value;
				if (this.applyDefaultSort && (this.sort == null || this.sort == string.Empty))
				{
					this.PopulateDefaultSort();
				}
				if (!this.inEndInit)
				{
					this.UpdateIndex(true);
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
				}
			}
		}

		[Browsable(false)]
		public int Count
		{
			get
			{
				return this.rowCache.Length;
			}
		}

		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				return this.dataViewManager;
			}
		}

		public DataRowView this[int recordIndex]
		{
			get
			{
				if (recordIndex > this.rowCache.Length)
				{
					throw new IndexOutOfRangeException("There is no row at position: " + recordIndex + ".");
				}
				return this.rowCache[recordIndex];
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public virtual string RowFilter
		{
			get
			{
				return this.rowFilter;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this.isInitPhase)
				{
					this.initRowFilter = value;
					return;
				}
				CultureInfo cultureInfo = ((this.Table == null) ? CultureInfo.CurrentCulture : this.Table.Locale);
				if (string.Compare(this.rowFilter, value, false, cultureInfo) == 0)
				{
					return;
				}
				if (value.Length == 0)
				{
					this.rowFilterExpr = null;
				}
				else
				{
					Parser parser = new Parser();
					this.rowFilterExpr = parser.Compile(value);
				}
				this.rowFilter = value;
				if (!this.inEndInit)
				{
					this.UpdateIndex(true);
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
				}
			}
		}

		[DataCategory("Data")]
		[DefaultValue(DataViewRowState.CurrentRows)]
		public DataViewRowState RowStateFilter
		{
			get
			{
				return this.rowState;
			}
			set
			{
				if (this.isInitPhase)
				{
					this.initRowState = value;
					return;
				}
				if (value == this.rowState)
				{
					return;
				}
				this.rowState = value;
				if (!this.inEndInit)
				{
					this.UpdateIndex(true);
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
				}
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string Sort
		{
			get
			{
				if (this.useDefaultSort)
				{
					return string.Empty;
				}
				return this.sort;
			}
			set
			{
				if (this.isInitPhase)
				{
					this.initSort = value;
					return;
				}
				if (value == this.sort)
				{
					return;
				}
				if (value == null || value.Length == 0)
				{
					this.useDefaultSort = true;
					if (this.ApplyDefaultSort)
					{
						this.PopulateDefaultSort();
					}
				}
				else
				{
					this.useDefaultSort = false;
					this.sort = value;
				}
				if (!this.inEndInit)
				{
					this.UpdateIndex(true);
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
				}
			}
		}

		[DataCategory("Data")]
		[TypeConverter(typeof(DataTableTypeConverter))]
		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		public DataTable Table
		{
			get
			{
				return this.dataTable;
			}
			set
			{
				if (value == this.dataTable)
				{
					return;
				}
				if (this.isInitPhase)
				{
					this.initTable = value;
					return;
				}
				if (value != null && value.TableName.Equals(string.Empty))
				{
					throw new DataException("Cannot bind to DataTable with no name.");
				}
				if (this.dataTable != null)
				{
					this.UnregisterEventHandlers();
				}
				this.dataTable = value;
				if (this.dataTable != null)
				{
					this.RegisterEventHandlers();
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0, 0));
					this.sort = string.Empty;
					this.rowFilter = string.Empty;
					if (!this.inEndInit)
					{
						this.UpdateIndex(true);
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
					}
				}
			}
		}

		public virtual DataRowView AddNew()
		{
			if (!this.IsOpen)
			{
				throw new DataException("DataView is not open.");
			}
			if (!this.AllowNew)
			{
				throw new DataException("Cannot call AddNew on a DataView where AllowNew is false.");
			}
			if (this._lastAdded != null)
			{
				this.CompleteLastAdded(true);
			}
			this._lastAdded = this.dataTable.NewRow();
			this.UpdateIndex(true);
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, this.Count - 1, -1));
			return this[this.Count - 1];
		}

		internal void CompleteLastAdded(bool add)
		{
			DataRow lastAdded = this._lastAdded;
			if (add)
			{
				try
				{
					this.dataTable.Rows.Add(this._lastAdded);
					this._lastAdded = null;
					this.UpdateIndex();
				}
				catch (Exception)
				{
					this._lastAdded = lastAdded;
					throw;
				}
			}
			else
			{
				this._lastAdded.CancelEdit();
				this._lastAdded = null;
				this.UpdateIndex();
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, this.Count, -1));
			}
		}

		public void BeginInit()
		{
			this.initTable = this.Table;
			this.initApplyDefaultSort = this.ApplyDefaultSort;
			this.initSort = this.Sort;
			this.initRowFilter = this.RowFilter;
			this.initRowState = this.RowStateFilter;
			this.isInitPhase = true;
			this.DataViewInitialized(false);
		}

		public void CopyTo(Array array, int index)
		{
			if (index + this.rowCache.Length > array.Length)
			{
				throw new IndexOutOfRangeException();
			}
			int num = 0;
			while (num < this.rowCache.Length && num < array.Length)
			{
				array.SetValue(this.rowCache[num], index + num);
				num++;
			}
		}

		public void Delete(int index)
		{
			if (!this.IsOpen)
			{
				throw new DataException("DataView is not open.");
			}
			if (this._lastAdded != null && index == this.Count)
			{
				this.CompleteLastAdded(false);
				return;
			}
			if (!this.AllowDelete)
			{
				throw new DataException("Cannot delete on a DataSource where AllowDelete is false.");
			}
			if (index > this.rowCache.Length)
			{
				throw new IndexOutOfRangeException("There is no row at position: " + index + ".");
			}
			DataRowView dataRowView = this.rowCache[index];
			dataRowView.Row.Delete();
		}

		public void EndInit()
		{
			this.isInitPhase = false;
			this.inEndInit = true;
			this.Table = this.initTable;
			this.ApplyDefaultSort = this.initApplyDefaultSort;
			this.Sort = this.initSort;
			this.RowFilter = this.initRowFilter;
			this.RowStateFilter = this.initRowState;
			this.inEndInit = false;
			this.UpdateIndex(true);
			this.DataViewInitialized(true);
		}

		public int Find(object key)
		{
			object[] array = new object[] { key };
			return this.Find(array);
		}

		public int Find(object[] key)
		{
			if (this.sort == null || this.sort.Length == 0)
			{
				throw new ArgumentException("Find finds a row based on a Sort order, and no Sort order is specified");
			}
			if (this.Index == null)
			{
				this.UpdateIndex(true);
			}
			int num = -1;
			try
			{
				num = this.Index.FindIndex(key);
			}
			catch (FormatException)
			{
			}
			catch (InvalidCastException)
			{
			}
			return num;
		}

		public DataRowView[] FindRows(object key)
		{
			return this.FindRows(new object[] { key });
		}

		public DataRowView[] FindRows(object[] key)
		{
			if (this.sort == null || this.sort.Length == 0)
			{
				throw new ArgumentException("Find finds a row based on a Sort order, and no Sort order is specified");
			}
			if (this.Index == null)
			{
				this.UpdateIndex(true);
			}
			int[] array = this.Index.FindAllIndexes(key);
			DataRowView[] array2 = new DataRowView[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = this.rowCache[array[i]];
			}
			return array2;
		}

		public IEnumerator GetEnumerator()
		{
			DataRowView[] array = new DataRowView[this.Count];
			this.CopyTo(array, 0);
			return array.GetEnumerator();
		}

		[Browsable(false)]
		protected bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
		}

		internal Index Index
		{
			get
			{
				return this._index;
			}
			set
			{
				if (this._index != null)
				{
					this._index.RemoveRef();
					this.Table.DropIndex(this._index);
				}
				this._index = value;
				if (this._index != null)
				{
					this._index.AddRef();
				}
			}
		}

		protected void Close()
		{
			if (this.dataTable != null)
			{
				this.UnregisterEventHandlers();
			}
			this.Index = null;
			this.rowCache = new DataRowView[0];
			this.isOpen = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
			base.Dispose(disposing);
		}

		protected virtual void IndexListChanged(object sender, ListChangedEventArgs e)
		{
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			try
			{
				if (this.ListChanged != null)
				{
					this.ListChanged(this, e);
				}
			}
			catch
			{
			}
		}

		internal void ChangedList(ListChangedType listChangedType, int newIndex, int oldIndex)
		{
			ListChangedEventArgs e = new ListChangedEventArgs(listChangedType, newIndex, oldIndex);
			this.OnListChanged(e);
		}

		protected void Open()
		{
			this.UpdateIndex(true);
			if (this.dataTable != null)
			{
				this.RegisterEventHandlers();
			}
			this.isOpen = true;
		}

		private void RegisterEventHandlers()
		{
			this.dataTable.ColumnChanged += this.OnColumnChanged;
			this.dataTable.RowChanged += this.OnRowChanged;
			this.dataTable.RowDeleted += this.OnRowDeleted;
			this.dataTable.Columns.CollectionChanged += this.ColumnCollectionChanged;
			this.dataTable.Columns.CollectionMetaDataChanged += this.ColumnCollectionChanged;
			this.dataTable.Constraints.CollectionChanged += this.OnConstraintCollectionChanged;
			this.dataTable.ChildRelations.CollectionChanged += this.OnRelationCollectionChanged;
			this.dataTable.ParentRelations.CollectionChanged += this.OnRelationCollectionChanged;
			this.dataTable.Rows.ListChanged += this.OnRowCollectionChanged;
		}

		private void OnRowCollectionChanged(object sender, ListChangedEventArgs args)
		{
			if (args.ListChangedType == ListChangedType.Reset)
			{
				this.rowCache = new DataRowView[0];
				this.UpdateIndex(true);
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
			}
		}

		private void UnregisterEventHandlers()
		{
			this.dataTable.ColumnChanged -= this.OnColumnChanged;
			this.dataTable.RowChanged -= this.OnRowChanged;
			this.dataTable.RowDeleted -= this.OnRowDeleted;
			this.dataTable.Columns.CollectionChanged -= this.ColumnCollectionChanged;
			this.dataTable.Columns.CollectionMetaDataChanged -= this.ColumnCollectionChanged;
			this.dataTable.Constraints.CollectionChanged -= this.OnConstraintCollectionChanged;
			this.dataTable.ChildRelations.CollectionChanged -= this.OnRelationCollectionChanged;
			this.dataTable.ParentRelations.CollectionChanged -= this.OnRelationCollectionChanged;
			this.dataTable.Rows.ListChanged -= this.OnRowCollectionChanged;
		}

		private void OnColumnChanged(object sender, DataColumnChangeEventArgs args)
		{
		}

		private void OnRowChanged(object sender, DataRowChangeEventArgs args)
		{
			int num = this.IndexOf(args.Row);
			this.UpdateIndex(true);
			int num2 = this.IndexOf(args.Row);
			if (args.Action == DataRowAction.Add && num != num2)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, num2, -1));
			}
			if (args.Action == DataRowAction.Change)
			{
				if (num != -1 && num == num2)
				{
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, num2, -1));
				}
				else if (num != num2)
				{
					if (num2 < 0)
					{
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, num2, num));
					}
					else
					{
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, num2, num));
					}
				}
			}
			if (args.Action == DataRowAction.Rollback)
			{
				if (num < 0 && num2 > -1)
				{
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, num2, -1));
				}
				else if (num > -1 && num2 < 0)
				{
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, num2, num));
				}
				else if (num != -1 && num == num2)
				{
					this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, num2, -1));
				}
			}
		}

		private void OnRowDeleted(object sender, DataRowChangeEventArgs args)
		{
			int count = this.Count;
			int num = this.IndexOf(args.Row);
			this.UpdateIndex(true);
			if (count != this.Count)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, num, -1));
			}
		}

		protected virtual void ColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, 0, 0));
			}
			if (e.Action == CollectionChangeAction.Remove)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, 0, 0));
			}
			if (e.Action == CollectionChangeAction.Refresh)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0, 0));
			}
		}

		private void OnConstraintCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			if (args.Action == CollectionChangeAction.Add && args.Element is UniqueConstraint && this.ApplyDefaultSort && this.useDefaultSort)
			{
				this.PopulateDefaultSort((UniqueConstraint)args.Element);
			}
		}

		private void OnRelationCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			if (args.Action == CollectionChangeAction.Add)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, 0, 0));
			}
			if (args.Action == CollectionChangeAction.Remove)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, 0, 0));
			}
			if (args.Action == CollectionChangeAction.Refresh)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0, 0));
			}
		}

		protected void Reset()
		{
			this.Close();
			this.rowCache = new DataRowView[0];
			this.Open();
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}

		protected void UpdateIndex()
		{
			this.UpdateIndex(false);
		}

		protected virtual void UpdateIndex(bool force)
		{
			if (this.Table == null)
			{
				return;
			}
			if (this.Index == null || force)
			{
				this.sortColumns = DataTable.ParseSortString(this.Table, this.Sort, out this.sortOrder, false);
				this.Index = this.dataTable.GetIndex(this.sortColumns, this.sortOrder, this.RowStateFilter, this.FilterExpression, true);
			}
			else
			{
				this.Index.Key.RowStateFilter = this.RowStateFilter;
				this.Index.Reset();
			}
			int[] all = this.Index.GetAll();
			if (all != null)
			{
				this.InitDataRowViewArray(all, this.Index.Size);
			}
			else
			{
				this.rowCache = new DataRowView[0];
			}
		}

		internal virtual IExpression FilterExpression
		{
			get
			{
				return this.rowFilterExpr;
			}
		}

		private void InitDataRowViewArray(int[] records, int size)
		{
			if (this._lastAdded != null)
			{
				this.rowCache = new DataRowView[size + 1];
			}
			else
			{
				this.rowCache = new DataRowView[size];
			}
			for (int i = 0; i < size; i++)
			{
				this.rowCache[i] = new DataRowView(this, this.Table.RecordCache[records[i]], i);
			}
			if (this._lastAdded != null)
			{
				this.rowCache[size] = new DataRowView(this, this._lastAdded, size);
			}
		}

		private int IndexOf(DataRow dr)
		{
			for (int i = 0; i < this.rowCache.Length; i++)
			{
				if (dr.Equals(this.rowCache[i].Row))
				{
					return i;
				}
			}
			return -1;
		}

		private void PopulateDefaultSort()
		{
			this.sort = string.Empty;
			foreach (object obj in this.dataTable.Constraints)
			{
				Constraint constraint = (Constraint)obj;
				if (constraint is UniqueConstraint)
				{
					this.PopulateDefaultSort((UniqueConstraint)constraint);
					break;
				}
			}
		}

		private void PopulateDefaultSort(UniqueConstraint uc)
		{
			if (this.isInitPhase)
			{
				return;
			}
			DataColumn[] columns = uc.Columns;
			if (columns.Length == 0)
			{
				this.sort = string.Empty;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(columns[0].ColumnName);
			for (int i = 1; i < columns.Length; i++)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(columns[i].ColumnName);
			}
			this.sort = stringBuilder.ToString();
		}

		internal DataView CreateChildView(DataRelation relation, int index)
		{
			if (relation == null || relation.ParentTable != this.Table)
			{
				throw new ArgumentException("The relation is not parented to the table to which this DataView points.");
			}
			int record = this.GetRecord(index);
			object[] array = new object[relation.ParentColumns.Length];
			for (int i = 0; i < relation.ParentColumns.Length; i++)
			{
				array[i] = relation.ParentColumns[i][record];
			}
			return new RelatedDataView(relation.ChildColumns, array);
		}

		private int GetRecord(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new IndexOutOfRangeException(string.Format("There is no row at position {0}.", index));
			}
			return (index != this.Index.Size) ? this.Index.IndexToRecord(index) : this._lastAdded.IndexFromVersion(DataRowVersion.Default);
		}

		internal DataRowVersion GetRowVersion(int index)
		{
			int record = this.GetRecord(index);
			return this.Table.RecordCache[record].VersionFromIndex(record);
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return this.dataViewInitialized;
			}
		}

		private void DataViewInitialized(bool value)
		{
			this.dataViewInitialized = value;
			if (value)
			{
				this.OnDataViewInitialized(new EventArgs());
			}
		}

		private void OnDataViewInitialized(EventArgs e)
		{
			if (this.Initialized != null)
			{
				this.Initialized(this, e);
			}
		}

		public virtual bool Equals(DataView dv)
		{
			if (this == dv)
			{
				return true;
			}
			if (this.Table != dv.Table || !(this.Sort == dv.Sort) || !(this.RowFilter == dv.RowFilter) || this.RowStateFilter != dv.RowStateFilter || this.AllowEdit != dv.AllowEdit || this.AllowNew != dv.AllowNew || this.AllowDelete != dv.AllowDelete || this.Count != dv.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Count; i++)
			{
				if (!this[i].Equals(dv[i]))
				{
					return false;
				}
			}
			return true;
		}

		public DataTable ToTable()
		{
			return this.ToTable(this.Table.TableName, false, new string[0]);
		}

		public DataTable ToTable(string tableName)
		{
			return this.ToTable(tableName, false, new string[0]);
		}

		public DataTable ToTable(bool isDistinct, params string[] columnNames)
		{
			return this.ToTable(this.Table.TableName, isDistinct, columnNames);
		}

		public DataTable ToTable(string tablename, bool isDistinct, params string[] columnNames)
		{
			if (columnNames == null)
			{
				throw new ArgumentNullException("columnNames", "'columnNames' argument cannot be null.");
			}
			DataTable dataTable = new DataTable(tablename);
			ListSortDirection[] array = null;
			DataColumn[] array2;
			if (columnNames.Length > 0)
			{
				array2 = new DataColumn[columnNames.Length];
				for (int i = 0; i < columnNames.Length; i++)
				{
					array2[i] = this.Table.Columns[columnNames[i]];
				}
				if (this.sortColumns != null)
				{
					array = new ListSortDirection[columnNames.Length];
					for (int j = 0; j < columnNames.Length; j++)
					{
						array[j] = ListSortDirection.Ascending;
						for (int k = 0; k < this.sortColumns.Length; k++)
						{
							if (this.sortColumns[k] == array2[j])
							{
								array[j] = this.sortOrder[k];
							}
						}
					}
				}
			}
			else
			{
				array2 = (DataColumn[])this.Table.Columns.ToArray(typeof(DataColumn));
				array = this.sortOrder;
			}
			ArrayList arrayList = new ArrayList();
			for (int l = 0; l < array2.Length; l++)
			{
				DataColumn dataColumn = array2[l].Clone();
				if (dataColumn.Expression != string.Empty)
				{
					dataColumn.Expression = string.Empty;
					arrayList.Add(dataColumn);
				}
				if (dataColumn.ReadOnly)
				{
					dataColumn.ReadOnly = false;
				}
				dataTable.Columns.Add(dataColumn);
			}
			Index index;
			if (this.sort != string.Empty)
			{
				index = this.Table.GetIndex(this.sortColumns, this.sortOrder, this.RowStateFilter, this.FilterExpression, true);
			}
			else
			{
				index = new Index(new Key(this.Table, array2, array, this.RowStateFilter, this.rowFilterExpr));
			}
			DataRow[] array3;
			if (isDistinct)
			{
				array3 = index.GetDistinctRows();
			}
			else
			{
				array3 = index.GetAllRows();
			}
			foreach (DataRow dataRow in array3)
			{
				DataRow dataRow2 = dataTable.NewNotInitializedRow();
				dataTable.Rows.AddInternal(dataRow2);
				dataRow2.Original = -1;
				if (dataRow.HasVersion(DataRowVersion.Current))
				{
					dataRow2.Current = dataTable.RecordCache.CopyRecord(this.Table, dataRow.Current, -1);
				}
				else if (dataRow.HasVersion(DataRowVersion.Original))
				{
					dataRow2.Current = dataTable.RecordCache.CopyRecord(this.Table, dataRow.Original, -1);
				}
				foreach (object obj in arrayList)
				{
					DataColumn dataColumn2 = (DataColumn)obj;
					dataRow2[dataColumn2] = dataRow[dataColumn2.ColumnName];
				}
				dataRow2.Original = -1;
			}
			return dataTable;
		}

		internal DataTable dataTable;

		private string rowFilter = string.Empty;

		private IExpression rowFilterExpr;

		private string sort = string.Empty;

		private ListSortDirection[] sortOrder;

		private PropertyDescriptor sortProperty;

		private DataColumn[] sortColumns;

		internal DataViewRowState rowState;

		internal DataRowView[] rowCache = new DataRowView[0];

		private bool isInitPhase;

		private bool inEndInit;

		private DataTable initTable;

		private bool initApplyDefaultSort;

		private string initSort;

		private string initRowFilter;

		private DataViewRowState initRowState;

		private bool allowNew = true;

		private bool allowEdit = true;

		private bool allowDelete = true;

		private bool applyDefaultSort;

		private bool isOpen;

		private bool useDefaultSort = true;

		private Index _index;

		internal DataRow _lastAdded;

		private DataViewManager dataViewManager;

		internal static ListChangedEventArgs ListResetEventArgs = new ListChangedEventArgs(ListChangedType.Reset, -1, -1);

		private bool dataViewInitialized = true;
	}
}
