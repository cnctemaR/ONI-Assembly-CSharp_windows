using System;
using System.Reflection;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTypeDelegate : CodeTypeDeclaration
	{
		public CodeTypeDelegate()
		{
			base.TypeAttributes &= ~TypeAttributes.ClassSemanticsMask;
			base.TypeAttributes |= TypeAttributes.NotPublic;
			base.BaseTypes.Clear();
			base.BaseTypes.Add(new CodeTypeReference("System.Delegate"));
		}

		public CodeTypeDelegate(string name)
			: this()
		{
			base.Name = name;
		}

		public CodeTypeReference ReturnType
		{
			get
			{
				CodeTypeReference codeTypeReference;
				if ((codeTypeReference = this._returnType) == null)
				{
					codeTypeReference = (this._returnType = new CodeTypeReference(""));
				}
				return codeTypeReference;
			}
			set
			{
				this._returnType = value;
			}
		}

		public CodeParameterDeclarationExpressionCollection Parameters { get; } = new CodeParameterDeclarationExpressionCollection();

		private CodeTypeReference _returnType;
	}
}
