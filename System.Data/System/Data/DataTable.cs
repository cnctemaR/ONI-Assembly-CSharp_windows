using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data
{
	[DefaultEvent("RowChanging")]
	[DefaultProperty("TableName")]
	[DesignTimeVisible(false)]
	[Editor("Microsoft.VSDesigner.Data.Design.DataTableEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ToolboxItem(false)]
	[XmlSchemaProvider("GetDataTableSchema")]
	[Serializable]
	public class DataTable : MarshalByValueComponent, IListSource, ISupportInitialize, ISupportInitializeNotification, ISerializable, IXmlSerializable
	{
		public DataTable()
		{
		}

		protected DataTable(SerializationInfo info, StreamingContext context)
		{
		}

		public DataTable(string tableName)
		{
		}

		public DataTable(string tableName, string tableNamespace)
		{
		}

		public bool CaseSensitive
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRelationCollection ChildRelations
		{
			get
			{
				throw null;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataColumnCollection Columns
		{
			get
			{
				throw null;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ConstraintCollection Constraints
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataSet DataSet
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public DataView DefaultView
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue("")]
		public string DisplayExpression
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				throw null;
			}
		}

		public CultureInfo Locale
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(50)]
		public int MinimumCapacity
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string Namespace
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRelationCollection ParentRelations
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.PrimaryKeyEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[TypeConverter("System.Data.PrimaryKeyTypeConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public DataColumn[] PrimaryKey
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(SerializationFormat.Xml)]
		public SerializationFormat RemotingFormat
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public DataRowCollection Rows
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		bool IListSource.ContainsListCollection
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public string TableName
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public event DataColumnChangeEventHandler ColumnChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataColumnChangeEventHandler ColumnChanging
		{
			add
			{
			}
			remove
			{
			}
		}

		public event EventHandler Initialized
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataRowChangeEventHandler RowChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataRowChangeEventHandler RowChanging
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataRowChangeEventHandler RowDeleted
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataRowChangeEventHandler RowDeleting
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataTableClearEventHandler TableCleared
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataTableClearEventHandler TableClearing
		{
			add
			{
			}
			remove
			{
			}
		}

		public event DataTableNewRowEventHandler TableNewRow
		{
			add
			{
			}
			remove
			{
			}
		}

		public void AcceptChanges()
		{
		}

		public virtual void BeginInit()
		{
		}

		public void BeginLoadData()
		{
		}

		public void Clear()
		{
		}

		public virtual DataTable Clone()
		{
			throw null;
		}

		public object Compute(string expression, string filter)
		{
			throw null;
		}

		public DataTable Copy()
		{
			throw null;
		}

		public DataTableReader CreateDataReader()
		{
			throw null;
		}

		protected virtual DataTable CreateInstance()
		{
			throw null;
		}

		public virtual void EndInit()
		{
		}

		public void EndLoadData()
		{
		}

		public DataTable GetChanges()
		{
			throw null;
		}

		public DataTable GetChanges(DataRowState rowStates)
		{
			throw null;
		}

		public static XmlSchemaComplexType GetDataTableSchema(XmlSchemaSet schemaSet)
		{
			throw null;
		}

		public DataRow[] GetErrors()
		{
			throw null;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		protected virtual Type GetRowType()
		{
			throw null;
		}

		[MonoTODO]
		protected virtual XmlSchema GetSchema()
		{
			throw null;
		}

		public void ImportRow(DataRow row)
		{
		}

		public void Load(IDataReader reader)
		{
		}

		public void Load(IDataReader reader, LoadOption loadOption)
		{
		}

		public virtual void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
		{
		}

		public DataRow LoadDataRow(object[] values, bool fAcceptChanges)
		{
			throw null;
		}

		public DataRow LoadDataRow(object[] values, LoadOption loadOption)
		{
			throw null;
		}

		public void Merge(DataTable table)
		{
		}

		public void Merge(DataTable table, bool preserveChanges)
		{
		}

		public void Merge(DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
		}

		public DataRow NewRow()
		{
			throw null;
		}

		protected internal DataRow[] NewRowArray(int size)
		{
			throw null;
		}

		protected virtual DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			throw null;
		}

		protected virtual void OnColumnChanged(DataColumnChangeEventArgs e)
		{
		}

		protected virtual void OnColumnChanging(DataColumnChangeEventArgs e)
		{
		}

		[MonoTODO]
		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
		}

		protected internal virtual void OnRemoveColumn(DataColumn column)
		{
		}

		protected virtual void OnRowChanged(DataRowChangeEventArgs e)
		{
		}

		protected virtual void OnRowChanging(DataRowChangeEventArgs e)
		{
		}

		protected virtual void OnRowDeleted(DataRowChangeEventArgs e)
		{
		}

		protected virtual void OnRowDeleting(DataRowChangeEventArgs e)
		{
		}

		protected virtual void OnTableCleared(DataTableClearEventArgs e)
		{
		}

		protected virtual void OnTableClearing(DataTableClearEventArgs e)
		{
		}

		protected virtual void OnTableNewRow(DataTableNewRowEventArgs e)
		{
		}

		public XmlReadMode ReadXml(Stream stream)
		{
			throw null;
		}

		public XmlReadMode ReadXml(TextReader reader)
		{
			throw null;
		}

		public XmlReadMode ReadXml(string fileName)
		{
			throw null;
		}

		public XmlReadMode ReadXml(XmlReader reader)
		{
			throw null;
		}

		public void ReadXmlSchema(Stream stream)
		{
		}

		public void ReadXmlSchema(TextReader reader)
		{
		}

		public void ReadXmlSchema(string fileName)
		{
		}

		public void ReadXmlSchema(XmlReader reader)
		{
		}

		[MonoNotSupported("")]
		protected virtual void ReadXmlSerializable(XmlReader reader)
		{
		}

		public XmlReadMode ReadXml_internal(XmlReader reader, bool serializable)
		{
			throw null;
		}

		public void RejectChanges()
		{
		}

		public virtual void Reset()
		{
		}

		public DataRow[] Select()
		{
			throw null;
		}

		public DataRow[] Select(string filterExpression)
		{
			throw null;
		}

		public DataRow[] Select(string filterExpression, string sort)
		{
			throw null;
		}

		public DataRow[] Select(string filterExpression, string sort, DataViewRowState recordStates)
		{
			throw null;
		}

		IList IListSource.GetList()
		{
			throw null;
		}

		[MonoNotSupported("")]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
		}

		public override string ToString()
		{
			throw null;
		}

		public void WriteXml(Stream stream)
		{
		}

		public void WriteXml(Stream stream, bool writeHierarchy)
		{
		}

		public void WriteXml(Stream stream, XmlWriteMode mode)
		{
		}

		public void WriteXml(Stream stream, XmlWriteMode mode, bool writeHierarchy)
		{
		}

		public void WriteXml(TextWriter writer)
		{
		}

		public void WriteXml(TextWriter writer, bool writeHierarchy)
		{
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode)
		{
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
		}

		public void WriteXml(string fileName)
		{
		}

		public void WriteXml(string fileName, bool writeHierarchy)
		{
		}

		public void WriteXml(string fileName, XmlWriteMode mode)
		{
		}

		public void WriteXml(string fileName, XmlWriteMode mode, bool writeHierarchy)
		{
		}

		public void WriteXml(XmlWriter writer)
		{
		}

		public void WriteXml(XmlWriter writer, bool writeHierarchy)
		{
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode)
		{
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
		}

		public void WriteXmlSchema(Stream stream)
		{
		}

		public void WriteXmlSchema(Stream stream, bool writeHierarchy)
		{
		}

		public void WriteXmlSchema(TextWriter writer)
		{
		}

		public void WriteXmlSchema(TextWriter writer, bool writeHierarchy)
		{
		}

		public void WriteXmlSchema(string fileName)
		{
		}

		public void WriteXmlSchema(string fileName, bool writeHierarchy)
		{
		}

		public void WriteXmlSchema(XmlWriter writer)
		{
		}

		public void WriteXmlSchema(XmlWriter writer, bool writeHierarchy)
		{
		}

		protected internal bool fInitInProgress;
	}
}
