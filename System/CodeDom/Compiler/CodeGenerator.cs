using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.CodeDom.Compiler
{
	public abstract class CodeGenerator : ICodeGenerator
	{
		protected CodeGenerator()
		{
			this.visitor = new CodeGenerator.Visitor(this);
		}

		string ICodeGenerator.CreateEscapedIdentifier(string value)
		{
			return this.CreateEscapedIdentifier(value);
		}

		string ICodeGenerator.CreateValidIdentifier(string value)
		{
			return this.CreateValidIdentifier(value);
		}

		void ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter output, CodeGeneratorOptions options)
		{
			this.InitOutput(output, options);
			if (compileUnit is CodeSnippetCompileUnit)
			{
				this.GenerateSnippetCompileUnit((CodeSnippetCompileUnit)compileUnit);
			}
			else
			{
				this.GenerateCompileUnit(compileUnit);
			}
		}

		void ICodeGenerator.GenerateCodeFromExpression(CodeExpression expression, TextWriter output, CodeGeneratorOptions options)
		{
			this.InitOutput(output, options);
			this.GenerateExpression(expression);
		}

		void ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace ns, TextWriter output, CodeGeneratorOptions options)
		{
			this.InitOutput(output, options);
			this.GenerateNamespace(ns);
		}

		void ICodeGenerator.GenerateCodeFromStatement(CodeStatement statement, TextWriter output, CodeGeneratorOptions options)
		{
			this.InitOutput(output, options);
			this.GenerateStatement(statement);
		}

		void ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration type, TextWriter output, CodeGeneratorOptions options)
		{
			this.InitOutput(output, options);
			this.GenerateType(type);
		}

		string ICodeGenerator.GetTypeOutput(CodeTypeReference type)
		{
			return this.GetTypeOutput(type);
		}

		bool ICodeGenerator.IsValidIdentifier(string value)
		{
			return this.IsValidIdentifier(value);
		}

		bool ICodeGenerator.Supports(GeneratorSupport value)
		{
			return this.Supports(value);
		}

		void ICodeGenerator.ValidateIdentifier(string value)
		{
			this.ValidateIdentifier(value);
		}

		protected CodeTypeDeclaration CurrentClass
		{
			get
			{
				return this.currentType;
			}
		}

		protected CodeTypeMember CurrentMember
		{
			get
			{
				return this.currentMember;
			}
		}

		protected string CurrentMemberName
		{
			get
			{
				if (this.currentMember == null)
				{
					return "<% unknown %>";
				}
				return this.currentMember.Name;
			}
		}

		protected string CurrentTypeName
		{
			get
			{
				if (this.currentType == null)
				{
					return "<% unknown %>";
				}
				return this.currentType.Name;
			}
		}

		protected int Indent
		{
			get
			{
				return this.output.Indent;
			}
			set
			{
				this.output.Indent = value;
			}
		}

		protected bool IsCurrentClass
		{
			get
			{
				return this.currentType != null && this.currentType.IsClass && !(this.currentType is CodeTypeDelegate);
			}
		}

		protected bool IsCurrentDelegate
		{
			get
			{
				return this.currentType is CodeTypeDelegate;
			}
		}

		protected bool IsCurrentEnum
		{
			get
			{
				return this.currentType != null && this.currentType.IsEnum;
			}
		}

		protected bool IsCurrentInterface
		{
			get
			{
				return this.currentType != null && this.currentType.IsInterface;
			}
		}

		protected bool IsCurrentStruct
		{
			get
			{
				return this.currentType != null && this.currentType.IsStruct;
			}
		}

		protected abstract string NullToken { get; }

		protected CodeGeneratorOptions Options
		{
			get
			{
				return this.options;
			}
		}

		protected TextWriter Output
		{
			get
			{
				return this.output;
			}
		}

		protected virtual void ContinueOnNewLine(string st)
		{
			this.output.WriteLine(st);
		}

		protected abstract void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e);

		protected abstract void GenerateArrayCreateExpression(CodeArrayCreateExpression e);

		protected abstract void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e);

		protected abstract void GenerateAssignStatement(CodeAssignStatement s);

		protected abstract void GenerateAttachEventStatement(CodeAttachEventStatement s);

		protected abstract void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes);

		protected abstract void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes);

		protected abstract void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e);

		protected virtual void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
		{
			this.output.Write('(');
			this.GenerateExpression(e.Left);
			this.output.Write(' ');
			this.OutputOperator(e.Operator);
			this.output.Write(' ');
			this.GenerateExpression(e.Right);
			this.output.Write(')');
		}

		protected abstract void GenerateCastExpression(CodeCastExpression e);

		[global::System.MonoTODO]
		public virtual void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
		{
			throw new NotImplementedException();
		}

		protected abstract void GenerateComment(CodeComment comment);

		protected virtual void GenerateCommentStatement(CodeCommentStatement statement)
		{
			this.GenerateComment(statement.Comment);
		}

		protected virtual void GenerateCommentStatements(CodeCommentStatementCollection statements)
		{
			foreach (object obj in statements)
			{
				CodeCommentStatement codeCommentStatement = (CodeCommentStatement)obj;
				this.GenerateCommentStatement(codeCommentStatement);
			}
		}

		protected virtual void GenerateCompileUnit(CodeCompileUnit compileUnit)
		{
			this.GenerateCompileUnitStart(compileUnit);
			CodeAttributeDeclarationCollection assemblyCustomAttributes = compileUnit.AssemblyCustomAttributes;
			if (assemblyCustomAttributes.Count != 0)
			{
				foreach (object obj in assemblyCustomAttributes)
				{
					CodeAttributeDeclaration codeAttributeDeclaration = (CodeAttributeDeclaration)obj;
					this.GenerateAttributeDeclarationsStart(assemblyCustomAttributes);
					this.output.Write("assembly: ");
					this.OutputAttributeDeclaration(codeAttributeDeclaration);
					this.GenerateAttributeDeclarationsEnd(assemblyCustomAttributes);
				}
				this.output.WriteLine();
			}
			foreach (object obj2 in compileUnit.Namespaces)
			{
				CodeNamespace codeNamespace = (CodeNamespace)obj2;
				this.GenerateNamespace(codeNamespace);
			}
			this.GenerateCompileUnitEnd(compileUnit);
		}

		protected virtual void GenerateCompileUnitEnd(CodeCompileUnit compileUnit)
		{
			if (compileUnit.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(compileUnit.EndDirectives);
			}
		}

		protected virtual void GenerateCompileUnitStart(CodeCompileUnit compileUnit)
		{
			if (compileUnit.StartDirectives.Count > 0)
			{
				this.GenerateDirectives(compileUnit.StartDirectives);
				this.Output.WriteLine();
			}
		}

		protected abstract void GenerateConditionStatement(CodeConditionStatement s);

		protected abstract void GenerateConstructor(CodeConstructor x, CodeTypeDeclaration d);

		protected virtual void GenerateDecimalValue(decimal d)
		{
			this.Output.Write(d.ToString(CultureInfo.InvariantCulture));
		}

		[global::System.MonoTODO]
		protected virtual void GenerateDefaultValueExpression(CodeDefaultValueExpression e)
		{
			throw new NotImplementedException();
		}

		protected abstract void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e);

		protected abstract void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e);

		protected virtual void GenerateDirectionExpression(CodeDirectionExpression e)
		{
			this.OutputDirection(e.Direction);
			this.output.Write(' ');
			this.GenerateExpression(e.Expression);
		}

		protected virtual void GenerateDoubleValue(double d)
		{
			this.Output.Write(d.ToString(CultureInfo.InvariantCulture));
		}

		protected abstract void GenerateEntryPointMethod(CodeEntryPointMethod m, CodeTypeDeclaration d);

		protected abstract void GenerateEvent(CodeMemberEvent ev, CodeTypeDeclaration d);

		protected abstract void GenerateEventReferenceExpression(CodeEventReferenceExpression e);

		protected void GenerateExpression(CodeExpression e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			try
			{
				e.Accept(this.visitor);
			}
			catch (NotImplementedException)
			{
				throw new ArgumentException("Element type " + e.GetType() + " is not supported.", "e");
			}
		}

		protected abstract void GenerateExpressionStatement(CodeExpressionStatement statement);

		protected abstract void GenerateField(CodeMemberField f);

		protected abstract void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e);

		protected abstract void GenerateGotoStatement(CodeGotoStatement statement);

		protected abstract void GenerateIndexerExpression(CodeIndexerExpression e);

		protected abstract void GenerateIterationStatement(CodeIterationStatement s);

		protected abstract void GenerateLabeledStatement(CodeLabeledStatement statement);

		protected abstract void GenerateLinePragmaStart(CodeLinePragma p);

		protected abstract void GenerateLinePragmaEnd(CodeLinePragma p);

		protected abstract void GenerateMethod(CodeMemberMethod m, CodeTypeDeclaration d);

		protected abstract void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e);

		protected abstract void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e);

		protected abstract void GenerateMethodReturnStatement(CodeMethodReturnStatement e);

		protected virtual void GenerateNamespace(CodeNamespace ns)
		{
			foreach (object obj in ns.Comments)
			{
				CodeCommentStatement codeCommentStatement = (CodeCommentStatement)obj;
				this.GenerateCommentStatement(codeCommentStatement);
			}
			this.GenerateNamespaceStart(ns);
			foreach (object obj2 in ns.Imports)
			{
				CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)obj2;
				if (codeNamespaceImport.LinePragma != null)
				{
					this.GenerateLinePragmaStart(codeNamespaceImport.LinePragma);
				}
				this.GenerateNamespaceImport(codeNamespaceImport);
				if (codeNamespaceImport.LinePragma != null)
				{
					this.GenerateLinePragmaEnd(codeNamespaceImport.LinePragma);
				}
			}
			this.output.WriteLine();
			this.GenerateTypes(ns);
			this.GenerateNamespaceEnd(ns);
		}

		protected abstract void GenerateNamespaceStart(CodeNamespace ns);

		protected abstract void GenerateNamespaceEnd(CodeNamespace ns);

		protected abstract void GenerateNamespaceImport(CodeNamespaceImport i);

		protected void GenerateNamespaceImports(CodeNamespace e)
		{
			foreach (object obj in e.Imports)
			{
				CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)obj;
				if (codeNamespaceImport.LinePragma != null)
				{
					this.GenerateLinePragmaStart(codeNamespaceImport.LinePragma);
				}
				this.GenerateNamespaceImport(codeNamespaceImport);
				if (codeNamespaceImport.LinePragma != null)
				{
					this.GenerateLinePragmaEnd(codeNamespaceImport.LinePragma);
				}
			}
		}

		protected void GenerateNamespaces(CodeCompileUnit e)
		{
			foreach (object obj in e.Namespaces)
			{
				CodeNamespace codeNamespace = (CodeNamespace)obj;
				this.GenerateNamespace(codeNamespace);
			}
		}

		protected abstract void GenerateObjectCreateExpression(CodeObjectCreateExpression e);

		protected virtual void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
		{
			if (e.CustomAttributes != null && e.CustomAttributes.Count > 0)
			{
				this.OutputAttributeDeclarations(e.CustomAttributes);
			}
			this.OutputDirection(e.Direction);
			this.OutputType(e.Type);
			this.output.Write(' ');
			this.output.Write(e.Name);
		}

		protected virtual void GeneratePrimitiveExpression(CodePrimitiveExpression e)
		{
			object value = e.Value;
			if (value == null)
			{
				this.output.Write(this.NullToken);
				return;
			}
			Type type = value.GetType();
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				this.output.Write(value.ToString().ToLower(CultureInfo.InvariantCulture));
				return;
			case TypeCode.Char:
				this.output.Write("'" + value.ToString() + "'");
				return;
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
				this.output.Write(((IFormattable)value).ToString(null, CultureInfo.InvariantCulture));
				return;
			case TypeCode.Single:
				this.GenerateSingleFloatValue((float)value);
				return;
			case TypeCode.Double:
				this.GenerateDoubleValue((double)value);
				return;
			case TypeCode.Decimal:
				this.GenerateDecimalValue((decimal)value);
				return;
			case TypeCode.String:
				this.output.Write(this.QuoteSnippetString((string)value));
				return;
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid Primitive Type: {0}. Only CLS compliant primitive types can be used. Consider using CodeObjectCreateExpression.", new object[] { type.FullName }));
		}

		protected abstract void GenerateProperty(CodeMemberProperty p, CodeTypeDeclaration d);

		protected abstract void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e);

		protected abstract void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e);

		protected abstract void GenerateRemoveEventStatement(CodeRemoveEventStatement statement);

		protected virtual void GenerateSingleFloatValue(float s)
		{
			this.output.Write(s.ToString(CultureInfo.InvariantCulture));
		}

		protected virtual void GenerateSnippetCompileUnit(CodeSnippetCompileUnit e)
		{
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaStart(e.LinePragma);
			}
			this.output.WriteLine(e.Value);
			if (e.LinePragma != null)
			{
				this.GenerateLinePragmaEnd(e.LinePragma);
			}
		}

		protected abstract void GenerateSnippetExpression(CodeSnippetExpression e);

		protected abstract void GenerateSnippetMember(CodeSnippetTypeMember m);

		protected virtual void GenerateSnippetStatement(CodeSnippetStatement s)
		{
			this.output.WriteLine(s.Value);
		}

		protected void GenerateStatement(CodeStatement s)
		{
			if (s.StartDirectives.Count > 0)
			{
				this.GenerateDirectives(s.StartDirectives);
			}
			if (s.LinePragma != null)
			{
				this.GenerateLinePragmaStart(s.LinePragma);
			}
			CodeSnippetStatement codeSnippetStatement = s as CodeSnippetStatement;
			if (codeSnippetStatement != null)
			{
				int indent = this.Indent;
				try
				{
					this.Indent = 0;
					this.GenerateSnippetStatement(codeSnippetStatement);
				}
				finally
				{
					this.Indent = indent;
				}
			}
			else
			{
				try
				{
					s.Accept(this.visitor);
				}
				catch (NotImplementedException)
				{
					throw new ArgumentException("Element type " + s.GetType() + " is not supported.", "s");
				}
			}
			if (s.LinePragma != null)
			{
				this.GenerateLinePragmaEnd(s.LinePragma);
			}
			if (s.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(s.EndDirectives);
			}
		}

		protected void GenerateStatements(CodeStatementCollection c)
		{
			foreach (object obj in c)
			{
				CodeStatement codeStatement = (CodeStatement)obj;
				this.GenerateStatement(codeStatement);
			}
		}

		protected abstract void GenerateThisReferenceExpression(CodeThisReferenceExpression e);

		protected abstract void GenerateThrowExceptionStatement(CodeThrowExceptionStatement s);

		protected abstract void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement s);

		protected abstract void GenerateTypeEnd(CodeTypeDeclaration declaration);

		protected abstract void GenerateTypeConstructor(CodeTypeConstructor constructor);

		protected virtual void GenerateTypeOfExpression(CodeTypeOfExpression e)
		{
			this.output.Write("typeof(");
			this.OutputType(e.Type);
			this.output.Write(")");
		}

		protected virtual void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e)
		{
			this.OutputType(e.Type);
		}

		protected void GenerateTypes(CodeNamespace e)
		{
			foreach (object obj in e.Types)
			{
				CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration)obj;
				if (this.options.BlankLinesBetweenMembers)
				{
					this.output.WriteLine();
				}
				this.GenerateType(codeTypeDeclaration);
			}
		}

		protected abstract void GenerateTypeStart(CodeTypeDeclaration declaration);

		protected abstract void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e);

		protected abstract void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e);

		protected virtual void OutputAttributeArgument(CodeAttributeArgument argument)
		{
			string name = argument.Name;
			if (name != null && name.Length > 0)
			{
				this.output.Write(name);
				this.output.Write('=');
			}
			this.GenerateExpression(argument.Value);
		}

		private void OutputAttributeDeclaration(CodeAttributeDeclaration attribute)
		{
			this.output.Write(attribute.Name.Replace('+', '.'));
			this.output.Write('(');
			IEnumerator enumerator = attribute.Arguments.GetEnumerator();
			if (enumerator.MoveNext())
			{
				CodeAttributeArgument codeAttributeArgument = (CodeAttributeArgument)enumerator.Current;
				this.OutputAttributeArgument(codeAttributeArgument);
				while (enumerator.MoveNext())
				{
					this.output.Write(',');
					codeAttributeArgument = (CodeAttributeArgument)enumerator.Current;
					this.OutputAttributeArgument(codeAttributeArgument);
				}
			}
			this.output.Write(')');
		}

		protected virtual void OutputAttributeDeclarations(CodeAttributeDeclarationCollection attributes)
		{
			this.GenerateAttributeDeclarationsStart(attributes);
			IEnumerator enumerator = attributes.GetEnumerator();
			if (enumerator.MoveNext())
			{
				CodeAttributeDeclaration codeAttributeDeclaration = (CodeAttributeDeclaration)enumerator.Current;
				this.OutputAttributeDeclaration(codeAttributeDeclaration);
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					codeAttributeDeclaration = (CodeAttributeDeclaration)obj;
					this.output.WriteLine(',');
					this.OutputAttributeDeclaration(codeAttributeDeclaration);
				}
			}
			this.GenerateAttributeDeclarationsEnd(attributes);
		}

		protected virtual void OutputDirection(FieldDirection direction)
		{
			switch (direction)
			{
			case FieldDirection.Out:
				this.output.Write("out ");
				break;
			case FieldDirection.Ref:
				this.output.Write("ref ");
				break;
			}
		}

		protected virtual void OutputExpressionList(CodeExpressionCollection expressions)
		{
			this.OutputExpressionList(expressions, false);
		}

		protected virtual void OutputExpressionList(CodeExpressionCollection expressions, bool newLineBetweenItems)
		{
			this.Indent++;
			IEnumerator enumerator = expressions.GetEnumerator();
			if (enumerator.MoveNext())
			{
				CodeExpression codeExpression = (CodeExpression)enumerator.Current;
				this.GenerateExpression(codeExpression);
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					codeExpression = (CodeExpression)obj;
					this.output.Write(',');
					if (newLineBetweenItems)
					{
						this.output.WriteLine();
					}
					else
					{
						this.output.Write(' ');
					}
					this.GenerateExpression(codeExpression);
				}
			}
			this.Indent--;
		}

		protected virtual void OutputFieldScopeModifier(MemberAttributes attributes)
		{
			if ((attributes & MemberAttributes.VTableMask) == MemberAttributes.New)
			{
				this.output.Write("new ");
			}
			switch (attributes & MemberAttributes.ScopeMask)
			{
			case MemberAttributes.Static:
				this.output.Write("static ");
				break;
			case MemberAttributes.Const:
				this.output.Write("const ");
				break;
			}
		}

		protected virtual void OutputIdentifier(string ident)
		{
			this.output.Write(ident);
		}

		protected virtual void OutputMemberAccessModifier(MemberAttributes attributes)
		{
			MemberAttributes memberAttributes = attributes & MemberAttributes.AccessMask;
			if (memberAttributes != MemberAttributes.Assembly)
			{
				if (memberAttributes != MemberAttributes.FamilyAndAssembly)
				{
					if (memberAttributes != MemberAttributes.Family)
					{
						if (memberAttributes != MemberAttributes.FamilyOrAssembly)
						{
							if (memberAttributes != MemberAttributes.Private)
							{
								if (memberAttributes == MemberAttributes.Public)
								{
									this.output.Write("public ");
								}
							}
							else
							{
								this.output.Write("private ");
							}
						}
						else
						{
							this.output.Write("protected internal ");
						}
					}
					else
					{
						this.output.Write("protected ");
					}
				}
				else
				{
					this.output.Write("internal ");
				}
			}
			else
			{
				this.output.Write("internal ");
			}
		}

		protected virtual void OutputMemberScopeModifier(MemberAttributes attributes)
		{
			if ((attributes & MemberAttributes.VTableMask) == MemberAttributes.New)
			{
				this.output.Write("new ");
			}
			switch (attributes & MemberAttributes.ScopeMask)
			{
			case MemberAttributes.Abstract:
				this.output.Write("abstract ");
				break;
			case MemberAttributes.Final:
				break;
			case MemberAttributes.Static:
				this.output.Write("static ");
				break;
			case MemberAttributes.Override:
				this.output.Write("override ");
				break;
			default:
			{
				MemberAttributes memberAttributes = attributes & MemberAttributes.AccessMask;
				if (memberAttributes == MemberAttributes.Public || memberAttributes == MemberAttributes.Family)
				{
					this.output.Write("virtual ");
				}
				break;
			}
			}
		}

		protected virtual void OutputOperator(CodeBinaryOperatorType op)
		{
			switch (op)
			{
			case CodeBinaryOperatorType.Add:
				this.output.Write("+");
				break;
			case CodeBinaryOperatorType.Subtract:
				this.output.Write("-");
				break;
			case CodeBinaryOperatorType.Multiply:
				this.output.Write("*");
				break;
			case CodeBinaryOperatorType.Divide:
				this.output.Write("/");
				break;
			case CodeBinaryOperatorType.Modulus:
				this.output.Write("%");
				break;
			case CodeBinaryOperatorType.Assign:
				this.output.Write("=");
				break;
			case CodeBinaryOperatorType.IdentityInequality:
				this.output.Write("!=");
				break;
			case CodeBinaryOperatorType.IdentityEquality:
				this.output.Write("==");
				break;
			case CodeBinaryOperatorType.ValueEquality:
				this.output.Write("==");
				break;
			case CodeBinaryOperatorType.BitwiseOr:
				this.output.Write("|");
				break;
			case CodeBinaryOperatorType.BitwiseAnd:
				this.output.Write("&");
				break;
			case CodeBinaryOperatorType.BooleanOr:
				this.output.Write("||");
				break;
			case CodeBinaryOperatorType.BooleanAnd:
				this.output.Write("&&");
				break;
			case CodeBinaryOperatorType.LessThan:
				this.output.Write("<");
				break;
			case CodeBinaryOperatorType.LessThanOrEqual:
				this.output.Write("<=");
				break;
			case CodeBinaryOperatorType.GreaterThan:
				this.output.Write(">");
				break;
			case CodeBinaryOperatorType.GreaterThanOrEqual:
				this.output.Write(">=");
				break;
			}
		}

		protected virtual void OutputParameters(CodeParameterDeclarationExpressionCollection parameters)
		{
			bool flag = true;
			foreach (object obj in parameters)
			{
				CodeParameterDeclarationExpression codeParameterDeclarationExpression = (CodeParameterDeclarationExpression)obj;
				if (flag)
				{
					flag = false;
				}
				else
				{
					this.output.Write(", ");
				}
				this.GenerateExpression(codeParameterDeclarationExpression);
			}
		}

		protected abstract void OutputType(CodeTypeReference t);

		protected virtual void OutputTypeAttributes(TypeAttributes attributes, bool isStruct, bool isEnum)
		{
			switch (attributes & TypeAttributes.VisibilityMask)
			{
			case TypeAttributes.Public:
			case TypeAttributes.NestedPublic:
				this.output.Write("public ");
				break;
			case TypeAttributes.NestedPrivate:
				this.output.Write("private ");
				break;
			}
			if (isStruct)
			{
				this.output.Write("struct ");
			}
			else if (isEnum)
			{
				this.output.Write("enum ");
			}
			else if ((attributes & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic)
			{
				this.output.Write("interface ");
			}
			else if (this.currentType is CodeTypeDelegate)
			{
				this.output.Write("delegate ");
			}
			else
			{
				if ((attributes & TypeAttributes.Sealed) != TypeAttributes.NotPublic)
				{
					this.output.Write("sealed ");
				}
				if ((attributes & TypeAttributes.Abstract) != TypeAttributes.NotPublic)
				{
					this.output.Write("abstract ");
				}
				this.output.Write("class ");
			}
		}

		protected virtual void OutputTypeNamePair(CodeTypeReference type, string name)
		{
			this.OutputType(type);
			this.output.Write(' ');
			this.output.Write(name);
		}

		protected abstract string QuoteSnippetString(string value);

		protected abstract string CreateEscapedIdentifier(string value);

		protected abstract string CreateValidIdentifier(string value);

		private void InitOutput(TextWriter output, CodeGeneratorOptions options)
		{
			if (options == null)
			{
				options = new CodeGeneratorOptions();
			}
			this.output = new IndentedTextWriter(output, options.IndentString);
			this.options = options;
		}

		private void GenerateType(CodeTypeDeclaration type)
		{
			this.currentType = type;
			this.currentMember = null;
			if (type.StartDirectives.Count > 0)
			{
				this.GenerateDirectives(type.StartDirectives);
			}
			foreach (object obj in type.Comments)
			{
				CodeCommentStatement codeCommentStatement = (CodeCommentStatement)obj;
				this.GenerateCommentStatement(codeCommentStatement);
			}
			if (type.LinePragma != null)
			{
				this.GenerateLinePragmaStart(type.LinePragma);
			}
			this.GenerateTypeStart(type);
			CodeTypeMember[] array = new CodeTypeMember[type.Members.Count];
			type.Members.CopyTo(array, 0);
			if (!this.Options.VerbatimOrder)
			{
				int[] array2 = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = Array.IndexOf<Type>(CodeGenerator.memberTypes, array[i].GetType()) * array.Length + i;
				}
				Array.Sort<int, CodeTypeMember>(array2, array);
			}
			CodeTypeDeclaration codeTypeDeclaration = null;
			foreach (CodeTypeMember codeTypeMember in array)
			{
				CodeTypeMember codeTypeMember2 = this.currentMember;
				this.currentMember = codeTypeMember;
				if (codeTypeMember2 != null && codeTypeDeclaration == null)
				{
					if (codeTypeMember2.LinePragma != null)
					{
						this.GenerateLinePragmaEnd(codeTypeMember2.LinePragma);
					}
					if (codeTypeMember2.EndDirectives.Count > 0)
					{
						this.GenerateDirectives(codeTypeMember2.EndDirectives);
					}
				}
				if (this.options.BlankLinesBetweenMembers)
				{
					this.output.WriteLine();
				}
				codeTypeDeclaration = codeTypeMember as CodeTypeDeclaration;
				if (codeTypeDeclaration != null)
				{
					this.GenerateType(codeTypeDeclaration);
					this.currentType = type;
				}
				else
				{
					if (this.currentMember.StartDirectives.Count > 0)
					{
						this.GenerateDirectives(this.currentMember.StartDirectives);
					}
					foreach (object obj2 in codeTypeMember.Comments)
					{
						CodeCommentStatement codeCommentStatement2 = (CodeCommentStatement)obj2;
						this.GenerateCommentStatement(codeCommentStatement2);
					}
					if (codeTypeMember.LinePragma != null)
					{
						this.GenerateLinePragmaStart(codeTypeMember.LinePragma);
					}
					try
					{
						codeTypeMember.Accept(this.visitor);
					}
					catch (NotImplementedException)
					{
						throw new ArgumentException("Element type " + codeTypeMember.GetType() + " is not supported.");
					}
				}
			}
			if (this.currentMember != null && !(this.currentMember is CodeTypeDeclaration))
			{
				if (this.currentMember.LinePragma != null)
				{
					this.GenerateLinePragmaEnd(this.currentMember.LinePragma);
				}
				if (this.currentMember.EndDirectives.Count > 0)
				{
					this.GenerateDirectives(this.currentMember.EndDirectives);
				}
			}
			this.currentType = type;
			this.GenerateTypeEnd(type);
			if (type.LinePragma != null)
			{
				this.GenerateLinePragmaEnd(type.LinePragma);
			}
			if (type.EndDirectives.Count > 0)
			{
				this.GenerateDirectives(type.EndDirectives);
			}
		}

		protected abstract string GetTypeOutput(CodeTypeReference type);

		protected abstract bool IsValidIdentifier(string value);

		public static bool IsValidLanguageIndependentIdentifier(string value)
		{
			if (value == null)
			{
				return false;
			}
			if (value.Equals(string.Empty))
			{
				return false;
			}
			UnicodeCategory unicodeCategory = char.GetUnicodeCategory(value[0]);
			switch (unicodeCategory)
			{
			case UnicodeCategory.UppercaseLetter:
			case UnicodeCategory.LowercaseLetter:
			case UnicodeCategory.TitlecaseLetter:
			case UnicodeCategory.ModifierLetter:
			case UnicodeCategory.OtherLetter:
			case UnicodeCategory.LetterNumber:
				break;
			default:
				if (unicodeCategory != UnicodeCategory.ConnectorPunctuation)
				{
					return false;
				}
				break;
			}
			int i = 1;
			while (i < value.Length)
			{
				switch (char.GetUnicodeCategory(value[i]))
				{
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.SpacingCombiningMark:
				case UnicodeCategory.DecimalDigitNumber:
				case UnicodeCategory.LetterNumber:
				case UnicodeCategory.Format:
				case UnicodeCategory.ConnectorPunctuation:
					i++;
					continue;
				}
				return false;
			}
			return true;
		}

		protected abstract bool Supports(GeneratorSupport supports);

		protected virtual void ValidateIdentifier(string value)
		{
			if (!this.IsValidIdentifier(value))
			{
				throw new ArgumentException("Identifier is invalid", "value");
			}
		}

		[global::System.MonoTODO]
		public static void ValidateIdentifiers(CodeObject e)
		{
			throw new NotImplementedException();
		}

		protected virtual void GenerateDirectives(CodeDirectiveCollection directives)
		{
		}

		private IndentedTextWriter output;

		private CodeGeneratorOptions options;

		private CodeTypeMember currentMember;

		private CodeTypeDeclaration currentType;

		private CodeGenerator.Visitor visitor;

		private static Type[] memberTypes = new Type[]
		{
			typeof(CodeMemberField),
			typeof(CodeSnippetTypeMember),
			typeof(CodeTypeConstructor),
			typeof(CodeConstructor),
			typeof(CodeMemberProperty),
			typeof(CodeMemberEvent),
			typeof(CodeMemberMethod),
			typeof(CodeTypeDeclaration),
			typeof(CodeEntryPointMethod)
		};

		internal class Visitor : ICodeDomVisitor
		{
			public Visitor(CodeGenerator generator)
			{
				this.g = generator;
			}

			public void Visit(CodeArgumentReferenceExpression o)
			{
				this.g.GenerateArgumentReferenceExpression(o);
			}

			public void Visit(CodeArrayCreateExpression o)
			{
				this.g.GenerateArrayCreateExpression(o);
			}

			public void Visit(CodeArrayIndexerExpression o)
			{
				this.g.GenerateArrayIndexerExpression(o);
			}

			public void Visit(CodeBaseReferenceExpression o)
			{
				this.g.GenerateBaseReferenceExpression(o);
			}

			public void Visit(CodeBinaryOperatorExpression o)
			{
				this.g.GenerateBinaryOperatorExpression(o);
			}

			public void Visit(CodeCastExpression o)
			{
				this.g.GenerateCastExpression(o);
			}

			public void Visit(CodeDefaultValueExpression o)
			{
				this.g.GenerateDefaultValueExpression(o);
			}

			public void Visit(CodeDelegateCreateExpression o)
			{
				this.g.GenerateDelegateCreateExpression(o);
			}

			public void Visit(CodeDelegateInvokeExpression o)
			{
				this.g.GenerateDelegateInvokeExpression(o);
			}

			public void Visit(CodeDirectionExpression o)
			{
				this.g.GenerateDirectionExpression(o);
			}

			public void Visit(CodeEventReferenceExpression o)
			{
				this.g.GenerateEventReferenceExpression(o);
			}

			public void Visit(CodeFieldReferenceExpression o)
			{
				this.g.GenerateFieldReferenceExpression(o);
			}

			public void Visit(CodeIndexerExpression o)
			{
				this.g.GenerateIndexerExpression(o);
			}

			public void Visit(CodeMethodInvokeExpression o)
			{
				this.g.GenerateMethodInvokeExpression(o);
			}

			public void Visit(CodeMethodReferenceExpression o)
			{
				this.g.GenerateMethodReferenceExpression(o);
			}

			public void Visit(CodeObjectCreateExpression o)
			{
				this.g.GenerateObjectCreateExpression(o);
			}

			public void Visit(CodeParameterDeclarationExpression o)
			{
				this.g.GenerateParameterDeclarationExpression(o);
			}

			public void Visit(CodePrimitiveExpression o)
			{
				this.g.GeneratePrimitiveExpression(o);
			}

			public void Visit(CodePropertyReferenceExpression o)
			{
				this.g.GeneratePropertyReferenceExpression(o);
			}

			public void Visit(CodePropertySetValueReferenceExpression o)
			{
				this.g.GeneratePropertySetValueReferenceExpression(o);
			}

			public void Visit(CodeSnippetExpression o)
			{
				this.g.GenerateSnippetExpression(o);
			}

			public void Visit(CodeThisReferenceExpression o)
			{
				this.g.GenerateThisReferenceExpression(o);
			}

			public void Visit(CodeTypeOfExpression o)
			{
				this.g.GenerateTypeOfExpression(o);
			}

			public void Visit(CodeTypeReferenceExpression o)
			{
				this.g.GenerateTypeReferenceExpression(o);
			}

			public void Visit(CodeVariableReferenceExpression o)
			{
				this.g.GenerateVariableReferenceExpression(o);
			}

			public void Visit(CodeAssignStatement o)
			{
				this.g.GenerateAssignStatement(o);
			}

			public void Visit(CodeAttachEventStatement o)
			{
				this.g.GenerateAttachEventStatement(o);
			}

			public void Visit(CodeCommentStatement o)
			{
				this.g.GenerateCommentStatement(o);
			}

			public void Visit(CodeConditionStatement o)
			{
				this.g.GenerateConditionStatement(o);
			}

			public void Visit(CodeExpressionStatement o)
			{
				this.g.GenerateExpressionStatement(o);
			}

			public void Visit(CodeGotoStatement o)
			{
				this.g.GenerateGotoStatement(o);
			}

			public void Visit(CodeIterationStatement o)
			{
				this.g.GenerateIterationStatement(o);
			}

			public void Visit(CodeLabeledStatement o)
			{
				this.g.GenerateLabeledStatement(o);
			}

			public void Visit(CodeMethodReturnStatement o)
			{
				this.g.GenerateMethodReturnStatement(o);
			}

			public void Visit(CodeRemoveEventStatement o)
			{
				this.g.GenerateRemoveEventStatement(o);
			}

			public void Visit(CodeThrowExceptionStatement o)
			{
				this.g.GenerateThrowExceptionStatement(o);
			}

			public void Visit(CodeTryCatchFinallyStatement o)
			{
				this.g.GenerateTryCatchFinallyStatement(o);
			}

			public void Visit(CodeVariableDeclarationStatement o)
			{
				this.g.GenerateVariableDeclarationStatement(o);
			}

			public void Visit(CodeConstructor o)
			{
				this.g.GenerateConstructor(o, this.g.CurrentClass);
			}

			public void Visit(CodeEntryPointMethod o)
			{
				this.g.GenerateEntryPointMethod(o, this.g.CurrentClass);
			}

			public void Visit(CodeMemberEvent o)
			{
				this.g.GenerateEvent(o, this.g.CurrentClass);
			}

			public void Visit(CodeMemberField o)
			{
				this.g.GenerateField(o);
			}

			public void Visit(CodeMemberMethod o)
			{
				this.g.GenerateMethod(o, this.g.CurrentClass);
			}

			public void Visit(CodeMemberProperty o)
			{
				this.g.GenerateProperty(o, this.g.CurrentClass);
			}

			public void Visit(CodeSnippetTypeMember o)
			{
				this.g.GenerateSnippetMember(o);
			}

			public void Visit(CodeTypeConstructor o)
			{
				this.g.GenerateTypeConstructor(o);
			}

			private CodeGenerator g;
		}
	}
}
