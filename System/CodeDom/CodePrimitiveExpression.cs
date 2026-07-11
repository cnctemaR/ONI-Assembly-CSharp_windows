using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodePrimitiveExpression : CodeExpression
	{
		public CodePrimitiveExpression()
		{
		}

		public CodePrimitiveExpression(object value)
		{
			this.Value = value;
		}

		public object Value { get; set; }
	}
}
