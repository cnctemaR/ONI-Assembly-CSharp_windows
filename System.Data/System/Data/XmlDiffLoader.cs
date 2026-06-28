using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	internal class XmlDiffLoader
	{
		public XmlDiffLoader(DataSet DSet)
		{
			this.DSet = DSet;
		}

		public XmlDiffLoader(DataTable table)
		{
			this.table = table;
		}

		public void Load(XmlReader reader)
		{
			bool flag = false;
			if (this.DSet != null)
			{
				flag = this.DSet.EnforceConstraints;
				this.DSet.EnforceConstraints = false;
			}
			reader.MoveToContent();
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement("diffgram", "urn:schemas-microsoft-com:xml-diffgram-v1");
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.LocalName == "before" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
					{
						this.LoadBefore(reader);
					}
					else if (reader.LocalName == "errors" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
					{
						this.LoadErrors(reader);
					}
					else
					{
						this.LoadCurrent(reader);
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
			if (this.DSet != null)
			{
				this.DSet.EnforceConstraints = flag;
			}
		}

		private void LoadCurrent(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					DataTable dataTable = this.GetTable(reader.LocalName);
					if (dataTable != null)
					{
						this.LoadCurrentTable(dataTable, reader);
					}
					else
					{
						reader.Skip();
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
		}

		private void LoadBefore(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					DataTable dataTable = this.GetTable(reader.LocalName);
					if (dataTable == null)
					{
						throw new DataException(Locale.GetText("Cannot load diffGram. Table '" + reader.LocalName + "' is missing in the destination dataset"));
					}
					this.LoadBeforeTable(dataTable, reader);
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
		}

		private void LoadErrors(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					DataRow dataRow = null;
					string attribute = reader.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
					if (attribute != null)
					{
						dataRow = (DataRow)this.ErrorRows[attribute];
					}
					if (!reader.IsEmptyElement)
					{
						reader.ReadStartElement();
						while (reader.NodeType != XmlNodeType.EndElement)
						{
							if (reader.NodeType == XmlNodeType.Element)
							{
								string attribute2 = reader.GetAttribute("Error", "urn:schemas-microsoft-com:xml-diffgram-v1");
								dataRow.SetColumnError(reader.LocalName, attribute2);
							}
							reader.Read();
						}
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
		}

		private void LoadColumns(DataTable Table, DataRow Row, XmlReader reader, DataRowVersion loadType)
		{
			this.LoadColumnAttributes(Table, Row, reader, loadType);
			this.LoadColumnChildren(Table, Row, reader, loadType);
		}

		private void LoadColumnAttributes(DataTable Table, DataRow Row, XmlReader reader, DataRowVersion loadType)
		{
			if (!reader.HasAttributes || !reader.MoveToFirstAttribute())
			{
				return;
			}
			for (;;)
			{
				string namespaceURI = reader.NamespaceURI;
				if (namespaceURI == null)
				{
					goto IL_00A6;
				}
				if (XmlDiffLoader.<>f__switch$map0 == null)
				{
					XmlDiffLoader.<>f__switch$map0 = new Dictionary<string, int>(6)
					{
						{ "http://www.w3.org/2000/xmlns/", 0 },
						{ "http://www.w3.org/XML/1998/namespace", 0 },
						{ "urn:schemas-microsoft-com:xml-diffgram-v1", 0 },
						{ "urn:schemas-microsoft-com:xml-msdata", 0 },
						{ "urn:schemas-microsoft-com:xml-msprop", 0 },
						{ "http://www.w3.org/2001/XMLSchema", 0 }
					};
				}
				int num;
				if (!XmlDiffLoader.<>f__switch$map0.TryGetValue(namespaceURI, out num))
				{
					goto IL_00A6;
				}
				if (num != 0)
				{
					goto IL_00A6;
				}
				IL_0142:
				if (!reader.MoveToNextAttribute())
				{
					break;
				}
				continue;
				IL_00A6:
				DataColumn dataColumn = Table.Columns[XmlHelper.Decode(reader.LocalName)];
				if (dataColumn == null || dataColumn.ColumnMapping != MappingType.Attribute)
				{
					goto IL_0142;
				}
				if ((dataColumn.Namespace != null || !(reader.NamespaceURI == string.Empty)) && !(dataColumn.Namespace == reader.NamespaceURI))
				{
					goto IL_0142;
				}
				object obj = XmlDataLoader.StringToObject(dataColumn.DataType, reader.Value);
				if (loadType == DataRowVersion.Current)
				{
					Row[dataColumn] = obj;
					goto IL_0142;
				}
				Row.SetOriginalValue(dataColumn.ColumnName, obj);
				goto IL_0142;
			}
			reader.MoveToElement();
		}

		private void LoadColumnChildren(DataTable Table, DataRow Row, XmlReader reader, DataRowVersion loadType)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType != XmlNodeType.Element)
				{
					reader.Read();
				}
				else
				{
					string text = XmlHelper.Decode(reader.LocalName);
					if (Table.Columns.Contains(text))
					{
						object obj = XmlDataLoader.StringToObject(Table.Columns[text].DataType, reader.ReadString());
						if (loadType == DataRowVersion.Current)
						{
							Row[text] = obj;
						}
						else
						{
							Row.SetOriginalValue(text, obj);
						}
						reader.Read();
					}
					else
					{
						DataTable dataTable = this.GetTable(reader.LocalName);
						if (dataTable != null)
						{
							if (loadType == DataRowVersion.Original)
							{
								this.LoadBeforeTable(dataTable, reader);
							}
							else if (loadType == DataRowVersion.Current)
							{
								this.LoadCurrentTable(dataTable, reader);
							}
						}
						else
						{
							reader.Skip();
						}
					}
				}
			}
			reader.ReadEndElement();
		}

		private void LoadBeforeTable(DataTable Table, XmlReader reader)
		{
			string attribute = reader.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
			string attribute2 = reader.GetAttribute("rowOrder", "urn:schemas-microsoft-com:xml-msdata");
			DataRow dataRow = (DataRow)this.DiffGrRows[attribute];
			if (dataRow == null)
			{
				dataRow = Table.NewRow();
				this.LoadColumns(Table, dataRow, reader, DataRowVersion.Current);
				Table.Rows.InsertAt(dataRow, int.Parse(attribute2));
				dataRow.AcceptChanges();
				dataRow.Delete();
			}
			else
			{
				this.LoadColumns(Table, dataRow, reader, DataRowVersion.Original);
			}
		}

		private void LoadCurrentTable(DataTable Table, XmlReader reader)
		{
			DataRow dataRow = Table.NewRow();
			string attribute = reader.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
			string attribute2 = reader.GetAttribute("hasErrors");
			string attribute3 = reader.GetAttribute("hasChanges", "urn:schemas-microsoft-com:xml-diffgram-v1");
			DataRowState dataRowState;
			if (attribute3 != null)
			{
				if (string.Compare(attribute3, "modified", true, CultureInfo.InvariantCulture) == 0)
				{
					this.DiffGrRows.Add(attribute, dataRow);
					dataRowState = DataRowState.Modified;
				}
				else
				{
					if (string.Compare(attribute3, "inserted", true, CultureInfo.InvariantCulture) != 0)
					{
						throw new InvalidOperationException("Invalid row change state");
					}
					dataRowState = DataRowState.Added;
				}
			}
			else
			{
				dataRowState = DataRowState.Unchanged;
			}
			if (attribute2 != null && string.Compare(attribute2, "true", true, CultureInfo.InvariantCulture) == 0)
			{
				this.ErrorRows.Add(attribute, dataRow);
			}
			this.LoadColumns(Table, dataRow, reader, DataRowVersion.Current);
			Table.Rows.Add(dataRow);
			if (dataRowState != DataRowState.Added)
			{
				dataRow.AcceptChanges();
			}
		}

		private DataTable GetTable(string name)
		{
			if (this.DSet != null)
			{
				return this.DSet.Tables[name];
			}
			if (name == this.table.TableName)
			{
				return this.table;
			}
			return null;
		}

		private DataSet DSet;

		private DataTable table;

		private Hashtable DiffGrRows = new Hashtable();

		private Hashtable ErrorRows = new Hashtable();
	}
}
