using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	internal class XmlDataInferenceLoader
	{
		private XmlDataInferenceLoader(DataSet ds, XmlDocument doc, XmlReadMode mode, string[] ignoredNamespaces)
		{
			this.dataset = ds;
			this.document = doc;
			this.mode = mode;
			this.ignoredNamespaces = ((ignoredNamespaces == null) ? new ArrayList() : new ArrayList(ignoredNamespaces));
			foreach (object obj in this.dataset.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				this.tables.Add(new TableMapping(dataTable));
			}
		}

		public static void Infer(DataSet dataset, XmlDocument document, XmlReadMode mode, string[] ignoredNamespaces)
		{
			new XmlDataInferenceLoader(dataset, document, mode, ignoredNamespaces).ReadXml();
		}

		private void ReadXml()
		{
			if (this.document.DocumentElement == null)
			{
				return;
			}
			this.dataset.Locale = new CultureInfo("en-US");
			XmlElement documentElement = this.document.DocumentElement;
			if (documentElement.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				throw new InvalidOperationException("DataSet is not designed to handle XML Schema as data content. Please use ReadXmlSchema method instead of InferXmlSchema method.");
			}
			if (this.IsDocumentElementTable())
			{
				this.InferTopLevelTable(documentElement);
			}
			else
			{
				string text = XmlHelper.Decode(documentElement.LocalName);
				this.dataset.DataSetName = text;
				this.dataset.Namespace = documentElement.NamespaceURI;
				this.dataset.Prefix = documentElement.Prefix;
				foreach (object obj in documentElement.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode.NamespaceURI == "http://www.w3.org/2001/XMLSchema"))
					{
						if (xmlNode.NodeType == XmlNodeType.Element)
						{
							this.InferTopLevelTable(xmlNode as XmlElement);
						}
					}
				}
			}
			int num = 0;
			foreach (object obj2 in this.tables)
			{
				TableMapping tableMapping = (TableMapping)obj2;
				string text2 = ((tableMapping.PrimaryKey == null) ? (tableMapping.Table.TableName + "_Id") : tableMapping.PrimaryKey.ColumnName);
				string text3 = text2;
				if (tableMapping.ChildTables[tableMapping.Table.TableName] != null)
				{
					text3 = text2 + '_' + num;
					while (tableMapping.GetColumn(text3) != null)
					{
						num++;
						text3 = text2 + '_' + num;
					}
				}
				foreach (object obj3 in tableMapping.ChildTables)
				{
					TableMapping tableMapping2 = (TableMapping)obj3;
					tableMapping2.ReferenceKey = this.GetMappedColumn(tableMapping2, text3, tableMapping.Table.Prefix, tableMapping.Table.Namespace, MappingType.Hidden, (tableMapping.PrimaryKey == null) ? typeof(int) : tableMapping.PrimaryKey.DataType);
				}
			}
			foreach (object obj4 in this.tables)
			{
				TableMapping tableMapping3 = (TableMapping)obj4;
				if (!tableMapping3.ExistsInDataSet)
				{
					if (tableMapping3.PrimaryKey != null)
					{
						tableMapping3.Table.Columns.Add(tableMapping3.PrimaryKey);
					}
					foreach (object obj5 in tableMapping3.Elements)
					{
						DataColumn dataColumn = (DataColumn)obj5;
						tableMapping3.Table.Columns.Add(dataColumn);
					}
					foreach (object obj6 in tableMapping3.Attributes)
					{
						DataColumn dataColumn2 = (DataColumn)obj6;
						tableMapping3.Table.Columns.Add(dataColumn2);
					}
					if (tableMapping3.SimpleContent != null)
					{
						tableMapping3.Table.Columns.Add(tableMapping3.SimpleContent);
					}
					if (tableMapping3.ReferenceKey != null)
					{
						tableMapping3.Table.Columns.Add(tableMapping3.ReferenceKey);
					}
					this.dataset.Tables.Add(tableMapping3.Table);
				}
			}
			foreach (object obj7 in this.relations)
			{
				RelationStructure relationStructure = (RelationStructure)obj7;
				string text4 = ((relationStructure.ExplicitName == null) ? (relationStructure.ParentTableName + "_" + relationStructure.ChildTableName) : relationStructure.ExplicitName);
				DataTable dataTable = this.dataset.Tables[relationStructure.ParentTableName];
				DataTable dataTable2 = this.dataset.Tables[relationStructure.ChildTableName];
				DataColumn dataColumn3 = dataTable.Columns[relationStructure.ParentColumnName];
				DataColumn dataColumn4 = null;
				if (relationStructure.ParentTableName == relationStructure.ChildTableName)
				{
					dataColumn4 = dataTable2.Columns[relationStructure.ChildColumnName + "_" + num];
				}
				if (dataColumn4 == null)
				{
					dataColumn4 = dataTable2.Columns[relationStructure.ChildColumnName];
				}
				if (dataTable == null)
				{
					throw new DataException("Parent table was not found : " + relationStructure.ParentTableName);
				}
				if (dataTable2 == null)
				{
					throw new DataException("Child table was not found : " + relationStructure.ChildTableName);
				}
				if (dataColumn3 == null)
				{
					throw new DataException("Parent column was not found :" + relationStructure.ParentColumnName);
				}
				if (dataColumn4 == null)
				{
					throw new DataException("Child column was not found :" + relationStructure.ChildColumnName);
				}
				DataRelation dataRelation = new DataRelation(text4, dataColumn3, dataColumn4, relationStructure.CreateConstraint);
				if (relationStructure.IsNested)
				{
					dataRelation.Nested = true;
					dataRelation.ParentTable.PrimaryKey = dataRelation.ParentColumns;
				}
				this.dataset.Relations.Add(dataRelation);
			}
		}

		private void InferTopLevelTable(XmlElement el)
		{
			this.InferTableElement(null, el);
		}

		private void InferColumnElement(TableMapping table, XmlElement el)
		{
			string text = XmlHelper.Decode(el.LocalName);
			DataColumn dataColumn = table.GetColumn(text);
			if (dataColumn != null)
			{
				if (dataColumn.ColumnMapping != MappingType.Element)
				{
					throw new DataException(string.Format("Column {0} is already mapped to {1}.", text, dataColumn.ColumnMapping));
				}
				table.lastElementIndex = table.Elements.IndexOf(dataColumn);
				return;
			}
			else
			{
				if (table.ChildTables[text] != null)
				{
					return;
				}
				dataColumn = new DataColumn(text, typeof(string));
				dataColumn.Namespace = el.NamespaceURI;
				dataColumn.Prefix = el.Prefix;
				table.Elements.Insert(++table.lastElementIndex, dataColumn);
				return;
			}
		}

		private void CheckExtraneousElementColumn(TableMapping parentTable, XmlElement el)
		{
			if (parentTable == null)
			{
				return;
			}
			string text = XmlHelper.Decode(el.LocalName);
			DataColumn column = parentTable.GetColumn(text);
			if (column != null)
			{
				parentTable.RemoveElementColumn(text);
			}
		}

		private void PopulatePrimaryKey(TableMapping table)
		{
			table.PrimaryKey = new DataColumn(table.Table.TableName + "_Id")
			{
				ColumnMapping = MappingType.Hidden,
				DataType = typeof(int),
				AllowDBNull = false,
				AutoIncrement = true,
				Namespace = table.Table.Namespace,
				Prefix = table.Table.Prefix
			};
		}

		private void PopulateRelationStructure(string parent, string child, string pkeyColumn)
		{
			if (this.relations[parent, child] != null)
			{
				return;
			}
			RelationStructure relationStructure = new RelationStructure();
			relationStructure.ParentTableName = parent;
			relationStructure.ChildTableName = child;
			relationStructure.ParentColumnName = pkeyColumn;
			relationStructure.ChildColumnName = pkeyColumn;
			relationStructure.CreateConstraint = true;
			relationStructure.IsNested = true;
			this.relations.Add(relationStructure);
		}

		private void InferRepeatedElement(TableMapping parentTable, XmlElement el)
		{
			string text = XmlHelper.Decode(el.LocalName);
			this.CheckExtraneousElementColumn(parentTable, el);
			TableMapping mappedTable = this.GetMappedTable(parentTable, text, el.NamespaceURI);
			if (mappedTable.Elements.Count > 0)
			{
				return;
			}
			if (mappedTable.SimpleContent != null)
			{
				return;
			}
			this.GetMappedColumn(mappedTable, text + "_Column", el.Prefix, el.NamespaceURI, MappingType.SimpleContent, null);
		}

		private void InferTableElement(TableMapping parentTable, XmlElement el)
		{
			this.CheckExtraneousElementColumn(parentTable, el);
			string text = XmlHelper.Decode(el.LocalName);
			TableMapping mappedTable = this.GetMappedTable(parentTable, text, el.NamespaceURI);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			foreach (object obj in el.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (!(xmlAttribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") && !(xmlAttribute.NamespaceURI == "http://www.w3.org/XML/1998/namespace"))
				{
					if (this.ignoredNamespaces == null || !this.ignoredNamespaces.Contains(xmlAttribute.NamespaceURI))
					{
						flag2 = true;
						DataColumn mappedColumn = this.GetMappedColumn(mappedTable, XmlHelper.Decode(xmlAttribute.LocalName), xmlAttribute.Prefix, xmlAttribute.NamespaceURI, MappingType.Attribute, null);
					}
				}
			}
			foreach (object obj2 in el.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.ProcessingInstruction && nodeType != XmlNodeType.Comment)
				{
					if (nodeType != XmlNodeType.Element)
					{
						flag3 = true;
						if (XmlDataInferenceLoader.GetElementMappingType(el, this.ignoredNamespaces, null) == ElementMappingType.Repeated)
						{
							flag4 = true;
						}
					}
					else
					{
						flag = true;
						XmlElement xmlElement = xmlNode as XmlElement;
						string text2 = XmlHelper.Decode(xmlElement.LocalName);
						switch (XmlDataInferenceLoader.GetElementMappingType(xmlElement, this.ignoredNamespaces, null))
						{
						case ElementMappingType.Simple:
							this.InferColumnElement(mappedTable, xmlElement);
							break;
						case ElementMappingType.Repeated:
							if (mappedTable.PrimaryKey == null)
							{
								this.PopulatePrimaryKey(mappedTable);
							}
							this.PopulateRelationStructure(mappedTable.Table.TableName, text2, mappedTable.PrimaryKey.ColumnName);
							this.InferRepeatedElement(mappedTable, xmlElement);
							break;
						case ElementMappingType.Complex:
							if (mappedTable.PrimaryKey == null)
							{
								this.PopulatePrimaryKey(mappedTable);
							}
							this.PopulateRelationStructure(mappedTable.Table.TableName, text2, mappedTable.PrimaryKey.ColumnName);
							this.InferTableElement(mappedTable, xmlElement);
							break;
						}
					}
				}
			}
			if (mappedTable.SimpleContent == null && !flag && flag3 && (flag2 || flag4))
			{
				this.GetMappedColumn(mappedTable, mappedTable.Table.TableName + "_Text", string.Empty, string.Empty, MappingType.SimpleContent, null);
			}
		}

		private TableMapping GetMappedTable(TableMapping parent, string tableName, string ns)
		{
			TableMapping tableMapping = this.tables[tableName];
			if (tableMapping != null)
			{
				if (parent != null && tableMapping.ParentTable != null && tableMapping.ParentTable != parent)
				{
					throw new DataException(string.Format("The table '{0}' is already allocated as a child of another table '{1}'. Cannot set table '{2}' as parent table.", tableName, tableMapping.ParentTable.Table.TableName, parent.Table.TableName));
				}
			}
			else
			{
				tableMapping = new TableMapping(tableName, ns);
				tableMapping.ParentTable = parent;
				this.tables.Add(tableMapping);
			}
			if (parent != null)
			{
				bool flag = true;
				foreach (object obj in parent.ChildTables)
				{
					TableMapping tableMapping2 = (TableMapping)obj;
					if (tableMapping2.Table.TableName == tableName)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					parent.ChildTables.Add(tableMapping);
				}
			}
			return tableMapping;
		}

		private DataColumn GetMappedColumn(TableMapping table, string name, string prefix, string ns, MappingType type, Type optColType)
		{
			DataColumn dataColumn = table.GetColumn(name);
			if (dataColumn == null)
			{
				dataColumn = new DataColumn(name);
				dataColumn.Prefix = prefix;
				dataColumn.Namespace = ns;
				dataColumn.ColumnMapping = type;
				switch (type)
				{
				case MappingType.Element:
					table.Elements.Add(dataColumn);
					break;
				case MappingType.Attribute:
					table.Attributes.Add(dataColumn);
					break;
				case MappingType.SimpleContent:
					table.SimpleContent = dataColumn;
					break;
				case MappingType.Hidden:
					dataColumn.DataType = optColType;
					table.ReferenceKey = dataColumn;
					break;
				}
			}
			else if (dataColumn.ColumnMapping != type)
			{
				throw new DataException(string.Format("There are already another column that has different mapping type. Column is {0}, existing mapping type is {1}", dataColumn.ColumnName, dataColumn.ColumnMapping));
			}
			return dataColumn;
		}

		private static void SetAsExistingTable(XmlElement el, Hashtable existingTables)
		{
			if (existingTables == null)
			{
				return;
			}
			ArrayList arrayList = existingTables[el.NamespaceURI] as ArrayList;
			if (arrayList == null)
			{
				arrayList = new ArrayList();
				existingTables[el.NamespaceURI] = arrayList;
			}
			if (arrayList.Contains(el.LocalName))
			{
				return;
			}
			arrayList.Add(el.LocalName);
		}

		private static ElementMappingType GetElementMappingType(XmlElement el, ArrayList ignoredNamespaces, Hashtable existingTables)
		{
			if (existingTables != null)
			{
				ArrayList arrayList = existingTables[el.NamespaceURI] as ArrayList;
				if (arrayList != null && arrayList.Contains(el.LocalName))
				{
					return ElementMappingType.Complex;
				}
			}
			foreach (object obj in el.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (!(xmlAttribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") && !(xmlAttribute.NamespaceURI == "http://www.w3.org/XML/1998/namespace"))
				{
					if (ignoredNamespaces == null || !ignoredNamespaces.Contains(xmlAttribute.NamespaceURI))
					{
						XmlDataInferenceLoader.SetAsExistingTable(el, existingTables);
						return ElementMappingType.Complex;
					}
				}
			}
			foreach (object obj2 in el.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					XmlDataInferenceLoader.SetAsExistingTable(el, existingTables);
					return ElementMappingType.Complex;
				}
			}
			for (XmlNode xmlNode2 = el.NextSibling; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
			{
				if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.LocalName == el.LocalName)
				{
					XmlDataInferenceLoader.SetAsExistingTable(el, existingTables);
					return (XmlDataInferenceLoader.GetElementMappingType(xmlNode2 as XmlElement, ignoredNamespaces, null) != ElementMappingType.Complex) ? ElementMappingType.Repeated : ElementMappingType.Complex;
				}
			}
			return ElementMappingType.Simple;
		}

		private bool IsDocumentElementTable()
		{
			return XmlDataInferenceLoader.IsDocumentElementTable(this.document.DocumentElement, this.ignoredNamespaces);
		}

		internal static bool IsDocumentElementTable(XmlElement top, ArrayList ignoredNamespaces)
		{
			foreach (object obj in top.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (!(xmlAttribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") && !(xmlAttribute.NamespaceURI == "http://www.w3.org/XML/1998/namespace"))
				{
					if (ignoredNamespaces == null || !ignoredNamespaces.Contains(xmlAttribute.NamespaceURI))
					{
						return true;
					}
				}
			}
			Hashtable hashtable = new Hashtable();
			foreach (object obj2 in top.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					if (XmlDataInferenceLoader.GetElementMappingType(xmlElement, ignoredNamespaces, hashtable) == ElementMappingType.Simple)
					{
						return true;
					}
				}
			}
			return false;
		}

		private DataSet dataset;

		private XmlDocument document;

		private XmlReadMode mode;

		private ArrayList ignoredNamespaces;

		private TableMappingCollection tables = new TableMappingCollection();

		private RelationStructureCollection relations = new RelationStructureCollection();
	}
}
