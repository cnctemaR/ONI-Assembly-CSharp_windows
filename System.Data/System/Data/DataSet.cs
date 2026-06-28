using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data
{
	[ToolboxItem("Microsoft.VSDesigner.Data.VS.DataSetToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[XmlRoot("DataSet")]
	[XmlSchemaProvider("GetDataSetSchema")]
	[Designer("Microsoft.VSDesigner.Data.VS.DataSetDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[DefaultProperty("DataSetName")]
	[Serializable]
	public class DataSet : MarshalByValueComponent, IXmlSerializable, IListSource, ISupportInitialize, ISerializable, ISupportInitializeNotification
	{
		public DataSet()
			: this("NewDataSet")
		{
		}

		public DataSet(string dataSetName)
		{
			this.dataSetName = dataSetName;
			this.tableCollection = new DataTableCollection(this);
			this.relationCollection = new DataRelationCollection.DataSetRelationCollection(this);
			this.properties = new PropertyCollection();
			this.prefix = string.Empty;
		}

		protected DataSet(SerializationInfo info, StreamingContext context)
			: this()
		{
			if (this.IsBinarySerialized(info, context))
			{
				this.BinaryDeserialize(info);
				return;
			}
			string text = info.GetValue("XmlSchema", typeof(string)) as string;
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(text));
			this.ReadXmlSchema(xmlTextReader);
			xmlTextReader.Close();
			this.GetSerializationData(info, context);
		}

		protected DataSet(SerializationInfo info, StreamingContext context, bool constructSchema)
			: this()
		{
			if (this.DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.ExcludeSchema)
			{
				this.InitializeDerivedDataSet();
			}
			if (this.IsBinarySerialized(info, context))
			{
				this.BinaryDeserialize(info);
				return;
			}
			if (constructSchema)
			{
				string text = info.GetValue("XmlSchema", typeof(string)) as string;
				XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(text));
				this.ReadXmlSchema(xmlTextReader);
				xmlTextReader.Close();
				this.GetSerializationData(info, context);
			}
		}

		[DataCategory("Action")]
		public event MergeFailedEventHandler MergeFailed;

		public event EventHandler Initialized;

		IList IListSource.GetList()
		{
			return this.DefaultViewManager;
		}

		bool IListSource.ContainsListCollection
		{
			get
			{
				return true;
			}
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			this.ReadXmlSerializable(reader);
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			this.DoWriteXmlSchema(writer);
			this.WriteXml(writer, XmlWriteMode.DiffGram);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			if (base.GetType() == typeof(DataSet))
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, null);
			this.WriteXmlSchema(xmlTextWriter);
			memoryStream.Position = 0L;
			return XmlSchema.Read(new XmlTextReader(memoryStream), null);
		}

		[DefaultValue(false)]
		[DataCategory("Data")]
		public bool CaseSensitive
		{
			get
			{
				return this.caseSensitive;
			}
			set
			{
				this.caseSensitive = value;
				if (!this.caseSensitive)
				{
					foreach (object obj in this.Tables)
					{
						DataTable dataTable = (DataTable)obj;
						dataTable.ResetCaseSensitiveIndexes();
						foreach (object obj2 in dataTable.Constraints)
						{
							Constraint constraint = (Constraint)obj2;
							constraint.AssertConstraint();
						}
					}
				}
				else
				{
					foreach (object obj3 in this.Tables)
					{
						DataTable dataTable2 = (DataTable)obj3;
						dataTable2.ResetCaseSensitiveIndexes();
					}
				}
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		public string DataSetName
		{
			get
			{
				return this.dataSetName;
			}
			set
			{
				this.dataSetName = value;
			}
		}

		[Browsable(false)]
		public DataViewManager DefaultViewManager
		{
			get
			{
				if (this.defaultView == null)
				{
					this.defaultView = new DataViewManager(this);
				}
				return this.defaultView;
			}
		}

		[DefaultValue(true)]
		public bool EnforceConstraints
		{
			get
			{
				return this.enforceConstraints;
			}
			set
			{
				this.InternalEnforceConstraints(value, true);
			}
		}

		[Browsable(false)]
		[DataCategory("Data")]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				return this.properties;
			}
		}

		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				for (int i = 0; i < this.Tables.Count; i++)
				{
					if (this.Tables[i].HasErrors)
					{
						return true;
					}
				}
				return false;
			}
		}

		[DataCategory("Data")]
		public CultureInfo Locale
		{
			get
			{
				return (this.locale == null) ? Thread.CurrentThread.CurrentCulture : this.locale;
			}
			set
			{
				if (this.locale == null || !this.locale.Equals(value))
				{
					this.locale = value;
				}
			}
		}

		internal bool LocaleSpecified
		{
			get
			{
				return this.locale != null;
			}
		}

		internal TableAdapterSchemaInfo TableAdapterSchemaData
		{
			get
			{
				return this.tableAdapterSchemaInfo;
			}
		}

		internal void InternalEnforceConstraints(bool value, bool resetIndexes)
		{
			if (value == this.enforceConstraints)
			{
				return;
			}
			if (value)
			{
				if (resetIndexes)
				{
					foreach (object obj in this.Tables)
					{
						DataTable dataTable = (DataTable)obj;
						dataTable.ResetIndexes();
					}
				}
				bool flag = false;
				foreach (object obj2 in this.Tables)
				{
					DataTable dataTable2 = (DataTable)obj2;
					foreach (object obj3 in dataTable2.Constraints)
					{
						Constraint constraint = (Constraint)obj3;
						constraint.AssertConstraint();
					}
					dataTable2.AssertNotNullConstraints();
					if (!flag && dataTable2.HasErrors)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Constraint.ThrowConstraintException();
				}
			}
			this.enforceConstraints = value;
		}

		public void Merge(DataRow[] rows)
		{
			this.Merge(rows, false, MissingSchemaAction.Add);
		}

		public void Merge(DataSet dataSet)
		{
			this.Merge(dataSet, false, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table)
		{
			this.Merge(table, false, MissingSchemaAction.Add);
		}

		public void Merge(DataSet dataSet, bool preserveChanges)
		{
			this.Merge(dataSet, preserveChanges, MissingSchemaAction.Add);
		}

		public void Merge(DataRow[] rows, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (rows == null)
			{
				throw new ArgumentNullException("rows");
			}
			if (!DataSet.IsLegalSchemaAction(missingSchemaAction))
			{
				throw new ArgumentOutOfRangeException("missingSchemaAction");
			}
			MergeManager.Merge(this, rows, preserveChanges, missingSchemaAction);
		}

		public void Merge(DataSet dataSet, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			if (!DataSet.IsLegalSchemaAction(missingSchemaAction))
			{
				throw new ArgumentOutOfRangeException("missingSchemaAction");
			}
			MergeManager.Merge(this, dataSet, preserveChanges, missingSchemaAction);
		}

		public void Merge(DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (!DataSet.IsLegalSchemaAction(missingSchemaAction))
			{
				throw new ArgumentOutOfRangeException("missingSchemaAction");
			}
			MergeManager.Merge(this, table, preserveChanges, missingSchemaAction);
		}

		private static bool IsLegalSchemaAction(MissingSchemaAction missingSchemaAction)
		{
			return missingSchemaAction == MissingSchemaAction.Add || missingSchemaAction == MissingSchemaAction.AddWithKey || missingSchemaAction == MissingSchemaAction.Error || missingSchemaAction == MissingSchemaAction.Ignore;
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string Namespace
		{
			get
			{
				return this._namespace;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (value != this._namespace)
				{
					this.RaisePropertyChanging("Namespace");
				}
				this._namespace = value;
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				return this.prefix;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				for (int i = 0; i < value.Length; i++)
				{
					if (!char.IsLetterOrDigit(value[i]) && value[i] != '_' && value[i] != ':')
					{
						throw new DataException("Prefix '" + value + "' is not valid, because it contains special characters.");
					}
				}
				if (value != this.prefix)
				{
					this.RaisePropertyChanging("Prefix");
				}
				this.prefix = value;
			}
		}

		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataRelationCollection Relations
		{
			get
			{
				return this.relationCollection;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DataCategory("Data")]
		public DataTableCollection Tables
		{
			get
			{
				return this.tableCollection;
			}
		}

		public void AcceptChanges()
		{
			foreach (object obj in this.tableCollection)
			{
				DataTable dataTable = (DataTable)obj;
				dataTable.AcceptChanges();
			}
		}

		public void Clear()
		{
			if (this._xmlDataDocument != null)
			{
				throw new NotSupportedException("Clear function on dataset and datatable is not supported when XmlDataDocument is bound to the DataSet.");
			}
			bool flag = this.EnforceConstraints;
			this.EnforceConstraints = false;
			for (int i = 0; i < this.tableCollection.Count; i++)
			{
				this.tableCollection[i].Clear();
			}
			this.EnforceConstraints = flag;
		}

		public virtual DataSet Clone()
		{
			DataSet dataSet = (DataSet)Activator.CreateInstance(base.GetType(), true);
			this.CopyProperties(dataSet);
			foreach (object obj in this.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				if (!dataSet.Tables.Contains(dataTable.TableName))
				{
					dataSet.Tables.Add(dataTable.Clone());
				}
			}
			this.CopyRelations(dataSet);
			return dataSet;
		}

		public DataSet Copy()
		{
			DataSet dataSet = (DataSet)Activator.CreateInstance(base.GetType(), true);
			this.CopyProperties(dataSet);
			foreach (object obj in this.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				if (!dataSet.Tables.Contains(dataTable.TableName))
				{
					dataSet.Tables.Add(dataTable.Copy());
				}
				else
				{
					foreach (object obj2 in dataTable.Rows)
					{
						DataRow dataRow = (DataRow)obj2;
						dataSet.Tables[dataTable.TableName].ImportRow(dataRow);
					}
				}
			}
			this.CopyRelations(dataSet);
			return dataSet;
		}

		private void CopyProperties(DataSet Copy)
		{
			Copy.CaseSensitive = this.CaseSensitive;
			Copy.DataSetName = this.DataSetName;
			Copy.EnforceConstraints = this.EnforceConstraints;
			if (this.ExtendedProperties.Count > 0)
			{
				Array array = Array.CreateInstance(typeof(object), this.ExtendedProperties.Count);
				this.ExtendedProperties.Keys.CopyTo(array, 0);
				for (int i = 0; i < this.ExtendedProperties.Count; i++)
				{
					Copy.ExtendedProperties.Add(array.GetValue(i), this.ExtendedProperties[array.GetValue(i)]);
				}
			}
			Copy.locale = this.locale;
			Copy.Namespace = this.Namespace;
			Copy.Prefix = this.Prefix;
		}

		private void CopyRelations(DataSet Copy)
		{
			foreach (object obj in this.Relations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (!Copy.Relations.Contains(dataRelation.RelationName))
				{
					string tableName = dataRelation.ParentTable.TableName;
					string tableName2 = dataRelation.ChildTable.TableName;
					DataColumn[] array = new DataColumn[dataRelation.ParentColumns.Length];
					DataColumn[] array2 = new DataColumn[dataRelation.ChildColumns.Length];
					int num = 0;
					foreach (DataColumn dataColumn in dataRelation.ParentColumns)
					{
						array[num] = Copy.Tables[tableName].Columns[dataColumn.ColumnName];
						num++;
					}
					num = 0;
					foreach (DataColumn dataColumn2 in dataRelation.ChildColumns)
					{
						array2[num] = Copy.Tables[tableName2].Columns[dataColumn2.ColumnName];
						num++;
					}
					DataRelation dataRelation2 = new DataRelation(dataRelation.RelationName, array, array2, false);
					Copy.Relations.Add(dataRelation2);
				}
			}
			foreach (object obj2 in this.Tables)
			{
				DataTable dataTable = (DataTable)obj2;
				foreach (object obj3 in dataTable.Constraints)
				{
					Constraint constraint = (Constraint)obj3;
					if (constraint is ForeignKeyConstraint && !Copy.Tables[dataTable.TableName].Constraints.Contains(constraint.ConstraintName))
					{
						ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraint;
						DataTable dataTable2 = Copy.Tables[foreignKeyConstraint.RelatedTable.TableName];
						DataTable dataTable3 = Copy.Tables[dataTable.TableName];
						DataColumn[] array3 = new DataColumn[foreignKeyConstraint.RelatedColumns.Length];
						DataColumn[] array4 = new DataColumn[foreignKeyConstraint.Columns.Length];
						for (int k = 0; k < array3.Length; k++)
						{
							array3[k] = dataTable2.Columns[foreignKeyConstraint.RelatedColumns[k].ColumnName];
						}
						for (int l = 0; l < array4.Length; l++)
						{
							array4[l] = dataTable3.Columns[foreignKeyConstraint.Columns[l].ColumnName];
						}
						dataTable3.Constraints.Add(foreignKeyConstraint.ConstraintName, array3, array4);
					}
				}
			}
		}

		public DataSet GetChanges()
		{
			return this.GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}

		public DataSet GetChanges(DataRowState rowStates)
		{
			if (!this.HasChanges(rowStates))
			{
				return null;
			}
			DataSet dataSet = this.Clone();
			bool flag = dataSet.EnforceConstraints;
			dataSet.EnforceConstraints = false;
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < this.Tables.Count; i++)
			{
				DataTable dataTable = this.Tables[i];
				DataTable dataTable2 = dataSet.Tables[dataTable.TableName];
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					DataRow dataRow = dataTable.Rows[j];
					if (dataRow.IsRowChanged(rowStates) && !hashtable.Contains(dataRow))
					{
						this.AddChangedRow(hashtable, dataTable2, dataRow);
					}
				}
			}
			dataSet.EnforceConstraints = flag;
			return dataSet;
		}

		private void AddChangedRow(Hashtable addedRows, DataTable copyTable, DataRow row)
		{
			if (addedRows.ContainsKey(row))
			{
				return;
			}
			foreach (object obj in row.Table.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				DataRow dataRow = ((row.RowState == DataRowState.Deleted) ? row.GetParentRow(dataRelation, DataRowVersion.Original) : row.GetParentRow(dataRelation));
				if (dataRow != null)
				{
					DataTable dataTable = copyTable.DataSet.Tables[dataRow.Table.TableName];
					this.AddChangedRow(addedRows, dataTable, dataRow);
				}
			}
			DataRow dataRow2 = copyTable.NewNotInitializedRow();
			copyTable.Rows.AddInternal(dataRow2);
			row.CopyValuesToRow(dataRow2);
			dataRow2.XmlRowID = row.XmlRowID;
			addedRows.Add(row, row);
		}

		public string GetXml()
		{
			StringWriter stringWriter = new StringWriter();
			this.WriteXml(stringWriter, XmlWriteMode.IgnoreSchema);
			return stringWriter.ToString();
		}

		public string GetXmlSchema()
		{
			StringWriter stringWriter = new StringWriter();
			this.WriteXmlSchema(stringWriter);
			return stringWriter.ToString();
		}

		public bool HasChanges()
		{
			return this.HasChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}

		public bool HasChanges(DataRowState rowStates)
		{
			if (((long)rowStates & (long)((ulong)(-32))) != 0L)
			{
				throw new ArgumentOutOfRangeException("rowStates");
			}
			DataTableCollection tables = this.Tables;
			for (int i = 0; i < tables.Count; i++)
			{
				DataTable dataTable = tables[i];
				DataRowCollection rows = dataTable.Rows;
				for (int j = 0; j < rows.Count; j++)
				{
					DataRow dataRow = rows[j];
					if ((dataRow.RowState & rowStates) != (DataRowState)0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void InferXmlSchema(XmlReader reader, string[] nsArray)
		{
			if (reader == null)
			{
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(reader);
			this.InferXmlSchema(xmlDocument, nsArray);
		}

		private void InferXmlSchema(XmlDocument doc, string[] nsArray)
		{
			XmlDataInferenceLoader.Infer(this, doc, XmlReadMode.InferSchema, nsArray);
		}

		public void InferXmlSchema(Stream stream, string[] nsArray)
		{
			this.InferXmlSchema(new XmlTextReader(stream), nsArray);
		}

		public void InferXmlSchema(TextReader reader, string[] nsArray)
		{
			this.InferXmlSchema(new XmlTextReader(reader), nsArray);
		}

		public void InferXmlSchema(string fileName, string[] nsArray)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			try
			{
				this.InferXmlSchema(xmlTextReader, nsArray);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		public virtual void RejectChanges()
		{
			bool flag = this.EnforceConstraints;
			this.EnforceConstraints = false;
			for (int i = 0; i < this.Tables.Count; i++)
			{
				this.Tables[i].RejectChanges();
			}
			this.EnforceConstraints = flag;
		}

		public virtual void Reset()
		{
			for (int i = 0; i < this.Tables.Count; i++)
			{
				ConstraintCollection constraints = this.Tables[i].Constraints;
				for (int j = 0; j < constraints.Count; j++)
				{
					if (constraints[j] is ForeignKeyConstraint)
					{
						constraints.Remove(constraints[j]);
					}
				}
			}
			this.Clear();
			this.Relations.Clear();
			this.Tables.Clear();
		}

		public void WriteXml(Stream stream)
		{
			this.WriteXml(new XmlTextWriter(stream, null)
			{
				Formatting = Formatting.Indented
			});
		}

		public void WriteXml(string fileName)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartDocument(true);
			try
			{
				this.WriteXml(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
			}
		}

		public void WriteXml(TextWriter writer)
		{
			this.WriteXml(new XmlTextWriter(writer)
			{
				Formatting = Formatting.Indented
			});
		}

		public void WriteXml(XmlWriter writer)
		{
			this.WriteXml(writer, XmlWriteMode.IgnoreSchema);
		}

		public void WriteXml(string fileName, XmlWriteMode mode)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartDocument(true);
			try
			{
				this.WriteXml(xmlTextWriter, mode);
			}
			finally
			{
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
			}
		}

		public void WriteXml(Stream stream, XmlWriteMode mode)
		{
			this.WriteXml(new XmlTextWriter(stream, null)
			{
				Formatting = Formatting.Indented
			}, mode);
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode)
		{
			this.WriteXml(new XmlTextWriter(writer)
			{
				Formatting = Formatting.Indented
			}, mode);
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode)
		{
			if (mode == XmlWriteMode.DiffGram)
			{
				this.SetRowsID();
				DataSet.WriteDiffGramElement(writer);
			}
			bool flag = mode != XmlWriteMode.DiffGram;
			int num = 0;
			while (num < this.tableCollection.Count && !flag)
			{
				flag = this.tableCollection[num].Rows.Count > 0;
				num++;
			}
			if (flag)
			{
				DataSet.WriteStartElement(writer, mode, this.Namespace, this.Prefix, XmlHelper.Encode(this.DataSetName));
				if (mode == XmlWriteMode.WriteSchema)
				{
					this.DoWriteXmlSchema(writer);
				}
				this.WriteTables(writer, mode, this.Tables, DataRowVersion.Default);
				writer.WriteEndElement();
			}
			if (mode == XmlWriteMode.DiffGram && this.HasChanges(DataRowState.Deleted | DataRowState.Modified))
			{
				DataSet changes = this.GetChanges(DataRowState.Deleted | DataRowState.Modified);
				DataSet.WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "before");
				this.WriteTables(writer, mode, changes.Tables, DataRowVersion.Original);
				writer.WriteEndElement();
			}
			if (mode == XmlWriteMode.DiffGram)
			{
				writer.WriteEndElement();
			}
			writer.Flush();
		}

		public void WriteXmlSchema(Stream stream)
		{
			this.WriteXmlSchema(new XmlTextWriter(stream, null)
			{
				Formatting = Formatting.Indented
			});
		}

		public void WriteXmlSchema(string fileName)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null);
			try
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartDocument(true);
				this.WriteXmlSchema(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
			}
		}

		public void WriteXmlSchema(TextWriter writer)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
			try
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				this.WriteXmlSchema(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.Close();
			}
		}

		public void WriteXmlSchema(XmlWriter writer)
		{
			this.DoWriteXmlSchema(writer);
		}

		public void ReadXmlSchema(Stream stream)
		{
			XmlReader xmlReader = new XmlTextReader(stream, null);
			this.ReadXmlSchema(xmlReader);
		}

		public void ReadXmlSchema(string fileName)
		{
			XmlReader xmlReader = new XmlTextReader(fileName);
			try
			{
				this.ReadXmlSchema(xmlReader);
			}
			finally
			{
				xmlReader.Close();
			}
		}

		public void ReadXmlSchema(TextReader reader)
		{
			XmlReader xmlReader = new XmlTextReader(reader);
			this.ReadXmlSchema(xmlReader);
		}

		public void ReadXmlSchema(XmlReader reader)
		{
			XmlSchemaDataImporter xmlSchemaDataImporter = new XmlSchemaDataImporter(this, reader, true);
			xmlSchemaDataImporter.Process();
			this.tableAdapterSchemaInfo = xmlSchemaDataImporter.CurrentAdapter;
		}

		public XmlReadMode ReadXml(Stream stream)
		{
			return this.ReadXml(new XmlTextReader(stream));
		}

		public XmlReadMode ReadXml(string fileName)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			XmlReadMode xmlReadMode;
			try
			{
				xmlReadMode = this.ReadXml(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
			return xmlReadMode;
		}

		public XmlReadMode ReadXml(TextReader reader)
		{
			return this.ReadXml(new XmlTextReader(reader));
		}

		public XmlReadMode ReadXml(XmlReader reader)
		{
			return this.ReadXml(reader, XmlReadMode.Auto);
		}

		public XmlReadMode ReadXml(Stream stream, XmlReadMode mode)
		{
			return this.ReadXml(new XmlTextReader(stream), mode);
		}

		public XmlReadMode ReadXml(string fileName, XmlReadMode mode)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			XmlReadMode xmlReadMode;
			try
			{
				xmlReadMode = this.ReadXml(xmlTextReader, mode);
			}
			finally
			{
				xmlTextReader.Close();
			}
			return xmlReadMode;
		}

		public XmlReadMode ReadXml(TextReader reader, XmlReadMode mode)
		{
			return this.ReadXml(new XmlTextReader(reader), mode);
		}

		public XmlReadMode ReadXml(XmlReader reader, XmlReadMode mode)
		{
			if (reader == null)
			{
				return mode;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return mode;
			default:
			{
				reader.MoveToContent();
				if (reader.EOF)
				{
					return mode;
				}
				if (reader is XmlTextReader)
				{
					((XmlTextReader)reader).WhitespaceHandling = WhitespaceHandling.None;
				}
				XmlDiffLoader xmlDiffLoader = null;
				if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
				{
					switch (mode)
					{
					case XmlReadMode.Auto:
					case XmlReadMode.DiffGram:
						if (xmlDiffLoader == null)
						{
							xmlDiffLoader = new XmlDiffLoader(this);
						}
						xmlDiffLoader.Load(reader);
						return XmlReadMode.DiffGram;
					case XmlReadMode.Fragment:
						reader.Skip();
						goto IL_00D3;
					}
					reader.Skip();
					return mode;
				}
				IL_00D3:
				if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					switch (mode)
					{
					case XmlReadMode.Auto:
						if (this.Tables.Count == 0)
						{
							this.ReadXmlSchema(reader);
							return XmlReadMode.ReadSchema;
						}
						reader.Skip();
						return XmlReadMode.IgnoreSchema;
					case XmlReadMode.IgnoreSchema:
					case XmlReadMode.InferSchema:
						reader.Skip();
						return mode;
					case XmlReadMode.Fragment:
						this.ReadXmlSchema(reader);
						goto IL_0162;
					}
					this.ReadXmlSchema(reader);
					return mode;
				}
				IL_0162:
				if (reader.EOF)
				{
					return mode;
				}
				int num = ((reader.NodeType != XmlNodeType.Element) ? (-1) : reader.Depth);
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement xmlElement = xmlDocument.CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
				if (reader.HasAttributes)
				{
					for (int i = 0; i < reader.AttributeCount; i++)
					{
						reader.MoveToAttribute(i);
						if (reader.NamespaceURI == "http://www.w3.org/2000/xmlns/")
						{
							xmlElement.SetAttribute(reader.Name, reader.GetAttribute(i));
						}
						else
						{
							XmlAttribute xmlAttribute = xmlElement.SetAttributeNode(reader.LocalName, reader.NamespaceURI);
							xmlAttribute.Prefix = reader.Prefix;
							xmlAttribute.Value = reader.GetAttribute(i);
						}
					}
				}
				reader.Read();
				XmlReadMode xmlReadMode = mode;
				bool flag = false;
				while (reader.Depth != num && reader.NodeType != XmlNodeType.EndElement)
				{
					if (reader.NodeType != XmlNodeType.Element)
					{
						if (!reader.Read())
						{
							IL_0364:
							if (reader.NodeType == XmlNodeType.EndElement)
							{
								reader.Read();
							}
							reader.MoveToContent();
							if (mode == XmlReadMode.DiffGram)
							{
								return xmlReadMode;
							}
							xmlDocument.AppendChild(xmlElement);
							if (!flag && xmlReadMode != XmlReadMode.ReadSchema && mode != XmlReadMode.IgnoreSchema && mode != XmlReadMode.Fragment && (this.Tables.Count == 0 || mode == XmlReadMode.InferSchema))
							{
								this.InferXmlSchema(xmlDocument, null);
								if (mode == XmlReadMode.Auto)
								{
									xmlReadMode = XmlReadMode.InferSchema;
								}
							}
							reader = new XmlNodeReader(xmlDocument);
							XmlDataReader.ReadXml(this, reader, mode);
							return (xmlReadMode != XmlReadMode.Auto) ? xmlReadMode : XmlReadMode.IgnoreSchema;
						}
					}
					else if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
					{
						if (mode != XmlReadMode.IgnoreSchema && mode != XmlReadMode.InferSchema)
						{
							this.ReadXmlSchema(reader);
							xmlReadMode = XmlReadMode.ReadSchema;
							flag = true;
						}
						else
						{
							reader.Skip();
						}
					}
					else if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
					{
						if (mode == XmlReadMode.DiffGram || mode == XmlReadMode.IgnoreSchema || mode == XmlReadMode.Auto)
						{
							if (xmlDiffLoader == null)
							{
								xmlDiffLoader = new XmlDiffLoader(this);
							}
							xmlDiffLoader.Load(reader);
							xmlReadMode = XmlReadMode.DiffGram;
						}
						else
						{
							reader.Skip();
						}
					}
					else
					{
						XmlNode xmlNode = xmlDocument.ReadNode(reader);
						xmlElement.AppendChild(xmlNode);
					}
				}
				goto IL_0364;
			}
			}
		}

		internal bool InitInProgress
		{
			get
			{
				return this.initInProgress;
			}
			set
			{
				this.initInProgress = value;
			}
		}

		public void BeginInit()
		{
			this.InitInProgress = true;
			this.dataSetInitialized = false;
		}

		public void EndInit()
		{
			this.Tables.PostAddRange();
			for (int i = 0; i < this.Tables.Count; i++)
			{
				if (this.Tables[i].InitInProgress)
				{
					this.Tables[i].FinishInit();
				}
			}
			this.Relations.PostAddRange();
			this.InitInProgress = false;
			this.dataSetInitialized = true;
			this.DataSetInitialized();
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (this.RemotingFormat == SerializationFormat.Xml)
			{
				info.AddValue("SchemaSerializationMode.DataSet", this.SchemaSerializationMode);
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				this.DoWriteXmlSchema(xmlTextWriter);
				xmlTextWriter.Flush();
				info.AddValue("XmlSchema", stringWriter.ToString());
				stringWriter = new StringWriter();
				xmlTextWriter = new XmlTextWriter(stringWriter);
				this.WriteXml(xmlTextWriter, XmlWriteMode.DiffGram);
				xmlTextWriter.Flush();
				info.AddValue("XmlDiffGram", stringWriter.ToString());
			}
			else
			{
				this.BinarySerialize(info);
			}
		}

		protected void GetSerializationData(SerializationInfo info, StreamingContext context)
		{
			string text = info.GetValue("XmlDiffGram", typeof(string)) as string;
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(text));
			this.ReadXml(xmlTextReader, XmlReadMode.DiffGram);
			xmlTextReader.Close();
		}

		protected virtual XmlSchema GetSchemaSerializable()
		{
			return null;
		}

		protected virtual void ReadXmlSerializable(XmlReader reader)
		{
			this.ReadXml(reader, XmlReadMode.DiffGram);
		}

		protected virtual bool ShouldSerializeRelations()
		{
			return true;
		}

		protected virtual bool ShouldSerializeTables()
		{
			return true;
		}

		[MonoTODO]
		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected virtual void OnRemoveRelation(DataRelation relation)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected virtual void OnRemoveTable(DataTable table)
		{
			throw new NotImplementedException();
		}

		internal virtual void OnMergeFailed(MergeFailedEventArgs e)
		{
			if (this.MergeFailed != null)
			{
				this.MergeFailed(this, e);
				return;
			}
			throw new DataException(e.Conflict);
		}

		[MonoTODO]
		protected internal void RaisePropertyChanging(string name)
		{
		}

		internal static string WriteObjectXml(object o)
		{
			switch (Type.GetTypeCode(o.GetType()))
			{
			case TypeCode.Boolean:
				return XmlConvert.ToString((bool)o);
			case TypeCode.Char:
				return XmlConvert.ToString((char)o);
			case TypeCode.SByte:
				return XmlConvert.ToString((sbyte)o);
			case TypeCode.Byte:
				return XmlConvert.ToString((byte)o);
			case TypeCode.Int16:
				return XmlConvert.ToString((short)o);
			case TypeCode.UInt16:
				return XmlConvert.ToString((ushort)o);
			case TypeCode.Int32:
				return XmlConvert.ToString((int)o);
			case TypeCode.UInt32:
				return XmlConvert.ToString((uint)o);
			case TypeCode.Int64:
				return XmlConvert.ToString((long)o);
			case TypeCode.UInt64:
				return XmlConvert.ToString((ulong)o);
			case TypeCode.Single:
				return XmlConvert.ToString((float)o);
			case TypeCode.Double:
				return XmlConvert.ToString((double)o);
			case TypeCode.Decimal:
				return XmlConvert.ToString((decimal)o);
			case TypeCode.DateTime:
				return XmlConvert.ToString((DateTime)o, XmlDateTimeSerializationMode.Unspecified);
			default:
				if (o is TimeSpan)
				{
					return XmlConvert.ToString((TimeSpan)o);
				}
				if (o is Guid)
				{
					return XmlConvert.ToString((Guid)o);
				}
				if (o is byte[])
				{
					return Convert.ToBase64String((byte[])o);
				}
				return o.ToString();
			}
		}

		private void WriteTables(XmlWriter writer, XmlWriteMode mode, DataTableCollection tableCollection, DataRowVersion version)
		{
			foreach (object obj in tableCollection)
			{
				DataTable dataTable = (DataTable)obj;
				DataSet.WriteTable(writer, dataTable, mode, version);
			}
		}

		internal static void WriteTable(XmlWriter writer, DataTable table, XmlWriteMode mode, DataRowVersion version)
		{
			DataRow[] array = table.NewRowArray(table.Rows.Count);
			table.Rows.CopyTo(array, 0);
			DataSet.WriteTable(writer, array, mode, version, true);
		}

		internal static void WriteTable(XmlWriter writer, DataRow[] rows, XmlWriteMode mode, DataRowVersion version, bool skipIfNested)
		{
			if (rows.Length == 0)
			{
				return;
			}
			DataTable table = rows[0].Table;
			if (table.TableName == null || table.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			DataColumn dataColumn = null;
			ArrayList arrayList;
			ArrayList arrayList2;
			DataSet.SplitColumns(table, out arrayList, out arrayList2, out dataColumn);
			int count = table.ParentRelations.Count;
			int i = 0;
			while (i < rows.Length)
			{
				DataRow dataRow = rows[i];
				if (!skipIfNested)
				{
					goto IL_00D6;
				}
				bool flag = false;
				for (int j = 0; j < table.ParentRelations.Count; j++)
				{
					DataRelation dataRelation = table.ParentRelations[j];
					if (dataRelation.Nested)
					{
						if (dataRow.GetParentRow(dataRelation) != null)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					goto IL_00D6;
				}
				IL_02C5:
				i++;
				continue;
				IL_00D6:
				if (!dataRow.HasVersion(version) || (mode == XmlWriteMode.DiffGram && dataRow.RowState == DataRowState.Unchanged && version == DataRowVersion.Original))
				{
					goto IL_02C5;
				}
				bool flag2 = true;
				foreach (object obj in table.Columns)
				{
					DataColumn dataColumn2 = (DataColumn)obj;
					if (dataRow[dataColumn2.ColumnName, version] != DBNull.Value)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					writer.WriteElementString(XmlHelper.Encode(table.TableName), string.Empty);
					goto IL_02C5;
				}
				DataSet.WriteTableElement(writer, mode, table, dataRow, version);
				foreach (object obj2 in arrayList)
				{
					DataColumn dataColumn3 = (DataColumn)obj2;
					DataSet.WriteColumnAsAttribute(writer, mode, dataColumn3, dataRow, version);
				}
				if (dataColumn != null)
				{
					writer.WriteString(DataSet.WriteObjectXml(dataRow[dataColumn, version]));
				}
				else
				{
					foreach (object obj3 in arrayList2)
					{
						DataColumn dataColumn4 = (DataColumn)obj3;
						DataSet.WriteColumnAsElement(writer, mode, dataColumn4, dataRow, version);
					}
				}
				foreach (object obj4 in table.ChildRelations)
				{
					DataRelation dataRelation2 = (DataRelation)obj4;
					if (dataRelation2.Nested)
					{
						DataSet.WriteTable(writer, dataRow.GetChildRows(dataRelation2), mode, version, false);
					}
				}
				writer.WriteEndElement();
				goto IL_02C5;
			}
		}

		internal static void WriteColumnAsElement(XmlWriter writer, XmlWriteMode mode, DataColumn col, DataRow row, DataRowVersion version)
		{
			string text = null;
			object obj = row[col, version];
			if (obj == null || obj == DBNull.Value)
			{
				return;
			}
			if (col.Namespace != string.Empty)
			{
				text = col.Namespace;
			}
			DataSet.WriteStartElement(writer, mode, text, col.Prefix, XmlHelper.Encode(col.ColumnName));
			if (typeof(IXmlSerializable).IsAssignableFrom(col.DataType) || col.DataType == typeof(object))
			{
				if (!(obj is IXmlSerializable))
				{
					throw new InvalidOperationException();
				}
				((IXmlSerializable)obj).WriteXml(writer);
			}
			else
			{
				writer.WriteString(DataSet.WriteObjectXml(obj));
			}
			writer.WriteEndElement();
		}

		internal static void WriteColumnAsAttribute(XmlWriter writer, XmlWriteMode mode, DataColumn col, DataRow row, DataRowVersion version)
		{
			if (!row.IsNull(col))
			{
				DataSet.WriteAttributeString(writer, mode, col.Namespace, col.Prefix, XmlHelper.Encode(col.ColumnName), DataSet.WriteObjectXml(row[col, version]));
			}
		}

		internal static void WriteTableElement(XmlWriter writer, XmlWriteMode mode, DataTable table, DataRow row, DataRowVersion version)
		{
			string text = ((table.Namespace.Length <= 0 && table.DataSet != null) ? table.DataSet.Namespace : table.Namespace);
			DataSet.WriteStartElement(writer, mode, text, table.Prefix, XmlHelper.Encode(table.TableName));
			if (mode == XmlWriteMode.DiffGram)
			{
				DataSet.WriteAttributeString(writer, mode, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "id", table.TableName + (row.XmlRowID + 1));
				DataSet.WriteAttributeString(writer, mode, "urn:schemas-microsoft-com:xml-msdata", "msdata", "rowOrder", XmlConvert.ToString(row.XmlRowID));
				string text2 = null;
				if (row.RowState == DataRowState.Modified)
				{
					text2 = "modified";
				}
				else if (row.RowState == DataRowState.Added)
				{
					text2 = "inserted";
				}
				if (version != DataRowVersion.Original && text2 != null)
				{
					DataSet.WriteAttributeString(writer, mode, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "hasChanges", text2);
				}
			}
		}

		internal static void WriteStartElement(XmlWriter writer, XmlWriteMode mode, string nspc, string prefix, string name)
		{
			writer.WriteStartElement(prefix, name, nspc);
		}

		internal static void WriteAttributeString(XmlWriter writer, XmlWriteMode mode, string nspc, string prefix, string name, string stringValue)
		{
			if (mode != XmlWriteMode.DiffGram)
			{
				writer.WriteAttributeString(name, stringValue);
			}
			else
			{
				writer.WriteAttributeString(prefix, name, nspc, stringValue);
			}
		}

		internal void WriteIndividualTableContent(XmlWriter writer, DataTable table, XmlWriteMode mode)
		{
			if (mode == XmlWriteMode.DiffGram)
			{
				table.SetRowsID();
				DataSet.WriteDiffGramElement(writer);
			}
			DataSet.WriteStartElement(writer, mode, this.Namespace, this.Prefix, XmlHelper.Encode(this.DataSetName));
			DataSet.WriteTable(writer, table, mode, DataRowVersion.Default);
			if (mode == XmlWriteMode.DiffGram)
			{
				writer.WriteEndElement();
				if (this.HasChanges(DataRowState.Deleted | DataRowState.Modified))
				{
					DataSet changes = this.GetChanges(DataRowState.Deleted | DataRowState.Modified);
					DataSet.WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "before");
					DataSet.WriteTable(writer, changes.Tables[table.TableName], mode, DataRowVersion.Original);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}

		private void DoWriteXmlSchema(XmlWriter writer)
		{
			if (writer.WriteState == WriteState.Start)
			{
				writer.WriteStartDocument();
			}
			XmlSchemaWriter.WriteXmlSchema(this, writer);
		}

		internal static void SplitColumns(DataTable table, out ArrayList atts, out ArrayList elements, out DataColumn simple)
		{
			atts = new ArrayList();
			elements = new ArrayList();
			simple = null;
			foreach (object obj in table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				switch (dataColumn.ColumnMapping)
				{
				case MappingType.Element:
					elements.Add(dataColumn);
					break;
				case MappingType.Attribute:
					atts.Add(dataColumn);
					break;
				case MappingType.SimpleContent:
					if (simple != null)
					{
						throw new InvalidOperationException("There may only be one simple content element");
					}
					simple = dataColumn;
					break;
				}
			}
		}

		internal static void WriteDiffGramElement(XmlWriter writer)
		{
			DataSet.WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "diffgram");
			DataSet.WriteAttributeString(writer, XmlWriteMode.DiffGram, null, "xmlns", "msdata", "urn:schemas-microsoft-com:xml-msdata");
		}

		private void SetRowsID()
		{
			foreach (object obj in this.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				dataTable.SetRowsID();
			}
		}

		[DefaultValue(SerializationFormat.Xml)]
		public SerializationFormat RemotingFormat
		{
			get
			{
				return this.remotingFormat;
			}
			set
			{
				this.remotingFormat = value;
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return this.dataSetInitialized;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual SchemaSerializationMode SchemaSerializationMode
		{
			get
			{
				return SchemaSerializationMode.IncludeSchema;
			}
			set
			{
				if (value != SchemaSerializationMode.IncludeSchema)
				{
					throw new InvalidOperationException("Only IncludeSchema Mode can be set for Untyped DataSet");
				}
			}
		}

		public DataTableReader CreateDataReader(params DataTable[] dataTables)
		{
			return new DataTableReader(dataTables);
		}

		public DataTableReader CreateDataReader()
		{
			return new DataTableReader((DataTable[])this.Tables.ToArray(typeof(DataTable)));
		}

		public static XmlSchemaComplexType GetDataSetSchema(XmlSchemaSet schemaSet)
		{
			return new XmlSchemaComplexType();
		}

		public void Load(IDataReader reader, LoadOption loadOption, params DataTable[] tables)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			foreach (DataTable dataTable in tables)
			{
				if (dataTable.DataSet == null || dataTable.DataSet != this)
				{
					throw new ArgumentException("Table " + dataTable.TableName + " does not belong to this DataSet.");
				}
				dataTable.Load(reader, loadOption);
				reader.NextResult();
			}
		}

		public void Load(IDataReader reader, LoadOption loadOption, params string[] tables)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			foreach (string text in tables)
			{
				DataTable dataTable = this.Tables[text];
				if (dataTable == null)
				{
					dataTable = new DataTable(text);
					this.Tables.Add(dataTable);
				}
				dataTable.Load(reader, loadOption);
				reader.NextResult();
			}
		}

		public virtual void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler, params DataTable[] tables)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			foreach (DataTable dataTable in tables)
			{
				if (dataTable.DataSet == null || dataTable.DataSet != this)
				{
					throw new ArgumentException("Table " + dataTable.TableName + " does not belong to this DataSet.");
				}
				dataTable.Load(reader, loadOption, errorHandler);
				reader.NextResult();
			}
		}

		private void BinarySerialize(SerializationInfo si)
		{
			Version version = new Version(2, 0);
			si.AddValue("DataSet.RemotingVersion", version, typeof(Version));
			si.AddValue("DataSet.RemotingFormat", this.RemotingFormat, typeof(SerializationFormat));
			si.AddValue("DataSet.DataSetName", this.DataSetName);
			si.AddValue("DataSet.Namespace", this.Namespace);
			si.AddValue("DataSet.Prefix", this.Prefix);
			si.AddValue("DataSet.CaseSensitive", this.CaseSensitive);
			si.AddValue("DataSet.LocaleLCID", this.Locale.LCID);
			si.AddValue("DataSet.EnforceConstraints", this.EnforceConstraints);
			si.AddValue("DataSet.ExtendedProperties", this.properties, typeof(PropertyCollection));
			this.Tables.BinarySerialize_Schema(si);
			this.Tables.BinarySerialize_Data(si);
			this.Relations.BinarySerialize(si);
		}

		private void BinaryDeserialize(SerializationInfo info)
		{
			this.DataSetName = info.GetString("DataSet.DataSetName");
			this.Namespace = info.GetString("DataSet.Namespace");
			this.CaseSensitive = info.GetBoolean("DataSet.CaseSensitive");
			this.Locale = new CultureInfo(info.GetInt32("DataSet.LocaleLCID"));
			this.EnforceConstraints = info.GetBoolean("DataSet.EnforceConstraints");
			this.Prefix = info.GetString("DataSet.Prefix");
			this.properties = (PropertyCollection)info.GetValue("DataSet.ExtendedProperties", typeof(PropertyCollection));
			int @int = info.GetInt32("DataSet.Tables.Count");
			ArrayList arrayList2;
			for (int i = 0; i < @int; i++)
			{
				byte[] array = (byte[])info.GetValue("DataSet.Tables_" + i, typeof(byte[]));
				MemoryStream memoryStream = new MemoryStream(array);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				DataTable dataTable = (DataTable)binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					dataTable.Columns[j].Expression = info.GetString(string.Concat(new object[] { "DataTable_", i, ".DataColumn_", j, ".Expression" }));
				}
				ArrayList arrayList = (ArrayList)info.GetValue("DataTable_" + i + ".NullBits", typeof(ArrayList));
				arrayList2 = (ArrayList)info.GetValue("DataTable_" + i + ".Records", typeof(ArrayList));
				BitArray bitArray = (BitArray)info.GetValue("DataTable_" + i + ".RowStates", typeof(BitArray));
				dataTable.DeserializeRecords(arrayList2, arrayList, bitArray);
				this.Tables.Add(dataTable);
			}
			for (int k = 0; k < @int; k++)
			{
				DataTable dataTable = this.Tables[k];
				dataTable.dataSet = this;
				arrayList2 = (ArrayList)info.GetValue("DataTable_" + k + ".Constraints", typeof(ArrayList));
				if (dataTable.Constraints == null)
				{
					dataTable.Constraints = new ConstraintCollection(dataTable);
				}
				dataTable.DeserializeConstraints(arrayList2);
			}
			arrayList2 = (ArrayList)info.GetValue("DataSet.Relations", typeof(ArrayList));
			bool flag = true;
			for (int l = 0; l < arrayList2.Count; l++)
			{
				ArrayList arrayList3 = (ArrayList)arrayList2[l];
				ArrayList arrayList4 = new ArrayList();
				ArrayList arrayList5 = new ArrayList();
				for (int m = 0; m < arrayList3.Count; m++)
				{
					if (arrayList3[m] != null && typeof(int) == arrayList3[m].GetType().GetElementType())
					{
						Array array2 = (Array)arrayList3[m];
						if (flag)
						{
							arrayList5.Add(this.Tables[(int)array2.GetValue(0)].Columns[(int)array2.GetValue(1)]);
							flag = false;
						}
						else
						{
							arrayList4.Add(this.Tables[(int)array2.GetValue(0)].Columns[(int)array2.GetValue(1)]);
							flag = true;
						}
					}
				}
				this.Relations.Add((string)arrayList3[0], (DataColumn[])arrayList5.ToArray(typeof(DataColumn)), (DataColumn[])arrayList4.ToArray(typeof(DataColumn)), false);
			}
		}

		private void OnDataSetInitialized(EventArgs e)
		{
			if (this.Initialized != null)
			{
				this.Initialized(this, e);
			}
		}

		private void DataSetInitialized()
		{
			EventArgs e = new EventArgs();
			this.OnDataSetInitialized(e);
		}

		protected virtual void InitializeDerivedDataSet()
		{
		}

		protected SchemaSerializationMode DetermineSchemaSerializationMode(XmlReader reader)
		{
			return SchemaSerializationMode.IncludeSchema;
		}

		protected SchemaSerializationMode DetermineSchemaSerializationMode(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Name == "SchemaSerializationMode.DataSet")
				{
					return (SchemaSerializationMode)((int)enumerator.Value);
				}
			}
			return SchemaSerializationMode.IncludeSchema;
		}

		protected bool IsBinarySerialized(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.ObjectType == typeof(SerializationFormat))
				{
					return true;
				}
			}
			return false;
		}

		private string dataSetName;

		private string _namespace = string.Empty;

		private string prefix;

		private bool caseSensitive;

		private bool enforceConstraints = true;

		private DataTableCollection tableCollection;

		private DataRelationCollection relationCollection;

		private PropertyCollection properties;

		private DataViewManager defaultView;

		private CultureInfo locale;

		internal XmlDataDocument _xmlDataDocument;

		internal TableAdapterSchemaInfo tableAdapterSchemaInfo;

		private bool initInProgress;

		private bool dataSetInitialized = true;

		private SerializationFormat remotingFormat;
	}
}
