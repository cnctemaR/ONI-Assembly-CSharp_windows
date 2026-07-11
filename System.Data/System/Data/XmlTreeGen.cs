using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Xml;

namespace System.Data
{
	internal sealed class XmlTreeGen
	{
		internal XmlTreeGen(SchemaFormat format)
		{
			this._schFormat = format;
		}

		internal static void AddExtendedProperties(PropertyCollection props, XmlElement node)
		{
			XmlTreeGen.AddExtendedProperties(props, node, null);
		}

		internal static void AddExtendedProperties(PropertyCollection props, XmlElement node, Type type)
		{
			if (props != null)
			{
				foreach (object obj in props)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					string text;
					if (dictionaryEntry.Key is INullable)
					{
						text = (string)SqlConvert.ChangeTypeForXML(dictionaryEntry.Key, typeof(string));
					}
					else
					{
						text = Convert.ToString(dictionaryEntry.Key, CultureInfo.InvariantCulture);
					}
					string text2;
					if (dictionaryEntry.Value is INullable)
					{
						text2 = (string)SqlConvert.ChangeTypeForXML(dictionaryEntry.Value, typeof(string));
					}
					else if (dictionaryEntry.Value is BigInteger)
					{
						text2 = (string)BigIntegerStorage.ConvertFromBigInteger((BigInteger)dictionaryEntry.Value, typeof(string), CultureInfo.InvariantCulture);
					}
					else
					{
						text2 = Convert.ToString(dictionaryEntry.Value, CultureInfo.InvariantCulture);
					}
					if (type == typeof(DataRelation))
					{
						text = "rel_" + text;
					}
					else if (type == typeof(ForeignKeyConstraint))
					{
						text = "fk_" + text;
					}
					node.SetAttribute(XmlConvert.EncodeLocalName(text), "urn:schemas-microsoft-com:xml-msprop", text2);
				}
			}
		}

		internal void AddXdoProperties(object instance, XmlElement root, XmlDocument xd)
		{
			if (instance == null)
			{
				return;
			}
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
			if (!(instance is DataSet) && !(instance is DataTable) && !(instance is DataColumn) && !(instance is DataRelation))
			{
				return;
			}
			for (int i = 0; i < properties.Count; i++)
			{
				this.AddXdoProperty(properties[i], instance, root, xd);
			}
		}

		internal void AddXdoProperty(PropertyDescriptor pd, object instance, XmlElement root, XmlDocument xd)
		{
			Type propertyType = pd.PropertyType;
			bool flag = false;
			DataColumn dataColumn = null;
			bool flag2 = false;
			bool flag3 = false;
			if (instance is DataColumn)
			{
				dataColumn = (DataColumn)instance;
				flag = true;
				flag2 = dataColumn.IsSqlType;
				flag3 = dataColumn.ImplementsINullable;
			}
			if (!flag3 && propertyType != typeof(string) && propertyType != typeof(bool) && propertyType != typeof(Type) && propertyType != typeof(object) && propertyType != typeof(CultureInfo) && propertyType != typeof(long) && propertyType != typeof(int))
			{
				return;
			}
			if ((!pd.ShouldSerializeValue(instance) || !pd.Attributes.Contains(DesignerSerializationVisibilityAttribute.Visible)) && !flag2)
			{
				return;
			}
			object value = pd.GetValue(instance);
			if (value is InternalDataCollectionBase)
			{
				return;
			}
			if (value is PropertyCollection)
			{
				return;
			}
			if (string.Equals(pd.Name, "Namespace", StringComparison.Ordinal) || string.Equals(pd.Name, "PrimaryKey", StringComparison.Ordinal) || string.Equals(pd.Name, "ColumnName", StringComparison.Ordinal) || string.Equals(pd.Name, "DefaultValue", StringComparison.Ordinal) || string.Equals(pd.Name, "TableName", StringComparison.Ordinal) || string.Equals(pd.Name, "DataSetName", StringComparison.Ordinal) || string.Equals(pd.Name, "AllowDBNull", StringComparison.Ordinal) || string.Equals(pd.Name, "Unique", StringComparison.Ordinal) || string.Equals(pd.Name, "NestedInDataSet", StringComparison.Ordinal) || string.Equals(pd.Name, "Locale", StringComparison.Ordinal) || string.Equals(pd.Name, "CaseSensitive", StringComparison.Ordinal) || string.Equals(pd.Name, "RemotingFormat", StringComparison.Ordinal))
			{
				return;
			}
			if (flag)
			{
				if (string.Equals(pd.Name, "DataType", StringComparison.Ordinal))
				{
					string text = XmlTreeGen.XmlDataTypeName(dataColumn.DataType);
					if (flag2 || dataColumn.DataType == typeof(BigInteger))
					{
						root.SetAttribute("DataType", "urn:schemas-microsoft-com:xml-msdata", dataColumn.DataType.FullName);
						return;
					}
					if (text.Length == 0 || flag3 || (text == "anyType" && dataColumn.XmlDataType != "anyType") || dataColumn.DataType == typeof(DateTimeOffset))
					{
						this.SetMSDataAttribute(root, dataColumn.DataType);
					}
					return;
				}
				else if (string.Equals(pd.Name, "Attribute", StringComparison.Ordinal))
				{
					return;
				}
			}
			string text2 = pd.Converter.ConvertToString(value);
			root.SetAttribute(pd.Name, "urn:schemas-microsoft-com:xml-msdata", text2);
		}

		internal static string XmlDataTypeName(Type type)
		{
			if (type == typeof(char))
			{
				return "_";
			}
			if (type == typeof(byte[]) || type == typeof(SqlBytes))
			{
				return "base64Binary";
			}
			if (type == typeof(DateTime) || type == typeof(SqlDateTime))
			{
				return "dateTime";
			}
			if (type == typeof(TimeSpan))
			{
				return "duration";
			}
			if (type == typeof(decimal) || type == typeof(SqlDecimal) || type == typeof(SqlMoney))
			{
				return "decimal";
			}
			if (type == typeof(int))
			{
				return "int";
			}
			if (type == typeof(bool) || type == typeof(SqlBoolean))
			{
				return "boolean";
			}
			if (type == typeof(float) || type == typeof(SqlSingle))
			{
				return "float";
			}
			if (type == typeof(double) || type == typeof(SqlDouble))
			{
				return "double";
			}
			if (type == typeof(sbyte) || type == typeof(SqlByte))
			{
				return "byte";
			}
			if (type == typeof(byte))
			{
				return "unsignedByte";
			}
			if (type == typeof(short) || type == typeof(SqlInt16))
			{
				return "short";
			}
			if (type == typeof(int) || type == typeof(SqlInt32))
			{
				return "int";
			}
			if (type == typeof(long) || type == typeof(SqlInt64))
			{
				return "long";
			}
			if (type == typeof(ushort))
			{
				return "unsignedShort";
			}
			if (type == typeof(uint))
			{
				return "unsignedInt";
			}
			if (type == typeof(ulong))
			{
				return "unsignedLong";
			}
			if (type == typeof(BigInteger))
			{
				return "anyType";
			}
			if (type == typeof(Uri))
			{
				return "anyURI";
			}
			if (type == typeof(SqlBinary))
			{
				return "hexBinary";
			}
			if (type == typeof(string) || type == typeof(SqlGuid) || type == typeof(SqlString) || type == typeof(SqlChars))
			{
				return "string";
			}
			if (type == typeof(object) || type == typeof(SqlXml) || type == typeof(DateTimeOffset))
			{
				return "anyType";
			}
			return string.Empty;
		}

		private void GenerateConstraintNames(DataTable table, bool fromTable)
		{
			StringBuilder stringBuilder = null;
			foreach (object obj in table.Constraints)
			{
				Constraint constraint = (Constraint)obj;
				if (!fromTable || !(constraint is ForeignKeyConstraint) || this._tables.Contains(((ForeignKeyConstraint)constraint).RelatedTable))
				{
					int num = 0;
					string text = constraint.ConstraintName;
					while (this._constraintNames.Contains(text))
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder();
						}
						stringBuilder.Append(table.TableName).Append('_').Append(constraint.ConstraintName);
						if (0 < num)
						{
							stringBuilder.Append('_').Append(num);
						}
						num++;
						text = stringBuilder.ToString();
						stringBuilder.Length = 0;
					}
					this._constraintNames.Add(text);
					constraint.SchemaName = text;
				}
			}
		}

		private void GenerateConstraintNames(ArrayList tables)
		{
			for (int i = 0; i < tables.Count; i++)
			{
				this.GenerateConstraintNames((DataTable)tables[i], true);
			}
		}

		private void GenerateConstraintNames(DataSet ds)
		{
			foreach (object obj in ds.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				this.GenerateConstraintNames(dataTable, false);
			}
		}

		private static bool _PropsNotEmpty(PropertyCollection props)
		{
			return props != null && props.Count != 0;
		}

		private bool HaveExtendedProperties(DataSet ds)
		{
			if (XmlTreeGen._PropsNotEmpty(ds._extendedProperties))
			{
				return true;
			}
			for (int i = 0; i < ds.Tables.Count; i++)
			{
				DataTable dataTable = ds.Tables[i];
				if (XmlTreeGen._PropsNotEmpty(dataTable._extendedProperties))
				{
					return true;
				}
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					if (XmlTreeGen._PropsNotEmpty(dataTable.Columns[j]._extendedProperties))
					{
						return true;
					}
				}
			}
			for (int k = 0; k < ds.Relations.Count; k++)
			{
				if (XmlTreeGen._PropsNotEmpty(ds.Relations[k]._extendedProperties))
				{
					return true;
				}
			}
			return false;
		}

		internal void WriteSchemaRoot(XmlDocument xd, XmlElement rootSchema, string targetNamespace)
		{
			if (!string.IsNullOrEmpty(targetNamespace))
			{
				rootSchema.SetAttribute("targetNamespace", targetNamespace);
				rootSchema.SetAttribute("xmlns:mstns", targetNamespace);
			}
			rootSchema.SetAttribute("xmlns", targetNamespace);
			rootSchema.SetAttribute("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
			rootSchema.SetAttribute("xmlns:msdata", "urn:schemas-microsoft-com:xml-msdata");
			if (this._ds != null && this.HaveExtendedProperties(this._ds))
			{
				rootSchema.SetAttribute("xmlns:msprop", "urn:schemas-microsoft-com:xml-msprop");
			}
			if (!string.IsNullOrEmpty(targetNamespace))
			{
				rootSchema.SetAttribute("attributeFormDefault", "qualified");
				rootSchema.SetAttribute("elementFormDefault", "qualified");
			}
		}

		internal static void ValidateColumnMapping(Type columnType)
		{
			if (DataStorage.IsTypeCustomType(columnType))
			{
				throw ExceptionBuilder.InvalidDataColumnMapping(columnType);
			}
		}

		internal void SetupAutoGenerated(DataSet ds)
		{
			foreach (object obj in ds.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				this.SetupAutoGenerated(dataTable);
			}
		}

		internal void SetupAutoGenerated(ArrayList dt)
		{
			for (int i = 0; i < dt.Count; i++)
			{
				this.SetupAutoGenerated((DataTable)dt[i]);
			}
		}

		internal void SetupAutoGenerated(DataTable dt)
		{
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (XmlTreeGen.AutoGenerated(dataColumn))
				{
					this._autogenerated[dataColumn] = dataColumn;
				}
			}
			foreach (object obj2 in dt.Constraints)
			{
				Constraint constraint = (Constraint)obj2;
				ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
				if (foreignKeyConstraint != null)
				{
					if (this.AutoGenerated(foreignKeyConstraint))
					{
						this._autogenerated[foreignKeyConstraint] = foreignKeyConstraint;
					}
					else
					{
						if (this._autogenerated[foreignKeyConstraint.Columns[0]] != null)
						{
							this._autogenerated[foreignKeyConstraint.Columns[0]] = null;
						}
						if (this._autogenerated[foreignKeyConstraint.RelatedColumnsReference[0]] != null)
						{
							this._autogenerated[foreignKeyConstraint.RelatedColumnsReference[0]] = null;
						}
						UniqueConstraint uniqueConstraint = (UniqueConstraint)foreignKeyConstraint.RelatedTable.Constraints.FindConstraint(new UniqueConstraint("TEMP", foreignKeyConstraint.RelatedColumnsReference));
						if (uniqueConstraint != null)
						{
							if (this._autogenerated[uniqueConstraint] != null)
							{
								this._autogenerated[uniqueConstraint] = null;
							}
							if (this._autogenerated[uniqueConstraint.Key.ColumnsReference[0]] != null)
							{
								this._autogenerated[uniqueConstraint.Key.ColumnsReference[0]] = null;
							}
						}
					}
				}
				else
				{
					UniqueConstraint uniqueConstraint2 = (UniqueConstraint)constraint;
					if (XmlTreeGen.AutoGenerated(uniqueConstraint2))
					{
						this._autogenerated[uniqueConstraint2] = uniqueConstraint2;
					}
					else if (this._autogenerated[uniqueConstraint2.Key.ColumnsReference[0]] != null)
					{
						this._autogenerated[uniqueConstraint2.Key.ColumnsReference[0]] = null;
					}
				}
			}
		}

		private void CreateTablesHierarchy(DataTable dt)
		{
			foreach (object obj in dt.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (!this._tables.Contains(dataRelation.ChildTable))
				{
					this._tables.Add(dataRelation.ChildTable);
					this.CreateTablesHierarchy(dataRelation.ChildTable);
				}
			}
		}

		private void CreateRelations(DataTable dt)
		{
			foreach (object obj in dt.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (!this._relations.Contains(dataRelation))
				{
					this._relations.Add(dataRelation);
					this.CreateRelations(dataRelation.ChildTable);
				}
			}
		}

		private DataTable[] CreateToplevelTables()
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this._tables.Count; i++)
			{
				DataTable dataTable = (DataTable)this._tables[i];
				if (dataTable.ParentRelations.Count == 0)
				{
					arrayList.Add(dataTable);
				}
				else
				{
					bool flag = false;
					for (int j = 0; j < dataTable.ParentRelations.Count; j++)
					{
						if (dataTable.ParentRelations[j].Nested)
						{
							if (dataTable.ParentRelations[j].ParentTable == dataTable)
							{
								flag = false;
								break;
							}
							flag = true;
						}
					}
					if (!flag)
					{
						arrayList.Add(dataTable);
					}
				}
			}
			if (arrayList.Count == 0)
			{
				return Array.Empty<DataTable>();
			}
			DataTable[] array = new DataTable[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}

		internal void SchemaTree(XmlDocument xd, XmlWriter xmlWriter, DataSet ds, DataTable dt, bool writeHierarchy)
		{
			this._constraintNames = new ArrayList();
			this._autogenerated = new Hashtable();
			bool flag = this._filePath != null;
			this._dsElement = xd.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			bool flag2 = false;
			if (ds != null)
			{
				this._ds = ds;
				using (IEnumerator enumerator = ds.Tables.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						DataTable dataTable = (DataTable)obj;
						this._tables.Add(dataTable);
					}
					goto IL_00C5;
				}
			}
			if (dt.DataSet != null)
			{
				this._ds = dt.DataSet;
			}
			this._tables.Add(dt);
			if (writeHierarchy)
			{
				this.CreateTablesHierarchy(dt);
			}
			IL_00C5:
			this._dc = xd;
			this._namespaces = new Hashtable();
			this._prefixes = new Hashtable();
			XmlElement xmlElement = xd.CreateElement("xs", "schema", "http://www.w3.org/2001/XMLSchema");
			this._sRoot = xmlElement;
			if (this._ds != null)
			{
				xmlElement.SetAttribute("id", XmlConvert.EncodeLocalName(this._ds.DataSetName));
			}
			else
			{
				xmlElement.SetAttribute("id", XmlConvert.EncodeLocalName("NewDataSet"));
			}
			if (this._ds != null)
			{
				this.WriteSchemaRoot(xd, xmlElement, this._ds.Namespace);
			}
			else
			{
				this.WriteSchemaRoot(xd, xmlElement, dt.Namespace);
			}
			if (this._schFormat == SchemaFormat.Remoting)
			{
				if (this._ds != null)
				{
					this._namespaces[this._ds.Namespace] = xmlElement;
				}
				else
				{
					this._namespaces[dt.Namespace] = xmlElement;
				}
			}
			if (this._schFormat != SchemaFormat.Remoting && this._ds != null)
			{
				this._namespaces[this._ds.Namespace] = xmlElement;
				if (this._ds.Namespace.Length == 0)
				{
					this._prefixes[this._ds.Namespace] = null;
				}
				else
				{
					xmlElement.SetAttribute("xmlns:mstns", this._ds.Namespace);
					this._prefixes[this._ds.Namespace] = "mstns";
				}
			}
			if (ds != null)
			{
				this.GenerateConstraintNames(ds);
			}
			else
			{
				this.GenerateConstraintNames(this._tables);
			}
			if (this._schFormat != SchemaFormat.Remoting)
			{
				if (ds != null)
				{
					this.SetupAutoGenerated(ds);
				}
				else
				{
					this.SetupAutoGenerated(this._tables);
				}
			}
			DataTable[] array = ((ds != null) ? ds.TopLevelTables(true) : this.CreateToplevelTables());
			if (array.Length == 0 || this._schFormat == SchemaFormat.WebServiceSkipSchema || this._schFormat == SchemaFormat.RemotingSkipSchema)
			{
				this.FillDataSetElement(xd, ds, dt);
				xmlElement.AppendChild(this._dsElement);
				this.AddXdoProperties(this._ds, this._dsElement, xd);
				XmlTreeGen.AddExtendedProperties(ds._extendedProperties, this._dsElement);
				xd.AppendChild(xmlElement);
				xd.Save(xmlWriter);
				xmlWriter.Flush();
				return;
			}
			XmlElement xmlElement2 = this.FillDataSetElement(xd, ds, dt);
			this._constraintSeparator = xd.CreateElement("xs", "SHOULDNOTBEHERE", "http://www.w3.org/2001/XMLSchema");
			this._dsElement.AppendChild(this._constraintSeparator);
			if (this._ds != null)
			{
				this.AddXdoProperties(this._ds, this._dsElement, xd);
				XmlTreeGen.AddExtendedProperties(this._ds._extendedProperties, this._dsElement);
			}
			for (int i = 0; i < array.Length; i++)
			{
				XmlElement xmlElement3 = this.HandleTable(array[i], xd, xmlElement);
				if ((this._ds != null && this._ds.Namespace == array[i].Namespace) || string.IsNullOrEmpty(array[i].Namespace) || this._schFormat == SchemaFormat.Remoting)
				{
					bool flag3 = array[i]._fNestedInDataset;
					if (this._ds != null && this._ds.Namespace.Length != 0 && string.IsNullOrEmpty(array[i].Namespace))
					{
						flag3 = true;
					}
					if (array[i].SelfNested)
					{
						flag3 = false;
					}
					if (array[i].NestedParentsCount > 1)
					{
						flag3 = false;
					}
					if (flag3)
					{
						if (array[i].MinOccurs != 1m)
						{
							xmlElement3.SetAttribute("minOccurs", array[i].MinOccurs.ToString(CultureInfo.InvariantCulture));
						}
						if (array[i].MaxOccurs == -1m)
						{
							xmlElement3.SetAttribute("maxOccurs", "unbounded");
						}
						else if (array[i].MaxOccurs != 1m)
						{
							xmlElement3.SetAttribute("maxOccurs", array[i].MaxOccurs.ToString(CultureInfo.InvariantCulture));
						}
					}
					if (!flag3)
					{
						xmlElement.AppendChild(xmlElement3);
						XmlElement xmlElement4 = xd.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
						if ((this._ds != null && this._ds.Namespace == array[i].Namespace) || string.IsNullOrEmpty(array[i].Namespace) || this._schFormat == SchemaFormat.Remoting)
						{
							xmlElement4.SetAttribute("ref", array[i].EncodedTableName);
						}
						else
						{
							xmlElement4.SetAttribute("ref", (string)this._prefixes[array[i].Namespace] + ":" + array[i].EncodedTableName);
						}
						xmlElement2.AppendChild(xmlElement4);
					}
					else
					{
						xmlElement2.AppendChild(xmlElement3);
					}
				}
				else
				{
					this.AppendChildWithoutRef(xmlElement, array[i].Namespace, xmlElement3, "element");
					XmlElement xmlElement5 = xd.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
					xmlElement5.SetAttribute("ref", (string)this._prefixes[array[i].Namespace] + ":" + array[i].EncodedTableName);
					xmlElement2.AppendChild(xmlElement5);
				}
			}
			this._dsElement.RemoveChild(this._constraintSeparator);
			xmlElement.AppendChild(this._dsElement);
			DataRelation[] array2 = Array.Empty<DataRelation>();
			if (ds != null && this._tables.Count > 0)
			{
				array2 = new DataRelation[ds.Relations.Count];
				for (int j = 0; j < ds.Relations.Count; j++)
				{
					array2[j] = ds.Relations[j];
				}
			}
			else if (writeHierarchy && this._tables.Count > 0)
			{
				this.CreateRelations((DataTable)this._tables[0]);
				array2 = new DataRelation[this._relations.Count];
				this._relations.CopyTo(array2, 0);
			}
			XmlElement xmlElement6 = null;
			XmlElement xmlElement7 = null;
			foreach (DataRelation dataRelation in array2)
			{
				if ((!dataRelation.Nested || flag2) && dataRelation.ChildKeyConstraint == null)
				{
					if (xmlElement6 == null)
					{
						xmlElement6 = xd.CreateElement("xs", "annotation", "http://www.w3.org/2001/XMLSchema");
						xmlElement.AppendChild(xmlElement6);
						xmlElement7 = xd.CreateElement("xs", "appinfo", "http://www.w3.org/2001/XMLSchema");
						xmlElement6.AppendChild(xmlElement7);
					}
					xmlElement7.AppendChild(this.HandleRelation(dataRelation, xd));
				}
			}
			XmlComment xmlComment = null;
			bool flag4 = this._namespaces.Count > 1 && !flag;
			if (this._schFormat != SchemaFormat.Remoting && this._schFormat != SchemaFormat.RemotingSkipSchema)
			{
				foreach (object obj2 in this._namespaces.Keys)
				{
					string text = (string)obj2;
					if (!(text == ((this._ds != null) ? this._ds.Namespace : dt.Namespace)) && !string.IsNullOrEmpty(text))
					{
						XmlElement xmlElement8 = xd.CreateElement("xs", "import", "http://www.w3.org/2001/XMLSchema");
						xmlElement8.SetAttribute("namespace", text);
						if (this._schFormat != SchemaFormat.WebService && !flag4)
						{
							xmlElement8.SetAttribute("schemaLocation", string.Concat(new object[]
							{
								this._fileName,
								"_",
								this._prefixes[text],
								".xsd"
							}));
						}
						xmlElement.PrependChild(xmlElement8);
					}
				}
				if (this._schFormat != SchemaFormat.WebService && flag4)
				{
					xmlElement.SetAttribute("schemafragmentcount", "urn:schemas-microsoft-com:xml-msdata", this._namespaces.Count.ToString(CultureInfo.InvariantCulture));
				}
				xd.AppendChild(xmlElement);
				if (this._schFormat != SchemaFormat.WebService && flag4)
				{
					xd.WriteTo(xmlWriter);
				}
				else
				{
					xd.Save(xmlWriter);
				}
				xd.RemoveChild(xmlElement);
				using (IEnumerator enumerator = this._namespaces.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj3 = enumerator.Current;
						string text2 = (string)obj3;
						if (!(text2 == ((this._ds != null) ? this._ds.Namespace : dt.Namespace)) && !string.IsNullOrEmpty(text2))
						{
							XmlWriter xmlWriter2 = null;
							if (!flag)
							{
								xmlWriter2 = xmlWriter;
							}
							else
							{
								xmlWriter2 = new XmlTextWriter(string.Concat(new object[]
								{
									this._filePath,
									this._fileName,
									"_",
									this._prefixes[text2],
									".xsd"
								}), null);
							}
							try
							{
								if (flag)
								{
									if (xmlWriter2 is XmlTextWriter)
									{
										((XmlTextWriter)xmlWriter2).Formatting = Formatting.Indented;
									}
									xmlWriter2.WriteStartDocument(true);
								}
								XmlElement xmlElement9 = (XmlElement)this._namespaces[text2];
								this._dc.AppendChild(xmlElement9);
								foreach (object obj4 in this._namespaces.Keys)
								{
									string text3 = (string)obj4;
									if (!(text2 == text3))
									{
										string text4 = (string)this._prefixes[text3];
										if (text4 != null)
										{
											xmlElement9.SetAttribute("xmlns:" + text4, text3);
											XmlElement xmlElement10 = this._dc.CreateElement("xs", "import", "http://www.w3.org/2001/XMLSchema");
											xmlElement10.SetAttribute("namespace", text3);
											if (this._schFormat != SchemaFormat.WebService && !flag4)
											{
												if (text3 == ((this._ds != null) ? this._ds.Namespace : dt.Namespace))
												{
													xmlElement10.SetAttribute("schemaLocation", this._fileName + this._fileExt);
												}
												else
												{
													xmlElement10.SetAttribute("schemaLocation", this._fileName + "_" + text4 + ".xsd");
												}
											}
											xmlElement9.PrependChild(xmlElement10);
										}
									}
								}
								if (this._schFormat != SchemaFormat.WebService && flag4)
								{
									this._dc.WriteTo(xmlWriter2);
								}
								else
								{
									this._dc.Save(xmlWriter2);
								}
								this._dc.RemoveChild(xmlElement9);
								if (flag)
								{
									xmlWriter2.WriteEndDocument();
								}
							}
							finally
							{
								if (flag)
								{
									xmlWriter2.Close();
								}
							}
						}
					}
					goto IL_0B69;
				}
			}
			xd.AppendChild(xmlElement);
			xd.Save(xmlWriter);
			IL_0B69:
			if (xmlComment != null)
			{
				xmlElement.PrependChild(xmlComment);
			}
			if (!flag)
			{
				xmlWriter.Flush();
			}
		}

		internal XmlElement SchemaTree(XmlDocument xd, DataTable dt)
		{
			this._dsElement = xd.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			this._constraintNames = new ArrayList();
			this._ds = dt.DataSet;
			this._dc = xd;
			this._namespaces = new Hashtable();
			this._prefixes = new Hashtable();
			if (this._schFormat != SchemaFormat.Remoting)
			{
				this._autogenerated = new Hashtable();
			}
			XmlElement xmlElement = xd.CreateElement("xs", "schema", "http://www.w3.org/2001/XMLSchema");
			this._sRoot = xmlElement;
			this.WriteSchemaRoot(xd, xmlElement, dt.Namespace);
			this.FillDataSetElement(xd, null, dt);
			this._constraintSeparator = xd.CreateElement("xs", "SHOULDNOTBEHERE", "http://www.w3.org/2001/XMLSchema");
			this._dsElement.AppendChild(this._constraintSeparator);
			if (this._schFormat != SchemaFormat.Remoting)
			{
				if (this._ds != null)
				{
					this._namespaces[this._ds.Namespace] = xmlElement;
					if (this._ds.Namespace.Length == 0)
					{
						this._prefixes[this._ds.Namespace] = null;
					}
					else
					{
						xmlElement.SetAttribute("xmlns:mstns", this._ds.Namespace);
						this._prefixes[this._ds.Namespace] = "mstns";
					}
				}
				else
				{
					this._namespaces[dt.Namespace] = xmlElement;
					if (dt.Namespace.Length == 0)
					{
						this._prefixes[dt.Namespace] = null;
					}
					else
					{
						xmlElement.SetAttribute("xmlns:mstns", dt.Namespace);
						this._prefixes[dt.Namespace] = "mstns";
					}
				}
			}
			this.GenerateConstraintNames(dt, true);
			XmlElement xmlElement2 = this.HandleTable(dt, xd, xmlElement, false);
			xmlElement.AppendChild(xmlElement2);
			this._dsElement.RemoveChild(this._constraintSeparator);
			xmlElement.AppendChild(this._dsElement);
			return xmlElement;
		}

		internal XmlElement FillDataSetElement(XmlDocument xd, DataSet ds, DataTable dt)
		{
			DataSet dataSet = ((ds != null) ? ds : dt.DataSet);
			if (dataSet != null)
			{
				this._dsElement.SetAttribute("name", XmlConvert.EncodeLocalName(dataSet.DataSetName));
				this._dsElement.SetAttribute("IsDataSet", "urn:schemas-microsoft-com:xml-msdata", "true");
				if (ds == null)
				{
					this._dsElement.SetAttribute("MainDataTable", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.EncodeLocalName((dt.Namespace.Length == 0) ? dt.TableName : (dt.Namespace + ":" + dt.TableName)));
				}
				if (dataSet.CaseSensitive)
				{
					this._dsElement.SetAttribute("CaseSensitive", "urn:schemas-microsoft-com:xml-msdata", "true");
				}
				if (dataSet.ShouldSerializeLocale() || !dataSet.Locale.Equals(CultureInfo.CurrentCulture))
				{
					this._dsElement.SetAttribute("Locale", "urn:schemas-microsoft-com:xml-msdata", dataSet.Locale.ToString());
				}
				else
				{
					this._dsElement.SetAttribute("UseCurrentLocale", "urn:schemas-microsoft-com:xml-msdata", "true");
				}
			}
			else if (dt != null)
			{
				this._dsElement.SetAttribute("name", XmlConvert.EncodeLocalName("NewDataSet"));
				this._dsElement.SetAttribute("IsDataSet", "urn:schemas-microsoft-com:xml-msdata", "true");
				this._dsElement.SetAttribute("MainDataTable", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.EncodeLocalName((dt.Namespace.Length == 0) ? dt.TableName : (dt.Namespace + ":" + dt.TableName)));
				if (dt.CaseSensitive)
				{
					this._dsElement.SetAttribute("CaseSensitive", "urn:schemas-microsoft-com:xml-msdata", "true");
				}
				if (dt.ShouldSerializeLocale() || !dt.Locale.Equals(CultureInfo.CurrentCulture))
				{
					this._dsElement.SetAttribute("Locale", "urn:schemas-microsoft-com:xml-msdata", dt.Locale.ToString());
				}
				else
				{
					this._dsElement.SetAttribute("UseCurrentLocale", "urn:schemas-microsoft-com:xml-msdata", "true");
				}
			}
			XmlElement xmlElement = xd.CreateElement("xs", "complexType", "http://www.w3.org/2001/XMLSchema");
			this._dsElement.AppendChild(xmlElement);
			XmlElement xmlElement2 = xd.CreateElement("xs", "choice", "http://www.w3.org/2001/XMLSchema");
			xmlElement2.SetAttribute("minOccurs", "0");
			xmlElement2.SetAttribute("maxOccurs", "unbounded");
			xmlElement.AppendChild(xmlElement2);
			return xmlElement2;
		}

		internal void SetPath(XmlWriter xw)
		{
			DataTextWriter dataTextWriter = xw as DataTextWriter;
			FileStream fileStream = ((dataTextWriter != null) ? (dataTextWriter.BaseStream as FileStream) : null);
			if (fileStream == null)
			{
				XmlTextWriter xmlTextWriter = xw as XmlTextWriter;
				if (xmlTextWriter == null)
				{
					return;
				}
				fileStream = xmlTextWriter.BaseStream as FileStream;
				if (fileStream == null)
				{
					return;
				}
			}
			this._filePath = Path.GetDirectoryName(fileStream.Name);
			this._fileName = Path.GetFileNameWithoutExtension(fileStream.Name);
			this._fileExt = Path.GetExtension(fileStream.Name);
			if (!string.IsNullOrEmpty(this._filePath))
			{
				this._filePath += "\\";
			}
		}

		internal void Save(DataSet ds, XmlWriter xw)
		{
			this.Save(ds, null, xw);
		}

		internal void Save(DataTable dt, XmlWriter xw)
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (this._schFormat == SchemaFormat.Public)
			{
				this.SetPath(xw);
			}
			XmlElement xmlElement = this.SchemaTree(xmlDocument, dt);
			xmlDocument.AppendChild(xmlElement);
			xmlDocument.Save(xw);
		}

		internal void Save(DataSet ds, DataTable dt, XmlWriter xw)
		{
			this.Save(ds, dt, xw, false);
		}

		internal void Save(DataSet ds, DataTable dt, XmlWriter xw, bool writeHierarchy)
		{
			this.Save(ds, dt, xw, writeHierarchy, null);
		}

		internal void Save(DataSet ds, DataTable dt, XmlWriter xw, bool writeHierarchy, Converter<Type, string> multipleTargetConverter)
		{
			this._targetConverter = multipleTargetConverter;
			XmlDocument xmlDocument = new XmlDocument();
			if (this._schFormat == SchemaFormat.Public)
			{
				this.SetPath(xw);
			}
			if (this._schFormat == SchemaFormat.WebServiceSkipSchema && xw.WriteState == WriteState.Element)
			{
				xw.WriteAttributeString("msdata", "SchemaSerializationMode", "urn:schemas-microsoft-com:xml-msdata", "ExcludeSchema");
			}
			this.SchemaTree(xmlDocument, xw, ds, dt, writeHierarchy);
		}

		internal XmlElement HandleRelation(DataRelation rel, XmlDocument dc)
		{
			XmlElement xmlElement = dc.CreateElement("msdata", "Relationship", "urn:schemas-microsoft-com:xml-msdata");
			xmlElement.SetAttribute("name", XmlConvert.EncodeLocalName(rel.RelationName));
			xmlElement.SetAttribute("parent", "urn:schemas-microsoft-com:xml-msdata", rel.ParentKey.Table.EncodedTableName);
			xmlElement.SetAttribute("child", "urn:schemas-microsoft-com:xml-msdata", rel.ChildKey.Table.EncodedTableName);
			if (this._ds == null || this._ds.Tables.InternalIndexOf(rel.ParentKey.Table.TableName) == -3)
			{
				xmlElement.SetAttribute("ParentTableNamespace", "urn:schemas-microsoft-com:xml-msdata", rel.ParentKey.Table.Namespace);
			}
			if (this._ds == null || this._ds.Tables.InternalIndexOf(rel.ChildKey.Table.TableName) == -3)
			{
				xmlElement.SetAttribute("ChildTableNamespace", "urn:schemas-microsoft-com:xml-msdata", rel.ChildKey.Table.Namespace);
			}
			DataColumn[] array = rel.ParentKey.ColumnsReference;
			string text = array[0].EncodedColumnName;
			StringBuilder stringBuilder = null;
			if (1 < array.Length)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.Append(text);
				for (int i = 1; i < array.Length; i++)
				{
					stringBuilder.Append(' ').Append(array[i].EncodedColumnName);
				}
				text = stringBuilder.ToString();
			}
			xmlElement.SetAttribute("parentkey", "urn:schemas-microsoft-com:xml-msdata", text);
			array = rel.ChildKey.ColumnsReference;
			text = array[0].EncodedColumnName;
			if (1 < array.Length)
			{
				if (stringBuilder != null)
				{
					stringBuilder.Length = 0;
				}
				else
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append(text);
				for (int j = 1; j < array.Length; j++)
				{
					stringBuilder.Append(' ').Append(array[j].EncodedColumnName);
				}
				text = stringBuilder.ToString();
			}
			xmlElement.SetAttribute("childkey", "urn:schemas-microsoft-com:xml-msdata", text);
			XmlTreeGen.AddExtendedProperties(rel._extendedProperties, xmlElement);
			return xmlElement;
		}

		private static XmlElement FindSimpleType(XmlElement schema, string name)
		{
			for (XmlNode xmlNode = schema.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode is XmlElement)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					if (xmlElement.GetAttribute("name") == name)
					{
						return xmlElement;
					}
				}
			}
			return null;
		}

		internal XmlElement GetSchema(string NamespaceURI)
		{
			XmlElement xmlElement = (XmlElement)this._namespaces[NamespaceURI];
			if (xmlElement == null)
			{
				xmlElement = this._dc.CreateElement("xs", "schema", "http://www.w3.org/2001/XMLSchema");
				this.WriteSchemaRoot(this._dc, xmlElement, NamespaceURI);
				if (!string.IsNullOrEmpty(NamespaceURI))
				{
					string text = "app";
					int num = this._prefixCount + 1;
					this._prefixCount = num;
					string text2 = text + Convert.ToString(num, CultureInfo.InvariantCulture);
					this._sRoot.SetAttribute("xmlns:" + text2, NamespaceURI);
					xmlElement.SetAttribute("xmlns:" + text2, NamespaceURI);
					this._prefixes[NamespaceURI] = text2;
				}
				this._namespaces[NamespaceURI] = xmlElement;
			}
			return xmlElement;
		}

		internal void HandleColumnType(DataColumn col, XmlDocument dc, XmlElement root, XmlElement schema)
		{
			string text = "type";
			if (col.ColumnMapping == MappingType.SimpleContent)
			{
				text = "base";
			}
			if (col.SimpleType != null)
			{
				for (SimpleType simpleType = col.SimpleType; simpleType != null; simpleType = simpleType.BaseSimpleType)
				{
					string name = simpleType.Name;
					if (name != null && name.Length != 0)
					{
						string text2 = ((this._schFormat != SchemaFormat.Remoting) ? simpleType.Namespace : ((col.Table.DataSet != null) ? col.Table.DataSet.Namespace : col.Table.Namespace));
						XmlElement schema2 = this.GetSchema(text2);
						if (simpleType.BaseSimpleType != null && simpleType.BaseSimpleType.Namespace != null && simpleType.BaseSimpleType.Namespace.Length > 0)
						{
							this.GetSchema(simpleType.BaseSimpleType.Namespace);
						}
						XmlNode xmlNode = simpleType.ToNode(dc, this._prefixes, this._schFormat == SchemaFormat.Remoting);
						if (simpleType == col.SimpleType)
						{
							string text3 = (string)this._prefixes[text2];
							if (text3 != null && text3.Length > 0)
							{
								if (this._schFormat != SchemaFormat.Remoting)
								{
									root.SetAttribute(text, text3 + ":" + name);
								}
								else
								{
									root.SetAttribute(text, name);
								}
							}
							else
							{
								root.SetAttribute(text, name);
							}
						}
						if (XmlTreeGen.FindSimpleType(schema2, name) == null)
						{
							schema2.AppendChild(xmlNode);
						}
					}
					else
					{
						if (simpleType.BaseSimpleType != null && simpleType.BaseSimpleType.Namespace != null && simpleType.BaseSimpleType.Namespace.Length > 0)
						{
							this.GetSchema(simpleType.BaseSimpleType.Namespace);
						}
						XmlNode xmlNode = simpleType.ToNode(dc, this._prefixes, this._schFormat == SchemaFormat.Remoting);
						root.AppendChild(xmlNode);
					}
				}
				return;
			}
			if (col.XmlDataType != null && col.XmlDataType.Length != 0 && XSDSchema.IsXsdType(col.XmlDataType))
			{
				root.SetAttribute(text, XSDSchema.QualifiedName(col.XmlDataType));
				return;
			}
			string text4 = XmlTreeGen.XmlDataTypeName(col.DataType);
			if (text4 == null || text4.Length == 0)
			{
				if (col.DataType == typeof(Guid) || col.DataType == typeof(Type))
				{
					text4 = "string";
				}
				else
				{
					if (col.ColumnMapping == MappingType.Attribute)
					{
						XmlTreeGen.ValidateColumnMapping(col.DataType);
					}
					text4 = "anyType";
				}
			}
			root.SetAttribute(text, XSDSchema.QualifiedName(text4));
		}

		internal void AddColumnProperties(DataColumn col, XmlElement root)
		{
			if (col.DataType != typeof(string))
			{
				string text = XmlTreeGen.XmlDataTypeName(col.DataType);
				if ((col.IsSqlType && (text.Length == 0 || col.ImplementsINullable)) || typeof(SqlXml) == col.DataType || col.DataType == typeof(DateTimeOffset) || col.DataType == typeof(BigInteger))
				{
					root.SetAttribute("DataType", "urn:schemas-microsoft-com:xml-msdata", col.DataType.FullName);
				}
				else if (text.Length == 0 || col.ImplementsINullable || (text == "anyType" && col.XmlDataType != "anyType"))
				{
					this.SetMSDataAttribute(root, col.DataType);
				}
			}
			if (col.ReadOnly)
			{
				root.SetAttribute("ReadOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			if (col.Expression.Length != 0)
			{
				root.SetAttribute("Expression", "urn:schemas-microsoft-com:xml-msdata", col.Expression);
			}
			if (col.AutoIncrement)
			{
				root.SetAttribute("AutoIncrement", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			if (col.AutoIncrementSeed != 0L)
			{
				root.SetAttribute("AutoIncrementSeed", "urn:schemas-microsoft-com:xml-msdata", col.AutoIncrementSeed.ToString(CultureInfo.InvariantCulture));
			}
			if (col.AutoIncrementStep != 1L)
			{
				root.SetAttribute("AutoIncrementStep", "urn:schemas-microsoft-com:xml-msdata", col.AutoIncrementStep.ToString(CultureInfo.InvariantCulture));
			}
			if (col.Caption != col.ColumnName)
			{
				root.SetAttribute("Caption", "urn:schemas-microsoft-com:xml-msdata", col.Caption);
			}
			if (col.Prefix.Length != 0)
			{
				root.SetAttribute("Prefix", "urn:schemas-microsoft-com:xml-msdata", col.Prefix);
			}
			if (col.DataType == typeof(DateTime) && col.DateTimeMode != DataSetDateTime.UnspecifiedLocal)
			{
				root.SetAttribute("DateTimeMode", "urn:schemas-microsoft-com:xml-msdata", col.DateTimeMode.ToString());
			}
		}

		private string FindTargetNamespace(DataTable table)
		{
			string text = (table.TypeName.IsEmpty ? table.Namespace : table.TypeName.Namespace);
			if (string.IsNullOrEmpty(text))
			{
				DataRelation[] nestedParentRelations = table.NestedParentRelations;
				if (nestedParentRelations.Length != 0)
				{
					for (int i = 0; i < nestedParentRelations.Length; i++)
					{
						DataTable parentTable = nestedParentRelations[i].ParentTable;
						if (table != parentTable)
						{
							text = this.FindTargetNamespace(parentTable);
							if (!string.IsNullOrEmpty(text))
							{
								break;
							}
						}
					}
				}
				else
				{
					text = this._ds.Namespace;
				}
			}
			return text;
		}

		internal XmlElement HandleColumn(DataColumn col, XmlDocument dc, XmlElement schema, bool fWriteOrdinal)
		{
			string text = ((col.ColumnMapping != MappingType.Element) ? "attribute" : "element");
			XmlElement xmlElement = dc.CreateElement("xs", text, "http://www.w3.org/2001/XMLSchema");
			xmlElement.SetAttribute("name", col.EncodedColumnName);
			if (col.Namespace.Length == 0)
			{
				DataTable table = col.Table;
				string text2 = this.FindTargetNamespace(table);
				if (col.Namespace != text2)
				{
					xmlElement.SetAttribute("form", "unqualified");
				}
			}
			if (col.GetType() != typeof(DataColumn))
			{
				this.AddXdoProperties(col, xmlElement, dc);
			}
			else
			{
				this.AddColumnProperties(col, xmlElement);
			}
			XmlTreeGen.AddExtendedProperties(col._extendedProperties, xmlElement);
			this.HandleColumnType(col, dc, xmlElement, schema);
			if (col.ColumnMapping == MappingType.Hidden)
			{
				if (!col.AllowDBNull)
				{
					xmlElement.SetAttribute("AllowDBNull", "urn:schemas-microsoft-com:xml-msdata", "false");
				}
				if (!col.DefaultValueIsNull)
				{
					if (col.DataType == typeof(bool))
					{
						xmlElement.SetAttribute("DefaultValue", "urn:schemas-microsoft-com:xml-msdata", ((bool)col.DefaultValue) ? "true" : "false");
					}
					else
					{
						XmlTreeGen.ValidateColumnMapping(col.DataType);
						xmlElement.SetAttribute("DefaultValue", "urn:schemas-microsoft-com:xml-msdata", col.ConvertObjectToXml(col.DefaultValue));
					}
				}
			}
			if (!col.DefaultValueIsNull && col.ColumnMapping != MappingType.Hidden)
			{
				XmlTreeGen.ValidateColumnMapping(col.DataType);
				if (col.ColumnMapping == MappingType.Attribute && !col.AllowDBNull)
				{
					if (col.DataType == typeof(bool))
					{
						xmlElement.SetAttribute("DefaultValue", "urn:schemas-microsoft-com:xml-msdata", ((bool)col.DefaultValue) ? "true" : "false");
					}
					else
					{
						xmlElement.SetAttribute("DefaultValue", "urn:schemas-microsoft-com:xml-msdata", col.ConvertObjectToXml(col.DefaultValue));
					}
				}
				else if (col.DataType == typeof(bool))
				{
					xmlElement.SetAttribute("default", ((bool)col.DefaultValue) ? "true" : "false");
				}
				else if (!col.IsCustomType)
				{
					xmlElement.SetAttribute("default", col.ConvertObjectToXml(col.DefaultValue));
				}
			}
			if (this._schFormat == SchemaFormat.Remoting)
			{
				xmlElement.SetAttribute("targetNamespace", "urn:schemas-microsoft-com:xml-msdata", col.Namespace);
			}
			else if (col.Namespace != (col.Table.TypeName.IsEmpty ? col.Table.Namespace : col.Table.TypeName.Namespace) && col.Namespace.Length != 0)
			{
				XmlElement schema2 = this.GetSchema(col.Namespace);
				if (this.FindTypeNode(schema2, col.EncodedColumnName) == null)
				{
					schema2.AppendChild(xmlElement);
				}
				xmlElement = this._dc.CreateElement("xs", text, "http://www.w3.org/2001/XMLSchema");
				xmlElement.SetAttribute("ref", this._prefixes[col.Namespace] + ":" + col.EncodedColumnName);
				if (col.Table.Namespace != this._ds.Namespace)
				{
					string text3 = (string)this._prefixes[col.Namespace];
					this.GetSchema(col.Table.Namespace);
				}
			}
			int num = (col.AllowDBNull ? 0 : 1);
			if (col.ColumnMapping == MappingType.Attribute && num != 0)
			{
				xmlElement.SetAttribute("use", "required");
			}
			if (col.ColumnMapping == MappingType.Hidden)
			{
				xmlElement.SetAttribute("use", "prohibited");
			}
			else if (col.ColumnMapping != MappingType.Attribute && num != 1)
			{
				xmlElement.SetAttribute("minOccurs", num.ToString(CultureInfo.InvariantCulture));
			}
			if (col.ColumnMapping == MappingType.Element && fWriteOrdinal)
			{
				xmlElement.SetAttribute("Ordinal", "urn:schemas-microsoft-com:xml-msdata", col.Ordinal.ToString(CultureInfo.InvariantCulture));
			}
			return xmlElement;
		}

		internal static string TranslateAcceptRejectRule(AcceptRejectRule rule)
		{
			if (rule == AcceptRejectRule.None)
			{
				return "None";
			}
			if (rule == AcceptRejectRule.Cascade)
			{
				return "Cascade";
			}
			return null;
		}

		internal static string TranslateRule(Rule rule)
		{
			switch (rule)
			{
			case Rule.None:
				return "None";
			case Rule.Cascade:
				return "Cascade";
			case Rule.SetNull:
				return "SetNull";
			case Rule.SetDefault:
				return "SetDefault";
			default:
				return null;
			}
		}

		internal void AppendChildWithoutRef(XmlElement node, string Namespace, XmlElement el, string refString)
		{
			XmlElement schema = this.GetSchema(Namespace);
			if (this.FindTypeNode(schema, el.GetAttribute("name")) == null)
			{
				schema.AppendChild(el);
			}
		}

		internal XmlElement FindTypeNode(XmlElement node, string strType)
		{
			if (node == null)
			{
				return null;
			}
			for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode is XmlElement)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					if ((XMLSchema.FEqualIdentity(xmlElement, "element", "http://www.w3.org/2001/XMLSchema") || XMLSchema.FEqualIdentity(xmlElement, "attribute", "http://www.w3.org/2001/XMLSchema") || XMLSchema.FEqualIdentity(xmlElement, "complexType", "http://www.w3.org/2001/XMLSchema") || XMLSchema.FEqualIdentity(xmlElement, "simpleType", "http://www.w3.org/2001/XMLSchema")) && xmlElement.GetAttribute("name") == strType)
					{
						return xmlElement;
					}
				}
			}
			return null;
		}

		internal XmlElement HandleTable(DataTable table, XmlDocument dc, XmlElement schema)
		{
			return this.HandleTable(table, dc, schema, true);
		}

		private bool HasMixedColumns(DataTable table)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (object obj in table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (!flag2 && dataColumn.ColumnMapping == MappingType.Element)
				{
					flag2 = true;
				}
				if (!flag && (dataColumn.ColumnMapping == MappingType.Attribute || dataColumn.ColumnMapping == MappingType.Hidden))
				{
					flag = !XmlTreeGen.AutoGenerated(dataColumn);
				}
				if (flag && flag2)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool AutoGenerated(DataColumn col)
		{
			if (col.ColumnMapping != MappingType.Hidden)
			{
				return false;
			}
			if (col.DataType != typeof(int))
			{
				return false;
			}
			string text = col.Table.TableName + "_Id";
			if (col.ColumnName == text || col.ColumnName == text + "_0")
			{
				return true;
			}
			text = string.Empty;
			foreach (object obj in col.Table.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Nested && dataRelation.ChildColumnsReference.Length == 1 && dataRelation.ChildColumnsReference[0] == col && dataRelation.ParentColumnsReference.Length == 1)
				{
					text = dataRelation.ParentColumnsReference[0].Table.TableName + "_Id";
				}
			}
			return col.ColumnName == text || col.ColumnName == text + "_0";
		}

		internal static bool AutoGenerated(DataRelation rel)
		{
			string text = rel.ParentTable.TableName + "_" + rel.ChildTable.TableName;
			return rel.RelationName.StartsWith(text, StringComparison.Ordinal) && rel.ExtendedProperties.Count <= 0;
		}

		internal static bool AutoGenerated(UniqueConstraint unique)
		{
			return unique.ConstraintName.StartsWith("Constraint", StringComparison.Ordinal) && unique.Key.ColumnsReference.Length == 1 && unique.ExtendedProperties.Count <= 0 && XmlTreeGen.AutoGenerated(unique.Key.ColumnsReference[0]);
		}

		private bool AutoGenerated(ForeignKeyConstraint fk)
		{
			return XmlTreeGen.AutoGenerated(fk, true);
		}

		internal static bool AutoGenerated(ForeignKeyConstraint fk, bool checkRelation)
		{
			DataRelation dataRelation = fk.FindParentRelation();
			if (checkRelation)
			{
				if (dataRelation == null)
				{
					return false;
				}
				if (!XmlTreeGen.AutoGenerated(dataRelation))
				{
					return false;
				}
				if (dataRelation.RelationName != fk.ConstraintName)
				{
					return false;
				}
			}
			return fk.ExtendedProperties.Count <= 0 && fk.AcceptRejectRule == AcceptRejectRule.None && fk.DeleteRule == Rule.Cascade && fk.DeleteRule == Rule.Cascade && fk.RelatedColumnsReference.Length == 1 && XmlTreeGen.AutoGenerated(fk.RelatedColumnsReference[0]);
		}

		private bool IsAutoGenerated(object o)
		{
			return this._schFormat != SchemaFormat.Remoting && this._autogenerated[o] != null;
		}

		internal XmlElement HandleTable(DataTable table, XmlDocument dc, XmlElement schema, bool genNested)
		{
			XmlElement xmlElement = dc.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			bool flag = false;
			if ((table.DataSet == null || (this._ds != null && table.Namespace != this._ds.Namespace)) && this._schFormat == SchemaFormat.Remoting)
			{
				xmlElement.SetAttribute("targetNamespace", "urn:schemas-microsoft-com:xml-msdata", table.Namespace);
			}
			xmlElement.SetAttribute("name", table.EncodedTableName);
			if (table.Namespace.Length == 0)
			{
				DataTable dataTable = table;
				string text = dataTable.Namespace;
				while (string.IsNullOrEmpty(text))
				{
					DataRelation[] nestedParentRelations = dataTable.NestedParentRelations;
					if (nestedParentRelations.Length == 0)
					{
						text = ((this._ds != null) ? this._ds.Namespace : "");
						break;
					}
					int num = -1;
					for (int i = 0; i < nestedParentRelations.Length; i++)
					{
						if (nestedParentRelations[i].ParentTable != dataTable)
						{
							num = i;
							break;
						}
					}
					if (num == -1)
					{
						break;
					}
					dataTable = nestedParentRelations[num].ParentTable;
					text = dataTable.Namespace;
				}
				if (table.Namespace != text)
				{
					xmlElement.SetAttribute("form", "unqualified");
					flag = true;
				}
			}
			if (table.ShouldSerializeCaseSensitive())
			{
				xmlElement.SetAttribute("CaseSensitive", "urn:schemas-microsoft-com:xml-msdata", table.CaseSensitive.ToString());
			}
			if (table.ShouldSerializeLocale())
			{
				xmlElement.SetAttribute("Locale", "urn:schemas-microsoft-com:xml-msdata", table.Locale.ToString());
			}
			this.AddXdoProperties(table, xmlElement, dc);
			DataColumnCollection columns = table.Columns;
			int count = columns.Count;
			int num2 = 0;
			if (count == 1 || count == 2)
			{
				for (int j = 0; j < count; j++)
				{
					DataColumn dataColumn = columns[j];
					if (dataColumn.ColumnMapping == MappingType.Hidden)
					{
						DataRelationCollection childRelations = table.ChildRelations;
						for (int k = 0; k < childRelations.Count; k++)
						{
							if (childRelations[k].Nested && childRelations[k].ParentKey.ColumnsReference.Length == 1 && childRelations[k].ParentKey.ColumnsReference[0] == dataColumn)
							{
								num2++;
							}
						}
					}
					if (dataColumn.ColumnMapping == MappingType.Element)
					{
						num2++;
					}
				}
			}
			if (table._repeatableElement && num2 == 1)
			{
				string text2 = XmlTreeGen.XmlDataTypeName(table.Columns[0].DataType);
				if (text2 == null || text2.Length == 0)
				{
					text2 = "anyType";
				}
				xmlElement.SetAttribute("type", XSDSchema.QualifiedName(text2));
				return xmlElement;
			}
			XmlElement xmlElement2 = dc.CreateElement("xs", "complexType", "http://www.w3.org/2001/XMLSchema");
			if (!table.TypeName.IsEmpty && this._schFormat != SchemaFormat.Remoting)
			{
				XmlElement xmlElement3 = this.GetSchema(table.TypeName.Namespace);
				if (string.IsNullOrEmpty(table.TypeName.Namespace))
				{
					if (this._ds == null)
					{
						xmlElement3 = this.GetSchema(table.Namespace);
					}
					else
					{
						xmlElement3 = (flag ? this.GetSchema(this._ds.Namespace) : this.GetSchema(table.Namespace));
					}
				}
				if (this.FindTypeNode(xmlElement3, table.TypeName.Name) == null)
				{
					xmlElement3.AppendChild(xmlElement2);
				}
				xmlElement2.SetAttribute("name", table.TypeName.Name);
			}
			else
			{
				xmlElement.AppendChild(xmlElement2);
			}
			if (!table.TypeName.IsEmpty && this._schFormat != SchemaFormat.Remoting)
			{
				xmlElement.SetAttribute("type", NewDiffgramGen.QualifiedName((string)this._prefixes[table.TypeName.Namespace], table.TypeName.Name));
			}
			DataColumn xmlText = table.XmlText;
			if (xmlText != null)
			{
				XmlElement xmlElement4 = dc.CreateElement("xs", "simpleContent", "http://www.w3.org/2001/XMLSchema");
				if (xmlText.GetType() != typeof(DataColumn))
				{
					this.AddXdoProperties(xmlText, xmlElement4, dc);
				}
				else
				{
					this.AddColumnProperties(xmlText, xmlElement4);
				}
				XmlTreeGen.AddExtendedProperties(xmlText._extendedProperties, xmlElement4);
				if (xmlText.AllowDBNull)
				{
					xmlElement.SetAttribute("nillable", string.Empty, "true");
				}
				if (!xmlText.DefaultValueIsNull)
				{
					XmlTreeGen.ValidateColumnMapping(xmlText.DataType);
					xmlElement4.SetAttribute("DefaultValue", "urn:schemas-microsoft-com:xml-msdata", xmlText.ConvertObjectToXml(xmlText.DefaultValue));
				}
				xmlElement4.SetAttribute("ColumnName", "urn:schemas-microsoft-com:xml-msdata", xmlText.ColumnName);
				xmlElement4.SetAttribute("Ordinal", "urn:schemas-microsoft-com:xml-msdata", xmlText.Ordinal.ToString(CultureInfo.InvariantCulture));
				xmlElement2.AppendChild(xmlElement4);
				XmlElement xmlElement5 = dc.CreateElement("xs", "extension", "http://www.w3.org/2001/XMLSchema");
				xmlElement4.AppendChild(xmlElement5);
				this.HandleColumnType(xmlText, dc, xmlElement5, schema);
				xmlElement2 = xmlElement5;
			}
			XmlElement xmlElement6 = dc.CreateElement("xs", "sequence", "http://www.w3.org/2001/XMLSchema");
			xmlElement2.AppendChild(xmlElement6);
			bool flag2 = this.HasMixedColumns(table);
			for (int l = 0; l < count; l++)
			{
				DataColumn dataColumn2 = columns[l];
				if (dataColumn2.ColumnMapping != MappingType.SimpleContent && (dataColumn2.ColumnMapping == MappingType.Attribute || dataColumn2.ColumnMapping == MappingType.Element || dataColumn2.ColumnMapping == MappingType.Hidden) && !this.IsAutoGenerated(dataColumn2))
				{
					bool flag3 = dataColumn2.ColumnMapping != MappingType.Element;
					XmlElement xmlElement7 = this.HandleColumn(dataColumn2, dc, schema, flag2);
					(flag3 ? xmlElement2 : xmlElement6).AppendChild(xmlElement7);
				}
			}
			if (table.XmlText == null && genNested)
			{
				DataRelationCollection childRelations2 = table.ChildRelations;
				for (int m = 0; m < childRelations2.Count; m++)
				{
					if (childRelations2[m].Nested)
					{
						DataTable childTable = childRelations2[m].ChildTable;
						XmlElement xmlElement8;
						if (childTable == table)
						{
							xmlElement8 = dc.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
							xmlElement8.SetAttribute("ref", table.EncodedTableName);
						}
						else if (childTable.NestedParentsCount > 1)
						{
							xmlElement8 = dc.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
							xmlElement8.SetAttribute("ref", childTable.EncodedTableName);
						}
						else
						{
							xmlElement8 = this.HandleTable(childTable, dc, schema);
						}
						if (childTable.Namespace == table.Namespace)
						{
							xmlElement8.SetAttribute("minOccurs", "0");
							xmlElement8.SetAttribute("maxOccurs", "unbounded");
						}
						if (childTable.Namespace == table.Namespace || childTable.Namespace.Length == 0 || this._schFormat == SchemaFormat.Remoting)
						{
							xmlElement6.AppendChild(xmlElement8);
						}
						else
						{
							if (childTable.NestedParentsCount <= 1)
							{
								this.GetSchema(childTable.Namespace).AppendChild(xmlElement8);
							}
							xmlElement8 = dc.CreateElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
							xmlElement8.SetAttribute("ref", (string)this._prefixes[childTable.Namespace] + ":" + childTable.EncodedTableName);
							xmlElement6.AppendChild(xmlElement8);
						}
						if (childRelations2[m].ChildKeyConstraint == null)
						{
							XmlElement xmlElement9 = this._dc.CreateElement("xs", "annotation", "http://www.w3.org/2001/XMLSchema");
							xmlElement8.PrependChild(xmlElement9);
							XmlElement xmlElement10 = this._dc.CreateElement("xs", "appinfo", "http://www.w3.org/2001/XMLSchema");
							xmlElement9.AppendChild(xmlElement10);
							xmlElement10.AppendChild(this.HandleRelation(childRelations2[m], dc));
						}
					}
				}
			}
			if (xmlElement6 != null && !xmlElement6.HasChildNodes)
			{
				xmlElement2.RemoveChild(xmlElement6);
			}
			ConstraintCollection constraints = table.Constraints;
			string text3 = ((this._ds != null) ? ((this._ds.Namespace.Length != 0) ? "mstns:" : string.Empty) : string.Empty);
			if (this._schFormat != SchemaFormat.Remoting)
			{
				this.GetSchema(table.Namespace);
				text3 = ((table.Namespace.Length != 0) ? ((string)this._prefixes[table.Namespace] + ":") : string.Empty);
			}
			for (int n = 0; n < constraints.Count; n++)
			{
				if (constraints[n] is UniqueConstraint)
				{
					UniqueConstraint uniqueConstraint = (UniqueConstraint)constraints[n];
					if (!this.IsAutoGenerated(uniqueConstraint))
					{
						DataColumn[] array = uniqueConstraint.Key.ColumnsReference;
						XmlElement xmlElement11 = dc.CreateElement("xs", "unique", "http://www.w3.org/2001/XMLSchema");
						if (this._ds == null || this._ds.Tables.InternalIndexOf(table.TableName) == -3)
						{
							xmlElement11.SetAttribute("TableNamespace", "urn:schemas-microsoft-com:xml-msdata", table.Namespace);
						}
						xmlElement11.SetAttribute("name", XmlConvert.EncodeLocalName(uniqueConstraint.SchemaName));
						if (uniqueConstraint.ConstraintName != uniqueConstraint.SchemaName)
						{
							xmlElement11.SetAttribute("ConstraintName", "urn:schemas-microsoft-com:xml-msdata", uniqueConstraint.ConstraintName);
						}
						XmlTreeGen.AddExtendedProperties(uniqueConstraint._extendedProperties, xmlElement11);
						XmlElement xmlElement12 = dc.CreateElement("xs", "selector", "http://www.w3.org/2001/XMLSchema");
						xmlElement12.SetAttribute("xpath", ".//" + text3 + table.EncodedTableName);
						xmlElement11.AppendChild(xmlElement12);
						if (uniqueConstraint.IsPrimaryKey)
						{
							xmlElement11.SetAttribute("PrimaryKey", "urn:schemas-microsoft-com:xml-msdata", "true");
						}
						if (array.Length != 0)
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int num3 = 0; num3 < array.Length; num3++)
							{
								stringBuilder.Length = 0;
								if (this._schFormat != SchemaFormat.Remoting)
								{
									this.GetSchema(array[num3].Namespace);
									if (!string.IsNullOrEmpty(array[num3].Namespace))
									{
										stringBuilder.Append(this._prefixes[array[num3].Namespace]).Append(':');
									}
									stringBuilder.Append(array[num3].EncodedColumnName);
								}
								else
								{
									stringBuilder.Append(text3).Append(array[num3].EncodedColumnName);
								}
								if (array[num3].ColumnMapping == MappingType.Attribute || array[num3].ColumnMapping == MappingType.Hidden)
								{
									stringBuilder.Insert(0, '@');
								}
								XmlElement xmlElement13 = dc.CreateElement("xs", "field", "http://www.w3.org/2001/XMLSchema");
								xmlElement13.SetAttribute("xpath", stringBuilder.ToString());
								xmlElement11.AppendChild(xmlElement13);
							}
						}
						this._dsElement.InsertBefore(xmlElement11, this._constraintSeparator);
					}
				}
				else if (constraints[n] is ForeignKeyConstraint && genNested)
				{
					ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraints[n];
					if ((this._tables.Count <= 0 || (this._tables.Contains(foreignKeyConstraint.RelatedTable) && this._tables.Contains(foreignKeyConstraint.Table))) && !this.IsAutoGenerated(foreignKeyConstraint))
					{
						DataRelation dataRelation = foreignKeyConstraint.FindParentRelation();
						DataColumn[] array = foreignKeyConstraint.RelatedColumnsReference;
						UniqueConstraint uniqueConstraint2 = (UniqueConstraint)foreignKeyConstraint.RelatedTable.Constraints.FindConstraint(new UniqueConstraint("TEMP", array));
						XmlElement xmlElement11;
						XmlElement xmlElement12;
						if (uniqueConstraint2 == null)
						{
							xmlElement11 = dc.CreateElement("xs", "key", "http://www.w3.org/2001/XMLSchema");
							xmlElement11.SetAttribute("name", XmlConvert.EncodeLocalName(foreignKeyConstraint.SchemaName));
							if (this._ds == null || this._ds.Tables.InternalIndexOf(table.TableName) == -3)
							{
								xmlElement11.SetAttribute("TableNamespace", "urn:schemas-microsoft-com:xml-msdata", table.Namespace);
							}
							xmlElement12 = dc.CreateElement("xs", "selector", "http://www.w3.org/2001/XMLSchema");
							xmlElement12.SetAttribute("xpath", ".//" + text3 + foreignKeyConstraint.RelatedTable.EncodedTableName);
							xmlElement11.AppendChild(xmlElement12);
							if (array.Length != 0)
							{
								StringBuilder stringBuilder2 = new StringBuilder();
								for (int num4 = 0; num4 < array.Length; num4++)
								{
									stringBuilder2.Length = 0;
									if (this._schFormat != SchemaFormat.Remoting)
									{
										this.GetSchema(array[num4].Namespace);
										if (!string.IsNullOrEmpty(array[num4].Namespace))
										{
											stringBuilder2.Append(this._prefixes[array[num4].Namespace]).Append(':');
										}
										stringBuilder2.Append(array[num4].EncodedColumnName);
									}
									else
									{
										stringBuilder2.Append(text3).Append(array[num4].EncodedColumnName);
									}
									if (array[num4].ColumnMapping == MappingType.Attribute || array[num4].ColumnMapping == MappingType.Hidden)
									{
										stringBuilder2.Insert(0, '@');
									}
									XmlElement xmlElement13 = dc.CreateElement("xs", "field", "http://www.w3.org/2001/XMLSchema");
									xmlElement13.SetAttribute("xpath", stringBuilder2.ToString());
									xmlElement11.AppendChild(xmlElement13);
								}
							}
							this._dsElement.InsertBefore(xmlElement11, this._constraintSeparator);
						}
						xmlElement11 = dc.CreateElement("xs", "keyref", "http://www.w3.org/2001/XMLSchema");
						xmlElement11.SetAttribute("name", XmlConvert.EncodeLocalName(foreignKeyConstraint.SchemaName));
						if (this._ds == null || this._ds.Tables.InternalIndexOf(foreignKeyConstraint.RelatedTable.TableName) == -3)
						{
							xmlElement11.SetAttribute("TableNamespace", "urn:schemas-microsoft-com:xml-msdata", foreignKeyConstraint.Table.Namespace);
						}
						if (uniqueConstraint2 == null)
						{
							xmlElement11.SetAttribute("refer", XmlConvert.EncodeLocalName(foreignKeyConstraint.SchemaName));
						}
						else
						{
							xmlElement11.SetAttribute("refer", XmlConvert.EncodeLocalName(uniqueConstraint2.SchemaName));
						}
						XmlTreeGen.AddExtendedProperties(foreignKeyConstraint._extendedProperties, xmlElement11, typeof(ForeignKeyConstraint));
						if (foreignKeyConstraint.ConstraintName != foreignKeyConstraint.SchemaName)
						{
							xmlElement11.SetAttribute("ConstraintName", "urn:schemas-microsoft-com:xml-msdata", foreignKeyConstraint.ConstraintName);
						}
						if (dataRelation == null)
						{
							xmlElement11.SetAttribute("ConstraintOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
						}
						else
						{
							if (dataRelation.Nested)
							{
								xmlElement11.SetAttribute("IsNested", "urn:schemas-microsoft-com:xml-msdata", "true");
							}
							XmlTreeGen.AddExtendedProperties(dataRelation._extendedProperties, xmlElement11, typeof(DataRelation));
							if (foreignKeyConstraint.ConstraintName != dataRelation.RelationName)
							{
								xmlElement11.SetAttribute("RelationName", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.EncodeLocalName(dataRelation.RelationName));
							}
						}
						xmlElement12 = dc.CreateElement("xs", "selector", "http://www.w3.org/2001/XMLSchema");
						xmlElement12.SetAttribute("xpath", ".//" + text3 + table.EncodedTableName);
						xmlElement11.AppendChild(xmlElement12);
						if (foreignKeyConstraint.AcceptRejectRule != AcceptRejectRule.None)
						{
							xmlElement11.SetAttribute("AcceptRejectRule", "urn:schemas-microsoft-com:xml-msdata", XmlTreeGen.TranslateAcceptRejectRule(foreignKeyConstraint.AcceptRejectRule));
						}
						if (foreignKeyConstraint.UpdateRule != Rule.Cascade)
						{
							xmlElement11.SetAttribute("UpdateRule", "urn:schemas-microsoft-com:xml-msdata", XmlTreeGen.TranslateRule(foreignKeyConstraint.UpdateRule));
						}
						if (foreignKeyConstraint.DeleteRule != Rule.Cascade)
						{
							xmlElement11.SetAttribute("DeleteRule", "urn:schemas-microsoft-com:xml-msdata", XmlTreeGen.TranslateRule(foreignKeyConstraint.DeleteRule));
						}
						array = foreignKeyConstraint.Columns;
						if (array.Length != 0)
						{
							StringBuilder stringBuilder3 = new StringBuilder();
							for (int num5 = 0; num5 < array.Length; num5++)
							{
								stringBuilder3.Length = 0;
								if (this._schFormat != SchemaFormat.Remoting)
								{
									this.GetSchema(array[num5].Namespace);
									if (!string.IsNullOrEmpty(array[num5].Namespace))
									{
										stringBuilder3.Append(this._prefixes[array[num5].Namespace]).Append(':');
									}
									stringBuilder3.Append(array[num5].EncodedColumnName);
								}
								else
								{
									stringBuilder3.Append(text3).Append(array[num5].EncodedColumnName);
								}
								if (array[num5].ColumnMapping == MappingType.Attribute || array[num5].ColumnMapping == MappingType.Hidden)
								{
									stringBuilder3.Insert(0, '@');
								}
								XmlElement xmlElement13 = dc.CreateElement("xs", "field", "http://www.w3.org/2001/XMLSchema");
								xmlElement13.SetAttribute("xpath", stringBuilder3.ToString());
								xmlElement11.AppendChild(xmlElement13);
							}
						}
						this._dsElement.InsertAfter(xmlElement11, this._constraintSeparator);
					}
				}
			}
			XmlTreeGen.AddExtendedProperties(table._extendedProperties, xmlElement);
			return xmlElement;
		}

		private void SetMSDataAttribute(XmlElement root, Type type)
		{
			string text = DataStorage.GetQualifiedName(type);
			try
			{
				if (this._targetConverter != null)
				{
					text = this._targetConverter(type);
				}
				if (!string.IsNullOrEmpty(text))
				{
					root.SetAttribute("DataType", "urn:schemas-microsoft-com:xml-msdata", text);
				}
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				ExceptionBuilder.ThrowMultipleTargetConverter(ex);
			}
			if (string.IsNullOrEmpty(text))
			{
				ExceptionBuilder.ThrowMultipleTargetConverter(null);
			}
		}

		private ArrayList _constraintNames;

		private Hashtable _namespaces;

		private Hashtable _autogenerated;

		private Hashtable _prefixes;

		private DataSet _ds;

		private ArrayList _tables = new ArrayList();

		private ArrayList _relations = new ArrayList();

		private XmlDocument _dc;

		private XmlElement _sRoot;

		private int _prefixCount;

		private SchemaFormat _schFormat = SchemaFormat.Public;

		private string _filePath;

		private string _fileName;

		private string _fileExt;

		private XmlElement _dsElement;

		private XmlElement _constraintSeparator;

		private Converter<Type, string> _targetConverter;
	}
}
