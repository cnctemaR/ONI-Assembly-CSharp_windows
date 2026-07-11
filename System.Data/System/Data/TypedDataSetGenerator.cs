using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data
{
	[Obsolete("TypedDataSetGenerator class will be removed in a future release. Please use System.Data.Design.TypedDataSetGenerator in System.Design.dll.")]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true, Synchronization = true)]
	public class TypedDataSetGenerator
	{
		public static void Generate(DataSet dataSet, CodeNamespace codeNamespace, ICodeGenerator codeGen)
		{
			new TypedDataSetGenerator().GenerateCode(dataSet, codeNamespace, codeGen);
			CodeGenerator.ValidateIdentifiers(codeNamespace);
		}

		public static string GenerateIdName(string name, ICodeGenerator codeGen)
		{
			if (codeGen.IsValidIdentifier(name))
			{
				return name;
			}
			string text = name.Replace(' ', '_');
			if (!codeGen.IsValidIdentifier(text))
			{
				text = "_" + text;
				for (int i = 1; i < text.Length; i++)
				{
					UnicodeCategory unicodeCategory = char.GetUnicodeCategory(text[i]);
					if (unicodeCategory != UnicodeCategory.UppercaseLetter && UnicodeCategory.LowercaseLetter != unicodeCategory && UnicodeCategory.TitlecaseLetter != unicodeCategory && UnicodeCategory.ModifierLetter != unicodeCategory && UnicodeCategory.OtherLetter != unicodeCategory && UnicodeCategory.LetterNumber != unicodeCategory && UnicodeCategory.NonSpacingMark != unicodeCategory && UnicodeCategory.SpacingCombiningMark != unicodeCategory && UnicodeCategory.DecimalDigitNumber != unicodeCategory && UnicodeCategory.ConnectorPunctuation != unicodeCategory)
					{
						text = text.Replace(text[i], '_');
					}
				}
			}
			return text;
		}

		internal CodeTypeDeclaration GenerateCode(DataSet dataSet, CodeNamespace codeNamespace, ICodeGenerator codeGen)
		{
			this.useExtendedNaming = false;
			this.errorList = new ArrayList();
			this.conflictingTables = new ArrayList();
			this.codeGen = codeGen;
			CodeTypeDeclaration codeTypeDeclaration = this.CreateTypedDataSet(dataSet);
			foreach (object obj in dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				codeTypeDeclaration.Members.Add(this.CreateTypedRowEventHandler(dataTable));
			}
			foreach (object obj2 in dataSet.Tables)
			{
				DataTable dataTable2 = (DataTable)obj2;
				codeTypeDeclaration.Members.Add(this.CreateTypedTable(dataTable2));
				codeTypeDeclaration.Members.Add(this.CreateTypedRow(dataTable2));
				codeTypeDeclaration.Members.Add(this.CreateTypedRowEvent(dataTable2));
			}
			if (this.errorList.Count > 0)
			{
				throw new TypedDataSetGeneratorException(this.errorList);
			}
			codeNamespace.Types.Add(codeTypeDeclaration);
			return codeTypeDeclaration;
		}

		private void InitLookupIdentifiers()
		{
			this.lookupIdentifiers = new Hashtable();
			foreach (PropertyInfo propertyInfo in typeof(DataRow).GetProperties())
			{
				this.lookupIdentifiers[propertyInfo.Name] = "_" + propertyInfo.Name;
			}
		}

		private string FixIdName(string inVarName)
		{
			if (this.lookupIdentifiers == null)
			{
				this.InitLookupIdentifiers();
			}
			string text = (string)this.lookupIdentifiers[inVarName];
			if (text == null)
			{
				text = TypedDataSetGenerator.GenerateIdName(inVarName, this.codeGen);
				while (this.lookupIdentifiers.ContainsValue(text))
				{
					text = "_" + text;
				}
				this.lookupIdentifiers[inVarName] = text;
				if (!this.codeGen.IsValidIdentifier(text))
				{
					this.errorList.Add(Res.GetString("Cannot generate identifier for name '{0}'", new object[] { text }));
				}
			}
			return text;
		}

		private static bool isEmpty(string s)
		{
			return s == null || s.Length == 0;
		}

		private string RowClassName(DataTable table)
		{
			string text = (string)table.ExtendedProperties["typedName"];
			if (TypedDataSetGenerator.isEmpty(text))
			{
				text = this.FixIdName(table.TableName) + "Row";
			}
			return text;
		}

		private string RowBaseClassName(DataTable table)
		{
			if (this.useExtendedNaming)
			{
				string text = (string)table.ExtendedProperties["typedBaseClass"];
				if (TypedDataSetGenerator.isEmpty(text))
				{
					text = (string)table.DataSet.ExtendedProperties["typedBaseClass"];
					if (TypedDataSetGenerator.isEmpty(text))
					{
						text = "DataRow";
					}
				}
				return text;
			}
			return "DataRow";
		}

		private string RowConcreteClassName(DataTable table)
		{
			if (this.useExtendedNaming)
			{
				string text = (string)table.ExtendedProperties["typedConcreteClass"];
				if (TypedDataSetGenerator.isEmpty(text))
				{
					text = this.RowClassName(table);
				}
				return text;
			}
			return this.RowClassName(table);
		}

		private string TableClassName(DataTable table)
		{
			string text = (string)table.ExtendedProperties["typedPlural"];
			if (TypedDataSetGenerator.isEmpty(text))
			{
				text = (string)table.ExtendedProperties["typedName"];
				if (TypedDataSetGenerator.isEmpty(text))
				{
					if (table.DataSet.Tables.InternalIndexOf(table.TableName) == -3 && !this.conflictingTables.Contains(table.TableName))
					{
						this.conflictingTables.Add(table.TableName);
						this.errorList.Add(Res.GetString("There is more than one table with the same name '{0}' (even if namespace is different)", new object[] { table.TableName }));
					}
					text = this.FixIdName(table.TableName);
				}
			}
			return text + "DataTable";
		}

		private string TablePropertyName(DataTable table)
		{
			string text = (string)table.ExtendedProperties["typedPlural"];
			if (TypedDataSetGenerator.isEmpty(text))
			{
				text = (string)table.ExtendedProperties["typedName"];
				if (TypedDataSetGenerator.isEmpty(text))
				{
					text = this.FixIdName(table.TableName);
				}
				else
				{
					text += "Table";
				}
			}
			return text;
		}

		private string TableFieldName(DataTable table)
		{
			return "table" + this.TablePropertyName(table);
		}

		private string RowColumnPropertyName(DataColumn column)
		{
			string text = (string)column.ExtendedProperties["typedName"];
			if (TypedDataSetGenerator.isEmpty(text))
			{
				text = this.FixIdName(column.ColumnName);
			}
			return text;
		}

		private string TableColumnFieldName(DataColumn column)
		{
			string text = this.RowColumnPropertyName(column);
			if (string.Compare("column", text, StringComparison.OrdinalIgnoreCase) != 0)
			{
				return "column" + text;
			}
			return "columnField" + text;
		}

		private string TableColumnPropertyName(DataColumn column)
		{
			return this.RowColumnPropertyName(column) + "Column";
		}

		private static int TablesConnectedness(DataTable parentTable, DataTable childTable)
		{
			int num = 0;
			DataRelationCollection parentRelations = childTable.ParentRelations;
			for (int i = 0; i < parentRelations.Count; i++)
			{
				if (parentRelations[i].ParentTable == parentTable)
				{
					num++;
				}
			}
			return num;
		}

		private string ChildPropertyName(DataRelation relation)
		{
			string text = (string)relation.ExtendedProperties["typedChildren"];
			if (TypedDataSetGenerator.isEmpty(text))
			{
				string text2 = (string)relation.ChildTable.ExtendedProperties["typedPlural"];
				if (TypedDataSetGenerator.isEmpty(text2))
				{
					text2 = (string)relation.ChildTable.ExtendedProperties["typedName"];
					if (TypedDataSetGenerator.isEmpty(text2))
					{
						text = "Get" + relation.ChildTable.TableName + "Rows";
						if (1 < TypedDataSetGenerator.TablesConnectedness(relation.ParentTable, relation.ChildTable))
						{
							text = text + "By" + relation.RelationName;
						}
						return this.FixIdName(text);
					}
					text2 += "Rows";
				}
				text = "Get" + text2;
			}
			return text;
		}

		private string ParentPropertyName(DataRelation relation)
		{
			string text = (string)relation.ExtendedProperties["typedParent"];
			if (TypedDataSetGenerator.isEmpty(text))
			{
				text = this.RowClassName(relation.ParentTable);
				if (relation.ChildTable == relation.ParentTable || relation.ChildColumnsReference.Length != 1)
				{
					text += "Parent";
				}
				if (1 < TypedDataSetGenerator.TablesConnectedness(relation.ParentTable, relation.ChildTable))
				{
					text = text + "By" + this.FixIdName(relation.RelationName);
				}
			}
			return text;
		}

		private string RelationFieldName(DataRelation relation)
		{
			return this.FixIdName("relation" + relation.RelationName);
		}

		private string GetTypeName(Type t)
		{
			return t.FullName;
		}

		private bool ChildRelationFollowable(DataRelation relation)
		{
			return relation != null && (relation.ChildTable != relation.ParentTable || relation.ChildTable.Columns.Count != 1);
		}

		private static CodeMemberMethod CreateOnRowEventMethod(string eventName, string rowClassName)
		{
			CodeMemberMethod codeMemberMethod = TypedDataSetGenerator.MethodDecl(typeof(void), "OnRow" + eventName, (MemberAttributes)12292);
			codeMemberMethod.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(DataRowChangeEventArgs), "e"));
			codeMemberMethod.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Base(), "OnRow" + eventName, TypedDataSetGenerator.Argument("e")));
			codeMemberMethod.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(TypedDataSetGenerator.Event(rowClassName + eventName), TypedDataSetGenerator.Primitive(null)), TypedDataSetGenerator.Stm(TypedDataSetGenerator.DelegateCall(TypedDataSetGenerator.Event(rowClassName + eventName), TypedDataSetGenerator.New(rowClassName + "ChangeEvent", new CodeExpression[]
			{
				TypedDataSetGenerator.Cast(rowClassName, TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("e"), "Row")),
				TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("e"), "Action")
			})))));
			return codeMemberMethod;
		}

		private CodeTypeDeclaration CreateTypedTable(DataTable table)
		{
			string text = this.RowClassName(table);
			string text2 = this.TableClassName(table);
			string text3 = this.RowConcreteClassName(table);
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(text2);
			codeTypeDeclaration.BaseTypes.Add(typeof(DataTable));
			codeTypeDeclaration.BaseTypes.Add(typeof(IEnumerable));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.Serializable"));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.Diagnostics.DebuggerStepThrough"));
			for (int i = 0; i < table.Columns.Count; i++)
			{
				codeTypeDeclaration.Members.Add(TypedDataSetGenerator.FieldDecl(typeof(DataColumn), this.TableColumnFieldName(table.Columns[i])));
			}
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.EventDecl(text + "ChangeEventHandler", text + "Changed"));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.EventDecl(text + "ChangeEventHandler", text + "Changing"));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.EventDecl(text + "ChangeEventHandler", text + "Deleted"));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.EventDecl(text + "ChangeEventHandler", text + "Deleting"));
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = (MemberAttributes)4098;
			codeConstructor.BaseConstructorArgs.Add(TypedDataSetGenerator.Str(table.TableName));
			codeConstructor.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitClass"));
			codeTypeDeclaration.Members.Add(codeConstructor);
			codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = MemberAttributes.Family;
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(SerializationInfo), "info"));
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(StreamingContext), "context"));
			codeConstructor.BaseConstructorArgs.AddRange(new CodeExpression[]
			{
				TypedDataSetGenerator.Argument("info"),
				TypedDataSetGenerator.Argument("context")
			});
			codeConstructor.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitVars"));
			codeTypeDeclaration.Members.Add(codeConstructor);
			codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = (MemberAttributes)4098;
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(DataTable), "table"));
			codeConstructor.BaseConstructorArgs.Add(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "TableName"));
			codeConstructor.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "CaseSensitive"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "DataSet"), "CaseSensitive")), TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "CaseSensitive"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "CaseSensitive"))));
			codeConstructor.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "Locale"), "ToString"), TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "DataSet"), "Locale"), "ToString")), TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Locale"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "Locale"))));
			codeConstructor.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "Namespace"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "DataSet"), "Namespace")), TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Namespace"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "Namespace"))));
			codeConstructor.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Prefix"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "Prefix")));
			codeConstructor.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "MinimumCapacity"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "MinimumCapacity")));
			codeConstructor.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "DisplayExpression"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("table"), "DisplayExpression")));
			codeTypeDeclaration.Members.Add(codeConstructor);
			CodeMemberProperty codeMemberProperty = TypedDataSetGenerator.PropertyDecl(typeof(int), "Count", (MemberAttributes)24578);
			codeMemberProperty.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.ComponentModel.Browsable", TypedDataSetGenerator.Primitive(false)));
			codeMemberProperty.GetStatements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Property(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Rows"), "Count")));
			codeTypeDeclaration.Members.Add(codeMemberProperty);
			for (int j = 0; j < table.Columns.Count; j++)
			{
				DataColumn dataColumn = table.Columns[j];
				CodeMemberProperty codeMemberProperty2 = TypedDataSetGenerator.PropertyDecl(typeof(DataColumn), this.TableColumnPropertyName(dataColumn), (MemberAttributes)4098);
				codeMemberProperty2.GetStatements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableColumnFieldName(dataColumn))));
				codeTypeDeclaration.Members.Add(codeMemberProperty2);
			}
			CodeMemberProperty codeMemberProperty3 = TypedDataSetGenerator.PropertyDecl(text3, "Item", (MemberAttributes)24578);
			codeMemberProperty3.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(int), "index"));
			codeMemberProperty3.GetStatements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Cast(text3, TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Rows"), TypedDataSetGenerator.Argument("index")))));
			codeTypeDeclaration.Members.Add(codeMemberProperty3);
			CodeMemberMethod codeMemberMethod = TypedDataSetGenerator.MethodDecl(typeof(void), "Add" + text, (MemberAttributes)24578);
			codeMemberMethod.Parameters.Add(TypedDataSetGenerator.ParameterDecl(text3, "row"));
			codeMemberMethod.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Rows"), "Add", TypedDataSetGenerator.Argument("row")));
			codeTypeDeclaration.Members.Add(codeMemberMethod);
			ArrayList arrayList = new ArrayList();
			for (int k = 0; k < table.Columns.Count; k++)
			{
				if (!table.Columns[k].AutoIncrement)
				{
					arrayList.Add(table.Columns[k]);
				}
			}
			CodeMemberMethod codeMemberMethod2 = TypedDataSetGenerator.MethodDecl(text3, "Add" + text, (MemberAttributes)24578);
			DataColumn[] array = new DataColumn[arrayList.Count];
			arrayList.CopyTo(array, 0);
			for (int l = 0; l < array.Length; l++)
			{
				Type dataType = array[l].DataType;
				DataRelation dataRelation = array[l].FindParentRelation();
				if (this.ChildRelationFollowable(dataRelation))
				{
					string text4 = this.RowClassName(dataRelation.ParentTable);
					string text5 = this.FixIdName("parent" + text4 + "By" + dataRelation.RelationName);
					codeMemberMethod2.Parameters.Add(TypedDataSetGenerator.ParameterDecl(text4, text5));
				}
				else
				{
					codeMemberMethod2.Parameters.Add(TypedDataSetGenerator.ParameterDecl(this.GetTypeName(dataType), this.RowColumnPropertyName(array[l])));
				}
			}
			codeMemberMethod2.Statements.Add(TypedDataSetGenerator.VariableDecl(text3, "row" + text, TypedDataSetGenerator.Cast(text3, TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "NewRow"))));
			CodeExpression codeExpression = TypedDataSetGenerator.Variable("row" + text);
			CodeAssignStatement codeAssignStatement = new CodeAssignStatement();
			codeAssignStatement.Left = TypedDataSetGenerator.Property(codeExpression, "ItemArray");
			CodeArrayCreateExpression codeArrayCreateExpression = new CodeArrayCreateExpression();
			codeArrayCreateExpression.CreateType = TypedDataSetGenerator.Type(typeof(object));
			array = new DataColumn[table.Columns.Count];
			table.Columns.CopyTo(array, 0);
			for (int m = 0; m < array.Length; m++)
			{
				if (array[m].AutoIncrement)
				{
					codeArrayCreateExpression.Initializers.Add(TypedDataSetGenerator.Primitive(null));
				}
				else
				{
					DataRelation dataRelation2 = array[m].FindParentRelation();
					if (this.ChildRelationFollowable(dataRelation2))
					{
						string text6 = this.RowClassName(dataRelation2.ParentTable);
						string text7 = this.FixIdName("parent" + text6 + "By" + dataRelation2.RelationName);
						codeArrayCreateExpression.Initializers.Add(TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Argument(text7), TypedDataSetGenerator.Primitive(dataRelation2.ParentColumnsReference[0].Ordinal)));
					}
					else
					{
						codeArrayCreateExpression.Initializers.Add(TypedDataSetGenerator.Argument(this.RowColumnPropertyName(array[m])));
					}
				}
			}
			codeAssignStatement.Right = codeArrayCreateExpression;
			codeMemberMethod2.Statements.Add(codeAssignStatement);
			codeMemberMethod2.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Rows"), "Add", codeExpression));
			codeMemberMethod2.Statements.Add(TypedDataSetGenerator.Return(codeExpression));
			codeTypeDeclaration.Members.Add(codeMemberMethod2);
			for (int n = 0; n < table.Constraints.Count; n++)
			{
				if (table.Constraints[n] is UniqueConstraint && ((UniqueConstraint)table.Constraints[n]).IsPrimaryKey)
				{
					DataColumn[] columnsReference = ((UniqueConstraint)table.Constraints[n]).ColumnsReference;
					string text8 = "FindBy";
					bool flag = true;
					for (int num = 0; num < columnsReference.Length; num++)
					{
						text8 += this.RowColumnPropertyName(columnsReference[num]);
						if (columnsReference[num].ColumnMapping != MappingType.Hidden)
						{
							flag = false;
						}
					}
					if (!flag)
					{
						CodeMemberMethod codeMemberMethod3 = TypedDataSetGenerator.MethodDecl(text, this.FixIdName(text8), (MemberAttributes)24578);
						for (int num2 = 0; num2 < columnsReference.Length; num2++)
						{
							codeMemberMethod3.Parameters.Add(TypedDataSetGenerator.ParameterDecl(this.GetTypeName(columnsReference[num2].DataType), this.RowColumnPropertyName(columnsReference[num2])));
						}
						CodeArrayCreateExpression codeArrayCreateExpression2 = new CodeArrayCreateExpression(typeof(object), columnsReference.Length);
						for (int num3 = 0; num3 < columnsReference.Length; num3++)
						{
							codeArrayCreateExpression2.Initializers.Add(TypedDataSetGenerator.Argument(this.RowColumnPropertyName(columnsReference[num3])));
						}
						codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Cast(text, TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Rows"), "Find", codeArrayCreateExpression2))));
						codeTypeDeclaration.Members.Add(codeMemberMethod3);
					}
				}
			}
			CodeMemberMethod codeMemberMethod4 = TypedDataSetGenerator.MethodDecl(typeof(IEnumerator), "GetEnumerator", (MemberAttributes)24578);
			codeMemberMethod4.ImplementationTypes.Add(TypedDataSetGenerator.Type("System.Collections.IEnumerable"));
			codeMemberMethod4.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Rows"), "GetEnumerator")));
			codeTypeDeclaration.Members.Add(codeMemberMethod4);
			CodeMemberMethod codeMemberMethod5 = TypedDataSetGenerator.MethodDecl(typeof(DataTable), "Clone", (MemberAttributes)24580);
			codeMemberMethod5.Statements.Add(TypedDataSetGenerator.VariableDecl(text2, "cln", TypedDataSetGenerator.Cast(text2, TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Base(), "Clone", new CodeExpression[0]))));
			codeMemberMethod5.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Variable("cln"), "InitVars", new CodeExpression[0]));
			codeMemberMethod5.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Variable("cln")));
			codeTypeDeclaration.Members.Add(codeMemberMethod5);
			CodeMemberMethod codeMemberMethod6 = TypedDataSetGenerator.MethodDecl(typeof(DataTable), "CreateInstance", (MemberAttributes)12292);
			codeMemberMethod6.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.New(text2, new CodeExpression[0])));
			codeTypeDeclaration.Members.Add(codeMemberMethod6);
			CodeMemberMethod codeMemberMethod7 = TypedDataSetGenerator.MethodDecl(typeof(void), "InitClass", MemberAttributes.Private);
			CodeMemberMethod codeMemberMethod8 = TypedDataSetGenerator.MethodDecl(typeof(void), "InitVars", (MemberAttributes)4098);
			for (int num4 = 0; num4 < table.Columns.Count; num4++)
			{
				DataColumn dataColumn2 = table.Columns[num4];
				string text9 = this.TableColumnFieldName(dataColumn2);
				CodeExpression codeExpression2 = TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text9);
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(codeExpression2, TypedDataSetGenerator.New(typeof(DataColumn), new CodeExpression[]
				{
					TypedDataSetGenerator.Str(dataColumn2.ColumnName),
					TypedDataSetGenerator.TypeOf(this.GetTypeName(dataColumn2.DataType)),
					TypedDataSetGenerator.Primitive(null),
					TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(typeof(MappingType)), (dataColumn2.ColumnMapping == MappingType.SimpleContent) ? "SimpleContent" : ((dataColumn2.ColumnMapping == MappingType.Attribute) ? "Attribute" : ((dataColumn2.ColumnMapping == MappingType.Hidden) ? "Hidden" : "Element")))
				})));
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Columns"), "Add", TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text9)));
			}
			for (int num5 = 0; num5 < table.Constraints.Count; num5++)
			{
				if (table.Constraints[num5] is UniqueConstraint)
				{
					UniqueConstraint uniqueConstraint = (UniqueConstraint)table.Constraints[num5];
					DataColumn[] columnsReference2 = uniqueConstraint.ColumnsReference;
					CodeExpression[] array2 = new CodeExpression[columnsReference2.Length];
					for (int num6 = 0; num6 < columnsReference2.Length; num6++)
					{
						array2[num6] = TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableColumnFieldName(columnsReference2[num6]));
					}
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Constraints"), "Add", TypedDataSetGenerator.New(typeof(UniqueConstraint), new CodeExpression[]
					{
						TypedDataSetGenerator.Str(uniqueConstraint.ConstraintName),
						new CodeArrayCreateExpression(typeof(DataColumn), array2),
						TypedDataSetGenerator.Primitive(uniqueConstraint.IsPrimaryKey)
					})));
				}
			}
			for (int num7 = 0; num7 < table.Columns.Count; num7++)
			{
				DataColumn dataColumn3 = table.Columns[num7];
				string text10 = this.TableColumnFieldName(dataColumn3);
				CodeExpression codeExpression3 = TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text10);
				codeMemberMethod8.Statements.Add(TypedDataSetGenerator.Assign(codeExpression3, TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Columns"), TypedDataSetGenerator.Str(dataColumn3.ColumnName))));
				if (dataColumn3.AutoIncrement)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "AutoIncrement"), TypedDataSetGenerator.Primitive(true)));
				}
				if (dataColumn3.AutoIncrementSeed != 0L)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "AutoIncrementSeed"), TypedDataSetGenerator.Primitive(dataColumn3.AutoIncrementSeed)));
				}
				if (dataColumn3.AutoIncrementStep != 1L)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "AutoIncrementStep"), TypedDataSetGenerator.Primitive(dataColumn3.AutoIncrementStep)));
				}
				if (!dataColumn3.AllowDBNull)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "AllowDBNull"), TypedDataSetGenerator.Primitive(false)));
				}
				if (dataColumn3.ReadOnly)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "ReadOnly"), TypedDataSetGenerator.Primitive(true)));
				}
				if (dataColumn3.Unique)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "Unique"), TypedDataSetGenerator.Primitive(true)));
				}
				if (!ADP.IsEmpty(dataColumn3.Prefix))
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "Prefix"), TypedDataSetGenerator.Str(dataColumn3.Prefix)));
				}
				if (dataColumn3._columnUri != null)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "Namespace"), TypedDataSetGenerator.Str(dataColumn3.Namespace)));
				}
				if (dataColumn3.Caption != dataColumn3.ColumnName)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "Caption"), TypedDataSetGenerator.Str(dataColumn3.Caption)));
				}
				if (dataColumn3.DefaultValue != DBNull.Value)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "DefaultValue"), TypedDataSetGenerator.Primitive(dataColumn3.DefaultValue)));
				}
				if (dataColumn3.MaxLength != -1)
				{
					codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "MaxLength"), TypedDataSetGenerator.Primitive(dataColumn3.MaxLength)));
				}
			}
			if (table.ShouldSerializeCaseSensitive())
			{
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "CaseSensitive"), TypedDataSetGenerator.Primitive(table.CaseSensitive)));
			}
			if (table.ShouldSerializeLocale())
			{
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Locale"), TypedDataSetGenerator.New(typeof(CultureInfo), new CodeExpression[] { TypedDataSetGenerator.Str(table.Locale.ToString()) })));
			}
			if (!ADP.IsEmpty(table.Prefix))
			{
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Prefix"), TypedDataSetGenerator.Str(table.Prefix)));
			}
			if (table._tableNamespace != null)
			{
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Namespace"), TypedDataSetGenerator.Str(table.Namespace)));
			}
			if (table.MinimumCapacity != 50)
			{
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "MinimumCapacity"), TypedDataSetGenerator.Primitive(table.MinimumCapacity)));
			}
			if (table._displayExpression != null)
			{
				codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "DisplayExpression"), TypedDataSetGenerator.Str(table.DisplayExpressionInternal)));
			}
			codeTypeDeclaration.Members.Add(codeMemberMethod8);
			codeTypeDeclaration.Members.Add(codeMemberMethod7);
			CodeMemberMethod codeMemberMethod9 = TypedDataSetGenerator.MethodDecl(text3, "New" + text, (MemberAttributes)24578);
			codeMemberMethod9.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Cast(text3, TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "NewRow"))));
			codeTypeDeclaration.Members.Add(codeMemberMethod9);
			CodeMemberMethod codeMemberMethod10 = TypedDataSetGenerator.MethodDecl(typeof(DataRow), "NewRowFromBuilder", (MemberAttributes)12292);
			codeMemberMethod10.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(DataRowBuilder), "builder"));
			codeMemberMethod10.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.New(text3, new CodeExpression[] { TypedDataSetGenerator.Argument("builder") })));
			codeTypeDeclaration.Members.Add(codeMemberMethod10);
			CodeMemberMethod codeMemberMethod11 = TypedDataSetGenerator.MethodDecl(typeof(Type), "GetRowType", (MemberAttributes)12292);
			codeMemberMethod11.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.TypeOf(text3)));
			codeTypeDeclaration.Members.Add(codeMemberMethod11);
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.CreateOnRowEventMethod("Changed", text));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.CreateOnRowEventMethod("Changing", text));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.CreateOnRowEventMethod("Deleted", text));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.CreateOnRowEventMethod("Deleting", text));
			CodeMemberMethod codeMemberMethod12 = TypedDataSetGenerator.MethodDecl(typeof(void), "Remove" + text, (MemberAttributes)24578);
			codeMemberMethod12.Parameters.Add(TypedDataSetGenerator.ParameterDecl(text3, "row"));
			codeMemberMethod12.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Rows"), "Remove", TypedDataSetGenerator.Argument("row")));
			codeTypeDeclaration.Members.Add(codeMemberMethod12);
			return codeTypeDeclaration;
		}

		private CodeTypeDeclaration CreateTypedRow(DataTable table)
		{
			string text = this.RowClassName(table);
			string text2 = this.TableClassName(table);
			string text3 = this.TableFieldName(table);
			bool flag = false;
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
			codeTypeDeclaration.Name = text;
			string text4 = this.RowBaseClassName(table);
			if (string.Compare(text4, "DataRow", StringComparison.Ordinal) == 0)
			{
				codeTypeDeclaration.BaseTypes.Add(typeof(DataRow));
			}
			else
			{
				codeTypeDeclaration.BaseTypes.Add(text4);
			}
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.Diagnostics.DebuggerStepThrough"));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.FieldDecl(text2, text3));
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = (MemberAttributes)4098;
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(DataRowBuilder), "rb"));
			codeConstructor.BaseConstructorArgs.Add(TypedDataSetGenerator.Argument("rb"));
			codeConstructor.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text3), TypedDataSetGenerator.Cast(text2, TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Table"))));
			codeTypeDeclaration.Members.Add(codeConstructor);
			foreach (object obj in table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.ColumnMapping != MappingType.Hidden)
				{
					Type dataType = dataColumn.DataType;
					string text5 = this.RowColumnPropertyName(dataColumn);
					string text6 = this.TableColumnPropertyName(dataColumn);
					CodeMemberProperty codeMemberProperty = TypedDataSetGenerator.PropertyDecl(dataType, text5, (MemberAttributes)24578);
					CodeStatement codeStatement = TypedDataSetGenerator.Return(TypedDataSetGenerator.Cast(this.GetTypeName(dataType), TypedDataSetGenerator.Indexer(TypedDataSetGenerator.This(), TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text3), text6))));
					if (dataColumn.AllowDBNull)
					{
						string text7 = (string)dataColumn.ExtendedProperties["nullValue"];
						if (text7 == null || text7 == "_throw")
						{
							codeStatement = TypedDataSetGenerator.Try(codeStatement, TypedDataSetGenerator.Catch(typeof(InvalidCastException), "e", TypedDataSetGenerator.Throw(typeof(StrongTypingException), "StrongTyping_CananotAccessDBNull", "e")));
						}
						else
						{
							CodeExpression codeExpression = null;
							CodeExpression codeExpression2;
							if (text7 == "_null")
							{
								if (dataColumn.DataType.IsSubclassOf(typeof(ValueType)))
								{
									this.errorList.Add(Res.GetString("Column '{0}': Type '{1}' cannot be null", new object[]
									{
										dataColumn.ColumnName,
										dataColumn.DataType.Name
									}));
									continue;
								}
								codeExpression2 = TypedDataSetGenerator.Primitive(null);
							}
							else if (text7 == "_empty")
							{
								if (dataColumn.DataType == typeof(string))
								{
									codeExpression2 = TypedDataSetGenerator.Property(TypedDataSetGenerator.TypeExpr(dataColumn.DataType), "Empty");
								}
								else
								{
									codeExpression2 = TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(text), text5 + "_nullValue");
									ConstructorInfo constructor = dataColumn.DataType.GetConstructor(new Type[] { typeof(string) });
									if (constructor == null)
									{
										this.errorList.Add(Res.GetString("Column '{0}': Type '{1}' does not have parameterless constructor", new object[]
										{
											dataColumn.ColumnName,
											dataColumn.DataType.Name
										}));
										continue;
									}
									constructor.Invoke(new object[0]);
									codeExpression = TypedDataSetGenerator.New(dataColumn.DataType, new CodeExpression[0]);
								}
							}
							else
							{
								if (!flag)
								{
									table.NewRow();
									flag = true;
								}
								object obj2 = dataColumn.ConvertXmlToObject(text7);
								if (dataColumn.DataType == typeof(char) || dataColumn.DataType == typeof(string) || dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(bool) || dataColumn.DataType == typeof(float) || dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(sbyte) || dataColumn.DataType == typeof(byte) || dataColumn.DataType == typeof(short) || dataColumn.DataType == typeof(ushort) || dataColumn.DataType == typeof(int) || dataColumn.DataType == typeof(uint) || dataColumn.DataType == typeof(long) || dataColumn.DataType == typeof(ulong))
								{
									codeExpression2 = TypedDataSetGenerator.Primitive(obj2);
								}
								else
								{
									codeExpression2 = TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(text), text5 + "_nullValue");
									if (dataColumn.DataType == typeof(byte[]))
									{
										codeExpression = TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.TypeExpr(typeof(Convert)), "FromBase64String", TypedDataSetGenerator.Primitive(text7));
									}
									else if (dataColumn.DataType == typeof(DateTime) || dataColumn.DataType == typeof(TimeSpan))
									{
										codeExpression = TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.TypeExpr(dataColumn.DataType), "Parse", TypedDataSetGenerator.Primitive(obj2.ToString()));
									}
									else
									{
										ConstructorInfo constructor2 = dataColumn.DataType.GetConstructor(new Type[] { typeof(string) });
										if (constructor2 == null)
										{
											this.errorList.Add(Res.GetString("Column '{0}': Type '{1}' does not have constructor with string argument", new object[]
											{
												dataColumn.ColumnName,
												dataColumn.DataType.Name
											}));
											continue;
										}
										constructor2.Invoke(new object[] { text7 });
										codeExpression = TypedDataSetGenerator.New(dataColumn.DataType, new CodeExpression[] { TypedDataSetGenerator.Primitive(text7) });
									}
								}
							}
							codeStatement = TypedDataSetGenerator.If(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "Is" + text5 + "Null"), new CodeStatement[] { TypedDataSetGenerator.Return(codeExpression2) }, new CodeStatement[] { codeStatement });
							if (codeExpression != null)
							{
								CodeMemberField codeMemberField = TypedDataSetGenerator.FieldDecl(dataColumn.DataType, text5 + "_nullValue");
								codeMemberField.Attributes = (MemberAttributes)20483;
								codeMemberField.InitExpression = codeExpression;
								codeTypeDeclaration.Members.Add(codeMemberField);
							}
						}
					}
					codeMemberProperty.GetStatements.Add(codeStatement);
					codeMemberProperty.SetStatements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Indexer(TypedDataSetGenerator.This(), TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text3), text6)), TypedDataSetGenerator.Value()));
					codeTypeDeclaration.Members.Add(codeMemberProperty);
					if (dataColumn.AllowDBNull)
					{
						CodeMemberMethod codeMemberMethod = TypedDataSetGenerator.MethodDecl(typeof(bool), "Is" + text5 + "Null", (MemberAttributes)24578);
						codeMemberMethod.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "IsNull", TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text3), text6))));
						codeTypeDeclaration.Members.Add(codeMemberMethod);
						CodeMemberMethod codeMemberMethod2 = TypedDataSetGenerator.MethodDecl(typeof(void), "Set" + text5 + "Null", (MemberAttributes)24578);
						codeMemberMethod2.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Indexer(TypedDataSetGenerator.This(), TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text3), text6)), TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(typeof(Convert)), "DBNull")));
						codeTypeDeclaration.Members.Add(codeMemberMethod2);
					}
				}
			}
			DataRelationCollection childRelations = table.ChildRelations;
			for (int i = 0; i < childRelations.Count; i++)
			{
				DataRelation dataRelation = childRelations[i];
				string text8 = this.RowConcreteClassName(dataRelation.ChildTable);
				CodeMemberMethod codeMemberMethod3 = TypedDataSetGenerator.Method(TypedDataSetGenerator.Type(text8, 1), this.ChildPropertyName(dataRelation), (MemberAttributes)24578);
				codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Cast(TypedDataSetGenerator.Type(text8, 1), TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "GetChildRows", TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Table"), "ChildRelations"), TypedDataSetGenerator.Str(dataRelation.RelationName))))));
				codeTypeDeclaration.Members.Add(codeMemberMethod3);
			}
			DataRelationCollection parentRelations = table.ParentRelations;
			for (int j = 0; j < parentRelations.Count; j++)
			{
				DataRelation dataRelation2 = parentRelations[j];
				string text9 = this.RowClassName(dataRelation2.ParentTable);
				CodeMemberProperty codeMemberProperty2 = TypedDataSetGenerator.PropertyDecl(text9, this.ParentPropertyName(dataRelation2), (MemberAttributes)24578);
				codeMemberProperty2.GetStatements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Cast(text9, TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "GetParentRow", TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Table"), "ParentRelations"), TypedDataSetGenerator.Str(dataRelation2.RelationName))))));
				codeMemberProperty2.SetStatements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "SetParentRow", new CodeExpression[]
				{
					TypedDataSetGenerator.Value(),
					TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Table"), "ParentRelations"), TypedDataSetGenerator.Str(dataRelation2.RelationName))
				}));
				codeTypeDeclaration.Members.Add(codeMemberProperty2);
			}
			return codeTypeDeclaration;
		}

		private CodeTypeDeclaration CreateTypedRowEvent(DataTable table)
		{
			string text = this.RowClassName(table);
			this.TableClassName(table);
			string text2 = this.RowConcreteClassName(table);
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
			codeTypeDeclaration.Name = text + "ChangeEvent";
			codeTypeDeclaration.BaseTypes.Add(typeof(EventArgs));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.Diagnostics.DebuggerStepThrough"));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.FieldDecl(text2, "eventRow"));
			codeTypeDeclaration.Members.Add(TypedDataSetGenerator.FieldDecl(typeof(DataRowAction), "eventAction"));
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = (MemberAttributes)24578;
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(text2, "row"));
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(DataRowAction), "action"));
			codeConstructor.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), "eventRow"), TypedDataSetGenerator.Argument("row")));
			codeConstructor.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), "eventAction"), TypedDataSetGenerator.Argument("action")));
			codeTypeDeclaration.Members.Add(codeConstructor);
			CodeMemberProperty codeMemberProperty = TypedDataSetGenerator.PropertyDecl(text2, "Row", (MemberAttributes)24578);
			codeMemberProperty.GetStatements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), "eventRow")));
			codeTypeDeclaration.Members.Add(codeMemberProperty);
			codeMemberProperty = TypedDataSetGenerator.PropertyDecl(typeof(DataRowAction), "Action", (MemberAttributes)24578);
			codeMemberProperty.GetStatements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), "eventAction")));
			codeTypeDeclaration.Members.Add(codeMemberProperty);
			return codeTypeDeclaration;
		}

		private CodeTypeDelegate CreateTypedRowEventHandler(DataTable table)
		{
			string text = this.RowClassName(table);
			CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate(text + "ChangeEventHandler");
			codeTypeDelegate.TypeAttributes |= TypeAttributes.Public;
			codeTypeDelegate.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(object), "sender"));
			codeTypeDelegate.Parameters.Add(TypedDataSetGenerator.ParameterDecl(text + "ChangeEvent", "e"));
			return codeTypeDelegate;
		}

		private CodeTypeDeclaration CreateTypedDataSet(DataSet dataSet)
		{
			string text = this.FixIdName(dataSet.DataSetName);
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(text);
			codeTypeDeclaration.BaseTypes.Add(typeof(DataSet));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.Serializable"));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.ComponentModel.DesignerCategoryAttribute", TypedDataSetGenerator.Str("code")));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.Diagnostics.DebuggerStepThrough"));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.ComponentModel.ToolboxItem", TypedDataSetGenerator.Primitive(true)));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl(typeof(XmlSchemaProviderAttribute).FullName, TypedDataSetGenerator.Primitive("GetTypedDataSetSchema")));
			codeTypeDeclaration.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl(typeof(XmlRootAttribute).FullName, TypedDataSetGenerator.Primitive(text)));
			for (int i = 0; i < dataSet.Tables.Count; i++)
			{
				codeTypeDeclaration.Members.Add(TypedDataSetGenerator.FieldDecl(this.TableClassName(dataSet.Tables[i]), this.TableFieldName(dataSet.Tables[i])));
			}
			for (int j = 0; j < dataSet.Relations.Count; j++)
			{
				codeTypeDeclaration.Members.Add(TypedDataSetGenerator.FieldDecl(typeof(DataRelation), this.RelationFieldName(dataSet.Relations[j])));
			}
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = MemberAttributes.Public;
			codeConstructor.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "BeginInit"));
			codeConstructor.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitClass"));
			codeConstructor.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(CollectionChangeEventHandler), "schemaChangedHandler", new CodeDelegateCreateExpression(TypedDataSetGenerator.Type(typeof(CollectionChangeEventHandler)), TypedDataSetGenerator.This(), "SchemaChanged")));
			codeConstructor.Statements.Add(new CodeAttachEventStatement(new CodeEventReferenceExpression(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Tables"), "CollectionChanged"), TypedDataSetGenerator.Variable("schemaChangedHandler")));
			codeConstructor.Statements.Add(new CodeAttachEventStatement(new CodeEventReferenceExpression(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Relations"), "CollectionChanged"), TypedDataSetGenerator.Variable("schemaChangedHandler")));
			codeConstructor.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "EndInit"));
			codeTypeDeclaration.Members.Add(codeConstructor);
			codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = MemberAttributes.Family;
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(SerializationInfo), "info"));
			codeConstructor.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(StreamingContext), "context"));
			codeConstructor.BaseConstructorArgs.AddRange(new CodeExpression[]
			{
				TypedDataSetGenerator.Argument("info"),
				TypedDataSetGenerator.Argument("context")
			});
			codeConstructor.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.EQ(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "IsBinarySerialized", new CodeExpression[]
			{
				TypedDataSetGenerator.Argument("info"),
				TypedDataSetGenerator.Argument("context")
			}), TypedDataSetGenerator.Primitive(true)), new CodeStatement[]
			{
				TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitVars", TypedDataSetGenerator.Primitive(false))),
				TypedDataSetGenerator.VariableDecl(typeof(CollectionChangeEventHandler), "schemaChangedHandler1", new CodeDelegateCreateExpression(TypedDataSetGenerator.Type(typeof(CollectionChangeEventHandler)), TypedDataSetGenerator.This(), "SchemaChanged")),
				new CodeAttachEventStatement(new CodeEventReferenceExpression(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Tables"), "CollectionChanged"), TypedDataSetGenerator.Variable("schemaChangedHandler1")),
				new CodeAttachEventStatement(new CodeEventReferenceExpression(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Relations"), "CollectionChanged"), TypedDataSetGenerator.Variable("schemaChangedHandler1")),
				TypedDataSetGenerator.Return()
			}));
			codeConstructor.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(string), "strSchema", TypedDataSetGenerator.Cast("System.String", TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Argument("info"), "GetValue", new CodeExpression[]
			{
				TypedDataSetGenerator.Str("XmlSchema"),
				TypedDataSetGenerator.TypeOf("System.String")
			}))));
			ArrayList arrayList = new ArrayList();
			arrayList.Add(TypedDataSetGenerator.VariableDecl(typeof(DataSet), "ds", TypedDataSetGenerator.New(typeof(DataSet), new CodeExpression[0])));
			arrayList.Add(TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Variable("ds"), "ReadXmlSchema", new CodeExpression[] { TypedDataSetGenerator.New(typeof(XmlTextReader), new CodeExpression[] { TypedDataSetGenerator.New("System.IO.StringReader", new CodeExpression[] { TypedDataSetGenerator.Variable("strSchema") }) }) })));
			for (int k = 0; k < dataSet.Tables.Count; k++)
			{
				arrayList.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Tables"), TypedDataSetGenerator.Str(dataSet.Tables[k].TableName)), TypedDataSetGenerator.Primitive(null)), TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Tables"), "Add", TypedDataSetGenerator.New(this.TableClassName(dataSet.Tables[k]), new CodeExpression[] { TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Tables"), TypedDataSetGenerator.Str(dataSet.Tables[k].TableName)) })))));
			}
			arrayList.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "DataSetName"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "DataSetName")));
			arrayList.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Prefix"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Prefix")));
			arrayList.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Namespace"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Namespace")));
			arrayList.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Locale"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Locale")));
			arrayList.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "CaseSensitive"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "CaseSensitive")));
			arrayList.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "EnforceConstraints"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "EnforceConstraints")));
			arrayList.Add(TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "Merge", new CodeExpression[]
			{
				TypedDataSetGenerator.Variable("ds"),
				TypedDataSetGenerator.Primitive(false),
				TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(typeof(MissingSchemaAction)), "Add")
			})));
			arrayList.Add(TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitVars")));
			CodeStatement[] array = new CodeStatement[arrayList.Count];
			arrayList.CopyTo(array);
			codeConstructor.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(TypedDataSetGenerator.Variable("strSchema"), TypedDataSetGenerator.Primitive(null)), array, new CodeStatement[]
			{
				TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "BeginInit")),
				TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitClass")),
				TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "EndInit"))
			}));
			codeConstructor.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "GetSerializationData", new CodeExpression[]
			{
				TypedDataSetGenerator.Argument("info"),
				TypedDataSetGenerator.Argument("context")
			}));
			codeConstructor.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(CollectionChangeEventHandler), "schemaChangedHandler", new CodeDelegateCreateExpression(TypedDataSetGenerator.Type(typeof(CollectionChangeEventHandler)), TypedDataSetGenerator.This(), "SchemaChanged")));
			codeConstructor.Statements.Add(new CodeAttachEventStatement(new CodeEventReferenceExpression(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Tables"), "CollectionChanged"), TypedDataSetGenerator.Variable("schemaChangedHandler")));
			codeConstructor.Statements.Add(new CodeAttachEventStatement(new CodeEventReferenceExpression(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Relations"), "CollectionChanged"), TypedDataSetGenerator.Variable("schemaChangedHandler")));
			codeTypeDeclaration.Members.Add(codeConstructor);
			CodeMemberMethod codeMemberMethod = TypedDataSetGenerator.MethodDecl(typeof(DataSet), "Clone", (MemberAttributes)24580);
			codeMemberMethod.Statements.Add(TypedDataSetGenerator.VariableDecl(text, "cln", TypedDataSetGenerator.Cast(text, TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Base(), "Clone", new CodeExpression[0]))));
			codeMemberMethod.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Variable("cln"), "InitVars", new CodeExpression[0]));
			codeMemberMethod.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Variable("cln")));
			codeTypeDeclaration.Members.Add(codeMemberMethod);
			CodeMemberMethod codeMemberMethod2 = TypedDataSetGenerator.MethodDecl(typeof(void), "InitVars", (MemberAttributes)4098);
			codeMemberMethod2.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitVars", new CodeExpression[] { TypedDataSetGenerator.Primitive(true) }));
			codeTypeDeclaration.Members.Add(codeMemberMethod2);
			CodeMemberMethod codeMemberMethod3 = TypedDataSetGenerator.MethodDecl(typeof(void), "InitClass", MemberAttributes.Private);
			CodeMemberMethod codeMemberMethod4 = TypedDataSetGenerator.MethodDecl(typeof(void), "InitVars", (MemberAttributes)4098);
			codeMemberMethod4.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(bool), "initTable"));
			codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "DataSetName"), TypedDataSetGenerator.Str(dataSet.DataSetName)));
			codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Prefix"), TypedDataSetGenerator.Str(dataSet.Prefix)));
			codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Namespace"), TypedDataSetGenerator.Str(dataSet.Namespace)));
			codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Locale"), TypedDataSetGenerator.New(typeof(CultureInfo), new CodeExpression[] { TypedDataSetGenerator.Str(dataSet.Locale.ToString()) })));
			codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "CaseSensitive"), TypedDataSetGenerator.Primitive(dataSet.CaseSensitive)));
			codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "EnforceConstraints"), TypedDataSetGenerator.Primitive(dataSet.EnforceConstraints)));
			for (int l = 0; l < dataSet.Tables.Count; l++)
			{
				CodeExpression codeExpression = TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableFieldName(dataSet.Tables[l]));
				codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(codeExpression, TypedDataSetGenerator.New(this.TableClassName(dataSet.Tables[l]), new CodeExpression[0])));
				codeMemberMethod3.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Tables"), "Add", codeExpression));
				codeMemberMethod4.Statements.Add(TypedDataSetGenerator.Assign(codeExpression, TypedDataSetGenerator.Cast(this.TableClassName(dataSet.Tables[l]), TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Tables"), TypedDataSetGenerator.Str(dataSet.Tables[l].TableName)))));
				codeMemberMethod4.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.EQ(TypedDataSetGenerator.Variable("initTable"), TypedDataSetGenerator.Primitive(true)), new CodeStatement[] { TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(codeExpression, TypedDataSetGenerator.Primitive(null)), TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(codeExpression, "InitVars"))) }));
			}
			CodeMemberMethod codeMemberMethod5 = TypedDataSetGenerator.MethodDecl(typeof(bool), "ShouldSerializeTables", (MemberAttributes)12292);
			codeMemberMethod5.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Primitive(false)));
			codeTypeDeclaration.Members.Add(codeMemberMethod5);
			CodeMemberMethod codeMemberMethod6 = TypedDataSetGenerator.MethodDecl(typeof(bool), "ShouldSerializeRelations", (MemberAttributes)12292);
			codeMemberMethod6.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Primitive(false)));
			codeTypeDeclaration.Members.Add(codeMemberMethod6);
			CodeMemberMethod codeMemberMethod7 = TypedDataSetGenerator.MethodDecl(typeof(XmlSchemaComplexType), "GetTypedDataSetSchema", (MemberAttributes)24579);
			codeMemberMethod7.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(XmlSchemaSet), "xs"));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.VariableDecl(text, "ds", TypedDataSetGenerator.New(text, new CodeExpression[0])));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Argument("xs"), "Add", new CodeExpression[] { TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Variable("ds"), "GetSchemaSerializable", new CodeExpression[0]) }));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(XmlSchemaComplexType), "type", TypedDataSetGenerator.New(typeof(XmlSchemaComplexType), new CodeExpression[0])));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(XmlSchemaSequence), "sequence", TypedDataSetGenerator.New(typeof(XmlSchemaSequence), new CodeExpression[0])));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(XmlSchemaAny), "any", TypedDataSetGenerator.New(typeof(XmlSchemaAny), new CodeExpression[0])));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("any"), "Namespace"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Namespace")));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("sequence"), "Items"), "Add", new CodeExpression[] { TypedDataSetGenerator.Variable("any") }));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("type"), "Particle"), TypedDataSetGenerator.Variable("sequence")));
			codeMemberMethod7.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Variable("type")));
			codeTypeDeclaration.Members.Add(codeMemberMethod7);
			CodeMemberMethod codeMemberMethod8 = TypedDataSetGenerator.MethodDecl(typeof(void), "ReadXmlSerializable", (MemberAttributes)12292);
			codeMemberMethod8.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(XmlReader), "reader"));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "Reset", new CodeExpression[0]));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(DataSet), "ds", TypedDataSetGenerator.New(typeof(DataSet), new CodeExpression[0])));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Variable("ds"), "ReadXml", new CodeExpression[] { TypedDataSetGenerator.Argument("reader") }));
			for (int m = 0; m < dataSet.Tables.Count; m++)
			{
				codeMemberMethod8.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.IdNotEQ(TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Tables"), TypedDataSetGenerator.Str(dataSet.Tables[m].TableName)), TypedDataSetGenerator.Primitive(null)), TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Tables"), "Add", TypedDataSetGenerator.New(this.TableClassName(dataSet.Tables[m]), new CodeExpression[] { TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Tables"), TypedDataSetGenerator.Str(dataSet.Tables[m].TableName)) })))));
			}
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "DataSetName"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "DataSetName")));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Prefix"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Prefix")));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Namespace"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Namespace")));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Locale"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "Locale")));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "CaseSensitive"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "CaseSensitive")));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "EnforceConstraints"), TypedDataSetGenerator.Property(TypedDataSetGenerator.Variable("ds"), "EnforceConstraints")));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "Merge", new CodeExpression[]
			{
				TypedDataSetGenerator.Variable("ds"),
				TypedDataSetGenerator.Primitive(false),
				TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(typeof(MissingSchemaAction)), "Add")
			}));
			codeMemberMethod8.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitVars"));
			codeTypeDeclaration.Members.Add(codeMemberMethod8);
			CodeMemberMethod codeMemberMethod9 = TypedDataSetGenerator.MethodDecl(typeof(XmlSchema), "GetSchemaSerializable", (MemberAttributes)12292);
			codeMemberMethod9.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(MemoryStream), "stream", TypedDataSetGenerator.New(typeof(MemoryStream), new CodeExpression[0])));
			codeMemberMethod9.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "WriteXmlSchema", TypedDataSetGenerator.New(typeof(XmlTextWriter), new CodeExpression[]
			{
				TypedDataSetGenerator.Argument("stream"),
				TypedDataSetGenerator.Primitive(null)
			})));
			codeMemberMethod9.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("stream"), "Position"), TypedDataSetGenerator.Primitive(0)));
			codeMemberMethod9.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.TypeExpr("System.Xml.Schema.XmlSchema"), "Read", new CodeExpression[]
			{
				TypedDataSetGenerator.New(typeof(XmlTextReader), new CodeExpression[] { TypedDataSetGenerator.Argument("stream") }),
				TypedDataSetGenerator.Primitive(null)
			})));
			codeTypeDeclaration.Members.Add(codeMemberMethod9);
			CodeExpression codeExpression2 = null;
			foreach (object obj in dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				foreach (object obj2 in dataTable.Constraints)
				{
					Constraint constraint = (Constraint)obj2;
					if (constraint is ForeignKeyConstraint)
					{
						ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraint;
						CodeArrayCreateExpression codeArrayCreateExpression = new CodeArrayCreateExpression(typeof(DataColumn), 0);
						foreach (DataColumn dataColumn in foreignKeyConstraint.Columns)
						{
							codeArrayCreateExpression.Initializers.Add(TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableFieldName(dataColumn.Table)), this.TableColumnPropertyName(dataColumn)));
						}
						CodeArrayCreateExpression codeArrayCreateExpression2 = new CodeArrayCreateExpression(typeof(DataColumn), 0);
						foreach (DataColumn dataColumn2 in foreignKeyConstraint.RelatedColumnsReference)
						{
							codeArrayCreateExpression2.Initializers.Add(TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableFieldName(dataColumn2.Table)), this.TableColumnPropertyName(dataColumn2)));
						}
						if (codeExpression2 == null)
						{
							codeMemberMethod3.Statements.Add(TypedDataSetGenerator.VariableDecl(typeof(ForeignKeyConstraint), "fkc"));
							codeExpression2 = TypedDataSetGenerator.Variable("fkc");
						}
						codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(codeExpression2, TypedDataSetGenerator.New(typeof(ForeignKeyConstraint), new CodeExpression[]
						{
							TypedDataSetGenerator.Str(foreignKeyConstraint.ConstraintName),
							codeArrayCreateExpression2,
							codeArrayCreateExpression
						})));
						codeMemberMethod3.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableFieldName(dataTable)), "Constraints"), "Add", codeExpression2));
						string text2 = foreignKeyConstraint.AcceptRejectRule.ToString();
						string text3 = foreignKeyConstraint.DeleteRule.ToString();
						string text4 = foreignKeyConstraint.UpdateRule.ToString();
						codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression2, "AcceptRejectRule"), TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(foreignKeyConstraint.AcceptRejectRule.GetType()), text2)));
						codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression2, "DeleteRule"), TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(foreignKeyConstraint.DeleteRule.GetType()), text3)));
						codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression2, "UpdateRule"), TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(foreignKeyConstraint.UpdateRule.GetType()), text4)));
					}
				}
			}
			foreach (object obj3 in dataSet.Relations)
			{
				DataRelation dataRelation = (DataRelation)obj3;
				CodeArrayCreateExpression codeArrayCreateExpression3 = new CodeArrayCreateExpression(typeof(DataColumn), 0);
				string text5 = this.TableFieldName(dataRelation.ParentTable);
				foreach (DataColumn dataColumn3 in dataRelation.ParentColumnsReference)
				{
					codeArrayCreateExpression3.Initializers.Add(TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text5), this.TableColumnPropertyName(dataColumn3)));
				}
				CodeArrayCreateExpression codeArrayCreateExpression4 = new CodeArrayCreateExpression(typeof(DataColumn), 0);
				string text6 = this.TableFieldName(dataRelation.ChildTable);
				foreach (DataColumn dataColumn4 in dataRelation.ChildColumnsReference)
				{
					codeArrayCreateExpression4.Initializers.Add(TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), text6), this.TableColumnPropertyName(dataColumn4)));
				}
				codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.RelationFieldName(dataRelation)), TypedDataSetGenerator.New(typeof(DataRelation), new CodeExpression[]
				{
					TypedDataSetGenerator.Str(dataRelation.RelationName),
					codeArrayCreateExpression3,
					codeArrayCreateExpression4,
					TypedDataSetGenerator.Primitive(false)
				})));
				if (dataRelation.Nested)
				{
					codeMemberMethod3.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.RelationFieldName(dataRelation)), "Nested"), TypedDataSetGenerator.Primitive(true)));
				}
				codeMemberMethod3.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Relations"), "Add", TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.RelationFieldName(dataRelation))));
				codeMemberMethod4.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.RelationFieldName(dataRelation)), TypedDataSetGenerator.Indexer(TypedDataSetGenerator.Property(TypedDataSetGenerator.This(), "Relations"), TypedDataSetGenerator.Str(dataRelation.RelationName))));
			}
			codeTypeDeclaration.Members.Add(codeMemberMethod4);
			codeTypeDeclaration.Members.Add(codeMemberMethod3);
			for (int num = 0; num < dataSet.Tables.Count; num++)
			{
				string text7 = this.TablePropertyName(dataSet.Tables[num]);
				CodeMemberProperty codeMemberProperty = TypedDataSetGenerator.PropertyDecl(this.TableClassName(dataSet.Tables[num]), text7, (MemberAttributes)24578);
				codeMemberProperty.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.ComponentModel.Browsable", TypedDataSetGenerator.Primitive(false)));
				codeMemberProperty.CustomAttributes.Add(TypedDataSetGenerator.AttributeDecl("System.ComponentModel.DesignerSerializationVisibilityAttribute", TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(typeof(DesignerSerializationVisibility)), "Content")));
				codeMemberProperty.GetStatements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableFieldName(dataSet.Tables[num]))));
				codeTypeDeclaration.Members.Add(codeMemberProperty);
				CodeMemberMethod codeMemberMethod10 = TypedDataSetGenerator.MethodDecl(typeof(bool), "ShouldSerialize" + text7, MemberAttributes.Private);
				codeMemberMethod10.Statements.Add(TypedDataSetGenerator.Return(TypedDataSetGenerator.Primitive(false)));
				codeTypeDeclaration.Members.Add(codeMemberMethod10);
			}
			CodeMemberMethod codeMemberMethod11 = TypedDataSetGenerator.MethodDecl(typeof(void), "SchemaChanged", MemberAttributes.Private);
			codeMemberMethod11.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(object), "sender"));
			codeMemberMethod11.Parameters.Add(TypedDataSetGenerator.ParameterDecl(typeof(CollectionChangeEventArgs), "e"));
			codeMemberMethod11.Statements.Add(TypedDataSetGenerator.If(TypedDataSetGenerator.EQ(TypedDataSetGenerator.Property(TypedDataSetGenerator.Argument("e"), "Action"), TypedDataSetGenerator.Field(TypedDataSetGenerator.TypeExpr(typeof(CollectionChangeAction)), "Remove")), TypedDataSetGenerator.Stm(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitVars"))));
			codeTypeDeclaration.Members.Add(codeMemberMethod11);
			bool flag = false;
			CodeMemberMethod codeMemberMethod12 = TypedDataSetGenerator.MethodDecl(typeof(void), "InitExpressions", MemberAttributes.Private);
			foreach (object obj4 in dataSet.Tables)
			{
				DataTable dataTable2 = (DataTable)obj4;
				for (int num2 = 0; num2 < dataTable2.Columns.Count; num2++)
				{
					DataColumn dataColumn5 = dataTable2.Columns[num2];
					CodeExpression codeExpression3 = TypedDataSetGenerator.Property(TypedDataSetGenerator.Field(TypedDataSetGenerator.This(), this.TableFieldName(dataTable2)), this.TableColumnPropertyName(dataColumn5));
					if (dataColumn5.Expression.Length > 0)
					{
						flag = true;
						codeMemberMethod12.Statements.Add(TypedDataSetGenerator.Assign(TypedDataSetGenerator.Property(codeExpression3, "Expression"), TypedDataSetGenerator.Str(dataColumn5.Expression)));
					}
				}
			}
			if (flag)
			{
				codeTypeDeclaration.Members.Add(codeMemberMethod12);
				codeMemberMethod3.Statements.Add(TypedDataSetGenerator.MethodCall(TypedDataSetGenerator.This(), "InitExpressions"));
			}
			return codeTypeDeclaration;
		}

		private static CodeExpression This()
		{
			return new CodeThisReferenceExpression();
		}

		private static CodeExpression Base()
		{
			return new CodeBaseReferenceExpression();
		}

		private static CodeExpression Value()
		{
			return new CodePropertySetValueReferenceExpression();
		}

		private static CodeTypeReference Type(string type)
		{
			return new CodeTypeReference(type);
		}

		private static CodeTypeReference Type(Type type)
		{
			return new CodeTypeReference(type);
		}

		private static CodeTypeReference Type(string type, int rank)
		{
			return new CodeTypeReference(type, rank);
		}

		private static CodeTypeReferenceExpression TypeExpr(Type type)
		{
			return new CodeTypeReferenceExpression(type);
		}

		private static CodeTypeReferenceExpression TypeExpr(string type)
		{
			return new CodeTypeReferenceExpression(type);
		}

		private static CodeExpression Cast(string type, CodeExpression expr)
		{
			return new CodeCastExpression(type, expr);
		}

		private static CodeExpression Cast(CodeTypeReference type, CodeExpression expr)
		{
			return new CodeCastExpression(type, expr);
		}

		private static CodeExpression TypeOf(string type)
		{
			return new CodeTypeOfExpression(type);
		}

		private static CodeExpression Field(CodeExpression exp, string field)
		{
			return new CodeFieldReferenceExpression(exp, field);
		}

		private static CodeExpression Property(CodeExpression exp, string property)
		{
			return new CodePropertyReferenceExpression(exp, property);
		}

		private static CodeExpression Argument(string argument)
		{
			return new CodeArgumentReferenceExpression(argument);
		}

		private static CodeExpression Variable(string variable)
		{
			return new CodeVariableReferenceExpression(variable);
		}

		private static CodeExpression Event(string eventName)
		{
			return new CodeEventReferenceExpression(TypedDataSetGenerator.This(), eventName);
		}

		private static CodeExpression New(string type, CodeExpression[] parameters)
		{
			return new CodeObjectCreateExpression(type, parameters);
		}

		private static CodeExpression New(Type type, CodeExpression[] parameters)
		{
			return new CodeObjectCreateExpression(type, parameters);
		}

		private static CodeExpression Primitive(object primitive)
		{
			return new CodePrimitiveExpression(primitive);
		}

		private static CodeExpression Str(string str)
		{
			return TypedDataSetGenerator.Primitive(str);
		}

		private static CodeExpression MethodCall(CodeExpression targetObject, string methodName, CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(targetObject, methodName, parameters);
		}

		private static CodeExpression MethodCall(CodeExpression targetObject, string methodName)
		{
			return new CodeMethodInvokeExpression(targetObject, methodName, Array.Empty<CodeExpression>());
		}

		private static CodeExpression MethodCall(CodeExpression targetObject, string methodName, CodeExpression par)
		{
			return new CodeMethodInvokeExpression(targetObject, methodName, new CodeExpression[] { par });
		}

		private static CodeExpression DelegateCall(CodeExpression targetObject, CodeExpression par)
		{
			return new CodeDelegateInvokeExpression(targetObject, new CodeExpression[]
			{
				TypedDataSetGenerator.This(),
				par
			});
		}

		private static CodeExpression Indexer(CodeExpression targetObject, CodeExpression indices)
		{
			return new CodeIndexerExpression(targetObject, new CodeExpression[] { indices });
		}

		private static CodeBinaryOperatorExpression BinOperator(CodeExpression left, CodeBinaryOperatorType op, CodeExpression right)
		{
			return new CodeBinaryOperatorExpression(left, op, right);
		}

		private static CodeBinaryOperatorExpression IdNotEQ(CodeExpression left, CodeExpression right)
		{
			return TypedDataSetGenerator.BinOperator(left, CodeBinaryOperatorType.IdentityInequality, right);
		}

		private static CodeBinaryOperatorExpression EQ(CodeExpression left, CodeExpression right)
		{
			return TypedDataSetGenerator.BinOperator(left, CodeBinaryOperatorType.ValueEquality, right);
		}

		private static CodeStatement Stm(CodeExpression expr)
		{
			return new CodeExpressionStatement(expr);
		}

		private static CodeStatement Return(CodeExpression expr)
		{
			return new CodeMethodReturnStatement(expr);
		}

		private static CodeStatement Return()
		{
			return new CodeMethodReturnStatement();
		}

		private static CodeStatement Assign(CodeExpression left, CodeExpression right)
		{
			return new CodeAssignStatement(left, right);
		}

		private static CodeStatement Throw(Type exception, string arg, string inner)
		{
			return new CodeThrowExceptionStatement(TypedDataSetGenerator.New(exception, new CodeExpression[]
			{
				TypedDataSetGenerator.Str(Res.GetString(arg)),
				TypedDataSetGenerator.Variable(inner)
			}));
		}

		private static CodeStatement If(CodeExpression cond, CodeStatement[] trueStms, CodeStatement[] falseStms)
		{
			return new CodeConditionStatement(cond, trueStms, falseStms);
		}

		private static CodeStatement If(CodeExpression cond, CodeStatement[] trueStms)
		{
			return new CodeConditionStatement(cond, trueStms);
		}

		private static CodeStatement If(CodeExpression cond, CodeStatement trueStm)
		{
			return TypedDataSetGenerator.If(cond, new CodeStatement[] { trueStm });
		}

		private static CodeMemberField FieldDecl(string type, string name)
		{
			return new CodeMemberField(type, name);
		}

		private static CodeMemberField FieldDecl(Type type, string name)
		{
			return new CodeMemberField(type, name);
		}

		private static CodeMemberMethod Method(CodeTypeReference type, string name, MemberAttributes attributes)
		{
			return new CodeMemberMethod
			{
				ReturnType = type,
				Name = name,
				Attributes = attributes
			};
		}

		private static CodeMemberMethod MethodDecl(Type type, string name, MemberAttributes attributes)
		{
			return TypedDataSetGenerator.Method(TypedDataSetGenerator.Type(type), name, attributes);
		}

		private static CodeMemberMethod MethodDecl(string type, string name, MemberAttributes attributes)
		{
			return TypedDataSetGenerator.Method(TypedDataSetGenerator.Type(type), name, attributes);
		}

		private static CodeMemberProperty PropertyDecl(string type, string name, MemberAttributes attributes)
		{
			return new CodeMemberProperty
			{
				Type = TypedDataSetGenerator.Type(type),
				Name = name,
				Attributes = attributes
			};
		}

		private static CodeMemberProperty PropertyDecl(Type type, string name, MemberAttributes attributes)
		{
			return new CodeMemberProperty
			{
				Type = TypedDataSetGenerator.Type(type),
				Name = name,
				Attributes = attributes
			};
		}

		private static CodeStatement VariableDecl(Type type, string name)
		{
			return new CodeVariableDeclarationStatement(type, name);
		}

		private static CodeStatement VariableDecl(string type, string name, CodeExpression initExpr)
		{
			return new CodeVariableDeclarationStatement(type, name, initExpr);
		}

		private static CodeStatement VariableDecl(Type type, string name, CodeExpression initExpr)
		{
			return new CodeVariableDeclarationStatement(type, name, initExpr);
		}

		private static CodeMemberEvent EventDecl(string type, string name)
		{
			return new CodeMemberEvent
			{
				Name = name,
				Type = TypedDataSetGenerator.Type(type),
				Attributes = (MemberAttributes)24578
			};
		}

		private static CodeParameterDeclarationExpression ParameterDecl(string type, string name)
		{
			return new CodeParameterDeclarationExpression(type, name);
		}

		private static CodeParameterDeclarationExpression ParameterDecl(Type type, string name)
		{
			return new CodeParameterDeclarationExpression(type, name);
		}

		private static CodeAttributeDeclaration AttributeDecl(string name)
		{
			return new CodeAttributeDeclaration(name);
		}

		private static CodeAttributeDeclaration AttributeDecl(string name, CodeExpression value)
		{
			return new CodeAttributeDeclaration(name, new CodeAttributeArgument[]
			{
				new CodeAttributeArgument(value)
			});
		}

		private static CodeStatement Try(CodeStatement tryStmnt, CodeCatchClause catchClause)
		{
			return new CodeTryCatchFinallyStatement(new CodeStatement[] { tryStmnt }, new CodeCatchClause[] { catchClause });
		}

		private static CodeCatchClause Catch(Type type, string name, CodeStatement catchStmnt)
		{
			return new CodeCatchClause
			{
				CatchExceptionType = TypedDataSetGenerator.Type(type),
				LocalName = name,
				Statements = { catchStmnt }
			};
		}

		private bool useExtendedNaming;

		private ICodeGenerator codeGen;

		private ArrayList errorList;

		private ArrayList conflictingTables;

		private Hashtable lookupIdentifiers;
	}
}
