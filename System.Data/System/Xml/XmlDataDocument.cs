using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlDataDocument : XmlDocument
	{
		public XmlDataDocument()
		{
			this.InitDelegateFields();
			this.dataSet = new DataSet();
			this.dataSet._xmlDataDocument = this;
			this.dataSet.Tables.CollectionChanged += this.tablesChanged;
			this.AddXmlDocumentListeners();
			this.DataSet.EnforceConstraints = false;
		}

		public XmlDataDocument(DataSet dataset)
		{
			if (dataset == null)
			{
				throw new ArgumentException("Parameter dataset cannot be null.");
			}
			if (dataset._xmlDataDocument != null)
			{
				throw new ArgumentException("DataSet cannot be associated with two or more XmlDataDocument.");
			}
			this.InitDelegateFields();
			this.dataSet = dataset;
			this.dataSet._xmlDataDocument = this;
			XmlElement xmlElement = this.CreateElement(this.dataSet.Prefix, XmlHelper.Encode(this.dataSet.DataSetName), this.dataSet.Namespace);
			foreach (object obj in this.dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				if (dataTable.ParentRelations.Count <= 0)
				{
					this.FillNodeRows(xmlElement, dataTable, dataTable.Rows);
				}
			}
			if (xmlElement.ChildNodes.Count > 0)
			{
				this.AppendChild(xmlElement);
			}
			foreach (object obj2 in this.dataSet.Tables)
			{
				DataTable dataTable2 = (DataTable)obj2;
				dataTable2.ColumnChanged += this.columnChanged;
				dataTable2.RowDeleted += this.rowDeleted;
				dataTable2.RowChanged += this.rowChanged;
			}
			this.AddXmlDocumentListeners();
		}

		private XmlDataDocument(DataSet dataset, bool clone)
		{
			this.InitDelegateFields();
			this.dataSet = dataset;
			this.dataSet._xmlDataDocument = this;
			foreach (object obj in this.DataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				foreach (object obj2 in dataTable.Rows)
				{
					DataRow dataRow = (DataRow)obj2;
					dataRow.XmlRowID = this.dataRowID;
					this.dataRowIDList.Add(this.dataRowID);
					this.dataRowID++;
				}
			}
			this.AddXmlDocumentListeners();
			foreach (object obj3 in this.dataSet.Tables)
			{
				DataTable dataTable2 = (DataTable)obj3;
				dataTable2.ColumnChanged += this.columnChanged;
				dataTable2.RowDeleted += this.rowDeleted;
				dataTable2.RowChanged += this.rowChanged;
			}
		}

		public DataSet DataSet
		{
			get
			{
				return this.dataSet;
			}
		}

		private void FillNodeRows(XmlElement parent, DataTable dt, ICollection rows)
		{
			foreach (object obj in dt.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				XmlDataDocument.XmlDataElement dataElement = dataRow.DataElement;
				this.FillNodeChildrenFromRow(dataRow, dataElement);
				foreach (object obj2 in dt.ChildRelations)
				{
					DataRelation dataRelation = (DataRelation)obj2;
					this.FillNodeRows(dataElement, dataRelation.ChildTable, dataRow.GetChildRows(dataRelation));
				}
				parent.AppendChild(dataElement);
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDataDocument xmlDataDocument;
			if (deep)
			{
				xmlDataDocument = new XmlDataDocument(this.DataSet.Copy(), true);
			}
			else
			{
				xmlDataDocument = new XmlDataDocument(this.DataSet.Clone(), true);
			}
			xmlDataDocument.RemoveXmlDocumentListeners();
			xmlDataDocument.PreserveWhitespace = base.PreserveWhitespace;
			if (deep)
			{
				foreach (object obj in this.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					xmlDataDocument.AppendChild(xmlDataDocument.ImportNode(xmlNode, deep));
				}
			}
			xmlDataDocument.AddXmlDocumentListeners();
			return xmlDataDocument;
		}

		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			DataTable dataTable = this.DataSet.Tables[XmlHelper.Decode(localName)];
			DataRow dataRow = ((dataTable == null) ? null : dataTable.NewRow());
			if (dataRow != null)
			{
				return this.GetElementFromRow(dataRow);
			}
			return base.CreateElement(prefix, localName, namespaceURI);
		}

		public override XmlEntityReference CreateEntityReference(string name)
		{
			throw new NotSupportedException();
		}

		public override XmlElement GetElementById(string elemId)
		{
			throw new NotSupportedException();
		}

		public XmlElement GetElementFromRow(DataRow r)
		{
			return r.DataElement;
		}

		public DataRow GetRowFromElement(XmlElement e)
		{
			XmlDataDocument.XmlDataElement xmlDataElement = e as XmlDataDocument.XmlDataElement;
			if (xmlDataElement == null)
			{
				return null;
			}
			return xmlDataElement.DataRow;
		}

		public override void Load(Stream inStream)
		{
			this.Load(new XmlTextReader(inStream));
		}

		public override void Load(string filename)
		{
			this.Load(new XmlTextReader(filename));
		}

		public override void Load(TextReader txtReader)
		{
			this.Load(new XmlTextReader(txtReader));
		}

		public override void Load(XmlReader reader)
		{
			if (base.DocumentElement != null)
			{
				throw new InvalidOperationException("XmlDataDocument does not support multi-time loading. New XmlDadaDocument is always required.");
			}
			bool enforceConstraints = this.DataSet.EnforceConstraints;
			this.DataSet.EnforceConstraints = false;
			this.dataSet.Tables.CollectionChanged -= this.tablesChanged;
			base.Load(reader);
			this.DataSet.EnforceConstraints = enforceConstraints;
			this.dataSet.Tables.CollectionChanged += this.tablesChanged;
		}

		[MonoTODO("Create optimized XPathNavigator")]
		protected override XPathNavigator CreateNavigator(XmlNode node)
		{
			return base.CreateNavigator(node);
		}

		private void OnNodeChanging(object sender, XmlNodeChangedEventArgs args)
		{
			if (!this.raiseDocumentEvents)
			{
				return;
			}
			if (this.DataSet.EnforceConstraints)
			{
				throw new InvalidOperationException(Locale.GetText("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations."));
			}
		}

		private void OnNodeChanged(object sender, XmlNodeChangedEventArgs args)
		{
			if (!this.raiseDocumentEvents)
			{
				return;
			}
			bool flag = this.raiseDataSetEvents;
			this.raiseDataSetEvents = false;
			try
			{
				if (args.Node != null)
				{
					DataRow rowFromElement = this.GetRowFromElement((XmlElement)args.Node.ParentNode.ParentNode);
					if (rowFromElement != null)
					{
						if (rowFromElement.Table.Columns.Contains(args.Node.ParentNode.Name))
						{
							if (rowFromElement[args.Node.ParentNode.Name].ToString() != args.Node.InnerText)
							{
								DataColumn dataColumn = rowFromElement.Table.Columns[args.Node.ParentNode.Name];
								rowFromElement[dataColumn] = XmlDataDocument.StringToObject(dataColumn.DataType, args.Node.InnerText);
							}
						}
					}
				}
			}
			finally
			{
				this.raiseDataSetEvents = flag;
			}
		}

		private void OnNodeRemoving(object sender, XmlNodeChangedEventArgs args)
		{
			if (!this.raiseDocumentEvents)
			{
				return;
			}
			if (this.DataSet.EnforceConstraints)
			{
				throw new InvalidOperationException(Locale.GetText("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations."));
			}
		}

		private void OnNodeRemoved(object sender, XmlNodeChangedEventArgs args)
		{
			if (!this.raiseDocumentEvents)
			{
				return;
			}
			bool flag = this.raiseDataSetEvents;
			this.raiseDataSetEvents = false;
			try
			{
				if (args.OldParent != null)
				{
					XmlElement xmlElement = args.OldParent as XmlElement;
					if (xmlElement != null)
					{
						XmlElement xmlElement2 = args.Node as XmlElement;
						if (xmlElement2 != null)
						{
							DataRow rowFromElement = this.GetRowFromElement(xmlElement2);
							if (rowFromElement != null)
							{
								rowFromElement.Table.Rows.Remove(rowFromElement);
							}
						}
						DataRow rowFromElement2 = this.GetRowFromElement(xmlElement);
						if (rowFromElement2 != null)
						{
							rowFromElement2[args.Node.Name] = null;
						}
					}
				}
			}
			finally
			{
				this.raiseDataSetEvents = flag;
			}
		}

		private void OnNodeInserting(object sender, XmlNodeChangedEventArgs args)
		{
			if (!this.raiseDocumentEvents)
			{
				return;
			}
			if (this.DataSet.EnforceConstraints)
			{
				throw new InvalidOperationException(Locale.GetText("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations."));
			}
		}

		private void OnNodeInserted(object sender, XmlNodeChangedEventArgs args)
		{
			if (!this.raiseDocumentEvents)
			{
				return;
			}
			bool flag = this.raiseDataSetEvents;
			this.raiseDataSetEvents = false;
			try
			{
				if (!(args.NewParent is XmlElement))
				{
					foreach (object obj in args.Node.ChildNodes)
					{
						XmlNode xmlNode = (XmlNode)obj;
						this.CheckDescendantRelationship(xmlNode);
					}
				}
				else
				{
					DataRow rowFromElement = this.GetRowFromElement(args.NewParent as XmlElement);
					if (rowFromElement == null)
					{
						if (args.NewParent == base.DocumentElement)
						{
							this.CheckDescendantRelationship(args.Node);
						}
					}
					else
					{
						XmlAttribute xmlAttribute = args.Node as XmlAttribute;
						if (xmlAttribute != null)
						{
							DataColumn dataColumn = rowFromElement.Table.Columns[XmlHelper.Decode(xmlAttribute.LocalName)];
							if (dataColumn != null)
							{
								rowFromElement[dataColumn] = XmlDataDocument.StringToObject(dataColumn.DataType, args.Node.Value);
							}
						}
						else
						{
							DataRow rowFromElement2 = this.GetRowFromElement(args.Node as XmlElement);
							if (rowFromElement2 != null)
							{
								if (rowFromElement2.RowState != DataRowState.Detached && rowFromElement.RowState != DataRowState.Detached)
								{
									this.FillRelationship(rowFromElement, rowFromElement2, args.NewParent, args.Node);
								}
							}
							else if (args.Node.NodeType == XmlNodeType.Element)
							{
								DataColumn dataColumn2 = rowFromElement.Table.Columns[XmlHelper.Decode(args.Node.LocalName)];
								if (dataColumn2 != null)
								{
									rowFromElement[dataColumn2] = XmlDataDocument.StringToObject(dataColumn2.DataType, args.Node.InnerText);
								}
							}
							else if (args.Node is XmlCharacterData && args.Node.NodeType != XmlNodeType.Comment)
							{
								for (int i = 0; i < rowFromElement.Table.Columns.Count; i++)
								{
									DataColumn dataColumn3 = rowFromElement.Table.Columns[i];
									if (dataColumn3.ColumnMapping == MappingType.SimpleContent)
									{
										rowFromElement[dataColumn3] = XmlDataDocument.StringToObject(dataColumn3.DataType, args.Node.Value);
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				this.raiseDataSetEvents = flag;
			}
		}

		private void CheckDescendantRelationship(XmlNode n)
		{
			XmlElement xmlElement = n as XmlElement;
			DataRow rowFromElement = this.GetRowFromElement(xmlElement);
			if (rowFromElement == null)
			{
				return;
			}
			rowFromElement.Table.Rows.Add(rowFromElement);
			this.CheckDescendantRelationship(n, rowFromElement);
		}

		private void CheckDescendantRelationship(XmlNode p, DataRow row)
		{
			foreach (object obj in p.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					DataRow rowFromElement = this.GetRowFromElement(xmlElement);
					if (rowFromElement != null)
					{
						rowFromElement.Table.Rows.Add(rowFromElement);
						this.FillRelationship(row, rowFromElement, p, xmlElement);
					}
				}
			}
		}

		private void FillRelationship(DataRow row, DataRow childRow, XmlNode parentNode, XmlNode childNode)
		{
			for (int i = 0; i < childRow.Table.ParentRelations.Count; i++)
			{
				DataRelation dataRelation = childRow.Table.ParentRelations[i];
				if (dataRelation.ParentTable == row.Table)
				{
					childRow.SetParentRow(row);
					break;
				}
			}
			this.CheckDescendantRelationship(childNode, childRow);
		}

		private void OnDataTableChanged(object sender, CollectionChangeEventArgs eventArgs)
		{
			if (!this.raiseDataSetEvents)
			{
				return;
			}
			bool flag = this.raiseDocumentEvents;
			this.raiseDocumentEvents = false;
			try
			{
				DataTable dataTable = (DataTable)eventArgs.Element;
				CollectionChangeAction action = eventArgs.Action;
				if (action != CollectionChangeAction.Add)
				{
					if (action == CollectionChangeAction.Remove)
					{
						dataTable.ColumnChanged -= this.columnChanged;
						dataTable.RowDeleted -= this.rowDeleted;
						dataTable.RowChanged -= this.rowChanged;
					}
				}
				else
				{
					dataTable.ColumnChanged += this.columnChanged;
					dataTable.RowDeleted += this.rowDeleted;
					dataTable.RowChanged += this.rowChanged;
				}
			}
			finally
			{
				this.raiseDocumentEvents = flag;
			}
		}

		private void OnDataTableColumnChanged(object sender, DataColumnChangeEventArgs eventArgs)
		{
			if (!this.raiseDataSetEvents)
			{
				return;
			}
			bool flag = this.raiseDocumentEvents;
			this.raiseDocumentEvents = false;
			try
			{
				DataRow row = eventArgs.Row;
				XmlElement elementFromRow = this.GetElementFromRow(row);
				if (elementFromRow != null)
				{
					DataColumn column = eventArgs.Column;
					string text = ((!row.IsNull(column)) ? row[column].ToString() : string.Empty);
					switch (column.ColumnMapping)
					{
					case MappingType.Element:
					{
						bool flag2 = false;
						for (int i = 0; i < elementFromRow.ChildNodes.Count; i++)
						{
							XmlElement xmlElement = elementFromRow.ChildNodes[i] as XmlElement;
							if (xmlElement != null && xmlElement.LocalName == XmlHelper.Encode(column.ColumnName) && xmlElement.NamespaceURI == column.Namespace)
							{
								flag2 = true;
								xmlElement.InnerText = text;
								break;
							}
						}
						if (!flag2)
						{
							XmlElement xmlElement2 = this.CreateElement(column.Prefix, XmlHelper.Encode(column.ColumnName), column.Namespace);
							xmlElement2.InnerText = text;
							elementFromRow.AppendChild(xmlElement2);
						}
						break;
					}
					case MappingType.Attribute:
						elementFromRow.SetAttribute(XmlHelper.Encode(column.ColumnName), column.Namespace, text);
						break;
					case MappingType.SimpleContent:
						elementFromRow.InnerText = text;
						break;
					}
				}
			}
			finally
			{
				this.raiseDocumentEvents = flag;
			}
		}

		private void OnDataTableRowDeleted(object sender, DataRowChangeEventArgs eventArgs)
		{
			if (!this.raiseDataSetEvents)
			{
				return;
			}
			bool flag = this.raiseDocumentEvents;
			this.raiseDocumentEvents = false;
			try
			{
				XmlElement elementFromRow = this.GetElementFromRow(eventArgs.Row);
				if (elementFromRow != null)
				{
					elementFromRow.ParentNode.RemoveChild(elementFromRow);
				}
			}
			finally
			{
				this.raiseDocumentEvents = flag;
			}
		}

		[MonoTODO("Need to handle hidden columns? - see comments on each private method")]
		private void OnDataTableRowChanged(object sender, DataRowChangeEventArgs eventArgs)
		{
			if (!this.raiseDataSetEvents)
			{
				return;
			}
			bool flag = this.raiseDocumentEvents;
			this.raiseDocumentEvents = false;
			try
			{
				DataRowAction action = eventArgs.Action;
				switch (action)
				{
				case DataRowAction.Delete:
					this.OnDataTableRowDeleted(sender, eventArgs);
					break;
				default:
					if (action == DataRowAction.Add)
					{
						this.OnDataTableRowAdded(eventArgs);
					}
					break;
				case DataRowAction.Rollback:
					this.OnDataTableRowRollback(eventArgs);
					break;
				}
			}
			finally
			{
				this.raiseDocumentEvents = flag;
			}
		}

		private void OnDataTableRowAdded(DataRowChangeEventArgs args)
		{
			if (!this.raiseDataSetEvents)
			{
				return;
			}
			bool flag = this.raiseDocumentEvents;
			this.raiseDocumentEvents = false;
			try
			{
				DataRow row = args.Row;
				if (base.DocumentElement == null)
				{
					this.AppendChild(base.CreateElement(XmlHelper.Encode(this.DataSet.DataSetName)));
				}
				DataTable table = args.Row.Table;
				XmlElement xmlElement = this.GetElementFromRow(row);
				if (xmlElement == null)
				{
					xmlElement = this.CreateElement(table.Prefix, XmlHelper.Encode(table.TableName), table.Namespace);
				}
				if (xmlElement.ChildNodes.Count == 0)
				{
					this.FillNodeChildrenFromRow(row, xmlElement);
				}
				if (xmlElement.ParentNode == null)
				{
					XmlElement xmlElement2 = null;
					if (table.ParentRelations.Count > 0)
					{
						for (int i = 0; i < table.ParentRelations.Count; i++)
						{
							DataRelation dataRelation = table.ParentRelations[i];
							DataRow parentRow = row.GetParentRow(dataRelation);
							if (parentRow != null)
							{
								xmlElement2 = this.GetElementFromRow(parentRow);
							}
						}
					}
					if (xmlElement2 == null)
					{
						xmlElement2 = base.DocumentElement;
					}
					xmlElement2.AppendChild(xmlElement);
				}
			}
			finally
			{
				this.raiseDocumentEvents = flag;
			}
		}

		private void FillNodeChildrenFromRow(DataRow row, XmlElement element)
		{
			DataTable table = row.Table;
			for (int i = 0; i < table.Columns.Count; i++)
			{
				DataColumn dataColumn = table.Columns[i];
				string text = ((!row.IsNull(dataColumn)) ? row[dataColumn].ToString() : string.Empty);
				switch (dataColumn.ColumnMapping)
				{
				case MappingType.Element:
				{
					XmlElement xmlElement = this.CreateElement(dataColumn.Prefix, XmlHelper.Encode(dataColumn.ColumnName), dataColumn.Namespace);
					xmlElement.InnerText = text;
					element.AppendChild(xmlElement);
					break;
				}
				case MappingType.Attribute:
				{
					XmlAttribute xmlAttribute = this.CreateAttribute(dataColumn.Prefix, XmlHelper.Encode(dataColumn.ColumnName), dataColumn.Namespace);
					xmlAttribute.Value = text;
					element.SetAttributeNode(xmlAttribute);
					break;
				}
				case MappingType.SimpleContent:
				{
					XmlText xmlText = this.CreateTextNode(text);
					element.AppendChild(xmlText);
					break;
				}
				}
			}
		}

		[MonoTODO("It does not look complete.")]
		private void OnDataTableRowRollback(DataRowChangeEventArgs args)
		{
			if (!this.raiseDataSetEvents)
			{
				return;
			}
			bool flag = this.raiseDocumentEvents;
			this.raiseDocumentEvents = false;
			try
			{
				DataRow row = args.Row;
				XmlElement elementFromRow = this.GetElementFromRow(row);
				if (elementFromRow != null)
				{
					DataTable table = row.Table;
					ArrayList arrayList = new ArrayList();
					foreach (object obj in elementFromRow.Attributes)
					{
						XmlAttribute xmlAttribute = (XmlAttribute)obj;
						DataColumn dataColumn = table.Columns[XmlHelper.Decode(xmlAttribute.LocalName)];
						if (dataColumn != null)
						{
							if (row.IsNull(dataColumn))
							{
								arrayList.Add(xmlAttribute);
							}
							else
							{
								xmlAttribute.Value = row[dataColumn].ToString();
							}
						}
					}
					foreach (object obj2 in arrayList)
					{
						XmlAttribute xmlAttribute2 = (XmlAttribute)obj2;
						elementFromRow.RemoveAttributeNode(xmlAttribute2);
					}
					arrayList.Clear();
					foreach (object obj3 in elementFromRow.ChildNodes)
					{
						XmlNode xmlNode = (XmlNode)obj3;
						if (xmlNode.NodeType == XmlNodeType.Element)
						{
							DataColumn dataColumn2 = table.Columns[XmlHelper.Decode(xmlNode.LocalName)];
							if (dataColumn2 != null)
							{
								if (row.IsNull(dataColumn2))
								{
									arrayList.Add(xmlNode);
								}
								else
								{
									xmlNode.InnerText = row[dataColumn2].ToString();
								}
							}
						}
					}
					foreach (object obj4 in arrayList)
					{
						XmlNode xmlNode2 = (XmlNode)obj4;
						elementFromRow.RemoveChild(xmlNode2);
					}
				}
			}
			finally
			{
				this.raiseDocumentEvents = flag;
			}
		}

		private void InitDelegateFields()
		{
			this.columnChanged = new DataColumnChangeEventHandler(this.OnDataTableColumnChanged);
			this.rowDeleted = new DataRowChangeEventHandler(this.OnDataTableRowDeleted);
			this.rowChanged = new DataRowChangeEventHandler(this.OnDataTableRowChanged);
			this.tablesChanged = new CollectionChangeEventHandler(this.OnDataTableChanged);
		}

		private void RemoveXmlDocumentListeners()
		{
			base.NodeInserting -= this.OnNodeInserting;
			base.NodeInserted -= this.OnNodeInserted;
			base.NodeChanging -= this.OnNodeChanging;
			base.NodeChanged -= this.OnNodeChanged;
			base.NodeRemoving -= this.OnNodeRemoving;
			base.NodeRemoved -= this.OnNodeRemoved;
		}

		private void AddXmlDocumentListeners()
		{
			base.NodeInserting += this.OnNodeInserting;
			base.NodeInserted += this.OnNodeInserted;
			base.NodeChanging += this.OnNodeChanging;
			base.NodeChanged += this.OnNodeChanged;
			base.NodeRemoving += this.OnNodeRemoving;
			base.NodeRemoved += this.OnNodeRemoved;
		}

		internal static object StringToObject(Type type, string value)
		{
			if (value == null || value == string.Empty)
			{
				return DBNull.Value;
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
				return Convert.ChangeType(value, type);
			}
		}

		private DataSet dataSet;

		private int dataRowID = 1;

		private ArrayList dataRowIDList = new ArrayList();

		private bool raiseDataSetEvents = true;

		private bool raiseDocumentEvents = true;

		private DataColumnChangeEventHandler columnChanged;

		private DataRowChangeEventHandler rowDeleted;

		private DataRowChangeEventHandler rowChanged;

		private CollectionChangeEventHandler tablesChanged;

		internal class XmlDataElement : XmlElement
		{
			internal XmlDataElement(DataRow row, string prefix, string localName, string ns, XmlDataDocument doc)
				: base(prefix, localName, ns, doc)
			{
				this.row = row;
				if (row != null)
				{
					row.DataElement = this;
					row.XmlRowID = doc.dataRowID;
					doc.dataRowIDList.Add(row.XmlRowID);
					doc.dataRowID++;
				}
			}

			internal DataRow DataRow
			{
				get
				{
					return this.row;
				}
			}

			private DataRow row;
		}
	}
}
