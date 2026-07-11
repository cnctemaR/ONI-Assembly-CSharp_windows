using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Threading;

namespace System.Data
{
	[TypeConverter(typeof(RelationshipConverter))]
	[DefaultProperty("RelationName")]
	public class DataRelation
	{
		public DataRelation(string relationName, DataColumn parentColumn, DataColumn childColumn)
			: this(relationName, parentColumn, childColumn, true)
		{
		}

		public DataRelation(string relationName, DataColumn parentColumn, DataColumn childColumn, bool createConstraints)
		{
			this._relationName = string.Empty;
			this._checkMultipleNested = true;
			this._objectID = Interlocked.Increment(ref DataRelation.s_objectTypeCount);
			base..ctor();
			DataCommonEventSource.Log.Trace<int, string, int, int, bool>("<ds.DataRelation.DataRelation|API> {0}, relationName='{1}', parentColumn={2}, childColumn={3}, createConstraints={4}", this.ObjectID, relationName, (parentColumn != null) ? parentColumn.ObjectID : 0, (childColumn != null) ? childColumn.ObjectID : 0, createConstraints);
			this.Create(relationName, new DataColumn[] { parentColumn }, new DataColumn[] { childColumn }, createConstraints);
		}

		public DataRelation(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns)
			: this(relationName, parentColumns, childColumns, true)
		{
		}

		public DataRelation(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns, bool createConstraints)
		{
			this._relationName = string.Empty;
			this._checkMultipleNested = true;
			this._objectID = Interlocked.Increment(ref DataRelation.s_objectTypeCount);
			base..ctor();
			this.Create(relationName, parentColumns, childColumns, createConstraints);
		}

		[Browsable(false)]
		public DataRelation(string relationName, string parentTableName, string childTableName, string[] parentColumnNames, string[] childColumnNames, bool nested)
		{
			this._relationName = string.Empty;
			this._checkMultipleNested = true;
			this._objectID = Interlocked.Increment(ref DataRelation.s_objectTypeCount);
			base..ctor();
			this._relationName = relationName;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._parentTableName = parentTableName;
			this._childTableName = childTableName;
			this._nested = nested;
		}

		[Browsable(false)]
		public DataRelation(string relationName, string parentTableName, string parentTableNamespace, string childTableName, string childTableNamespace, string[] parentColumnNames, string[] childColumnNames, bool nested)
		{
			this._relationName = string.Empty;
			this._checkMultipleNested = true;
			this._objectID = Interlocked.Increment(ref DataRelation.s_objectTypeCount);
			base..ctor();
			this._relationName = relationName;
			this._parentColumnNames = parentColumnNames;
			this._childColumnNames = childColumnNames;
			this._parentTableName = parentTableName;
			this._childTableName = childTableName;
			this._parentTableNamespace = parentTableNamespace;
			this._childTableNamespace = childTableNamespace;
			this._nested = nested;
		}

		public virtual DataColumn[] ChildColumns
		{
			get
			{
				this.CheckStateForProperty();
				return this._childKey.ToArray();
			}
		}

		internal DataColumn[] ChildColumnsReference
		{
			get
			{
				this.CheckStateForProperty();
				return this._childKey.ColumnsReference;
			}
		}

		internal DataKey ChildKey
		{
			get
			{
				this.CheckStateForProperty();
				return this._childKey;
			}
		}

		public virtual DataTable ChildTable
		{
			get
			{
				this.CheckStateForProperty();
				return this._childKey.Table;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual DataSet DataSet
		{
			get
			{
				this.CheckStateForProperty();
				return this._dataSet;
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

		private static bool IsKeyNull(object[] values)
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

		internal static DataRow[] GetChildRows(DataKey parentKey, DataKey childKey, DataRow parentRow, DataRowVersion version)
		{
			object[] keyValues = parentRow.GetKeyValues(parentKey, version);
			if (DataRelation.IsKeyNull(keyValues))
			{
				return childKey.Table.NewRowArray(0);
			}
			return childKey.GetSortIndex((version == DataRowVersion.Original) ? DataViewRowState.OriginalRows : DataViewRowState.CurrentRows).GetRows(keyValues);
		}

		internal static DataRow[] GetParentRows(DataKey parentKey, DataKey childKey, DataRow childRow, DataRowVersion version)
		{
			object[] keyValues = childRow.GetKeyValues(childKey, version);
			if (DataRelation.IsKeyNull(keyValues))
			{
				return parentKey.Table.NewRowArray(0);
			}
			return parentKey.GetSortIndex((version == DataRowVersion.Original) ? DataViewRowState.OriginalRows : DataViewRowState.CurrentRows).GetRows(keyValues);
		}

		internal static DataRow GetParentRow(DataKey parentKey, DataKey childKey, DataRow childRow, DataRowVersion version)
		{
			if (!childRow.HasVersion((version == DataRowVersion.Original) ? DataRowVersion.Original : DataRowVersion.Current) && childRow._tempRecord == -1)
			{
				return null;
			}
			object[] keyValues = childRow.GetKeyValues(childKey, version);
			if (DataRelation.IsKeyNull(keyValues))
			{
				return null;
			}
			Index sortIndex = parentKey.GetSortIndex((version == DataRowVersion.Original) ? DataViewRowState.OriginalRows : DataViewRowState.CurrentRows);
			Range range = sortIndex.FindRecords(keyValues);
			if (range.IsNull)
			{
				return null;
			}
			if (range.Count > 1)
			{
				throw ExceptionBuilder.MultipleParents();
			}
			return parentKey.Table._recordManager[sortIndex.GetRecord(range.Min)];
		}

		internal void SetDataSet(DataSet dataSet)
		{
			if (this._dataSet != dataSet)
			{
				this._dataSet = dataSet;
			}
		}

		internal void SetParentRowRecords(DataRow childRow, DataRow parentRow)
		{
			object[] keyValues = parentRow.GetKeyValues(this.ParentKey);
			if (childRow._tempRecord != -1)
			{
				this.ChildTable._recordManager.SetKeyValues(childRow._tempRecord, this.ChildKey, keyValues);
			}
			if (childRow._newRecord != -1)
			{
				this.ChildTable._recordManager.SetKeyValues(childRow._newRecord, this.ChildKey, keyValues);
			}
			if (childRow._oldRecord != -1)
			{
				this.ChildTable._recordManager.SetKeyValues(childRow._oldRecord, this.ChildKey, keyValues);
			}
		}

		public virtual DataColumn[] ParentColumns
		{
			get
			{
				this.CheckStateForProperty();
				return this._parentKey.ToArray();
			}
		}

		internal DataColumn[] ParentColumnsReference
		{
			get
			{
				return this._parentKey.ColumnsReference;
			}
		}

		internal DataKey ParentKey
		{
			get
			{
				this.CheckStateForProperty();
				return this._parentKey;
			}
		}

		public virtual DataTable ParentTable
		{
			get
			{
				this.CheckStateForProperty();
				return this._parentKey.Table;
			}
		}

		[DefaultValue("")]
		public virtual string RelationName
		{
			get
			{
				this.CheckStateForProperty();
				return this._relationName;
			}
			set
			{
				long num = DataCommonEventSource.Log.EnterScope<int, string>("<ds.DataRelation.set_RelationName|API> {0}, '{1}'", this.ObjectID, value);
				try
				{
					if (value == null)
					{
						value = string.Empty;
					}
					CultureInfo cultureInfo = ((this._dataSet != null) ? this._dataSet.Locale : CultureInfo.CurrentCulture);
					if (string.Compare(this._relationName, value, true, cultureInfo) != 0)
					{
						if (this._dataSet != null)
						{
							if (value.Length == 0)
							{
								throw ExceptionBuilder.NoRelationName();
							}
							this._dataSet.Relations.RegisterName(value);
							if (this._relationName.Length != 0)
							{
								this._dataSet.Relations.UnregisterName(this._relationName);
							}
						}
						this._relationName = value;
						((DataRelationCollection.DataTableRelationCollection)this.ParentTable.ChildRelations).OnRelationPropertyChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
						((DataRelationCollection.DataTableRelationCollection)this.ChildTable.ParentRelations).OnRelationPropertyChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
					}
					else if (string.Compare(this._relationName, value, false, cultureInfo) != 0)
					{
						this._relationName = value;
						((DataRelationCollection.DataTableRelationCollection)this.ParentTable.ChildRelations).OnRelationPropertyChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
						((DataRelationCollection.DataTableRelationCollection)this.ChildTable.ParentRelations).OnRelationPropertyChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(num);
				}
			}
		}

		internal void CheckNamespaceValidityForNestedRelations(string ns)
		{
			foreach (object obj in this.ChildTable.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if ((dataRelation == this || dataRelation.Nested) && dataRelation.ParentTable.Namespace != ns)
				{
					throw ExceptionBuilder.InValidNestedRelation(this.ChildTable.TableName);
				}
			}
		}

		internal void CheckNestedRelations()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.DataRelation.CheckNestedRelations|INFO> {0}", this.ObjectID);
			DataTable parentTable = this.ParentTable;
			if (this.ChildTable != this.ParentTable)
			{
				List<DataTable> list = new List<DataTable>();
				list.Add(this.ChildTable);
				for (int i = 0; i < list.Count; i++)
				{
					foreach (DataRelation dataRelation in list[i].NestedParentRelations)
					{
						if (dataRelation.ParentTable == this.ChildTable && dataRelation.ChildTable != this.ChildTable)
						{
							throw ExceptionBuilder.LoopInNestedRelations(this.ChildTable.TableName);
						}
						if (!list.Contains(dataRelation.ParentTable))
						{
							list.Add(dataRelation.ParentTable);
						}
					}
				}
				return;
			}
			if (string.Compare(this.ChildTable.TableName, this.ChildTable.DataSet.DataSetName, true, this.ChildTable.DataSet.Locale) == 0)
			{
				throw ExceptionBuilder.SelfnestedDatasetConflictingName(this.ChildTable.TableName);
			}
		}

		[DefaultValue(false)]
		public virtual bool Nested
		{
			get
			{
				this.CheckStateForProperty();
				return this._nested;
			}
			set
			{
				long num = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataRelation.set_Nested|API> {0}, {1}", this.ObjectID, value);
				try
				{
					if (this._nested != value)
					{
						if (this._dataSet != null && value)
						{
							if (this.ChildTable.IsNamespaceInherited())
							{
								this.CheckNamespaceValidityForNestedRelations(this.ParentTable.Namespace);
							}
							ForeignKeyConstraint foreignKeyConstraint = this.ChildTable.Constraints.FindForeignKeyConstraint(this.ChildKey.ColumnsReference, this.ParentKey.ColumnsReference);
							if (foreignKeyConstraint != null)
							{
								foreignKeyConstraint.CheckConstraint();
							}
							this.ValidateMultipleNestedRelations();
						}
						if (!value && this._parentKey.ColumnsReference[0].ColumnMapping == MappingType.Hidden)
						{
							throw ExceptionBuilder.RelationNestedReadOnly();
						}
						if (value)
						{
							this.ParentTable.Columns.RegisterColumnName(this.ChildTable.TableName, null);
						}
						else
						{
							this.ParentTable.Columns.UnregisterName(this.ChildTable.TableName);
						}
						this.RaisePropertyChanging("Nested");
						if (value)
						{
							this.CheckNestedRelations();
							if (this.DataSet != null)
							{
								if (this.ParentTable == this.ChildTable)
								{
									foreach (object obj in this.ChildTable.Rows)
									{
										((DataRow)obj).CheckForLoops(this);
									}
									if (this.ChildTable.DataSet != null && string.Compare(this.ChildTable.TableName, this.ChildTable.DataSet.DataSetName, true, this.ChildTable.DataSet.Locale) == 0)
									{
										throw ExceptionBuilder.DatasetConflictingName(this._dataSet.DataSetName);
									}
									this.ChildTable._fNestedInDataset = false;
								}
								else
								{
									foreach (object obj2 in this.ChildTable.Rows)
									{
										((DataRow)obj2).GetParentRow(this);
									}
								}
							}
							DataTable parentTable = this.ParentTable;
							int num2 = parentTable.ElementColumnCount;
							parentTable.ElementColumnCount = num2 + 1;
						}
						else
						{
							DataTable parentTable2 = this.ParentTable;
							int num2 = parentTable2.ElementColumnCount;
							parentTable2.ElementColumnCount = num2 - 1;
						}
						this._nested = value;
						this.ChildTable.CacheNestedParent();
						if (value && string.IsNullOrEmpty(this.ChildTable.Namespace) && (this.ChildTable.NestedParentsCount > 1 || (this.ChildTable.NestedParentsCount > 0 && !this.ChildTable.DataSet.Relations.Contains(this.RelationName))))
						{
							string text = null;
							foreach (object obj3 in this.ChildTable.ParentRelations)
							{
								DataRelation dataRelation = (DataRelation)obj3;
								if (dataRelation.Nested)
								{
									if (text == null)
									{
										text = dataRelation.ParentTable.Namespace;
									}
									else if (string.Compare(text, dataRelation.ParentTable.Namespace, StringComparison.Ordinal) != 0)
									{
										this._nested = false;
										throw ExceptionBuilder.InvalidParentNamespaceinNestedRelation(this.ChildTable.TableName);
									}
								}
							}
							if (this.CheckMultipleNested && this.ChildTable._tableNamespace != null && this.ChildTable._tableNamespace.Length == 0)
							{
								throw ExceptionBuilder.TableCantBeNestedInTwoTables(this.ChildTable.TableName);
							}
							this.ChildTable._tableNamespace = null;
						}
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(num);
				}
			}
		}

		public virtual UniqueConstraint ParentKeyConstraint
		{
			get
			{
				this.CheckStateForProperty();
				return this._parentKeyConstraint;
			}
		}

		internal void SetParentKeyConstraint(UniqueConstraint value)
		{
			this._parentKeyConstraint = value;
		}

		public virtual ForeignKeyConstraint ChildKeyConstraint
		{
			get
			{
				this.CheckStateForProperty();
				return this._childKeyConstraint;
			}
		}

		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				PropertyCollection propertyCollection;
				if ((propertyCollection = this._extendedProperties) == null)
				{
					propertyCollection = (this._extendedProperties = new PropertyCollection());
				}
				return propertyCollection;
			}
		}

		internal bool CheckMultipleNested
		{
			get
			{
				return this._checkMultipleNested;
			}
			set
			{
				this._checkMultipleNested = value;
			}
		}

		internal void SetChildKeyConstraint(ForeignKeyConstraint value)
		{
			this._childKeyConstraint = value;
		}

		internal event PropertyChangedEventHandler PropertyChanging;

		internal void CheckState()
		{
			if (this._dataSet == null)
			{
				this._parentKey.CheckState();
				this._childKey.CheckState();
				if (this._parentKey.Table.DataSet != this._childKey.Table.DataSet)
				{
					throw ExceptionBuilder.RelationDataSetMismatch();
				}
				if (this._childKey.ColumnsEqual(this._parentKey))
				{
					throw ExceptionBuilder.KeyColumnsIdentical();
				}
				for (int i = 0; i < this._parentKey.ColumnsReference.Length; i++)
				{
					if (this._parentKey.ColumnsReference[i].DataType != this._childKey.ColumnsReference[i].DataType || (this._parentKey.ColumnsReference[i].DataType == typeof(DateTime) && this._parentKey.ColumnsReference[i].DateTimeMode != this._childKey.ColumnsReference[i].DateTimeMode && (this._parentKey.ColumnsReference[i].DateTimeMode & this._childKey.ColumnsReference[i].DateTimeMode) != DataSetDateTime.Unspecified))
					{
						throw ExceptionBuilder.ColumnsTypeMismatch();
					}
				}
			}
		}

		protected void CheckStateForProperty()
		{
			try
			{
				this.CheckState();
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				throw ExceptionBuilder.BadObjectPropertyAccess(ex.Message);
			}
		}

		private void Create(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns, bool createConstraints)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, string, bool>("<ds.DataRelation.Create|INFO> {0}, relationName='{1}', createConstraints={2}", this.ObjectID, relationName, createConstraints);
			try
			{
				this._parentKey = new DataKey(parentColumns, true);
				this._childKey = new DataKey(childColumns, true);
				if (parentColumns.Length != childColumns.Length)
				{
					throw ExceptionBuilder.KeyLengthMismatch();
				}
				for (int i = 0; i < parentColumns.Length; i++)
				{
					if (parentColumns[i].Table.DataSet == null || childColumns[i].Table.DataSet == null)
					{
						throw ExceptionBuilder.ParentOrChildColumnsDoNotHaveDataSet();
					}
				}
				this.CheckState();
				this._relationName = ((relationName == null) ? "" : relationName);
				this._createConstraints = createConstraints;
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		internal DataRelation Clone(DataSet destination)
		{
			DataCommonEventSource.Log.Trace<int, int>("<ds.DataRelation.Clone|INFO> {0}, destination={1}", this.ObjectID, (destination != null) ? destination.ObjectID : 0);
			DataTable dataTable = destination.Tables[this.ParentTable.TableName, this.ParentTable.Namespace];
			DataTable dataTable2 = destination.Tables[this.ChildTable.TableName, this.ChildTable.Namespace];
			int num = this._parentKey.ColumnsReference.Length;
			DataColumn[] array = new DataColumn[num];
			DataColumn[] array2 = new DataColumn[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = dataTable.Columns[this.ParentKey.ColumnsReference[i].ColumnName];
				array2[i] = dataTable2.Columns[this.ChildKey.ColumnsReference[i].ColumnName];
			}
			DataRelation dataRelation = new DataRelation(this._relationName, array, array2, false);
			dataRelation.CheckMultipleNested = false;
			dataRelation.Nested = this.Nested;
			dataRelation.CheckMultipleNested = true;
			if (this._extendedProperties != null)
			{
				foreach (object obj in this._extendedProperties.Keys)
				{
					dataRelation.ExtendedProperties[obj] = this._extendedProperties[obj];
				}
			}
			return dataRelation;
		}

		protected internal void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			if (this.PropertyChanging != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataRelation.OnPropertyChanging|INFO> {0}", this.ObjectID);
				this.PropertyChanging(this, pcevent);
			}
		}

		protected internal void RaisePropertyChanging(string name)
		{
			this.OnPropertyChanging(new PropertyChangedEventArgs(name));
		}

		public override string ToString()
		{
			return this.RelationName;
		}

		internal void ValidateMultipleNestedRelations()
		{
			if (!this.Nested || !this.CheckMultipleNested)
			{
				return;
			}
			if (this.ChildTable.NestedParentRelations.Length != 0)
			{
				DataColumn[] childColumns = this.ChildColumns;
				if (childColumns.Length != 1 || !this.IsAutoGenerated(childColumns[0]))
				{
					throw ExceptionBuilder.TableCantBeNestedInTwoTables(this.ChildTable.TableName);
				}
				if (!XmlTreeGen.AutoGenerated(this))
				{
					throw ExceptionBuilder.TableCantBeNestedInTwoTables(this.ChildTable.TableName);
				}
				foreach (object obj in this.ChildTable.Constraints)
				{
					Constraint constraint = (Constraint)obj;
					if (constraint is ForeignKeyConstraint)
					{
						if (!XmlTreeGen.AutoGenerated((ForeignKeyConstraint)constraint, true))
						{
							throw ExceptionBuilder.TableCantBeNestedInTwoTables(this.ChildTable.TableName);
						}
					}
					else if (!XmlTreeGen.AutoGenerated((UniqueConstraint)constraint))
					{
						throw ExceptionBuilder.TableCantBeNestedInTwoTables(this.ChildTable.TableName);
					}
				}
			}
		}

		private bool IsAutoGenerated(DataColumn col)
		{
			if (col.ColumnMapping != MappingType.Hidden)
			{
				return false;
			}
			if (col.DataType != typeof(int))
			{
				return false;
			}
			string text = col.Table.TableName + "_Id";
			if (col.ColumnName == text || col.ColumnName == text + "_0")
			{
				return true;
			}
			text = this.ParentColumnsReference[0].Table.TableName + "_Id";
			return col.ColumnName == text || col.ColumnName == text + "_0";
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		private DataSet _dataSet;

		internal PropertyCollection _extendedProperties;

		internal string _relationName;

		private DataKey _childKey;

		private DataKey _parentKey;

		private UniqueConstraint _parentKeyConstraint;

		private ForeignKeyConstraint _childKeyConstraint;

		internal string[] _parentColumnNames;

		internal string[] _childColumnNames;

		internal string _parentTableName;

		internal string _childTableName;

		internal string _parentTableNamespace;

		internal string _childTableNamespace;

		internal bool _nested;

		internal bool _createConstraints;

		private bool _checkMultipleNested;

		private static int s_objectTypeCount;

		private readonly int _objectID;
	}
}
