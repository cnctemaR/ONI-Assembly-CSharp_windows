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
	[DefaultProperty("DataSetName")]
	[Designer("Microsoft.VSDesigner.Data.VS.DataSetDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("Microsoft.VSDesigner.Data.VS.DataSetToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[XmlRoot("DataSet")]
	[XmlSchemaProvider("GetDataSetSchema")]
	[Serializable]
	public class DataSet : MarshalByValueComponent, IListSource, ISupportInitialize, ISupportInitializeNotification, ISerializable, IXmlSerializable
	{
		public DataSet()
		{
		}

		protected DataSet(SerializationInfo info, StreamingContext context)
		{
		}

		protected DataSet(SerializationInfo info, StreamingContext context, bool ConstructSchema)
		{
		}

		public DataSet(string dataSetName)
		{
		}

		[DefaultValue(false)]
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

		[DefaultValue("")]
		public string DataSetName
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
		public DataViewManager DefaultViewManager
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue(true)]
		public bool EnforceConstraints
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

		[DefaultValue("")]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataRelationCollection Relations
		{
			get
			{
				throw null;
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
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual SchemaSerializationMode SchemaSerializationMode
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataTableCollection Tables
		{
			get
			{
				throw null;
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

		public event MergeFailedEventHandler MergeFailed
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

		public void BeginInit()
		{
		}

		public void Clear()
		{
		}

		public virtual DataSet Clone()
		{
			throw null;
		}

		public DataSet Copy()
		{
			throw null;
		}

		public DataTableReader CreateDataReader()
		{
			throw null;
		}

		public DataTableReader CreateDataReader(params DataTable[] dataTables)
		{
			throw null;
		}

		protected SchemaSerializationMode DetermineSchemaSerializationMode(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		protected SchemaSerializationMode DetermineSchemaSerializationMode(XmlReader reader)
		{
			throw null;
		}

		public void EndInit()
		{
		}

		public DataSet GetChanges()
		{
			throw null;
		}

		public DataSet GetChanges(DataRowState rowStates)
		{
			throw null;
		}

		public static XmlSchemaComplexType GetDataSetSchema(XmlSchemaSet schemaSet)
		{
			throw null;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		protected virtual XmlSchema GetSchemaSerializable()
		{
			throw null;
		}

		protected void GetSerializationData(SerializationInfo info, StreamingContext context)
		{
		}

		public string GetXml()
		{
			throw null;
		}

		public string GetXmlSchema()
		{
			throw null;
		}

		public bool HasChanges()
		{
			throw null;
		}

		public bool HasChanges(DataRowState rowStates)
		{
			throw null;
		}

		public void InferXmlSchema(Stream stream, string[] nsArray)
		{
		}

		public void InferXmlSchema(TextReader reader, string[] nsArray)
		{
		}

		public void InferXmlSchema(string fileName, string[] nsArray)
		{
		}

		public void InferXmlSchema(XmlReader reader, string[] nsArray)
		{
		}

		protected virtual void InitializeDerivedDataSet()
		{
		}

		protected bool IsBinarySerialized(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public void Load(IDataReader reader, LoadOption loadOption, params DataTable[] tables)
		{
		}

		public virtual void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler, params DataTable[] tables)
		{
		}

		public void Load(IDataReader reader, LoadOption loadOption, params string[] tables)
		{
		}

		public void Merge(DataRow[] rows)
		{
		}

		public void Merge(DataRow[] rows, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
		}

		public void Merge(DataSet dataSet)
		{
		}

		public void Merge(DataSet dataSet, bool preserveChanges)
		{
		}

		public void Merge(DataSet dataSet, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
		}

		public void Merge(DataTable table)
		{
		}

		public void Merge(DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
		}

		[MonoTODO]
		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
		}

		[MonoTODO]
		protected virtual void OnRemoveRelation(DataRelation relation)
		{
		}

		[MonoTODO]
		protected virtual void OnRemoveTable(DataTable table)
		{
		}

		[MonoTODO]
		protected internal void RaisePropertyChanging(string name)
		{
		}

		public XmlReadMode ReadXml(Stream stream)
		{
			throw null;
		}

		public XmlReadMode ReadXml(Stream stream, XmlReadMode mode)
		{
			throw null;
		}

		public XmlReadMode ReadXml(TextReader reader)
		{
			throw null;
		}

		public XmlReadMode ReadXml(TextReader reader, XmlReadMode mode)
		{
			throw null;
		}

		public XmlReadMode ReadXml(string fileName)
		{
			throw null;
		}

		public XmlReadMode ReadXml(string fileName, XmlReadMode mode)
		{
			throw null;
		}

		public XmlReadMode ReadXml(XmlReader reader)
		{
			throw null;
		}

		public XmlReadMode ReadXml(XmlReader reader, XmlReadMode mode)
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

		protected virtual void ReadXmlSerializable(XmlReader reader)
		{
		}

		public virtual void RejectChanges()
		{
		}

		public virtual void Reset()
		{
		}

		protected virtual bool ShouldSerializeRelations()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeTables()
		{
			throw null;
		}

		IList IListSource.GetList()
		{
			throw null;
		}

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

		public void WriteXml(Stream stream)
		{
		}

		public void WriteXml(Stream stream, XmlWriteMode mode)
		{
		}

		public void WriteXml(TextWriter writer)
		{
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode)
		{
		}

		public void WriteXml(string fileName)
		{
		}

		public void WriteXml(string fileName, XmlWriteMode mode)
		{
		}

		public void WriteXml(XmlWriter writer)
		{
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode)
		{
		}

		public void WriteXmlSchema(Stream stream)
		{
		}

		public void WriteXmlSchema(TextWriter writer)
		{
		}

		public void WriteXmlSchema(string fileName)
		{
		}

		public void WriteXmlSchema(XmlWriter writer)
		{
		}
	}
}
