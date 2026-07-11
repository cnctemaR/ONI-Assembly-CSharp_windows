using System;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal sealed class TypedDynamicExpression1 : DynamicExpression1
	{
		internal TypedDynamicExpression1(Type retType, Type delegateType, CallSiteBinder binder, Expression arg0)
			: base(delegateType, binder, arg0)
		{
			this.Type = retType;
		}

		public sealed override Type Type { get; }
	}
}
