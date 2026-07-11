using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeArgumentReferenceExpression : CodeExpression
	{
		public CodeArgumentReferenceExpression()
		{
		}

		public CodeArgumentReferenceExpression(string name)
		{
			this.parameterName = name;
		}

		public string ParameterName
		{
			get
			{
				if (this.parameterName == null)
				{
					return string.Empty;
				}
				return this.parameterName;
			}
			set
			{
				this.parameterName = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private string parameterName;
	}
}
