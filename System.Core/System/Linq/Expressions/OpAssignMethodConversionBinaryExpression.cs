using System;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal sealed class OpAssignMethodConversionBinaryExpression : MethodBinaryExpression
	{
		internal OpAssignMethodConversionBinaryExpression(ExpressionType nodeType, Expression left, Expression right, Type type, MethodInfo method, LambdaExpression conversion)
			: base(nodeType, left, right, type, method)
		{
			this._conversion = conversion;
		}

		internal override LambdaExpression GetConversion()
		{
			return this._conversion;
		}

		private readonly LambdaExpression _conversion;
	}
}
