using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeMemberMethod : CodeTypeMember
	{
		public event EventHandler PopulateImplementationTypes;

		public event EventHandler PopulateParameters;

		public event EventHandler PopulateStatements;

		public CodeTypeReferenceCollection ImplementationTypes
		{
			get
			{
				if (this.implementationTypes == null)
				{
					this.implementationTypes = new CodeTypeReferenceCollection();
					if (this.PopulateImplementationTypes != null)
					{
						this.PopulateImplementationTypes(this, EventArgs.Empty);
					}
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
					if (this.PopulateParameters != null)
					{
						this.PopulateParameters(this, EventArgs.Empty);
					}
				}
				return this.parameters;
			}
		}

		public CodeTypeReference PrivateImplementationType
		{
			get
			{
				return this.privateImplements;
			}
			set
			{
				this.privateImplements = value;
			}
		}

		public CodeTypeReference ReturnType
		{
			get
			{
				if (this.returnType == null)
				{
					return new CodeTypeReference(typeof(void));
				}
				return this.returnType;
			}
			set
			{
				this.returnType = value;
			}
		}

		public CodeStatementCollection Statements
		{
			get
			{
				if (this.statements == null)
				{
					this.statements = new CodeStatementCollection();
					if (this.PopulateStatements != null)
					{
						this.PopulateStatements(this, EventArgs.Empty);
					}
				}
				return this.statements;
			}
		}

		public CodeAttributeDeclarationCollection ReturnTypeCustomAttributes
		{
			get
			{
				if (this.returnAttributes == null)
				{
					this.returnAttributes = new CodeAttributeDeclarationCollection();
				}
				return this.returnAttributes;
			}
		}

		[ComVisible(false)]
		public CodeTypeParameterCollection TypeParameters
		{
			get
			{
				if (this.typeParameters == null)
				{
					this.typeParameters = new CodeTypeParameterCollection();
				}
				return this.typeParameters;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeTypeReferenceCollection implementationTypes;

		private CodeParameterDeclarationExpressionCollection parameters;

		private CodeTypeReference privateImplements;

		private CodeTypeReference returnType;

		private CodeStatementCollection statements;

		private CodeAttributeDeclarationCollection returnAttributes;

		private int populated;

		private CodeTypeParameterCollection typeParameters;
	}
}
