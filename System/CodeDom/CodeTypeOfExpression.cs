using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTypeOfExpression : CodeExpression
	{
		public CodeTypeOfExpression()
		{
		}

		public CodeTypeOfExpression(CodeTypeReference type)
		{
			this.Type = type;
		}

		public CodeTypeOfExpression(string type)
		{
			this.Type = new CodeTypeReference(type);
		}

		public CodeTypeOfExpression(Type type)
		{
			this.Type = new CodeTypeReference(type);
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
