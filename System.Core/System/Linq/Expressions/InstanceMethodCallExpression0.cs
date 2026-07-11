using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal sealed class InstanceMethodCallExpression0 : InstanceMethodCallExpression, IArgumentProvider
	{
		public InstanceMethodCallExpression0(MethodInfo method, Expression instance)
			: base(method, instance)
		{
		}

		public override Expression GetArgument(int index)
		{
			throw new ArgumentOutOfRangeException("index");
		}

		public override int ArgumentCount
		{
			get
			{
				return 0;
			}
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			return EmptyReadOnlyCollection<Expression>.Instance;
		}

		internal override bool SameArguments(ICollection<Expression> arguments)
		{
			return arguments == null || arguments.Count == 0;
		}

		internal override MethodCallExpression Rewrite(Expression instance, IReadOnlyList<Expression> args)
		{
			return Expression.Call(instance, base.Method);
		}
	}
}
