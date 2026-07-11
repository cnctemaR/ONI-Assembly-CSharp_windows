using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Unity;

namespace System.Data
{
	[DefaultEvent("CollectionChanged")]
	[ListBindable(false)]
	public sealed class DataTableCollection : InternalDataCollectionBase
	{
		internal DataTableCollection(DataSet dataSet)
		{
			this._list = new ArrayList();
			this._defaultNameIndex = 1;
			this._objectID = Interlocked.Increment(ref DataTableCollection.s_objectTypeCount);
			base..ctor();
			DataCommonEventSource.Log.Trace<int, int>("<ds.DataTableCollection.DataTableCollection|INFO> {0}, dataSet={1}", this.ObjectID, (dataSet != null) ? dataSet.ObjectID : 0);
			this._dataSet = dataSet;
		}

		protected override ArrayList List
		{
			get
			{
				return this._list;
			}
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		public DataTable this[int index]
		{
			get
			{
				DataTable dataTable;
				try
				{
					dataTable = (DataTable)this._list[index];
				}
				catch (ArgumentOutOfRangeException)
				{
					throw ExceptionBuilder.TableOutOfRange(index);
				}
				return dataTable;
			}
		}

		public DataTable this[string name]
		{
			get
			{
				int num = this.InternalIndexOf(name);
				if (num == -2)
				{
					throw ExceptionBuilder.CaseInsensitiveNameConflict(name);
				}
				if (num == -3)
				{
					throw ExceptionBuilder.NamespaceNameConflict(name);
				}
				if (num >= 0)
				{
					return (DataTable)this._list[num];
				}
				return null;
			}
		}

		public DataTable this[string name, string tableNamespace]
		{
			get
			{
				if (tableNamespace == null)
				{
					throw ExceptionBuilder.ArgumentNull("tableNamespace");
				}
				int num = this.InternalIndexOf(name, tableNamespace);
				if (num == -2)
				{
					throw ExceptionBuilder.CaseInsensitiveNameConflict(name);
				}
				if (num >= 0)
				{
					return (DataTable)this._list[num];
				}
				return null;
			}
		}

		internal DataTable GetTable(string name, string ns)
		{
			for (int i = 0; i < this._list.Count; i++)
			{
				DataTable dataTable = (DataTable)this._list[i];
				if (dataTable.TableName == name && dataTable.Namespace == ns)
				{
					return dataTable;
				}
			}
			return null;
		}

		internal DataTable GetTableSmart(string name, string ns)
		{
			int num = 0;
			DataTable dataTable = null;
			for (int i = 0; i < this._list.Count; i++)
			{
				DataTable dataTable2 = (DataTable)this._list[i];
				if (dataTable2.TableName == name)
				{
					if (dataTable2.Namespace == ns)
					{
						return dataTable2;
					}
					num++;
					dataTable = dataTable2;
				}
			}
			if (num != 1)
			{
				return null;
			}
			return dataTable;
		}

		public void Add(DataTable table)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataTableCollection.Add|API> {0}, table={1}", this.ObjectID, (table != null) ? table.ObjectID : 0);
			try
			{
				this.OnCollectionChanging(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));
				this.BaseAdd(table);
				this.ArrayAdd(table);
				if (table.SetLocaleValue(this._dataSet.Locale, false, false) || table.SetCaseSensitiveValue(this._dataSet.CaseSensitive, false, false))
				{
					table.ResetIndexes();
				}
				this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void AddRange(DataTable[] tables)
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTableCollection.AddRange|API> {0}", this.ObjectID);
			try
			{
				if (this._dataSet._fInitInProgress)
				{
					this._delayedAddRangeTables = tables;
				}
				else if (tables != null)
				{
					foreach (DataTable dataTable in tables)
					{
						if (dataTable != null)
						{
							this.Add(dataTable);
						}
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public DataTable Add(string name)
		{
			DataTable dataTable = new DataTable(name);
			this.Add(dataTable);
			return dataTable;
		}

		public DataTable Add(string name, string tableNamespace)
		{
			DataTable dataTable = new DataTable(name, tableNamespace);
			this.Add(dataTable);
			return dataTable;
		}

		public DataTable Add()
		{
			DataTable dataTable = new DataTable();
			this.Add(dataTable);
			return dataTable;
		}

		public event CollectionChangeEventHandler CollectionChanged
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTableCollection.add_CollectionChanged|API> {0}", this.ObjectID);
				this._onCollectionChangedDelegate = (CollectionChangeEventHandler)Delegate.Combine(this._onCollectionChangedDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTableCollection.remove_CollectionChanged|API> {0}", this.ObjectID);
				this._onCollectionChangedDelegate = (CollectionChangeEventHandler)Delegate.Remove(this._onCollectionChangedDelegate, value);
			}
		}

		public event CollectionChangeEventHandler CollectionChanging
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTableCollection.add_CollectionChanging|API> {0}", this.ObjectID);
				this._onCollectionChangingDelegate = (CollectionChangeEventHandler)Delegate.Combine(this._onCollectionChangingDelegate, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTableCollection.remove_CollectionChanging|API> {0}", this.ObjectID);
				this._onCollectionChangingDelegate = (CollectionChangeEventHandler)Delegate.Remove(this._onCollectionChangingDelegate, value);
			}
		}

		private void ArrayAdd(DataTable table)
		{
			this._list.Add(table);
		}

		internal string AssignName()
		{
			string text;
			while (this.Contains(text = this.MakeName(this._defaultNameIndex)))
			{
				this._defaultNameIndex++;
			}
			return text;
		}

		private void BaseAdd(DataTable table)
		{
			if (table == null)
			{
				throw ExceptionBuilder.ArgumentNull("table");
			}
			if (table.DataSet == this._dataSet)
			{
				throw ExceptionBuilder.TableAlreadyInTheDataSet();
			}
			if (table.DataSet != null)
			{
				throw ExceptionBuilder.TableAlreadyInOtherDataSet();
			}
			if (table.TableName.Length == 0)
			{
				table.TableName = this.AssignName();
			}
			else
			{
				if (base.NamesEqual(table.TableName, this._dataSet.DataSetName, false, this._dataSet.Locale) != 0 && !table._fNestedInDataset)
				{
					throw ExceptionBuilder.DatasetConflictingName(this._dataSet.DataSetName);
				}
				this.RegisterName(table.TableName, table.Namespace);
			}
			table.SetDataSet(this._dataSet);
		}

		private void BaseGroupSwitch(DataTable[] oldArray, int oldLength, DataTable[] newArray, int newLength)
		{
			int num = 0;
			for (int i = 0; i < oldLength; i++)
			{
				bool flag = false;
				for (int j = num; j < newLength; j++)
				{
					if (oldArray[i] == newArray[j])
					{
						if (num == j)
						{
							num++;
						}
						flag = true;
						break;
					}
				}
				if (!flag && oldArray[i].DataSet == this._dataSet)
				{
					this.BaseRemove(oldArray[i]);
				}
			}
			for (int k = 0; k < newLength; k++)
			{
				if (newArray[k].DataSet != this._dataSet)
				{
					this.BaseAdd(newArray[k]);
					this._list.Add(newArray[k]);
				}
			}
		}

		private void BaseRemove(DataTable table)
		{
			if (this.CanRemove(table, true))
			{
				this.UnregisterName(table.TableName);
				table.SetDataSet(null);
			}
			this._list.Remove(table);
			this._dataSet.OnRemovedTable(table);
		}

		public bool CanRemove(DataTable table)
		{
			return this.CanRemove(table, false);
		}

		internal bool CanRemove(DataTable table, bool fThrowException)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, int, bool>("<ds.DataTableCollection.CanRemove|INFO> {0}, table={1}, fThrowException={2}", this.ObjectID, (table != null) ? table.ObjectID : 0, fThrowException);
			bool flag;
			try
			{
				if (table == null)
				{
					if (fThrowException)
					{
						throw ExceptionBuilder.ArgumentNull("table");
					}
					flag = false;
				}
				else if (table.DataSet != this._dataSet)
				{
					if (fThrowException)
					{
						throw ExceptionBuilder.TableNotInTheDataSet(table.TableName);
					}
					flag = false;
				}
				else
				{
					this._dataSet.OnRemoveTable(table);
					if (table.ChildRelations.Count != 0 || table.ParentRelations.Count != 0)
					{
						if (fThrowException)
						{
							throw ExceptionBuilder.TableInRelation();
						}
						flag = false;
					}
					else
					{
						ParentForeignKeyConstraintEnumerator parentForeignKeyConstraintEnumerator = new ParentForeignKeyConstraintEnumerator(this._dataSet, table);
						while (parentForeignKeyConstraintEnumerator.GetNext())
						{
							ForeignKeyConstraint foreignKeyConstraint = parentForeignKeyConstraintEnumerator.GetForeignKeyConstraint();
							if (foreignKeyConstraint.Table != table || foreignKeyConstraint.RelatedTable != table)
							{
								if (!fThrowException)
								{
									return false;
								}
								throw ExceptionBuilder.TableInConstraint(table, foreignKeyConstraint);
							}
						}
						ChildForeignKeyConstraintEnumerator childForeignKeyConstraintEnumerator = new ChildForeignKeyConstraintEnumerator(this._dataSet, table);
						while (childForeignKeyConstraintEnumerator.GetNext())
						{
							ForeignKeyConstraint foreignKeyConstraint2 = childForeignKeyConstraintEnumerator.GetForeignKeyConstraint();
							if (foreignKeyConstraint2.Table != table || foreignKeyConstraint2.RelatedTable != table)
							{
								if (!fThrowException)
								{
									return false;
								}
								throw ExceptionBuilder.TableInConstraint(table, foreignKeyConstraint2);
							}
						}
						flag = true;
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
			return flag;
		}

		public void Clear()
		{
			long num = DataCommonEventSource.Log.EnterScope<int>("<ds.DataTableCollection.Clear|API> {0}", this.ObjectID);
			try
			{
				int count = this._list.Count;
				DataTable[] array = new DataTable[this._list.Count];
				this._list.CopyTo(array, 0);
				this.OnCollectionChanging(InternalDataCollectionBase.s_refreshEventArgs);
				if (this._dataSet._fInitInProgress && this._delayedAddRangeTables != null)
				{
					this._delayedAddRangeTables = null;
				}
				this.BaseGroupSwitch(array, count, null, 0);
				this._list.Clear();
				this.OnCollectionChanged(InternalDataCollectionBase.s_refreshEventArgs);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public bool Contains(string name)
		{
			return this.InternalIndexOf(name) >= 0;
		}

		public bool Contains(string name, string tableNamespace)
		{
			if (name == null)
			{
				throw ExceptionBuilder.ArgumentNull("name");
			}
			if (tableNamespace == null)
			{
				throw ExceptionBuilder.ArgumentNull("tableNamespace");
			}
			return this.InternalIndexOf(name, tableNamespace) >= 0;
		}

		internal bool Contains(string name, string tableNamespace, bool checkProperty, bool caseSensitive)
		{
			if (!caseSensitive)
			{
				return this.InternalIndexOf(name) >= 0;
			}
			int count = this._list.Count;
			for (int i = 0; i < count; i++)
			{
				DataTable dataTable = (DataTable)this._list[i];
				string text = (checkProperty ? dataTable.Namespace : dataTable._tableNamespace);
				if (base.NamesEqual(dataTable.TableName, name, true, this._dataSet.Locale) == 1 && text == tableNamespace)
				{
					return true;
				}
			}
			return false;
		}

		internal bool Contains(string name, bool caseSensitive)
		{
			if (!caseSensitive)
			{
				return this.InternalIndexOf(name) >= 0;
			}
			int count = this._list.Count;
			for (int i = 0; i < count; i++)
			{
				DataTable dataTable = (DataTable)this._list[i];
				if (base.NamesEqual(dataTable.TableName, name, true, this._dataSet.Locale) == 1)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(DataTable[] array, int index)
		{
			if (array == null)
			{
				throw ExceptionBuilder.ArgumentNull("array");
			}
			if (index < 0)
			{
				throw ExceptionBuilder.ArgumentOutOfRange("index");
			}
			if (array.Length - index < this._list.Count)
			{
				throw ExceptionBuilder.InvalidOffsetLength();
			}
			for (int i = 0; i < this._list.Count; i++)
			{
				array[index + i] = (DataTable)this._list[i];
			}
		}

		public int IndexOf(DataTable table)
		{
			int count = this._list.Count;
			for (int i = 0; i < count; i++)
			{
				if (table == (DataTable)this._list[i])
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(string tableName)
		{
			int num = this.InternalIndexOf(tableName);
			if (num >= 0)
			{
				return num;
			}
			return -1;
		}

		public int IndexOf(string tableName, string tableNamespace)
		{
			return this.IndexOf(tableName, tableNamespace, true);
		}

		internal int IndexOf(string tableName, string tableNamespace, bool chekforNull)
		{
			if (chekforNull)
			{
				if (tableName == null)
				{
					throw ExceptionBuilder.ArgumentNull("tableName");
				}
				if (tableNamespace == null)
				{
					throw ExceptionBuilder.ArgumentNull("tableNamespace");
				}
			}
			int num = this.InternalIndexOf(tableName, tableNamespace);
			if (num >= 0)
			{
				return num;
			}
			return -1;
		}

		internal void ReplaceFromInference(List<DataTable> tableList)
		{
			this._list.Clear();
			this._list.AddRange(tableList);
		}

		internal int InternalIndexOf(string tableName)
		{
			int num = -1;
			if (tableName != null && 0 < tableName.Length)
			{
				int count = this._list.Count;
				for (int i = 0; i < count; i++)
				{
					DataTable dataTable = (DataTable)this._list[i];
					int num2 = base.NamesEqual(dataTable.TableName, tableName, false, this._dataSet.Locale);
					if (num2 == 1)
					{
						for (int j = i + 1; j < count; j++)
						{
							DataTable dataTable2 = (DataTable)this._list[j];
							if (base.NamesEqual(dataTable2.TableName, tableName, false, this._dataSet.Locale) == 1)
							{
								return -3;
							}
						}
						return i;
					}
					if (num2 == -1)
					{
						num = ((num == -1) ? i : (-2));
					}
				}
			}
			return num;
		}

		internal int InternalIndexOf(string tableName, string tableNamespace)
		{
			int num = -1;
			if (tableName != null && 0 < tableName.Length)
			{
				int count = this._list.Count;
				for (int i = 0; i < count; i++)
				{
					DataTable dataTable = (DataTable)this._list[i];
					int num2 = base.NamesEqual(dataTable.TableName, tableName, false, this._dataSet.Locale);
					if (num2 == 1 && dataTable.Namespace == tableNamespace)
					{
						return i;
					}
					if (num2 == -1 && dataTable.Namespace == tableNamespace)
					{
						num = ((num == -1) ? i : (-2));
					}
				}
			}
			return num;
		}

		internal void FinishInitCollection()
		{
			if (this._delayedAddRangeTables != null)
			{
				foreach (DataTable dataTable in this._delayedAddRangeTables)
				{
					if (dataTable != null)
					{
						this.Add(dataTable);
					}
				}
				this._delayedAddRangeTables = null;
			}
		}

		private string MakeName(int index)
		{
			if (1 != index)
			{
				return "Table" + index.ToString(CultureInfo.InvariantCulture);
			}
			return "Table1";
		}

		private void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			if (this._onCollectionChangedDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTableCollection.OnCollectionChanged|INFO> {0}", this.ObjectID);
				this._onCollectionChangedDelegate(this, ccevent);
			}
		}

		private void OnCollectionChanging(CollectionChangeEventArgs ccevent)
		{
			if (this._onCollectionChangingDelegate != null)
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataTableCollection.OnCollectionChanging|INFO> {0}", this.ObjectID);
				this._onCollectionChangingDelegate(this, ccevent);
			}
		}

		internal void RegisterName(string name, string tbNamespace)
		{
			DataCommonEventSource.Log.Trace<int, string, string>("<ds.DataTableCollection.RegisterName|INFO> {0}, name='{1}', tbNamespace='{2}'", this.ObjectID, name, tbNamespace);
			CultureInfo locale = this._dataSet.Locale;
			int count = this._list.Count;
			for (int i = 0; i < count; i++)
			{
				DataTable dataTable = (DataTable)this._list[i];
				if (base.NamesEqual(name, dataTable.TableName, true, locale) != 0 && tbNamespace == dataTable.Namespace)
				{
					throw ExceptionBuilder.DuplicateTableName(((DataTable)this._list[i]).TableName);
				}
			}
			if (base.NamesEqual(name, this.MakeName(this._defaultNameIndex), true, locale) != 0)
			{
				this._defaultNameIndex++;
			}
		}

		public void Remove(DataTable table)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataTableCollection.Remove|API> {0}, table={1}", this.ObjectID, (table != null) ? table.ObjectID : 0);
			try
			{
				this.OnCollectionChanging(new CollectionChangeEventArgs(CollectionChangeAction.Remove, table));
				this.BaseRemove(table);
				this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, table));
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void RemoveAt(int index)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataTableCollection.RemoveAt|API> {0}, index={1}", this.ObjectID, index);
			try
			{
				DataTable dataTable = this[index];
				if (dataTable == null)
				{
					throw ExceptionBuilder.TableOutOfRange(index);
				}
				this.Remove(dataTable);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void Remove(string name)
		{
			long num = DataCommonEventSource.Log.EnterScope<int, string>("<ds.DataTableCollection.Remove|API> {0}, name='{1}'", this.ObjectID, name);
			try
			{
				DataTable dataTable = this[name];
				if (dataTable == null)
				{
					throw ExceptionBuilder.TableNotInTheDataSet(name);
				}
				this.Remove(dataTable);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(num);
			}
		}

		public void Remove(string name, string tableNamespace)
		{
			if (name == null)
			{
				throw ExceptionBuilder.ArgumentNull("name");
			}
			if (tableNamespace == null)
			{
				throw ExceptionBuilder.ArgumentNull("tableNamespace");
			}
			DataTable dataTable = this[name, tableNamespace];
			if (dataTable == null)
			{
				throw ExceptionBuilder.TableNotInTheDataSet(name);
			}
			this.Remove(dataTable);
		}

		internal void UnregisterName(string name)
		{
			DataCommonEventSource.Log.Trace<int, string>("<ds.DataTableCollection.UnregisterName|INFO> {0}, name='{1}'", this.ObjectID, name);
			if (base.NamesEqual(name, this.MakeName(this._defaultNameIndex - 1), true, this._dataSet.Locale) != 0)
			{
				do
				{
					this._defaultNameIndex--;
				}
				while (this._defaultNameIndex > 1 && !this.Contains(this.MakeName(this._defaultNameIndex - 1)));
			}
		}

		internal DataTableCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly DataSet _dataSet;

		private readonly ArrayList _list;

		private int _defaultNameIndex;

		private DataTable[] _delayedAddRangeTables;

		private CollectionChangeEventHandler _onCollectionChangedDelegate;

		private CollectionChangeEventHandler _onCollectionChangingDelegate;

		private static int s_objectTypeCount;

		private readonly int _objectID;
	}
}
