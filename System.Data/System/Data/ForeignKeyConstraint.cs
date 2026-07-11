using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	[DefaultProperty("ConstraintName")]
	public class ForeignKeyConstraint : Constraint
	{
		public ForeignKeyConstraint(DataColumn parentColumn, DataColumn childColumn)
			: this(null, parentColumn, childColumn)
		{
		}

		public ForeignKeyConstraint(string constraintName, DataColumn parentColumn, DataColumn childColumn)
		{
			this._deleteRule = Rule.Cascade;
			this._updateRule = Rule.Cascade;
			base..ctor();
			DataColumn[] array = new DataColumn[] { parentColumn };
			DataColumn[] array2 = new DataColumn[] { childColumn };
			this.Create(constraintName, array, array2);
		}

		public ForeignKeyConstraint(DataColumn[] parentColumns, DataColumn[] childColumns)
			: this(null, parentColumns, childColumns)
		{
		}

		public ForeignKeyConstraint(string constraintName, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			this._deleteRule = Rule.Cascade;
			this._updateRule = Rule.Cascade;
			base..ctor();
			this.Create(constraintName, parentColumns, childColumns);
		}

		[Browsable(false)]
		public ForeignKeyConstraint(string constraintName, string parentTableName, string[] parentColumnNames, string[] childColumnNames, AcceptRejectRule acceptRejectRule, Rule deleteRule, Rule updateRule)
		{
			this._deleteRule = Rule.Cascade;
			this._updateRule = Rule.Cascade;
			base..ctor();
			this._constraintName = constraintName;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._parentTableName = parentTableName;
			this._acceptRejectRule = acceptRejectRule;
			this._deleteRule = deleteRule;
			this._updateRule = updateRule;
		}

		[Browsable(false)]
		public ForeignKeyConstraint(string constraintName, string parentTableName, string parentTableNamespace, string[] parentColumnNames, string[] childColumnNames, AcceptRejectRule acceptRejectRule, Rule deleteRule, Rule updateRule)
		{
			this._deleteRule = Rule.Cascade;
			this._updateRule = Rule.Cascade;
			base..ctor();
			this._constraintName = constraintName;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._parentTableName = parentTableName;
			this._parentTableNamespace = parentTableNamespace;
			this._acceptRejectRule = acceptRejectRule;
			this._deleteRule = deleteRule;
			this._updateRule = updateRule;
		}

		internal DataKey ChildKey
		{
			get
			{
				base.CheckStateForProperty();
				return this._childKey;
			}
		}

		[ReadOnly(true)]
		public virtual DataColumn[] Columns
		{
			get
			{
				base.CheckStateForProperty();
				return this._childKey.ToArray();
			}
		}

		[ReadOnly(true)]
		public override DataTable Table
		{
			get
			{
				base.CheckStateForProperty();
				return this._childKey.Table;
			}
		}

		internal string[] ParentColumnNames
		{
			get
			{
				return this._parentKey.GetColumnNames();
			}
		}

		internal string[] ChildColumnNames
		{
			get
			{
				return this._childKey.GetColumnNames();
			}
		}

		internal override void CheckCanAddToCollection(ConstraintCollection constraints)
		{
			if (this.Table != constraints.Table)
			{
				throw ExceptionBuilder.ConstraintAddFailed(constraints.Table);
			}
			if (this.Table.Locale.LCID != this.RelatedTable.Locale.LCID || this.Table.CaseSensitive != this.RelatedTable.CaseSensitive)
			{
				throw ExceptionBuilder.CaseLocaleMismatch();
			}
		}

		internal override bool CanBeRemovedFromCollection(ConstraintCollection constraints, bool fThrowException)
		{
			return true;
		}

		internal bool IsKeyNull(object[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (!DataStorage.IsObjectNull(values[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal override bool IsConstraintViolated()
		{
			Index sortIndex = this._childKey.GetSortIndex();
			object[] uniqueKeyValues = sortIndex.GetUniqueKeyValues();
			bool flag = false;
			Index sortIndex2 = this._parentKey.GetSortIndex();
			foreach (object[] array in uniqueKeyValues)
			{
				if (!this.IsKeyNull(array) && !sortIndex2.IsKeyInIndex(array))
				{
					DataRow[] rows = sortIndex.GetRows(sortIndex.FindRecords(array));
					string text = SR.Format("ForeignKeyConstraint {0} requires the child key values ({1}) to exist in the parent table.", this.ConstraintName, ExceptionBuilder.KeysToString(array));
					for (int j = 0; j < rows.Length; j++)
					{
						rows[j].RowError = text;
					}
					flag = true;
				}
			}
			return flag;
		}

		internal override bool CanEnableConstraint()
		{
			if (this.Table.DataSet == null || !this.Table.DataSet.EnforceConstraints)
			{
				return true;
			}
			object[] uniqueKeyValues = this._childKey.GetSortIndex().GetUniqueKeyValues();
			Index sortIndex = this._parentKey.GetSortIndex();
			foreach (object[] array in uniqueKeyValues)
			{
				if (!this.IsKeyNull(array) && !sortIndex.IsKeyInIndex(array))
				{
					return false;
				}
			}
			return true;
		}

		internal void CascadeCommit(DataRow row)
		{
			if (row.RowState == DataRowState.Detached)
			{
				return;
			}
			if (this._acceptRejectRule == AcceptRejectRule.Cascade)
			{
				Index sortIndex = this._childKey.GetSortIndex((row.RowState == DataRowState.Deleted) ? DataViewRowState.Deleted : DataViewRowState.CurrentRows);
				object[] keyValues = row.GetKeyValues(this._parentKey, (row.RowState == DataRowState.Deleted) ? DataRowVersion.Original : DataRowVersion.Default);
				if (this.IsKeyNull(keyValues))
				{
					return;
				}
				Range range = sortIndex.FindRecords(keyValues);
				if (!range.IsNull)
				{
					foreach (DataRow dataRow in sortIndex.GetRows(range))
					{
						if (DataRowState.Detached != dataRow.RowState && !dataRow._inCascade)
						{
							dataRow.AcceptChanges();
						}
					}
				}
			}
		}

		internal void CascadeDelete(DataRow row)
		{
			if (-1 == row._newRecord)
			{
				return;
			}
			object[] keyValues = row.GetKeyValues(this._parentKey, DataRowVersion.Current);
			if (this.IsKeyNull(keyValues))
			{
				return;
			}
			Index sortIndex = this._childKey.GetSortIndex();
			switch (this.DeleteRule)
			{
			case Rule.None:
				if (row.Table.DataSet.EnforceConstraints)
				{
					Range range = sortIndex.FindRecords(keyValues);
					if (!range.IsNull)
					{
						if (range.Count == 1 && sortIndex.GetRow(range.Min) == row)
						{
							return;
						}
						throw ExceptionBuilder.FailedCascadeDelete(this.ConstraintName);
					}
				}
				break;
			case Rule.Cascade:
			{
				object[] keyValues2 = row.GetKeyValues(this._parentKey, DataRowVersion.Default);
				Range range2 = sortIndex.FindRecords(keyValues2);
				if (!range2.IsNull)
				{
					foreach (DataRow dataRow in sortIndex.GetRows(range2))
					{
						if (!dataRow._inCascade)
						{
							dataRow.Table.DeleteRow(dataRow);
						}
					}
					return;
				}
				break;
			}
			case Rule.SetNull:
			{
				object[] array = new object[this._childKey.ColumnsReference.Length];
				for (int j = 0; j < this._childKey.ColumnsReference.Length; j++)
				{
					array[j] = DBNull.Value;
				}
				Range range3 = sortIndex.FindRecords(keyValues);
				if (!range3.IsNull)
				{
					DataRow[] rows2 = sortIndex.GetRows(range3);
					for (int k = 0; k < rows2.Length; k++)
					{
						if (row != rows2[k])
						{
							rows2[k].SetKeyValues(this._childKey, array);
						}
					}
					return;
				}
				break;
			}
			case Rule.SetDefault:
			{
				object[] array2 = new object[this._childKey.ColumnsReference.Length];
				for (int l = 0; l < this._childKey.ColumnsReference.Length; l++)
				{
					array2[l] = this._childKey.ColumnsReference[l].DefaultValue;
				}
				Range range4 = sortIndex.FindRecords(keyValues);
				if (!range4.IsNull)
				{
					DataRow[] rows3 = sortIndex.GetRows(range4);
					for (int m = 0; m < rows3.Length; m++)
					{
						if (row != rows3[m])
						{
							rows3[m].SetKeyValues(this._childKey, array2);
						}
					}
				}
				break;
			}
			default:
				return;
			}
		}

		internal void CascadeRollback(DataRow row)
		{
			Index sortIndex = this._childKey.GetSortIndex((row.RowState == DataRowState.Deleted) ? DataViewRowState.OriginalRows : DataViewRowState.CurrentRows);
			object[] keyValues = row.GetKeyValues(this._parentKey, (row.RowState == DataRowState.Modified) ? DataRowVersion.Current : DataRowVersion.Default);
			if (this.IsKeyNull(keyValues))
			{
				return;
			}
			Range range = sortIndex.FindRecords(keyValues);
			if (this._acceptRejectRule == AcceptRejectRule.Cascade)
			{
				if (!range.IsNull)
				{
					DataRow[] rows = sortIndex.GetRows(range);
					for (int i = 0; i < rows.Length; i++)
					{
						if (!rows[i]._inCascade)
						{
							rows[i].RejectChanges();
						}
					}
					return;
				}
			}
			else if (row.RowState != DataRowState.Deleted && row.Table.DataSet.EnforceConstraints && !range.IsNull)
			{
				if (range.Count == 1 && sortIndex.GetRow(range.Min) == row)
				{
					return;
				}
				if (row.HasKeyChanged(this._parentKey))
				{
					throw ExceptionBuilder.FailedCascadeUpdate(this.ConstraintName);
				}
			}
		}

		internal void CascadeUpdate(DataRow row)
		{
			if (-1 == row._newRecord)
			{
				return;
			}
			object[] keyValues = row.GetKeyValues(this._parentKey, DataRowVersion.Current);
			if (!this.Table.DataSet._fInReadXml && this.IsKeyNull(keyValues))
			{
				return;
			}
			Index sortIndex = this._childKey.GetSortIndex();
			switch (this.UpdateRule)
			{
			case Rule.None:
				if (row.Table.DataSet.EnforceConstraints && !sortIndex.FindRecords(keyValues).IsNull)
				{
					throw ExceptionBuilder.FailedCascadeUpdate(this.ConstraintName);
				}
				break;
			case Rule.Cascade:
			{
				Range range = sortIndex.FindRecords(keyValues);
				if (!range.IsNull)
				{
					object[] keyValues2 = row.GetKeyValues(this._parentKey, DataRowVersion.Proposed);
					DataRow[] rows = sortIndex.GetRows(range);
					for (int i = 0; i < rows.Length; i++)
					{
						rows[i].SetKeyValues(this._childKey, keyValues2);
					}
					return;
				}
				break;
			}
			case Rule.SetNull:
			{
				object[] array = new object[this._childKey.ColumnsReference.Length];
				for (int j = 0; j < this._childKey.ColumnsReference.Length; j++)
				{
					array[j] = DBNull.Value;
				}
				Range range2 = sortIndex.FindRecords(keyValues);
				if (!range2.IsNull)
				{
					DataRow[] rows2 = sortIndex.GetRows(range2);
					for (int k = 0; k < rows2.Length; k++)
					{
						rows2[k].SetKeyValues(this._childKey, array);
					}
					return;
				}
				break;
			}
			case Rule.SetDefault:
			{
				object[] array2 = new object[this._childKey.ColumnsReference.Length];
				for (int l = 0; l < this._childKey.ColumnsReference.Length; l++)
				{
					array2[l] = this._childKey.ColumnsReference[l].DefaultValue;
				}
				Range range3 = sortIndex.FindRecords(keyValues);
				if (!range3.IsNull)
				{
					DataRow[] rows3 = sortIndex.GetRows(range3);
					for (int m = 0; m < rows3.Length; m++)
					{
						rows3[m].SetKeyValues(this._childKey, array2);
					}
				}
				break;
			}
			default:
				return;
			}
		}

		internal void CheckCanClearParentTable(DataTable table)
		{
			if (this.Table.DataSet.EnforceConstraints && this.Table.Rows.Count > 0)
			{
				throw ExceptionBuilder.FailedClearParentTable(table.TableName, this.ConstraintName, this.Table.TableName);
			}
		}

		internal void CheckCanRemoveParentRow(DataRow row)
		{
			if (!this.Table.DataSet.EnforceConstraints)
			{
				return;
			}
			if (DataRelation.GetChildRows(this.ParentKey, this.ChildKey, row, DataRowVersion.Default).Length != 0)
			{
				throw ExceptionBuilder.RemoveParentRow(this);
			}
		}

		internal void CheckCascade(DataRow row, DataRowAction action)
		{
			if (row._inCascade)
			{
				return;
			}
			row._inCascade = true;
			try
			{
				if (action == DataRowAction.Change)
				{
					if (row.HasKeyChanged(this._parentKey))
					{
						this.CascadeUpdate(row);
					}
				}
				else if (action == DataRowAction.Delete)
				{
					this.CascadeDelete(row);
				}
				else if (action == DataRowAction.Commit)
				{
					this.CascadeCommit(row);
				}
				else if (action == DataRowAction.Rollback)
				{
					this.CascadeRollback(row);
				}
			}
			finally
			{
				row._inCascade = false;
			}
		}

		internal override void CheckConstraint(DataRow childRow, DataRowAction action)
		{
			if ((action == DataRowAction.Change || action == DataRowAction.Add || action == DataRowAction.Rollback) && this.Table.DataSet != null && this.Table.DataSet.EnforceConstraints && childRow.HasKeyChanged(this._childKey))
			{
				DataRowVersion dataRowVersion = ((action == DataRowAction.Rollback) ? DataRowVersion.Original : DataRowVersion.Current);
				object[] keyValues = childRow.GetKeyValues(this._childKey);
				if (childRow.HasVersion(dataRowVersion))
				{
					DataRow parentRow = DataRelation.GetParentRow(this.ParentKey, this.ChildKey, childRow, dataRowVersion);
					if (parentRow != null && parentRow._inCascade)
					{
						object[] keyValues2 = parentRow.GetKeyValues(this._parentKey, (action == DataRowAction.Rollback) ? dataRowVersion : DataRowVersion.Default);
						int num = childRow.Table.NewRecord();
						childRow.Table.SetKeyValues(this._childKey, keyValues2, num);
						if (this._childKey.RecordsEqual(childRow._tempRecord, num))
						{
							return;
						}
					}
				}
				object[] keyValues3 = childRow.GetKeyValues(this._childKey);
				if (!this.IsKeyNull(keyValues3) && !this._parentKey.GetSortIndex().IsKeyInIndex(keyValues3))
				{
					if (this._childKey.Table == this._parentKey.Table && childRow._tempRecord != -1)
					{
						int i;
						for (i = 0; i < keyValues3.Length; i++)
						{
							DataColumn dataColumn = this._parentKey.ColumnsReference[i];
							object obj = dataColumn.ConvertValue(keyValues3[i]);
							if (dataColumn.CompareValueTo(childRow._tempRecord, obj) != 0)
							{
								break;
							}
						}
						if (i == keyValues3.Length)
						{
							return;
						}
					}
					throw ExceptionBuilder.ForeignKeyViolation(this.ConstraintName, keyValues);
				}
			}
		}

		private void NonVirtualCheckState()
		{
			if (this._DataSet == null)
			{
				this._parentKey.CheckState();
				this._childKey.CheckState();
				if (this._parentKey.Table.DataSet != this._childKey.Table.DataSet)
				{
					throw ExceptionBuilder.TablesInDifferentSets();
				}
				for (int i = 0; i < this._parentKey.ColumnsReference.Length; i++)
				{
					if (this._parentKey.ColumnsReference[i].DataType != this._childKey.ColumnsReference[i].DataType || (this._parentKey.ColumnsReference[i].DataType == typeof(DateTime) && this._parentKey.ColumnsReference[i].DateTimeMode != this._childKey.ColumnsReference[i].DateTimeMode && (this._parentKey.ColumnsReference[i].DateTimeMode & this._childKey.ColumnsReference[i].DateTimeMode) != DataSetDateTime.Unspecified))
					{
						throw ExceptionBuilder.ColumnsTypeMismatch();
					}
				}
				if (this._childKey.ColumnsEqual(this._parentKey))
				{
					throw ExceptionBuilder.KeyColumnsIdentical();
				}
			}
		}

		internal override void CheckState()
		{
			this.NonVirtualCheckState();
		}

		[DefaultValue(AcceptRejectRule.None)]
		public virtual AcceptRejectRule AcceptRejectRule
		{
			get
			{
				base.CheckStateForProperty();
				return this._acceptRejectRule;
			}
			set
			{
				if (value <= AcceptRejectRule.Cascade)
				{
					this._acceptRejectRule = value;
					return;
				}
				throw ADP.InvalidAcceptRejectRule(value);
			}
		}

		internal override bool ContainsColumn(DataColumn column)
		{
			return this._parentKey.ContainsColumn(column) || this._childKey.ContainsColumn(column);
		}

		internal override Constraint Clone(DataSet destination)
		{
			return this.Clone(destination, false);
		}

		internal override Constraint Clone(DataSet destination, bool ignorNSforTableLookup)
		{
			int num;
			if (ignorNSforTableLookup)
			{
				num = destination.Tables.IndexOf(this.Table.TableName);
			}
			else
			{
				num = destination.Tables.IndexOf(this.Table.TableName, this.Table.Namespace, false);
			}
			if (num < 0)
			{
				return null;
			}
			DataTable dataTable = destination.Tables[num];
			if (ignorNSforTableLookup)
			{
				num = destination.Tables.IndexOf(this.RelatedTable.TableName);
			}
			else
			{
				num = destination.Tables.IndexOf(this.RelatedTable.TableName, this.RelatedTable.Namespace, false);
			}
			if (num < 0)
			{
				return null;
			}
			DataTable dataTable2 = destination.Tables[num];
			int num2 = this.Columns.Length;
			DataColumn[] array = new DataColumn[num2];
			DataColumn[] array2 = new DataColumn[num2];
			for (int i = 0; i < num2; i++)
			{
				DataColumn dataColumn = this.Columns[i];
				num = dataTable.Columns.IndexOf(dataColumn.ColumnName);
				if (num < 0)
				{
					return null;
				}
				array[i] = dataTable.Columns[num];
				dataColumn = this.RelatedColumnsReference[i];
				num = dataTable2.Columns.IndexOf(dataColumn.ColumnName);
				if (num < 0)
				{
					return null;
				}
				array2[i] = dataTable2.Columns[num];
			}
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(this.ConstraintName, array2, array);
			foreignKeyConstraint.UpdateRule = this.UpdateRule;
			foreignKeyConstraint.DeleteRule = this.DeleteRule;
			foreignKeyConstraint.AcceptRejectRule = this.AcceptRejectRule;
			foreach (object obj in base.ExtendedProperties.Keys)
			{
				foreignKeyConstraint.ExtendedProperties[obj] = base.ExtendedProperties[obj];
			}
			return foreignKeyConstraint;
		}

		internal ForeignKeyConstraint Clone(DataTable destination)
		{
			int num = this.Columns.Length;
			DataColumn[] array = new DataColumn[num];
			DataColumn[] array2 = new DataColumn[num];
			for (int i = 0; i < num; i++)
			{
				DataColumn dataColumn = this.Columns[i];
				int num2 = destination.Columns.IndexOf(dataColumn.ColumnName);
				if (num2 < 0)
				{
					return null;
				}
				array[i] = destination.Columns[num2];
				dataColumn = this.RelatedColumnsReference[i];
				num2 = destination.Columns.IndexOf(dataColumn.ColumnName);
				if (num2 < 0)
				{
					return null;
				}
				array2[i] = destination.Columns[num2];
			}
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(this.ConstraintName, array2, array);
			foreignKeyConstraint.UpdateRule = this.UpdateRule;
			foreignKeyConstraint.DeleteRule = this.DeleteRule;
			foreignKeyConstraint.AcceptRejectRule = this.AcceptRejectRule;
			foreach (object obj in base.ExtendedProperties.Keys)
			{
				foreignKeyConstraint.ExtendedProperties[obj] = base.ExtendedProperties[obj];
			}
			return foreignKeyConstraint;
		}

		private void Create(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			if (parentColumns.Length == 0 || childColumns.Length == 0)
			{
				throw ExceptionBuilder.KeyLengthZero();
			}
			if (parentColumns.Length != childColumns.Length)
			{
				throw ExceptionBuilder.KeyLengthMismatch();
			}
			for (int i = 0; i < parentColumns.Length; i++)
			{
				if (parentColumns[i].Computed)
				{
					throw ExceptionBuilder.ExpressionInConstraint(parentColumns[i]);
				}
				if (childColumns[i].Computed)
				{
					throw ExceptionBuilder.ExpressionInConstraint(childColumns[i]);
				}
			}
			this._parentKey = new DataKey(parentColumns, true);
			this._childKey = new DataKey(childColumns, true);
			this.ConstraintName = relationName;
			this.NonVirtualCheckState();
		}

		[DefaultValue(Rule.Cascade)]
		public virtual Rule DeleteRule
		{
			get
			{
				base.CheckStateForProperty();
				return this._deleteRule;
			}
			set
			{
				if (value <= Rule.SetDefault)
				{
					this._deleteRule = value;
					return;
				}
				throw ADP.InvalidRule(value);
			}
		}

		public override bool Equals(object key)
		{
			if (!(key is ForeignKeyConstraint))
			{
				return false;
			}
			ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)key;
			return this.ParentKey.ColumnsEqual(foreignKeyConstraint.ParentKey) && this.ChildKey.ColumnsEqual(foreignKeyConstraint.ChildKey);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[ReadOnly(true)]
		public virtual DataColumn[] RelatedColumns
		{
			get
			{
				base.CheckStateForProperty();
				return this._parentKey.ToArray();
			}
		}

		internal DataColumn[] RelatedColumnsReference
		{
			get
			{
				base.CheckStateForProperty();
				return this._parentKey.ColumnsReference;
			}
		}

		internal DataKey ParentKey
		{
			get
			{
				base.CheckStateForProperty();
				return this._parentKey;
			}
		}

		internal DataRelation FindParentRelation()
		{
			DataRelationCollection parentRelations = this.Table.ParentRelations;
			for (int i = 0; i < parentRelations.Count; i++)
			{
				if (parentRelations[i].ChildKeyConstraint == this)
				{
					return parentRelations[i];
				}
			}
			return null;
		}

		[ReadOnly(true)]
		public virtual DataTable RelatedTable
		{
			get
			{
				base.CheckStateForProperty();
				return this._parentKey.Table;
			}
		}

		[DefaultValue(Rule.Cascade)]
		public virtual Rule UpdateRule
		{
			get
			{
				base.CheckStateForProperty();
				return this._updateRule;
			}
			set
			{
				if (value <= Rule.SetDefault)
				{
					this._updateRule = value;
					return;
				}
				throw ADP.InvalidRule(value);
			}
		}

		internal const Rule Rule_Default = Rule.Cascade;

		internal const AcceptRejectRule AcceptRejectRule_Default = AcceptRejectRule.None;

		internal Rule _deleteRule;

		internal Rule _updateRule;

		internal AcceptRejectRule _acceptRejectRule;

		private DataKey _childKey;

		private DataKey _parentKey;

		internal string _constraintName;

		internal string[] _parentColumnNames;

		internal string[] _childColumnNames;

		internal string _parentTableName;

		internal string _parentTableNamespace;
	}
}
