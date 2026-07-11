using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeAttributeDeclaration
	{
		public CodeAttributeDeclaration()
		{
		}

		public CodeAttributeDeclaration(string name)
		{
			this.Name = name;
		}

		public CodeAttributeDeclaration(string name, params CodeAttributeArgument[] arguments)
		{
			this.Name = name;
			this.Arguments.AddRange(arguments);
		}

		public CodeAttributeDeclaration(CodeTypeReference attributeType)
			: this(attributeType, null)
		{
		}

		public CodeAttributeDeclaration(CodeTypeReference attributeType, params CodeAttributeArgument[] arguments)
		{
			this._attributeType = attributeType;
			if (attributeType != null)
			{
				this._name = attributeType.BaseType;
			}
			if (arguments != null)
			{
				this.Arguments.AddRange(arguments);
			}
		}

		public string Name
		{
			get
			{
				return this._name ?? string.Empty;
			}
			set
			{
				this._name = value;
				this._attributeType = new CodeTypeReference(this._name);
			}
		}

		public CodeAttributeArgumentCollection Arguments
		{
			get
			{
				return this._arguments;
			}
		}

		public CodeTypeReference AttributeType
		{
			get
			{
				return this._attributeType;
			}
		}

		private string _name;

		private readonly CodeAttributeArgumentCollection _arguments = new CodeAttributeArgumentCollection();

		private CodeTypeReference _attributeType;
	}
}
