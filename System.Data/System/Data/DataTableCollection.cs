using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Data
{
	[DefaultEvent("CollectionChanged")]
	[Editor("Microsoft.VSDesigner.Data.Design.TablesCollectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class DataTableCollection : InternalDataCollectionBase
	{
		internal DataTableCollection(DataSet dataSet)
		{
			this.dataSet = dataSet;
		}

		[ResDescription("Occurs whenever this collection's membership changes.")]
		public event CollectionChangeEventHandler CollectionChanged;

		public event CollectionChangeEventHandler CollectionChanging;

		public DataTable this[int index]
		{
			get
			{
				if (index < 0 || index >= this.List.Count)
				{
					throw new IndexOutOfRangeException(string.Format("Cannot find table {0}", index));
				}
				return (DataTable)this.List[index];
			}
		}

		public DataTable this[string name]
		{
			get
			{
				int num = this.IndexOf(name, true);
				return (num >= 0) ? ((DataTable)this.List[num]) : null;
			}
		}

		protected override ArrayList List
		{
			get
			{
				return base.List;
			}
		}

		public DataTable Add()
		{
			DataTable dataTable = new DataTable();
			this.Add(dataTable);
			return dataTable;
		}

		public void Add(DataTable table)
		{
			this.OnCollectionChanging(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (this.List.Contains(table))
			{
				throw new ArgumentException("DataTable already belongs to this DataSet.");
			}
			if (table.DataSet != null && table.DataSet != this.dataSet)
			{
				throw new ArgumentException("DataTable already belongs to another DataSet");
			}
			if (table.TableName == null || table.TableName == string.Empty)
			{
				this.NameTable(table);
			}
			int num = this.IndexOf(table.TableName, table.Namespace);
			if (num != -1 && table.TableName == this[num].TableName)
			{
				throw new DuplicateNameException("A DataTable named '" + table.TableName + "' already belongs to this DataSet.");
			}
			this.List.Add(table);
			table.dataSet = this.dataSet;
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));
		}

		public DataTable Add(string name)
		{
			DataTable dataTable = new DataTable(name);
			this.Add(dataTable);
			return dataTable;
		}

		public void AddRange(DataTable[] tables)
		{
			if (this.dataSet != null && this.dataSet.InitInProgress)
			{
				this.mostRecentTables = tables;
				return;
			}
			if (tables == null)
			{
				return;
			}
			foreach (DataTable dataTable in tables)
			{
				if (dataTable != null)
				{
					this.Add(dataTable);
				}
			}
		}

		internal void PostAddRange()
		{
			if (this.mostRecentTables == null)
			{
				return;
			}
			foreach (DataTable dataTable in this.mostRecentTables)
			{
				if (dataTable != null)
				{
					this.Add(dataTable);
				}
			}
			this.mostRecentTables = null;
		}

		public bool CanRemove(DataTable table)
		{
			return this.CanRemove(table, false);
		}

		public void Clear()
		{
			this.List.Clear();
		}

		public bool Contains(string name)
		{
			return -1 != this.IndexOf(name, false);
		}

		public int IndexOf(DataTable table)
		{
			return this.List.IndexOf(table);
		}

		public int IndexOf(string tableName)
		{
			return this.IndexOf(tableName, false);
		}

		public void Remove(DataTable table)
		{
			this.OnCollectionChanging(new CollectionChangeEventArgs(CollectionChangeAction.Remove, table));
			if (this.CanRemove(table, true))
			{
				table.dataSet = null;
			}
			this.List.Remove(table);
			table.dataSet = null;
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, table));
		}

		public void Remove(string name)
		{
			int num = this.IndexOf(name, false);
			if (num == -1)
			{
				throw new ArgumentException("Table " + name + " does not belong to this DataSet");
			}
			this.RemoveAt(num);
		}

		public void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		internal void OnCollectionChanging(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanging != null)
			{
				this.CollectionChanging(this, ccevent);
			}
		}

		internal void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		private int IndexOf(string name, bool error, int start)
		{
			int num = 0;
			int num2 = -1;
			for (int i = start; i < this.List.Count; i++)
			{
				string tableName = ((DataTable)this.List[i]).TableName;
				if (string.Compare(name, tableName, false) == 0)
				{
					return i;
				}
				if (string.Compare(name, tableName, true) == 0)
				{
					num2 = i;
					num++;
				}
			}
			if (num == 1)
			{
				return num2;
			}
			if (num > 1 && error)
			{
				throw new ArgumentException("There is no match for the name in the same case and there are multiple matches in different case.");
			}
			return -1;
		}

		private void NameTable(DataTable Table)
		{
			string text = "Table";
			int num = 1;
			while (this.Contains(text + num))
			{
				num++;
			}
			Table.TableName = text + num;
		}

		private bool CanRemove(DataTable table, bool throwException)
		{
			if (table == null)
			{
				if (throwException)
				{
					throw new ArgumentNullException("table");
				}
				return false;
			}
			else if (table.DataSet != this.dataSet)
			{
				if (!throwException)
				{
					return false;
				}
				throw new ArgumentException("Table " + table.TableName + " does not belong to this DataSet.");
			}
			else
			{
				if (table.ParentRelations.Count <= 0 && table.ChildRelations.Count <= 0)
				{
					foreach (object obj in table.Constraints)
					{
						Constraint constraint = (Constraint)obj;
						UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
						if (uniqueConstraint != null)
						{
							if (uniqueConstraint.ChildConstraint == null)
							{
								continue;
							}
							if (!throwException)
							{
								return false;
							}
							this.RaiseForeignKeyReferenceException(table.TableName, uniqueConstraint.ChildConstraint.ConstraintName);
						}
						ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
						if (foreignKeyConstraint != null)
						{
							if (!throwException)
							{
								return false;
							}
							this.RaiseForeignKeyReferenceException(table.TableName, foreignKeyConstraint.ConstraintName);
						}
					}
					return true;
				}
				if (!throwException)
				{
					return false;
				}
				throw new ArgumentException("Cannot remove a table that has existing relations. Remove relations first.");
			}
		}

		private void RaiseForeignKeyReferenceException(string table, string constraint)
		{
			throw new ArgumentException(string.Format("Cannot remove table {0}, because it is referenced in ForeignKeyConstraint {1}. Remove the constraint first.", table, constraint));
		}

		public DataTable this[string name, string tbNamespace]
		{
			get
			{
				int num = this.IndexOf(name, tbNamespace, true);
				return (num >= 0) ? ((DataTable)this.List[num]) : null;
			}
		}

		public DataTable Add(string name, string tbNamespace)
		{
			DataTable dataTable = new DataTable(name, tbNamespace);
			this.Add(dataTable);
			return dataTable;
		}

		public bool Contains(string name, string tableNamespace)
		{
			return this.IndexOf(name, tableNamespace) != -1;
		}

		public int IndexOf(string tableName, string tableNamespace)
		{
			if (tableNamespace == null)
			{
				throw new ArgumentNullException("'tableNamespace' argument cannot be null.", "tableNamespace");
			}
			return this.IndexOf(tableName, tableNamespace, false);
		}

		public void Remove(string name, string tableNamespace)
		{
			int num = this.IndexOf(name, tableNamespace, true);
			if (num == -1)
			{
				throw new ArgumentException("Table " + name + " does not belong to this DataSet");
			}
			this.RemoveAt(num);
		}

		private int IndexOf(string name, string ns, bool error)
		{
			int num = -1;
			int num2 = 0;
			int num3 = -1;
			do
			{
				num = this.IndexOf(name, error, num + 1);
				if (num == -1)
				{
					break;
				}
				if (ns == null)
				{
					if (num2 > 1)
					{
						break;
					}
					num2++;
					num3 = num;
				}
				else if (this[num].Namespace.Equals(ns))
				{
					return num;
				}
			}
			while (num != -1 && num < this.Count);
			if (num2 == 1)
			{
				return num3;
			}
			if (num2 == 0 || !error)
			{
				return -1;
			}
			throw new ArgumentException("The given name '" + name + "' matches atleast two namesin the collection object with different namespaces");
		}

		private int IndexOf(string name, bool error)
		{
			return this.IndexOf(name, null, error);
		}

		public void CopyTo(DataTable[] array, int index)
		{
			this.CopyTo(array, index);
		}

		internal void BinarySerialize_Schema(SerializationInfo si)
		{
			si.AddValue("DataSet.Tables.Count", this.Count);
			for (int i = 0; i < this.Count; i++)
			{
				DataTable dataTable = (DataTable)this.List[i];
				if (dataTable.dataSet != this.dataSet)
				{
					throw new SystemException("Internal Error: inconsistent DataTable");
				}
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, dataTable);
				byte[] array = memoryStream.ToArray();
				memoryStream.Close();
				si.AddValue("DataSet.Tables_" + i, array, typeof(byte[]));
			}
		}

		internal void BinarySerialize_Data(SerializationInfo si)
		{
			for (int i = 0; i < this.Count; i++)
			{
				DataTable dataTable = (DataTable)this.List[i];
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					si.AddValue(string.Concat(new object[] { "DataTable_", i, ".DataColumn_", j, ".Expression" }), dataTable.Columns[j].Expression);
				}
				dataTable.BinarySerialize(si, "DataTable_" + i + ".");
			}
		}

		private DataSet dataSet;

		private DataTable[] mostRecentTables;
	}
}
