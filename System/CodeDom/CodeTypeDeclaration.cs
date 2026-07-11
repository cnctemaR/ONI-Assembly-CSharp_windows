using System;
using System.Reflection;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTypeDeclaration : CodeTypeMember
	{
		public event EventHandler PopulateBaseTypes;

		public event EventHandler PopulateMembers;

		public CodeTypeDeclaration()
		{
		}

		public CodeTypeDeclaration(string name)
		{
			base.Name = name;
		}

		public TypeAttributes TypeAttributes { get; set; } = TypeAttributes.Public;

		public CodeTypeReferenceCollection BaseTypes
		{
			get
			{
				if ((this._populated & 1) == 0)
				{
					this._populated |= 1;
					EventHandler populateBaseTypes = this.PopulateBaseTypes;
					if (populateBaseTypes != null)
					{
						populateBaseTypes(this, EventArgs.Empty);
					}
				}
				return this._baseTypes;
			}
		}

		public bool IsClass
		{
			get
			{
				return (this.TypeAttributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.NotPublic && !this._isEnum && !this._isStruct;
			}
			set
			{
				if (value)
				{
					this.TypeAttributes &= ~TypeAttributes.ClassSemanticsMask;
					this.TypeAttributes |= TypeAttributes.NotPublic;
					this._isStruct = false;
					this._isEnum = false;
				}
			}
		}

		public bool IsStruct
		{
			get
			{
				return this._isStruct;
			}
			set
			{
				if (value)
				{
					this.TypeAttributes &= ~TypeAttributes.ClassSemanticsMask;
					this._isEnum = false;
				}
				this._isStruct = value;
			}
		}

		public bool IsEnum
		{
			get
			{
				return this._isEnum;
			}
			set
			{
				if (value)
				{
					this.TypeAttributes &= ~TypeAttributes.ClassSemanticsMask;
					this._isStruct = false;
				}
				this._isEnum = value;
			}
		}

		public bool IsInterface
		{
			get
			{
				return (this.TypeAttributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask;
			}
			set
			{
				if (value)
				{
					this.TypeAttributes &= ~TypeAttributes.ClassSemanticsMask;
					this.TypeAttributes |= TypeAttributes.ClassSemanticsMask;
					this._isStruct = false;
					this._isEnum = false;
					return;
				}
				this.TypeAttributes &= ~TypeAttributes.ClassSemanticsMask;
			}
		}

		public bool IsPartial { get; set; }

		public CodeTypeMemberCollection Members
		{
			get
			{
				if ((this._populated & 2) == 0)
				{
					this._populated |= 2;
					EventHandler populateMembers = this.PopulateMembers;
					if (populateMembers != null)
					{
						populateMembers(this, EventArgs.Empty);
					}
				}
				return this._members;
			}
		}

		public CodeTypeParameterCollection TypeParameters
		{
			get
			{
				CodeTypeParameterCollection codeTypeParameterCollection;
				if ((codeTypeParameterCollection = this._typeParameters) == null)
				{
					codeTypeParameterCollection = (this._typeParameters = new CodeTypeParameterCollection());
				}
				return codeTypeParameterCollection;
			}
		}

		private readonly CodeTypeReferenceCollection _baseTypes = new CodeTypeReferenceCollection();

		private readonly CodeTypeMemberCollection _members = new CodeTypeMemberCollection();

		private bool _isEnum;

		private bool _isStruct;

		private int _populated;

		private const int BaseTypesCollection = 1;

		private const int MembersCollection = 2;

		private CodeTypeParameterCollection _typeParameters;
	}
}
