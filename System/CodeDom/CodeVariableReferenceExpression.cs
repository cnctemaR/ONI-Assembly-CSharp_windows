using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeVariableReferenceExpression : CodeExpression
	{
		public CodeVariableReferenceExpression()
		{
		}

		public CodeVariableReferenceExpression(string variableName)
		{
			this.variableName = variableName;
		}

		public string VariableName
		{
			get
			{
				if (this.variableName == null)
				{
					return string.Empty;
				}
				return this.variableName;
			}
			set
			{
				this.variableName = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private string variableName;
	}
}
