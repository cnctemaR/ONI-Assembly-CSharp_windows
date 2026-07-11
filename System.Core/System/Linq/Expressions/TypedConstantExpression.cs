using System;

namespace System.Linq.Expressions
{
	internal class TypedConstantExpression : ConstantExpression
	{
		internal TypedConstantExpression(object value, Type type)
			: base(value)
		{
			this.Type = type;
		}

		public sealed override Type Type { get; }
	}
}
