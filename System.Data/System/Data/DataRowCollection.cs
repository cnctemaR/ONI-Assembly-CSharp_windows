using System;
using System.Collections;
using Unity;

namespace System.Data
{
	public sealed class DataRowCollection : InternalDataCollectionBase
	{
		internal DataRowCollection(DataTable table)
		{
			this._list = new DataRowCollection.DataRowTree();
			base..ctor();
			this._table = table;
		}

		public override int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public DataRow this[int index]
		{
			get
			{
				return this._list[index];
			}
		}

		public void Add(DataRow row)
		{
			this._table.AddRow(row, -1);
		}

		public void InsertAt(DataRow row, int pos)
		{
			if (pos < 0)
			{
				throw ExceptionBuilder.RowInsertOutOfRange(pos);
			}
			if (pos >= this._list.Count)
			{
				this._table.AddRow(row, -1);
				return;
			}
			this._table.InsertRow(row, -1, pos);
		}

		internal void DiffInsertAt(DataRow row, int pos)
		{
			if (pos < 0 || pos == this._list.Count)
			{
				this._table.AddRow(row, (pos > -1) ? (pos + 1) : (-1));
				return;
			}
			if (this._table.NestedParentRelations.Length == 0)
			{
				this._table.InsertRow(row, pos + 1, (pos > this._list.Count) ? (-1) : pos);
				return;
			}
			if (pos >= this._list.Count)
			{
				while (pos > this._list.Count)
				{
					this._list.Add(null);
					this._nullInList++;
				}
				this._table.AddRow(row, pos + 1);
				return;
			}
			if (this._list[pos] != null)
			{
				throw ExceptionBuilder.RowInsertTwice(pos, this._table.TableName);
			}
			this._list.RemoveAt(pos);
			this._nullInList--;
			this._table.InsertRow(row, pos + 1, pos);
		}

		public int IndexOf(DataRow row)
		{
			if (row != null && row.Table == this._table && (row.RBTreeNodeId != 0 || row.RowState != DataRowState.Detached))
			{
				return this._list.IndexOf(row.RBTreeNodeId, row);
			}
			return -1;
		}

		internal DataRow AddWithColumnEvents(params object[] values)
		{
			DataRow dataRow = this._table.NewRow(-1);
			dataRow.ItemArray = values;
			this._table.AddRow(dataRow, -1);
			return dataRow;
		}

		public DataRow Add(params object[] values)
		{
			int num = this._table.NewRecordFromArray(values);
			DataRow dataRow = this._table.NewRow(num);
			this._table.AddRow(dataRow, -1);
			return dataRow;
		}

		internal void ArrayAdd(DataRow row)
		{
			row.RBTreeNodeId = this._list.Add(row);
		}

		internal void ArrayInsert(DataRow row, int pos)
		{
			row.RBTreeNodeId = this._list.Insert(pos, row);
		}

		internal void ArrayClear()
		{
			this._list.Clear();
		}

		internal void ArrayRemove(DataRow row)
		{
			if (row.RBTreeNodeId == 0)
			{
				throw ExceptionBuilder.InternalRBTreeError(RBTreeError.AttachedNodeWithZerorbTreeNodeId);
			}
			this._list.RBDelete(row.RBTreeNodeId);
			row.RBTreeNodeId = 0;
		}

		public DataRow Find(object key)
		{
			return this._table.FindByPrimaryKey(key);
		}

		public DataRow Find(object[] keys)
		{
			return this._table.FindByPrimaryKey(keys);
		}

		public void Clear()
		{
			this._table.Clear(false);
		}

		public bool Contains(object key)
		{
			return this._table.FindByPrimaryKey(key) != null;
		}

		public bool Contains(object[] keys)
		{
			return this._table.FindByPrimaryKey(keys) != null;
		}

		public override void CopyTo(Array ar, int index)
		{
			this._list.CopyTo(ar, index);
		}

		public void CopyTo(DataRow[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		public void Remove(DataRow row)
		{
			if (row == null || row.Table != this._table || -1L == row.rowID)
			{
				throw ExceptionBuilder.RowOutOfRange();
			}
			if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
			{
				row.Delete();
			}
			if (row.RowState != DataRowState.Detached)
			{
				row.AcceptChanges();
			}
		}

		public void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		internal DataRowCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly DataTable _table;

		private readonly DataRowCollection.DataRowTree _list;

		internal int _nullInList;

		private sealed class DataRowTree : RBTree<DataRow>
		{
			internal DataRowTree()
				: base(TreeAccessMethod.INDEX_ONLY)
			{
			}

			protected override int CompareNode(DataRow record1, DataRow record2)
			{
				throw ExceptionBuilder.InternalRBTreeError(RBTreeError.CompareNodeInDataRowTree);
			}

			protected override int CompareSateliteTreeNode(DataRow record1, DataRow record2)
			{
				throw ExceptionBuilder.InternalRBTreeError(RBTreeError.CompareSateliteTreeNodeInDataRowTree);
			}
		}
	}
}
