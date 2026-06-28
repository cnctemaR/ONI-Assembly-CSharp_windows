using System;
using System.ComponentModel;
using Mono.Data.SqlExpressions;

namespace System.Data.Common
{
	internal class Key
	{
		internal Key(DataTable table, DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter)
		{
			this._table = table;
			this._filter = filter;
			if (this._filter != null)
			{
				this._tmpRow = this._table.NewNotInitializedRow();
			}
			this._columns = columns;
			if (sort != null && sort.Length == columns.Length)
			{
				this._sortDirection = sort;
			}
			else
			{
				this._sortDirection = new ListSortDirection[columns.Length];
				for (int i = 0; i < this._sortDirection.Length; i++)
				{
					this._sortDirection[i] = ListSortDirection.Ascending;
				}
			}
			if (rowState != DataViewRowState.None)
			{
				this._rowStateFilter = rowState;
			}
			else
			{
				this._rowStateFilter = DataViewRowState.CurrentRows;
			}
		}

		internal DataColumn[] Columns
		{
			get
			{
				return this._columns;
			}
		}

		internal DataTable Table
		{
			get
			{
				return this._table;
			}
		}

		private ListSortDirection[] Sort
		{
			get
			{
				return this._sortDirection;
			}
		}

		internal DataViewRowState RowStateFilter
		{
			get
			{
				return this._rowStateFilter;
			}
			set
			{
				this._rowStateFilter = value;
			}
		}

		internal bool HasFilter
		{
			get
			{
				return this._filter != null;
			}
		}

		internal int CompareRecords(int first, int second)
		{
			if (first == second)
			{
				return 0;
			}
			for (int i = 0; i < this.Columns.Length; i++)
			{
				int num = this.Columns[i].CompareValues(first, second);
				if (num != 0)
				{
					return (this.Sort[i] != ListSortDirection.Ascending) ? (-num) : num;
				}
			}
			return 0;
		}

		internal int GetRecord(DataRow row)
		{
			int record = Key.GetRecord(row, this._rowStateFilter);
			if (this._filter == null)
			{
				return record;
			}
			if (record < 0)
			{
				return record;
			}
			return (!this.CanContain(record)) ? (-1) : record;
		}

		internal bool CanContain(int index)
		{
			if (this._filter == null)
			{
				return true;
			}
			this._tmpRow._current = index;
			return this._filter.EvalBoolean(this._tmpRow);
		}

		internal bool ContainsVersion(DataRowState state, DataRowVersion version)
		{
			switch (state)
			{
			case DataRowState.Unchanged:
				if ((this._rowStateFilter & DataViewRowState.Unchanged) != DataViewRowState.None)
				{
					return (version & DataRowVersion.Default) != (DataRowVersion)0;
				}
				break;
			default:
				if (state != DataRowState.Deleted)
				{
					if ((this._rowStateFilter & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
					{
						return (version & DataRowVersion.Default) != (DataRowVersion)0;
					}
					if ((this._rowStateFilter & DataViewRowState.ModifiedOriginal) != DataViewRowState.None)
					{
						return version == DataRowVersion.Original;
					}
				}
				else if ((this._rowStateFilter & DataViewRowState.Deleted) != DataViewRowState.None)
				{
					return version == DataRowVersion.Original;
				}
				break;
			case DataRowState.Added:
				if ((this._rowStateFilter & DataViewRowState.Added) != DataViewRowState.None)
				{
					return (version & DataRowVersion.Default) != (DataRowVersion)0;
				}
				break;
			}
			return false;
		}

		internal static int GetRecord(DataRow row, DataViewRowState rowStateFilter)
		{
			DataRowState rowState = row.RowState;
			switch (rowState)
			{
			case DataRowState.Unchanged:
				if ((rowStateFilter & DataViewRowState.Unchanged) != DataViewRowState.None)
				{
					return (row.Proposed < 0) ? row.Current : row.Proposed;
				}
				break;
			default:
				if (rowState != DataRowState.Deleted)
				{
					if ((rowStateFilter & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
					{
						return (row.Proposed < 0) ? row.Current : row.Proposed;
					}
					if ((rowStateFilter & DataViewRowState.ModifiedOriginal) != DataViewRowState.None)
					{
						return row.Original;
					}
				}
				else if ((rowStateFilter & DataViewRowState.Deleted) != DataViewRowState.None)
				{
					return row.Original;
				}
				break;
			case DataRowState.Added:
				if ((rowStateFilter & DataViewRowState.Added) != DataViewRowState.None)
				{
					return (row.Proposed < 0) ? row.Current : row.Proposed;
				}
				break;
			}
			return -1;
		}

		internal bool Equals(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter)
		{
			if (rowState != DataViewRowState.None && this.RowStateFilter != rowState)
			{
				return false;
			}
			if (this._filter != null)
			{
				if (!this._filter.Equals(filter))
				{
					return false;
				}
			}
			else if (filter != null)
			{
				return false;
			}
			if (this.Columns.Length != columns.Length)
			{
				return false;
			}
			if (sort != null && this.Sort.Length != sort.Length)
			{
				return false;
			}
			if (sort != null)
			{
				for (int i = 0; i < columns.Length; i++)
				{
					if (this.Sort[i] != sort[i] || this.Columns[i] != columns[i])
					{
						return false;
					}
				}
			}
			else
			{
				for (int j = 0; j < columns.Length; j++)
				{
					if (this.Sort[j] != ListSortDirection.Ascending || this.Columns[j] != columns[j])
					{
						return false;
					}
				}
			}
			return true;
		}

		internal bool DependsOn(DataColumn column)
		{
			return this._filter != null && this._filter.DependsOn(column);
		}

		private DataTable _table;

		private DataColumn[] _columns;

		private ListSortDirection[] _sortDirection;

		private DataViewRowState _rowStateFilter;

		private IExpression _filter;

		private DataRow _tmpRow;
	}
}
