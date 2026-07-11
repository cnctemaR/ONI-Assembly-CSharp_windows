using System;
using System.Data;
using System.Diagnostics;

namespace System.Xml
{
	internal sealed class DataPointer : IXmlDataVirtualNode
	{
		internal DataPointer(XmlDataDocument doc, XmlNode node)
		{
			this._doc = doc;
			this._node = node;
			this._column = null;
			this._fOnValue = false;
			this._bNeedFoliate = false;
			this._isInUse = true;
		}

		internal DataPointer(DataPointer pointer)
		{
			this._doc = pointer._doc;
			this._node = pointer._node;
			this._column = pointer._column;
			this._fOnValue = pointer._fOnValue;
			this._bNeedFoliate = false;
			this._isInUse = true;
		}

		internal void AddPointer()
		{
			this._doc.AddPointer(this);
		}

		private XmlBoundElement GetRowElement()
		{
			XmlBoundElement xmlBoundElement;
			if (this._column != null)
			{
				xmlBoundElement = this._node as XmlBoundElement;
				return xmlBoundElement;
			}
			this._doc.Mapper.GetRegion(this._node, out xmlBoundElement);
			return xmlBoundElement;
		}

		private DataRow Row
		{
			get
			{
				XmlBoundElement rowElement = this.GetRowElement();
				if (rowElement == null)
				{
					return null;
				}
				return rowElement.Row;
			}
		}

		private static bool IsFoliated(XmlNode node)
		{
			return node == null || !(node is XmlBoundElement) || ((XmlBoundElement)node).IsFoliated;
		}

		internal void MoveTo(DataPointer pointer)
		{
			this._doc = pointer._doc;
			this._node = pointer._node;
			this._column = pointer._column;
			this._fOnValue = pointer._fOnValue;
		}

		private void MoveTo(XmlNode node)
		{
			this._node = node;
			this._column = null;
			this._fOnValue = false;
		}

		private void MoveTo(XmlNode node, DataColumn column, bool fOnValue)
		{
			this._node = node;
			this._column = column;
			this._fOnValue = fOnValue;
		}

		private DataColumn NextColumn(DataRow row, DataColumn col, bool fAttribute, bool fNulls)
		{
			if (row.RowState == DataRowState.Deleted)
			{
				return null;
			}
			DataColumnCollection columns = row.Table.Columns;
			int i = ((col != null) ? (col.Ordinal + 1) : 0);
			int count = columns.Count;
			DataRowVersion dataRowVersion = ((row.RowState == DataRowState.Detached) ? DataRowVersion.Proposed : DataRowVersion.Current);
			while (i < count)
			{
				DataColumn dataColumn = columns[i];
				if (!this._doc.IsNotMapped(dataColumn) && dataColumn.ColumnMapping == MappingType.Attribute == fAttribute && (fNulls || !Convert.IsDBNull(row[dataColumn, dataRowVersion])))
				{
					return dataColumn;
				}
				i++;
			}
			return null;
		}

		private DataColumn NthColumn(DataRow row, bool fAttribute, int iColumn, bool fNulls)
		{
			DataColumn dataColumn = null;
			checked
			{
				while ((dataColumn = this.NextColumn(row, dataColumn, fAttribute, fNulls)) != null)
				{
					if (iColumn == 0)
					{
						return dataColumn;
					}
					iColumn--;
				}
				return null;
			}
		}

		private int ColumnCount(DataRow row, bool fAttribute, bool fNulls)
		{
			DataColumn dataColumn = null;
			int num = 0;
			while ((dataColumn = this.NextColumn(row, dataColumn, fAttribute, fNulls)) != null)
			{
				num++;
			}
			return num;
		}

		internal bool MoveToFirstChild()
		{
			this.RealFoliate();
			if (this._node == null)
			{
				return false;
			}
			if (this._column != null)
			{
				if (this._fOnValue)
				{
					return false;
				}
				this._fOnValue = true;
				return true;
			}
			else
			{
				if (!DataPointer.IsFoliated(this._node))
				{
					DataColumn dataColumn = this.NextColumn(this.Row, null, false, false);
					if (dataColumn != null)
					{
						this.MoveTo(this._node, dataColumn, this._doc.IsTextOnly(dataColumn));
						return true;
					}
				}
				XmlNode xmlNode = this._doc.SafeFirstChild(this._node);
				if (xmlNode != null)
				{
					this.MoveTo(xmlNode);
					return true;
				}
				return false;
			}
		}

		internal bool MoveToNextSibling()
		{
			this.RealFoliate();
			if (this._node != null)
			{
				if (this._column != null)
				{
					if (this._fOnValue && !this._doc.IsTextOnly(this._column))
					{
						return false;
					}
					DataColumn dataColumn = this.NextColumn(this.Row, this._column, false, false);
					if (dataColumn != null)
					{
						this.MoveTo(this._node, dataColumn, false);
						return true;
					}
					XmlNode xmlNode = this._doc.SafeFirstChild(this._node);
					if (xmlNode != null)
					{
						this.MoveTo(xmlNode);
						return true;
					}
				}
				else
				{
					XmlNode xmlNode2 = this._doc.SafeNextSibling(this._node);
					if (xmlNode2 != null)
					{
						this.MoveTo(xmlNode2);
						return true;
					}
				}
			}
			return false;
		}

		internal bool MoveToParent()
		{
			this.RealFoliate();
			if (this._node != null)
			{
				if (this._column != null)
				{
					if (this._fOnValue && !this._doc.IsTextOnly(this._column))
					{
						this.MoveTo(this._node, this._column, false);
						return true;
					}
					if (this._column.ColumnMapping != MappingType.Attribute)
					{
						this.MoveTo(this._node, null, false);
						return true;
					}
				}
				else
				{
					XmlNode parentNode = this._node.ParentNode;
					if (parentNode != null)
					{
						this.MoveTo(parentNode);
						return true;
					}
				}
			}
			return false;
		}

		internal bool MoveToOwnerElement()
		{
			this.RealFoliate();
			if (this._node != null)
			{
				if (this._column != null)
				{
					if (this._fOnValue || this._doc.IsTextOnly(this._column) || this._column.ColumnMapping != MappingType.Attribute)
					{
						return false;
					}
					this.MoveTo(this._node, null, false);
					return true;
				}
				else if (this._node.NodeType == XmlNodeType.Attribute)
				{
					XmlNode ownerElement = ((XmlAttribute)this._node).OwnerElement;
					if (ownerElement != null)
					{
						this.MoveTo(ownerElement, null, false);
						return true;
					}
				}
			}
			return false;
		}

		internal int AttributeCount
		{
			get
			{
				this.RealFoliate();
				if (this._node == null || this._column != null || this._node.NodeType != XmlNodeType.Element)
				{
					return 0;
				}
				if (!DataPointer.IsFoliated(this._node))
				{
					return this.ColumnCount(this.Row, true, false);
				}
				return this._node.Attributes.Count;
			}
		}

		internal bool MoveToAttribute(int i)
		{
			this.RealFoliate();
			if (i < 0)
			{
				return false;
			}
			if (this._node != null && (this._column == null || this._column.ColumnMapping == MappingType.Attribute) && this._node.NodeType == XmlNodeType.Element)
			{
				if (!DataPointer.IsFoliated(this._node))
				{
					DataColumn dataColumn = this.NthColumn(this.Row, true, i, false);
					if (dataColumn != null)
					{
						this.MoveTo(this._node, dataColumn, false);
						return true;
					}
				}
				else
				{
					XmlNode xmlNode = this._node.Attributes.Item(i);
					if (xmlNode != null)
					{
						this.MoveTo(xmlNode, null, false);
						return true;
					}
				}
			}
			return false;
		}

		internal XmlNodeType NodeType
		{
			get
			{
				this.RealFoliate();
				if (this._node == null)
				{
					return XmlNodeType.None;
				}
				if (this._column == null)
				{
					return this._node.NodeType;
				}
				if (this._fOnValue)
				{
					return XmlNodeType.Text;
				}
				if (this._column.ColumnMapping == MappingType.Attribute)
				{
					return XmlNodeType.Attribute;
				}
				return XmlNodeType.Element;
			}
		}

		internal string LocalName
		{
			get
			{
				this.RealFoliate();
				if (this._node == null)
				{
					return string.Empty;
				}
				if (this._column == null)
				{
					string localName = this._node.LocalName;
					if (this.IsLocalNameEmpty(this._node.NodeType))
					{
						return string.Empty;
					}
					return localName;
				}
				else
				{
					if (this._fOnValue)
					{
						return string.Empty;
					}
					return this._doc.NameTable.Add(this._column.EncodedColumnName);
				}
			}
		}

		internal string NamespaceURI
		{
			get
			{
				this.RealFoliate();
				if (this._node == null)
				{
					return string.Empty;
				}
				if (this._column == null)
				{
					return this._node.NamespaceURI;
				}
				if (this._fOnValue)
				{
					return string.Empty;
				}
				return this._doc.NameTable.Add(this._column.Namespace);
			}
		}

		internal string Name
		{
			get
			{
				this.RealFoliate();
				if (this._node == null)
				{
					return string.Empty;
				}
				if (this._column == null)
				{
					string name = this._node.Name;
					if (this.IsLocalNameEmpty(this._node.NodeType))
					{
						return string.Empty;
					}
					return name;
				}
				else
				{
					string prefix = this.Prefix;
					string localName = this.LocalName;
					if (prefix == null || prefix.Length <= 0)
					{
						return localName;
					}
					if (localName != null && localName.Length > 0)
					{
						return this._doc.NameTable.Add(prefix + ":" + localName);
					}
					return prefix;
				}
			}
		}

		private bool IsLocalNameEmpty(XmlNodeType nt)
		{
			switch (nt)
			{
			case XmlNodeType.None:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.Comment:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
			case XmlNodeType.EndElement:
			case XmlNodeType.EndEntity:
				return true;
			case XmlNodeType.Element:
			case XmlNodeType.Attribute:
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.DocumentType:
			case XmlNodeType.Notation:
			case XmlNodeType.XmlDeclaration:
				return false;
			default:
				return true;
			}
		}

		internal string Prefix
		{
			get
			{
				this.RealFoliate();
				if (this._node == null)
				{
					return string.Empty;
				}
				if (this._column == null)
				{
					return this._node.Prefix;
				}
				return string.Empty;
			}
		}

		internal string Value
		{
			get
			{
				this.RealFoliate();
				if (this._node == null)
				{
					return null;
				}
				if (this._column == null)
				{
					return this._node.Value;
				}
				if (this._column.ColumnMapping != MappingType.Attribute && !this._fOnValue)
				{
					return null;
				}
				DataRow row = this.Row;
				DataRowVersion dataRowVersion = ((row.RowState == DataRowState.Detached) ? DataRowVersion.Proposed : DataRowVersion.Current);
				object obj = row[this._column, dataRowVersion];
				if (!Convert.IsDBNull(obj))
				{
					return this._column.ConvertObjectToXml(obj);
				}
				return null;
			}
		}

		bool IXmlDataVirtualNode.IsOnNode(XmlNode nodeToCheck)
		{
			this.RealFoliate();
			return nodeToCheck == this._node;
		}

		bool IXmlDataVirtualNode.IsOnColumn(DataColumn col)
		{
			this.RealFoliate();
			return col == this._column;
		}

		internal XmlNode GetNode()
		{
			return this._node;
		}

		internal bool IsEmptyElement
		{
			get
			{
				this.RealFoliate();
				return this._node != null && this._column == null && this._node.NodeType == XmlNodeType.Element && ((XmlElement)this._node).IsEmpty;
			}
		}

		internal bool IsDefault
		{
			get
			{
				this.RealFoliate();
				return this._node != null && this._column == null && this._node.NodeType == XmlNodeType.Attribute && !((XmlAttribute)this._node).Specified;
			}
		}

		void IXmlDataVirtualNode.OnFoliated(XmlNode foliatedNode)
		{
			if (this._node == foliatedNode)
			{
				if (this._column == null)
				{
					return;
				}
				this._bNeedFoliate = true;
			}
		}

		internal void RealFoliate()
		{
			if (!this._bNeedFoliate)
			{
				return;
			}
			XmlNode xmlNode;
			if (this._doc.IsTextOnly(this._column))
			{
				xmlNode = this._node.FirstChild;
			}
			else
			{
				if (this._column.ColumnMapping == MappingType.Attribute)
				{
					xmlNode = this._node.Attributes.GetNamedItem(this._column.EncodedColumnName, this._column.Namespace);
				}
				else
				{
					xmlNode = this._node.FirstChild;
					while (xmlNode != null && (!(xmlNode.LocalName == this._column.EncodedColumnName) || !(xmlNode.NamespaceURI == this._column.Namespace)))
					{
						xmlNode = xmlNode.NextSibling;
					}
				}
				if (xmlNode != null && this._fOnValue)
				{
					xmlNode = xmlNode.FirstChild;
				}
			}
			if (xmlNode == null)
			{
				throw new InvalidOperationException("Invalid foliation.");
			}
			this._node = xmlNode;
			this._column = null;
			this._fOnValue = false;
			this._bNeedFoliate = false;
		}

		internal string PublicId
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Entity)
				{
					return ((XmlEntity)this._node).PublicId;
				}
				if (nodeType == XmlNodeType.DocumentType)
				{
					return ((XmlDocumentType)this._node).PublicId;
				}
				if (nodeType != XmlNodeType.Notation)
				{
					return null;
				}
				return ((XmlNotation)this._node).PublicId;
			}
		}

		internal string SystemId
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Entity)
				{
					return ((XmlEntity)this._node).SystemId;
				}
				if (nodeType == XmlNodeType.DocumentType)
				{
					return ((XmlDocumentType)this._node).SystemId;
				}
				if (nodeType != XmlNodeType.Notation)
				{
					return null;
				}
				return ((XmlNotation)this._node).SystemId;
			}
		}

		internal string InternalSubset
		{
			get
			{
				if (this.NodeType == XmlNodeType.DocumentType)
				{
					return ((XmlDocumentType)this._node).InternalSubset;
				}
				return null;
			}
		}

		internal XmlDeclaration Declaration
		{
			get
			{
				XmlNode xmlNode = this._doc.SafeFirstChild(this._doc);
				if (xmlNode != null && xmlNode.NodeType == XmlNodeType.XmlDeclaration)
				{
					return (XmlDeclaration)xmlNode;
				}
				return null;
			}
		}

		internal string Encoding
		{
			get
			{
				if (this.NodeType == XmlNodeType.XmlDeclaration)
				{
					return ((XmlDeclaration)this._node).Encoding;
				}
				if (this.NodeType == XmlNodeType.Document)
				{
					XmlDeclaration declaration = this.Declaration;
					if (declaration != null)
					{
						return declaration.Encoding;
					}
				}
				return null;
			}
		}

		internal string Standalone
		{
			get
			{
				if (this.NodeType == XmlNodeType.XmlDeclaration)
				{
					return ((XmlDeclaration)this._node).Standalone;
				}
				if (this.NodeType == XmlNodeType.Document)
				{
					XmlDeclaration declaration = this.Declaration;
					if (declaration != null)
					{
						return declaration.Standalone;
					}
				}
				return null;
			}
		}

		internal string Version
		{
			get
			{
				if (this.NodeType == XmlNodeType.XmlDeclaration)
				{
					return ((XmlDeclaration)this._node).Version;
				}
				if (this.NodeType == XmlNodeType.Document)
				{
					XmlDeclaration declaration = this.Declaration;
					if (declaration != null)
					{
						return declaration.Version;
					}
				}
				return null;
			}
		}

		[Conditional("DEBUG")]
		private void AssertValid()
		{
			if (this._column != null)
			{
				XmlBoundElement xmlBoundElement = this._node as XmlBoundElement;
				DataRow row = xmlBoundElement.Row;
				ElementState elementState = xmlBoundElement.ElementState;
				DataRowState rowState = row.RowState;
			}
		}

		bool IXmlDataVirtualNode.IsInUse()
		{
			return this._isInUse;
		}

		internal void SetNoLongerUse()
		{
			this._node = null;
			this._column = null;
			this._fOnValue = false;
			this._bNeedFoliate = false;
			this._isInUse = false;
		}

		private XmlDataDocument _doc;

		private XmlNode _node;

		private DataColumn _column;

		private bool _fOnValue;

		private bool _bNeedFoliate;

		private bool _isInUse;
	}
}
