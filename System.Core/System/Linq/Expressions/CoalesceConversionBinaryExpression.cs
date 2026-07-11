using System;

namespace System.Linq.Expressions
{
	internal sealed class CoalesceConversionBinaryExpression : BinaryExpression
	{
		internal CoalesceConversionBinaryExpression(Expression left, Expression right, LambdaExpression conversion)
			: base(left, right)
		{
			this._conversion = conversion;
		}

		internal override LambdaExpression GetConversion()
		{
			return this._conversion;
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Coalesce;
			}
		}

		public sealed override Type Type
		{
			get
			{
				return base.Right.Type;
			}
		}

		private readonly LambdaExpression _conversion;
	}
}
