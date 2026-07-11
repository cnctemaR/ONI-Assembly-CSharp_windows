using System;
using System.Collections;
using System.Data.Common;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data
{
	internal sealed class XmlDataTreeWriter
	{
		internal XmlDataTreeWriter(DataSet ds)
		{
			this._ds = ds;
			this._topLevelTables = ds.TopLevelTables();
			foreach (object obj in ds.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				this._dTables.Add(dataTable);
			}
		}

		internal XmlDataTreeWriter(DataTable dt, bool writeHierarchy)
		{
			this._dt = dt;
			this._fFromTable = true;
			if (dt.DataSet == null)
			{
				this._dTables.Add(dt);
				this._topLevelTables = new DataTable[] { dt };
				return;
			}
			this._ds = dt.DataSet;
			this._dTables.Add(dt);
			if (writeHierarchy)
			{
				this._writeHierarchy = true;
				this.CreateTablesHierarchy(dt);
				this._topLevelTables = this.CreateToplevelTables();
				return;
			}
			this._topLevelTables = new DataTable[] { dt };
		}

		private DataTable[] CreateToplevelTables()
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this._dTables.Count; i++)
			{
				DataTable dataTable = (DataTable)this._dTables[i];
				if (dataTable.ParentRelations.Count == 0)
				{
					arrayList.Add(dataTable);
				}
				else
				{
					bool flag = false;
					for (int j = 0; j < dataTable.ParentRelations.Count; j++)
					{
						if (dataTable.ParentRelations[j].Nested)
						{
							if (dataTable.ParentRelations[j].ParentTable == dataTable)
							{
								flag = false;
								break;
							}
							flag = true;
						}
					}
					if (!flag)
					{
						arrayList.Add(dataTable);
					}
				}
			}
			if (arrayList.Count == 0)
			{
				return Array.Empty<DataTable>();
			}
			DataTable[] array = new DataTable[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}

		private void CreateTablesHierarchy(DataTable dt)
		{
			foreach (object obj in dt.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (!this._dTables.Contains(dataRelation.ChildTable))
				{
					this._dTables.Add(dataRelation.ChildTable);
					this.CreateTablesHierarchy(dataRelation.ChildTable);
				}
			}
		}

		internal static bool RowHasErrors(DataRow row)
		{
			int count = row.Table.Columns.Count;
			if (row.HasErrors && row.RowError.Length > 0)
			{
				return true;
			}
			for (int i = 0; i < count; i++)
			{
				DataColumn dataColumn = row.Table.Columns[i];
				string columnError = row.GetColumnError(dataColumn);
				if (columnError != null && columnError.Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		internal void SaveDiffgramData(XmlWriter xw, Hashtable rowsOrder)
		{
			this._xmlw = DataTextWriter.CreateWriter(xw);
			this._isDiffgram = true;
			this._rowsOrder = rowsOrder;
			int num = this._topLevelTables.Length;
			string text = ((this._ds != null) ? ((this._ds.Namespace.Length == 0) ? "" : this._ds.Prefix) : ((this._dt.Namespace.Length == 0) ? "" : this._dt.Prefix));
			if (this._ds == null || this._ds.DataSetName == null || this._ds.DataSetName.Length == 0)
			{
				this._xmlw.WriteStartElement(text, "DocumentElement", (this._dt.Namespace == null) ? "" : this._dt.Namespace);
			}
			else
			{
				this._xmlw.WriteStartElement(text, XmlConvert.EncodeLocalName(this._ds.DataSetName), this._ds.Namespace);
			}
			for (int i = 0; i < this._dTables.Count; i++)
			{
				DataTable dataTable = (DataTable)this._dTables[i];
				foreach (object obj in dataTable.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					if (dataRow.RowState != DataRowState.Deleted)
					{
						int nestedParentCount = dataRow.GetNestedParentCount();
						if (nestedParentCount == 0)
						{
							DataTable dataTable2 = (DataTable)this._dTables[i];
							this.XmlDataRowWriter(dataRow, dataTable2.EncodedTableName);
						}
						else if (nestedParentCount > 1)
						{
							throw ExceptionBuilder.MultipleParentRows((dataTable.Namespace.Length == 0) ? dataTable.TableName : (dataTable.Namespace + dataTable.TableName));
						}
					}
				}
			}
			this._xmlw.WriteEndElement();
			this._xmlw.Flush();
		}

		internal void Save(XmlWriter xw, bool writeSchema)
		{
			this._xmlw = DataTextWriter.CreateWriter(xw);
			int num = this._topLevelTables.Length;
			bool flag = true;
			string text = ((this._ds != null) ? ((this._ds.Namespace.Length == 0) ? "" : this._ds.Prefix) : ((this._dt.Namespace.Length == 0) ? "" : this._dt.Prefix));
			if (!writeSchema && this._ds != null && this._ds._fTopLevelTable && num == 1 && this._ds.TopLevelTables()[0].Rows.Count == 1)
			{
				flag = false;
			}
			if (flag)
			{
				if (this._ds == null)
				{
					this._xmlw.WriteStartElement(text, "DocumentElement", this._dt.Namespace);
				}
				else if (this._ds.DataSetName == null || this._ds.DataSetName.Length == 0)
				{
					this._xmlw.WriteStartElement(text, "DocumentElement", this._ds.Namespace);
				}
				else
				{
					this._xmlw.WriteStartElement(text, XmlConvert.EncodeLocalName(this._ds.DataSetName), this._ds.Namespace);
				}
				for (int i = 0; i < this._dTables.Count; i++)
				{
					if (((DataTable)this._dTables[i])._xmlText != null)
					{
						this._xmlw.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");
						break;
					}
				}
				if (writeSchema)
				{
					if (!this._fFromTable)
					{
						new XmlTreeGen(SchemaFormat.Public).Save(this._ds, this._xmlw);
					}
					else
					{
						new XmlTreeGen(SchemaFormat.Public).Save(null, this._dt, this._xmlw, this._writeHierarchy);
					}
				}
			}
			for (int j = 0; j < this._dTables.Count; j++)
			{
				foreach (object obj in ((DataTable)this._dTables[j]).Rows)
				{
					DataRow dataRow = (DataRow)obj;
					if (dataRow.RowState != DataRowState.Deleted)
					{
						int nestedParentCount = dataRow.GetNestedParentCount();
						if (nestedParentCount == 0)
						{
							this.XmlDataRowWriter(dataRow, ((DataTable)this._dTables[j]).EncodedTableName);
						}
						else if (nestedParentCount > 1)
						{
							DataTable dataTable = (DataTable)this._dTables[j];
							throw ExceptionBuilder.MultipleParentRows((dataTable.Namespace.Length == 0) ? dataTable.TableName : (dataTable.Namespace + dataTable.TableName));
						}
					}
				}
			}
			if (flag)
			{
				this._xmlw.WriteEndElement();
			}
			this._xmlw.Flush();
		}

		private ArrayList GetNestedChildRelations(DataRow row)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object obj in row.Table.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Nested)
				{
					arrayList.Add(dataRelation);
				}
			}
			return arrayList;
		}

		internal void XmlDataRowWriter(DataRow row, string encodedTableName)
		{
			string text = ((row.Table.Namespace.Length == 0) ? "" : row.Table.Prefix);
			this._xmlw.WriteStartElement(text, encodedTableName, row.Table.Namespace);
			if (this._isDiffgram)
			{
				this._xmlw.WriteAttributeString("diffgr", "id", "urn:schemas-microsoft-com:xml-diffgram-v1", row.Table.TableName + row.rowID.ToString(CultureInfo.InvariantCulture));
				this._xmlw.WriteAttributeString("msdata", "rowOrder", "urn:schemas-microsoft-com:xml-msdata", this._rowsOrder[row].ToString());
				if (row.RowState == DataRowState.Added)
				{
					this._xmlw.WriteAttributeString("diffgr", "hasChanges", "urn:schemas-microsoft-com:xml-diffgram-v1", "inserted");
				}
				if (row.RowState == DataRowState.Modified)
				{
					this._xmlw.WriteAttributeString("diffgr", "hasChanges", "urn:schemas-microsoft-com:xml-diffgram-v1", "modified");
				}
				if (XmlDataTreeWriter.RowHasErrors(row))
				{
					this._xmlw.WriteAttributeString("diffgr", "hasErrors", "urn:schemas-microsoft-com:xml-diffgram-v1", "true");
				}
			}
			foreach (object obj in row.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn._columnMapping == MappingType.Attribute)
				{
					object obj2 = row[dataColumn];
					string text2 = ((dataColumn.Namespace.Length == 0) ? "" : dataColumn.Prefix);
					if (obj2 != DBNull.Value && (!dataColumn.ImplementsINullable || !DataStorage.IsObjectSqlNull(obj2)))
					{
						XmlTreeGen.ValidateColumnMapping(dataColumn.DataType);
						this._xmlw.WriteAttributeString(text2, dataColumn.EncodedColumnName, dataColumn.Namespace, dataColumn.ConvertObjectToXml(obj2));
					}
				}
				if (this._isDiffgram && dataColumn._columnMapping == MappingType.Hidden)
				{
					object obj2 = row[dataColumn];
					if (obj2 != DBNull.Value && (!dataColumn.ImplementsINullable || !DataStorage.IsObjectSqlNull(obj2)))
					{
						XmlTreeGen.ValidateColumnMapping(dataColumn.DataType);
						this._xmlw.WriteAttributeString("msdata", "hidden" + dataColumn.EncodedColumnName, "urn:schemas-microsoft-com:xml-msdata", dataColumn.ConvertObjectToXml(obj2));
					}
				}
			}
			foreach (object obj3 in row.Table.Columns)
			{
				DataColumn dataColumn2 = (DataColumn)obj3;
				if (dataColumn2._columnMapping != MappingType.Hidden)
				{
					object obj2 = row[dataColumn2];
					string text3 = ((dataColumn2.Namespace.Length == 0) ? "" : dataColumn2.Prefix);
					bool flag = true;
					if ((obj2 == DBNull.Value || (dataColumn2.ImplementsINullable && DataStorage.IsObjectSqlNull(obj2))) && dataColumn2.ColumnMapping == MappingType.SimpleContent)
					{
						this._xmlw.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
					}
					if (obj2 != DBNull.Value && (!dataColumn2.ImplementsINullable || !DataStorage.IsObjectSqlNull(obj2)) && dataColumn2._columnMapping != MappingType.Attribute)
					{
						if (dataColumn2._columnMapping != MappingType.SimpleContent && (!dataColumn2.IsCustomType || !dataColumn2.IsValueCustomTypeInstance(obj2) || typeof(IXmlSerializable).IsAssignableFrom(obj2.GetType())))
						{
							this._xmlw.WriteStartElement(text3, dataColumn2.EncodedColumnName, dataColumn2.Namespace);
							flag = false;
						}
						Type type = obj2.GetType();
						if (!dataColumn2.IsCustomType)
						{
							if ((type == typeof(char) || type == typeof(string)) && XmlDataTreeWriter.PreserveSpace(obj2))
							{
								this._xmlw.WriteAttributeString("xml", "space", "http://www.w3.org/XML/1998/namespace", "preserve");
							}
							this._xmlw.WriteString(dataColumn2.ConvertObjectToXml(obj2));
						}
						else if (dataColumn2.IsValueCustomTypeInstance(obj2))
						{
							if (!flag && type != dataColumn2.DataType)
							{
								this._xmlw.WriteAttributeString("msdata", "InstanceType", "urn:schemas-microsoft-com:xml-msdata", DataStorage.GetQualifiedName(type));
							}
							if (!flag)
							{
								dataColumn2.ConvertObjectToXml(obj2, this._xmlw, null);
							}
							else
							{
								if (obj2.GetType() != dataColumn2.DataType)
								{
									throw ExceptionBuilder.PolymorphismNotSupported(type.AssemblyQualifiedName);
								}
								XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(dataColumn2.EncodedColumnName);
								xmlRootAttribute.Namespace = dataColumn2.Namespace;
								dataColumn2.ConvertObjectToXml(obj2, this._xmlw, xmlRootAttribute);
							}
						}
						else
						{
							if (type == typeof(Type) || type == typeof(Guid) || type == typeof(char) || DataStorage.IsSqlType(type))
							{
								this._xmlw.WriteAttributeString("msdata", "InstanceType", "urn:schemas-microsoft-com:xml-msdata", type.FullName);
							}
							else if (obj2 is Type)
							{
								this._xmlw.WriteAttributeString("msdata", "InstanceType", "urn:schemas-microsoft-com:xml-msdata", "Type");
							}
							else
							{
								string text4 = "xs:" + XmlTreeGen.XmlDataTypeName(type);
								this._xmlw.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", text4);
								this._xmlw.WriteAttributeString("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
							}
							if (!DataStorage.IsSqlType(type))
							{
								this._xmlw.WriteString(dataColumn2.ConvertObjectToXml(obj2));
							}
							else
							{
								dataColumn2.ConvertObjectToXml(obj2, this._xmlw, null);
							}
						}
						if (dataColumn2._columnMapping != MappingType.SimpleContent && !flag)
						{
							this._xmlw.WriteEndElement();
						}
					}
				}
			}
			if (this._ds != null)
			{
				foreach (object obj4 in this.GetNestedChildRelations(row))
				{
					DataRelation dataRelation = (DataRelation)obj4;
					foreach (DataRow dataRow in row.GetChildRows(dataRelation))
					{
						this.XmlDataRowWriter(dataRow, dataRelation.ChildTable.EncodedTableName);
					}
				}
			}
			this._xmlw.WriteEndElement();
		}

		internal static bool PreserveSpace(object value)
		{
			string text = value.ToString();
			if (text.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < text.Length; i++)
			{
				if (!char.IsWhiteSpace(text, i))
				{
					return false;
				}
			}
			return true;
		}

		private XmlWriter _xmlw;

		private DataSet _ds;

		private DataTable _dt;

		private ArrayList _dTables = new ArrayList();

		private DataTable[] _topLevelTables;

		private bool _fFromTable;

		private bool _isDiffgram;

		private Hashtable _rowsOrder;

		private bool _writeHierarchy;
	}
}
