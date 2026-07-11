using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeCastExpression : CodeExpression
	{
		public CodeCastExpression()
		{
		}

		public CodeCastExpression(CodeTypeReference targetType, CodeExpression expression)
		{
			this.TargetType = targetType;
			this.Expression = expression;
		}

		public CodeCastExpression(string targetType, CodeExpression expression)
		{
			this.TargetType = new CodeTypeReference(targetType);
			this.Expression = expression;
		}

		public CodeCastExpression(Type targetType, CodeExpression expression)
		{
			this.TargetType = new CodeTypeReference(targetType);
			this.Expression = expression;
		}

		public CodeTypeReference TargetType
		{
			get
			{
				CodeTypeReference codeTypeReference;
				if ((codeTypeReference = this._targetType) == null)
				{
					codeTypeReference = (this._targetType = new CodeTypeReference(""));
				}
				return codeTypeReference;
			}
			set
			{
				this._targetType = value;
			}
		}

		public CodeExpression Expression { get; set; }

		private CodeTypeReference _targetType;
	}
}
