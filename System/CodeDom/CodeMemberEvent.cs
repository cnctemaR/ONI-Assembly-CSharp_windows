using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeMemberEvent : CodeTypeMember
	{
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

		private CodeTypeReferenceCollection implementationTypes;

		private CodeTypeReference privateImplementationType;

		private CodeTypeReference type;
	}
}
