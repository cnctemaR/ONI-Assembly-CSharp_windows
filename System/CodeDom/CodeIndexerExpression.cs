using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeIndexerExpression : CodeExpression
	{
		public CodeIndexerExpression()
		{
		}

		public CodeIndexerExpression(CodeExpression targetObject, params CodeExpression[] indices)
		{
			this.targetObject = targetObject;
			this.Indices.AddRange(indices);
		}

		public CodeExpressionCollection Indices
		{
			get
			{
				if (this.indices == null)
				{
					this.indices = new CodeExpressionCollection();
				}
				return this.indices;
			}
		}

		public CodeExpression TargetObject
		{
			get
			{
				return this.targetObject;
			}
			set
			{
				this.targetObject = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpression targetObject;

		private CodeExpressionCollection indices;
	}
}
