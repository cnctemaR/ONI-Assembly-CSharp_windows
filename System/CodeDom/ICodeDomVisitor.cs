using System;

namespace System.CodeDom
{
	internal interface ICodeDomVisitor
	{
		void Visit(CodeArgumentReferenceExpression o);

		void Visit(CodeArrayCreateExpression o);

		void Visit(CodeArrayIndexerExpression o);

		void Visit(CodeBaseReferenceExpression o);

		void Visit(CodeBinaryOperatorExpression o);

		void Visit(CodeCastExpression o);

		void Visit(CodeDefaultValueExpression o);

		void Visit(CodeDelegateCreateExpression o);

		void Visit(CodeDelegateInvokeExpression o);

		void Visit(CodeDirectionExpression o);

		void Visit(CodeEventReferenceExpression o);

		void Visit(CodeFieldReferenceExpression o);

		void Visit(CodeIndexerExpression o);

		void Visit(CodeMethodInvokeExpression o);

		void Visit(CodeMethodReferenceExpression o);

		void Visit(CodeObjectCreateExpression o);

		void Visit(CodeParameterDeclarationExpression o);

		void Visit(CodePrimitiveExpression o);

		void Visit(CodePropertyReferenceExpression o);

		void Visit(CodePropertySetValueReferenceExpression o);

		void Visit(CodeSnippetExpression o);

		void Visit(CodeThisReferenceExpression o);

		void Visit(CodeTypeOfExpression o);

		void Visit(CodeTypeReferenceExpression o);

		void Visit(CodeVariableReferenceExpression o);

		void Visit(CodeAssignStatement o);

		void Visit(CodeAttachEventStatement o);

		void Visit(CodeCommentStatement o);

		void Visit(CodeConditionStatement o);

		void Visit(CodeExpressionStatement o);

		void Visit(CodeGotoStatement o);

		void Visit(CodeIterationStatement o);

		void Visit(CodeLabeledStatement o);

		void Visit(CodeMethodReturnStatement o);

		void Visit(CodeRemoveEventStatement o);

		void Visit(CodeThrowExceptionStatement o);

		void Visit(CodeTryCatchFinallyStatement o);

		void Visit(CodeVariableDeclarationStatement o);

		void Visit(CodeConstructor o);

		void Visit(CodeEntryPointMethod o);

		void Visit(CodeMemberEvent o);

		void Visit(CodeMemberField o);

		void Visit(CodeMemberMethod o);

		void Visit(CodeMemberProperty o);

		void Visit(CodeSnippetTypeMember o);

		void Visit(CodeTypeConstructor o);
	}
}
