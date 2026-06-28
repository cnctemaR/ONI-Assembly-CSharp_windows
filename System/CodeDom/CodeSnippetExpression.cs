using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeSnippetExpression : CodeExpression
	{
		public CodeSnippetExpression()
		{
		}

		public CodeSnippetExpression(string value)
		{
			this.value = value;
		}

		public string Value
		{
			get
			{
				if (this.value == null)
				{
					return string.Empty;
				}
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private string value;
	}
}
