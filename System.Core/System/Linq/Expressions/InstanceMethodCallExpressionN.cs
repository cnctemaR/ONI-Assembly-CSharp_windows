using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal sealed class InstanceMethodCallExpressionN : InstanceMethodCallExpression, IArgumentProvider
	{
		public InstanceMethodCallExpressionN(MethodInfo method, Expression instance, IReadOnlyList<Expression> args)
			: base(method, instance)
		{
			this._arguments = args;
		}

		public override Expression GetArgument(int index)
		{
			return this._arguments[index];
		}

		public override int ArgumentCount
		{
			get
			{
				return this._arguments.Count;
			}
		}

		internal override bool SameArguments(ICollection<Expression> arguments)
		{
			return ExpressionUtils.SameElements<Expression>(arguments, this._arguments);
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			return ExpressionUtils.ReturnReadOnly<Expression>(ref this._arguments);
		}

		internal override MethodCallExpression Rewrite(Expression instance, IReadOnlyList<Expression> args)
		{
			return Expression.Call(instance, base.Method, args ?? this._arguments);
		}

		private IReadOnlyList<Expression> _arguments;
	}
}
