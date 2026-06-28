using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataTableMappingCollectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class DataTableMappingCollection : MarshalByRefObject, IList, ITableMappingCollection, IEnumerable, ICollection
	{
		public DataTableMappingCollection()
		{
			this.mappings = new ArrayList();
			this.sourceTables = new Hashtable();
			this.dataSetTables = new Hashtable();
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is DataTableMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataTableMapping)value;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.mappings.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.mappings.SyncRoot;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object ITableMappingCollection.this[string index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is DataTableMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataTableMapping)value;
			}
		}

		ITableMapping ITableMappingCollection.Add(string sourceTableName, string dataSetTableName)
		{
			ITableMapping tableMapping = new DataTableMapping(sourceTableName, dataSetTableName);
			this.Add(tableMapping);
			return tableMapping;
		}

		ITableMapping ITableMappingCollection.GetByDataSetTable(string dataSetTableName)
		{
			return this[this.mappings.IndexOf(this.dataSetTables[dataSetTableName])];
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int Count
		{
			get
			{
				return this.mappings.Count;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTableMapping this[int index]
		{
			get
			{
				return (DataTableMapping)this.mappings[index];
			}
			set
			{
				DataTableMapping dataTableMapping = (DataTableMapping)this.mappings[index];
				this.sourceTables[dataTableMapping.SourceTable] = value;
				this.dataSetTables[dataTableMapping.DataSetTable] = value;
				this.mappings[index] = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataTableMapping this[string sourceTable]
		{
			get
			{
				return (DataTableMapping)this.sourceTables[sourceTable];
			}
			set
			{
				this[this.mappings.IndexOf(this.sourceTables[sourceTable])] = value;
			}
		}

		public int Add(object value)
		{
			if (!(value is DataTableMapping))
			{
				throw new InvalidCastException("The object passed in was not a DataTableMapping object.");
			}
			this.sourceTables[((DataTableMapping)value).SourceTable] = value;
			this.dataSetTables[((DataTableMapping)value).DataSetTable] = value;
			return this.mappings.Add(value);
		}

		public DataTableMapping Add(string sourceTable, string dataSetTable)
		{
			DataTableMapping dataTableMapping = new DataTableMapping(sourceTable, dataSetTable);
			this.Add(dataTableMapping);
			return dataTableMapping;
		}

		public void AddRange(Array values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				this.Add(values.GetValue(i));
			}
		}

		public void AddRange(DataTableMapping[] values)
		{
			foreach (DataTableMapping dataTableMapping in values)
			{
				this.Add(dataTableMapping);
			}
		}

		public void Clear()
		{
			this.sourceTables.Clear();
			this.dataSetTables.Clear();
			this.mappings.Clear();
		}

		public bool Contains(object value)
		{
			return this.mappings.Contains(value);
		}

		public bool Contains(string value)
		{
			return this.sourceTables.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.mappings.CopyTo(array, index);
		}

		public void CopyTo(DataTableMapping[] array, int index)
		{
			this.mappings.CopyTo(array, index);
		}

		public DataTableMapping GetByDataSetTable(string dataSetTable)
		{
			if (this.dataSetTables[dataSetTable] != null)
			{
				return (DataTableMapping)this.dataSetTables[dataSetTable];
			}
			string text = dataSetTable.ToLower();
			object[] array = new object[this.dataSetTables.Count];
			this.dataSetTables.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return (DataTableMapping)this.dataSetTables[array[i]];
				}
			}
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataTableMapping GetTableMappingBySchemaAction(DataTableMappingCollection tableMappings, string sourceTable, string dataSetTable, MissingMappingAction mappingAction)
		{
			if (tableMappings.Contains(sourceTable))
			{
				return tableMappings[sourceTable];
			}
			if (mappingAction == MissingMappingAction.Error)
			{
				throw new InvalidOperationException(string.Format("Missing source table mapping: '{0}'", sourceTable));
			}
			if (mappingAction == MissingMappingAction.Ignore)
			{
				return null;
			}
			return new DataTableMapping(sourceTable, dataSetTable);
		}

		public IEnumerator GetEnumerator()
		{
			return this.mappings.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return this.mappings.IndexOf(value);
		}

		public int IndexOf(string sourceTable)
		{
			return this.IndexOf(this.sourceTables[sourceTable]);
		}

		public int IndexOfDataSetTable(string dataSetTable)
		{
			if (this.dataSetTables[dataSetTable] != null)
			{
				return this.IndexOf((DataTableMapping)this.dataSetTables[dataSetTable]);
			}
			string text = dataSetTable.ToLower();
			object[] array = new object[this.dataSetTables.Count];
			this.dataSetTables.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return this.IndexOf((DataTableMapping)this.dataSetTables[array[i]]);
				}
			}
			return -1;
		}

		public void Insert(int index, object value)
		{
			this.mappings.Insert(index, value);
			this.sourceTables[((DataTableMapping)value).SourceTable] = value;
			this.dataSetTables[((DataTableMapping)value).DataSetTable] = value;
		}

		public void Insert(int index, DataTableMapping value)
		{
			this.mappings.Insert(index, value);
			this.sourceTables[value.SourceTable] = value;
			this.dataSetTables[value.DataSetTable] = value;
		}

		public void Remove(object value)
		{
			if (!(value is DataTableMapping))
			{
				throw new InvalidCastException();
			}
			int num = this.mappings.IndexOf(value);
			if (num < 0 || num >= this.mappings.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			this.mappings.Remove((DataTableMapping)value);
		}

		public void Remove(DataTableMapping value)
		{
			int num = this.mappings.IndexOf(value);
			if (num < 0 || num >= this.mappings.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			this.mappings.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.mappings.Count)
			{
				throw new IndexOutOfRangeException("There is no element in collection.");
			}
			this.mappings.RemoveAt(index);
		}

		public void RemoveAt(string sourceTable)
		{
			this.RemoveAt(this.mappings.IndexOf(this.sourceTables[sourceTable]));
		}

		private ArrayList mappings;

		private Hashtable sourceTables;

		private Hashtable dataSetTables;
	}
}
