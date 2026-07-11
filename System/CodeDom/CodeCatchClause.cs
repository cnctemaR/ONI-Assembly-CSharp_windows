using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeCatchClause
	{
		public CodeCatchClause()
		{
		}

		public CodeCatchClause(string localName)
		{
			this._localName = localName;
		}

		public CodeCatchClause(string localName, CodeTypeReference catchExceptionType)
		{
			this._localName = localName;
			this._catchExceptionType = catchExceptionType;
		}

		public CodeCatchClause(string localName, CodeTypeReference catchExceptionType, params CodeStatement[] statements)
		{
			this._localName = localName;
			this._catchExceptionType = catchExceptionType;
			this.Statements.AddRange(statements);
		}

		public string LocalName
		{
			get
			{
				return this._localName ?? string.Empty;
			}
			set
			{
				this._localName = value;
			}
		}

		public CodeTypeReference CatchExceptionType
		{
			get
			{
				CodeTypeReference codeTypeReference;
				if ((codeTypeReference = this._catchExceptionType) == null)
				{
					codeTypeReference = (this._catchExceptionType = new CodeTypeReference(typeof(Exception)));
				}
				return codeTypeReference;
			}
			set
			{
				this._catchExceptionType = value;
			}
		}

		public CodeStatementCollection Statements
		{
			get
			{
				CodeStatementCollection codeStatementCollection;
				if ((codeStatementCollection = this._statements) == null)
				{
					codeStatementCollection = (this._statements = new CodeStatementCollection());
				}
				return codeStatementCollection;
			}
		}

		private CodeStatementCollection _statements;

		private CodeTypeReference _catchExceptionType;

		private string _localName;
	}
}
