using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace System.Data.Common
{
	[ListBindable(false)]
	public sealed class DataTableMappingCollection : MarshalByRefObject, ITableMappingCollection, IList, ICollection, IEnumerable
	{
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this.ValidateType(value);
				this[index] = (DataTableMapping)value;
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
				this.ValidateType(value);
				this[index] = (DataTableMapping)value;
			}
		}

		ITableMapping ITableMappingCollection.Add(string sourceTableName, string dataSetTableName)
		{
			return this.Add(sourceTableName, dataSetTableName);
		}

		ITableMapping ITableMappingCollection.GetByDataSetTable(string dataSetTableName)
		{
			return this.GetByDataSetTable(dataSetTableName);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Count
		{
			get
			{
				if (this._items == null)
				{
					return 0;
				}
				return this._items.Count;
			}
		}

		private Type ItemType
		{
			get
			{
				return typeof(DataTableMapping);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataTableMapping this[int index]
		{
			get
			{
				this.RangeCheck(index);
				return this._items[index];
			}
			set
			{
				this.RangeCheck(index);
				this.Replace(index, value);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTableMapping this[string sourceTable]
		{
			get
			{
				int num = this.RangeCheck(sourceTable);
				return this._items[num];
			}
			set
			{
				int num = this.RangeCheck(sourceTable);
				this.Replace(num, value);
			}
		}

		public int Add(object value)
		{
			this.ValidateType(value);
			this.Add((DataTableMapping)value);
			return this.Count - 1;
		}

		private DataTableMapping Add(DataTableMapping value)
		{
			this.AddWithoutEvents(value);
			return value;
		}

		public void AddRange(DataTableMapping[] values)
		{
			this.AddEnumerableRange(values, false);
		}

		public void AddRange(Array values)
		{
			this.AddEnumerableRange(values, false);
		}

		private void AddEnumerableRange(IEnumerable values, bool doClone)
		{
			if (values == null)
			{
				throw ADP.ArgumentNull("values");
			}
			foreach (object obj in values)
			{
				this.ValidateType(obj);
			}
			if (doClone)
			{
				using (IEnumerator enumerator = values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj2 = enumerator.Current;
						ICloneable cloneable = (ICloneable)obj2;
						this.AddWithoutEvents(cloneable.Clone() as DataTableMapping);
					}
					return;
				}
			}
			foreach (object obj3 in values)
			{
				DataTableMapping dataTableMapping = (DataTableMapping)obj3;
				this.AddWithoutEvents(dataTableMapping);
			}
		}

		public DataTableMapping Add(string sourceTable, string dataSetTable)
		{
			return this.Add(new DataTableMapping(sourceTable, dataSetTable));
		}

		private void AddWithoutEvents(DataTableMapping value)
		{
			this.Validate(-1, value);
			value.Parent = this;
			this.ArrayList().Add(value);
		}

		private List<DataTableMapping> ArrayList()
		{
			List<DataTableMapping> list;
			if ((list = this._items) == null)
			{
				list = (this._items = new List<DataTableMapping>());
			}
			return list;
		}

		public void Clear()
		{
			if (0 < this.Count)
			{
				this.ClearWithoutEvents();
			}
		}

		private void ClearWithoutEvents()
		{
			if (this._items != null)
			{
				foreach (DataTableMapping dataTableMapping in this._items)
				{
					dataTableMapping.Parent = null;
				}
				this._items.Clear();
			}
		}

		public bool Contains(string value)
		{
			return -1 != this.IndexOf(value);
		}

		public bool Contains(object value)
		{
			return -1 != this.IndexOf(value);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)this.ArrayList()).CopyTo(array, index);
		}

		public void CopyTo(DataTableMapping[] array, int index)
		{
			this.ArrayList().CopyTo(array, index);
		}

		public DataTableMapping GetByDataSetTable(string dataSetTable)
		{
			int num = this.IndexOfDataSetTable(dataSetTable);
			if (0 > num)
			{
				throw ADP.TablesDataSetTable(dataSetTable);
			}
			return this._items[num];
		}

		public IEnumerator GetEnumerator()
		{
			return this.ArrayList().GetEnumerator();
		}

		public int IndexOf(object value)
		{
			if (value != null)
			{
				this.ValidateType(value);
				for (int i = 0; i < this.Count; i++)
				{
					if (this._items[i] == value)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int IndexOf(string sourceTable)
		{
			if (!string.IsNullOrEmpty(sourceTable))
			{
				for (int i = 0; i < this.Count; i++)
				{
					string sourceTable2 = this._items[i].SourceTable;
					if (sourceTable2 != null && ADP.SrcCompare(sourceTable, sourceTable2) == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int IndexOfDataSetTable(string dataSetTable)
		{
			if (!string.IsNullOrEmpty(dataSetTable))
			{
				for (int i = 0; i < this.Count; i++)
				{
					string dataSetTable2 = this._items[i].DataSetTable;
					if (dataSetTable2 != null && ADP.DstCompare(dataSetTable, dataSetTable2) == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public void Insert(int index, object value)
		{
			this.ValidateType(value);
			this.Insert(index, (DataTableMapping)value);
		}

		public void Insert(int index, DataTableMapping value)
		{
			if (value == null)
			{
				throw ADP.TablesAddNullAttempt("value");
			}
			this.Validate(-1, value);
			value.Parent = this;
			this.ArrayList().Insert(index, value);
		}

		private void RangeCheck(int index)
		{
			if (index < 0 || this.Count <= index)
			{
				throw ADP.TablesIndexInt32(index, this);
			}
		}

		private int RangeCheck(string sourceTable)
		{
			int num = this.IndexOf(sourceTable);
			if (num < 0)
			{
				throw ADP.TablesSourceIndex(sourceTable);
			}
			return num;
		}

		public void RemoveAt(int index)
		{
			this.RangeCheck(index);
			this.RemoveIndex(index);
		}

		public void RemoveAt(string sourceTable)
		{
			int num = this.RangeCheck(sourceTable);
			this.RemoveIndex(num);
		}

		private void RemoveIndex(int index)
		{
			this._items[index].Parent = null;
			this._items.RemoveAt(index);
		}

		public void Remove(object value)
		{
			this.ValidateType(value);
			this.Remove((DataTableMapping)value);
		}

		public void Remove(DataTableMapping value)
		{
			if (value == null)
			{
				throw ADP.TablesAddNullAttempt("value");
			}
			int num = this.IndexOf(value);
			if (-1 != num)
			{
				this.RemoveIndex(num);
				return;
			}
			throw ADP.CollectionRemoveInvalidObject(this.ItemType, this);
		}

		private void Replace(int index, DataTableMapping newValue)
		{
			this.Validate(index, newValue);
			this._items[index].Parent = null;
			newValue.Parent = this;
			this._items[index] = newValue;
		}

		private void ValidateType(object value)
		{
			if (value == null)
			{
				throw ADP.TablesAddNullAttempt("value");
			}
			if (!this.ItemType.IsInstanceOfType(value))
			{
				throw ADP.NotADataTableMapping(value);
			}
		}

		private void Validate(int index, DataTableMapping value)
		{
			if (value == null)
			{
				throw ADP.TablesAddNullAttempt("value");
			}
			if (value.Parent != null)
			{
				if (this != value.Parent)
				{
					throw ADP.TablesIsNotParent(this);
				}
				if (index != this.IndexOf(value))
				{
					throw ADP.TablesIsParent(this);
				}
			}
			string text = value.SourceTable;
			if (string.IsNullOrEmpty(text))
			{
				index = 1;
				do
				{
					text = "SourceTable" + index.ToString(CultureInfo.InvariantCulture);
					index++;
				}
				while (-1 != this.IndexOf(text));
				value.SourceTable = text;
				return;
			}
			this.ValidateSourceTable(index, text);
		}

		internal void ValidateSourceTable(int index, string value)
		{
			int num = this.IndexOf(value);
			if (-1 != num && index != num)
			{
				throw ADP.TablesUniqueSourceTable(value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataTableMapping GetTableMappingBySchemaAction(DataTableMappingCollection tableMappings, string sourceTable, string dataSetTable, MissingMappingAction mappingAction)
		{
			if (tableMappings != null)
			{
				int num = tableMappings.IndexOf(sourceTable);
				if (-1 != num)
				{
					return tableMappings._items[num];
				}
			}
			if (string.IsNullOrEmpty(sourceTable))
			{
				throw ADP.InvalidSourceTable("sourceTable");
			}
			switch (mappingAction)
			{
			case MissingMappingAction.Passthrough:
				return new DataTableMapping(sourceTable, dataSetTable);
			case MissingMappingAction.Ignore:
				return null;
			case MissingMappingAction.Error:
				throw ADP.MissingTableMapping(sourceTable);
			default:
				throw ADP.InvalidMissingMappingAction(mappingAction);
			}
		}

		private List<DataTableMapping> _items;
	}
}
