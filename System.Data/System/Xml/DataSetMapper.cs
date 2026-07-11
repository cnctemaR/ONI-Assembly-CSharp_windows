using System;
using System.Collections;
using System.Data;

namespace System.Xml
{
	internal sealed class DataSetMapper
	{
		internal DataSetMapper()
		{
			this._tableSchemaMap = new Hashtable();
			this._columnSchemaMap = new Hashtable();
		}

		internal void SetupMapping(XmlDataDocument xd, DataSet ds)
		{
			if (this.IsMapped())
			{
				this._tableSchemaMap = new Hashtable();
				this._columnSchemaMap = new Hashtable();
			}
			this._doc = xd;
			this._dataSet = ds;
			foreach (object obj in this._dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				this.AddTableSchema(dataTable);
				foreach (object obj2 in dataTable.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj2;
					if (!DataSetMapper.IsNotMapped(dataColumn))
					{
						this.AddColumnSchema(dataColumn);
					}
				}
			}
		}

		internal bool IsMapped()
		{
			return this._dataSet != null;
		}

		internal DataTable SearchMatchingTableSchema(string localName, string namespaceURI)
		{
			object identity = DataSetMapper.GetIdentity(localName, namespaceURI);
			return (DataTable)this._tableSchemaMap[identity];
		}

		internal DataTable SearchMatchingTableSchema(XmlBoundElement rowElem, XmlBoundElement elem)
		{
			DataTable dataTable = this.SearchMatchingTableSchema(elem.LocalName, elem.NamespaceURI);
			if (dataTable == null)
			{
				return null;
			}
			if (rowElem == null)
			{
				return dataTable;
			}
			if (this.GetColumnSchemaForNode(rowElem, elem) == null)
			{
				return dataTable;
			}
			using (IEnumerator enumerator = elem.Attributes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((XmlAttribute)enumerator.Current).NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						return dataTable;
					}
				}
			}
			for (XmlNode xmlNode = elem.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					return dataTable;
				}
			}
			return null;
		}

		internal DataColumn GetColumnSchemaForNode(XmlBoundElement rowElem, XmlNode node)
		{
			object identity = DataSetMapper.GetIdentity(rowElem.LocalName, rowElem.NamespaceURI);
			object identity2 = DataSetMapper.GetIdentity(node.LocalName, node.NamespaceURI);
			Hashtable hashtable = (Hashtable)this._columnSchemaMap[identity];
			if (hashtable == null)
			{
				return null;
			}
			DataColumn dataColumn = (DataColumn)hashtable[identity2];
			if (dataColumn == null)
			{
				return null;
			}
			MappingType columnMapping = dataColumn.ColumnMapping;
			if (node.NodeType == XmlNodeType.Attribute && columnMapping == MappingType.Attribute)
			{
				return dataColumn;
			}
			if (node.NodeType == XmlNodeType.Element && columnMapping == MappingType.Element)
			{
				return dataColumn;
			}
			return null;
		}

		internal DataTable GetTableSchemaForElement(XmlElement elem)
		{
			XmlBoundElement xmlBoundElement = elem as XmlBoundElement;
			if (xmlBoundElement == null)
			{
				return null;
			}
			return this.GetTableSchemaForElement(xmlBoundElement);
		}

		internal DataTable GetTableSchemaForElement(XmlBoundElement be)
		{
			DataRow row = be.Row;
			if (row == null)
			{
				return null;
			}
			return row.Table;
		}

		internal static bool IsNotMapped(DataColumn c)
		{
			return c.ColumnMapping == MappingType.Hidden;
		}

		internal DataRow GetRowFromElement(XmlElement e)
		{
			XmlBoundElement xmlBoundElement = e as XmlBoundElement;
			if (xmlBoundElement == null)
			{
				return null;
			}
			return xmlBoundElement.Row;
		}

		internal DataRow GetRowFromElement(XmlBoundElement be)
		{
			return be.Row;
		}

		internal bool GetRegion(XmlNode node, out XmlBoundElement rowElem)
		{
			while (node != null)
			{
				XmlBoundElement xmlBoundElement = node as XmlBoundElement;
				if (xmlBoundElement != null && this.GetRowFromElement(xmlBoundElement) != null)
				{
					rowElem = xmlBoundElement;
					return true;
				}
				if (node.NodeType == XmlNodeType.Attribute)
				{
					node = ((XmlAttribute)node).OwnerElement;
				}
				else
				{
					node = node.ParentNode;
				}
			}
			rowElem = null;
			return false;
		}

		internal bool IsRegionRadical(XmlBoundElement rowElem)
		{
			if (rowElem.ElementState == ElementState.Defoliated)
			{
				return true;
			}
			DataColumnCollection columns = this.GetTableSchemaForElement(rowElem).Columns;
			int num = 0;
			int count = rowElem.Attributes.Count;
			for (int i = 0; i < count; i++)
			{
				XmlAttribute xmlAttribute = rowElem.Attributes[i];
				if (!xmlAttribute.Specified)
				{
					return false;
				}
				DataColumn columnSchemaForNode = this.GetColumnSchemaForNode(rowElem, xmlAttribute);
				if (columnSchemaForNode == null)
				{
					return false;
				}
				if (!this.IsNextColumn(columns, ref num, columnSchemaForNode))
				{
					return false;
				}
				XmlNode firstChild = xmlAttribute.FirstChild;
				if (firstChild == null || firstChild.NodeType != XmlNodeType.Text || firstChild.NextSibling != null)
				{
					return false;
				}
			}
			num = 0;
			for (XmlNode xmlNode = rowElem.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType != XmlNodeType.Element)
				{
					return false;
				}
				XmlElement xmlElement = xmlNode as XmlElement;
				if (this.GetRowFromElement(xmlElement) != null)
				{
					IL_0135:
					while (xmlNode != null)
					{
						if (xmlNode.NodeType != XmlNodeType.Element)
						{
							return false;
						}
						if (this.GetRowFromElement((XmlElement)xmlNode) == null)
						{
							return false;
						}
						xmlNode = xmlNode.NextSibling;
					}
					return true;
				}
				DataColumn columnSchemaForNode2 = this.GetColumnSchemaForNode(rowElem, xmlElement);
				if (columnSchemaForNode2 == null)
				{
					return false;
				}
				if (!this.IsNextColumn(columns, ref num, columnSchemaForNode2))
				{
					return false;
				}
				if (xmlElement.HasAttributes)
				{
					return false;
				}
				XmlNode firstChild2 = xmlElement.FirstChild;
				if (firstChild2 == null || firstChild2.NodeType != XmlNodeType.Text || firstChild2.NextSibling != null)
				{
					return false;
				}
			}
			goto IL_0135;
		}

		private void AddTableSchema(DataTable table)
		{
			object identity = DataSetMapper.GetIdentity(table.EncodedTableName, table.Namespace);
			this._tableSchemaMap[identity] = table;
		}

		private void AddColumnSchema(DataColumn col)
		{
			DataTable table = col.Table;
			object identity = DataSetMapper.GetIdentity(table.EncodedTableName, table.Namespace);
			object identity2 = DataSetMapper.GetIdentity(col.EncodedColumnName, col.Namespace);
			Hashtable hashtable = (Hashtable)this._columnSchemaMap[identity];
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				this._columnSchemaMap[identity] = hashtable;
			}
			hashtable[identity2] = col;
		}

		private static object GetIdentity(string localName, string namespaceURI)
		{
			return localName + ":" + namespaceURI;
		}

		private bool IsNextColumn(DataColumnCollection columns, ref int iColumn, DataColumn col)
		{
			while (iColumn < columns.Count)
			{
				if (columns[iColumn] == col)
				{
					iColumn++;
					return true;
				}
				iColumn++;
			}
			return false;
		}

		private Hashtable _tableSchemaMap;

		private Hashtable _columnSchemaMap;

		private XmlDataDocument _doc;

		private DataSet _dataSet;

		internal const string strReservedXmlns = "http://www.w3.org/2000/xmlns/";
	}
}
