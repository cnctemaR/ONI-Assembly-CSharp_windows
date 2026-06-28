using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeTypeDelegate : CodeTypeDeclaration
	{
		public CodeTypeDelegate()
		{
			base.BaseTypes.Add(new CodeTypeReference("System.Delegate"));
		}

		public CodeTypeDelegate(string name)
			: this()
		{
			base.Name = name;
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

		public CodeTypeReference ReturnType
		{
			get
			{
				if (this.returnType == null)
				{
					this.returnType = new CodeTypeReference(string.Empty);
				}
				return this.returnType;
			}
			set
			{
				this.returnType = value;
			}
		}

		private CodeParameterDeclarationExpressionCollection parameters;

		private CodeTypeReference returnType;
	}
}
