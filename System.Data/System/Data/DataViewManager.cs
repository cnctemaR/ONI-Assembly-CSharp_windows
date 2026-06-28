using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace System.Data
{
	[Designer("Microsoft.VSDesigner.Data.VS.DataViewManagerDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	public class DataViewManager : MarshalByValueComponent, IList, IEnumerable, ITypedList, IBindingList, ICollection
	{
		public DataViewManager()
			: this(null)
		{
		}

		public DataViewManager(DataSet dataSet)
		{
			this.SetDataSet(dataSet);
		}

		public event ListChangedEventHandler ListChanged;

		int ICollection.Count
		{
			get
			{
				return 1;
			}
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
				return true;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		object IList.this[int index]
		{
			get
			{
				if (this.descriptor == null)
				{
					this.descriptor = new DataViewManagerListItemTypeDescriptor(this);
				}
				return this.descriptor;
			}
			set
			{
				throw new ArgumentException("Not modifiable");
			}
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.AllowNew
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

		bool IBindingList.IsSorted
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				throw new NotSupportedException();
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

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		object IBindingList.AddNew()
		{
			throw new NotSupportedException();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			array.SetValue(this.descriptor, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			DataViewManagerListItemTypeDescriptor[] array = new DataViewManagerListItemTypeDescriptor[((ICollection)this).Count];
			((ICollection)this).CopyTo(array, 0);
			return array.GetEnumerator();
		}

		int IList.Add(object value)
		{
			throw new ArgumentException("Not modifiable");
		}

		void IList.Clear()
		{
			throw new ArgumentException("Not modifiable");
		}

		bool IList.Contains(object value)
		{
			return value == this.descriptor;
		}

		int IList.IndexOf(object value)
		{
			if (value == this.descriptor)
			{
				return 0;
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			throw new ArgumentException("Not modifiable");
		}

		void IList.Remove(object value)
		{
			throw new ArgumentException("Not modifiable");
		}

		void IList.RemoveAt(int index)
		{
			throw new ArgumentException("Not modifiable");
		}

		[MonoLimitation("Supported only empty list of listAccessors")]
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (this.dataSet == null)
			{
				throw new DataException("dataset is null");
			}
			if (listAccessors == null || listAccessors.Length == 0)
			{
				ICustomTypeDescriptor customTypeDescriptor = new DataViewManagerListItemTypeDescriptor(this);
				return customTypeDescriptor.GetProperties();
			}
			throw new NotImplementedException();
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (this.dataSet != null && (listAccessors == null || listAccessors.Length == 0))
			{
				return this.dataSet.DataSetName;
			}
			return string.Empty;
		}

		[DefaultValue(null)]
		public DataSet DataSet
		{
			get
			{
				return this.dataSet;
			}
			set
			{
				if (value == null)
				{
					throw new DataException("Cannot set null DataSet.");
				}
				this.SetDataSet(value);
			}
		}

		public string DataViewSettingCollectionString
		{
			get
			{
				return this.xml;
			}
			set
			{
				try
				{
					this.ParseSettingString(value);
					this.xml = this.BuildSettingString();
				}
				catch (XmlException ex)
				{
					throw new DataException("Cannot set DataViewSettingCollectionString.", ex);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewSettingCollection DataViewSettings
		{
			get
			{
				return this.settings;
			}
		}

		private void SetDataSet(DataSet ds)
		{
			if (this.dataSet != null)
			{
				this.dataSet.Tables.CollectionChanged -= this.TableCollectionChanged;
				this.dataSet.Relations.CollectionChanged -= this.RelationCollectionChanged;
			}
			this.dataSet = ds;
			this.settings = new DataViewSettingCollection(this);
			this.xml = this.BuildSettingString();
			if (this.dataSet != null)
			{
				this.dataSet.Tables.CollectionChanged += this.TableCollectionChanged;
				this.dataSet.Relations.CollectionChanged += this.RelationCollectionChanged;
			}
		}

		private void ParseSettingString(string source)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(source, XmlNodeType.Element, null);
			xmlTextReader.Read();
			if (xmlTextReader.Name != "DataViewSettingCollectionString")
			{
				xmlTextReader.ReadStartElement("DataViewSettingCollectionString");
			}
			if (xmlTextReader.IsEmptyElement)
			{
				return;
			}
			xmlTextReader.Read();
			do
			{
				xmlTextReader.MoveToContent();
				if (xmlTextReader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (xmlTextReader.NodeType == XmlNodeType.Element)
				{
					this.ReadTableSetting(xmlTextReader);
				}
				else
				{
					xmlTextReader.Skip();
				}
			}
			while (!xmlTextReader.EOF);
			if (xmlTextReader.NodeType == XmlNodeType.EndElement)
			{
				xmlTextReader.ReadEndElement();
			}
		}

		private void ReadTableSetting(XmlReader reader)
		{
			DataTable dataTable = this.DataSet.Tables[XmlConvert.DecodeName(reader.LocalName)];
			DataViewSetting dataViewSetting = this.settings[dataTable];
			string attribute = reader.GetAttribute("Sort");
			if (attribute != null)
			{
				dataViewSetting.Sort = attribute.Trim();
			}
			string attribute2 = reader.GetAttribute("ApplyDefaultSort");
			if (attribute2 != null && attribute2.Trim() == "true")
			{
				dataViewSetting.ApplyDefaultSort = true;
			}
			string attribute3 = reader.GetAttribute("RowFilter");
			if (attribute3 != null)
			{
				dataViewSetting.RowFilter = attribute3.Trim();
			}
			string attribute4 = reader.GetAttribute("RowStateFilter");
			if (attribute4 != null)
			{
				dataViewSetting.RowStateFilter = (DataViewRowState)((int)Enum.Parse(typeof(DataViewRowState), attribute4.Trim()));
			}
			reader.Skip();
		}

		private string BuildSettingString()
		{
			if (this.dataSet == null)
			{
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter();
			stringWriter.Write('<');
			stringWriter.Write("DataViewSettingCollectionString>");
			foreach (object obj in this.DataViewSettings)
			{
				DataViewSetting dataViewSetting = (DataViewSetting)obj;
				stringWriter.Write('<');
				stringWriter.Write(XmlConvert.EncodeName(dataViewSetting.Table.TableName));
				stringWriter.Write(" Sort=\"");
				stringWriter.Write(this.Escape(dataViewSetting.Sort));
				stringWriter.Write('"');
				if (dataViewSetting.ApplyDefaultSort)
				{
					stringWriter.Write(" ApplyDefaultSort=\"true\"");
				}
				stringWriter.Write(" RowFilter=\"");
				stringWriter.Write(this.Escape(dataViewSetting.RowFilter));
				stringWriter.Write("\" RowStateFilter=\"");
				stringWriter.Write(dataViewSetting.RowStateFilter.ToString());
				stringWriter.Write("\"/>");
			}
			stringWriter.Write("</DataViewSettingCollectionString>");
			return stringWriter.ToString();
		}

		private string Escape(string s)
		{
			return s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}

		public DataView CreateDataView(DataTable table)
		{
			if (this.settings[table] != null)
			{
				DataViewSetting dataViewSetting = this.settings[table];
				return new DataView(table, this, dataViewSetting.Sort, dataViewSetting.RowFilter, dataViewSetting.RowStateFilter);
			}
			return new DataView(table);
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			if (this.ListChanged != null)
			{
				this.ListChanged(this, e);
			}
		}

		protected virtual void RelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			this.OnListChanged(this.CollectionToListChangeEventArgs(e));
		}

		protected virtual void TableCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			this.OnListChanged(this.CollectionToListChangeEventArgs(e));
		}

		private ListChangedEventArgs CollectionToListChangeEventArgs(CollectionChangeEventArgs e)
		{
			ListChangedEventArgs e2;
			if (e.Action == CollectionChangeAction.Remove)
			{
				e2 = null;
			}
			else if (e.Action == CollectionChangeAction.Refresh)
			{
				e2 = new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, null);
			}
			else
			{
				object obj;
				if (typeof(DataTable).IsAssignableFrom(e.Element.GetType()))
				{
					obj = new DataTablePropertyDescriptor((DataTable)e.Element);
				}
				else
				{
					obj = new DataRelationPropertyDescriptor((DataRelation)e.Element);
				}
				if (e.Action == CollectionChangeAction.Add)
				{
					e2 = new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, (PropertyDescriptor)obj);
				}
				else
				{
					e2 = new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, (PropertyDescriptor)obj);
				}
			}
			return e2;
		}

		private DataSet dataSet;

		private DataViewManagerListItemTypeDescriptor descriptor;

		private DataViewSettingCollection settings;

		private string xml;
	}
}
