using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data
{
	internal sealed class XMLDiffLoader
	{
		internal void LoadDiffGram(DataSet ds, XmlReader dataTextReader)
		{
			XmlReader xmlReader = DataTextReader.CreateReader(dataTextReader);
			this._dataSet = ds;
			while (xmlReader.LocalName == "before")
			{
				if (!(xmlReader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1"))
				{
					break;
				}
				this.ProcessDiffs(ds, xmlReader);
				xmlReader.Read();
			}
			while (xmlReader.LocalName == "errors" && xmlReader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
			{
				this.ProcessErrors(ds, xmlReader);
				xmlReader.Read();
			}
		}

		private void CreateTablesHierarchy(DataTable dt)
		{
			foreach (object obj in dt.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (!this._tables.Contains(dataRelation.ChildTable))
				{
					this._tables.Add(dataRelation.ChildTable);
					this.CreateTablesHierarchy(dataRelation.ChildTable);
				}
			}
		}

		internal void LoadDiffGram(DataTable dt, XmlReader dataTextReader)
		{
			XmlReader xmlReader = DataTextReader.CreateReader(dataTextReader);
			this._dataTable = dt;
			this._tables = new ArrayList();
			this._tables.Add(dt);
			this.CreateTablesHierarchy(dt);
			while (xmlReader.LocalName == "before")
			{
				if (!(xmlReader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1"))
				{
					break;
				}
				this.ProcessDiffs(this._tables, xmlReader);
				xmlReader.Read();
			}
			while (xmlReader.LocalName == "errors" && xmlReader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
			{
				this.ProcessErrors(this._tables, xmlReader);
				xmlReader.Read();
			}
		}

		internal void ProcessDiffs(DataSet ds, XmlReader ssync)
		{
			int num = -1;
			int i = ssync.Depth;
			ssync.Read();
			this.SkipWhitespaces(ssync);
			while (i < ssync.Depth)
			{
				DataTable dataTable = null;
				int depth = ssync.Depth;
				string attribute = ssync.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
				bool flag = ssync.GetAttribute("hasErrors", "urn:schemas-microsoft-com:xml-diffgram-v1") == "true";
				int num2 = this.ReadOldRowData(ds, ref dataTable, ref num, ssync);
				if (num2 != -1)
				{
					if (dataTable == null)
					{
						throw ExceptionBuilder.DiffgramMissingSQL();
					}
					DataRow dataRow = (DataRow)dataTable.RowDiffId[attribute];
					if (dataRow != null)
					{
						dataRow._oldRecord = num2;
						dataTable._recordManager[num2] = dataRow;
					}
					else
					{
						dataRow = dataTable.NewEmptyRow();
						dataTable._recordManager[num2] = dataRow;
						dataRow._oldRecord = num2;
						dataRow._newRecord = num2;
						dataTable.Rows.DiffInsertAt(dataRow, num);
						dataRow.Delete();
						if (flag)
						{
							dataTable.RowDiffId[attribute] = dataRow;
						}
					}
				}
			}
		}

		internal void ProcessDiffs(ArrayList tableList, XmlReader ssync)
		{
			int num = -1;
			int i = ssync.Depth;
			ssync.Read();
			while (i < ssync.Depth)
			{
				DataTable dataTable = null;
				int depth = ssync.Depth;
				string attribute = ssync.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
				bool flag = ssync.GetAttribute("hasErrors", "urn:schemas-microsoft-com:xml-diffgram-v1") == "true";
				int num2 = this.ReadOldRowData(this._dataSet, ref dataTable, ref num, ssync);
				if (num2 != -1)
				{
					if (dataTable == null)
					{
						throw ExceptionBuilder.DiffgramMissingSQL();
					}
					DataRow dataRow = (DataRow)dataTable.RowDiffId[attribute];
					if (dataRow != null)
					{
						dataRow._oldRecord = num2;
						dataTable._recordManager[num2] = dataRow;
					}
					else
					{
						dataRow = dataTable.NewEmptyRow();
						dataTable._recordManager[num2] = dataRow;
						dataRow._oldRecord = num2;
						dataRow._newRecord = num2;
						dataTable.Rows.DiffInsertAt(dataRow, num);
						dataRow.Delete();
						if (flag)
						{
							dataTable.RowDiffId[attribute] = dataRow;
						}
					}
				}
			}
		}

		internal void ProcessErrors(DataSet ds, XmlReader ssync)
		{
			int i = ssync.Depth;
			ssync.Read();
			while (i < ssync.Depth)
			{
				DataTable table = ds.Tables.GetTable(XmlConvert.DecodeName(ssync.LocalName), ssync.NamespaceURI);
				if (table == null)
				{
					throw ExceptionBuilder.DiffgramMissingSQL();
				}
				string attribute = ssync.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
				DataRow dataRow = (DataRow)table.RowDiffId[attribute];
				string attribute2 = ssync.GetAttribute("Error", "urn:schemas-microsoft-com:xml-diffgram-v1");
				if (attribute2 != null)
				{
					dataRow.RowError = attribute2;
				}
				int j = ssync.Depth;
				ssync.Read();
				while (j < ssync.Depth)
				{
					if (XmlNodeType.Element == ssync.NodeType)
					{
						DataColumn dataColumn = table.Columns[XmlConvert.DecodeName(ssync.LocalName), ssync.NamespaceURI];
						string attribute3 = ssync.GetAttribute("Error", "urn:schemas-microsoft-com:xml-diffgram-v1");
						dataRow.SetColumnError(dataColumn, attribute3);
					}
					ssync.Read();
				}
				while (ssync.NodeType == XmlNodeType.EndElement && i < ssync.Depth)
				{
					ssync.Read();
				}
			}
		}

		internal void ProcessErrors(ArrayList dt, XmlReader ssync)
		{
			int i = ssync.Depth;
			ssync.Read();
			while (i < ssync.Depth)
			{
				DataTable dataTable = this.GetTable(XmlConvert.DecodeName(ssync.LocalName), ssync.NamespaceURI);
				if (dataTable == null)
				{
					throw ExceptionBuilder.DiffgramMissingSQL();
				}
				string attribute = ssync.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
				DataRow dataRow = (DataRow)dataTable.RowDiffId[attribute];
				if (dataRow == null)
				{
					for (int j = 0; j < dt.Count; j++)
					{
						dataRow = (DataRow)((DataTable)dt[j]).RowDiffId[attribute];
						if (dataRow != null)
						{
							dataTable = dataRow.Table;
							break;
						}
					}
				}
				string attribute2 = ssync.GetAttribute("Error", "urn:schemas-microsoft-com:xml-diffgram-v1");
				if (attribute2 != null)
				{
					dataRow.RowError = attribute2;
				}
				int k = ssync.Depth;
				ssync.Read();
				while (k < ssync.Depth)
				{
					if (XmlNodeType.Element == ssync.NodeType)
					{
						DataColumn dataColumn = dataTable.Columns[XmlConvert.DecodeName(ssync.LocalName), ssync.NamespaceURI];
						string attribute3 = ssync.GetAttribute("Error", "urn:schemas-microsoft-com:xml-diffgram-v1");
						dataRow.SetColumnError(dataColumn, attribute3);
					}
					ssync.Read();
				}
				while (ssync.NodeType == XmlNodeType.EndElement && i < ssync.Depth)
				{
					ssync.Read();
				}
			}
		}

		private DataTable GetTable(string tableName, string ns)
		{
			if (this._tables == null)
			{
				return this._dataSet.Tables.GetTable(tableName, ns);
			}
			if (this._tables.Count == 0)
			{
				return (DataTable)this._tables[0];
			}
			for (int i = 0; i < this._tables.Count; i++)
			{
				DataTable dataTable = (DataTable)this._tables[i];
				if (string.Equals(dataTable.TableName, tableName, StringComparison.Ordinal) && string.Equals(dataTable.Namespace, ns, StringComparison.Ordinal))
				{
					return dataTable;
				}
			}
			return null;
		}

		private int ReadOldRowData(DataSet ds, ref DataTable table, ref int pos, XmlReader row)
		{
			if (ds != null)
			{
				table = ds.Tables.GetTable(XmlConvert.DecodeName(row.LocalName), row.NamespaceURI);
			}
			else
			{
				table = this.GetTable(XmlConvert.DecodeName(row.LocalName), row.NamespaceURI);
			}
			if (table == null)
			{
				row.Skip();
				return -1;
			}
			int depth = row.Depth;
			if (table == null)
			{
				throw ExceptionBuilder.DiffgramMissingTable(XmlConvert.DecodeName(row.LocalName));
			}
			string text = row.GetAttribute("rowOrder", "urn:schemas-microsoft-com:xml-msdata");
			if (!string.IsNullOrEmpty(text))
			{
				pos = (int)Convert.ChangeType(text, typeof(int), null);
			}
			int num = table.NewRecord();
			foreach (object obj in table.Columns)
			{
				((DataColumn)obj)[num] = DBNull.Value;
			}
			foreach (object obj2 in table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj2;
				if (dataColumn.ColumnMapping != MappingType.Element && dataColumn.ColumnMapping != MappingType.SimpleContent)
				{
					if (dataColumn.ColumnMapping == MappingType.Hidden)
					{
						text = row.GetAttribute("hidden" + dataColumn.EncodedColumnName, "urn:schemas-microsoft-com:xml-msdata");
					}
					else
					{
						text = row.GetAttribute(dataColumn.EncodedColumnName, dataColumn.Namespace);
					}
					if (text != null)
					{
						dataColumn[num] = dataColumn.ConvertXmlToObject(text);
					}
				}
			}
			row.Read();
			this.SkipWhitespaces(row);
			int depth2 = row.Depth;
			if (depth2 <= depth)
			{
				if (depth2 == depth && row.NodeType == XmlNodeType.EndElement)
				{
					row.Read();
					this.SkipWhitespaces(row);
				}
				return num;
			}
			if (table.XmlText != null)
			{
				DataColumn xmlText = table.XmlText;
				xmlText[num] = xmlText.ConvertXmlToObject(row.ReadString());
			}
			else
			{
				while (row.Depth > depth)
				{
					string text2 = XmlConvert.DecodeName(row.LocalName);
					string namespaceURI = row.NamespaceURI;
					DataColumn dataColumn2 = table.Columns[text2, namespaceURI];
					if (dataColumn2 == null)
					{
						while (row.NodeType != XmlNodeType.EndElement && row.LocalName != text2 && row.NamespaceURI != namespaceURI)
						{
							row.Read();
						}
						row.Read();
					}
					else if (dataColumn2.IsCustomType)
					{
						bool flag = dataColumn2.DataType == typeof(object) || row.GetAttribute("InstanceType", "urn:schemas-microsoft-com:xml-msdata") != null || row.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance") != null;
						bool flag2 = false;
						if (dataColumn2.Table.DataSet != null && dataColumn2.Table.DataSet._udtIsWrapped)
						{
							row.Read();
							flag2 = true;
						}
						XmlRootAttribute xmlRootAttribute = null;
						if (!flag && !dataColumn2.ImplementsIXMLSerializable)
						{
							if (flag2)
							{
								xmlRootAttribute = new XmlRootAttribute(row.LocalName);
								xmlRootAttribute.Namespace = row.NamespaceURI;
							}
							else
							{
								xmlRootAttribute = new XmlRootAttribute(dataColumn2.EncodedColumnName);
								xmlRootAttribute.Namespace = dataColumn2.Namespace;
							}
						}
						dataColumn2[num] = dataColumn2.ConvertXmlToObject(row, xmlRootAttribute);
						if (flag2)
						{
							row.Read();
						}
					}
					else
					{
						int depth3 = row.Depth;
						row.Read();
						if (row.Depth > depth3)
						{
							if (row.NodeType == XmlNodeType.Text || row.NodeType == XmlNodeType.Whitespace || row.NodeType == XmlNodeType.SignificantWhitespace)
							{
								string text3 = row.ReadString();
								dataColumn2[num] = dataColumn2.ConvertXmlToObject(text3);
								row.Read();
							}
						}
						else if (dataColumn2.DataType == typeof(string))
						{
							dataColumn2[num] = string.Empty;
						}
					}
				}
			}
			row.Read();
			this.SkipWhitespaces(row);
			return num;
		}

		internal void SkipWhitespaces(XmlReader reader)
		{
			while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.SignificantWhitespace)
			{
				reader.Read();
			}
		}

		private ArrayList _tables;

		private DataSet _dataSet;

		private DataTable _dataTable;
	}
}
