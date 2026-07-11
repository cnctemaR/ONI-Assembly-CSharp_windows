using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace System.Data
{
	internal sealed class Index
	{
		public Index(DataTable table, IndexField[] indexFields, DataViewRowState recordStates, IFilter rowFilter)
			: this(table, indexFields, null, recordStates, rowFilter)
		{
		}

		public Index(DataTable table, Comparison<DataRow> comparison, DataViewRowState recordStates, IFilter rowFilter)
			: this(table, Index.GetAllFields(table.Columns), comparison, recordStates, rowFilter)
		{
		}

		private static IndexField[] GetAllFields(DataColumnCollection columns)
		{
			IndexField[] array = new IndexField[columns.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new IndexField(columns[i], false);
			}
			return array;
		}

		private Index(DataTable table, IndexField[] indexFields, Comparison<DataRow> comparison, DataViewRowState recordStates, IFilter rowFilter)
		{
			DataCommonEventSource.Log.Trace<int, int, DataViewRowState>("<ds.Index.Index|API> {0}, table={1}, recordStates={2}", this.ObjectID, (table != null) ? table.ObjectID : 0, recordStates);
			if ((recordStates & ~(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.ModifiedOriginal)) != DataViewRowState.None)
			{
				throw ExceptionBuilder.RecordStateRange();
			}
			this._table = table;
			this._listeners = new Listeners<DataViewListener>(this.ObjectID, (DataViewListener listener) => listener != null);
			this._indexFields = indexFields;
			this._recordStates = recordStates;
			this._comparison = comparison;
			DataColumnCollection columns = table.Columns;
			this._isSharable = rowFilter == null && comparison == null;
			if (rowFilter != null)
			{
				this._rowFilter = new WeakReference(rowFilter);
				DataExpression dataExpression = rowFilter as DataExpression;
				if (dataExpression != null)
				{
					this._hasRemoteAggregate = dataExpression.HasRemoteAggregate();
				}
			}
			this.InitRecords(rowFilter);
		}

		public bool Equal(IndexField[] indexDesc, DataViewRowState recordStates, IFilter rowFilter)
		{
			if (!this._isSharable || this._indexFields.Length != indexDesc.Length || this._recordStates != recordStates || rowFilter != null)
			{
				return false;
			}
			for (int i = 0; i < this._indexFields.Length; i++)
			{
				if (this._indexFields[i].Column != indexDesc[i].Column || this._indexFields[i].IsDescending != indexDesc[i].IsDescending)
				{
					return false;
				}
			}
			return true;
		}

		internal bool HasRemoteAggregate
		{
			get
			{
				return this._hasRemoteAggregate;
			}
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		public DataViewRowState RecordStates
		{
			get
			{
				return this._recordStates;
			}
		}

		public IFilter RowFilter
		{
			get
			{
				return (IFilter)((this._rowFilter != null) ? this._rowFilter.Target : null);
			}
		}

		public int GetRecord(int recordIndex)
		{
			return this._records[recordIndex];
		}

		public bool HasDuplicates
		{
			get
			{
				return this._records.HasDuplicates;
			}
		}

		public int RecordCount
		{
			get
			{
				return this._recordCount;
			}
		}

		public bool IsSharable
		{
			get
			{
				return this._isSharable;
			}
		}

		private bool AcceptRecord(int record)
		{
			return this.AcceptRecord(record, this.RowFilter);
		}

		private bool AcceptRecord(int record, IFilter filter)
		{
			DataCommonEventSource.Log.Trace<int, int>("<ds.Index.AcceptRecord|API> {0}, record={1}", this.ObjectID, record);
			if (filter == null)
			{
				return true;
			}
			DataRow dataRow = this._table._recordManager[record];
			if (dataRow == null)
			{
				return true;
			}
			DataRowVersion dataRowVersion = DataRowVersion.Default;
			if (dataRow._oldRecord == record)
			{
				dataRowVersion = DataRowVersion.Original;
			}
			else if (dataRow._newRecord == record)
			{
				dataRowVersion = DataRowVersion.Current;
			}
			else if (dataRow._tempRecord == record)
			{
				dataRowVersion = DataRowVersion.Proposed;
			}
			return filter.Invoke(dataRow, dataRowVersion);
		}

		internal void ListChangedAdd(DataViewListener listener)
		{
			this._listeners.Add(listener);
		}

		internal void ListChangedRemove(DataViewListener listener)
		{
			this._listeners.Remove(listener);
		}

		public int RefCount
		{
			get
			{
				return this._refCount;
			}
		}

		public void AddRef()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.Index.AddRef|API> {0}", this.ObjectID);
			this._table._indexesLock.EnterWriteLock();
			try
			{
				if (this._refCount == 0)
				{
					this._table.ShadowIndexCopy();
					this._table._indexes.Add(this);
				}
				this._refCount++;
			}
			finally
			{
				this._table._indexesLock.ExitWriteLock();
			}
		}

		public int RemoveRef()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.Index.RemoveRef|API> {0}", this.ObjectID);
			this._table._indexesLock.EnterWriteLock();
			int num2;
			try
			{
				int num = this._refCount - 1;
				this._refCount = num;
				num2 = num;
				if (this._refCount <= 0)
				{
					this._table.ShadowIndexCopy();
					this._table._indexes.Remove(this);
				}
			}
			finally
			{
				this._table._indexesLock.ExitWriteLock();
			}
			return num2;
		}

		private void ApplyChangeAction(int record, int action, int changeRecord)
		{
			if (action != 0)
			{
				if (action > 0)
				{
					if (this.AcceptRecord(record))
					{
						this.InsertRecord(record, true);
						return;
					}
				}
				else
				{
					if (this._comparison != null && -1 != record)
					{
						this.DeleteRecord(this.GetIndex(record, changeRecord));
						return;
					}
					this.DeleteRecord(this.GetIndex(record));
				}
			}
		}

		public bool CheckUnique()
		{
			return !this.HasDuplicates;
		}

		private int CompareRecords(int record1, int record2)
		{
			if (this._comparison != null)
			{
				return this.CompareDataRows(record1, record2);
			}
			if (this._indexFields.Length != 0)
			{
				int i = 0;
				while (i < this._indexFields.Length)
				{
					int num = this._indexFields[i].Column.Compare(record1, record2);
					if (num != 0)
					{
						if (!this._indexFields[i].IsDescending)
						{
							return num;
						}
						return -num;
					}
					else
					{
						i++;
					}
				}
				return 0;
			}
			return this._table.Rows.IndexOf(this._table._recordManager[record1]).CompareTo(this._table.Rows.IndexOf(this._table._recordManager[record2]));
		}

		private int CompareDataRows(int record1, int record2)
		{
			return this._comparison(this._table._recordManager[record1], this._table._recordManager[record2]);
		}

		private int CompareDuplicateRecords(int record1, int record2)
		{
			if (this._table._recordManager[record1] == null)
			{
				if (this._table._recordManager[record2] != null)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (this._table._recordManager[record2] == null)
				{
					return 1;
				}
				int num = this._table._recordManager[record1].rowID.CompareTo(this._table._recordManager[record2].rowID);
				if (num == 0 && record1 != record2)
				{
					num = ((int)this._table._recordManager[record1].GetRecordState(record1)).CompareTo((int)this._table._recordManager[record2].GetRecordState(record2));
				}
				return num;
			}
		}

		private int CompareRecordToKey(int record1, object[] vals)
		{
			int i = 0;
			while (i < this._indexFields.Length)
			{
				int num = this._indexFields[i].Column.CompareValueTo(record1, vals[i]);
				if (num != 0)
				{
					if (!this._indexFields[i].IsDescending)
					{
						return num;
					}
					return -num;
				}
				else
				{
					i++;
				}
			}
			return 0;
		}

		public void DeleteRecordFromIndex(int recordIndex)
		{
			this.DeleteRecord(recordIndex, false);
		}

		private void DeleteRecord(int recordIndex)
		{
			this.DeleteRecord(recordIndex, true);
		}

		private void DeleteRecord(int recordIndex, bool fireEvent)
		{
			DataCommonEventSource.Log.Trace<int, int, bool>("<ds.Index.DeleteRecord|INFO> {0}, recordIndex={1}, fireEvent={2}", this.ObjectID, recordIndex, fireEvent);
			if (recordIndex >= 0)
			{
				this._recordCount--;
				int num = this._records.DeleteByIndex(recordIndex);
				this.MaintainDataView(ListChangedType.ItemDeleted, num, !fireEvent);
				if (fireEvent)
				{
					this.OnListChanged(ListChangedType.ItemDeleted, recordIndex);
				}
			}
		}

		public RBTree<int>.RBTreeEnumerator GetEnumerator(int startIndex)
		{
			return new RBTree<int>.RBTreeEnumerator(this._records, startIndex);
		}

		public int GetIndex(int record)
		{
			return this._records.GetIndexByKey(record);
		}

		private int GetIndex(int record, int changeRecord)
		{
			DataRow dataRow = this._table._recordManager[record];
			int newRecord = dataRow._newRecord;
			int oldRecord = dataRow._oldRecord;
			int indexByKey;
			try
			{
				if (changeRecord != 1)
				{
					if (changeRecord == 2)
					{
						dataRow._oldRecord = record;
					}
				}
				else
				{
					dataRow._newRecord = record;
				}
				indexByKey = this._records.GetIndexByKey(record);
			}
			finally
			{
				if (changeRecord != 1)
				{
					if (changeRecord == 2)
					{
						dataRow._oldRecord = oldRecord;
					}
				}
				else
				{
					dataRow._newRecord = newRecord;
				}
			}
			return indexByKey;
		}

		public object[] GetUniqueKeyValues()
		{
			if (this._indexFields == null || this._indexFields.Length == 0)
			{
				return Array.Empty<object>();
			}
			List<object[]> list = new List<object[]>();
			this.GetUniqueKeyValues(list, this._records.root);
			return list.ToArray();
		}

		public int FindRecord(int record)
		{
			int num = this._records.Search(record);
			if (num != 0)
			{
				return this._records.GetIndexByNode(num);
			}
			return -1;
		}

		public int FindRecordByKey(object key)
		{
			int num = this.FindNodeByKey(key);
			if (num != 0)
			{
				return this._records.GetIndexByNode(num);
			}
			return -1;
		}

		public int FindRecordByKey(object[] key)
		{
			int num = this.FindNodeByKeys(key);
			if (num != 0)
			{
				return this._records.GetIndexByNode(num);
			}
			return -1;
		}

		private int FindNodeByKey(object originalKey)
		{
			if (this._indexFields.Length != 1)
			{
				throw ExceptionBuilder.IndexKeyLength(this._indexFields.Length, 1);
			}
			int num = this._records.root;
			if (num != 0)
			{
				DataColumn column = this._indexFields[0].Column;
				object obj = column.ConvertValue(originalKey);
				num = this._records.root;
				if (this._indexFields[0].IsDescending)
				{
					while (num != 0)
					{
						int num2 = column.CompareValueTo(this._records.Key(num), obj);
						if (num2 == 0)
						{
							break;
						}
						if (num2 < 0)
						{
							num = this._records.Left(num);
						}
						else
						{
							num = this._records.Right(num);
						}
					}
				}
				else
				{
					while (num != 0)
					{
						int num2 = column.CompareValueTo(this._records.Key(num), obj);
						if (num2 == 0)
						{
							break;
						}
						if (num2 > 0)
						{
							num = this._records.Left(num);
						}
						else
						{
							num = this._records.Right(num);
						}
					}
				}
			}
			return num;
		}

		private int FindNodeByKeys(object[] originalKey)
		{
			int num = ((originalKey != null) ? originalKey.Length : 0);
			if (num == 0 || this._indexFields.Length != num)
			{
				throw ExceptionBuilder.IndexKeyLength(this._indexFields.Length, num);
			}
			int num2 = this._records.root;
			if (num2 != 0)
			{
				object[] array = new object[originalKey.Length];
				for (int i = 0; i < originalKey.Length; i++)
				{
					array[i] = this._indexFields[i].Column.ConvertValue(originalKey[i]);
				}
				num2 = this._records.root;
				while (num2 != 0)
				{
					num = this.CompareRecordToKey(this._records.Key(num2), array);
					if (num == 0)
					{
						break;
					}
					if (num > 0)
					{
						num2 = this._records.Left(num2);
					}
					else
					{
						num2 = this._records.Right(num2);
					}
				}
			}
			return num2;
		}

		private int FindNodeByKeyRecord(int record)
		{
			int num = this._records.root;
			if (num != 0)
			{
				num = this._records.root;
				while (num != 0)
				{
					int num2 = this.CompareRecords(this._records.Key(num), record);
					if (num2 == 0)
					{
						break;
					}
					if (num2 > 0)
					{
						num = this._records.Left(num);
					}
					else
					{
						num = this._records.Right(num);
					}
				}
			}
			return num;
		}

		private Range GetRangeFromNode(int nodeId)
		{
			if (nodeId == 0)
			{
				return default(Range);
			}
			int indexByNode = this._records.GetIndexByNode(nodeId);
			if (this._records.Next(nodeId) == 0)
			{
				return new Range(indexByNode, indexByNode);
			}
			int num = this._records.SubTreeSize(this._records.Next(nodeId));
			return new Range(indexByNode, indexByNode + num - 1);
		}

		public Range FindRecords(object key)
		{
			int num = this.FindNodeByKey(key);
			return this.GetRangeFromNode(num);
		}

		public Range FindRecords(object[] key)
		{
			int num = this.FindNodeByKeys(key);
			return this.GetRangeFromNode(num);
		}

		internal void FireResetEvent()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.Index.FireResetEvent|API> {0}", this.ObjectID);
			if (this.DoListChanged)
			{
				this.OnListChanged(DataView.s_resetEventArgs);
			}
		}

		private int GetChangeAction(DataViewRowState oldState, DataViewRowState newState)
		{
			int num = (((this._recordStates & oldState) == DataViewRowState.None) ? 0 : 1);
			return (((this._recordStates & newState) == DataViewRowState.None) ? 0 : 1) - num;
		}

		private static int GetReplaceAction(DataViewRowState oldState)
		{
			if ((DataViewRowState.CurrentRows & oldState) != DataViewRowState.None)
			{
				return 1;
			}
			if ((DataViewRowState.OriginalRows & oldState) == DataViewRowState.None)
			{
				return 0;
			}
			return 2;
		}

		public DataRow GetRow(int i)
		{
			return this._table._recordManager[this.GetRecord(i)];
		}

		public DataRow[] GetRows(object[] values)
		{
			return this.GetRows(this.FindRecords(values));
		}

		public DataRow[] GetRows(Range range)
		{
			DataRow[] array = this._table.NewRowArray(range.Count);
			if (array.Length != 0)
			{
				RBTree<int>.RBTreeEnumerator enumerator = this.GetEnumerator(range.Min);
				int num = 0;
				while (num < array.Length && enumerator.MoveNext())
				{
					array[num] = this._table._recordManager[enumerator.Current];
					num++;
				}
			}
			return array;
		}

		private void InitRecords(IFilter filter)
		{
			DataViewRowState recordStates = this._recordStates;
			bool flag = this._indexFields.Length == 0;
			this._records = new Index.IndexTree(this);
			this._recordCount = 0;
			foreach (object obj in this._table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				int num = -1;
				if (dataRow._oldRecord == dataRow._newRecord)
				{
					if ((recordStates & DataViewRowState.Unchanged) != DataViewRowState.None)
					{
						num = dataRow._oldRecord;
					}
				}
				else if (dataRow._oldRecord == -1)
				{
					if ((recordStates & DataViewRowState.Added) != DataViewRowState.None)
					{
						num = dataRow._newRecord;
					}
				}
				else if (dataRow._newRecord == -1)
				{
					if ((recordStates & DataViewRowState.Deleted) != DataViewRowState.None)
					{
						num = dataRow._oldRecord;
					}
				}
				else if ((recordStates & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
				{
					num = dataRow._newRecord;
				}
				else if ((recordStates & DataViewRowState.ModifiedOriginal) != DataViewRowState.None)
				{
					num = dataRow._oldRecord;
				}
				if (num != -1 && this.AcceptRecord(num, filter))
				{
					this._records.InsertAt(-1, num, flag);
					this._recordCount++;
				}
			}
		}

		public int InsertRecordToIndex(int record)
		{
			int num = -1;
			if (this.AcceptRecord(record))
			{
				num = this.InsertRecord(record, false);
			}
			return num;
		}

		private int InsertRecord(int record, bool fireEvent)
		{
			DataCommonEventSource.Log.Trace<int, int, bool>("<ds.Index.InsertRecord|INFO> {0}, record={1}, fireEvent={2}", this.ObjectID, record, fireEvent);
			bool flag = false;
			if (this._indexFields.Length == 0 && this._table != null)
			{
				DataRow dataRow = this._table._recordManager[record];
				flag = this._table.Rows.IndexOf(dataRow) + 1 == this._table.Rows.Count;
			}
			int num = this._records.InsertAt(-1, record, flag);
			this._recordCount++;
			this.MaintainDataView(ListChangedType.ItemAdded, record, !fireEvent);
			if (fireEvent)
			{
				if (this.DoListChanged)
				{
					this.OnListChanged(ListChangedType.ItemAdded, this._records.GetIndexByNode(num));
				}
				return 0;
			}
			return this._records.GetIndexByNode(num);
		}

		public bool IsKeyInIndex(object key)
		{
			int num = this.FindNodeByKey(key);
			return num != 0;
		}

		public bool IsKeyInIndex(object[] key)
		{
			int num = this.FindNodeByKeys(key);
			return num != 0;
		}

		public bool IsKeyRecordInIndex(int record)
		{
			int num = this.FindNodeByKeyRecord(record);
			return num != 0;
		}

		private bool DoListChanged
		{
			get
			{
				return !this._suspendEvents && this._listeners.HasListeners && !this._table.AreIndexEventsSuspended;
			}
		}

		private void OnListChanged(ListChangedType changedType, int newIndex, int oldIndex)
		{
			if (this.DoListChanged)
			{
				this.OnListChanged(new ListChangedEventArgs(changedType, newIndex, oldIndex));
			}
		}

		private void OnListChanged(ListChangedType changedType, int index)
		{
			if (this.DoListChanged)
			{
				this.OnListChanged(new ListChangedEventArgs(changedType, index));
			}
		}

		private void OnListChanged(ListChangedEventArgs e)
		{
			DataCommonEventSource.Log.Trace<int>("<ds.Index.OnListChanged|INFO> {0}", this.ObjectID);
			this._listeners.Notify<ListChangedEventArgs, bool, bool>(e, false, false, delegate(DataViewListener listener, ListChangedEventArgs args, bool arg2, bool arg3)
			{
				listener.IndexListChanged(args);
			});
		}

		private void MaintainDataView(ListChangedType changedType, int record, bool trackAddRemove)
		{
			this._listeners.Notify<ListChangedType, DataRow, bool>(changedType, (0 <= record) ? this._table._recordManager[record] : null, trackAddRemove, delegate(DataViewListener listener, ListChangedType type, DataRow row, bool track)
			{
				listener.MaintainDataView(changedType, row, track);
			});
		}

		public void Reset()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.Index.Reset|API> {0}", this.ObjectID);
			this.InitRecords(this.RowFilter);
			this.MaintainDataView(ListChangedType.Reset, -1, false);
			this.FireResetEvent();
		}

		public void RecordChanged(int record)
		{
			DataCommonEventSource.Log.Trace<int, int>("<ds.Index.RecordChanged|API> {0}, record={1}", this.ObjectID, record);
			if (this.DoListChanged)
			{
				int index = this.GetIndex(record);
				if (index >= 0)
				{
					this.OnListChanged(ListChangedType.ItemChanged, index);
				}
			}
		}

		public void RecordChanged(int oldIndex, int newIndex)
		{
			DataCommonEventSource.Log.Trace<int, int, int>("<ds.Index.RecordChanged|API> {0}, oldIndex={1}, newIndex={2}", this.ObjectID, oldIndex, newIndex);
			if (oldIndex > -1 || newIndex > -1)
			{
				if (oldIndex == newIndex)
				{
					this.OnListChanged(ListChangedType.ItemChanged, newIndex, oldIndex);
					return;
				}
				if (oldIndex == -1)
				{
					this.OnListChanged(ListChangedType.ItemAdded, newIndex, oldIndex);
					return;
				}
				if (newIndex == -1)
				{
					this.OnListChanged(ListChangedType.ItemDeleted, oldIndex);
					return;
				}
				this.OnListChanged(ListChangedType.ItemMoved, newIndex, oldIndex);
			}
		}

		public void RecordStateChanged(int record, DataViewRowState oldState, DataViewRowState newState)
		{
			DataCommonEventSource.Log.Trace<int, int, DataViewRowState, DataViewRowState>("<ds.Index.RecordStateChanged|API> {0}, record={1}, oldState={2}, newState={3}", this.ObjectID, record, oldState, newState);
			int changeAction = this.GetChangeAction(oldState, newState);
			this.ApplyChangeAction(record, changeAction, Index.GetReplaceAction(oldState));
		}

		public void RecordStateChanged(int oldRecord, DataViewRowState oldOldState, DataViewRowState oldNewState, int newRecord, DataViewRowState newOldState, DataViewRowState newNewState)
		{
			DataCommonEventSource.Log.Trace<int, int, DataViewRowState, DataViewRowState, int, DataViewRowState, DataViewRowState>("<ds.Index.RecordStateChanged|API> {0}, oldRecord={1}, oldOldState={2}, oldNewState={3}, newRecord={4}, newOldState={5}, newNewState={6}", this.ObjectID, oldRecord, oldOldState, oldNewState, newRecord, newOldState, newNewState);
			int changeAction = this.GetChangeAction(oldOldState, oldNewState);
			int changeAction2 = this.GetChangeAction(newOldState, newNewState);
			if (changeAction != -1 || changeAction2 != 1 || !this.AcceptRecord(newRecord))
			{
				this.ApplyChangeAction(oldRecord, changeAction, Index.GetReplaceAction(oldOldState));
				this.ApplyChangeAction(newRecord, changeAction2, Index.GetReplaceAction(newOldState));
				return;
			}
			int num;
			if (this._comparison != null && changeAction < 0)
			{
				num = this.GetIndex(oldRecord, Index.GetReplaceAction(oldOldState));
			}
			else
			{
				num = this.GetIndex(oldRecord);
			}
			if (this._comparison == null && num != -1 && this.CompareRecords(oldRecord, newRecord) == 0)
			{
				this._records.UpdateNodeKey(oldRecord, newRecord);
				int index = this.GetIndex(newRecord);
				this.OnListChanged(ListChangedType.ItemChanged, index, index);
				return;
			}
			this._suspendEvents = true;
			if (num != -1)
			{
				this._records.DeleteByIndex(num);
				this._recordCount--;
			}
			this._records.Insert(newRecord);
			this._recordCount++;
			this._suspendEvents = false;
			int index2 = this.GetIndex(newRecord);
			if (num == index2)
			{
				this.OnListChanged(ListChangedType.ItemChanged, index2, num);
				return;
			}
			if (num == -1)
			{
				this.MaintainDataView(ListChangedType.ItemAdded, newRecord, false);
				this.OnListChanged(ListChangedType.ItemAdded, this.GetIndex(newRecord));
				return;
			}
			this.OnListChanged(ListChangedType.ItemMoved, index2, num);
		}

		internal DataTable Table
		{
			get
			{
				return this._table;
			}
		}

		private void GetUniqueKeyValues(List<object[]> list, int curNodeId)
		{
			if (curNodeId != 0)
			{
				this.GetUniqueKeyValues(list, this._records.Left(curNodeId));
				int num = this._records.Key(curNodeId);
				object[] array = new object[this._indexFields.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._indexFields[i].Column[num];
				}
				list.Add(array);
				this.GetUniqueKeyValues(list, this._records.Right(curNodeId));
			}
		}

		internal static int IndexOfReference<T>(List<T> list, T item) where T : class
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == item)
					{
						return i;
					}
				}
			}
			return -1;
		}

		internal static bool ContainsReference<T>(List<T> list, T item) where T : class
		{
			return 0 <= Index.IndexOfReference<T>(list, item);
		}

		private const int DoNotReplaceCompareRecord = 0;

		private const int ReplaceNewRecordForCompare = 1;

		private const int ReplaceOldRecordForCompare = 2;

		private readonly DataTable _table;

		internal readonly IndexField[] _indexFields;

		private readonly Comparison<DataRow> _comparison;

		private readonly DataViewRowState _recordStates;

		private WeakReference _rowFilter;

		private Index.IndexTree _records;

		private int _recordCount;

		private int _refCount;

		private Listeners<DataViewListener> _listeners;

		private bool _suspendEvents;

		private readonly bool _isSharable;

		private readonly bool _hasRemoteAggregate;

		internal const int MaskBits = 2147483647;

		private static int s_objectTypeCount;

		private readonly int _objectID = Interlocked.Increment(ref Index.s_objectTypeCount);

		private sealed class IndexTree : RBTree<int>
		{
			internal IndexTree(Index index)
				: base(TreeAccessMethod.KEY_SEARCH_AND_INDEX)
			{
				this._index = index;
			}

			protected override int CompareNode(int record1, int record2)
			{
				return this._index.CompareRecords(record1, record2);
			}

			protected override int CompareSateliteTreeNode(int record1, int record2)
			{
				return this._index.CompareDuplicateRecords(record1, record2);
			}

			private readonly Index _index;
		}
	}
}
