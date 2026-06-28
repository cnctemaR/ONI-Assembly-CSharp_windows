using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data
{
	internal class Generator
	{
		public Generator(DataSet ds, CodeNamespace cns, ICodeGenerator codeGen, ClassGeneratorOptions options)
		{
			this.ds = ds;
			this.cns = cns;
			this.opts = options;
			this.cunit = null;
			if (this.opts == null)
			{
				this.opts = new ClassICodeGeneratorOptions(codeGen);
			}
		}

		public Generator(DataSet ds, CodeNamespace cns, CodeDomProvider codeProvider, ClassGeneratorOptions options)
		{
			this.ds = ds;
			this.cns = cns;
			this.opts = options;
			this.cunit = null;
			if (this.opts == null)
			{
				this.opts = new ClassCodeDomProviderOptions(codeProvider);
			}
		}

		public Generator(DataSet ds, CodeCompileUnit cunit, CodeNamespace cns, CodeDomProvider codeProvider, ClassGeneratorOptions options)
		{
			this.ds = ds;
			this.cns = cns;
			this.opts = options;
			this.cunit = cunit;
			if (this.opts == null)
			{
				this.opts = new ClassCodeDomProviderOptions(codeProvider);
			}
		}

		public void Run()
		{
			this.cns.Imports.Add(new CodeNamespaceImport("System"));
			this.cns.Imports.Add(new CodeNamespaceImport("System.Collections"));
			this.cns.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
			this.cns.Imports.Add(new CodeNamespaceImport("System.Data"));
			this.cns.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
			this.cns.Imports.Add(new CodeNamespaceImport("System.Xml"));
			CodeTypeDeclaration codeTypeDeclaration = this.GenerateDataSetType();
			this.cns.Types.Add(codeTypeDeclaration);
			foreach (object obj in this.ds.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				CodeTypeDeclaration codeTypeDeclaration2 = this.GenerateDataTableType(dataTable);
				CodeTypeDeclaration codeTypeDeclaration3 = this.GenerateDataRowType(dataTable);
				CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate(this.opts.TableDelegateName(dataTable.TableName));
				codeTypeDelegate.Parameters.Add(this.Param(typeof(object), "o"));
				codeTypeDelegate.Parameters.Add(this.Param(this.opts.EventArgsName(dataTable.TableName), "e"));
				CodeTypeDeclaration codeTypeDeclaration4 = this.GenerateEventType(dataTable);
				if (this.opts.MakeClassesInsideDataSet)
				{
					codeTypeDeclaration.Members.Add(codeTypeDeclaration2);
					codeTypeDeclaration.Members.Add(codeTypeDeclaration3);
					codeTypeDeclaration.Members.Add(codeTypeDelegate);
					codeTypeDeclaration.Members.Add(codeTypeDeclaration4);
				}
				else
				{
					this.cns.Types.Add(codeTypeDeclaration2);
					this.cns.Types.Add(codeTypeDeclaration3);
					this.cns.Types.Add(codeTypeDelegate);
					this.cns.Types.Add(codeTypeDeclaration4);
				}
			}
			if (this.cunit == null)
			{
				return;
			}
			TableAdapterSchemaInfo tableAdapterSchemaData = this.ds.TableAdapterSchemaData;
			if (tableAdapterSchemaData != null)
			{
				CodeNamespace codeNamespace = new CodeNamespace(this.opts.TableAdapterNSName(this.opts.DataSetName(this.ds.DataSetName)));
				CodeTypeDeclaration codeTypeDeclaration5 = this.GenerateTableAdapterType(tableAdapterSchemaData);
				codeNamespace.Types.Add(codeTypeDeclaration5);
				this.cunit.Namespaces.Add(codeNamespace);
			}
		}

		private CodeThisReferenceExpression This()
		{
			return new CodeThisReferenceExpression();
		}

		private CodeBaseReferenceExpression Base()
		{
			return new CodeBaseReferenceExpression();
		}

		private CodePrimitiveExpression Const(object value)
		{
			return new CodePrimitiveExpression(value);
		}

		private CodeTypeReference TypeRef(Type t)
		{
			return new CodeTypeReference(t);
		}

		private CodeTypeReference TypeRef(string name)
		{
			return new CodeTypeReference(name);
		}

		private CodeTypeReference TypeRefArray(Type t, int dimension)
		{
			return new CodeTypeReference(this.TypeRef(t), dimension);
		}

		private CodeTypeReference TypeRefArray(string name, int dimension)
		{
			return new CodeTypeReference(this.TypeRef(name), dimension);
		}

		private CodeParameterDeclarationExpression Param(string t, string name)
		{
			return new CodeParameterDeclarationExpression(t, name);
		}

		private CodeParameterDeclarationExpression Param(Type t, string name)
		{
			return new CodeParameterDeclarationExpression(t, name);
		}

		private CodeParameterDeclarationExpression Param(CodeTypeReference t, string name)
		{
			return new CodeParameterDeclarationExpression(t, name);
		}

		private CodeArgumentReferenceExpression ParamRef(string name)
		{
			return new CodeArgumentReferenceExpression(name);
		}

		private CodeCastExpression Cast(string t, CodeExpression exp)
		{
			return new CodeCastExpression(t, exp);
		}

		private CodeCastExpression Cast(Type t, CodeExpression exp)
		{
			return new CodeCastExpression(t, exp);
		}

		private CodeCastExpression Cast(CodeTypeReference t, CodeExpression exp)
		{
			return new CodeCastExpression(t, exp);
		}

		private CodeExpression New(Type t, params CodeExpression[] parameters)
		{
			return new CodeObjectCreateExpression(t, parameters);
		}

		private CodeExpression New(string t, params CodeExpression[] parameters)
		{
			return new CodeObjectCreateExpression(this.TypeRef(t), parameters);
		}

		private CodeExpression NewArray(Type t, params CodeExpression[] parameters)
		{
			return new CodeArrayCreateExpression(t, parameters);
		}

		private CodeExpression NewArray(Type t, int size)
		{
			return new CodeArrayCreateExpression(t, size);
		}

		private CodeVariableReferenceExpression Local(string name)
		{
			return new CodeVariableReferenceExpression(name);
		}

		private CodeFieldReferenceExpression FieldRef(string name)
		{
			return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), name);
		}

		private CodeFieldReferenceExpression FieldRef(CodeExpression exp, string name)
		{
			return new CodeFieldReferenceExpression(exp, name);
		}

		private CodePropertyReferenceExpression PropRef(string name)
		{
			return new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name);
		}

		private CodePropertyReferenceExpression PropRef(CodeExpression target, string name)
		{
			return new CodePropertyReferenceExpression(target, name);
		}

		private CodeIndexerExpression IndexerRef(CodeExpression target, CodeExpression parameters)
		{
			return new CodeIndexerExpression(target, new CodeExpression[] { parameters });
		}

		private CodeIndexerExpression IndexerRef(CodeExpression param)
		{
			return new CodeIndexerExpression(new CodeThisReferenceExpression(), new CodeExpression[] { param });
		}

		private CodeEventReferenceExpression EventRef(string name)
		{
			return new CodeEventReferenceExpression(new CodeThisReferenceExpression(), name);
		}

		private CodeEventReferenceExpression EventRef(CodeExpression target, string name)
		{
			return new CodeEventReferenceExpression(target, name);
		}

		private CodeMethodInvokeExpression MethodInvoke(string name, params CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), name, parameters);
		}

		private CodeMethodInvokeExpression MethodInvoke(CodeExpression target, string name, params CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(target, name, parameters);
		}

		private CodeBinaryOperatorExpression EqualsValue(CodeExpression exp1, CodeExpression exp2)
		{
			return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.ValueEquality, exp2);
		}

		private CodeBinaryOperatorExpression Equals(CodeExpression exp1, CodeExpression exp2)
		{
			return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.IdentityEquality, exp2);
		}

		private CodeBinaryOperatorExpression Inequals(CodeExpression exp1, CodeExpression exp2)
		{
			return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.IdentityInequality, exp2);
		}

		private CodeBinaryOperatorExpression GreaterThan(CodeExpression exp1, CodeExpression exp2)
		{
			return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.GreaterThan, exp2);
		}

		private CodeBinaryOperatorExpression LessThan(CodeExpression exp1, CodeExpression exp2)
		{
			return new CodeBinaryOperatorExpression(exp1, CodeBinaryOperatorType.LessThan, exp2);
		}

		private CodeBinaryOperatorExpression Compute(CodeExpression exp1, CodeExpression exp2, CodeBinaryOperatorType ops)
		{
			if (ops >= CodeBinaryOperatorType.Add && ops < CodeBinaryOperatorType.Assign)
			{
				return new CodeBinaryOperatorExpression(exp1, ops, exp2);
			}
			return null;
		}

		private CodeBinaryOperatorExpression BitOps(CodeExpression exp1, CodeExpression exp2, CodeBinaryOperatorType ops)
		{
			if (ops >= CodeBinaryOperatorType.BitwiseOr && ops <= CodeBinaryOperatorType.BitwiseAnd)
			{
				return new CodeBinaryOperatorExpression(exp1, ops, exp2);
			}
			return null;
		}

		private CodeBinaryOperatorExpression BooleanOps(CodeExpression exp1, CodeExpression exp2, CodeBinaryOperatorType ops)
		{
			if (ops >= CodeBinaryOperatorType.BooleanOr && ops <= CodeBinaryOperatorType.BooleanAnd)
			{
				return new CodeBinaryOperatorExpression(exp1, ops, exp2);
			}
			return null;
		}

		private CodeTypeReferenceExpression TypeRefExp(Type t)
		{
			return new CodeTypeReferenceExpression(t);
		}

		private CodeTypeOfExpression TypeOfRef(string name)
		{
			return new CodeTypeOfExpression(this.TypeRef(name));
		}

		private CodeExpressionStatement Eval(CodeExpression exp)
		{
			return new CodeExpressionStatement(exp);
		}

		private CodeAssignStatement Let(CodeExpression exp, CodeExpression value)
		{
			return new CodeAssignStatement(exp, value);
		}

		private CodeMethodReturnStatement Return(CodeExpression exp)
		{
			return new CodeMethodReturnStatement(exp);
		}

		private CodeVariableDeclarationStatement VarDecl(Type t, string name, CodeExpression init)
		{
			return new CodeVariableDeclarationStatement(t, name, init);
		}

		private CodeVariableDeclarationStatement VarDecl(string t, string name, CodeExpression init)
		{
			return new CodeVariableDeclarationStatement(t, name, init);
		}

		private CodeCommentStatement Comment(string comment)
		{
			return new CodeCommentStatement(comment);
		}

		private CodeThrowExceptionStatement Throw(CodeExpression exp)
		{
			return new CodeThrowExceptionStatement(exp);
		}

		private CodeTypeDeclaration GenerateDataSetType()
		{
			this.dsType = new CodeTypeDeclaration(this.opts.DataSetName(this.ds.DataSetName));
			this.dsType.BaseTypes.Add(this.TypeRef(typeof(DataSet)));
			this.dsType.BaseTypes.Add(this.TypeRef(typeof(IXmlSerializable)));
			this.dsType.Members.Add(this.CreateDataSetDefaultCtor());
			this.dsType.Members.Add(this.CreateDataSetSerializationCtor());
			this.dsType.Members.Add(this.CreateDataSetCloneMethod(this.dsType));
			this.dsType.Members.Add(this.CreateDataSetGetSchemaSerializable());
			this.dsType.Members.Add(this.CreateDataSetGetSchema());
			this.dsType.Members.Add(this.CreateDataSetInitializeClass());
			this.dsType.Members.Add(this.CreateDataSetInitializeFields());
			this.dsType.Members.Add(this.CreateDataSetSchemaChanged());
			foreach (object obj in this.ds.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				this.CreateDataSetTableMembers(this.dsType, dataTable);
			}
			foreach (object obj2 in this.ds.Relations)
			{
				DataRelation dataRelation = (DataRelation)obj2;
				this.CreateDataSetRelationMembers(this.dsType, dataRelation);
			}
			return this.dsType;
		}

		private CodeConstructor CreateDataSetDefaultCtor()
		{
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = MemberAttributes.Public;
			codeConstructor.Statements.Add(this.Eval(this.MethodInvoke("InitializeClass", new CodeExpression[0])));
			CodeVariableDeclarationStatement codeVariableDeclarationStatement = this.VarDecl(typeof(CollectionChangeEventHandler), "handler", this.New(typeof(CollectionChangeEventHandler), new CodeExpression[]
			{
				new CodeDelegateCreateExpression(new CodeTypeReference(typeof(CollectionChangeEventHandler)), new CodeThisReferenceExpression(), "SchemaChanged")
			}));
			codeConstructor.Statements.Add(codeVariableDeclarationStatement);
			codeConstructor.Statements.Add(new CodeAttachEventStatement(this.EventRef(this.PropRef("Tables"), "CollectionChanged"), this.Local("handler")));
			codeConstructor.Statements.Add(new CodeAttachEventStatement(this.EventRef(this.PropRef("Relations"), "CollectionChanged"), this.Local("handler")));
			return codeConstructor;
		}

		private CodeConstructor CreateDataSetSerializationCtor()
		{
			return new CodeConstructor
			{
				Attributes = MemberAttributes.Family,
				Parameters = 
				{
					this.Param(typeof(SerializationInfo), "info"),
					this.Param(typeof(StreamingContext), "ctx")
				},
				Statements = 
				{
					this.Comment("TODO: implement"),
					this.Throw(this.New(typeof(NotImplementedException), new CodeExpression[0]))
				}
			};
		}

		private CodeMemberMethod CreateDataSetCloneMethod(CodeTypeDeclaration dsType)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.ReturnType = this.TypeRef(typeof(DataSet));
			codeMemberMethod.Attributes = (MemberAttributes)24580;
			codeMemberMethod.Name = "Clone";
			CodeVariableReferenceExpression codeVariableReferenceExpression = this.Local("set");
			codeMemberMethod.Statements.Add(this.VarDecl(dsType.Name, "set", this.Cast(dsType.Name, this.MethodInvoke(this.Base(), "Clone", new CodeExpression[0]))));
			codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(codeVariableReferenceExpression, "InitializeFields", new CodeExpression[0])));
			codeMemberMethod.Statements.Add(this.Return(codeVariableReferenceExpression));
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateDataSetGetSchema()
		{
			return new CodeMemberMethod
			{
				PrivateImplementationType = this.TypeRef(typeof(IXmlSerializable)),
				Name = "GetSchema",
				ReturnType = this.TypeRef(typeof(XmlSchema)),
				Statements = { this.Return(this.MethodInvoke("GetSchemaSerializable", new CodeExpression[0])) }
			};
		}

		private CodeMemberMethod CreateDataSetGetSchemaSerializable()
		{
			return new CodeMemberMethod
			{
				Attributes = (MemberAttributes)12292,
				Name = "GetSchemaSerializable",
				ReturnType = this.TypeRef(typeof(XmlSchema)),
				Statements = 
				{
					this.VarDecl(typeof(StringWriter), "sw", this.New(typeof(StringWriter), new CodeExpression[0])),
					this.Eval(this.MethodInvoke("WriteXmlSchema", new CodeExpression[] { this.Local("sw") })),
					this.Return(this.MethodInvoke(this.TypeRefExp(typeof(XmlSchema)), "Read", new CodeExpression[]
					{
						this.New(typeof(XmlTextReader), new CodeExpression[] { this.New(typeof(StringReader), new CodeExpression[] { this.MethodInvoke(this.Local("sw"), "ToString", new CodeExpression[0]) }) }),
						this.Const(null)
					}))
				}
			};
		}

		private CodeMemberMethod CreateDataSetInitializeClass()
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "InitializeClass";
			codeMemberMethod.Attributes = MemberAttributes.Assembly;
			codeMemberMethod.Statements.Add(this.Let(this.PropRef("DataSetName"), this.Const(this.ds.DataSetName)));
			codeMemberMethod.Statements.Add(this.Let(this.PropRef("Prefix"), this.Const(this.ds.Prefix)));
			codeMemberMethod.Statements.Add(this.Let(this.PropRef("Namespace"), this.Const(this.ds.Namespace)));
			codeMemberMethod.Statements.Add(this.Let(this.PropRef("Locale"), this.New(typeof(CultureInfo), new CodeExpression[] { this.Const(this.ds.Locale.Name) })));
			codeMemberMethod.Statements.Add(this.Let(this.PropRef("CaseSensitive"), this.Const(this.ds.CaseSensitive)));
			codeMemberMethod.Statements.Add(this.Let(this.PropRef("EnforceConstraints"), this.Const(this.ds.EnforceConstraints)));
			foreach (object obj in this.ds.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				string text = "__table" + this.opts.TableMemberName(dataTable.TableName);
				string text2 = this.opts.TableTypeName(dataTable.TableName);
				codeMemberMethod.Statements.Add(this.Let(this.FieldRef(text), this.New(text2, new CodeExpression[0])));
				codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.PropRef("Tables"), "Add", new CodeExpression[] { this.FieldRef(text) })));
			}
			bool flag = false;
			bool flag2 = false;
			foreach (object obj2 in this.ds.Tables)
			{
				DataTable dataTable2 = (DataTable)obj2;
				string text3 = "__table" + this.opts.TableMemberName(dataTable2.TableName);
				foreach (object obj3 in dataTable2.Constraints)
				{
					Constraint constraint = (Constraint)obj3;
					UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
					if (uniqueConstraint != null)
					{
						if (!flag2)
						{
							codeMemberMethod.Statements.Add(this.VarDecl(typeof(UniqueConstraint), "uc", null));
							flag2 = true;
						}
						this.CreateUniqueKeyStatements(codeMemberMethod, uniqueConstraint, text3);
					}
				}
			}
			foreach (object obj4 in this.ds.Tables)
			{
				DataTable dataTable3 = (DataTable)obj4;
				string text4 = "__table" + this.opts.TableMemberName(dataTable3.TableName);
				foreach (object obj5 in dataTable3.Constraints)
				{
					Constraint constraint2 = (Constraint)obj5;
					ForeignKeyConstraint foreignKeyConstraint = constraint2 as ForeignKeyConstraint;
					if (foreignKeyConstraint != null)
					{
						if (!flag)
						{
							codeMemberMethod.Statements.Add(this.VarDecl(typeof(ForeignKeyConstraint), "fkc", null));
							flag = true;
						}
						string text5 = "__table" + this.opts.TableMemberName(foreignKeyConstraint.RelatedTable.TableName);
						this.CreateForeignKeyStatements(codeMemberMethod, foreignKeyConstraint, text4, text5);
					}
				}
			}
			foreach (object obj6 in this.ds.Relations)
			{
				DataRelation dataRelation = (DataRelation)obj6;
				string text6 = this.opts.RelationName(dataRelation.RelationName);
				ArrayList arrayList = new ArrayList();
				foreach (DataColumn dataColumn in dataRelation.ParentColumns)
				{
					arrayList.Add(this.IndexerRef(this.PropRef(this.FieldRef("__table" + this.opts.TableMemberName(dataRelation.ParentTable.TableName)), "Columns"), this.Const(dataColumn.ColumnName)));
				}
				ArrayList arrayList2 = new ArrayList();
				foreach (DataColumn dataColumn2 in dataRelation.ChildColumns)
				{
					arrayList2.Add(this.IndexerRef(this.PropRef(this.FieldRef("__table" + this.opts.TableMemberName(dataRelation.ChildTable.TableName)), "Columns"), this.Const(dataColumn2.ColumnName)));
				}
				string text7 = "__relation" + text6;
				codeMemberMethod.Statements.Add(this.Let(this.FieldRef(text7), this.New(typeof(DataRelation), new CodeExpression[]
				{
					this.Const(dataRelation.RelationName),
					this.NewArray(typeof(DataColumn), arrayList.ToArray(typeof(CodeExpression)) as CodeExpression[]),
					this.NewArray(typeof(DataColumn), arrayList2.ToArray(typeof(CodeExpression)) as CodeExpression[]),
					this.Const(false)
				})));
				codeMemberMethod.Statements.Add(this.Let(this.PropRef(this.FieldRef(text7), "Nested"), this.Const(dataRelation.Nested)));
				codeMemberMethod.Statements.Add(this.MethodInvoke(this.PropRef("Relations"), "Add", new CodeExpression[] { this.FieldRef(text7) }));
			}
			return codeMemberMethod;
		}

		private void CreateUniqueKeyStatements(CodeMemberMethod m, UniqueConstraint uc, string tableField)
		{
			ArrayList arrayList = new ArrayList();
			foreach (DataColumn dataColumn in uc.Columns)
			{
				arrayList.Add(this.IndexerRef(this.PropRef(this.FieldRef(tableField), "Columns"), this.Const(dataColumn.ColumnName)));
			}
			m.Statements.Add(this.Let(this.Local("uc"), this.New(typeof(UniqueConstraint), new CodeExpression[]
			{
				this.Const(uc.ConstraintName),
				this.NewArray(typeof(DataColumn), arrayList.ToArray(typeof(CodeExpression)) as CodeExpression[]),
				this.Const(uc.IsPrimaryKey)
			})));
			m.Statements.Add(this.MethodInvoke(this.PropRef(this.FieldRef(tableField), "Constraints"), "Add", new CodeExpression[] { this.Local("uc") }));
		}

		private void CreateForeignKeyStatements(CodeMemberMethod m, ForeignKeyConstraint fkc, string tableField, string rtableField)
		{
			ArrayList arrayList = new ArrayList();
			foreach (DataColumn dataColumn in fkc.RelatedColumns)
			{
				arrayList.Add(this.IndexerRef(this.PropRef(this.FieldRef(rtableField), "Columns"), this.Const(dataColumn.ColumnName)));
			}
			ArrayList arrayList2 = new ArrayList();
			foreach (DataColumn dataColumn2 in fkc.Columns)
			{
				arrayList2.Add(this.IndexerRef(this.PropRef(this.FieldRef(tableField), "Columns"), this.Const(dataColumn2.ColumnName)));
			}
			m.Statements.Add(this.Let(this.Local("fkc"), this.New(typeof(ForeignKeyConstraint), new CodeExpression[]
			{
				this.Const(fkc.ConstraintName),
				this.NewArray(typeof(DataColumn), arrayList.ToArray(typeof(CodeExpression)) as CodeExpression[]),
				this.NewArray(typeof(DataColumn), arrayList2.ToArray(typeof(CodeExpression)) as CodeExpression[])
			})));
			m.Statements.Add(this.Let(this.PropRef(this.Local("fkc"), "AcceptRejectRule"), this.FieldRef(this.TypeRefExp(typeof(AcceptRejectRule)), Enum.GetName(typeof(AcceptRejectRule), fkc.AcceptRejectRule))));
			m.Statements.Add(this.Let(this.PropRef(this.Local("fkc"), "DeleteRule"), this.FieldRef(this.TypeRefExp(typeof(Rule)), Enum.GetName(typeof(Rule), fkc.DeleteRule))));
			m.Statements.Add(this.Let(this.PropRef(this.Local("fkc"), "UpdateRule"), this.FieldRef(this.TypeRefExp(typeof(Rule)), Enum.GetName(typeof(Rule), fkc.UpdateRule))));
			m.Statements.Add(this.MethodInvoke(this.PropRef(this.FieldRef(tableField), "Constraints"), "Add", new CodeExpression[] { this.Local("fkc") }));
		}

		private CodeMemberMethod CreateDataSetInitializeFields()
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Attributes = MemberAttributes.Assembly;
			codeMemberMethod.Name = "InitializeFields";
			foreach (object obj in this.ds.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.FieldRef("__table" + this.opts.TableMemberName(dataTable.TableName)), "InitializeFields", new CodeExpression[0])));
			}
			foreach (object obj2 in this.ds.Relations)
			{
				DataRelation dataRelation = (DataRelation)obj2;
				codeMemberMethod.Statements.Add(this.Let(this.FieldRef("__relation" + this.opts.RelationName(dataRelation.RelationName)), this.IndexerRef(this.PropRef("Relations"), this.Const(dataRelation.RelationName))));
			}
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateDataSetSchemaChanged()
		{
			return new CodeMemberMethod
			{
				Name = "SchemaChanged",
				Parameters = 
				{
					this.Param(typeof(object), "sender"),
					this.Param(typeof(CollectionChangeEventArgs), "e")
				},
				Statements = 
				{
					new CodeConditionStatement(this.EqualsValue(this.PropRef(this.ParamRef("e"), "Action"), this.FieldRef(this.TypeRefExp(typeof(CollectionChangeAction)), "Remove")), new CodeStatement[] { this.Eval(this.MethodInvoke("InitializeFields", new CodeExpression[0])) }, new CodeStatement[0])
				}
			};
		}

		private void CreateDataSetTableMembers(CodeTypeDeclaration dsType, DataTable table)
		{
			string text = this.opts.TableTypeName(table.TableName);
			string text2 = this.opts.TableMemberName(table.TableName);
			CodeMemberField codeMemberField = new CodeMemberField();
			codeMemberField.Type = this.TypeRef(text);
			codeMemberField.Name = "__table" + text2;
			dsType.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Type = this.TypeRef(text);
			codeMemberProperty.Attributes = MemberAttributes.Public;
			codeMemberProperty.Name = ((!(text2 == table.TableName)) ? text2 : ("_" + text2));
			codeMemberProperty.HasSet = false;
			codeMemberProperty.GetStatements.Add(this.Return(this.FieldRef("__table" + text2)));
			dsType.Members.Add(codeMemberProperty);
		}

		private void CreateDataSetRelationMembers(CodeTypeDeclaration dsType, DataRelation relation)
		{
			string text = this.opts.RelationName(relation.RelationName);
			string text2 = "__relation" + text;
			CodeMemberField codeMemberField = new CodeMemberField();
			codeMemberField.Type = this.TypeRef(typeof(DataRelation));
			codeMemberField.Name = text2;
			dsType.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Type = this.TypeRef(typeof(DataRelation));
			codeMemberProperty.Attributes = MemberAttributes.Public;
			codeMemberProperty.Name = text;
			codeMemberProperty.HasSet = false;
			codeMemberProperty.GetStatements.Add(this.Return(this.FieldRef(text2)));
			dsType.Members.Add(codeMemberProperty);
		}

		private CodeTypeDeclaration GenerateDataTableType(DataTable dt)
		{
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
			codeTypeDeclaration.Name = this.opts.TableTypeName(dt.TableName);
			codeTypeDeclaration.BaseTypes.Add(this.TypeRef(typeof(DataTable)));
			codeTypeDeclaration.BaseTypes.Add(this.TypeRef(typeof(IEnumerable)));
			codeTypeDeclaration.Members.Add(this.CreateTableCtor1(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableCtor2(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableCount(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableIndexer(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableInitializeClass(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableInitializeFields(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableGetEnumerator(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableClone(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableCreateInstance(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableAddRow1(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableAddRow2(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableNewRow(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableNewRowFromBuilder(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableRemoveRow(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableGetRowType(dt));
			codeTypeDeclaration.Members.Add(this.CreateTableEventStarter(dt, "Changing"));
			codeTypeDeclaration.Members.Add(this.CreateTableEventStarter(dt, "Changed"));
			codeTypeDeclaration.Members.Add(this.CreateTableEventStarter(dt, "Deleting"));
			codeTypeDeclaration.Members.Add(this.CreateTableEventStarter(dt, "Deleted"));
			codeTypeDeclaration.Members.Add(this.CreateTableEvent(dt, "RowChanging"));
			codeTypeDeclaration.Members.Add(this.CreateTableEvent(dt, "RowChanged"));
			codeTypeDeclaration.Members.Add(this.CreateTableEvent(dt, "RowDeleting"));
			codeTypeDeclaration.Members.Add(this.CreateTableEvent(dt, "RowDeleted"));
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				codeTypeDeclaration.Members.Add(this.CreateTableColumnField(dt, dataColumn));
				codeTypeDeclaration.Members.Add(this.CreateTableColumnProperty(dt, dataColumn));
			}
			return codeTypeDeclaration;
		}

		private CodeConstructor CreateTableCtor1(DataTable dt)
		{
			return new CodeConstructor
			{
				Attributes = MemberAttributes.Assembly,
				BaseConstructorArgs = { this.Const(dt.TableName) },
				Statements = 
				{
					this.Eval(this.MethodInvoke("InitializeClass", new CodeExpression[0])),
					this.Eval(this.MethodInvoke("InitializeFields", new CodeExpression[0]))
				}
			};
		}

		private CodeConstructor CreateTableCtor2(DataTable dt)
		{
			return new CodeConstructor
			{
				Attributes = MemberAttributes.Assembly,
				Parameters = { this.Param(typeof(DataTable), this.GetRowTableFieldName(dt)) },
				BaseConstructorArgs = { this.PropRef(this.ParamRef(this.GetRowTableFieldName(dt)), "TableName") },
				Statements = 
				{
					this.Comment("TODO: implement"),
					this.Throw(this.New(typeof(NotImplementedException), new CodeExpression[0]))
				}
			};
		}

		private CodeMemberMethod CreateTableInitializeClass(DataTable dt)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "InitializeClass";
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.PropRef("Columns"), "Add", new CodeExpression[] { this.New(typeof(DataColumn), new CodeExpression[]
				{
					this.Const(dataColumn.ColumnName),
					new CodeTypeOfExpression(dataColumn.DataType)
				}) })));
			}
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateTableInitializeFields(DataTable dt)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "InitializeFields";
			codeMemberMethod.Attributes = MemberAttributes.Assembly;
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				string text = string.Format("__column{0}", this.opts.TableColName(dataColumn.ColumnName));
				codeMemberMethod.Statements.Add(this.Let(this.FieldRef(text), this.IndexerRef(this.PropRef("Columns"), this.Const(dataColumn.ColumnName))));
				if (!dataColumn.AllowDBNull)
				{
					codeMemberMethod.Statements.Add(this.Let(this.FieldRef(this.PropRef(text), "AllowDBNull"), this.Const(dataColumn.AllowDBNull)));
				}
				if (dataColumn.DefaultValue != null && dataColumn.DefaultValue.GetType() != typeof(DBNull))
				{
					codeMemberMethod.Statements.Add(this.Let(this.FieldRef(this.PropRef(text), "DefaultValue"), this.Const(dataColumn.DefaultValue)));
				}
				if (dataColumn.AutoIncrement)
				{
					codeMemberMethod.Statements.Add(this.Let(this.FieldRef(this.PropRef(text), "AutoIncrement"), this.Const(dataColumn.AutoIncrement)));
				}
				if (dataColumn.AutoIncrementSeed != 0L)
				{
					codeMemberMethod.Statements.Add(this.Let(this.FieldRef(this.PropRef(text), "AutoIncrementSeed"), this.Const(dataColumn.AutoIncrementSeed)));
				}
				if (dataColumn.AutoIncrementStep != 1L)
				{
					codeMemberMethod.Statements.Add(this.Let(this.FieldRef(this.PropRef(text), "AutoIncrementStep"), this.Const(dataColumn.AutoIncrementStep)));
				}
				if (dataColumn.ReadOnly)
				{
					codeMemberMethod.Statements.Add(this.Let(this.FieldRef(this.PropRef(text), "ReadOnly"), this.Const(dataColumn.ReadOnly)));
				}
			}
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateTableClone(DataTable dt)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "Clone";
			codeMemberMethod.Attributes = (MemberAttributes)24580;
			codeMemberMethod.ReturnType = this.TypeRef(typeof(DataTable));
			string text = this.opts.TableTypeName(dt.TableName);
			codeMemberMethod.Statements.Add(this.VarDecl(text, "t", this.Cast(text, this.MethodInvoke(this.Base(), "Clone", new CodeExpression[0]))));
			codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.Local("t"), "InitializeFields", new CodeExpression[0])));
			codeMemberMethod.Statements.Add(this.Return(this.Local("t")));
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateTableGetEnumerator(DataTable dt)
		{
			return new CodeMemberMethod
			{
				Name = "GetEnumerator",
				Attributes = MemberAttributes.Public,
				ReturnType = this.TypeRef(typeof(IEnumerator)),
				Statements = { this.Return(this.MethodInvoke(this.PropRef("Rows"), "GetEnumerator", new CodeExpression[0])) },
				ImplementationTypes = { this.TypeRef(typeof(IEnumerable)) }
			};
		}

		private CodeMemberMethod CreateTableCreateInstance(DataTable dt)
		{
			return new CodeMemberMethod
			{
				Name = "CreateInstance",
				Attributes = (MemberAttributes)12292,
				ReturnType = this.TypeRef(typeof(DataTable)),
				Statements = { this.Return(this.New(this.opts.TableTypeName(dt.TableName), new CodeExpression[0])) }
			};
		}

		private CodeMemberField CreateTableColumnField(DataTable dt, DataColumn col)
		{
			return new CodeMemberField
			{
				Name = "__column" + this.opts.ColumnName(col.ColumnName),
				Type = this.TypeRef(typeof(DataColumn))
			};
		}

		private CodeMemberProperty CreateTableColumnProperty(DataTable dt, DataColumn col)
		{
			string text = this.opts.ColumnName(col.ColumnName);
			return new CodeMemberProperty
			{
				Name = text + "Column",
				Attributes = MemberAttributes.Assembly,
				Type = this.TypeRef(typeof(DataColumn)),
				HasSet = false,
				GetStatements = { this.Return(this.FieldRef("__column" + text)) }
			};
		}

		private CodeMemberProperty CreateTableCount(DataTable dt)
		{
			return new CodeMemberProperty
			{
				Name = "Count",
				Attributes = MemberAttributes.Public,
				Type = this.TypeRef(typeof(int)),
				HasSet = false,
				GetStatements = { this.Return(this.PropRef(this.PropRef("Rows"), "Count")) }
			};
		}

		private CodeMemberProperty CreateTableIndexer(DataTable dt)
		{
			string text = this.opts.RowName(dt.TableName);
			return new CodeMemberProperty
			{
				Name = "Item",
				Attributes = MemberAttributes.Public,
				Type = this.TypeRef(text),
				Parameters = { this.Param(typeof(int), "i") },
				HasSet = false,
				GetStatements = { this.Return(this.Cast(text, this.IndexerRef(this.PropRef("Rows"), this.ParamRef("i")))) }
			};
		}

		private CodeMemberMethod CreateTableAddRow1(DataTable dt)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			string text = this.opts.RowName(dt.TableName);
			codeMemberMethod.Name = "Add" + text;
			codeMemberMethod.Attributes = MemberAttributes.Public;
			codeMemberMethod.Parameters.Add(this.Param(this.TypeRef(text), "row"));
			codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.PropRef("Rows"), "Add", new CodeExpression[] { this.ParamRef("row") })));
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateTableAddRow2(DataTable dt)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			string text = this.opts.RowName(dt.TableName);
			codeMemberMethod.Name = "Add" + text;
			codeMemberMethod.ReturnType = this.TypeRef(text);
			codeMemberMethod.Attributes = MemberAttributes.Public;
			codeMemberMethod.Statements.Add(this.VarDecl(text, "row", this.MethodInvoke("New" + text, new CodeExpression[0])));
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.ColumnMapping == MappingType.Hidden)
				{
					foreach (object obj2 in dt.DataSet.Relations)
					{
						DataRelation dataRelation = (DataRelation)obj2;
						if (dataRelation.ChildTable == dt)
						{
							string text2 = this.opts.RowName(dataRelation.ParentTable.TableName);
							string text3 = text2;
							codeMemberMethod.Parameters.Add(this.Param(text2, text3));
							codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.Local("row"), "SetParentRow", new CodeExpression[]
							{
								this.ParamRef(text3),
								this.IndexerRef(this.PropRef(this.PropRef("DataSet"), "Relations"), this.Const(dataRelation.RelationName))
							})));
							break;
						}
					}
				}
				else
				{
					string text4 = this.opts.ColumnName(dataColumn.ColumnName);
					codeMemberMethod.Parameters.Add(this.Param(dataColumn.DataType, text4));
					codeMemberMethod.Statements.Add(this.Let(this.IndexerRef(this.Local("row"), this.Const(text4)), this.ParamRef(text4)));
				}
			}
			codeMemberMethod.Statements.Add(this.MethodInvoke(this.PropRef("Rows"), "Add", new CodeExpression[] { this.Local("row") }));
			codeMemberMethod.Statements.Add(this.Return(this.Local("row")));
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateTableNewRow(DataTable dt)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			string text = this.opts.RowName(dt.TableName);
			codeMemberMethod.Name = "New" + text;
			codeMemberMethod.ReturnType = this.TypeRef(text);
			codeMemberMethod.Attributes = MemberAttributes.Public;
			codeMemberMethod.Statements.Add(this.Return(this.Cast(text, this.MethodInvoke("NewRow", new CodeExpression[0]))));
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateTableNewRowFromBuilder(DataTable dt)
		{
			return new CodeMemberMethod
			{
				Name = "NewRowFromBuilder",
				Attributes = (MemberAttributes)12292,
				ReturnType = this.TypeRef(typeof(DataRow)),
				Parameters = { this.Param(typeof(DataRowBuilder), "builder") },
				Statements = { this.Return(this.New(this.opts.RowName(dt.TableName), new CodeExpression[] { this.ParamRef("builder") })) }
			};
		}

		private CodeMemberMethod CreateTableRemoveRow(DataTable dt)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			string text = this.opts.RowName(dt.TableName);
			codeMemberMethod.Name = "Remove" + text;
			codeMemberMethod.Attributes = MemberAttributes.Public;
			codeMemberMethod.Parameters.Add(this.Param(this.TypeRef(text), "row"));
			codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.PropRef("Rows"), "Remove", new CodeExpression[] { this.ParamRef("row") })));
			return codeMemberMethod;
		}

		private CodeMemberMethod CreateTableGetRowType(DataTable dt)
		{
			return new CodeMemberMethod
			{
				Name = "GetRowType",
				Attributes = (MemberAttributes)12292,
				ReturnType = this.TypeRef(typeof(Type)),
				Statements = { this.Return(new CodeTypeOfExpression(this.opts.RowName(dt.TableName))) }
			};
		}

		private CodeMemberMethod CreateTableEventStarter(DataTable dt, string type)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "OnRow" + type;
			codeMemberMethod.Attributes = (MemberAttributes)12292;
			codeMemberMethod.Parameters.Add(this.Param(typeof(DataRowChangeEventArgs), "e"));
			codeMemberMethod.Statements.Add(this.Eval(this.MethodInvoke(this.Base(), codeMemberMethod.Name, new CodeExpression[] { this.ParamRef("e") })));
			string text = this.opts.TableMemberName(dt.TableName) + "Row" + type;
			CodeStatement codeStatement = this.Eval(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(this.This(), text), new CodeExpression[]
			{
				this.This(),
				this.New(this.opts.EventArgsName(dt.TableName), new CodeExpression[]
				{
					this.Cast(this.opts.RowName(dt.TableName), this.PropRef(this.ParamRef("e"), "Row")),
					this.PropRef(this.ParamRef("e"), "Action")
				})
			}));
			codeMemberMethod.Statements.Add(new CodeConditionStatement(this.Inequals(this.EventRef(text), this.Const(null)), new CodeStatement[] { codeStatement }, new CodeStatement[0]));
			return codeMemberMethod;
		}

		private CodeMemberEvent CreateTableEvent(DataTable dt, string nameSuffix)
		{
			return new CodeMemberEvent
			{
				Attributes = MemberAttributes.Public,
				Name = this.opts.TableMemberName(dt.TableName) + nameSuffix,
				Type = this.TypeRef(this.opts.TableDelegateName(dt.TableName))
			};
		}

		public CodeTypeDeclaration GenerateDataRowType(DataTable dt)
		{
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
			codeTypeDeclaration.Name = this.opts.RowName(dt.TableName);
			codeTypeDeclaration.BaseTypes.Add(this.TypeRef(typeof(DataRow)));
			codeTypeDeclaration.Members.Add(this.CreateRowCtor(dt));
			codeTypeDeclaration.Members.Add(this.CreateRowTableField(dt));
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.ColumnMapping != MappingType.Hidden)
				{
					codeTypeDeclaration.Members.Add(this.CreateRowColumnProperty(dt, dataColumn));
					codeTypeDeclaration.Members.Add(this.CreateRowColumnIsNull(dt, dataColumn));
					codeTypeDeclaration.Members.Add(this.CreateRowColumnSetNull(dt, dataColumn));
				}
			}
			foreach (object obj2 in dt.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj2;
				codeTypeDeclaration.Members.Add(this.CreateRowParentRowProperty(dt, dataRelation));
			}
			foreach (object obj3 in dt.ChildRelations)
			{
				DataRelation dataRelation2 = (DataRelation)obj3;
				codeTypeDeclaration.Members.Add(this.CreateRowGetChildRows(dt, dataRelation2));
			}
			return codeTypeDeclaration;
		}

		private CodeConstructor CreateRowCtor(DataTable dt)
		{
			return new CodeConstructor
			{
				Attributes = MemberAttributes.Assembly,
				Parameters = { this.Param(typeof(DataRowBuilder), "builder") },
				BaseConstructorArgs = { this.ParamRef("builder") },
				Statements = { this.Let(this.FieldRef(this.GetRowTableFieldName(dt)), this.Cast(this.opts.TableTypeName(dt.TableName), this.PropRef("Table"))) }
			};
		}

		private string GetRowTableFieldName(DataTable dt)
		{
			return "table" + dt.TableName;
		}

		private CodeMemberField CreateRowTableField(DataTable dt)
		{
			return new CodeMemberField
			{
				Name = this.GetRowTableFieldName(dt),
				Type = this.TypeRef(this.opts.TableTypeName(dt.TableName))
			};
		}

		private CodeMemberProperty CreateRowColumnProperty(DataTable dt, DataColumn col)
		{
			return new CodeMemberProperty
			{
				Name = this.opts.ColumnName(col.ColumnName),
				Type = this.TypeRef(col.DataType),
				Attributes = MemberAttributes.Public,
				GetStatements = 
				{
					this.VarDecl(typeof(object), "ret", this.IndexerRef(this.PropRef(this.PropRef(this.GetRowTableFieldName(dt)), this.opts.TableColName(col.ColumnName) + "Column"))),
					new CodeConditionStatement(this.Equals(this.Local("ret"), this.PropRef(this.TypeRefExp(typeof(DBNull)), "Value")), new CodeStatement[] { this.Throw(this.New(typeof(StrongTypingException), new CodeExpression[]
					{
						this.Const("Cannot get strong typed value since it is DB null."),
						this.Const(null)
					})) }, new CodeStatement[] { this.Return(this.Cast(col.DataType, this.Local("ret"))) })
				},
				SetStatements = { this.Let(this.IndexerRef(this.PropRef(this.PropRef(this.GetRowTableFieldName(dt)), this.opts.TableColName(col.ColumnName) + "Column")), new CodePropertySetValueReferenceExpression()) }
			};
		}

		private CodeMemberMethod CreateRowColumnIsNull(DataTable dt, DataColumn col)
		{
			return new CodeMemberMethod
			{
				Name = "Is" + this.opts.ColumnName(col.ColumnName) + "Null",
				Attributes = MemberAttributes.Public,
				ReturnType = this.TypeRef(typeof(bool)),
				Statements = { this.Return(this.MethodInvoke("IsNull", new CodeExpression[] { this.PropRef(this.PropRef(this.GetRowTableFieldName(dt)), this.opts.TableColName(col.ColumnName) + "Column") })) }
			};
		}

		private CodeMemberMethod CreateRowColumnSetNull(DataTable dt, DataColumn col)
		{
			return new CodeMemberMethod
			{
				Name = "Set" + this.opts.ColumnName(col.ColumnName) + "Null",
				Attributes = MemberAttributes.Public,
				Statements = { this.Let(this.IndexerRef(this.PropRef(this.PropRef(this.GetRowTableFieldName(dt)), this.opts.TableColName(col.ColumnName) + "Column")), this.PropRef(this.TypeRefExp(typeof(DBNull)), "Value")) }
			};
		}

		private CodeMemberProperty CreateRowParentRowProperty(DataTable dt, DataRelation rel)
		{
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Name = this.opts.TableMemberName(rel.ParentTable.TableName) + "Row" + ((!(rel.ParentTable.TableName == rel.ChildTable.TableName)) ? string.Empty : "Parent");
			codeMemberProperty.Attributes = MemberAttributes.Public;
			codeMemberProperty.Type = this.TypeRef(this.opts.RowName(rel.ParentTable.TableName));
			codeMemberProperty.GetStatements.Add(this.Return(this.Cast(codeMemberProperty.Type, this.MethodInvoke("GetParentRow", new CodeExpression[] { this.IndexerRef(this.PropRef(this.PropRef(this.PropRef("Table"), "DataSet"), "Relations"), this.Const(rel.RelationName)) }))));
			codeMemberProperty.SetStatements.Add(this.Eval(this.MethodInvoke("SetParentRow", new CodeExpression[]
			{
				new CodePropertySetValueReferenceExpression(),
				this.IndexerRef(this.PropRef(this.PropRef(this.PropRef("Table"), "DataSet"), "Relations"), this.Const(rel.RelationName))
			})));
			return codeMemberProperty;
		}

		private CodeMemberMethod CreateRowGetChildRows(DataTable dt, DataRelation rel)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "Get" + this.opts.TableMemberName(rel.ChildTable.TableName) + "Rows";
			codeMemberMethod.Attributes = MemberAttributes.Public;
			codeMemberMethod.ReturnType = new CodeTypeReference(this.opts.RowName(rel.ChildTable.TableName), 1);
			codeMemberMethod.Statements.Add(this.Return(this.Cast(codeMemberMethod.ReturnType, this.MethodInvoke("GetChildRows", new CodeExpression[] { this.IndexerRef(this.PropRef(this.PropRef(this.PropRef("Table"), "DataSet"), "Relations"), this.Const(rel.RelationName)) }))));
			return codeMemberMethod;
		}

		private CodeTypeDeclaration GenerateEventType(DataTable dt)
		{
			return new CodeTypeDeclaration
			{
				Name = this.opts.EventArgsName(dt.TableName),
				BaseTypes = { this.TypeRef(typeof(EventArgs)) },
				Attributes = MemberAttributes.Public,
				Members = 
				{
					new CodeMemberField(this.TypeRef(this.opts.RowName(dt.TableName)), "eventRow"),
					new CodeMemberField(this.TypeRef(typeof(DataRowAction)), "eventAction"),
					this.CreateEventCtor(dt),
					this.CreateEventRow(dt),
					this.CreateEventAction(dt)
				}
			};
		}

		private CodeConstructor CreateEventCtor(DataTable dt)
		{
			return new CodeConstructor
			{
				Attributes = MemberAttributes.Public,
				Parameters = 
				{
					this.Param(this.TypeRef(this.opts.RowName(dt.TableName)), "r"),
					this.Param(this.TypeRef(typeof(DataRowAction)), "a")
				},
				Statements = 
				{
					this.Let(this.FieldRef("eventRow"), this.ParamRef("r")),
					this.Let(this.FieldRef("eventAction"), this.ParamRef("a"))
				}
			};
		}

		private CodeMemberProperty CreateEventRow(DataTable dt)
		{
			return new CodeMemberProperty
			{
				Name = "Row",
				Attributes = (MemberAttributes)24578,
				Type = this.TypeRef(this.opts.RowName(dt.TableName)),
				HasSet = false,
				GetStatements = { this.Return(this.FieldRef("eventRow")) }
			};
		}

		private CodeMemberProperty CreateEventAction(DataTable dt)
		{
			return new CodeMemberProperty
			{
				Name = "Action",
				Attributes = (MemberAttributes)24578,
				Type = this.TypeRef(typeof(DataRowAction)),
				HasSet = false,
				GetStatements = { this.Return(this.FieldRef("eventAction")) }
			};
		}

		private CodeTypeDeclaration GenerateTableAdapterType(TableAdapterSchemaInfo taInfo)
		{
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
			codeTypeDeclaration.Name = this.opts.TableAdapterName(taInfo.Name);
			codeTypeDeclaration.BaseTypes.Add(this.TypeRef(taInfo.BaseClass));
			codeTypeDeclaration.Members.Add(this.CreateTableAdapterDefaultCtor());
			this.CreateDBAdapterFieldAndProperty(codeTypeDeclaration, taInfo.Adapter);
			this.CreateDBConnectionFieldAndProperty(codeTypeDeclaration, taInfo.Connection);
			DbCommand dbCommand;
			if (taInfo.Commands.Count > 0)
			{
				dbCommand = ((DbCommandInfo)taInfo.Commands[0]).Command;
			}
			else
			{
				dbCommand = taInfo.Provider.CreateCommand();
			}
			this.CreateDBCommandCollectionFieldAndProperty(codeTypeDeclaration, dbCommand);
			this.CreateAdapterClearBeforeFillFieldAndProperty(codeTypeDeclaration);
			this.CreateAdapterInitializeMethod(codeTypeDeclaration, taInfo);
			this.CreateConnectionInitializeMethod(codeTypeDeclaration, taInfo);
			this.CreateCommandCollectionInitializeMethod(codeTypeDeclaration, taInfo);
			this.CreateDbSourceMethods(codeTypeDeclaration, taInfo);
			if (taInfo.ShortCommands)
			{
				this.CreateShortCommandMethods(codeTypeDeclaration, taInfo);
			}
			return codeTypeDeclaration;
		}

		private CodeConstructor CreateTableAdapterDefaultCtor()
		{
			return new CodeConstructor
			{
				Attributes = MemberAttributes.Public,
				Statements = { this.Let(this.PropRef("ClearBeforeFill"), this.Const(true)) }
			};
		}

		private void CreateDBAdapterFieldAndProperty(CodeTypeDeclaration t, DbDataAdapter adapter)
		{
			CodeMemberField codeMemberField = new CodeMemberField();
			codeMemberField.Name = "_adapter";
			codeMemberField.Type = this.TypeRef(adapter.GetType());
			t.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Name = "Adapter";
			codeMemberProperty.Attributes = MemberAttributes.Private;
			codeMemberProperty.Type = codeMemberField.Type;
			codeMemberProperty.HasSet = false;
			CodeExpression codeExpression = this.FieldRef("_adapter");
			CodeStatement codeStatement = this.Eval(this.MethodInvoke("InitAdapter", new CodeExpression[0]));
			CodeStatement codeStatement2 = new CodeConditionStatement(this.Equals(codeExpression, this.Const(null)), new CodeStatement[] { codeStatement }, new CodeStatement[0]);
			codeMemberProperty.GetStatements.Add(codeStatement2);
			codeMemberProperty.GetStatements.Add(this.Return(codeExpression));
			t.Members.Add(codeMemberProperty);
		}

		private void CreateDBConnectionFieldAndProperty(CodeTypeDeclaration t, DbConnection conn)
		{
			CodeMemberField codeMemberField = new CodeMemberField();
			codeMemberField.Name = "_connection";
			codeMemberField.Type = this.TypeRef(conn.GetType());
			t.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Name = "Connection";
			codeMemberProperty.Attributes = MemberAttributes.Assembly;
			codeMemberProperty.Type = codeMemberField.Type;
			CodeExpression codeExpression = this.FieldRef("_connection");
			CodeStatement codeStatement = this.Eval(this.MethodInvoke("InitConnection", new CodeExpression[0]));
			CodeStatement codeStatement2 = new CodeConditionStatement(this.Equals(codeExpression, this.Const(null)), new CodeStatement[] { codeStatement }, new CodeStatement[0]);
			codeMemberProperty.GetStatements.Add(codeStatement2);
			codeMemberProperty.GetStatements.Add(this.Return(codeExpression));
			codeMemberProperty.SetStatements.Add(this.Let(codeExpression, new CodePropertySetValueReferenceExpression()));
			string text = "InsertCommand";
			string text2 = "Connection";
			codeExpression = this.PropRef(this.PropRef("Adapter"), text);
			codeStatement = this.Let(this.PropRef(codeExpression, text2), new CodePropertySetValueReferenceExpression());
			codeStatement2 = new CodeConditionStatement(this.Inequals(codeExpression, this.Const(null)), new CodeStatement[] { codeStatement }, new CodeStatement[0]);
			codeMemberProperty.SetStatements.Add(codeStatement2);
			text = "DeleteCommand";
			codeExpression = this.PropRef(this.PropRef("Adapter"), text);
			codeStatement = this.Let(this.PropRef(codeExpression, text2), new CodePropertySetValueReferenceExpression());
			codeStatement2 = new CodeConditionStatement(this.Inequals(codeExpression, this.Const(null)), new CodeStatement[] { codeStatement }, new CodeStatement[0]);
			codeMemberProperty.SetStatements.Add(codeStatement2);
			text = "UpdateCommand";
			codeExpression = this.PropRef(this.PropRef("Adapter"), text);
			codeStatement = this.Let(this.PropRef(codeExpression, text2), new CodePropertySetValueReferenceExpression());
			codeStatement2 = new CodeConditionStatement(this.Inequals(codeExpression, this.Const(null)), new CodeStatement[] { codeStatement }, new CodeStatement[0]);
			codeMemberProperty.SetStatements.Add(codeStatement2);
			codeStatement = this.VarDecl(typeof(int), "i", this.Const(0));
			codeExpression = this.LessThan(this.Local("i"), this.PropRef(this.PropRef("CommandCollection"), "Length"));
			codeStatement2 = this.Let(this.Local("i"), this.Compute(this.Local("i"), this.Const(1), CodeBinaryOperatorType.Add));
			CodeExpression codeExpression2 = this.IndexerRef(this.PropRef("CommandCollection"), this.Local("i"));
			CodeStatement codeStatement3 = this.Let(this.PropRef(codeExpression2, "Connection"), new CodePropertySetValueReferenceExpression());
			CodeStatement codeStatement4 = new CodeConditionStatement(this.Inequals(codeExpression2, this.Const(null)), new CodeStatement[] { codeStatement3 }, new CodeStatement[0]);
			CodeIterationStatement codeIterationStatement = new CodeIterationStatement(codeStatement, codeExpression, codeStatement2, new CodeStatement[] { codeStatement4 });
			codeMemberProperty.SetStatements.Add(codeIterationStatement);
			t.Members.Add(codeMemberProperty);
		}

		private void CreateDBCommandCollectionFieldAndProperty(CodeTypeDeclaration t, DbCommand cmd)
		{
			CodeMemberField codeMemberField = new CodeMemberField();
			codeMemberField.Name = "_commandCollection";
			codeMemberField.Type = this.TypeRefArray(cmd.GetType(), 1);
			t.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Name = "CommandCollection";
			codeMemberProperty.Attributes = MemberAttributes.Family;
			codeMemberProperty.Type = codeMemberField.Type;
			codeMemberProperty.HasSet = false;
			CodeExpression codeExpression = this.FieldRef("_commandCollection");
			CodeStatement codeStatement = this.Eval(this.MethodInvoke("InitCommandCollection", new CodeExpression[0]));
			CodeStatement codeStatement2 = new CodeConditionStatement(this.Equals(codeExpression, this.Const(null)), new CodeStatement[] { codeStatement }, new CodeStatement[0]);
			codeMemberProperty.GetStatements.Add(codeStatement2);
			codeMemberProperty.GetStatements.Add(this.Return(codeExpression));
			t.Members.Add(codeMemberProperty);
		}

		private void CreateAdapterClearBeforeFillFieldAndProperty(CodeTypeDeclaration t)
		{
			CodeMemberField codeMemberField = new CodeMemberField();
			codeMemberField.Name = "_clearBeforeFill";
			codeMemberField.Type = this.TypeRef(typeof(bool));
			t.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Name = "ClearBeforeFill";
			codeMemberProperty.Attributes = MemberAttributes.Public;
			codeMemberProperty.Type = codeMemberField.Type;
			codeMemberProperty.SetStatements.Add(this.Let(this.FieldRef("_clearBeforeFill"), new CodePropertySetValueReferenceExpression()));
			codeMemberProperty.GetStatements.Add(this.Return(this.FieldRef("_clearBeforeFill")));
			t.Members.Add(codeMemberProperty);
		}

		private void CreateAdapterInitializeMethod(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "InitAdapter";
			codeMemberMethod.Attributes = MemberAttributes.Private;
			CodeExpression codeExpression = this.FieldRef("_adapter");
			CodeStatement codeStatement = this.Let(codeExpression, this.New(taInfo.Adapter.GetType(), new CodeExpression[0]));
			codeMemberMethod.Statements.Add(codeStatement);
			codeStatement = this.VarDecl(typeof(DataTableMapping), "tableMapping", null);
			codeMemberMethod.Statements.Add(codeStatement);
			foreach (object obj in taInfo.Adapter.TableMappings)
			{
				DataTableMapping dataTableMapping = (DataTableMapping)obj;
				codeExpression = this.Local("tableMapping");
				codeStatement = this.Let(codeExpression, this.New(dataTableMapping.GetType(), new CodeExpression[0]));
				codeMemberMethod.Statements.Add(codeStatement);
				codeStatement = this.Let(this.PropRef(codeExpression, "SourceTable"), this.Const(dataTableMapping.SourceTable));
				codeMemberMethod.Statements.Add(codeStatement);
				codeStatement = this.Let(this.PropRef(codeExpression, "DataSetTable"), this.Const(dataTableMapping.DataSetTable));
				codeMemberMethod.Statements.Add(codeStatement);
				foreach (object obj2 in dataTableMapping.ColumnMappings)
				{
					DataColumnMapping dataColumnMapping = (DataColumnMapping)obj2;
					codeStatement = this.Eval(this.MethodInvoke(this.PropRef(codeExpression, "ColumnMappings"), "Add", new CodeExpression[]
					{
						this.Const(dataColumnMapping.SourceColumn),
						this.Const(dataColumnMapping.DataSetColumn)
					}));
					codeMemberMethod.Statements.Add(codeStatement);
				}
				codeExpression = this.PropRef(this.FieldRef("_adapter"), "TableMappings");
				codeStatement = this.Eval(this.MethodInvoke(codeExpression, "Add", new CodeExpression[] { this.Local("tableMapping") }));
				codeMemberMethod.Statements.Add(codeStatement);
			}
			codeExpression = this.PropRef(this.FieldRef("_adapter"), "DeleteCommand");
			DbCommand dbCommand = taInfo.Adapter.DeleteCommand;
			this.AddDbCommandStatements(codeMemberMethod, codeExpression, dbCommand);
			codeExpression = this.PropRef(this.FieldRef("_adapter"), "InsertCommand");
			dbCommand = taInfo.Adapter.InsertCommand;
			this.AddDbCommandStatements(codeMemberMethod, codeExpression, dbCommand);
			codeExpression = this.PropRef(this.FieldRef("_adapter"), "UpdateCommand");
			dbCommand = taInfo.Adapter.UpdateCommand;
			this.AddDbCommandStatements(codeMemberMethod, codeExpression, dbCommand);
			t.Members.Add(codeMemberMethod);
		}

		private void AddDbCommandStatements(CodeMemberMethod m, CodeExpression expr, DbCommand cmd)
		{
			if (cmd == null)
			{
				return;
			}
			CodeStatement codeStatement = this.Let(expr, this.New(cmd.GetType(), new CodeExpression[0]));
			m.Statements.Add(codeStatement);
			codeStatement = this.Let(this.PropRef(expr, "Connection"), this.PropRef("Connection"));
			m.Statements.Add(codeStatement);
			codeStatement = this.Let(this.PropRef(expr, "CommandText"), this.Const(cmd.CommandText));
			m.Statements.Add(codeStatement);
			CodeExpression codeExpression = this.PropRef(this.Local(typeof(CommandType).FullName), cmd.CommandType.ToString());
			codeStatement = this.Let(this.PropRef(expr, "CommandType"), codeExpression);
			m.Statements.Add(codeStatement);
			codeExpression = this.PropRef(expr, "Parameters");
			foreach (object obj in cmd.Parameters)
			{
				DbParameter dbParameter = (DbParameter)obj;
				this.AddDbParameterStatements(m, codeExpression, dbParameter);
			}
		}

		private void AddDbParameterStatements(CodeMemberMethod m, CodeExpression expr, DbParameter param)
		{
			object frameworkDbType = param.FrameworkDbType;
			string text = null;
			if (param.SourceColumn != string.Empty)
			{
				text = param.SourceColumn;
			}
			CodeExpression[] array = new CodeExpression[]
			{
				this.Const(param.ParameterName),
				this.PropRef(this.Local(frameworkDbType.GetType().FullName), frameworkDbType.ToString()),
				this.Const(param.Size),
				this.PropRef(this.Local(typeof(ParameterDirection).FullName), param.Direction.ToString()),
				this.Const(param.IsNullable),
				this.Const(((IDbDataParameter)param).Precision),
				this.Const(((IDbDataParameter)param).Scale),
				this.Const(text),
				this.PropRef(this.Local(typeof(DataRowVersion).FullName), param.SourceVersion.ToString()),
				this.Const(param.Value)
			};
			m.Statements.Add(this.Eval(this.MethodInvoke(expr, "Add", new CodeExpression[] { this.New(param.GetType(), array) })));
		}

		private void CreateConnectionInitializeMethod(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "InitConnection";
			codeMemberMethod.Attributes = MemberAttributes.Private;
			CodeExpression codeExpression = this.FieldRef("_connection");
			CodeStatement codeStatement = this.Let(codeExpression, this.New(taInfo.Connection.GetType(), new CodeExpression[0]));
			codeMemberMethod.Statements.Add(codeStatement);
			codeExpression = this.PropRef(this.FieldRef("_connection"), "ConnectionString");
			CodeExpression codeExpression2 = this.IndexerRef(this.PropRef(this.Local(typeof(ConfigurationManager).ToString()), "ConnectionStrings"), this.Const(taInfo.ConnectionString));
			codeStatement = this.Let(codeExpression, this.PropRef(codeExpression2, "ConnectionString"));
			codeMemberMethod.Statements.Add(codeStatement);
			t.Members.Add(codeMemberMethod);
		}

		private void CreateCommandCollectionInitializeMethod(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = "InitCommandCollection";
			codeMemberMethod.Attributes = MemberAttributes.Private;
			Type type = ((DbCommandInfo)taInfo.Commands[0]).Command.GetType();
			CodeExpression codeExpression = this.FieldRef("_commandCollection");
			CodeStatement codeStatement = this.Let(codeExpression, this.NewArray(type, taInfo.Commands.Count));
			codeMemberMethod.Statements.Add(codeStatement);
			for (int i = 0; i < taInfo.Commands.Count; i++)
			{
				DbCommand command = ((DbCommandInfo)taInfo.Commands[i]).Command;
				CodeExpression codeExpression2 = this.IndexerRef(codeExpression, this.Const(i));
				codeStatement = this.Let(codeExpression2, this.New(type, new CodeExpression[0]));
				codeMemberMethod.Statements.Add(codeStatement);
				codeStatement = this.Let(this.PropRef(codeExpression2, "Connection"), this.PropRef("Connection"));
				codeMemberMethod.Statements.Add(codeStatement);
				codeStatement = this.Let(this.PropRef(codeExpression2, "CommandText"), this.Const(command.CommandText));
				codeMemberMethod.Statements.Add(codeStatement);
				codeStatement = this.Let(this.PropRef(codeExpression2, "CommandType"), this.PropRef(this.Local(typeof(CommandType).FullName), command.CommandType.ToString()));
				codeMemberMethod.Statements.Add(codeStatement);
				codeExpression2 = this.PropRef(codeExpression2, "Parameters");
				foreach (object obj in command.Parameters)
				{
					DbParameter dbParameter = (DbParameter)obj;
					this.AddDbParameterStatements(codeMemberMethod, codeExpression2, dbParameter);
				}
			}
			t.Members.Add(codeMemberMethod);
		}

		private void CreateDbSourceMethods(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
		{
			string text = null;
			CodeExpression codeExpression = this.PropRef(this.PropRef("Adapter"), "SelectCommand");
			for (int i = 0; i < taInfo.Commands.Count; i++)
			{
				DbCommandInfo dbCommandInfo = (DbCommandInfo)taInfo.Commands[i];
				foreach (DbSourceMethodInfo dbSourceMethodInfo in dbCommandInfo.Methods)
				{
					if (dbSourceMethodInfo.MethodType != GenerateMethodsType.Fill)
					{
						CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
						codeMemberMethod.Name = dbSourceMethodInfo.Name;
						CodeStatement codeStatement = this.Let(codeExpression, this.IndexerRef(this.PropRef("CommandCollection"), this.Const(i)));
						codeMemberMethod.Statements.Add(codeStatement);
						MemberAttributes memberAttributes = (MemberAttributes)((int)Enum.Parse(typeof(MemberAttributes), dbSourceMethodInfo.Modifier));
						if (memberAttributes != MemberAttributes.Assembly)
						{
							if (memberAttributes != MemberAttributes.Family)
							{
								if (memberAttributes != MemberAttributes.Private)
								{
									if (memberAttributes == MemberAttributes.Public)
									{
										codeMemberMethod.Attributes = MemberAttributes.Public;
									}
								}
								else
								{
									codeMemberMethod.Attributes = MemberAttributes.Private;
								}
							}
							else
							{
								codeMemberMethod.Attributes = MemberAttributes.Family;
							}
						}
						else
						{
							codeMemberMethod.Attributes = MemberAttributes.Assembly;
						}
						QueryType queryType = (QueryType)((int)Enum.Parse(typeof(QueryType), dbSourceMethodInfo.QueryType));
						switch (queryType)
						{
						case QueryType.NoData:
						case QueryType.Scalar:
						{
							codeMemberMethod.ReturnType = this.TypeRef(typeof(int));
							this.AddGeneratedMethodParametersAndStatements(codeMemberMethod, codeExpression, dbCommandInfo.Command);
							string text2 = typeof(ConnectionState).FullName;
							CodeExpression codeExpression2 = this.PropRef(this.Local("command"), "Connection");
							CodeExpression codeExpression3 = this.PropRef(this.PropRef(this.Local("System"), "Data"), "ConnectionState");
							codeStatement = this.VarDecl(text2, "previousConnectionState", this.PropRef(codeExpression2, "State"));
							codeMemberMethod.Statements.Add(codeStatement);
							CodeExpression codeExpression4 = this.BitOps(this.PropRef(codeExpression2, "State"), this.PropRef(codeExpression3, "Open"), CodeBinaryOperatorType.BitwiseAnd);
							codeStatement = new CodeConditionStatement(this.Inequals(codeExpression4, this.PropRef(codeExpression3, "Open")), new CodeStatement[] { this.Eval(this.MethodInvoke(codeExpression2, "Open", new CodeExpression[0])) }, new CodeStatement[0]);
							codeMemberMethod.Statements.Add(codeStatement);
							CodeTryCatchFinallyStatement codeTryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
							if (queryType == QueryType.NoData)
							{
								codeMemberMethod.Statements.Add(this.VarDecl(typeof(int), "returnValue", null));
								codeExpression4 = this.MethodInvoke(this.Local("command"), "ExecuteNonQuery", new CodeExpression[0]);
							}
							else
							{
								text = dbSourceMethodInfo.ScalarCallRetval.Substring(0, dbSourceMethodInfo.ScalarCallRetval.IndexOf(','));
								codeMemberMethod.Statements.Add(this.VarDecl(this.TypeRef(text).BaseType, "returnValue", null));
								codeExpression4 = this.MethodInvoke(this.Local("command"), "ExecuteScalar", new CodeExpression[0]);
							}
							codeTryCatchFinallyStatement.TryStatements.Add(this.Let(this.Local("returnValue"), codeExpression4));
							codeStatement = new CodeConditionStatement(this.Equals(this.Local("previousConnectionState"), this.PropRef(codeExpression3, "Closed")), new CodeStatement[] { this.Eval(this.MethodInvoke(codeExpression2, "Close", new CodeExpression[0])) }, new CodeStatement[0]);
							codeTryCatchFinallyStatement.FinallyStatements.Add(codeStatement);
							codeMemberMethod.Statements.Add(codeTryCatchFinallyStatement);
							if (queryType == QueryType.NoData)
							{
								codeMemberMethod.Statements.Add(this.Return(this.Local("returnValue")));
							}
							else
							{
								codeExpression3 = this.Equals(this.Local("returnValue"), this.Const(null));
								codeExpression4 = this.Equals(this.MethodInvoke(this.Local("returnValue"), "GetType", new CodeExpression[0]), this.TypeOfRef("System.DBNull"));
								codeStatement = new CodeConditionStatement(this.BooleanOps(codeExpression3, codeExpression4, CodeBinaryOperatorType.BooleanOr), new CodeStatement[] { this.Return(this.Const(null)) }, new CodeStatement[] { this.Return(this.Cast(text, this.Local("returnValue"))) });
								codeMemberMethod.Statements.Add(codeStatement);
							}
							break;
						}
						case QueryType.Rowset:
						{
							string text2 = this.opts.DataSetName(this.ds.DataSetName) + "." + this.opts.TableTypeName(this.ds.Tables[0].TableName);
							codeMemberMethod.ReturnType = this.TypeRef(text2);
							this.AddGeneratedMethodParametersAndStatements(codeMemberMethod, codeExpression, dbCommandInfo.Command);
							codeStatement = this.VarDecl(text2, "dataTable", this.New(text2, new CodeExpression[0]));
							codeMemberMethod.Statements.Add(codeStatement);
							codeExpression = this.PropRef("Adapter");
							codeStatement = this.Eval(this.MethodInvoke(codeExpression, "Fill", new CodeExpression[] { this.Local("dataTable") }));
							codeMemberMethod.Statements.Add(codeStatement);
							codeMemberMethod.Statements.Add(this.Return(this.Local("dataTable")));
							break;
						}
						}
						t.Members.Add(codeMemberMethod);
					}
				}
			}
		}

		private void AddGeneratedMethodParametersAndStatements(CodeMemberMethod m, CodeExpression expr, DbCommand cmd)
		{
			int num = 0;
			foreach (object obj in cmd.Parameters)
			{
				DbParameter dbParameter = (DbParameter)obj;
				if (dbParameter.Direction != ParameterDirection.ReturnValue)
				{
					string text;
					if (dbParameter.ParameterName[0] == '@')
					{
						text = dbParameter.ParameterName.Substring(1);
					}
					else
					{
						text = dbParameter.ParameterName;
					}
					if (dbParameter.SystemType != null)
					{
						m.Parameters.Add(this.Param(this.TypeRef(dbParameter.SystemType), text));
					}
					CodeExpression codeExpression = this.IndexerRef(this.PropRef(expr, "Parameters"), this.Const(num));
					CodeStatement codeStatement = this.Let(codeExpression, this.ParamRef(text));
					m.Statements.Add(codeStatement);
				}
				num++;
			}
		}

		private void CreateShortCommandMethods(CodeTypeDeclaration t, TableAdapterSchemaInfo taInfo)
		{
		}

		private DataSet ds;

		private CodeNamespace cns;

		private ClassGeneratorOptions opts;

		private CodeCompileUnit cunit;

		private CodeTypeDeclaration dsType;
	}
}
