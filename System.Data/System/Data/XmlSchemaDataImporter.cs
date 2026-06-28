using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace System.Data
{
	internal class XmlSchemaDataImporter
	{
		public XmlSchemaDataImporter(DataSet dataset, XmlReader reader, bool forDataSet)
		{
			this.dataset = dataset;
			this.forDataSet = forDataSet;
			dataset.DataSetName = "NewDataSet";
			this.schema = XmlSchema.Read(reader, null);
			if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				reader.ReadEndElement();
			}
			this.schema.Compile(null);
		}

		static XmlSchemaDataImporter()
		{
			XmlSchema xmlSchema = new XmlSchema();
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = "foo";
			xmlSchemaAttribute.SchemaTypeName = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");
			xmlSchema.Items.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
			xmlSchemaAttribute2.Name = "bar";
			xmlSchemaAttribute2.SchemaTypeName = new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
			xmlSchema.Items.Add(xmlSchemaAttribute2);
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = "bar";
			xmlSchema.Items.Add(xmlSchemaElement);
			xmlSchema.Compile(null);
			XmlSchemaDataImporter.schemaIntegerType = xmlSchemaAttribute.AttributeSchemaType.Datatype;
			XmlSchemaDataImporter.schemaDecimalType = xmlSchemaAttribute2.AttributeSchemaType.Datatype;
			XmlSchemaDataImporter.schemaAnyType = xmlSchemaElement.ElementSchemaType as XmlSchemaComplexType;
		}

		internal TableAdapterSchemaInfo CurrentAdapter
		{
			get
			{
				return this.currentAdapter;
			}
		}

		public void Process()
		{
			if (this.schema.Id != null)
			{
				this.dataset.DataSetName = this.schema.Id;
			}
			this.dataset.Namespace = this.schema.TargetNamespace;
			foreach (XmlSchemaObject xmlSchemaObject in this.schema.Items)
			{
				XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
				if (xmlSchemaElement != null)
				{
					if (this.datasetElement == null && this.IsDataSetElement(xmlSchemaElement))
					{
						this.datasetElement = xmlSchemaElement;
					}
					if (xmlSchemaElement.ElementSchemaType is XmlSchemaComplexType && xmlSchemaElement.ElementSchemaType != XmlSchemaDataImporter.schemaAnyType)
					{
						this.targetElements.Add(xmlSchemaObject);
					}
				}
			}
			if (this.datasetElement != null)
			{
				foreach (XmlSchemaObject xmlSchemaObject2 in this.datasetElement.Constraints)
				{
					if (!(xmlSchemaObject2 is XmlSchemaKeyref))
					{
						this.ReserveSelfIdentity((XmlSchemaIdentityConstraint)xmlSchemaObject2);
					}
				}
				foreach (XmlSchemaObject xmlSchemaObject3 in this.datasetElement.Constraints)
				{
					if (xmlSchemaObject3 is XmlSchemaKeyref)
					{
						this.ReserveRelationIdentity(this.datasetElement, (XmlSchemaKeyref)xmlSchemaObject3);
					}
				}
			}
			foreach (XmlSchemaObject xmlSchemaObject4 in this.schema.Items)
			{
				if (xmlSchemaObject4 is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement2 = xmlSchemaObject4 as XmlSchemaElement;
					if (xmlSchemaElement2.ElementSchemaType is XmlSchemaComplexType && xmlSchemaElement2.ElementSchemaType != XmlSchemaDataImporter.schemaAnyType)
					{
						this.targetElements.Add(xmlSchemaObject4);
					}
				}
			}
			int count = this.targetElements.Count;
			for (int i = 0; i < count; i++)
			{
				this.ProcessGlobalElement((XmlSchemaElement)this.targetElements[i]);
			}
			for (int j = count; j < this.targetElements.Count; j++)
			{
				this.ProcessDataTableElement((XmlSchemaElement)this.targetElements[j]);
			}
			foreach (XmlSchemaObject xmlSchemaObject5 in this.schema.Items)
			{
				if (xmlSchemaObject5 is XmlSchemaAnnotation)
				{
					this.HandleAnnotations((XmlSchemaAnnotation)xmlSchemaObject5, false);
				}
			}
			if (this.datasetElement != null)
			{
				foreach (XmlSchemaObject xmlSchemaObject6 in this.datasetElement.Constraints)
				{
					if (!(xmlSchemaObject6 is XmlSchemaKeyref))
					{
						this.ProcessSelfIdentity(this.reservedConstraints[xmlSchemaObject6] as ConstraintStructure);
					}
				}
				foreach (XmlSchemaObject xmlSchemaObject7 in this.datasetElement.Constraints)
				{
					if (xmlSchemaObject7 is XmlSchemaKeyref)
					{
						this.ProcessRelationIdentity(this.datasetElement, this.reservedConstraints[xmlSchemaObject7] as ConstraintStructure);
					}
				}
			}
			foreach (object obj in this.relations)
			{
				RelationStructure relationStructure = (RelationStructure)obj;
				this.dataset.Relations.Add(this.GenerateRelationship(relationStructure));
			}
		}

		private bool IsDataSetElement(XmlSchemaElement el)
		{
			if (el.UnhandledAttributes != null)
			{
				foreach (XmlAttribute xmlAttribute in el.UnhandledAttributes)
				{
					if (xmlAttribute.LocalName == "IsDataSet" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						string value = xmlAttribute.Value;
						if (value != null)
						{
							if (XmlSchemaDataImporter.<>f__switch$mapB == null)
							{
								XmlSchemaDataImporter.<>f__switch$mapB = new Dictionary<string, int>(2)
								{
									{ "true", 0 },
									{ "false", 1 }
								};
							}
							int num;
							if (XmlSchemaDataImporter.<>f__switch$mapB.TryGetValue(value, out num))
							{
								if (num == 0)
								{
									return true;
								}
								if (num == 1)
								{
									goto IL_00CD;
								}
							}
						}
						throw new DataException(string.Format("Value {0} is invalid for attribute 'IsDataSet'.", xmlAttribute.Value));
					}
					IL_00CD:;
				}
			}
			if (this.schema.Elements.Count != 1)
			{
				return false;
			}
			if (!(el.SchemaType is XmlSchemaComplexType))
			{
				return false;
			}
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)el.SchemaType;
			if (xmlSchemaComplexType.AttributeUses.Count > 0)
			{
				return false;
			}
			XmlSchemaGroupBase xmlSchemaGroupBase = xmlSchemaComplexType.ContentTypeParticle as XmlSchemaGroupBase;
			if (xmlSchemaGroupBase == null || xmlSchemaGroupBase.Items.Count == 0)
			{
				return false;
			}
			foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaGroupBase.Items)
			{
				XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)xmlSchemaObject;
				if (this.ContainsColumn(xmlSchemaParticle))
				{
					return false;
				}
			}
			return true;
		}

		private bool ContainsColumn(XmlSchemaParticle p)
		{
			XmlSchemaElement xmlSchemaElement = p as XmlSchemaElement;
			if (xmlSchemaElement != null)
			{
				XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaElement.ElementSchemaType as XmlSchemaComplexType;
				return xmlSchemaComplexType == null || xmlSchemaComplexType == XmlSchemaDataImporter.schemaAnyType || (xmlSchemaComplexType.AttributeUses.Count <= 0 && xmlSchemaComplexType.ContentType == XmlSchemaContentType.TextOnly);
			}
			XmlSchemaGroupBase xmlSchemaGroupBase = p as XmlSchemaGroupBase;
			for (int i = 0; i < xmlSchemaGroupBase.Items.Count; i++)
			{
				if (this.ContainsColumn((XmlSchemaParticle)xmlSchemaGroupBase.Items[i]))
				{
					return true;
				}
			}
			return false;
		}

		private void ProcessGlobalElement(XmlSchemaElement el)
		{
			if (this.dataset.Tables.Contains(el.QualifiedName.Name))
			{
				return;
			}
			if (!(el.ElementSchemaType is XmlSchemaComplexType) || el.ElementSchemaType == XmlSchemaDataImporter.schemaAnyType)
			{
				return;
			}
			if (this.IsDataSetElement(el))
			{
				this.ProcessDataSetElement(el);
				return;
			}
			this.dataset.Locale = CultureInfo.CurrentCulture;
			this.topLevelElements.Add(el);
			this.ProcessDataTableElement(el);
		}

		private void ProcessDataSetElement(XmlSchemaElement el)
		{
			this.dataset.DataSetName = el.Name;
			this.datasetElement = el;
			bool flag = false;
			if (el.UnhandledAttributes != null)
			{
				foreach (XmlAttribute xmlAttribute in el.UnhandledAttributes)
				{
					if (xmlAttribute.LocalName == "UseCurrentLocale" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						flag = true;
					}
					if (xmlAttribute.LocalName == "Locale" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						CultureInfo cultureInfo = new CultureInfo(xmlAttribute.Value);
						this.dataset.Locale = cultureInfo;
					}
				}
			}
			if (!flag && !this.dataset.LocaleSpecified)
			{
				this.dataset.Locale = CultureInfo.CurrentCulture;
			}
			XmlSchemaComplexType xmlSchemaComplexType = el.ElementSchemaType as XmlSchemaComplexType;
			XmlSchemaParticle xmlSchemaParticle = ((xmlSchemaComplexType == null) ? null : xmlSchemaComplexType.ContentTypeParticle);
			if (xmlSchemaParticle != null)
			{
				this.HandleDataSetContentTypeParticle(xmlSchemaParticle);
			}
		}

		private void HandleDataSetContentTypeParticle(XmlSchemaParticle p)
		{
			XmlSchemaElement xmlSchemaElement = p as XmlSchemaElement;
			if (xmlSchemaElement != null)
			{
				if (xmlSchemaElement.ElementSchemaType is XmlSchemaComplexType && xmlSchemaElement.RefName != xmlSchemaElement.QualifiedName)
				{
					this.ProcessDataTableElement(xmlSchemaElement);
				}
			}
			else if (p is XmlSchemaGroupBase)
			{
				foreach (XmlSchemaObject xmlSchemaObject in ((XmlSchemaGroupBase)p).Items)
				{
					XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)xmlSchemaObject;
					this.HandleDataSetContentTypeParticle(xmlSchemaParticle);
				}
			}
		}

		private void ProcessDataTableElement(XmlSchemaElement el)
		{
			string text = XmlHelper.Decode(el.QualifiedName.Name);
			if (this.dataset.Tables.Contains(text))
			{
				return;
			}
			DataTable dataTable = new DataTable(text);
			dataTable.Namespace = el.QualifiedName.Namespace;
			TableStructure tableStructure = this.currentTable;
			this.currentTable = new TableStructure(dataTable);
			this.dataset.Tables.Add(dataTable);
			if (el.UnhandledAttributes != null)
			{
				foreach (XmlAttribute xmlAttribute in el.UnhandledAttributes)
				{
					if (xmlAttribute.LocalName == "Locale" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						dataTable.Locale = new CultureInfo(xmlAttribute.Value);
					}
				}
			}
			XmlSchemaComplexType xmlSchemaComplexType = null;
			xmlSchemaComplexType = (XmlSchemaComplexType)el.ElementSchemaType;
			foreach (object obj in xmlSchemaComplexType.AttributeUses)
			{
				this.ImportColumnAttribute((XmlSchemaAttribute)((DictionaryEntry)obj).Value);
			}
			if (xmlSchemaComplexType.ContentTypeParticle is XmlSchemaElement)
			{
				this.ImportColumnElement(el, (XmlSchemaElement)xmlSchemaComplexType.ContentTypeParticle);
			}
			else if (xmlSchemaComplexType.ContentTypeParticle is XmlSchemaGroupBase)
			{
				this.ImportColumnGroupBase(el, (XmlSchemaGroupBase)xmlSchemaComplexType.ContentTypeParticle);
			}
			XmlSchemaContentType contentType = xmlSchemaComplexType.ContentType;
			if (contentType == XmlSchemaContentType.TextOnly)
			{
				string text2 = el.QualifiedName.Name + "_text";
				DataColumn dataColumn = new DataColumn(text2);
				dataColumn.Namespace = el.QualifiedName.Namespace;
				dataColumn.AllowDBNull = el.MinOccurs == 0m;
				dataColumn.ColumnMapping = MappingType.SimpleContent;
				dataColumn.DataType = this.ConvertDatatype(xmlSchemaComplexType.Datatype);
				this.currentTable.NonOrdinalColumns.Add(dataColumn);
			}
			SortedList sortedList = new SortedList();
			foreach (object obj2 in this.currentTable.OrdinalColumns)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
				sortedList.Add(dictionaryEntry.Value, dictionaryEntry.Key);
			}
			foreach (object obj3 in sortedList)
			{
				DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj3;
				dataTable.Columns.Add((DataColumn)dictionaryEntry2.Value);
			}
			foreach (object obj4 in this.currentTable.NonOrdinalColumns)
			{
				DataColumn dataColumn2 = (DataColumn)obj4;
				dataTable.Columns.Add(dataColumn2);
			}
			this.currentTable = tableStructure;
		}

		private DataRelation GenerateRelationship(RelationStructure rs)
		{
			DataTable dataTable = this.dataset.Tables[rs.ParentTableName];
			DataTable dataTable2 = this.dataset.Tables[rs.ChildTableName];
			string text = ((rs.ExplicitName == null) ? (XmlHelper.Decode(dataTable.TableName) + '_' + XmlHelper.Decode(dataTable2.TableName)) : rs.ExplicitName);
			DataRelation dataRelation;
			if (this.datasetElement != null)
			{
				string[] array = rs.ParentColumnName.Split(null);
				string[] array2 = rs.ChildColumnName.Split(null);
				DataColumn[] array3 = new DataColumn[array.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i] = dataTable.Columns[XmlHelper.Decode(array[i])];
				}
				DataColumn[] array4 = new DataColumn[array2.Length];
				for (int j = 0; j < array4.Length; j++)
				{
					array4[j] = dataTable2.Columns[XmlHelper.Decode(array2[j])];
					if (array4[j] == null)
					{
						array4[j] = this.CreateChildColumn(array3[j], dataTable2);
					}
				}
				dataRelation = new DataRelation(text, array3, array4, rs.CreateConstraint);
			}
			else
			{
				DataColumn dataColumn = dataTable.Columns[XmlHelper.Decode(rs.ParentColumnName)];
				DataColumn dataColumn2 = dataTable2.Columns[XmlHelper.Decode(rs.ChildColumnName)];
				if (dataColumn2 == null)
				{
					dataColumn2 = this.CreateChildColumn(dataColumn, dataTable2);
				}
				dataRelation = new DataRelation(text, dataColumn, dataColumn2, rs.CreateConstraint);
			}
			dataRelation.Nested = rs.IsNested;
			if (rs.CreateConstraint)
			{
				dataRelation.ParentTable.PrimaryKey = dataRelation.ParentColumns;
			}
			return dataRelation;
		}

		private DataColumn CreateChildColumn(DataColumn parentColumn, DataTable childTable)
		{
			DataColumn dataColumn = childTable.Columns.Add(parentColumn.ColumnName, parentColumn.DataType);
			dataColumn.Namespace = string.Empty;
			dataColumn.ColumnMapping = MappingType.Hidden;
			return dataColumn;
		}

		private void ImportColumnGroupBase(XmlSchemaElement parent, XmlSchemaGroupBase gb)
		{
			foreach (XmlSchemaObject xmlSchemaObject in gb.Items)
			{
				XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)xmlSchemaObject;
				XmlSchemaElement xmlSchemaElement = xmlSchemaParticle as XmlSchemaElement;
				if (xmlSchemaElement != null)
				{
					this.ImportColumnElement(parent, xmlSchemaElement);
				}
				else if (xmlSchemaParticle is XmlSchemaGroupBase)
				{
					this.ImportColumnGroupBase(parent, (XmlSchemaGroupBase)xmlSchemaParticle);
				}
			}
		}

		private XmlSchemaDatatype GetSchemaPrimitiveType(object type)
		{
			if (type is XmlSchemaComplexType)
			{
				return null;
			}
			XmlSchemaDatatype xmlSchemaDatatype = type as XmlSchemaDatatype;
			if (xmlSchemaDatatype == null && type != null)
			{
				xmlSchemaDatatype = ((XmlSchemaSimpleType)type).Datatype;
			}
			return xmlSchemaDatatype;
		}

		private void ImportColumnAttribute(XmlSchemaAttribute attr)
		{
			DataColumn dataColumn = new DataColumn();
			dataColumn.ColumnName = attr.QualifiedName.Name;
			dataColumn.Namespace = attr.QualifiedName.Namespace;
			XmlSchemaDatatype schemaPrimitiveType = this.GetSchemaPrimitiveType(attr.AttributeSchemaType.Datatype);
			dataColumn.DataType = this.ConvertDatatype(schemaPrimitiveType);
			if (dataColumn.DataType == typeof(object))
			{
				dataColumn.DataType = typeof(string);
			}
			if (attr.Use == XmlSchemaUse.Prohibited)
			{
				dataColumn.ColumnMapping = MappingType.Hidden;
			}
			else
			{
				dataColumn.ColumnMapping = MappingType.Attribute;
				dataColumn.DefaultValue = this.GetAttributeDefaultValue(attr);
			}
			if (attr.Use == XmlSchemaUse.Required)
			{
				dataColumn.AllowDBNull = false;
			}
			this.FillFacet(dataColumn, attr.AttributeSchemaType);
			this.ImportColumnMetaInfo(attr, attr.QualifiedName, dataColumn);
			this.AddColumn(dataColumn);
		}

		private void ImportColumnElement(XmlSchemaElement parent, XmlSchemaElement el)
		{
			DataColumn dataColumn = new DataColumn();
			dataColumn.DefaultValue = this.GetElementDefaultValue(el);
			dataColumn.AllowDBNull = el.MinOccurs == 0m;
			if (el.ElementSchemaType is XmlSchemaComplexType && el.ElementSchemaType != XmlSchemaDataImporter.schemaAnyType)
			{
				this.FillDataColumnComplexElement(parent, el, dataColumn);
			}
			else if (el.MaxOccurs != 1m)
			{
				this.FillDataColumnRepeatedSimpleElement(parent, el, dataColumn);
			}
			else
			{
				this.FillDataColumnSimpleElement(el, dataColumn);
			}
		}

		private void ImportColumnMetaInfo(XmlSchemaAnnotated obj, XmlQualifiedName name, DataColumn col)
		{
			if (obj.UnhandledAttributes != null)
			{
				foreach (XmlAttribute xmlAttribute in obj.UnhandledAttributes)
				{
					if (!(xmlAttribute.NamespaceURI != "urn:schemas-microsoft-com:xml-msdata"))
					{
						string localName = xmlAttribute.LocalName;
						switch (localName)
						{
						case "Caption":
							col.Caption = xmlAttribute.Value;
							break;
						case "DataType":
							col.DataType = Type.GetType(xmlAttribute.Value);
							break;
						case "AutoIncrement":
							col.AutoIncrement = bool.Parse(xmlAttribute.Value);
							break;
						case "AutoIncrementSeed":
							col.AutoIncrementSeed = (long)int.Parse(xmlAttribute.Value);
							break;
						case "AutoIncrementStep":
							col.AutoIncrementStep = (long)int.Parse(xmlAttribute.Value);
							break;
						case "ReadOnly":
							col.ReadOnly = XmlConvert.ToBoolean(xmlAttribute.Value);
							break;
						case "Ordinal":
						{
							int num2 = int.Parse(xmlAttribute.Value);
							break;
						}
						}
					}
				}
			}
		}

		private void FillDataColumnComplexElement(XmlSchemaElement parent, XmlSchemaElement el, DataColumn col)
		{
			if (this.targetElements.Contains(el))
			{
				return;
			}
			string text = XmlHelper.Decode(el.QualifiedName.Name);
			if (text == this.dataset.DataSetName)
			{
				throw new ArgumentException("Nested element must not have the same name as DataSet's name.");
			}
			if (el.Annotation != null)
			{
				this.HandleAnnotations(el.Annotation, true);
			}
			else if (!this.DataSetDefinesKey(text))
			{
				this.AddParentKeyColumn(parent, el, col);
				RelationStructure relationStructure = new RelationStructure();
				relationStructure.ParentTableName = XmlHelper.Decode(parent.QualifiedName.Name);
				relationStructure.ChildTableName = text;
				relationStructure.ParentColumnName = col.ColumnName;
				relationStructure.ChildColumnName = col.ColumnName;
				relationStructure.CreateConstraint = true;
				relationStructure.IsNested = true;
				this.relations.Add(relationStructure);
			}
			if (el.RefName == XmlQualifiedName.Empty)
			{
				this.ProcessDataTableElement(el);
			}
		}

		private bool DataSetDefinesKey(string name)
		{
			foreach (object obj in this.reservedConstraints.Values)
			{
				ConstraintStructure constraintStructure = (ConstraintStructure)obj;
				if (constraintStructure.TableName == name && (constraintStructure.IsPrimaryKey || constraintStructure.IsNested))
				{
					return true;
				}
			}
			return false;
		}

		private void AddParentKeyColumn(XmlSchemaElement parent, XmlSchemaElement el, DataColumn col)
		{
			if (this.currentTable.Table.PrimaryKey.Length > 0)
			{
				throw new DataException(string.Format("There is already primary key columns in the table \"{0}\".", this.currentTable.Table.TableName));
			}
			if (this.currentTable.PrimaryKey != null)
			{
				col.ColumnName = this.currentTable.PrimaryKey.ColumnName;
				col.ColumnMapping = this.currentTable.PrimaryKey.ColumnMapping;
				col.Namespace = this.currentTable.PrimaryKey.Namespace;
				col.DataType = this.currentTable.PrimaryKey.DataType;
				col.AutoIncrement = this.currentTable.PrimaryKey.AutoIncrement;
				col.AllowDBNull = this.currentTable.PrimaryKey.AllowDBNull;
				this.ImportColumnMetaInfo(el, el.QualifiedName, col);
				return;
			}
			string text = XmlHelper.Decode(parent.QualifiedName.Name) + "_Id";
			int num = 0;
			while (this.currentTable.ContainsColumn(text))
			{
				text = string.Format("{0}_{1}", text, num++);
			}
			col.ColumnName = text;
			col.ColumnMapping = MappingType.Hidden;
			col.Namespace = parent.QualifiedName.Namespace;
			col.DataType = typeof(int);
			col.AutoIncrement = true;
			col.AllowDBNull = false;
			this.ImportColumnMetaInfo(el, el.QualifiedName, col);
			this.AddColumn(col);
			this.currentTable.PrimaryKey = col;
		}

		private void FillDataColumnRepeatedSimpleElement(XmlSchemaElement parent, XmlSchemaElement el, DataColumn col)
		{
			if (this.targetElements.Contains(el))
			{
				return;
			}
			this.AddParentKeyColumn(parent, el, col);
			DataColumn primaryKey = this.currentTable.PrimaryKey;
			string text = XmlHelper.Decode(el.QualifiedName.Name);
			string text2 = XmlHelper.Decode(parent.QualifiedName.Name);
			DataTable dataTable = new DataTable();
			dataTable.TableName = text;
			dataTable.Namespace = el.QualifiedName.Namespace;
			DataColumn dataColumn = new DataColumn();
			dataColumn.ColumnName = text2 + "_Id";
			dataColumn.Namespace = parent.QualifiedName.Namespace;
			dataColumn.ColumnMapping = MappingType.Hidden;
			dataColumn.DataType = typeof(int);
			DataColumn dataColumn2 = new DataColumn();
			dataColumn2.ColumnName = text + "_Column";
			dataColumn2.Namespace = el.QualifiedName.Namespace;
			dataColumn2.ColumnMapping = MappingType.SimpleContent;
			dataColumn2.AllowDBNull = false;
			dataColumn2.DataType = this.ConvertDatatype(this.GetSchemaPrimitiveType(el.ElementSchemaType));
			dataTable.Columns.Add(dataColumn2);
			dataTable.Columns.Add(dataColumn);
			this.dataset.Tables.Add(dataTable);
			RelationStructure relationStructure = new RelationStructure();
			relationStructure.ParentTableName = text2;
			relationStructure.ChildTableName = dataTable.TableName;
			relationStructure.ParentColumnName = primaryKey.ColumnName;
			relationStructure.ChildColumnName = dataColumn.ColumnName;
			relationStructure.IsNested = true;
			relationStructure.CreateConstraint = true;
			this.relations.Add(relationStructure);
		}

		private void FillDataColumnSimpleElement(XmlSchemaElement el, DataColumn col)
		{
			col.ColumnName = XmlHelper.Decode(el.QualifiedName.Name);
			col.Namespace = el.QualifiedName.Namespace;
			col.ColumnMapping = MappingType.Element;
			col.DataType = this.ConvertDatatype(this.GetSchemaPrimitiveType(el.ElementSchemaType));
			this.FillFacet(col, el.ElementSchemaType as XmlSchemaSimpleType);
			this.ImportColumnMetaInfo(el, el.QualifiedName, col);
			this.AddColumn(col);
		}

		private void AddColumn(DataColumn col)
		{
			if (col.Ordinal < 0)
			{
				this.currentTable.NonOrdinalColumns.Add(col);
			}
			else
			{
				this.currentTable.OrdinalColumns.Add(col, col.Ordinal);
			}
		}

		private void FillFacet(DataColumn col, XmlSchemaSimpleType st)
		{
			if (st == null || st.Content == null)
			{
				return;
			}
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = ((st != null) ? (st.Content as XmlSchemaSimpleTypeRestriction) : null);
			if (xmlSchemaSimpleTypeRestriction == null)
			{
				throw new DataException("DataSet does not suport 'list' nor 'union' simple type.");
			}
			foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSimpleTypeRestriction.Facets)
			{
				XmlSchemaFacet xmlSchemaFacet = (XmlSchemaFacet)xmlSchemaObject;
				if (xmlSchemaFacet is XmlSchemaMaxLengthFacet)
				{
					col.MaxLength = int.Parse(xmlSchemaFacet.Value);
				}
			}
		}

		private Type ConvertDatatype(XmlSchemaDatatype dt)
		{
			if (dt == null)
			{
				return typeof(string);
			}
			if (dt.ValueType != typeof(decimal))
			{
				return dt.ValueType;
			}
			if (dt == XmlSchemaDataImporter.schemaDecimalType)
			{
				return typeof(decimal);
			}
			if (dt == XmlSchemaDataImporter.schemaIntegerType)
			{
				return typeof(long);
			}
			return typeof(ulong);
		}

		private string GetSelectorTarget(string xpath)
		{
			string text = xpath;
			int num = text.LastIndexOf('/');
			if (num > 0)
			{
				text = text.Substring(num + 1);
			}
			num = text.LastIndexOf(':');
			if (num > 0)
			{
				text = text.Substring(num + 1);
			}
			return XmlHelper.Decode(text);
		}

		private void ReserveSelfIdentity(XmlSchemaIdentityConstraint ic)
		{
			string selectorTarget = this.GetSelectorTarget(ic.Selector.XPath);
			string[] array = new string[ic.Fields.Count];
			bool[] array2 = new bool[array.Length];
			int num = 0;
			foreach (XmlSchemaObject xmlSchemaObject in ic.Fields)
			{
				XmlSchemaXPath xmlSchemaXPath = (XmlSchemaXPath)xmlSchemaObject;
				string text = xmlSchemaXPath.XPath;
				bool flag = text.Length > 0 && text[0] == '@';
				int num2 = text.LastIndexOf(':');
				if (num2 > 0)
				{
					text = text.Substring(num2 + 1);
				}
				else if (flag)
				{
					text = text.Substring(1);
				}
				text = XmlHelper.Decode(text);
				array[num] = text;
				array2[num] = flag;
				num++;
			}
			bool flag2 = false;
			string text2 = ic.Name;
			if (ic.UnhandledAttributes != null)
			{
				foreach (XmlAttribute xmlAttribute in ic.UnhandledAttributes)
				{
					if (!(xmlAttribute.NamespaceURI != "urn:schemas-microsoft-com:xml-msdata"))
					{
						string localName = xmlAttribute.LocalName;
						if (localName != null)
						{
							if (XmlSchemaDataImporter.<>f__switch$mapD == null)
							{
								XmlSchemaDataImporter.<>f__switch$mapD = new Dictionary<string, int>(2)
								{
									{ "ConstraintName", 0 },
									{ "PrimaryKey", 1 }
								};
							}
							int num3;
							if (XmlSchemaDataImporter.<>f__switch$mapD.TryGetValue(localName, out num3))
							{
								if (num3 != 0)
								{
									if (num3 == 1)
									{
										flag2 = bool.Parse(xmlAttribute.Value);
									}
								}
								else
								{
									text2 = xmlAttribute.Value;
								}
							}
						}
					}
				}
			}
			this.reservedConstraints.Add(ic, new ConstraintStructure(selectorTarget, array, array2, text2, flag2, null, false, false));
		}

		private void ProcessSelfIdentity(ConstraintStructure c)
		{
			string tableName = c.TableName;
			DataTable dataTable = this.dataset.Tables[tableName];
			if (dataTable != null)
			{
				DataColumn[] array = new DataColumn[c.Columns.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text = c.Columns[i];
					bool flag = c.IsAttribute[i];
					DataColumn dataColumn = dataTable.Columns[text];
					if (dataColumn == null)
					{
						throw new DataException(string.Format("Invalid XPath selection inside field. Cannot find: {0}", tableName));
					}
					if (flag && dataColumn.ColumnMapping != MappingType.Attribute)
					{
						throw new DataException("The XPath specified attribute field, but mapping type is not attribute.");
					}
					if (!flag && dataColumn.ColumnMapping != MappingType.Element)
					{
						throw new DataException("The XPath specified simple element field, but mapping type is not simple element.");
					}
					array[i] = dataTable.Columns[text];
				}
				bool isPrimaryKey = c.IsPrimaryKey;
				string constraintName = c.ConstraintName;
				dataTable.Constraints.Add(new UniqueConstraint(constraintName, array, isPrimaryKey));
				return;
			}
			if (this.forDataSet)
			{
				throw new DataException(string.Format("Invalid XPath selection inside selector. Cannot find: {0}", tableName));
			}
		}

		private void ReserveRelationIdentity(XmlSchemaElement element, XmlSchemaKeyref keyref)
		{
			string selectorTarget = this.GetSelectorTarget(keyref.Selector.XPath);
			string[] array = new string[keyref.Fields.Count];
			bool[] array2 = new bool[array.Length];
			int num = 0;
			foreach (XmlSchemaObject xmlSchemaObject in keyref.Fields)
			{
				XmlSchemaXPath xmlSchemaXPath = (XmlSchemaXPath)xmlSchemaObject;
				string text = xmlSchemaXPath.XPath;
				bool flag = text.Length > 0 && text[0] == '@';
				int num2 = text.LastIndexOf(':');
				if (num2 > 0)
				{
					text = text.Substring(num2 + 1);
				}
				else if (flag)
				{
					text = text.Substring(1);
				}
				text = XmlHelper.Decode(text);
				array[num] = text;
				array2[num] = flag;
				num++;
			}
			string text2 = keyref.Name;
			bool flag2 = false;
			bool flag3 = false;
			if (keyref.UnhandledAttributes != null)
			{
				foreach (XmlAttribute xmlAttribute in keyref.UnhandledAttributes)
				{
					if (!(xmlAttribute.NamespaceURI != "urn:schemas-microsoft-com:xml-msdata"))
					{
						string localName = xmlAttribute.LocalName;
						switch (localName)
						{
						case "ConstraintName":
							text2 = xmlAttribute.Value;
							break;
						case "IsNested":
							if (xmlAttribute.Value == "true")
							{
								flag2 = true;
							}
							break;
						case "ConstraintOnly":
							if (xmlAttribute.Value == "true")
							{
								flag3 = true;
							}
							break;
						}
					}
				}
			}
			this.reservedConstraints.Add(keyref, new ConstraintStructure(selectorTarget, array, array2, text2, false, keyref.Refer.Name, flag2, flag3));
		}

		private void ProcessRelationIdentity(XmlSchemaElement element, ConstraintStructure c)
		{
			string tableName = c.TableName;
			DataTable dataTable = this.dataset.Tables[tableName];
			if (dataTable == null)
			{
				throw new DataException(string.Format("Invalid XPath selection inside selector. Cannot find: {0}", tableName));
			}
			DataColumn[] array = new DataColumn[c.Columns.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = c.Columns[i];
				bool flag = c.IsAttribute[i];
				DataColumn dataColumn = dataTable.Columns[text];
				if (flag && dataColumn.ColumnMapping != MappingType.Attribute)
				{
					throw new DataException("The XPath specified attribute field, but mapping type is not attribute.");
				}
				if (!flag && dataColumn.ColumnMapping != MappingType.Element)
				{
					throw new DataException("The XPath specified simple element field, but mapping type is not simple element.");
				}
				array[i] = dataColumn;
			}
			string referName = c.ReferName;
			UniqueConstraint uniqueConstraint = this.FindConstraint(referName, element);
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(c.ConstraintName, uniqueConstraint.Columns, array);
			dataTable.Constraints.Add(foreignKeyConstraint);
			if (!c.IsConstraintOnly)
			{
				DataRelation dataRelation = new DataRelation(c.ConstraintName, uniqueConstraint.Columns, array, true);
				dataRelation.Nested = c.IsNested;
				dataRelation.SetParentKeyConstraint(uniqueConstraint);
				dataRelation.SetChildKeyConstraint(foreignKeyConstraint);
				this.dataset.Relations.Add(dataRelation);
			}
		}

		private UniqueConstraint FindConstraint(string name, XmlSchemaElement element)
		{
			foreach (XmlSchemaObject xmlSchemaObject in element.Constraints)
			{
				XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = (XmlSchemaIdentityConstraint)xmlSchemaObject;
				if (!(xmlSchemaIdentityConstraint is XmlSchemaKeyref))
				{
					if (xmlSchemaIdentityConstraint.Name == name)
					{
						string selectorTarget = this.GetSelectorTarget(xmlSchemaIdentityConstraint.Selector.XPath);
						DataTable dataTable = this.dataset.Tables[selectorTarget];
						string text = xmlSchemaIdentityConstraint.Name;
						if (xmlSchemaIdentityConstraint.UnhandledAttributes != null)
						{
							foreach (XmlAttribute xmlAttribute in xmlSchemaIdentityConstraint.UnhandledAttributes)
							{
								if (xmlAttribute.LocalName == "ConstraintName" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
								{
									text = xmlAttribute.Value;
								}
							}
						}
						return (UniqueConstraint)dataTable.Constraints[text];
					}
				}
			}
			throw new DataException("Target identity constraint was not found: " + name);
		}

		private void HandleAnnotations(XmlSchemaAnnotation an, bool nested)
		{
			foreach (XmlSchemaObject xmlSchemaObject in an.Items)
			{
				XmlSchemaAppInfo xmlSchemaAppInfo = xmlSchemaObject as XmlSchemaAppInfo;
				if (xmlSchemaAppInfo != null)
				{
					foreach (XmlNode xmlNode in xmlSchemaAppInfo.Markup)
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						if (xmlElement != null && xmlElement.LocalName == "Relationship" && xmlElement.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
						{
							this.HandleRelationshipAnnotation(xmlElement, nested);
						}
						if (xmlElement != null && xmlElement.LocalName == "DataSource" && xmlElement.NamespaceURI == "urn:schemas-microsoft-com:xml-msdatasource")
						{
							this.HandleDataSourceAnnotation(xmlElement, nested);
						}
					}
				}
			}
		}

		private void HandleDataSourceAnnotation(XmlElement el, bool nested)
		{
			string text = null;
			DbProviderFactory dbProviderFactory = null;
			XmlElement xmlElement = null;
			foreach (object obj in el.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement2 = xmlNode as XmlElement;
				if (xmlElement2 != null)
				{
					XmlElement xmlElement3;
					if (xmlElement2.LocalName == "Connections" && (xmlElement3 = xmlElement2.FirstChild as XmlElement) != null)
					{
						string attribute = xmlElement3.GetAttribute("Provider");
						text = xmlElement3.GetAttribute("AppSettingsPropertyName");
						dbProviderFactory = DbProviderFactories.GetFactory(attribute);
					}
					else if (xmlElement2.LocalName == "Tables")
					{
						xmlElement = xmlElement2;
					}
				}
			}
			if (xmlElement != null && dbProviderFactory != null)
			{
				foreach (object obj2 in xmlElement.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj2;
					this.ProcessTableAdapter(xmlNode2 as XmlElement, dbProviderFactory, text);
				}
			}
		}

		private void ProcessTableAdapter(XmlElement el, DbProviderFactory provider, string connStr)
		{
			string text = null;
			if (el == null)
			{
				return;
			}
			this.currentAdapter = new TableAdapterSchemaInfo(provider);
			this.currentAdapter.ConnectionString = connStr;
			this.currentAdapter.BaseClass = el.GetAttribute("BaseClass");
			text = el.GetAttribute("Name");
			this.currentAdapter.Name = el.GetAttribute("GeneratorDataComponentClassName");
			if (string.IsNullOrEmpty(this.currentAdapter.Name))
			{
				this.currentAdapter.Name = el.GetAttribute("DataAccessorName");
			}
			foreach (object obj in el.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					string localName = xmlElement.LocalName;
					if (localName != null)
					{
						if (XmlSchemaDataImporter.<>f__switch$mapF == null)
						{
							XmlSchemaDataImporter.<>f__switch$mapF = new Dictionary<string, int>(3)
							{
								{ "MainSource", 0 },
								{ "Sources", 0 },
								{ "Mappings", 1 }
							};
						}
						int num;
						if (XmlSchemaDataImporter.<>f__switch$mapF.TryGetValue(localName, out num))
						{
							if (num != 0)
							{
								if (num == 1)
								{
									DataTableMapping dataTableMapping = new DataTableMapping();
									dataTableMapping.SourceTable = "Table";
									dataTableMapping.DataSetTable = text;
									foreach (object obj2 in xmlElement.ChildNodes)
									{
										XmlNode xmlNode2 = (XmlNode)obj2;
										this.ProcessColumnMapping(xmlNode2 as XmlElement, dataTableMapping);
									}
									this.currentAdapter.Adapter.TableMappings.Add(dataTableMapping);
								}
							}
							else
							{
								foreach (object obj3 in xmlElement.ChildNodes)
								{
									XmlNode xmlNode3 = (XmlNode)obj3;
									this.ProcessDbSource(xmlNode3 as XmlElement);
								}
							}
						}
					}
				}
			}
		}

		private void ProcessDbSource(XmlElement el)
		{
			if (el == null)
			{
				return;
			}
			string text = el.GetAttribute("GenerateShortCommands");
			if (!string.IsNullOrEmpty(text))
			{
				this.currentAdapter.ShortCommands = Convert.ToBoolean(text);
			}
			DbCommandInfo dbCommandInfo = new DbCommandInfo();
			text = el.GetAttribute("GenerateMethods");
			if (!string.IsNullOrEmpty(text))
			{
				switch ((int)Enum.Parse(typeof(GenerateMethodsType), text))
				{
				case 1:
				{
					DbSourceMethodInfo dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("GetMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("GetMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = el.GetAttribute("ScalarCallRetval");
					dbSourceMethodInfo.QueryType = el.GetAttribute("QueryType");
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Get;
					dbCommandInfo.Methods = new DbSourceMethodInfo[1];
					dbCommandInfo.Methods[0] = dbSourceMethodInfo;
					break;
				}
				case 2:
				{
					DbSourceMethodInfo dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("FillMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("FillMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = null;
					dbSourceMethodInfo.QueryType = null;
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Fill;
					dbCommandInfo.Methods = new DbSourceMethodInfo[1];
					dbCommandInfo.Methods[0] = dbSourceMethodInfo;
					break;
				}
				case 3:
				{
					DbSourceMethodInfo dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("GetMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("GetMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = el.GetAttribute("ScalarCallRetval");
					dbSourceMethodInfo.QueryType = el.GetAttribute("QueryType");
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Get;
					dbCommandInfo.Methods = new DbSourceMethodInfo[2];
					dbCommandInfo.Methods[0] = dbSourceMethodInfo;
					dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("FillMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("FillMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = null;
					dbSourceMethodInfo.QueryType = null;
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Fill;
					dbCommandInfo.Methods[1] = dbSourceMethodInfo;
					break;
				}
				}
			}
			else
			{
				DbSourceMethodInfo dbSourceMethodInfo2 = new DbSourceMethodInfo();
				dbSourceMethodInfo2.Name = el.GetAttribute("Name");
				dbSourceMethodInfo2.Modifier = el.GetAttribute("Modifier");
				if (string.IsNullOrEmpty(dbSourceMethodInfo2.Modifier))
				{
					dbSourceMethodInfo2.Modifier = "Public";
				}
				dbSourceMethodInfo2.ScalarCallRetval = el.GetAttribute("ScalarCallRetval");
				dbSourceMethodInfo2.QueryType = el.GetAttribute("QueryType");
				dbSourceMethodInfo2.MethodType = GenerateMethodsType.None;
				dbCommandInfo.Methods = new DbSourceMethodInfo[1];
				dbCommandInfo.Methods[0] = dbSourceMethodInfo2;
			}
			foreach (object obj in el.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					string localName = xmlElement.LocalName;
					switch (localName)
					{
					case "SelectCommand":
						dbCommandInfo.Command = this.ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						this.currentAdapter.Commands.Add(dbCommandInfo);
						break;
					case "InsertCommand":
						this.currentAdapter.Adapter.InsertCommand = this.ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						break;
					case "UpdateCommand":
						this.currentAdapter.Adapter.UpdateCommand = this.ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						break;
					case "DeleteCommand":
						this.currentAdapter.Adapter.DeleteCommand = this.ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						break;
					}
				}
			}
		}

		private DbCommand ProcessDbCommand(XmlElement el)
		{
			string text = null;
			string text2 = null;
			ArrayList arrayList = null;
			if (el == null)
			{
				return null;
			}
			text2 = el.GetAttribute("CommandType");
			foreach (object obj in el.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null && xmlElement.LocalName == "CommandText")
				{
					text = xmlElement.InnerText;
				}
				else if (xmlElement != null && xmlElement.LocalName == "Parameters" && !xmlElement.IsEmpty)
				{
					arrayList = this.ProcessDbParameters(xmlElement);
				}
			}
			DbCommand dbCommand = this.currentAdapter.Provider.CreateCommand();
			dbCommand.CommandText = text;
			if (text2 == "StoredProcedure")
			{
				dbCommand.CommandType = CommandType.StoredProcedure;
			}
			else
			{
				dbCommand.CommandType = CommandType.Text;
			}
			if (arrayList != null)
			{
				dbCommand.Parameters.AddRange(arrayList.ToArray());
			}
			return dbCommand;
		}

		private ArrayList ProcessDbParameters(XmlElement el)
		{
			ArrayList arrayList = new ArrayList();
			if (el == null)
			{
				return arrayList;
			}
			foreach (object obj in el.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					DbParameter dbParameter = this.currentAdapter.Provider.CreateParameter();
					string text = xmlElement.GetAttribute("AllowDbNull");
					if (!string.IsNullOrEmpty(text))
					{
						dbParameter.IsNullable = Convert.ToBoolean(text);
					}
					dbParameter.ParameterName = xmlElement.GetAttribute("ParameterName");
					text = xmlElement.GetAttribute("ProviderType");
					if (!string.IsNullOrEmpty(text))
					{
						text = xmlElement.GetAttribute("DbType");
					}
					dbParameter.FrameworkDbType = text;
					text = xmlElement.GetAttribute("Direction");
					dbParameter.Direction = (ParameterDirection)((int)Enum.Parse(typeof(ParameterDirection), text));
					((IDbDataParameter)dbParameter).Precision = Convert.ToByte(xmlElement.GetAttribute("Precision"));
					((IDbDataParameter)dbParameter).Scale = Convert.ToByte(xmlElement.GetAttribute("Scale"));
					dbParameter.Size = Convert.ToInt32(xmlElement.GetAttribute("Size"));
					dbParameter.SourceColumn = xmlElement.GetAttribute("SourceColumn");
					text = xmlElement.GetAttribute("SourceColumnNullMapping");
					if (!string.IsNullOrEmpty(text))
					{
						dbParameter.SourceColumnNullMapping = Convert.ToBoolean(text);
					}
					text = xmlElement.GetAttribute("SourceVersion");
					dbParameter.SourceVersion = (DataRowVersion)((int)Enum.Parse(typeof(DataRowVersion), text));
					arrayList.Add(dbParameter);
				}
			}
			return arrayList;
		}

		private void ProcessColumnMapping(XmlElement el, DataTableMapping tableMapping)
		{
			if (el == null)
			{
				return;
			}
			tableMapping.ColumnMappings.Add(el.GetAttribute("SourceColumn"), el.GetAttribute("DataSetColumn"));
		}

		private void HandleRelationshipAnnotation(XmlElement el, bool nested)
		{
			string attribute = el.GetAttribute("name");
			string attribute2 = el.GetAttribute("parent", "urn:schemas-microsoft-com:xml-msdata");
			string attribute3 = el.GetAttribute("child", "urn:schemas-microsoft-com:xml-msdata");
			string attribute4 = el.GetAttribute("parentkey", "urn:schemas-microsoft-com:xml-msdata");
			string attribute5 = el.GetAttribute("childkey", "urn:schemas-microsoft-com:xml-msdata");
			RelationStructure relationStructure = new RelationStructure();
			relationStructure.ExplicitName = XmlHelper.Decode(attribute);
			relationStructure.ParentTableName = XmlHelper.Decode(attribute2);
			relationStructure.ChildTableName = XmlHelper.Decode(attribute3);
			relationStructure.ParentColumnName = attribute4;
			relationStructure.ChildColumnName = attribute5;
			relationStructure.IsNested = nested;
			relationStructure.CreateConstraint = false;
			this.relations.Add(relationStructure);
		}

		private object GetElementDefaultValue(XmlSchemaElement elem)
		{
			if (elem.RefName == XmlQualifiedName.Empty)
			{
				return elem.DefaultValue;
			}
			XmlSchemaElement xmlSchemaElement = this.schema.Elements[elem.RefName] as XmlSchemaElement;
			if (xmlSchemaElement == null)
			{
				return null;
			}
			return xmlSchemaElement.DefaultValue;
		}

		private object GetAttributeDefaultValue(XmlSchemaAttribute attr)
		{
			if (attr.DefaultValue != null)
			{
				return attr.DefaultValue;
			}
			if (attr.FixedValue != null)
			{
				return attr.FixedValue;
			}
			if (attr.RefName == XmlQualifiedName.Empty)
			{
				return null;
			}
			XmlSchemaAttribute xmlSchemaAttribute = this.schema.Attributes[attr.RefName] as XmlSchemaAttribute;
			if (xmlSchemaAttribute == null)
			{
				return null;
			}
			if (xmlSchemaAttribute.DefaultValue != null)
			{
				return xmlSchemaAttribute.DefaultValue;
			}
			return xmlSchemaAttribute.FixedValue;
		}

		private static readonly XmlSchemaDatatype schemaIntegerType;

		private static readonly XmlSchemaDatatype schemaDecimalType;

		private static readonly XmlSchemaComplexType schemaAnyType;

		private DataSet dataset;

		private bool forDataSet;

		private XmlSchema schema;

		private ArrayList relations = new ArrayList();

		private Hashtable reservedConstraints = new Hashtable();

		private XmlSchemaElement datasetElement;

		private ArrayList topLevelElements = new ArrayList();

		private ArrayList targetElements = new ArrayList();

		private TableStructure currentTable;

		private TableAdapterSchemaInfo currentAdapter;
	}
}
