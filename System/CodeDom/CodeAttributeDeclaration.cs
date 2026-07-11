using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
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
		{
			this.attribute = attributeType;
			if (attributeType != null)
			{
				this.name = attributeType.BaseType;
			}
		}

		public CodeAttributeDeclaration(CodeTypeReference attributeType, params CodeAttributeArgument[] arguments)
		{
			this.attribute = attributeType;
			if (attributeType != null)
			{
				this.name = attributeType.BaseType;
			}
			this.Arguments.AddRange(arguments);
		}

		public CodeAttributeArgumentCollection Arguments
		{
			get
			{
				if (this.arguments == null)
				{
					this.arguments = new CodeAttributeArgumentCollection();
				}
				return this.arguments;
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
				this.attribute = new CodeTypeReference(this.name);
			}
		}

		public CodeTypeReference AttributeType
		{
			get
			{
				return this.attribute;
			}
		}

		private string name;

		private CodeAttributeArgumentCollection arguments;

		private CodeTypeReference attribute;
	}
}
