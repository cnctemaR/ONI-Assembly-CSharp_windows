using System;
using System.Collections;
using System.Xml;

namespace System.Data
{
	internal class XmlDataLoader
	{
		public XmlDataLoader(DataSet set)
		{
			this.DSet = set;
		}

		public XmlReadMode LoadData(XmlReader reader, XmlReadMode mode)
		{
			XmlReadMode xmlReadMode = mode;
			switch (mode)
			{
			case XmlReadMode.Auto:
				xmlReadMode = ((this.DSet.Tables.Count != 0) ? XmlReadMode.IgnoreSchema : XmlReadMode.InferSchema);
				this.ReadModeSchema(reader, (this.DSet.Tables.Count != 0) ? XmlReadMode.IgnoreSchema : XmlReadMode.Auto);
				return xmlReadMode;
			case XmlReadMode.IgnoreSchema:
				xmlReadMode = XmlReadMode.IgnoreSchema;
				this.ReadModeSchema(reader, mode);
				return xmlReadMode;
			case XmlReadMode.InferSchema:
				xmlReadMode = XmlReadMode.InferSchema;
				this.ReadModeSchema(reader, mode);
				return xmlReadMode;
			}
			reader.Skip();
			return xmlReadMode;
		}

		private void ReadModeSchema(XmlReader reader, XmlReadMode mode)
		{
			bool flag = mode == XmlReadMode.InferSchema || mode == XmlReadMode.Auto;
			bool flag2 = mode != XmlReadMode.InferSchema;
			if (reader.LocalName == "schema")
			{
				if (mode != XmlReadMode.Auto)
				{
					reader.Skip();
				}
				else
				{
					this.DSet.ReadXmlSchema(reader);
				}
				reader.MoveToContent();
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(reader);
			if (xmlDocument.DocumentElement == null)
			{
				return;
			}
			int num = XmlDataLoader.XmlNodeElementsDepth(xmlDocument.DocumentElement);
			int num2 = num;
			if (num2 != 1)
			{
				if (num2 != 2)
				{
					if (flag)
					{
						this.DSet.DataSetName = xmlDocument.DocumentElement.LocalName;
						this.DSet.Prefix = xmlDocument.DocumentElement.Prefix;
						this.DSet.Namespace = xmlDocument.DocumentElement.NamespaceURI;
					}
				}
				else
				{
					XmlDocument xmlDocument2 = new XmlDocument();
					XmlElement xmlElement = xmlDocument2.CreateElement("dummy");
					xmlDocument2.AppendChild(xmlElement);
					XmlNode xmlNode = xmlDocument2.ImportNode(xmlDocument.DocumentElement, true);
					xmlElement.AppendChild(xmlNode);
					xmlDocument = xmlDocument2;
				}
				bool enforceConstraints = this.DSet.EnforceConstraints;
				this.DSet.EnforceConstraints = false;
				XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
				for (int i = 0; i < childNodes.Count; i++)
				{
					XmlNode xmlNode2 = childNodes[i];
					if (xmlNode2.NodeType == XmlNodeType.Element)
					{
						this.AddRowToTable(xmlNode2, null, flag, flag2);
					}
				}
				this.DSet.EnforceConstraints = enforceConstraints;
				return;
			}
			if (flag)
			{
				this.DSet.DataSetName = xmlDocument.DocumentElement.LocalName;
				this.DSet.Prefix = xmlDocument.DocumentElement.Prefix;
				this.DSet.Namespace = xmlDocument.DocumentElement.NamespaceURI;
			}
		}

		internal static object StringToObject(Type type, string value)
		{
			if (type == null)
			{
				return value;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return XmlConvert.ToBoolean(value);
			case TypeCode.Char:
				return (char)XmlConvert.ToInt32(value);
			case TypeCode.SByte:
				return XmlConvert.ToSByte(value);
			case TypeCode.Byte:
				return XmlConvert.ToByte(value);
			case TypeCode.Int16:
				return XmlConvert.ToInt16(value);
			case TypeCode.UInt16:
				return XmlConvert.ToUInt16(value);
			case TypeCode.Int32:
				return XmlConvert.ToInt32(value);
			case TypeCode.UInt32:
				return XmlConvert.ToUInt32(value);
			case TypeCode.Int64:
				return XmlConvert.ToInt64(value);
			case TypeCode.UInt64:
				return XmlConvert.ToUInt64(value);
			case TypeCode.Single:
				return XmlConvert.ToSingle(value);
			case TypeCode.Double:
				return XmlConvert.ToDouble(value);
			case TypeCode.Decimal:
				return XmlConvert.ToDecimal(value);
			case TypeCode.DateTime:
				return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified);
			default:
				if (type == typeof(TimeSpan))
				{
					return XmlConvert.ToTimeSpan(value);
				}
				if (type == typeof(Guid))
				{
					return XmlConvert.ToGuid(value);
				}
				if (type == typeof(byte[]))
				{
					return Convert.FromBase64String(value);
				}
				if (type == typeof(Type))
				{
					return Type.GetType(value);
				}
				return Convert.ChangeType(value, type);
			}
		}

		private void AddRowToTable(XmlNode tableNode, DataColumn relationColumn, bool inferSchema, bool fillRows)
		{
			Hashtable hashtable = new Hashtable();
			DataTable dataTable;
			if (this.DSet.Tables.Contains(tableNode.LocalName))
			{
				dataTable = this.DSet.Tables[tableNode.LocalName];
			}
			else
			{
				if (!inferSchema)
				{
					return;
				}
				dataTable = new DataTable(tableNode.LocalName);
				this.DSet.Tables.Add(dataTable);
			}
			if (!this.HaveChildElements(tableNode) && this.HaveText(tableNode) && !this.IsRepeatedHaveChildNodes(tableNode))
			{
				string text = tableNode.Name + "_Text";
				if (!dataTable.Columns.Contains(text))
				{
					dataTable.Columns.Add(text);
				}
				hashtable.Add(text, tableNode.InnerText);
			}
			XmlNodeList childNodes = tableNode.ChildNodes;
			for (int i = 0; i < childNodes.Count; i++)
			{
				XmlNode xmlNode = childNodes[i];
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (this.IsInferredAsTable(xmlNode))
					{
						if (inferSchema)
						{
							string text2 = dataTable.TableName + "_Id";
							if (!dataTable.Columns.Contains(text2))
							{
								DataColumn dataColumn = new DataColumn(text2, typeof(int));
								dataColumn.AllowDBNull = false;
								dataColumn.AutoIncrement = true;
								dataColumn.ColumnMapping = MappingType.Hidden;
								dataTable.Columns.Add(dataColumn);
							}
							this.AddRowToTable(xmlNode, dataTable.Columns[text2], inferSchema, fillRows);
						}
						else
						{
							this.AddRowToTable(xmlNode, null, inferSchema, fillRows);
						}
					}
					else
					{
						object obj;
						if (xmlNode.FirstChild != null)
						{
							obj = xmlNode.FirstChild.Value;
						}
						else
						{
							obj = string.Empty;
						}
						if (dataTable.Columns.Contains(xmlNode.LocalName))
						{
							hashtable.Add(xmlNode.LocalName, obj);
						}
						else if (inferSchema)
						{
							dataTable.Columns.Add(xmlNode.LocalName);
							hashtable.Add(xmlNode.LocalName, obj);
						}
					}
				}
			}
			XmlAttributeCollection attributes = tableNode.Attributes;
			for (int j = 0; j < attributes.Count; j++)
			{
				XmlAttribute xmlAttribute = attributes[j];
				if (xmlAttribute.Prefix.Equals("xmlns"))
				{
					dataTable.Namespace = xmlAttribute.Value;
				}
				else
				{
					if (!dataTable.Columns.Contains(xmlAttribute.LocalName))
					{
						DataColumn dataColumn2 = dataTable.Columns.Add(xmlAttribute.LocalName);
						dataColumn2.ColumnMapping = MappingType.Attribute;
					}
					dataTable.Columns[xmlAttribute.LocalName].Namespace = dataTable.Namespace;
					hashtable.Add(xmlAttribute.LocalName, xmlAttribute.Value);
				}
			}
			if (relationColumn != null)
			{
				if (!dataTable.Columns.Contains(relationColumn.ColumnName))
				{
					DataColumn dataColumn3 = new DataColumn(relationColumn.ColumnName, typeof(int));
					dataColumn3.ColumnMapping = MappingType.Hidden;
					dataTable.Columns.Add(dataColumn3);
					DataRelation dataRelation = new DataRelation(relationColumn.Table.TableName + "_" + dataColumn3.Table.TableName, relationColumn, dataColumn3);
					dataRelation.Nested = true;
					this.DSet.Relations.Add(dataRelation);
					UniqueConstraint.SetAsPrimaryKey(dataRelation.ParentTable.Constraints, dataRelation.ParentKeyConstraint);
				}
				hashtable.Add(relationColumn.ColumnName, relationColumn.GetAutoIncrementValue());
			}
			DataRow dataRow = dataTable.NewRow();
			IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				dataRow[enumerator.Key.ToString()] = XmlDataLoader.StringToObject(dataTable.Columns[enumerator.Key.ToString()].DataType, enumerator.Value.ToString());
			}
			if (fillRows)
			{
				dataTable.Rows.Add(dataRow);
			}
		}

		private static int XmlNodeElementsDepth(XmlNode node)
		{
			int num = -1;
			if (node == null)
			{
				return -1;
			}
			if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Element)
			{
				for (int i = 0; i < node.ChildNodes.Count; i++)
				{
					if (node.ChildNodes[i].NodeType == XmlNodeType.Element)
					{
						int num2 = XmlDataLoader.XmlNodeElementsDepth(node.ChildNodes[i]);
						num = ((num >= num2) ? num : num2);
					}
				}
				return num + 1;
			}
			return 1;
		}

		private bool HaveChildElements(XmlNode node)
		{
			bool flag = true;
			if (node.ChildNodes.Count > 0)
			{
				foreach (object obj in node.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType != XmlNodeType.Element)
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private bool HaveText(XmlNode node)
		{
			bool flag = true;
			if (node.ChildNodes.Count > 0)
			{
				foreach (object obj in node.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType != XmlNodeType.Text)
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private bool IsRepeat(XmlNode node)
		{
			bool flag = false;
			if (node.ParentNode != null)
			{
				foreach (object obj in node.ParentNode.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode != node && xmlNode.Name == node.Name)
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		private bool HaveAttributes(XmlNode node)
		{
			return node.Attributes != null && node.Attributes.Count > 0;
		}

		private bool IsInferredAsTable(XmlNode node)
		{
			return this.HaveChildElements(node) || this.HaveAttributes(node) || this.IsRepeat(node);
		}

		private bool IsRepeatedHaveChildNodes(XmlNode node)
		{
			bool flag = false;
			if (node.ParentNode != null)
			{
				foreach (object obj in node.ParentNode.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode != node && xmlNode.Name == node.Name && this.HaveChildElements(xmlNode))
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		private DataSet DSet;
	}
}
