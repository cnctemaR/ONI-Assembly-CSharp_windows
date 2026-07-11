using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace System.Data
{
	internal sealed class Merger
	{
		internal Merger(DataSet dataSet, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			this._dataSet = dataSet;
			this._preserveChanges = preserveChanges;
			this._missingSchemaAction = ((missingSchemaAction == MissingSchemaAction.AddWithKey) ? MissingSchemaAction.Add : missingSchemaAction);
		}

		internal Merger(DataTable dataTable, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			this._isStandAlonetable = true;
			this._dataTable = dataTable;
			this._preserveChanges = preserveChanges;
			this._missingSchemaAction = ((missingSchemaAction == MissingSchemaAction.AddWithKey) ? MissingSchemaAction.Add : missingSchemaAction);
		}

		internal void MergeDataSet(DataSet source)
		{
			if (source == this._dataSet)
			{
				return;
			}
			bool enforceConstraints = this._dataSet.EnforceConstraints;
			this._dataSet.EnforceConstraints = false;
			this._IgnoreNSforTableLookup = this._dataSet._namespaceURI != source._namespaceURI;
			List<DataColumn> list = null;
			if (MissingSchemaAction.Add == this._missingSchemaAction)
			{
				list = new List<DataColumn>();
				foreach (object obj in this._dataSet.Tables)
				{
					foreach (object obj2 in ((DataTable)obj).Columns)
					{
						DataColumn dataColumn = (DataColumn)obj2;
						list.Add(dataColumn);
					}
				}
			}
			for (int i = 0; i < source.Tables.Count; i++)
			{
				this.MergeTableData(source.Tables[i]);
			}
			if (MissingSchemaAction.Ignore != this._missingSchemaAction)
			{
				this.MergeConstraints(source);
				for (int j = 0; j < source.Relations.Count; j++)
				{
					this.MergeRelation(source.Relations[j]);
				}
			}
			if (MissingSchemaAction.Add == this._missingSchemaAction)
			{
				foreach (object obj3 in source.Tables)
				{
					DataTable dataTable = (DataTable)obj3;
					DataTable dataTable2;
					if (this._IgnoreNSforTableLookup)
					{
						dataTable2 = this._dataSet.Tables[dataTable.TableName];
					}
					else
					{
						dataTable2 = this._dataSet.Tables[dataTable.TableName, dataTable.Namespace];
					}
					foreach (object obj4 in dataTable.Columns)
					{
						DataColumn dataColumn2 = (DataColumn)obj4;
						if (dataColumn2.Computed)
						{
							DataColumn dataColumn3 = dataTable2.Columns[dataColumn2.ColumnName];
							if (!list.Contains(dataColumn3))
							{
								dataColumn3.Expression = dataColumn2.Expression;
							}
						}
					}
				}
			}
			this.MergeExtendedProperties(source.ExtendedProperties, this._dataSet.ExtendedProperties);
			foreach (object obj5 in this._dataSet.Tables)
			{
				((DataTable)obj5).EvaluateExpressions();
			}
			this._dataSet.EnforceConstraints = enforceConstraints;
		}

		internal void MergeTable(DataTable src)
		{
			bool flag = false;
			if (!this._isStandAlonetable)
			{
				if (src.DataSet == this._dataSet)
				{
					return;
				}
				flag = this._dataSet.EnforceConstraints;
				this._dataSet.EnforceConstraints = false;
			}
			else
			{
				if (src == this._dataTable)
				{
					return;
				}
				this._dataTable.SuspendEnforceConstraints = true;
			}
			if (this._dataSet != null)
			{
				if (src.DataSet == null || src.DataSet._namespaceURI != this._dataSet._namespaceURI)
				{
					this._IgnoreNSforTableLookup = true;
				}
			}
			else if (this._dataTable.DataSet == null || src.DataSet == null || src.DataSet._namespaceURI != this._dataTable.DataSet._namespaceURI)
			{
				this._IgnoreNSforTableLookup = true;
			}
			this.MergeTableData(src);
			DataTable dataTable = this._dataTable;
			if (dataTable == null && this._dataSet != null)
			{
				dataTable = (this._IgnoreNSforTableLookup ? this._dataSet.Tables[src.TableName] : this._dataSet.Tables[src.TableName, src.Namespace]);
			}
			if (dataTable != null)
			{
				dataTable.EvaluateExpressions();
			}
			if (!this._isStandAlonetable)
			{
				this._dataSet.EnforceConstraints = flag;
				return;
			}
			this._dataTable.SuspendEnforceConstraints = false;
			try
			{
				if (this._dataTable.EnforceConstraints)
				{
					this._dataTable.EnableConstraints();
				}
			}
			catch (ConstraintException)
			{
				if (this._dataTable.DataSet != null)
				{
					this._dataTable.DataSet.EnforceConstraints = false;
				}
				throw;
			}
		}

		private void MergeTable(DataTable src, DataTable dst)
		{
			int count = src.Rows.Count;
			bool flag = dst.Rows.Count == 0;
			if (0 < count)
			{
				Index index = null;
				DataKey dataKey = default(DataKey);
				dst.SuspendIndexEvents();
				try
				{
					if (!flag && dst._primaryKey != null)
					{
						dataKey = this.GetSrcKey(src, dst);
						if (dataKey.HasValue)
						{
							index = dst._primaryKey.Key.GetSortIndex(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedOriginal);
						}
					}
					foreach (object obj in src.Rows)
					{
						DataRow dataRow = (DataRow)obj;
						DataRow dataRow2 = null;
						if (index != null)
						{
							dataRow2 = dst.FindMergeTarget(dataRow, dataKey, index);
						}
						dst.MergeRow(dataRow, dataRow2, this._preserveChanges, index);
					}
				}
				finally
				{
					dst.RestoreIndexEvents(true);
				}
			}
			this.MergeExtendedProperties(src.ExtendedProperties, dst.ExtendedProperties);
		}

		internal void MergeRows(DataRow[] rows)
		{
			DataTable dataTable = null;
			DataTable dataTable2 = null;
			DataKey dataKey = default(DataKey);
			Index index = null;
			bool enforceConstraints = this._dataSet.EnforceConstraints;
			this._dataSet.EnforceConstraints = false;
			for (int i = 0; i < rows.Length; i++)
			{
				DataRow dataRow = rows[i];
				if (dataRow == null)
				{
					throw ExceptionBuilder.ArgumentNull(string.Format("{0}[{1}]", "rows", i));
				}
				if (dataRow.Table == null)
				{
					throw ExceptionBuilder.ArgumentNull(string.Format("{0}[{1}].{2}", "rows", i, "Table"));
				}
				if (dataRow.Table.DataSet != this._dataSet)
				{
					if (dataTable != dataRow.Table)
					{
						dataTable = dataRow.Table;
						dataTable2 = this.MergeSchema(dataRow.Table);
						if (dataTable2 == null)
						{
							this._dataSet.EnforceConstraints = enforceConstraints;
							return;
						}
						if (dataTable2._primaryKey != null)
						{
							dataKey = this.GetSrcKey(dataTable, dataTable2);
						}
						if (dataKey.HasValue)
						{
							if (index != null)
							{
								index.RemoveRef();
							}
							index = new Index(dataTable2, dataTable2._primaryKey.Key.GetIndexDesc(), DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedOriginal, null);
							index.AddRef();
							index.AddRef();
						}
					}
					if (dataRow._newRecord != -1 || dataRow._oldRecord != -1)
					{
						DataRow dataRow2 = null;
						if (0 < dataTable2.Rows.Count && index != null)
						{
							dataRow2 = dataTable2.FindMergeTarget(dataRow, dataKey, index);
						}
						dataRow2 = dataTable2.MergeRow(dataRow, dataRow2, this._preserveChanges, index);
						if (dataRow2.Table._dependentColumns != null && dataRow2.Table._dependentColumns.Count > 0)
						{
							dataRow2.Table.EvaluateExpressions(dataRow2, DataRowAction.Change, null);
						}
					}
				}
			}
			if (index != null)
			{
				index.RemoveRef();
			}
			this._dataSet.EnforceConstraints = enforceConstraints;
		}

		private DataTable MergeSchema(DataTable table)
		{
			DataTable dataTable = null;
			if (!this._isStandAlonetable)
			{
				if (this._dataSet.Tables.Contains(table.TableName, true))
				{
					if (this._IgnoreNSforTableLookup)
					{
						dataTable = this._dataSet.Tables[table.TableName];
					}
					else
					{
						dataTable = this._dataSet.Tables[table.TableName, table.Namespace];
					}
				}
			}
			else
			{
				dataTable = this._dataTable;
			}
			if (dataTable == null)
			{
				if (MissingSchemaAction.Add == this._missingSchemaAction)
				{
					dataTable = table.Clone(table.DataSet);
					this._dataSet.Tables.Add(dataTable);
				}
				else if (MissingSchemaAction.Error == this._missingSchemaAction)
				{
					throw ExceptionBuilder.MergeMissingDefinition(table.TableName);
				}
			}
			else
			{
				if (MissingSchemaAction.Ignore != this._missingSchemaAction)
				{
					int count = dataTable.Columns.Count;
					for (int i = 0; i < table.Columns.Count; i++)
					{
						DataColumn dataColumn = table.Columns[i];
						DataColumn dataColumn2 = (dataTable.Columns.Contains(dataColumn.ColumnName, true) ? dataTable.Columns[dataColumn.ColumnName] : null);
						if (dataColumn2 == null)
						{
							if (MissingSchemaAction.Add == this._missingSchemaAction)
							{
								dataColumn2 = dataColumn.Clone();
								dataTable.Columns.Add(dataColumn2);
							}
							else
							{
								if (this._isStandAlonetable)
								{
									throw ExceptionBuilder.MergeFailed(SR.Format("Target table {0} missing definition for column {1}.", table.TableName, dataColumn.ColumnName));
								}
								this._dataSet.RaiseMergeFailed(dataTable, SR.Format("Target table {0} missing definition for column {1}.", table.TableName, dataColumn.ColumnName), this._missingSchemaAction);
							}
						}
						else
						{
							if (dataColumn2.DataType != dataColumn.DataType || (dataColumn2.DataType == typeof(DateTime) && dataColumn2.DateTimeMode != dataColumn.DateTimeMode && (dataColumn2.DateTimeMode & dataColumn.DateTimeMode) != DataSetDateTime.Unspecified))
							{
								if (this._isStandAlonetable)
								{
									throw ExceptionBuilder.MergeFailed(SR.Format("<target>.{0} and <source>.{0} have conflicting properties: DataType property mismatch.", dataColumn.ColumnName));
								}
								this._dataSet.RaiseMergeFailed(dataTable, SR.Format("<target>.{0} and <source>.{0} have conflicting properties: DataType property mismatch.", dataColumn.ColumnName), MissingSchemaAction.Error);
							}
							this.MergeExtendedProperties(dataColumn.ExtendedProperties, dataColumn2.ExtendedProperties);
						}
					}
					if (this._isStandAlonetable)
					{
						for (int j = count; j < dataTable.Columns.Count; j++)
						{
							dataTable.Columns[j].Expression = table.Columns[dataTable.Columns[j].ColumnName].Expression;
						}
					}
					DataColumn[] primaryKey = dataTable.PrimaryKey;
					DataColumn[] primaryKey2 = table.PrimaryKey;
					if (primaryKey.Length != primaryKey2.Length)
					{
						if (primaryKey.Length == 0)
						{
							DataColumn[] array = new DataColumn[primaryKey2.Length];
							for (int k = 0; k < primaryKey2.Length; k++)
							{
								array[k] = dataTable.Columns[primaryKey2[k].ColumnName];
							}
							dataTable.PrimaryKey = array;
						}
						else if (primaryKey2.Length != 0)
						{
							this._dataSet.RaiseMergeFailed(dataTable, "<target>.PrimaryKey and <source>.PrimaryKey have different Length.", this._missingSchemaAction);
						}
					}
					else
					{
						for (int l = 0; l < primaryKey.Length; l++)
						{
							if (string.Compare(primaryKey[l].ColumnName, primaryKey2[l].ColumnName, false, dataTable.Locale) != 0)
							{
								this._dataSet.RaiseMergeFailed(table, SR.Format("Mismatch columns in the PrimaryKey : <target>.{0} versus <source>.{1}.", primaryKey[l].ColumnName, primaryKey2[l].ColumnName), this._missingSchemaAction);
							}
						}
					}
				}
				this.MergeExtendedProperties(table.ExtendedProperties, dataTable.ExtendedProperties);
			}
			return dataTable;
		}

		private void MergeTableData(DataTable src)
		{
			DataTable dataTable = this.MergeSchema(src);
			if (dataTable == null)
			{
				return;
			}
			dataTable.MergingData = true;
			try
			{
				this.MergeTable(src, dataTable);
			}
			finally
			{
				dataTable.MergingData = false;
			}
		}

		private void MergeConstraints(DataSet source)
		{
			for (int i = 0; i < source.Tables.Count; i++)
			{
				this.MergeConstraints(source.Tables[i]);
			}
		}

		private void MergeConstraints(DataTable table)
		{
			for (int i = 0; i < table.Constraints.Count; i++)
			{
				Constraint constraint = table.Constraints[i];
				Constraint constraint2 = constraint.Clone(this._dataSet, this._IgnoreNSforTableLookup);
				if (constraint2 == null)
				{
					this._dataSet.RaiseMergeFailed(table, SR.Format("Target DataSet missing {0} {1}.", constraint.GetType().FullName, constraint.ConstraintName), this._missingSchemaAction);
				}
				else
				{
					Constraint constraint3 = constraint2.Table.Constraints.FindConstraint(constraint2);
					if (constraint3 == null)
					{
						if (MissingSchemaAction.Add == this._missingSchemaAction)
						{
							try
							{
								constraint2.Table.Constraints.Add(constraint2);
								goto IL_00F6;
							}
							catch (DuplicateNameException)
							{
								constraint2.ConstraintName = string.Empty;
								constraint2.Table.Constraints.Add(constraint2);
								goto IL_00F6;
							}
						}
						if (MissingSchemaAction.Error == this._missingSchemaAction)
						{
							this._dataSet.RaiseMergeFailed(table, SR.Format("Target DataSet missing {0} {1}.", constraint.GetType().FullName, constraint.ConstraintName), this._missingSchemaAction);
						}
					}
					else
					{
						this.MergeExtendedProperties(constraint.ExtendedProperties, constraint3.ExtendedProperties);
					}
				}
				IL_00F6:;
			}
		}

		private void MergeRelation(DataRelation relation)
		{
			DataRelation dataRelation = null;
			int num = this._dataSet.Relations.InternalIndexOf(relation.RelationName);
			if (num < 0)
			{
				if (MissingSchemaAction.Add == this._missingSchemaAction)
				{
					DataTable dataTable = (this._IgnoreNSforTableLookup ? this._dataSet.Tables[relation.ParentTable.TableName] : this._dataSet.Tables[relation.ParentTable.TableName, relation.ParentTable.Namespace]);
					DataTable dataTable2 = (this._IgnoreNSforTableLookup ? this._dataSet.Tables[relation.ChildTable.TableName] : this._dataSet.Tables[relation.ChildTable.TableName, relation.ChildTable.Namespace]);
					DataColumn[] array = new DataColumn[relation.ParentKey.ColumnsReference.Length];
					DataColumn[] array2 = new DataColumn[relation.ParentKey.ColumnsReference.Length];
					for (int i = 0; i < relation.ParentKey.ColumnsReference.Length; i++)
					{
						array[i] = dataTable.Columns[relation.ParentKey.ColumnsReference[i].ColumnName];
						array2[i] = dataTable2.Columns[relation.ChildKey.ColumnsReference[i].ColumnName];
					}
					try
					{
						dataRelation = new DataRelation(relation.RelationName, array, array2, relation._createConstraints);
						dataRelation.Nested = relation.Nested;
						this._dataSet.Relations.Add(dataRelation);
						goto IL_032D;
					}
					catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
					{
						ExceptionBuilder.TraceExceptionForCapture(ex);
						this._dataSet.RaiseMergeFailed(null, ex.Message, this._missingSchemaAction);
						goto IL_032D;
					}
				}
				throw ExceptionBuilder.MergeMissingDefinition(relation.RelationName);
			}
			dataRelation = this._dataSet.Relations[num];
			if (relation.ParentKey.ColumnsReference.Length != dataRelation.ParentKey.ColumnsReference.Length)
			{
				this._dataSet.RaiseMergeFailed(null, SR.Format("Target DataSet missing definition for {0}.", relation.RelationName), this._missingSchemaAction);
			}
			for (int j = 0; j < relation.ParentKey.ColumnsReference.Length; j++)
			{
				DataColumn dataColumn = dataRelation.ParentKey.ColumnsReference[j];
				DataColumn dataColumn2 = relation.ParentKey.ColumnsReference[j];
				if (string.Compare(dataColumn.ColumnName, dataColumn2.ColumnName, false, dataColumn.Table.Locale) != 0)
				{
					this._dataSet.RaiseMergeFailed(null, SR.Format("Relation {0} cannot be merged, because keys have mismatch columns.", relation.RelationName), this._missingSchemaAction);
				}
				dataColumn = dataRelation.ChildKey.ColumnsReference[j];
				dataColumn2 = relation.ChildKey.ColumnsReference[j];
				if (string.Compare(dataColumn.ColumnName, dataColumn2.ColumnName, false, dataColumn.Table.Locale) != 0)
				{
					this._dataSet.RaiseMergeFailed(null, SR.Format("Relation {0} cannot be merged, because keys have mismatch columns.", relation.RelationName), this._missingSchemaAction);
				}
			}
			IL_032D:
			this.MergeExtendedProperties(relation.ExtendedProperties, dataRelation.ExtendedProperties);
		}

		private void MergeExtendedProperties(PropertyCollection src, PropertyCollection dst)
		{
			if (MissingSchemaAction.Ignore == this._missingSchemaAction)
			{
				return;
			}
			IDictionaryEnumerator enumerator = src.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!this._preserveChanges || dst[enumerator.Key] == null)
				{
					dst[enumerator.Key] = enumerator.Value;
				}
			}
		}

		private DataKey GetSrcKey(DataTable src, DataTable dst)
		{
			if (src._primaryKey != null)
			{
				return src._primaryKey.Key;
			}
			DataKey dataKey = default(DataKey);
			if (dst._primaryKey != null)
			{
				DataColumn[] columnsReference = dst._primaryKey.Key.ColumnsReference;
				DataColumn[] array = new DataColumn[columnsReference.Length];
				for (int i = 0; i < columnsReference.Length; i++)
				{
					array[i] = src.Columns[columnsReference[i].ColumnName];
				}
				dataKey = new DataKey(array, false);
			}
			return dataKey;
		}

		private DataSet _dataSet;

		private DataTable _dataTable;

		private bool _preserveChanges;

		private MissingSchemaAction _missingSchemaAction;

		private bool _isStandAlonetable;

		private bool _IgnoreNSforTableLookup;
	}
}
