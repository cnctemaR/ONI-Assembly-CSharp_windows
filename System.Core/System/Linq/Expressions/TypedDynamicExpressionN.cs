using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal class TypedDynamicExpressionN : DynamicExpressionN
	{
		internal TypedDynamicExpressionN(Type returnType, Type delegateType, CallSiteBinder binder, IReadOnlyList<Expression> arguments)
			: base(delegateType, binder, arguments)
		{
			this.Type = returnType;
		}

		public sealed override Type Type { get; }
	}
}
