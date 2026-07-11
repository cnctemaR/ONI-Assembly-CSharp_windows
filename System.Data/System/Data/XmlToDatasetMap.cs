using System;
using System.Collections;
using System.Xml;

namespace System.Data
{
	internal sealed class XmlToDatasetMap
	{
		public XmlToDatasetMap(DataSet dataSet, XmlNameTable nameTable)
		{
			this.BuildIdentityMap(dataSet, nameTable);
		}

		public XmlToDatasetMap(XmlNameTable nameTable, DataSet dataSet)
		{
			this.BuildIdentityMap(nameTable, dataSet);
		}

		public XmlToDatasetMap(DataTable dataTable, XmlNameTable nameTable)
		{
			this.BuildIdentityMap(dataTable, nameTable);
		}

		public XmlToDatasetMap(XmlNameTable nameTable, DataTable dataTable)
		{
			this.BuildIdentityMap(nameTable, dataTable);
		}

		internal static bool IsMappedColumn(DataColumn c)
		{
			return c.ColumnMapping != MappingType.Hidden;
		}

		private XmlToDatasetMap.TableSchemaInfo AddTableSchema(DataTable table, XmlNameTable nameTable)
		{
			string text = nameTable.Get(table.EncodedTableName);
			string text2 = nameTable.Get(table.Namespace);
			if (text == null)
			{
				return null;
			}
			XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = new XmlToDatasetMap.TableSchemaInfo(table);
			this._tableSchemaMap[new XmlToDatasetMap.XmlNodeIdentety(text, text2)] = tableSchemaInfo;
			return tableSchemaInfo;
		}

		private XmlToDatasetMap.TableSchemaInfo AddTableSchema(XmlNameTable nameTable, DataTable table)
		{
			string encodedTableName = table.EncodedTableName;
			string text = nameTable.Get(encodedTableName);
			if (text == null)
			{
				text = nameTable.Add(encodedTableName);
			}
			table._encodedTableName = text;
			string text2 = nameTable.Get(table.Namespace);
			if (text2 == null)
			{
				text2 = nameTable.Add(table.Namespace);
			}
			else if (table._tableNamespace != null)
			{
				table._tableNamespace = text2;
			}
			XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = new XmlToDatasetMap.TableSchemaInfo(table);
			this._tableSchemaMap[new XmlToDatasetMap.XmlNodeIdentety(text, text2)] = tableSchemaInfo;
			return tableSchemaInfo;
		}

		private bool AddColumnSchema(DataColumn col, XmlNameTable nameTable, XmlToDatasetMap.XmlNodeIdHashtable columns)
		{
			string text = nameTable.Get(col.EncodedColumnName);
			string text2 = nameTable.Get(col.Namespace);
			if (text == null)
			{
				return false;
			}
			XmlToDatasetMap.XmlNodeIdentety xmlNodeIdentety = new XmlToDatasetMap.XmlNodeIdentety(text, text2);
			columns[xmlNodeIdentety] = col;
			if (col.ColumnName.StartsWith("xml", StringComparison.OrdinalIgnoreCase))
			{
				this.HandleSpecialColumn(col, nameTable, columns);
			}
			return true;
		}

		private bool AddColumnSchema(XmlNameTable nameTable, DataColumn col, XmlToDatasetMap.XmlNodeIdHashtable columns)
		{
			string text = XmlConvert.EncodeLocalName(col.ColumnName);
			string text2 = nameTable.Get(text);
			if (text2 == null)
			{
				text2 = nameTable.Add(text);
			}
			col._encodedColumnName = text2;
			string text3 = nameTable.Get(col.Namespace);
			if (text3 == null)
			{
				text3 = nameTable.Add(col.Namespace);
			}
			else if (col._columnUri != null)
			{
				col._columnUri = text3;
			}
			XmlToDatasetMap.XmlNodeIdentety xmlNodeIdentety = new XmlToDatasetMap.XmlNodeIdentety(text2, text3);
			columns[xmlNodeIdentety] = col;
			if (col.ColumnName.StartsWith("xml", StringComparison.OrdinalIgnoreCase))
			{
				this.HandleSpecialColumn(col, nameTable, columns);
			}
			return true;
		}

		private void BuildIdentityMap(DataSet dataSet, XmlNameTable nameTable)
		{
			this._tableSchemaMap = new XmlToDatasetMap.XmlNodeIdHashtable(dataSet.Tables.Count);
			foreach (object obj in dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = this.AddTableSchema(dataTable, nameTable);
				if (tableSchemaInfo != null)
				{
					foreach (object obj2 in dataTable.Columns)
					{
						DataColumn dataColumn = (DataColumn)obj2;
						if (XmlToDatasetMap.IsMappedColumn(dataColumn))
						{
							this.AddColumnSchema(dataColumn, nameTable, tableSchemaInfo.ColumnsSchemaMap);
						}
					}
				}
			}
		}

		private void BuildIdentityMap(XmlNameTable nameTable, DataSet dataSet)
		{
			this._tableSchemaMap = new XmlToDatasetMap.XmlNodeIdHashtable(dataSet.Tables.Count);
			string text = nameTable.Get(dataSet.Namespace);
			if (text == null)
			{
				text = nameTable.Add(dataSet.Namespace);
			}
			dataSet._namespaceURI = text;
			foreach (object obj in dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = this.AddTableSchema(nameTable, dataTable);
				if (tableSchemaInfo != null)
				{
					foreach (object obj2 in dataTable.Columns)
					{
						DataColumn dataColumn = (DataColumn)obj2;
						if (XmlToDatasetMap.IsMappedColumn(dataColumn))
						{
							this.AddColumnSchema(nameTable, dataColumn, tableSchemaInfo.ColumnsSchemaMap);
						}
					}
					foreach (object obj3 in dataTable.ChildRelations)
					{
						DataRelation dataRelation = (DataRelation)obj3;
						if (dataRelation.Nested)
						{
							string text2 = XmlConvert.EncodeLocalName(dataRelation.ChildTable.TableName);
							string text3 = nameTable.Get(text2);
							if (text3 == null)
							{
								text3 = nameTable.Add(text2);
							}
							string text4 = nameTable.Get(dataRelation.ChildTable.Namespace);
							if (text4 == null)
							{
								text4 = nameTable.Add(dataRelation.ChildTable.Namespace);
							}
							XmlToDatasetMap.XmlNodeIdentety xmlNodeIdentety = new XmlToDatasetMap.XmlNodeIdentety(text3, text4);
							tableSchemaInfo.ColumnsSchemaMap[xmlNodeIdentety] = dataRelation.ChildTable;
						}
					}
				}
			}
		}

		private void BuildIdentityMap(DataTable dataTable, XmlNameTable nameTable)
		{
			this._tableSchemaMap = new XmlToDatasetMap.XmlNodeIdHashtable(1);
			XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = this.AddTableSchema(dataTable, nameTable);
			if (tableSchemaInfo != null)
			{
				foreach (object obj in dataTable.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					if (XmlToDatasetMap.IsMappedColumn(dataColumn))
					{
						this.AddColumnSchema(dataColumn, nameTable, tableSchemaInfo.ColumnsSchemaMap);
					}
				}
			}
		}

		private void BuildIdentityMap(XmlNameTable nameTable, DataTable dataTable)
		{
			ArrayList selfAndDescendants = this.GetSelfAndDescendants(dataTable);
			this._tableSchemaMap = new XmlToDatasetMap.XmlNodeIdHashtable(selfAndDescendants.Count);
			foreach (object obj in selfAndDescendants)
			{
				DataTable dataTable2 = (DataTable)obj;
				XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = this.AddTableSchema(nameTable, dataTable2);
				if (tableSchemaInfo != null)
				{
					foreach (object obj2 in dataTable2.Columns)
					{
						DataColumn dataColumn = (DataColumn)obj2;
						if (XmlToDatasetMap.IsMappedColumn(dataColumn))
						{
							this.AddColumnSchema(nameTable, dataColumn, tableSchemaInfo.ColumnsSchemaMap);
						}
					}
					foreach (object obj3 in dataTable2.ChildRelations)
					{
						DataRelation dataRelation = (DataRelation)obj3;
						if (dataRelation.Nested)
						{
							string text = XmlConvert.EncodeLocalName(dataRelation.ChildTable.TableName);
							string text2 = nameTable.Get(text);
							if (text2 == null)
							{
								text2 = nameTable.Add(text);
							}
							string text3 = nameTable.Get(dataRelation.ChildTable.Namespace);
							if (text3 == null)
							{
								text3 = nameTable.Add(dataRelation.ChildTable.Namespace);
							}
							XmlToDatasetMap.XmlNodeIdentety xmlNodeIdentety = new XmlToDatasetMap.XmlNodeIdentety(text2, text3);
							tableSchemaInfo.ColumnsSchemaMap[xmlNodeIdentety] = dataRelation.ChildTable;
						}
					}
				}
			}
		}

		private ArrayList GetSelfAndDescendants(DataTable dt)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(dt);
			for (int i = 0; i < arrayList.Count; i++)
			{
				foreach (object obj in ((DataTable)arrayList[i]).ChildRelations)
				{
					DataRelation dataRelation = (DataRelation)obj;
					if (!arrayList.Contains(dataRelation.ChildTable))
					{
						arrayList.Add(dataRelation.ChildTable);
					}
				}
			}
			return arrayList;
		}

		public object GetColumnSchema(XmlNode node, bool fIgnoreNamespace)
		{
			XmlNode xmlNode = ((node.NodeType == XmlNodeType.Attribute) ? ((XmlAttribute)node).OwnerElement : node.ParentNode);
			while (xmlNode != null && xmlNode.NodeType == XmlNodeType.Element)
			{
				XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = (XmlToDatasetMap.TableSchemaInfo)(fIgnoreNamespace ? this._tableSchemaMap[xmlNode.LocalName] : this._tableSchemaMap[xmlNode]);
				xmlNode = xmlNode.ParentNode;
				if (tableSchemaInfo != null)
				{
					if (fIgnoreNamespace)
					{
						return tableSchemaInfo.ColumnsSchemaMap[node.LocalName];
					}
					return tableSchemaInfo.ColumnsSchemaMap[node];
				}
			}
			return null;
		}

		public object GetColumnSchema(DataTable table, XmlReader dataReader, bool fIgnoreNamespace)
		{
			if (this._lastTableSchemaInfo == null || this._lastTableSchemaInfo.TableSchema != table)
			{
				this._lastTableSchemaInfo = (XmlToDatasetMap.TableSchemaInfo)(fIgnoreNamespace ? this._tableSchemaMap[table.EncodedTableName] : this._tableSchemaMap[table]);
			}
			if (fIgnoreNamespace)
			{
				return this._lastTableSchemaInfo.ColumnsSchemaMap[dataReader.LocalName];
			}
			return this._lastTableSchemaInfo.ColumnsSchemaMap[dataReader];
		}

		public object GetSchemaForNode(XmlNode node, bool fIgnoreNamespace)
		{
			XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = null;
			if (node.NodeType == XmlNodeType.Element)
			{
				tableSchemaInfo = (XmlToDatasetMap.TableSchemaInfo)(fIgnoreNamespace ? this._tableSchemaMap[node.LocalName] : this._tableSchemaMap[node]);
			}
			if (tableSchemaInfo != null)
			{
				return tableSchemaInfo.TableSchema;
			}
			return this.GetColumnSchema(node, fIgnoreNamespace);
		}

		public DataTable GetTableForNode(XmlReader node, bool fIgnoreNamespace)
		{
			XmlToDatasetMap.TableSchemaInfo tableSchemaInfo = (XmlToDatasetMap.TableSchemaInfo)(fIgnoreNamespace ? this._tableSchemaMap[node.LocalName] : this._tableSchemaMap[node]);
			if (tableSchemaInfo != null)
			{
				this._lastTableSchemaInfo = tableSchemaInfo;
				return this._lastTableSchemaInfo.TableSchema;
			}
			return null;
		}

		private void HandleSpecialColumn(DataColumn col, XmlNameTable nameTable, XmlToDatasetMap.XmlNodeIdHashtable columns)
		{
			string text;
			if ('x' == col.ColumnName[0])
			{
				text = "_x0078_";
			}
			else
			{
				text = "_x0058_";
			}
			text += col.ColumnName.Substring(1);
			if (nameTable.Get(text) == null)
			{
				nameTable.Add(text);
			}
			string text2 = nameTable.Get(col.Namespace);
			XmlToDatasetMap.XmlNodeIdentety xmlNodeIdentety = new XmlToDatasetMap.XmlNodeIdentety(text, text2);
			columns[xmlNodeIdentety] = col;
		}

		private XmlToDatasetMap.XmlNodeIdHashtable _tableSchemaMap;

		private XmlToDatasetMap.TableSchemaInfo _lastTableSchemaInfo;

		private sealed class XmlNodeIdentety
		{
			public XmlNodeIdentety(string localName, string namespaceURI)
			{
				this.LocalName = localName;
				this.NamespaceURI = namespaceURI;
			}

			public override int GetHashCode()
			{
				return this.LocalName.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				XmlToDatasetMap.XmlNodeIdentety xmlNodeIdentety = (XmlToDatasetMap.XmlNodeIdentety)obj;
				return string.Equals(this.LocalName, xmlNodeIdentety.LocalName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.NamespaceURI, xmlNodeIdentety.NamespaceURI, StringComparison.OrdinalIgnoreCase);
			}

			public string LocalName;

			public string NamespaceURI;
		}

		internal sealed class XmlNodeIdHashtable : Hashtable
		{
			public XmlNodeIdHashtable(int capacity)
				: base(capacity)
			{
			}

			public object this[XmlNode node]
			{
				get
				{
					this._id.LocalName = node.LocalName;
					this._id.NamespaceURI = node.NamespaceURI;
					return this[this._id];
				}
			}

			public object this[XmlReader dataReader]
			{
				get
				{
					this._id.LocalName = dataReader.LocalName;
					this._id.NamespaceURI = dataReader.NamespaceURI;
					return this[this._id];
				}
			}

			public object this[DataTable table]
			{
				get
				{
					this._id.LocalName = table.EncodedTableName;
					this._id.NamespaceURI = table.Namespace;
					return this[this._id];
				}
			}

			public object this[string name]
			{
				get
				{
					this._id.LocalName = name;
					this._id.NamespaceURI = string.Empty;
					return this[this._id];
				}
			}

			private XmlToDatasetMap.XmlNodeIdentety _id = new XmlToDatasetMap.XmlNodeIdentety(string.Empty, string.Empty);
		}

		private sealed class TableSchemaInfo
		{
			public TableSchemaInfo(DataTable tableSchema)
			{
				this.TableSchema = tableSchema;
				this.ColumnsSchemaMap = new XmlToDatasetMap.XmlNodeIdHashtable(tableSchema.Columns.Count);
			}

			public DataTable TableSchema;

			public XmlToDatasetMap.XmlNodeIdHashtable ColumnsSchemaMap;
		}
	}
}
