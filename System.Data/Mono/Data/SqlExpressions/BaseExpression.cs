using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal abstract class BaseExpression : IExpression
	{
		public abstract object Eval(DataRow row);

		public abstract bool DependsOn(DataColumn other);

		public virtual bool EvalBoolean(DataRow row)
		{
			throw new EvaluateException("Not a Boolean Expression");
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is BaseExpression;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public virtual void ResetExpression()
		{
		}
	}
}
