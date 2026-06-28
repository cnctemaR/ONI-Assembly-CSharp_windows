using System;
using System.ComponentModel;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class ColumnReference : BaseExpression
	{
		public ColumnReference(string columnName)
			: this(ReferencedTable.Self, null, columnName)
		{
		}

		public ColumnReference(ReferencedTable refTable, string relationName, string columnName)
		{
			this.refTable = refTable;
			this.relationName = relationName;
			this.columnName = columnName;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is ColumnReference))
			{
				return false;
			}
			ColumnReference columnReference = (ColumnReference)obj;
			return columnReference.refTable == this.refTable && !(columnReference.columnName != this.columnName) && !(columnReference.relationName != this.relationName);
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num ^= this.refTable.GetHashCode();
			num ^= this.columnName.GetHashCode();
			return num ^ this.relationName.GetHashCode();
		}

		public ReferencedTable ReferencedTable
		{
			get
			{
				return this.refTable;
			}
		}

		private DataRelation GetRelation(DataRow row)
		{
			if (this._cachedRelation == null)
			{
				DataTable table = row.Table;
				if (this.relationName != null)
				{
					DataRelationCollection dataRelationCollection = table.DataSet.Relations;
					this._cachedRelation = dataRelationCollection[dataRelationCollection.IndexOf(this.relationName)];
				}
				else
				{
					DataRelationCollection dataRelationCollection;
					if (this.refTable == ReferencedTable.Parent)
					{
						dataRelationCollection = table.ParentRelations;
					}
					else
					{
						dataRelationCollection = table.ChildRelations;
					}
					if (dataRelationCollection.Count > 1)
					{
						throw new EvaluateException(string.Format("The table [{0}] is involved in more than one relation.You must explicitly mention a relation name.", table.TableName));
					}
					this._cachedRelation = dataRelationCollection[0];
				}
				this._cachedRelation.DataSet.Relations.CollectionChanged += this.OnRelationRemoved;
			}
			return this._cachedRelation;
		}

		private DataColumn GetColumn(DataRow row)
		{
			if (this._cachedColumn == null)
			{
				DataTable dataTable = row.Table;
				ReferencedTable referencedTable = this.refTable;
				if (referencedTable != ReferencedTable.Parent)
				{
					if (referencedTable == ReferencedTable.Child)
					{
						dataTable = this.GetRelation(row).ChildTable;
					}
				}
				else
				{
					dataTable = this.GetRelation(row).ParentTable;
				}
				this._cachedColumn = dataTable.Columns[this.columnName];
				if (this._cachedColumn == null)
				{
					throw new EvaluateException(string.Format("Cannot find column [{0}].", this.columnName));
				}
				this._cachedColumn.PropertyChanged += this.OnColumnPropertyChanged;
				this._cachedColumn.Table.Columns.CollectionChanged += this.OnColumnRemoved;
			}
			return this._cachedColumn;
		}

		public DataRow GetReferencedRow(DataRow row)
		{
			this.GetColumn(row);
			switch (this.refTable)
			{
			default:
				return row;
			case ReferencedTable.Parent:
				return row.GetParentRow(this.GetRelation(row));
			case ReferencedTable.Child:
				return row.GetChildRows(this.GetRelation(row))[0];
			}
		}

		public DataRow[] GetReferencedRows(DataRow row)
		{
			this.GetColumn(row);
			switch (this.refTable)
			{
			default:
			{
				DataRow[] array = row.Table.NewRowArray(row.Table.Rows.Count);
				row.Table.Rows.CopyTo(array, 0);
				return array;
			}
			case ReferencedTable.Parent:
				return row.GetParentRows(this.GetRelation(row));
			case ReferencedTable.Child:
				return row.GetChildRows(this.GetRelation(row));
			}
		}

		public object[] GetValues(DataRow[] rows)
		{
			object[] array = new object[rows.Length];
			for (int i = 0; i < rows.Length; i++)
			{
				array[i] = this.Unify(rows[i][this.GetColumn(rows[i])]);
			}
			return array;
		}

		private object Unify(object val)
		{
			if (Numeric.IsNumeric(val))
			{
				return Numeric.Unify((IConvertible)val);
			}
			if (val == null || val == DBNull.Value)
			{
				return null;
			}
			if (val is bool || val is string || val is DateTime || val is Guid || val is char)
			{
				return val;
			}
			if (val is Enum)
			{
				return (int)val;
			}
			throw new EvaluateException(string.Format("Cannot handle data type found in column '{0}'.", this.columnName));
		}

		public override object Eval(DataRow row)
		{
			DataRow referencedRow = this.GetReferencedRow(row);
			if (referencedRow == null)
			{
				return null;
			}
			object obj;
			try
			{
				referencedRow._inExpressionEvaluation = true;
				obj = referencedRow[this.GetColumn(row)];
				referencedRow._inExpressionEvaluation = false;
			}
			catch (IndexOutOfRangeException)
			{
				throw new EvaluateException(string.Format("Cannot find column [{0}].", this.columnName));
			}
			return this.Unify(obj);
		}

		public override bool EvalBoolean(DataRow row)
		{
			DataColumn column = this.GetColumn(row);
			if (column.DataType != typeof(bool))
			{
				throw new EvaluateException("Not a Boolean Expression");
			}
			object obj = this.Eval(row);
			return obj != null && obj != DBNull.Value && (bool)obj;
		}

		public override bool DependsOn(DataColumn other)
		{
			return this.refTable == ReferencedTable.Self && this.columnName == other.ColumnName;
		}

		private void DropCached(DataColumnCollection columnCollection, DataRelationCollection relationCollection)
		{
			if (this._cachedColumn != null)
			{
				this._cachedColumn.PropertyChanged -= this.OnColumnPropertyChanged;
				if (columnCollection != null)
				{
					columnCollection.CollectionChanged -= this.OnColumnRemoved;
				}
				else if (this._cachedColumn.Table != null)
				{
					this._cachedColumn.Table.Columns.CollectionChanged -= this.OnColumnRemoved;
				}
				this._cachedColumn = null;
			}
			if (this._cachedRelation != null)
			{
				if (relationCollection != null)
				{
					relationCollection.CollectionChanged -= this.OnRelationRemoved;
				}
				else if (this._cachedRelation.DataSet != null)
				{
					this._cachedRelation.DataSet.Relations.CollectionChanged -= this.OnRelationRemoved;
				}
				this._cachedRelation = null;
			}
		}

		private void OnColumnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (!(sender is DataColumn))
			{
				return;
			}
			DataColumn dataColumn = (DataColumn)sender;
			if (dataColumn == this._cachedColumn && args.PropertyName == "ColumnName")
			{
				this.DropCached(null, null);
			}
		}

		private void OnColumnRemoved(object sender, CollectionChangeEventArgs args)
		{
			if (!(args.Element is DataColumnCollection))
			{
				return;
			}
			if (args.Action != CollectionChangeAction.Remove)
			{
				return;
			}
			DataColumnCollection dataColumnCollection = (DataColumnCollection)args.Element;
			if (this._cachedColumn != null && dataColumnCollection != null && dataColumnCollection.IndexOf(this._cachedColumn) == -1)
			{
				this.DropCached(dataColumnCollection, null);
			}
		}

		private void OnRelationRemoved(object sender, CollectionChangeEventArgs args)
		{
			if (!(args.Element is DataRelationCollection))
			{
				return;
			}
			if (args.Action != CollectionChangeAction.Remove)
			{
				return;
			}
			DataRelationCollection dataRelationCollection = (DataRelationCollection)args.Element;
			if (this._cachedRelation != null && dataRelationCollection != null && dataRelationCollection.IndexOf(this._cachedRelation) == -1)
			{
				this.DropCached(null, dataRelationCollection);
			}
		}

		private ReferencedTable refTable;

		private string relationName;

		private string columnName;

		private DataColumn _cachedColumn;

		private DataRelation _cachedRelation;
	}
}
