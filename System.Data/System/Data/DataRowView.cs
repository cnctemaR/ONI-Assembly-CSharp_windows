using System;
using System.ComponentModel;

namespace System.Data
{
	public class DataRowView : ICustomTypeDescriptor, IEditableObject, IDataErrorInfo, INotifyPropertyChanged
	{
		internal DataRowView(DataView dataView, DataRow row, int index)
		{
			this._dataView = dataView;
			this._dataRow = row;
			this._index = index;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		[MonoTODO("Not implemented.   Always returns String.Empty")]
		string ICustomTypeDescriptor.GetClassName()
		{
			return string.Empty;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		[MonoTODO("Not implemented.   Always returns null")]
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		[MonoTODO("Not implemented.   Always returns an empty collection")]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		[MonoTODO("Not implemented.   Always returns an empty collection")]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			if (this.DataView == null)
			{
				ITypedList dataView = this._dataView;
				return dataView.GetItemProperties(new PropertyDescriptor[0]);
			}
			return this.DataView.Table.GetPropertyDescriptorCollection();
		}

		[MonoTODO("It currently reports more descriptors than necessary")]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return ((ICustomTypeDescriptor)this).GetProperties();
		}

		[MonoTODO]
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		string IDataErrorInfo.Error
		{
			[MonoTODO("Not implemented, always returns String.Empty")]
			get
			{
				return string.Empty;
			}
		}

		string IDataErrorInfo.this[string colName]
		{
			[MonoTODO("Not implemented, always returns String.Empty")]
			get
			{
				return string.Empty;
			}
		}

		public override bool Equals(object other)
		{
			return other != null && other is DataRowView && ((DataRowView)other)._dataRow != null && ((DataRowView)other)._dataRow.Equals(this._dataRow);
		}

		public void BeginEdit()
		{
			this._dataRow.BeginEdit();
		}

		public void CancelEdit()
		{
			if (this.Row == this.DataView._lastAdded)
			{
				this.DataView.CompleteLastAdded(false);
			}
			else
			{
				this._dataRow.CancelEdit();
			}
		}

		public DataView CreateChildView(DataRelation relation)
		{
			return this.DataView.CreateChildView(relation, this._index);
		}

		public DataView CreateChildView(string relationName)
		{
			return this.CreateChildView(this.Row.Table.ChildRelations[relationName]);
		}

		public void Delete()
		{
			this.DataView.Delete(this._index);
		}

		public void EndEdit()
		{
			if (this.Row == this.DataView._lastAdded)
			{
				this.DataView.CompleteLastAdded(true);
			}
			else
			{
				this._dataRow.EndEdit();
			}
		}

		private void CheckAllowEdit()
		{
			if (!this.DataView.AllowEdit && this.Row != this.DataView._lastAdded)
			{
				throw new DataException("Cannot edit on a DataSource where AllowEdit is false.");
			}
		}

		public DataView DataView
		{
			get
			{
				return this._dataView;
			}
		}

		public bool IsEdit
		{
			get
			{
				return this._dataRow.HasVersion(DataRowVersion.Proposed);
			}
		}

		public bool IsNew
		{
			get
			{
				return this.Row == this.DataView._lastAdded;
			}
		}

		public object this[string property]
		{
			get
			{
				DataColumn dataColumn = this._dataView.Table.Columns[property];
				if (dataColumn == null)
				{
					throw new ArgumentException(property + " is neither a DataColumn nor a DataRelation for table " + this._dataView.Table.TableName);
				}
				return this._dataRow[dataColumn, this.GetActualRowVersion()];
			}
			set
			{
				this.CheckAllowEdit();
				DataColumn dataColumn = this._dataView.Table.Columns[property];
				if (dataColumn == null)
				{
					throw new ArgumentException(property + " is neither a DataColumn nor a DataRelation for table " + this._dataView.Table.TableName);
				}
				this._dataRow[dataColumn] = value;
			}
		}

		public object this[int ndx]
		{
			get
			{
				DataColumn dataColumn = this._dataView.Table.Columns[ndx];
				if (dataColumn == null)
				{
					throw new ArgumentException(ndx + " is neither a DataColumn nor a DataRelation for table " + this._dataView.Table.TableName);
				}
				return this._dataRow[dataColumn, this.GetActualRowVersion()];
			}
			set
			{
				this.CheckAllowEdit();
				DataColumn dataColumn = this._dataView.Table.Columns[ndx];
				if (dataColumn == null)
				{
					throw new ArgumentException(ndx + " is neither a DataColumn nor a DataRelation for table " + this._dataView.Table.TableName);
				}
				this._dataRow[dataColumn] = value;
			}
		}

		private DataRowVersion GetActualRowVersion()
		{
			DataViewRowState rowStateFilter = this._dataView.RowStateFilter;
			switch (rowStateFilter)
			{
			case DataViewRowState.Unchanged:
				break;
			default:
				if (rowStateFilter != DataViewRowState.Deleted)
				{
					if (rowStateFilter == DataViewRowState.ModifiedCurrent)
					{
						return DataRowVersion.Current;
					}
					if (rowStateFilter != DataViewRowState.ModifiedOriginal && rowStateFilter != DataViewRowState.OriginalRows)
					{
						return DataRowVersion.Default;
					}
				}
				break;
			case DataViewRowState.Added:
				return DataRowVersion.Proposed;
			}
			return DataRowVersion.Original;
		}

		public DataRow Row
		{
			get
			{
				return this._dataRow;
			}
		}

		public DataRowVersion RowVersion
		{
			get
			{
				DataRowVersion dataRowVersion = this.DataView.GetRowVersion(this._index);
				if (dataRowVersion != DataRowVersion.Original)
				{
					dataRowVersion = DataRowVersion.Current;
				}
				return dataRowVersion;
			}
		}

		public override int GetHashCode()
		{
			return this._dataRow.GetHashCode();
		}

		internal int Index
		{
			get
			{
				return this._index;
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
				this.PropertyChanged(this, e);
			}
		}

		private DataView _dataView;

		private DataRow _dataRow;

		private int _index = -1;
	}
}
