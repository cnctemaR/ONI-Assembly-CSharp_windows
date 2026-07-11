using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Data
{
	internal sealed class DataViewListener
	{
		internal DataViewListener(DataView dv)
		{
			this._objectID = dv.ObjectID;
			this._dvWeak = new WeakReference(dv);
		}

		private void ChildRelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataView dataView = (DataView)this._dvWeak.Target;
			if (dataView != null)
			{
				dataView.ChildRelationCollectionChanged(sender, e);
				return;
			}
			this.CleanUp(true);
		}

		private void ParentRelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataView dataView = (DataView)this._dvWeak.Target;
			if (dataView != null)
			{
				dataView.ParentRelationCollectionChanged(sender, e);
				return;
			}
			this.CleanUp(true);
		}

		private void ColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataView dataView = (DataView)this._dvWeak.Target;
			if (dataView != null)
			{
				dataView.ColumnCollectionChangedInternal(sender, e);
				return;
			}
			this.CleanUp(true);
		}

		internal void MaintainDataView(ListChangedType changedType, DataRow row, bool trackAddRemove)
		{
			DataView dataView = (DataView)this._dvWeak.Target;
			if (dataView != null)
			{
				dataView.MaintainDataView(changedType, row, trackAddRemove);
				return;
			}
			this.CleanUp(true);
		}

		internal void IndexListChanged(ListChangedEventArgs e)
		{
			DataView dataView = (DataView)this._dvWeak.Target;
			if (dataView != null)
			{
				dataView.IndexListChangedInternal(e);
				return;
			}
			this.CleanUp(true);
		}

		internal void RegisterMetaDataEvents(DataTable table)
		{
			this._table = table;
			if (table != null)
			{
				this.RegisterListener(table);
				CollectionChangeEventHandler collectionChangeEventHandler = new CollectionChangeEventHandler(this.ColumnCollectionChanged);
				table.Columns.ColumnPropertyChanged += collectionChangeEventHandler;
				table.Columns.CollectionChanged += collectionChangeEventHandler;
				CollectionChangeEventHandler collectionChangeEventHandler2 = new CollectionChangeEventHandler(this.ChildRelationCollectionChanged);
				((DataRelationCollection.DataTableRelationCollection)table.ChildRelations).RelationPropertyChanged += collectionChangeEventHandler2;
				table.ChildRelations.CollectionChanged += collectionChangeEventHandler2;
				CollectionChangeEventHandler collectionChangeEventHandler3 = new CollectionChangeEventHandler(this.ParentRelationCollectionChanged);
				((DataRelationCollection.DataTableRelationCollection)table.ParentRelations).RelationPropertyChanged += collectionChangeEventHandler3;
				table.ParentRelations.CollectionChanged += collectionChangeEventHandler3;
			}
		}

		internal void UnregisterMetaDataEvents()
		{
			this.UnregisterMetaDataEvents(true);
		}

		private void UnregisterMetaDataEvents(bool updateListeners)
		{
			DataTable table = this._table;
			this._table = null;
			if (table != null)
			{
				CollectionChangeEventHandler collectionChangeEventHandler = new CollectionChangeEventHandler(this.ColumnCollectionChanged);
				table.Columns.ColumnPropertyChanged -= collectionChangeEventHandler;
				table.Columns.CollectionChanged -= collectionChangeEventHandler;
				CollectionChangeEventHandler collectionChangeEventHandler2 = new CollectionChangeEventHandler(this.ChildRelationCollectionChanged);
				((DataRelationCollection.DataTableRelationCollection)table.ChildRelations).RelationPropertyChanged -= collectionChangeEventHandler2;
				table.ChildRelations.CollectionChanged -= collectionChangeEventHandler2;
				CollectionChangeEventHandler collectionChangeEventHandler3 = new CollectionChangeEventHandler(this.ParentRelationCollectionChanged);
				((DataRelationCollection.DataTableRelationCollection)table.ParentRelations).RelationPropertyChanged -= collectionChangeEventHandler3;
				table.ParentRelations.CollectionChanged -= collectionChangeEventHandler3;
				if (updateListeners)
				{
					List<DataViewListener> listeners = table.GetListeners();
					List<DataViewListener> list = listeners;
					lock (list)
					{
						listeners.Remove(this);
					}
				}
			}
		}

		internal void RegisterListChangedEvent(Index index)
		{
			this._index = index;
			if (index != null)
			{
				lock (index)
				{
					index.AddRef();
					index.ListChangedAdd(this);
				}
			}
		}

		internal void UnregisterListChangedEvent()
		{
			Index index = this._index;
			this._index = null;
			if (index != null)
			{
				Index index2 = index;
				lock (index2)
				{
					index.ListChangedRemove(this);
					if (index.RemoveRef() <= 1)
					{
						index.RemoveRef();
					}
				}
			}
		}

		private void CleanUp(bool updateListeners)
		{
			this.UnregisterMetaDataEvents(updateListeners);
			this.UnregisterListChangedEvent();
		}

		private void RegisterListener(DataTable table)
		{
			List<DataViewListener> listeners = table.GetListeners();
			List<DataViewListener> list = listeners;
			lock (list)
			{
				int num = listeners.Count - 1;
				while (0 <= num)
				{
					DataViewListener dataViewListener = listeners[num];
					if (!dataViewListener._dvWeak.IsAlive)
					{
						listeners.RemoveAt(num);
						dataViewListener.CleanUp(false);
					}
					num--;
				}
				listeners.Add(this);
			}
		}

		private readonly WeakReference _dvWeak;

		private DataTable _table;

		private Index _index;

		internal readonly int _objectID;
	}
}
