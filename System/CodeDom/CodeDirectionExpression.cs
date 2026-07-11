using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeDirectionExpression : CodeExpression
	{
		public CodeDirectionExpression()
		{
		}

		public CodeDirectionExpression(FieldDirection direction, CodeExpression expression)
		{
			this.Expression = expression;
			this.Direction = direction;
		}

		public CodeExpression Expression { get; set; }

		public FieldDirection Direction { get; set; }
	}
}
