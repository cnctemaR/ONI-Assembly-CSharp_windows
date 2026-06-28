using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeMemberProperty : CodeTypeMember
	{
		public CodeStatementCollection GetStatements
		{
			get
			{
				if (this.getStatements == null)
				{
					this.getStatements = new CodeStatementCollection();
				}
				return this.getStatements;
			}
		}

		public bool HasGet
		{
			get
			{
				return this.hasGet || (this.getStatements != null && this.getStatements.Count > 0);
			}
			set
			{
				this.hasGet = value;
				if (!this.hasGet && this.getStatements != null)
				{
					this.getStatements.Clear();
				}
			}
		}

		public bool HasSet
		{
			get
			{
				return this.hasSet || (this.setStatements != null && this.setStatements.Count > 0);
			}
			set
			{
				this.hasSet = value;
				if (!this.hasSet && this.setStatements != null)
				{
					this.setStatements.Clear();
				}
			}
		}

		public CodeTypeReferenceCollection ImplementationTypes
		{
			get
			{
				if (this.implementationTypes == null)
				{
					this.implementationTypes = new CodeTypeReferenceCollection();
				}
				return this.implementationTypes;
			}
		}

		public CodeParameterDeclarationExpressionCollection Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new CodeParameterDeclarationExpressionCollection();
				}
				return this.parameters;
			}
		}

		public CodeTypeReference PrivateImplementationType
		{
			get
			{
				return this.privateImplementationType;
			}
			set
			{
				this.privateImplementationType = value;
			}
		}

		public CodeStatementCollection SetStatements
		{
			get
			{
				if (this.setStatements == null)
				{
					this.setStatements = new CodeStatementCollection();
				}
				return this.setStatements;
			}
		}

		public CodeTypeReference Type
		{
			get
			{
				if (this.type == null)
				{
					this.type = new CodeTypeReference(string.Empty);
				}
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeStatementCollection getStatements;

		private bool hasGet;

		private bool hasSet;

		private CodeTypeReferenceCollection implementationTypes;

		private CodeParameterDeclarationExpressionCollection parameters;

		private CodeTypeReference privateImplementationType;

		private CodeStatementCollection setStatements;

		private CodeTypeReference type;
	}
}
