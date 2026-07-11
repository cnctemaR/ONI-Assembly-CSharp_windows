using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeIndexerExpression : CodeExpression
	{
		public CodeIndexerExpression()
		{
		}

		public CodeIndexerExpression(CodeExpression targetObject, params CodeExpression[] indices)
		{
			this.TargetObject = targetObject;
			this.Indices.AddRange(indices);
		}

		public CodeExpression TargetObject { get; set; }

		public CodeExpressionCollection Indices
		{
			get
			{
				CodeExpressionCollection codeExpressionCollection;
				if ((codeExpressionCollection = this._indices) == null)
				{
					codeExpressionCollection = (this._indices = new CodeExpressionCollection());
				}
				return codeExpressionCollection;
			}
		}

		private CodeExpressionCollection _indices;
	}
}
