using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.SwitchCaseProxy))]
	public sealed class SwitchCase
	{
		internal SwitchCase(Expression body, ReadOnlyCollection<Expression> testValues)
		{
			this.Body = body;
			this.TestValues = testValues;
		}

		public ReadOnlyCollection<Expression> TestValues { get; }

		public Expression Body { get; }

		public override string ToString()
		{
			return ExpressionStringBuilder.SwitchCaseToString(this);
		}

		public SwitchCase Update(IEnumerable<Expression> testValues, Expression body)
		{
			if (((body == this.Body) & (testValues != null)) && ExpressionUtils.SameElements<Expression>(ref testValues, this.TestValues))
			{
				return this;
			}
			return Expression.SwitchCase(body, testValues);
		}
	}
}
