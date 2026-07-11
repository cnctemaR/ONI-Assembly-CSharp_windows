using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
	public sealed class ElementInit : IArgumentProvider
	{
		internal ElementInit(MethodInfo addMethod, ReadOnlyCollection<Expression> arguments)
		{
			this.AddMethod = addMethod;
			this.Arguments = arguments;
		}

		public MethodInfo AddMethod { get; }

		public ReadOnlyCollection<Expression> Arguments { get; }

		public Expression GetArgument(int index)
		{
			return this.Arguments[index];
		}

		public int ArgumentCount
		{
			get
			{
				return this.Arguments.Count;
			}
		}

		public override string ToString()
		{
			return ExpressionStringBuilder.ElementInitBindingToString(this);
		}

		public ElementInit Update(IEnumerable<Expression> arguments)
		{
			if (arguments == this.Arguments)
			{
				return this;
			}
			return Expression.ElementInit(this.AddMethod, arguments);
		}
	}
}
