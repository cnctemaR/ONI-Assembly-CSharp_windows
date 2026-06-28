using System;
using System.ComponentModel;
using System.Text;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.ForeignKeyConstraintEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultProperty("ConstraintName")]
	public class ForeignKeyConstraint : Constraint
	{
		public ForeignKeyConstraint(DataColumn parentColumn, DataColumn childColumn)
		{
			if (parentColumn == null || childColumn == null)
			{
				throw new NullReferenceException("Neither parentColumn or childColumn can be null.");
			}
			this._foreignKeyConstraint(null, new DataColumn[] { parentColumn }, new DataColumn[] { childColumn });
		}

		public ForeignKeyConstraint(DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			this._foreignKeyConstraint(null, parentColumns, childColumns);
		}

		public ForeignKeyConstraint(string constraintName, DataColumn parentColumn, DataColumn childColumn)
		{
			if (parentColumn == null || childColumn == null)
			{
				throw new NullReferenceException("Neither parentColumn or childColumn can be null.");
			}
			this._foreignKeyConstraint(constraintName, new DataColumn[] { parentColumn }, new DataColumn[] { childColumn });
		}

		public ForeignKeyConstraint(string constraintName, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			this._foreignKeyConstraint(constraintName, parentColumns, childColumns);
		}

		[Browsable(false)]
		public ForeignKeyConstraint(string constraintName, string parentTableName, string[] parentColumnNames, string[] childColumnNames, AcceptRejectRule acceptRejectRule, Rule deleteRule, Rule updateRule)
		{
			this.InitInProgress = true;
			base.ConstraintName = constraintName;
			this._parentTableName = parentTableName;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._acceptRejectRule = acceptRejectRule;
			this._deleteRule = deleteRule;
			this._updateRule = updateRule;
		}

		[Browsable(false)]
		public ForeignKeyConstraint(string constraintName, string parentTableName, string parentTableNamespace, string[] parentColumnNames, string[] childColumnNames, AcceptRejectRule acceptRejectRule, Rule deleteRule, Rule updateRule)
		{
			this.InitInProgress = true;
			base.ConstraintName = constraintName;
			this._parentTableName = parentTableName;
			this._parentTableNamespace = parentTableNamespace;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._acceptRejectRule = acceptRejectRule;
			this._deleteRule = deleteRule;
			this._updateRule = updateRule;
		}

		internal override void FinishInit(DataTable childTable)
		{
			if (childTable.DataSet == null)
			{
				throw new InvalidConstraintException("ChildTable : " + childTable.TableName + " does not belong to any DataSet");
			}
			DataSet dataSet = childTable.DataSet;
			this._childTableName = childTable.TableName;
			if (!dataSet.Tables.Contains(this._parentTableName))
			{
				throw new InvalidConstraintException(string.Concat(new object[] { "Table : ", this._parentTableName, "does not exist in DataSet : ", dataSet }));
			}
			DataTable dataTable = dataSet.Tables[this._parentTableName];
			int num = 0;
			int num2 = 0;
			if (this._parentColumnNames.Length < 0 || this._childColumnNames.Length < 0)
			{
				throw new InvalidConstraintException("Neither parent nor child columns can be zero length");
			}
			if (this._parentColumnNames.Length != this._childColumnNames.Length)
			{
				throw new InvalidConstraintException("Both parent and child columns must be of same length");
			}
			DataColumn[] array = new DataColumn[this._parentColumnNames.Length];
			DataColumn[] array2 = new DataColumn[this._childColumnNames.Length];
			foreach (string text in this._parentColumnNames)
			{
				if (!dataTable.Columns.Contains(text))
				{
					throw new InvalidConstraintException("Table : " + this._parentTableName + "does not contain the column :" + text);
				}
				array[num++] = dataTable.Columns[text];
			}
			foreach (string text2 in this._childColumnNames)
			{
				if (!childTable.Columns.Contains(text2))
				{
					throw new InvalidConstraintException("Table : " + this._childTableName + "does not contain the column : " + text2);
				}
				array2[num2++] = childTable.Columns[text2];
			}
			this._validateColumns(array, array2);
			this._parentColumns = array;
			this._childColumns = array2;
			dataTable.Namespace = this._parentTableNamespace;
			this.InitInProgress = false;
		}

		private void _foreignKeyConstraint(string constraintName, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			this._validateColumns(parentColumns, childColumns);
			base.ConstraintName = constraintName;
			this._parentColumns = parentColumns;
			this._childColumns = childColumns;
		}

		private void _validateColumns(DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			if (parentColumns == null || childColumns == null)
			{
				throw new ArgumentNullException();
			}
			if (parentColumns.Length < 1 || childColumns.Length < 1)
			{
				throw new ArgumentException("Neither ParentColumns or ChildColumns can't be zero length.");
			}
			if (parentColumns.Length != childColumns.Length)
			{
				throw new ArgumentException("Parent columns and child columns must be the same length.");
			}
			DataTable table = parentColumns[0].Table;
			DataTable table2 = childColumns[0].Table;
			for (int i = 0; i < parentColumns.Length; i++)
			{
				DataColumn dataColumn = parentColumns[i];
				DataColumn dataColumn2 = childColumns[i];
				if (dataColumn.Table == null)
				{
					throw new ArgumentException("All columns must belong to a table. ColumnName: " + dataColumn.ColumnName + " does not belong to a table.");
				}
				if (table != dataColumn.Table)
				{
					throw new InvalidConstraintException("Parent columns must all belong to the same table.");
				}
				if (dataColumn2.Table == null)
				{
					throw new ArgumentException("All columns must belong to a table. ColumnName: " + dataColumn.ColumnName + " does not belong to a table.");
				}
				if (table2 != dataColumn2.Table)
				{
					throw new InvalidConstraintException("Child columns must all belong to the same table.");
				}
				if (dataColumn.CompiledExpression != null)
				{
					throw new ArgumentException(string.Format("Cannot create a constraint based on Expression column {0}.", dataColumn.ColumnName));
				}
				if (dataColumn2.CompiledExpression != null)
				{
					throw new ArgumentException(string.Format("Cannot create a constraint based on Expression column {0}.", dataColumn2.ColumnName));
				}
			}
			if (table.DataSet != table2.DataSet)
			{
				throw new InvalidOperationException("Parent column and child column must belong to tables that belong to the same DataSet.");
			}
			int num = 0;
			for (int j = 0; j < parentColumns.Length; j++)
			{
				DataColumn dataColumn3 = parentColumns[j];
				DataColumn dataColumn4 = childColumns[j];
				if (dataColumn3 == dataColumn4)
				{
					num++;
				}
				else if (!dataColumn3.DataTypeMatches(dataColumn4))
				{
					throw new InvalidOperationException("Parent column is not type compatible with it's child column.");
				}
			}
			if (num == parentColumns.Length)
			{
				throw new InvalidOperationException("Property not accessible because 'ParentKey and ChildKey are identical.'.");
			}
		}

		private void _ensureUniqueConstraintExists(ConstraintCollection collection, DataColumn[] parentColumns)
		{
			if (parentColumns == null)
			{
				throw new ArgumentNullException("ParentColumns can't be null");
			}
			UniqueConstraint uniqueConstraint = null;
			if (parentColumns[0] != null)
			{
				uniqueConstraint = UniqueConstraint.GetUniqueConstraintForColumnSet(parentColumns[0].Table.Constraints, parentColumns);
			}
			if (uniqueConstraint == null)
			{
				uniqueConstraint = new UniqueConstraint(parentColumns, false);
				parentColumns[0].Table.Constraints.Add(uniqueConstraint);
			}
			this._parentUniqueConstraint = uniqueConstraint;
			this._parentUniqueConstraint.ChildConstraint = this;
		}

		[DataCategory("Data")]
		[DefaultValue(AcceptRejectRule.None)]
		public virtual AcceptRejectRule AcceptRejectRule
		{
			get
			{
				return this._acceptRejectRule;
			}
			set
			{
				this._acceptRejectRule = value;
			}
		}

		[ReadOnly(true)]
		[DataCategory("Data")]
		public virtual DataColumn[] Columns
		{
			get
			{
				return this._childColumns;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(Rule.Cascade)]
		public virtual Rule DeleteRule
		{
			get
			{
				return this._deleteRule;
			}
			set
			{
				this._deleteRule = value;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(Rule.Cascade)]
		public virtual Rule UpdateRule
		{
			get
			{
				return this._updateRule;
			}
			set
			{
				this._updateRule = value;
			}
		}

		[DataCategory("Data")]
		[ReadOnly(true)]
		public virtual DataColumn[] RelatedColumns
		{
			get
			{
				return this._parentColumns;
			}
		}

		[DataCategory("Data")]
		[ReadOnly(true)]
		public virtual DataTable RelatedTable
		{
			get
			{
				if (this._parentColumns != null && this._parentColumns.Length > 0)
				{
					return this._parentColumns[0].Table;
				}
				throw new InvalidOperationException("Property not accessible because 'Object reference not set to an instance of an object'");
			}
		}

		[DataCategory("Data")]
		[ReadOnly(true)]
		public override DataTable Table
		{
			get
			{
				if (this._childColumns != null && this._childColumns.Length > 0)
				{
					return this._childColumns[0].Table;
				}
				throw new InvalidOperationException("Property not accessible because 'Object reference not set to an instance of an object'");
			}
		}

		internal UniqueConstraint ParentConstraint
		{
			get
			{
				return this._parentUniqueConstraint;
			}
		}

		public override bool Equals(object key)
		{
			ForeignKeyConstraint foreignKeyConstraint = key as ForeignKeyConstraint;
			return foreignKeyConstraint != null && DataColumn.AreColumnSetsTheSame(this.RelatedColumns, foreignKeyConstraint.RelatedColumns) && DataColumn.AreColumnSetsTheSame(this.Columns, foreignKeyConstraint.Columns);
		}

		public override int GetHashCode()
		{
			int num = 32;
			int num2 = 88;
			if (this.Columns.Length > 0)
			{
				num ^= this.Columns[0].GetHashCode();
			}
			for (int i = 1; i < this.Columns.Length; i++)
			{
				num ^= this.Columns[1].GetHashCode();
			}
			if (this.RelatedColumns.Length > 0)
			{
				num2 ^= this.Columns[0].GetHashCode();
			}
			for (int i = 1; i < this.RelatedColumns.Length; i++)
			{
				num2 ^= this.RelatedColumns[1].GetHashCode();
			}
			return num ^ num2;
		}

		internal override void AddToConstraintCollectionSetup(ConstraintCollection collection)
		{
			if (collection.Table != this.Table)
			{
				throw new InvalidConstraintException("This constraint cannot be added since ForeignKey doesn't belong to table " + this.RelatedTable.TableName + ".");
			}
			this._validateColumns(this._parentColumns, this._childColumns);
			this._ensureUniqueConstraintExists(collection, this._parentColumns);
			if (((this.Table.DataSet != null && this.Table.DataSet.EnforceConstraints) || (this.Table.DataSet == null && this.Table.EnforceConstraints)) && this.IsConstraintViolated())
			{
				throw new ArgumentException("This constraint cannot be enabled as not all values have corresponding parent values.");
			}
		}

		internal override void RemoveFromConstraintCollectionCleanup(ConstraintCollection collection)
		{
			this._parentUniqueConstraint.ChildConstraint = null;
			base.Index = null;
		}

		internal override bool IsConstraintViolated()
		{
			if (this.Table.DataSet == null || this.RelatedTable.DataSet == null)
			{
				return false;
			}
			bool flag = false;
			foreach (object obj in this.Table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (!dataRow.IsNullColumns(this._childColumns))
				{
					if (!this.RelatedTable.RowsExist(this._parentColumns, this._childColumns, dataRow))
					{
						flag = true;
						string[] array = new string[this._childColumns.Length];
						for (int i = 0; i < this._childColumns.Length; i++)
						{
							DataColumn dataColumn = this._childColumns[i];
							array[i] = dataRow[dataColumn].ToString();
						}
						dataRow.RowError = string.Format("ForeignKeyConstraint {0} requires the child key values ({1}) to exist in the parent table.", this.ConstraintName, string.Join(",", array));
					}
				}
			}
			return flag;
		}

		internal override void AssertConstraint(DataRow row)
		{
			if (row.IsNullColumns(this._childColumns))
			{
				return;
			}
			if (!this.RelatedTable.RowsExist(this._parentColumns, this._childColumns, row))
			{
				throw new InvalidConstraintException(this.GetErrorMessage(row));
			}
		}

		internal override bool IsColumnContained(DataColumn column)
		{
			for (int i = 0; i < this._parentColumns.Length; i++)
			{
				if (column == this._parentColumns[i])
				{
					return true;
				}
			}
			for (int j = 0; j < this._childColumns.Length; j++)
			{
				if (column == this._childColumns[j])
				{
					return true;
				}
			}
			return false;
		}

		internal override bool CanRemoveFromCollection(ConstraintCollection col, bool shouldThrow)
		{
			return true;
		}

		private string GetErrorMessage(DataRow row)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this._childColumns.Length; i++)
			{
				stringBuilder.Append(row[this._childColumns[0]].ToString());
				if (i != this._childColumns.Length - 1)
				{
					stringBuilder.Append(',');
				}
			}
			string text = stringBuilder.ToString();
			return string.Concat(new string[] { "ForeignKeyConstraint ", this.ConstraintName, " requires the child key values (", text, ") to exist in the parent table." });
		}

		private UniqueConstraint _parentUniqueConstraint;

		private DataColumn[] _parentColumns;

		private DataColumn[] _childColumns;

		private Rule _deleteRule = Rule.Cascade;

		private Rule _updateRule = Rule.Cascade;

		private AcceptRejectRule _acceptRejectRule;

		private string _parentTableName;

		private string _parentTableNamespace;

		private string _childTableName;

		private string[] _parentColumnNames;

		private string[] _childColumnNames;
	}
}
