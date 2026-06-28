using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public sealed class DataColumnMappingCollection : MarshalByRefObject, IList, IColumnMappingCollection, IEnumerable, ICollection
	{
		public DataColumnMappingCollection()
		{
			this.list = new ArrayList();
			this.sourceColumns = new Hashtable();
			this.dataSetColumns = new Hashtable();
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		object IColumnMappingCollection.this[string index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is DataColumnMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataColumnMapping)value;
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
				if (!(value is DataColumnMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataColumnMapping)value;
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

		IColumnMapping IColumnMappingCollection.Add(string sourceColumnName, string dataSetColumnName)
		{
			return this.Add(sourceColumnName, dataSetColumnName);
		}

		IColumnMapping IColumnMappingCollection.GetByDataSetColumn(string dataSetColumnName)
		{
			return this.GetByDataSetColumn(dataSetColumnName);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataColumnMapping this[int index]
		{
			get
			{
				return (DataColumnMapping)this.list[index];
			}
			set
			{
				DataColumnMapping dataColumnMapping = (DataColumnMapping)this.list[index];
				this.sourceColumns[dataColumnMapping] = value;
				this.dataSetColumns[dataColumnMapping] = value;
				this.list[index] = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataColumnMapping this[string sourceColumn]
		{
			get
			{
				if (!this.Contains(sourceColumn))
				{
					throw new IndexOutOfRangeException("DataColumnMappingCollection doesn't contain DataColumnMapping with SourceColumn '" + sourceColumn + "'.");
				}
				return (DataColumnMapping)this.sourceColumns[sourceColumn];
			}
			set
			{
				this[this.list.IndexOf(this.sourceColumns[sourceColumn])] = value;
			}
		}

		public int Add(object value)
		{
			if (!(value is DataColumnMapping))
			{
				throw new InvalidCastException();
			}
			this.list.Add(value);
			this.sourceColumns[((DataColumnMapping)value).SourceColumn] = value;
			this.dataSetColumns[((DataColumnMapping)value).DataSetColumn] = value;
			return this.list.IndexOf(value);
		}

		public DataColumnMapping Add(string sourceColumn, string dataSetColumn)
		{
			DataColumnMapping dataColumnMapping = new DataColumnMapping(sourceColumn, dataSetColumn);
			this.Add(dataColumnMapping);
			return dataColumnMapping;
		}

		public void AddRange(Array values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				this.Add(values.GetValue(i));
			}
		}

		public void AddRange(DataColumnMapping[] values)
		{
			foreach (DataColumnMapping dataColumnMapping in values)
			{
				this.Add(dataColumnMapping);
			}
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(object value)
		{
			if (!(value is DataColumnMapping))
			{
				throw new InvalidCastException("Object is not of type DataColumnMapping");
			}
			return this.list.Contains(value);
		}

		public bool Contains(string value)
		{
			return this.sourceColumns.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public void CopyTo(DataColumnMapping[] arr, int index)
		{
			this.list.CopyTo(arr, index);
		}

		public DataColumnMapping GetByDataSetColumn(string value)
		{
			if (this.dataSetColumns[value] != null)
			{
				return (DataColumnMapping)this.dataSetColumns[value];
			}
			string text = value.ToLower();
			object[] array = new object[this.dataSetColumns.Count];
			this.dataSetColumns.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return (DataColumnMapping)this.dataSetColumns[array[i]];
				}
			}
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataColumnMapping GetColumnMappingBySchemaAction(DataColumnMappingCollection columnMappings, string sourceColumn, MissingMappingAction mappingAction)
		{
			if (columnMappings.Contains(sourceColumn))
			{
				return columnMappings[sourceColumn];
			}
			if (mappingAction == MissingMappingAction.Ignore)
			{
				return null;
			}
			if (mappingAction == MissingMappingAction.Error)
			{
				throw new InvalidOperationException(string.Format("Missing SourceColumn mapping for '{0}'", sourceColumn));
			}
			return new DataColumnMapping(sourceColumn, sourceColumn);
		}

		[MonoTODO]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataColumn GetDataColumn(DataColumnMappingCollection columnMappings, string sourceColumn, Type dataType, DataTable dataTable, MissingMappingAction mappingAction, MissingSchemaAction schemaAction)
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}

		public int IndexOf(string sourceColumn)
		{
			return this.list.IndexOf(this.sourceColumns[sourceColumn]);
		}

		public int IndexOfDataSetColumn(string dataSetColumn)
		{
			if (this.dataSetColumns[dataSetColumn] != null)
			{
				return this.list.IndexOf(this.dataSetColumns[dataSetColumn]);
			}
			string text = dataSetColumn.ToLower();
			object[] array = new object[this.dataSetColumns.Count];
			this.dataSetColumns.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return this.list.IndexOf(this.dataSetColumns[array[i]]);
				}
			}
			return -1;
		}

		public void Insert(int index, object value)
		{
			this.list.Insert(index, value);
			this.sourceColumns[((DataColumnMapping)value).SourceColumn] = value;
			this.dataSetColumns[((DataColumnMapping)value).DataSetColumn] = value;
		}

		public void Insert(int index, DataColumnMapping mapping)
		{
			this.list.Insert(index, mapping);
			this.sourceColumns[mapping.SourceColumn] = mapping;
			this.dataSetColumns[mapping.DataSetColumn] = mapping;
		}

		public void Remove(object value)
		{
			int num = this.list.IndexOf(value);
			this.sourceColumns.Remove(((DataColumnMapping)value).SourceColumn);
			this.dataSetColumns.Remove(((DataColumnMapping)value).DataSetColumn);
			if (num < 0 || num >= this.list.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			this.list.Remove(value);
		}

		public void Remove(DataColumnMapping value)
		{
			int num = this.list.IndexOf(value);
			this.sourceColumns.Remove(value.SourceColumn);
			this.dataSetColumns.Remove(value.DataSetColumn);
			if (num < 0 || num >= this.list.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			this.list.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.list.Count)
			{
				throw new IndexOutOfRangeException("There is no element in collection.");
			}
			this.Remove(this.list[index]);
		}

		public void RemoveAt(string sourceColumn)
		{
			this.RemoveAt(this.list.IndexOf(this.sourceColumns[sourceColumn]));
		}

		private readonly ArrayList list;

		private readonly Hashtable sourceColumns;

		private readonly Hashtable dataSetColumns;
	}
}
