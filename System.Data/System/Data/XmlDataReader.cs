using System;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data
{
	internal class XmlDataReader
	{
		public XmlDataReader(DataSet ds, XmlReader xr, XmlReadMode m)
		{
			this.dataset = ds;
			this.reader = xr;
			this.mode = m;
		}

		public static void ReadXml(DataSet dataset, XmlReader reader, XmlReadMode mode)
		{
			new XmlDataReader(dataset, reader, mode).Process();
		}

		private void Process()
		{
			bool enforceConstraints = this.dataset.EnforceConstraints;
			try
			{
				this.dataset.EnforceConstraints = false;
				this.reader.MoveToContent();
				if (this.mode == XmlReadMode.Fragment)
				{
					while (this.reader.NodeType == XmlNodeType.Element && !this.reader.EOF)
					{
						this.ReadTopLevelElement();
					}
				}
				else
				{
					this.ReadTopLevelElement();
				}
			}
			finally
			{
				this.dataset.EnforceConstraints = enforceConstraints;
			}
		}

		private bool IsTopLevelDataSet()
		{
			string text = XmlHelper.Decode(this.reader.LocalName);
			if (this.dataset.Tables[text] == null)
			{
				return true;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = (XmlElement)xmlDocument.ReadNode(this.reader);
			xmlDocument.AppendChild(xmlElement);
			this.reader = new XmlNodeReader(xmlElement);
			this.reader.MoveToContent();
			return !XmlDataInferenceLoader.IsDocumentElementTable(xmlElement, null);
		}

		private void ReadTopLevelElement()
		{
			if (this.mode == XmlReadMode.Fragment && (XmlHelper.Decode(this.reader.LocalName) != this.dataset.DataSetName || this.reader.NamespaceURI != this.dataset.Namespace))
			{
				this.reader.Skip();
			}
			else if (this.mode == XmlReadMode.Fragment || this.IsTopLevelDataSet())
			{
				int depth = this.reader.Depth;
				this.reader.Read();
				this.reader.MoveToContent();
				do
				{
					this.ReadDataSetContent();
				}
				while (this.reader.Depth > depth && !this.reader.EOF);
				if (this.reader.NodeType == XmlNodeType.EndElement)
				{
					this.reader.ReadEndElement();
				}
				this.reader.MoveToContent();
			}
			else
			{
				this.ReadDataSetContent();
			}
		}

		private void ReadDataSetContent()
		{
			DataTable dataTable = this.dataset.Tables[XmlHelper.Decode(this.reader.LocalName)];
			if (dataTable == null || dataTable.Namespace != this.reader.NamespaceURI)
			{
				this.reader.Skip();
				this.reader.MoveToContent();
				return;
			}
			if (dataTable.Namespace != this.reader.NamespaceURI)
			{
				this.reader.Skip();
				this.reader.MoveToContent();
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			this.ReadElement(dataRow);
			dataTable.Rows.Add(dataRow);
		}

		private void ReadElement(DataRow row)
		{
			if (this.reader.MoveToFirstAttribute())
			{
				do
				{
					if (!(this.reader.NamespaceURI == "http://www.w3.org/2000/xmlns/") && !(this.reader.NamespaceURI == "http://www.w3.org/XML/1998/namespace"))
					{
						this.ReadElementAttribute(row);
					}
				}
				while (this.reader.MoveToNextAttribute());
				this.reader.MoveToElement();
			}
			if (this.reader.IsEmptyElement)
			{
				this.reader.Skip();
				this.reader.MoveToContent();
			}
			else
			{
				int depth = this.reader.Depth;
				this.reader.Read();
				this.reader.MoveToContent();
				do
				{
					this.ReadElementContent(row);
				}
				while (this.reader.Depth > depth && !this.reader.EOF);
				if (this.reader.IsEmptyElement)
				{
					this.reader.Read();
				}
				if (this.reader.NodeType == XmlNodeType.EndElement)
				{
					this.reader.ReadEndElement();
				}
				this.reader.MoveToContent();
			}
		}

		private void ReadElementAttribute(DataRow row)
		{
			DataColumn dataColumn = row.Table.Columns[XmlHelper.Decode(this.reader.LocalName)];
			if (dataColumn == null || dataColumn.Namespace != this.reader.NamespaceURI)
			{
				return;
			}
			row[dataColumn] = XmlDataReader.StringToObject(dataColumn.DataType, this.reader.Value);
		}

		private void ReadElementContent(DataRow row)
		{
			XmlNodeType nodeType = this.reader.NodeType;
			switch (nodeType)
			{
			case XmlNodeType.Element:
				this.ReadElementElement(row);
				return;
			default:
				switch (nodeType)
				{
				case XmlNodeType.Whitespace:
					this.reader.ReadString();
					return;
				case XmlNodeType.SignificantWhitespace:
					break;
				case XmlNodeType.EndElement:
					return;
				default:
					return;
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				break;
			}
			DataColumn dataColumn = null;
			DataColumnCollection columns = row.Table.Columns;
			for (int i = 0; i < columns.Count; i++)
			{
				DataColumn dataColumn2 = columns[i];
				if (dataColumn2.ColumnMapping == MappingType.SimpleContent)
				{
					dataColumn = dataColumn2;
					break;
				}
			}
			string text = this.reader.ReadString();
			this.reader.MoveToContent();
			if (dataColumn != null)
			{
				DataColumn dataColumn4;
				DataColumn dataColumn3 = (dataColumn4 = dataColumn);
				object obj = row[dataColumn4];
				row[dataColumn3] = obj + text;
			}
		}

		private void ReadElementElement(DataRow row)
		{
			DataColumn dataColumn = row.Table.Columns[XmlHelper.Decode(this.reader.LocalName)];
			if (dataColumn == null || dataColumn.Namespace != this.reader.NamespaceURI)
			{
				dataColumn = null;
			}
			if (dataColumn != null && dataColumn.ColumnMapping == MappingType.Element)
			{
				if (dataColumn.Namespace != this.reader.NamespaceURI)
				{
					this.reader.Skip();
					return;
				}
				bool isEmptyElement = this.reader.IsEmptyElement;
				int depth = this.reader.Depth;
				if (typeof(IXmlSerializable).IsAssignableFrom(dataColumn.DataType))
				{
					try
					{
						IXmlSerializable xmlSerializable = (IXmlSerializable)Activator.CreateInstance(dataColumn.DataType, new object[0]);
						if (!this.reader.IsEmptyElement)
						{
							xmlSerializable.ReadXml(this.reader);
							this.reader.ReadEndElement();
						}
						else
						{
							this.reader.Skip();
						}
						row[dataColumn] = xmlSerializable;
					}
					catch (XmlException ex)
					{
						row[dataColumn] = this.reader.ReadInnerXml();
					}
					catch (InvalidOperationException ex2)
					{
						row[dataColumn] = this.reader.ReadInnerXml();
					}
				}
				else
				{
					row[dataColumn] = XmlDataReader.StringToObject(dataColumn.DataType, this.reader.ReadElementString());
				}
				if (!isEmptyElement && this.reader.Depth > depth)
				{
					while (this.reader.Depth > depth)
					{
						this.reader.Read();
					}
					this.reader.Read();
				}
				this.reader.MoveToContent();
				return;
			}
			else
			{
				if (dataColumn != null)
				{
					this.reader.Skip();
					this.reader.MoveToContent();
					return;
				}
				DataRelationCollection childRelations = row.Table.ChildRelations;
				for (int i = 0; i < childRelations.Count; i++)
				{
					DataRelation dataRelation = childRelations[i];
					if (dataRelation.Nested)
					{
						DataTable childTable = dataRelation.ChildTable;
						if (!(childTable.TableName != XmlHelper.Decode(this.reader.LocalName)) && !(childTable.Namespace != this.reader.NamespaceURI))
						{
							DataRow dataRow = dataRelation.ChildTable.NewRow();
							this.ReadElement(dataRow);
							for (int j = 0; j < dataRelation.ChildColumns.Length; j++)
							{
								dataRow[dataRelation.ChildColumns[j]] = row[dataRelation.ParentColumns[j]];
							}
							dataRelation.ChildTable.Rows.Add(dataRow);
							return;
						}
					}
				}
				this.reader.Skip();
				this.reader.MoveToContent();
				return;
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

		private const string xmlnsNS = "http://www.w3.org/2000/xmlns/";

		private DataSet dataset;

		private XmlReader reader;

		private XmlReadMode mode;
	}
}
