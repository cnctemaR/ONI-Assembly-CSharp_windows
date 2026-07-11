using System;
using System.ComponentModel;

namespace System.Data
{
	internal sealed class DataTableReaderListener
	{
		internal DataTableReaderListener(DataTableReader reader)
		{
			if (reader == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTableReader");
			}
			if (this._currentDataTable != null)
			{
				this.UnSubscribeEvents();
			}
			this._readerWeak = new WeakReference(reader);
			this._currentDataTable = reader.CurrentDataTable;
			if (this._currentDataTable != null)
			{
				this.SubscribeEvents();
			}
		}

		internal void CleanUp()
		{
			this.UnSubscribeEvents();
		}

		internal void UpdataTable(DataTable datatable)
		{
			if (datatable == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTable");
			}
			this.UnSubscribeEvents();
			this._currentDataTable = datatable;
			this.SubscribeEvents();
		}

		private void SubscribeEvents()
		{
			if (this._currentDataTable == null)
			{
				return;
			}
			if (this._isSubscribed)
			{
				return;
			}
			this._currentDataTable.Columns.ColumnPropertyChanged += this.SchemaChanged;
			this._currentDataTable.Columns.CollectionChanged += this.SchemaChanged;
			this._currentDataTable.RowChanged += this.DataChanged;
			this._currentDataTable.RowDeleted += this.DataChanged;
			this._currentDataTable.TableCleared += this.DataTableCleared;
			this._isSubscribed = true;
		}

		private void UnSubscribeEvents()
		{
			if (this._currentDataTable == null)
			{
				return;
			}
			if (!this._isSubscribed)
			{
				return;
			}
			this._currentDataTable.Columns.ColumnPropertyChanged -= this.SchemaChanged;
			this._currentDataTable.Columns.CollectionChanged -= this.SchemaChanged;
			this._currentDataTable.RowChanged -= this.DataChanged;
			this._currentDataTable.RowDeleted -= this.DataChanged;
			this._currentDataTable.TableCleared -= this.DataTableCleared;
			this._isSubscribed = false;
		}

		private void DataTableCleared(object sender, DataTableClearEventArgs e)
		{
			DataTableReader dataTableReader = (DataTableReader)this._readerWeak.Target;
			if (dataTableReader != null)
			{
				dataTableReader.DataTableCleared();
				return;
			}
			this.UnSubscribeEvents();
		}

		private void SchemaChanged(object sender, CollectionChangeEventArgs e)
		{
			DataTableReader dataTableReader = (DataTableReader)this._readerWeak.Target;
			if (dataTableReader != null)
			{
				dataTableReader.SchemaChanged();
				return;
			}
			this.UnSubscribeEvents();
		}

		private void DataChanged(object sender, DataRowChangeEventArgs args)
		{
			DataTableReader dataTableReader = (DataTableReader)this._readerWeak.Target;
			if (dataTableReader != null)
			{
				dataTableReader.DataChanged(args);
				return;
			}
			this.UnSubscribeEvents();
		}

		private DataTable _currentDataTable;

		private bool _isSubscribed;

		private WeakReference _readerWeak;
	}
}
