using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[DefaultEvent("CollectionChanged")]
	[Editor("Microsoft.VSDesigner.Data.Design.ColumnsCollectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class DataColumnCollection : InternalDataCollectionBase
	{
		internal DataColumnCollection(DataTable table)
		{
			this.parentTable = table;
		}

		[ResDescription("Occurs whenever this collection's membership changes.")]
		public event CollectionChangeEventHandler CollectionChanged;

		internal event CollectionChangeEventHandler CollectionMetaDataChanged;

		public DataColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= base.List.Count)
				{
					throw new IndexOutOfRangeException("Cannot find column " + index + ".");
				}
				return (DataColumn)base.List[index];
			}
		}

		public DataColumn this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				DataColumn dataColumn = this.columnFromName[name] as DataColumn;
				if (dataColumn != null)
				{
					return dataColumn;
				}
				int num = this.IndexOf(name, true);
				return (num != -1) ? ((DataColumn)base.List[num]) : null;
			}
		}

		protected override ArrayList List
		{
			get
			{
				return base.List;
			}
		}

		internal ArrayList AutoIncrmentColumns
		{
			get
			{
				return this.autoIncrement;
			}
		}

		public DataColumn Add()
		{
			DataColumn dataColumn = new DataColumn(null);
			this.Add(dataColumn);
			return dataColumn;
		}

		public void CopyTo(DataColumn[] array, int index)
		{
			this.CopyTo(array, index);
		}

		internal void RegisterName(string name, DataColumn column)
		{
			try
			{
				this.columnFromName.Add(name, column);
			}
			catch (ArgumentException)
			{
				throw new DuplicateNameException("A DataColumn named '" + name + "' already belongs to this DataTable.");
			}
			Doublet doublet = (Doublet)this.columnNameCount[name];
			if (doublet != null)
			{
				doublet.count++;
				doublet.columnNames.Add(name);
			}
			else
			{
				doublet = new Doublet(1, name);
				this.columnNameCount[name] = doublet;
			}
			if (name.Length <= DataColumnCollection.ColumnPrefix.Length || !name.StartsWith(DataColumnCollection.ColumnPrefix, StringComparison.Ordinal))
			{
				return;
			}
			if (name == DataColumnCollection.MakeName(this.defaultColumnIndex + 1))
			{
				do
				{
					this.defaultColumnIndex++;
				}
				while (this.Contains(DataColumnCollection.MakeName(this.defaultColumnIndex + 1)));
			}
		}

		internal void UnregisterName(string name)
		{
			if (this.columnFromName.Contains(name))
			{
				this.columnFromName.Remove(name);
			}
			Doublet doublet = (Doublet)this.columnNameCount[name];
			if (doublet != null)
			{
				doublet.count--;
				doublet.columnNames.Remove(name);
				if (doublet.count == 0)
				{
					this.columnNameCount.Remove(name);
				}
			}
			if (name.StartsWith(DataColumnCollection.ColumnPrefix) && name == DataColumnCollection.MakeName(this.defaultColumnIndex - 1))
			{
				do
				{
					this.defaultColumnIndex--;
				}
				while (!this.Contains(DataColumnCollection.MakeName(this.defaultColumnIndex - 1)) && this.defaultColumnIndex > 1);
			}
		}

		private string GetNextDefaultColumnName()
		{
			string text = DataColumnCollection.MakeName(this.defaultColumnIndex);
			int num = this.defaultColumnIndex + 1;
			while (this.Contains(text))
			{
				text = DataColumnCollection.MakeName(num);
				this.defaultColumnIndex++;
				num++;
			}
			this.defaultColumnIndex++;
			return text;
		}

		private static string MakeName(int index)
		{
			if (index < 10)
			{
				return DataColumnCollection.TenColumns[index];
			}
			return DataColumnCollection.ColumnPrefix + index.ToString();
		}

		public void Add(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column", "'column' argument cannot be null.");
			}
			if (column.ColumnName.Length == 0)
			{
				column.ColumnName = this.GetNextDefaultColumnName();
			}
			if (column.Table != null)
			{
				throw new ArgumentException("Column '" + column.ColumnName + "' already belongs to this or another DataTable.");
			}
			column.SetTable(this.parentTable);
			this.RegisterName(column.ColumnName, column);
			int num = base.List.Add(column);
			column.Ordinal = num;
			if (column.CompiledExpression != null)
			{
				if (this.parentTable.Rows.Count == 0)
				{
					column.CompiledExpression.Eval(this.parentTable.NewRow());
				}
				else
				{
					column.CompiledExpression.Eval(this.parentTable.Rows[0]);
				}
			}
			if (this.parentTable.Rows.Count > 0)
			{
				column.DataContainer.Capacity = this.parentTable.RecordCache.CurrentCapacity;
			}
			if (column.AutoIncrement)
			{
				DataRowCollection rows = column.Table.Rows;
				for (int i = 0; i < rows.Count; i++)
				{
					rows[i][num] = column.AutoIncrementValue();
				}
			}
			if (column.AutoIncrement)
			{
				this.autoIncrement.Add(column);
			}
			column.PropertyChanged += this.ColumnPropertyChanged;
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
		}

		public DataColumn Add(string columnName)
		{
			DataColumn dataColumn = new DataColumn(columnName);
			this.Add(dataColumn);
			return dataColumn;
		}

		public DataColumn Add(string columnName, Type type)
		{
			if (columnName == null || columnName == string.Empty)
			{
				columnName = this.GetNextDefaultColumnName();
			}
			DataColumn dataColumn = new DataColumn(columnName, type);
			this.Add(dataColumn);
			return dataColumn;
		}

		public DataColumn Add(string columnName, Type type, string expression)
		{
			if (columnName == null || columnName == string.Empty)
			{
				columnName = this.GetNextDefaultColumnName();
			}
			DataColumn dataColumn = new DataColumn(columnName, type, expression);
			this.Add(dataColumn);
			return dataColumn;
		}

		public void AddRange(DataColumn[] columns)
		{
			if (this.parentTable.InitInProgress)
			{
				this._mostRecentColumns = columns;
				return;
			}
			if (columns == null)
			{
				return;
			}
			foreach (DataColumn dataColumn in columns)
			{
				if (dataColumn != null)
				{
					this.Add(dataColumn);
				}
			}
		}

		private string GetColumnDependency(DataColumn column)
		{
			foreach (object obj in this.parentTable.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (Array.IndexOf<DataColumn>(dataRelation.ChildColumns, column) != -1)
				{
					return string.Format(" child key for relationship {0}.", dataRelation.RelationName);
				}
			}
			foreach (object obj2 in this.parentTable.ChildRelations)
			{
				DataRelation dataRelation2 = (DataRelation)obj2;
				if (Array.IndexOf<DataColumn>(dataRelation2.ParentColumns, column) != -1)
				{
					return string.Format(" parent key for relationship {0}.", dataRelation2.RelationName);
				}
			}
			foreach (object obj3 in this.parentTable.Constraints)
			{
				Constraint constraint = (Constraint)obj3;
				if (constraint.IsColumnContained(column))
				{
					return string.Format(" constraint {0} on the table {1}.", constraint.ConstraintName, this.parentTable);
				}
			}
			if (this.parentTable.DataSet != null)
			{
				foreach (object obj4 in this.parentTable.DataSet.Tables)
				{
					DataTable dataTable = (DataTable)obj4;
					foreach (object obj5 in dataTable.Constraints)
					{
						Constraint constraint2 = (Constraint)obj5;
						if (constraint2 is ForeignKeyConstraint && constraint2.IsColumnContained(column))
						{
							return string.Format(" constraint {0} on the table {1}.", constraint2.ConstraintName, dataTable.TableName);
						}
					}
				}
			}
			foreach (object obj6 in this)
			{
				DataColumn dataColumn = (DataColumn)obj6;
				if (dataColumn.CompiledExpression != null && dataColumn.CompiledExpression.DependsOn(column))
				{
					return dataColumn.Expression;
				}
			}
			return string.Empty;
		}

		public bool CanRemove(DataColumn column)
		{
			return column != null && column.Table == this.parentTable && !(this.GetColumnDependency(column) != string.Empty);
		}

		public void Clear()
		{
			CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this);
			if (this.parentTable.Constraints.Count != 0 || this.parentTable.ParentRelations.Count != 0 || this.parentTable.ChildRelations.Count != 0)
			{
				foreach (object obj in this)
				{
					DataColumn dataColumn = (DataColumn)obj;
					string columnDependency = this.GetColumnDependency(dataColumn);
					if (columnDependency != string.Empty)
					{
						throw new ArgumentException("Cannot remove this column, because it is part of the" + columnDependency);
					}
				}
			}
			if (this.parentTable.DataSet != null)
			{
				foreach (object obj2 in this.parentTable.DataSet.Tables)
				{
					DataTable dataTable = (DataTable)obj2;
					foreach (object obj3 in dataTable.Constraints)
					{
						Constraint constraint = (Constraint)obj3;
						if (constraint is ForeignKeyConstraint && ((ForeignKeyConstraint)constraint).RelatedTable == this.parentTable)
						{
							throw new ArgumentException(string.Format("Cannot remove this column, because it is part of the constraint {0} on the table {1}", constraint.ConstraintName, dataTable.TableName));
						}
					}
				}
			}
			foreach (object obj4 in this)
			{
				DataColumn dataColumn2 = (DataColumn)obj4;
				dataColumn2.ResetColumnInfo();
			}
			this.columnFromName.Clear();
			this.autoIncrement.Clear();
			this.columnNameCount.Clear();
			base.List.Clear();
			this.defaultColumnIndex = 1;
			this.OnCollectionChanged(e);
		}

		public bool Contains(string name)
		{
			return this.columnFromName.Contains(name) || this.IndexOf(name, false) != -1;
		}

		public int IndexOf(DataColumn column)
		{
			if (column == null)
			{
				return -1;
			}
			return base.List.IndexOf(column);
		}

		public int IndexOf(string columnName)
		{
			if (columnName == null)
			{
				return -1;
			}
			DataColumn dataColumn = this.columnFromName[columnName] as DataColumn;
			if (dataColumn != null)
			{
				return this.IndexOf(dataColumn);
			}
			return this.IndexOf(columnName, false);
		}

		internal void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			this.parentTable.ResetPropertyDescriptorsCache();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		internal void OnCollectionChanging(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				throw new NotImplementedException();
			}
		}

		public void Remove(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column", "'column' argument cannot be null.");
			}
			if (!this.Contains(column.ColumnName))
			{
				throw new ArgumentException("Cannot remove a column that doesn't belong to this table.");
			}
			string columnDependency = this.GetColumnDependency(column);
			if (columnDependency != string.Empty)
			{
				throw new ArgumentException("Cannot remove this column, because it is part of " + columnDependency);
			}
			CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Remove, column);
			int ordinal = column.Ordinal;
			this.UnregisterName(column.ColumnName);
			base.List.Remove(column);
			column.ResetColumnInfo();
			for (int i = ordinal; i < this.Count; i++)
			{
				this[i].Ordinal = i;
			}
			if (this.parentTable != null)
			{
				this.parentTable.OnRemoveColumn(column);
			}
			if (column.AutoIncrement)
			{
				this.autoIncrement.Remove(column);
			}
			column.PropertyChanged -= this.ColumnPropertyChanged;
			this.OnCollectionChanged(e);
		}

		public void Remove(string name)
		{
			DataColumn dataColumn = this[name];
			if (dataColumn == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Column '",
					name,
					"' does not belong to table ",
					(this.parentTable != null) ? this.parentTable.TableName : string.Empty,
					"."
				}));
			}
			this.Remove(dataColumn);
		}

		public void RemoveAt(int index)
		{
			if (this.Count <= index)
			{
				throw new IndexOutOfRangeException("Cannot find column " + index + ".");
			}
			DataColumn dataColumn = this[index];
			this.Remove(dataColumn);
		}

		internal void PostAddRange()
		{
			if (this._mostRecentColumns == null)
			{
				return;
			}
			foreach (DataColumn dataColumn in this._mostRecentColumns)
			{
				if (dataColumn != null)
				{
					this.Add(dataColumn);
				}
			}
			this._mostRecentColumns = null;
		}

		internal void UpdateAutoIncrement(DataColumn col, bool isAutoIncrement)
		{
			if (isAutoIncrement)
			{
				if (!this.autoIncrement.Contains(col))
				{
					this.autoIncrement.Add(col);
				}
			}
			else if (this.autoIncrement.Contains(col))
			{
				this.autoIncrement.Remove(col);
			}
		}

		private int IndexOf(string name, bool error)
		{
			Doublet doublet = (Doublet)this.columnNameCount[name];
			if (doublet == null)
			{
				return -1;
			}
			if (doublet.count == 1)
			{
				return base.List.IndexOf(this.columnFromName[doublet.columnNames[0]]);
			}
			if (doublet.count > 1 && error)
			{
				throw new ArgumentException("There is no match for '" + name + "' in the same case and there are multiple matches in different case.");
			}
			return -1;
		}

		private void OnCollectionMetaDataChanged(CollectionChangeEventArgs ccevent)
		{
			this.parentTable.ResetPropertyDescriptorsCache();
			if (this.CollectionMetaDataChanged != null)
			{
				this.CollectionMetaDataChanged(this, ccevent);
			}
		}

		private void ColumnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			this.OnCollectionMetaDataChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}

		internal void MoveColumn(int oldOrdinal, int newOrdinal)
		{
			if (newOrdinal == -1 || newOrdinal > this.Count)
			{
				throw new ArgumentOutOfRangeException("ordinal", "Ordinal '" + newOrdinal + "' exceeds the maximum number.");
			}
			if (oldOrdinal == newOrdinal)
			{
				return;
			}
			int num = ((newOrdinal <= oldOrdinal) ? newOrdinal : oldOrdinal);
			int num2 = ((newOrdinal <= oldOrdinal) ? oldOrdinal : newOrdinal);
			int num3 = ((newOrdinal <= oldOrdinal) ? (-1) : 1);
			DataColumn dataColumn = this[num];
			for (int i = num; i < num2; i += num3)
			{
				this.List[i] = this.List[i + num3];
				((DataColumn)this.List[i]).Ordinal = i;
			}
			this.List[num2] = dataColumn;
			dataColumn.Ordinal = num2;
		}

		private Hashtable columnNameCount = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable columnFromName = new Hashtable();

		private ArrayList autoIncrement = new ArrayList();

		private int defaultColumnIndex = 1;

		private DataTable parentTable;

		private DataColumn[] _mostRecentColumns;

		private static readonly string ColumnPrefix = "Column";

		private static readonly string[] TenColumns = new string[] { "Column0", "Column1", "Column2", "Column3", "Column4", "Column5", "Column6", "Column7", "Column8", "Column9" };
	}
}
