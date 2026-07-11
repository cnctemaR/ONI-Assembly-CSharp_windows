using System;

namespace System.Linq.Expressions
{
	internal class TypedParameterExpression : ParameterExpression
	{
		internal TypedParameterExpression(Type type, string name)
			: base(name)
		{
			this.Type = type;
		}

		public sealed override Type Type { get; }
	}
}
