using System;
using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
	[Obsolete("do not use this type", true)]
	public class ExecutionScope
	{
		internal ExecutionScope()
		{
			this.Parent = null;
			this.Globals = null;
			this.Locals = null;
		}

		public object[] CreateHoistedLocals()
		{
			throw new NotSupportedException();
		}

		public Delegate CreateDelegate(int indexLambda, object[] locals)
		{
			throw new NotSupportedException();
		}

		public Expression IsolateExpression(Expression expression, object[] locals)
		{
			throw new NotSupportedException();
		}

		public ExecutionScope Parent;

		public object[] Globals;

		public object[] Locals;
	}
}
