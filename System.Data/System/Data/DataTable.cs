using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Mono.Data.SqlExpressions;

namespace System.Data
{
	[DefaultEvent("RowChanging")]
	[ToolboxItem(false)]
	[Editor("Microsoft.VSDesigner.Data.Design.DataTableEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[XmlSchemaProvider("GetDataTableSchema")]
	[DesignTimeVisible(false)]
	[DefaultProperty("TableName")]
	[Serializable]
	public class DataTable : MarshalByValueComponent, IXmlSerializable, IListSource, ISupportInitialize, ISerializable, ISupportInitializeNotification
	{
		public DataTable()
		{
			this.dataSet = null;
			this._columnCollection = new DataColumnCollection(this);
			this._constraintCollection = new ConstraintCollection(this);
			this._extendedProperties = new PropertyCollection();
			this._tableName = string.Empty;
			this._nameSpace = null;
			this._caseSensitive = false;
			this._displayExpression = null;
			this._primaryKeyConstraint = null;
			this._site = null;
			this._rows = new DataRowCollection(this);
			this._indexes = new ArrayList();
			this._recordCache = new RecordCache(this);
			this._minimumCapacity = 50;
			this._childRelations = new DataRelationCollection.DataTableRelationCollection(this);
			this._parentRelations = new DataRelationCollection.DataTableRelationCollection(this);
		}

		public DataTable(string tableName)
			: this()
		{
			this._tableName = tableName;
		}

		protected DataTable(SerializationInfo info, StreamingContext context)
			: this()
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			SerializationFormat serializationFormat = SerializationFormat.Xml;
			while (enumerator.MoveNext())
			{
				if (enumerator.ObjectType == typeof(SerializationFormat))
				{
					serializationFormat = (SerializationFormat)((int)enumerator.Value);
					break;
				}
			}
			if (serializationFormat == SerializationFormat.Xml)
			{
				string @string = info.GetString("XmlSchema");
				string string2 = info.GetString("XmlDiffGram");
				DataSet dataSet = new DataSet();
				dataSet.ReadXmlSchema(new StringReader(@string));
				dataSet.Tables[0].CopyProperties(this);
				dataSet = new DataSet();
				dataSet.Tables.Add(this);
				dataSet.ReadXml(new StringReader(string2), XmlReadMode.DiffGram);
				dataSet.Tables.Remove(this);
			}
			else
			{
				this.BinaryDeserializeTable(info);
			}
		}

		public DataTable(string tableName, string tbNamespace)
			: this(tableName)
		{
			this._nameSpace = tbNamespace;
		}

		[DataCategory("Data")]
		public event DataColumnChangeEventHandler ColumnChanged;

		[DataCategory("Data")]
		public event DataColumnChangeEventHandler ColumnChanging;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowChanged;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowChanging;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowDeleted;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowDeleting;

		public event EventHandler Initialized;

		[DataCategory("Data")]
		public event DataTableClearEventHandler TableCleared;

		[DataCategory("Data")]
		public event DataTableClearEventHandler TableClearing;

		public event DataTableNewRowEventHandler TableNewRow;

		bool IListSource.ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		IList IListSource.GetList()
		{
			return this.DefaultView;
		}

		[MonoNotSupported("")]
		XmlSchema IXmlSerializable.GetSchema()
		{
			return this.GetSchema();
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			this.ReadXml_internal(reader, true);
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			DataSet dataSet = this.dataSet;
			bool flag = true;
			if (this.dataSet == null)
			{
				dataSet = new DataSet();
				dataSet.Tables.Add(this);
				flag = false;
			}
			XmlSchemaWriter.WriteXmlSchema(writer, new DataTable[] { this }, null, this.TableName, dataSet.DataSetName, (!this.LocaleSpecified) ? ((!dataSet.LocaleSpecified) ? null : dataSet.Locale) : this.Locale);
			dataSet.WriteIndividualTableContent(writer, this, XmlWriteMode.DiffGram);
			writer.Flush();
			if (!flag)
			{
				this.dataSet.Tables.Remove(this);
			}
		}

		public bool CaseSensitive
		{
			get
			{
				if (this._virginCaseSensitive && this.dataSet != null)
				{
					return this.dataSet.CaseSensitive;
				}
				return this._caseSensitive;
			}
			set
			{
				if (this._childRelations.Count > 0 || this._parentRelations.Count > 0)
				{
					throw new ArgumentException("Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.");
				}
				this._virginCaseSensitive = false;
				this._caseSensitive = value;
				this.ResetCaseSensitiveIndexes();
			}
		}

		internal ArrayList Indexes
		{
			get
			{
				return this._indexes;
			}
		}

		internal void ChangedDataColumn(DataRow dr, DataColumn dc, object pv)
		{
			DataColumnChangeEventArgs e = new DataColumnChangeEventArgs(dr, dc, pv);
			this.OnColumnChanged(e);
		}

		internal void ChangingDataColumn(DataRow dr, DataColumn dc, object pv)
		{
			DataColumnChangeEventArgs e = new DataColumnChangeEventArgs(dr, dc, pv);
			this.OnColumnChanging(e);
		}

		internal void DeletedDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			this.OnRowDeleted(e);
		}

		internal void DeletingDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			this.OnRowDeleting(e);
		}

		internal void ChangedDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			this.OnRowChanged(e);
		}

		internal void ChangingDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			this.OnRowChanging(e);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRelationCollection ChildRelations
		{
			get
			{
				return this._childRelations;
			}
		}

		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataColumnCollection Columns
		{
			get
			{
				return this._columnCollection;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DataCategory("Data")]
		public ConstraintCollection Constraints
		{
			get
			{
				return this._constraintCollection;
			}
			internal set
			{
				this._constraintCollection = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataSet DataSet
		{
			get
			{
				return this.dataSet;
			}
		}

		[Browsable(false)]
		public DataView DefaultView
		{
			get
			{
				if (this._defaultView == null)
				{
					lock (this)
					{
						if (this._defaultView == null)
						{
							if (this.dataSet != null)
							{
								this._defaultView = this.dataSet.DefaultViewManager.CreateDataView(this);
							}
							else
							{
								this._defaultView = new DataView(this);
							}
						}
					}
				}
				return this._defaultView;
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string DisplayExpression
		{
			get
			{
				return (this._displayExpression != null) ? this._displayExpression : string.Empty;
			}
			set
			{
				this._displayExpression = value;
			}
		}

		[DataCategory("Data")]
		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				return this._extendedProperties;
			}
		}

		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				for (int i = 0; i < this._rows.Count; i++)
				{
					if (this._rows[i].HasErrors)
					{
						return true;
					}
				}
				return false;
			}
		}

		public CultureInfo Locale
		{
			get
			{
				if (this._locale != null)
				{
					return this._locale;
				}
				if (this.DataSet != null)
				{
					return this.DataSet.Locale;
				}
				return CultureInfo.CurrentCulture;
			}
			set
			{
				if (this._childRelations.Count > 0 || this._parentRelations.Count > 0)
				{
					throw new ArgumentException("Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.");
				}
				if (this._locale == null || !this._locale.Equals(value))
				{
					this._locale = value;
				}
			}
		}

		internal bool LocaleSpecified
		{
			get
			{
				return this._locale != null;
			}
		}

		[DefaultValue(50)]
		[DataCategory("Data")]
		public int MinimumCapacity
		{
			get
			{
				return this._minimumCapacity;
			}
			set
			{
				this._minimumCapacity = value;
			}
		}

		[DataCategory("Data")]
		public string Namespace
		{
			get
			{
				if (this._nameSpace != null)
				{
					return this._nameSpace;
				}
				if (this.DataSet != null)
				{
					return this.DataSet.Namespace;
				}
				return string.Empty;
			}
			set
			{
				this._nameSpace = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRelationCollection ParentRelations
		{
			get
			{
				return this._parentRelations;
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				return (this._prefix != null) ? this._prefix : string.Empty;
			}
			set
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (!char.IsLetterOrDigit(value[i]) && value[i] != '_' && value[i] != ':')
					{
						throw new DataException("Prefix '" + value + "' is not valid, because it contains special characters.");
					}
				}
				this._prefix = value;
			}
		}

		[TypeConverter("System.Data.PrimaryKeyTypeConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		[DataCategory("Data")]
		[Editor("Microsoft.VSDesigner.Data.Design.PrimaryKeyEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public DataColumn[] PrimaryKey
		{
			get
			{
				if (this._primaryKeyConstraint == null)
				{
					return new DataColumn[0];
				}
				return this._primaryKeyConstraint.Columns;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					if (this._primaryKeyConstraint != null)
					{
						this._primaryKeyConstraint.SetIsPrimaryKey(false);
						this.Constraints.Remove(this._primaryKeyConstraint);
						this._primaryKeyConstraint = null;
					}
					return;
				}
				if (this.InitInProgress)
				{
					this._latestPrimaryKeyCols = value;
					return;
				}
				if (this._primaryKeyConstraint != null && DataColumn.AreColumnSetsTheSame(value, this._primaryKeyConstraint.Columns))
				{
					return;
				}
				UniqueConstraint uniqueConstraint = UniqueConstraint.GetUniqueConstraintForColumnSet(this.Constraints, value);
				if (uniqueConstraint == null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						DataColumn dataColumn = value[i];
						if (dataColumn.Table == null)
						{
							break;
						}
						if (this.Columns.IndexOf(dataColumn) < 0)
						{
							throw new ArgumentException("PrimaryKey columns do not belong to this table.");
						}
					}
					uniqueConstraint = new UniqueConstraint(value, false);
					this.Constraints.Add(uniqueConstraint);
				}
				if (this._primaryKeyConstraint != null)
				{
					this._primaryKeyConstraint.SetIsPrimaryKey(false);
					this.Constraints.Remove(this._primaryKeyConstraint);
					this._primaryKeyConstraint = null;
				}
				UniqueConstraint.SetAsPrimaryKey(this.Constraints, uniqueConstraint);
				this._primaryKeyConstraint = uniqueConstraint;
				for (int j = 0; j < uniqueConstraint.Columns.Length; j++)
				{
					uniqueConstraint.Columns[j].AllowDBNull = false;
				}
			}
		}

		internal UniqueConstraint PrimaryKeyConstraint
		{
			get
			{
				return this._primaryKeyConstraint;
			}
		}

		[Browsable(false)]
		public DataRowCollection Rows
		{
			get
			{
				return this._rows;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site
		{
			get
			{
				return this._site;
			}
			set
			{
				this._site = value;
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		public string TableName
		{
			get
			{
				return (this._tableName != null) ? this._tableName : string.Empty;
			}
			set
			{
				this._tableName = value;
			}
		}

		internal RecordCache RecordCache
		{
			get
			{
				return this._recordCache;
			}
		}

		private DataRowBuilder RowBuilder
		{
			get
			{
				if (this._rowBuilder == null)
				{
					this._rowBuilder = new DataRowBuilder(this, -1, 0);
				}
				else
				{
					this._rowBuilder._rowId = -1;
				}
				return this._rowBuilder;
			}
		}

		internal bool EnforceConstraints
		{
			get
			{
				return this.enforceConstraints;
			}
			set
			{
				if (value == this.enforceConstraints)
				{
					return;
				}
				if (value)
				{
					this.ResetIndexes();
					foreach (object obj in this.Constraints)
					{
						Constraint constraint = (Constraint)obj;
						constraint.AssertConstraint();
					}
					this.AssertNotNullConstraints();
					if (this.HasErrors)
					{
						Constraint.ThrowConstraintException();
					}
				}
				this.enforceConstraints = value;
			}
		}

		internal void AssertNotNullConstraints()
		{
			if (this._duringDataLoad && !this._nullConstraintViolationDuringDataLoad)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.Columns.Count; i++)
			{
				DataColumn dataColumn = this.Columns[i];
				if (!dataColumn.AllowDBNull)
				{
					for (int j = 0; j < this.Rows.Count; j++)
					{
						if (this.Rows[j].HasVersion(DataRowVersion.Default) && this.Rows[j].IsNull(dataColumn))
						{
							flag = true;
							string text = string.Format("Column '{0}' does not allow DBNull.Value.", dataColumn.ColumnName);
							this.Rows[j].SetColumnError(i, text);
							this.Rows[j].RowError = text;
						}
					}
				}
			}
			this._nullConstraintViolationDuringDataLoad = flag;
		}

		internal bool RowsExist(DataColumn[] columns, DataColumn[] relatedColumns, DataRow row)
		{
			int num = row.IndexFromVersion(DataRowVersion.Default);
			int num2 = this.RecordCache.NewRecord();
			bool flag;
			try
			{
				for (int i = 0; i < relatedColumns.Length; i++)
				{
					columns[i].DataContainer.CopyValue(relatedColumns[i].DataContainer, num, num2);
				}
				flag = this.RowsExist(columns, num2);
			}
			finally
			{
				this.RecordCache.DisposeRecord(num2);
			}
			return flag;
		}

		private bool RowsExist(DataColumn[] columns, int index)
		{
			Index index2 = this.FindIndex(columns);
			if (index2 != null)
			{
				return index2.Find(index) != -1;
			}
			foreach (object obj in this.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow.RowState != DataRowState.Deleted)
				{
					int num = dataRow.IndexFromVersion((dataRow.RowState != DataRowState.Modified) ? DataRowVersion.Current : DataRowVersion.Original);
					bool flag = true;
					foreach (DataColumn dataColumn in columns)
					{
						if (dataColumn.DataContainer.CompareValues(num, index) != 0)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void AcceptChanges()
		{
			int i = 0;
			while (i < this.Rows.Count)
			{
				DataRow dataRow = this.Rows[i];
				dataRow.AcceptChanges();
				if (dataRow.RowState != DataRowState.Detached)
				{
					i++;
				}
			}
			this._rows.OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}

		public virtual void BeginInit()
		{
			this.InitInProgress = true;
			this.tableInitialized = false;
		}

		public void BeginLoadData()
		{
			if (this._duringDataLoad)
			{
				return;
			}
			this._duringDataLoad = true;
			this._nullConstraintViolationDuringDataLoad = false;
			if (this.dataSet != null)
			{
				this.dataSetPrevEnforceConstraints = this.dataSet.EnforceConstraints;
				this.dataSet.EnforceConstraints = false;
			}
			else
			{
				this.EnforceConstraints = false;
			}
		}

		public void Clear()
		{
			this._rows.Clear();
		}

		public virtual DataTable Clone()
		{
			DataTable dataTable = (DataTable)Activator.CreateInstance(base.GetType(), true);
			this.CopyProperties(dataTable);
			return dataTable;
		}

		public object Compute(string expression, string filter)
		{
			DataRow[] array = this.Select(filter);
			if (array == null || array.Length == 0)
			{
				return DBNull.Value;
			}
			Parser parser = new Parser(array);
			IExpression expression2 = parser.Compile(expression);
			return expression2.Eval(array[0]);
		}

		public DataTable Copy()
		{
			DataTable dataTable = this.Clone();
			dataTable._duringDataLoad = true;
			foreach (object obj in this.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				DataRow dataRow2 = dataTable.NewNotInitializedRow();
				dataTable.Rows.AddInternal(dataRow2);
				this.CopyRow(dataRow, dataRow2);
			}
			dataTable._duringDataLoad = false;
			dataTable.ResetIndexes();
			return dataTable;
		}

		internal void CopyRow(DataRow fromRow, DataRow toRow)
		{
			if (fromRow.HasErrors)
			{
				fromRow.CopyErrors(toRow);
			}
			if (fromRow.HasVersion(DataRowVersion.Original))
			{
				toRow.Original = toRow.Table.RecordCache.CopyRecord(this, fromRow.Original, -1);
			}
			if (fromRow.HasVersion(DataRowVersion.Current))
			{
				if (fromRow.Original != fromRow.Current)
				{
					toRow.Current = toRow.Table.RecordCache.CopyRecord(this, fromRow.Current, -1);
				}
				else
				{
					toRow.Current = toRow.Original;
				}
			}
		}

		private void CopyProperties(DataTable Copy)
		{
			Copy.CaseSensitive = this.CaseSensitive;
			Copy._virginCaseSensitive = this._virginCaseSensitive;
			Copy.DisplayExpression = this.DisplayExpression;
			if (this.ExtendedProperties.Count > 0)
			{
				Array array = Array.CreateInstance(typeof(object), this.ExtendedProperties.Count);
				this.ExtendedProperties.Keys.CopyTo(array, 0);
				for (int i = 0; i < this.ExtendedProperties.Count; i++)
				{
					Copy.ExtendedProperties.Add(array.GetValue(i), this.ExtendedProperties[array.GetValue(i)]);
				}
			}
			Copy._locale = this._locale;
			Copy.MinimumCapacity = this.MinimumCapacity;
			Copy.Namespace = this.Namespace;
			Copy.Prefix = this.Prefix;
			Copy.Site = this.Site;
			Copy.TableName = this.TableName;
			bool flag = Copy.Columns.Count == 0;
			foreach (object obj in this.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (flag || !Copy.Columns.Contains(dataColumn.ColumnName))
				{
					Copy.Columns.Add(dataColumn.Clone());
				}
			}
			this.CopyConstraints(Copy);
			if (this.PrimaryKey.Length > 0)
			{
				DataColumn[] array2 = new DataColumn[this.PrimaryKey.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = Copy.Columns[this.PrimaryKey[j].ColumnName];
				}
				Copy.PrimaryKey = array2;
			}
		}

		private void CopyConstraints(DataTable copy)
		{
			for (int i = 0; i < this.Constraints.Count; i++)
			{
				if (this.Constraints[i] is UniqueConstraint)
				{
					if (!copy.Constraints.Contains(this.Constraints[i].ConstraintName))
					{
						UniqueConstraint uniqueConstraint = (UniqueConstraint)this.Constraints[i];
						DataColumn[] array = new DataColumn[uniqueConstraint.Columns.Length];
						for (int j = 0; j < array.Length; j++)
						{
							array[j] = copy.Columns[uniqueConstraint.Columns[j].ColumnName];
						}
						UniqueConstraint uniqueConstraint2 = new UniqueConstraint(uniqueConstraint.ConstraintName, array, uniqueConstraint.IsPrimaryKey);
						copy.Constraints.Add(uniqueConstraint2);
					}
				}
			}
		}

		public virtual void EndInit()
		{
			this.InitInProgress = false;
			this.DataTableInitialized();
			this.FinishInit();
		}

		internal bool InitInProgress
		{
			get
			{
				return this.fInitInProgress;
			}
			set
			{
				this.fInitInProgress = value;
			}
		}

		internal void FinishInit()
		{
			UniqueConstraint primaryKeyConstraint = this._primaryKeyConstraint;
			this.Columns.PostAddRange();
			this._constraintCollection.PostAddRange();
			if (this._primaryKeyConstraint == primaryKeyConstraint)
			{
				this.PrimaryKey = this._latestPrimaryKeyCols;
			}
		}

		public void EndLoadData()
		{
			if (this._duringDataLoad)
			{
				if (this.dataSet != null)
				{
					this.dataSet.InternalEnforceConstraints(this.dataSetPrevEnforceConstraints, true);
				}
				else
				{
					this.EnforceConstraints = true;
				}
				this._duringDataLoad = false;
			}
		}

		public DataTable GetChanges()
		{
			return this.GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}

		public DataTable GetChanges(DataRowState rowStates)
		{
			DataTable dataTable = null;
			foreach (object obj in this.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow.IsRowChanged(rowStates))
				{
					if (dataTable == null)
					{
						dataTable = this.Clone();
					}
					DataRow dataRow2 = dataTable.NewNotInitializedRow();
					dataRow.CopyValuesToRow(dataRow2);
					dataRow2.XmlRowID = dataRow.XmlRowID;
					dataTable.Rows.AddInternal(dataRow2);
				}
			}
			return dataTable;
		}

		public DataRow[] GetErrors()
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this._rows.Count; i++)
			{
				if (this._rows[i].HasErrors)
				{
					arrayList.Add(this._rows[i]);
				}
			}
			DataRow[] array = this.NewRowArray(arrayList.Count);
			arrayList.CopyTo(array, 0);
			return array;
		}

		protected virtual DataTable CreateInstance()
		{
			return Activator.CreateInstance(base.GetType(), true) as DataTable;
		}

		protected virtual Type GetRowType()
		{
			return typeof(DataRow);
		}

		public void ImportRow(DataRow row)
		{
			if (row.RowState == DataRowState.Detached)
			{
				return;
			}
			DataRow dataRow = this.NewNotInitializedRow();
			int num = -1;
			if (row.HasVersion(DataRowVersion.Original))
			{
				num = row.IndexFromVersion(DataRowVersion.Original);
				dataRow.Original = this.RecordCache.NewRecord();
				this.RecordCache.CopyRecord(row.Table, num, dataRow.Original);
			}
			if (row.HasVersion(DataRowVersion.Current))
			{
				int num2 = row.IndexFromVersion(DataRowVersion.Current);
				if (num2 == num)
				{
					dataRow.Current = dataRow.Original;
				}
				else
				{
					dataRow.Current = this.RecordCache.NewRecord();
					this.RecordCache.CopyRecord(row.Table, num2, dataRow.Current);
				}
			}
			if (row.RowState != DataRowState.Deleted)
			{
				dataRow.Validate();
			}
			else
			{
				this.AddRowToIndexes(dataRow);
			}
			this.Rows.AddInternal(dataRow);
			if (row.HasErrors)
			{
				row.CopyErrors(dataRow);
			}
		}

		internal int DefaultValuesRowIndex
		{
			get
			{
				return this._defaultValuesRowIndex;
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (this.RemotingFormat == SerializationFormat.Xml)
			{
				DataSet dataSet;
				if (this.dataSet != null)
				{
					dataSet = this.dataSet;
				}
				else
				{
					dataSet = new DataSet("tmpDataSet");
					dataSet.Tables.Add(this);
				}
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.Formatting = Formatting.Indented;
				dataSet.WriteIndividualTableContent(xmlTextWriter, this, XmlWriteMode.DiffGram);
				xmlTextWriter.Close();
				StringWriter stringWriter2 = new StringWriter();
				DataTableCollection dataTableCollection = new DataTableCollection(dataSet);
				dataTableCollection.Add(this);
				XmlSchemaWriter.WriteXmlSchema(dataSet, new XmlTextWriter(stringWriter2), dataTableCollection, null);
				stringWriter2.Close();
				info.AddValue("XmlSchema", stringWriter2.ToString(), typeof(string));
				info.AddValue("XmlDiffGram", stringWriter.ToString(), typeof(string));
			}
			else
			{
				this.BinarySerializeProperty(info);
				if (this.dataSet == null)
				{
					for (int i = 0; i < this.Columns.Count; i++)
					{
						info.AddValue("DataTable.DataColumn_" + i + ".Expression", this.Columns[i].Expression);
					}
					this.BinarySerialize(info, "DataTable_0.");
				}
			}
		}

		public DataRow LoadDataRow(object[] values, bool fAcceptChanges)
		{
			DataRow dataRow;
			if (this.PrimaryKey.Length == 0)
			{
				dataRow = this.Rows.Add(values);
			}
			else
			{
				this.EnsureDefaultValueRowIndex();
				int num = this.CreateRecord(values);
				int num2 = this._primaryKeyConstraint.Index.Find(num);
				if (num2 < 0)
				{
					dataRow = this.NewRowFromBuilder(this.RowBuilder);
					dataRow.Proposed = num;
					this.Rows.AddInternal(dataRow);
					if (!this._duringDataLoad)
					{
						this.AddRowToIndexes(dataRow);
					}
				}
				else
				{
					dataRow = this.RecordCache[num2];
					dataRow.BeginEdit();
					dataRow.ImportRecord(num);
					dataRow.EndEdit();
				}
			}
			if (fAcceptChanges)
			{
				dataRow.AcceptChanges();
			}
			return dataRow;
		}

		internal DataRow LoadDataRow(IDataRecord record, int[] mapping, int length, bool fAcceptChanges)
		{
			DataRow dataRow = null;
			int num = this.RecordCache.NewRecord();
			try
			{
				this.RecordCache.ReadIDataRecord(num, record, mapping, length);
				if (this.PrimaryKey.Length != 0)
				{
					bool flag = true;
					foreach (DataColumn dataColumn in this.PrimaryKey)
					{
						if (dataColumn.Ordinal >= mapping.Length)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						int num2 = this._primaryKeyConstraint.Index.Find(num);
						if (num2 != -1)
						{
							dataRow = this.RecordCache[num2];
						}
					}
				}
				if (dataRow == null)
				{
					dataRow = this.NewNotInitializedRow();
					dataRow.Proposed = num;
					this.Rows.AddInternal(dataRow);
				}
				else
				{
					dataRow.BeginEdit();
					dataRow.ImportRecord(num);
					dataRow.EndEdit();
				}
				if (fAcceptChanges)
				{
					dataRow.AcceptChanges();
				}
			}
			catch
			{
				this.RecordCache.DisposeRecord(num);
				throw;
			}
			return dataRow;
		}

		public DataRow NewRow()
		{
			this.EnsureDefaultValueRowIndex();
			DataRow dataRow = this.NewRowFromBuilder(this.RowBuilder);
			dataRow.Proposed = this.CreateRecord(null);
			this.NewRowAdded(dataRow);
			return dataRow;
		}

		internal int CreateRecord(object[] values)
		{
			int num = ((values == null) ? 0 : values.Length);
			if (num > this.Columns.Count)
			{
				throw new ArgumentException("Input array is longer than the number of columns in this table.");
			}
			int num2 = this.RecordCache.NewRecord();
			int num3;
			try
			{
				for (int i = 0; i < num; i++)
				{
					if (values[i] == null)
					{
						this.Columns[i].SetDefaultValue(num2);
					}
					else
					{
						this.Columns[i][num2] = values[i];
					}
				}
				for (int j = num; j < this.Columns.Count; j++)
				{
					this.Columns[j].SetDefaultValue(num2);
				}
				num3 = num2;
			}
			catch
			{
				this.RecordCache.DisposeRecord(num2);
				throw;
			}
			return num3;
		}

		private void EnsureDefaultValueRowIndex()
		{
			if (this._defaultValuesRowIndex == -1)
			{
				this._defaultValuesRowIndex = this.RecordCache.NewRecord();
				for (int i = 0; i < this.Columns.Count; i++)
				{
					DataColumn dataColumn = this.Columns[i];
					dataColumn.DataContainer[this._defaultValuesRowIndex] = dataColumn.DefaultValue;
				}
			}
		}

		protected internal DataRow[] NewRowArray(int size)
		{
			if (size == 0 && this.empty_rows != null)
			{
				return this.empty_rows;
			}
			Type rowType = this.GetRowType();
			DataRow[] array = ((rowType != typeof(DataRow)) ? ((DataRow[])Array.CreateInstance(rowType, size)) : new DataRow[size]);
			if (size == 0)
			{
				this.empty_rows = array;
			}
			return array;
		}

		protected virtual DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new DataRow(builder);
		}

		internal DataRow NewNotInitializedRow()
		{
			this.EnsureDefaultValueRowIndex();
			return this.NewRowFromBuilder(this.RowBuilder);
		}

		public void RejectChanges()
		{
			for (int i = this._rows.Count - 1; i >= 0; i--)
			{
				DataRow dataRow = this._rows[i];
				if (dataRow.RowState != DataRowState.Unchanged)
				{
					this._rows[i].RejectChanges();
				}
			}
		}

		public virtual void Reset()
		{
			this.Clear();
			while (this.ParentRelations.Count > 0)
			{
				if (this.dataSet.Relations.Contains(this.ParentRelations[this.ParentRelations.Count - 1].RelationName))
				{
					this.dataSet.Relations.Remove(this.ParentRelations[this.ParentRelations.Count - 1]);
				}
			}
			while (this.ChildRelations.Count > 0)
			{
				if (this.dataSet.Relations.Contains(this.ChildRelations[this.ChildRelations.Count - 1].RelationName))
				{
					this.dataSet.Relations.Remove(this.ChildRelations[this.ChildRelations.Count - 1]);
				}
			}
			this.Constraints.Clear();
			this.Columns.Clear();
		}

		public DataRow[] Select()
		{
			return this.Select(string.Empty, string.Empty, DataViewRowState.CurrentRows);
		}

		public DataRow[] Select(string filterExpression)
		{
			return this.Select(filterExpression, string.Empty, DataViewRowState.CurrentRows);
		}

		public DataRow[] Select(string filterExpression, string sort)
		{
			return this.Select(filterExpression, sort, DataViewRowState.CurrentRows);
		}

		public DataRow[] Select(string filterExpression, string sort, DataViewRowState recordStates)
		{
			if (filterExpression == null)
			{
				filterExpression = string.Empty;
			}
			IExpression expression = null;
			if (filterExpression != string.Empty)
			{
				Parser parser = new Parser();
				expression = parser.Compile(filterExpression);
			}
			DataColumn[] array = DataTable._emptyColumnArray;
			ListSortDirection[] array2 = null;
			if (sort != null && !sort.Equals(string.Empty))
			{
				array = DataTable.ParseSortString(this, sort, out array2, false);
			}
			if (this.Rows.Count == 0)
			{
				return this.NewRowArray(0);
			}
			if (array.Length == 0 && expression != null)
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < this.Columns.Count; i++)
				{
					if (expression.DependsOn(this.Columns[i]))
					{
						arrayList.Add(this.Columns[i]);
					}
				}
				array = (DataColumn[])arrayList.ToArray(typeof(DataColumn));
			}
			bool flag = true;
			if (filterExpression != string.Empty)
			{
				flag = false;
			}
			Index index = this.GetIndex(array, array2, recordStates, expression, false, flag);
			int[] all = index.GetAll();
			DataRow[] array3 = this.NewRowArray(index.Size);
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j] = this.RecordCache[all[j]];
			}
			return array3;
		}

		private void AddIndex(Index index)
		{
			if (this._indexes == null)
			{
				this._indexes = new ArrayList();
			}
			this._indexes.Add(index);
		}

		internal Index GetIndex(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter, bool reset)
		{
			return this.GetIndex(columns, sort, rowState, filter, reset, true);
		}

		internal Index GetIndex(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter, bool reset, bool addIndex)
		{
			Index index = this.FindIndex(columns, sort, rowState, filter);
			if (index == null)
			{
				index = new Index(new Key(this, columns, sort, rowState, filter));
				if (addIndex)
				{
					this.AddIndex(index);
				}
			}
			else if (reset)
			{
				index.Reset();
			}
			return index;
		}

		internal Index FindIndex(DataColumn[] columns)
		{
			return this.FindIndex(columns, null, DataViewRowState.None, null);
		}

		internal Index FindIndex(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter)
		{
			if (this.Indexes != null)
			{
				foreach (object obj in this.Indexes)
				{
					Index index = (Index)obj;
					if (index.Key.Equals(columns, sort, rowState, filter))
					{
						return index;
					}
				}
			}
			return null;
		}

		internal void ResetIndexes()
		{
			foreach (object obj in this.Indexes)
			{
				Index index = (Index)obj;
				index.Reset();
			}
		}

		internal void ResetCaseSensitiveIndexes()
		{
			foreach (object obj in this.Indexes)
			{
				Index index = (Index)obj;
				bool flag = false;
				foreach (DataColumn dataColumn in index.Key.Columns)
				{
					if (dataColumn.DataType == typeof(string))
					{
						flag = true;
						break;
					}
				}
				if (!flag && index.Key.HasFilter)
				{
					foreach (object obj2 in this.Columns)
					{
						DataColumn dataColumn2 = (DataColumn)obj2;
						if (dataColumn2.DataType == DbTypes.TypeOfString && index.Key.DependsOn(dataColumn2))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					index.Reset();
				}
			}
		}

		internal void DropIndex(Index index)
		{
			if (index != null && index.RefCount == 0)
			{
				this._indexes.Remove(index);
			}
		}

		internal void DropReferencedIndexes(DataColumn column)
		{
			if (this._indexes != null)
			{
				for (int i = this._indexes.Count - 1; i >= 0; i--)
				{
					Index index = (Index)this._indexes[i];
					if (index.Key.DependsOn(column))
					{
						this._indexes.Remove(index);
					}
				}
			}
		}

		internal void AddRowToIndexes(DataRow row)
		{
			if (this._indexes != null)
			{
				for (int i = 0; i < this._indexes.Count; i++)
				{
					((Index)this._indexes[i]).Add(row);
				}
			}
		}

		internal void DeleteRowFromIndexes(DataRow row)
		{
			if (this._indexes != null)
			{
				foreach (object obj in this._indexes)
				{
					Index index = (Index)obj;
					index.Delete(row);
				}
			}
		}

		public override string ToString()
		{
			string text = this.TableName;
			if (this.DisplayExpression != null && this.DisplayExpression != string.Empty)
			{
				text = text + " + " + this.DisplayExpression;
			}
			return text;
		}

		protected virtual void OnColumnChanged(DataColumnChangeEventArgs e)
		{
			if (this.ColumnChanged != null)
			{
				this.ColumnChanged(this, e);
			}
		}

		internal void RaiseOnColumnChanged(DataColumnChangeEventArgs e)
		{
			this.OnColumnChanged(e);
		}

		protected virtual void OnColumnChanging(DataColumnChangeEventArgs e)
		{
			if (this.ColumnChanging != null)
			{
				this.ColumnChanging(this, e);
			}
		}

		internal void RaiseOnColumnChanging(DataColumnChangeEventArgs e)
		{
			this.OnColumnChanging(e);
		}

		[MonoTODO]
		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			throw new NotImplementedException();
		}

		protected internal virtual void OnRemoveColumn(DataColumn column)
		{
			this.DropReferencedIndexes(column);
		}

		protected virtual void OnRowChanged(DataRowChangeEventArgs e)
		{
			if (this.RowChanged != null)
			{
				this.RowChanged(this, e);
			}
		}

		protected virtual void OnRowChanging(DataRowChangeEventArgs e)
		{
			if (this.RowChanging != null)
			{
				this.RowChanging(this, e);
			}
		}

		protected virtual void OnRowDeleted(DataRowChangeEventArgs e)
		{
			if (this.RowDeleted != null)
			{
				this.RowDeleted(this, e);
			}
		}

		protected virtual void OnRowDeleting(DataRowChangeEventArgs e)
		{
			if (this.RowDeleting != null)
			{
				this.RowDeleting(this, e);
			}
		}

		internal static DataColumn[] ParseSortString(DataTable table, string sort, out ListSortDirection[] sortDirections, bool rejectNoResult)
		{
			DataColumn[] array = DataTable._emptyColumnArray;
			sortDirections = null;
			ArrayList arrayList = null;
			ArrayList arrayList2 = null;
			if (sort != null && !sort.Equals(string.Empty))
			{
				arrayList = new ArrayList();
				arrayList2 = new ArrayList();
				string[] array2 = sort.Trim().Split(new char[] { ',' });
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i].Trim();
					Match match = DataTable.SortRegex.Match(text);
					Group group = match.Groups["ColName"];
					if (!group.Success)
					{
						throw new IndexOutOfRangeException("Could not find column: " + text);
					}
					string value = group.Value;
					DataColumn dataColumn = table.Columns[value];
					if (dataColumn == null)
					{
						try
						{
							dataColumn = table.Columns[int.Parse(value)];
						}
						catch (FormatException)
						{
							throw new IndexOutOfRangeException("Cannot find column " + value);
						}
					}
					arrayList.Add(dataColumn);
					group = match.Groups["Order"];
					if (!group.Success || string.Compare(group.Value, "ASC", true, CultureInfo.InvariantCulture) == 0)
					{
						arrayList2.Add(ListSortDirection.Ascending);
					}
					else
					{
						arrayList2.Add(ListSortDirection.Descending);
					}
				}
				array = (DataColumn[])arrayList.ToArray(typeof(DataColumn));
				sortDirections = new ListSortDirection[arrayList2.Count];
				for (int j = 0; j < sortDirections.Length; j++)
				{
					sortDirections[j] = (ListSortDirection)((int)arrayList2[j]);
				}
			}
			if (rejectNoResult)
			{
				if (array == null)
				{
					throw new SystemException("sort expression result is null");
				}
				if (array.Length == 0)
				{
					throw new SystemException("sort expression result is 0");
				}
			}
			return array;
		}

		private void UpdatePropertyDescriptorsCache()
		{
			PropertyDescriptor[] array = new PropertyDescriptor[this.Columns.Count + this.ChildRelations.Count];
			int num = 0;
			foreach (object obj in this.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				array[num++] = new DataColumnPropertyDescriptor(dataColumn);
			}
			foreach (object obj2 in this.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj2;
				array[num++] = new DataRelationPropertyDescriptor(dataRelation);
			}
			this._propertyDescriptorsCache = new PropertyDescriptorCollection(array);
		}

		internal PropertyDescriptorCollection GetPropertyDescriptorCollection()
		{
			if (this._propertyDescriptorsCache == null)
			{
				this.UpdatePropertyDescriptorsCache();
			}
			return this._propertyDescriptorsCache;
		}

		internal void ResetPropertyDescriptorsCache()
		{
			this._propertyDescriptorsCache = null;
		}

		internal void SetRowsID()
		{
			int num = 0;
			foreach (object obj in this.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				dataRow.XmlRowID = num;
				num++;
			}
		}

		[MonoTODO]
		protected virtual XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		public static XmlSchemaComplexType GetDataTableSchema(XmlSchemaSet schemaSet)
		{
			return new XmlSchemaComplexType();
		}

		public XmlReadMode ReadXml(Stream stream)
		{
			return this.ReadXml(new XmlTextReader(stream, null));
		}

		public XmlReadMode ReadXml(string fileName)
		{
			XmlReader xmlReader = new XmlTextReader(fileName);
			XmlReadMode xmlReadMode;
			try
			{
				xmlReadMode = this.ReadXml(xmlReader);
			}
			finally
			{
				xmlReader.Close();
			}
			return xmlReadMode;
		}

		public XmlReadMode ReadXml(TextReader reader)
		{
			return this.ReadXml(new XmlTextReader(reader));
		}

		public XmlReadMode ReadXml(XmlReader reader)
		{
			return this.ReadXml_internal(reader, false);
		}

		public XmlReadMode ReadXml_internal(XmlReader reader, bool serializable)
		{
			bool flag = true;
			bool flag2 = false;
			XmlReadMode xmlReadMode = XmlReadMode.ReadSchema;
			DataSet dataSet = null;
			DataSet dataSet2 = new DataSet();
			reader.MoveToContent();
			if ((this.Columns.Count > 0 && reader.LocalName != "diffgram") || serializable)
			{
				xmlReadMode = dataSet2.ReadXml(reader);
			}
			else
			{
				if (this.Columns.Count > 0 && reader.LocalName == "diffgram")
				{
					try
					{
						if (this.TableName == string.Empty)
						{
							flag2 = true;
						}
						if (this.DataSet == null)
						{
							flag = false;
							dataSet2.Tables.Add(this);
							xmlReadMode = dataSet2.ReadXml(reader);
						}
						else
						{
							xmlReadMode = this.DataSet.ReadXml(reader);
						}
					}
					catch (DataException)
					{
						xmlReadMode = XmlReadMode.DiffGram;
						if (flag2)
						{
							this.TableName = string.Empty;
						}
					}
					finally
					{
						if (!flag)
						{
							dataSet2.Tables.Remove(this);
						}
					}
					return xmlReadMode;
				}
				xmlReadMode = dataSet2.ReadXml(reader, XmlReadMode.ReadSchema);
			}
			if (xmlReadMode == XmlReadMode.InferSchema)
			{
				xmlReadMode = XmlReadMode.IgnoreSchema;
			}
			if (this.DataSet == null)
			{
				flag = false;
				dataSet = new DataSet();
				if (this.TableName == string.Empty)
				{
					flag2 = true;
				}
				dataSet.Tables.Add(this);
			}
			this.DenyXmlResolving(this, dataSet2, xmlReadMode, flag2, flag);
			if (this.Columns.Count > 0 && this.TableName != dataSet2.Tables[0].TableName)
			{
				if (!flag)
				{
					dataSet.Tables.Remove(this);
				}
				if (flag2 && !flag)
				{
					this.TableName = string.Empty;
				}
				return xmlReadMode;
			}
			this.TableName = dataSet2.Tables[0].TableName;
			if (!flag)
			{
				if (this.Columns.Count > 0)
				{
					dataSet.Merge(dataSet2, true, MissingSchemaAction.Ignore);
				}
				else
				{
					dataSet.Merge(dataSet2, true, MissingSchemaAction.AddWithKey);
				}
				if (this.ChildRelations.Count == 0)
				{
					dataSet.Tables.Remove(this);
				}
				else
				{
					dataSet.DataSetName = dataSet2.DataSetName;
				}
			}
			else if (this.Columns.Count > 0)
			{
				this.DataSet.Merge(dataSet2, true, MissingSchemaAction.Ignore);
			}
			else
			{
				this.DataSet.Merge(dataSet2, true, MissingSchemaAction.AddWithKey);
			}
			return xmlReadMode;
		}

		private void DenyXmlResolving(DataTable table, DataSet ds, XmlReadMode mode, bool isTableNameBlank, bool isPartOfDataSet)
		{
			if (ds.Tables.Count == 0 && table.Columns.Count == 0)
			{
				throw new InvalidOperationException("DataTable does not support schema inference from XML");
			}
			if (table.Columns.Count == 0 && ds.Tables[0].TableName != table.TableName && !isTableNameBlank)
			{
				throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source", table.TableName));
			}
			if (table.Columns.Count > 0 && ds.Tables[0].TableName != table.TableName && !isTableNameBlank && mode == XmlReadMode.ReadSchema && !isPartOfDataSet)
			{
				throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source", table.TableName));
			}
			if (isPartOfDataSet && table.Columns.Count > 0 && mode == XmlReadMode.ReadSchema && table.TableName != ds.Tables[0].TableName)
			{
				throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source", table.TableName));
			}
		}

		public void ReadXmlSchema(Stream stream)
		{
			this.ReadXmlSchema(new XmlTextReader(stream));
		}

		public void ReadXmlSchema(TextReader reader)
		{
			this.ReadXmlSchema(new XmlTextReader(reader));
		}

		public void ReadXmlSchema(string fileName)
		{
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(fileName);
				this.ReadXmlSchema(xmlTextReader);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		public void ReadXmlSchema(XmlReader reader)
		{
			if (this.Columns.Count > 0)
			{
				return;
			}
			DataSet dataSet = new DataSet();
			new XmlSchemaDataImporter(dataSet, reader, false).Process();
			DataTable dataTable = null;
			if (this.TableName == string.Empty)
			{
				if (dataSet.Tables.Count > 0)
				{
					dataTable = dataSet.Tables[0];
				}
			}
			else
			{
				dataTable = dataSet.Tables[this.TableName];
				if (dataTable == null)
				{
					throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source.", this.TableName));
				}
			}
			if (dataTable != null)
			{
				dataTable.CopyProperties(this);
			}
		}

		[MonoNotSupported("")]
		protected virtual void ReadXmlSerializable(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		private XmlWriterSettings GetWriterSettings()
		{
			return new XmlWriterSettings
			{
				Indent = true,
				OmitXmlDeclaration = true
			};
		}

		public void WriteXml(Stream stream)
		{
			this.WriteXml(stream, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(TextWriter writer)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(XmlWriter writer)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(string fileName)
		{
			this.WriteXml(fileName, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(Stream stream, XmlWriteMode mode)
		{
			this.WriteXml(stream, mode, false);
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode)
		{
			this.WriteXml(writer, mode, false);
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode)
		{
			this.WriteXml(writer, mode, false);
		}

		public void WriteXml(string fileName, XmlWriteMode mode)
		{
			this.WriteXml(fileName, mode, false);
		}

		public void WriteXml(Stream stream, bool writeHierarchy)
		{
			this.WriteXml(stream, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(string fileName, bool writeHierarchy)
		{
			this.WriteXml(fileName, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(TextWriter writer, bool writeHierarchy)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(XmlWriter writer, bool writeHierarchy)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(Stream stream, XmlWriteMode mode, bool writeHierarchy)
		{
			this.WriteXml(XmlWriter.Create(stream, this.GetWriterSettings()), mode, writeHierarchy);
		}

		public void WriteXml(string fileName, XmlWriteMode mode, bool writeHierarchy)
		{
			XmlWriter xmlWriter = null;
			try
			{
				xmlWriter = XmlWriter.Create(fileName, this.GetWriterSettings());
				this.WriteXml(xmlWriter, mode, writeHierarchy);
			}
			finally
			{
				if (xmlWriter != null)
				{
					xmlWriter.Close();
				}
			}
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
			this.WriteXml(XmlWriter.Create(writer, this.GetWriterSettings()), mode, writeHierarchy);
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
			List<DataTable> list = new List<DataTable>();
			if (!writeHierarchy)
			{
				list.Add(this);
			}
			else
			{
				this.FindAllChildren(list, this);
			}
			List<DataRelation> list2 = new List<DataRelation>();
			if (this.DataSet != null)
			{
				foreach (object obj in this.DataSet.Relations)
				{
					DataRelation dataRelation = (DataRelation)obj;
					if (list.Contains(dataRelation.ParentTable) && list.Contains(dataRelation.ChildTable))
					{
						list2.Add(dataRelation);
					}
				}
			}
			string text = null;
			if (mode == XmlWriteMode.WriteSchema)
			{
				text = this.TableName;
			}
			string text2;
			if (this.DataSet != null)
			{
				text2 = this.DataSet.DataSetName;
			}
			else if (this.DataSet == null && mode == XmlWriteMode.WriteSchema)
			{
				text2 = "NewDataSet";
			}
			else
			{
				text2 = "DocumentElement";
			}
			XmlTableWriter.WriteTables(writer, mode, list, list2, text, text2);
		}

		private void FindAllChildren(List<DataTable> list, DataTable root)
		{
			if (!list.Contains(root))
			{
				list.Add(root);
				foreach (object obj in root.ChildRelations)
				{
					DataRelation dataRelation = (DataRelation)obj;
					this.FindAllChildren(list, dataRelation.ChildTable);
				}
			}
		}

		public void WriteXmlSchema(Stream stream)
		{
			if (this.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = this.GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			this.WriteXmlSchema(XmlWriter.Create(stream, writerSettings));
		}

		public void WriteXmlSchema(TextWriter writer)
		{
			if (this.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = this.GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			this.WriteXmlSchema(XmlWriter.Create(writer, writerSettings));
		}

		public void WriteXmlSchema(XmlWriter writer)
		{
			this.WriteXmlSchema(writer, false);
		}

		public void WriteXmlSchema(string fileName)
		{
			if (fileName == string.Empty)
			{
				throw new ArgumentException("Empty path name is not legal.");
			}
			if (this.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlTextWriter xmlTextWriter = null;
			try
			{
				XmlWriterSettings writerSettings = this.GetWriterSettings();
				writerSettings.OmitXmlDeclaration = false;
				xmlTextWriter = new XmlTextWriter(fileName, null);
				this.WriteXmlSchema(xmlTextWriter);
			}
			finally
			{
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
			}
		}

		public void WriteXmlSchema(Stream stream, bool writeHierarchy)
		{
			if (this.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = this.GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			this.WriteXmlSchema(XmlWriter.Create(stream, writerSettings), writeHierarchy);
		}

		public void WriteXmlSchema(TextWriter writer, bool writeHierarchy)
		{
			if (this.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = this.GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			this.WriteXmlSchema(XmlWriter.Create(writer, writerSettings), writeHierarchy);
		}

		public void WriteXmlSchema(XmlWriter writer, bool writeHierarchy)
		{
			if (this.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			DataSet dataSet = this.DataSet;
			DataSet dataSet2 = null;
			try
			{
				if (dataSet == null)
				{
					dataSet = (dataSet2 = new DataSet());
					dataSet.Tables.Add(this);
				}
				writer.WriteStartDocument();
				DataRelation[] array = null;
				DataTable[] array2;
				if (writeHierarchy && this.ChildRelations.Count > 0)
				{
					array = new DataRelation[this.ChildRelations.Count];
					for (int i = 0; i < this.ChildRelations.Count; i++)
					{
						array[i] = this.ChildRelations[i];
					}
					array2 = new DataTable[dataSet.Tables.Count];
					for (int j = 0; j < dataSet.Tables.Count; j++)
					{
						array2[j] = dataSet.Tables[j];
					}
				}
				else
				{
					array2 = new DataTable[] { this };
				}
				string text;
				if (dataSet.Namespace == string.Empty)
				{
					text = this.TableName;
				}
				else
				{
					text = dataSet.Namespace + "_x003A_" + this.TableName;
				}
				XmlSchemaWriter.WriteXmlSchema(writer, array2, array, text, dataSet.DataSetName, (!this.LocaleSpecified) ? ((!dataSet.LocaleSpecified) ? null : dataSet.Locale) : this.Locale);
			}
			finally
			{
				if (dataSet2 != null)
				{
					dataSet.Tables.Remove(this);
				}
			}
		}

		public void WriteXmlSchema(string fileName, bool writeHierarchy)
		{
			if (fileName == string.Empty)
			{
				throw new ArgumentException("Empty path name is not legal.");
			}
			if (this.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlTextWriter xmlTextWriter = null;
			try
			{
				XmlWriterSettings writerSettings = this.GetWriterSettings();
				writerSettings.OmitXmlDeclaration = false;
				xmlTextWriter = new XmlTextWriter(fileName, null);
				this.WriteXmlSchema(xmlTextWriter, writeHierarchy);
			}
			finally
			{
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return this.tableInitialized;
			}
		}

		private void OnTableInitialized(EventArgs e)
		{
			if (this.Initialized != null)
			{
				this.Initialized(this, e);
			}
		}

		private void DataTableInitialized()
		{
			this.tableInitialized = true;
			this.OnTableInitialized(new EventArgs());
		}

		[DefaultValue(SerializationFormat.Xml)]
		public SerializationFormat RemotingFormat
		{
			get
			{
				if (this.dataSet != null)
				{
					this.remotingFormat = this.dataSet.RemotingFormat;
				}
				return this.remotingFormat;
			}
			set
			{
				if (this.dataSet != null)
				{
					throw new ArgumentException("Cannot have different remoting format property value for DataSet and DataTable");
				}
				this.remotingFormat = value;
			}
		}

		internal void DeserializeConstraints(ArrayList arrayList)
		{
			bool flag = false;
			for (int i = 0; i < arrayList.Count; i++)
			{
				ArrayList arrayList2 = arrayList[i] as ArrayList;
				if (arrayList2 != null)
				{
					if ((string)arrayList2[0] == "F")
					{
						int[] array = arrayList2[2] as int[];
						if (array != null)
						{
							ArrayList arrayList3 = new ArrayList();
							DataTable dataTable = this.dataSet.Tables[array[0]];
							for (int j = 0; j < array.Length - 1; j++)
							{
								arrayList3.Add(dataTable.Columns[array[j + 1]]);
							}
							array = arrayList2[3] as int[];
							if (array != null)
							{
								ArrayList arrayList4 = new ArrayList();
								dataTable = this.dataSet.Tables[array[0]];
								for (int k = 0; k < array.Length - 1; k++)
								{
									arrayList4.Add(dataTable.Columns[array[k + 1]]);
								}
								ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint((string)arrayList2[1], (DataColumn[])arrayList3.ToArray(typeof(DataColumn)), (DataColumn[])arrayList4.ToArray(typeof(DataColumn)));
								Array array2 = (Array)arrayList2[4];
								foreignKeyConstraint.AcceptRejectRule = (AcceptRejectRule)((int)array2.GetValue(0));
								foreignKeyConstraint.UpdateRule = (Rule)((int)array2.GetValue(1));
								foreignKeyConstraint.DeleteRule = (Rule)((int)array2.GetValue(2));
								foreignKeyConstraint.SetExtendedProperties((PropertyCollection)arrayList2[5]);
								this.Constraints.Add(foreignKeyConstraint);
								flag = true;
							}
						}
					}
					else if (!flag && (string)arrayList2[0] == "U")
					{
						ArrayList arrayList5 = new ArrayList();
						int[] array3 = arrayList2[2] as int[];
						if (array3 != null)
						{
							for (int l = 0; l < array3.Length; l++)
							{
								arrayList5.Add(this.Columns[array3[l]]);
							}
							UniqueConstraint uniqueConstraint = new UniqueConstraint((string)arrayList2[1], (DataColumn[])arrayList5.ToArray(typeof(DataColumn)), (bool)arrayList2[3]);
							if (this.Constraints.IndexOf(uniqueConstraint) == -1 && this.Constraints.IndexOf((string)arrayList2[1]) == -1)
							{
								uniqueConstraint.SetExtendedProperties((PropertyCollection)arrayList2[4]);
								this.Constraints.Add(uniqueConstraint);
							}
						}
					}
					else
					{
						flag = false;
					}
				}
			}
		}

		private DataRowState GetCurrentRowState(BitArray rowStateBitArray, int i)
		{
			DataRowState dataRowState;
			if (!rowStateBitArray[i] && !rowStateBitArray[i + 1] && rowStateBitArray[i + 2])
			{
				dataRowState = DataRowState.Detached;
			}
			else if (!rowStateBitArray[i] && !rowStateBitArray[i + 1] && !rowStateBitArray[i + 2])
			{
				dataRowState = DataRowState.Unchanged;
			}
			else if (!rowStateBitArray[i] && rowStateBitArray[i + 1] && !rowStateBitArray[i + 2])
			{
				dataRowState = DataRowState.Added;
			}
			else if (rowStateBitArray[i] && rowStateBitArray[i + 1] && !rowStateBitArray[i + 2])
			{
				dataRowState = DataRowState.Deleted;
			}
			else
			{
				dataRowState = DataRowState.Modified;
			}
			return dataRowState;
		}

		internal void DeserializeRecords(ArrayList arrayList, ArrayList nullBits, BitArray rowStateBitArray)
		{
			if (arrayList == null || arrayList.Count < 1)
			{
				return;
			}
			int length = ((Array)arrayList[0]).Length;
			object[] array = new object[arrayList.Count];
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				DataRowState currentRowState = this.GetCurrentRowState(rowStateBitArray, num * 3);
				for (int j = 0; j < arrayList.Count; j++)
				{
					Array array2 = (Array)arrayList[j];
					BitArray bitArray = (BitArray)nullBits[j];
					if (!bitArray[i])
					{
						array[j] = array2.GetValue(i);
					}
					else
					{
						array[j] = null;
					}
				}
				this.LoadDataRow(array, false);
				if (currentRowState == DataRowState.Modified)
				{
					this.Rows[num].AcceptChanges();
					i++;
					for (int k = 0; k < arrayList.Count; k++)
					{
						Array array3 = (Array)arrayList[k];
						BitArray bitArray = (BitArray)nullBits[k];
						if (!bitArray[i])
						{
							this.Rows[num][k] = array3.GetValue(i);
						}
						else
						{
							this.Rows[num][k] = null;
						}
					}
				}
				else if (currentRowState == DataRowState.Unchanged)
				{
					this.Rows[num].AcceptChanges();
				}
				else if (currentRowState == DataRowState.Deleted)
				{
					this.Rows[num].AcceptChanges();
					this.Rows[num].Delete();
				}
				num++;
			}
		}

		private void BinaryDeserializeTable(SerializationInfo info)
		{
			this.TableName = info.GetString("DataTable.TableName");
			this.Namespace = info.GetString("DataTable.Namespace");
			this.Prefix = info.GetString("DataTable.Prefix");
			this.CaseSensitive = info.GetBoolean("DataTable.CaseSensitive");
			this.Locale = new CultureInfo(info.GetInt32("DataTable.LocaleLCID"));
			this._extendedProperties = (PropertyCollection)info.GetValue("DataTable.ExtendedProperties", typeof(PropertyCollection));
			this.MinimumCapacity = info.GetInt32("DataTable.MinimumCapacity");
			int @int = info.GetInt32("DataTable.Columns.Count");
			for (int i = 0; i < @int; i++)
			{
				this.Columns.Add();
				string text = "DataTable.DataColumn_" + i + ".";
				this.Columns[i].ColumnName = info.GetString(text + "ColumnName");
				this.Columns[i].Namespace = info.GetString(text + "Namespace");
				this.Columns[i].Caption = info.GetString(text + "Caption");
				this.Columns[i].Prefix = info.GetString(text + "Prefix");
				this.Columns[i].DataType = (Type)info.GetValue(text + "DataType", typeof(Type));
				this.Columns[i].DefaultValue = info.GetValue(text + "DefaultValue", typeof(object));
				this.Columns[i].AllowDBNull = info.GetBoolean(text + "AllowDBNull");
				this.Columns[i].AutoIncrement = info.GetBoolean(text + "AutoIncrement");
				this.Columns[i].AutoIncrementStep = info.GetInt64(text + "AutoIncrementStep");
				this.Columns[i].AutoIncrementSeed = info.GetInt64(text + "AutoIncrementSeed");
				this.Columns[i].ReadOnly = info.GetBoolean(text + "ReadOnly");
				this.Columns[i].MaxLength = info.GetInt32(text + "MaxLength");
				this.Columns[i].ExtendedProperties = (PropertyCollection)info.GetValue(text + "ExtendedProperties", typeof(PropertyCollection));
				if (this.Columns[i].DataType == typeof(DataSetDateTime))
				{
					this.Columns[i].DateTimeMode = (DataSetDateTime)((int)info.GetValue(text + "DateTimeMode", typeof(DataSetDateTime)));
				}
				this.Columns[i].ColumnMapping = (MappingType)((int)info.GetValue(text + "ColumnMapping", typeof(MappingType)));
				try
				{
					this.Columns[i].Expression = info.GetString(text + "Expression");
					text = "DataTable_0.";
					ArrayList arrayList = (ArrayList)info.GetValue(text + "Constraints", typeof(ArrayList));
					if (this.Constraints == null)
					{
						this.Constraints = new ConstraintCollection(this);
					}
					this.DeserializeConstraints(arrayList);
				}
				catch (SerializationException)
				{
				}
			}
			try
			{
				string text2 = "DataTable_0.";
				ArrayList arrayList2 = (ArrayList)info.GetValue(text2 + "NullBits", typeof(ArrayList));
				ArrayList arrayList = (ArrayList)info.GetValue(text2 + "Records", typeof(ArrayList));
				BitArray bitArray = (BitArray)info.GetValue(text2 + "RowStates", typeof(BitArray));
				Hashtable hashtable = (Hashtable)info.GetValue(text2 + "RowErrors", typeof(Hashtable));
				this.DeserializeRecords(arrayList, arrayList2, bitArray);
			}
			catch (SerializationException)
			{
			}
		}

		internal void BinarySerializeProperty(SerializationInfo info)
		{
			Version version = new Version(2, 0);
			info.AddValue("DataTable.RemotingVersion", version);
			info.AddValue("DataTable.RemotingFormat", this.RemotingFormat);
			info.AddValue("DataTable.TableName", this.TableName);
			info.AddValue("DataTable.Namespace", this.Namespace);
			info.AddValue("DataTable.Prefix", this.Prefix);
			info.AddValue("DataTable.CaseSensitive", this.CaseSensitive);
			info.AddValue("DataTable.caseSensitiveAmbient", true);
			info.AddValue("DataTable.NestedInDataSet", true);
			info.AddValue("DataTable.RepeatableElement", false);
			info.AddValue("DataTable.LocaleLCID", this.Locale.LCID);
			info.AddValue("DataTable.MinimumCapacity", this.MinimumCapacity);
			info.AddValue("DataTable.Columns.Count", this.Columns.Count);
			info.AddValue("DataTable.ExtendedProperties", this._extendedProperties);
			for (int i = 0; i < this.Columns.Count; i++)
			{
				info.AddValue("DataTable.DataColumn_" + i + ".ColumnName", this.Columns[i].ColumnName);
				info.AddValue("DataTable.DataColumn_" + i + ".Namespace", this.Columns[i].Namespace);
				info.AddValue("DataTable.DataColumn_" + i + ".Caption", this.Columns[i].Caption);
				info.AddValue("DataTable.DataColumn_" + i + ".Prefix", this.Columns[i].Prefix);
				info.AddValue("DataTable.DataColumn_" + i + ".DataType", this.Columns[i].DataType, typeof(Type));
				info.AddValue("DataTable.DataColumn_" + i + ".DefaultValue", this.Columns[i].DefaultValue, typeof(DBNull));
				info.AddValue("DataTable.DataColumn_" + i + ".AllowDBNull", this.Columns[i].AllowDBNull);
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrement", this.Columns[i].AutoIncrement);
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrementStep", this.Columns[i].AutoIncrementStep);
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrementSeed", this.Columns[i].AutoIncrementSeed);
				info.AddValue("DataTable.DataColumn_" + i + ".ReadOnly", this.Columns[i].ReadOnly);
				info.AddValue("DataTable.DataColumn_" + i + ".MaxLength", this.Columns[i].MaxLength);
				info.AddValue("DataTable.DataColumn_" + i + ".ExtendedProperties", this.Columns[i].ExtendedProperties);
				info.AddValue("DataTable.DataColumn_" + i + ".DateTimeMode", this.Columns[i].DateTimeMode);
				info.AddValue("DataTable.DataColumn_" + i + ".ColumnMapping", this.Columns[i].ColumnMapping, typeof(MappingType));
				info.AddValue("DataTable.DataColumn_" + i + ".SimpleType", null, typeof(string));
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrementCurrent", this.Columns[i].AutoIncrementValue());
				info.AddValue("DataTable.DataColumn_" + i + ".XmlDataType", null, typeof(string));
			}
			info.AddValue("DataTable.TypeName", null, typeof(string));
		}

		internal void SerializeConstraints(SerializationInfo info, string prefix)
		{
			ArrayList arrayList = new ArrayList();
			int i = 0;
			while (i < this.Constraints.Count)
			{
				ArrayList arrayList2 = new ArrayList();
				if (this.Constraints[i] is UniqueConstraint)
				{
					arrayList2.Add("U");
					UniqueConstraint uniqueConstraint = (UniqueConstraint)this.Constraints[i];
					arrayList2.Add(uniqueConstraint.ConstraintName);
					DataColumn[] columns = uniqueConstraint.Columns;
					int[] array = new int[columns.Length];
					for (int j = 0; j < columns.Length; j++)
					{
						array[j] = uniqueConstraint.Table.Columns.IndexOf(uniqueConstraint.Columns[j]);
					}
					arrayList2.Add(array);
					arrayList2.Add(uniqueConstraint.IsPrimaryKey);
					arrayList2.Add(uniqueConstraint.ExtendedProperties);
					goto IL_022C;
				}
				if (this.Constraints[i] is ForeignKeyConstraint)
				{
					arrayList2.Add("F");
					ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)this.Constraints[i];
					arrayList2.Add(foreignKeyConstraint.ConstraintName);
					int[] array2 = new int[foreignKeyConstraint.RelatedColumns.Length + 1];
					array2[0] = this.DataSet.Tables.IndexOf(foreignKeyConstraint.RelatedTable);
					for (int k = 0; k < foreignKeyConstraint.Columns.Length; k++)
					{
						array2[k + 1] = foreignKeyConstraint.RelatedColumns[k].Ordinal;
					}
					arrayList2.Add(array2);
					array2 = new int[foreignKeyConstraint.Columns.Length + 1];
					array2[0] = this.DataSet.Tables.IndexOf(foreignKeyConstraint.Table);
					for (int l = 0; l < foreignKeyConstraint.Columns.Length; l++)
					{
						array2[l + 1] = foreignKeyConstraint.Columns[l].Ordinal;
					}
					arrayList2.Add(array2);
					arrayList2.Add(new int[]
					{
						(int)foreignKeyConstraint.AcceptRejectRule,
						(int)foreignKeyConstraint.UpdateRule,
						(int)foreignKeyConstraint.DeleteRule
					});
					arrayList2.Add(foreignKeyConstraint.ExtendedProperties);
					goto IL_022C;
				}
				IL_0234:
				i++;
				continue;
				IL_022C:
				arrayList.Add(arrayList2);
				goto IL_0234;
			}
			info.AddValue(prefix, arrayList, typeof(ArrayList));
		}

		internal void BinarySerialize(SerializationInfo info, string prefix)
		{
			int count = this.Columns.Count;
			int count2 = this.Rows.Count;
			int num = this.Rows.Count;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			BitArray bitArray = new BitArray(count2 * 3);
			for (int i = 0; i < this.Rows.Count; i++)
			{
				if (this.Rows[i].RowState == DataRowState.Modified)
				{
					num++;
				}
			}
			this.SerializeConstraints(info, prefix + "Constraints");
			for (int j = 0; j < count; j++)
			{
				if (count2 != 0)
				{
					BitArray bitArray2 = new BitArray(count2);
					DataColumn dataColumn = this.Columns[j];
					Array array = Array.CreateInstance(dataColumn.DataType, num);
					int k = 0;
					int num2 = 0;
					while (k < this.Rows.Count)
					{
						DataRow dataRow = this.Rows[k];
						DataRowVersion dataRowVersion;
						if (dataRow.RowState == DataRowState.Modified)
						{
							dataRowVersion = DataRowVersion.Default;
							bitArray2.Length++;
							if (!dataRow.IsNull(dataColumn, dataRowVersion))
							{
								bitArray2[num2] = false;
								array.SetValue(dataRow[j, dataRowVersion], num2);
							}
							else
							{
								bitArray2[num2] = true;
							}
							num2++;
							dataRowVersion = DataRowVersion.Current;
						}
						else if (dataRow.RowState == DataRowState.Deleted)
						{
							dataRowVersion = DataRowVersion.Original;
						}
						else
						{
							dataRowVersion = DataRowVersion.Default;
						}
						if (!dataRow.IsNull(dataColumn, dataRowVersion))
						{
							bitArray2[num2] = false;
							array.SetValue(dataRow[j, dataRowVersion], num2);
						}
						else
						{
							bitArray2[num2] = true;
						}
						k++;
						num2++;
					}
					arrayList.Add(array);
					arrayList2.Add(bitArray2);
				}
			}
			for (int l = 0; l < this.Rows.Count; l++)
			{
				int num3 = l * 3;
				DataRowState rowState = this.Rows[l].RowState;
				if (rowState == DataRowState.Detached)
				{
					bitArray[num3] = false;
					bitArray[num3 + 1] = false;
					bitArray[num3 + 2] = true;
				}
				else if (rowState == DataRowState.Unchanged)
				{
					bitArray[num3] = false;
					bitArray[num3 + 1] = false;
					bitArray[num3 + 2] = false;
				}
				else if (rowState == DataRowState.Added)
				{
					bitArray[num3] = false;
					bitArray[num3 + 1] = true;
					bitArray[num3 + 2] = false;
				}
				else if (rowState == DataRowState.Deleted)
				{
					bitArray[num3] = true;
					bitArray[num3 + 1] = true;
					bitArray[num3 + 2] = false;
				}
				else
				{
					bitArray[num3] = true;
					bitArray[num3 + 1] = false;
					bitArray[num3 + 2] = false;
				}
			}
			info.AddValue(prefix + "Rows.Count", this.Rows.Count);
			info.AddValue(prefix + "Records.Count", num);
			info.AddValue(prefix + "Records", arrayList, typeof(ArrayList));
			info.AddValue(prefix + "NullBits", arrayList2, typeof(ArrayList));
			info.AddValue(prefix + "RowStates", bitArray, typeof(BitArray));
			Hashtable hashtable = new Hashtable();
			info.AddValue(prefix + "RowErrors", hashtable, typeof(Hashtable));
			Hashtable hashtable2 = new Hashtable();
			info.AddValue(prefix + "ColumnErrors", hashtable2, typeof(Hashtable));
		}

		public DataTableReader CreateDataReader()
		{
			return new DataTableReader(this);
		}

		public void Load(IDataReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			this.Load(reader, LoadOption.PreserveChanges);
		}

		public void Load(IDataReader reader, LoadOption loadOption)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			bool flag = this.EnforceConstraints;
			try
			{
				this.EnforceConstraints = false;
				int[] array = DataAdapter.BuildSchema(reader, this, SchemaType.Mapped, MissingSchemaAction.AddWithKey, MissingMappingAction.Passthrough, new DataTableMappingCollection());
				DbDataAdapter.FillFromReader(this, reader, 0, 0, array, loadOption);
			}
			finally
			{
				this.EnforceConstraints = flag;
			}
		}

		public virtual void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			bool flag = this.EnforceConstraints;
			try
			{
				this.EnforceConstraints = false;
				int[] array = DataAdapter.BuildSchema(reader, this, SchemaType.Mapped, MissingSchemaAction.AddWithKey, MissingMappingAction.Passthrough, new DataTableMappingCollection());
				DbDataAdapter.FillFromReader(this, reader, 0, 0, array, loadOption, errorHandler);
			}
			finally
			{
				this.EnforceConstraints = flag;
			}
		}

		public DataRow LoadDataRow(object[] values, LoadOption loadOption)
		{
			DataRow dataRow = null;
			if (this.PrimaryKey.Length > 0)
			{
				object[] array = new object[this.PrimaryKey.Length];
				for (int i = 0; i < this.PrimaryKey.Length; i++)
				{
					array[i] = values[this.PrimaryKey[i].Ordinal];
				}
				dataRow = this.Rows.Find(array, DataViewRowState.OriginalRows);
				if (dataRow == null)
				{
					dataRow = this.Rows.Find(array);
				}
			}
			if (dataRow == null || (dataRow.RowState == DataRowState.Deleted && loadOption == LoadOption.Upsert))
			{
				dataRow = this.NewNotInitializedRow();
				dataRow.ImportRecord(this.CreateRecord(values));
				dataRow.Validate();
				if (loadOption == LoadOption.OverwriteChanges || loadOption == LoadOption.PreserveChanges)
				{
					this.Rows.AddInternal(dataRow, DataRowAction.ChangeCurrentAndOriginal);
				}
				else
				{
					this.Rows.AddInternal(dataRow);
				}
				return dataRow;
			}
			dataRow.Load(values, loadOption);
			return dataRow;
		}

		public void Merge(DataTable table)
		{
			this.Merge(table, false, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table, bool preserveChanges)
		{
			this.Merge(table, preserveChanges, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			MergeManager.Merge(this, table, preserveChanges, missingSchemaAction);
		}

		internal int CompareRecords(int x, int y)
		{
			for (int i = 0; i < this.Columns.Count; i++)
			{
				int num = this.Columns[i].DataContainer.CompareValues(x, y);
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}

		protected virtual void OnTableCleared(DataTableClearEventArgs e)
		{
			if (this.TableCleared != null)
			{
				this.TableCleared(this, e);
			}
		}

		internal void DataTableCleared()
		{
			this.OnTableCleared(new DataTableClearEventArgs(this));
		}

		protected virtual void OnTableClearing(DataTableClearEventArgs e)
		{
			if (this.TableClearing != null)
			{
				this.TableClearing(this, e);
			}
		}

		internal void DataTableClearing()
		{
			this.OnTableClearing(new DataTableClearEventArgs(this));
		}

		protected virtual void OnTableNewRow(DataTableNewRowEventArgs e)
		{
			if (this.TableNewRow != null)
			{
				this.TableNewRow(this, e);
			}
		}

		private void NewRowAdded(DataRow dr)
		{
			this.OnTableNewRow(new DataTableNewRowEventArgs(dr));
		}

		internal DataSet dataSet;

		private bool _caseSensitive;

		private DataColumnCollection _columnCollection;

		private ConstraintCollection _constraintCollection;

		private DataView _defaultView;

		private string _displayExpression;

		private PropertyCollection _extendedProperties;

		private bool _hasErrors;

		private CultureInfo _locale;

		private int _minimumCapacity;

		private string _nameSpace;

		private DataRelationCollection _childRelations;

		private DataRelationCollection _parentRelations;

		private string _prefix;

		private UniqueConstraint _primaryKeyConstraint;

		private DataRowCollection _rows;

		private ISite _site;

		private string _tableName;

		private bool _containsListCollection;

		private string _encodedTableName;

		internal bool _duringDataLoad;

		internal bool _nullConstraintViolationDuringDataLoad;

		private bool dataSetPrevEnforceConstraints;

		private bool dataTablePrevEnforceConstraints;

		private bool enforceConstraints = true;

		private DataRowBuilder _rowBuilder;

		private ArrayList _indexes;

		private RecordCache _recordCache;

		private int _defaultValuesRowIndex = -1;

		protected internal bool fInitInProgress;

		private bool _virginCaseSensitive = true;

		private PropertyDescriptorCollection _propertyDescriptorsCache;

		private static DataColumn[] _emptyColumnArray = new DataColumn[0];

		private static Regex SortRegex = new Regex("^((\\[(?<ColName>.+)\\])|(?<ColName>\\S+))([ ]+(?<Order>ASC|DESC))?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

		private DataColumn[] _latestPrimaryKeyCols;

		private DataRow[] empty_rows;

		private bool tableInitialized = true;

		private SerializationFormat remotingFormat;
	}
}
