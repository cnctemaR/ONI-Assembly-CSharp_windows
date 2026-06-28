using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeCatchClause
	{
		public CodeCatchClause()
		{
		}

		public CodeCatchClause(string localName)
		{
			this.localName = localName;
		}

		public CodeCatchClause(string localName, CodeTypeReference catchExceptionType)
		{
			this.localName = localName;
			this.catchExceptionType = catchExceptionType;
		}

		public CodeCatchClause(string localName, CodeTypeReference catchExceptionType, params CodeStatement[] statements)
		{
			this.localName = localName;
			this.catchExceptionType = catchExceptionType;
			this.Statements.AddRange(statements);
		}

		public CodeTypeReference CatchExceptionType
		{
			get
			{
				if (this.catchExceptionType == null)
				{
					this.catchExceptionType = new CodeTypeReference(typeof(Exception));
				}
				return this.catchExceptionType;
			}
			set
			{
				this.catchExceptionType = value;
			}
		}

		public string LocalName
		{
			get
			{
				if (this.localName == null)
				{
					return string.Empty;
				}
				return this.localName;
			}
			set
			{
				this.localName = value;
			}
		}

		public CodeStatementCollection Statements
		{
			get
			{
				if (this.statements == null)
				{
					this.statements = new CodeStatementCollection();
				}
				return this.statements;
			}
		}

		private CodeTypeReference catchExceptionType;

		private string localName;

		private CodeStatementCollection statements;
	}
}
