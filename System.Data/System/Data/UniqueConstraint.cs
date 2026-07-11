using System;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Data
{
	[DefaultProperty("ConstraintName")]
	public class UniqueConstraint : Constraint
	{
		public UniqueConstraint(string name, DataColumn column)
		{
			this.Create(name, new DataColumn[] { column });
		}

		public UniqueConstraint(DataColumn column)
		{
			this.Create(null, new DataColumn[] { column });
		}

		public UniqueConstraint(string name, DataColumn[] columns)
		{
			this.Create(name, columns);
		}

		public UniqueConstraint(DataColumn[] columns)
		{
			this.Create(null, columns);
		}

		[Browsable(false)]
		public UniqueConstraint(string name, string[] columnNames, bool isPrimaryKey)
		{
			this._constraintName = name;
			this._columnNames = columnNames;
			this._bPrimaryKey = isPrimaryKey;
		}

		public UniqueConstraint(string name, DataColumn column, bool isPrimaryKey)
		{
			DataColumn[] array = new DataColumn[] { column };
			this._bPrimaryKey = isPrimaryKey;
			this.Create(name, array);
		}

		public UniqueConstraint(DataColumn column, bool isPrimaryKey)
		{
			DataColumn[] array = new DataColumn[] { column };
			this._bPrimaryKey = isPrimaryKey;
			this.Create(null, array);
		}

		public UniqueConstraint(string name, DataColumn[] columns, bool isPrimaryKey)
		{
			this._bPrimaryKey = isPrimaryKey;
			this.Create(name, columns);
		}

		public UniqueConstraint(DataColumn[] columns, bool isPrimaryKey)
		{
			this._bPrimaryKey = isPrimaryKey;
			this.Create(null, columns);
		}

		internal string[] ColumnNames
		{
			get
			{
				return this._key.GetColumnNames();
			}
		}

		internal Index ConstraintIndex
		{
			get
			{
				return this._constraintIndex;
			}
		}

		[Conditional("DEBUG")]
		private void AssertConstraintAndKeyIndexes()
		{
			DataColumn[] array = new DataColumn[this._constraintIndex._indexFields.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this._constraintIndex._indexFields[i].Column;
			}
		}

		internal void ConstraintIndexClear()
		{
			if (this._constraintIndex != null)
			{
				this._constraintIndex.RemoveRef();
				this._constraintIndex = null;
			}
		}

		internal void ConstraintIndexInitialize()
		{
			if (this._constraintIndex == null)
			{
				this._constraintIndex = this._key.GetSortIndex();
				this._constraintIndex.AddRef();
			}
		}

		internal override void CheckState()
		{
			this.NonVirtualCheckState();
		}

		private void NonVirtualCheckState()
		{
			this._key.CheckState();
		}

		internal override void CheckCanAddToCollection(ConstraintCollection constraints)
		{
		}

		internal override bool CanBeRemovedFromCollection(ConstraintCollection constraints, bool fThrowException)
		{
			if (!this.Equals(constraints.Table._primaryKey))
			{
				ParentForeignKeyConstraintEnumerator parentForeignKeyConstraintEnumerator = new ParentForeignKeyConstraintEnumerator(this.Table.DataSet, this.Table);
				while (parentForeignKeyConstraintEnumerator.GetNext())
				{
					ForeignKeyConstraint foreignKeyConstraint = parentForeignKeyConstraintEnumerator.GetForeignKeyConstraint();
					if (this._key.ColumnsEqual(foreignKeyConstraint.ParentKey))
					{
						if (!fThrowException)
						{
							return false;
						}
						throw ExceptionBuilder.NeededForForeignKeyConstraint(this, foreignKeyConstraint);
					}
				}
				return true;
			}
			if (!fThrowException)
			{
				return false;
			}
			throw ExceptionBuilder.RemovePrimaryKey(constraints.Table);
		}

		internal override bool CanEnableConstraint()
		{
			return !this.Table.EnforceConstraints || this.ConstraintIndex.CheckUnique();
		}

		internal override bool IsConstraintViolated()
		{
			bool flag = false;
			Index constraintIndex = this.ConstraintIndex;
			if (constraintIndex.HasDuplicates)
			{
				object[] uniqueKeyValues = constraintIndex.GetUniqueKeyValues();
				for (int i = 0; i < uniqueKeyValues.Length; i++)
				{
					Range range = constraintIndex.FindRecords((object[])uniqueKeyValues[i]);
					if (1 < range.Count)
					{
						DataRow[] rows = constraintIndex.GetRows(range);
						string text = ExceptionBuilder.UniqueConstraintViolationText(this._key.ColumnsReference, (object[])uniqueKeyValues[i]);
						for (int j = 0; j < rows.Length; j++)
						{
							rows[j].RowError = text;
							foreach (DataColumn dataColumn in this._key.ColumnsReference)
							{
								rows[j].SetColumnError(dataColumn, text);
							}
						}
						flag = true;
					}
				}
			}
			return flag;
		}

		internal override void CheckConstraint(DataRow row, DataRowAction action)
		{
			if (this.Table.EnforceConstraints && (action == DataRowAction.Add || action == DataRowAction.Change || (action == DataRowAction.Rollback && row._tempRecord != -1)) && row.HaveValuesChanged(this.ColumnsReference) && this.ConstraintIndex.IsKeyRecordInIndex(row.GetDefaultRecord()))
			{
				object[] columnValues = row.GetColumnValues(this.ColumnsReference);
				throw ExceptionBuilder.ConstraintViolation(this.ColumnsReference, columnValues);
			}
		}

		internal override bool ContainsColumn(DataColumn column)
		{
			return this._key.ContainsColumn(column);
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
			int num2 = this.ColumnsReference.Length;
			DataColumn[] array = new DataColumn[num2];
			for (int i = 0; i < num2; i++)
			{
				DataColumn dataColumn = this.ColumnsReference[i];
				num = dataTable.Columns.IndexOf(dataColumn.ColumnName);
				if (num < 0)
				{
					return null;
				}
				array[i] = dataTable.Columns[num];
			}
			UniqueConstraint uniqueConstraint = new UniqueConstraint(this.ConstraintName, array);
			foreach (object obj in base.ExtendedProperties.Keys)
			{
				uniqueConstraint.ExtendedProperties[obj] = base.ExtendedProperties[obj];
			}
			return uniqueConstraint;
		}

		internal UniqueConstraint Clone(DataTable table)
		{
			int num = this.ColumnsReference.Length;
			DataColumn[] array = new DataColumn[num];
			for (int i = 0; i < num; i++)
			{
				DataColumn dataColumn = this.ColumnsReference[i];
				int num2 = table.Columns.IndexOf(dataColumn.ColumnName);
				if (num2 < 0)
				{
					return null;
				}
				array[i] = table.Columns[num2];
			}
			UniqueConstraint uniqueConstraint = new UniqueConstraint(this.ConstraintName, array);
			foreach (object obj in base.ExtendedProperties.Keys)
			{
				uniqueConstraint.ExtendedProperties[obj] = base.ExtendedProperties[obj];
			}
			return uniqueConstraint;
		}

		[ReadOnly(true)]
		public virtual DataColumn[] Columns
		{
			get
			{
				return this._key.ToArray();
			}
		}

		internal DataColumn[] ColumnsReference
		{
			get
			{
				return this._key.ColumnsReference;
			}
		}

		public bool IsPrimaryKey
		{
			get
			{
				return this.Table != null && this == this.Table._primaryKey;
			}
		}

		private void Create(string constraintName, DataColumn[] columns)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i].Computed)
				{
					throw ExceptionBuilder.ExpressionInConstraint(columns[i]);
				}
			}
			this._key = new DataKey(columns, true);
			this.ConstraintName = constraintName;
			this.NonVirtualCheckState();
		}

		public override bool Equals(object key2)
		{
			return key2 is UniqueConstraint && this.Key.ColumnsEqual(((UniqueConstraint)key2).Key);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal override bool InCollection
		{
			set
			{
				base.InCollection = value;
				if (this._key.ColumnsReference.Length == 1)
				{
					this._key.ColumnsReference[0].InternalUnique(value);
				}
			}
		}

		internal DataKey Key
		{
			get
			{
				return this._key;
			}
		}

		[ReadOnly(true)]
		public override DataTable Table
		{
			get
			{
				if (this._key.HasValue)
				{
					return this._key.Table;
				}
				return null;
			}
		}

		private DataKey _key;

		private Index _constraintIndex;

		internal bool _bPrimaryKey;

		internal string _constraintName;

		internal string[] _columnNames;
	}
}
