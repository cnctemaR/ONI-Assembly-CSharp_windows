using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeTypeParameter : CodeObject
	{
		public CodeTypeParameter()
		{
		}

		public CodeTypeParameter(string name)
		{
			this.name = name;
		}

		public CodeTypeReferenceCollection Constraints
		{
			get
			{
				if (this.constraints == null)
				{
					this.constraints = new CodeTypeReferenceCollection();
				}
				return this.constraints;
			}
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
		}

		public bool HasConstructorConstraint
		{
			get
			{
				return this.hasConstructorConstraint;
			}
			set
			{
				this.hasConstructorConstraint = value;
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

		private CodeTypeReferenceCollection constraints;

		private CodeAttributeDeclarationCollection customAttributes;

		private bool hasConstructorConstraint;

		private string name;
	}
}
