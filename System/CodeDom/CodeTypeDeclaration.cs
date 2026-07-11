using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeTypeDeclaration : CodeTypeMember
	{
		public CodeTypeDeclaration()
		{
		}

		public CodeTypeDeclaration(string name)
		{
			base.Name = name;
		}

		public event EventHandler PopulateBaseTypes;

		public event EventHandler PopulateMembers;

		public CodeTypeReferenceCollection BaseTypes
		{
			get
			{
				if (this.baseTypes == null)
				{
					this.baseTypes = new CodeTypeReferenceCollection();
					if (this.PopulateBaseTypes != null)
					{
						this.PopulateBaseTypes(this, EventArgs.Empty);
					}
				}
				return this.baseTypes;
			}
		}

		public bool IsClass
		{
			get
			{
				return (this.attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.NotPublic && !this.isEnum && !this.isStruct;
			}
			set
			{
				if (value)
				{
					this.attributes &= ~TypeAttributes.ClassSemanticsMask;
					this.isEnum = false;
					this.isStruct = false;
				}
			}
		}

		public bool IsEnum
		{
			get
			{
				return this.isEnum;
			}
			set
			{
				if (value)
				{
					this.attributes &= ~TypeAttributes.ClassSemanticsMask;
					this.isEnum = true;
					this.isStruct = false;
				}
			}
		}

		public bool IsInterface
		{
			get
			{
				return (this.attributes & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic;
			}
			set
			{
				if (value)
				{
					this.attributes |= TypeAttributes.ClassSemanticsMask;
					this.isEnum = false;
					this.isStruct = false;
				}
			}
		}

		public bool IsStruct
		{
			get
			{
				return this.isStruct;
			}
			set
			{
				if (value)
				{
					this.attributes &= ~TypeAttributes.ClassSemanticsMask;
					this.isEnum = false;
					this.isStruct = true;
				}
			}
		}

		public CodeTypeMemberCollection Members
		{
			get
			{
				if (this.members == null)
				{
					this.members = new CodeTypeMemberCollection();
					if (this.PopulateMembers != null)
					{
						this.PopulateMembers(this, EventArgs.Empty);
					}
				}
				return this.members;
			}
		}

		public TypeAttributes TypeAttributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		public bool IsPartial
		{
			get
			{
				return this.isPartial;
			}
			set
			{
				this.isPartial = value;
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

		private CodeTypeReferenceCollection baseTypes;

		private CodeTypeMemberCollection members;

		private TypeAttributes attributes = TypeAttributes.Public;

		private bool isEnum;

		private bool isStruct;

		private int populated;

		private bool isPartial;

		private CodeTypeParameterCollection typeParameters;
	}
}
