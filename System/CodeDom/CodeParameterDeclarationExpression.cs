using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeParameterDeclarationExpression : CodeExpression
	{
		public CodeParameterDeclarationExpression()
		{
		}

		public CodeParameterDeclarationExpression(CodeTypeReference type, string name)
		{
			this.type = type;
			this.name = name;
		}

		public CodeParameterDeclarationExpression(string type, string name)
		{
			this.type = new CodeTypeReference(type);
			this.name = name;
		}

		public CodeParameterDeclarationExpression(Type type, string name)
		{
			this.type = new CodeTypeReference(type);
			this.name = name;
		}

		public CodeAttributeDeclarationCollection CustomAttributes
		{
			get
			{
				if (this.customAttributes == null)
				{
					this.customAttributes = new CodeAttributeDeclarationCollection();
				}
				return this.customAttributes;
			}
			set
			{
				this.customAttributes = value;
			}
		}

		public FieldDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					return string.Empty;
				}
				return this.name;
			}
			set
			{
				this.name = value;
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

		private CodeAttributeDeclarationCollection customAttributes;

		private FieldDirection direction;

		private string name;

		private CodeTypeReference type;
	}
}
