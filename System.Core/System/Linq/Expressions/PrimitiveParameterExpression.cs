using System;

namespace System.Linq.Expressions
{
	internal sealed class PrimitiveParameterExpression<T> : ParameterExpression
	{
		internal PrimitiveParameterExpression(string name)
			: base(name)
		{
		}

		public sealed override Type Type
		{
			get
			{
				return typeof(T);
			}
		}
	}
}
