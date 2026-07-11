using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeDefaultValueExpression : CodeExpression
	{
		public CodeDefaultValueExpression()
		{
		}

		public CodeDefaultValueExpression(CodeTypeReference type)
		{
			this._type = type;
		}

		public CodeTypeReference Type
		{
			get
			{
				CodeTypeReference codeTypeReference;
				if ((codeTypeReference = this._type) == null)
				{
					codeTypeReference = (this._type = new CodeTypeReference(""));
				}
				return codeTypeReference;
			}
			set
			{
				this._type = value;
			}
		}

		private CodeTypeReference _type;
	}
}
