using System;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal class MethodBinaryExpression : SimpleBinaryExpression
	{
		internal MethodBinaryExpression(ExpressionType nodeType, Expression left, Expression right, Type type, MethodInfo method)
			: base(nodeType, left, right, type)
		{
			this._method = method;
		}

		internal override MethodInfo GetMethod()
		{
			return this._method;
		}

		private readonly MethodInfo _method;
	}
}
