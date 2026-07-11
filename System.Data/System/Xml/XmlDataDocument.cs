using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;

namespace System.Xml
{
	[Obsolete("XmlDataDocument class will be removed in a future release.")]
	public class XmlDataDocument : XmlDocument
	{
		internal void AddPointer(IXmlDataVirtualNode pointer)
		{
			Hashtable pointers = this._pointers;
			lock (pointers)
			{
				this._countAddPointer++;
				if (this._countAddPointer >= 5)
				{
					ArrayList arrayList = new ArrayList();
					foreach (object obj in this._pointers)
					{
						IXmlDataVirtualNode xmlDataVirtualNode = (IXmlDataVirtualNode)((DictionaryEntry)obj).Value;
						if (!xmlDataVirtualNode.IsInUse())
						{
							arrayList.Add(xmlDataVirtualNode);
						}
					}
					for (int i = 0; i < arrayList.Count; i++)
					{
						this._pointers.Remove(arrayList[i]);
					}
					this._countAddPointer = 0;
				}
				this._pointers[pointer] = pointer;
			}
		}

		[Conditional("DEBUG")]
		internal void AssertPointerPresent(IXmlDataVirtualNode pointer)
		{
		}

		private void AttachDataSet(DataSet ds)
		{
			if (ds.FBoundToDocument)
			{
				throw new ArgumentException("DataSet can be associated with at most one XmlDataDocument. Cannot associate the DataSet with the current XmlDataDocument because the DataSet is already associated with another XmlDataDocument.");
			}
			ds.FBoundToDocument = true;
			this._dataSet = ds;
			this.BindSpecialListeners();
		}

		internal void SyncRows(DataRow parentRow, XmlNode node, bool fAddRowsToTable)
		{
			XmlBoundElement xmlBoundElement = node as XmlBoundElement;
			if (xmlBoundElement != null)
			{
				DataRow row = xmlBoundElement.Row;
				if (row != null && xmlBoundElement.ElementState == ElementState.Defoliated)
				{
					return;
				}
				if (row != null)
				{
					this.SynchronizeRowFromRowElement(xmlBoundElement);
					xmlBoundElement.ElementState = ElementState.WeakFoliation;
					this.DefoliateRegion(xmlBoundElement);
					if (parentRow != null)
					{
						XmlDataDocument.SetNestedParentRow(row, parentRow);
					}
					if (fAddRowsToTable && row.RowState == DataRowState.Detached)
					{
						row.Table.Rows.Add(row);
					}
					parentRow = row;
				}
			}
			for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				this.SyncRows(parentRow, xmlNode, fAddRowsToTable);
			}
		}

		internal void SyncTree(XmlNode node)
		{
			XmlBoundElement xmlBoundElement = null;
			this._mapper.GetRegion(node, out xmlBoundElement);
			DataRow dataRow = null;
			bool flag = this.IsConnected(node);
			if (xmlBoundElement != null)
			{
				DataRow row = xmlBoundElement.Row;
				if (row != null && xmlBoundElement.ElementState == ElementState.Defoliated)
				{
					return;
				}
				if (row != null)
				{
					this.SynchronizeRowFromRowElement(xmlBoundElement);
					if (node == xmlBoundElement)
					{
						xmlBoundElement.ElementState = ElementState.WeakFoliation;
						this.DefoliateRegion(xmlBoundElement);
					}
					if (flag && row.RowState == DataRowState.Detached)
					{
						row.Table.Rows.Add(row);
					}
					dataRow = row;
				}
			}
			for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				this.SyncRows(dataRow, xmlNode, flag);
			}
		}

		internal ElementState AutoFoliationState
		{
			get
			{
				return this._autoFoliationState;
			}
			set
			{
				this._autoFoliationState = value;
			}
		}

		private void BindForLoad()
		{
			this._ignoreDataSetEvents = true;
			this._mapper.SetupMapping(this, this._dataSet);
			if (this._dataSet.Tables.Count > 0)
			{
				this.LoadDataSetFromTree();
			}
			this.BindListeners();
			this._ignoreDataSetEvents = false;
		}

		private void Bind(bool fLoadFromDataSet)
		{
			this._ignoreDataSetEvents = true;
			this._ignoreXmlEvents = true;
			this._mapper.SetupMapping(this, this._dataSet);
			if (base.DocumentElement != null)
			{
				this.LoadDataSetFromTree();
				this.BindListeners();
			}
			else if (fLoadFromDataSet)
			{
				this._bLoadFromDataSet = true;
				this.LoadTreeFromDataSet(this.DataSet);
				this.BindListeners();
			}
			this._ignoreDataSetEvents = false;
			this._ignoreXmlEvents = false;
		}

		internal void Bind(DataRow r, XmlBoundElement e)
		{
			r.Element = e;
			e.Row = r;
		}

		private void BindSpecialListeners()
		{
			this._dataSet.DataRowCreated += this.OnDataRowCreatedSpecial;
			this._fDataRowCreatedSpecial = true;
		}

		private void UnBindSpecialListeners()
		{
			this._dataSet.DataRowCreated -= this.OnDataRowCreatedSpecial;
			this._fDataRowCreatedSpecial = false;
		}

		private void BindListeners()
		{
			this.BindToDocument();
			this.BindToDataSet();
		}

		private void BindToDataSet()
		{
			if (this._fBoundToDataSet)
			{
				return;
			}
			if (this._fDataRowCreatedSpecial)
			{
				this.UnBindSpecialListeners();
			}
			this._dataSet.Tables.CollectionChanging += this.OnDataSetTablesChanging;
			this._dataSet.Relations.CollectionChanging += this.OnDataSetRelationsChanging;
			this._dataSet.DataRowCreated += this.OnDataRowCreated;
			this._dataSet.PropertyChanging += this.OnDataSetPropertyChanging;
			this._dataSet.ClearFunctionCalled += this.OnClearCalled;
			if (this._dataSet.Tables.Count > 0)
			{
				foreach (object obj in this._dataSet.Tables)
				{
					DataTable dataTable = (DataTable)obj;
					this.BindToTable(dataTable);
				}
			}
			foreach (object obj2 in this._dataSet.Relations)
			{
				((DataRelation)obj2).PropertyChanging += this.OnRelationPropertyChanging;
			}
			this._fBoundToDataSet = true;
		}

		private void BindToDocument()
		{
			if (!this._fBoundToDocument)
			{
				base.NodeInserting += this.OnNodeInserting;
				base.NodeInserted += this.OnNodeInserted;
				base.NodeRemoving += this.OnNodeRemoving;
				base.NodeRemoved += this.OnNodeRemoved;
				base.NodeChanging += this.OnNodeChanging;
				base.NodeChanged += this.OnNodeChanged;
				this._fBoundToDocument = true;
			}
		}

		private void BindToTable(DataTable t)
		{
			t.ColumnChanged += this.OnColumnChanged;
			t.RowChanging += this.OnRowChanging;
			t.RowChanged += this.OnRowChanged;
			t.RowDeleting += this.OnRowChanging;
			t.RowDeleted += this.OnRowChanged;
			t.PropertyChanging += this.OnTablePropertyChanging;
			t.Columns.CollectionChanging += this.OnTableColumnsChanging;
			foreach (object obj in t.Columns)
			{
				((DataColumn)obj).PropertyChanging += this.OnColumnPropertyChanging;
			}
		}

		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			if (namespaceURI == null)
			{
				namespaceURI = string.Empty;
			}
			if (!this._fAssociateDataRow)
			{
				return new XmlBoundElement(prefix, localName, namespaceURI, this);
			}
			this.EnsurePopulatedMode();
			DataTable dataTable = this._mapper.SearchMatchingTableSchema(localName, namespaceURI);
			if (dataTable != null)
			{
				DataRow dataRow = dataTable.CreateEmptyRow();
				foreach (object obj in dataTable.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					if (dataColumn.ColumnMapping != MappingType.Hidden)
					{
						XmlDataDocument.SetRowValueToNull(dataRow, dataColumn);
					}
				}
				XmlBoundElement element = dataRow.Element;
				element.Prefix = prefix;
				return element;
			}
			return new XmlBoundElement(prefix, localName, namespaceURI, this);
		}

		public override XmlEntityReference CreateEntityReference(string name)
		{
			throw new NotSupportedException("Cannot create entity references on DataDocument.");
		}

		public DataSet DataSet
		{
			get
			{
				return this._dataSet;
			}
		}

		private void DefoliateRegion(XmlBoundElement rowElem)
		{
			if (!this._optimizeStorage)
			{
				return;
			}
			if (rowElem.ElementState != ElementState.WeakFoliation)
			{
				return;
			}
			if (!this._mapper.IsRegionRadical(rowElem))
			{
				return;
			}
			bool ignoreXmlEvents = this.IgnoreXmlEvents;
			this.IgnoreXmlEvents = true;
			rowElem.ElementState = ElementState.Defoliating;
			try
			{
				rowElem.RemoveAllAttributes();
				XmlNode nextSibling;
				for (XmlNode xmlNode = rowElem.FirstChild; xmlNode != null; xmlNode = nextSibling)
				{
					nextSibling = xmlNode.NextSibling;
					XmlBoundElement xmlBoundElement = xmlNode as XmlBoundElement;
					if (xmlBoundElement != null && xmlBoundElement.Row != null)
					{
						break;
					}
					rowElem.RemoveChild(xmlNode);
				}
				rowElem.ElementState = ElementState.Defoliated;
			}
			finally
			{
				this.IgnoreXmlEvents = ignoreXmlEvents;
			}
		}

		private XmlElement EnsureDocumentElement()
		{
			XmlElement xmlElement = base.DocumentElement;
			if (xmlElement == null)
			{
				string text = XmlConvert.EncodeLocalName(this.DataSet.DataSetName);
				if (text == null || text.Length == 0)
				{
					text = "Xml";
				}
				string text2 = this.DataSet.Namespace;
				if (text2 == null)
				{
					text2 = string.Empty;
				}
				xmlElement = new XmlBoundElement(string.Empty, text, text2, this);
				this.AppendChild(xmlElement);
			}
			return xmlElement;
		}

		private XmlElement EnsureNonRowDocumentElement()
		{
			XmlElement documentElement = base.DocumentElement;
			if (documentElement == null)
			{
				return this.EnsureDocumentElement();
			}
			if (this.GetRowFromElement(documentElement) == null)
			{
				return documentElement;
			}
			return this.DemoteDocumentElement();
		}

		private XmlElement DemoteDocumentElement()
		{
			XmlElement documentElement = base.DocumentElement;
			this.RemoveChild(documentElement);
			XmlElement xmlElement = this.EnsureDocumentElement();
			xmlElement.AppendChild(documentElement);
			return xmlElement;
		}

		private void EnsurePopulatedMode()
		{
			if (this._fDataRowCreatedSpecial)
			{
				this.UnBindSpecialListeners();
				this._mapper.SetupMapping(this, this._dataSet);
				this.BindListeners();
				this._fAssociateDataRow = true;
			}
		}

		private void FixNestedChildren(DataRow row, XmlElement rowElement)
		{
			foreach (object obj in this.GetNestedChildRelations(row))
			{
				DataRelation dataRelation = (DataRelation)obj;
				DataRow[] childRows = row.GetChildRows(dataRelation);
				for (int i = 0; i < childRows.Length; i++)
				{
					XmlElement element = childRows[i].Element;
					if (element != null && element.ParentNode != rowElement)
					{
						element.ParentNode.RemoveChild(element);
						rowElement.AppendChild(element);
					}
				}
			}
		}

		internal void Foliate(XmlBoundElement node, ElementState newState)
		{
			if (this.IsFoliationEnabled)
			{
				if (node.ElementState == ElementState.Defoliated)
				{
					this.ForceFoliation(node, newState);
					return;
				}
				if (node.ElementState == ElementState.WeakFoliation && newState == ElementState.StrongFoliation)
				{
					node.ElementState = newState;
				}
			}
		}

		private void Foliate(XmlElement element)
		{
			if (element is XmlBoundElement)
			{
				((XmlBoundElement)element).Foliate(ElementState.WeakFoliation);
			}
		}

		private void FoliateIfDataPointers(DataRow row, XmlElement rowElement)
		{
			if (!this.IsFoliated(rowElement) && this.HasPointers(rowElement))
			{
				bool isFoliationEnabled = this.IsFoliationEnabled;
				this.IsFoliationEnabled = true;
				try
				{
					this.Foliate(rowElement);
				}
				finally
				{
					this.IsFoliationEnabled = isFoliationEnabled;
				}
			}
		}

		private void EnsureFoliation(XmlBoundElement rowElem, ElementState foliation)
		{
			if (rowElem.IsFoliated)
			{
				return;
			}
			this.ForceFoliation(rowElem, foliation);
		}

		private void ForceFoliation(XmlBoundElement node, ElementState newState)
		{
			object foliationLock = this._foliationLock;
			lock (foliationLock)
			{
				if (node.ElementState == ElementState.Defoliated)
				{
					node.ElementState = ElementState.Foliating;
					bool ignoreXmlEvents = this.IgnoreXmlEvents;
					this.IgnoreXmlEvents = true;
					try
					{
						XmlNode xmlNode = null;
						DataRow row = node.Row;
						DataRowVersion dataRowVersion = ((row.RowState == DataRowState.Detached) ? DataRowVersion.Proposed : DataRowVersion.Current);
						foreach (object obj in row.Table.Columns)
						{
							DataColumn dataColumn = (DataColumn)obj;
							if (!this.IsNotMapped(dataColumn))
							{
								object obj2 = row[dataColumn, dataRowVersion];
								if (!Convert.IsDBNull(obj2))
								{
									if (dataColumn.ColumnMapping == MappingType.Attribute)
									{
										node.SetAttribute(dataColumn.EncodedColumnName, dataColumn.Namespace, dataColumn.ConvertObjectToXml(obj2));
									}
									else if (dataColumn.ColumnMapping == MappingType.Element)
									{
										XmlNode xmlNode2 = new XmlBoundElement(string.Empty, dataColumn.EncodedColumnName, dataColumn.Namespace, this);
										xmlNode2.AppendChild(this.CreateTextNode(dataColumn.ConvertObjectToXml(obj2)));
										if (xmlNode != null)
										{
											node.InsertAfter(xmlNode2, xmlNode);
										}
										else if (node.FirstChild != null)
										{
											node.InsertBefore(xmlNode2, node.FirstChild);
										}
										else
										{
											node.AppendChild(xmlNode2);
										}
										xmlNode = xmlNode2;
									}
									else
									{
										XmlNode xmlNode2 = this.CreateTextNode(dataColumn.ConvertObjectToXml(obj2));
										if (node.FirstChild != null)
										{
											node.InsertBefore(xmlNode2, node.FirstChild);
										}
										else
										{
											node.AppendChild(xmlNode2);
										}
										if (xmlNode == null)
										{
											xmlNode = xmlNode2;
										}
									}
								}
								else if (dataColumn.ColumnMapping == MappingType.SimpleContent)
								{
									XmlAttribute xmlAttribute = this.CreateAttribute("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance");
									xmlAttribute.Value = "true";
									node.SetAttributeNode(xmlAttribute);
									this._bHasXSINIL = true;
								}
							}
						}
					}
					finally
					{
						this.IgnoreXmlEvents = ignoreXmlEvents;
						node.ElementState = newState;
					}
					this.OnFoliated(node);
				}
			}
		}

		private XmlNode GetColumnInsertAfterLocation(DataRow row, DataColumn col, XmlBoundElement rowElement)
		{
			XmlNode xmlNode = null;
			if (this.IsTextOnly(col))
			{
				return null;
			}
			for (XmlNode xmlNode2 = rowElement.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
			{
				if (!XmlDataDocument.IsTextLikeNode(xmlNode2))
				{
					IL_0081:
					while (xmlNode2 != null && xmlNode2.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = xmlNode2 as XmlElement;
						if (this._mapper.GetRowFromElement(xmlElement) != null)
						{
							break;
						}
						object columnSchemaForNode = this._mapper.GetColumnSchemaForNode(rowElement, xmlNode2);
						if (columnSchemaForNode == null || !(columnSchemaForNode is DataColumn) || ((DataColumn)columnSchemaForNode).Ordinal > col.Ordinal)
						{
							break;
						}
						xmlNode = xmlNode2;
						xmlNode2 = xmlNode2.NextSibling;
					}
					return xmlNode;
				}
				xmlNode = xmlNode2;
			}
			goto IL_0081;
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

		private DataRow GetNestedParent(DataRow row)
		{
			DataRelation nestedParentRelation = XmlDataDocument.GetNestedParentRelation(row);
			if (nestedParentRelation != null)
			{
				return row.GetParentRow(nestedParentRelation);
			}
			return null;
		}

		private static DataRelation GetNestedParentRelation(DataRow row)
		{
			DataRelation[] nestedParentRelations = row.Table.NestedParentRelations;
			if (nestedParentRelations.Length == 0)
			{
				return null;
			}
			return nestedParentRelations[0];
		}

		private DataColumn GetTextOnlyColumn(DataRow row)
		{
			return row.Table.XmlText;
		}

		public DataRow GetRowFromElement(XmlElement e)
		{
			return this._mapper.GetRowFromElement(e);
		}

		private XmlNode GetRowInsertBeforeLocation(DataRow row, XmlElement rowElement, XmlNode parentElement)
		{
			DataRow dataRow = row;
			int i = 0;
			while (i < row.Table.Rows.Count && row != row.Table.Rows[i])
			{
				i++;
			}
			int num = i;
			DataRow nestedParent = this.GetNestedParent(row);
			for (i = num + 1; i < row.Table.Rows.Count; i++)
			{
				dataRow = row.Table.Rows[i];
				if (this.GetNestedParent(dataRow) == nestedParent && this.GetElementFromRow(dataRow).ParentNode == parentElement)
				{
					break;
				}
			}
			if (i < row.Table.Rows.Count)
			{
				return this.GetElementFromRow(dataRow);
			}
			return null;
		}

		public XmlElement GetElementFromRow(DataRow r)
		{
			return r.Element;
		}

		internal bool HasPointers(XmlNode node)
		{
			bool flag;
			for (;;)
			{
				try
				{
					if (this._pointers.Count > 0)
					{
						foreach (object obj in this._pointers)
						{
							if (((IXmlDataVirtualNode)((DictionaryEntry)obj).Value).IsOnNode(node))
							{
								return true;
							}
						}
					}
					flag = false;
				}
				catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
				{
					continue;
				}
				break;
			}
			return flag;
		}

		internal bool IgnoreXmlEvents
		{
			get
			{
				return this._ignoreXmlEvents;
			}
			set
			{
				this._ignoreXmlEvents = value;
			}
		}

		internal bool IgnoreDataSetEvents
		{
			get
			{
				return this._ignoreDataSetEvents;
			}
			set
			{
				this._ignoreDataSetEvents = value;
			}
		}

		private bool IsFoliated(XmlElement element)
		{
			return !(element is XmlBoundElement) || ((XmlBoundElement)element).IsFoliated;
		}

		private bool IsFoliated(XmlBoundElement be)
		{
			return be.IsFoliated;
		}

		internal bool IsFoliationEnabled
		{
			get
			{
				return this._isFoliationEnabled;
			}
			set
			{
				this._isFoliationEnabled = value;
			}
		}

		internal XmlNode CloneTree(DataPointer other)
		{
			this.EnsurePopulatedMode();
			bool ignoreDataSetEvents = this._ignoreDataSetEvents;
			bool ignoreXmlEvents = this._ignoreXmlEvents;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			bool fAssociateDataRow = this._fAssociateDataRow;
			XmlNode xmlNode;
			try
			{
				this._ignoreDataSetEvents = true;
				this._ignoreXmlEvents = true;
				this.IsFoliationEnabled = false;
				this._fAssociateDataRow = false;
				xmlNode = this.CloneTreeInternal(other);
				this.LoadRows(null, xmlNode);
				this.SyncRows(null, xmlNode, false);
			}
			finally
			{
				this._ignoreDataSetEvents = ignoreDataSetEvents;
				this._ignoreXmlEvents = ignoreXmlEvents;
				this.IsFoliationEnabled = isFoliationEnabled;
				this._fAssociateDataRow = fAssociateDataRow;
			}
			return xmlNode;
		}

		private XmlNode CloneTreeInternal(DataPointer other)
		{
			XmlNode xmlNode = this.CloneNode(other);
			DataPointer dataPointer = new DataPointer(other);
			try
			{
				dataPointer.AddPointer();
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					int attributeCount = dataPointer.AttributeCount;
					for (int i = 0; i < attributeCount; i++)
					{
						dataPointer.MoveToOwnerElement();
						if (dataPointer.MoveToAttribute(i))
						{
							xmlNode.Attributes.Append((XmlAttribute)this.CloneTreeInternal(dataPointer));
						}
					}
					dataPointer.MoveTo(other);
				}
				bool flag = dataPointer.MoveToFirstChild();
				while (flag)
				{
					xmlNode.AppendChild(this.CloneTreeInternal(dataPointer));
					flag = dataPointer.MoveToNextSibling();
				}
			}
			finally
			{
				dataPointer.SetNoLongerUse();
			}
			return xmlNode;
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDataDocument xmlDataDocument = (XmlDataDocument)base.CloneNode(false);
			xmlDataDocument.Init(this.DataSet.Clone());
			xmlDataDocument._dataSet.EnforceConstraints = this._dataSet.EnforceConstraints;
			if (deep)
			{
				DataPointer dataPointer = new DataPointer(this, this);
				try
				{
					dataPointer.AddPointer();
					bool flag = dataPointer.MoveToFirstChild();
					while (flag)
					{
						XmlNode xmlNode;
						if (dataPointer.NodeType == XmlNodeType.Element)
						{
							xmlNode = xmlDataDocument.CloneTree(dataPointer);
						}
						else
						{
							xmlNode = xmlDataDocument.CloneNode(dataPointer);
						}
						xmlDataDocument.AppendChild(xmlNode);
						flag = dataPointer.MoveToNextSibling();
					}
				}
				finally
				{
					dataPointer.SetNoLongerUse();
				}
			}
			return xmlDataDocument;
		}

		private XmlNode CloneNode(DataPointer dp)
		{
			switch (dp.NodeType)
			{
			case XmlNodeType.Element:
				return this.CreateElement(dp.Prefix, dp.LocalName, dp.NamespaceURI);
			case XmlNodeType.Attribute:
				return this.CreateAttribute(dp.Prefix, dp.LocalName, dp.NamespaceURI);
			case XmlNodeType.Text:
				return this.CreateTextNode(dp.Value);
			case XmlNodeType.CDATA:
				return this.CreateCDataSection(dp.Value);
			case XmlNodeType.EntityReference:
				return this.CreateEntityReference(dp.Name);
			case XmlNodeType.ProcessingInstruction:
				return this.CreateProcessingInstruction(dp.Name, dp.Value);
			case XmlNodeType.Comment:
				return this.CreateComment(dp.Value);
			case XmlNodeType.DocumentType:
				return this.CreateDocumentType(dp.Name, dp.PublicId, dp.SystemId, dp.InternalSubset);
			case XmlNodeType.DocumentFragment:
				return this.CreateDocumentFragment();
			case XmlNodeType.Whitespace:
				return this.CreateWhitespace(dp.Value);
			case XmlNodeType.SignificantWhitespace:
				return this.CreateSignificantWhitespace(dp.Value);
			case XmlNodeType.XmlDeclaration:
				return this.CreateXmlDeclaration(dp.Version, dp.Encoding, dp.Standalone);
			}
			throw new InvalidOperationException(SR.Format("This type of node cannot be cloned: {0}.", dp.NodeType.ToString()));
		}

		internal static bool IsTextLikeNode(XmlNode n)
		{
			XmlNodeType nodeType = n.NodeType;
			if (nodeType - XmlNodeType.Text > 1)
			{
				if (nodeType == XmlNodeType.EntityReference)
				{
					return false;
				}
				if (nodeType - XmlNodeType.Whitespace > 1)
				{
					return false;
				}
			}
			return true;
		}

		internal bool IsNotMapped(DataColumn c)
		{
			return DataSetMapper.IsNotMapped(c);
		}

		private bool IsSame(DataColumn c, int recNo1, int recNo2)
		{
			return c.Compare(recNo1, recNo2) == 0;
		}

		internal bool IsTextOnly(DataColumn c)
		{
			return c.ColumnMapping == MappingType.SimpleContent;
		}

		public override void Load(string filename)
		{
			this._bForceExpandEntity = true;
			base.Load(filename);
			this._bForceExpandEntity = false;
		}

		public override void Load(Stream inStream)
		{
			this._bForceExpandEntity = true;
			base.Load(inStream);
			this._bForceExpandEntity = false;
		}

		public override void Load(TextReader txtReader)
		{
			this._bForceExpandEntity = true;
			base.Load(txtReader);
			this._bForceExpandEntity = false;
		}

		public override void Load(XmlReader reader)
		{
			if (this.FirstChild != null)
			{
				throw new InvalidOperationException("Cannot load XmlDataDocument if it already contains data. Please use a new XmlDataDocument.");
			}
			try
			{
				this._ignoreXmlEvents = true;
				if (this._fDataRowCreatedSpecial)
				{
					this.UnBindSpecialListeners();
				}
				this._fAssociateDataRow = false;
				this._isFoliationEnabled = false;
				if (this._bForceExpandEntity)
				{
					((XmlTextReader)reader).EntityHandling = EntityHandling.ExpandEntities;
				}
				base.Load(reader);
				this.BindForLoad();
			}
			finally
			{
				this._ignoreXmlEvents = false;
				this._isFoliationEnabled = true;
				this._autoFoliationState = ElementState.StrongFoliation;
				this._fAssociateDataRow = true;
			}
		}

		private void LoadDataSetFromTree()
		{
			this._ignoreDataSetEvents = true;
			this._ignoreXmlEvents = true;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = false;
			bool enforceConstraints = this._dataSet.EnforceConstraints;
			this._dataSet.EnforceConstraints = false;
			try
			{
				this.LoadRows(null, base.DocumentElement);
				this.SyncRows(null, base.DocumentElement, true);
				this._dataSet.EnforceConstraints = enforceConstraints;
			}
			finally
			{
				this._ignoreDataSetEvents = false;
				this._ignoreXmlEvents = false;
				this.IsFoliationEnabled = isFoliationEnabled;
			}
		}

		private void LoadTreeFromDataSet(DataSet ds)
		{
			this._ignoreDataSetEvents = true;
			this._ignoreXmlEvents = true;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = false;
			this._fAssociateDataRow = false;
			DataTable[] array = this.OrderTables(ds);
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					foreach (object obj in array[i].Rows)
					{
						DataRow dataRow = (DataRow)obj;
						this.AttachBoundElementToDataRow(dataRow);
						DataRowState rowState = dataRow.RowState;
						switch (rowState)
						{
						case DataRowState.Detached:
						case DataRowState.Detached | DataRowState.Unchanged:
							continue;
						case DataRowState.Unchanged:
						case DataRowState.Added:
							break;
						default:
							if (rowState == DataRowState.Deleted || rowState != DataRowState.Modified)
							{
								continue;
							}
							break;
						}
						this.OnAddRow(dataRow);
					}
				}
			}
			finally
			{
				this._ignoreDataSetEvents = false;
				this._ignoreXmlEvents = false;
				this.IsFoliationEnabled = isFoliationEnabled;
				this._fAssociateDataRow = true;
			}
		}

		private void LoadRows(XmlBoundElement rowElem, XmlNode node)
		{
			XmlBoundElement xmlBoundElement = node as XmlBoundElement;
			if (xmlBoundElement != null)
			{
				DataTable dataTable = this._mapper.SearchMatchingTableSchema(rowElem, xmlBoundElement);
				if (dataTable != null)
				{
					DataRow dataRow = this.GetRowFromElement(xmlBoundElement);
					if (xmlBoundElement.ElementState == ElementState.None)
					{
						xmlBoundElement.ElementState = ElementState.WeakFoliation;
					}
					dataRow = dataTable.CreateEmptyRow();
					this.Bind(dataRow, xmlBoundElement);
					rowElem = xmlBoundElement;
				}
			}
			for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				this.LoadRows(rowElem, xmlNode);
			}
		}

		internal DataSetMapper Mapper
		{
			get
			{
				return this._mapper;
			}
		}

		internal void OnDataRowCreated(object oDataSet, DataRow row)
		{
			this.OnNewRow(row);
		}

		internal void OnClearCalled(object oDataSet, DataTable table)
		{
			throw new NotSupportedException("Clear function on DateSet and DataTable is not supported on XmlDataDocument.");
		}

		internal void OnDataRowCreatedSpecial(object oDataSet, DataRow row)
		{
			this.Bind(true);
			this.OnNewRow(row);
		}

		internal void OnNewRow(DataRow row)
		{
			this.AttachBoundElementToDataRow(row);
		}

		private XmlBoundElement AttachBoundElementToDataRow(DataRow row)
		{
			DataTable table = row.Table;
			XmlBoundElement xmlBoundElement = new XmlBoundElement(string.Empty, table.EncodedTableName, table.Namespace, this);
			xmlBoundElement.IsEmpty = false;
			this.Bind(row, xmlBoundElement);
			xmlBoundElement.ElementState = ElementState.Defoliated;
			return xmlBoundElement;
		}

		private bool NeedXSI_NilAttr(DataRow row)
		{
			DataTable table = row.Table;
			return table._xmlText != null && Convert.IsDBNull(row[table._xmlText]);
		}

		private void OnAddRow(DataRow row)
		{
			XmlBoundElement xmlBoundElement = (XmlBoundElement)this.GetElementFromRow(row);
			if (this.NeedXSI_NilAttr(row) && !xmlBoundElement.IsFoliated)
			{
				this.ForceFoliation(xmlBoundElement, this.AutoFoliationState);
			}
			if (this.GetRowFromElement(base.DocumentElement) != null && this.GetNestedParent(row) == null)
			{
				this.DemoteDocumentElement();
			}
			this.EnsureDocumentElement().AppendChild(xmlBoundElement);
			this.FixNestedChildren(row, xmlBoundElement);
			this.OnNestedParentChange(row, xmlBoundElement, null);
		}

		private void OnColumnValueChanged(DataRow row, DataColumn col, XmlBoundElement rowElement)
		{
			if (!this.IsNotMapped(col))
			{
				object obj = row[col];
				if (col.ColumnMapping == MappingType.SimpleContent && Convert.IsDBNull(obj) && !rowElement.IsFoliated)
				{
					this.ForceFoliation(rowElement, ElementState.WeakFoliation);
				}
				else if (!this.IsFoliated(rowElement))
				{
					goto IL_0318;
				}
				if (this.IsTextOnly(col))
				{
					if (Convert.IsDBNull(obj))
					{
						obj = string.Empty;
						XmlAttribute xmlAttribute = rowElement.GetAttributeNode("xsi:nil");
						if (xmlAttribute == null)
						{
							xmlAttribute = this.CreateAttribute("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance");
							xmlAttribute.Value = "true";
							rowElement.SetAttributeNode(xmlAttribute);
							this._bHasXSINIL = true;
						}
						else
						{
							xmlAttribute.Value = "true";
						}
					}
					else
					{
						XmlAttribute attributeNode = rowElement.GetAttributeNode("xsi:nil");
						if (attributeNode != null)
						{
							attributeNode.Value = "false";
						}
					}
					this.ReplaceInitialChildText(rowElement, col.ConvertObjectToXml(obj));
				}
				else
				{
					bool flag = false;
					if (col.ColumnMapping == MappingType.Attribute)
					{
						foreach (object obj2 in rowElement.Attributes)
						{
							XmlAttribute xmlAttribute2 = (XmlAttribute)obj2;
							if (xmlAttribute2.LocalName == col.EncodedColumnName && xmlAttribute2.NamespaceURI == col.Namespace)
							{
								if (Convert.IsDBNull(obj))
								{
									xmlAttribute2.OwnerElement.Attributes.Remove(xmlAttribute2);
								}
								else
								{
									xmlAttribute2.Value = col.ConvertObjectToXml(obj);
								}
								flag = true;
								break;
							}
						}
						if (!flag && !Convert.IsDBNull(obj))
						{
							rowElement.SetAttribute(col.EncodedColumnName, col.Namespace, col.ConvertObjectToXml(obj));
						}
					}
					else
					{
						RegionIterator regionIterator = new RegionIterator(rowElement);
						bool flag2 = regionIterator.Next();
						while (flag2)
						{
							if (regionIterator.CurrentNode.NodeType == XmlNodeType.Element)
							{
								XmlElement xmlElement = (XmlElement)regionIterator.CurrentNode;
								XmlBoundElement xmlBoundElement = xmlElement as XmlBoundElement;
								if (xmlBoundElement != null && xmlBoundElement.Row != null)
								{
									flag2 = regionIterator.NextRight();
									continue;
								}
								if (xmlElement.LocalName == col.EncodedColumnName && xmlElement.NamespaceURI == col.Namespace)
								{
									flag = true;
									if (Convert.IsDBNull(obj))
									{
										this.PromoteNonValueChildren(xmlElement);
										flag2 = regionIterator.NextRight();
										xmlElement.ParentNode.RemoveChild(xmlElement);
										continue;
									}
									this.ReplaceInitialChildText(xmlElement, col.ConvertObjectToXml(obj));
									XmlAttribute attributeNode2 = xmlElement.GetAttributeNode("xsi:nil");
									if (attributeNode2 != null)
									{
										attributeNode2.Value = "false";
										goto IL_0318;
									}
									goto IL_0318;
								}
							}
							flag2 = regionIterator.Next();
						}
						if (!flag && !Convert.IsDBNull(obj))
						{
							XmlElement xmlElement2 = new XmlBoundElement(string.Empty, col.EncodedColumnName, col.Namespace, this);
							xmlElement2.AppendChild(this.CreateTextNode(col.ConvertObjectToXml(obj)));
							XmlNode columnInsertAfterLocation = this.GetColumnInsertAfterLocation(row, col, rowElement);
							if (columnInsertAfterLocation != null)
							{
								rowElement.InsertAfter(xmlElement2, columnInsertAfterLocation);
							}
							else if (rowElement.FirstChild != null)
							{
								rowElement.InsertBefore(xmlElement2, rowElement.FirstChild);
							}
							else
							{
								rowElement.AppendChild(xmlElement2);
							}
						}
					}
				}
			}
			IL_0318:
			DataRelation nestedParentRelation = XmlDataDocument.GetNestedParentRelation(row);
			if (nestedParentRelation != null && nestedParentRelation.ChildKey.ContainsColumn(col))
			{
				this.OnNestedParentChange(row, rowElement, col);
			}
		}

		private void OnColumnChanged(object sender, DataColumnChangeEventArgs args)
		{
			if (this._ignoreDataSetEvents)
			{
				return;
			}
			bool ignoreXmlEvents = this._ignoreXmlEvents;
			this._ignoreXmlEvents = true;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = false;
			try
			{
				DataRow row = args.Row;
				DataColumn column = args.Column;
				object proposedValue = args.ProposedValue;
				if (row.RowState == DataRowState.Detached)
				{
					XmlBoundElement element = row.Element;
					if (element.IsFoliated)
					{
						this.OnColumnValueChanged(row, column, element);
					}
				}
			}
			finally
			{
				this.IsFoliationEnabled = isFoliationEnabled;
				this._ignoreXmlEvents = ignoreXmlEvents;
			}
		}

		private void OnColumnValuesChanged(DataRow row, XmlBoundElement rowElement)
		{
			if (this._columnChangeList.Count > 0)
			{
				if (((DataColumn)this._columnChangeList[0]).Table == row.Table)
				{
					using (IEnumerator enumerator = this._columnChangeList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							DataColumn dataColumn = (DataColumn)obj;
							this.OnColumnValueChanged(row, dataColumn, rowElement);
						}
						goto IL_00F8;
					}
				}
				using (IEnumerator enumerator = row.Table.Columns.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj2 = enumerator.Current;
						DataColumn dataColumn2 = (DataColumn)obj2;
						this.OnColumnValueChanged(row, dataColumn2, rowElement);
					}
					goto IL_00F8;
				}
			}
			foreach (object obj3 in row.Table.Columns)
			{
				DataColumn dataColumn3 = (DataColumn)obj3;
				this.OnColumnValueChanged(row, dataColumn3, rowElement);
			}
			IL_00F8:
			this._columnChangeList.Clear();
		}

		private void OnDeleteRow(DataRow row, XmlBoundElement rowElement)
		{
			if (rowElement == base.DocumentElement)
			{
				this.DemoteDocumentElement();
			}
			this.PromoteInnerRegions(rowElement);
			rowElement.ParentNode.RemoveChild(rowElement);
		}

		private void OnDeletingRow(DataRow row, XmlBoundElement rowElement)
		{
			if (this.IsFoliated(rowElement))
			{
				return;
			}
			bool ignoreXmlEvents = this.IgnoreXmlEvents;
			this.IgnoreXmlEvents = true;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = true;
			try
			{
				this.Foliate(rowElement);
			}
			finally
			{
				this.IsFoliationEnabled = isFoliationEnabled;
				this.IgnoreXmlEvents = ignoreXmlEvents;
			}
		}

		private void OnFoliated(XmlNode node)
		{
			for (;;)
			{
				try
				{
					if (this._pointers.Count > 0)
					{
						foreach (object obj in this._pointers)
						{
							((IXmlDataVirtualNode)((DictionaryEntry)obj).Value).OnFoliated(node);
						}
					}
				}
				catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
				{
					continue;
				}
				break;
			}
		}

		private DataColumn FindAssociatedParentColumn(DataRelation relation, DataColumn childCol)
		{
			DataColumn[] columnsReference = relation.ChildKey.ColumnsReference;
			for (int i = 0; i < columnsReference.Length; i++)
			{
				if (childCol == columnsReference[i])
				{
					return relation.ParentKey.ColumnsReference[i];
				}
			}
			return null;
		}

		private void OnNestedParentChange(DataRow child, XmlBoundElement childElement, DataColumn childCol)
		{
			DataRow dataRow;
			if (childElement == base.DocumentElement || childElement.ParentNode == null)
			{
				dataRow = null;
			}
			else
			{
				dataRow = this.GetRowFromElement((XmlElement)childElement.ParentNode);
			}
			DataRow nestedParent = this.GetNestedParent(child);
			if (dataRow != nestedParent)
			{
				if (nestedParent != null)
				{
					this.GetElementFromRow(nestedParent).AppendChild(childElement);
					return;
				}
				DataRelation nestedParentRelation = XmlDataDocument.GetNestedParentRelation(child);
				if (childCol == null || nestedParentRelation == null || Convert.IsDBNull(child[childCol]))
				{
					this.EnsureNonRowDocumentElement().AppendChild(childElement);
					return;
				}
				DataColumn dataColumn = this.FindAssociatedParentColumn(nestedParentRelation, childCol);
				object obj = dataColumn.ConvertValue(child[childCol]);
				if (dataRow._tempRecord != -1 && dataColumn.CompareValueTo(dataRow._tempRecord, obj) != 0)
				{
					this.EnsureNonRowDocumentElement().AppendChild(childElement);
				}
			}
		}

		private void OnNodeChanged(object sender, XmlNodeChangedEventArgs args)
		{
			if (this._ignoreXmlEvents)
			{
				return;
			}
			bool ignoreDataSetEvents = this._ignoreDataSetEvents;
			bool ignoreXmlEvents = this._ignoreXmlEvents;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this._ignoreDataSetEvents = true;
			this._ignoreXmlEvents = true;
			this.IsFoliationEnabled = false;
			bool fEnableCascading = this.DataSet._fEnableCascading;
			this.DataSet._fEnableCascading = false;
			try
			{
				XmlBoundElement xmlBoundElement = null;
				if (this._mapper.GetRegion(args.Node, out xmlBoundElement))
				{
					this.SynchronizeRowFromRowElement(xmlBoundElement);
				}
			}
			finally
			{
				this._ignoreDataSetEvents = ignoreDataSetEvents;
				this._ignoreXmlEvents = ignoreXmlEvents;
				this.IsFoliationEnabled = isFoliationEnabled;
				this.DataSet._fEnableCascading = fEnableCascading;
			}
		}

		private void OnNodeChanging(object sender, XmlNodeChangedEventArgs args)
		{
			if (this._ignoreXmlEvents)
			{
				return;
			}
			if (this.DataSet.EnforceConstraints)
			{
				throw new InvalidOperationException("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations.");
			}
		}

		private void OnNodeInserted(object sender, XmlNodeChangedEventArgs args)
		{
			if (this._ignoreXmlEvents)
			{
				return;
			}
			bool ignoreDataSetEvents = this._ignoreDataSetEvents;
			bool ignoreXmlEvents = this._ignoreXmlEvents;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this._ignoreDataSetEvents = true;
			this._ignoreXmlEvents = true;
			this.IsFoliationEnabled = false;
			bool fEnableCascading = this.DataSet._fEnableCascading;
			this.DataSet._fEnableCascading = false;
			try
			{
				XmlNode node = args.Node;
				XmlNode oldParent = args.OldParent;
				XmlNode newParent = args.NewParent;
				if (this.IsConnected(newParent))
				{
					this.OnNodeInsertedInTree(node);
				}
				else
				{
					this.OnNodeInsertedInFragment(node);
				}
			}
			finally
			{
				this._ignoreDataSetEvents = ignoreDataSetEvents;
				this._ignoreXmlEvents = ignoreXmlEvents;
				this.IsFoliationEnabled = isFoliationEnabled;
				this.DataSet._fEnableCascading = fEnableCascading;
			}
		}

		private void OnNodeInserting(object sender, XmlNodeChangedEventArgs args)
		{
			if (this._ignoreXmlEvents)
			{
				return;
			}
			if (this.DataSet.EnforceConstraints)
			{
				throw new InvalidOperationException("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations.");
			}
		}

		private void OnNodeRemoved(object sender, XmlNodeChangedEventArgs args)
		{
			if (this._ignoreXmlEvents)
			{
				return;
			}
			bool ignoreDataSetEvents = this._ignoreDataSetEvents;
			bool ignoreXmlEvents = this._ignoreXmlEvents;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this._ignoreDataSetEvents = true;
			this._ignoreXmlEvents = true;
			this.IsFoliationEnabled = false;
			bool fEnableCascading = this.DataSet._fEnableCascading;
			this.DataSet._fEnableCascading = false;
			try
			{
				XmlNode node = args.Node;
				XmlNode oldParent = args.OldParent;
				if (this.IsConnected(oldParent))
				{
					this.OnNodeRemovedFromTree(node, oldParent);
				}
				else
				{
					this.OnNodeRemovedFromFragment(node, oldParent);
				}
			}
			finally
			{
				this._ignoreDataSetEvents = ignoreDataSetEvents;
				this._ignoreXmlEvents = ignoreXmlEvents;
				this.IsFoliationEnabled = isFoliationEnabled;
				this.DataSet._fEnableCascading = fEnableCascading;
			}
		}

		private void OnNodeRemoving(object sender, XmlNodeChangedEventArgs args)
		{
			if (this._ignoreXmlEvents)
			{
				return;
			}
			if (this.DataSet.EnforceConstraints)
			{
				throw new InvalidOperationException("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations.");
			}
		}

		private void OnNodeRemovedFromTree(XmlNode node, XmlNode oldParent)
		{
			XmlBoundElement xmlBoundElement;
			if (this._mapper.GetRegion(oldParent, out xmlBoundElement))
			{
				this.SynchronizeRowFromRowElement(xmlBoundElement);
			}
			XmlBoundElement xmlBoundElement2 = node as XmlBoundElement;
			if (xmlBoundElement2 != null && xmlBoundElement2.Row != null)
			{
				this.EnsureDisconnectedDataRow(xmlBoundElement2);
			}
			TreeIterator treeIterator = new TreeIterator(node);
			bool flag = treeIterator.NextRowElement();
			while (flag)
			{
				xmlBoundElement2 = (XmlBoundElement)treeIterator.CurrentNode;
				this.EnsureDisconnectedDataRow(xmlBoundElement2);
				flag = treeIterator.NextRowElement();
			}
		}

		private void OnNodeRemovedFromFragment(XmlNode node, XmlNode oldParent)
		{
			XmlBoundElement xmlBoundElement;
			if (this._mapper.GetRegion(oldParent, out xmlBoundElement))
			{
				DataRow row = xmlBoundElement.Row;
				if (xmlBoundElement.Row.RowState == DataRowState.Detached)
				{
					this.SynchronizeRowFromRowElement(xmlBoundElement);
				}
			}
			XmlBoundElement xmlBoundElement2 = node as XmlBoundElement;
			if (xmlBoundElement2 != null && xmlBoundElement2.Row != null)
			{
				this.SetNestedParentRegion(xmlBoundElement2, null);
				return;
			}
			TreeIterator treeIterator = new TreeIterator(node);
			bool flag = treeIterator.NextRowElement();
			while (flag)
			{
				XmlBoundElement xmlBoundElement3 = (XmlBoundElement)treeIterator.CurrentNode;
				this.SetNestedParentRegion(xmlBoundElement3, null);
				flag = treeIterator.NextRightRowElement();
			}
		}

		private void OnRowChanged(object sender, DataRowChangeEventArgs args)
		{
			if (this._ignoreDataSetEvents)
			{
				return;
			}
			this._ignoreXmlEvents = true;
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = false;
			try
			{
				DataRow row = args.Row;
				XmlBoundElement element = row.Element;
				DataRowAction action = args.Action;
				switch (action)
				{
				case DataRowAction.Delete:
					this.OnDeleteRow(row, element);
					break;
				case DataRowAction.Change:
					this.OnColumnValuesChanged(row, element);
					break;
				case DataRowAction.Delete | DataRowAction.Change:
					break;
				case DataRowAction.Rollback:
				{
					DataRowState rollbackState = this._rollbackState;
					if (rollbackState != DataRowState.Added)
					{
						if (rollbackState != DataRowState.Deleted)
						{
							if (rollbackState == DataRowState.Modified)
							{
								this.OnColumnValuesChanged(row, element);
							}
						}
						else
						{
							this.OnUndeleteRow(row, element);
							this.UpdateAllColumns(row, element);
						}
					}
					else
					{
						element.ParentNode.RemoveChild(element);
					}
					break;
				}
				default:
					if (action != DataRowAction.Commit)
					{
						if (action == DataRowAction.Add)
						{
							this.OnAddRow(row);
						}
					}
					else if (row.RowState == DataRowState.Detached)
					{
						element.RemoveAll();
					}
					break;
				}
			}
			finally
			{
				this.IsFoliationEnabled = isFoliationEnabled;
				this._ignoreXmlEvents = false;
			}
		}

		private void OnRowChanging(object sender, DataRowChangeEventArgs args)
		{
			DataRow row = args.Row;
			if (args.Action == DataRowAction.Delete && row.Element != null)
			{
				this.OnDeletingRow(row, row.Element);
				return;
			}
			if (this._ignoreDataSetEvents)
			{
				return;
			}
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = false;
			try
			{
				this._ignoreXmlEvents = true;
				XmlElement elementFromRow = this.GetElementFromRow(row);
				if (elementFromRow != null)
				{
					DataRowAction action = args.Action;
					int num;
					int num2;
					switch (action)
					{
					case DataRowAction.Delete:
					case DataRowAction.Delete | DataRowAction.Change:
						goto IL_0212;
					case DataRowAction.Change:
						break;
					case DataRowAction.Rollback:
					{
						this._rollbackState = row.RowState;
						DataRowState rollbackState = this._rollbackState;
						if (rollbackState <= DataRowState.Added)
						{
							if (rollbackState != DataRowState.Detached && rollbackState != DataRowState.Added)
							{
								return;
							}
							goto IL_0212;
						}
						else
						{
							if (rollbackState == DataRowState.Deleted)
							{
								goto IL_0212;
							}
							if (rollbackState != DataRowState.Modified)
							{
								return;
							}
							this._columnChangeList.Clear();
							num = row.GetRecordFromVersion(DataRowVersion.Original);
							num2 = row.GetRecordFromVersion(DataRowVersion.Current);
							using (IEnumerator enumerator = row.Table.Columns.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									object obj = enumerator.Current;
									DataColumn dataColumn = (DataColumn)obj;
									if (!this.IsSame(dataColumn, num, num2))
									{
										this._columnChangeList.Add(dataColumn);
									}
								}
								return;
							}
						}
						break;
					}
					default:
						if (action != DataRowAction.Commit && action != DataRowAction.Add)
						{
							goto IL_0212;
						}
						goto IL_0212;
					}
					this._columnChangeList.Clear();
					num = row.GetRecordFromVersion(DataRowVersion.Proposed);
					num2 = row.GetRecordFromVersion(DataRowVersion.Current);
					foreach (object obj2 in row.Table.Columns)
					{
						DataColumn dataColumn2 = (DataColumn)obj2;
						object obj3 = row[dataColumn2, DataRowVersion.Proposed];
						object obj4 = row[dataColumn2, DataRowVersion.Current];
						if (Convert.IsDBNull(obj3) && !Convert.IsDBNull(obj4) && dataColumn2.ColumnMapping != MappingType.Hidden)
						{
							this.FoliateIfDataPointers(row, elementFromRow);
						}
						if (!this.IsSame(dataColumn2, num, num2))
						{
							this._columnChangeList.Add(dataColumn2);
						}
					}
				}
				IL_0212:;
			}
			finally
			{
				this._ignoreXmlEvents = false;
				this.IsFoliationEnabled = isFoliationEnabled;
			}
		}

		private void OnDataSetPropertyChanging(object oDataSet, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "DataSetName")
			{
				throw new InvalidOperationException("Cannot change the DataSet name once the DataSet is mapped to a loaded XML document.");
			}
		}

		private void OnColumnPropertyChanging(object oColumn, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "ColumnName")
			{
				throw new InvalidOperationException("Cannot change the column name once the associated DataSet is mapped to a loaded XML document.");
			}
			if (args.PropertyName == "Namespace")
			{
				throw new InvalidOperationException("Cannot change the column namespace once the associated DataSet is mapped to a loaded XML document.");
			}
			if (args.PropertyName == "ColumnMapping")
			{
				throw new InvalidOperationException("Cannot change the ColumnMapping property once the associated DataSet is mapped to a loaded XML document.");
			}
		}

		private void OnTablePropertyChanging(object oTable, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "TableName")
			{
				throw new InvalidOperationException("Cannot change the table name once the associated DataSet is mapped to a loaded XML document.");
			}
			if (args.PropertyName == "Namespace")
			{
				throw new InvalidOperationException("Cannot change the table namespace once the associated DataSet is mapped to a loaded XML document.");
			}
		}

		private void OnTableColumnsChanging(object oColumnsCollection, CollectionChangeEventArgs args)
		{
			throw new InvalidOperationException("Cannot add or remove columns from the table once the DataSet is mapped to a loaded XML document.");
		}

		private void OnDataSetTablesChanging(object oTablesCollection, CollectionChangeEventArgs args)
		{
			throw new InvalidOperationException("Cannot add or remove tables from the DataSet once the DataSet is mapped to a loaded XML document.");
		}

		private void OnDataSetRelationsChanging(object oRelationsCollection, CollectionChangeEventArgs args)
		{
			DataRelation dataRelation = (DataRelation)args.Element;
			if (dataRelation != null && dataRelation.Nested)
			{
				throw new InvalidOperationException("Cannot add, remove, or change Nested relations from the DataSet once the DataSet is mapped to a loaded XML document.");
			}
			if (args.Action == CollectionChangeAction.Refresh)
			{
				using (IEnumerator enumerator = ((DataRelationCollection)oRelationsCollection).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((DataRelation)enumerator.Current).Nested)
						{
							throw new InvalidOperationException("Cannot add, remove, or change Nested relations from the DataSet once the DataSet is mapped to a loaded XML document.");
						}
					}
				}
			}
		}

		private void OnRelationPropertyChanging(object oRelationsCollection, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "Nested")
			{
				throw new InvalidOperationException("Cannot add, remove, or change Nested relations from the DataSet once the DataSet is mapped to a loaded XML document.");
			}
		}

		private void OnUndeleteRow(DataRow row, XmlElement rowElement)
		{
			if (rowElement.ParentNode != null)
			{
				rowElement.ParentNode.RemoveChild(rowElement);
			}
			DataRow nestedParent = this.GetNestedParent(row);
			XmlElement xmlElement;
			if (nestedParent == null)
			{
				xmlElement = this.EnsureNonRowDocumentElement();
			}
			else
			{
				xmlElement = this.GetElementFromRow(nestedParent);
			}
			XmlNode rowInsertBeforeLocation;
			if ((rowInsertBeforeLocation = this.GetRowInsertBeforeLocation(row, rowElement, xmlElement)) != null)
			{
				xmlElement.InsertBefore(rowElement, rowInsertBeforeLocation);
			}
			else
			{
				xmlElement.AppendChild(rowElement);
			}
			this.FixNestedChildren(row, rowElement);
		}

		private void PromoteChild(XmlNode child, XmlNode prevSibling)
		{
			if (child.ParentNode != null)
			{
				child.ParentNode.RemoveChild(child);
			}
			prevSibling.ParentNode.InsertAfter(child, prevSibling);
		}

		private void PromoteInnerRegions(XmlNode parent)
		{
			XmlBoundElement xmlBoundElement;
			this._mapper.GetRegion(parent.ParentNode, out xmlBoundElement);
			TreeIterator treeIterator = new TreeIterator(parent);
			bool flag = treeIterator.NextRowElement();
			while (flag)
			{
				XmlBoundElement xmlBoundElement2 = (XmlBoundElement)treeIterator.CurrentNode;
				flag = treeIterator.NextRightRowElement();
				this.PromoteChild(xmlBoundElement2, parent);
				this.SetNestedParentRegion(xmlBoundElement2, xmlBoundElement);
			}
		}

		private void PromoteNonValueChildren(XmlNode parent)
		{
			XmlNode xmlNode = parent;
			XmlNode xmlNode2 = parent.FirstChild;
			bool flag = true;
			while (xmlNode2 != null)
			{
				XmlNode xmlNode3 = xmlNode2.NextSibling;
				if (!flag || !XmlDataDocument.IsTextLikeNode(xmlNode2))
				{
					flag = false;
					xmlNode3 = xmlNode2.NextSibling;
					this.PromoteChild(xmlNode2, xmlNode);
					xmlNode = xmlNode2;
				}
				xmlNode2 = xmlNode3;
			}
		}

		private void RemoveInitialTextNodes(XmlNode node)
		{
			while (node != null && XmlDataDocument.IsTextLikeNode(node))
			{
				XmlNode nextSibling = node.NextSibling;
				node.ParentNode.RemoveChild(node);
				node = nextSibling;
			}
		}

		private void ReplaceInitialChildText(XmlNode parent, string value)
		{
			XmlNode xmlNode = parent.FirstChild;
			while (xmlNode != null && xmlNode.NodeType == XmlNodeType.Whitespace)
			{
				xmlNode = xmlNode.NextSibling;
			}
			if (xmlNode != null)
			{
				if (xmlNode.NodeType == XmlNodeType.Text)
				{
					xmlNode.Value = value;
				}
				else
				{
					xmlNode = parent.InsertBefore(this.CreateTextNode(value), xmlNode);
				}
				this.RemoveInitialTextNodes(xmlNode.NextSibling);
				return;
			}
			parent.AppendChild(this.CreateTextNode(value));
		}

		internal XmlNode SafeFirstChild(XmlNode n)
		{
			XmlBoundElement xmlBoundElement = n as XmlBoundElement;
			if (xmlBoundElement != null)
			{
				return xmlBoundElement.SafeFirstChild;
			}
			return n.FirstChild;
		}

		internal XmlNode SafeNextSibling(XmlNode n)
		{
			XmlBoundElement xmlBoundElement = n as XmlBoundElement;
			if (xmlBoundElement != null)
			{
				return xmlBoundElement.SafeNextSibling;
			}
			return n.NextSibling;
		}

		internal XmlNode SafePreviousSibling(XmlNode n)
		{
			XmlBoundElement xmlBoundElement = n as XmlBoundElement;
			if (xmlBoundElement != null)
			{
				return xmlBoundElement.SafePreviousSibling;
			}
			return n.PreviousSibling;
		}

		internal static void SetRowValueToNull(DataRow row, DataColumn col)
		{
			if (!row.IsNull(col))
			{
				row[col] = DBNull.Value;
			}
		}

		internal static void SetRowValueFromXmlText(DataRow row, DataColumn col, string xmlText)
		{
			object obj;
			try
			{
				obj = col.ConvertXmlToObject(xmlText);
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				XmlDataDocument.SetRowValueToNull(row, col);
				return;
			}
			if (!obj.Equals(row[col]))
			{
				row[col] = obj;
			}
		}

		private void SynchronizeRowFromRowElement(XmlBoundElement rowElement)
		{
			this.SynchronizeRowFromRowElement(rowElement, null);
		}

		private void SynchronizeRowFromRowElement(XmlBoundElement rowElement, ArrayList rowElemList)
		{
			DataRow row = rowElement.Row;
			if (row.RowState == DataRowState.Deleted)
			{
				return;
			}
			row.BeginEdit();
			this.SynchronizeRowFromRowElementEx(rowElement, rowElemList);
			row.EndEdit();
		}

		private void SynchronizeRowFromRowElementEx(XmlBoundElement rowElement, ArrayList rowElemList)
		{
			DataRow row = rowElement.Row;
			DataTable table = row.Table;
			Hashtable hashtable = new Hashtable();
			string text = string.Empty;
			RegionIterator regionIterator = new RegionIterator(rowElement);
			DataColumn textOnlyColumn = this.GetTextOnlyColumn(row);
			bool flag;
			if (textOnlyColumn != null)
			{
				hashtable[textOnlyColumn] = textOnlyColumn;
				string text2;
				flag = regionIterator.NextInitialTextLikeNodes(out text2);
				if (text2.Length == 0 && ((text = rowElement.GetAttribute("xsi:nil")) == "1" || text == "true"))
				{
					row[textOnlyColumn] = DBNull.Value;
				}
				else
				{
					XmlDataDocument.SetRowValueFromXmlText(row, textOnlyColumn, text2);
				}
			}
			else
			{
				flag = regionIterator.Next();
			}
			while (flag)
			{
				XmlElement xmlElement = regionIterator.CurrentNode as XmlElement;
				if (xmlElement == null)
				{
					flag = regionIterator.Next();
				}
				else
				{
					XmlBoundElement xmlBoundElement = xmlElement as XmlBoundElement;
					if (xmlBoundElement != null && xmlBoundElement.Row != null)
					{
						if (rowElemList != null)
						{
							rowElemList.Add(xmlElement);
						}
						flag = regionIterator.NextRight();
					}
					else
					{
						DataColumn columnSchemaForNode = this._mapper.GetColumnSchemaForNode(rowElement, xmlElement);
						if (columnSchemaForNode != null && hashtable[columnSchemaForNode] == null)
						{
							hashtable[columnSchemaForNode] = columnSchemaForNode;
							string text3;
							flag = regionIterator.NextInitialTextLikeNodes(out text3);
							if (text3.Length == 0 && ((text = xmlElement.GetAttribute("xsi:nil")) == "1" || text == "true"))
							{
								row[columnSchemaForNode] = DBNull.Value;
							}
							else
							{
								XmlDataDocument.SetRowValueFromXmlText(row, columnSchemaForNode, text3);
							}
						}
						else
						{
							flag = regionIterator.Next();
						}
					}
				}
			}
			foreach (object obj in rowElement.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				DataColumn columnSchemaForNode2 = this._mapper.GetColumnSchemaForNode(rowElement, xmlAttribute);
				if (columnSchemaForNode2 != null && hashtable[columnSchemaForNode2] == null)
				{
					hashtable[columnSchemaForNode2] = columnSchemaForNode2;
					XmlDataDocument.SetRowValueFromXmlText(row, columnSchemaForNode2, xmlAttribute.Value);
				}
			}
			foreach (object obj2 in row.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj2;
				if (hashtable[dataColumn] == null && !this.IsNotMapped(dataColumn))
				{
					if (!dataColumn.AutoIncrement)
					{
						XmlDataDocument.SetRowValueToNull(row, dataColumn);
					}
					else
					{
						dataColumn.Init(row._tempRecord);
					}
				}
			}
		}

		private void UpdateAllColumns(DataRow row, XmlBoundElement rowElement)
		{
			foreach (object obj in row.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				this.OnColumnValueChanged(row, dataColumn, rowElement);
			}
		}

		public XmlDataDocument()
			: base(new XmlDataImplementation())
		{
			this.Init();
			this.AttachDataSet(new DataSet());
			this._dataSet.EnforceConstraints = false;
		}

		public XmlDataDocument(DataSet dataset)
			: base(new XmlDataImplementation())
		{
			this.Init(dataset);
		}

		internal XmlDataDocument(XmlImplementation imp)
			: base(imp)
		{
		}

		private void Init()
		{
			this._pointers = new Hashtable();
			this._countAddPointer = 0;
			this._columnChangeList = new ArrayList();
			this._ignoreDataSetEvents = false;
			this._isFoliationEnabled = true;
			this._optimizeStorage = true;
			this._fDataRowCreatedSpecial = false;
			this._autoFoliationState = ElementState.StrongFoliation;
			this._fAssociateDataRow = true;
			this._mapper = new DataSetMapper();
			this._foliationLock = new object();
			this._ignoreXmlEvents = true;
			this._attrXml = this.CreateAttribute("xmlns", "xml", "http://www.w3.org/2000/xmlns/");
			this._attrXml.Value = "http://www.w3.org/XML/1998/namespace";
			this._ignoreXmlEvents = false;
		}

		private void Init(DataSet ds)
		{
			if (ds == null)
			{
				throw new ArgumentException("The DataSet parameter is invalid. It cannot be null.");
			}
			this.Init();
			if (ds.FBoundToDocument)
			{
				throw new ArgumentException("DataSet can be associated with at most one XmlDataDocument. Cannot associate the DataSet with the current XmlDataDocument because the DataSet is already associated with another XmlDataDocument.");
			}
			ds.FBoundToDocument = true;
			this._dataSet = ds;
			this.Bind(true);
		}

		private bool IsConnected(XmlNode node)
		{
			while (node != null)
			{
				if (node == this)
				{
					return true;
				}
				XmlAttribute xmlAttribute = node as XmlAttribute;
				if (xmlAttribute != null)
				{
					node = xmlAttribute.OwnerElement;
				}
				else
				{
					node = node.ParentNode;
				}
			}
			return false;
		}

		private bool IsRowLive(DataRow row)
		{
			return (row.RowState & (DataRowState.Unchanged | DataRowState.Added | DataRowState.Modified)) > (DataRowState)0;
		}

		private static void SetNestedParentRow(DataRow childRow, DataRow parentRow)
		{
			DataRelation nestedParentRelation = XmlDataDocument.GetNestedParentRelation(childRow);
			if (nestedParentRelation != null)
			{
				if (parentRow == null || nestedParentRelation.ParentKey.Table != parentRow.Table)
				{
					childRow.SetParentRow(null, nestedParentRelation);
					return;
				}
				childRow.SetParentRow(parentRow, nestedParentRelation);
			}
		}

		private void OnNodeInsertedInTree(XmlNode node)
		{
			ArrayList arrayList = new ArrayList();
			XmlBoundElement xmlBoundElement;
			if (this._mapper.GetRegion(node, out xmlBoundElement))
			{
				if (xmlBoundElement == node)
				{
					this.OnRowElementInsertedInTree(xmlBoundElement, arrayList);
				}
				else
				{
					this.OnNonRowElementInsertedInTree(node, xmlBoundElement, arrayList);
				}
			}
			else
			{
				TreeIterator treeIterator = new TreeIterator(node);
				bool flag = treeIterator.NextRowElement();
				while (flag)
				{
					arrayList.Add(treeIterator.CurrentNode);
					flag = treeIterator.NextRightRowElement();
				}
			}
			while (arrayList.Count > 0)
			{
				XmlBoundElement xmlBoundElement2 = (XmlBoundElement)arrayList[0];
				arrayList.RemoveAt(0);
				this.OnRowElementInsertedInTree(xmlBoundElement2, arrayList);
			}
		}

		private void OnNodeInsertedInFragment(XmlNode node)
		{
			XmlBoundElement xmlBoundElement;
			if (!this._mapper.GetRegion(node, out xmlBoundElement))
			{
				return;
			}
			if (xmlBoundElement == node)
			{
				this.SetNestedParentRegion(xmlBoundElement);
				return;
			}
			ArrayList arrayList = new ArrayList();
			this.OnNonRowElementInsertedInFragment(node, xmlBoundElement, arrayList);
			while (arrayList.Count > 0)
			{
				XmlBoundElement xmlBoundElement2 = (XmlBoundElement)arrayList[0];
				arrayList.RemoveAt(0);
				this.SetNestedParentRegion(xmlBoundElement2, xmlBoundElement);
			}
		}

		private void OnRowElementInsertedInTree(XmlBoundElement rowElem, ArrayList rowElemList)
		{
			DataRow row = rowElem.Row;
			DataRowState rowState = row.RowState;
			if (rowState != DataRowState.Detached)
			{
				if (rowState != DataRowState.Deleted)
				{
					return;
				}
				row.RejectChanges();
				this.SynchronizeRowFromRowElement(rowElem, rowElemList);
				this.SetNestedParentRegion(rowElem);
			}
			else
			{
				row.Table.Rows.Add(row);
				this.SetNestedParentRegion(rowElem);
				if (rowElemList != null)
				{
					RegionIterator regionIterator = new RegionIterator(rowElem);
					bool flag = regionIterator.NextRowElement();
					while (flag)
					{
						rowElemList.Add(regionIterator.CurrentNode);
						flag = regionIterator.NextRightRowElement();
					}
					return;
				}
			}
		}

		private void EnsureDisconnectedDataRow(XmlBoundElement rowElem)
		{
			DataRow row = rowElem.Row;
			DataRowState rowState = row.RowState;
			switch (rowState)
			{
			case DataRowState.Detached:
				this.SetNestedParentRegion(rowElem);
				return;
			case DataRowState.Unchanged:
				break;
			case DataRowState.Detached | DataRowState.Unchanged:
				return;
			case DataRowState.Added:
				this.EnsureFoliation(rowElem, ElementState.WeakFoliation);
				row.Delete();
				this.SetNestedParentRegion(rowElem);
				return;
			default:
				if (rowState == DataRowState.Deleted)
				{
					return;
				}
				if (rowState != DataRowState.Modified)
				{
					return;
				}
				break;
			}
			this.EnsureFoliation(rowElem, ElementState.WeakFoliation);
			row.Delete();
		}

		private void OnNonRowElementInsertedInTree(XmlNode node, XmlBoundElement rowElement, ArrayList rowElemList)
		{
			DataRow row = rowElement.Row;
			this.SynchronizeRowFromRowElement(rowElement);
			if (rowElemList != null)
			{
				TreeIterator treeIterator = new TreeIterator(node);
				bool flag = treeIterator.NextRowElement();
				while (flag)
				{
					rowElemList.Add(treeIterator.CurrentNode);
					flag = treeIterator.NextRightRowElement();
				}
			}
		}

		private void OnNonRowElementInsertedInFragment(XmlNode node, XmlBoundElement rowElement, ArrayList rowElemList)
		{
			if (rowElement.Row.RowState == DataRowState.Detached)
			{
				this.SynchronizeRowFromRowElementEx(rowElement, rowElemList);
			}
		}

		private void SetNestedParentRegion(XmlBoundElement childRowElem)
		{
			XmlBoundElement xmlBoundElement;
			this._mapper.GetRegion(childRowElem.ParentNode, out xmlBoundElement);
			this.SetNestedParentRegion(childRowElem, xmlBoundElement);
		}

		private void SetNestedParentRegion(XmlBoundElement childRowElem, XmlBoundElement parentRowElem)
		{
			DataRow row = childRowElem.Row;
			if (parentRowElem == null)
			{
				XmlDataDocument.SetNestedParentRow(row, null);
				return;
			}
			DataRow row2 = parentRowElem.Row;
			DataRelation[] nestedParentRelations = row.Table.NestedParentRelations;
			if (nestedParentRelations.Length != 0 && nestedParentRelations[0].ParentTable == row2.Table)
			{
				XmlDataDocument.SetNestedParentRow(row, row2);
				return;
			}
			XmlDataDocument.SetNestedParentRow(row, null);
		}

		internal static bool IsTextNode(XmlNodeType nt)
		{
			return nt - XmlNodeType.Text <= 1 || nt - XmlNodeType.Whitespace <= 1;
		}

		protected override XPathNavigator CreateNavigator(XmlNode node)
		{
			if (XPathNodePointer.s_xmlNodeType_To_XpathNodeType_Map[(int)node.NodeType] == -1)
			{
				return null;
			}
			if (XmlDataDocument.IsTextNode(node.NodeType))
			{
				XmlNode parentNode = node.ParentNode;
				if (parentNode != null && parentNode.NodeType == XmlNodeType.Attribute)
				{
					return null;
				}
				XmlNode xmlNode = node.PreviousSibling;
				while (xmlNode != null && XmlDataDocument.IsTextNode(xmlNode.NodeType))
				{
					node = xmlNode;
					xmlNode = this.SafePreviousSibling(node);
				}
			}
			return new DataDocumentXPathNavigator(this, node);
		}

		[Conditional("DEBUG")]
		private void AssertLiveRows(XmlNode node)
		{
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = false;
			try
			{
				XmlBoundElement xmlBoundElement = node as XmlBoundElement;
				if (xmlBoundElement != null)
				{
					DataRow row = xmlBoundElement.Row;
				}
				TreeIterator treeIterator = new TreeIterator(node);
				bool flag = treeIterator.NextRowElement();
				while (flag)
				{
					xmlBoundElement = treeIterator.CurrentNode as XmlBoundElement;
					flag = treeIterator.NextRowElement();
				}
			}
			finally
			{
				this.IsFoliationEnabled = isFoliationEnabled;
			}
		}

		[Conditional("DEBUG")]
		private void AssertNonLiveRows(XmlNode node)
		{
			bool isFoliationEnabled = this.IsFoliationEnabled;
			this.IsFoliationEnabled = false;
			try
			{
				XmlBoundElement xmlBoundElement = node as XmlBoundElement;
				if (xmlBoundElement != null)
				{
					DataRow row = xmlBoundElement.Row;
				}
				TreeIterator treeIterator = new TreeIterator(node);
				bool flag = treeIterator.NextRowElement();
				while (flag)
				{
					xmlBoundElement = treeIterator.CurrentNode as XmlBoundElement;
					flag = treeIterator.NextRowElement();
				}
			}
			finally
			{
				this.IsFoliationEnabled = isFoliationEnabled;
			}
		}

		public override XmlElement GetElementById(string elemId)
		{
			throw new NotSupportedException("GetElementById() is not supported on DataDocument.");
		}

		public override XmlNodeList GetElementsByTagName(string name)
		{
			XmlNodeList elementsByTagName = base.GetElementsByTagName(name);
			int count = elementsByTagName.Count;
			return elementsByTagName;
		}

		private DataTable[] OrderTables(DataSet ds)
		{
			DataTable[] array = null;
			if (ds == null || ds.Tables.Count == 0)
			{
				array = Array.Empty<DataTable>();
			}
			else if (this.TablesAreOrdered(ds))
			{
				array = new DataTable[ds.Tables.Count];
				ds.Tables.CopyTo(array, 0);
			}
			if (array == null)
			{
				array = new DataTable[ds.Tables.Count];
				List<DataTable> list = new List<DataTable>();
				foreach (object obj in ds.Tables)
				{
					DataTable dataTable = (DataTable)obj;
					if (dataTable.ParentRelations.Count == 0)
					{
						list.Add(dataTable);
					}
				}
				if (list.Count > 0)
				{
					foreach (object obj2 in ds.Tables)
					{
						DataTable dataTable2 = (DataTable)obj2;
						if (this.IsSelfRelatedDataTable(dataTable2))
						{
							list.Add(dataTable2);
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						foreach (object obj3 in list[i].ChildRelations)
						{
							DataTable childTable = ((DataRelation)obj3).ChildTable;
							if (!list.Contains(childTable))
							{
								list.Add(childTable);
							}
						}
					}
					list.CopyTo(array);
				}
				else
				{
					ds.Tables.CopyTo(array, 0);
				}
			}
			return array;
		}

		private bool IsSelfRelatedDataTable(DataTable rootTable)
		{
			List<DataTable> list = new List<DataTable>();
			bool flag = false;
			foreach (object obj in rootTable.ChildRelations)
			{
				DataTable childTable = ((DataRelation)obj).ChildTable;
				if (childTable == rootTable)
				{
					flag = true;
					break;
				}
				if (!list.Contains(childTable))
				{
					list.Add(childTable);
				}
			}
			if (!flag)
			{
				for (int i = 0; i < list.Count; i++)
				{
					foreach (object obj2 in list[i].ChildRelations)
					{
						DataTable childTable2 = ((DataRelation)obj2).ChildTable;
						if (childTable2 == rootTable)
						{
							flag = true;
							break;
						}
						if (!list.Contains(childTable2))
						{
							list.Add(childTable2);
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		private bool TablesAreOrdered(DataSet ds)
		{
			using (IEnumerator enumerator = ds.Tables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((DataTable)enumerator.Current).Namespace != ds.Namespace)
					{
						return false;
					}
				}
			}
			return true;
		}

		private DataSet _dataSet;

		private DataSetMapper _mapper;

		internal Hashtable _pointers;

		private int _countAddPointer;

		private ArrayList _columnChangeList;

		private DataRowState _rollbackState;

		private bool _fBoundToDataSet;

		private bool _fBoundToDocument;

		private bool _fDataRowCreatedSpecial;

		private bool _ignoreXmlEvents;

		private bool _ignoreDataSetEvents;

		private bool _isFoliationEnabled;

		private bool _optimizeStorage;

		private ElementState _autoFoliationState;

		private bool _fAssociateDataRow;

		private object _foliationLock;

		internal const string XSI_NIL = "xsi:nil";

		internal const string XSI = "xsi";

		private bool _bForceExpandEntity;

		internal XmlAttribute _attrXml;

		internal bool _bLoadFromDataSet;

		internal bool _bHasXSINIL;
	}
}
