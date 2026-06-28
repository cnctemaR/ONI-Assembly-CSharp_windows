using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Text;

namespace System.Data
{
	[DefaultProperty("ConstraintName")]
	[Editor("Microsoft.VSDesigner.Data.Design.UniqueConstraintEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class UniqueConstraint : Constraint
	{
		public UniqueConstraint(DataColumn column)
		{
			this._uniqueConstraint(string.Empty, column, false);
		}

		public UniqueConstraint(DataColumn[] columns)
		{
			this._uniqueConstraint(string.Empty, columns, false);
		}

		public UniqueConstraint(DataColumn column, bool isPrimaryKey)
		{
			this._uniqueConstraint(string.Empty, column, isPrimaryKey);
		}

		public UniqueConstraint(DataColumn[] columns, bool isPrimaryKey)
		{
			this._uniqueConstraint(string.Empty, columns, isPrimaryKey);
		}

		public UniqueConstraint(string name, DataColumn column)
		{
			this._uniqueConstraint(name, column, false);
		}

		public UniqueConstraint(string name, DataColumn[] columns)
		{
			this._uniqueConstraint(name, columns, false);
		}

		public UniqueConstraint(string name, DataColumn column, bool isPrimaryKey)
		{
			this._uniqueConstraint(name, column, isPrimaryKey);
		}

		public UniqueConstraint(string name, DataColumn[] columns, bool isPrimaryKey)
		{
			this._uniqueConstraint(name, columns, isPrimaryKey);
		}

		[Browsable(false)]
		public UniqueConstraint(string name, string[] columnNames, bool isPrimaryKey)
		{
			this.InitInProgress = true;
			this._dataColumnNames = columnNames;
			base.ConstraintName = name;
			this._isPrimaryKey = isPrimaryKey;
		}

		private void _uniqueConstraint(string name, DataColumn column, bool isPrimaryKey)
		{
			this._validateColumn(column);
			base.ConstraintName = name;
			this._isPrimaryKey = isPrimaryKey;
			this._dataColumns = new DataColumn[] { column };
			this._dataTable = column.Table;
		}

		private void _uniqueConstraint(string name, DataColumn[] columns, bool isPrimaryKey)
		{
			this._validateColumns(columns, out this._dataTable);
			base.ConstraintName = name;
			this._dataColumns = columns;
			this._isPrimaryKey = isPrimaryKey;
		}

		private void _validateColumns(DataColumn[] columns)
		{
			DataTable dataTable;
			this._validateColumns(columns, out dataTable);
		}

		private void _validateColumns(DataColumn[] columns, out DataTable table)
		{
			table = null;
			if (columns == null)
			{
				throw new ArgumentNullException();
			}
			if (columns.Length < 1)
			{
				throw new InvalidConstraintException("Must be at least one column.");
			}
			DataTable table2 = columns[0].Table;
			foreach (DataColumn dataColumn in columns)
			{
				this._validateColumn(dataColumn);
				if (table2 != dataColumn.Table)
				{
					throw new InvalidConstraintException("Columns must be from the same table.");
				}
			}
			table = table2;
		}

		private void _validateColumn(DataColumn column)
		{
			if (column == null)
			{
				throw new NullReferenceException("Object reference not set to an instance of an object.");
			}
			if (column.Table == null)
			{
				throw new ArgumentException("Column must belong to a table.");
			}
		}

		internal static void SetAsPrimaryKey(ConstraintCollection collection, UniqueConstraint newPrimaryKey)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("ConstraintCollection can't be null.");
			}
			if (collection.IndexOf(newPrimaryKey) < 0 && newPrimaryKey != null)
			{
				throw new ArgumentException("newPrimaryKey must belong to collection.");
			}
			UniqueConstraint primaryKeyConstraint = UniqueConstraint.GetPrimaryKeyConstraint(collection);
			if (primaryKeyConstraint != null)
			{
				primaryKeyConstraint._isPrimaryKey = false;
			}
			if (newPrimaryKey != null)
			{
				newPrimaryKey._isPrimaryKey = true;
			}
		}

		internal static UniqueConstraint GetPrimaryKeyConstraint(ConstraintCollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("Collection can't be null.");
			}
			foreach (object obj in collection)
			{
				UniqueConstraint uniqueConstraint = obj as UniqueConstraint;
				if (uniqueConstraint != null)
				{
					if (uniqueConstraint.IsPrimaryKey)
					{
						return uniqueConstraint;
					}
				}
			}
			return null;
		}

		internal static UniqueConstraint GetUniqueConstraintForColumnSet(ConstraintCollection collection, DataColumn[] columns)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("Collection can't be null.");
			}
			if (columns == null)
			{
				return null;
			}
			foreach (object obj in collection)
			{
				Constraint constraint = (Constraint)obj;
				if (constraint is UniqueConstraint)
				{
					UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
					if (DataColumn.AreColumnSetsTheSame(uniqueConstraint.Columns, columns))
					{
						return uniqueConstraint;
					}
				}
			}
			return null;
		}

		internal ForeignKeyConstraint ChildConstraint
		{
			get
			{
				return this._childConstraint;
			}
			set
			{
				this._childConstraint = value;
			}
		}

		internal override void FinishInit(DataTable _setTable)
		{
			this._dataTable = _setTable;
			if (this._isPrimaryKey && _setTable.PrimaryKey.Length != 0)
			{
				throw new ArgumentException("Cannot add primary key constraint since primary keyis already set for the table");
			}
			DataColumn[] array = new DataColumn[this._dataColumnNames.Length];
			int num = 0;
			foreach (string text in this._dataColumnNames)
			{
				if (!_setTable.Columns.Contains(text))
				{
					throw new InvalidConstraintException("The named columns must exist in the table");
				}
				array[num] = _setTable.Columns[text];
				num++;
			}
			this._dataColumns = array;
			this._validateColumns(array);
			this.InitInProgress = false;
		}

		[DataCategory("Data")]
		[ReadOnly(true)]
		public virtual DataColumn[] Columns
		{
			get
			{
				return this._dataColumns;
			}
		}

		[DataCategory("Data")]
		public bool IsPrimaryKey
		{
			get
			{
				return this.Table != null && this._belongsToCollection && this._isPrimaryKey;
			}
		}

		[ReadOnly(true)]
		[DataCategory("Data")]
		public override DataTable Table
		{
			get
			{
				return this._dataTable;
			}
		}

		internal void SetIsPrimaryKey(bool value)
		{
			this._isPrimaryKey = value;
		}

		public override bool Equals(object key2)
		{
			UniqueConstraint uniqueConstraint = key2 as UniqueConstraint;
			return uniqueConstraint != null && DataColumn.AreColumnSetsTheSame(uniqueConstraint.Columns, this.Columns);
		}

		public override int GetHashCode()
		{
			int num = 42;
			if (this.Columns.Length > 0)
			{
				num ^= this.Columns[0].GetHashCode();
			}
			for (int i = 1; i < this.Columns.Length; i++)
			{
				num ^= this.Columns[1].GetHashCode();
			}
			return num;
		}

		internal override void AddToConstraintCollectionSetup(ConstraintCollection collection)
		{
			for (int i = 0; i < this.Columns.Length; i++)
			{
				if (this.Columns[i].Table != collection.Table)
				{
					throw new ArgumentException("These columns don't point to this table.");
				}
			}
			this._validateColumns(this._dataColumns);
			UniqueConstraint uniqueConstraint = UniqueConstraint.GetUniqueConstraintForColumnSet(collection, this.Columns);
			if (uniqueConstraint != null)
			{
				throw new ArgumentException("Unique constraint already exists for these columns. Existing ConstraintName is " + uniqueConstraint.ConstraintName);
			}
			if (this.IsPrimaryKey)
			{
				uniqueConstraint = UniqueConstraint.GetPrimaryKeyConstraint(collection);
				if (uniqueConstraint != null)
				{
					uniqueConstraint._isPrimaryKey = false;
				}
			}
			if (this._dataColumns.Length == 1)
			{
				this._dataColumns[0].SetUnique();
			}
			if (this.IsConstraintViolated())
			{
				throw new ArgumentException("These columns don't currently have unique values.");
			}
			this._belongsToCollection = true;
		}

		internal override void RemoveFromConstraintCollectionCleanup(ConstraintCollection collection)
		{
			if (this.Columns.Length == 1)
			{
				this.Columns[0].Unique = false;
			}
			this._belongsToCollection = false;
			Index index = base.Index;
			base.Index = null;
		}

		internal override bool IsConstraintViolated()
		{
			if (base.Index == null)
			{
				base.Index = this.Table.GetIndex(this.Columns, null, DataViewRowState.None, null, false);
			}
			if (base.Index.HasDuplicates)
			{
				int[] duplicates = base.Index.Duplicates;
				for (int i = 0; i < duplicates.Length; i++)
				{
					DataRow dataRow = this.Table.RecordCache[duplicates[i]];
					ArrayList arrayList = new ArrayList();
					ArrayList arrayList2 = new ArrayList();
					foreach (DataColumn dataColumn in this.Columns)
					{
						arrayList.Add(dataColumn.ColumnName);
						arrayList2.Add(dataRow[dataColumn].ToString());
					}
					string text = string.Join(", ", (string[])arrayList.ToArray(typeof(string)));
					string text2 = string.Join(", ", (string[])arrayList2.ToArray(typeof(string)));
					dataRow.RowError = string.Format("Column '{0}' is constrained to be unique.  Value '{1}' is already present.", text, text2);
					for (int k = 0; k < this.Columns.Length; k++)
					{
						dataRow.SetColumnError(this.Columns[k], dataRow.RowError);
					}
				}
				return true;
			}
			return false;
		}

		internal override void AssertConstraint(DataRow row)
		{
			if (this.IsPrimaryKey && row.HasVersion(DataRowVersion.Default))
			{
				for (int i = 0; i < this.Columns.Length; i++)
				{
					if (row.IsNull(this.Columns[i]))
					{
						throw new NoNullAllowedException("Column '" + this.Columns[i].ColumnName + "' does not allow nulls.");
					}
				}
			}
			if (base.Index == null)
			{
				base.Index = this.Table.GetIndex(this.Columns, null, DataViewRowState.None, null, false);
			}
			if (base.Index.HasDuplicates)
			{
				throw new ConstraintException(this.GetErrorMessage(row));
			}
		}

		internal override bool IsColumnContained(DataColumn column)
		{
			for (int i = 0; i < this._dataColumns.Length; i++)
			{
				if (column == this._dataColumns[i])
				{
					return true;
				}
			}
			return false;
		}

		internal override bool CanRemoveFromCollection(ConstraintCollection col, bool shouldThrow)
		{
			if (this.IsPrimaryKey)
			{
				if (shouldThrow)
				{
					throw new ArgumentException("Cannot remove unique constraint since it's the primary key of a table.");
				}
				return false;
			}
			else
			{
				if (this.Table.DataSet == null)
				{
					return true;
				}
				if (this.ChildConstraint == null)
				{
					return true;
				}
				if (!shouldThrow)
				{
					return false;
				}
				throw new ArgumentException(string.Format("Cannot remove unique constraint '{0}'.Remove foreign key constraint '{1}' first.", this.ConstraintName, this.ChildConstraint.ConstraintName));
			}
		}

		private string GetErrorMessage(DataRow row)
		{
			StringBuilder stringBuilder = new StringBuilder(row[this._dataColumns[0]].ToString());
			for (int i = 1; i < this._dataColumns.Length; i++)
			{
				stringBuilder = stringBuilder.Append(", ").Append(row[this._dataColumns[i].ColumnName]);
			}
			string text = stringBuilder.ToString();
			stringBuilder = new StringBuilder(this._dataColumns[0].ColumnName);
			for (int i = 1; i < this._dataColumns.Length; i++)
			{
				stringBuilder = stringBuilder.Append(", ").Append(this._dataColumns[i].ColumnName);
			}
			string text2 = stringBuilder.ToString();
			return string.Concat(new string[] { "Column '", text2, "' is constrained to be unique.  Value '", text, "' is already present." });
		}

		private bool _isPrimaryKey;

		private bool _belongsToCollection;

		private DataTable _dataTable;

		private DataColumn[] _dataColumns;

		private string[] _dataColumnNames;

		private ForeignKeyConstraint _childConstraint;
	}
}
