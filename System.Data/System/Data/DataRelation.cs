using System;
using System.ComponentModel;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataRelationEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[TypeConverter(typeof(RelationshipConverter))]
	[DefaultProperty("RelationName")]
	public class DataRelation
	{
		public DataRelation(string relationName, DataColumn parentColumn, DataColumn childColumn)
			: this(relationName, parentColumn, childColumn, true)
		{
		}

		public DataRelation(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns)
			: this(relationName, parentColumns, childColumns, true)
		{
		}

		public DataRelation(string relationName, DataColumn parentColumn, DataColumn childColumn, bool createConstraints)
			: this(relationName, new DataColumn[] { parentColumn }, new DataColumn[] { childColumn }, createConstraints)
		{
		}

		public DataRelation(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns, bool createConstraints)
		{
			this.createConstraints = true;
			this._parentTableNameSpace = string.Empty;
			this._childTableNameSpace = string.Empty;
			base..ctor();
			this.extendedProperties = new PropertyCollection();
			this.relationName = ((relationName != null) ? relationName : string.Empty);
			if (parentColumns == null)
			{
				throw new ArgumentNullException("parentColumns");
			}
			this.parentColumns = parentColumns;
			if (childColumns == null)
			{
				throw new ArgumentNullException("childColumns");
			}
			this.childColumns = childColumns;
			this.createConstraints = createConstraints;
			if (parentColumns.Length != childColumns.Length)
			{
				throw new ArgumentException("ParentColumns and ChildColumns should be the same length");
			}
			DataTable table = parentColumns[0].Table;
			DataTable table2 = childColumns[0].Table;
			if (table.DataSet != table2.DataSet)
			{
				throw new InvalidConstraintException();
			}
			foreach (DataColumn dataColumn in parentColumns)
			{
				if (dataColumn.Table != table)
				{
					throw new InvalidConstraintException();
				}
			}
			foreach (DataColumn dataColumn2 in childColumns)
			{
				if (dataColumn2.Table != table2)
				{
					throw new InvalidConstraintException();
				}
			}
			for (int k = 0; k < this.ChildColumns.Length; k++)
			{
				if (!parentColumns[k].DataTypeMatches(childColumns[k]))
				{
					throw new InvalidConstraintException("Parent Columns and Child Columns don't have matching column types");
				}
			}
		}

		[Browsable(false)]
		public DataRelation(string relationName, string parentTableName, string childTableName, string[] parentColumnNames, string[] childColumnNames, bool nested)
		{
			this.createConstraints = true;
			this._parentTableNameSpace = string.Empty;
			this._childTableNameSpace = string.Empty;
			base..ctor();
			this._relationName = relationName;
			this._parentTableName = parentTableName;
			this._childTableName = childTableName;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._nested = nested;
			this.InitInProgress = true;
		}

		[Browsable(false)]
		public DataRelation(string relationName, string parentTableName, string parentTableNameSpace, string childTableName, string childTableNameSpace, string[] parentColumnNames, string[] childColumnNames, bool nested)
		{
			this.createConstraints = true;
			this._parentTableNameSpace = string.Empty;
			this._childTableNameSpace = string.Empty;
			base..ctor();
			this._relationName = relationName;
			this._parentTableName = parentTableName;
			this._parentTableNameSpace = parentTableNameSpace;
			this._childTableName = childTableName;
			this._childTableNameSpace = childTableNameSpace;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._nested = nested;
			this.InitInProgress = true;
		}

		internal bool InitInProgress
		{
			get
			{
				return this.initInProgress;
			}
			set
			{
				this.initInProgress = value;
			}
		}

		internal void FinishInit(DataSet ds)
		{
			if (!ds.Tables.Contains(this._parentTableName) || !ds.Tables.Contains(this._childTableName))
			{
				throw new InvalidOperationException();
			}
			if (this._parentColumnNames.Length != this._childColumnNames.Length)
			{
				throw new InvalidOperationException();
			}
			DataTable dataTable = ds.Tables[this._parentTableName];
			DataTable dataTable2 = ds.Tables[this._childTableName];
			this.parentColumns = new DataColumn[this._parentColumnNames.Length];
			this.childColumns = new DataColumn[this._childColumnNames.Length];
			for (int i = 0; i < this._parentColumnNames.Length; i++)
			{
				if (!dataTable.Columns.Contains(this._parentColumnNames[i]))
				{
					throw new InvalidOperationException();
				}
				this.parentColumns[i] = dataTable.Columns[this._parentColumnNames[i]];
				if (!dataTable2.Columns.Contains(this._childColumnNames[i]))
				{
					throw new InvalidOperationException();
				}
				this.childColumns[i] = dataTable2.Columns[this._childColumnNames[i]];
			}
			this.RelationName = this._relationName;
			this.Nested = this._nested;
			this.initFinished = true;
			this.extendedProperties = new PropertyCollection();
			this.InitInProgress = false;
			if (this._parentTableNameSpace != string.Empty)
			{
				dataTable.Namespace = this._parentTableNameSpace;
			}
			if (this._childTableNameSpace != string.Empty)
			{
				dataTable2.Namespace = this._childTableNameSpace;
			}
		}

		[DataCategory("Data")]
		public virtual DataColumn[] ChildColumns
		{
			get
			{
				return this.childColumns;
			}
		}

		public virtual ForeignKeyConstraint ChildKeyConstraint
		{
			get
			{
				return this.childKeyConstraint;
			}
		}

		internal void SetChildKeyConstraint(ForeignKeyConstraint foreignKeyConstraint)
		{
			this.childKeyConstraint = foreignKeyConstraint;
		}

		public virtual DataTable ChildTable
		{
			get
			{
				return this.childColumns[0].Table;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DataSet DataSet
		{
			get
			{
				return this.childColumns[0].Table.DataSet;
			}
		}

		[DataCategory("Data")]
		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				if (this.extendedProperties == null)
				{
					this.extendedProperties = new PropertyCollection();
				}
				return this.extendedProperties;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(false)]
		public virtual bool Nested
		{
			get
			{
				return this.nested;
			}
			set
			{
				this.nested = value;
			}
		}

		[DataCategory("Data")]
		public virtual DataColumn[] ParentColumns
		{
			get
			{
				return this.parentColumns;
			}
		}

		public virtual UniqueConstraint ParentKeyConstraint
		{
			get
			{
				return this.parentKeyConstraint;
			}
		}

		internal void SetParentKeyConstraint(UniqueConstraint uniqueConstraint)
		{
			this.parentKeyConstraint = uniqueConstraint;
		}

		internal void SetDataSet(DataSet ds)
		{
			this.dataSet = ds;
		}

		public virtual DataTable ParentTable
		{
			get
			{
				return this.parentColumns[0].Table;
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public virtual string RelationName
		{
			get
			{
				return this.relationName;
			}
			set
			{
				this.relationName = value;
			}
		}

		protected void CheckStateForProperty()
		{
			DataTable table = this.parentColumns[0].Table;
			DataTable table2 = this.childColumns[0].Table;
			if (table.DataSet != table2.DataSet)
			{
				throw new DataException();
			}
			bool flag = false;
			for (int i = 0; i < this.parentColumns.Length; i++)
			{
				if (!this.parentColumns[i].DataType.Equals(this.childColumns[i].DataType))
				{
					throw new DataException();
				}
				if (this.parentColumns[i] != this.childColumns[i])
				{
					flag = false;
				}
			}
			if (flag)
			{
				throw new DataException();
			}
		}

		protected internal void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			if (this.onPropertyChangingDelegate != null)
			{
				this.onPropertyChangingDelegate(this, pcevent);
			}
		}

		protected internal void RaisePropertyChanging(string name)
		{
			this.OnPropertyChanging(new PropertyChangedEventArgs(name));
		}

		public override string ToString()
		{
			return this.relationName;
		}

		internal void UpdateConstraints()
		{
			if (this.initFinished || !this.createConstraints)
			{
				return;
			}
			ForeignKeyConstraint foreignKeyConstraint = this.FindForeignKey(this.ChildTable.Constraints);
			UniqueConstraint uniqueConstraint = this.FindUniqueConstraint(this.ParentTable.Constraints);
			if (uniqueConstraint == null)
			{
				uniqueConstraint = new UniqueConstraint(this.ParentColumns, false);
				this.ParentTable.Constraints.Add(uniqueConstraint);
			}
			if (foreignKeyConstraint == null)
			{
				foreignKeyConstraint = new ForeignKeyConstraint(this.RelationName, this.ParentColumns, this.ChildColumns);
				this.ChildTable.Constraints.Add(foreignKeyConstraint);
			}
			this.SetParentKeyConstraint(uniqueConstraint);
			this.SetChildKeyConstraint(foreignKeyConstraint);
		}

		private static bool CompareDataColumns(DataColumn[] dc1, DataColumn[] dc2)
		{
			if (dc1.Length != dc2.Length)
			{
				return false;
			}
			for (int i = 0; i < dc1.Length; i++)
			{
				if (dc1[i] != dc2[i])
				{
					return false;
				}
			}
			return true;
		}

		private ForeignKeyConstraint FindForeignKey(ConstraintCollection cl)
		{
			foreach (object obj in cl)
			{
				Constraint constraint = (Constraint)obj;
				if (constraint is ForeignKeyConstraint)
				{
					ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraint;
					if (DataRelation.CompareDataColumns(this.ChildColumns, foreignKeyConstraint.Columns) && DataRelation.CompareDataColumns(this.ParentColumns, foreignKeyConstraint.RelatedColumns))
					{
						return foreignKeyConstraint;
					}
				}
			}
			return null;
		}

		private UniqueConstraint FindUniqueConstraint(ConstraintCollection cl)
		{
			foreach (object obj in cl)
			{
				Constraint constraint = (Constraint)obj;
				if (constraint is UniqueConstraint)
				{
					UniqueConstraint uniqueConstraint = (UniqueConstraint)constraint;
					if (DataRelation.CompareDataColumns(this.ParentColumns, uniqueConstraint.Columns))
					{
						return uniqueConstraint;
					}
				}
			}
			return null;
		}

		internal bool Contains(DataColumn column)
		{
			foreach (DataColumn dataColumn in this.ParentColumns)
			{
				if (dataColumn == column)
				{
					return true;
				}
			}
			foreach (DataColumn dataColumn2 in this.ChildColumns)
			{
				if (dataColumn2 == column)
				{
					return true;
				}
			}
			return false;
		}

		private DataSet dataSet;

		private string relationName;

		private UniqueConstraint parentKeyConstraint;

		private ForeignKeyConstraint childKeyConstraint;

		private DataColumn[] parentColumns;

		private DataColumn[] childColumns;

		private bool nested;

		internal bool createConstraints;

		private bool initFinished;

		private PropertyCollection extendedProperties;

		private PropertyChangedEventHandler onPropertyChangingDelegate;

		private string _relationName;

		private string _parentTableName;

		private string _childTableName;

		private string[] _parentColumnNames;

		private string[] _childColumnNames;

		private bool _nested;

		private bool initInProgress;

		private string _parentTableNameSpace;

		private string _childTableNameSpace;
	}
}
