using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	public sealed class DataRowCollection : InternalDataCollectionBase
	{
		internal DataRowCollection(DataTable table)
		{
			this.table = table;
		}

		internal event ListChangedEventHandler ListChanged;

		public DataRow this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException("There is no row at position " + index + ".");
				}
				return (DataRow)this.List[index];
			}
		}

		public void Add(DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row", "'row' argument cannot be null.");
			}
			if (row.Table != this.table)
			{
				throw new ArgumentException("This row already belongs to another table.");
			}
			if (row.RowID != -1)
			{
				throw new ArgumentException("This row already belongs to this table.");
			}
			row.BeginEdit();
			row.Validate();
			this.AddInternal(row);
		}

		public int IndexOf(DataRow row)
		{
			if (row == null || row.Table != this.table)
			{
				return -1;
			}
			int rowID = row.RowID;
			return (rowID < 0 || rowID >= this.List.Count || row != this.List[rowID]) ? (-1) : rowID;
		}

		internal void AddInternal(DataRow row)
		{
			this.AddInternal(row, DataRowAction.Add);
		}

		internal void AddInternal(DataRow row, DataRowAction action)
		{
			row.Table.ChangingDataRow(row, action);
			this.List.Add(row);
			row.AttachAt(this.List.Count - 1, action);
			row.Table.ChangedDataRow(row, action);
			if (row._rowChanged)
			{
				row._rowChanged = false;
			}
		}

		public DataRow Add(params object[] values)
		{
			if (values == null)
			{
				throw new NullReferenceException();
			}
			DataRow dataRow = this.table.NewNotInitializedRow();
			int num = this.table.CreateRecord(values);
			dataRow.ImportRecord(num);
			dataRow.Validate();
			this.AddInternal(dataRow);
			return dataRow;
		}

		public void Clear()
		{
			if (this.table.DataSet != null && this.table.DataSet.EnforceConstraints)
			{
				foreach (object obj in this.table.Constraints)
				{
					Constraint constraint = (Constraint)obj;
					UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
					if (uniqueConstraint != null)
					{
						if (uniqueConstraint.ChildConstraint != null && uniqueConstraint.ChildConstraint.Table.Rows.Count != 0)
						{
							string text = string.Format("Cannot clear table Parent because ForeignKeyConstraint {0} enforces Child.", uniqueConstraint.ConstraintName);
							throw new InvalidConstraintException(text);
						}
					}
				}
			}
			this.table.DataTableClearing();
			this.List.Clear();
			this.table.ResetIndexes();
			this.table.DataTableCleared();
			this.OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}

		public bool Contains(object key)
		{
			return this.Find(key) != null;
		}

		public bool Contains(object[] keys)
		{
			return this.Find(keys) != null;
		}

		public DataRow Find(object key)
		{
			return this.Find(new object[] { key }, DataViewRowState.CurrentRows);
		}

		public DataRow Find(object[] keys)
		{
			return this.Find(keys, DataViewRowState.CurrentRows);
		}

		internal DataRow Find(object[] keys, DataViewRowState rowStateFilter)
		{
			if (this.table.PrimaryKey.Length == 0)
			{
				throw new MissingPrimaryKeyException("Table doesn't have a primary key.");
			}
			if (keys == null)
			{
				throw new ArgumentException("Expecting " + this.table.PrimaryKey.Length + " value(s) for the key being indexed, but received 0 value(s).");
			}
			Index index = this.table.GetIndex(this.table.PrimaryKey, null, rowStateFilter, null, false);
			int num = index.Find(keys);
			if (num != -1 || !this.table._duringDataLoad)
			{
				return (num == -1) ? null : this.table.RecordCache[num];
			}
			num = this.table.RecordCache.NewRecord();
			DataRow dataRow2;
			try
			{
				for (int i = 0; i < this.table.PrimaryKey.Length; i++)
				{
					this.table.PrimaryKey[i].DataContainer[num] = keys[i];
				}
				foreach (object obj in this)
				{
					DataRow dataRow = (DataRow)obj;
					int record = Key.GetRecord(dataRow, rowStateFilter);
					if (record != -1)
					{
						bool flag = true;
						for (int j = 0; j < this.table.PrimaryKey.Length; j++)
						{
							if (this.table.PrimaryKey[j].CompareValues(record, num) != 0)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							return dataRow;
						}
					}
				}
				dataRow2 = null;
			}
			finally
			{
				this.table.RecordCache.DisposeRecord(num);
			}
			return dataRow2;
		}

		public void InsertAt(DataRow row, int pos)
		{
			if (pos < 0)
			{
				throw new IndexOutOfRangeException("The row insert position " + pos + " is invalid.");
			}
			if (row == null)
			{
				throw new ArgumentNullException("row", "'row' argument cannot be null.");
			}
			if (row.Table != this.table)
			{
				throw new ArgumentException("This row already belongs to another table.");
			}
			if (row.RowID != -1)
			{
				throw new ArgumentException("This row already belongs to this table.");
			}
			row.Validate();
			row.Table.ChangingDataRow(row, DataRowAction.Add);
			if (pos >= this.List.Count)
			{
				pos = this.List.Count;
				this.List.Add(row);
			}
			else
			{
				this.List.Insert(pos, row);
				for (int i = pos + 1; i < this.List.Count; i++)
				{
					((DataRow)this.List[i]).RowID = i;
				}
			}
			row.AttachAt(pos, DataRowAction.Add);
			row.Table.ChangedDataRow(row, DataRowAction.Add);
		}

		internal void RemoveInternal(DataRow row)
		{
			if (row == null)
			{
				throw new IndexOutOfRangeException("The given datarow is not in the current DataRowCollection.");
			}
			int i = this.List.IndexOf(row);
			if (i < 0)
			{
				throw new IndexOutOfRangeException("The given datarow is not in the current DataRowCollection.");
			}
			this.List.RemoveAt(i);
			while (i < this.List.Count)
			{
				((DataRow)this.List[i]).RowID = i;
				i++;
			}
		}

		public void Remove(DataRow row)
		{
			if (this.IndexOf(row) < 0)
			{
				throw new IndexOutOfRangeException("The given datarow is not in the current DataRowCollection.");
			}
			DataRowState rowState = row.RowState;
			if (rowState != DataRowState.Deleted && rowState != DataRowState.Detached)
			{
				row.Delete();
				if (row.RowState != DataRowState.Detached)
				{
					row.AcceptChanges();
				}
			}
		}

		public void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		internal void OnListChanged(object sender, ListChangedEventArgs args)
		{
			if (this.ListChanged != null)
			{
				this.ListChanged(sender, args);
			}
		}

		public override int Count
		{
			get
			{
				return this.List.Count;
			}
		}

		public void CopyTo(DataRow[] array, int index)
		{
			this.CopyTo(array, index);
		}

		public override void CopyTo(Array array, int index)
		{
			base.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return base.GetEnumerator();
		}

		private DataTable table;
	}
}
