using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	internal class XmlSchemaWriter
	{
		public XmlSchemaWriter(DataSet dataset, XmlWriter writer, DataTableCollection tables, DataRelationCollection relations)
		{
			this.dataSetName = dataset.DataSetName;
			this.dataSetNamespace = dataset.Namespace;
			this.dataSetLocale = ((!dataset.LocaleSpecified) ? null : dataset.Locale);
			this.dataSetProperties = dataset.ExtendedProperties;
			this.w = writer;
			if (tables != null)
			{
				this.tables = new DataTable[tables.Count];
				for (int i = 0; i < tables.Count; i++)
				{
					this.tables[i] = tables[i];
				}
			}
			if (relations != null)
			{
				this.relations = new DataRelation[relations.Count];
				for (int j = 0; j < relations.Count; j++)
				{
					this.relations[j] = relations[j];
				}
			}
		}

		public XmlSchemaWriter(XmlWriter writer, DataTable[] tables, DataRelation[] relations, string mainDataTable, string dataSetName, CultureInfo locale)
		{
			this.w = writer;
			this.tables = tables;
			this.relations = relations;
			this.mainDataTable = mainDataTable;
			this.dataSetName = dataSetName;
			this.dataSetLocale = locale;
			this.dataSetProperties = new PropertyCollection();
			if (tables[0].DataSet != null)
			{
				this.dataSetNamespace = tables[0].DataSet.Namespace;
			}
			else
			{
				this.dataSetNamespace = tables[0].Namespace;
			}
		}

		public static void WriteXmlSchema(DataSet dataset, XmlWriter writer)
		{
			XmlSchemaWriter.WriteXmlSchema(dataset, writer, dataset.Tables, dataset.Relations);
		}

		public static void WriteXmlSchema(DataSet dataset, XmlWriter writer, DataTableCollection tables, DataRelationCollection relations)
		{
			new XmlSchemaWriter(dataset, writer, tables, relations).WriteSchema();
		}

		internal static void WriteXmlSchema(XmlWriter writer, DataTable[] tables, DataRelation[] relations, string mainDataTable, string dataSetName, CultureInfo locale)
		{
			new XmlSchemaWriter(writer, tables, relations, mainDataTable, dataSetName, locale).WriteSchema();
		}

		public string ConstraintPrefix
		{
			get
			{
				return (!(this.dataSetNamespace != string.Empty)) ? string.Empty : ("mstns" + ':');
			}
		}

		public void WriteSchema()
		{
			ListDictionary listDictionary = new ListDictionary();
			ListDictionary listDictionary2 = new ListDictionary();
			foreach (DataTable dataTable in this.tables)
			{
				foreach (object obj in dataTable.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					this.CheckNamespace(dataColumn.Prefix, dataColumn.Namespace, listDictionary, listDictionary2);
				}
				this.CheckNamespace(dataTable.Prefix, dataTable.Namespace, listDictionary, listDictionary2);
			}
			this.w.WriteStartElement("xs", "schema", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("id", XmlHelper.Encode(this.dataSetName));
			if (this.dataSetNamespace != string.Empty)
			{
				this.w.WriteAttributeString("targetNamespace", this.dataSetNamespace);
				this.w.WriteAttributeString("xmlns", "mstns", "http://www.w3.org/2000/xmlns/", this.dataSetNamespace);
			}
			this.w.WriteAttributeString("xmlns", this.dataSetNamespace);
			this.w.WriteAttributeString("xmlns", "xs", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("xmlns", "msdata", "http://www.w3.org/2000/xmlns/", "urn:schemas-microsoft-com:xml-msdata");
			if (this.CheckExtendedPropertyExists(this.tables, this.relations))
			{
				this.w.WriteAttributeString("xmlns", "msprop", "http://www.w3.org/2000/xmlns/", "urn:schemas-microsoft-com:xml-msprop");
			}
			if (this.dataSetNamespace != string.Empty)
			{
				this.w.WriteAttributeString("attributeFormDefault", "qualified");
				this.w.WriteAttributeString("elementFormDefault", "qualified");
			}
			foreach (object obj2 in listDictionary.Keys)
			{
				string text = (string)obj2;
				this.w.WriteAttributeString("xmlns", text, "http://www.w3.org/2000/xmlns/", listDictionary[text] as string);
			}
			if (listDictionary2.Count > 0)
			{
				this.w.WriteComment("ATTENTION: This schema contains references to other imported schemas");
			}
			foreach (object obj3 in listDictionary2.Keys)
			{
				string text2 = (string)obj3;
				this.w.WriteStartElement("xs", "import", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteAttributeString("namespace", text2);
				this.w.WriteAttributeString("schemaLocation", listDictionary2[text2] as string);
				this.w.WriteEndElement();
			}
			foreach (DataTable dataTable2 in this.tables)
			{
				bool flag = true;
				foreach (object obj4 in dataTable2.ParentRelations)
				{
					DataRelation dataRelation = (DataRelation)obj4;
					if (dataRelation.Nested)
					{
						flag = false;
						break;
					}
				}
				if (!flag && this.tables.Length < 2)
				{
					this.WriteTableElement(dataTable2);
				}
			}
			this.WriteDataSetElement();
			this.w.WriteEndElement();
			this.w.Flush();
		}

		private void WriteDataSetElement()
		{
			this.w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("name", XmlHelper.Encode(this.dataSetName));
			this.w.WriteAttributeString("msdata", "IsDataSet", "urn:schemas-microsoft-com:xml-msdata", "true");
			bool flag = this.dataSetLocale == null;
			if (!flag)
			{
				this.w.WriteAttributeString("msdata", "Locale", "urn:schemas-microsoft-com:xml-msdata", this.dataSetLocale.Name);
			}
			if (this.mainDataTable != null && this.mainDataTable != string.Empty)
			{
				this.w.WriteAttributeString("msdata", "MainDataTable", "urn:schemas-microsoft-com:xml-msdata", this.mainDataTable);
			}
			if (flag)
			{
				this.w.WriteAttributeString("msdata", "UseCurrentLocale", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			this.AddExtendedPropertyAttributes(this.dataSetProperties);
			this.w.WriteStartElement("xs", "complexType", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteStartElement("xs", "choice", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("minOccurs", "0");
			this.w.WriteAttributeString("maxOccurs", "unbounded");
			foreach (DataTable dataTable in this.tables)
			{
				bool flag2 = true;
				foreach (object obj in dataTable.ParentRelations)
				{
					DataRelation dataRelation = (DataRelation)obj;
					if (dataRelation.Nested)
					{
						flag2 = false;
						break;
					}
				}
				bool flag3 = false;
				if (!flag2 && this.tables.Length < 2)
				{
					flag2 = true;
					flag3 = true;
				}
				if (flag2)
				{
					if (this.dataSetNamespace != dataTable.Namespace || flag3)
					{
						this.w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
						this.w.WriteStartAttribute("ref", string.Empty);
						this.w.WriteQualifiedName(XmlHelper.Encode(dataTable.TableName), dataTable.Namespace);
						this.w.WriteEndAttribute();
						this.w.WriteEndElement();
					}
					else
					{
						this.WriteTableElement(dataTable);
					}
				}
			}
			this.w.WriteEndElement();
			this.w.WriteEndElement();
			this.WriteConstraints();
			this.w.WriteEndElement();
			if (this.annotation.Count > 0)
			{
				this.w.WriteStartElement("xs", "annotation", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteStartElement("xs", "appinfo", "http://www.w3.org/2001/XMLSchema");
				foreach (object obj2 in this.annotation)
				{
					if (obj2 is DataRelation)
					{
						this.WriteDataRelationAnnotation((DataRelation)obj2);
					}
				}
				this.w.WriteEndElement();
				this.w.WriteEndElement();
			}
		}

		private void WriteDataRelationAnnotation(DataRelation rel)
		{
			string text = string.Empty;
			this.w.WriteStartElement("msdata", "Relationship", "urn:schemas-microsoft-com:xml-msdata");
			this.w.WriteAttributeString("name", XmlHelper.Encode(rel.RelationName));
			this.w.WriteAttributeString("msdata", "parent", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(rel.ParentTable.TableName));
			this.w.WriteAttributeString("msdata", "child", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(rel.ChildTable.TableName));
			text = string.Empty;
			foreach (DataColumn dataColumn in rel.ParentColumns)
			{
				text = text + XmlHelper.Encode(dataColumn.ColumnName) + " ";
			}
			this.w.WriteAttributeString("msdata", "parentkey", "urn:schemas-microsoft-com:xml-msdata", text.TrimEnd(new char[0]));
			text = string.Empty;
			foreach (DataColumn dataColumn2 in rel.ChildColumns)
			{
				text = text + XmlHelper.Encode(dataColumn2.ColumnName) + " ";
			}
			this.w.WriteAttributeString("msdata", "childkey", "urn:schemas-microsoft-com:xml-msdata", text.TrimEnd(new char[0]));
			this.w.WriteEndElement();
		}

		private void WriteConstraints()
		{
			ArrayList arrayList = new ArrayList();
			foreach (DataTable dataTable in this.tables)
			{
				foreach (object obj in dataTable.Constraints)
				{
					Constraint constraint = (Constraint)obj;
					UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
					if (uniqueConstraint != null)
					{
						this.AddUniqueConstraints(uniqueConstraint, arrayList);
					}
					else
					{
						ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
						bool flag = false;
						if (this.relations != null)
						{
							foreach (DataRelation dataRelation in this.relations)
							{
								if (dataRelation.RelationName == foreignKeyConstraint.ConstraintName)
								{
									flag = true;
								}
							}
						}
						if (this.tables.Length > 1 && foreignKeyConstraint != null && !flag)
						{
							DataRelation dataRelation2 = new DataRelation(foreignKeyConstraint.ConstraintName, foreignKeyConstraint.RelatedColumns, foreignKeyConstraint.Columns);
							this.AddForeignKeys(dataRelation2, arrayList, true);
						}
					}
				}
			}
			if (this.relations != null)
			{
				foreach (DataRelation dataRelation3 in this.relations)
				{
					if (dataRelation3.ParentKeyConstraint == null || dataRelation3.ChildKeyConstraint == null)
					{
						this.annotation.Add(dataRelation3);
					}
					else
					{
						this.AddForeignKeys(dataRelation3, arrayList, false);
					}
				}
			}
		}

		private void AddUniqueConstraints(UniqueConstraint uniq, ArrayList names)
		{
			foreach (DataColumn dataColumn in uniq.Columns)
			{
				if (dataColumn.ColumnMapping == MappingType.Hidden)
				{
					return;
				}
			}
			this.w.WriteStartElement("xs", "unique", "http://www.w3.org/2001/XMLSchema");
			string text;
			if (!names.Contains(uniq.ConstraintName))
			{
				text = XmlHelper.Encode(uniq.ConstraintName);
				this.w.WriteAttributeString("name", text);
			}
			else
			{
				text = XmlHelper.Encode(uniq.Table.TableName) + "_" + XmlHelper.Encode(uniq.ConstraintName);
				this.w.WriteAttributeString("name", text);
				this.w.WriteAttributeString("msdata", "ConstraintName", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(uniq.ConstraintName));
			}
			names.Add(text);
			if (uniq.IsPrimaryKey)
			{
				this.w.WriteAttributeString("msdata", "PrimaryKey", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			this.AddExtendedPropertyAttributes(uniq.ExtendedProperties);
			this.w.WriteStartElement("xs", "selector", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("xpath", ".//" + this.ConstraintPrefix + XmlHelper.Encode(uniq.Table.TableName));
			this.w.WriteEndElement();
			foreach (DataColumn dataColumn2 in uniq.Columns)
			{
				this.w.WriteStartElement("xs", "field", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteStartAttribute("xpath", string.Empty);
				if (dataColumn2.ColumnMapping == MappingType.Attribute)
				{
					this.w.WriteString("@");
				}
				this.w.WriteString(this.ConstraintPrefix);
				this.w.WriteString(XmlHelper.Encode(dataColumn2.ColumnName));
				this.w.WriteEndAttribute();
				this.w.WriteEndElement();
			}
			this.w.WriteEndElement();
		}

		private void AddForeignKeys(DataRelation rel, ArrayList names, bool isConstraintOnly)
		{
			foreach (DataColumn dataColumn in rel.ParentColumns)
			{
				if (dataColumn.ColumnMapping == MappingType.Hidden)
				{
					return;
				}
			}
			foreach (DataColumn dataColumn2 in rel.ChildColumns)
			{
				if (dataColumn2.ColumnMapping == MappingType.Hidden)
				{
					return;
				}
			}
			this.w.WriteStartElement("xs", "keyref", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("name", XmlHelper.Encode(rel.RelationName));
			UniqueConstraint uniqueConstraint = null;
			if (isConstraintOnly)
			{
				foreach (object obj in rel.ParentTable.Constraints)
				{
					Constraint constraint = (Constraint)obj;
					uniqueConstraint = constraint as UniqueConstraint;
					if (uniqueConstraint != null)
					{
						if (uniqueConstraint.Columns == rel.ParentColumns)
						{
							break;
						}
					}
				}
			}
			else
			{
				uniqueConstraint = rel.ParentKeyConstraint;
			}
			string text = XmlHelper.Encode(rel.ParentTable.TableName) + "_" + XmlHelper.Encode(uniqueConstraint.ConstraintName);
			if (names.Contains(text))
			{
				this.w.WriteStartAttribute("refer", string.Empty);
				this.w.WriteQualifiedName(text, this.dataSetNamespace);
				this.w.WriteEndAttribute();
			}
			else
			{
				this.w.WriteStartAttribute("refer", string.Empty);
				this.w.WriteQualifiedName(XmlHelper.Encode(uniqueConstraint.ConstraintName), this.dataSetNamespace);
				this.w.WriteEndAttribute();
			}
			if (isConstraintOnly)
			{
				this.w.WriteAttributeString("msdata", "ConstraintOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			else if (rel.Nested)
			{
				this.w.WriteAttributeString("msdata", "IsNested", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			this.AddExtendedPropertyAttributes(uniqueConstraint.ExtendedProperties);
			this.w.WriteStartElement("xs", "selector", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("xpath", ".//" + this.ConstraintPrefix + XmlHelper.Encode(rel.ChildTable.TableName));
			this.w.WriteEndElement();
			foreach (DataColumn dataColumn3 in rel.ChildColumns)
			{
				this.w.WriteStartElement("xs", "field", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteStartAttribute("xpath", string.Empty);
				if (dataColumn3.ColumnMapping == MappingType.Attribute)
				{
					this.w.WriteString("@");
				}
				this.w.WriteString(this.ConstraintPrefix);
				this.w.WriteString(XmlHelper.Encode(dataColumn3.ColumnName));
				this.w.WriteEndAttribute();
				this.w.WriteEndElement();
			}
			this.w.WriteEndElement();
		}

		private bool CheckExtendedPropertyExists(DataTable[] tables, DataRelation[] relations)
		{
			if (this.dataSetProperties.Count > 0)
			{
				return true;
			}
			foreach (DataTable dataTable in tables)
			{
				if (dataTable.ExtendedProperties.Count > 0)
				{
					return true;
				}
				foreach (object obj in dataTable.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					if (dataColumn.ExtendedProperties.Count > 0)
					{
						return true;
					}
				}
				foreach (object obj2 in dataTable.Constraints)
				{
					Constraint constraint = (Constraint)obj2;
					if (constraint.ExtendedProperties.Count > 0)
					{
						return true;
					}
				}
			}
			if (relations == null)
			{
				return false;
			}
			foreach (DataRelation dataRelation in relations)
			{
				if (dataRelation.ExtendedProperties.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		private void AddExtendedPropertyAttributes(PropertyCollection props)
		{
			foreach (object obj in props)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				this.w.WriteStartAttribute("msprop", XmlConvert.EncodeName(dictionaryEntry.Key.ToString()), "urn:schemas-microsoft-com:xml-msprop");
				if (dictionaryEntry.Value != null)
				{
					this.w.WriteString(DataSet.WriteObjectXml(dictionaryEntry.Value));
				}
				this.w.WriteEndAttribute();
			}
		}

		private void WriteTableElement(DataTable table)
		{
			this.w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("name", XmlHelper.Encode(table.TableName));
			this.AddExtendedPropertyAttributes(table.ExtendedProperties);
			this.WriteTableType(table);
			this.w.WriteEndElement();
		}

		private void WriteTableType(DataTable table)
		{
			ArrayList arrayList;
			ArrayList arrayList2;
			DataColumn dataColumn;
			DataSet.SplitColumns(table, out arrayList, out arrayList2, out dataColumn);
			this.w.WriteStartElement("xs", "complexType", "http://www.w3.org/2001/XMLSchema");
			if (dataColumn != null)
			{
				this.w.WriteStartElement("xs", "simpleContent", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteAttributeString("msdata", "ColumnName", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(dataColumn.ColumnName));
				this.w.WriteAttributeString("msdata", "Ordinal", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.ToString(dataColumn.Ordinal));
				this.w.WriteStartElement("xs", "extension", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteStartAttribute("base", string.Empty);
				this.WriteQName(this.MapType(dataColumn.DataType));
				this.w.WriteEndAttribute();
				this.WriteTableAttributes(arrayList);
				this.w.WriteEndElement();
			}
			else
			{
				this.WriteTableAttributes(arrayList);
				bool flag = false;
				foreach (object obj in table.ParentRelations)
				{
					DataRelation dataRelation = (DataRelation)obj;
					if (dataRelation.Nested)
					{
						flag = true;
						break;
					}
				}
				if (arrayList2.Count > 0 || (flag && this.tables.Length < 2))
				{
					this.w.WriteStartElement("xs", "sequence", "http://www.w3.org/2001/XMLSchema");
					foreach (object obj2 in arrayList2)
					{
						DataColumn dataColumn2 = (DataColumn)obj2;
						this.WriteTableTypeParticles(dataColumn2);
					}
					foreach (object obj3 in table.ChildRelations)
					{
						DataRelation dataRelation2 = (DataRelation)obj3;
						if (dataRelation2.Nested)
						{
							this.WriteChildRelations(dataRelation2);
						}
					}
					this.w.WriteEndElement();
				}
			}
			this.w.WriteFullEndElement();
		}

		private void WriteTableTypeParticles(DataColumn col)
		{
			this.w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("name", XmlHelper.Encode(col.ColumnName));
			if (col.ColumnName != col.Caption && col.Caption != string.Empty)
			{
				this.w.WriteAttributeString("msdata", "Caption", "urn:schemas-microsoft-com:xml-msdata", col.Caption);
			}
			if (col.AutoIncrement)
			{
				this.w.WriteAttributeString("msdata", "AutoIncrement", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			if (col.AutoIncrementSeed != 0L)
			{
				this.w.WriteAttributeString("msdata", "AutoIncrementSeed", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.ToString(col.AutoIncrementSeed));
			}
			if (col.AutoIncrementStep != 1L)
			{
				this.w.WriteAttributeString("msdata", "AutoIncrementStep", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.ToString(col.AutoIncrementStep));
			}
			if (!DataColumn.GetDefaultValueForType(col.DataType).Equals(col.DefaultValue))
			{
				this.w.WriteAttributeString("default", DataSet.WriteObjectXml(col.DefaultValue));
			}
			if (col.ReadOnly)
			{
				this.w.WriteAttributeString("msdata", "ReadOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			XmlQualifiedName xmlQualifiedName = null;
			if (col.MaxLength < 0)
			{
				this.w.WriteStartAttribute("type", string.Empty);
				xmlQualifiedName = this.MapType(col.DataType);
				this.WriteQName(xmlQualifiedName);
				this.w.WriteEndAttribute();
			}
			if (xmlQualifiedName == XmlConstants.QnString && col.DataType != typeof(string) && col.DataType != typeof(char))
			{
				this.w.WriteStartAttribute("msdata", "DataType", "urn:schemas-microsoft-com:xml-msdata");
				string assemblyQualifiedName = col.DataType.AssemblyQualifiedName;
				this.w.WriteString(assemblyQualifiedName);
				this.w.WriteEndAttribute();
			}
			if (col.AllowDBNull)
			{
				this.w.WriteAttributeString("minOccurs", "0");
			}
			if (col.MaxLength > -1)
			{
				this.WriteSimpleType(col);
			}
			this.AddExtendedPropertyAttributes(col.ExtendedProperties);
			this.w.WriteEndElement();
		}

		private void WriteChildRelations(DataRelation rel)
		{
			if (rel.ChildTable.Namespace != this.dataSetNamespace || this.tables.Length < 2)
			{
				this.w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteStartAttribute("ref", string.Empty);
				this.w.WriteQualifiedName(XmlHelper.Encode(rel.ChildTable.TableName), rel.ChildTable.Namespace);
				this.w.WriteEndAttribute();
			}
			else
			{
				this.w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
				this.w.WriteStartAttribute("name", string.Empty);
				this.w.WriteQualifiedName(XmlHelper.Encode(rel.ChildTable.TableName), rel.ChildTable.Namespace);
				this.w.WriteEndAttribute();
				this.w.WriteAttributeString("minOccurs", "0");
				this.w.WriteAttributeString("maxOccurs", "unbounded");
				this.globalTypeTables.Add(rel.ChildTable);
			}
			if (this.tables.Length > 1)
			{
				this.WriteTableType(rel.ChildTable);
			}
			this.w.WriteEndElement();
		}

		private void WriteTableAttributes(ArrayList atts)
		{
			foreach (object obj in atts)
			{
				DataColumn dataColumn = (DataColumn)obj;
				this.w.WriteStartElement("xs", "attribute", "http://www.w3.org/2001/XMLSchema");
				string text = XmlHelper.Encode(dataColumn.ColumnName);
				if (dataColumn.Namespace != string.Empty)
				{
					this.w.WriteAttributeString("form", "qualified");
					string text2 = ((!(dataColumn.Prefix == string.Empty)) ? dataColumn.Prefix : ("app" + this.additionalNamespaces.Count));
					text = text2 + ":" + text;
					this.additionalNamespaces[text2] = dataColumn.Namespace;
				}
				this.w.WriteAttributeString("name", text);
				this.AddExtendedPropertyAttributes(dataColumn.ExtendedProperties);
				if (dataColumn.ReadOnly)
				{
					this.w.WriteAttributeString("msdata", "ReadOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
				}
				if (dataColumn.MaxLength < 0)
				{
					this.w.WriteStartAttribute("type", string.Empty);
					this.WriteQName(this.MapType(dataColumn.DataType));
					this.w.WriteEndAttribute();
				}
				if (!dataColumn.AllowDBNull)
				{
					this.w.WriteAttributeString("use", "required");
				}
				if (dataColumn.DefaultValue != DataColumn.GetDefaultValueForType(dataColumn.DataType))
				{
					this.w.WriteAttributeString("default", DataSet.WriteObjectXml(dataColumn.DefaultValue));
				}
				if (dataColumn.MaxLength > -1)
				{
					this.WriteSimpleType(dataColumn);
				}
				this.w.WriteEndElement();
			}
		}

		private void WriteSimpleType(DataColumn col)
		{
			this.w.WriteStartElement("xs", "simpleType", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteStartElement("xs", "restriction", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteStartAttribute("base", string.Empty);
			this.WriteQName(this.MapType(col.DataType));
			this.w.WriteEndAttribute();
			this.w.WriteStartElement("xs", "maxLength", "http://www.w3.org/2001/XMLSchema");
			this.w.WriteAttributeString("value", XmlConvert.ToString(col.MaxLength));
			this.w.WriteEndElement();
			this.w.WriteEndElement();
			this.w.WriteEndElement();
		}

		private void WriteQName(XmlQualifiedName name)
		{
			this.w.WriteQualifiedName(name.Name, name.Namespace);
		}

		private void CheckNamespace(string prefix, string ns, ListDictionary names, ListDictionary includes)
		{
			if (ns == string.Empty)
			{
				return;
			}
			if (this.dataSetNamespace != ns && (string)names[prefix] != ns)
			{
				for (int i = 1; i < 2147483647; i++)
				{
					string text = "app" + i;
					if (names[text] == null)
					{
						names.Add(text, ns);
						this.HandleExternalNamespace(text, ns, includes);
						break;
					}
				}
			}
		}

		private void HandleExternalNamespace(string prefix, string ns, ListDictionary includes)
		{
			if (includes.Contains(ns))
			{
				return;
			}
			includes.Add(ns, "_" + prefix + ".xsd");
		}

		private XmlQualifiedName MapType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return XmlConstants.QnBoolean;
			case TypeCode.SByte:
				return XmlConstants.QnSbyte;
			case TypeCode.Byte:
				return XmlConstants.QnUnsignedByte;
			case TypeCode.Int16:
				return XmlConstants.QnShort;
			case TypeCode.UInt16:
				return XmlConstants.QnUnsignedShort;
			case TypeCode.Int32:
				return XmlConstants.QnInt;
			case TypeCode.UInt32:
				return XmlConstants.QnUnsignedInt;
			case TypeCode.Int64:
				return XmlConstants.QnLong;
			case TypeCode.UInt64:
				return XmlConstants.QnUnsignedLong;
			case TypeCode.Single:
				return XmlConstants.QnFloat;
			case TypeCode.Double:
				return XmlConstants.QnDouble;
			case TypeCode.Decimal:
				return XmlConstants.QnDecimal;
			case TypeCode.DateTime:
				return XmlConstants.QnDateTime;
			case TypeCode.String:
				return XmlConstants.QnString;
			}
			if (typeof(TimeSpan) == type)
			{
				return XmlConstants.QnDuration;
			}
			if (typeof(Uri) == type)
			{
				return XmlConstants.QnUri;
			}
			if (typeof(byte[]) == type)
			{
				return XmlConstants.QnBase64Binary;
			}
			if (typeof(XmlQualifiedName) == type)
			{
				return XmlConstants.QnXmlQualifiedName;
			}
			return XmlConstants.QnString;
		}

		private const string xmlnsxs = "http://www.w3.org/2001/XMLSchema";

		private XmlWriter w;

		private DataTable[] tables;

		private DataRelation[] relations;

		private string mainDataTable;

		private string dataSetName;

		private string dataSetNamespace;

		private PropertyCollection dataSetProperties;

		private CultureInfo dataSetLocale;

		private ArrayList globalTypeTables = new ArrayList();

		private Hashtable additionalNamespaces = new Hashtable();

		private ArrayList annotation = new ArrayList();
	}
}
