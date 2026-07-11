using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTypeReferenceExpression : CodeExpression
	{
		public CodeTypeReferenceExpression()
		{
		}

		public CodeTypeReferenceExpression(CodeTypeReference type)
		{
			this.Type = type;
		}

		public CodeTypeReferenceExpression(string type)
		{
			this.Type = new CodeTypeReference(type);
		}

		public CodeTypeReferenceExpression(Type type)
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
