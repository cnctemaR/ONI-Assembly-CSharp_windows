using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	public class DataRow
	{
		protected internal DataRow(DataRowBuilder builder)
		{
			this._table = builder.Table;
			this._rowId = builder._rowId;
			this.rowError = string.Empty;
		}

		internal DataRow(DataTable table, int rowId)
		{
			this._table = table;
			this._rowId = rowId;
		}

		private ArrayList ColumnErrors
		{
			get
			{
				if (this._columnErrors == null)
				{
					this._columnErrors = new ArrayList();
				}
				return this._columnErrors;
			}
			set
			{
				this._columnErrors = value;
			}
		}

		public bool HasErrors
		{
			get
			{
				if (this.RowError != string.Empty)
				{
					return true;
				}
				foreach (object obj in this.ColumnErrors)
				{
					string text = (string)obj;
					if (text != null && text != string.Empty)
					{
						return true;
					}
				}
				return false;
			}
		}

		public object this[string columnName]
		{
			get
			{
				return this[columnName, DataRowVersion.Default];
			}
			set
			{
				DataColumn dataColumn = this._table.Columns[columnName];
				if (dataColumn == null)
				{
					throw new ArgumentException("The column '" + columnName + "' does not belong to the table : " + this._table.TableName);
				}
				this[dataColumn.Ordinal] = value;
			}
		}

		public object this[DataColumn column]
		{
			get
			{
				return this[column, DataRowVersion.Default];
			}
			set
			{
				if (column == null)
				{
					throw new ArgumentNullException("column");
				}
				int num = this._table.Columns.IndexOf(column);
				if (num == -1)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The column '{0}' does not belong to the table : {1}.", new object[]
					{
						column.ColumnName,
						this._table.TableName
					}));
				}
				this[num] = value;
			}
		}

		public object this[int columnIndex]
		{
			get
			{
				return this[columnIndex, DataRowVersion.Default];
			}
			set
			{
				if (columnIndex < 0 || columnIndex > this._table.Columns.Count)
				{
					throw new IndexOutOfRangeException();
				}
				if (this.RowState == DataRowState.Deleted)
				{
					throw new DeletedRowInaccessibleException();
				}
				DataColumn dataColumn = this._table.Columns[columnIndex];
				this._table.ChangingDataColumn(this, dataColumn, value);
				if (value == null && dataColumn.DataType.IsValueType)
				{
					throw new ArgumentException("Canot set column '" + dataColumn.ColumnName + "' to be null. Please use DBNull instead.");
				}
				this._rowChanged = true;
				this.CheckValue(value, dataColumn);
				bool flag = this.Proposed >= 0;
				if (!flag)
				{
					this.BeginEdit();
				}
				dataColumn[this.Proposed] = value;
				this._table.ChangedDataColumn(this, dataColumn, value);
				if (!flag)
				{
					this.EndEdit();
				}
			}
		}

		public object this[string columnName, DataRowVersion version]
		{
			get
			{
				DataColumn dataColumn = this._table.Columns[columnName];
				if (dataColumn == null)
				{
					throw new ArgumentException("The column '" + columnName + "' does not belong to the table : " + this._table.TableName);
				}
				return this[dataColumn.Ordinal, version];
			}
		}

		public object this[DataColumn column, DataRowVersion version]
		{
			get
			{
				if (column == null)
				{
					throw new ArgumentNullException("column");
				}
				if (column.Table != this.Table)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The column '{0}' does not belong to the table : {1}.", new object[]
					{
						column.ColumnName,
						this._table.TableName
					}));
				}
				return this[column.Ordinal, version];
			}
		}

		internal void SetValue(int column, object value, int version)
		{
			DataColumn dataColumn = this.Table.Columns[column];
			if (value == null && !dataColumn.AutoIncrement)
			{
				value = dataColumn.DefaultValue;
			}
			this.Table.ChangingDataColumn(this, dataColumn, value);
			this.CheckValue(value, dataColumn);
			if (!dataColumn.AutoIncrement)
			{
				dataColumn[version] = value;
			}
			else if (this._proposed >= 0 && this._proposed != version)
			{
				dataColumn[version] = dataColumn[this._proposed];
			}
		}

		public object this[int columnIndex, DataRowVersion version]
		{
			get
			{
				if (columnIndex < 0 || columnIndex > this._table.Columns.Count)
				{
					throw new IndexOutOfRangeException();
				}
				DataColumn dataColumn = this._table.Columns[columnIndex];
				int num = this.IndexFromVersion(version);
				if (dataColumn.Expression != string.Empty && this._table.Rows.IndexOf(this) != -1)
				{
					object obj = dataColumn.CompiledExpression.Eval(this);
					if (obj != null && obj != DBNull.Value)
					{
						obj = Convert.ChangeType(obj, dataColumn.DataType);
					}
					dataColumn[num] = obj;
					return dataColumn[num];
				}
				return dataColumn[num];
			}
		}

		public object[] ItemArray
		{
			get
			{
				if (this.RowState == DataRowState.Deleted)
				{
					throw new DeletedRowInaccessibleException("Deleted row information cannot be accessed through the row.");
				}
				int num = this.Current;
				if (this.RowState == DataRowState.Detached)
				{
					if (this.Proposed < 0)
					{
						throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
					}
					num = this.Proposed;
				}
				object[] array = new object[this._table.Columns.Count];
				foreach (object obj in this._table.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					array[dataColumn.Ordinal] = dataColumn[num];
				}
				return array;
			}
			set
			{
				if (value.Length > this._table.Columns.Count)
				{
					throw new ArgumentException();
				}
				if (this.RowState == DataRowState.Deleted)
				{
					throw new DeletedRowInaccessibleException();
				}
				this.BeginEdit();
				DataColumnChangeEventArgs e = new DataColumnChangeEventArgs();
				foreach (object obj in this._table.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					int ordinal = dataColumn.Ordinal;
					object obj2 = ((ordinal >= value.Length) ? null : value[ordinal]);
					if (obj2 != null)
					{
						e.Initialize(this, dataColumn, obj2);
						this.CheckValue(e.ProposedValue, dataColumn);
						this._table.RaiseOnColumnChanging(e);
						dataColumn[this.Proposed] = e.ProposedValue;
						this._table.RaiseOnColumnChanged(e);
					}
				}
				this.EndEdit();
			}
		}

		public DataRowState RowState
		{
			get
			{
				if (this.Original == -1 && this.Current == -1)
				{
					return DataRowState.Detached;
				}
				if (this.Original == this.Current)
				{
					return DataRowState.Unchanged;
				}
				if (this.Original == -1)
				{
					return DataRowState.Added;
				}
				if (this.Current == -1)
				{
					return DataRowState.Deleted;
				}
				return DataRowState.Modified;
			}
			internal set
			{
				if (value == DataRowState.Detached)
				{
					this.Original = -1;
					this.Current = -1;
				}
				if (value == DataRowState.Unchanged)
				{
					this.Original = this.Current;
				}
				if (value == DataRowState.Added)
				{
					this.Original = -1;
				}
				if (value == DataRowState.Deleted)
				{
					this.Current = -1;
				}
			}
		}

		public void SetAdded()
		{
			if (this.RowState != DataRowState.Unchanged)
			{
				throw new InvalidOperationException("SetAdded and SetModified can only be called on DataRows with Unchanged DataRowState.");
			}
			this.Original = -1;
		}

		public void SetModified()
		{
			if (this.RowState != DataRowState.Unchanged)
			{
				throw new InvalidOperationException("SetAdded and SetModified can only be called on DataRows with Unchanged DataRowState.");
			}
			this.Current = this._table.RecordCache.NewRecord();
			this._table.RecordCache.CopyRecord(this._table, this.Original, this.Current);
		}

		public DataTable Table
		{
			get
			{
				return this._table;
			}
			internal set
			{
				this._table = value;
			}
		}

		internal int XmlRowID
		{
			get
			{
				return this.xmlRowID;
			}
			set
			{
				this.xmlRowID = value;
			}
		}

		internal int RowID
		{
			get
			{
				return this._rowId;
			}
			set
			{
				this._rowId = value;
			}
		}

		internal int Original
		{
			get
			{
				return this._original;
			}
			set
			{
				if (this.Table != null)
				{
					this.Table.RecordCache[value] = this;
				}
				this._original = value;
			}
		}

		internal int Current
		{
			get
			{
				return this._current;
			}
			set
			{
				if (this.Table != null)
				{
					this.Table.RecordCache[value] = this;
				}
				this._current = value;
			}
		}

		internal int Proposed
		{
			get
			{
				return this._proposed;
			}
			set
			{
				if (this.Table != null)
				{
					this.Table.RecordCache[value] = this;
				}
				this._proposed = value;
			}
		}

		internal void AttachAt(int row_id, DataRowAction action)
		{
			this._rowId = row_id;
			if (this.Proposed != -1)
			{
				if (this.Current >= 0)
				{
					this.Table.RecordCache.DisposeRecord(this.Current);
				}
				this.Current = this.Proposed;
				this.Proposed = -1;
			}
			if ((action & (DataRowAction.ChangeCurrentAndOriginal | DataRowAction.ChangeOriginal)) != DataRowAction.Nothing)
			{
				this.Original = this.Current;
			}
		}

		private void Detach()
		{
			this.Table.DeleteRowFromIndexes(this);
			this._table.Rows.RemoveInternal(this);
			if (this.Proposed >= 0 && this.Proposed != this.Current && this.Proposed != this.Original)
			{
				this._table.RecordCache.DisposeRecord(this.Proposed);
			}
			this.Proposed = -1;
			if (this.Current >= 0 && this.Current != this.Original)
			{
				this._table.RecordCache.DisposeRecord(this.Current);
			}
			this.Current = -1;
			if (this.Original >= 0)
			{
				this._table.RecordCache.DisposeRecord(this.Original);
			}
			this.Original = -1;
			this._rowId = -1;
		}

		internal void ImportRecord(int record)
		{
			if (this.HasVersion(DataRowVersion.Proposed))
			{
				this.Table.RecordCache.DisposeRecord(this.Proposed);
			}
			this.Proposed = record;
			foreach (object obj in this.Table.Columns.AutoIncrmentColumns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				dataColumn.UpdateAutoIncrementValue(dataColumn.DataContainer.GetInt64(this.Proposed));
			}
			foreach (object obj2 in this.Table.Columns)
			{
				DataColumn dataColumn2 = (DataColumn)obj2;
				this.CheckValue(this[dataColumn2], dataColumn2, false);
			}
		}

		private void CheckValue(object v, DataColumn col)
		{
			this.CheckValue(v, col, true);
		}

		private void CheckValue(object v, DataColumn col, bool doROCheck)
		{
			if (doROCheck && this._rowId != -1 && col.ReadOnly)
			{
				throw new ReadOnlyException();
			}
			if (v == null || v == DBNull.Value)
			{
				if (col.AllowDBNull || col.AutoIncrement || col.DefaultValue != DBNull.Value)
				{
					return;
				}
				this._nullConstraintViolation = true;
				if (this.Table._duringDataLoad || (this.Table.DataSet != null && !this.Table.DataSet.EnforceConstraints))
				{
					this.Table._nullConstraintViolationDuringDataLoad = true;
				}
				this._nullConstraintMessage = "Column '" + col.ColumnName + "' does not allow nulls.";
			}
		}

		public string RowError
		{
			get
			{
				return this.rowError;
			}
			set
			{
				this.rowError = value;
			}
		}

		internal int IndexFromVersion(DataRowVersion version)
		{
			if (version == DataRowVersion.Original)
			{
				return this.AssertValidVersionIndex(version, this.Original);
			}
			if (version == DataRowVersion.Current)
			{
				return this.AssertValidVersionIndex(version, this.Current);
			}
			if (version == DataRowVersion.Proposed)
			{
				return this.AssertValidVersionIndex(version, this.Proposed);
			}
			if (version != DataRowVersion.Default)
			{
				throw new DataException("Version must be Original, Current, or Proposed.");
			}
			if (this.Proposed >= 0)
			{
				return this.Proposed;
			}
			if (this.Current >= 0)
			{
				return this.Current;
			}
			if (this.Original < 0)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			throw new DeletedRowInaccessibleException("Deleted row information cannot be accessed through the row.");
		}

		private int AssertValidVersionIndex(DataRowVersion version, int index)
		{
			if (index >= 0)
			{
				return index;
			}
			throw new VersionNotFoundException(string.Format("There is no {0} data to accces.", version));
		}

		internal DataRowVersion VersionFromIndex(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException("Index must not be negative.");
			}
			if (index == this.Current)
			{
				return DataRowVersion.Current;
			}
			if (index == this.Original)
			{
				return DataRowVersion.Original;
			}
			if (index == this.Proposed)
			{
				return DataRowVersion.Proposed;
			}
			throw new ArgumentException(string.Format("The index {0} does not belong to this row.", index));
		}

		internal XmlDataDocument.XmlDataElement DataElement
		{
			get
			{
				if (this.mappedElement != null || this._table.DataSet == null || this._table.DataSet._xmlDataDocument == null)
				{
					return this.mappedElement;
				}
				this.mappedElement = new XmlDataDocument.XmlDataElement(this, this._table.Prefix, XmlHelper.Encode(this._table.TableName), this._table.Namespace, this._table.DataSet._xmlDataDocument);
				return this.mappedElement;
			}
			set
			{
				this.mappedElement = value;
			}
		}

		internal void SetOriginalValue(string columnName, object val)
		{
			DataColumn dataColumn = this._table.Columns[columnName];
			this._table.ChangingDataColumn(this, dataColumn, val);
			if (this.Original < 0 || this.Original == this.Current)
			{
				this.Original = this.Table.RecordCache.NewRecord();
			}
			this.CheckValue(val, dataColumn);
			dataColumn[this.Original] = val;
		}

		public void AcceptChanges()
		{
			this.EndEdit();
			this._table.ChangingDataRow(this, DataRowAction.Commit);
			this.CheckChildRows(DataRowAction.Commit);
			DataRowState rowState = this.RowState;
			switch (rowState)
			{
			case DataRowState.Detached:
				throw new RowNotInTableException("Cannot perform this operation on a row not in the table.");
			default:
				if (rowState == DataRowState.Deleted)
				{
					this.Detach();
					goto IL_0096;
				}
				if (rowState != DataRowState.Modified)
				{
					goto IL_0096;
				}
				break;
			case DataRowState.Added:
				break;
			}
			if (this.Original >= 0)
			{
				this.Table.RecordCache.DisposeRecord(this.Original);
			}
			this.Original = this.Current;
			IL_0096:
			this._table.ChangedDataRow(this, DataRowAction.Commit);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void BeginEdit()
		{
			if (this._inChangingEvent)
			{
				throw new InRowChangingEventException("Cannot call BeginEdit inside an OnRowChanging event.");
			}
			if (this.RowState == DataRowState.Deleted)
			{
				throw new DeletedRowInaccessibleException();
			}
			if (!this.HasVersion(DataRowVersion.Proposed))
			{
				this.Proposed = this.Table.RecordCache.NewRecord();
				int num = ((!this.HasVersion(DataRowVersion.Current)) ? this.Table.DefaultValuesRowIndex : this.Current);
				for (int i = 0; i < this.Table.Columns.Count; i++)
				{
					DataColumn dataColumn = this.Table.Columns[i];
					dataColumn.DataContainer.CopyValue(num, this.Proposed);
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void CancelEdit()
		{
			if (this._inChangingEvent)
			{
				throw new InRowChangingEventException("Cannot call CancelEdit inside an OnRowChanging event.");
			}
			if (this.HasVersion(DataRowVersion.Proposed))
			{
				int proposed = this.Proposed;
				DataRowState rowState = this.RowState;
				this.Table.RecordCache.DisposeRecord(this.Proposed);
				this.Proposed = -1;
				foreach (object obj in this.Table.Indexes)
				{
					Index index = (Index)obj;
					index.Update(this, proposed, DataRowVersion.Proposed, rowState);
				}
			}
		}

		public void ClearErrors()
		{
			this.rowError = string.Empty;
			this.ColumnErrors.Clear();
		}

		public void Delete()
		{
			this._table.DeletingDataRow(this, DataRowAction.Delete);
			DataRowState rowState = this.RowState;
			switch (rowState)
			{
			case DataRowState.Detached:
				break;
			default:
				if (rowState != DataRowState.Deleted)
				{
					this.CheckChildRows(DataRowAction.Delete);
				}
				break;
			case DataRowState.Added:
				this.CheckChildRows(DataRowAction.Delete);
				this.Detach();
				break;
			}
			if (this.Current >= 0)
			{
				int num = this.Current;
				DataRowState rowState2 = this.RowState;
				if (this.Current != this.Original)
				{
					this._table.RecordCache.DisposeRecord(this.Current);
				}
				this.Current = -1;
				foreach (object obj in this.Table.Indexes)
				{
					Index index = (Index)obj;
					index.Update(this, num, DataRowVersion.Current, rowState2);
				}
			}
			this._table.DeletedDataRow(this, DataRowAction.Delete);
		}

		private void CheckChildRows(DataRowAction action)
		{
			DataSet dataSet = this._table.DataSet;
			if (dataSet == null || !dataSet.EnforceConstraints)
			{
				return;
			}
			if (this._table.Constraints.Count == 0)
			{
				return;
			}
			foreach (object obj in dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				foreach (object obj2 in dataTable.Constraints)
				{
					Constraint constraint = (Constraint)obj2;
					ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
					if (foreignKeyConstraint != null && foreignKeyConstraint.RelatedTable == this._table)
					{
						switch (action)
						{
						case DataRowAction.Delete:
							this.CheckChildRows(foreignKeyConstraint, action, foreignKeyConstraint.DeleteRule);
							continue;
						default:
							if (action != DataRowAction.Commit)
							{
								this.CheckChildRows(foreignKeyConstraint, action, foreignKeyConstraint.UpdateRule);
								continue;
							}
							break;
						case DataRowAction.Rollback:
							break;
						}
						if (foreignKeyConstraint.AcceptRejectRule != AcceptRejectRule.None)
						{
							this.CheckChildRows(foreignKeyConstraint, action, Rule.Cascade);
						}
					}
				}
			}
		}

		private void CheckChildRows(ForeignKeyConstraint fkc, DataRowAction action, Rule rule)
		{
			DataRow[] childRows = this.GetChildRows(fkc, DataRowVersion.Current);
			if (childRows == null)
			{
				return;
			}
			switch (rule)
			{
			case Rule.None:
			{
				for (int i = 0; i < childRows.Length; i++)
				{
					if (childRows[i].RowState != DataRowState.Deleted)
					{
						string text = "Cannot change this row because constraints are enforced on relation " + fkc.ConstraintName + ", and changing this row will strand child rows.";
						string text2 = "Cannot delete this row because constraints are enforced on relation " + fkc.ConstraintName + ", and deleting this row will strand child rows.";
						string text3 = ((action != DataRowAction.Delete) ? text : text2);
						throw new InvalidConstraintException(text3);
					}
				}
				break;
			}
			case Rule.Cascade:
				switch (action)
				{
				case DataRowAction.Delete:
				{
					for (int j = 0; j < childRows.Length; j++)
					{
						if (childRows[j].RowState != DataRowState.Deleted)
						{
							childRows[j].Delete();
						}
					}
					break;
				}
				case DataRowAction.Change:
				{
					for (int k = 0; k < childRows.Length; k++)
					{
						for (int l = 0; l < fkc.Columns.Length; l++)
						{
							if (!fkc.RelatedColumns[l].DataContainer[this.Current].Equals(fkc.RelatedColumns[l].DataContainer[this.Proposed]))
							{
								childRows[k][fkc.Columns[l]] = this[fkc.RelatedColumns[l], DataRowVersion.Proposed];
							}
						}
					}
					break;
				}
				case DataRowAction.Rollback:
				{
					for (int m = 0; m < childRows.Length; m++)
					{
						if (childRows[m].RowState != DataRowState.Unchanged)
						{
							childRows[m].RejectChanges();
						}
					}
					break;
				}
				}
				break;
			case Rule.SetNull:
			{
				for (int n = 0; n < childRows.Length; n++)
				{
					DataRow dataRow = childRows[n];
					if (childRows[n].RowState != DataRowState.Deleted)
					{
						for (int num = 0; num < fkc.Columns.Length; num++)
						{
							dataRow.SetNull(fkc.Columns[num]);
						}
					}
				}
				break;
			}
			case Rule.SetDefault:
				if (childRows.Length > 0)
				{
					int defaultValuesRowIndex = childRows[0].Table.DefaultValuesRowIndex;
					foreach (DataRow dataRow2 in childRows)
					{
						if (dataRow2.RowState != DataRowState.Deleted)
						{
							int num3 = dataRow2.IndexFromVersion(DataRowVersion.Default);
							foreach (DataColumn dataColumn in fkc.Columns)
							{
								dataColumn.DataContainer.CopyValue(defaultValuesRowIndex, num3);
							}
						}
					}
				}
				break;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndEdit()
		{
			if (this._inChangingEvent)
			{
				throw new InRowChangingEventException("Cannot call EndEdit inside an OnRowChanging event.");
			}
			if (this.RowState == DataRowState.Detached || !this.HasVersion(DataRowVersion.Proposed))
			{
				return;
			}
			this.CheckReadOnlyStatus();
			this._inChangingEvent = true;
			try
			{
				this._table.ChangingDataRow(this, DataRowAction.Change);
			}
			finally
			{
				this._inChangingEvent = false;
			}
			DataRowState rowState = this.RowState;
			int num = this.Current;
			this.Current = this.Proposed;
			this.Proposed = -1;
			foreach (object obj in this.Table.Indexes)
			{
				Index index = (Index)obj;
				index.Update(this, num, DataRowVersion.Current, rowState);
			}
			try
			{
				this.AssertConstraints();
				this.Proposed = this.Current;
				this.Current = num;
				this.CheckChildRows(DataRowAction.Change);
				this.Current = this.Proposed;
				this.Proposed = -1;
			}
			catch
			{
				int num2 = ((this.Proposed < 0) ? this.Current : this.Proposed);
				this.Current = num;
				foreach (object obj2 in this.Table.Indexes)
				{
					Index index2 = (Index)obj2;
					index2.Update(this, num2, DataRowVersion.Current, this.RowState);
				}
				throw;
			}
			if (this.Original != num)
			{
				this.Table.RecordCache.DisposeRecord(num);
			}
			if (this._rowChanged)
			{
				this._table.ChangedDataRow(this, DataRowAction.Change);
				this._rowChanged = false;
			}
		}

		public DataRow[] GetChildRows(DataRelation relation)
		{
			return this.GetChildRows(relation, DataRowVersion.Default);
		}

		public DataRow[] GetChildRows(string relationName)
		{
			return this.GetChildRows(this.Table.DataSet.Relations[relationName]);
		}

		public DataRow[] GetChildRows(DataRelation relation, DataRowVersion version)
		{
			if (relation == null)
			{
				return this.Table.NewRowArray(0);
			}
			if (this.Table == null)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			if (relation.DataSet != this.Table.DataSet)
			{
				throw new ArgumentException();
			}
			if (this._table != relation.ParentTable)
			{
				throw new InvalidConstraintException(string.Concat(new object[] { "GetChildRow requires a row whose Table is ", relation.ParentTable, ", but the specified row's table is ", this._table }));
			}
			if (relation.ChildKeyConstraint != null)
			{
				return this.GetChildRows(relation.ChildKeyConstraint, version);
			}
			ArrayList arrayList = new ArrayList();
			DataColumn[] parentColumns = relation.ParentColumns;
			DataColumn[] childColumns = relation.ChildColumns;
			int num = parentColumns.Length;
			DataRow[] array = null;
			int num2 = this.IndexFromVersion(version);
			int num3 = relation.ChildTable.RecordCache.NewRecord();
			try
			{
				for (int i = 0; i < num; i++)
				{
					childColumns[i].DataContainer.CopyValue(parentColumns[i].DataContainer, num2, num3);
				}
				Index index = relation.ChildTable.FindIndex(childColumns);
				if (index != null)
				{
					int[] array2 = index.FindAll(num3);
					array = relation.ChildTable.NewRowArray(array2.Length);
					for (int j = 0; j < array2.Length; j++)
					{
						array[j] = relation.ChildTable.RecordCache[array2[j]];
					}
				}
				else
				{
					foreach (object obj in relation.ChildTable.Rows)
					{
						DataRow dataRow = (DataRow)obj;
						bool flag = false;
						if (dataRow.HasVersion(DataRowVersion.Default))
						{
							flag = true;
							int num4 = dataRow.IndexFromVersion(DataRowVersion.Default);
							for (int k = 0; k < num; k++)
							{
								if (childColumns[k].DataContainer.CompareValues(num4, num3) != 0)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							arrayList.Add(dataRow);
						}
					}
					array = relation.ChildTable.NewRowArray(arrayList.Count);
					arrayList.CopyTo(array, 0);
				}
			}
			finally
			{
				relation.ChildTable.RecordCache.DisposeRecord(num3);
			}
			return array;
		}

		public DataRow[] GetChildRows(string relationName, DataRowVersion version)
		{
			return this.GetChildRows(this.Table.DataSet.Relations[relationName], version);
		}

		private DataRow[] GetChildRows(ForeignKeyConstraint fkc, DataRowVersion version)
		{
			ArrayList arrayList = new ArrayList();
			DataColumn[] relatedColumns = fkc.RelatedColumns;
			DataColumn[] columns = fkc.Columns;
			int num = relatedColumns.Length;
			Index index = fkc.Index;
			int num2 = this.IndexFromVersion(version);
			int num3 = fkc.Table.RecordCache.NewRecord();
			for (int i = 0; i < num; i++)
			{
				columns[i].DataContainer.CopyValue(relatedColumns[i].DataContainer, num2, num3);
			}
			try
			{
				if (index != null)
				{
					int[] array = index.FindAll(num3);
					for (int j = 0; j < array.Length; j++)
					{
						arrayList.Add(columns[j].Table.RecordCache[array[j]]);
					}
				}
				else
				{
					foreach (object obj in fkc.Table.Rows)
					{
						DataRow dataRow = (DataRow)obj;
						bool flag = false;
						if (dataRow.HasVersion(DataRowVersion.Default))
						{
							flag = true;
							int num4 = dataRow.IndexFromVersion(DataRowVersion.Default);
							for (int k = 0; k < num; k++)
							{
								if (columns[k].DataContainer.CompareValues(num4, num3) != 0)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							arrayList.Add(dataRow);
						}
					}
				}
			}
			finally
			{
				fkc.Table.RecordCache.DisposeRecord(num3);
			}
			DataRow[] array2 = fkc.Table.NewRowArray(arrayList.Count);
			arrayList.CopyTo(array2, 0);
			return array2;
		}

		public string GetColumnError(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			int num = this._table.Columns.IndexOf(column);
			if (num < 0)
			{
				throw new ArgumentException(string.Format("Column '{0}' does not belong to table {1}.", column.ColumnName, this.Table.TableName));
			}
			return this.GetColumnError(num);
		}

		public string GetColumnError(int columnIndex)
		{
			if (columnIndex < 0 || columnIndex >= this.Table.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			string text = null;
			if (columnIndex < this.ColumnErrors.Count)
			{
				text = (string)this.ColumnErrors[columnIndex];
			}
			return (text == null) ? string.Empty : text;
		}

		public string GetColumnError(string columnName)
		{
			return this.GetColumnError(this._table.Columns.IndexOf(columnName));
		}

		public DataColumn[] GetColumnsInError()
		{
			ArrayList arrayList = new ArrayList();
			int num = 0;
			foreach (object obj in this.ColumnErrors)
			{
				string text = (string)obj;
				if (text != null && text != string.Empty)
				{
					arrayList.Add(this._table.Columns[num]);
				}
				num++;
			}
			return (DataColumn[])arrayList.ToArray(typeof(DataColumn));
		}

		public DataRow GetParentRow(DataRelation relation)
		{
			return this.GetParentRow(relation, DataRowVersion.Default);
		}

		public DataRow GetParentRow(string relationName)
		{
			return this.GetParentRow(relationName, DataRowVersion.Default);
		}

		public DataRow GetParentRow(DataRelation relation, DataRowVersion version)
		{
			DataRow[] parentRows = this.GetParentRows(relation, version);
			if (parentRows.Length == 0)
			{
				return null;
			}
			return parentRows[0];
		}

		public DataRow GetParentRow(string relationName, DataRowVersion version)
		{
			return this.GetParentRow(this.Table.DataSet.Relations[relationName], version);
		}

		public DataRow[] GetParentRows(DataRelation relation)
		{
			return this.GetParentRows(relation, DataRowVersion.Default);
		}

		public DataRow[] GetParentRows(string relationName)
		{
			return this.GetParentRows(relationName, DataRowVersion.Default);
		}

		public DataRow[] GetParentRows(DataRelation relation, DataRowVersion version)
		{
			if (relation == null)
			{
				return this.Table.NewRowArray(0);
			}
			if (this.Table == null)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			if (relation.DataSet != this.Table.DataSet)
			{
				throw new ArgumentException();
			}
			if (this._table != relation.ChildTable)
			{
				throw new InvalidConstraintException(string.Concat(new object[] { "GetParentRows requires a row whose Table is ", relation.ChildTable, ", but the specified row's table is ", this._table }));
			}
			ArrayList arrayList = new ArrayList();
			DataColumn[] parentColumns = relation.ParentColumns;
			DataColumn[] childColumns = relation.ChildColumns;
			int num = parentColumns.Length;
			int num2 = this.IndexFromVersion(version);
			int num3 = relation.ParentTable.RecordCache.NewRecord();
			for (int i = 0; i < num; i++)
			{
				parentColumns[i].DataContainer.CopyValue(childColumns[i].DataContainer, num2, num3);
			}
			try
			{
				Index index = relation.ParentTable.FindIndex(parentColumns);
				if (index != null)
				{
					int[] array = index.FindAll(num3);
					for (int j = 0; j < array.Length; j++)
					{
						arrayList.Add(parentColumns[j].Table.RecordCache[array[j]]);
					}
				}
				else
				{
					foreach (object obj in relation.ParentTable.Rows)
					{
						DataRow dataRow = (DataRow)obj;
						bool flag = false;
						if (dataRow.HasVersion(DataRowVersion.Default))
						{
							flag = true;
							int num4 = dataRow.IndexFromVersion(DataRowVersion.Default);
							for (int k = 0; k < num; k++)
							{
								if (parentColumns[k].DataContainer.CompareValues(num4, num3) != 0)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							arrayList.Add(dataRow);
						}
					}
				}
			}
			finally
			{
				relation.ParentTable.RecordCache.DisposeRecord(num3);
			}
			DataRow[] array2 = relation.ParentTable.NewRowArray(arrayList.Count);
			arrayList.CopyTo(array2, 0);
			return array2;
		}

		public DataRow[] GetParentRows(string relationName, DataRowVersion version)
		{
			return this.GetParentRows(this.Table.DataSet.Relations[relationName], version);
		}

		public bool HasVersion(DataRowVersion version)
		{
			if (version == DataRowVersion.Original)
			{
				return this.Original >= 0;
			}
			if (version == DataRowVersion.Current)
			{
				return this.Current >= 0;
			}
			if (version == DataRowVersion.Proposed)
			{
				return this.Proposed >= 0;
			}
			if (version != DataRowVersion.Default)
			{
				return this.IndexFromVersion(version) >= 0;
			}
			return this.Proposed >= 0 || this.Current >= 0;
		}

		public bool IsNull(DataColumn column)
		{
			return this.IsNull(column, DataRowVersion.Default);
		}

		public bool IsNull(int columnIndex)
		{
			return this.IsNull(this.Table.Columns[columnIndex]);
		}

		public bool IsNull(string columnName)
		{
			return this.IsNull(this.Table.Columns[columnName]);
		}

		public bool IsNull(DataColumn column, DataRowVersion version)
		{
			object obj = this[column, version];
			return column.DataContainer.IsNull(this.IndexFromVersion(version));
		}

		internal bool IsNullColumns(DataColumn[] columns)
		{
			int i;
			for (i = 0; i < columns.Length; i++)
			{
				if (!this.IsNull(columns[i]))
				{
					break;
				}
			}
			return i == columns.Length;
		}

		public void RejectChanges()
		{
			if (this.RowState == DataRowState.Detached)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			this.Table.ChangingDataRow(this, DataRowAction.Rollback);
			DataRowState rowState = this.RowState;
			if (rowState != DataRowState.Added)
			{
				if (rowState != DataRowState.Deleted)
				{
					if (rowState == DataRowState.Modified)
					{
						int num = this.Current;
						this.Table.RecordCache.DisposeRecord(this.Current);
						this.CheckChildRows(DataRowAction.Rollback);
						this.Current = this.Original;
						foreach (object obj in this.Table.Indexes)
						{
							Index index = (Index)obj;
							index.Update(this, num, DataRowVersion.Current, DataRowState.Modified);
						}
					}
				}
				else
				{
					this.CheckChildRows(DataRowAction.Rollback);
					this.Current = this.Original;
					this.Validate();
				}
			}
			else
			{
				this.Detach();
			}
			this.Table.ChangedDataRow(this, DataRowAction.Rollback);
		}

		public void SetColumnError(DataColumn column, string error)
		{
			this.SetColumnError(this._table.Columns.IndexOf(column), error);
		}

		public void SetColumnError(int columnIndex, string error)
		{
			if (columnIndex < 0 || columnIndex >= this.Table.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			while (columnIndex >= this.ColumnErrors.Count)
			{
				this.ColumnErrors.Add(null);
			}
			this.ColumnErrors[columnIndex] = error;
		}

		public void SetColumnError(string columnName, string error)
		{
			this.SetColumnError(this._table.Columns.IndexOf(columnName), error);
		}

		protected void SetNull(DataColumn column)
		{
			this[column] = DBNull.Value;
		}

		public void SetParentRow(DataRow parentRow)
		{
			this.SetParentRow(parentRow, null);
		}

		public void SetParentRow(DataRow parentRow, DataRelation relation)
		{
			if (this._table == null || parentRow.Table == null)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			if (parentRow != null && this._table.DataSet != parentRow.Table.DataSet)
			{
				throw new ArgumentException();
			}
			if (this.RowState == DataRowState.Detached && !this.HasVersion(DataRowVersion.Default))
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			this.BeginEdit();
			IEnumerable enumerable;
			if (relation == null)
			{
				enumerable = this._table.ParentRelations;
			}
			else
			{
				enumerable = new DataRelation[] { relation };
			}
			foreach (object obj in enumerable)
			{
				DataRelation dataRelation = (DataRelation)obj;
				DataColumn[] childColumns = dataRelation.ChildColumns;
				DataColumn[] parentColumns = dataRelation.ParentColumns;
				for (int i = 0; i < parentColumns.Length; i++)
				{
					if (parentRow == null)
					{
						childColumns[i].DataContainer[this.Proposed] = DBNull.Value;
					}
					else
					{
						int num = parentRow.IndexFromVersion(DataRowVersion.Default);
						childColumns[i].DataContainer.CopyValue(parentColumns[i].DataContainer, num, this.Proposed);
					}
				}
			}
			this.EndEdit();
		}

		internal void CopyValuesToRow(DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			if (row == this)
			{
				throw new ArgumentException("'row' is the same as this object");
			}
			if (this.HasVersion(DataRowVersion.Original))
			{
				if (row.Original < 0)
				{
					row.Original = row.Table.RecordCache.NewRecord();
				}
				else if (row.Original == row.Current)
				{
					row.Original = row.Table.RecordCache.NewRecord();
					row.Table.RecordCache.CopyRecord(row.Table, row.Current, row.Original);
				}
			}
			else if (row.Original > 0)
			{
				if (row.Original != row.Current)
				{
					row.Table.RecordCache.DisposeRecord(row.Original);
				}
				row.Original = -1;
			}
			if (this.HasVersion(DataRowVersion.Current))
			{
				if (this.Current == this.Original)
				{
					if (row.Current >= 0)
					{
						row.Table.RecordCache.DisposeRecord(row.Current);
					}
					row.Current = row.Original;
				}
				else if (row.Current < 0)
				{
					row.Current = row.Table.RecordCache.NewRecord();
				}
			}
			else if (row.Current > 0)
			{
				row.Table.RecordCache.DisposeRecord(row.Current);
				row.Current = -1;
			}
			if (this.HasVersion(DataRowVersion.Proposed))
			{
				if (row.Proposed < 0)
				{
					row.Proposed = row.Table.RecordCache.NewRecord();
				}
			}
			else if (row.Proposed > 0)
			{
				row.Table.RecordCache.DisposeRecord(row.Proposed);
				row.Proposed = -1;
			}
			foreach (object obj in this.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				DataColumn dataColumn2 = row.Table.Columns[dataColumn.ColumnName];
				if (dataColumn2 != null)
				{
					if (this.HasVersion(DataRowVersion.Original))
					{
						object obj2 = dataColumn[this.Original];
						row.CheckValue(obj2, dataColumn2);
						dataColumn2[row.Original] = obj2;
					}
					if (this.HasVersion(DataRowVersion.Current) && this.Current != this.Original)
					{
						object obj3 = dataColumn[this.Current];
						row.CheckValue(obj3, dataColumn2);
						dataColumn2[row.Current] = obj3;
					}
					if (this.HasVersion(DataRowVersion.Proposed))
					{
						object obj4 = dataColumn[row.Proposed];
						row.CheckValue(obj4, dataColumn2);
						dataColumn2[row.Proposed] = obj4;
					}
				}
			}
			if (this.HasErrors)
			{
				this.CopyErrors(row);
			}
		}

		internal void MergeValuesToRow(DataRow row, bool preserveChanges)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			if (row == this)
			{
				throw new ArgumentException("'row' is the same as this object");
			}
			if (this.HasVersion(DataRowVersion.Original))
			{
				if (row.Original < 0)
				{
					row.Original = row.Table.RecordCache.NewRecord();
				}
				else if (row.Original == row.Current && (this.Original != this.Current || preserveChanges))
				{
					row.Original = row.Table.RecordCache.NewRecord();
					row.Table.RecordCache.CopyRecord(row.Table, row.Current, row.Original);
				}
			}
			else if (row.Original == row.Current)
			{
				row.Original = row.Table.RecordCache.NewRecord();
				row.Table.RecordCache.CopyRecord(row.Table, row.Current, row.Original);
			}
			if (this.HasVersion(DataRowVersion.Current))
			{
				if (!preserveChanges && row.Current < 0)
				{
					row.Current = row.Table.RecordCache.NewRecord();
				}
			}
			else if (row.Current > 0 && !preserveChanges)
			{
				row.Table.RecordCache.DisposeRecord(row.Current);
				row.Current = -1;
			}
			foreach (object obj in this.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				DataColumn dataColumn2 = row.Table.Columns[dataColumn.ColumnName];
				if (dataColumn2 != null)
				{
					if (this.HasVersion(DataRowVersion.Original))
					{
						object obj2 = dataColumn[this.Original];
						row.CheckValue(obj2, dataColumn2);
						dataColumn2[row.Original] = obj2;
					}
					if (this.HasVersion(DataRowVersion.Current) && !preserveChanges)
					{
						object obj3 = dataColumn[this.Current];
						row.CheckValue(obj3, dataColumn2);
						dataColumn2[row.Current] = obj3;
					}
				}
			}
			if (this.HasErrors)
			{
				this.CopyErrors(row);
			}
		}

		internal void CopyErrors(DataRow row)
		{
			row.RowError = this.RowError;
			DataColumn[] columnsInError = this.GetColumnsInError();
			foreach (DataColumn dataColumn in columnsInError)
			{
				DataColumn dataColumn2 = row.Table.Columns[dataColumn.ColumnName];
				row.SetColumnError(dataColumn2, this.GetColumnError(dataColumn));
			}
		}

		internal bool IsRowChanged(DataRowState rowState)
		{
			if ((this.RowState & rowState) != (DataRowState)0)
			{
				return true;
			}
			DataRowVersion dataRowVersion = ((rowState != DataRowState.Deleted) ? DataRowVersion.Current : DataRowVersion.Original);
			int count = this.Table.ChildRelations.Count;
			for (int i = 0; i < count; i++)
			{
				DataRelation dataRelation = this.Table.ChildRelations[i];
				DataRow[] childRows = this.GetChildRows(dataRelation, dataRowVersion);
				for (int j = 0; j < childRows.Length; j++)
				{
					if (childRows[j].IsRowChanged(rowState))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void Validate()
		{
			this.Table.AddRowToIndexes(this);
			this.AssertConstraints();
		}

		private void AssertConstraints()
		{
			if (this.Table == null || this.Table._duringDataLoad)
			{
				return;
			}
			if (this.Table.DataSet != null && !this.Table.DataSet.EnforceConstraints)
			{
				return;
			}
			for (int i = 0; i < this.Table.Columns.Count; i++)
			{
				DataColumn dataColumn = this.Table.Columns[i];
				if (!dataColumn.AllowDBNull && this.IsNull(dataColumn))
				{
					throw new NoNullAllowedException(this._nullConstraintMessage);
				}
			}
			foreach (object obj in this.Table.Constraints)
			{
				Constraint constraint = (Constraint)obj;
				try
				{
					constraint.AssertConstraint(this);
				}
				catch (Exception ex)
				{
					this.Table.DeleteRowFromIndexes(this);
					throw ex;
				}
			}
		}

		internal void CheckNullConstraints()
		{
			if (this._nullConstraintViolation)
			{
				if (this.HasVersion(DataRowVersion.Proposed))
				{
					foreach (object obj in this.Table.Columns)
					{
						DataColumn dataColumn = (DataColumn)obj;
						if (this.IsNull(dataColumn) && !dataColumn.AllowDBNull)
						{
							throw new NoNullAllowedException(this._nullConstraintMessage);
						}
					}
				}
				this._nullConstraintViolation = false;
			}
		}

		internal void CheckReadOnlyStatus()
		{
			int num = this.IndexFromVersion(DataRowVersion.Default);
			foreach (object obj in this.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.DataContainer.CompareValues(num, this.Proposed) != 0 && dataColumn.ReadOnly)
				{
					throw new ReadOnlyException();
				}
			}
		}

		internal void Load(object[] values, LoadOption loadOption)
		{
			if (loadOption == LoadOption.OverwriteChanges || (loadOption == LoadOption.PreserveChanges && this.RowState == DataRowState.Unchanged))
			{
				this.Table.ChangingDataRow(this, DataRowAction.ChangeCurrentAndOriginal);
				int num = this.Table.CreateRecord(values);
				this.Table.DeleteRowFromIndexes(this);
				if (this.HasVersion(DataRowVersion.Original) && this.Current != this.Original)
				{
					this.Table.RecordCache.DisposeRecord(this.Original);
				}
				this.Original = num;
				if (this.HasVersion(DataRowVersion.Current))
				{
					this.Table.RecordCache.DisposeRecord(this.Current);
				}
				this.Current = num;
				this.Table.AddRowToIndexes(this);
				this.Table.ChangedDataRow(this, DataRowAction.ChangeCurrentAndOriginal);
				return;
			}
			if (loadOption == LoadOption.PreserveChanges)
			{
				this.Table.ChangingDataRow(this, DataRowAction.ChangeOriginal);
				int num = this.Table.CreateRecord(values);
				if (this.HasVersion(DataRowVersion.Original) && this.Current != this.Original)
				{
					this.Table.RecordCache.DisposeRecord(this.Original);
				}
				this.Original = num;
				this.Table.ChangedDataRow(this, DataRowAction.ChangeOriginal);
				return;
			}
			if (this.RowState != DataRowState.Deleted)
			{
				int num2 = ((!this.HasVersion(DataRowVersion.Proposed)) ? this.Current : this.Proposed);
				int num = this.Table.CreateRecord(values);
				if (this.RowState == DataRowState.Added || this.Table.CompareRecords(num2, num) != 0)
				{
					this.Table.ChangingDataRow(this, DataRowAction.Change);
					this.Table.DeleteRowFromIndexes(this);
					if (this.HasVersion(DataRowVersion.Proposed))
					{
						this.Table.RecordCache.DisposeRecord(this.Proposed);
						this.Proposed = -1;
					}
					if (this.Original != this.Current)
					{
						this.Table.RecordCache.DisposeRecord(this.Current);
					}
					this.Current = num;
					this.Table.AddRowToIndexes(this);
					this.Table.ChangedDataRow(this, DataRowAction.Change);
				}
				else
				{
					this.Table.ChangingDataRow(this, DataRowAction.Nothing);
					this.Table.RecordCache.DisposeRecord(num);
					this.Table.ChangedDataRow(this, DataRowAction.Nothing);
				}
			}
		}

		private DataTable _table;

		internal int _original = -1;

		internal int _current = -1;

		internal int _proposed = -1;

		private ArrayList _columnErrors;

		private string rowError;

		internal int xmlRowID;

		internal bool _nullConstraintViolation;

		private string _nullConstraintMessage;

		private bool _inChangingEvent;

		private int _rowId;

		internal bool _rowChanged;

		private XmlDataDocument.XmlDataElement mappedElement;

		internal bool _inExpressionEvaluation;
	}
}
