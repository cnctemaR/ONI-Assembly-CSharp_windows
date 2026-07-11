using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Data
{
	public class DataViewManager : MarshalByValueComponent, IBindingList, IList, ICollection, IEnumerable, ITypedList
	{
		public DataViewManager()
			: this(null, false)
		{
		}

		public DataViewManager(DataSet dataSet)
			: this(dataSet, false)
		{
		}

		internal DataViewManager(DataSet dataSet, bool locked)
		{
			GC.SuppressFinalize(this);
			this._dataSet = dataSet;
			if (this._dataSet != null)
			{
				this._dataSet.Tables.CollectionChanged += this.TableCollectionChanged;
				this._dataSet.Relations.CollectionChanged += this.RelationCollectionChanged;
			}
			this._locked = locked;
			this._item = new DataViewManagerListItemTypeDescriptor(this);
			this._dataViewSettingsCollection = new DataViewSettingCollection(this);
		}

		[DefaultValue(null)]
		public DataSet DataSet
		{
			get
			{
				return this._dataSet;
			}
			set
			{
				if (value == null)
				{
					throw ExceptionBuilder.SetFailed("DataSet to null");
				}
				if (this._locked)
				{
					throw ExceptionBuilder.SetDataSetFailed();
				}
				if (this._dataSet != null)
				{
					if (this._nViews > 0)
					{
						throw ExceptionBuilder.CanNotSetDataSet();
					}
					this._dataSet.Tables.CollectionChanged -= this.TableCollectionChanged;
					this._dataSet.Relations.CollectionChanged -= this.RelationCollectionChanged;
				}
				this._dataSet = value;
				this._dataSet.Tables.CollectionChanged += this.TableCollectionChanged;
				this._dataSet.Relations.CollectionChanged += this.RelationCollectionChanged;
				this._dataViewSettingsCollection = new DataViewSettingCollection(this);
				this._item.Reset();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewSettingCollection DataViewSettings
		{
			get
			{
				return this._dataViewSettingsCollection;
			}
		}

		public string DataViewSettingCollectionString
		{
			get
			{
				if (this._dataSet == null)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<DataViewSettingCollectionString>");
				foreach (object obj in this._dataSet.Tables)
				{
					DataTable dataTable = (DataTable)obj;
					DataViewSetting dataViewSetting = this._dataViewSettingsCollection[dataTable];
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "<{0} Sort=\"{1}\" RowFilter=\"{2}\" RowStateFilter=\"{3}\"/>", new object[] { dataTable.EncodedTableName, dataViewSetting.Sort, dataViewSetting.RowFilter, dataViewSetting.RowStateFilter });
				}
				stringBuilder.Append("</DataViewSettingCollectionString>");
				return stringBuilder.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(value));
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
				xmlTextReader.Read();
				if (xmlTextReader.Name != "DataViewSettingCollectionString")
				{
					throw ExceptionBuilder.SetFailed("DataViewSettingCollectionString");
				}
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.NodeType == XmlNodeType.Element)
					{
						string text = XmlConvert.DecodeName(xmlTextReader.LocalName);
						if (xmlTextReader.MoveToAttribute("Sort"))
						{
							this._dataViewSettingsCollection[text].Sort = xmlTextReader.Value;
						}
						if (xmlTextReader.MoveToAttribute("RowFilter"))
						{
							this._dataViewSettingsCollection[text].RowFilter = xmlTextReader.Value;
						}
						if (xmlTextReader.MoveToAttribute("RowStateFilter"))
						{
							this._dataViewSettingsCollection[text].RowStateFilter = (DataViewRowState)Enum.Parse(typeof(DataViewRowState), xmlTextReader.Value);
						}
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			DataViewManagerListItemTypeDescriptor[] array = new DataViewManagerListItemTypeDescriptor[1];
			((ICollection)this).CopyTo(array, 0);
			return array.GetEnumerator();
		}

		int ICollection.Count
		{
			get
			{
				return 1;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool ICollection.IsSynchronized
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
				return true;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			array.SetValue(new DataViewManagerListItemTypeDescriptor(this), index);
		}

		object IList.this[int index]
		{
			get
			{
				return this._item;
			}
			set
			{
				throw ExceptionBuilder.CannotModifyCollection();
			}
		}

		int IList.Add(object value)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		void IList.Clear()
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		bool IList.Contains(object value)
		{
			return value == this._item;
		}

		int IList.IndexOf(object value)
		{
			if (value != this._item)
			{
				return -1;
			}
			return 1;
		}

		void IList.Insert(int index, object value)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		void IList.Remove(object value)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		void IList.RemoveAt(int index)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return false;
			}
		}

		object IBindingList.AddNew()
		{
			throw DataViewManager.s_notSupported;
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return false;
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
				return false;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				throw DataViewManager.s_notSupported;
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				throw DataViewManager.s_notSupported;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				throw DataViewManager.s_notSupported;
			}
		}

		public event ListChangedEventHandler ListChanged;

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw DataViewManager.s_notSupported;
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw DataViewManager.s_notSupported;
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.RemoveSort()
		{
			throw DataViewManager.s_notSupported;
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			DataSet dataSet = this.DataSet;
			if (dataSet == null)
			{
				throw ExceptionBuilder.CanNotUseDataViewManager();
			}
			if (listAccessors == null || listAccessors.Length == 0)
			{
				return dataSet.DataSetName;
			}
			DataTable dataTable = dataSet.FindTable(null, listAccessors, 0);
			if (dataTable != null)
			{
				return dataTable.TableName;
			}
			return string.Empty;
		}

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			DataSet dataSet = this.DataSet;
			if (dataSet == null)
			{
				throw ExceptionBuilder.CanNotUseDataViewManager();
			}
			if (listAccessors == null || listAccessors.Length == 0)
			{
				return ((ICustomTypeDescriptor)new DataViewManagerListItemTypeDescriptor(this)).GetProperties();
			}
			DataTable dataTable = dataSet.FindTable(null, listAccessors, 0);
			if (dataTable != null)
			{
				return dataTable.GetPropertyDescriptorCollection(null);
			}
			return new PropertyDescriptorCollection(null);
		}

		public DataView CreateDataView(DataTable table)
		{
			if (this._dataSet == null)
			{
				throw ExceptionBuilder.CanNotUseDataViewManager();
			}
			DataView dataView = new DataView(table);
			dataView.SetDataViewManager(this);
			return dataView;
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			try
			{
				ListChangedEventHandler listChanged = this.ListChanged;
				if (listChanged != null)
				{
					listChanged(this, e);
				}
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
			}
		}

		protected virtual void TableCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			PropertyDescriptor propertyDescriptor = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataTablePropertyDescriptor((DataTable)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, propertyDescriptor) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataTablePropertyDescriptor((DataTable)e.Element)) : null)));
		}

		protected virtual void RelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataRelationPropertyDescriptor dataRelationPropertyDescriptor = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, dataRelationPropertyDescriptor) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : null)));
		}

		private DataViewSettingCollection _dataViewSettingsCollection;

		private DataSet _dataSet;

		private DataViewManagerListItemTypeDescriptor _item;

		private bool _locked;

		internal int _nViews;

		private static NotSupportedException s_notSupported = new NotSupportedException();
	}
}
